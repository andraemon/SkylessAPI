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
    internal class ExchangeMerger : IMerger<Exchange>
    {
        private static ExchangeMerger _instance = null;
        public static ExchangeMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ExchangeMerger();
                return _instance;
            }
        }

        public void Merge(Exchange exchangeTo, JsonElement exchangeFrom, int offset)
        {
            var shops = RepositoryMerger.HandleListMerge(exchangeTo.Shops.ToList(), 
                exchangeFrom.GetPropertyOrDefault("Shops"), ShopMerger.Instance, offset).ToIList();

            exchangeTo.Name = (string)exchangeFrom.GetPropertyValueOrDefault("Name", exchangeTo.Name);
            exchangeTo.Image = (string)exchangeFrom.GetPropertyValueOrDefault("Image", exchangeTo.Image);
            exchangeTo.Title = (string)exchangeFrom.GetPropertyValueOrDefault("Title", exchangeTo.Title);
            exchangeTo.Description = (string)exchangeFrom.GetPropertyValueOrDefault("Description", exchangeTo.Description);
            exchangeTo.Shops = shops;
        }

        public Exchange FromJsonElement(JsonElement item, int offset)
        {
            var shops = RepositoryMerger.HandleListMerge(null, item.GetPropertyOrDefault("Shops"), 
                ShopMerger.Instance, offset).ToIList();

            return new Exchange()
            {
                Name = (string)item.GetPropertyValueOrDefault("Name"),
                Image = (string)item.GetPropertyValueOrDefault("Image"),
                Title = (string)item.GetPropertyValueOrDefault("Title"),
                Description = (string)item.GetPropertyValueOrDefault("Description"),
                Shops = shops,
                SettingIds = new Il2CppSystem.Collections.Generic.List<int>(),
                Id = item.Id(offset, false)
            };
        }

        public Exchange Clone(Exchange exchange)
        {
            return new Exchange()
            {
                Name = exchange.Name,
                Image = exchange.Image,
                Title = exchange.Title,
                Description = exchange.Description,
                Shops = exchange.Shops.ToList().Clone(ShopMerger.Instance.Clone).ToIList(),
                SettingIds = exchange.SettingIds,
                Id = exchange.Id
            };
        }
    }
}
