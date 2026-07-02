using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Terraria.ModLoader;

public static class ExtraJumpLoader
{
	public static int ExtraJumpCount => ExtraJumps.Count;

	// Order is the vanilla priority when consuming the extra jumps
	internal static readonly List<ExtraJump> ExtraJumps = new List<ExtraJump>()
	{
		ExtraJump.Flipper,
		ExtraJump.BasiliskMount,
		ExtraJump.GoatMount,
		ExtraJump.SantankMount,
		ExtraJump.UnicornMount,
		ExtraJump.DeadCellsDownDash,
		ExtraJump.SandstormInABottle,
		ExtraJump.BlizzardInABottle,
		ExtraJump.FartInAJar,
		ExtraJump.TsunamiInABottle,
		ExtraJump.CloudInABottle
	};

	private static readonly int DefaultExtraJumpCount = ExtraJumps.Count;

	private static IEnumerable<ExtraJump> ModdedExtraJumps => ExtraJumps.Skip(DefaultExtraJumpCount);

	private static ExtraJump[] orderedJumps;

	public static IReadOnlyList<ExtraJump> OrderedJumps => orderedJumps;

	static ExtraJumpLoader()
	{
		RegisterDefaultJumps();
	}

	internal static int Add(ExtraJump jump)
	{
		ExtraJumps.Add(jump);
		return ExtraJumps.Count - 1;
	}

	public static ExtraJump Get(int index) => index < 0 || index >= ExtraJumpCount ? null : ExtraJumps[index];

	internal static void Unload()
	{
		ExtraJumps.RemoveRange(DefaultExtraJumpCount, ExtraJumpCount - DefaultExtraJumpCount);
	}

	internal static void ResizeArrays()
	{
		if (!ModdedExtraJumps.Any()) {
			// Vanilla extra jumps are already sorted in the collection; any additional work would be a moot point
			orderedJumps = ExtraJumps.ToArray();
			return;
		}

		// Between each vanilla extra jump, before the first jump and after the last jump exists a "slot"
		// Modded jumps are added to a "slot", and then the slots are filled in load order by default
		// Modders can use "ModExtraJump::GetModdedConstraints()" to facilitate sorting within a slot
		var sortingSlots = new List<ExtraJump>[DefaultExtraJumpCount + 1];
		for (int i = 0; i < sortingSlots.Length; i++)
			sortingSlots[i] = new();

		// Initially put the modded extra jumps in load order
		foreach (ExtraJump jump in ModdedExtraJumps) {
			var position = jump.GetDefaultPosition();

			switch (position) {
				case ExtraJump.After after:
					if (after.Target is not null and not VanillaExtraJump)
						throw new ArgumentException($"ExtraJump {jump} did not refer to a vanilla ExtraJump in GetDefaultPosition()");

					int afterParent = after.Target?.Type is { } afterType ? afterType + 1 : 0;

					sortingSlots[afterParent].Add(jump);
					break;
				case ExtraJump.Before before:
					if (before.Target is not null and not VanillaExtraJump)
						throw new ArgumentException($"ExtraJump {jump} did not refer to a vanilla ExtraJump in GetDefaultPosition()");

					int beforeParent = before.Target?.Type is { } beforeType ? beforeType : sortingSlots.Length - 1;

					sortingSlots[beforeParent].Add(jump);
					break;
				default:
					throw new ArgumentException($"ExtraJump {jump} has unknown Position {position}");
			}
		}

		// Sort the modded jumps per slot
		List<ExtraJump> sorted = new();

		for (int i = 0; i < DefaultExtraJumpCount + 1; i++) {
			var elements = sortingSlots[i];
			var sort = new TopoSort<ExtraJump>(elements,
				j => j.GetModdedConstraints()?.OfType<ExtraJump.After>().Select(static a => a.Target).Where(elements.Contains) ?? Array.Empty<ExtraJump>(),
				j => j.GetModdedConstraints()?.OfType<ExtraJump.Before>().Select(static b => b.Target).Where(elements.Contains) ?? Array.Empty<ExtraJump>());

			foreach (ExtraJump jump in sort.Sort()) {
				sorted.Add(jump);
			}

			if (i < DefaultExtraJumpCount)
				sorted.Add(ExtraJumps[i]);
		}

		orderedJumps = sorted.ToArray();
	}

