using Sandbox;
using Sandbox.UI;

public partial class Hud : PanelComponent, IBoonEvent
{
	public string TimeRemaining => $"{GameState.Instance.GameTime:F0}";
	public string ComboMultiplier => $"{GameState.Instance.ComboMultiplier:F2}x";

	public bool IsBoonShopOpen { get; private set; }
	private BoonList BoonPanel { get; set; }
	private Panel Crosshair { get; set; }
	private bool ShouldShowStartButton => GameState.Instance.CurrentSessionGameCount < 1 || _sinceGameOver > 1f;
	private TimeSince _sinceGameOver;

	/// <summary>
	/// the hash determines if the system should be rebuilt. If it changes, it will be rebuilt
	/// </summary>
	protected override int BuildHash() => System.HashCode.Combine(
		GameState.Instance.Points,
		GameState.Instance.DucksHunted,
		TimeRemaining,
		GameState.Instance.IsGameActive,
		ShouldShowStartButton,
		GameState.Instance.GotPersonalBestPoints
	);

	protected override void OnEnabled()
	{
		base.OnEnabled();

		GameState.Instance.GameOver += () => _sinceGameOver = 0f;
	}


	protected override void OnUpdate()
	{
		if ( Crosshair is null )
			return;

		var screenSize = Screen.Size;
		var normalizedMousePosition = Mouse.Position / screenSize;

		Crosshair.Style.Left = Length.Percent( normalizedMousePosition.x * 100f );
		Crosshair.Style.Top = Length.Percent( normalizedMousePosition.y * 100f );
	}

	public void OpenBoonShop()
	{
		IsBoonShopOpen = true;
		StateHasChanged();
	}

	public void CloseBoonShop()
	{
		IsBoonShopOpen = false;
		StateHasChanged();
	}

	public void OnBoonsChanged()
	{
		if ( BoonPanel.IsValid() )
		{
			BoonPanel.RefreshBoonList();
		}
	}

	public void OnBoonToggled( Boon boon, bool enabled )
	{
		if ( BoonPanel.IsValid() )
		{
			BoonPanel.RefreshBoonList();
		}
	}
}
