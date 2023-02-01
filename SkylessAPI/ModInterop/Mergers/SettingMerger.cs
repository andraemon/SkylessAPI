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
    internal class SettingMerger : IMerger<Setting>
    {
        private static SettingMerger _instance = null;
        public static SettingMerger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SettingMerger();
                return _instance;
            }
        }

        public void Merge(Setting settingTo, JsonElement settingFrom, int offset)
        {
            var exchange = settingTo.Exchange;
            if (settingFrom.TryGetProperty("Exchange", out var exc))
            {
                if (exc.ValueKind == JsonValueKind.Null)
                    exchange = null;
                else
                    exchange = new Exchange()
                    {
                        Id = exc.Id(offset)
                    };
            }

            settingTo.OwnerName = (string)settingFrom.GetPropertyValueOrDefault("OwnerName", settingTo.OwnerName);
            settingTo.ItemsUsableHere = (bool)settingFrom.GetPropertyValueOrDefault("ItemsUsableHere", settingTo.ItemsUsableHere);
            settingTo.Exchange = exchange;
            settingTo.TurnLengthSeconds = (int)settingFrom.GetPropertyValueOrDefault("TurnLengthSeconds", settingTo.TurnLengthSeconds);
            settingTo.MaxActionsAllowed = (int)settingFrom.GetPropertyValueOrDefault("MaxActionsAllowed", settingTo.MaxActionsAllowed);
            settingTo.MaxCardsAllowed = (int)settingFrom.GetPropertyValueOrDefault("MaxCardsAllowed", settingTo.MaxCardsAllowed);
            settingTo.ActionsInPeriodBeforeExhaustion = (int)settingFrom.GetPropertyValueOrDefault("ActionsInPeriodBeforeExhaustion", settingTo.ActionsInPeriodBeforeExhaustion);
            settingTo.Description = (string)settingFrom.GetPropertyValueOrDefault("Description", settingTo.Description);
            settingTo.Name = (string)settingFrom.GetPropertyValueOrDefault("Name", settingTo.Name);
        }

        public Setting FromJsonElement(JsonElement item, int offset)
        {
            Exchange exchange = null;
            if (item.TryGetProperty("Exchange", out var exc) && exc.ValueKind != JsonValueKind.Null)
                exchange = new Exchange()
                {
                    Id = exc.Id(offset)
                };

            return new Setting() 
            {
                OwnerName = (string)item.GetPropertyValueOrDefault("OwnerName"),
                ItemsUsableHere = (bool)item.GetPropertyValueOrDefault("ItemsUsableHere", false),
                Exchange = exchange,
                TurnLengthSeconds = (int)item.GetPropertyValueOrDefault("TurnLengthSeconds", 0),
                MaxActionsAllowed = (int)item.GetPropertyValueOrDefault("MaxActionsAllowed", 0),
                MaxCardsAllowed = (int)item.GetPropertyValueOrDefault("MaxCardsAllowed", 0),
                ActionsInPeriodBeforeExhaustion = (int)item.GetPropertyValueOrDefault("ActionsInPeriodBeforeExhaustion", 0),
                Description = (string)item.GetPropertyValueOrDefault("Description"),
                Name = (string)item.GetPropertyValueOrDefault("Name"),
                Id = item.Id(offset)
            };
        }
    }
}
