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
    internal class EventQRequirementMerger : IMerger<EventQRequirement>
    {
        private static EventQRequirementMerger _instance = null;
        public static EventQRequirementMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new EventQRequirementMerger();
                return _instance;
            }
        }

        public void Merge(EventQRequirement eventQTo, JsonElement eventQFrom, int offset)
        {
            BaseQRequirementMerger.Instance.Merge(eventQTo, eventQFrom, offset);
        }

        public EventQRequirement FromJsonElement(JsonElement item, int offset)
        {
            return new EventQRequirement()
            {
                MinAdvanced = ((string)item.GetPropertyValueOrDefault("MinAdvanced")).ReplaceModIDsInTokens(offset),
                MaxAdvanced = ((string)item.GetPropertyValueOrDefault("MaxAdvanced")).ReplaceModIDsInTokens(offset),
                AssociatedQuality = new Quality(item.GetProperty("AssociatedQuality").Id(offset)),
                Id = item.Id(offset, false)
            };
        }
    }
}
