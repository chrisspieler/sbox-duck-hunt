using Sandbox.UI;
using System;

public partial class BoonShop : Panel
{
	public Action OnClose { get; set; }

	public void Close()
	{
		OnClose?.Invoke();
	}
}
