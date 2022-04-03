using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;

public class SituationUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<SituationQuestionDto> SituationQuestions { get; set; } = new();
    public List<SituationCategoryDto> CategorySituations { get; set; } = new();
    public List<SituationDetailDto> SituationDetails { get; set; } = new();
    public List<SituationParameterDto> SituationParameters { get; set; } = new();
    public List<SituationAssetDto> AssetSituations { get; set; } = new();
}
