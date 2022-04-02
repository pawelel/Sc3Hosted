﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;
public class ParameterCreateDto
{
    public string Name { get; set; }

    public ParameterCreateDto(string name)
    {
        Name = name;
    }

    public string? Description { get; set; }
}
