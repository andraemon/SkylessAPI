using Failbetter.Core;
using Failbetter.Core.Provider;
using Failbetter.Core.QAssoc;
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
    internal class EventMerger : IMerger<Event>
    {
        private static EventMerger _instance = null;
        public static EventMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new EventMerger();
                return _instance;
            }
        }

        public void Merge(Event eventTo, JsonElement eventFrom, int offset)
        {
            var childBranches = RepositoryMerger.HandleListMerge(eventTo.ChildBranches.ToList(), eventFrom.GetPropertyOrDefault("ChildBranches"),
                BranchMerger.Instance, offset).ToIList();

            var qualitiesAffected = RepositoryMerger.HandleListMerge(eventTo.QualitiesAffected.ToList(), eventFrom.GetPropertyOrDefault("QualitiesAffected"),
                EventQEffectMerger.Instance, offset).ToIList();

            var qualitiesRequired = RepositoryMerger.HandleListMerge(eventTo.QualitiesRequired, eventFrom.GetPropertyOrDefault("QualitiesRequired"),
                EventQRequirementMerger.Instance, offset);

            var linkToEvent = eventTo.LinkToEvent;
            if (eventFrom.TryGetProperty("LinkToEvent", out var link))
            {
                if (link.ValueKind == JsonValueKind.Null)
                {
                    linkToEvent = null;
                }
                else
                {
                    linkToEvent = new Event(link.Id(offset));
                }
            }

            var limitedToArea = eventTo.LimitedToArea;
            if (eventFrom.TryGetProperty("LimitedToArea", out var areaLimit))
            {
                if (areaLimit.ValueKind == JsonValueKind.Null)
                {
                    limitedToArea = null;
                }
                else
                {
                    limitedToArea = new Area
                    {
                        Id = areaLimit.Id(offset)
                    };
                }
            }

            var moveToArea = eventTo.MoveToArea;
            if (eventFrom.TryGetProperty("MoveToArea", out var areaMove))
            {
                if (areaMove.ValueKind == JsonValueKind.Null)
                {
                    moveToArea = null;
                }
                else
                {
                    moveToArea = new Area
                    {
                        Id = areaMove.Id(offset)
                    };
                }
            }

            var moveToDomicile = eventTo.MoveToDomicile;
            if (eventFrom.TryGetProperty("MoveToDomicile", out var domMove))
            {
                if (domMove.ValueKind == JsonValueKind.Null)
                {
                    moveToDomicile = null;
                }
                else
                {
                    moveToDomicile = new Domicile()
                    {
                        Id = domMove.Id(offset)
                    };
                }
            }

            var switchToSetting = eventTo.SwitchToSetting;
            if (eventFrom.TryGetProperty("SwitchToSetting", out var settingSwitch))
            {
                if (settingSwitch.ValueKind == JsonValueKind.Null)
                {
                    switchToSetting = null;
                }
                else
                {
                    switchToSetting = new Setting()
                    {
                        Id = settingSwitch.Id(offset)
                    };
                }
            }

            var logInJournalAgainstQuality = eventTo.LogInJournalAgainstQuality;
            if (eventFrom.TryGetProperty("LogInJournalAgainstQuality", out var logQuality))
            {
                if (logQuality.ValueKind == JsonValueKind.Null)
                {
                    logInJournalAgainstQuality = null;
                }
                else
                {
                    logInJournalAgainstQuality = new Quality(logQuality.Id(offset));
                }
            }

            var setting = eventTo.Setting;
            if (eventFrom.TryGetProperty("Setting", out var sett))
            {
                if (sett.ValueKind == JsonValueKind.Null)
                {
                    setting = null;
                }
                else
                {
                    setting = new Setting()
                    {
                        Id = sett.Id(offset)
                    };
                }
            }

            eventTo.ChildBranches = childBranches;
            eventTo.QualitiesAffected = qualitiesAffected;
            eventTo.QualitiesRequired = qualitiesRequired;
            eventTo.Image = (string)eventFrom.GetPropertyValueOrDefault("Image", eventTo.Image);
            eventTo.SecondImage = (string)eventFrom.GetPropertyValueOrDefault("SecondImage", eventTo.SecondImage);
            eventTo.Description = ((string)eventFrom.GetPropertyValueOrDefault("Description", eventTo.Description)).ReplaceModIDsInTokens(offset);
            eventTo.Tag = (string)eventFrom.GetPropertyValueOrDefault("Tag", eventTo.Tag);
            eventTo.ExoticEffects = (string)eventFrom.GetPropertyValueOrDefault("ExoticEffects", eventTo.ExoticEffects);
            eventTo.Note = (string)eventFrom.GetPropertyValueOrDefault("Note", eventTo.Note);
            eventTo.ChallengeLevel = (int)eventFrom.GetPropertyValueOrDefault("ChallengeLevel", eventTo.ChallengeLevel);
            eventTo.Ordering = eventFrom.TryGetProperty("Ordering", out var ord) ? ord.GetSingle() : eventTo.Ordering;
            eventTo.ShowAsMessage = (bool)eventFrom.GetPropertyValueOrDefault("ShowAsMessage", eventTo.ShowAsMessage);
            eventTo.LinkToEvent = linkToEvent;
            eventTo.Deck = eventFrom.TryGetProperty("Deck", out var deck) ? DeckMerger.Instance.FromJsonElement(deck, offset) : eventTo.Deck;
            eventTo.Category = (EventCategory)eventFrom.GetPropertyValueOrDefault("Category", eventTo.Category);
            eventTo.LimitedToArea = limitedToArea;
            eventTo.Transient = (bool)eventFrom.GetPropertyValueOrDefault("Transient", eventTo.Transient);
            eventTo.Stickiness = (int)eventFrom.GetPropertyValueOrDefault("Stickiness", eventTo.Stickiness);
            eventTo.MoveToAreaId = (int)eventFrom.GetPropertyValueOrDefault("MoveToAreaId", eventTo.MoveToAreaId);
            eventTo.MoveToArea = moveToArea;
            eventTo.MoveToDomicile = moveToDomicile;
            eventTo.SwitchToSetting = switchToSetting;
            eventTo.FatePointsChange = (int)eventFrom.GetPropertyValueOrDefault("FatePointsChange", eventTo.FatePointsChange);
            eventTo.BootyValue = (int)eventFrom.GetPropertyValueOrDefault("BootyValue", eventTo.BootyValue);
            eventTo.LogInJournalAgainstQuality = logInJournalAgainstQuality;
            eventTo.Setting = setting;
            eventTo.Urgency = (Urgency)eventFrom.GetPropertyValueOrDefault("Urgency", eventTo.Urgency);
            eventTo.OwnerName = (string)eventFrom.GetPropertyValueOrDefault("OwnerName", eventTo.OwnerName);
            eventTo.Distribution = (int)eventFrom.GetPropertyValueOrDefault("Distribution", eventTo.Distribution);
            eventTo.Autofire = (bool)eventFrom.GetPropertyValueOrDefault("Autofire", eventTo.Autofire);
            eventTo.CanGoBack = (bool)eventFrom.GetPropertyValueOrDefault("CanGoBack", eventTo.CanGoBack);
            eventTo.Name = ((string)eventFrom.GetPropertyValueOrDefault("Name", eventTo.Name)).ReplaceModIDsInTokens(offset);
        }

        public Event FromJsonElement(JsonElement item, int offset)
        {
            var childBranches = RepositoryMerger.HandleListMerge(null, item.GetPropertyOrDefault("ChildBranches"), 
                BranchMerger.Instance, offset).ToIList();

            var qualitiesAffected = RepositoryMerger.HandleListMerge(null, item.GetPropertyOrDefault("QualitiesAffected"),
                EventQEffectMerger.Instance, offset).ToIList();

            var qualitiesRequired = RepositoryMerger.HandleListMerge(null, item.GetPropertyOrDefault("QualitiesRequired"),
                EventQRequirementMerger.Instance, offset);

            Event linkToEvent = null;
            if (item.TryGetProperty("LinkToEvent", out var link) && link.ValueKind != JsonValueKind.Null)
            {
                linkToEvent = new Event(link.Id(offset));
            }

            Area limitedToArea = null;
            if (item.TryGetProperty("LimitedToArea", out var areaLimit) && areaLimit.ValueKind != JsonValueKind.Null)
            {
                limitedToArea = new Area()
                {
                    Id = areaLimit.Id(offset)
                };
            }

            Area moveToArea = null;
            if (item.TryGetProperty("MoveToArea", out var areaMove) && areaMove.ValueKind != JsonValueKind.Null)
            {
                moveToArea = new Area()
                {
                    Id = areaMove.Id(offset)
                };
            }

            Domicile moveToDomicile = null;
            if (item.TryGetProperty("MoveToDomicile", out var domMove) && domMove.ValueKind != JsonValueKind.Null)
            {
                moveToDomicile = new Domicile()
                {
                    Id = domMove.Id(offset)
                };
            }

            Setting switchToSetting = null;
            if (item.TryGetProperty("SwitchToSetting", out var settingSwitch) && settingSwitch.ValueKind != JsonValueKind.Null)
            {
                switchToSetting = new Setting()
                {
                    Id = settingSwitch.Id(offset)
                };
            }

            Quality logInJournalAgainstQuality = null;
            if (item.TryGetProperty("LogInJournalAgainstQuality", out var logQuality) && logQuality.ValueKind != JsonValueKind.Null)
            {
                logInJournalAgainstQuality = new Quality(logQuality.Id(offset));
            }

            Setting setting = null;
            if (item.TryGetProperty("Setting", out var sett) && sett.ValueKind != JsonValueKind.Null)
            {
                setting = new Setting()
                {
                    Id = sett.Id(offset)
                };
            }

            return new Event() {
                ChildBranches = childBranches,
                QualitiesAffected = qualitiesAffected,
                QualitiesRequired = qualitiesRequired,
                Image = (string)item.GetPropertyValueOrDefault("Image"),
                SecondImage = (string)item.GetPropertyValueOrDefault("SecondImage"),
                Description = ((string)item.GetPropertyValueOrDefault("Description")).ReplaceModIDsInTokens(offset),
                Tag = (string)item.GetPropertyValueOrDefault("Tag"),
                ExoticEffects = (string)item.GetPropertyValueOrDefault("ExoticEffects", ""),
                Note = (string)item.GetPropertyValueOrDefault("Note"),
                ChallengeLevel = (int)item.GetPropertyValueOrDefault("ChallengeLevel", 0),
                Ordering = item.TryGetProperty("Ordering", out var ord) ? ord.GetSingle() : 0,
                ShowAsMessage = (bool)item.GetPropertyValueOrDefault("ShowAsMessage", false),
                LinkToEvent = linkToEvent,
                Deck = DeckMerger.Instance.FromJsonElement(item.GetPropertyOrDefault("Deck"), offset),
                Category = (EventCategory)item.GetPropertyValueOrDefault("Category", 0),
                LimitedToArea = limitedToArea,
                Transient = (bool)item.GetPropertyValueOrDefault("Transient", false),
                Stickiness = (int)item.GetPropertyValueOrDefault("Stickiness", 0),
                MoveToAreaId = (int)item.GetPropertyValueOrDefault("MoveToAreaId", 0),
                MoveToArea = moveToArea,
                MoveToDomicile = moveToDomicile,
                SwitchToSetting = switchToSetting,
                FatePointsChange = (int)item.GetPropertyValueOrDefault("FatePointsChange", 0),
                BootyValue = (int)item.GetPropertyValueOrDefault("BootyValue", 0),
                LogInJournalAgainstQuality = logInJournalAgainstQuality,
                Setting = setting,
                Urgency = (Urgency)item.GetPropertyValueOrDefault("Urgency", 0),
                OwnerName = (string)item.GetPropertyValueOrDefault("OwnerName"),
                DateTimeCreated = Clock.Instance.Now(),
                Distribution = (int)item.GetPropertyValueOrDefault("Distribution", 0),
                Autofire = (bool)item.GetPropertyValueOrDefault("Autofire", true),
                CanGoBack = (bool)item.GetPropertyValueOrDefault("CanGoBack", false),
                Name = ((string)item.GetPropertyValueOrDefault("Name")).ReplaceModIDsInTokens(offset),
                Id = item.Id(offset, false)
            };
        }

        public Event Clone(Event @event)
        {
            return new Event()
            {
                ChildBranches = @event.ChildBranches.ToList().Clone(BranchMerger.Instance.Clone).ToIList(),
                ParentBranch = @event.ParentBranch,
                QualitiesAffected = @event.QualitiesAffected.ToList().Clone(EventQEffectMerger.Instance.Clone).ToIList(),
                QualitiesRequired = @event.QualitiesRequired.Clone(EventQRequirementMerger.Instance.Clone),
                Image = @event.Image,
                SecondImage = @event.SecondImage,
                Description = @event.Description,
                Tag = @event.Tag,
                ExoticEffects = @event.ExoticEffects,
                Note = @event.Note,
                ChallengeLevel = @event.ChallengeLevel,
                UnclearedEditAt = @event.UnclearedEditAt,
                LastEditedBy = @event.LastEditedBy,
                Ordering = @event.Ordering,
                ShowAsMessage = @event.ShowAsMessage,
                LivingStory = @event.LivingStory,
                LinkToEvent = @event.LinkToEvent,
                Deck = @event.Deck,
                Category = @event.Category,
                LimitedToArea = @event.LimitedToArea,
                World = @event.World,
                Transient = @event.Transient,
                Stickiness = @event.Stickiness,
                MoveToAreaId = @event.MoveToAreaId,
                MoveToArea = @event.MoveToArea,
                MoveToDomicile = @event.MoveToDomicile,
                SwitchToSetting = @event.SwitchToSetting,
                FatePointsChange = @event.FatePointsChange,
                BootyValue = @event.BootyValue,
                LogInJournalAgainstQuality = @event.LogInJournalAgainstQuality,
                Setting = @event.Setting,
                Urgency = @event.Urgency,
                Teaser = @event.Teaser,
                OwnerName = @event.OwnerName,
                DateTimeCreated = @event.DateTimeCreated,
                Distribution = @event.Distribution,
                Autofire = @event.Autofire,
                CanGoBack = @event.CanGoBack,
                Name = @event.Name,
                Id = @event.Id
            };
        }
    }
}
