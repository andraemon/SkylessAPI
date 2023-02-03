using Failbetter.Core;
using Failbetter.Core.QAssoc;
using SkylessAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SkylessAPI.ModInterop.Mergers
{
    internal class PersonaQRequirementMerger : IMerger<PersonaQRequirement>
    {
        private static PersonaQRequirementMerger _instance = null;
        public static PersonaQRequirementMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PersonaQRequirementMerger();
                return _instance;
            }
        }

        public void Merge(PersonaQRequirement personaQTo, JsonElement personaQFrom, int offset)
        {
            BaseQRequirementMerger.Instance.Merge(personaQTo, personaQFrom, offset);
        }

        public PersonaQRequirement FromJsonElement(JsonElement item, int offset)
        {
            return new PersonaQRequirement()
            {
                MinAdvanced = ((string)item.GetPropertyValueOrDefault("MinAdvanced")).ReplaceModIDsInTokens(offset),
                MaxAdvanced = ((string)item.GetPropertyValueOrDefault("MaxAdvanced")).ReplaceModIDsInTokens(offset),
                AssociatedQuality = new Quality(item.GetProperty("AssociatedQuality").Id(offset)),
                Id = item.Id(offset, false)
            };
        }
    }
}
