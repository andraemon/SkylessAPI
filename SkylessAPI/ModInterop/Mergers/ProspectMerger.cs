using Failbetter.Core;
using SkylessAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SkylessAPI.ModInterop.Mergers
{
    internal class ProspectMerger : IMerger<Prospect>
    {
        private static ProspectMerger _instance = null;
        public static ProspectMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ProspectMerger();
                return _instance;
            }
        }

        public void Merge(Prospect prospectTo, JsonElement prospectFrom, int offset)
        {
            var setting = prospectTo.Setting;
            if (prospectFrom.TryGetProperty("Setting", out var set))
            {
                if (set.ValueKind == JsonValueKind.Null)
                    setting = null;
                else
                    setting = new Setting()
                    {
                        Id = set.Id(offset)
                    };
            }

            var request = prospectTo.Request;
            if (prospectFrom.TryGetProperty("Request", out var req))
                request = new Quality(req.Id(offset));

            var qualitiesAffected = RepositoryMerger.HandleListMerge(prospectTo.QualitiesAffected.ToList(),
                prospectFrom.GetPropertyOrDefault("QualitiesAffected"), ProspectQEffectMerger.Instance, offset).ToIList();

            var qualitiesRequired = RepositoryMerger.HandleListMerge(prospectTo.QualitiesRequired.ToList(),
                prospectFrom.GetPropertyOrDefault("QualitiesRequired"), ProspectQRequirementMerger.Instance, offset).ToIList();

            var completions = RepositoryMerger.HandleListMerge(prospectTo.Completions.ToList(),
                prospectFrom.GetPropertyOrDefault("Completions"), CompletionMerger.Instance, offset).ToIList();

            prospectTo.Tags = (string)prospectFrom.GetPropertyValueOrDefault("Tags", prospectTo.Tags);
            prospectTo.Description = (string)prospectFrom.GetPropertyValueOrDefault("Description", prospectTo.Description);
            prospectTo.Setting = setting;
            prospectTo.Request = request;
            prospectTo.Demand = (int)prospectFrom.GetPropertyValueOrDefault("Demand", prospectTo.Demand);
            prospectTo.Payment = (string)prospectFrom.GetPropertyValueOrDefault("Payment", prospectTo.Payment);
            prospectTo.QualitiesAffected = qualitiesAffected;
            prospectTo.QualitiesRequired = qualitiesRequired;
            prospectTo.Completions = completions;
            prospectTo.Name = (string)prospectFrom.GetPropertyValueOrDefault("Name", prospectTo.Name);
        }

        public Prospect FromJsonElement(JsonElement item, int offset)
        {
            Setting setting = null;
            if (item.TryGetProperty("Setting", out var set) && set.ValueKind != JsonValueKind.Null)
                setting = new Setting()
                {
                    Id = set.Id(offset)
                };

            var qualitiesAffected = RepositoryMerger.HandleListMerge(null, 
                item.GetPropertyOrDefault("QualitiesAffected"), ProspectQEffectMerger.Instance, offset).ToIList();

            var qualitiesRequired = RepositoryMerger.HandleListMerge(null, 
                item.GetPropertyOrDefault("QualitiesRequired"), ProspectQRequirementMerger.Instance, offset).ToIList();

            var completions = RepositoryMerger.HandleListMerge(null, 
                item.GetPropertyOrDefault("Completions"), CompletionMerger.Instance, offset).ToIList();

            return new Prospect() 
            { 
                Tags = (string)item.GetPropertyValueOrDefault("Tags"),
                Description = (string)item.GetPropertyValueOrDefault("Description"),
                Setting = setting,
                Request = new Quality(item.GetProperty("Request").Id(offset)),
                Demand = (int)item.GetPropertyValueOrDefault("Demand"),
                Payment = (string)item.GetPropertyValueOrDefault("Payment"),
                QualitiesAffected = qualitiesAffected,
                QualitiesRequired = qualitiesRequired,
                Completions = completions,
                Name = (string)item.GetPropertyValueOrDefault("Name"),
                Id = item.Id(offset, false)
            };
        }
    }
}
