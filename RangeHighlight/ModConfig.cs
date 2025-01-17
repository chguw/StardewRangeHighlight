﻿// Copyright 2020-2024 Jamie Taylor
using System;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;

namespace RangeHighlight {
    internal enum HighlightActionLocationStyle {
        Never, WhenMouseHidden, Always
    }
    internal enum SprinklerModCompatibilityOption {
        Faster, MoreCompatible
    }
    internal class ModConfig {
        private uint _refreshInterval = 6; // once every 0.1s or so
        public uint RefreshInterval {
            get => _refreshInterval;
            set => _refreshInterval = Math.Clamp(value, 1, 60);
        }
        public bool ShowOverlaps { get; set; } = true;

        public bool ShowJunimoRange { get; set; } = true;
        public bool ShowSprinklerRange { get; set; } = true;
        public bool ShowScarecrowRange { get; set; } = true;
        public bool ShowBeehouseRange { get; set; } = true;
        public bool ShowMushroomLogRange { get; set; } = true;
        public bool ShowBombRange { get; set; } = true;

        public bool HighlightBuildingsOnMouseover { get; set; } = true;
        public HighlightActionLocationStyle HighlightActionLocation { get; set; } = HighlightActionLocationStyle.Always;
        public bool HideAtMouseOnMovement { get; set; } = true;

        public SprinklerModCompatibilityOption SprinklerModCompatibility { get; set; } = SprinklerModCompatibilityOption.MoreCompatible;

        public bool ShowOtherSprinklersWhenHoldingSprinkler { get; set; } = true;
        public bool ShowOtherScarecrowsWhenHoldingScarecrow { get; set; } = true;
        public bool ShowOtherBeehousesWhenHoldingBeehouse { get; set; } = false;
        public bool ShowOtherMushroomLogsWhenHoldingMushroomLog { get; set; } = false;

        public bool showHeldBombRange { get; set; } = true;
        public bool showPlacedBombRange { get; set; } = true;
        public bool hideHeldBombWhenPlacedAreTicking { get; set; } = true;
        public bool showBombInnerRange { get; set; } = false;
        public bool showBombOuterRange { get; set; } = true;
        public KeybindList ShowAllRangesKey { get; set; } = KeybindList.ForSingle(SButton.LeftShift);
        public KeybindList ShowSprinklerRangeKey { get; set; } = KeybindList.ForSingle(SButton.R);
        public KeybindList ShowScarecrowRangeKey { get; set; } = KeybindList.ForSingle(SButton.O); 
        public KeybindList ShowBeehouseRangeKey { get; set; } = KeybindList.ForSingle(SButton.H);
        public KeybindList ShowMushroomLogRangeKey { get; set; } = KeybindList.Parse("None");
        public KeybindList ShowJunimoRangeKey { get; set; } = KeybindList.ForSingle(SButton.J);
        public bool hotkeysToggle { get; set; } = false;

        public Color JunimoRangeTint { get; set; } = Color.White * 0.7f;
        public Color SprinklerRangeTint { get; set; } = new Color(0.6f, 0.6f, 0.9f, 0.7f);
        public Color ScarecrowRangeTint { get; set; } = new Color(0.6f, 1.0f, 0.6f, 0.7f);
        public Color BeehouseRangeTint { get; set; } = new Color(1.0f, 1.0f, 0.6f, 0.7f);
        public Color MushroomLogRangeTint { get; set; } = new Color(32, 93, 150, 178);
        public Color BombRangeTint { get; set; } = new Color(1.0f, 0.5f, 0.5f, 0.6f);
        public Color BombInnerRangeTint { get; set; } = new Color(8.0f, 0.7f, 0.5f, 0.1f);
        public Color BombOuterRangeTint { get; set; } = new Color(9.0f, 0.7f, 0.5f, 0.8f);

