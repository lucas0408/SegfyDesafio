using Microsoft.EntityFrameworkCore;
using Segfy.Api.Models;

namespace Segfy.Api.Data;

public class SegfyDbContext : DbContext
{
    public SegfyDbContext(DbContextOptions<SegfyDbContext> options) : base(options)
    {
    }

    public DbSet<Apolice> Apolices => Set<Apolice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Apolice>(entity =>
        {
            entity.ToTable("Apolices");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.NumeroApolice).IsRequired().HasMaxLength(20);
            entity.HasIndex(a => a.NumeroApolice).IsUnique();
            entity.Property(a => a.CpfCnpjSegurado).IsRequired().HasMaxLength(14);
            entity.Property(a => a.PlacaVeiculo).IsRequired().HasMaxLength(7);
            entity.Property(a => a.ValorPremio).HasColumnType("decimal(10,2)");
            entity.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);
        });
    }
}
