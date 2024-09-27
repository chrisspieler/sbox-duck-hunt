using Sandbox;
using Sandbox.Utility;
using System;

public partial class FloatingScoreText : PanelComponent
{
	[Property] public string Text { get; set; } = "Hello, fellas!";
	[Property] public string TextClass { get; set; }
	[Property] public float FloatAwayDistance { get; set; } = 200f;
	[Property] public float FloatAwayTime { get; set; } = 1.5f;
	[Property, Range( 0, 1 )] public float FadeInStart { get; set; } = 0.25f;
	[Property, Range( 0, 50, 1)] public float SwayFactor { get; set; } = 5f;
	[Property] public Vector3 StartPosition { get; set; }

	private TimeSince _floatStart;

	/// <summary>
	/// the hash determines if the system should be rebuilt. If it changes, it will be rebuilt
	/// </summary>
	protected override int BuildHash() => HashCode.Combine( Text, TextClass );

	protected override void OnEnabled()
	{
		base.OnEnabled();

		_floatStart = 0f;
	}

	protected override void OnUpdate()
	{
		var targetHeight = StartPosition.z + FloatAwayDistance;
		var currentProgress = MathX.LerpInverse( _floatStart, 0f, FloatAwayTime );
		var easedProgress = Easing.QuadraticIn( currentProgress );
		var currentHeight = MathX.Lerp( StartPosition.z, targetHeight, easedProgress );
		Transform.Position = StartPosition
			.WithY( StartPosition.y + MathF.Sin( Time.Now * MathF.PI ) * SwayFactor )
			.WithZ( currentHeight );
		
		if ( easedProgress > FadeInStart )
		{
			Panel.Style.Opacity = MathX.Remap( easedProgress, FadeInStart, 1f, 1f, 0f );
		}
	}
}
