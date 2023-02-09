using Failbetter.Core;
using Failbetter.Interfaces.Enums;
using SkylessAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SkylessAPI.ModInterop.Mergers
{
    internal class QualityMerger : IMerger<Quality>
    {
        private static QualityMerger _instance = null;
        public static QualityMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new QualityMerger();
                return _instance;
            }
        }

        public void Merge(Quality qualityTo, JsonElement qualityFrom, int offset)
        {
            var qualitiesPossessedList = RepositoryMerger.HandleListMerge(qualityTo.QualitiesPossessedList.ToList(),
                qualityFrom.GetPropertyOrDefault("QualitiesPossessedList"), AspectQPossessionMerger.Instance, offset).ToIList();

            var enhancements = RepositoryMerger.HandleListMerge(qualityTo.Enhancements.ToList(),
                qualityFrom.GetPropertyOrDefault("Enhancements"), QEnhancementMerger.Instance, offset).ToIList();

            if (qualityFrom.TryGetProperty("Cap", out var cap) && cap.ValueKind == JsonValueKind.Null)
                qualityTo.Cap = null;

            if (qualityFrom.TryGetProperty("QEffectMinimalLimit", out var lim) && lim.ValueKind == JsonValueKind.Null)
                qualityTo.QEffectMinimalLimit = null;

            var assignToSlot = qualityTo.AssignToSlot;
            if (qualityFrom.TryGetProperty("AssignToSlot", out var slot))
            {
                if (slot.ValueKind == JsonValueKind.Null)
                    assignToSlot = null;
                else
                    assignToSlot = new Quality(slot.Id(offset));
            }

            var parentQuality = qualityTo.ParentQuality;
            if (qualityFrom.TryGetProperty("ParentQuality", out var parent))
            {
                if (parent.ValueKind == JsonValueKind.Null)
                    parentQuality = null;
                else
                    parentQuality = new Quality(parent.Id(offset));
            }

            var useEvent = qualityTo.UseEvent;
            if (qualityFrom.TryGetProperty("UseEvent", out var use))
            {
                if (use.ValueKind == JsonValueKind.Null)
                    useEvent = null;
                else
                    useEvent = new Event(use.Id(offset));
            }

            qualityTo.QualitiesPossessedList = qualitiesPossessedList;
            qualityTo.RelationshipCapable = (bool)qualityFrom.GetPropertyValueOrDefault("RelationshipCapable", qualityTo.RelationshipCapable);
            qualityTo.PluralName = (string)qualityFrom.GetPropertyValueOrDefault("PluralName", qualityTo.PluralName);
            qualityTo.OwnerName = (string)qualityFrom.GetPropertyValueOrDefault("OwnerName", qualityTo.OwnerName);
            qualityTo.Description = ((string)qualityFrom.GetPropertyValueOrDefault("Description", qualityTo.Description)).ReplaceModIDsInTokens(offset);
            qualityTo.Image = (string)qualityFrom.GetPropertyValueOrDefault("Image", qualityTo.Image);
            qualityTo.Notes = (string)qualityFrom.GetPropertyValueOrDefault("Notes", qualityTo.Notes);
            qualityTo.Tag = (string)qualityFrom.GetPropertyValueOrDefault("Tag", qualityTo.Tag);
            qualityTo.CapAdvanced = ((string)qualityFrom.GetPropertyValueOrDefault("CapAdvanced", qualityTo.CapAdvanced)).ReplaceModIDsInTokens(offset);
            qualityTo.HimbleLevel = (int)qualityFrom.GetPropertyValueOrDefault("HimbleLevel", qualityTo.HimbleLevel);
            qualityTo.UsePyramidNumbers = (bool)qualityFrom.GetPropertyValueOrDefault("UsePyramidNumbers", qualityTo.UsePyramidNumbers);
            qualityTo.PyramidNumberIncreaseLimit = (int)qualityFrom.GetPropertyValueOrDefault("PyramidNumberIncreaseLimit", qualityTo.PyramidNumberIncreaseLimit);
            qualityTo.AvailableAt = (string)qualityFrom.GetPropertyValueOrDefault("AvailableAt", qualityTo.AvailableAt);
            qualityTo.PreventNaming = (bool)qualityFrom.GetPropertyValueOrDefault("PreventNaming", qualityTo.PreventNaming);
            qualityTo.CssClasses = (string)qualityFrom.GetPropertyValueOrDefault("CssClasses", qualityTo.CssClasses);
            qualityTo.QEffectPriority = (int)qualityFrom.GetPropertyValueOrDefault("QEffectPriority", qualityTo.QEffectPriority);
            qualityTo.Ordering = (int)qualityFrom.GetPropertyValueOrDefault("Ordering", qualityTo.Ordering);
            qualityTo.IsSlot = (bool)qualityFrom.GetPropertyValueOrDefault("IsSlot", qualityTo.IsSlot);
            qualityTo.AssignToSlot = assignToSlot;
            qualityTo.ParentQuality = parentQuality;
            qualityTo.Persistent = (bool)qualityFrom.GetPropertyValueOrDefault("Persistent", qualityTo.Persistent);
            qualityTo.Visible = (bool)qualityFrom.GetPropertyValueOrDefault("Visible", qualityTo.Visible);
            qualityTo.Enhancements = enhancements;
            qualityTo.EnhancementsDescription = (string)qualityFrom.GetPropertyValueOrDefault("EnhancementsDescription", qualityTo.EnhancementsDescription);
            qualityTo.UseEvent = useEvent;
            qualityTo.DifficultyTestType = (DifficultyTestType)qualityFrom.GetPropertyValueOrDefault("DifficultyTestType", qualityTo.DifficultyTestType);
            qualityTo.DifficultyScaler = (int)qualityFrom.GetPropertyValueOrDefault("DifficultyScaler", qualityTo.DifficultyScaler);
            qualityTo.AllowedOn = (QualityAllowedOn)qualityFrom.GetPropertyValueOrDefault("QualityAllowedOn", qualityTo.AllowedOn);
            qualityTo.Nature = (Nature)qualityFrom.GetPropertyValueOrDefault("Nature", qualityTo.Nature);
            qualityTo.Category = (Category)qualityFrom.GetPropertyValueOrDefault("Category", qualityTo.Category);
            qualityTo.LevelDescriptionText = ((string)qualityFrom.GetPropertyValueOrDefault("LevelDescriptionText", qualityTo.LevelDescriptionText)).ReplaceModIDsInTokens(offset);
            qualityTo.ChangeDescriptionText = (string)qualityFrom.GetPropertyValueOrDefault("ChangeDescriptionText", qualityTo.ChangeDescriptionText);
            qualityTo.DescendingChangeDescriptionText = (string)qualityFrom.GetPropertyValueOrDefault("DescendingChangeDescriptionText", qualityTo.DescendingChangeDescriptionText);
            qualityTo.LevelImageText = (string)qualityFrom.GetPropertyValueOrDefault("LevelImageText", qualityTo.LevelImageText);
            qualityTo.VariableDescriptionText = (string)qualityFrom.GetPropertyValueOrDefault("VariableDescriptionText", qualityTo.VariableDescriptionText);
            qualityTo.Name = (string)qualityFrom.GetPropertyValueOrDefault("Name", qualityTo.Name);
        }

        public Quality FromJsonElement(JsonElement item, int offset)
        {
            var qualitiesPossessedList = RepositoryMerger.HandleListMerge(null,
                item.GetPropertyOrDefault("QualitiesPossessedList"), AspectQPossessionMerger.Instance, offset).ToIList();

            var enhancements = RepositoryMerger.HandleListMerge(null,
                item.GetPropertyOrDefault("Enhancements"), QEnhancementMerger.Instance, offset).ToIList();

            Quality assignToSlot = null;
            if (item.TryGetProperty("AssignToSlot", out var slot) && slot.ValueKind != JsonValueKind.Null)
                assignToSlot = new Quality(slot.Id(offset));

            Quality parentQuality = null;
            if (item.TryGetProperty("ParentQuality", out var parent) && parent.ValueKind != JsonValueKind.Null)
                parentQuality = new Quality(parent.Id(offset));

            Event useEvent = null;
            if (item.TryGetProperty("UseEvent", out var use) && use.ValueKind != JsonValueKind.Null)
                useEvent = new Event(use.Id(offset));

            return new Quality()
            {
                QualitiesPossessedList = qualitiesPossessedList,
                RelationshipCapable = (bool)item.GetPropertyValueOrDefault("RelationshipCapable", false),
                PluralName = (string)item.GetPropertyValueOrDefault("PluralName"),
                OwnerName = (string)item.GetPropertyValueOrDefault("OwnerName", ""),
                Description = ((string)item.GetPropertyValueOrDefault("Description", "")).ReplaceModIDsInTokens(offset),
                Image = (string)item.GetPropertyValueOrDefault("Image"),
                Notes = (string)item.GetPropertyValueOrDefault("Notes"),
                Tag = (string)item.GetPropertyValueOrDefault("Tag", ""),
                CapAdvanced = ((string)item.GetPropertyValueOrDefault("CapAdvanced")).ReplaceModIDsInTokens(offset),
                HimbleLevel = (int)item.GetPropertyValueOrDefault("HimbleLevel", 0),
                UsePyramidNumbers = (bool)item.GetPropertyValueOrDefault("UsePyramidNumbers", false),
                PyramidNumberIncreaseLimit = (int)item.GetPropertyValueOrDefault("PyramidNumberIncreaseLimit", 50),
                AvailableAt = (string)item.GetPropertyValueOrDefault("AvailableAt"),
                PreventNaming = (bool)item.GetPropertyValueOrDefault("PreventNaming", false),
                CssClasses = (string)item.GetPropertyValueOrDefault("CssClasses"),
                QEffectPriority = (int)item.GetPropertyValueOrDefault("QEffectPriority", 2),
                Ordering = (int)item.GetPropertyValueOrDefault("Ordering", 0),
                IsSlot = (bool)item.GetPropertyValueOrDefault("IsSlot", false),
                AssignToSlot = assignToSlot,
                ParentQuality = parentQuality,
                Persistent = (bool)item.GetPropertyValueOrDefault("Persistent", false),
                Visible = (bool)item.GetPropertyValueOrDefault("Visible", true),
                Enhancements = enhancements,
                EnhancementsDescription = (string)item.GetPropertyValueOrDefault("EnhancementsDescription"),
                UseEvent = useEvent,
                DifficultyTestType = (DifficultyTestType)item.GetPropertyValueOrDefault("DifficultyTestType", 0),
                DifficultyScaler = (int)item.GetPropertyValueOrDefault("DifficultyScaler", 60),
                AllowedOn = (QualityAllowedOn)item.GetPropertyValueOrDefault("QualityAllowedOn", 0),
                Nature = (Nature)item.GetPropertyValueOrDefault("Nature", 0),
                Category = (Category)item.GetPropertyValueOrDefault("Category", 0),
                LevelDescriptionText = ((string)item.GetPropertyValueOrDefault("LevelDescriptionText")).ReplaceModIDsInTokens(offset),
                ChangeDescriptionText = (string)item.GetPropertyValueOrDefault("ChangeDescriptionText"),
                DescendingChangeDescriptionText = (string)item.GetPropertyValueOrDefault("DescendingChangeDescriptionText"),
                LevelImageText = (string)item.GetPropertyValueOrDefault("LevelImageText"),
                VariableDescriptionText = (string)item.GetPropertyValueOrDefault("VariableDescriptionText"),
                Name = (string)item.GetPropertyValueOrDefault("Name", ""),
                Id = item.Id(offset, false)
            };
        }

        public Quality Clone(Quality quality)
        {
            return new Quality()
            {
                QualitiesPossessedList = quality.QualitiesPossessedList.ToList().Clone(AspectQPossessionMerger.Instance.Clone).ToIList(),
                RelationshipCapable = quality.RelationshipCapable,
                PluralName = quality.PluralName,
                OwnerName = quality.OwnerName,
                Description = quality.Description,
                Image = quality.Image,
                Notes = quality.Notes,
                Tag = quality.Tag,
                Cap = quality.Cap,
                CapAdvanced = quality.CapAdvanced,
                HimbleLevel = quality.HimbleLevel,
                UsePyramidNumbers = quality.UsePyramidNumbers,
                PyramidNumberIncreaseLimit = quality.PyramidNumberIncreaseLimit,
                AvailableAt = quality.AvailableAt,
                PreventNaming = quality.PreventNaming,
                CssClasses = quality.CssClasses,
                QEffectPriority = quality.QEffectPriority,
                QEffectMinimalLimit = quality.QEffectMinimalLimit,
                World = quality.World,
                Ordering = quality.Ordering,
                IsSlot = quality.IsSlot,
                LimitedToArea = quality.LimitedToArea,
                AssignToSlot = quality.AssignToSlot,
                ParentQuality = quality.ParentQuality,
                Persistent = quality.Persistent,
                Visible = quality.Visible,
                Enhancements = quality.Enhancements.ToList().Clone(QEnhancementMerger.Instance.Clone).ToIList(),
                EnhancementsDescription = quality.EnhancementsDescription,
                SecondChanceQuality = quality.SecondChanceQuality,
                UseEvent = quality.UseEvent,
                DifficultyTestType = quality.DifficultyTestType,
                DifficultyScaler = quality.DifficultyScaler,
                AllowedOn = quality.AllowedOn,
                Nature = quality.Nature,
                Category = quality.Category,
                LevelDescriptionText = quality.LevelDescriptionText,
                ChangeDescriptionText = quality.ChangeDescriptionText,
                DescendingChangeDescriptionText = quality.DescendingChangeDescriptionText,
                LevelImageText = quality.LevelImageText,
                VariableDescriptionText = quality.VariableDescriptionText,
                Name = quality.Name,
                Id = quality.Id
            };
        }
    }
}
