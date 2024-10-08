using Sandbox;

public sealed class Launcher : Component
{
	[Property] public GameObject Prefab { get; set; }
	[Property, Range( 0, 2 )] public float DirectionRandomness { get; set; } = 0.5f;
	[Property] public float LaunchSpeed { get; set; } = 20f;
	[Property] public float LaunchSpin { get; set; } = 2f;

	protected override void DrawGizmos()
	{
		Gizmo.Draw.Color = Color.Red;
		var lineEnd = WorldPosition + WorldRotation.Forward * LaunchSpeed;
		Gizmo.Draw.Line( WorldPosition, lineEnd );
		Gizmo.Draw.LineSphere( new Sphere( lineEnd, 10f ) );
	}

	public void Launch()
	{
		if ( Prefab == null )
			return;

		var go = Prefab.Clone( Transform.World );
		go.Enabled = true;
		var rb = go.Components.GetOrCreate<Rigidbody>();
		var randomDirection = (WorldRotation.Forward + Vector3.Random * DirectionRandomness).Normal;
		rb.Velocity = randomDirection * LaunchSpeed;
		rb.AngularVelocity = Vector3.Random * LaunchSpin;
	}
}
