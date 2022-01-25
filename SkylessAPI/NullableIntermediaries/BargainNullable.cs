using Failbetter.Core;
using SkylessAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylessAPI.NullableIntermediaries
{
    internal class BargainNullable : IHasId, IFailbetterEquivalent<Bargain>
    {
		#region Interface Methods
		public Bargain ToIL2Cpp() =>
			new Bargain()
			{
				World = World,
				Tags = Tags,
				Description = Description,
				Offer = Offer,
				Stock = Stock ?? default,
				Price = Price,
				QualitiesRequired = QualitiesRequired.ToIl2CppList(),
				Name = Name,
				Id = Id
			};

		IFailbetterEquivalent<Bargain> IFailbetterEquivalent<Bargain>.FromIL2Cpp(Bargain bargain) => FromIL2Cpp(bargain);

		public static BargainNullable FromIL2Cpp(Bargain bargain) =>
			new BargainNullable()
			{
				World = bargain.World,
				Tags = bargain.Tags,
				Description = bargain.Description,
				Offer = bargain.Offer,
				Stock = bargain.Stock,
				Price = bargain.Price,
				QualitiesRequired = bargain.QualitiesRequired.ToManagedList(),
				Name = bargain.Name,
				Id = bargain.Id
			};
		#endregion

		public virtual World World { get; set; }
		public virtual string Tags { get; set; }
		public virtual string Description { get; set; }
		public virtual Quality Offer { get; set; }
		public virtual int? Stock { get; set; }
		public virtual string Price { get; set; }
		public virtual IList<BargainQRequirement> QualitiesRequired { get; set; }
		public string Name { get; set; }
		public int Id { get; set; }
	}
}
