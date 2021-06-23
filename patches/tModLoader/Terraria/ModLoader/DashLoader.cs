using System.Collections.Generic;
using Terraria.ID;

namespace Terraria.ModLoader
{
	public static class DashLoader
	{
		private static IDictionary<string, ModDash> dashes;

		private static int nextID = DashID.Unused + 1;

		private static int lastCheckedType = -1;
		private static ModDash lastReturnedDash;

		/// <summary>
		/// Registers a <seealso cref="ModDash"/> to the dictionary.
		/// The dash's type is set and its SetDefaults method is called in here.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="dash"></param>
		public static void AddDash(string name, ModDash dash){
			dash.SetDefaults();
			dash.type = nextID++;

			if(dashes is null)
				dashes = new Dictionary<string, ModDash>();

			dashes.Add(name, dash);
		}

		public static void Unload(){
			nextID = DashID.Unused + 1;
			lastCheckedType = -1;

			dashes.Clear();
		}

		public static int GetDashType(string name) => dashes.TryGetValue(name, out ModDash dash) ? dash.type : -1;

		public static int GetDashType<T>() where T : ModDash => GetDashType(typeof(T).Name);

		public static ModDash GetDash(int type){
			//Fast access for repeated uses of this method with the same type
			if(lastCheckedType == type)
				return lastReturnedDash;

			if(type <= DashID.Unused || type >= nextID){
				lastCheckedType = -1;
				lastReturnedDash = null;

				return null;
			}

			foreach(var pair in dashes){
				if(pair.Value.type == type){
					lastCheckedType = type;
					lastReturnedDash = pair.Value;

					return pair.Value;
				}
			}

			lastCheckedType = -1;
			lastReturnedDash = null;

			return null;
		}

		public static ModDash GetDash(string name) => dashes.TryGetValue(name, out ModDash dash) ? dash : null;

		public static ModDash GetDash<T>() where T : ModDash => GetDash(typeof(T).Name);

		public static bool PlayerHasModDash(Player player) => player.dashType > DashID.Unused;

		public static void BeginEffects(Player player){
			if(PlayerHasModDash(player)){
				var dash = GetDash(player.dashType);

				dash?.BeginEffects(player);
			}
		}

		public static void MiddleEffects(Player player){
			if(PlayerHasModDash(player)){
				var dash = GetDash(player.dashType);

				dash?.MiddleEffects(player);
			}
		}

		public static void ModifyDashvalues(Player player, ref float velocity, ref float deaccelerationFactor, ref float deaccelerationFactor2, ref int dashDelay){
			if(PlayerHasModDash(player)){
				var dash = GetDash(player.dashType);

				dash?.ModifyDashValues(player, ref velocity, ref deaccelerationFactor, ref deaccelerationFactor2, ref dashDelay);
			}
		}

		public static bool CanHitAnyNPC(Player player){
			if(PlayerHasModDash(player)){
				var dash = GetDash(player.dashType);

				return dash?.CanHitAnyNPC(player) ?? false;
			}

			return false;
		}

		public static bool CanHitNPC(Player player, NPC target){
			if(PlayerHasModDash(player)){
				var dash = GetDash(player.dashType);

				return dash?.CanHitNPC(player, target) ?? false;
			}

			return false;
		}

		public static void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit){
			if(PlayerHasModDash(player)){
				var dash = GetDash(player.dashType);

				dash?.ModifyHitNPC(player, target, ref damage, ref knockBack, ref crit);
			}
		}

		public static void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit){
			if(PlayerHasModDash(player)){
				var dash = GetDash(player.dashType);

				dash?.OnHitNPC(player, target, damage, knockBack, crit);
			}
		}

		public static void ModifyHitImmunity(Player player, NPC target, ref int playerImmunity, ref int npcImmunity){
			if(PlayerHasModDash(player)){
				var dash = GetDash(player.dashType);

				dash?.ModifyHitImmunity(player, target, ref playerImmunity, ref npcImmunity);
			}
		}
	}
}
