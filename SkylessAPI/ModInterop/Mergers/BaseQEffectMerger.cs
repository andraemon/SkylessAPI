using Failbetter.Core;
using Failbetter.Core.QAssoc.BaseClasses;
using SkylessAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SkylessAPI.ModInterop.Mergers
{
    internal class BaseQEffectMerger : IMerger<BaseQEffect>
    {
        private static BaseQEffectMerger _instance = null;
        public static BaseQEffectMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new BaseQEffectMerger();
                return _instance;
            }
        }

        public void Merge(BaseQEffect baseQTo, JsonElement baseQFrom, int offset)
        {
            if (baseQFrom.TryGetProperty("OnlyIfAtLeast", out var min) && min.ValueKind == JsonValueKind.Null)
                baseQTo.OnlyIfAtLeast = null;

            if (baseQFrom.TryGetProperty("OnlyIfNoMoreThan", out var max) && max.ValueKind == JsonValueKind.Null)
                baseQTo.OnlyIfNoMoreThan = null;

            if (baseQFrom.TryGetProperty("SetToExactly", out var set) && set.ValueKind == JsonValueKind.Null)
                baseQTo.SetToExactly = null;

            if (baseQFrom.TryGetProperty("TargetLevel", out var lev) && lev.ValueKind == JsonValueKind.Null)
                baseQTo.TargetLevel = null;

            var targetQuality = baseQTo.TargetQuality;
            if (baseQFrom.TryGetProperty("TargetQuality", out var target))
            {
                if (target.ValueKind == JsonValueKind.Null)
                {
                    targetQuality = null;
                }
                else
                {
                    targetQuality = new Quality(target.Id(offset));
                }
            }

            var associatedQuality = baseQTo.AssociatedQuality;
            if (baseQFrom.TryGetProperty("AssociatedQuality", out var assoc))
                associatedQuality = new Quality(assoc.Id(offset));

            baseQTo.ForceEquip = (bool)baseQFrom.GetPropertyValueOrDefault("ForceEquip", baseQTo.ForceEquip);
            baseQTo.OnlyIfNoMoreThanAdvanced = ((string)baseQFrom.GetPropertyValueOrDefault("OnlyIfNoMoreThanAdvanced", baseQTo.OnlyIfNoMoreThanAdvanced)).ReplaceModIDsInTokens(offset);
            baseQTo.SetToExactlyAdvanced = ((string)baseQFrom.GetPropertyValueOrDefault("SetToExactlyAdvanced", baseQTo.SetToExactlyAdvanced)).ReplaceModIDsInTokens(offset);
            baseQTo.ChangeByAdvanced = ((string)baseQFrom.GetPropertyValueOrDefault("ChangeByAdvanced", baseQTo.ChangeByAdvanced)).ReplaceModIDsInTokens(offset);
            baseQTo.OnlyIfAtLeastAdvanced = ((string)baseQFrom.GetPropertyValueOrDefault("OnlyIfAtLeastAdvanced", baseQTo.OnlyIfAtLeastAdvanced)).ReplaceModIDsInTokens(offset);
            baseQTo.TargetQuality = targetQuality;
            baseQTo.CompletionMessage = (string)baseQFrom.GetPropertyValueOrDefault("CompletionMessage", baseQTo.CompletionMessage);
            baseQTo.Level = (int)baseQFrom.GetPropertyValueOrDefault("Level", baseQTo.Level);
            baseQTo.AssociatedQuality = associatedQuality;
        }

        public BaseQEffect FromJsonElement(JsonElement item, int offset)
        {
            throw new NotImplementedException();
        }

        public BaseQEffect Clone(BaseQEffect qEffect)
        {
            throw new NotImplementedException();
        }
    }
}
