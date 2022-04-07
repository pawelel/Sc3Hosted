using AutoMapper;

using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

using Sc3Hosted.Server.AutoMapper;
using Sc3Hosted.Server.Services;

using Xunit;

namespace Sc3Hosted.UnitTests.AutoMapper;
public class MappingProfileTest
{
    [Fact]
    public void TestConfig()
    {
        // arrange
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<Sc3HostedProfile>();
        });

        // asssert
        config.AssertConfigurationIsValid();
    }

}