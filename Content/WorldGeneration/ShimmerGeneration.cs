using Microsoft.Build.FileSystem;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ReLogic.Utilities;
using ShimmerMod.Content.Drink;
using ShimmerMod.Content.Food;
using ShimmerMod.Content.Tiles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using Terraria.WorldBuilding;

namespace ShimmerMod.Content.WorldGeneration
{
    public class ShimmerGeneration : ModSystem
    {

        public static LocalizedText WorldGenShimmerPassMessage { get; private set; }

        public override void SetStaticDefaults()
        {
            WorldGenShimmerPassMessage = Language.GetOrRegister(Mod.GetLocalizationKey($"WorldGen.{nameof(WorldGenShimmerPassMessage)}"));
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int ShimmerIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shimmer"));
            if (ShimmerIndex != -1)
            {
                tasks.Insert(ShimmerIndex + 1, new WorldGenShimmerPass("Shimmer Expansion", 100f));
            }
        }

        //public static bool JustPressed(Keys key)
        //{
        //    return Main.keyState.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key);
        //}

        //public override void PostUpdateWorld()
        //{
        //    if (JustPressed(Keys.D1))
        //    {
        //        TestMethod((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
        //    }
        //    if (JustPressed(Keys.D2))
        //    {
        //        CreateShimmerBiome();
        //    }
        //}

        //private void TestMethod(int x, int y)
        //{
        //    Dust.QuickBox(new Vector2(x, y) * 16, new Vector2(x + 1, y + 1) * 16, 2, Color.YellowGreen, null);

        //    //WorldGen.TileRunner(x - 1, y, WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(2, 8), ModContent.TileType<VoidBlock>());
        //    Point point = new Point(x, y);

        //    WorldUtils.Gen(point, new ShapeBranch(90, 10), Actions.Chain(new GenAction[]
        //    {
        //        //new Modifiers.IsSolid(),
        //        new Actions.SetTile((ushort)ModContent.TileType<VoidBlock>()),
        //        new Actions.SetFrames()
        //    }));
        //    //WorldUtils.Gen(point,
        //    //    new ShapeRunner(
        //    //        strength: 10,  // radius of carve
        //    //        steps: 100,    // length of tunnel
        //    //        velocity: new Vector2D(1f, 0f)
        //    //    ),
        //    //    Actions.Chain(new GenAction[]
        //    //    {
        //    //        new Actions.ClearTile(),  // removes tiles
        //    //        new Actions.SetFrames()   // updates framing
        //    //    }));

        //    //WorldUtils.Gen(point, new ShapeRunner(), Actions.Chain(new GenAction[]
        //    //{
        //    //    //new Modifiers.IsSolid(),
        //    //    new Actions.SetTile((ushort)ModContent.TileType<VoidBlock>()),
        //    //    new Actions.SetFrames()
        //    //}));
        //    Main.NewText("Generated Blocks!");
        //}

        private void SealBiomeBorder(Rectangle biomeBounds)
        {
            for (int x = biomeBounds.Left; x <= biomeBounds.Right; x++)
            {
                for (int y = biomeBounds.Top; y <= biomeBounds.Bottom; y++)
                {
                    if (x == biomeBounds.Left || x == biomeBounds.Right || y == biomeBounds.Top || y == biomeBounds.Bottom)
                    {
                        Tile tile = Main.tile[x, y];

                        if (!tile.HasTile || !Main.tileSolid[tile.TileType])
                        {
                            Point point = new Point(x, y);

                            WorldUtils.Gen(point, new Shapes.Circle(4), Actions.Chain(new GenAction[]
                            {
                                //new Modifiers.IsSolid(),
                                new Actions.SetTile((ushort)ModContent.TileType<UnbreakableVoidBlock>()),
                                new Actions.SetFrames()
                            }));
                        }
                    }
                }
            }
        }

        private void GenerateShimmerBlocks(Rectangle biomeBounds)
        {
            int clusterCount = 100;
            for (int i = 0; i < clusterCount; i++)
            {
                int x = WorldGen.genRand.Next(biomeBounds.Left + 20, biomeBounds.Right - 20);
                int y = WorldGen.genRand.Next(biomeBounds.Top + 20, biomeBounds.Bottom - 20);
                WorldGen.TileRunner(x, y, WorldGen.genRand.Next(5, 10), WorldGen.genRand.Next(20, 40), TileID.ShimmerBlock);
            }
        }

