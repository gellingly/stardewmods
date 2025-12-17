using System.Reflection.Emit;
using HarmonyLib;
using StardewValley;

namespace FeedMoreAnimalCrackers;

[HarmonyPatch(typeof(StardewValley.Object), nameof(StardewValley.Object.DayUpdate))]
public class ObjectDayUpdatePatch
{
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
                    new CodeMatch(
                        OpCodes.Ldfld,
                        AccessTools.Field(
                            typeof(FarmAnimal),
                            nameof(FarmAnimal.hasEatenAnimalCracker)
                        )
                    )
                )
                .ThrowIfNotMatch(
                    $"Could not find match for 1st set of instructions {nameof(Transpiler)}"
                );
            var animalOperand = matcher.Operand;

            matcher
                .MatchStartForward(
                    Utils.CodeMatchLdloc_S(20),
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
                    new CodeInstruction(OpCodes.Ldloc_S, animalOperand),
                    new CodeInstruction(OpCodes.Ldloc_S, itemOperand),
                    new CodeInstruction(
                        OpCodes.Call,
                        AccessTools.Method(typeof(Utils), nameof(Utils.setStack))
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
