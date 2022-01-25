using Failbetter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylessAPI.NullableIntermediaries
{
    internal class AreaNullable : IHasId, IFailbetterEquivalent<Area>
    {
        #region Interface Methods
        public Area ToIL2Cpp() =>
			new Area()
			{
				Description = Description,
				ImageName = ImageName,
				World = World,
				MarketAccessPermitted = MarketAccessPermitted ?? default,
				MoveMessage = MoveMessage,
				HideName = HideName ?? default,
				RandomPostcard = RandomPostcard ?? default,
				MapX = MapX ?? default,
				MapY = MapY ?? default,
				ShowOps = ShowOps ?? default,
				PremiumSubRequired = PremiumSubRequired ?? default,
				Name = Name,
				Id = Id
			};

		IFailbetterEquivalent<Area> IFailbetterEquivalent<Area>.FromIL2Cpp(Area area) => FromIL2Cpp(area);

		public static AreaNullable FromIL2Cpp(Area area) =>
			new AreaNullable()
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
		#endregion

		public virtual string Description { get; set; }
		public virtual string ImageName { get; set; }
		public virtual World World { get; set; }
		public virtual bool? MarketAccessPermitted { get; set; }
		public virtual string MoveMessage { get; set; }
		public virtual bool? HideName { get; set; }
		public virtual bool? RandomPostcard { get; set; }
		public virtual int? MapX { get; set; }
		public virtual int? MapY { get; set; }
		public virtual Quality UnlocksWithQuality { get; set; }
		public virtual bool? ShowOps { get; set; }
		public virtual bool? PremiumSubRequired { get; set; }
		public string Name { get; set; }
		public int Id { get; set; }
    }
}
