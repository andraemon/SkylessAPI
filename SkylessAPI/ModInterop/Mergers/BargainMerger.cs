using Failbetter.Core;
using Il2CppSystem.Collections.Generic;
using SkylessAPI.Utilities;
using System.Linq;
using System.Text.Json;

namespace SkylessAPI.ModInterop.Mergers
{
    internal class BargainMerger : IMerger<Bargain>
    {
        private static BargainMerger _instance = null;
        public static BargainMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new BargainMerger();
                return _instance;
            }
        }

        public void Merge(Bargain bargainTo, JsonElement bargainFrom, int offset)
        {
            Quality offer = bargainTo.Offer;
            if (bargainFrom.TryGetProperty("Offer", out JsonElement off))
            {
                if (off.ValueKind == JsonValueKind.Null)
                {
                    offer = null;
                }
                else
                {
                    offer = new Quality(off.Id(offset));
                }
            }

            var qualitiesRequired = RepositoryMerger.HandleListMerge(bargainTo.QualitiesRequired.ToList(),
                bargainFrom.GetPropertyOrDefault("QualitiesRequired"), BargainQRequirementMerger.Instance, offset).ToIList();

            bargainTo.Tags = (string)bargainFrom.GetPropertyValueOrDefault("Tags", bargainTo.Tags);
            bargainTo.Description = ((string)bargainFrom.GetPropertyValueOrDefault("Description", bargainTo.Description)).ReplaceModIDsInTokens(offset);
            bargainTo.Offer = offer;
            bargainTo.Stock = (int)bargainFrom.GetPropertyValueOrDefault("Stock", bargainTo.Stock);
            bargainTo.Price = (string)bargainFrom.GetPropertyValueOrDefault("Price", bargainTo.Price);
            bargainTo.QualitiesRequired = qualitiesRequired;
            bargainTo.Name = ((string)bargainFrom.GetPropertyValueOrDefault("Name", bargainTo.Name)).ReplaceModIDsInTokens(offset);
        }

        public Bargain FromJsonElement(JsonElement item, int offset)
        {
            Quality offer = null;
            if (item.TryGetProperty("Offer", out JsonElement off) && off.ValueKind != JsonValueKind.Null)
            {
                offer = new Quality(off.Id(offset));
            }

            var qualitiesRequired = RepositoryMerger.HandleListMerge(null, item.GetPropertyOrDefault("QualitiesRequired"),
                BargainQRequirementMerger.Instance, offset).ToIList();

            return new Bargain()
            {
                Tags = (string)item.GetPropertyValueOrDefault("Tags"),
                Description = ((string)item.GetPropertyValueOrDefault("Description")).ReplaceModIDsInTokens(offset),
                Offer =  offer,
                Stock = (int)item.GetPropertyValueOrDefault("Stock", 0),
                Price = (string)item.GetPropertyValueOrDefault("Price"),
                QualitiesRequired = qualitiesRequired,
                Name = ((string)item.GetPropertyValueOrDefault("Name")).ReplaceModIDsInTokens(offset),
                Id = item.Id(offset, false)
            };
        }

        public Bargain Clone(Bargain bargain)
        {
            return new Bargain()
            {
                World = bargain.World,
                Tags = bargain.Tags,
                Description = bargain.Description,
                Offer = bargain.Offer,
                Stock = bargain.Stock,
                Price = bargain.Price,
                QualitiesRequired = bargain.QualitiesRequired.ToList().Clone(BargainQRequirementMerger.Instance.Clone).ToIList(),
                Teaser = bargain.Teaser,
                Name = bargain.Name,
                Id = bargain.Id
            };
        }
    }
}
