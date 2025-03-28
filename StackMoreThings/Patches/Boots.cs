using StardewValley;
using StardewValley.Objects;

namespace StackMoreThings.Patches;

public static class StackBootsPatches
{
    public static void maximumStackSize_Postfix(Boots __instance, ref int __result)
    {
        CommonUtils.setMaxStackSize(ref __result, CommonUtils.config.Boots);
    }

    public static void canStackWith_Postfix(ISalable other, ref bool __result, Item __instance)
    {
        try
        {
            if (__instance is Boots && CommonUtils.config.Boots)
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
