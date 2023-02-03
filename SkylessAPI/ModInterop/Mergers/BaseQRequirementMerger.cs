using Failbetter.Core;
using Failbetter.Core.QAssoc.BaseClasses;
using SkylessAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SkylessAPI.ModInterop.Mergers
{
    internal class BaseQRequirementMerger : IMerger<BaseQRequirement>
    {
        private static BaseQRequirementMerger _instance = null;
        public static BaseQRequirementMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new BaseQRequirementMerger();
                return _instance;
            }
        }

        public void Merge(BaseQRequirement baseQTo, JsonElement baseQFrom, int offset)
        {
            if (baseQFrom.TryGetProperty("MinLevel", out var min) && min.ValueKind == JsonValueKind.Null)
                baseQTo.MinLevel = null;

            if (baseQFrom.TryGetProperty("MaxLevel", out var max) && max.ValueKind == JsonValueKind.Null)
                baseQTo.MaxLevel = null;

            var associatedQuality = baseQTo.AssociatedQuality;
            if (baseQFrom.TryGetProperty("AssociatedQuality", out var assoc))
            {
                associatedQuality = new Quality(assoc.Id(offset));
            }

            baseQTo.MinAdvanced = ((string)baseQFrom.GetPropertyValueOrDefault("MinAdvanced", baseQTo.MinAdvanced)).ReplaceModIDsInTokens(offset);
            baseQTo.MaxAdvanced = ((string)baseQFrom.GetPropertyValueOrDefault("MaxAdvanced", baseQTo.MaxAdvanced)).ReplaceModIDsInTokens(offset);
            baseQTo.AssociatedQuality = associatedQuality;
        }

        public BaseQRequirement FromJsonElement(JsonElement item, int offset)
        {
            throw new NotImplementedException();
        }
    }
}
