namespace Terraria.ModLoader
{
	public class ModDash : ILoadable
	{
		public int type;
		public int damage;
		public float knockBack;
		public float velocity;

		public bool melee;
		public bool ranged;
		public bool magic;
		public bool summoner;

		public void Load(Mod mod) => DashLoader.AddDash(GetType().Name, this);

		public void Unload(){ }

		public virtual void SetDefaults(){ }

		/// <summary>
		/// This hook runs when the dash happens.
		/// Use this hook to make something happen at the start of the dash, such as the initial puff of smoke from the Tabi.
		/// </summary>
		/// <param name="player">The player that's doing the dashing.</param>
		public virtual void BeginEffects(Player player){ }

		/// <summary>
		/// This hook runs while the dash is still active.
		/// Use this hook to make something happen during the dash, such as the smoke dust from the Tabi.
		/// </summary>
		/// <param name="player">The player that's doing the dashing.</param>
		/// <param name="timeLeft">How many ticks are left until this dash ends.</param>
		public virtual void MiddleEffects(Player player){ }

		/// <summary>
		/// Use this hook to modify the various movement-related factors for this <seealso cref="ModDash"/>.
		/// </summary>
		/// <param name="player">The player that's doing the dashing.</param>
		/// <param name="velocityCheck">
		/// How slow the player must be moving in order to do another dash.
		/// Defaults to <c>12</c>.
		/// </param>
		/// <param name="deaccelerationFactor">
		/// The deacceleration factor applied to the player's velocity if they are moving faster than the velocity check.
		/// Defaults to <c>0.922</c>.
		/// </param>
		/// <param name="deaccelerationFactor2">
		/// The deacceleration factor applied to the player's velocity if they are moving faster than their max run speed.
		/// Defaults to <c>0.96</c>.
		/// </param>
		/// <param name="dashDelay">
		/// For how long in ticks the player will be unable to execute another dash.
		/// Defaults to <c>20</c>.
		/// </param>
		public virtual void ModifyDashValues(Player player, ref float velocityCheck, ref float deaccelerationFactor, ref float deaccelerationFactor2, ref int dashDelay){ }

		/// <summary>
		/// Whether or not the dash should be able to hit any NPC.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public virtual bool CanHitAnyNPC(Player player) => false;

		/// <summary>
		/// Whether or not the dash should check for collision against a certain NPC.
		/// Intangible/Invincible NPCs are not checked using this hook.
		/// This hook is ignored if <seealso cref="CanHitAnyNPC(Player)"/> returns <c>false</c>.
		/// </summary>
		/// <param name="player">The player that's doing the dashing.</param>
		/// <param name="target">The NPC being checked against.</param>
		/// <returns></returns>
		public virtual bool CanHitNPC(Player player, NPC target) => false;

		/// <summary>
		/// This hook allows you to customize what happens to the player and the NPC being hit.
		/// This hook is called before the hit happens.
		/// </summary>
		/// <param name="player">The player that's doing the dashing.</param>
		/// <param name="target">The NPC that's being hit by the dash.</param>
		/// <param name="damage">The damage dealt to the NPC.</param>
		/// <param name="knockBack">The knockback dealt to the NPC.</param>
		/// <param name="crit">Whether the hit should be a crit or not.</param>
		public virtual void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit){ }

		/// <summary>
		/// This hook allows you to customize what happens to the player and the NPC being hit.
		/// This hook is called after the hit happens.
		/// </summary>
		/// <param name="player">The player that's doing the dashing.</param>
		/// <param name="target">The NPC that's being hit by the dash.</param>
		/// <param name="damage">The damage dealt to the NPC.</param>
		/// <param name="knockBack">The knockback dealt to the NPC.</param>
		/// <param name="crit">Whether the hit should be a crit or not.</param>
		public virtual void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit){ }

		/// <summary>
		/// This hook allows you to modify how many immunity frames the player and the hit NPC get.
		/// </summary>
		/// <param name="player">The player that's doing the dashing.</param>
		/// <param name="target">The NPC that's being hit by the dash.</param>
		/// <param name="playerImmunity">How many ticks of immunity the player should receive.</param>
		/// <param name="npcImmunity">How many ticks of immunity the NPC should receive.</param>
		public virtual void ModifyHitImmunity(Player player, NPC target, ref int playerImmunity, ref int npcImmunity){ }
	}
}
