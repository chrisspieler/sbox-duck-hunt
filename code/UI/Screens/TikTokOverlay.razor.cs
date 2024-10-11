using Sandbox;
using System;

public partial class TikTokOverlay : PanelComponent
{
	[Property] public bool ShowBars { get; set; } = false;

	private string FillerClass => ShowBars ? "expanded" : string.Empty;

	protected override int BuildHash()
	{
		return HashCode.Combine( ShowBars );
	}
}
