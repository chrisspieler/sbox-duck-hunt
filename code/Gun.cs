using Sandbox;
using Sandbox.Utility;
using System;
using System.Linq;

public sealed class Gun : Component
{
	[Property] public SoundEvent ShotSound { get; set; }
	[Property] public SoundEvent BoltSound { get; set; }
	[Property] public float FireDelay { get; set; } = 0.4f;
	[Property] public float HitRadius { get; set; } = 5f;
	[Property, Range( 0.01f, 10f )] public float CrosshairScale { get; set; } = 1f;
	[Property, Range( 0.01f, 10f )] public float CrosshairDotScale { get; set; } = 1f;
	[Property, Range( 0.01f, 10f )] public float CrosshairMarginScale { get; set; } = 1f;
	[Property, Range( 0f, 30f )] public float CrosshairFireMarginJump { get; set; } = 16f;
	[Property, Range( 0.01f, 10f )] public float CrosshairLineLengthScale { get; set; } = 1f;
	[Property, Range( 0.01f, 10f )] public float CrosshairLineWidthScale { get; set; } = 1f;

	public float FireDelayScale { get; set; } = 1f;
	public bool CanFire => _lastFireTime > FireDelay * FireDelayScale;

	private TimeSince _lastFireTime = float.PositiveInfinity;

	protected override void OnUpdate()
	{
		if ( CanFire && Input.Pressed("attack1") )
		{
			var fireRay = Scene.Camera.ScreenPixelToRay( Mouse.Position );
			Fire( fireRay );
			_lastFireTime = 0f;
		}
	}

	protected override void OnPreRender()
	{
		var state = GameState.Instance;
		var camera = Scene.Camera;
		if ( !state.IsValid() || !camera.IsValid() )
			return;

		camera.Hud.SetBlendMode( BlendMode.Normal );

		var lightColor = CanFire ? Color.White.WithAlpha( 1f ) : Color.White.WithAlpha( 0.6f );
		var darkColor = CanFire ? Color.Black.WithAlpha( 0.6f ) : Color.Black.WithAlpha( 0.05f );

		var crosshairDotPixels = 7f;
		var crosshairDotSize = crosshairDotPixels * CrosshairDotScale * CrosshairScale;
		var crosshairCenter = Mouse.Position;

		camera.Hud.DrawCircle( crosshairCenter, crosshairDotSize, darkColor );
		camera.Hud.DrawCircle( crosshairCenter, crosshairDotSize - 2f, lightColor );

		var fireFrac = MathX.LerpInverse( _lastFireTime, FireDelay, 0f );
		fireFrac = Easing.ExpoOut( fireFrac );
		var fireOffset = CrosshairFireMarginJump * fireFrac;
		var crosshairCenterMargin = 2f * CrosshairMarginScale * CrosshairScale + fireOffset;
		var lineLengthPixels = 15f;
		var lineLengthSize = lineLengthPixels * CrosshairLineLengthScale * CrosshairScale;
		var lineWidthPixels = 1f;
		var lineWidthSize = lineWidthPixels * CrosshairLineWidthScale * CrosshairScale;

		var innerLineLengthSize = MathF.Max( 4f, lineLengthSize );
		var outerLineLengthSize = MathF.Max( 5f, innerLineLengthSize + 1f );

		var innerLineWidthSize = MathF.Max( 1f, lineWidthSize );
		var outerLineWidthSize = MathF.Max( 2f, innerLineWidthSize + 1f );

		var top = crosshairCenter + Vector2.Up * ( crosshairDotSize + crosshairCenterMargin );
		camera.Hud.DrawLine( top, top + Vector2.Up * outerLineLengthSize, outerLineWidthSize, darkColor );
		camera.Hud.DrawLine( top - 1f, top + Vector2.Up * innerLineLengthSize, innerLineWidthSize, lightColor );

		var right = crosshairCenter + Vector2.Right * (crosshairDotSize + crosshairCenterMargin); ;
		camera.Hud.DrawLine( right, right + Vector2.Right * outerLineLengthSize, outerLineWidthSize, darkColor );
		camera.Hud.DrawLine( right - 1f, right + Vector2.Right * innerLineLengthSize, innerLineWidthSize, lightColor );

		var down = crosshairCenter + Vector2.Down * (crosshairDotSize + crosshairCenterMargin); ;
		camera.Hud.DrawLine( down, down + Vector2.Down * outerLineLengthSize, outerLineWidthSize, darkColor );
		camera.Hud.DrawLine( down + 1f, down + Vector2.Down * innerLineLengthSize, innerLineWidthSize, lightColor );

		var left = crosshairCenter + Vector2.Left * (crosshairDotSize + crosshairCenterMargin); ;
		camera.Hud.DrawLine( left, left + Vector2.Left * outerLineLengthSize, outerLineWidthSize, darkColor );
		camera.Hud.DrawLine( left - 1f, left + Vector2.Left * innerLineLengthSize, innerLineWidthSize, lightColor );
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
