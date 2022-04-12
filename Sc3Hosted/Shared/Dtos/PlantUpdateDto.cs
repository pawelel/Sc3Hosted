﻿namespace Sc3Hosted.Shared.Dtos;
public class PlantUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<AreaDto> Areas { get; set; } = new();
}
