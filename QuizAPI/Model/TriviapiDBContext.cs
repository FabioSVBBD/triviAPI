using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text.Json;
using QuizAPI.Utils;

namespace QuizAPI.Model
{
    public partial class TriviapiDBContext : DbContext
    {
        public TriviapiDBContext(){}

        public TriviapiDBContext(DbContextOptions<TriviapiDBContext> options)
            : base(options){}

        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Difficulty> Difficulties { get; set; } = null!;
        public virtual DbSet<Question> Questions { get; set; } = null!;
        public virtual DbSet<QuestionTag> QuestionTags { get; set; } = null!;
        public virtual DbSet<Status> Statuses { get; set; } = null!;
        public virtual DbSet<Tag> Tags { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var appSettings = AppSettings.instance();
                string myDbConnection = appSettings.connectionString;
                optionsBuilder.UseSqlServer(myDbConnection);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.HasIndex(e => e.CategoryName, "UQ__Category__8517B2E07ACE1B3C")
                    .IsUnique();

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            //modelBuilder.Entity<Category>().Navigation(e => e.Questions).AutoInclude();

            modelBuilder.Entity<Difficulty>(entity =>
            {
                entity.ToTable("Difficulty");

                entity.HasIndex(e => e.DifficultyName, "UQ__Difficul__760CA2C1F98854E8")
                    .IsUnique();

                entity.Property(e => e.DifficultyId).HasColumnName("DifficultyID");

                entity.Property(e => e.DifficultyName)
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.ToTable("Question");

                entity.HasIndex(e => e.Question1, "UQ__Question__6B387A680CFB1560")
                    .IsUnique();

                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

                entity.Property(e => e.Answer)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.DifficultyId).HasColumnName("DifficultyID");

                entity.Property(e => e.Question1)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("Question");

                entity.Property(e => e.StatusId)
                    .HasColumnName("StatusID")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Question.CategoryID");

                entity.HasOne(d => d.Difficulty)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.DifficultyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Question.DifficultyID");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Question.StatusID");
            });

            modelBuilder.Entity<Question>().Navigation(e => e.Status).AutoInclude();
            modelBuilder.Entity<Question>().Navigation(e => e.Category).AutoInclude();
            modelBuilder.Entity<Question>().Navigation(e => e.Difficulty).AutoInclude();

            modelBuilder.Entity<QuestionTag>(entity =>
            {
                entity.ToTable("QuestionTag");

                entity.Property(e => e.QuestionTagId).HasColumnName("QuestionTagID");

                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

                entity.Property(e => e.TagId).HasColumnName("TagID");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.QuestionTags)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuestionTag.QuestionID");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.QuestionTags)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuestionTag.TagID");
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.ToTable("Status");

                entity.HasIndex(e => e.StatusName, "UQ__Status__05E7698ACDBA1956")
                    .IsUnique();

                entity.Property(e => e.StatusId).HasColumnName("StatusID");

                entity.Property(e => e.StatusName)
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("Tag");

                entity.HasIndex(e => e.TagName, "UQ__Tag__BDE0FD1D0BC85AEF")
                    .IsUnique();

                entity.Property(e => e.TagId).HasColumnName("TagID");

                entity.Property(e => e.TagName)
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
