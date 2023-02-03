using Failbetter.Core;
using Failbetter.Core.QAssoc;
using Failbetter.Core.QAssoc.BaseClasses;
using SkylessAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SkylessAPI.ModInterop.Mergers
{
    internal class ShopQRequirementMerger : IMerger<ShopQRequirement>
    {
        private static ShopQRequirementMerger _instance = null;
        public static ShopQRequirementMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ShopQRequirementMerger();
                return _instance;
            }
        }

        public void Merge(ShopQRequirement shopQTo, JsonElement shopQFrom, int offset)
        {
            BaseQRequirementMerger.Instance.Merge(shopQTo, shopQFrom, offset);
        }

        public ShopQRequirement FromJsonElement(JsonElement item, int offset)
        {
            return new ShopQRequirement()
            {
                MinAdvanced = ((string)item.GetPropertyValueOrDefault("MinAdvanced")).ReplaceModIDsInTokens(offset),
                MaxAdvanced = ((string)item.GetPropertyValueOrDefault("MaxAdvanced")).ReplaceModIDsInTokens(offset),
                AssociatedQuality = new Quality(item.GetProperty("AssociatedQuality").Id(offset)),
                Id = item.Id(offset, false)
            };
        }
    }
}
