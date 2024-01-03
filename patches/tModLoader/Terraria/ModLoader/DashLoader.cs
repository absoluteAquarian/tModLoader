using System;
using System.Collections.Generic;
using System.Linq;

namespace Terraria.ModLoader;

public static class DashLoader
{
	public static int DashCount => Dashes.Count;

	// Order is the vanilla priority for using dashes
	internal static readonly List<Dash> Dashes = new List<Dash>() {
		Dash.Tabi,
		Dash.ShieldOfCthulhu,
		Dash.SolarFlare,
		Dash.Unused4,
		Dash.CrystalAssassin
	};

	private static readonly int DefaultDashCount = Dashes.Count;

	private static IEnumerable<Dash> ModdedDashes => Dashes.Skip(DefaultDashCount);

	private static Dash[] orderedDashes;

	public static IReadOnlyList<Dash> OrderedDashes => orderedDashes;

	static DashLoader()
	{
		RegisterDefaultDashes();
	}

	internal static int Add(Dash dash)
	{
		Dashes.Add(dash);
		return Dashes.Count - 1;
	}

	public static Dash Get(int type) {
		// Invalid or "no dash" ID
		if (type < 1 || type >= DashCount)
			return null;


	}

	internal static void Unload()
	{
		Dashes.RemoveRange(DefaultDashCount, DashCount - DefaultDashCount);
	}

	internal static void ResizeArrays()
	{
		if (!ModdedDashes.Any()) {
			// Vanilla extra jumps are already sorted in the collection; any additional work would be a moot point
			orderedDashes = Dashes.ToArray();
			return;
		}

		// Between each vanilla extra jump, before the first jump and after the last jump exists a "slot"
		// Modded jumps are added to a "slot", and then the slots are filled in load order by default
		// Modders can use "ModDash::GetModdedConstraints()" to facilitate sorting within a slot
		var sortingSlots = new List<Dash>[DefaultDashCount + 1];
		for (int i = 0; i < sortingSlots.Length; i++)
			sortingSlots[i] = new();

		// Initially put the modded extra jumps in load order
		foreach (Dash dash in ModdedDashes) {
			var position = dash.GetDefaultPosition();

			switch (position) {
				case Dash.After after:
					if (after.Target is not null and not VanillaDash)
						throw new ArgumentException($"ModDash {dash} did not refer to a vanilla ModDash in GetDefaultPosition()");

					int afterParent = after.Target?.Type is { } afterType ? afterType + 1 : 0;

					sortingSlots[afterParent].Add(dash);
					break;
				case Dash.Before before:
					if (before.Target is not null and not VanillaDash)
						throw new ArgumentException($"ModDash {dash} did not refer to a vanilla ModDash in GetDefaultPosition()");

					int beforeParent = before.Target?.Type is { } beforeType ? beforeType : sortingSlots.Length - 1;

					sortingSlots[beforeParent].Add(dash);
					break;
				default:
					throw new ArgumentException($"ModDash {dash} has unknown Position {position}");
			}
		}

		// Sort the modded jumps per slot
		List<Dash> sorted = new();

		for (int i = 0; i < DefaultDashCount + 1; i++) {
			var elements = sortingSlots[i];
			var sort = new TopoSort<Dash>(elements,
				j => j.GetModdedConstraints()?.OfType<Dash.After>().Select(static a => a.Target).Where(elements.Contains) ?? Array.Empty<Dash>(),
				j => j.GetModdedConstraints()?.OfType<Dash.Before>().Select(static b => b.Target).Where(elements.Contains) ?? Array.Empty<Dash>());

			foreach (Dash jump in sort.Sort()) {
				sorted.Add(jump);
			}

			if (i < DefaultDashCount)
				sorted.Add(Dashes[i]);
		}

		orderedDashes = sorted.ToArray();
	}

	internal static void RegisterDefaultDashes()
	{
		int i = 0;
		foreach (var dash in Dashes) {
			ContentInstance.Register(dash);
			ModTypeLookup<Dash>.Register(dash);
		}
	}
}
