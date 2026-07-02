using Terraria.DataStructures;
using Terraria.ID;

namespace Terraria.ModLoader;

/// <summary>
/// <see cref="StompAbility"/> is a singleton, defining the properties and behaviour of stomping abilities like the ones from the slime mounts and <see cref="ItemID.DeadCellsRamRune"/> down dash.
/// </summary>
public abstract class StompAbility : ModType
{
	public static StompAbility SlimeMounts { get; private set; } = new SlimeMountStompAbility();

	public static StompAbility GolfCartMount { get; private set; } = new GolfCartMountStompAbility();

	public static StompAbility PogoStickMount { get; private set; } = new PogoStickMountStompAbility();

	public static StompAbility DeadCellsDownDash { get; private set; } = new DeadCellsDownDashStompAbility();

	/// <summary>
	/// The internal ID of this <see cref="StompAbility"/>.
	/// </summary>
	public int Type { get; internal set; }

	/// <summary>
	/// The stats and settings for this <see cref="StompAbility"/>
	/// </summary>
	public StompStrike Stats { get; protected set; } = StompStrike.Default;

	/// <summary>
	/// If <see langword="true"/>, this ability will cause the player to fastfall like the slime mounts and the <see cref="ItemID.DeadCellsRamRune"/> down dash.<br/>
	/// Defaults to <see langword="true"/>.
	/// </summary>
	public bool FastFall { get; protected set; } = true;

	/// <summary>
	/// If <see langword="true"/>, the ability can fast fall through liquids like the <see cref="ItemID.DeadCellsRamRune"/> down dash.<br/>
	/// Defaults to <see langword="false"/>.
	/// </summary>
	public bool FastFallInLiquids { get; protected set; } = false;

	/// <summary>
	/// If <see langword="true"/>, the ability will only attempt to make the player fastfall and will not damage NPCs.<br/>
	/// Defaults to <see langword="false"/>.
	/// </summary>
	public bool OnlyVisual { get; protected set; } = false;

	protected sealed override void Register()
	{
		ModTypeLookup<StompAbility>.Register(this);
		Type = StompAbilityLoader.Add(this);
	}

	public sealed override void SetupContent() => SetStaticDefaults();

	public override string ToString() => Name;

	/// <summary>
	/// Return whether the player should attempt to stomp NPCs with this ability.
	/// <para/>
	/// For example, vanilla uses this in <see cref="SlimeMounts"/> to check whether the player is riding one of the slime mounts.
	/// </summary>
	/// <param name="player">The player instance</param>
	public abstract bool IsActive(Player player);

	/// <summary>
	/// This hooks allows you to conditionally prevent <see cref="StompStrike.FastFall"/> from affecting the player's vertical movement while this stomp ability is active.
	/// </summary>
	/// <param name="player">The player instance</param>
	/// <returns><see langword="true"/> to allow the stomp ability to make the player fastfall; otherwise, <see langword="false"/>.</returns>
	public virtual bool CanFastFall(Player player)
	{
		return true;
	}

	/// <summary>
	/// This hook allows you to do something after <paramref name="player"/> has stomped <paramref name="target"/> with this ability.
	/// </summary>
	/// <param name="player">The player instance</param>
	/// <param name="target">The NPC that was stomped</param>
	public virtual void OnStomp(Player player, NPC target)
	{
	}
}
