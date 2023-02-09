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
    internal class QEnhancementMerger : IMerger<QEnhancement>
    {
        private static QEnhancementMerger _instance = null;
        public static QEnhancementMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new QEnhancementMerger();
                return _instance;
            }
        }

        public void Merge(QEnhancement qEnhanceTo, JsonElement qEnhanceFrom, int offset)
        {
            var associatedQuality = qEnhanceTo.AssociatedQuality;
            if (qEnhanceFrom.TryGetProperty("AssociatedQuality", out var qual))
                associatedQuality = new Quality(qual.Id(offset));

            qEnhanceTo.Level = (int)qEnhanceFrom.GetPropertyValueOrDefault("Level", qEnhanceTo.Level);
            qEnhanceTo.AssociatedQuality = associatedQuality;
        }

        public QEnhancement FromJsonElement(JsonElement item, int offset)
        {
            return new QEnhancement()
            {
                Level = (int)item.GetPropertyValueOrDefault("Level", 0),
                AssociatedQuality = new Quality(item.GetProperty("AssociatedQuality").Id(offset)),
                Id = item.Id(offset, false)
            };
        }

        public QEnhancement Clone(QEnhancement qEnhancement)
        {
            return new QEnhancement()
            {
                Level = qEnhancement.Level,
                AssociatedQuality = qEnhancement.AssociatedQuality,
                Id = qEnhancement.Id
            };
        }
    }
}
