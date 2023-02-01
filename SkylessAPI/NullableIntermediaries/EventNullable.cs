using Failbetter.Core;
using Failbetter.Core.QAssoc;
using Failbetter.Interfaces.Enums;
using SkylessAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylessAPI.NullableIntermediaries
{
    internal class EventNullable : IFailbetterEquivalent<Event>
    {
        #region Interface Methods
        public Event ToIL2Cpp() =>
            new Event()
            {
                ChildBranches = null, // ChildBranches.ToIl2CppIList(PUT FUNCTION HERE),
                ParentBranch = ParentBranch,
                QualitiesAffected = null, // QualitiesAffected.ToIl2CppIList(PUT FUNCTION HERE),
                QualitiesRequired = null, // QualitiesRequired.ToIl2CppList(PUT FUNCTION HERE),
                Image = Image,
                SecondImage = SecondImage,
                Description = Description,
                Tag = Tag,
                ExoticEffects = ExoticEffects,
                Note = Note,
                ChallengeLevel = ChallengeLevel.GetValueOrDefault(),
                UnclearedEditAt = null,
                LastEditedBy = LastEditedBy,
                Ordering = Ordering.GetValueOrDefault(),
                ShowAsMessage = ShowAsMessage.GetValueOrDefault(),
                LivingStory = LivingStory,
                LinkToEvent = LinkToEvent,
                Deck = Deck,
                Category = Category.GetValueOrDefault(),
                LimitedToArea = LimitedToArea.ToIL2Cpp(),
                World = World,
                Transient = Transient.GetValueOrDefault(),
                Stickiness = Stickiness ?? 1,
                MoveToAreaId = MoveToAreaId.GetValueOrDefault(),
                MoveToArea = MoveToArea.ToIL2Cpp(),
                MoveToDomicile = MoveToDomicile.ToIL2Cpp(),
                SwitchToSetting = SwitchToSetting.ToIL2Cpp(),
                FatePointsChange = FatePointsChange.GetValueOrDefault(),
                BootyValue = BootyValue.GetValueOrDefault(),
                LogInJournalAgainstQuality = LogInJournalAgainstQuality.ToIL2Cpp(),
                Setting = Setting.ToIL2Cpp(),
                Urgency = Urgency.GetValueOrDefault(),
                Teaser = Teaser,
                OwnerName = OwnerName,
                DateTimeCreated = default,
                Distribution = Distribution ?? 100,
                Autofire = Autofire ?? true,
                CanGoBack = CanGoBack ?? true,
                Name = Name,
                Id = Id
            };

        IFailbetterEquivalent<Event> IFailbetterEquivalent<Event>.FromIL2Cpp(Event @event) => FromIL2Cpp(@event);

        public static EventNullable FromIL2Cpp(Event @event) =>
            new EventNullable()
            {
                ChildBranches = null, // @event.ChildBranches.ToManagedIList(PUT FUNCTION HERE),
                ParentBranch = @event.ParentBranch,
                QualitiesAffected = null, // @event.QualitiesAffected.ToManagedIList(PUT FUNCTION HERE),
                QualitiesRequired = null, // @event.QualitiesRequired.ToManagedList(PUT FUNCTION HERE),
                Image = @event.Image,
                SecondImage = @event.SecondImage,
                Description = @event.Description,
                Tag = @event.Tag,
                ExoticEffects = @event.ExoticEffects,
                Note = @event.Note,
                ChallengeLevel = @event.ChallengeLevel,
                UnclearedEditAt = null,
                LastEditedBy = @event.LastEditedBy,
                Ordering = @event.Ordering,
                ShowAsMessage = @event.ShowAsMessage,
                LivingStory = @event.LivingStory,
                LinkToEvent = @event.LinkToEvent,
                Deck = @event.Deck,
                Category = @event.Category,
                LimitedToArea = AreaNullable.FromIL2Cpp(@event.LimitedToArea),
                World = @event.World,
                Transient = @event.Transient,
                Stickiness = @event.Stickiness,
                MoveToAreaId = @event.MoveToAreaId,
                MoveToArea = AreaNullable.FromIL2Cpp(@event.MoveToArea),
                MoveToDomicile = DomicileNullable.FromIL2Cpp(@event.MoveToDomicile),
                SwitchToSetting = SettingNullable.FromIL2Cpp(@event.SwitchToSetting),
                FatePointsChange = @event.FatePointsChange,
                BootyValue = @event.BootyValue,
                LogInJournalAgainstQuality = QualityNullable.FromIL2Cpp(@event.LogInJournalAgainstQuality),
                Setting = SettingNullable.FromIL2Cpp(@event.Setting),
                Urgency = @event.Urgency,
                Teaser = @event.Teaser,
                OwnerName = @event.OwnerName,
                DateTimeCreated = null,
                Distribution = @event.Distribution,
                Autofire = @event.Autofire,
                CanGoBack = @event.CanGoBack,
                Name = @event.Name,
                Id = @event.Id
            };
        #endregion

        public virtual IList<Branch> ChildBranches { get; set; } // Create nullable
        public virtual Branch ParentBranch { get; set; } // Always null during deserialization
        public virtual IList<EventQEffect> QualitiesAffected { get; set; } // Create nullable
        public virtual List<EventQRequirement> QualitiesRequired { get; set; } // Create nullable
        public virtual string Image { get; set; }
        public virtual string SecondImage { get; set; }
        public virtual string Description { get; set; }
        public virtual string Tag { get; set; }
        public virtual string ExoticEffects { get; set; }
        public virtual string Note { get; set; }
        public virtual int? ChallengeLevel { get; set; }
        public virtual DateTime? UnclearedEditAt { get; set; }
        public virtual User LastEditedBy { get; set; }
        public virtual float? Ordering { get; set; }
        public virtual bool? ShowAsMessage { get; set; }
        public virtual LivingStory LivingStory { get; set; }
        public virtual Event LinkToEvent { get; set; } // No need for a nullable, just ID is required
        public virtual Deck Deck { get; set; } 
        public virtual EventCategory? Category { get; set; }
        public virtual AreaNullable LimitedToArea { get; set; }
        public virtual World World { get; set; }
        public virtual bool? Transient { get; set; }
        public virtual int? Stickiness { get; set; }
        public virtual int? MoveToAreaId { get; set; }
        public virtual AreaNullable MoveToArea { get; set; }
        public virtual DomicileNullable MoveToDomicile { get; set; }
        public virtual SettingNullable SwitchToSetting { get; set; }
        public virtual int? FatePointsChange { get; set; }
        public virtual int? BootyValue { get; set; }
        public virtual QualityNullable LogInJournalAgainstQuality { get; set; }
        public virtual SettingNullable Setting { get; set; }
        public virtual Urgency? Urgency { get; set; }
        public virtual string Teaser { get; set; }
        public virtual string OwnerName { get; set; }
        public virtual DateTime? DateTimeCreated { get; set; }
        public virtual int? Distribution { get; set; }
        public virtual bool? Autofire { get; set; }
        public virtual bool? CanGoBack { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
    }
}
