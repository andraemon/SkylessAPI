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
    internal class PersonaQEffectMerger : IMerger<PersonaQEffect>
    {
        private static PersonaQEffectMerger _instance = null;
        public static PersonaQEffectMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PersonaQEffectMerger();
                return _instance;
            }
        }

        public void Merge(PersonaQEffect personaQTo, JsonElement personaQFrom, int offset)
        {
            BaseQEffectMerger.Instance.Merge(personaQTo, personaQFrom, offset);
        }

        public PersonaQEffect FromJsonElement(JsonElement item, int offset)
        {
            Quality targetQuality = null;
            if (item.TryGetProperty("TargetQuality", out JsonElement target) && target.ValueKind != JsonValueKind.Null)
            {
                targetQuality = new Quality(target.Id(offset));
            }

            return new PersonaQEffect()
            {
                ForceEquip = (bool)item.GetPropertyValueOrDefault("ForceEquip", false),
                OnlyIfNoMoreThanAdvanced = ((string)item.GetPropertyValueOrDefault("OnlyIfNoMoreThanAdvanced")).ReplaceModIDsInTokens(offset),
                SetToExactlyAdvanced = ((string)item.GetPropertyValueOrDefault("SetToExactlyAdvanced")).ReplaceModIDsInTokens(offset),
                ChangeByAdvanced = ((string)item.GetPropertyValueOrDefault("ChangeByAdvanced")).ReplaceModIDsInTokens(offset),
                OnlyIfAtLeastAdvanced = ((string)item.GetPropertyValueOrDefault("OnlyIfAtLeastAdvanced")).ReplaceModIDsInTokens(offset),
                TargetQuality = targetQuality,
                CompletionMessage = (string)item.GetPropertyValueOrDefault("CompletionMessage"),
                Level = (int)item.GetPropertyValueOrDefault("Level", 0),
                AssociatedQuality = new Quality(item.GetProperty("AssociatedQuality").Id(offset)),
                Id = item.Id(offset, false)
            };
        }
    }
}
