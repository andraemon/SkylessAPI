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
	internal class QualityNullable : IFailbetterEquivalent<Quality>
    {
		#region Interface Methods
		public Quality ToIL2Cpp() =>
			new Quality()
			{
				QualitiesPossessedList = null, // QualitiesPossessedList.ToIl2Cpp(), // ensure is not null
				RelationshipCapable = RelationshipCapable.GetValueOrDefault(), // fine
				PluralName = PluralName,
				OwnerName = OwnerName,
				Description = Description,
				Image = Image,
				Notes = Notes,
				Tag = Tag,
				Cap = new Il2CppSystem.Nullable<int>(0), // Cap.ToIL2CppNullable(),
				CapAdvanced = CapAdvanced,
				HimbleLevel = HimbleLevel.GetValueOrDefault(), // fine
				UsePyramidNumbers = UsePyramidNumbers.GetValueOrDefault(), // fine
				PyramidNumberIncreaseLimit = PyramidNumberIncreaseLimit ?? 50, // fine
				AvailableAt = AvailableAt,
				PreventNaming = PreventNaming.GetValueOrDefault(), // fine
				CssClasses = CssClasses,
				QEffectPriority = QEffectPriority ?? 2, // fine, this is unused anyway
				QEffectMinimalLimit = new Il2CppSystem.Nullable<int>(0),
				World = World,
				Ordering = Ordering.GetValueOrDefault(), // fine
				IsSlot = IsSlot.GetValueOrDefault(), // fine
				LimitedToArea = LimitedToArea,
				AssignToSlot = AssignToSlot,
				ParentQuality = ParentQuality,
				Persistent = Persistent.GetValueOrDefault(), // fine
				Visible = Visible ?? true,
				Enhancements = null, // Enhancements.ToIl2Cpp(), // ensure is not null
				EnhancementsDescription = EnhancementsDescription,
				SecondChanceQuality = SecondChanceQuality,
				UseEvent = UseEvent,
				DifficultyTestType = DifficultyTestType.GetValueOrDefault(), // fine
				DifficultyScaler = DifficultyScaler ?? 60,
				AllowedOn = AllowedOn.GetValueOrDefault(), // fine
				Nature = Nature.GetValueOrDefault(), // fine
				Category = Category.GetValueOrDefault(), // fine
				LevelDescriptionText = LevelDescriptionText,
				ChangeDescriptionText = ChangeDescriptionText,
				DescendingChangeDescriptionText = DescendingChangeDescriptionText,
				LevelImageText = LevelImageText,
				VariableDescriptionText = VariableDescriptionText,
				Name = Name,
				Id = Id
			};

		IFailbetterEquivalent<Quality> IFailbetterEquivalent<Quality>.FromIL2Cpp(Quality quality) => FromIL2Cpp(quality);

		public static QualityNullable FromIL2Cpp(Quality quality) =>
			new QualityNullable()
			{
				QualitiesPossessedList = null, // quality.QualitiesPossessedList.ToManagedList(),
				RelationshipCapable = quality.RelationshipCapable,
				PluralName = quality.PluralName,
				OwnerName = quality.OwnerName,
				Description = quality.Description,
				Image = quality.Image,
				Notes = quality.Notes,
				Tag = quality.Tag,
				//Cap = quality.Cap.ToManaged(),
				CapAdvanced = quality.CapAdvanced,
				HimbleLevel = quality.HimbleLevel,
				UsePyramidNumbers = quality.UsePyramidNumbers,
				PyramidNumberIncreaseLimit = quality.PyramidNumberIncreaseLimit,
				AvailableAt = quality.AvailableAt,
				PreventNaming = quality.PreventNaming,
				CssClasses = quality.CssClasses,
				QEffectPriority = quality.QEffectPriority,
				//QEffectMinimalLimit = quality.QEffectMinimalLimit.ToManaged(),
				World = quality.World,
				Ordering = quality.Ordering,
				IsSlot = quality.IsSlot,
				LimitedToArea = quality.LimitedToArea,
				AssignToSlot = quality.AssignToSlot,
				ParentQuality = quality.ParentQuality,
				Persistent = quality.Persistent,
				Visible = quality.Visible,
				Enhancements = null, // quality.Enhancements.ToManagedList(),
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
		#endregion

		public virtual IList<AspectQPossession> QualitiesPossessedList { get; set; }
        public virtual bool? RelationshipCapable { get; set; }
        public virtual string PluralName { get; set; }
		public virtual string OwnerName { get; set; }
		public virtual string Description { get; set; }
		public virtual string Image { get; set; }
		public virtual string Notes { get; set; }
		public virtual string Tag { get; set; }
		public virtual int? Cap { get; set; }
		public virtual string CapAdvanced { get; set; }
		public virtual int? HimbleLevel { get; set; }
		public virtual bool? UsePyramidNumbers { get; set; }
		public virtual int? PyramidNumberIncreaseLimit { get; set; }
		public virtual string AvailableAt { get; set; }
		public virtual bool? PreventNaming { get; set; }
		public virtual string CssClasses { get; set; }
		public virtual int? QEffectPriority { get; set; }
		public virtual int? QEffectMinimalLimit { get; set; }
		public virtual World World { get; set; }
		public virtual int? Ordering { get; set; }
		public virtual bool? IsSlot { get; set; }
		public virtual Area LimitedToArea { get; set; }
		public virtual Quality AssignToSlot { get; set; }
		public virtual Quality ParentQuality { get; set; }
		public virtual bool? Persistent { get; set; }
		public virtual bool? Visible { get; set; }
		public virtual IList<QEnhancement> Enhancements { get; set; }
		public virtual string EnhancementsDescription { get; set; }
		public virtual Quality SecondChanceQuality { get; set; }
		public virtual Event UseEvent { get; set; }
		public virtual DifficultyTestType? DifficultyTestType { get; set; }
		public virtual int? DifficultyScaler { get; set; }
		public virtual QualityAllowedOn? AllowedOn { get; set; }
		public virtual Nature? Nature { get; set; }
		public virtual Category? Category { get; set; }
		public virtual string LevelDescriptionText { get; set; }
		public virtual string ChangeDescriptionText { get; set; }
		public virtual string DescendingChangeDescriptionText { get; set; }
		public virtual string LevelImageText { get; set; }
		public virtual string VariableDescriptionText { get; set; }
		public string Name { get; set; }
		public int Id { get; set; }
	}
}
