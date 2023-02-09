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
    internal class ProspectQRequirementMerger : IMerger<ProspectQRequirement>
    {
        private static ProspectQRequirementMerger _instance = null;
        public static ProspectQRequirementMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ProspectQRequirementMerger();
                return _instance;
            }
        }

        public void Merge(ProspectQRequirement prospectQTo, JsonElement prospectQFrom, int offset)
        {
            BaseQRequirementMerger.Instance.Merge(prospectQTo, prospectQFrom, offset);

            prospectQTo.CustomLockedMessage = (string)prospectQFrom.GetPropertyValueOrDefault("CustomLockedMessage", prospectQTo.CustomLockedMessage);
            prospectQTo.CustomUnlockedMessage = (string)prospectQFrom.GetPropertyValueOrDefault("CustomUnlockedMessage", prospectQTo.CustomUnlockedMessage);
        }

        public ProspectQRequirement FromJsonElement(JsonElement item, int offset)
        {
            return new ProspectQRequirement()
            {
                CustomLockedMessage = (string)item.GetPropertyValueOrDefault("CustomLockedMessage"),
                CustomUnlockedMessage = (string)item.GetPropertyValueOrDefault("CustomUnlockedMessage"),
                MinAdvanced = ((string)item.GetPropertyValueOrDefault("MinAdvanced")).ReplaceModIDsInTokens(offset),
                MaxAdvanced = ((string)item.GetPropertyValueOrDefault("MaxAdvanced")).ReplaceModIDsInTokens(offset),
                AssociatedQuality = new Quality(item.GetProperty("AssociatedQuality").Id(offset)),
                Id = item.Id(offset, false)
            };
        }

        public ProspectQRequirement Clone(ProspectQRequirement qRequirement)
        {
            return new ProspectQRequirement()
            {
                Prospect = qRequirement.Prospect,
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
