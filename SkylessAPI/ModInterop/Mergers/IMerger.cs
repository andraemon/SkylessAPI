using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SkylessAPI.ModInterop.Mergers
{
    internal interface IMerger<T>
    {
        void Merge(T to, JsonElement from, int offset);

        T FromJsonElement(JsonElement item, int offset);

        T Clone(T item);
    }
}
