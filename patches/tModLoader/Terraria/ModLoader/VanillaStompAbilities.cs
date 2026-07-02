using Terraria.DataStructures;
using Terraria.ID;

namespace Terraria.ModLoader;

[Autoload(false)]
public abstract class VanillaStompAbility : StompAbility
{
}

public sealed class SlimeMountStompAbility : VanillaStompAbility
{
	public override void SetStaticDefaults()
	{
		Stats = StompStrike.Default with
		{
			DamageType = DamageClass.Summon
		};
	}

	public override bool IsActive(Player player)
	{
		return player.mount.Active && player.mount.IsConsideredASlimeMount && player.wetSlime > 0;
	}

	public override bool CanFastFall(Player player)
	{
		return !player.SlimeDontHyperJump;
	}

	public override void OnStomp(Player player, NPC target)
	{
		player.velocity.Y = -10f;
	}
}

public sealed class GolfCartMountStompAbility : VanillaStompAbility
{
	public override void SetStaticDefaults()
	{
		Stats = StompStrike.Default with
		{
			NPCImmuneTicks = 12,
			PlayerImmuneTicks = 12
		};

		FastFall = false;
	}

	public override bool IsActive(Player player)
	{
		return player.mount.Active && player.mount.Type == MountID.GolfCartSomebodySaveMe;
	}
}

public sealed class PogoStickMountStompAbility : VanillaStompAbility
{
	public override void SetStaticDefaults()
	{
		OnlyVisual = true;
	}

	public override bool IsActive(Player player)
	{
		return player.mount.Active && player.mount.Type == MountID.PogoStick;
	}
}

public sealed class DeadCellsDownDashStompAbility : VanillaStompAbility
{
	public override void SetStaticDefaults()
	{
		Stats = StompStrike.Default with
		{
			DamageType = DamageClass.Summon
		};
	}

	public override bool IsActive(Player player)
	{
		return player.GetJumpState<DeadCellsDownDashJump>().Active;
	}
}