	internal static void RegisterDefaultJumps()
	{
		int i = 0;
		foreach (var jump in ExtraJumps) {
			jump.Type = i++;
			ContentInstance.Register(jump);
			ModTypeLookup<ExtraJump>.Register(jump);
		}
	}

	internal static bool TryGetAvailableJump(Player player, bool checkingCarpetFlight, out ExtraJump availableJump)
	{
		foreach (ExtraJump jump in orderedJumps) {
			if (checkingCarpetFlight && !jump.OverridesCarpetFlight)
				continue;

			if (player.GetJumpState(jump).Available && jump.CanStart(player) && PlayerLoader.CanStartExtraJump(jump, player)) {
				availableJump = jump;
				return true;
			}
		}

		availableJump = null;
		return false;
	}

	/// <summary>
	/// Attempts to get the extra jump that is being performed by <paramref name="player"/>.
	/// </summary>
	/// <param name="player">The player instance</param>
	/// <param name="activeJump">The active jump instance, if any.</param>
	/// <returns><see langword="true"/> if an extra jump was being performed; otherwise, <see langword="false"/>.</returns>
	public static bool TryGetActiveJump(Player player, out ExtraJump activeJump)
	{
		foreach (ExtraJump jump in orderedJumps) {
			if (player.GetJumpState(jump).Active) {
				activeJump = jump;
				return true;
			}
		}

		activeJump = null;
		return false;
	}

	public static void UpdateHorizontalSpeeds(Player player)
	{
		foreach (ExtraJump moddedExtraJump in orderedJumps) {
			ref ExtraJumpState extraJump = ref player.GetJumpState(moddedExtraJump);
			if (extraJump.Active)
				moddedExtraJump.UpdateHorizontalSpeeds(player);
		}
	}

	public static void JumpVisuals(Player player)
	{
		foreach (ExtraJump jump in orderedJumps) {
			ref ExtraJumpState state = ref player.GetJumpState(jump);
			if (state.Active && jump.CanShowVisuals(player) && PlayerLoader.CanShowExtraJumpVisuals(jump, player)) {
				jump.ShowVisuals(player);
				PlayerLoader.ExtraJumpVisuals(jump, player);
			}
		}
	}

	public static void ProcessJumps(Player player)
	{
		if (TryGetAvailableJump(player, false, out ExtraJump jump))
		{
			player.GetJumpState(jump).Start();
			PerformJump(jump, player);
		}
	}

	public static void RefreshJumps(Player player)
	{
		foreach (ExtraJump jump in orderedJumps) {
			ref ExtraJumpState state = ref player.GetJumpState(jump);
			if (state.Enabled) {
				jump.OnRefreshed(player);
				PlayerLoader.OnExtraJumpRefreshed(jump, player);
				state.Available = true;
			}
		}
	}

	/// <summary>
	/// Stops the extra jump being performed by <paramref name="player"/>.
	/// </summary>
	/// <param name="player">The player instance</param>
	/// <returns><see langword="true"/> if an extra jump was stopped; otherwise, <see langword="false"/>.</returns>
	public static bool StopActiveJump(Player player)
	{
		bool anyJumpCancelled = false;

		foreach (ExtraJump jump in orderedJumps) {
			if (StopJump(jump, player))
				anyJumpCancelled = true;
		}

		return anyJumpCancelled;
	}

	internal static bool ClearExpiredJumps(Player player)
	{
		bool anyJumpCancelled = false;

		foreach (ExtraJump jump in orderedJumps) {
			if (!jump.ClearedWhenTimerExpires)
				continue;

			if (StopJump(jump, player))
				anyJumpCancelled = true;
		}

		return anyJumpCancelled;
	}

