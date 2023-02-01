using Failbetter.Core;
using Failbetter.Interfaces.Enums;
using SkylessAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SkylessAPI.ModInterop.Mergers
{
    internal class DeckMerger : IMerger<Deck>
    {
        private static DeckMerger _instance = null;
        public static DeckMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DeckMerger();
                return _instance;
            }
        }

        public void Merge(Deck eventQEffTo, JsonElement eventQEffFrom, int offset)
        {
            throw new NotImplementedException();
        }

        public Deck FromJsonElement(JsonElement item, int offset)
        {
            if (item.ValueKind == JsonValueKind.Undefined)
            {
                return new Deck()
                {
                    Name = "Always",
                    ImageName = "100x130",
                    Ordering = 1,
                    Description = "Always",
                    Availability = Frequency.Always,
                    DrawSize = 0,
                    MaxCards = 3,
                    Id = 15083
                };
            }

            return new Deck()
            {
                Name = (string)item.GetPropertyValueOrDefault("Name"),
                ImageName = (string)item.GetPropertyValueOrDefault("ImageName"),
                Ordering = (int)item.GetPropertyValueOrDefault("Ordering", 0),
                Description = (string)item.GetPropertyValueOrDefault("Description"),
                Availability = (Frequency)item.GetPropertyValueOrDefault("Availability", 0),
                DrawSize = (int)item.GetPropertyValueOrDefault("DrawSize", 0),
                MaxCards = (int)item.GetPropertyValueOrDefault("MaxCards", 0),
                Id = item.Id(offset, false)
            };
        }
    }
}
