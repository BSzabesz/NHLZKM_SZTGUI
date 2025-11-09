using Microsoft.EntityFrameworkCore;
using NHLZKM_SZTGUI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHLZKM_SZTGUI.Persistence.MsSql
{
    public class FormulaOneDbContext : DbContext
    {
        public DbSet<Team> Teams { get; set; }
        public DbSet<AnnualBudget> AnnualBudgets { get; set; }
        public DbSet<BudgetItem> BudgetItems { get; set; }

        public FormulaOneDbContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connStr = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=formuladb;Integrated Security=True;MultipleActiveResultSets=true";
            optionsBuilder.UseSqlServer(connStr);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>()
                .HasMany(t => t.AnnualBudgets)
                .WithOne(b => b.Team);

            modelBuilder.Entity<AnnualBudget>()
                .HasMany(b => b.BudgetItems)
                .WithOne(i => i.AnnualBudget);

        }
    }

}
