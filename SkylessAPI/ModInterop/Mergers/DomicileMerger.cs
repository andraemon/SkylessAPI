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
    internal class DomicileMerger : IMerger<Domicile>
    {
        private static DomicileMerger _instance = null;
        public static DomicileMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DomicileMerger();
                return _instance;
            }
        }

        public void Merge(Domicile domicileTo, JsonElement domicileFrom, int offset)
        {
            domicileTo.Name = (string)domicileFrom.GetPropertyValueOrDefault("Name", domicileTo.Name);
            domicileTo.Description = (string)domicileFrom.GetPropertyValueOrDefault("Description", domicileTo.Description);
            domicileTo.ImageName = (string)domicileFrom.GetPropertyValueOrDefault("ImageName", domicileTo.ImageName);
            domicileTo.MaxHandSize = (int)domicileFrom.GetPropertyValueOrDefault("MaxHandSize", domicileTo.MaxHandSize);
            domicileTo.DefenceBonus = (int)domicileFrom.GetPropertyValueOrDefault("DefenceBonus", domicileTo.DefenceBonus);
        }

        public Domicile FromJsonElement(JsonElement item, int offset)
        {
            return new Domicile()
            {
                Name = (string)item.GetPropertyValueOrDefault("Name"),
                Description = (string)item.GetPropertyValueOrDefault("Description"),
                ImageName = (string)item.GetPropertyValueOrDefault("ImageName"),
                MaxHandSize = (int)item.GetPropertyValueOrDefault("MaxHandSize", 0),
                DefenceBonus = (int)item.GetPropertyValueOrDefault("DefenceBonus", 0),
                Id = item.Id(offset, false)
            };
        }

        public Domicile Clone(Domicile domicile)
        {
            return new Domicile()
            {
                Name = domicile.Name,
                Description = domicile.Description,
                ImageName = domicile.ImageName,
                MaxHandSize = domicile.MaxHandSize,
                DefenceBonus = domicile.DefenceBonus,
                World = domicile.World,
                Id = domicile.Id
            };
        }
    }
}
