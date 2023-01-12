using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PJSK.Utility.Data.EventShifts.DAL
{
    public partial class EventShiftsContext : DbContext
    {
        public EventShiftsContext()
        {
        }

        public EventShiftsContext(DbContextOptions<EventShiftsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Event> Events { get; set; } = null!;
        public virtual DbSet<Player> Players { get; set; } = null!;
        public virtual DbSet<Shift> Shifts { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(System.Configuration.ConfigurationManager.ConnectionStrings["EventShiftsDatabase"].ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>(entity =>
            {
                entity.Property(e => e.EventId).HasColumnName("event_id");

                entity.Property(e => e.EventEndDate)
                    .HasColumnType("text")
                    .HasColumnName("event_end_date");

                entity.Property(e => e.EventName)
                    .HasColumnType("text")
                    .HasColumnName("event_name");

                entity.Property(e => e.EventStartDate)
                    .HasColumnType("text")
                    .HasColumnName("event_start_date");

                entity.Property(e => e.EventType)
                    .HasColumnType("tinyint")
                    .HasColumnName("event_type");
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.Property(e => e.PlayerId).HasColumnName("player_id");

                entity.Property(e => e.BgColorHex)
                    .HasColumnType("text")
                    .HasColumnName("bg_color_hex");

                entity.Property(e => e.EncorePlayerInternalValue)
                    .HasColumnType("smallint")
                    .HasColumnName("encore_player_internal_value");

                entity.Property(e => e.EncorePlayerLeaderSkill)
                    .HasColumnType("smallint")
                    .HasColumnName("encore_player_leader_skill");

                entity.Property(e => e.EncorePlayerTotalPower)
                    .HasColumnType("mediumint")
                    .HasColumnName("encore_player_total_power");

                entity.Property(e => e.EventId)
                    .HasColumnType("bigint")
                    .HasColumnName("event_id");

                entity.Property(e => e.IsRunnerInteger)
                    .HasColumnType("tinyint")
                    .HasColumnName("is_runner_integer");

                entity.Property(e => e.NgPlayerIdData)
                    .HasColumnType("text")
                    .HasColumnName("ng_player_id_data");

                entity.Property(e => e.Notes)
                    .HasColumnType("text")
                    .HasColumnName("notes");

                entity.Property(e => e.PlayerInternalValue)
                    .HasColumnType("smallint")
                    .HasColumnName("player_internal_value");

                entity.Property(e => e.PlayerLeaderSkill)
                    .HasColumnType("smallint")
                    .HasColumnName("player_leader_skill");

                entity.Property(e => e.PlayerName)
                    .HasColumnType("text")
                    .HasColumnName("player_name");

                entity.Property(e => e.PlayerOtherName)
                    .HasColumnType("text")
                    .HasColumnName("player_other_name");

                entity.Property(e => e.PlayerTotalPower)
                    .HasColumnType("mediumint")
                    .HasColumnName("player_total_power");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.Players)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Shift>(entity =>
            {
                entity.Property(e => e.ShiftId).HasColumnName("shift_id");

                entity.Property(e => e.ActiveData)
                    .HasColumnType("text")
                    .HasColumnName("active_data");

                entity.Property(e => e.CanEncoreInteger)
                    .HasColumnType("tinyint")
                    .HasColumnName("can_encore_integer");

                entity.Property(e => e.CanStandbyInteger)
                    .HasColumnType("tinyint")
                    .HasColumnName("can_standby_integer");

                entity.Property(e => e.EncoreData)
                    .HasColumnType("text")
                    .HasColumnName("encore_data");

                entity.Property(e => e.EventId)
                    .HasColumnType("bigint")
                    .HasColumnName("event_id");

                entity.Property(e => e.Notes)
                    .HasColumnType("text")
                    .HasColumnName("notes");

                entity.Property(e => e.PlayerId)
                    .HasColumnType("bigint")
                    .HasColumnName("player_id");

                entity.Property(e => e.PositionData)
                    .HasColumnType("text")
                    .HasColumnName("position_data");

                entity.Property(e => e.ShiftDate)
                    .HasColumnType("text")
                    .HasColumnName("shift_date");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.Shifts)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.Shifts)
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
