using Sandbox;

public sealed class Breakable : Component
{
	/// <summary>
	/// A sound that plays when this object breaks.
	/// </summary>
	[Property] public SoundEvent BreakSound { get; set; }
	/// <summary>
	/// A prefab that spawns when this object breaks.
	/// </summary>
	[Property] public GameObject BreakDebris { get; set; }

	protected override void OnEnabled()
	{
		GameObject.Tags.Add( "breakable" );
	}

	protected override void OnDisabled()
	{
		GameObject.Tags.Remove( "breakable" );
	}

	public void Break()
	{
		if ( BreakSound is not null )
		{
			Sound.Play( BreakSound, Transform.Position );
		}
		if ( BreakDebris is not null )
		{
			var debris = BreakDebris.Clone( Transform.World );
			var rb = Components.Get<Rigidbody>();
			if ( rb is not null )
			{
				var particleEffect = debris.Components.Get<ParticleEffect>( FindMode.EverythingInSelfAndChildren );
				particleEffect.ForceDirection = rb.Velocity.WithZ( rb.Velocity.z - 800f );
			}
			debris.Enabled = true;
		}
		GameObject.Destroy();
	}
}
