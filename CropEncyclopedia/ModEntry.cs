using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewUI.Framework;
using StardewValley;

namespace CropEncyclopedia;

internal sealed class ModEntry : Mod
{
    private IModHelper helper;
    private ModConfig Config;
    public static IViewEngine viewEngine;

    public override void Entry(IModHelper helper)
    {
        Utils.Monitor = this.Monitor;
        this.Config = this.Helper.ReadConfig<ModConfig>();
        helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
        helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        this.helper = helper;
        var harmony = new Harmony(this.ModManifest.UniqueID);
        harmony.PatchAll(typeof(ModEntry).Assembly);
    }

    private void GameLoop_GameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        viewEngine = Helper.ModRegistry.GetApi<IViewEngine>("focustense.StardewUI")!;
        viewEngine.RegisterViews("gellingly.CropEncyclopedia", "assets/views");
        // viewEngine.EnableHotReloadingWithSourceSync();
    }

    private void Input_ButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (Context.IsPlayerFree && e.Button == Config.KeyBinding)
        {
            var context = Retain.getCropEncyclopediaData();
            var menu = viewEngine.CreateMenuControllerFromAsset(
                "gellingly.CropEncyclopedia/CropEncyclopedia",
                context
            );
            menu.EnableCloseButton();
            Game1.activeClickableMenu = menu.Menu;
        }
    }
}

internal static class Utils
{
    public static IMonitor Monitor;

    public static void Log(string s)
    {
        Monitor.Log(s, LogLevel.Debug);
    }
}
