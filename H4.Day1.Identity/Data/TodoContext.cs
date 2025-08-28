using Microsoft.EntityFrameworkCore;

namespace H4.Day1.Identity.Data
{
public partial class TodoContext : DbContext
{
    public TodoContext()
    {
    }

    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cpr> Cprs { get; set; }

    public virtual DbSet<Todolist> Todolists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cpr>(entity =>
        {
            entity.ToTable("Cpr");

            entity.Property(e => e.CprNr).HasMaxLength(500);
            entity.Property(e => e.User).HasMaxLength(500);
        });

        modelBuilder.Entity<Todolist>(entity =>
        {
            entity.HasKey(e => e.Int);

            entity.ToTable("Todolist");

            entity.Property(e => e.Int).HasColumnName("int");
            entity.Property(e => e.Item).HasMaxLength(500);

            entity.HasOne(d => d.User).WithMany(p => p.Todolists)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Todolist_Cpr");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
}
