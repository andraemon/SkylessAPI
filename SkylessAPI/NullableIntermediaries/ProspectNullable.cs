using Failbetter.Core;
using Failbetter.Core.QAssoc;
using SkylessAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylessAPI.NullableIntermediaries
{
    internal class ProspectNullable : IFailbetterEquivalent<Prospect>
    {
        #region Interface Methods
        public Prospect ToIL2Cpp() =>
            new Prospect()
            {
                World = World,
                Tags = Tags,
                Description = Description,
                Setting = Setting,
                Request = Request,
                Demand = Demand ?? default,
                Payment = Payment,
                QualitiesAffected = null, // QualitiesAffected.ToIl2Cpp(),
                QualitiesRequired = null, // QualitiesRequired.ToIl2Cpp(),
                Completions = null, // Completions.ToIl2Cpp(),
                Name = Name,
                Id = Id
            };

        IFailbetterEquivalent<Prospect> IFailbetterEquivalent<Prospect>.FromIL2Cpp(Prospect prospect) => FromIL2Cpp(prospect);

        public static ProspectNullable FromIL2Cpp(Prospect prospect) =>
            new ProspectNullable()
            {
                World = prospect.World,
                Tags = prospect.Tags,
                Description = prospect.Description,
                Setting = prospect.Setting,
                Request = prospect.Request,
                Demand = prospect.Demand,
                Payment = prospect.Payment,
                QualitiesAffected = null, // prospect.QualitiesAffected.ToManagedList(),
                QualitiesRequired = null, // prospect.QualitiesRequired.ToManagedList(),
                Completions = null, // prospect.Completions.ToManagedList(),
                Name = prospect.Name,
                Id = prospect.Id
            };
        #endregion

        public virtual World World { get; set; }
        public virtual string Tags { get; set; }
        public virtual string Description { get; set; }
        public virtual Setting Setting { get; set; }
        public virtual Quality Request { get; set; }
        public virtual int? Demand { get; set; }
        public virtual string Payment { get; set; } // should be parsable as an int (why did they do it this way? who knows!)
        public virtual IList<ProspectQEffect> QualitiesAffected { get; set; }
        public virtual IList<ProspectQRequirement> QualitiesRequired { get; set; }
        public virtual IList<Completion> Completions { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
    }
}
