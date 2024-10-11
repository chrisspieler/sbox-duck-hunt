using Sandbox;
using Sandbox.Services;
using System;
using System.Collections.Generic;
using System.Linq;

public sealed class GameState : Component
{
	public static GameState Instance { get; private set; }

	[Property] public Hud GameHud { get; set; }
	[Property] public float MaxGameTime { get; set; } = 99f;
	[Property] public float MinLaunchDelay { get; set; } = 1f;
	[Property] public float MaxLaunchDelay { get; set; } = 3.5f;
	[Property] public int MaxCombo { get; set; } = 5;
	[Property] public float ComboMultiplierBonus { get; set; } = 0.1f;
	[Property] public GameObject FloatingScorePrefab { get; set; }
	public float ComboMultiplier => 1f + MathF.Min( MaxCombo, Combo ) * ComboMultiplierBonus;
	public int Points { get; private set; }
	public int DucksHunted { get; private set; }
	public int CurrentSessionGameCount { get; private set; } = 0;
	public int Combo { get; private set; } = 0;
	public float GameTime { get; private set; } = 0f;
	public bool IsGameActive { get; private set; } = false;
	public int HighestCombo { get; private set; }
	public int ShotsFired { get; private set; }
	public int MissedShots { get; private set; }
	public float Accuracy => ShotsFired == 0 ? 0f : 1f - (float)MissedShots / ShotsFired;
	public bool GotPersonalBestPoints { get; private set; } = false;
	public bool GotPersonalBestDucksHunted { get; private set; } = false;
	public bool GotPersonalBestCombo { get; private set; } = false;
	public DuckStats OfflineStats { get; private set; }

	public event Action GameStarted;
	public event Action GameOver;

	private List<Launcher> _launchers = new();
	private TimeUntil _nextLaunch;

	public GameState()
	{
		Instance = this;
	}

	protected override void OnStart()
	{
		_launchers = Scene.GetAllComponents<Launcher>().ToList();
		OfflineStats = DuckStats.Load();
	}

	protected override void OnUpdate()
	{
		if ( !IsGameActive )
			return;

		if ( Input.EscapePressed )
		{
			GameTime = 0f;
		}

		GameTime -= Time.Delta;
		GameTime = MathF.Max( GameTime, 0f );
		if ( GameTime == 0f )
		{
			EndGame();
		}

		if ( !_nextLaunch )
			return;

		var launcher = _launchers.OrderBy( _ => Guid.NewGuid() ).First();
		launcher.Launch();
		_nextLaunch = Game.Random.Float( MinLaunchDelay, MaxLaunchDelay );
	}

	public void StartGame()
	{
		OfflineStats.TotalGamesPlayed++;
		CurrentSessionGameCount++;
		IsGameActive = true;
		GameTime = MaxGameTime;
		Combo = 0;
		HighestCombo = 0;
		Points = 0;
		DucksHunted = 0;
		ShotsFired = 0;
		MissedShots = 0;
		_nextLaunch = Game.Random.Float( MinLaunchDelay, MaxLaunchDelay );
		foreach( var boon in Boon.GetEnabled() )
		{
			boon.OnGameStart?.Invoke();
		}
		GameStarted?.Invoke();
	}

	public void EndGame()
	{
		IsGameActive = false;
		Stats.SetValue( "score", Points );
		GotPersonalBestPoints = Points > OfflineStats.HighestPoints;
		if ( GotPersonalBestPoints )
		{
			OfflineStats.HighestPoints = Points;
		}
		Stats.SetValue( "ducks-hunted", DucksHunted );
		GotPersonalBestDucksHunted = DucksHunted > OfflineStats.HighestDucksHunted;
		if ( GotPersonalBestDucksHunted )
		{
			OfflineStats.HighestDucksHunted = DucksHunted;
		}
		Stats.SetValue( "highest-combo", HighestCombo );
		GotPersonalBestCombo = HighestCombo > OfflineStats.HighestCombo;
		if ( GotPersonalBestCombo )
		{
			OfflineStats.HighestCombo = HighestCombo;
		}
		OfflineStats.Save();
		Stats.Flush();
		GameOver?.Invoke();
	}

	public void AddPoints( int points )
	{
		Points += Math.Max( 0, points );
	}

	public void RemovePoints( int points )
	{
		Points -= Math.Max( 0, points );
		Points = Math.Max( 0, Points );
	}

	public void OnDuckShot( Vector3 position )
	{
		DucksHunted++;
		int pointsAwarded = (int)( 25f * ComboMultiplier );
		AddPoints( pointsAwarded );
		Combo++;
		if ( Combo == MaxCombo )
		{
			SpawnFloatingText( "Max Combo!", "positive", position + Vector3.Down * 18, 0.75f );
		}
		if ( Combo > HighestCombo )
		{
			HighestCombo = Combo;
		}
		ShotsFired++;
		SpawnScoreText( pointsAwarded, position );
	}

	public void OnMissedShot( Vector3 position )
	{
		if ( Combo > 1 )
		{
			SpawnFloatingText( "Combo Dropped!", "negative", position + Vector3.Down * 18, 0.75f );
		}
		Combo = 0;
		RemovePoints( 20 );
		MissedShots++;
		ShotsFired++;
		SpawnScoreText( -20, position );
		
	}

	private void SpawnScoreText( int points, Vector3 position )
	{
		var sign = points > 0 ? "+" : "";
		var textClass = points > 0 ? "positive" : "negative";
		SpawnFloatingText( $"{sign}{points}", textClass, position );
	}

	private void SpawnFloatingText( string text, string textClass, Vector3 position, float scale = 1f )
	{
		var floatingScore = FloatingScorePrefab.Clone( position );
		floatingScore.Enabled = true;
		floatingScore.WorldScale = scale;
		var scoreText = floatingScore.Components.Get<FloatingScoreText>();
		scoreText.Text = text;
		scoreText.TextClass = textClass;
		scoreText.StartPosition = position;
	}
}
