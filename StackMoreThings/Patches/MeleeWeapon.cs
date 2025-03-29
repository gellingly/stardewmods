using HarmonyLib;
using StardewValley;
using StardewValley.Tools;

namespace StackMoreThings.Patches;

[HarmonyPatch(typeof(MeleeWeapon), nameof(MeleeWeapon.maximumStackSize))]
public static class MeleeWeaponMaxStackSize
{
    public static void Postfix(ref int __result)
    {
        CommonUtils.setMaxStackSize(ref __result, CommonUtils.config.Weapons);
    }
}

[HarmonyPatch(typeof(Item), nameof(Item.canStackWith))]
public static class MeleeWeaponCanStackWith
{
    public static void Postfix(ISalable other, ref bool __result, Item __instance)
    {
        try
        {
            if (__instance is MeleeWeapon a && CommonUtils.config.Weapons)
            {
                __result =
                    other is MeleeWeapon b
                    && CommonUtils.commonCompares(__instance, other)
                    && a.appearance.Value == b.appearance.Value
                    && CommonUtils.equalEnchantLists(a.enchantments, b.enchantments);
            }
        }
        catch (Exception ex)
        {
            CommonUtils.harmonyExceptionPrint(ex);
        }
    }
}
