using Microsoft.AspNetCore.Components;

using Sc3Hosted.Shared.Dtos;

namespace Sc3Hosted.Client.Components.AssetComponents;
public partial class AssetCard : ComponentBase
{
    [Parameter] public AssetDisplayDto AssetModel { get; set; } = new();

}