	/// <summary>
	/// Attempts to stop <paramref name="jump"/> if it is being performed by <paramref name="player"/>.
	/// </summary>
	/// <param name="jump">The jump instance</param>
	/// <param name="player">The player instance</param>
	/// <returns><see langword="true"/> if <paramref name="player"/> was performing <paramref name="jump"/>; otherwise, <see langword="false"/>.</returns>
	public static bool StopJump(ExtraJump jump, Player player) {
		ref ExtraJumpState state = ref player.GetJumpState(jump);

		if (state.Active) {
			HandleEndOfJump(jump, player);
			return true;
		}

		return false;
	}

	/// <summary>
	/// Attempts to stop the <typeparamref name="T"/> jump instance if it is being performed by <paramref name="player"/>.
	/// </summary>
	/// <param name="player">The player instance</param>
	/// <returns><see langword="true"/> if <paramref name="player"/> was performing the <typeparamref name="T"/> jump instance; otherwise, <see langword="false"/>.</returns>
	public static bool StopJump<T>(Player player) where T : ExtraJump => StopJump(ModContent.GetInstance<T>(), player);

	internal static void ResetEnableFlags(Player player)
	{
		foreach (ExtraJump jump in ExtraJumps) {
			player.GetJumpState(jump).ResetEnabled();
		}
	}

	internal static void ConsumeAndStopUnavailableJumps(Player player)
	{
		foreach (ExtraJump jump in ExtraJumps) {
			player.GetJumpState(jump).CommitEnabledState(out bool jumpEnded);

			// Force the jump to stop early if unequipped or disabled
			if (jumpEnded) {
				HandleEndOfJump(jump, player);
				player.jump = 0;
			}
		}
	}

	/// <summary>
	/// Sets <see cref="ExtraJumpState.Available"/> for all extra jumps on <paramref name="player"/> to <see langword="false"/>.
	/// </summary>
	/// <param name="player">The player instance</param>
	public static void ConsumeAllJumps(Player player)
	{
		foreach (ExtraJump jump in ExtraJumps) {
			player.GetJumpState(jump).Available = false;
		}
	}

	internal static float GetJumpAscentSpeed(Player player)
	{
		if (TryGetActiveJump(player, out ExtraJump activeJump))
			return GetJumpAscentSpeed(activeJump, player);

		return Player.jumpSpeed;
	}

	private static float GetJumpAscentSpeed(ExtraJump jump, Player player)
	{
		StatModifier speed = StatModifier.Default;

		jump.ModifyAscentSpeed(player, ref speed);
		PlayerLoader.ModifyExtraJumpAscentSpeed(jump, player, ref speed);

		return speed.ApplyTo(Player.jumpSpeed);
	}

	private static void PerformJump(ExtraJump jump, Player player)
	{
		// Set the jump duration
		float duration = jump.GetDurationMultiplier(player);

		if (duration > 0f) {
			StatModifier modifier = StatModifier.Default;
			PlayerLoader.ModifyExtraJumpDurationMultiplier(jump, player, ref modifier);
			duration = modifier.ApplyTo(duration);
		}

		duration = Math.Max(duration, 0f);

		// This was solely implemented for the Ram Rune dash, but would be useful for mods who want non-conventional jumps
		if (jump.PreStart(player, duration))
			player.velocity.Y = -1 * GetJumpAscentSpeed(jump, player) * player.gravDir;

		player.jump = (int)(Player.jumpHeight * duration);

		bool playSound = true;
		jump.OnStarted(player, ref playSound);
		PlayerLoader.OnExtraJumpStarted(jump, player, ref playSound);

		if (playSound)
			SoundEngine.PlaySound(16, (int)player.position.X, (int)player.position.Y);
	}

	private static void HandleEndOfJump(ExtraJump jump, Player player)
	{
		jump.OnEnded(player);
		PlayerLoader.OnExtraJumpEnded(jump, player);
		player.GetJumpState(jump).Stop();
	}
}
