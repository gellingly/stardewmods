using System.Runtime.CompilerServices;
using StardewModdingAPI;
using StardewValley;

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
}
