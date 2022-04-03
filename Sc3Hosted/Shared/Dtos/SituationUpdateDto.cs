using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;

public class SituationUpdateDto
{
    public string Name { get; set; }

    public SituationUpdateDto(string name)
    {
        Name = name;
    }
public List<SituationQuestionDto> SituationQuestions { get; set; }
public List<SituationCategoryDto> CategorySituations { get; set; }
public List<SituationDetailDto> SituationDetails { get; set; }
public List<SituationParameterDto> SituationParameters { get; set; }
public List<SituationAssetDto> AssetSituations { get; set; }
}
