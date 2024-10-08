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
			
			var fireRay = Scene.Camera.ScreenPixelToRay( Mouse.Position );
			Fire( fireRay );
			_lastFireTime = 0f;
		}
	}

	public void Fire( Ray aimRay )
	{
		if ( ShotSound is not null )
		{
			Sound.Play( ShotSound, Scene.Camera.WorldPosition );
		}
		if ( BoltSound is not null )
		{
			Sound.Play( BoltSound, Scene.Camera.WorldPosition );
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
				GameState.Instance.OnDuckShot( breakable.WorldPosition );
			}
		}
		else
		{
			var hitPosition = aimRay.Project( 750f );
			GameState.Instance.OnMissedShot( hitPosition );
		}
	}
}
