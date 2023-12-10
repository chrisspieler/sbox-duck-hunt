using Sandbox;

public sealed class Gun : Component
{
	[Property] public SoundEvent ShotSound { get; set; }
	[Property] public SoundEvent BoltSound { get; set; }
	[Property] public float FireRate { get; set; } = 1f;
	[Property] public float HitRadius { get; set; } = 5f;

	private TimeSince _lastFireTime = float.PositiveInfinity;
	protected override void OnUpdate()
	{
		if ( _lastFireTime > FireRate && Input.Pressed("attack1") )
		{
			var fireDirection = Screen.GetDirection( Mouse.Position );
			var fireRay = new Ray( Camera.Main.Position, fireDirection );
			Fire( fireRay );
			_lastFireTime = 0f;
		}
	}

	public void Fire( Ray aimRay )
	{
		if ( ShotSound is not null )
		{
			Sound.Play( ShotSound, Camera.Main.Position );
		}
		if ( BoltSound is not null )
		{
			Sound.Play( BoltSound, Camera.Main.Position );
		}
		var tr = Scene.Trace
			.Ray( aimRay, 5000f )
			.Radius( HitRadius )
			.WithTag( "breakable" )
			.Run();
		if ( tr.Hit )
		{
			var breakable = tr.GameObject.Components.Get<Breakable>();
			breakable.Break();
			if ( tr.GameObject.Tags.Has( "target" ) )
			{
				GameState.Instance.OnDuckShot();
			}
		}
		else
		{
			GameState.Instance.OnMissedShot();
		}
	}
}
