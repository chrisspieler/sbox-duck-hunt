using Sandbox;
using Sandbox.Services;
using System.Collections.Generic;

[GameResource( "Boon", "boon", "A game pass with additional information specific to Ducc Hunt", Icon = "emoji_events")]
public class Boon : GamePass
{
	[Category( "Game Pass Setup" )]
	public bool IsOneTimePurchase { get; set; } = false;

	[Category( "Display Information" )]
	public string Description { get; set; } = string.Empty;

	public static IEnumerable<Boon> GetPurchaseable()
	{
		var allBoons = ResourceLibrary.GetAll<Boon>();

		foreach( var boon in allBoons )
		{
			if ( !boon.IsOneTimePurchase || !boon.Has() )
			{
				yield return boon;
			}
		}
	}

	public static IEnumerable<Boon> GetOwned()
	{
		var allBoons = ResourceLibrary.GetAll<Boon>();

		foreach( var boon in allBoons )
		{
			if ( boon.Has() )
			{
				yield return boon;
			}
		}
	}
}
