using Dgp.Domain.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Cosmos;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Data.EFCosmosDB
{
    public class DgpDbContext : DbContext
    {
        public DgpDbContext(IOptions<CosmosDbOptions> options)
        {
            Options = options?.Value ?? throw new ArgumentNullException();
        }


        public DbSet<CourseDocument> Courses { get; set; }
        public DbSet<EventDocument> Events { get; set; }
        public DbSet<ScorecardDocument> Scorecards { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseCosmos(
                                Options.Endpoint, 
                                Options.AuthKey, 
                                Options.DatabaseName);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // By default EF will create on collection for the entire DbContext adding a
            // field named "Discriminator" with the value of entity type.

            // To create a collection per entity type. Note the "Discrimiator" field is still added.
            //modelBuilder.Entity<CourseDocument>().ToContainer("Courses");
            //modelBuilder.Entity<EventDocument>().ToContainer("Events");
            //modelBuilder.Entity<ScorecardDocument>().ToContainer("Scorecards");
        }


        private readonly CosmosDbOptions Options;
    }
}
