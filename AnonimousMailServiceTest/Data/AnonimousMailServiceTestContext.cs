using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AnonimousMailServiceTest.Models;
using System.Reflection.Metadata;

namespace AnonimousMailServiceTest.Data
{
    public class AnonimousMailServiceTestContext : DbContext
    {
        public AnonimousMailServiceTestContext (DbContextOptions<AnonimousMailServiceTestContext> options)
            : base(options)
        {
        }

        public DbSet<Message> Message { get; set; } = default!;
        public DbSet<UserOfMailService> UserOfMailService { get; set; } = default!;

        //https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-7.0/breaking-changes
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>()
                .ToTable(tb => tb.HasTrigger("SomeTrigger"));
            modelBuilder.Entity<UserOfMailService>()
                .ToTable(tb => tb.HasTrigger("SomeTrigger"));
        }
    }
}
