using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShimmerMod.Content.Drink
{
    public class ShimmerBottle : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.UseSound = SoundID.Item3;
            Item.buffType = BuffID.Shimmer;
            Item.buffTime = 8 * 60 * 60;
            Item.value = Item.buyPrice(0, 1);
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddCondition(Condition.NearShimmer);
            recipe.AddIngredient(ItemID.Bottle);
            recipe.Register();
        }
    }
}
