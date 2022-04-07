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

        // custom
        CreateMap<Coordinate, LocationDto>()
            .ForMember(dest => dest.AreaId, opt => opt.MapFrom(src => src.Space.AreaId))
            .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Space.Area.Name))
            .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.Space.Area.PlantId))
            .ForMember(dest => dest.Plant, opt => opt.MapFrom(src => src.Space.Area.Plant.Name))
            .ForMember(dest => dest.Coordinate, opt => opt.MapFrom(src => src.Name));
        CreateMap<Category, CategoryWithAssetsDto>()
            .ForMember(dest => dest.Assets, opt => opt.MapFrom(x => x.AssetCategories.Select(y => y.Asset).ToList()))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(x => x.CategoryId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name));
        CreateMap<Communicate, CommunicateWithAssetsDto>()
             .ForMember(dest => dest.Assets, opt => opt.MapFrom(x => x.CommunicateAssets.Select(y => y.Asset).ToList()))
             .ForMember(dest => dest.CommunicateId, opt => opt.MapFrom(x => x.CommunicateId))
             .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name));
        CreateMap<Detail, DetailWithAssetsDto>()
            .ForMember(dest => dest.Assets, opt => opt.MapFrom(x => x.AssetDetails.Select(y => y.Asset).ToList()))
            .ForMember(dest => dest.DetailId, opt => opt.MapFrom(x => x.DetailId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name));
        CreateMap<Parameter, ParameterWithModelsDto>()
            .ForMember(dest => dest.Models, opt => opt.MapFrom(x => x.ModelParameters.Select(y => y.Model).ToList()))
            .ForMember(dest => dest.ParameterId, opt => opt.MapFrom(x => x.ParameterId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name));
        CreateMap<Question, QuestionWithSituationsDto>()
            .ForMember(dest => dest.Situations, opt => opt.MapFrom(x => x.SituationQuestions.Select(y => y.Situation).ToList()))
            .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(x => x.QuestionId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name));
        CreateMap<Situation, SituationWithAssetsDto>()
            .ForMember(dest => dest.SituationId, opt => opt.MapFrom(x => x.SituationId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name))
            .ForMember(dest => dest.Assets, opt => opt.MapFrom(x => x.AssetSituations.Select(y => y.Asset).ToList()));
        CreateMap<Situation, SituationWithCategoriesDto>()
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(x => x.CategorySituations.Select(y => y.Category).ToList()))
            .ForMember(dest => dest.SituationId, opt => opt.MapFrom(x => x.SituationId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(x => x.Description));
    }
}