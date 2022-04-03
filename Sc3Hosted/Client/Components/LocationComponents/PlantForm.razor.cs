using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using MudBlazor;
using Sc3Hosted.Shared.Dtos;
using Sc3Hosted.Client.Services;

namespace Sc3Hosted.Client.Components.LocationComponents;
public partial class PlantForm : ComponentBase
{
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = default!;
    [Inject] IPlantService PlantService { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    private void Submit() => MudDialog.Close(DialogResult.Ok(true));
    private MudForm _form=new();
    bool _isOpen;
    private void Cancel() => MudDialog.Cancel();
    private PlantCreateDto _plantCreateDto = new();
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

    async Task HandleSave()
    {
        await _form.Validate();
        if (_form.IsValid)
        {
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
}
