using Failbetter.Core;
using Failbetter.Core.Provider;
using SkylessAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SkylessAPI.ModInterop.Mergers
{
    internal class PersonaMerger : IMerger<Persona>
    {
        private static PersonaMerger _instance = null;
        public static PersonaMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PersonaMerger();
                return _instance;
            }
        }

        public void Merge(Persona personaTo, JsonElement personaFrom, int offset)
        {
            var qualitiesAffected = RepositoryMerger.HandleListMerge(personaTo.QualitiesAffected.ToList(),
                personaFrom.GetPropertyOrDefault("QualitiesAffected"), PersonaQEffectMerger.Instance, offset).ToIList();

            var qualitiesRequired = RepositoryMerger.HandleListMerge(personaTo.QualitiesRequired.ToList(),
                personaFrom.GetPropertyOrDefault("QualitiesRequired"), PersonaQRequirementMerger.Instance, offset).ToIList();

            var setting = personaTo.Setting;
            if (personaFrom.TryGetProperty("Setting", out var set))
            {
                if (set.ValueKind == JsonValueKind.Null)
                    setting = null;
                else
                    setting = new Setting()
                    {
                        Id = set.Id(offset)
                    };
            }

            personaTo.QualitiesAffected = qualitiesAffected;
            personaTo.QualitiesRequired = qualitiesRequired;
            personaTo.Description = (string)personaFrom.GetPropertyValueOrDefault("Description", personaTo.Description);
            personaTo.OwnerName = (string)personaFrom.GetPropertyValueOrDefault("OwnerName", personaTo.OwnerName);
            personaTo.Setting = setting;
            personaTo.Name = (string)personaFrom.GetPropertyValueOrDefault("Name", personaTo.Name);
        }

        public Persona FromJsonElement(JsonElement item, int offset)
        {
            var qualitiesAffected = RepositoryMerger.HandleListMerge(null, 
                item.GetPropertyOrDefault("QualitiesAffected"), PersonaQEffectMerger.Instance, offset).ToIList();

            var qualitiesRequired = RepositoryMerger.HandleListMerge(null,
                item.GetPropertyOrDefault("QualitiesRequired"), PersonaQRequirementMerger.Instance, offset).ToIList();

            Setting setting = null;
            if (item.TryGetProperty("Setting", out var set) && set.ValueKind != JsonValueKind.Null)
                setting = new Setting()
                {
                    Id = set.Id(offset)
                };

            return new Persona()
            {
                QualitiesAffected = qualitiesAffected,
                QualitiesRequired = qualitiesRequired,
                Description = (string)item.GetPropertyValueOrDefault("Description"),
                OwnerName = (string)item.GetPropertyValueOrDefault("OwnerName"),
                Setting = setting,
                DateTimeCreated = Clock.Instance.Now(),
                Name = (string)item.GetPropertyValueOrDefault("Name"),
                Id = item.Id(offset, false)
            };
        }

        public Persona Clone(Persona persona)
        {
            return new Persona()
            {
                QualitiesAffected = persona.QualitiesAffected.ToList().Clone(PersonaQEffectMerger.Instance.Clone).ToIList(),
                QualitiesRequired = persona.QualitiesRequired.ToList().Clone(PersonaQRequirementMerger.Instance.Clone).ToIList(),
                Description = persona.Description,
                OwnerName = persona.OwnerName,
                Setting = persona.Setting,
                DateTimeCreated = persona.DateTimeCreated,
                Name = persona.Name,
                Id = persona.Id
            };
        }
    }
}
