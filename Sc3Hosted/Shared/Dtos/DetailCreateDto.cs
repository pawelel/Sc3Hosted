﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;
public class DetailCreateDto
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public DetailCreateDto(string name)
    {
        Name = name;
    }
}
