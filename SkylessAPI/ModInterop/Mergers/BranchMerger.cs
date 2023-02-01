using Failbetter.Core;
using Failbetter.Core.Provider;
using SkylessAPI.Utilities;
using System.Text.Json;

namespace SkylessAPI.ModInterop.Mergers
{
    internal class BranchMerger : IMerger<Branch>
    {
        private static BranchMerger _instance = null;
        public static BranchMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new BranchMerger();
                return _instance;
            }
        }

        public void Merge(Branch branchTo, JsonElement branchFrom, int offset)
        {
            var qualitiesRequired = RepositoryMerger.HandleListMerge(branchTo.QualitiesRequired.ToList(), 
                branchFrom.GetPropertyOrDefault("QualitiesRequired"), BranchQRequirementMerger.Instance, offset).ToIList();

            branchTo.SuccessEvent = MergeBranchEvents(branchTo.SuccessEvent, branchFrom, "SuccessEvent", offset);
            branchTo.DefaultEvent = MergeBranchEvents(branchTo.DefaultEvent, branchFrom, "DefaultEvent", offset);
            branchTo.RareDefaultEvent = MergeBranchEvents(branchTo.RareDefaultEvent, branchFrom, "RareDefaultEvent", offset);
            branchTo.RareDefaultEventChance = (int)branchFrom.GetPropertyValueOrDefault("RareDefaultEventChance", branchTo.RareDefaultEventChance);
            branchTo.RareSuccessEvent = MergeBranchEvents(branchTo.RareSuccessEvent, branchFrom, "RareSuccessEvent", offset);
            branchTo.RareSuccessEventChance = (int)branchFrom.GetPropertyValueOrDefault("RareSuccessEventChance", branchTo.RareSuccessEventChance);
            branchTo.QualitiesRequired = qualitiesRequired;
            branchTo.Image = (string)branchFrom.GetPropertyValueOrDefault("Image", branchTo.Image);
            branchTo.Description = (string)branchFrom.GetPropertyValueOrDefault("Description", branchTo.Description);
            branchTo.OwnerName = (string)branchFrom.GetPropertyValueOrDefault("OwnerName", branchTo.OwnerName);
            branchTo.CurrencyCost = (int)branchFrom.GetPropertyValueOrDefault("CurrencyCost", branchTo.CurrencyCost);
            branchTo.Archived = (bool)branchFrom.GetPropertyValueOrDefault("Archived", branchTo.Archived);
            branchTo.ButtonText = (string)branchFrom.GetPropertyValueOrDefault("ButtonText", branchTo.ButtonText);
            branchTo.Ordering = (int)branchFrom.GetPropertyValueOrDefault("Ordering", branchTo.Ordering);
            branchTo.ActionCost = (int)branchFrom.GetPropertyValueOrDefault("ActionCost", branchTo.ActionCost);
            branchTo.Name = (string)branchFrom.GetPropertyValueOrDefault("Name", branchTo.Name);
        }

        public Branch FromJsonElement(JsonElement item, int offset)
        {
            var qualitiesRequired = RepositoryMerger.HandleListMerge(null, item.GetPropertyOrDefault("QualitiesRequired"), 
                BranchQRequirementMerger.Instance, offset).ToIList();

            return new Branch()
            {
                SuccessEvent = GetBranchEvent(item, "SuccessEvent", offset),
                DefaultEvent = GetBranchEvent(item, "DefaultEvent", offset),
                RareDefaultEvent = GetBranchEvent(item, "RareDefaultEvent", offset),
                RareDefaultEventChance = (int)item.GetPropertyValueOrDefault("RareDefaultEventChance", 0),
                RareSuccessEvent = GetBranchEvent(item, "RareSuccessEvent", offset),
                RareSuccessEventChance = (int)item.GetPropertyValueOrDefault("RareSuccessEventChance", 0),
                QualitiesRequired = qualitiesRequired,
                Image = (string)item.GetPropertyValueOrDefault("Image"),
                Description = (string)item.GetPropertyValueOrDefault("Description"),
                OwnerName = (string)item.GetPropertyValueOrDefault("OwnerName"),
                DateTimeCreated = Clock.Instance.Now(),
                CurrencyCost = (int)item.GetPropertyValueOrDefault("CurrencyCost", 0),
                Archived = (bool)item.GetPropertyValueOrDefault("Archived", false),
                ButtonText = (string)item.GetPropertyValueOrDefault("ButtonText"),
                Ordering = (int)item.GetPropertyValueOrDefault("Ordering", 0),
                ActionCost = (int)item.GetPropertyValueOrDefault("ActionCost", 0),
                Name = (string)item.GetPropertyValueOrDefault("Name"),
                Id = item.Id(offset, false)
            };
        }

        private Event MergeBranchEvents(Event eventTo, JsonElement branchFrom, string eventName, int offset)
        {
            if (branchFrom.TryGetProperty(eventName, out var eventFrom))
            {
                if (eventFrom.ValueKind == JsonValueKind.Null)
                    return null;
                else if (eventTo == null)
                    return EventMerger.Instance.FromJsonElement(eventFrom, offset);
                else
                    EventMerger.Instance.Merge(eventTo, eventFrom, offset);
            }

            return eventTo;
        }

        private Event GetBranchEvent(JsonElement branch, string eventName, int offset)
        {
            if (branch.TryGetProperty(eventName, out var @event) && @event.ValueKind != JsonValueKind.Null)
            {
                return EventMerger.Instance.FromJsonElement(@event, offset);
            }

            return null;
        }
    }
}
