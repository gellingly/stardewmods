using StardewValley;
using StardewValley.GameData.Crops;
using StardewValley.ItemTypeDefinitions;

namespace CropEncyclopedia;

internal record SeedInfo
{
    public SeedInfo(CropData cd, int fertilizer, HashSet<string> giantCropIds, bool greenhouseLogic)
    {
        cropData = cd;
        daysForGrowth = cd.DaysInPhase.Sum();
        harvestItem = ItemRegistry.GetData(cd.HarvestItemId);
        hasGiantCrop =
            giantCropIds.Contains(harvestItem.ItemId)
            || giantCropIds.Contains(harvestItem.QualifiedItemId);
        category = harvestItem.Category;
        sprite = new SDUISprite(harvestItem);
        seasons = cd.Seasons;
        regrowDays = cd.RegrowDays;
        neededForPerfection =
            !Game1.player.basicShipped.ContainsKey(harvestItem.ItemId)
            && StardewValley.Object.isPotentialBasicShipped(
                harvestItem.QualifiedItemId,
                harvestItem.Category,
                harvestItem.ObjectType
            );
        this.applyFertilizer(fertilizer);
        this.updateHarvestDate(greenhouseLogic);
        price = ItemRegistry.Create(harvestItem.QualifiedItemId).salePrice();
        this.calculateGoldPerDay(greenhouseLogic);
    }

    public void calculateGoldPerDay(bool greenhouseLogic)
    {
        if (greenhouseLogic)
        {
            if (regrowDays == -1)
            {
                goldPerDay = price / (double)daysForGrowth;
            }
            else
            {
                goldPerDay = price / (double)regrowDays;
            }
        }
        else
        {
            if (lastHarvestDate == null)
            {
                goldPerDay = 0;
            }
            else
            {
                goldPerDay =
                    price * harvests / (double)(lastHarvestDate.TotalDays - Game1.Date.TotalDays);
            }
        }
    }

    public virtual float GetFertilizerSpeedBoost(int fertilizer)
    {
        switch (fertilizer)
        {
            case 1: // Speed-Gro
                return 0.1f;
            case 2: // Deluxe Speed-Gro
                return 0.25f;
            case 3: // Hyper Speed-Gro
                return 0.33f;
            default:
                return 0f;
        }
    }

    public void applyFertilizer(int fertilizer)
    {
        var hasFasterGrowthProfession = Game1.player.professions.Contains(5);
        var speedIncrease = 0f;
        if (hasFasterGrowthProfession)
        {
            speedIncrease += 0.1f;
        }
        speedIncrease += GetFertilizerSpeedBoost(fertilizer);

        // Copied from source code
        int daysToRemove = (int)Math.Ceiling((float)daysForGrowth * speedIncrease);
        var phaseDays = this.cropData.DaysInPhase.ToList();
        int tries = 0;
        while (daysToRemove > 0 && tries < 3)
        {
            for (int j = 0; j < phaseDays.Count; j++)
            {
                if ((j > 0 || phaseDays[j] > 1) && phaseDays[j] != 99999 && phaseDays[j] > 0)
                {
                    phaseDays[j]--;
                    daysToRemove--;
                }
                if (daysToRemove <= 0)
                {
                    break;
                }
            }
            tries++;
        }
        daysForGrowth = phaseDays.Sum();
    }

    public void updateHarvestDate(bool greenhouseLogic)
    {
        bool isInSeason(WorldDate date)
        {
            return greenhouseLogic || cropData.Seasons.Contains(date.Season);
        }

        harvestDate = lastHarvestDate = WorldDate.ForDaysPlayed(
            Game1.Date.TotalDays + daysForGrowth
        );
        if (!isInSeason(Game1.Date) || !isInSeason(harvestDate))
        {
            harvestDate = lastHarvestDate = null;
            harvests = 0;
            return;
        }
        harvests = 1;
        if (regrowDays == -1)
        {
            return;
        }
        while (
            isInSeason(lastHarvestDate!)
            && lastHarvestDate!.TotalDays - Game1.Date.TotalDays < 28 * 4
        )
        {
            harvests++;
            lastHarvestDate = WorldDate.ForDaysPlayed(lastHarvestDate!.TotalDays + regrowDays);
        }
    }

    public WorldDate? harvestDate;
    private WorldDate? lastHarvestDate;
    public DateDisplay? harvestDateDisplay => new DateDisplay(harvestDate ?? null);

    public int harvests;

    public bool hasGiantCrop;
    public ParsedItemData harvestItem;
    public CropData cropData;
    public List<Season> seasons;
    public SDUISprite sprite;
    public int price;

    public double goldPerDay;
    public List<SDUISprite> seasonSprites =>
        new List<Season>([Season.Spring, Season.Summer, Season.Fall, Season.Winter])
            .Select(s =>
                seasons.Contains(s) ? CommonSprites.seasonSprites[(int)s] : CommonSprites.NoSeason
            )
            .ToList();
    public int daysForGrowth;
    public int regrowDays;
    public string regrowDaysString => regrowDays == -1 ? "" : regrowDays.ToString();

    public int category;
    public bool neededForPerfection;
}
