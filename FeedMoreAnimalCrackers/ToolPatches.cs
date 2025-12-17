using System.Reflection.Emit;
using HarmonyLib;
using StardewValley;
using StardewValley.Tools;

namespace FeedMoreAnimalCrackers;

[HarmonyPatch(typeof(MilkPail), nameof(MilkPail.beginUsing))]
public class MilkPailBeginUsingPatch
{
    static int getStackSize(MilkPail milkPail)
    {
        return 2 + Utils.getAdditionalCrackers(milkPail.animal);
    }

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher matcher = new(instructions);
        matcher
            .MatchStartForward(
                new CodeMatch(OpCodes.Ldc_I4_2),
                new CodeMatch(OpCodes.Ldc_I4_0),
                new CodeMatch(
                    OpCodes.Callvirt,
                    AccessTools.Method(
                        typeof(Farmer),
                        nameof(Farmer.couldInventoryAcceptThisItem),
                        new[] { typeof(string), typeof(int), typeof(int) }
                    )
                )
            )
            .ThrowIfNotMatch(
                $"Could not find match for 1st set of instructions {nameof(Transpiler)}"
            );
        var label = matcher.Labels;
        matcher
            .RemoveInstruction()
            .Insert(
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(label),
                new CodeInstruction(
                    OpCodes.Call,
                    AccessTools.Method(
                        typeof(MilkPailBeginUsingPatch),
                        nameof(MilkPailBeginUsingPatch.getStackSize)
                    )
                )
            );

        return matcher.InstructionEnumeration();
    }
}

[HarmonyPatch(typeof(MilkPail), nameof(MilkPail.DoFunction))]
public class MilkPailDoFunctionPatch
{
    static void setStackMilkPail(MilkPail milkPail, Item item)
    {
        Utils.setStack(milkPail.animal, item);
    }

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher matcher = new(instructions);
        // Right after setting stack = 2, add additional things to stack if needed
        // This will only run if the hasEatenAnimalCracker=true for the animal
        // TODO: figure out labels or get better compat with the mod that removes golden animal crackers
        matcher
            .MatchEndForward(
                new CodeMatch(OpCodes.Ldloc_0),
                new CodeMatch(OpCodes.Ldc_I4_2),
                new CodeMatch(
                    OpCodes.Callvirt,
                    AccessTools.PropertySetter(typeof(Item), nameof(Item.Stack))
                )
            )
            .ThrowIfNotMatch(
                $"Could not find match for 1st set of instructions {nameof(Transpiler)}"
            )
            .Advance(1)
            .Insert(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(
                    OpCodes.Call,
                    AccessTools.Method(typeof(MilkPailDoFunctionPatch), nameof(setStackMilkPail))
                )
            );

        return matcher.InstructionEnumeration();
    }
}

[HarmonyPatch(typeof(Shears), nameof(Shears.DoFunction))]
public class ShearsDoFunctionPatch
{
    static void setStackShears(Shears shears, Item item)
    {
        Utils.setStack(shears.animal, item);
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
                .MatchEndForward(
                    new CodeMatch(OpCodes.Ldloc_0),
                    new CodeMatch(OpCodes.Ldc_I4_2),
                    new CodeMatch(
                        OpCodes.Callvirt,
                        AccessTools.PropertySetter(typeof(Item), nameof(Item.Stack))
                    )
                )
                .Advance(1)
                .ThrowIfNotMatch(
                    $"Could not find match for 1st set of instructions {nameof(Transpiler)}"
                )
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        AccessTools.Method(typeof(ShearsDoFunctionPatch), nameof(setStackShears))
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
