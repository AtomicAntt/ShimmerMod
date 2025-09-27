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

            WorldGen.TileRunner(x - 1, y, WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(2, 8), ModContent.TileType<VoidBlock>());

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

            int biomeSizeY = 100;

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

            //Main.NewText(lengthXShimmer);

            Point point = new Point(middleXShimmer, mostLowShimmer + (biomeSizeY / 2));
            WorldUtils.Gen(point, new Shapes.Circle(lengthXShimmer, biomeSizeY), Actions.Chain(new GenAction[]
            {
                new Modifiers.IsSolid(),
                new Actions.SetTile((ushort)ModContent.TileType<VoidBlock>())
            }));
            Main.NewText("Shimmer Generated!");

        }
    }
}
