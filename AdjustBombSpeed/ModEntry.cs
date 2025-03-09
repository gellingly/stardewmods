using GenericModConfigMenu;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Monsters;

namespace AdjustBombSpeed
{
    internal sealed class ModEntry : Mod
    {
        private ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            this.Monitor.Log($"Bomb multiplier: {this.Config.Multiplier}");

            this.Helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            Patches.Initialize(this.Monitor, this.Config);

            var harmony = new Harmony(this.ModManifest.UniqueID);

            // Probably not good practice to put patches for different class in
            // the same patching class but w/e
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

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("name"),
                getValue: () => this.Config.Multiplier,
                setValue: value => this.Config.Multiplier = value,
                tooltip: () => this.Helper.Translation.Get("tooltip"),
                min: 0,
                max: 3,
                interval: 0.1F
            );
        }
    }
}
