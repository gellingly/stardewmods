using System.Reflection.Emit;
using HarmonyLib;
using StardewValley;

namespace FeedMoreAnimalCrackers;

[HarmonyPatch(typeof(FarmAnimal), nameof(FarmAnimal.pet))]
public static class FarmAnimalPetPatch
{
    public static bool? ateAnimalCracker;
    public static bool? wasPet;

    static bool holdingGoldenAnimalCracker(Farmer who)
    {
        return who.ActiveObject?.QualifiedItemId == Utils.AnimalCrackerID;
    }

    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        try
        {
            CodeMatcher matcher = new(instructions);
            // Prevent from opening the animal info menu when you are holding a cracker
            // Old: else if (!is_auto_pet && who.ActiveObject?.QualifiedItemId != "(O)178")
            // New same as above but also && who.ActiveObject?.QualifiedItemId != golden animal cracker
            matcher
                .MatchStartForward(
                    new CodeMatch(OpCodes.Ldstr, "(O)178"),
                    new CodeMatch(
                        OpCodes.Call,
                        AccessTools.Method(
                            typeof(string),
                            "op_Inequality",
                            new[] { typeof(string), typeof(string) }
                        )
                    ),
                    new CodeMatch(OpCodes.Brfalse_S)
                )
                .ThrowIfNotMatch($"Could not find match for {nameof(Transpiler)}");

            var jumpToLabel = matcher.Advance(2).Operand;
            matcher
                .Advance(1)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(
                        OpCodes.Call,
                        AccessTools.Method(
                            typeof(FarmAnimalPetPatch),
                            nameof(holdingGoldenAnimalCracker)
                        )
                    ),
                    new CodeInstruction(OpCodes.Brtrue_S, jumpToLabel)
                );
            return matcher.InstructionEnumeration();
        }
        catch (Exception ex)
        {
            Utils.harmonyExceptionPrint(ex);
            return instructions;
        }
    }

    public static bool Prefix(FarmAnimal __instance, Farmer who, bool is_auto_pet)
    {
        try
        {
            if (ateAnimalCracker != null)
            {
                throw new Exception($"ateAnimalCracker was not null?: {ateAnimalCracker}");
            }
            ateAnimalCracker = __instance.hasEatenAnimalCracker.Value;
            wasPet = __instance.wasPet.Value;
        }
        catch (Exception ex)
        {
            Utils.harmonyExceptionPrint(ex);
        }
        return true;
    }

    public static void Postfix(FarmAnimal __instance, Farmer who, bool is_auto_pet)
    {
        try
        {
            if (ateAnimalCracker == null)
            {
                throw new Exception($"ateAnimalCracker was null?: {ateAnimalCracker}");
            }
            if (
                !is_auto_pet
                && (ateAnimalCracker ?? false)
                && (wasPet ?? false)
                && who.ActiveObject?.QualifiedItemId == Utils.AnimalCrackerID
            )
            {
                if (!__instance.GetAnimalData()?.CanEatGoldenCrackers ?? false)
                {
                    Game1.playSound("cancel", null);
                    __instance.doEmote(8);
                }
                else if (Utils.getAdditionalCrackers(__instance) < 999)
                {
                    Utils.increment(__instance);
                    Game1.playSound("give_gift", null);
                    __instance.doEmote(56);
                    Game1.player.reduceActiveItemByOne();
                }
            }
        }
        catch (Exception ex)
        {
            Utils.harmonyExceptionPrint(ex);
        }
        ateAnimalCracker = null;
        return;
    }
}
