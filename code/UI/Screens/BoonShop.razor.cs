using Sandbox.Services;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class BoonShop : Panel
{
	public Action OnClose { get; set; }

	private List<Boon> AllBoons { get; set; } = new();
	private Task PurchaseTask { get; set; }
	private string RootClass => PurchaseTask != null ? "blur" : null;

	protected override int BuildHash()
	{
		return HashCode.Combine( PurchaseTask );
	}

	public BoonShop()
	{
		RefreshBoons();
	}

	public void Close()
	{
		OnClose?.Invoke();
	}

	public void RefreshBoons()
	{
		AllBoons = Boon.GetAll().ToList();
		StateHasChanged();
	}

	private async void BuyBoon( Boon boon )
	{
		if ( Boon.Debug )
		{
			Boon.DebugAdd( boon.ResourceName );
			return;
		}

		// There's no way to cancel a purchase in progress.
		if ( PurchaseTask != null )
		{
			Log.Info( "Purchase task already in progress." );
			return;
		}

		Log.Info( $"Purchasing boon: {boon?.ResourcePath}" );

		PurchaseTask = Monetization.Purchase( boon );
		await PurchaseTask;
		PurchaseTask = null;
	}
}
