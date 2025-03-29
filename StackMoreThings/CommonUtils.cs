using System.Runtime.CompilerServices;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Enchantments;

namespace StackMoreThings;

static class CommonUtils
{
    public static ModConfig config;
    public static IMonitor monitor;

    public static bool commonCompares(Item a, ISalable b)
    {
        return a != null
            && b != null
            && a.QualifiedItemId == b.QualifiedItemId
            && a.GetType() == b.GetType()
            && a.DisplayName == b.DisplayName
            && a.getDescription() == b.getDescription();
    }

    public static void setMaxStackSize(
        ref int __result,
        bool shouldSet,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0
    )
    {
        try
        {
            if (shouldSet)
            {
                __result = config.MaxStackSize;
                return;
            }
        }
        catch (Exception ex)
        {
            monitor.Log($"Failed in {filePath}:{memberName}:{lineNumber}:\n{ex}", LogLevel.Error);
        }
    }

    public static void harmonyExceptionPrint(
        Exception ex,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0
    )
    {
        monitor.Log($"Failed in {filePath}:{memberName}:{lineNumber}:\n{ex}", LogLevel.Error);
    }

    private static bool enchantListContainsAll(
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

    public static bool equalEnchantLists(
        NetList<BaseEnchantment, NetRef<BaseEnchantment>> list,
        NetList<BaseEnchantment, NetRef<BaseEnchantment>> otherList
    )
    {
        return list.Count == otherList.Count
            && enchantListContainsAll(list, otherList)
            && enchantListContainsAll(otherList, list);
    }

    public static void log(string s)
    {
        monitor.Log(s);
    }
}
