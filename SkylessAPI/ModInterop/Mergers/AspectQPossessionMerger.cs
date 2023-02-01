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
    internal class AspectQPossessionMerger : IMerger<AspectQPossession>
    {
        private static AspectQPossessionMerger _instance = null;
        public static AspectQPossessionMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AspectQPossessionMerger();
                return _instance;
            }
        }

        public void Merge(AspectQPossession aspectQTo, JsonElement aspectQFrom, int offset)
        {
            BaseQPossessionMerger.Instance.Merge(aspectQTo, aspectQFrom, offset);
        }

        public AspectQPossession FromJsonElement(JsonElement item, int offset)
        {
            return new AspectQPossession()
            {
                XP = (int)item.GetPropertyValueOrDefault("XP", 0),
                EffectiveLevelModifier = (int)item.GetPropertyValueOrDefault("EffectiveLevelModifier", 0),
                CompletionMessage = (string)item.GetPropertyValueOrDefault("CompletionMessage"),
                Level = (int)item.GetPropertyValueOrDefault("Level", 0),
                AssociatedQuality = new Quality(item.GetProperty("AssociatedQuality").Id(offset)),
                Id = item.Id(offset)
            };
        }
    }
}
