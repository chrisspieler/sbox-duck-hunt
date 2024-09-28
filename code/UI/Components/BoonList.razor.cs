using Sandbox.UI;
using System.Collections.Generic;
using System.Linq;

public partial class BoonList : Panel
{
	private List<Boon> Boons { get; set; } = new();

	protected override void OnAfterTreeRender( bool firstTime )
	{
		base.OnAfterTreeRender( firstTime );

		if ( !firstTime )
			return;

		Boons = Boon.GetPurchaseable().ToList();
	}

	private void OpenBoonShop()
	{

	}
}
