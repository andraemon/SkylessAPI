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
    internal class CompletionMerger : IMerger<Completion>
    {
        private static CompletionMerger _instance = null;
        public static CompletionMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CompletionMerger();
                return _instance;
            }
        }

        public void Merge(Completion completionTo, JsonElement completionFrom, int offset)
        {
            var qualitiesAffected = RepositoryMerger.HandleListMerge(completionTo.QualitiesAffected.ToList(),
                completionFrom.GetPropertyOrDefault("QualitiesAffected"), CompletionQEffectMerger.Instance, offset).ToIList();

            var qualitiesRequired = RepositoryMerger.HandleListMerge(completionTo.QualitiesRequired.ToList(),
                completionFrom.GetPropertyOrDefault("QualitiesRequired"), CompletionQRequirementMerger.Instance, offset).ToIList();

            completionTo.Description = (string)completionFrom.GetPropertyValueOrDefault("Description", completionTo.Description);
            completionTo.SatisfactionMessage = (string)completionFrom.GetPropertyValueOrDefault("SatisfactionMessage", completionTo.SatisfactionMessage);
            completionTo.QualitiesAffected = qualitiesAffected;
            completionTo.QualitiesRequired = qualitiesRequired;
        }

        public Completion FromJsonElement(JsonElement item, int offset)
        {
            var qualitiesAffected = RepositoryMerger.HandleListMerge(null, 
                item.GetPropertyOrDefault("QualitiesAffected"), CompletionQEffectMerger.Instance, offset).ToIList();

            var qualitiesRequired = RepositoryMerger.HandleListMerge(null, 
                item.GetPropertyOrDefault("QualitiesRequired"), CompletionQRequirementMerger.Instance, offset).ToIList();

            return new Completion()
            {
                Description = (string)item.GetPropertyValueOrDefault("Description"),
                SatisfactionMessage = (string)item.GetPropertyValueOrDefault("SatisfactionMessage"),
                QualitiesAffected = qualitiesAffected,
                QualitiesRequired = qualitiesRequired,
                Id = item.Id(offset, false)
            };
        }

        public Completion Clone(Completion completion)
        {
            return new Completion()
            {
                Prospect = completion.Prospect,
                Description = completion.Description,
                SatisfactionMessage = completion.SatisfactionMessage,
                QualitiesAffected = completion.QualitiesAffected.ToList().Clone(CompletionQEffectMerger.Instance.Clone).ToIList(),
                QualitiesRequired = completion.QualitiesRequired.ToList().Clone(CompletionQRequirementMerger.Instance.Clone).ToIList(),
                Id = completion.Id
            };
        }
    }
}
