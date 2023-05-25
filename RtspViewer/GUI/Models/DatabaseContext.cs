using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtspViewer.GUI.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Source> Sources { get; set; } = null!;
        public DbSet<Report> Reports { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={Properties.Settings.Default.DatabaseLocation}");
        }

        public DatabaseContext()
        {
            Database.EnsureCreated();
        }
    }
}
