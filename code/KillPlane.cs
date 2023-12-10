using Sandbox;

public sealed class KillPlane : Component, Component.ICollisionListener
{
	[Property] public TagSet TargetTags { get; set; }

	public void OnCollisionStart( Collision other )
	{
		foreach( var tag in TargetTags.TryGetAll() )
		{
			if ( other.Other.GameObject.Tags.Has( tag ) )
			{
				other.Other.GameObject.Destroy();
			}
		}
	}

	public void OnCollisionStop( CollisionStop other ) { }
	public void OnCollisionUpdate( Collision other ) { }
}
