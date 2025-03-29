using HarmonyLib;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace StackMoreThings.Patches;

[HarmonyPatch(typeof(Tool), nameof(Tool.maximumStackSize))]
public static class ToolStackSize
{
    public static void Postfix(ref int __result, Tool __instance)
    {
        CommonUtils.setMaxStackSize(
            ref __result,
            CommonUtils.config.Tools
                && __instance.AttachmentSlotsCount == 0
                && __instance is not MeleeWeapon
        );
    }
}

[HarmonyPatch(typeof(Item), nameof(Item.canStackWith))]
public static class ToolCanStackWith
{
    public static void Postfix(ISalable other, ref bool __result, Item __instance)
    {
        try
        {
            if (
                __instance is Tool a
                && CommonUtils.config.Tools
                && __instance is not MeleeWeapon
                && a.AttachmentSlotsCount == 0
                && CommonUtils.config.EnableComplexPatches
            )
            {
                __result =
                    other is Tool b
                    && CommonUtils.commonCompares(__instance, other)
                    && a.UpgradeLevel == b.UpgradeLevel
                    && CommonUtils.equalEnchantLists(a.enchantments, b.enchantments);
            }
        }
        catch (Exception ex)
        {
            CommonUtils.harmonyExceptionPrint(ex);
        }
    }
}

[HarmonyPatch(typeof(Tool), nameof(Tool.canThisBeAttached), [typeof(StardewValley.Object)])]
public static class FishingRodAttachment
{
    // Allows right click on fishing rod to unstack if it doesn't have
    // attachment slots
    public static void Postfix(StardewValley.Object o, ref bool __result, Tool __instance)
    {
        try
        {
            if (
                o == null
                && __instance.AttachmentSlotsCount == 0
                && CommonUtils.config.EnableComplexPatches
            )
            {
                __result = false;
            }
        }
        catch (Exception ex)
        {
            CommonUtils.harmonyExceptionPrint(ex);
        }
    }
}

[HarmonyPatch(typeof(ItemGrabMenu), nameof(ItemGrabMenu.organizeItemsInList))]
public static class StackToolsWhileSorting
{
    public static bool Prefix(ref IList<Item> items)
    {
        try
        {
            if (!CommonUtils.config.EnableComplexPatches)
            {
                return true;
            }
            // Helps make sure we don't accidentally sort something that
            // shouldn't be sorted?  Copied pretty much directly from game code
            for (int i = 0; i < items.Count; i++)
            {
                Item item = items[i];
                if (item is not Tool || item.getRemainingStackSpace() <= 0)
                {
                    continue;
                }

                for (int j = i + 1; j < items.Count; j++)
                {
                    Item item2 = items[j];
                    if (item.canStackWith(item2))
                    {
                        item2.Stack = item.addToStack(item2);
                        if (item2.Stack == 0)
                        {
                            items.RemoveAt(j);
                            j--;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            CommonUtils.harmonyExceptionPrint(ex);
        }
        return true;
    }
}
