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
    internal class ProspectQEffectMerger : IMerger<ProspectQEffect>
    {
        private static ProspectQEffectMerger _instance = null;
        public static ProspectQEffectMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ProspectQEffectMerger();
                return _instance;
            }
        }

        public void Merge(ProspectQEffect prospectQTo, JsonElement prospectQFrom, int offset)
        {
            BaseQEffectMerger.Instance.Merge(prospectQTo, prospectQFrom, offset);
        }

        public ProspectQEffect FromJsonElement(JsonElement item, int offset)
        {
            Quality targetQuality = null;
            if (item.TryGetProperty("TargetQuality", out JsonElement target) && target.ValueKind != JsonValueKind.Null)
            {
                targetQuality = new Quality(target.Id(offset));
            }

            return new ProspectQEffect()
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

        public ProspectQEffect Clone(ProspectQEffect qEffect)
        {
            return new ProspectQEffect()
            {
                ForceEquip = qEffect.ForceEquip,
                OnlyIfNoMoreThanAdvanced = qEffect.OnlyIfNoMoreThanAdvanced,
                OnlyIfAtLeast = qEffect.OnlyIfAtLeast,
                OnlyIfNoMoreThan = qEffect.OnlyIfNoMoreThan,
                SetToExactlyAdvanced = qEffect.SetToExactlyAdvanced,
                ChangeByAdvanced = qEffect.ChangeByAdvanced,
                OnlyIfAtLeastAdvanced = qEffect.OnlyIfAtLeastAdvanced,
                SetToExactly = qEffect.SetToExactly,
                TargetQuality = qEffect.TargetQuality,
                TargetLevel = qEffect.TargetLevel,
                CompletionMessage = qEffect.CompletionMessage,
                Level = qEffect.Level,
                AssociatedQuality = qEffect.AssociatedQuality,
                Id = qEffect.Id
            };
        }
    }
}
