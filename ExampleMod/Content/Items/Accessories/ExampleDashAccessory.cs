using ExampleMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Accessories
{
	[AutoloadEquip(EquipType.Shield)]
	public class ExampleDashAccessory : ModItem
	{
		public override void SetStaticDefaults(){
			Tooltip.SetDefault("This is a modded dash accessory.");
		}

		public override void SetDefaults(){
			item.defense = 5;
			item.width = 24;
			item.height = 28;
			item.accessory = true;
			item.value = Item.buyPrice(silver: 50);
			item.rare = ItemRarityID.Blue;
		}

		public override void UpdateAccessory(Player player, bool hideVisual){
			player.dashType = DashLoader.GetDashType<ExampleDamagingDash>();
		}

		public override void AddRecipes(){
			CreateRecipe()
				.AddIngredient<ExampleItem>(5)
				.AddTile(TileID.WorkBenches);
		}
	}

	public class ExampleDamagingDash : ModDash{
		public override void SetDefaults(){
			melee = true;
			velocity = 24f;
			damage = 50;
			knockBack = 5f;
		}

		public override bool IgnoreNPCs(Player player) => false;

		public override bool CanHitNPC(Player player, NPC target) => true;

		public override void BeginEffects(Player player){
			for(int i = 0; i < 16; i++){
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, ModContent.DustType<Sparkle>(), Scale: 1.5f);
				dust.noGravity = true;
			}
		}

		public override void MiddleEffects(Player player){
			for(int i = 0; i < 4; i++){
				Dust dust = Dust.NewDustDirect(player.Center - new Vector2(4), 8, 8, ModContent.DustType<Sparkle>());
				dust.noGravity = true;
			}
		}

		public override void ModifyHitImmunity(Player player, NPC target, ref int playerImmunity, ref int npcImmunity){
			//Make both the player and the target's i-frames last for 10 ticks
			playerImmunity = 10;
			npcImmunity = 10;
		}

		public override void ModifyDashValues(Player player, ref float velocityCheck, ref float deaccelerationFactor, ref float deaccelerationFactor2, ref int dashDelay){
			velocityCheck = 15f;
			deaccelerationFactor -= 0.05f;
			deaccelerationFactor2 -= 0.025f;
			dashDelay = 2;
		}

		public override bool ModifyHitRecoil(Player player, ref float recoilX, ref float recoilY){
			recoilX = 11;
			recoilY = 5;

			return true;
		}
	}
}
