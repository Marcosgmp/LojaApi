using LojaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LojaApi.Data;

public class LojaContext : DbContext
{
    public LojaContext(DbContextOptions<LojaContext> options) : base(options) { }

    public DbSet<Pessoa> Pessoas => Set<Pessoa>();
    public DbSet<Endereco> Enderecos => Set<Endereco>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<ProdutoTag> ProdutoTags => Set<ProdutoTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ---------- 1:1  Pessoa <-> Endereco ----------
        modelBuilder.Entity<Pessoa>()
            .HasOne(p => p.Endereco)
            .WithOne(e => e.Pessoa)
            .HasForeignKey<Endereco>(e => e.PessoaId)
            .OnDelete(DeleteBehavior.Cascade);

        // ---------- 1:N  Categoria -> Produto ----------
        modelBuilder.Entity<Categoria>()
            .HasMany(c => c.Produtos)
            .WithOne(p => p.Categoria)
            .HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---------- N:M  Produto <-> Tag (via ProdutoTag) ----------
        modelBuilder.Entity<ProdutoTag>()
            .HasKey(pt => new { pt.ProdutoId, pt.TagId });

        modelBuilder.Entity<ProdutoTag>()
            .HasOne(pt => pt.Produto)
            .WithMany(p => p.ProdutoTags)
            .HasForeignKey(pt => pt.ProdutoId);

        modelBuilder.Entity<ProdutoTag>()
            .HasOne(pt => pt.Tag)
            .WithMany(t => t.ProdutoTags)
            .HasForeignKey(pt => pt.TagId);

        // Restrições simples de dados
        modelBuilder.Entity<Produto>()
            .Property(p => p.Preco)
            .HasColumnType("decimal(10,2)");
    }
}
