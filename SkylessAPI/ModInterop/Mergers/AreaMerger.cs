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
    internal class AreaMerger : IMerger<Area>
    {
        private static AreaMerger _instance = null;
        public static AreaMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AreaMerger();
                return _instance;
            }
        }

        public void Merge(Area areaTo, JsonElement areaFrom, int offset)
        {
            Quality unlockQuality = areaTo.UnlocksWithQuality;
            if (areaFrom.TryGetProperty("UnlocksWithQuality", out JsonElement qUnlock))
            {
                if (qUnlock.ValueKind == JsonValueKind.Null)
                {
                    unlockQuality = null;
                }
                else
                {
                    unlockQuality = new Quality(qUnlock.Id(offset));
                }
            }

            areaTo.Description = (string)areaFrom.GetPropertyValueOrDefault("Description", areaTo.Description);
            areaTo.ImageName = (string)areaFrom.GetPropertyValueOrDefault("ImageName", areaTo.ImageName);
            areaTo.MarketAccessPermitted = (bool)areaFrom.GetPropertyValueOrDefault("MarketAccessPermitted", areaTo.MarketAccessPermitted);
            areaTo.MoveMessage = (string)areaFrom.GetPropertyValueOrDefault("MoveMessage", areaTo.MoveMessage);
            areaTo.HideName = (bool)areaFrom.GetPropertyValueOrDefault("HideName", areaTo.HideName);
            areaTo.RandomPostcard = (bool)areaFrom.GetPropertyValueOrDefault("RandomPostcard", areaTo.RandomPostcard);
            areaTo.MapX = (int)areaFrom.GetPropertyValueOrDefault("MapX", areaTo.MapX);
            areaTo.MapY = (int)areaFrom.GetPropertyValueOrDefault("MapY", areaTo.MapY);
            areaTo.UnlocksWithQuality = unlockQuality;
            areaTo.ShowOps = (bool)areaFrom.GetPropertyValueOrDefault("ShowOps", areaTo.ShowOps);
            areaTo.PremiumSubRequired = (bool)areaFrom.GetPropertyValueOrDefault("PremiumSubRequired", areaTo.PremiumSubRequired);
            areaTo.Name = (string)areaFrom.GetPropertyValueOrDefault("Name", areaTo.Name);
        }

        public Area FromJsonElement(JsonElement item, int offset)
        {
            Quality unlockQuality = null;
            if (item.TryGetProperty("UnlocksWithQuality", out JsonElement qUnlock) && qUnlock.ValueKind != JsonValueKind.Null)
            {
                unlockQuality = new Quality(qUnlock.Id(offset));
            }

            return new Area()
            {
                Description = (string)item.GetPropertyValueOrDefault("Description"),
                ImageName = (string)item.GetPropertyValueOrDefault("ImageName"),
                MarketAccessPermitted = (bool)item.GetPropertyValueOrDefault("MarketAccessPermitted", false),
                MoveMessage = (string)item.GetPropertyValueOrDefault("MoveMessage"),
                HideName = (bool)item.GetPropertyValueOrDefault("HideName", false),
                RandomPostcard = (bool)item.GetPropertyValueOrDefault("RandomPostcard", false),
                MapX = (int)item.GetPropertyValueOrDefault("MapX", 0),
                MapY = (int)item.GetPropertyValueOrDefault("MapY", 0),
                UnlocksWithQuality = unlockQuality,
                ShowOps = (bool)item.GetPropertyValueOrDefault("ShowOps", false),
                PremiumSubRequired = (bool)item.GetPropertyValueOrDefault("PremiumSubRequired", false),
                Name = (string)item.GetPropertyValueOrDefault("Name"),
                Id = item.Id(offset, false)
            };
        }

        public Area Clone(Area area)
        {
            return new Area()
            {
                Description = area.Description,
                ImageName = area.ImageName,
                World = area.World,
                MarketAccessPermitted = area.MarketAccessPermitted,
                MoveMessage = area.MoveMessage,
                HideName = area.HideName,
                RandomPostcard = area.RandomPostcard,
                MapX = area.MapX,
                MapY = area.MapY,
                UnlocksWithQuality = area.UnlocksWithQuality,
                ShowOps = area.ShowOps,
                PremiumSubRequired = area.PremiumSubRequired,
                Name = area.Name,
                Id = area.Id
            };
        }
    }
}
