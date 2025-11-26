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

    public virtual DbSet<QuestionGroup> QuestionGroups { get; set; }

    public virtual DbSet<Test> Tests { get; set; }

    public virtual DbSet<TestSession> TestSessions { get; set; }

    public virtual DbSet<UserAnswer> UserAnswers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=aws-1-ap-southeast-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.msyxtzgstmvcyjjxyiso;Password=Conggtvt123456@@;Ssl Mode=Require;Trust Server Certificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("auth", "aal_level", new[] { "aal1", "aal2", "aal3" })
            .HasPostgresEnum("auth", "code_challenge_method", new[] { "s256", "plain" })
            .HasPostgresEnum("auth", "factor_status", new[] { "unverified", "verified" })
            .HasPostgresEnum("auth", "factor_type", new[] { "totp", "webauthn", "phone" })
            .HasPostgresEnum("auth", "oauth_authorization_status", new[] { "pending", "approved", "denied", "expired" })
            .HasPostgresEnum("auth", "oauth_client_type", new[] { "public", "confidential" })
            .HasPostgresEnum("auth", "oauth_registration_type", new[] { "dynamic", "manual" })
            .HasPostgresEnum("auth", "oauth_response_type", new[] { "code" })
            .HasPostgresEnum("auth", "one_time_token_type", new[] { "confirmation_token", "reauthentication_token", "recovery_token", "email_change_token_new", "email_change_token_current", "phone_change_token" })
            .HasPostgresEnum("realtime", "action", new[] { "INSERT", "UPDATE", "DELETE", "TRUNCATE", "ERROR" })
            .HasPostgresEnum("realtime", "equality_op", new[] { "eq", "neq", "lt", "lte", "gt", "gte", "in" })
            .HasPostgresEnum("storage", "buckettype", new[] { "STANDARD", "ANALYTICS", "VECTOR" })
            .HasPostgresExtension("extensions", "pg_stat_statements")
            .HasPostgresExtension("extensions", "pgcrypto")
            .HasPostgresExtension("extensions", "uuid-ossp")
            .HasPostgresExtension("graphql", "pg_graphql")
            .HasPostgresExtension("vault", "supabase_vault");

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
            entity.Property(e => e.GroupId)
                .HasMaxLength(50)
                .HasColumnName("group_id");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("image_url");
            entity.Property(e => e.Part).HasColumnName("part");
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

            entity.HasOne(d => d.Group).WithMany(p => p.Questions)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_question_group");

            entity.HasOne(d => d.Test).WithMany(p => p.Questions)
                .HasForeignKey(d => d.TestId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("questions_test_id_fkey");
        });

        modelBuilder.Entity<QuestionGroup>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("question_groups_pkey");

            entity.ToTable("question_groups");

            entity.Property(e => e.GroupId)
                .HasMaxLength(50)
                .HasColumnName("group_id");
            entity.Property(e => e.AudioUrl)
                .HasMaxLength(500)
                .HasColumnName("audio_url");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("image_url");
            entity.Property(e => e.Part).HasColumnName("part");
            entity.Property(e => e.PassageText).HasColumnName("passage_text");
            entity.Property(e => e.TestId)
                .HasMaxLength(50)
                .HasColumnName("test_id");

            entity.HasOne(d => d.Test).WithMany(p => p.QuestionGroups)
                .HasForeignKey(d => d.TestId)
                .HasConstraintName("fk_group_test");
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

        modelBuilder.Entity<TestSession>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("test_sessions_pkey");

            entity.ToTable("test_sessions");

            entity.Property(e => e.SessionId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("session_id");
            entity.Property(e => e.FinishedAt).HasColumnName("finished_at");
            entity.Property(e => e.StartedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("started_at");
            entity.Property(e => e.TestId)
                .HasMaxLength(50)
                .HasColumnName("test_id");
            entity.Property(e => e.TotalScore).HasColumnName("total_score");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(255)
                .HasColumnName("user_email");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<UserAnswer>(entity =>
        {
            entity.HasKey(e => e.AnswerId).HasName("user_answers_pkey");

            entity.ToTable("user_answers");

            entity.HasIndex(e => new { e.SessionId, e.QuestionId }, "unique_user_answer").IsUnique();

            entity.Property(e => e.AnswerId).HasColumnName("answer_id");
            entity.Property(e => e.AnsweredAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("answered_at");
            entity.Property(e => e.IsCorrect).HasColumnName("is_correct");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.SelectedOption)
                .HasMaxLength(1)
                .HasColumnName("selected_option");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Question).WithMany(p => p.UserAnswers)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("fk_user_answers_question");

            entity.HasOne(d => d.Session).WithMany(p => p.UserAnswers)
                .HasForeignKey(d => d.SessionId)
                .HasConstraintName("fk_user_answers_session");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
