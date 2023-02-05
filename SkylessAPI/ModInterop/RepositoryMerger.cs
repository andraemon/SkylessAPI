using Failbetter.Core;
using Failbetter.Core.DataInterfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SkylessAPI.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SkylessAPI.ModInterop
{
    internal static class RepositoryMerger
    {
        public static List<T> MergeOrLoadRepos<T>(this List<T> baseRepo, Mergers.IMerger<T> merger, string slug) where T : Entity
        {
            var jsonSlug = slug + ".json";
            var path = Path.Combine(SkylessAPI.DataPath, slug + ".bytes");

            if (File.Exists(path) && !AddonAPI.AddonsUpdated)
            {
                try
                {
                    var preMergedRepos = (List<T>)DeserializeBytes(slug);

                    return preMergedRepos;
                }
                catch (Exception e)
                {
                    SkylessAPI.Logging.LogWarning($"SkylessAPI merged repo {jsonSlug} is malformed, attempting to re-merge mods...");
                    SkylessAPI.Logging.LogDebug(e);
                }
            }

            foreach (string guid in AddonAPI.LoadOrder)
            {
                var addon = AddonAPI.Addons[guid];

                if (addon.Repos.Contains(jsonSlug))
                {
                    var name = addon.Manifest.Name;

                    try
                    {
                        SkylessAPI.Logging.LogDebug($"Merging {name} {jsonSlug} to base...");
                        var modRepo = JsonSerializer.Deserialize<List<JsonElement>>
                            (File.ReadAllText(Path.Combine(addon.Directory, jsonSlug)), AddonAPI.JsonOptions);

                        baseRepo.MergeModRepoToBase(modRepo, merger, addon.IdOffset);
                        addon.Loaded = true;
                    }
                    catch (Exception e)
                    {
                        SkylessAPI.Logging.LogError($"Failed to load {name} when processing {jsonSlug}!");
                        SkylessAPI.Logging.LogError(e);
                    }
                }
            }

            SerializeBytes(slug, baseRepo.ToIl2CppList().Cast<Il2CppSystem.Collections.Generic.IEnumerable<T>>());

            return baseRepo;
        }

        public static void MergeModRepoToBase<T>(this List<T> listTo, List<JsonElement> listFrom, Mergers.IMerger<T> merger, int offset) where T : Entity
        {
            var dictTo = listTo.ToDictionary(s => s.Id);

            foreach (var itemFrom in listFrom)
            {
                var id = itemFrom.GetProperty("Id").GetInt32();
                if (id < AddonAPI.ModIdCutoff)
                {
                    var itemTo = dictTo[id];
                    if (itemFrom.TryGetProperty("Command", out JsonElement command))
                    {
                        var cmdStr = command.GetString();
                        if (cmdStr == "Remove")
                        {
                            listTo.Remove(itemTo);
                            SkylessAPI.Logging.LogDebug($" | Removed vanilla item with ID {itemTo}");
                        }
                        else if (cmdStr == "Merge")
                        {
                            merger.Merge(itemTo, itemFrom, offset);
                            SkylessAPI.Logging.LogDebug($" | Merged modded and vanilla items with ID {id}");
                        }
                    }
                    else
                    {
                        merger.Merge(itemTo, itemFrom, offset);
                        SkylessAPI.Logging.LogDebug($" | Merged modded and vanilla items with ID {id}");
                    }
                }
                else
                {
                    if (itemFrom.TryGetProperty("TargetMod", out JsonElement targetMod))
                    {
                        var targetModGuid = targetMod.GetString();

                        if (AddonAPI.IsLoaded(targetModGuid))
                        {
                            var offsetId = id + AddonAPI.IDOffset(targetModGuid);
                            var targetModName = AddonAPI.GetName(targetModGuid);
                            var itemTo = dictTo[offsetId];

                            if (itemFrom.TryGetProperty("Command", out JsonElement command))
                            {
                                var cmdStr = command.GetString();
                                if (cmdStr == "Remove" && itemTo != null)
                                {
                                    listTo.Remove(itemTo);
                                    SkylessAPI.Logging.LogDebug($" | Removed item with ID {id} (offset ID {offsetId}) from mod {targetModName}");
                                }
                                else if (cmdStr == "Merge")
                                {
                                    merger.Merge(itemTo, itemFrom, offset);
                                    SkylessAPI.Logging.LogDebug($" | Merged modded item with ID {id} (offset ID {offsetId}) with corresponding item from mod {targetModName}");
                                }
                            }
                            else
                            {
                                merger.Merge(itemTo, itemFrom, offset);
                                SkylessAPI.Logging.LogDebug($" | Merged modded item with ID {id} (offset ID {offsetId}) with corresponding item from mod {targetModName}");
                            }
                        }
                    }
                    else
                    {
                        listTo.Add(merger.FromJsonElement(itemFrom, offset));
                        SkylessAPI.Logging.LogDebug($" | Added modded item with ID {id} (offset ID {id + offset})");
                    }
                }
            }
        }

        public static Il2CppSystem.Collections.Generic.List<T> HandleListMerge<T>(Il2CppSystem.Collections.Generic.List<T> listTo, JsonElement listFrom, 
            Mergers.IMerger<T> merger, int offset) where T : Entity
        {
            if (listTo != null)
            {
                if (listFrom.ValueKind != JsonValueKind.Undefined)
                {
                    var managedList = listTo.ToManagedList();

                    managedList.MergeModRepoToBase(listFrom.GetList(), merger, offset);

                    return managedList.ToIl2CppList();
                }

                return listTo;
            }
            if (listFrom.ValueKind != JsonValueKind.Undefined)
            {
                var listFromElements = listFrom.GetList(); 
                var newList = new Il2CppSystem.Collections.Generic.List<T>();
                for (int i = 0; i < listFromElements.Count; i++)
                {
                    newList.Add(merger.FromJsonElement(listFromElements[i], offset));
                }
                return newList;
            }

            return new Il2CppSystem.Collections.Generic.List<T>();
        }

        private static object DeserializeBytes(string slug)
        {
            var stream = Il2CppSystem.IO.File.OpenRead(Path.Combine(SkylessAPI.DataPath, slug + ".bytes"));
            var reader = new Il2CppSystem.IO.BinaryReader(stream);
            object repo = null;

            try
            {
                switch (slug)
                {
                    case "areas":
                        repo = BinarySerializer.BinarySerializer_Area.DeserializeCollection(reader).ToManagedList();
                        break;
                    case "bargains":
                        repo = BinarySerializer.BinarySerializer_Bargain.DeserializeCollection(reader).ToManagedList();
                        break;
                    case "domiciles":
                        repo = BinarySerializer.BinarySerializer_Domicile.DeserializeCollection(reader).ToManagedList();
                        break;
                    case "events":
                        repo = BinarySerializer.BinarySerializer_Event.DeserializeCollection(reader).ToManagedList();
                        break;
                    case "exchanges":
                        repo = BinarySerializer.BinarySerializer_Exchange.DeserializeCollection(reader).ToManagedList();
                        break;
                    case "personas":
                        repo = BinarySerializer.BinarySerializer_Persona.DeserializeCollection(reader).ToManagedList();
                        break;
                    case "prospects":
                        repo = BinarySerializer.BinarySerializer_Prospect.DeserializeCollection(reader).ToManagedList();
                        break;
                    case "qualities":
                        repo = BinarySerializer.BinarySerializer_Quality.DeserializeCollection(reader).ToManagedList();
                        break;
                    case "settings":
                        repo = BinarySerializer.BinarySerializer_Setting.DeserializeCollection(reader).ToManagedList();
                        break;
                }
            }
            finally
            {
                stream?.Dispose();
                reader?.Dispose();
            }

            if (repo == null) throw new RepoSerializationException($"Failed to deserialize {slug}.bytes");

            return repo;
        }

        private static void SerializeBytes<T>(string slug, Il2CppSystem.Collections.Generic.IEnumerable<T> repo)
        {
            var stream = Il2CppSystem.IO.File.Open(Path.Combine(SkylessAPI.DataPath, slug + ".bytes"), Il2CppSystem.IO.FileMode.Create);
            var writer = new Il2CppSystem.IO.BinaryWriter(stream);

            try
            {
                switch (slug)
                {
                    case "areas":
                        BinarySerializer.BinarySerializer_Area.SerializeCollection(writer, repo.Cast<Il2CppSystem.Collections.Generic.IEnumerable<Area>>());
                        break;
                    case "bargains":
                        BinarySerializer.BinarySerializer_Bargain.SerializeCollection(writer, repo.Cast<Il2CppSystem.Collections.Generic.IEnumerable<Bargain>>());
                        break;
                    case "domiciles":
                        BinarySerializer.BinarySerializer_Domicile.SerializeCollection(writer, repo.Cast<Il2CppSystem.Collections.Generic.IEnumerable<Domicile>>());
                        break;
                    case "events":
                        BinarySerializer.BinarySerializer_Event.SerializeCollection(writer, repo.Cast<Il2CppSystem.Collections.Generic.IEnumerable<Event>>());
                        break;
                    case "exchanges":
                        BinarySerializer.BinarySerializer_Exchange.SerializeCollection(writer, repo.Cast<Il2CppSystem.Collections.Generic.IEnumerable<Exchange>>());
                        break;
                    case "personas":
                        BinarySerializer.BinarySerializer_Persona.SerializeCollection(writer, repo.Cast<Il2CppSystem.Collections.Generic.IEnumerable<Persona>>());
                        break;
                    case "prospects":
                        BinarySerializer.BinarySerializer_Prospect.SerializeCollection(writer, repo.Cast<Il2CppSystem.Collections.Generic.IEnumerable<Prospect>>());
                        break;
                    case "qualities":
                        BinarySerializer.BinarySerializer_Quality.SerializeCollection(writer, repo.Cast<Il2CppSystem.Collections.Generic.IEnumerable<Quality>>());
                        break;
                    case "settings":
                        BinarySerializer.BinarySerializer_Setting.SerializeCollection(writer, repo.Cast<Il2CppSystem.Collections.Generic.IEnumerable<Setting>>());
                        break;
                }
            }
            finally
            {
                stream?.Dispose();
                writer?.Dispose();
            }
        }

        private class RepoSerializationException : Exception
        {
            public RepoSerializationException() : base() { }
            public RepoSerializationException(string message) : base(message) { }
            public RepoSerializationException(string message, Exception inner) : base(message, inner) { }
        }
    }
}
