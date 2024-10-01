using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SqlD.Network;
using SqlD.UI.Models.Services;
using SqlD.UI.Services;

namespace SqlD.UI.Controllers;

public class SettingsController : Controller
{
    private readonly SettingsService _settings;

    public SettingsController(SettingsService services)
    {
        _settings = services;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View();
    }
}
