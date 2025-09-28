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

            WorldUtils.Gen(point, new Shapes.Mound(5, 5), Actions.Chain(new GenAction[]
            {
                //new Modifiers.IsSolid(),
                new Actions.SetTile((ushort)ModContent.TileType<VoidBlock>()),
                new Actions.SetFrames()
            }));

            Main.NewText("Generated Blocks!");
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

            //int surfaceBiomeSizeY = 75;
            int biomeSizeY = 100;
            int biomeSizeY2 = 200;

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
            WorldUtils.Gen(point2, new Shapes.Mound((int)(lengthXShimmer * (1.5)), biomeSizeY), Actions.Chain(new GenAction[]
            {
                new Actions.SetTile((ushort)ModContent.TileType<VoidBlock>()),
                new Actions.SetFrames()
            }));

            // Step 3: Generate a rectangular shape for the inner layer of the biome
            Point point3 = new Point(middleXShimmer - (int)(lengthXShimmer * (1.5)), mostLowShimmer + biomeSizeY + 11); // middle of shimmer and to the left, the half width of mound // also same y position as mound but it gotta be 1 under
            WorldUtils.Gen(point3, new Shapes.Rectangle(lengthXShimmer * 3, biomeSizeY2), Actions.Chain(new GenAction[]
            {
                new Actions.SetTile((ushort)ModContent.TileType<VoidBlock>()),
                new Actions.SetFrames()
            }));

            // Step 4: Generate a tunnel in that first layer
            Point randomPoint = new Point(WorldGen.genRand.Next(mostLeftShimmer, mostRightShimmer), WorldGen.genRand.Next((mostLowShimmer + (biomeSizeY/2) + 10), (mostLowShimmer + biomeSizeY + 10)));
            WorldGen.digTunnel(randomPoint.X, randomPoint.Y, 1, 0, ((int)(lengthXShimmer * (1.4))), 6);

            Main.NewText(randomPoint + " is where the tunnel is and " + point + " is the top left of the shimmer");

            Main.NewText("Shimmer Generated!!");

        }
    }
}
