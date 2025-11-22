using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DeThi.Models;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Option> Options { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Test> Tests { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=aws-1-ap-southeast-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.msyxtzgstmvcyjjxyiso;Password=Congkcs12345@@;Ssl Mode=Require;Trust Server Certificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Option>(entity =>
        {
            entity.HasKey(e => e.OptionId).HasName("options_pkey");

            entity.ToTable("options");

            entity.HasIndex(e => new { e.QuestionId, e.OptionLabel }, "options_question_id_option_label_key").IsUnique();

            entity.Property(e => e.OptionId).HasColumnName("option_id");
            entity.Property(e => e.OptionLabel)
                .HasMaxLength(1)
                .HasColumnName("option_label");
            entity.Property(e => e.OptionText).HasColumnName("option_text");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");

            entity.HasOne(d => d.Question).WithMany(p => p.Options)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("options_question_id_fkey");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.QuestionId).HasName("questions_pkey");

            entity.ToTable("questions");

            entity.HasIndex(e => new { e.TestId, e.QuestionNumber }, "questions_test_id_question_number_key").IsUnique();

            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.AnswerExplanation).HasColumnName("answer_explanation");
            entity.Property(e => e.AudioUrl)
                .HasMaxLength(500)
                .HasColumnName("audio_url");
            entity.Property(e => e.CorrectAnswerLabel)
                .HasMaxLength(1)
                .HasColumnName("correct_answer_label");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("image_url");
            entity.Property(e => e.QuestionGroupId)
                .HasMaxLength(50)
                .HasColumnName("question_group_id");
            entity.Property(e => e.QuestionNumber).HasColumnName("question_number");
            entity.Property(e => e.QuestionText).HasColumnName("question_text");
            entity.Property(e => e.QuestionType)
                .HasMaxLength(50)
                .HasColumnName("question_type");
            entity.Property(e => e.TestId)
                .HasMaxLength(50)
                .HasColumnName("test_id");

            entity.HasOne(d => d.Test).WithMany(p => p.Questions)
                .HasForeignKey(d => d.TestId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("questions_test_id_fkey");
        });

        modelBuilder.Entity<Test>(entity =>
        {
            entity.HasKey(e => e.TestId).HasName("tests_pkey");

            entity.ToTable("tests");

            entity.Property(e => e.TestId)
                .HasMaxLength(50)
                .HasColumnName("test_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DurationMinutes).HasColumnName("duration_minutes");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.TotalQuestions).HasColumnName("total_questions");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
