using Failbetter.Core;
using SkylessAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylessAPI.NullableIntermediaries
{
    internal class ExchangeNullable : IFailbetterEquivalent<Exchange>
    {
        #region Interface Methods
        public Exchange ToIL2Cpp() =>
            new Exchange()
            {
                Name = Name,
                Image = Image,
                Title = Title,
                Description = Description,
                Shops = null, // Shops.ToIl2Cpp(),
                SettingIds = null, //tfw list not ilist
                Id = Id
            };

        IFailbetterEquivalent<Exchange> IFailbetterEquivalent<Exchange>.FromIL2Cpp(Exchange exchange) => FromIL2Cpp(exchange);

        public static ExchangeNullable FromIL2Cpp(Exchange exchange) =>
            new ExchangeNullable()
            {
                Name = exchange.Name,
                Image = exchange.Image,
                Title = exchange.Title,
                Description = exchange.Description,
                Shops = null, // exchange.Shops.ToManagedList(),
                SettingIds = null, //see above
                Id = exchange.Id
            };
        #endregion

        public virtual string Name { get; set; }
        public virtual string Image { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual IList<Shop> Shops { get; set; }
        public virtual List<int> SettingIds { get; set; }
        public int Id { get; set; }
    }
}
