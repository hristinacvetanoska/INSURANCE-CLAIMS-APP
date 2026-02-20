using Claims.Domain.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Claims
{
    /// <summary>
    /// Represents the database context for claims and covers.
    /// </summary>
    public class ClaimsContext : DbContext
    {
        /// <summary>
        /// Gets or sets the Claims DbSet.
        /// </summary>
        public DbSet<Claim> Claims { get; init; }

        /// <summary>
        /// Gets or sets the Covers DbSet.
        /// </summary>
        public DbSet<Cover> Covers { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimsContext"/> class.
        /// </summary>
        /// <param name="options">The options to configure the context.</param>
        public ClaimsContext(DbContextOptions options)
            : base(options)
        {
        }

        /// <summary>
        /// Configures the model and maps entities to MongoDB collections.
        /// </summary>
        /// <param name="modelBuilder">The model builder used to configure entity mappings.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Claim>().ToCollection("claims");
            modelBuilder.Entity<Cover>().ToCollection("covers");
        }
    }
}
