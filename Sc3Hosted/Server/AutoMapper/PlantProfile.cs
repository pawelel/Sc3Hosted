using AutoMapper;

using Sc3Hosted.Server.Entities;
using Sc3Hosted.Shared.Dtos;

namespace Sc3Hosted.Server.AutoMapper;

public class PlantProfile : Profile
{
    public PlantProfile()
    {
        CreateMap<Plant, PlantDto>();
        CreateMap<PlantDto, Plant>();
    }
}