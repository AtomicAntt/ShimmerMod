using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Input;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Generation;
using ShimmerMod.Content.Tiles;

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
            for (int x = 0; x < Main.maxTilesX; x++)
            {
                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    if (Main.tile[x, y].LiquidType == LiquidID.Shimmer)
                    {
                        //Dust.QuickBox(new Vector2(x, y) * 16, new Vector2(x + 1, y + 1) * 16, 2, Color.YellowGreen, null);
                        WorldGen.PlaceTile(x, y, ModContent.TileType<VoidBlock>(), true, true);
                        Main.NewText("Shimmer found!!!");
                    }
                }
            }
        }
    }
}
