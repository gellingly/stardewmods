using System.Reflection.Emit;
using HarmonyLib;
using StardewValley;

namespace FeedMoreAnimalCrackers;

[HarmonyPatch(typeof(FarmAnimal), nameof(FarmAnimal.dayUpdate))]
public class FarmAnimalDayUpdatePatch
{
    public static void spawnMoreObjects(FarmAnimal animal, Item item, GameLocation location)
    {
        for (int i = 2; i < item.Stack; i++)
        {
            Utility.spawnObjectAround(animal.Tile, (StardewValley.Object)item.getOne(), location);
        }
    }

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        try
        {
            CodeMatcher matcher = new(instructions);
            // Right after setting stack = 2, add additional things to stack if needed
            // This will only run if the hasEatenAnimalCracker=true for the animal
            // TODO: figure out labels or get better compat with the mod that removes golden animal crackers
            matcher
                .MatchStartForward(
                    Utils.CodeMatchLdloc_S(18),
                    new CodeMatch(OpCodes.Ldc_I4_2),
                    new CodeMatch(
                        OpCodes.Callvirt,
                        AccessTools.PropertySetter(typeof(Item), nameof(Item.Stack))
                    )
                )
                .ThrowIfNotMatch(
                    $"Could not find match for 1st set of instructions {nameof(Transpiler)}"
                );
            var itemOperand = matcher.Operand;
            matcher
                .Advance(3)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_S, itemOperand),
                    new CodeInstruction(
                        OpCodes.Call,
                        AccessTools.Method(typeof(Utils), nameof(Utils.setStack))
                    )
                );

            // Go to the part before where it spawns produce that couldn't be fit into a autograbber, and add more spawns
            // Is there a bug when the chest is too full?
            matcher
                .MatchStartForward(
                    Utils.CodeMatchLdloc_S(18),
                    new CodeMatch(OpCodes.Ldc_I4_1),
                    new CodeMatch(
                        OpCodes.Callvirt,
                        AccessTools.PropertySetter(typeof(Item), nameof(Item.Stack))
                    )
                )
                .ThrowIfNotMatch(
                    $"Could not find match for 2nd set of instructions {nameof(Transpiler)}"
                );
            var itemOperand2 = matcher.Operand;
            matcher.Insert(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc_S, itemOperand2),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(
                    OpCodes.Call,
                    AccessTools.Method(
                        typeof(FarmAnimalDayUpdatePatch),
                        nameof(FarmAnimalDayUpdatePatch.spawnMoreObjects)
                    )
                )
            );

            return matcher.InstructionEnumeration();
        }
        catch (Exception ex)
        {
            Utils.harmonyExceptionPrint(ex);
            return instructions;
        }
    }
}
