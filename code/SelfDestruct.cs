using Sandbox;

public sealed class SelfDestruct : Component
{
	[Property] public float Delay { get; set; } = 2f;

	private TimeSince _creation;

	protected override void OnEnabled()
	{
		_creation = 0f;
	}

	protected override void OnUpdate()
	{
		if ( _creation > Delay )
		{
			GameObject.Destroy();
		}
	}
}
