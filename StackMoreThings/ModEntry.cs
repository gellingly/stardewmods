using GenericModConfigMenu;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace StackMoreThings;

internal sealed class ModEntry : Mod
{
    private ModConfig Config;

    public override void Entry(IModHelper helper)
    {
        this.Config = this.Helper.ReadConfig<ModConfig>();
        CommonUtils.config = this.Config;
        CommonUtils.monitor = this.Monitor;
        CommonUtils.hasHappyHomeDesigner = this.Helper.ModRegistry.IsLoaded(
            "tlitookilakin.HappyHomeDesigner"
        );

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;

        CommonUtils.log("Start patching");
        var harmony = new Harmony(this.ModManifest.UniqueID);
        harmony.PatchAll(typeof(ModEntry).Assembly);
        CommonUtils.log("Finished patching");
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>(
            "spacechase0.GenericModConfigMenu"
        );
        if (configMenu is null)
            return;

        configMenu.Register(
            mod: this.ModManifest,
            reset: () => this.Config = new ModConfig(),
            save: () => this.Helper.WriteConfig(this.Config)
        );
        configMenu.AddTextOption(
            mod: this.ModManifest,
            name: () => this.Helper.Translation.Get("MaxStackSize"),
            tooltip: () => this.Helper.Translation.Get("MaxStackSizeTooltip"),
            getValue: () => this.Config.MaxStackSize.ToString(),
            setValue: value =>
            {
                int stackSize = 9999;
                int.TryParse(value, out stackSize);
                if (stackSize <= 0)
                {
                    stackSize = 9999;
                }
                this.Config.MaxStackSize = stackSize;
            }
        );
        configMenu.AddBoolOption(
            mod: this.ModManifest,
            name: () => this.Helper.Translation.Get("EnableComplexPatches"),
            tooltip: () => this.Helper.Translation.Get("EnableComplexPatchesTooltip"),
            getValue: () => this.Config.EnableComplexPatches,
            setValue: value => this.Config.EnableComplexPatches = value
        );

        configMenu.AddSectionTitle(
            mod: this.ModManifest,
            text: () => this.Helper.Translation.Get("Stacks")
        );

        configMenu.AddParagraph(
            mod: this.ModManifest,
            text: () => this.Helper.Translation.Get("SettingsInstructions")
        );

        Dictionary<string, Func<string>> configNames =
            new()
            {
                {
                    "Tackle",
                    () => Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12858")
                },
                {
                    "Rings",
                    () => Game1.content.LoadString("Strings\\StringsFromCSFiles:Ring.cs.1")
                },
                { "Weapons", () => this.Helper.Translation.Get("Weapons") },
                {
                    "Furniture",
                    () => Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12859")
                },
                {
                    "Boots",
                    () => Game1.content.LoadString("Strings\\StringsFromCSFiles:Boots.cs.12501")
                },
                {
                    "Clothing",
                    () => Game1.content.LoadString("Strings\\StringsFromCSFiles:category_clothes")
                },
                { "Tools", () => this.Helper.Translation.Get("Tools") },
                { "Trinkets", () => Game1.content.LoadString("Strings\\1_6_Strings:Trinket") },
            };
        foreach (var configName in configNames)
        {
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: configNames[configName.Key],
                getValue: () =>
                    this.Config.GetType().GetProperty(configName.Key)?.GetValue(this.Config)
                        as bool?
                    ?? false,
                setValue: value =>
                    this.Config.GetType().GetProperty(configName.Key)?.SetValue(this.Config, value)
            );
        }
        configMenu.AddBoolOption(
            mod: this.ModManifest,
            name: () => this.Helper.Translation.Get("AggressiveTrinketStacking"),
            tooltip: () => this.Helper.Translation.Get("AggressiveTrinketStackingTooltip"),
            getValue: () => this.Config.AggressiveTrinketStacking,
            setValue: value => this.Config.AggressiveTrinketStacking = value
        );
        configMenu.AddSectionTitle(
            mod: this.ModManifest,
            text: () => this.Helper.Translation.Get("Keybinds")
        );
        configMenu.AddKeybind(
            mod: this.ModManifest,
            name: () => this.Helper.Translation.Get("ColorMerge"),
            tooltip: () => this.Helper.Translation.Get("ColorMergeTooltip"),
            getValue: () => this.Config.ColorMergeKey,
            setValue: (value) => this.Config.ColorMergeKey = value
        );
        configMenu.AddKeybind(
            mod: this.ModManifest,
            name: () => this.Helper.Translation.Get("QualityReduce"),
            tooltip: () => this.Helper.Translation.Get("QualityReduceTooltip"),
            getValue: () => this.Config.QualityReduceKey,
            setValue: (value) => this.Config.QualityReduceKey = value
        );
    }
}
