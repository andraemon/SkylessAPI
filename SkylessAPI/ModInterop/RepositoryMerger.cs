using Failbetter.Core;
using Failbetter.Core.QAssoc.BaseClasses;
using SkylessAPI.NullableIntermediaries;
using SkylessAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylessAPI.ModInterop
{
    internal static class RepositoryMerger
    {
        // eventually this will merge entities instead of just overwriting them like in ssea
        // also resolve id conflicts between addons
        // todo also save the merged repos to disk so we don't have to do it every time the game starts
        // use addon versioning to determine when repos need to be re-merged
        public const int ModIdCutoff = 500000;

        public static List<T> MergeGenericRepositories<T>(this List<T> list1, List<T> list2, Func<T, T, T> merger) where T : IHasId =>
            list1.FullOuterJoin(list2,
                l1 => l1.Id,
                l2 => l2.Id,
                merger).ToList();

        #region Merge Methods
        public static AreaNullable MergeAreas(AreaNullable areaTo, AreaNullable areaFrom)
        {
            if (areaTo == null && areaFrom == null) throw new ArgumentNullException(message: "Both arguments cannot be null", null);
            else if (areaTo == null) return areaFrom;
            else if (areaFrom == null) return areaTo;
            else return new AreaNullable()
            {
                Description = areaFrom.Description ?? areaTo.Description,
                ImageName = areaFrom.ImageName ?? areaTo.ImageName,
                World = null,
                MarketAccessPermitted = areaFrom.MarketAccessPermitted ?? areaTo.MarketAccessPermitted,
                MoveMessage = areaFrom.MoveMessage ?? areaTo.MoveMessage,
                HideName = areaFrom.HideName ?? areaTo.HideName,
                RandomPostcard = areaFrom.RandomPostcard ?? areaTo.RandomPostcard,
                MapX = areaFrom.MapX ?? areaTo.MapX,
                MapY = areaFrom.MapY ?? areaTo.MapY,
                UnlocksWithQuality = areaFrom.UnlocksWithQuality ?? areaTo.UnlocksWithQuality,
                ShowOps = areaFrom.ShowOps ?? areaTo.ShowOps,
                PremiumSubRequired = areaFrom.PremiumSubRequired ?? areaTo.PremiumSubRequired,
                Name = areaFrom.Name ?? areaTo.Name,
                Id = areaTo.Id
            };
        }

        public static BargainNullable MergeBargains(BargainNullable bargainTo, BargainNullable bargainFrom)
        {
            if (bargainTo == null && bargainFrom == null) throw new ArgumentNullException(message: "Both arguments cannot be null", null);
            else if (bargainTo == null) return bargainFrom;
            else if (bargainFrom == null) return bargainTo;
            else return new BargainNullable()
            {
                World = null,
                Offer = bargainFrom.Offer ?? bargainTo.Offer,
                Stock = bargainFrom.Stock ?? bargainTo.Stock,
                Price = bargainFrom.Price ?? bargainTo.Price,
                QualitiesRequired = (bargainFrom.QualitiesRequired == null) ? bargainTo.QualitiesRequired : 
                    bargainFrom.QualitiesRequired.Concat(bargainTo.QualitiesRequired.Where(req => req.Id >= ModIdCutoff)).ToList(),
                Name = bargainFrom.Name ?? bargainTo.Name,
                Id = bargainTo.Id
            };
        }

        public static DomicileNullable MergeDomiciles(DomicileNullable domicileTo, DomicileNullable domicileFrom)
        {
            if (domicileTo == null && domicileTo == null) throw new ArgumentNullException(message: "Both arguments cannot be null", null);
            else if (domicileTo == null) return domicileFrom;
            else if (domicileFrom == null) return domicileTo;
            else return new DomicileNullable()
            {
                Name = domicileFrom.Name ?? domicileTo.Name,
                Description = domicileFrom.Description ?? domicileTo.Description,
                ImageName = domicileFrom.ImageName ?? domicileTo.ImageName,
                MaxHandSize = domicileFrom.MaxHandSize ?? domicileTo.MaxHandSize,
                DefenceBonus = domicileFrom.DefenceBonus ?? domicileTo.DefenceBonus,
                World = domicileFrom.World ?? domicileTo.World,
                Id = domicileTo.Id
            };
        }
        #endregion
    }
}
