using Sandbox;
using System;

public sealed class GameState : Component
{
	public static GameState Instance { get; private set; }

	[Property] public Hud GameHud { get; set; }
	public int Points { get; private set; }

	public GameState()
	{
		Instance = this;
	}

	public void AddPoints( int points )
	{
		Points += Math.Max( 0, points );
		GameHud.Points = Points;
	}

	public void OnDuckShot()
	{
		GameHud.DucksHunted++;
	}
}
