namespace Terraria.ModLoader.Default;

// This player handles special ExtraJump interactions, notably the Ram Rune's dash ability
internal sealed class ExtraJumpsPlayer : ModPlayer
{
	public override void OnExtraJumpStarted(ExtraJump jump, ref bool playSound)
	{
		// The extra jump system from Player.JumpMovement() seemed difficult to understand at first, but in practice any extra jump cancelled the Ram Rune's down dash ability.
		// Furthermore, performing any extra jump allows the Ram Rune ability to be used again.
		// That behaviour can be easily mimicked via this hook.

		if (jump is not DeadCellsDownDashJump)
			ExtraJumpLoader.StopJump<DeadCellsDownDashJump>(Player);
		else
			Player.GetJumpState<DeadCellsDownDashJump>().Available = true;
	}
}
