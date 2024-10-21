using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SqlD.Network;
using SqlD.UI.Models.Services;
using SqlD.UI.Models.Settings;
using SqlD.UI.Services;

namespace SqlD.UI.Controllers;

public class SettingsController : Controller
{
    private readonly SettingsService _settings;

    public SettingsController(SettingsService settings)
    {
        _settings = settings;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(_settings.ReadConfig());
    }
    
    [HttpPost]
    public IActionResult Save([FromForm] SettingsWriteModel model)
    {
        var result = _settings.WriteConfig(model.Data);
        return PartialView(result);
    }
}
