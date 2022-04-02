using AutoMapper;
using Sc3Hosted.Shared.Entities;
using Sc3Hosted.Shared.ViewModels;

namespace Sc3Hosted.Server.AutoMapper;

public class PlantProfile : Profile
{
    public PlantProfile()
    {
        CreateMap<Plant, PlantDto>();
        CreateMap<PlantDto, Plant>();
    }
}