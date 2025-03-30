using GenericModConfigMenu;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace AdjustBombSpeed;

internal sealed class ModEntry : Mod
{
    private ModConfig Config;

    // https://www.nexusmods.com/stardewvalley/mods/26476
    private bool HasIridiumBombsInstalled;

    public override void Entry(IModHelper helper)
    {
        this.Config = this.Helper.ReadConfig<ModConfig>();
        this.HasIridiumBombsInstalled = this.Helper.ModRegistry.IsLoaded(
            "ApryllForever.IridiumBombs"
        );
        this.Monitor.Log(
            $"Multipliers: {this.Config.CherryBombMultiplier}, {this.Config.BombMultiplier}, "
                + $"{this.Config.MegaBombMultiplier}, {this.Config.IridiumBombMultiplier}, "
                + $"{this.Config.IridiumClusterBombMultiplier}. ApryllForever.IridiumBombs installed: {this.HasIridiumBombsInstalled}",
            LogLevel.Debug
        );

        this.Helper.Events.GameLoop.GameLaunched += OnGameLaunched;

        Patches.Initialize(this.Monitor, this.Config);

        var harmony = new Harmony(this.ModManifest.UniqueID);

        harmony.Patch(
            original: AccessTools.Method(
                typeof(StardewValley.Object),
                nameof(StardewValley.Object.placementAction)
            ),
            prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.placementAction_Prefix))
        );
        harmony.Patch(
            original: AccessTools.Method(
                typeof(StardewValley.Object),
                nameof(StardewValley.Object.placementAction)
            ),
            postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.placementAction_Postfix))
        );
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
        configMenu.AddParagraph(
            mod: this.ModManifest,
            text: () => this.Helper.Translation.Get("SettingsInstructions")
        );

        configMenu.AddNumberOption(
            mod: this.ModManifest,
            name: () => Game1.content.LoadString("Strings\\Objects:CherryBomb_Name"),
            getValue: () => this.Config.CherryBombMultiplier,
            setValue: value => this.Config.CherryBombMultiplier = value,
            min: 0,
            max: 3,
            interval: 0.1F
        );

        configMenu.AddNumberOption(
            mod: this.ModManifest,
            name: () => Game1.content.LoadString("Strings\\Objects:Bomb_Name"),
            getValue: () => this.Config.BombMultiplier,
            setValue: value => this.Config.BombMultiplier = value,
            min: 0,
            max: 3,
            interval: 0.1F
        );

        configMenu.AddNumberOption(
            mod: this.ModManifest,
            name: () => Game1.content.LoadString("Strings\\Objects:MegaBomb_Name"),
            getValue: () => this.Config.MegaBombMultiplier,
            setValue: value => this.Config.MegaBombMultiplier = value,
            min: 0,
            max: 3,
            interval: 0.1F
        );

        if (HasIridiumBombsInstalled)
        {
            // TODO: Check if Iridium Bombs mod has i18n
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Iridium Bomb",
                getValue: () => this.Config.IridiumBombMultiplier,
                setValue: value => this.Config.IridiumBombMultiplier = value,
                tooltip: () => "From Iridium Bombs (Iridium Expansion to Vanilla Bomb Tier) mod",
                min: 0,
                max: 3,
                interval: 0.1F
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Iridium Cluster Bomb",
                getValue: () => this.Config.IridiumClusterBombMultiplier,
                setValue: value => this.Config.IridiumClusterBombMultiplier = value,
                tooltip: () => "From Iridium Bombs (Iridium Expansion to Vanilla Bomb Tier) mod",
                min: 0,
                max: 3,
                interval: 0.1F
            );
        }
    }
}
