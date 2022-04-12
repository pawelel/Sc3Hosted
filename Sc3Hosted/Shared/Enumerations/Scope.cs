using System.ComponentModel.DataAnnotations;
namespace Sc3Hosted.Shared.Enumerations;
public enum Scope
{
    Asset,
    [Display(Name = "Koordynat")]
    Coordinate,
    [Display(Name = "Przestrzeń")]
    Space,
    [Display(Name = "Obszar")]
    Area,
    [Display(Name = "Model")]
    Model,
    [Display(Name = "Sprzęt")]
    Device,
    [Display(Name = "Ogólny")]
    General
}
