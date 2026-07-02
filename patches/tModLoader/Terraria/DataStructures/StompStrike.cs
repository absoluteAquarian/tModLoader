using Terraria.ModLoader;

namespace Terraria.DataStructures;

/// <summary>
/// Stores the stats for a stomp attack, used by <see cref="Player.StompNPCs"/>
/// </summary>
public struct StompStrike
{
	public static readonly StompStrike Default = new();

	/// <summary>
	/// The base damage when striking a stomped NPC.<br/>
	/// Defaults to <c>40</c>.
	/// </summary>
	public int Damage;

	/// <summary>
	/// The knockback when striking a stomped NPC.<br/>
	/// Defaults to <c>5.0f</c>.
	/// </summary>
	public float Knockback;

	/// <summary>
	/// The duration in ticks of the invincibility frames given to a stomped NPC.<br/>
	/// Defaults to <c>10</c>.
	/// </summary>
	public int NPCImmuneTicks;

	/// <summary>
	/// The duration in ticks of the invincibility frames given to the player when stomping an NPC.<br/>
	/// Defaults to <c>6</c>.
	/// </summary>
	public int PlayerImmuneTicks;

	/// <summary>
	/// The damage type of the stomp attack.<br/>
	/// Defaults to <see langword="null"/>, which is treated as classless.
	/// </summary>
	public DamageClass DamageType;

	public StompStrike()
	{
		Damage = 40;
		Knockback = 5f;
		NPCImmuneTicks = 10;
		PlayerImmuneTicks = 6;
		DamageType = null;
	}
}
