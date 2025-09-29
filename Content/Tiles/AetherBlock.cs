using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShimmerMod.Content.Tiles
{
    public class AetherBlock : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            MinPick = 300;

            DustType = DustID.ShimmerSpark;
            VanillaFallbackOnModDeletion = TileID.DiamondGemspark;

            AddMapEntry(new Color(224, 212, 177));
        }
    }
}
