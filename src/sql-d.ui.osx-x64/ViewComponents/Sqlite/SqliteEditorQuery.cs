using Microsoft.AspNetCore.Mvc;

namespace SqlD.UI.ViewComponents.Sqlite
{
	public class SqliteEditorQuery : ViewComponent
	{
		public IViewComponentResult Invoke()
		{
			return View();
		}
	}
}