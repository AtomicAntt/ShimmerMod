using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace ShimmerMod.Content.Food
{
    public class Pancakes : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            ItemID.Sets.FoodParticleColors[Item.type] = [
                new Color(161, 82, 3),
                new Color(214, 114, 13),
                new Color(179, 100, 21)
            ];
            ItemID.Sets.IsFood[Type] = true;
        }

        public override void SetDefaults()
        {

            Item.DefaultToFood(22, 22, BuffID.WellFed2, 5 * 60 * 60); // 5 minute
            Item.value = Item.buyPrice(0, 0, 20);
            Item.rare = ItemRarityID.White;
        }
    }
}