        private void GenerateShimmerChests(Rectangle biomeBounds)
        {
            int chestCount = 15;
            for (int i = 0; i < chestCount; i++)
            {
                bool success = false;
                int attempts = 0;
                int x = 0;
                int y = 0;
                int chestIndex = -2;
                while (!success)
                {
                    attempts++;
                    if (attempts > 1000)
                    {
                        break;
                    }
                    x = WorldGen.genRand.Next(biomeBounds.Left + 20, biomeBounds.Right - 20);
                    y = WorldGen.genRand.Next(biomeBounds.Top + 20, biomeBounds.Bottom - 20);
                    //chestIndex = WorldGen.PlaceChest(x, y, (ushort)ModContent.TileType<ShimmerChest>());
                    chestIndex = WorldGen.PlaceChest(x, y, style: 37);

                    success = chestIndex != -1;
                }

                if (success)
                {
                    //Main.NewText($"Placed chest at {x}, {y} after {attempts} attempts.");
                    Chest shimmerChest = Main.chest[chestIndex];

                    var itemsToAdd = new List<(int type, int stack)>();
                    int specialItem = new Terraria.Utilities.WeightedRandom<int>(
                        Tuple.Create((int)ItemID.ShimmerCloak, 1.5),
                        Tuple.Create((int)ItemID.BandofStarpower, 2.0),
                        Tuple.Create((int)ItemID.None, 5.0),
                        Tuple.Create(ModContent.ItemType<Bread>(), 3.0),
                        Tuple.Create(ModContent.ItemType<Pancakes>(), 3.0),
                        Tuple.Create(ModContent.ItemType<Toast>(), 3.0),
                        Tuple.Create(ModContent.ItemType<Waffle>(), 3.0)
                    );
                    if (specialItem != ItemID.None)
                    {
                        itemsToAdd.Add((specialItem, 1));
                    }

                    switch (Main.rand.Next(4))
                    {
                        case 0:
                            itemsToAdd.Add((ItemID.FallenStar, Main.rand.Next(15, 45)));
                            itemsToAdd.Add((ItemID.ShimmerTorch, Main.rand.Next(13, 28)));
                            break;
                        case 1:
                            itemsToAdd.Add((ItemID.Amethyst, Main.rand.Next(5, 12)));
                            itemsToAdd.Add((ItemID.Emerald, Main.rand.Next(5, 12)));
                            itemsToAdd.Add((ItemID.GemTreeAmethystSeed, Main.rand.Next(12, 18)));
                            itemsToAdd.Add((ItemID.GemTreeEmeraldSeed, Main.rand.Next(12, 18)));
                            break;
                        case 2:
                            itemsToAdd.Add((ItemID.Diamond, Main.rand.Next(5, 12)));
                            itemsToAdd.Add((ItemID.Ruby, Main.rand.Next(5, 12)));
                            itemsToAdd.Add((ItemID.GemTreeRubySeed, Main.rand.Next(12, 18)));
                            itemsToAdd.Add((ItemID.GemTreeDiamondSeed, Main.rand.Next(12, 18)));
                            break;
                        case 3:
                            itemsToAdd.Add((ItemID.Sapphire, Main.rand.Next(5, 12)));
                            itemsToAdd.Add((ItemID.Topaz, Main.rand.Next(5, 12)));
                            itemsToAdd.Add((ItemID.GemTreeSapphireSeed, Main.rand.Next(12, 18)));
                            itemsToAdd.Add((ItemID.GemTreeTopazSeed, Main.rand.Next(12, 18)));
                            break;
                    }

                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            itemsToAdd.Add((ItemID.TeleportationPotion, Main.rand.Next(3, 8)));
                            break;
                        case 1:
                            itemsToAdd.Add((ItemID.GravitationPotion, Main.rand.Next(3, 8)));
                            break;
                        case 2:
                            itemsToAdd.Add((ModContent.ItemType<ShimmerBottle>(), Main.rand.Next(10, 28)));
                            break;
                    }

                    itemsToAdd.Add((ItemID.GoldCoin, Main.rand.Next(3, 18)));


                    //Item item = new Item();
                    //item.SetDefaults((int)ItemID.ShimmerBlock);
                    //shimmerChest.item[0] = item;

                    int chestItemIndex = 0;
                    foreach (var itemToAdd in itemsToAdd)
                    {
                        Item item = new Item();
                        item.SetDefaults(itemToAdd.type);
                        item.stack = itemToAdd.stack;
                        shimmerChest.item[chestItemIndex] = item;
                        chestItemIndex++;
                        if (chestItemIndex >= 40)
                            break;

                        //shimmerChest.item[0] = new Item();
                        //shimmerChest.item[0].SetDefaults(ItemID.ShimmerBlock);
                    }
                    //else
                    //    Main.NewText($"Failed to place chest after {attempts} attempts.");
                }
            }
        }

