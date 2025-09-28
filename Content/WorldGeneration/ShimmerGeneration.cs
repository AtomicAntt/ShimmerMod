using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Input;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Generation;
using ShimmerMod.Content.Tiles;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Terraria.WorldBuilding;
using Terraria.ModLoader.IO;
using ReLogic.Utilities;

namespace ShimmerMod.Content.WorldGeneration
{
    public class ShimmerGeneration : ModSystem
    {
        public static bool JustPressed(Keys key)
        {
            return Main.keyState.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key);
        }

        public override void PostUpdateWorld()
        {
            if (JustPressed(Keys.D1))
            {
                TestMethod((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
            }
            if (JustPressed(Keys.D2))
            {
                CreateShimmerBiome();
            }
        }

        private void TestMethod(int x, int y)
        {
            Dust.QuickBox(new Vector2(x, y) * 16, new Vector2(x + 1, y + 1) * 16, 2, Color.YellowGreen, null);

            //WorldGen.TileRunner(x - 1, y, WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(2, 8), ModContent.TileType<VoidBlock>());
            Point point = new Point(x, y);

            WorldUtils.Gen(point, new ShapeBranch(90, 10), Actions.Chain(new GenAction[]
            {
                //new Modifiers.IsSolid(),
                new Actions.SetTile((ushort)ModContent.TileType<VoidBlock>()),
                new Actions.SetFrames()
            }));
            //WorldUtils.Gen(point,
            //    new ShapeRunner(
            //        strength: 10,  // radius of carve
            //        steps: 100,    // length of tunnel
            //        velocity: new Vector2D(1f, 0f)
            //    ),
            //    Actions.Chain(new GenAction[]
            //    {
            //        new Actions.ClearTile(),  // removes tiles
            //        new Actions.SetFrames()   // updates framing
            //    }));

            //WorldUtils.Gen(point, new ShapeRunner(), Actions.Chain(new GenAction[]
            //{
            //    //new Modifiers.IsSolid(),
            //    new Actions.SetTile((ushort)ModContent.TileType<VoidBlock>()),
            //    new Actions.SetFrames()
            //}));
            Main.NewText("Generated Blocks!");
        }

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
                                new Actions.SetTile((ushort)ModContent.TileType<VoidBlock>()),
                                new Actions.SetFrames()
                            }));
                        }
                    }
                }
            }
        }

        private void CreateShimmerBiome()
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
                new Actions.SetTile((ushort)ModContent.TileType<VoidBlock>()),
                new Actions.SetFrames()
            }));

            // Step 2: Generate a mound shape for the inside of the first layer of the biome
            Point point2 = new Point(middleXShimmer, mostLowShimmer + biomeSizeY + 10); // Location of mound 10 spaces right under the shimmer
            WorldUtils.Gen(point2, new Shapes.Mound((int)(lengthXShimmer * (biomeSizeMultiplier)), biomeSizeY), Actions.Chain(new GenAction[]
            {
                new Actions.SetTile((ushort)ModContent.TileType<VoidBlock>()),
                new Actions.SetFrames()
            }));

            // Step 3: Generate a rectangular shape for the inner layer of the biome
            Point point3 = new Point(middleXShimmer - (int)(lengthXShimmer * (biomeSizeMultiplier)), mostLowShimmer + biomeSizeY + 11); // middle of shimmer and to the left, the half width of mound // also same y position as mound but it gotta be 1 under
            WorldUtils.Gen(point3, new Shapes.Rectangle((int)(lengthXShimmer * 2 * biomeSizeMultiplier), biomeSizeY2), Actions.Chain(new GenAction[]
            {
                new Modifiers.IsSolid(),
                new Actions.SetTile((ushort)ModContent.TileType<VoidBlock>()),
                new Actions.SetFrames()
            }));

            // Step 4: Seal the inner layer

            Rectangle biomeBounds = new Rectangle(point3.X, point3.Y, (int)(lengthXShimmer * 2 * biomeSizeMultiplier), biomeSizeY2);
            SealBiomeBorder(biomeBounds);

            //Step 5: Put a little shimmer in every non solid tile and rid of walls
            Point point4 = new Point(middleXShimmer - (int)(lengthXShimmer * (biomeSizeMultiplier)), mostLowShimmer + biomeSizeY + 11); // middle of shimmer and to the left, the half width of mound // also same y position as mound but it gotta be 1 under
            WorldUtils.Gen(point4, new Shapes.Rectangle((int)(lengthXShimmer * 2 * biomeSizeMultiplier), biomeSizeY2), Actions.Chain(new GenAction[]
            {
                new Modifiers.IsEmpty(),
                new Actions.RemoveWall(),
                new Actions.SetLiquid(LiquidID.Shimmer, 100),
                new Actions.SetFrames()
            }));

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
            Main.NewText("Shimmer Generated!!");

        }
    }
}
