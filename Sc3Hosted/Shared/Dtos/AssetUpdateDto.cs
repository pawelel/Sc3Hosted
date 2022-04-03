﻿using Sc3Hosted.Shared.Enumerations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;
public class AssetUpdateDto
{
    public string Name { get; set; }= string.Empty;

    public Status Status { get; set; }
    public List<AssetDetailDto> AssetDetails { get; set; } = new();
    /// <summary>
    /// Process based location
    /// </summary>
    public int CoordinateId { get; set; }
    public List<AssetCategoryDto> AssetCategories { get; set; } = new();
    /// <summary>
    /// Process name
    /// </summary>
    public string Process { get; set; }= string.Empty;
}