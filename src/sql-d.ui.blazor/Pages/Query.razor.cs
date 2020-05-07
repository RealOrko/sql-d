using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SqlD.UI.Blazor.Shared.Components.RegistryList;
using SqlD.UI.Services;
using SqlD.UI.Models.Registry;

namespace SqlD.UI.Blazor.Pages
{
    public class QueryBase : ComponentBase
    {
        [Inject] 
        private IJSRuntime JavaScript { get; set; }

        [Inject]
        private RegistryService RegistryService { get; set; }
        
        [Inject]
        private EventService EventService { get; set; }

        [Parameter]
        public RegistryViewModel Registry { get; set; } = new RegistryViewModel();
        
        [CascadingParameter]
        public string ConnectedService { get; set; }
    
        protected override async Task OnInitializedAsync()
        {
            Registry = await RegistryService.GetServices();
        }

        protected void RegistryList_NewServiceClick(RegistryListEventArgs args)
        {
            Console.WriteLine("New Service Clicked!");
        }
        
        protected void RegistryList_ServiceIdentityClick(RegistryListEventArgs args)
        {
            JavaScript.InvokeAsync<object>("open", args.Service.EndPoint.ToUrl("api/id"),"_blank");
        }

        protected void RegistryList_ServiceSwaggerClick(RegistryListEventArgs args)
        {
            JavaScript.InvokeAsync<object>("open", args.Service.EndPoint.ToUrl("swagger"),"_blank");
        }

        protected void RegistryList_ServiceConnectClick(RegistryListEventArgs args)
        {
            EventService.Dispatch("ConnectedService", args.Service.Uri);
        }
    }
}