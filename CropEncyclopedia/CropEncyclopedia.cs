using System.ComponentModel;
using PropertyChanged.SourceGenerator;
using StardewValley;
using StardewValley.GameData.Crops;

namespace CropEncyclopedia;

internal static class Retain
{
    static CropEncyclopediaData? cropEncyclopediaData = null;

    public static CropEncyclopediaData getCropEncyclopediaData()
    {
        if (cropEncyclopediaData == null)
        {
            cropEncyclopediaData = new CropEncyclopediaData();
            cropEncyclopediaData.PropertyChanged += new PropertyChangedEventHandler(
                cropEncyclopediaData.SpecialPropertyChangedHandler
            );
        }
        cropEncyclopediaData.DoRefresh();
        return cropEncyclopediaData;
    }
}

internal partial class CropEncyclopediaData : INotifyPropertyChanged
{
    public IEnumerable<SeedInfo> _seedInfos { get; set; } = [];

    public IEnumerable<SeedInfo[]> seedInfos
    {
        get
        {
            return _seedInfos
                .Where(s =>
                    s.harvestItem.DisplayName.ToLower().Contains(SearchString.ToLower())
                    && (Season == 0 || s.seasons.Contains((Season)(Season - 1)))
                    && (
                        Regrowth == 0
                        || (Regrowth == 1 && s.regrowDays == -1)
                        || (Regrowth == 2 && s.regrowDays != -1)
                    )
                    && (
                        Category == 0
                        || (Category != 1 && s.harvestItem.Category == Category)
                        || (Category == 1 && !categoryOptions.Contains(s.harvestItem.Category))
                    )
                    && (!NeededForPerfection || s.neededForPerfection)
                    && (!WontDie || s.harvestDate != null)
                    && (!GiantCrop || s.hasGiantCrop)
                )
                .Chunk(itemsPerPage);
        }
    }

    public SeedInfo[] currentPageSeedInfo => seedInfos.ElementAtOrDefault(PageIndex - 1) ?? [];
    public int itemsPerPage = 25;

    public int pageCount => seedInfos.Count();

    [Notify]
    public int pageIndex { get; set; } = 1;

    public bool hasNextPage => PageIndex < pageCount;

    public bool hasPrevPage => PageIndex > 1;

    public void nextPage()
    {
        if (hasNextPage)
        {
            PageIndex++;
        }
    }

    public void prevPage()
    {
        if (hasPrevPage)
        {
            PageIndex--;
        }
    }

    [Notify]
    public string searchString { get; set; } = "";

    [Notify]
    public bool neededForPerfection { get; set; } = false;

    [Notify]
    public bool wontDie { get; set; } = false;

    [Notify]
    public bool giantCrop { get; set; } = false;

    [Notify]
    public bool greenhouseLogic { get; set; } = false;

    public string SortByValue = "Name";
    public bool SortReverse = false;

    public void doSort()
    {
        _seedInfos = _seedInfos.OrderBy<SeedInfo, object>(s =>
            SortByValue switch
            {
                "Name" => s.harvestItem.DisplayName,
                "Gold" => -s.price,
                "Gold/day" => -s.goldPerDay,
                "Growth days" => s.daysForGrowth,
                "Regrowth days" => s.regrowDays == -1 ? 99999 : s.regrowDays,
                "Harvests" => -s.harvests,
                "Harvest day" => s.harvestDate?.TotalDays ?? 99999,
                "Perfection" => s.neededForPerfection ? 0 : 1,
                _ => s.harvestItem.DisplayName,
            }
        );
        if (SortReverse)
        {
            _seedInfos = _seedInfos.Reverse();
        }
    }

