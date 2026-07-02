using System.Collections.Generic;

namespace Terraria.ModLoader;

public static class StompAbilityLoader
{
	public static int AbilityCount => Abilities.Count;

	internal static readonly List<StompAbility> Abilities = new List<StompAbility>()
	{
		StompAbility.SlimeMounts
	};

	private static readonly int DefaultAbilityCount = Abilities.Count;

	static StompAbilityLoader()
	{
		RegisterDefaultAbilities();
	}

	internal static int Add(StompAbility ability)
	{
		Abilities.Add(ability);
		return Abilities.Count - 1;
	}

	public static StompAbility Get(int index) => index < 0 || index >= AbilityCount ? null : Abilities[index];

	internal static void Unload()
	{
		Abilities.RemoveRange(DefaultAbilityCount, AbilityCount - DefaultAbilityCount);
	}

	internal static void RegisterDefaultAbilities()
	{
		int i = 0;
		foreach (var ability in Abilities) {
			ability.Type = i++;
			ContentInstance.Register(ability);
			ModTypeLookup<StompAbility>.Register(ability);
		}
	}

	internal static void HandleStompCollisions(Player player)
	{
		if (player.velocity.Y <= 0f)
			return;

		foreach (StompAbility ability in Abilities) {
			if (!ability.OnlyVisual && player.StompNPCs(ability.Stats, out NPC victim))
				ability.OnStomp(player, victim);
		}
	}

	internal static void HandleFastFall(Player player, bool fallThroughPlatforms, bool ignorePlatforms)
	{
		if (player.velocity.Y == 0f)
			return;

		bool didWetCollision = IsWetColliding(player);

		foreach (StompAbility ability in Abilities) {
			if (ability.FastFall && (ability.FastFallInLiquids || !didWetCollision) && ability.CanFastFall(player))
				DoVerticalDryCollision(player, fallThroughPlatforms, ignorePlatforms);
		}
	}

	internal static bool ShouldBeFastFalling(Player player)
	{
		if (player.velocity.Y == 0f)
			return false;

		bool wouldDoWetCollision = IsWetColliding(player);

		foreach (StompAbility ability in Abilities) {
			if (ability.FastFall && (ability.FastFallInLiquids || !wouldDoWetCollision) && ability.CanFastFall(player))
				return true;
		}

		return false;
	}

	private static bool IsWetColliding(Player player)
	{
		// Context: These conditions are from Player.Update() after the call to PlayerLoader.PreUpdateMovement()
		return player.shimmerWet
			|| (player.honeyWet && !player.ignoreWater)
			|| (player.wet && !player.merman && !player.ignoreWater && !player.trident);
	}

	private static void DoVerticalDryCollision(Player player, bool fallThroughPlatforms, bool ignorePlatforms)
	{
		float saved = player.velocity.X;
		player.velocity.X = 0f;
		player.DryCollision(fallThroughPlatforms, ignorePlatforms);
		player.velocity.X = saved;
	}
}
