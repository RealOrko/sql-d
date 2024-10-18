using Microsoft.AspNetCore.Mvc;
using SqlD.UI.Models;

namespace SqlD.UI.ViewComponents.Registry;

public class RegistrySelector : ViewComponent
{
    public IViewComponentResult Invoke(SqlLiteViewModel query)
    {
        return View();
    }
}