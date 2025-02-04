using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Entities.Models
{
    public partial class StandardContest2023Context : DbContext
    {
        public StandardContest2023Context()
        {
        }

        public StandardContest2023Context(DbContextOptions<StandardContest2023Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Contest> Contests { get; set; } = null!;
        // public virtual DbSet<ContestFields> ContestFields { get; set; } = null!;
        public virtual DbSet<ContestFieldDetails> ContestFieldDetails { get; set; } = null!;
        public virtual DbSet<RegexValidation> RegexValidations { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
