using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Model;

public partial class PlataformaEducativaOnlineDbContext : DbContext
{
    public PlataformaEducativaOnlineDbContext()
    {
    }

    public PlataformaEducativaOnlineDbContext(DbContextOptions<PlataformaEducativaOnlineDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Calificacione> Calificaciones { get; set; }

    public virtual DbSet<Curso> Cursos { get; set; }

    public virtual DbSet<Estudiante> Estudiantes { get; set; }

    public virtual DbSet<Profesore> Profesores { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=host.docker.internal,8000;Initial Catalog=PlataformaEducativaOnlineDB; User ID=sa; Password=Chau45-99; MultipleActiveResultSets=true;Connection Timeout=30;Trust Server Certificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Calificacione>(entity =>
        {
            entity.HasKey(e => e.CalId);

            entity.Property(e => e.CalId).HasColumnName("calId");
            entity.Property(e => e.CalCursoId).HasColumnName("calCursoId");
            entity.Property(e => e.CalEstudianteId).HasColumnName("calEstudianteId");
            entity.Property(e => e.CalNota).HasColumnName("calNota");

            entity.HasOne(d => d.CalCurso).WithMany(p => p.Calificaciones)
                .HasForeignKey(d => d.CalCursoId)
                .HasConstraintName("FK_Calificaciones_Cursos");

            entity.HasOne(d => d.CalEstudiante).WithMany(p => p.Calificaciones)
                .HasForeignKey(d => d.CalEstudianteId)
                .HasConstraintName("FK_Calificaciones_Estudiantes");
        });

        modelBuilder.Entity<Curso>(entity =>
        {
            entity.HasKey(e => e.CurId);

            entity.Property(e => e.CurId).HasColumnName("curId");
            entity.Property(e => e.CurDescripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("curDescripcion");
            entity.Property(e => e.CurNombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("curNombre");
            entity.Property(e => e.CurProfesorId).HasColumnName("curProfesorId");

            entity.HasOne(d => d.CurProfesor).WithMany(p => p.Cursos)
                .HasForeignKey(d => d.CurProfesorId)
                .HasConstraintName("FK_Cursos_Profesores");
        });

        modelBuilder.Entity<Estudiante>(entity =>
        {
            entity.HasKey(e => e.EstId);

            entity.Property(e => e.EstId).HasColumnName("estId");
            entity.Property(e => e.EstCorreo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("estCorreo");
            entity.Property(e => e.EstNombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("estNombre");
        });

        modelBuilder.Entity<Profesore>(entity =>
        {
            entity.HasKey(e => e.ProId);

            entity.Property(e => e.ProId).HasColumnName("proId");
            entity.Property(e => e.ProCorreo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("proCorreo");
            entity.Property(e => e.ProNombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("proNombre");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
