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

        public void Merge(ProspectQRequirement eventQTo, JsonElement eventQFrom, int offset)
        {
            BaseQRequirementMerger.Instance.Merge(eventQTo, eventQFrom, offset);

            eventQTo.CustomLockedMessage = (string)eventQFrom.GetPropertyValueOrDefault("CustomLockedMessage", eventQTo.CustomLockedMessage);
            eventQTo.CustomUnlockedMessage = (string)eventQFrom.GetPropertyValueOrDefault("CustomUnlockedMessage", eventQTo.CustomUnlockedMessage);
        }

        public ProspectQRequirement FromJsonElement(JsonElement item, int offset)
        {
            return new ProspectQRequirement()
            {
                CustomLockedMessage = (string)item.GetPropertyValueOrDefault("CustomLockedMessage"),
                CustomUnlockedMessage = (string)item.GetPropertyValueOrDefault("CustomUnlockedMessage"),
                MinAdvanced = (string)item.GetPropertyValueOrDefault("MinAdvanced"),
                MaxAdvanced = (string)item.GetPropertyValueOrDefault("MaxAdvanced"),
                AssociatedQuality = new Quality(item.GetProperty("AssociatedQuality").Id(offset)),
                Id = item.Id(offset, false)
            };
        }
    }
}
