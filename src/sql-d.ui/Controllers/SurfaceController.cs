using Microsoft.AspNetCore.Mvc;
using SqlD.UI.Models.Surface;
using SqlD.UI.Services;

namespace SqlD.UI.Controllers;

public class SurfaceController : Controller
{
    public IActionResult Index()
    {
        var viewModel = new SurfaceViewModel(Configs.Configuration.Instance);
        return View(viewModel);
    }
}