using AutoMapper;

using Sc3Hosted.Server.Entities;
using Sc3Hosted.Shared.Dtos;

namespace Sc3Hosted.Server.AutoMapper;

public class Sc3HostedProfile : Profile
{
    public Sc3HostedProfile()
    {
        // single
        CreateMap<Plant, PlantDto>();
        CreateMap<Area, AreaDto>();
        CreateMap<Space, SpaceDto>();
        CreateMap<Coordinate, CoordinateDto>();
        CreateMap<Device, DeviceDto>();
        CreateMap<Model, ModelDto>();
        CreateMap<Asset, AssetDto>();
        CreateMap<Detail, DetailDto>();
        CreateMap<Parameter, ParameterDto>();
        CreateMap<Category, CategoryDto>();
        CreateMap<Situation, SituationDto>();
        CreateMap<Communicate, CommunicateDto>();
        
        // navigation properties
        CreateMap<CommunicateCoordinate, CommunicateCoordinateDto>();
        CreateMap<CommunicateSpace, CommunicateSpaceDto>();
        CreateMap<CommunicateArea, CommunicateAreaDto>();
        CreateMap<CommunicateDevice, CommunicateDeviceDto>();
        CreateMap<CommunicateAsset, CommunicateAssetDto>();
        CreateMap<CommunicateModel, CommunicateModelDto>();
        CreateMap<ModelParameter, ModelParameterDto>();
        CreateMap<AssetDetail, AssetDetailDto>();
        CreateMap<AssetCategory, AssetCategoryDto>();
        CreateMap<DeviceSituation, DeviceSituationDto>();
        CreateMap<CategorySituation, CategorySituationDto>();
        CreateMap<SituationQuestion, SituationQuestionDto>();
        CreateMap<SituationDetail, SituationDetailDto>();
        CreateMap<SituationParameter, SituationParameterDto>();
        CreateMap<Coordinate, LocationDto>()
            .ForMember(dest => dest.AreaId, opt => opt.MapFrom(src => src.Space.AreaId))
            .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Space.Area.Name))
            .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.Space.Area.PlantId))
            .ForMember(dest => dest.Plant, opt => opt.MapFrom(src => src.Space.Area.Plant.Name))
            .ForMember(dest => dest.Coordinate, opt => opt.MapFrom(src => src.Name));
    }
}