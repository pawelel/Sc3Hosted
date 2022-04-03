using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3Hosted.Server.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Server.Data.Configurations;
public class SituationParameterConfig : IEntityTypeConfiguration<SituationParameter>
{
    public void Configure(EntityTypeBuilder<SituationParameter> builder)
    {
        builder.ToTable("SituationParameters", x => x.IsTemporal());
        builder.HasKey(x => x.SituationParameterId);
        builder.Property(x => x.SituationParameterId).ValueGeneratedOnAdd();
        builder.Property(x => x.SituationId).IsRequired();
        builder.Property(x => x.ParameterId).IsRequired();

    }
}
