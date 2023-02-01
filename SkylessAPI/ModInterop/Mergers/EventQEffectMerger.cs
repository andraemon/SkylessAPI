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
    internal class EventQEffectMerger : IMerger<EventQEffect>
    {
        private static EventQEffectMerger _instance = null;
        public static EventQEffectMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new EventQEffectMerger();
                return _instance;
            }
        }

        public void Merge(EventQEffect eventQTo, JsonElement eventQFrom, int offset)
        {
            BaseQEffectMerger.Instance.Merge(eventQTo, eventQFrom, offset);

            if (eventQFrom.TryGetProperty("Priority", out var prio) && prio.ValueKind == JsonValueKind.Null)
                eventQTo.Priority = null;
        }

        public EventQEffect FromJsonElement(JsonElement item, int offset)
        {
            Quality targetQuality = null;
            if (item.TryGetProperty("TargetQuality", out JsonElement target) && target.ValueKind != JsonValueKind.Null)
            {
                targetQuality = new Quality(target.Id(offset));
            }

            return new EventQEffect()
            {
                ForceEquip = (bool)item.GetPropertyValueOrDefault("ForceEquip", false),
                OnlyIfNoMoreThanAdvanced = (string)item.GetPropertyValueOrDefault("OnlyIfNoMoreThanAdvanced"),
                SetToExactlyAdvanced = (string)item.GetPropertyValueOrDefault("SetToExactlyAdvanced"),
                ChangeByAdvanced = (string)item.GetPropertyValueOrDefault("ChangeByAdvanced"),
                OnlyIfAtLeastAdvanced = (string)item.GetPropertyValueOrDefault("OnlyIfAtLeastAdvanced"),
                TargetQuality = targetQuality,
                CompletionMessage = (string)item.GetPropertyValueOrDefault("CompletionMessage"),
                Level = (int)item.GetPropertyValueOrDefault("Level", 0),
                AssociatedQuality = new Quality(item.GetProperty("AssociatedQuality").Id(offset)),
                Id = item.Id(offset, false)
            };
        }
    }
}
