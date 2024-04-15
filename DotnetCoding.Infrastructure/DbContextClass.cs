using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotnetCoding.Core.Models;

namespace DotnetCoding.Infrastructure
{
    public class DbContextClass : DbContext
    {
        public DbContextClass(DbContextOptions<DbContextClass> contextOptions) : base(contextOptions)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<ApprovalQueue> ApprovalQueues { get; set; }
        public DbSet<ProductHistory> ProductHistories { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Indexing for Product Name
            modelBuilder.Entity<Product>().HasIndex(p => p.Name).IsUnique();

            // Indexing for ApprovalQueue RequestDate
            modelBuilder.Entity<ApprovalQueue>().HasIndex(a => a.RequestDate);

        }
    }
}
