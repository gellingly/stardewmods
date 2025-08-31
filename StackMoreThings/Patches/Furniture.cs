using HarmonyLib;
using StardewValley;
using StardewValley.Objects;

namespace StackMoreThings.Patches;

[HarmonyPatch(typeof(Furniture), nameof(Furniture.maximumStackSize))]
public class FurnitureMaxStackSize
{
    public static void Postfix(ref int __result)
    {
        CommonUtils.setMaxStackSize(ref __result, CommonUtils.config.Furniture);
    }
}

[HarmonyPatch(typeof(Item), nameof(Item.canStackWith))]
public class FurnitureCanStackWith
{
    public static void Postfix(ISalable other, ref bool __result, Item __instance)
    {
        try
        {
            if (__instance is Furniture && CommonUtils.config.Furniture)
            {
                __result = CommonUtils.commonCompares(__instance, other);
                if (
                    __instance is RandomizedPlantFurniture rpf1
                    && other is RandomizedPlantFurniture rpf2
                )
                {
                    __result &=
                        rpf1.topIndex.Value == rpf2.topIndex.Value
                        && rpf1.middleIndex.Value == rpf2.middleIndex.Value
                        && rpf1.bottomIndex.Value == rpf2.bottomIndex.Value;
                }
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
}

[HarmonyPatch(typeof(Furniture), nameof(Furniture.placementAction))]
public class FurniturePlacementAction
{
    public static void Postfix(Furniture __instance)
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
                if (CommonUtils.hasHappyHomeDesigner)
                {
                    copy.Stack--;
                }
            }
        }
        catch (Exception ex)
        {
            CommonUtils.harmonyExceptionPrint(ex);
        }
    }
}
