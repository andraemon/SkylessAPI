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
    internal class BranchQRequirementMerger : IMerger<BranchQRequirement>
    {
        private static BranchQRequirementMerger _instance = null;
        public static BranchQRequirementMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new BranchQRequirementMerger();
                return _instance;
            }
        }

        public void Merge(BranchQRequirement branchQTo, JsonElement branchQFrom, int offset)
        {
            BaseQRequirementMerger.Instance.Merge(branchQTo, branchQFrom, offset);

            if (branchQFrom.TryGetProperty("DifficultyLevel", out var diff) && diff.ValueKind == JsonValueKind.Null)
                branchQTo.DifficultyLevel = null;

            branchQTo.DifficultyAdvanced = ((string)branchQFrom.GetPropertyValueOrDefault("DifficultyAdvanced", branchQTo.DifficultyAdvanced)).ReplaceModIDsInTokens(offset);
            branchQTo.VisibleWhenRequirementFailed = (bool)branchQFrom.GetPropertyValueOrDefault("VisibleWhenRequirementFailed", branchQTo.VisibleWhenRequirementFailed);
            branchQTo.CustomLockedMessage = (string)branchQFrom.GetPropertyValueOrDefault("CustomLockedMessage", branchQTo.CustomLockedMessage);
            branchQTo.CustomUnlockedMessage = (string)branchQFrom.GetPropertyValueOrDefault("CustomUnlockedMessage", branchQTo.CustomUnlockedMessage);
            branchQTo.IsCostRequirement = (bool)branchQFrom.GetPropertyValueOrDefault("IsCostRequirement", branchQTo.IsCostRequirement);
        }

        public BranchQRequirement FromJsonElement(JsonElement item, int offset)
        {
            return new BranchQRequirement()
            {
                DifficultyAdvanced = ((string)item.GetPropertyValueOrDefault("DifficultyAdvanced")).ReplaceModIDsInTokens(offset),
                VisibleWhenRequirementFailed = (bool)item.GetPropertyValueOrDefault("VisibleWhenRequirementFailed", false),
                CustomLockedMessage = (string)item.GetPropertyValueOrDefault("CustomLockedMessage"),
                CustomUnlockedMessage = (string)item.GetPropertyValueOrDefault("CustomUnlockedMessage"),
                IsCostRequirement = (bool)item.GetPropertyValueOrDefault("IsCostRequirement", false),
                MinAdvanced = ((string)item.GetPropertyValueOrDefault("MinAdvanced")).ReplaceModIDsInTokens(offset),
                MaxAdvanced = ((string)item.GetPropertyValueOrDefault("MaxAdvanced")).ReplaceModIDsInTokens(offset),
                AssociatedQuality = new Quality(item.GetProperty("AssociatedQuality").Id(offset)),
                Id = item.Id(offset, false)
            };
        }
    }
}
