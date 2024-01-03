using System.Collections.Generic;

namespace Terraria.ModLoader;

[Autoload(false)]
public abstract class VanillaDash : Dash
{
	public sealed override Position GetDefaultPosition() => null;

	public sealed override IEnumerable<Position> GetModdedConstraints() => null;
}

/// <summary>
/// The <see cref="Dash"/> used by the Tabi and Master Ninja Gear accessories.
/// </summary>
public sealed class TabiDash : VanillaDash
{

}

/// <summary>
/// The <see cref="Dash"/> used by the Shield of Cthulhu accessory.
/// </summary>
public sealed class ShieldOfCthulhuDash : VanillaDash
{

}

/// <summary>
/// The <see cref="Dash"/> used by the Solar Flare armor set bonus.
/// </summary>
public sealed class SolarFlareDash : VanillaDash
{

}

/// <summary>
/// An unused and incomplete <see cref="Dash"/>, included for vanilla parity.
/// </summary>
public sealed class Unused4Dash : VanillaDash
{
	
}

/// <summary>
/// The <see cref="Dash"/> used by the Crystal Assassin armor set bonus.
/// </summary>
public sealed class CrystalAssassinDash : VanillaDash
{

}