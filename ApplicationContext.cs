using Microsoft.EntityFrameworkCore;
using MTUBankBase.Database.Models;
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

        public DbSet<User> Users { get; set; }
        public DbSet<Token> Tokens { get; set; }
    }
}
