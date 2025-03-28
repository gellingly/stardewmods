using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Tools;

namespace StackMoreThings.Patches;

public class StackTacklePatches
{
    public static void maximumStackSize_Postfix(StardewValley.Object __instance, ref int __result)
    {
        CommonUtils.setMaxStackSize(
            ref __result,
            CommonUtils.config.Tackle && __instance.Category == -22
        );
    }

    public static bool ranPrefix = false;
    public static int totalUses;

    public static bool addToStack_Prefix(Item otherStack, Item __instance)
    {
        ranPrefix = true;
        if (
            !CommonUtils.config.EnableComplexPatches
            || __instance.Category != -22
            || __instance is not StardewValley.Object obj
            || otherStack is not StardewValley.Object otherObject
            || !CommonUtils.config.Tackle
            || __instance.QualifiedItemId != otherStack.QualifiedItemId
        )
        {
            totalUses = 0;
            return true;
        }
        int selfUsesLeft = obj.Stack * FishingRod.maxTackleUses - obj.uses.Value;
        int otherUsesLeft = otherObject.Stack * FishingRod.maxTackleUses - otherObject.uses.Value;
        totalUses = selfUsesLeft + otherUsesLeft;
        return true;
    }

    public static void addToStack_Postfix(Item otherStack, ref int __result, Item __instance)
    {
        try
        {
            if (
                !CommonUtils.config.EnableComplexPatches
                || __instance.Category != -22
                || __instance is not StardewValley.Object obj
                || otherStack is not StardewValley.Object otherObject
                || !CommonUtils.config.Tackle
                || __instance.QualifiedItemId != otherStack.QualifiedItemId
            )
            {
                return;
            }
            if (!ranPrefix)
            {
                CommonUtils.monitor.Log(
                    $"Prefix didn't run when stacking tackles. Another mod interfered? Aborting",
                    LogLevel.Error
                );
                return;
            }
            // easier to think of in terms of uses left
            int stacks = totalUses / FishingRod.maxTackleUses;
            int extraUses = totalUses % FishingRod.maxTackleUses;
            if (extraUses == 0)
            {
                extraUses = FishingRod.maxTackleUses;
            }
            else
            {
                stacks += 1;
            }
            if (stacks > __instance.maximumStackSize())
            {
                __instance.stack.Value = __instance.maximumStackSize();
                obj.uses.Value = FishingRod.maxTackleUses - extraUses;
                __result = stacks - __instance.maximumStackSize();
                otherObject.uses.Value = 0;
            }
            else
            {
                __instance.stack.Value = stacks;
                obj.uses.Value = FishingRod.maxTackleUses - extraUses;
                otherObject.uses.Value = 0;
                __result = 0;
            }
        }
        catch (Exception ex)
        {
            CommonUtils.harmonyExceptionPrint(ex);
        }
        ranPrefix = false;
        return;
    }

    public static void ConsumeStack_Postfix(Item __instance, ref Item? __result)
    {
        try
        {
            if (!CommonUtils.config.EnableComplexPatches)
            {
                return;
            }
            if (__result is StardewValley.Object obj && obj.Category == -22)
            {
                obj.uses.Value = 0;
            }
        }
        catch (Exception ex)
        {
            CommonUtils.harmonyExceptionPrint(ex);
        }
    }

    static List<StardewValley.Object> tacklesOnRod;

    public static void doDoneFishing_Prefix(FishingRod __instance)
    {
        try
        {
            if (!CommonUtils.config.EnableComplexPatches)
            {
                return;
            }
            tacklesOnRod = __instance.GetTackle();
        }
        catch (Exception ex)
        {
            CommonUtils.harmonyExceptionPrint(ex);
        }
    }

    public static void doDoneFishing_Postfix(FishingRod __instance)
    {
        try
        {
            if (!CommonUtils.config.EnableComplexPatches)
            {
                return;
            }
            for (int i = 0; i < tacklesOnRod.Count; i++)
            {
                var tackle = tacklesOnRod[i];
                if (__instance.attachments[i + 1] == null && tackle != null && tackle.Stack > 1)
                {
                    tackle.ConsumeStack(1);
                    tackle.uses.Value = 0;
                    __instance.attachments[i + 1] = tackle;
                    string hudMessage = Game1.parseText(
                        Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14086"),
                        Game1.dialogueFont,
                        384
                    );

                    Game1.hudMessages.RemoveWhere(m => m.message == hudMessage);
                }
            }
        }
        catch (Exception ex)
        {
            CommonUtils.harmonyExceptionPrint(ex);
        }
    }
}
