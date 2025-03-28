namespace StackMoreThings.Patches;

public class MaxStackSizeGeneral
{
    public static void maximumStackSize_Postfix(ref int __result, StardewValley.Object __instance)
    {
        try
        {
            if (__result == 999)
            {
                __result = CommonUtils.config.MaxStackSize;
            }
        }
        catch (Exception ex)
        {
            CommonUtils.harmonyExceptionPrint(ex);
        }
    }
}
