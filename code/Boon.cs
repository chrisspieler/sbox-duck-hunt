using Sandbox;
using Sandbox.Services;
using System;
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

	[Category( "Behavior" )]
	public Action OnGameStart { get; set; }
	[Category( "Behavior" )]
	public Action OnEnabled { get; set; }
	[Category( "Behavior" )]
	public Action OnDisabled { get; set; }


	public static bool Has( Boon boon )
	{
		if ( !boon.IsValid() )
			return false;

		return Debug ? DebugBoons.Contains( boon ) : boon.Has();
	}

	public static Boon Get( string resourceName )
	{
		return GetAll().FirstOrDefault( b => b.ResourceName == resourceName );
	}

	public static IEnumerable<Boon> GetAll()
	{
		return ResourceLibrary.GetAll<Boon>();
	}

	public static IEnumerable<Boon> GetEnabled()
	{
		return GetAll()
			.Where( b => _boonState.ContainsKey( b.ResourceName ) && _boonState[b.ResourceName] );
	}

	public static IEnumerable<Boon> GetPurchaseable()
	{
		return GetAll().Where( b => !b.IsOneTimePurchase || !Has( b ) );
	}

	public static IEnumerable<Boon> GetOwned()
	{
		return GetAll().Where( Has );
	}

	private static HashSet<Boon> DebugBoons { get; set; } = new();

	private static void RaiseBoonsChanged()
	{
		if ( Debug )
		{
			Log.Info( "Boons changed." );
		}
		IBoonEvent.Post( x => x.OnBoonsChanged() );
	}

	[ConCmd("boon_add")]
	public static void DebugAdd( string boonName )
	{
		if ( !Debug || string.IsNullOrWhiteSpace( boonName ) )
			return;

		DebugBoons ??= new();

		if ( boonName == "all" )
		{
			var boons = ResourceLibrary.GetAll<Boon>().ToArray();
			Log.Info( $"Adding {boons.Length} debug boons." );
			foreach( var boon in boons )
			{
				if ( !_boonState.ContainsKey( boon.ResourceName ) )
				{
					_boonState[boon.ResourceName] = false;
				}
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
			if ( !_boonState.ContainsKey( matchingBoon.ResourceName ) )
			{
				_boonState[matchingBoon.ResourceName] = false;
			}
			Log.Info( $"Add debug boon: {matchingBoon.ResourceName}" );
			RaiseBoonsChanged();
		}
	}

	[ConCmd("boon_clearall")]
	public static void DebugClearAll()
	{
		if ( !Debug )
			return;

		DebugBoons ??= new();

		if ( !DebugBoons.Any() )
		{
			Log.Info( "No debug boons to clear." );
			return;
		}

		Log.Info( $"Clearing {DebugBoons.Count} debug boons." );
		foreach( var boonName in _boonState.Keys )
		{
			if ( _boonState[boonName] )
			{
				Disable( boonName );
			}
		}
		DebugBoons.Clear();
		_boonState.Clear();
		RaiseBoonsChanged();
	}

	private static Dictionary<string, bool> _boonState = new();

	private static void OnBoonToggled( Boon boon, bool enabled )
	{
		if ( Debug )
		{
			Log.Info( $"Boon toggled: {boon.ResourceName} - {enabled}" );
		}
		if ( enabled )
		{
			boon.OnEnabled?.Invoke();
		}
		else
		{
			boon.OnDisabled?.Invoke();
		}
		IBoonEvent.Post( x => x.OnBoonToggled( boon, enabled ) );
	}

	[ConCmd("boon_toggle")]
	public static void Toggle( string boonName )
	{
		SetEnabledState( boonName, !GetEnabledState( boonName ) );
	}

	[ConCmd("boon_enable")]
	public static void Enable( string boonName )
	{
		SetEnabledState( boonName, true );
	}

	[ConCmd( "boon_disable" )]
	public static void Disable( string boonName )
	{
		SetEnabledState( boonName, false );
	}
	
	internal static void InitializeBoonStates()
	{
		_boonState.Clear();
		foreach( var boon in GetOwned() )
		{
			_boonState[boon.ResourceName] = false;
		}
	}

	public static bool GetEnabledState( string boonName )
	{
		if ( !_boonState.ContainsKey( boonName ) )
			return false;

		return _boonState[boonName];
	}

	public static void SetEnabledState( string boonName, bool enabled )
	{
		var boon = GetOwned()
			.FirstOrDefault( b => b.ResourceName.ToLower() == boonName.ToLower() );

		if ( !boon.IsValid() )
		{
			Log.Info( $"No boon found matching: {boonName}" );
			return;
		}

		if ( !_boonState.ContainsKey( boonName ) )
		{
			_boonState[boonName] = enabled;
			OnBoonToggled( boon, enabled );
			return;
		}

		var changed = _boonState[boonName] != enabled;
		_boonState[boonName] = enabled;
		if ( changed )
		{
			OnBoonToggled( boon, enabled );
		}
	}
}

public interface IBoonEvent : ISceneEvent<IBoonEvent>
{
	void OnBoonsChanged();
	void OnBoonToggled( Boon boon, bool enabled );
}
