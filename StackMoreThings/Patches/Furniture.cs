using StardewValley;
using StardewValley.Objects;

namespace StackMoreThings.Patches;

public class StackFurniturePatches
{
    public static void maximumStackSize_Postfix(ref int __result, Furniture __instance)
    {
        CommonUtils.setMaxStackSize(ref __result, CommonUtils.config.Furniture);
    }

    public static void canStackWith_Postfix(ISalable other, ref bool __result, Item __instance)
    {
        try
        {
            if (__instance is Furniture && CommonUtils.config.Furniture)
            {
                __result = CommonUtils.commonCompares(__instance, other);

                if (__instance is StorageFurniture sf1 && other is StorageFurniture sf2)
                {
                    if (sf1.heldItems.Count != 0 || sf2.heldItems.Count != 0)
                    {
                        __result = false;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            CommonUtils.harmonyExceptionPrint(ex);
        }
    }

    public static void placementAction_Postfix(Furniture __instance)
    {
        try
        {
            if (CommonUtils.config.EnableComplexPatches && __instance.Stack > 1)
            {
                var copy = __instance.getOne() as StardewValley.Object;
                copy!.CopyFieldsFrom(__instance);
                copy.Stack = __instance.Stack;
                __instance.Stack = 1;
                Game1.player.ActiveObject = copy;
            }
        }
        catch (Exception ex)
        {
            CommonUtils.harmonyExceptionPrint(ex);
        }
    }
}
