using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buffs;
using StardewValley.Locations;
using StardewValley.Menus;

namespace StackableMonsterMuskEffects;

internal sealed class ModEntry : Mod
{
    private ModConfig Config;

    public override void Entry(IModHelper helper)
    {
        Config = Helper.ReadConfig<ModConfig>();
        Common.config = Config;
        Common.monitor = Monitor;
        Common.helper = helper;
        var harmony = new Harmony(ModManifest.UniqueID);
        harmony.PatchAll(typeof(ModEntry).Assembly);
    }
}

public static class Common
{
    public static ModConfig config;
    public static IMonitor monitor;
    public static IModHelper helper;
    public static List<int> times = new();

    public static void HarmonyExceptionPrint(
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
        monitor.Log(s, LogLevel.Trace);
    }

    public static int GetBuffAmount()
    {
        if (config.Multiplicative)
        {
            int buffAmount = 1;
            for (int i = 0; i < times.Count; i++)
            {
                buffAmount *= 2;
            }
            return buffAmount;
        }
        else
        {
            return times.Count + 1;
        }
    }

    public static double AdjustMonsterChance(double monsterChance)
    {
        int buffAmount = GetBuffAmount();
        if (buffAmount > 0)
        {
            monsterChance = monsterChance / 2 * buffAmount;
            Log(
                $"Monster Musk count={times.Count}, buffAmount={buffAmount}, monsterChance={monsterChance}"
            );
        }
        return monsterChance;
    }
}

[HarmonyPatch(typeof(BuffManager), nameof(BuffManager.Apply))]
public class ApplyMonsterMusk
{
    public static void Postfix(Buff buff)
    {
        try
        {
            if (buff.id == "24")
            {
                Common.times.Add(buff.millisecondsDuration);
                Common.Log($"Applying Monster Musk, count={Common.times.Count}");
            }
        }
        catch (Exception ex)
        {
            Common.HarmonyExceptionPrint(ex);
        }
    }
}

[HarmonyPatch(typeof(Buff), nameof(Buff.update))]
public class UpdateCountdown
{
    public static void Postfix(GameTime time, Buff __instance)
    {
        try
        {
            if (!Game1.shouldTimePass())
            {
                return;
            }
            if (__instance.id == "24")
            {
                for (int i = 0; i < Common.times.Count; i++)
                {
                    Common.times[i] -= time.ElapsedGameTime.Milliseconds;
                }
                var previousCount = Common.times.Count;
                Common.times.RemoveAll(t => t <= 0);
                if (previousCount != Common.times.Count)
                {
                    Common.Log(
                        $"Removed MonsterMusk, expired={previousCount - Common.times.Count}, count={Common.times.Count}"
                    );
                }
            }
        }
        catch (Exception ex)
        {
            Common.HarmonyExceptionPrint(ex);
        }
    }
}

[HarmonyPatch(typeof(MineShaft), "adjustLevelChances")]
public class AdjustLevelChancesMineShaft
{
    public static void Postfix(ref double monsterChance)
    {
        try
        {
            monsterChance = Common.AdjustMonsterChance(monsterChance);
        }
        catch (Exception ex)
        {
            Common.HarmonyExceptionPrint(ex);
        }
    }
}

[HarmonyPatch(typeof(VolcanoDungeon), "adjustLevelChances")]
public class AdjustLevelChancesVolcano
{
    public static void Postfix(ref double monsterChance)
    {
        try
        {
            monsterChance = Common.AdjustMonsterChance(monsterChance);
        }
        catch (Exception ex)
        {
            Common.HarmonyExceptionPrint(ex);
        }
    }
}

[HarmonyPatch(typeof(BuffsDisplay), nameof(BuffsDisplay.draw))]
public class DrawMultiplierInBuffIcon
{
    public static void Postfix(BuffsDisplay __instance, SpriteBatch b)
    {
        try
        {
            Dictionary<ClickableTextureComponent, Buff> buffs =
                (Dictionary<ClickableTextureComponent, Buff>)
                    typeof(BuffsDisplay)
                        .GetField("buffs", BindingFlags.NonPublic | BindingFlags.Instance)!
                        .GetValue(__instance)!;

            foreach (KeyValuePair<ClickableTextureComponent, Buff> pair in buffs)
            {
                if (pair.Value.id != "24")
                {
                    continue;
                }
                Utility.drawTinyDigits(
                    Common.GetBuffAmount(),
                    b,
                    new Vector2(pair.Key.bounds.X, pair.Key.bounds.Y),
                    3f,
                    1f,
                    Color.White
                );
            }
        }
        catch (Exception ex)
        {
            Common.HarmonyExceptionPrint(ex);
        }
    }
}
