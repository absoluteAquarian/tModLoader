namespace Terraria.ModLoader;

partial class Dash
{
	public abstract class Position
	{
		public Dash Target { get; protected init; }
	}

	public sealed class Before : Position
	{
		public Before(Dash parent)
		{
			Target = parent;
		}
	}

	public sealed class After : Position
	{
		public After(Dash parent)
		{
			Target = parent;
		}
	}
}
