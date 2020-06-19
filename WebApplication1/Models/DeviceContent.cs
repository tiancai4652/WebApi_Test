using Microsoft.EntityFrameworkCore;
using My.Share.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class DeviceContent : DbContext
    {
        public DbSet<Device> Devices { get; set; }
        public DbSet<Spectrum> Spectrums { get; set; }
        public DbSet<SpectrumData> SpectrumData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=device.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Device>().HasMany(f => f.Spectrum).WithOne().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Spectrum>().HasMany(f => f.Data).WithOne().OnDelete(DeleteBehavior.Cascade);
          
        }

    }
}
