using Xunit;
using Sc3Hosted.Server.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Bogus;
using Sc3Hosted.Shared.Dtos;
using Sc3Hosted.Shared.Helpers;

using AutoMapper;
using FluentAssertions;

namespace Sc3Hosted.UnitTests.Services;

public class DbServiceTests2
{

    [Fact]
    public void CreatePlantTestAsync()
    {
        // Arrange
        PlantCreateDto plantCreateDto = new Faker<PlantCreateDto>().
        RuleFor(o => o.Name, f => f.Lorem.Word()).
        RuleFor(o => o.Description, f => f.Lorem.Paragraph()).
        Generate();
        string userId = Guid.NewGuid().ToString();
        var mockService = new Mock<IDbService>();
        mockService.Setup(x => x.CreatePlant(plantCreateDto, userId)).ReturnsAsync(new ServiceResponse("Plant created", true));
        // Assert

    }
}