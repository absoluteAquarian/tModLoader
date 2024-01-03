using System.Collections.Generic;
using Terraria.ID;

namespace Terraria.ModLoader;

/// <summary>
/// <see cref="Dash"/> is a singleton, defining the properties and behaviour of dashes.<br/>
/// </summary>
public abstract partial class Dash : ModType
{
	/// <inheritdoc cref="TabiDash"/>
	public static Dash Tabi { get; private set; } = new TabiDash() { Type = DashID.TabiAndMasterNinjaGear };

	/// <inheritdoc cref="ShieldOfCthulhuDash"/>
	public static Dash ShieldOfCthulhu { get; private set; } = new ShieldOfCthulhuDash() { Type = DashID.ShieldOfCthulhu };

	/// <inheritdoc cref="SolarFlareDash"/>
	public static Dash SolarFlare { get; private set; } = new SolarFlareDash() { Type = DashID.SolarFlare };

	/// <inheritdoc cref="Unused4Dash"/>
	public static Dash Unused4 { get; private set; } = new Unused4Dash() { Type = DashID.Unused4 };

	/// <inheritdoc cref="CrystalAssassinDash"/>
	public static Dash CrystalAssassin { get; private set; } = new CrystalAssassinDash() { Type = DashID.CrystalAssassin };

	/// <summary>
	/// The internal ID of this <see cref="Dash"/>.
	/// </summary>
	public int Type { get; internal set; }

	protected sealed override void Register()
	{
		ModTypeLookup<Dash>.Register(this);
		Type = DashLoader.Add(this);
	}

	public sealed override void SetupContent() => SetStaticDefaults();

	public override string ToString() => Name;

	/// <summary>
	/// Returns this dash's default position in regard to the vanilla dashes.  Make use of e.g. <see cref="Before"/>/<see cref="After"/>, and provide a dash.<br/><br/>
	/// 
	/// <b>NOTE:</b> The position must specify a vanilla <see cref="Dash"/> otherwise an exception will be thrown.
	/// </summary>
	public abstract Position GetDefaultPosition();

	/// <summary>
	/// Modded dashes are placed between vanilla dashes via <see cref="GetDefaultPosition"/> and, by default, are sorted in load order.<br/>
	/// This hook allows you to sort this dash before/after other modded dashes that were placed between the same two vanilla dashes.<br/>
	/// Example:
	/// <para>
	/// <c>yield return new After(ModContent.GetInstance&lt;SimpleDash&gt;());</c>
	/// </para>
	/// By default, this hook returns <see langword="null"/>, which indicates that this dash has no modded ordering constraints.
	/// </summary>
	/// <returns></returns>
	public virtual IEnumerable<Position> GetModdedConstraints() => null;
}
