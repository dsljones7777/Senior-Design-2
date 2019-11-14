namespace RFIDCommandCenter
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using RFIDCommandCenter.Models;

    public partial class DataContext : DbContext
    {
        public DataContext()
            : base("name=DataContext")
        {
        }

        public DbSet<Location> Locations { get; set; }
        public DbSet<SystemUser> SystemUsers { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TagLocationBridge> AllowedLocations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
