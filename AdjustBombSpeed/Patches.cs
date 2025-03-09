using StardewModdingAPI;
using StardewValley;

internal class Patches
{
    private static IMonitor Monitor;
    private static ModConfig Config;

    internal static void Initialize(IMonitor monitor, ModConfig config)
    {
        Monitor = monitor;
        Config = config;
    }

    private static bool isBomb(StardewValley.Object o)
    {
        return o.QualifiedItemId == "(O)287"
            || o.QualifiedItemId == "(O)286"
            || o.QualifiedItemId == "(O)288";
    }

    internal static bool placementAction_Prefix(
        StardewValley.Object __instance,
        out TemporaryAnimatedSpriteList __state
    )
    {
        __state = new TemporaryAnimatedSpriteList();

        if (isBomb(__instance))
        {
            // Save the existing sprites so we can be sure we don't accidentally
            // re process a bomb
            foreach (TemporaryAnimatedSprite sprite in Game1.currentLocation.temporarySprites)
            {
                __state.Add(sprite);
            }
        }

        return true;
    }

    internal static void placementAction_Postfix(
        StardewValley.Object __instance,
        TemporaryAnimatedSpriteList __state
    )
    {
        try
        {
            if (isBomb(__instance) && Game1.shouldTimePass())
            {
                Monitor.Log("Placed bomb");
                foreach (TemporaryAnimatedSprite sprite in Game1.currentLocation.temporarySprites)
                {
                    if (!__state.Contains(sprite) && sprite.bombRadius > 0)
                    {
                        Monitor.Log(
                            $"Setting totalNumberOfLoops for bomb sprite with multiplier {Config.Multiplier}"
                        );
                        sprite.totalNumberOfLoops = (int)(
                            sprite.totalNumberOfLoops * Config.Multiplier
                        );
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Monitor.Log($"Failed in {nameof(placementAction_Postfix)}:\n{ex}", LogLevel.Error);
        }
    }
}
