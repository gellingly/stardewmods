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

    private static float getMultiplier(StardewValley.Object o)
    {
        switch (o.QualifiedItemId)
        {
            case "(O)286":
                return Config.CherryBombMultiplier;
            case "(O)287":
                return Config.BombMultiplier;
            case "(O)288":
                return Config.MegaBombMultiplier;
            case "(O)ApryllForever.IridiumBombsCP_IridiumBomb":
                return Config.IridiumBombMultiplier;
            case "(O)ApryllForever.IridiumBombsCP_IridiumClusterBomb":
                return Config.IridiumClusterBombMultiplier;
            default:
                Monitor.Log(
                    $"How did you get here? Bomb multiplier not found for {o.QualifiedItemId}. Using default multiplier 1.",
                    LogLevel.Debug
                );
                return 1f;
        }
    }

    private static bool isBomb(StardewValley.Object o)
    {
        return (
            o.QualifiedItemId == "(O)286"
            || o.QualifiedItemId == "(O)287"
            || o.QualifiedItemId == "(O)288"
            || o.QualifiedItemId == "(O)ApryllForever.IridiumBombsCP_IridiumBomb"
            || o.QualifiedItemId == "(O)ApryllForever.IridiumBombsCP_IridiumClusterBomb"
        );
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
            if (isBomb(__instance))
            {
                Monitor.Log("Placed bomb");
                foreach (TemporaryAnimatedSprite sprite in Game1.currentLocation.temporarySprites)
                {
                    if (!__state.Contains(sprite) && sprite.bombRadius > 0)
                    {
                        float multiplier = getMultiplier(__instance);
                        Monitor.Log(
                            $"Setting totalNumberOfLoops for bomb sprite with multiplier {multiplier}"
                        );
                        sprite.totalNumberOfLoops = (int)(sprite.totalNumberOfLoops * multiplier);
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
