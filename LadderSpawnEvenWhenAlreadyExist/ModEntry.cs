using System.Numerics;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Locations;
using xTile.Layers;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace LadderSpawnEvenWhenAlreadyExist
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        private ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.World.ObjectListChanged += this.OnObjectListChanged;
        }

        private HashSet<Vector2> getLadders(MineShaft mine)
        {
            HashSet<Vector2> ladders = new HashSet<Vector2>();
            Layer layer = mine.map.RequireLayer("Buildings");

            for (int x = 0; x < layer.LayerWidth; x++)
            {
                for (int y = 0; y < layer.LayerHeight; y++)
                {
                    int tileIndex = layer.GetTileIndexAt(x, y);
                    if (tileIndex == 174 || tileIndex == 173)
                    {
                        ladders.Add(new Vector2(x, y));
                    }
                }
            }

            return ladders;
        }

        private bool closeToExistingLadder(HashSet<Vector2> ladders, Vector2 position)
        {
            int range = 7;

            for (int x = (int)position.X - range; x < (int)position.X + range + 1; x++)
            {
                for (int y = (int)position.Y - range; y < (int)position.Y + range + 1; y++)
                {
                    if (ladders.Contains(new Vector2(x, y)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void OnObjectListChanged(object? sender, ObjectListChangedEventArgs e)
        {
            // this.Monitor.Log(
            //     $"Added {e.Added.Count()} Removed {e.Removed.Count()}",
            //     LogLevel.Debug
            // );

            if (e.IsCurrentLocation && e.Location is MineShaft mine)
            {
                int stonesLeftBeforeBreaking = mine.stonesLeftOnThisLevel;
                foreach (KeyValuePair<Vector2, StardewValley.Object> r in e.Removed)
                {
                    if (r.Value.Name == "Stone")
                    {
                        // Don't make another ladder if one spawned when thse
                        // broke
                        int tileIndex = mine.map.GetTileIndexAt(
                            (int)r.Key.X,
                            (int)r.Key.Y,
                            "Buildings"
                        );
                        if (tileIndex == 174 || tileIndex == 173)
                        {
                            // this.Monitor.Log("  Stone had a ladder", LogLevel.Debug);
                            return;
                        }
                        // Count the number of stones on this level prior to
                        // getting removed
                        stonesLeftBeforeBreaking++;
                    }
                }
                HashSet<Vector2> ladders = getLadders(mine);

                // this.Monitor.Log(
                //     $"  {ladders} ladders, {stonesLeftBeforeBreaking} stones left",
                //     LogLevel.Debug
                // );
                // Treat ladders as unbroken stone to prevent too many ladders
                // from spawning when there are very few stones left
                stonesLeftBeforeBreaking += ladders.Count();

                foreach (KeyValuePair<Vector2, StardewValley.Object> r in e.Removed)
                {
                    if (r.Value.Name == "Stone")
                    {
                        // Copied from game code, but it doesn't really matter
                        // that the spawn rate stay the same
                        stonesLeftBeforeBreaking--;
                        if (closeToExistingLadder(ladders, r.Key))
                        {
                            break;
                        }

                        int farmerLuckLevel = Game1.player.LuckLevel;

                        double chanceForLadderDown =
                            0.02
                            + 1.0 / (double)Math.Max(1, stonesLeftBeforeBreaking)
                            + (double)farmerLuckLevel / 100.0
                            + Game1.player.DailyLuck / 5.0;
                        if (mine.EnemyCount == 0)
                        {
                            chanceForLadderDown += 0.04;
                        }
                        if (Game1.player.hasBuff("dwarfStatue_1"))
                        {
                            chanceForLadderDown *= 1.25;
                        }
                        Random rand = Utility.CreateDaySaveRandom(
                            r.Key.X * 1000,
                            r.Key.Y,
                            mine.mineLevel
                        );
                        rand.NextDouble();

                        if (
                            // !mine.ladderHasSpawned &&
                            !mine.mustKillAllMonstersToAdvance()
                            && (
                                stonesLeftBeforeBreaking == 0
                                || rand.NextDouble() < chanceForLadderDown
                            )
                            && mine.shouldCreateLadderOnThisLevel()
                        )
                        {
                            if (this.Config.PlayRodBend)
                            {
                                mine.playSound("fishingRodBend");
                            }
                            mine.createLadderDown((int)r.Key.X, (int)r.Key.Y);
                            // this.Monitor.Log("  Creating a ladder", LogLevel.Debug);
                            // Don't create more
                            return;
                        }
                    }
                }
            }
        }
    }
}
