using Netcode;
using StardewValley;
using StardewValley.Enchantments;
using StardewValley.Tools;

public class StackMeleeWeaponPatches
{
    public static void maximumStackSize_Postfix(ref int __result, MeleeWeapon __instance)
    {
        CommonUtils.setMaxStackSize(ref __result, CommonUtils.config.Weapons);
    }

    public static bool listContainsAll(
        NetList<BaseEnchantment, NetRef<BaseEnchantment>> list,
        NetList<BaseEnchantment, NetRef<BaseEnchantment>> otherList
    )
    {
        for (int i = 0; i < list.Count; i++)
        {
            var found = false;
            for (int j = 0; j < otherList.Count; j++)
            {
                if (
                    list[i].GetType() == otherList[j].GetType()
                    && list[i].GetLevel() == otherList[j].GetLevel()
                )
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                return false;
            }
        }
        return true;
    }

    public static void canStackWith_Postfix(ISalable other, ref bool __result, Item __instance)
    {
        try
        {
            if (__instance is MeleeWeapon a && CommonUtils.config.Weapons)
            {
                __result =
                    other is MeleeWeapon b
                    && CommonUtils.commonCompares(__instance, other)
                    && a.appearance.Value == b.appearance.Value
                    && a.enchantments.Count == b.enchantments.Count
                    && listContainsAll(a.enchantments, b.enchantments)
                    && listContainsAll(a.enchantments, b.enchantments);
            }
        }
        catch (Exception ex)
        {
            CommonUtils.harmonyExceptionPrint(ex);
        }
    }
}
