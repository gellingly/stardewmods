using StardewValley;
using StardewValley.Objects;

namespace StackMoreThings.Patches;

public class StackWallpaperPatches
{
    public static void maximumStackSize_Postfix(Wallpaper __instance, ref int __result)
    {
        CommonUtils.setMaxStackSize(ref __result, CommonUtils.config.Wallpaper);
    }

    public static void canStackWith_Postfix(ISalable other, ref bool __result, Item __instance)
    {
        try
        {
            if (__instance is Wallpaper && CommonUtils.config.Wallpaper)
            {
                __result = CommonUtils.commonCompares(__instance, other);
            }
        }
        catch (Exception ex)
        {
            CommonUtils.harmonyExceptionPrint(ex);
        }
    }
}
