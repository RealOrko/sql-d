using System.Collections.Generic;
using System.Linq;

namespace SqlD.UI.Models.Registry;

public class RegistryViewModel
{
    public RegistryViewModel(List<RegistryEntryViewModel> items)
    {
        Entries = items.OrderBy(x => x.Tags).ToList();
    }

    public List<RegistryEntryViewModel> Entries { get; set; }
}