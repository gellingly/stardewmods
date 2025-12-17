using HarmonyLib;
using StardewValley;
using StardewValley.Buildings;

namespace FeedMoreAnimalCrackers;

[HarmonyPatch(typeof(FishPond), nameof(FishPond.GetFishProduce))]
public class FishPondGetFishProducePatch
{
    public static void Postfix(FishPond __instance, ref Item __result)
    {
        if (__result != null && __instance.goldenAnimalCracker.Value)
        {
            var originalAmount = __result.Stack / 2;
            __result.Stack += originalAmount * Utils.getAdditionalCrackers(__instance);
        }
    }
}

[HarmonyPatch(typeof(FishPond), nameof(FishPond.performActiveObjectDropInAction))]
public class FishPondPerformActiveObjectDropInActionPatch
{
    public static void Postfix(FishPond __instance, ref bool __result, bool probe, Farmer who)
    {
        if (__result)
        {
            return;
        }
        if (
            who.ActiveObject?.QualifiedItemId == Utils.AnimalCrackerID
            && __instance.goldenAnimalCracker.Value
            && __instance.currentOccupants.Value > 0
        )
        {
            if (probe)
            {
                __result = true;
                return;
            }
            Utils.increment(__instance);
            __instance.isPlayingGoldenCrackerAnimation.Value = true;
            AccessTools
                .Method(typeof(FishPond), "showObjectThrownIntoPondAnimation")
                .Invoke(
                    __instance,
                    new object[]
                    {
                        who,
                        who.ActiveObject,
                        () =>
                        {
                            __instance.isPlayingGoldenCrackerAnimation.Value = false;
                        },
                    }
                );
            who.reduceActiveItemByOne();
            __result = true;
            return;
        }
    }
}
