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
    }
}
