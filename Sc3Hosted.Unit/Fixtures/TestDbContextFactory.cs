using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Sc3Hosted.Server.Data;

namespace Sc3Hosted.UnitTests.Fixtures;
public class TestDbContextFactory : IDbContextFactory<Sc3HostedDbContext>
{
    private DbContextOptions<Sc3HostedDbContext> _options;
    public TestDbContextFactory(string databaseName = "InMemoryTest")
    {
        _options = new DbContextOptionsBuilder<Sc3HostedDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;
    }
    public Sc3HostedDbContext CreateDbContext()
    {
        return new Sc3HostedDbContext(_options);
    }
}
