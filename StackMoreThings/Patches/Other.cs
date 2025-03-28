using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace StackMoreThings.Patches;

[HarmonyPatch(typeof(ItemGrabMenu), nameof(ItemGrabMenu.organizeItemsInList))]
public static class StackColorQuality
{
    public static bool Prefix(IList<Item> items)
    {
        try
        {
            mergeColors(items);
            mergeQuality(items);
        }
        catch (Exception ex)
        {
            CommonUtils.harmonyExceptionPrint(ex);
        }
        return true;
    }

    private static List<int> cropCategories =
    [
        StardewValley.Object.FruitsCategory,
        StardewValley.Object.VegetableCategory,
        StardewValley.Object.GreensCategory,
        StardewValley.Object.flowersCategory,
    ];

    private static bool isSButtonDown(SButton sButton)
    {
        Keys key;
        sButton.TryGetKeyboard(out key);
        return Game1.input.GetKeyboardState().IsKeyDown(key);
    }

    public static void mergeColors(IList<Item> items)
    {
        if (!isSButtonDown(CommonUtils.config.ColorMergeKey))
        {
            return;
        }
        Dictionary<string, Color> colorCache = new Dictionary<string, Color>();
        foreach (var i in items)
        {
            if (
                i is StardewValley.Object o
                && cropCategories.Contains(o.Category)
                && o is ColoredObject co
            )
            {
                if (colorCache.ContainsKey(i.QualifiedItemId))
                {
                    co.color.Value = colorCache[i.QualifiedItemId];
                }
                else
                {
                    colorCache[i.QualifiedItemId] = co.color.Value;
                }
            }
        }
    }

    public static void mergeQuality(IList<Item> items)
    {
        if (!isSButtonDown(CommonUtils.config.QualityReduceKey))
        {
            return;
        }
        foreach (Item i in items)
        {
            if (i != null && i?.Quality != null)
            {
                i.Quality = 0;
            }
        }
    }
}

[HarmonyPatch(typeof(TailoringMenu), nameof(TailoringMenu.CraftItem))]
public static class TailoringMenuCraft
{
    public static Item clothingLeftSpot;

    public static bool Prefix(TailoringMenu __instance)
    {
        try
        {
            clothingLeftSpot = __instance.leftIngredientSpot.item.getOne();
            clothingLeftSpot.stack.Value = __instance.leftIngredientSpot.item.stack.Value;
            __instance.leftIngredientSpot.item.stack.Value = 1;
        }
        catch (Exception ex)
        {
            CommonUtils.harmonyExceptionPrint(ex);
        }
        return true;
    }

    public static void Postfix(TailoringMenu __instance)
    {
        try
        {
            if (!CommonUtils.config.EnableComplexPatches)
            {
                return;
            }
            if (clothingLeftSpot != null)
            {
                __instance.leftIngredientSpot.item = clothingLeftSpot;
            }
            else
            {
                CommonUtils.monitor.Log(
                    $"Left ingredient spot was null, prefix didn't run. Did another mod patch TailoringMenu?",
                    LogLevel.Error
                );
            }
            clothingLeftSpot = null;
        }
        catch (Exception ex)
        {
            CommonUtils.harmonyExceptionPrint(ex);
        }
    }
}

[HarmonyPatch(typeof(ForgeMenu), nameof(ForgeMenu.CraftItem))]
public static class ForgeMenuCraft
{
    public static Item forgeLeftSpot;

    public static bool Prefix(ForgeMenu __instance)
    {
        try
        {
            forgeLeftSpot = __instance.leftIngredientSpot.item.getOne();
            forgeLeftSpot.stack.Value = __instance.leftIngredientSpot.item.stack.Value;
            __instance.leftIngredientSpot.item.stack.Value = 1;
        }
        catch (Exception ex)
        {
            CommonUtils.harmonyExceptionPrint(ex);
        }
        return true;
    }

    public static void Postfix(ForgeMenu __instance)
    {
        try
        {
            if (!CommonUtils.config.EnableComplexPatches)
            {
                return;
            }
            if (forgeLeftSpot != null)
            {
                __instance.leftIngredientSpot.item = forgeLeftSpot;
            }
            else
            {
                CommonUtils.monitor.Log(
                    $"Left ingredient spot was null, prefix didn't run. Did another mod patch ForgeMenu?",
                    LogLevel.Error
                );
            }
            forgeLeftSpot = null;
        }
        catch (Exception ex)
        {
            CommonUtils.harmonyExceptionPrint(ex);
        }
    }
}
