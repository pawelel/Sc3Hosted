using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sc3Hosted.Client.Services;
using Sc3Hosted.Shared.Dtos;
namespace Sc3Hosted.Client.Components.LocationComponents;
public partial class PlantForm : ComponentBase
{
    private readonly PlantCreateDto _plantCreateDto = new();
    private MudForm _form = new();
    private bool _isOpen;
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = default!;
    [Inject]
    private IPlantService PlantService { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    private void Submit()
    {
        MudDialog.Close(DialogResult.Ok(true));
    }
    private void Cancel()
    {
        MudDialog.Cancel();
    }
    protected override async Task OnInitializedAsync()
    {

    }
    private static IEnumerable<string> MaxNameCharacters(string ch)
    {
        if (!string.IsNullOrEmpty(ch) && 60 < ch?.Length)
            yield return "Max 59 znaków";
    }
    private static IEnumerable<string> MaxDescriptionCharacters(string ch)
    {
        if (!string.IsNullOrEmpty(ch) && 250 < ch?.Length)
            yield return "Max 249 znaków";
    }
    public void ToggleOpen()
    {
        _isOpen = !_isOpen;
    }

    private async Task HandleSave()
    {
        await _form.Validate();
        if (_form.IsValid)
            try
            {
                await PlantService.CreatePlant(_plantCreateDto);
                Snackbar.Add("Pomyślnie dodano nowy zakład", Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }

    }
}
