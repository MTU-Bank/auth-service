using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MTUModelContainer.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTUAuthService
{
    internal class ApplicationContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                Program.serviceConfig.ConnectionString,
                ServerVersion.AutoDetect(Program.serviceConfig.ConnectionString)
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // "User" -> "OwnerId" (Account)
            modelBuilder.Entity<Account>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.OwnerId)
                .IsRequired();
        }

        public DbSet<User> Users { get; set; }
    }
}
