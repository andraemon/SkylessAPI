using Failbetter.Core;
using SkylessAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SkylessAPI.ModInterop.Mergers
{
    internal class BargainQRequirementMerger : IMerger<BargainQRequirement>
    {
        private static BargainQRequirementMerger _instance = null;
        public static BargainQRequirementMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new BargainQRequirementMerger();
                return _instance;
            }
        }

        public void Merge(BargainQRequirement bargainQTo, JsonElement bargainQFrom, int offset)
        {
            BaseQRequirementMerger.Instance.Merge(bargainQTo, bargainQFrom, offset);

            bargainQTo.CustomLockedMessage = (string)bargainQFrom.GetPropertyValueOrDefault("CustomLockedMessage", bargainQTo.CustomLockedMessage);
            bargainQTo.CustomUnlockedMessage = (string)bargainQFrom.GetPropertyValueOrDefault("CustomUnlockedMessage", bargainQTo.CustomUnlockedMessage);
        }

        public BargainQRequirement FromJsonElement(JsonElement item, int offset)
        {
            return new BargainQRequirement()
            {
                CustomLockedMessage = (string)item.GetPropertyValueOrDefault("CustomLockedMessage"),
                CustomUnlockedMessage = (string)item.GetPropertyValueOrDefault("CustomUnlockedMessage"),
                MinAdvanced = ((string)item.GetPropertyValueOrDefault("MinAdvanced")).ReplaceModIDsInTokens(offset),
                MaxAdvanced = ((string)item.GetPropertyValueOrDefault("MaxAdvanced")).ReplaceModIDsInTokens(offset),
                AssociatedQuality = new Quality(item.GetProperty("AssociatedQuality").Id(offset)),
                Id = item.Id(offset, false)
            };
        }

        public BargainQRequirement Clone(BargainQRequirement qRequirement)
        {
            return new BargainQRequirement()
            {
                CustomLockedMessage = qRequirement.CustomLockedMessage,
                CustomUnlockedMessage = qRequirement.CustomUnlockedMessage,
                MinLevel = qRequirement.MinLevel,
                MaxLevel = qRequirement.MaxLevel,
                MinAdvanced = qRequirement.MinAdvanced,
                MaxAdvanced = qRequirement.MaxAdvanced,
                AssociatedQuality = qRequirement.AssociatedQuality,
                Id = qRequirement.Id
            };
        }
    }
}
