using Microsoft.EntityFrameworkCore;
using DomainModel.Classes.User;
using Microsoft.Extensions.Configuration;
using Persistence.EF.PostgreSQL.Models;

namespace Persistence.EF.DbContexts
{
    public class PgDataContext: DbContext
    {
        public DbSet<UserModel> Users { get; set; }

        public IConfiguration Configuration { get; }

        private readonly DbContextOptions<PgDataContext> _dbContextOptions;

        public PgDataContext(DbContextOptions<PgDataContext> options, IConfiguration configuration)
            : base(options)
        {
            _dbContextOptions = options;
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Configuration.GetConnectionString("Postgres"),
                npgsqlOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),null);
                }
             );
        }

        #region Required
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("unilab");

            new UserEntityTypeConfiguration().Configure(modelBuilder.Entity<UserModel>());

        }
        #endregion
    }
}

