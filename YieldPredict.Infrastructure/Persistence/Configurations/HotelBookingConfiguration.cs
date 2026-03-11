using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YieldPredict.Core.Entities;

namespace YieldPredict.Infrastructure.Persistence.Configurations;

public class HotelBookingConfiguration : IEntityTypeConfiguration<HotelBooking>
{
    public void Configure(EntityTypeBuilder<HotelBooking> builder)
    {
        // Domain entity'si ile fiziksel tablo şeması arasındaki mapping, Infrastructure katmanında merkezileştirilerek veri erişimi stratejisi (ör. RDBMS seçimi) domain kurallarından ayrıştırılır.
        builder.ToTable("HotelBookings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Hotel)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.IsCanceled)
            .IsRequired();

        builder.Property(x => x.LeadTime)
            .IsRequired();

        builder.Property(x => x.ArrivalDateMonth)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Adults)
            .IsRequired();

        builder.Property(x => x.Children)
            .IsRequired();

        builder.Property(x => x.Meal)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.MarketSegment)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ReservedRoomType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Adr)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
    }
}
