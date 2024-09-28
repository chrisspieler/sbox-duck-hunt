using Sandbox;
using Sandbox.Services;
using Sandbox.UI;

public partial class BoonToggle : Panel
{
	public GamePass Boon { get; set; }
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

		if ( !Boon.IsValid() || !Toggle.IsValid() )
		{
			Delete( true );
			return;
		}

		Toggle.OnValueChanged += v => _value = v;
		_value = Monetization.Has( Boon );
	}
}
