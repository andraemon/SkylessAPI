using Failbetter.Core;
using Failbetter.Core.QAssoc;
using SkylessAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SkylessAPI.ModInterop.Mergers
{
    internal class CompletionQRequirementMerger : IMerger<CompletionQRequirement>
    {
        private static CompletionQRequirementMerger _instance = null;
        public static CompletionQRequirementMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CompletionQRequirementMerger();
                return _instance;
            }
        }

        public void Merge(CompletionQRequirement comQTo, JsonElement comQFrom, int offset)
        {
            BaseQRequirementMerger.Instance.Merge(comQTo, comQFrom, offset);
        }

        public CompletionQRequirement FromJsonElement(JsonElement item, int offset)
        {
            return new CompletionQRequirement()
            {
                MinAdvanced = ((string)item.GetPropertyValueOrDefault("MinAdvanced")).ReplaceModIDsInTokens(offset),
                MaxAdvanced = ((string)item.GetPropertyValueOrDefault("MaxAdvanced")).ReplaceModIDsInTokens(offset),
                AssociatedQuality = new Quality(item.GetProperty("AssociatedQuality").Id(offset)),
                Id = item.Id(offset, false)
            };
        }

        public CompletionQRequirement Clone(CompletionQRequirement qRequirement)
        {
            return new CompletionQRequirement()
            {
                MinLevel = qRequirement.MinLevel,
                MaxLevel = qRequirement.MaxLevel,
                MinAdvanced = qRequirement.MinAdvanced,
                MaxAdvanced = qRequirement.MaxAdvanced,
                AssociatedQuality = qRequirement.AssociatedQuality,
                Id = qRequirement.Id
            };
        }
    }
}
