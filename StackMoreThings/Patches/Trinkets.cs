using StardewValley;
using StardewValley.Objects.Trinkets;

public class StackTrinketPatches
{
    public static void maximumStackSize_Postfix(Trinket __instance, ref int __result)
    {
        CommonUtils.setMaxStackSize(ref __result, CommonUtils.config.Trinkets);
    }

    public static void canStackWith_Postfix(ISalable other, ref bool __result, Item __instance)
    {
        try
        {
            if (__instance is Trinket && CommonUtils.config.Trinkets)
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
