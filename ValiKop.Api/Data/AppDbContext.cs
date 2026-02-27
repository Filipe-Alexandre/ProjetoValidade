using Microsoft.EntityFrameworkCore;
using ValiKop.Shared.Models;

namespace ValiKop.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        //DbSets para suas entidades (models / tabelas)
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Salgado> Salgados { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USUARIO - UNIQUE
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Login)
                .IsUnique();

            // PRODUTO - impede delete em cascata com categoria e usuario
            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Categoria)
                .WithMany()
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Usuario)
                .WithMany()
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Impede que duplicidade de produtos
            modelBuilder.Entity<Produto>()
                .HasIndex(p => new { p.Nome, p.Lote, p.CategoriaId })
                .IsUnique();

            //Impede delete em cascata
            modelBuilder.Entity<Salgado>()
                .HasOne(s => s.Usuario)
                .WithMany()
                .HasForeignKey(s => s.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            //Categoria - case INsensitive e unique"
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.Property(c => c.Nome)
                    .IsRequired()
                    .HasMaxLength(100)
                    .UseCollation("SQL_Latin1_General_CP1_CI_AS"); // CI = Case Insensitive

                entity.HasIndex(c => c.Nome)
                    .IsUnique();
            });

        }
    }
}
