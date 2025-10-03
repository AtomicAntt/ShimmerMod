using Microsoft.Xna.Framework;
using ShimmerMod.Content.Tiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace ShimmerMod.Content.Blocks
{
    public class VoidBlock : ModItem
    {
        public override void SetDefaults()
        {

            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.VoidBlock>(), 0);
            Item.value = 0;
        }
    }
}