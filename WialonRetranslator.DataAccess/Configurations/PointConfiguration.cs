using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WialonRetranslator.DataAccess.Entities;

namespace WialonRetranslator.DataAccess.Configurations
{
    public class PointConfiguration: IEntityTypeConfiguration<Point>
    {
        public void Configure(EntityTypeBuilder<Point> builder)
        {
            builder.HasKey(x => x.PointId);
        }
    }
}