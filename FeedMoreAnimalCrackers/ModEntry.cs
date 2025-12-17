using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;

namespace FeedMoreAnimalCrackers;

internal sealed class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        Utils.monitor = this.Monitor;

        var harmony = new Harmony(this.ModManifest.UniqueID);
        harmony.PatchAll(typeof(ModEntry).Assembly);

        helper.ConsoleCommands.Add(
            "gellingly.remove_crackers",
            "Remove all golden crackers from this animal or fish pond.  Only works if the animal or fish pond menu is open",
            (a, b) => RemoveCrackers()
        );
    }

    public static void RemoveCrackers()
    {
        var activeMenu = Game1.activeClickableMenu;
        if (activeMenu is not AnimalQueryMenu && activeMenu is not PondQueryMenu)
        {
            Utils.monitor.Log(
                "The current menu is not an animal or pond query menu, doing nothing",
                LogLevel.Info
            );
            return;
        }

        bool toReturn = false;
        IHaveModData? instance = null;

        if (activeMenu is AnimalQueryMenu aqm)
        {
            var animal = aqm.animal;
            toReturn = animal.hasEatenAnimalCracker.Value;
            animal.hasEatenAnimalCracker.Value = false;
            instance = animal;
        }
        if (activeMenu is PondQueryMenu pqm)
        {
            FishPond pond = (FishPond)
                AccessTools.Field(typeof(PondQueryMenu), "_pond").GetValue(pqm)!;
            toReturn = pond.goldenAnimalCracker.Value;
            pond.goldenAnimalCracker.Value = false;
            instance = pond;
        }
        if (toReturn)
        {
            Utils.monitor.Log($"Found a base cracker", LogLevel.Info);
            Game1.player.addItemToInventory(ItemRegistry.Create(Utils.AnimalCrackerID, 1));
        }
        var additionalCrackers = Utils.getAdditionalCrackers(instance!);
        Utils.monitor.Log($"Found {additionalCrackers} additional crackers", LogLevel.Info);
        Utils.setZero(instance!);
        if (additionalCrackers > 0)
        {
            Game1.player.addItemToInventory(
                ItemRegistry.Create(Utils.AnimalCrackerID, additionalCrackers)
            );
        }
    }
}
