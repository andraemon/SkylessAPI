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
    internal class PersonaNullable : IFailbetterEquivalent<Persona>
    {
        #region Interface Classes
        public Persona ToIL2Cpp() =>
            new Persona()
            {
                QualitiesAffected = null, // QualitiesAffected.ToIl2Cpp(),
                QualitiesRequired = null, // QualitiesRequired.ToIl2Cpp(),
                Description = Description,
                OwnerName = OwnerName,
                DateTimeCreated = default,
                Name = Name,
                Id = Id
            };

        IFailbetterEquivalent<Persona> IFailbetterEquivalent<Persona>.FromIL2Cpp(Persona persona) => FromIL2Cpp(persona);

        public static PersonaNullable FromIL2Cpp(Persona persona) =>
            new PersonaNullable()
            {
                QualitiesAffected = null, // persona.QualitiesAffected.ToManagedList(),
                QualitiesRequired = null, // persona.QualitiesRequired.ToManagedList(),
                Description = persona.Description,
                OwnerName = persona.OwnerName,
                Setting = persona.Setting,
                DateTimeCreated = null,
                Name = persona.Name,
                Id = persona.Id
            };
        #endregion

        public virtual IList<PersonaQEffect> QualitiesAffected { get; set; }
        public virtual IList<PersonaQRequirement> QualitiesRequired { get; set; }
        public virtual string Description { get; set; }
        public virtual string OwnerName { get; set; }
        public virtual Setting Setting { get; set; }
        public virtual DateTime? DateTimeCreated { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
    }
}
