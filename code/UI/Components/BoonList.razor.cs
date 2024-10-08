﻿using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BoonList : Panel
{
	public Action OnOpenBoonShop { get; set; }

	private List<Boon> PurchasedBoons { get; set; } = new();

	protected override void OnAfterTreeRender( bool firstTime )
	{
		base.OnAfterTreeRender( firstTime );

		if ( !firstTime )
			return;

		RefreshBoonList();
	}

	public void RefreshBoonList()
	{
		PurchasedBoons = Boon.GetOwned().ToList();
		StateHasChanged();
	}

	private void OpenBoonShop()
	{
		OnOpenBoonShop?.Invoke();
	}
}
