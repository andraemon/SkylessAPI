using Failbetter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylessAPI.NullableIntermediaries
{
    internal class DomicileNullable : IFailbetterEquivalent<Domicile>
    {
		#region Interface Methods
		public Domicile ToIL2Cpp() =>
			new Domicile()
			{
				Name = Name,
				Description = Description,
				ImageName = ImageName,
				MaxHandSize = MaxHandSize.GetValueOrDefault(),
				DefenceBonus = DefenceBonus.GetValueOrDefault(),
				World = World,
				Id = Id
			};

		IFailbetterEquivalent<Domicile> IFailbetterEquivalent<Domicile>.FromIL2Cpp(Domicile domicile) => FromIL2Cpp(domicile);

		public static DomicileNullable FromIL2Cpp(Domicile domicile) =>
			new DomicileNullable()
			{
				Name = domicile.Name,
				Description = domicile.Description,
				ImageName = domicile.ImageName,
				MaxHandSize = domicile.MaxHandSize,
				DefenceBonus = domicile.DefenceBonus,
				World = domicile.World,
				Id = domicile.Id
			};
		#endregion

		public virtual string Name { get; set; }
		public virtual string Description { get; set; }
		public virtual string ImageName { get; set; }
		public virtual int? MaxHandSize { get; set; }
		public virtual int? DefenceBonus { get; set; }
		public virtual World World { get; set; }
		public int Id { get; set; }
	}
}
