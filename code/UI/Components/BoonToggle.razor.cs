using Sandbox;
using Sandbox.UI;

public partial class BoonToggle : Panel
{
	public Boon Boon { get; set; }
	public SwitchControl Toggle { get; set; }
	public bool Value 
	{
		get => _value;
		set
		{
			_value = value;
			if ( Toggle.IsValid() )
			{
				Toggle.Value = value;
			}
		}
	}
	private bool _value;

	protected override void OnAfterTreeRender( bool firstTime )
	{
		base.OnAfterTreeRender( firstTime );

		if ( !firstTime )
			return;

		if ( Boon is null || !Toggle.IsValid() )
		{
			Delete( true );
			return;
		}

		Toggle.Value = Boon.GetEnabledState( Boon.ResourceName );
		_value = Toggle.Value;
		Toggle.OnValueChanged += v =>
		{
			_value = v;
			Boon.SetEnabledState( Boon.ResourceName, v );
		};
	}
}