        public void CreateShimmerBiome()
        {
            List<Vector2> shimmerLocations = new List<Vector2>();

            for (int x = 0; x < Main.maxTilesX; x++)
            {
                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    if (Main.tile[x, y].LiquidType == LiquidID.Shimmer)
                    {
                        //WorldGen.PlaceTile(x, y, ModContent.TileType<VoidBlock>(), true, true);
                        //Main.NewText("Shimmer found @ " + x + ", " + y);
                        shimmerLocations.Add(new Vector2(x, y));
                    }
                }
            }

            int mostLeftShimmer = 0;
            int mostRightShimmer = 0;
            int mostHighShimmer = 0;
            int mostLowShimmer = 0;

            double biomeSizeMultiplier = 2.5;

            // best practice is to check if any locations exist, but lets just skip that lol
            mostLeftShimmer = (int)shimmerLocations[0].X;
            mostRightShimmer = (int)shimmerLocations[0].X;
            mostLowShimmer = (int)shimmerLocations[0].Y;
            mostHighShimmer = (int)shimmerLocations[0].Y;

            foreach (Vector2 location in shimmerLocations)
            {
                if (location.X < mostLeftShimmer)
                {
                    mostLeftShimmer = (int)location.X;
                }
                if (location.X > mostRightShimmer)
                {
                    mostRightShimmer = (int)location.X;
                }
                if (location.Y > mostLowShimmer)
                {
                    mostLowShimmer = (int)location.Y;
                }
                if (location.Y < mostHighShimmer)
                {
                    mostHighShimmer= (int)location.Y;
                }
            }

            int biomeSizeY = 50;
            //int biomeSizeY2 = 200;

            // Based off of underworld starting at Main.maxTilesY - 200
            int biomeSizeY2 = (Main.maxTilesY - 400) - (mostLowShimmer + biomeSizeY + 11);
            Main.NewText("Biome Size: " + (Main.maxTilesY - 400) + " - " + (mostLowShimmer + biomeSizeY + 11));

            int middleXShimmer = (mostLeftShimmer + mostRightShimmer) / 2;
            int lengthXShimmer = mostRightShimmer - mostLeftShimmer;
            int heightYShimmer = mostLowShimmer - mostHighShimmer;

            //Main.NewText(lengthXShimmer);

            //Step 1: Generate the surface shimmer shape
            Point point = new Point(mostLeftShimmer - 3, mostHighShimmer - 3); // Top left of rectangle basically, with an additional up 3 and left 3
            WorldUtils.Gen(point, new Shapes.Rectangle(lengthXShimmer + 6, heightYShimmer + 45), Actions.Chain(new GenAction[]
            {
                new Modifiers.IsSolid(),
                new Actions.SetTile((ushort)ModContent.TileType<UnbreakableVoidBlock>()),
                new Actions.SetFrames()
            }));

            // Step 2: Generate a mound shape for the inside of the first layer of the biome
            Point point2 = new Point(middleXShimmer, mostLowShimmer + biomeSizeY + 10); // Location of mound 10 spaces right under the shimmer
            WorldUtils.Gen(point2, new Shapes.Mound((int)(lengthXShimmer * (biomeSizeMultiplier)), biomeSizeY), Actions.Chain(new GenAction[]
            {
                new Actions.SetTile((ushort)ModContent.TileType<UnbreakableVoidBlock>()),
                new Actions.SetFrames()
            }));

            // Step 3: Generate a rectangular shape for the inner layer of the biome
            Point point3 = new Point(middleXShimmer - (int)(lengthXShimmer * (biomeSizeMultiplier)), mostLowShimmer + biomeSizeY + 11); // middle of shimmer and to the left, the half width of mound // also same y position as mound but it gotta be 1 under
            WorldUtils.Gen(point3, new Shapes.Rectangle((int)(lengthXShimmer * 2 * biomeSizeMultiplier), biomeSizeY2), Actions.Chain(new GenAction[]
            {
                new Modifiers.IsSolid(),
                new Actions.SetTile((ushort)ModContent.TileType<UnbreakableVoidBlock>()),
                new Actions.SetFrames()
            }));

