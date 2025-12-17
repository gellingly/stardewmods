using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;

namespace FeedMoreAnimalCrackers;

[HarmonyPatch(typeof(AnimalPage), "drawNPCSlot")]
public static class AnimalPageDraw
{
    public static void Postfix(AnimalPage __instance, SpriteBatch b, int i)
    {
        var entry = __instance.GetSocialEntry(i);
        if (!entry.ReceivedAnimalCracker)
        {
            return;
        }
        var num = Utils.getAdditionalCrackers((FarmAnimal)entry.Animal) + 1;
        if (num == 1)
        {
            return;
        }
        int yOffset = (entry.TextureSourceRect.Height <= 16) ? (-40) : 8;
        Utility.drawTinyDigits(
            num,
            b,
            new Vector2(
                __instance.xPositionOnScreen + 576 - 20,
                __instance.sprites[i].bounds.Y + yOffset + 64 - 16
            ) + new Vector2(64 - Utility.getWidthOfTinyDigitString(num, 3f) + 3f, 32f),
            3f,
            0.8f,
            Color.White
        );
    }
}

[HarmonyPatch(typeof(AnimalQueryMenu), nameof(AnimalQueryMenu.draw))]
public static class AnimalQueryMenuDrawPatch
{
    public static void Postfix(AnimalQueryMenu __instance, SpriteBatch b)
    {
        if (
            __instance.animal != null
            && __instance.animal.hasEatenAnimalCracker.Value
            && Game1.objectSpriteSheet_2 != null
        )
        {
            var num = Utils.getAdditionalCrackers(__instance.animal) + 1;
            if (num == 1)
            {
                return;
            }
            Utility.drawTinyDigits(
                num,
                b,
                new Vector2(
                    __instance.xPositionOnScreen + AnimalQueryMenu.width - 105.6f,
                    __instance.yPositionOnScreen + 224f
                ) + new Vector2(64 - Utility.getWidthOfTinyDigitString(num, 3f) + 3f, 47f),
                3f,
                0.89f,
                Color.White
            );
        }
    }
}

[HarmonyPatch(typeof(PondQueryMenu), nameof(PondQueryMenu.draw))]
public static class PondQueryMenuDrawPatch
{
    public static void Postfix(PondQueryMenu __instance, SpriteBatch b)
    {
        FishPond pond = (FishPond)
            AccessTools.Field(typeof(PondQueryMenu), "_pond").GetValue(__instance)!;
        if (pond.goldenAnimalCracker.Value && Game1.objectSpriteSheet_2 != null)
        {
            var num = Utils.getAdditionalCrackers(pond) + 1;
            if (num == 1)
            {
                return;
            }
            Utility.drawTinyDigits(
                num,
                b,
                new Vector2(
                    __instance.xPositionOnScreen + PondQueryMenu.width - 105.6f,
                    __instance.yPositionOnScreen + 224f
                ) + new Vector2(64 - Utility.getWidthOfTinyDigitString(num, 3f) + 3f, 47f),
                3f,
                0.89f,
                Color.White
            );
        }
    }
}
