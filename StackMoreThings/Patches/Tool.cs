using HarmonyLib;
using StardewValley;
using StardewValley.Tools;

namespace StackMoreThings.Patches;

[HarmonyPatch(typeof(Tool), nameof(Tool.maximumStackSize))]
public static class ToolStackSize
{
    public static void Postfix(ref int __result, Tool __instance)
    {
        CommonUtils.setMaxStackSize(
            ref __result,
            CommonUtils.config.Tools && __instance.AttachmentSlotsCount == 0
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
            if (__instance is Tool a && CommonUtils.config.Tools && __instance is not MeleeWeapon)
            {
                __result =
                    other is Tool b
                    && CommonUtils.commonCompares(__instance, other)
                    && a.UpgradeLevel == b.UpgradeLevel
                    && CommonUtils.equalEnchantLists(a.enchantments, b.enchantments)
                    && a.AttachmentSlotsCount == 0;
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
    // Allows right click on fishing rod to unstack if they don't have
    // attachment slots
    public static void Postfix(StardewValley.Object o, ref bool __result, Tool __instance)
    {
        try
        {
            if (o == null && __instance.AttachmentSlotsCount == 0)
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
