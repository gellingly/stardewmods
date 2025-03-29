using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace StackMoreThings.Patches;

public class FarmerPatches
{
    private static IMonitor Monitor;
    private static ModConfig Config;

    public static void Initialize(IMonitor monitor, ModConfig config)
    {
        Config = config;
        Monitor = monitor;
    }

    public static bool noStacksInEquipmentIcons(
        List<ClickableComponent> equipmentIcons,
        int x,
        int y
    )
    {
        try
        {
            if (!Config.EnableComplexPatches)
            {
                return true;
            }
            if (
                Game1.player.CursorSlotItem?.Stack > 1
                && equipmentIcons.Any(e => e.containsPoint(x, y))
            )
            {
                Game1.showGlobalMessage("You cannot equip items with a stack size greater than 1.");
                return false;
            }
        }
        catch (Exception ex)
        {
            Monitor.Log($"Failed in {nameof(noStacksInEquipmentIcons)}:\n{ex}", LogLevel.Error);
        }
        return true;
    }

    public static bool receiveLeftClick_Prefix(InventoryPage __instance, int x, int y)
    {
        return noStacksInEquipmentIcons(__instance.equipmentIcons, x, y);
    }

    public static bool receiveLeftClick_Prefix(ForgeMenu __instance, int x, int y)
    {
        return noStacksInEquipmentIcons(__instance.equipmentIcons, x, y);
    }

    public static bool receiveLeftClick_Prefix(TailoringMenu __instance, int x, int y)
    {
        return noStacksInEquipmentIcons(__instance.equipmentIcons, x, y);
    }

    public static void IsValidCraft_Postfix(
        ForgeMenu __instance,
        Item left_item,
        Item right_item,
        ref bool __result
    )
    {
        if (left_item?.Stack > 1)
        {
            __result = false;
            return;
        }
        if (left_item is Ring && right_item is Ring && right_item.Stack > 1)
        {
            __result = false;
            return;
        }
    }

    public static void IsValidCraft_Postfix(
        TailoringMenu __instance,
        Item left_item,
        Item right_item,
        ref bool __result
    )
    {
        if (left_item?.Stack > 1)
        {
            __result = false;
            return;
        }
        if (left_item is Boots && right_item is Boots && right_item.Stack > 1)
        {
            __result = false;
            return;
        }
    }
}
