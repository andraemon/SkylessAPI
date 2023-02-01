using Failbetter.Core;
using SkylessAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylessAPI.NullableIntermediaries
{
    public class SettingNullable : IFailbetterEquivalent<Setting>
    {
		#region Interface Methods
		public Setting ToIL2Cpp() =>
			new Setting()
			{
				World = World,
				OwnerName = OwnerName,
				Personae = null, // Personae.ToIl2Cpp(),
				StartingArea = StartingArea,
				StartingDomicile = StartingDomicile,
				ItemsUsableHere = ItemsUsableHere.GetValueOrDefault(),
				Exchange = Exchange,
				TurnLengthSeconds = TurnLengthSeconds.GetValueOrDefault(),
				MaxActionsAllowed = MaxActionsAllowed ?? 1000,
				MaxCardsAllowed = MaxCardsAllowed.GetValueOrDefault(),
				ActionsInPeriodBeforeExhaustion = ActionsInPeriodBeforeExhaustion.GetValueOrDefault(),
				Description = Description,
				Name = Name,
				Id = Id
			};

		IFailbetterEquivalent<Setting> IFailbetterEquivalent<Setting>.FromIL2Cpp(Setting setting) => FromIL2Cpp(setting);

		public static SettingNullable FromIL2Cpp(Setting setting) =>
			new SettingNullable()
			{
				World = setting.World,
				OwnerName = setting.OwnerName,
				Personae = null, // setting.Personae.ToManagedList(),
				StartingArea = setting.StartingArea,
				StartingDomicile = setting.StartingDomicile,
				ItemsUsableHere = setting.ItemsUsableHere,
				Exchange = setting.Exchange,
				TurnLengthSeconds = setting.TurnLengthSeconds,
				MaxActionsAllowed = setting.MaxActionsAllowed,
				MaxCardsAllowed = setting.MaxCardsAllowed,
				ActionsInPeriodBeforeExhaustion = setting.ActionsInPeriodBeforeExhaustion,
				Description = setting.Description,
				Name = setting.Name,
				Id = setting.Id
			};
		#endregion

		public virtual World World { get; set; }
		public virtual string OwnerName { get; set; }
		public virtual IList<Persona> Personae { get; set; }
		public virtual Area StartingArea { get; set; }
		public virtual Domicile StartingDomicile { get; set; }
		public virtual bool? ItemsUsableHere { get; set; }
		public virtual Exchange Exchange { get; set; }
		public virtual int? TurnLengthSeconds { get; set; }
		public virtual int? MaxActionsAllowed { get; set; }
		public virtual int? MaxCardsAllowed { get; set; }
		public virtual int? ActionsInPeriodBeforeExhaustion { get; set; }
		public virtual string Description { get; set; }
		public string Name { get; set; }
		public int Id { get; set; }
	}
}
