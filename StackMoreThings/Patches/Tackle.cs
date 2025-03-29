using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Tools;

namespace StackMoreThings.Patches;

[HarmonyPatch(typeof(StardewValley.Object), nameof(StardewValley.Object.maximumStackSize))]
public static class TackleMaxStackSize
{
    public static void Postfix(StardewValley.Object __instance, ref int __result)
    {
        CommonUtils.setMaxStackSize(
            ref __result,
            CommonUtils.config.Tackle && __instance.Category == -22
        );
    }
}

[HarmonyPatch(typeof(Item), nameof(Item.addToStack))]
public static class TackleAddToStack
{
    public static bool ranPrefix = false;
    public static int totalUses;

    public static bool Prefix(Item otherStack, Item __instance)
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

    public static void Postfix(Item otherStack, ref int __result, Item __instance)
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
}

[HarmonyPatch(typeof(Item), nameof(Item.ConsumeStack))]
public static class TackleConsumeStack
{
    public static void Postfix(ref Item? __result)
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
}

[HarmonyPatch(typeof(FishingRod), nameof(FishingRod.doneFishing))]
public static class TackleDoneFishing
{
    static List<StardewValley.Object> tacklesOnRod;

    public static void Prefix(FishingRod __instance)
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

    public static void Postfix(FishingRod __instance)
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

[HarmonyPatch(typeof(Tool), nameof(Tool.attach))]
public static class ToolAttach
{
    static StardewValley.Object copyTackle(StardewValley.Object o)
    {
        StardewValley.Object newO = (StardewValley.Object)o.getOne();
        newO.uses.Value = o.uses.Value;
        newO.Stack = o.Stack;
        return newO;
    }

    // Prioritize putting tackle into empty tackle spots over stacking.  Prefix
    // figures out if this is the case, postfix applies the change
    static List<StardewValley.Object> desiredTackle = null;
    static bool replaceTackle = false;

    public static bool Prefix(Tool __instance, StardewValley.Object o)
    {
        try
        {
            if (
                __instance is FishingRod
                && o != null
                && CommonUtils.config.EnableComplexPatches
                && __instance.AttachmentSlotsCount == 3
                && o.Category == -22
            )
            {
                desiredTackle = [];
                for (int i = 1; i < __instance.attachments.Count; i++)
                {
                    StardewValley.Object a = __instance.attachments[i];
                    if (a == null)
                    {
                        if (replaceTackle)
                        {
                            desiredTackle.Add(null);
                            continue;
                        }
                        desiredTackle.Add(copyTackle(o));
                        replaceTackle = true;
                        continue;
                    }
                    desiredTackle.Add(copyTackle(a));
                }
            }
        }
        catch (Exception ex)
        {
            CommonUtils.harmonyExceptionPrint(ex);
        }

        return true;
    }

    public static void Postfix(Tool __instance, StardewValley.Object o)
    {
        try
        {
            if (
                __instance is FishingRod
                && CommonUtils.config.EnableComplexPatches
                && __instance.AttachmentSlotsCount == 3
                && replaceTackle
            )
            {
                for (int i = 1; i < __instance.attachments.Count; i++)
                {
                    __instance.attachments[i] = desiredTackle[i - 1];
                }
            }
            desiredTackle = null;
            replaceTackle = false;
        }
        catch (Exception ex)
        {
            CommonUtils.harmonyExceptionPrint(ex);
        }
    }
}
