using Sandbox;
using System.Collections.Generic;
using System.Linq;

public sealed class RandomParticleColor : Component
{
	[Property] public ParticleEffect Effect { get; set; }
	[Property] public List<Color> ColorList { get; set; } = new();
	[Property] public RangedFloat HueRange { get; set; }

	protected override void OnStart()
	{
		if ( !Effect.IsValid() )
			return;

		Effect.Tint = GetRandomColor();
		Destroy();
	}

	private Color GetRandomColor()
	{
		if ( ColorList.Any() )
		{
			return Game.Random.FromList( ColorList );
		}

		var hue = HueRange.GetValue();
		return new ColorHsv( hue, 0.9f, 0.75f );
	}
}
