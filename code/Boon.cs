using Sandbox;
using Sandbox.Services;
using System.Collections.Generic;
using System.Linq;

[GameResource( "Boon", "boon", "A game pass with additional information specific to Ducc Hunt", Icon = "emoji_events")]
public class Boon : GamePass
{
	[ConVar("boon_debug")] 
	public static bool Debug 
	{
		get => _debug;
		set
		{
			var changed = value != _debug;
			_debug = value;
			if ( changed && Game.IsPlaying )
			{
				RaiseBoonsChanged();
			}
		}
	}
	private static bool _debug = false;

	[Category( "Game Pass Setup" )]
	public bool IsOneTimePurchase { get; set; } = false;

	[Category( "Display Information" )]
	public string Description { get; set; } = string.Empty;

	public static bool Has( Boon boon )
	{
		if ( !boon.IsValid() )
			return false;

		var useDebug = Debug && Game.IsEditor;
		return useDebug ? DebugBoons.Contains( boon ) : boon.Has();
	}

	public static IEnumerable<Boon> GetPurchaseable()
	{
		var allBoons = ResourceLibrary.GetAll<Boon>();

		foreach( var boon in allBoons )
		{
			if ( !boon.IsOneTimePurchase || !Has( boon ) )
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
			if ( Has( boon ) )
			{
				yield return boon;
			}
		}
	}

	private static HashSet<Boon> DebugBoons { get; set; } = new();

	private static void RaiseBoonsChanged()
	{
		IBoonEvent.Post( x => x.OnBoonsChanged() );
	}

	[ConCmd("boon_add")]
	public static void DebugAdd( string boonName )
	{
		var useDebug = Debug && Game.IsEditor;
		if ( !useDebug || string.IsNullOrWhiteSpace( boonName ) )
			return;

		DebugBoons ??= new();

		if ( boonName == "all" )
		{
			var boons = ResourceLibrary.GetAll<Boon>().ToArray();
			Log.Info( $"Adding {boons.Length} debug boons." );
			foreach( var boon in boons )
			{
				DebugBoons.Add( boon );
			}
			RaiseBoonsChanged();
			return;
		}
		// Does not handle the case where boons with the same name are in different folders.
		var matchingBoon = ResourceLibrary.GetAll<Boon>()
			.Where( b => b.ResourceName == boonName )
			.FirstOrDefault();
		if ( matchingBoon != null )
		{
			DebugBoons.Add( matchingBoon );
			Log.Info( $"Add debug boon: {matchingBoon.ResourceName}" );
			RaiseBoonsChanged();
		}
	}

	[ConCmd("boon_clearall")]
	public static void DebugClearAll()
	{
		var useDebug = Debug && Game.IsEditor;
		if ( !useDebug )
			return;

		DebugBoons ??= new();

		if ( !DebugBoons.Any() )
		{
			Log.Info( "No debug boons to clear." );
			return;
		}

		Log.Info( $"Clearing {DebugBoons.Count} debug boons." );
		DebugBoons.Clear();
		RaiseBoonsChanged();
	}
}

public interface IBoonEvent : ISceneEvent<IBoonEvent>
{
	void OnBoonsChanged();
}
