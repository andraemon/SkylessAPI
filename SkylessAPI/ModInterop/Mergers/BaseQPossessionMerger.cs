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
    internal class BaseQPossessionMerger : IMerger<BaseQPossession>
    {
        private static BaseQPossessionMerger _instance = null;
        public static BaseQPossessionMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new BaseQPossessionMerger();
                return _instance;
            }
        }

        public void Merge(BaseQPossession baseQTo, JsonElement baseQFrom, int offset)
        {
            if (baseQFrom.TryGetProperty("TargetLevel", out var target) && target.ValueKind == JsonValueKind.Null)
                baseQTo.TargetLevel = null;

            var associatedQuality = baseQTo.AssociatedQuality;
            if (baseQFrom.TryGetProperty("AssociatedQuality", out var qual))
                associatedQuality = new Quality(qual.Id(offset));

            baseQTo.XP = (int)baseQFrom.GetPropertyValueOrDefault("XP", baseQTo.XP);
            baseQTo.EffectiveLevelModifier = (int)baseQFrom.GetPropertyValueOrDefault("EffectiveLevelModifier", baseQTo.EffectiveLevelModifier);
            baseQTo.CompletionMessage = (string)baseQFrom.GetPropertyValueOrDefault("CompletionMessage", baseQTo.CompletionMessage);
            baseQTo.Level = (int)baseQFrom.GetPropertyValueOrDefault("Level", baseQTo.Level);
            baseQTo.AssociatedQuality = associatedQuality;
        }

        public BaseQPossession FromJsonElement(JsonElement item, int offset)
        {
            throw new NotImplementedException();
        }
    }
}
