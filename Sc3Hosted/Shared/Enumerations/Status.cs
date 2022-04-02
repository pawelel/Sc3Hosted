using System.ComponentModel.DataAnnotations;

namespace Sc3Hosted.Shared.Enumerations;
public enum Status
{
    [Display(Name = "Nieznany")]
    Unknown =0,
    [Display(Name = "W użyciu")]
    InUse =1,
    [Display(Name = "Na stanie")]
    InStock =2,
    [Display(Name = "W naprawie")]
    InRepair =3,
    [Display(Name = "Wycofany")]
    Retired=4
}