    public void sortBy(string value)
    {
        if (value == SortByValue)
        {
            SortReverse = !SortReverse;
            _seedInfos = _seedInfos.Reverse();
        }
        else
        {
            SortByValue = value;
            SortReverse = false;
            doSort();
        }
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_seedInfos)));
    }

    [Notify]
    public int season = 0;
    public int[] seasonOptions = [0, 1, 2, 3, 4];
    public Func<int, string> SeasonFormat { get; } =
        s =>
            s switch
            {
                0 => "All Seasons",
                1 => "Spring",
                2 => "Summer",
                3 => "Fall",
                _ => "Winter",
            };

    [Notify]
    public int regrowth = 0;
    public int[] regrowthOptions = [0, 1, 2];
    public Func<int, string> RegrowthFormat { get; } =
        mh =>
            mh switch
            {
                0 => "Both",
                1 => "Single Harvest",
                _ => "Regrows",
            };

    [Notify]
    public int category = 0;
    public int[] categoryOptions =
    [
        0,
        StardewValley.Object.VegetableCategory,
        StardewValley.Object.FruitsCategory,
        StardewValley.Object.flowersCategory,
        1,
    ];
    public Func<int, string> CategoryFormat { get; } =
        wt =>
            wt switch
            {
                0 => "All",
                StardewValley.Object.VegetableCategory => "Vegetable",
                StardewValley.Object.FruitsCategory => "Fruit",
                StardewValley.Object.flowersCategory => "Flower",
                _ => "Other",
            };

    [Notify]
    public int fertilizer = 0;
    public int[] fertilizerOptions = [0, 1, 2, 3];
    public Func<int, string> FertilizerFormat { get; } =
        wt =>
            wt switch
            {
                0 => "None",
                1 => "Speed-Gro (10%)",
                2 => "Deluxe Speed-Gro (25%)",
                _ => "Hyper Speed-Gro (33%)",
            };

    public event PropertyChangedEventHandler? PropertyChanged;

    public CropEncyclopediaData()
    {
        DoRefresh();
    }

    public void DoRefresh()
    {
        _seedInfos = CropUtils.getAllSeeds(Fertilizer, GreenhouseLogic);
        doSort();
    }

    public Dictionary<string, string[]> DependencyMapping = new Dictionary<string, string[]>
    {
        { "GreenhouseLogic", new[] { nameof(_seedInfos) } },
        { "Fertilizer", new[] { nameof(_seedInfos) } },
        { "_seedInfos", new[] { nameof(seedInfos) } },
        { "pageCount", new[] { nameof(hasNextPage), nameof(hasPrevPage) } },
        {
            "seedInfos",
            new[] { nameof(PageIndex), nameof(pageCount), nameof(currentPageSeedInfo) }
        },
    };

    internal void SpecialPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Fertilizer) || e.PropertyName == nameof(GreenhouseLogic))
        {
            _seedInfos = CropUtils.getAllSeeds(Fertilizer, GreenhouseLogic);
            doSort();
        }
        if (e.PropertyName == nameof(seedInfos))
        {
            PageIndex = 1;
        }

        string[] dependencies = DependencyMapping.GetValueOrDefault(e.PropertyName ?? "", []);
        foreach (string depPropName in dependencies)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(depPropName));
        }
    }
}

internal static class CropUtils
{
    public static List<SeedInfo> getAllSeeds(int fertilizer, bool greenhouseLogic)
    {
        var giantCrops = DataLoader
            .GiantCrops(Game1.content)
            .Values.Select(item => item.FromItemId)
            .ToHashSet();

        var items = ItemRegistry
            .ItemTypes.Single(type => type.Identifier == ItemRegistry.type_object)
            .GetAllIds()
            .Select(id => ItemRegistry.GetDataOrErrorItem(id))
            .Where(data => data.Category == StardewValley.Object.SeedsCategory)
            .ToArray();

        List<SeedInfo> seedInfo = [];

        foreach (CropData cd in Game1.cropData.Values)
        {
            seedInfo.Add(new SeedInfo(cd, fertilizer, giantCrops, greenhouseLogic));
        }
        return seedInfo;
    }
}
