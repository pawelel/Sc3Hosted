using System.ComponentModel.DataAnnotations;
namespace Sc3Hosted.Shared.Enumerations;
/// <summary>
///     SkmUpdate - need to include in service
///     Reminder - event that may occur in time, i.e. VIP delegation
/// </summary>
public enum CommunicationType
{
    [Display(Name = "Aktualizacja instrukcji")]
    SkmUpdate,
    [Display(Name = "Przypomnienie")]
    Reminder
}
