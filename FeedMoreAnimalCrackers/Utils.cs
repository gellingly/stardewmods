using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace FeedMoreAnimalCrackers;

public static class Utils
{
    public static string ModDataKey = "gellingly.FeedMoreAnimalCrackers";
    public static string AnimalCrackerID = "(O)GoldenAnimalCracker";
    public static IMonitor monitor;
    public static IModHelper helper;

    public static void harmonyExceptionPrint(
        Exception ex,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0
    )
    {
        monitor.Log($"Failed in {filePath}:{memberName}:{lineNumber}:\n{ex}", LogLevel.Error);
    }

    public static void Log(string s)
    {
        monitor.Log(s, LogLevel.Debug);
    }

    public static int getAdditionalCrackers(IHaveModData item)
    {
        if (item.modData.TryGetValue(ModDataKey, out var value))
        {
            return int.Parse(value);
        }
        return 0;
    }

    public static void setStack(IHaveModData animal, Item item)
    {
        item.Stack += getAdditionalCrackers(animal);
    }

    public static CodeMatch CodeMatchLdloc_S(int loc)
    {
        return new CodeMatch(instr =>
            instr.opcode == OpCodes.Ldloc_S
            && instr.operand is LocalBuilder lb
            && lb.LocalIndex == loc
        );
    }

    public static void increment(IHaveModData item)
    {
        int num = 1 + getAdditionalCrackers(item);
        item.modData[ModDataKey] = num.ToString();
    }

    public static void setZero(IHaveModData item)
    {
        item.modData[ModDataKey] = "0";
    }
}
