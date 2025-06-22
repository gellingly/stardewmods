using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.ItemTypeDefinitions;

namespace CropEncyclopedia;

internal static class CommonSprites
{
    public static List<SDUISprite> seasonSprites =>
        new List<Season>([Season.Spring, Season.Summer, Season.Fall, Season.Winter])
            .Select(s => new SDUISprite(
                Game1.mouseCursors,
                new Rectangle(406, 441 + ((int)s) * 8, 12, 8)
            ))
            .ToList();
    public static SDUISprite NoSeason = new SDUISprite(
        null,
        new Rectangle(406, 441 + 0 * 8, 12, 8)
    );
}

internal class DateDisplay(WorldDate? date)
{
    public WorldDate? date { get; set; } = date;
    public SDUISprite seasonSprite =>
        date != null ? CommonSprites.seasonSprites[(int)this.date.Season] : CommonSprites.NoSeason;
    public string day => date != null ? $"{date.DayOfMonth}" : "";
}

internal record SDUISprite(Texture2D Texture, Rectangle SourceRect)
{
    public SDUISprite(Texture2D Texture)
        : this(Texture, Texture.Bounds) { }

    public SDUISprite(ParsedItemData itemData)
        : this(itemData.GetTexture(), itemData.GetSourceRect()) { }
};
