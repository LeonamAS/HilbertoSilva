using HilbertoSilva.Models;
using Microsoft.EntityFrameworkCore;

namespace HilbertoSilva.Data;

public class EscolaDbContext : DbContext
{
    public EscolaDbContext(DbContextOptions<EscolaDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Turma> Turmas { get; set; }
    public DbSet<Aluno> Alunos { get; set; }
    public DbSet<Professor> Professores { get; set; }
    public DbSet<Disciplina> Disciplinas { get; set; }
    public DbSet<DiarioClasse> DiarioClasse { get; set; }
    public DbSet<Boletim> Boletins { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ==========================================
        // Conversão do Enum TipoUsuario para String
        // ==========================================
        modelBuilder.Entity<Usuario>()
            .Property(u => u.TipoUsuario)
            .HasConversion<string>();

        // ==========================================
        // 1. Configuração de Índices Únicos
        // ==========================================
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Cpf)
            .IsUnique();

        modelBuilder.Entity<Aluno>()
            .HasIndex(a => a.Matricula)
            .IsUnique();

        // ==========================================
        // 2. Relacionamentos 1:1 (Usuários)
        // ==========================================
        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Aluno)
            .WithOne(a => a.Usuario)
            .HasForeignKey<Aluno>(a => a.FkUsuario)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Professor)
            .WithOne(p => p.Usuario)
            .HasForeignKey<Professor>(p => p.FkUsuario)
            .OnDelete(DeleteBehavior.Cascade);

        // ==========================================
        // 3. Relacionamento N:1 (Aluno -> Turma)
        // ==========================================
        modelBuilder.Entity<Aluno>()
            .HasOne(a => a.Turma)
            .WithMany(t => t.Alunos)
            .HasForeignKey(a => a.FkTurma)
            .OnDelete(DeleteBehavior.SetNull);

        // ==========================================
        // 4. Regras do Boletim 
        // ==========================================

        // Garante que o aluno não tenha dois boletins da mesma matéria na mesma turma
        modelBuilder.Entity<Boletim>()
            .HasIndex(b => new { b.FkAluno, b.FkTurmaDisciplina })
            .IsUnique()
            .HasDatabaseName("aluno_materia_unico");

        // Exclusão em cascata do Boletim
        modelBuilder.Entity<Boletim>()
            .HasOne(b => b.Aluno)
            .WithMany()
            .HasForeignKey(b => b.FkAluno)
            .OnDelete(DeleteBehavior.Cascade);

        // ==========================================
        // 5. Precisão dos campos Decimal (Notas e Frequência)
        // ==========================================
        modelBuilder.Entity<Boletim>().Property(b => b.NotaU1).HasPrecision(4, 2);
        modelBuilder.Entity<Boletim>().Property(b => b.NotaU2).HasPrecision(4, 2);
        modelBuilder.Entity<Boletim>().Property(b => b.NotaU3).HasPrecision(4, 2);
        modelBuilder.Entity<Boletim>().Property(b => b.Frequencia).HasPrecision(5, 2);
    }
}