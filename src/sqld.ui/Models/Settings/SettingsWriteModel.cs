using System.ComponentModel.DataAnnotations;
using SqlD.Configs.Model;

namespace SqlD.UI.Models.Settings;

public class SettingsWriteModel
{
    [Required]
    public string Data { get; set; }
}