using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SqlD.Network;
using SqlD.UI.Models.Services;
using SqlD.UI.Services;

namespace SqlD.UI.Controllers;

public class ServiceController : Controller
{
    private readonly ServiceService _services;

    public ServiceController(ServiceService services)
    {
        this._services = services;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string q = null, string s = null)
    {
        var registry = await _services.GetRegistry();
        var viewModel = new ServiceViewModel(Configs.Configuration.Instance, registry.Entries);
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var registry = await _services.GetRegistry();
        return View(new ServiceFormViewModel(registry.Entries));
    }

    [HttpPost]
    public IActionResult Create([FromForm] ServiceFormViewModel formViewModel)
    {
        if (ModelState.IsValid)
        {
            _services.CreateService(formViewModel);
            return Redirect("/Service");
        }

        return View(formViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Edit([FromQuery] string host = null, [FromQuery] int port = 0)
    {
        var serviceEndPoint = new EndPoint(host, port);
        var serviceModel = Configs.Configuration.Instance.Services.First(x => x.IsEqualTo(serviceEndPoint));
        
        var registry = await _services.GetRegistry();
        var viewModel = new ServiceFormViewModel(serviceModel, registry.Entries);

        var forwardEntry = Configs.Configuration.Instance.Services.FirstOrDefault(x => x.IsEqualTo(serviceEndPoint));
        if (forwardEntry != null)
        {
            foreach (var forward in forwardEntry.ForwardingTo)
            {
                var forwardRegistryEntry = viewModel.Forwards.First(x => x.EndPoint.IsEqualTo(forward));
                forwardRegistryEntry.Selected = true;
            }
        }
        
        var returnEntries = Configs.Configuration.Instance.Services.Where(x => !x.IsEqualTo(serviceEndPoint));
        if (returnEntries.Any())
        {
            foreach (var @return in returnEntries)
            {
                if (@return.ForwardingTo.Any(x => x.IsEqualTo(serviceEndPoint)))
                {
                    var returnRegistryEntry = viewModel.Returns.First(x =>
                    {
                        var returnEndPoint = new EndPoint(@return.Host, @return.Port);
                        return x.EndPoint.IsEqualTo(returnEndPoint);
                    });
                    returnRegistryEntry.Selected = true;
                }
            }
        }
        
        return View(viewModel);
    }

    [HttpPost]
    public IActionResult Edit([FromForm] ServiceFormViewModel formViewModel)
    {
        if (ModelState.IsValid)
        {
            _services.UpdateService(formViewModel);
            return Redirect("/Service");
        }

        return View(formViewModel);
    }

    [HttpGet]
    public IActionResult Delete([FromQuery] string host, [FromQuery] int port)
    {
        _services.RemoveService(host, port);
        return Redirect("/Service");
    }
    
    [HttpGet("Service/Forward/SynchroniseAll")]
    public async Task<IActionResult> ForwardSynchroniseAll()
    {
        await _services.SynchroniseForwardAll();
        return Redirect("/Service");
    }

    [HttpGet("Service/Forward/Synchronise")]
    public async Task<IActionResult> ForwardSynchronise([FromQuery] string thisHost, [FromQuery] int thisPort, [FromQuery] string fromHost, [FromQuery] int fromPort)
    {
        var thisEndPoint = new EndPoint(thisHost, thisPort);
        var fromEndPoint = new EndPoint(fromHost, fromPort);
        await _services.SynchroniseForward(thisEndPoint, fromEndPoint);
        return Redirect("/Service");
    }

    [HttpGet("Service/Forward/Delete")]
    public async Task<IActionResult> ForwardDelete([FromQuery] string thisHost, [FromQuery] int thisPort, [FromQuery] string fromHost, [FromQuery] int fromPort)
    {
        var thisEndPoint = new EndPoint(thisHost, thisPort);
        var fromEndPoint = new EndPoint(fromHost, fromPort);
        await _services.RemoveForward(thisEndPoint, fromEndPoint);
        return Redirect("/Service");
    }
}