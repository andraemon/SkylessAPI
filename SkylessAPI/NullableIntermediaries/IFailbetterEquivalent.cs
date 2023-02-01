using Failbetter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylessAPI.NullableIntermediaries
{
    internal interface IFailbetterEquivalent<TIl2Cpp> where TIl2Cpp : Entity
    {
        TIl2Cpp ToIL2Cpp();
        
        IFailbetterEquivalent<TIl2Cpp> FromIL2Cpp(TIl2Cpp entity);

        int Id { get; set; }
    }
}
