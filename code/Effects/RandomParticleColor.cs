using Sandbox;
using System.Collections.Generic;
using System.Linq;

public sealed class RandomParticleColor : Component
{
	[Property] public ParticleEffect Effect { get; set; }
	[Property] public List<Color> ColorList { get; set; } = new();

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

		return Color.Random.ToHsv().WithValue( 1f ).WithSaturation( 0.85f );
	}
}