        public static void RegisterGMCM(TheMod theMod) {
            var mod = theMod.ModManifest;
            var defaultColorPickerStyle = (uint)(GMCMOptionsAPI.ColorPickerStyle.AllStyles | GMCMOptionsAPI.ColorPickerStyle.RadioChooser);
            var gmcm = theMod.helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (gmcm is null) return;
            gmcm.Register(
                mod: mod,
                reset: () => theMod.config = new ModConfig(),
                save: () => theMod.Helper.WriteConfig(theMod.config),
                titleScreenOnly: false);

            var gmcmOpt = theMod.helper.ModRegistry.GetApi<GMCMOptionsAPI>("jltaylor-us.GMCMOptions");
            if (gmcmOpt is null) {
                theMod.Monitor.Log(I18n.Message_InstallGmcmOptions(theMod.ModManifest.Name), LogLevel.Info);
                gmcm.AddParagraph(mod, I18n.Config_InstallGmcmOptions);
            }

            gmcm.AddNumberOption(
                mod: mod,
                name: I18n.Config_RefreshInterval,
                tooltip: I18n.Config_RefreshInterval_Tooltip,
                getValue: () => (int)theMod.config.RefreshInterval,
                setValue: (v) => theMod.config.RefreshInterval = (uint)v,
                min: 1,
                max: 60
                );
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_HotkeysToggle,
                tooltip: I18n.Config_HotkeysToggle_Tooltip,
                getValue: () => theMod.config.hotkeysToggle,
                setValue: (v) => theMod.config.hotkeysToggle = v);
            gmcm.AddKeybindList(
                mod: mod,
                name: I18n.Config_ShowAllKey,
                tooltip: I18n.Config_ShowAllKey_Tooltip,
                getValue: () => theMod.config.ShowAllRangesKey,
                setValue: (v) => theMod.config.ShowAllRangesKey = v);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_HighlightBuildingOnMouseover,
                tooltip: I18n.Config_HighlightBuildingOnMouseover_Tooltip,
                getValue: () => theMod.config.HighlightBuildingsOnMouseover,
                setValue: (v) => theMod.config.HighlightBuildingsOnMouseover = v);
            gmcm.AddTextOption(
                mod: mod,
                name: I18n.Config_HighlightActionLocation,
                tooltip: I18n.Config_HighlightActionLocation_Tooltip,
                allowedValues: Enum.GetNames<HighlightActionLocationStyle>(),
                formatAllowedValue: (v) => theMod.helper.Translation.Get("config.highlight-action-location-style." + v),
                getValue: () => theMod.config.HighlightActionLocation.ToString(),
                setValue: (v) => theMod.config.HighlightActionLocation = Enum.Parse<HighlightActionLocationStyle>(v));
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_HideAtMouseLocationOnMovement,
                tooltip: I18n.Config_HideAtMouseLocationOnMovement_Tooltip,
                getValue: () => theMod.config.HideAtMouseOnMovement,
                setValue: (v) => theMod.config.HideAtMouseOnMovement = v);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_ShowOverlaps,
                tooltip: I18n.Config_ShowOverlaps_Tooltip,
                getValue: () => theMod.config.ShowOverlaps,
                setValue: (v) => theMod.config.ShowOverlaps = v);

            // Junimo Huts
            gmcm.AddSectionTitle(mod, I18n.Config_Junimo);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_Enable,
                tooltip: I18n.Config_Junimo_Enable_Tooltip,
                getValue: () => theMod.config.ShowJunimoRange,
                setValue: (v) => theMod.config.ShowJunimoRange = v);
            gmcm.AddKeybindList(
                mod: mod,
                name: I18n.Config_Hotkey,
                tooltip: I18n.Config_Junimo_Hotkey_Tooltip,
                getValue: () => theMod.config.ShowJunimoRangeKey,
                setValue: (v) => theMod.config.ShowJunimoRangeKey = v);
            gmcmOpt?.AddColorOption(
                mod: mod,
                name: I18n.Config_Tint,
                tooltip: I18n.Config_Junimo_Tint_Tooltip,
                getValue: () => theMod.config.JunimoRangeTint,
                setValue: (v) => theMod.config.JunimoRangeTint = v,
                colorPickerStyle: defaultColorPickerStyle);


            // Sprinklers
            gmcm.AddSectionTitle(mod, I18n.Config_Sprinkler);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_Enable,
                tooltip: I18n.Config_Sprinkler_Enable_Tooltip,
                getValue: () => theMod.config.ShowSprinklerRange,
                setValue: (v) => theMod.config.ShowSprinklerRange = v);
            gmcm.AddKeybindList(
                mod: mod,
                name: I18n.Config_Hotkey,
                tooltip: I18n.Config_Sprinkler_Hotkey_Tooltip,
                getValue: () => theMod.config.ShowSprinklerRangeKey,
                setValue: (v) => theMod.config.ShowSprinklerRangeKey = v);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_ShowOthers,
                tooltip: I18n.Config_Sprinkler_ShowOthers_Tooltip,
                getValue: () => theMod.config.ShowOtherSprinklersWhenHoldingSprinkler,
                setValue: (v) => theMod.config.ShowOtherSprinklersWhenHoldingSprinkler = v);
            gmcmOpt?.AddColorOption(
                mod: mod,
                name: I18n.Config_Tint,
                tooltip: I18n.Config_Sprinkler_Tint_Tooltip,
                getValue: () => theMod.config.SprinklerRangeTint,
                setValue: (v) => theMod.config.SprinklerRangeTint = v,
                colorPickerStyle: defaultColorPickerStyle);
            gmcm.AddTextOption(
                mod: mod,
                name: I18n.Config_SprinklerModCompatibility,
                tooltip: I18n.Config_SprinklerModCompatibilityTooltip,
                allowedValues: Enum.GetNames<SprinklerModCompatibilityOption>(),
                formatAllowedValue: (v) => theMod.helper.Translation.Get("config.sprinkler-mod-compatibility-option." + v),
                getValue: () => theMod.config.SprinklerModCompatibility.ToString(),
                setValue: (v) => theMod.config.SprinklerModCompatibility = Enum.Parse<SprinklerModCompatibilityOption>(v));


            // Scarecrows
            gmcm.AddSectionTitle(mod, I18n.Config_Scarecrow);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_Enable,
                tooltip: I18n.Config_Scarecrow_Enable_Tooltip,
                getValue: () => theMod.config.ShowScarecrowRange,
                setValue: (v) => theMod.config.ShowScarecrowRange = v);
            gmcm.AddKeybindList(
                mod: mod,
                name: I18n.Config_Hotkey,
                tooltip: I18n.Config_Scarecrow_Hotkey_Tooltip,
                getValue: () => theMod.config.ShowScarecrowRangeKey,
                setValue: (v) => theMod.config.ShowScarecrowRangeKey = v);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_ShowOthers,
                tooltip: I18n.Config_Scarecrow_ShowOthers_Tooltip,
                getValue: () => theMod.config.ShowOtherScarecrowsWhenHoldingScarecrow,
                setValue: (v) => theMod.config.ShowOtherScarecrowsWhenHoldingScarecrow = v);
            gmcmOpt?.AddColorOption(
                mod: mod,
                name: I18n.Config_Tint,
                tooltip: I18n.Config_Scarecrow_Tint_Tooltip,
                getValue: () => theMod.config.ScarecrowRangeTint,
                setValue: (v) => theMod.config.ScarecrowRangeTint = v,
                colorPickerStyle: defaultColorPickerStyle);

            // Beehouses
            gmcm.AddSectionTitle(mod, I18n.Config_Beehouse);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_Enable,
                tooltip: I18n.Config_Beehouse_Enable_Tooltip,
                getValue: () => theMod.config.ShowBeehouseRange,
                setValue: (v) => theMod.config.ShowBeehouseRange = v);
            gmcm.AddKeybindList(
                mod: mod,
                name: I18n.Config_Hotkey,
                tooltip: I18n.Config_Beehouse_Hotkey_Tooltip,
                getValue: () => theMod.config.ShowBeehouseRangeKey,
                setValue: (v) => theMod.config.ShowBeehouseRangeKey = v);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_ShowOthers,
                tooltip: I18n.Config_Beehouse_ShowOthers_Tooltip,
                getValue: () => theMod.config.ShowOtherBeehousesWhenHoldingBeehouse,
                setValue: (v) => theMod.config.ShowOtherBeehousesWhenHoldingBeehouse = v);
            gmcmOpt?.AddColorOption(
                mod: mod,
                name: I18n.Config_Tint,
                tooltip: I18n.Config_Beehouse_Tint_Tooltip,
                getValue: () => theMod.config.BeehouseRangeTint,
                setValue: (v) => theMod.config.MushroomLogRangeTint = v,
                colorPickerStyle: defaultColorPickerStyle);

            // MushroomLogs
            gmcm.AddSectionTitle(mod, I18n.Config_MushroomLog);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_Enable,
                tooltip: I18n.Config_MushroomLog_Enable_Tooltip,
                getValue: () => theMod.config.ShowMushroomLogRange,
                setValue: (v) => theMod.config.ShowMushroomLogRange = v);
            gmcm.AddKeybindList(
                mod: mod,
                name: I18n.Config_Hotkey,
                tooltip: I18n.Config_MushroomLog_Hotkey_Tooltip,
                getValue: () => theMod.config.ShowMushroomLogRangeKey,
                setValue: (v) => theMod.config.ShowMushroomLogRangeKey = v);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_ShowOthers,
                tooltip: I18n.Config_MushroomLog_ShowOthers_Tooltip,
                getValue: () => theMod.config.ShowOtherMushroomLogsWhenHoldingMushroomLog,
                setValue: (v) => theMod.config.ShowOtherMushroomLogsWhenHoldingMushroomLog = v);
            gmcmOpt?.AddColorOption(
                mod: mod,
                name: I18n.Config_Tint,
                tooltip: I18n.Config_MushroomLog_Tint_Tooltip,
                getValue: () => theMod.config.MushroomLogRangeTint,
                setValue: (v) => theMod.config.MushroomLogRangeTint = v,
                colorPickerStyle: defaultColorPickerStyle);

            // Bombs
            gmcm.AddSectionTitle(mod, I18n.Config_Bomb);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_Enable,
                tooltip: I18n.Config_Bomb_Enable_Tooltip,
                getValue: () => theMod.config.ShowBombRange,
                setValue: (v) => theMod.config.ShowBombRange = v);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_Bomb_Held,
                tooltip: I18n.Config_Bomb_Held_Tooltip,
                getValue: () => theMod.config.showHeldBombRange,
                setValue: (v) => theMod.config.showHeldBombRange = v);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_Bomb_Placed,
                tooltip: I18n.Config_Bomb_Placed_Tooltip,
                getValue: () => theMod.config.showPlacedBombRange,
                setValue: (v) => theMod.config.showPlacedBombRange = v);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_Bomb_HideHeldWhenTicking,
                tooltip: I18n.Config_Bomb_HideHeldWhenTicking_Tooltip,
                getValue: () => theMod.config.hideHeldBombWhenPlacedAreTicking,
                setValue: (v) => theMod.config.hideHeldBombWhenPlacedAreTicking = v);
            gmcmOpt?.AddColorOption(
                mod: mod,
                name: I18n.Config_Tint,
                tooltip: I18n.Config_Bomb_Tint_Tooltip,
                getValue: () => theMod.config.BombRangeTint,
                setValue: (v) => theMod.config.BombRangeTint = v,
                colorPickerStyle: defaultColorPickerStyle);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_Bomb_Inner_Enable,
                tooltip: I18n.Config_Bomb_Inner_Enable_Tooltip,
                getValue: () => theMod.config.showBombInnerRange,
                setValue: (v) => theMod.config.showBombInnerRange = v);
            gmcmOpt?.AddColorOption(
                mod: mod,
                name: I18n.Config_Bomb_Inner_Tint,
                tooltip: I18n.Config_Bomb_Inner_Tint_Tooltip,
                getValue: () => theMod.config.BombInnerRangeTint,
                setValue: (v) => theMod.config.BombInnerRangeTint = v,
                colorPickerStyle: defaultColorPickerStyle);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_Bomb_Outer_Enable,
                tooltip: I18n.Config_Bomb_Outer_Enable_Tooltip,
                getValue: () => theMod.config.showBombOuterRange,
                setValue: (v) => theMod.config.showBombOuterRange = v);
            gmcmOpt?.AddColorOption(
                mod: mod,
                name: I18n.Config_Bomb_Outer_Tint,
                tooltip: I18n.Config_Bomb_Outer_Tint_Tooltip,
                getValue: () => theMod.config.BombOuterRangeTint,
                setValue: (v) => theMod.config.BombOuterRangeTint = v,
                colorPickerStyle: defaultColorPickerStyle);



        }
    }
    // See https://github.com/spacechase0/StardewValleyMods/blob/develop/GenericModConfigMenu/IGenericModConfigMenuApi.cs for full API
    public interface GenericModConfigMenuAPI {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);
        void AddSectionTitle(IManifest mod, Func<string> text, Func<string>? tooltip = null);
        void AddParagraph(IManifest mod, Func<string> text);
        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string>? tooltip = null, string? fieldId = null);
        void AddKeybindList(IManifest mod, Func<KeybindList> getValue, Action<KeybindList> setValue, Func<string> name, Func<string>? tooltip = null, string? fieldId = null);
        void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string>? tooltip = null, string[]? allowedValues = null, Func<string, string>? formatAllowedValue = null, string? fieldId = null);
        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string>? tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string>? formatValue = null, string? fieldId = null);
    }
    // See https://github.com/jltaylor-us/StardewGMCMOptions/blob/default/StardewGMCMOptions/IGMCMOptionsAPI.cs
    public interface GMCMOptionsAPI {
        void AddColorOption(IManifest mod, Func<Color> getValue, Action<Color> setValue, Func<string> name,
            Func<string>? tooltip = null, bool showAlpha = true, uint colorPickerStyle = 0, string? fieldId = null);
        #pragma warning disable format
        [Flags]
        public enum ColorPickerStyle : uint {
            Default = 0,
            RGBSliders    = 0b00000001,
            HSVColorWheel = 0b00000010,
            HSLColorWheel = 0b00000100,
            AllStyles     = 0b11111111,
            NoChooser     = 0,
            RadioChooser  = 0b01 << 8,
            ToggleChooser = 0b10 << 8
        }
        #pragma warning restore format
    }
}
