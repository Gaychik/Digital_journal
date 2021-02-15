using DigitalJournal.Models;
using DigitalJournal.Models.Abstractions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.IO;

namespace DigitalJournal.Data.Context
{
    internal class DigitalJournalContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = JObject.Parse(File.ReadAllText("dbgatewaysettings.json"))["connectionString"].ToObject<string>();
            optionsBuilder.UseSqlServer(connectionString);
        }

        public DigitalJournalContext()
        {
        }

        public DbSet<Human> Humen { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Moderator> Moderators { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Mark> Marks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Human>().Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
            modelBuilder.Entity<Group>().Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
            modelBuilder.Entity<Mark>().Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
            modelBuilder.Entity<Lesson>().Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
            modelBuilder.Entity<Subject>().Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()");

            modelBuilder.Entity<Group>().HasOne(x => x.Teacher).WithOne(x => x.Group).HasForeignKey<Group>();
        }
    }
}