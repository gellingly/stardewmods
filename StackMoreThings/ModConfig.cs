using StardewModdingAPI;

public sealed class ModConfig
{
    public int MaxStackSize { get; set; } = 9999;
    public bool Trinkets { get; set; } = true;
    public bool Rings { get; set; } = true;
    public bool Weapons { get; set; } = true;
    public bool Furniture { get; set; } = true;
    public bool Tackle { get; set; } = true;
    public bool Boots { get; set; } = true;
    public bool Clothing { get; set; } = true;
    public bool Wallpaper { get; set; } = true;
    public bool Hats { get; set; } = true;
    public bool EnableComplexPatches { get; set; } = true;
    public SButton ColorMergeKey { get; set; } = SButton.F10;
    public SButton QualityReduceKey { get; set; } = SButton.F9;
}
