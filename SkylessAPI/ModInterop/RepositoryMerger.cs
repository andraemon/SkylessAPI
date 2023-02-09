using Failbetter.Core;
using Il2CppSystem.Linq;
using SkylessAPI.ModInterop.Mergers;
using SkylessAPI.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SkylessAPI.ModInterop
{
    internal static class RepositoryMerger
    {
        public static Il2CppSystem.Collections.Generic.IList<T> MergeOrLoadRepos<T>
            (this Il2CppSystem.Collections.Generic.IList<T> baseRepo, IMerger<T> merger, string slug) where T : Entity, new()
        {
            var path = Path.Combine(SkylessAPI.DataPath, slug + ".bytes");

            if (File.Exists(path) && !AddonAPI.AddonsUpdated)
            {
                try
                {
                    return DeserializeBytes(slug).Cast<Il2CppSystem.Collections.Generic.IList<T>>();
                }
                catch (Exception e)
                {
                    SkylessAPI.Logging.LogWarning($"Merged repository {slug}.bytes is malformed, re-merging addon repositories...");
                    SkylessAPI.Logging.LogDebug(e);
                }
            }

            var repos = baseRepo.MergeRepos(merger, slug);

            //foreach (string guid in AddonAPI.LoadOrder)
            //{
            //    var addon = AddonAPI.Addons[guid];

            //    if (addon.Repos.Contains(jsonSlug))
            //    {
            //        var name = addon.Manifest.Name;

            //        try
            //        {
            //            SkylessAPI.Logging.LogDebug($"Merging {name} {jsonSlug} to base...");
            //            var modRepo = JsonSerializer.Deserialize<List<JsonElement>>
            //                (File.ReadAllText(Path.Combine(addon.Directory, jsonSlug)), AddonAPI.JsonOptions);

            //            baseRepo.MergeModRepoToBase(modRepo, merger, addon.IdOffset);
            //            addon.Loaded = true;
            //        }
            //        catch (Exception e)
            //        {
            //            SkylessAPI.Logging.LogError($"Failed to load {name} when processing {jsonSlug}!");
            //            SkylessAPI.Logging.LogError(e);
            //        }
            //    }
            //}

            if (AddonAPI.AddonsUpdated)
                SerializeBytes(slug, baseRepo.Cast<Il2CppSystem.Collections.Generic.IEnumerable<T>>());

            SkylessAPI.Logging.LogDebug("Merged repos");

            return baseRepo;
        }

        private static Il2CppSystem.Collections.Generic.IList<T> MergeRepos<T>
            (this Il2CppSystem.Collections.Generic.IList<T> baseRepo, IMerger<T> merger, string slug) where T : Entity, new()
        {
            var baseRepoList = baseRepo.Cast<Il2CppSystem.Collections.Generic.List<T>>();

            return baseRepo;
        }

        private static bool TryMergeModToBase<T>(this Il2CppSystem.Collections.Generic.List<T> listTo, 
            List<JsonElement> listFrom, IMerger<T> merger, int offset) where T : Entity, new()
        {
            Dictionary<int, T> dictTo;
            var changes = new Dictionary<int, T>();
            var sourcePlugin = PluginManager.Plugins.Values.First(s => s.Addon?.IdOffset == offset);

            try
            {
                dictTo = listTo.ToDictionary(e => e.Id);
            }
            catch (ArgumentException)
            {
                SkylessAPI.Logging.LogWarning($"Two or more entities in the base repository share the same ID.");
                return false;
            }

            foreach (var itemFrom in listFrom)
            {
                if (!itemFrom.TryGetID(out int id))
                    return false;

                if (id < AddonAPI.ModIdCutoff)
                {
                    if (!dictTo.TryProcessVanillaItem(itemFrom, merger, offset, out var item))
                        return false;

                    if (changes.ContainsKey(id))
                    {
                        SkylessAPI.Logging.LogWarning($"Two or more entities in the addon repository share the ID {id}.");
                        return false;
                    }

                    changes.Add(id, item);
                }
                else
                {
                    var targetGuid = itemFrom.GetPropertyValueOrDefault("TargetMod")?.ToString();

                    if (targetGuid != null)
                    {
                        if (!AddonAPI.IsLoaded(targetGuid))
                        {
                            if (sourcePlugin.Manifest.HardDependencies.Contains(targetGuid))
                            {
                                SkylessAPI.Logging.LogWarning($"Hard dependency {targetGuid} is not loaded.");
                                return false;
                            }
                            else
                            {
                                SkylessAPI.Logging.LogDebug($" | Didn't process item with ID {id}; soft dependency {targetGuid} not loaded");
                                continue;
                            }
                        }
                    }

                    if (itemFrom.TryGetProperty("TargetMod", out JsonElement targetMod2))
                    {
                        var targetModGuid = targetMod2.GetString();

                        if (AddonAPI.IsLoaded(targetModGuid))
                        {
                            var offsetId = id + AddonAPI.IDOffset(targetModGuid);
                            var targetModName = PluginManager.PluginName(targetModGuid);
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

            return true;
        }

        #region Helper Methods
        private static bool TryGetID(this JsonElement element, out int id)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                if (element.TryGetProperty("Id", out var idProperty))
                {
                    if (idProperty.TryGetInt32(out id))
                        return true;
                    else SkylessAPI.Logging.LogWarning($"An entity has an invalid ID {element.GetPropertyValueOrDefault("Id")}.");
                }
                else SkylessAPI.Logging.LogWarning("An entity is missing an ID.");
            }
            else SkylessAPI.Logging.LogWarning("An item is not a valid entity.");

            id = 0;
            return false;
        }

        private static bool TryProcessVanillaItem<T>(this Dictionary<int, T> dictTo, JsonElement itemFrom, IMerger<T> merger, int offset, out T item) where T : Entity, new()
        {
            item = null;
            var id = itemFrom.Id(checkTargetMod: false);

            if (!dictTo.TryGetValue(id, out T itemTo))
            {
                SkylessAPI.Logging.LogWarning($"Could not find vanilla entity with ID {id}.");
                return false;
            }

            var command = itemFrom.GetPropertyValueOrDefault("Command")?.ToString();

            if (command != null)
            {
                var conditional = SatisfiesConditional(itemFrom, id, command, offset);

                if (conditional == false)
                {
                    item = new T() { Id = -1 };
                    return true;
                }
                if (conditional == null) return false;

                if (command.Contains("Remove"))
                {
                    SkylessAPI.Logging.LogDebug($" | Removed vanilla item with ID {id}");
                    return true;
                }
                if (command.Contains("Merge"))
                {
                    var clone2 = merger.Clone(itemTo);

                    if (TryMerge(clone2, itemFrom, merger, offset))
                    {
                        SkylessAPI.Logging.LogDebug($" | Merged modded and vanilla items with ID {id}");
                        item = clone2;
                        return true;
                    }

                    SkylessAPI.Logging.LogWarning($"Error when merging entity with ID {id}.");
                    return false;
                }

                SkylessAPI.Logging.LogWarning($"Unrecognized command {command} on entity with ID {id}.");
                return false;
            }

            var clone = merger.Clone(itemTo);

            if (TryMerge(clone, itemFrom, merger, offset))
            {
                SkylessAPI.Logging.LogDebug($" | Merged modded and vanilla items with ID {id}");
                item = clone;
                return true;
            }

            SkylessAPI.Logging.LogWarning($"Error when merging entity with ID {id}.");
            return false;
        }

        private static bool TryProcessModdedItem<T>(this Dictionary<int, T> itemTo, JsonElement itemFrom, IMerger<T> merger, int offset, out T item) where T : Entity
        {
            item = null;
            return false;
        }

        private static bool TryMerge<T>(T itemTo, JsonElement itemFrom, IMerger<T> merger, int offset) where T : Entity
        {
            try
            {
                merger.Merge(itemTo, itemFrom, offset);

                return true;
            }
            catch (Exception e)
            {
                SkylessAPI.Logging.LogDebug($"Error when merging entities with ID {itemTo.Id}:\n{e}");
                return false;
            }
        }

        private static bool? SatisfiesConditional(JsonElement itemFrom, int id, string command, int offset)
        {
            if (command.Contains("Conditional"))
            {
                var guid = itemFrom.GetPropertyValueOrDefault("TargetMod", string.Empty)?.ToString();

                if (guid == string.Empty)
                {
                    SkylessAPI.Logging.LogWarning($"Conditional command on entity with ID {id} is missing a target mod.");
                    return null;
                }
                if (guid != null)
                {
                    if (PluginManager.Plugins.Values.First(s => s.Addon?.IdOffset == offset).Manifest.Dependencies.Contains(guid))
                    {
                        if (!AddonAPI.IsLoaded(guid))
                        {
                            SkylessAPI.Logging.LogDebug($" | Didn't process item with ID {id}; soft dependency {guid} not loaded");
                            return false;
                        }
                    }
                    else
                    {
                        SkylessAPI.Logging.LogWarning($"Unrecognized dependency {guid} on entity with ID {id}.");
                        return null;
                    }
                }
            }

            return true;
        }

        private static Il2CppSystem.Object DeserializeBytes(string slug)
        {
            var stream = Il2CppSystem.IO.File.OpenRead(Path.Combine(SkylessAPI.DataPath, slug + ".bytes"));
            var reader = new Il2CppSystem.IO.BinaryReader(stream);
            Il2CppSystem.Object repo = null;

            try
            {
                switch (slug)
                {
                    case "areas":
                        repo = BinarySerializer.BinarySerializer_Area.DeserializeCollection(reader);
                        break;
                    case "bargains":
                        repo = BinarySerializer.BinarySerializer_Bargain.DeserializeCollection(reader);
                        break;
                    case "domiciles":
                        repo = BinarySerializer.BinarySerializer_Domicile.DeserializeCollection(reader);
                        break;
                    case "events":
                        repo = BinarySerializer.BinarySerializer_Event.DeserializeCollection(reader);
                        break;
                    case "exchanges":
                        repo = BinarySerializer.BinarySerializer_Exchange.DeserializeCollection(reader);
                        break;
                    case "personas":
                        repo = BinarySerializer.BinarySerializer_Persona.DeserializeCollection(reader);
                        break;
                    case "prospects":
                        repo = BinarySerializer.BinarySerializer_Prospect.DeserializeCollection(reader);
                        break;
                    case "qualities":
                        repo = BinarySerializer.BinarySerializer_Quality.DeserializeCollection(reader);
                        break;
                    case "settings":
                        repo = BinarySerializer.BinarySerializer_Setting.DeserializeCollection(reader);
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
        #endregion

        public static Il2CppSystem.Collections.Generic.List<T> HandleListMerge<T>(Il2CppSystem.Collections.Generic.List<T> listTo, JsonElement listFrom, 
            IMerger<T> merger, int offset) where T : Entity
        {
            if (listTo != null)
            {
                if (listFrom.ValueKind != JsonValueKind.Undefined)
                {
                    var managedList = listTo.ToManagedList();

                    // managedList.TryMergeModToBase(listFrom.GetList(), merger, offset);

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

        private class RepoSerializationException : Exception
        {
            public RepoSerializationException() : base() { }
            public RepoSerializationException(string message) : base(message) { }
            public RepoSerializationException(string message, Exception inner) : base(message, inner) { }
        }
    }
}
