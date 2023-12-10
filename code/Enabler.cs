using Sandbox;

public sealed class Enabler : Component
{
	[Property] public float Delay { get; set; }
	[Property] public GameObject TargetGameObject { get; set; }
	[Property] public Component TargetComponent { get; set; }

	private TimeSince _creation = 0f;

	protected override void OnUpdate()
	{
		if ( _creation > Delay )
		{
			if ( TargetGameObject is not null )
			{
				TargetGameObject.Enabled = true;
			}
			if ( TargetComponent is not null )
			{
				TargetComponent.Enabled = true;
			}
		}
	}
}
