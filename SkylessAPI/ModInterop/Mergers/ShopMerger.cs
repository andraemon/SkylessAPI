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
    internal class ShopMerger : IMerger<Shop>
    {
        private static ShopMerger _instance = null;
        public static ShopMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ShopMerger();
                return _instance;
            }
        }

        public void Merge(Shop shopTo, JsonElement shopFrom, int offset)
        {
            var availabilities = RepositoryMerger.HandleListMerge(shopTo.Availabilities.ToList(), 
                shopFrom.GetPropertyOrDefault("Availabilities"), AvailabilityMerger.Instance, offset).ToIList();

            var qualitiesRequired = RepositoryMerger.HandleListMerge(shopTo.QualitiesRequired.ToList(),
                shopFrom.GetPropertyOrDefault("QualitiesRequired"), ShopQRequirementMerger.Instance, offset).ToIList();

            shopTo.Name = (string)shopFrom.GetPropertyValueOrDefault("Name", shopTo.Name);
            shopTo.Image = (string)shopFrom.GetPropertyValueOrDefault("Image", shopTo.Image);
            shopTo.Description = (string)shopFrom.GetPropertyValueOrDefault("Description", shopTo.Description);
            shopTo.Ordering = (int)shopFrom.GetPropertyValueOrDefault("Ordering", shopTo.Ordering);
            shopTo.Availabilities = availabilities;
            shopTo.QualitiesRequired = qualitiesRequired;
        }

        public Shop FromJsonElement(JsonElement item, int offset)
        {
            var availabilities = RepositoryMerger.HandleListMerge(null, item.GetPropertyOrDefault("Availabilities"), 
                AvailabilityMerger.Instance, offset).ToIList();

            var qualitiesRequired = RepositoryMerger.HandleListMerge(null, item.GetPropertyOrDefault("QualitiesRequired"), 
                ShopQRequirementMerger.Instance, offset).ToIList();

            return new Shop()
            {
                Name = (string)item.GetPropertyValueOrDefault("Name"),
                Image = (string)item.GetPropertyValueOrDefault("Image"),
                Description = (string)item.GetPropertyValueOrDefault("Description"),
                Ordering = (int)item.GetPropertyValueOrDefault("Ordering", 0),
                Availabilities = availabilities,
                QualitiesRequired = qualitiesRequired,
                Id = item.Id(offset, false)
            };
        }

        public Shop Clone(Shop shop)
        {
            return new Shop()
            {
                Name = shop.Name,
                Image = shop.Image,
                Description = shop.Image,
                Ordering = shop.Ordering,
                Exchange = shop.Exchange,
                Availabilities = shop.Availabilities.ToList().Clone(AvailabilityMerger.Instance.Clone).ToIList(),
                QualitiesRequired = shop.QualitiesRequired.ToList().Clone(ShopQRequirementMerger.Instance.Clone).ToIList(),
                Id = shop.Id
            };
        }
    }
}
