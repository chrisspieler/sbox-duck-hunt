using Sandbox.UI;
using System.Collections.Generic;
using System.Linq;

public partial class BoonList : Panel
{
	private List<Boon> PurchasedBoons { get; set; } = new();

	protected override void OnAfterTreeRender( bool firstTime )
	{
		base.OnAfterTreeRender( firstTime );

		if ( !firstTime )
			return;

		PurchasedBoons = Boon.GetOwned().ToList();
	}

	private void OpenBoonShop()
	{

	}
}
