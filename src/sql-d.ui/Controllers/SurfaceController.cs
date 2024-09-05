using Microsoft.AspNetCore.Mvc;
using SqlD.UI.Models.Surface;
using SqlD.UI.Services;

namespace SqlD.UI.Controllers
{
    public class SurfaceController : Controller
    {
	    private readonly SurfaceService surface;

	    public SurfaceController(SurfaceService surface)
	    {
		    this.surface = surface;
	    }

	    public IActionResult Index()
        {
	        var config = this.surface.GetConfig();
	        var viewModel = new SurfaceViewModel(config);
	        return View(viewModel);
        }
	}
}