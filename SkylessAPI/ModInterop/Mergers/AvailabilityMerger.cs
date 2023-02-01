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
    internal class AvailabilityMerger : IMerger<Availability>
    {
        private static AvailabilityMerger _instance = null;
        public static AvailabilityMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AvailabilityMerger();
                return _instance;
            }
        }

        public void Merge(Availability availTo, JsonElement availFrom, int offset)
        {
            var quality = availTo.Quality;
            if (availFrom.TryGetProperty("Quality", out var qual))
                quality = new Quality(qual.Id(offset));

            var purchaseQuality = availTo.PurchaseQuality;
            if (availFrom.TryGetProperty("PurchaseQuality", out var purchaseQual))
                purchaseQuality = new Quality(purchaseQual.Id(offset));

            availTo.Quality = quality;
            availTo.Cost = (int)availFrom.GetPropertyValueOrDefault("Cost", availTo.Cost);
            availTo.SellPrice = (int)availFrom.GetPropertyValueOrDefault("SellPrice", availTo.SellPrice);
            availTo.PurchaseQuality = purchaseQuality;
            availTo.BuyMessage = (string)availFrom.GetPropertyValueOrDefault("BuyMessage", availTo.BuyMessage);
            availTo.SellMessage = (string)availFrom.GetPropertyValueOrDefault("SellMessage", availTo.SellMessage);
            availTo.SaleDescription = (string)availFrom.GetPropertyValueOrDefault("SaleDescription", availTo.SaleDescription);
        }

        public Availability FromJsonElement(JsonElement item, int offset)
        {
            return new Availability()
            {
                Quality = new Quality(item.GetProperty("Quality").Id(offset)),
                Cost = (int)item.GetPropertyValueOrDefault("Cost", 0),
                SellPrice = (int)item.GetPropertyValueOrDefault("SellPrice", 0),
                PurchaseQuality = new Quality(item.GetProperty("PurchaseQuality").Id(offset)),
                BuyMessage = (string)item.GetPropertyValueOrDefault("BuyMessage"),
                SellMessage = (string)item.GetPropertyValueOrDefault("SellMessage"),
                SaleDescription = (string)item.GetPropertyValueOrDefault("SaleDescription"),
                Id = item.Id(offset, false)
            };
        }
    }
}