            // Step 3.5: Generate an inner rectangular shape inside to make breakable aether blocks
            int offset = 5;
            Point point3B = new Point(middleXShimmer - (int)(lengthXShimmer * (biomeSizeMultiplier)) + offset, mostLowShimmer + biomeSizeY + 11 + offset); // middle of shimmer and to the left, the half width of mound // also same y position as mound but it gotta be 1 under
            WorldUtils.Gen(point3B, new Shapes.Rectangle((int)(lengthXShimmer * 2 * biomeSizeMultiplier) - (2*offset), biomeSizeY2 - (2*offset)), Actions.Chain(new GenAction[]
            {
                new Modifiers.IsSolid(),
                new Actions.SetTile((ushort)ModContent.TileType<VoidBlock>()),
                new Actions.SetFrames()
            }));

            // Step 4: Seal the inner layer

            Rectangle biomeBounds = new Rectangle(point3.X, point3.Y, (int)(lengthXShimmer * 2 * biomeSizeMultiplier), biomeSizeY2);
            SealBiomeBorder(biomeBounds);

            //Step 4.5: Put some chests now
            GenerateShimmerChests(biomeBounds);

            //Step 5: Put a little shimmer in every non solid tile and rid of walls
            Point point4 = new Point(middleXShimmer - (int)(lengthXShimmer * (biomeSizeMultiplier)), mostLowShimmer + biomeSizeY + 11); // middle of shimmer and to the left, the half width of mound // also same y position as mound but it gotta be 1 under
            WorldUtils.Gen(point4, new Shapes.Rectangle((int)(lengthXShimmer * 2 * biomeSizeMultiplier), biomeSizeY2), Actions.Chain(new GenAction[]
            {
                new Modifiers.IsEmpty(),
                new Actions.RemoveWall(),
                new Actions.SetLiquid(LiquidID.Shimmer, 75),
                new Actions.SetFrames()
            }));

            //Step 6: Replace some inner blocks with shimmer blocks
            GenerateShimmerBlocks(biomeBounds);


            /*
            // Step 4: Generate a tunnel in that first layer
            //Point randomPoint = new Point(WorldGen.genRand.Next(mostLeftShimmer, mostRightShimmer), WorldGen.genRand.Next((mostLowShimmer + (biomeSizeY/2) + 10), (mostLowShimmer + biomeSizeY + 10)));

            Point tunnelPoint = new Point(mostLeftShimmer + WorldGen.genRand.Next(-10, -5), (mostLowShimmer + biomeSizeY + 10) + WorldGen.genRand.Next(-10, -5));
            ReLogic.Utilities.Vector2D vectorFound = WorldGen.digTunnel(tunnelPoint.X, tunnelPoint.Y, 1, 0, ((int)(lengthXShimmer * (1.5))), 10);
            Point pointFound = new Point((int)vectorFound.X, (int)vectorFound.Y);

            WorldUtils.Gen(pointFound, new Shapes.Circle(5), Actions.Chain(new GenAction[]
            {
                new Modifiers.IsEmpty(),
                new Actions.SetTile(TileID.ShimmerBlock),
                //new Actions.SetLiquid(LiquidID.Shimmer),
                new Actions.SetFrames()
            }));

            WorldUtils.Gen(tunnelPoint, new Shapes.Circle(5), Actions.Chain(new GenAction[]
            {
                new Modifiers.IsEmpty(),
                new Actions.SetTile(TileID.ShimmerBlock),
                //new Actions.SetLiquid(LiquidID.Shimmer),
                new Actions.SetFrames()
            }));

            Main.NewText(tunnelPoint + " is where the tunnel is and " + point + " is the top left of the shimmer");

            // Step 5: Generate two patches of shimmer water to help make it a shimmer biome!

            Point tunnelPoint2 = new Point(mostRightShimmer + WorldGen.genRand.Next(5, 10), (mostLowShimmer + (biomeSizeY/2) + 10) + WorldGen.genRand.Next(-10, 10));
            WorldGen.TileRunner(tunnelPoint2.X, tunnelPoint2.Y, 16, 5, TileID.ShimmerBlock);

            Point tunnelPoint3 = new Point(mostLeftShimmer + WorldGen.genRand.Next(-10, -5), (mostLowShimmer + (biomeSizeY / 2) + 10) + WorldGen.genRand.Next(-10, 10));
            WorldGen.TileRunner(tunnelPoint3.X, tunnelPoint2.Y, 16, 5, TileID.ShimmerBlock);
      
            for (int i = 0; i < 10; i++)
            {
                
            }
            */
            //Main.NewText("Shimmer Generated!!");

        }
    }

    public class WorldGenShimmerPass : GenPass
    {
        public WorldGenShimmerPass(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = ShimmerGeneration.WorldGenShimmerPassMessage.Value;
            ShimmerGeneration shimmerGenerator = new ShimmerGeneration();
            shimmerGenerator.CreateShimmerBiome();
        }
    }
}
