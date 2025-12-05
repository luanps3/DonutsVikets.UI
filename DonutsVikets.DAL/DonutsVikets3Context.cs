// Project.DAL/Data/DonutsVikets3Context.cs
using Microsoft.EntityFrameworkCore;
using DonutsVikets.Models; // ajuste conforme o namespace real das suas models

namespace Project.DAL.Data
{
    public class DonutsVikets3Context : DbContext
    {
        public DonutsVikets3Context(DbContextOptions<DonutsVikets3Context> options)
            : base(options)
        {
        }

        // DbSets (existem nas models que você enviou)
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<TipoUsuario> TipoUsuarios { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemPedido> ItensPedido { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=DonutsVikets3Context;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Exemplo de seed para Tipos de Usuario (Administrador / Cliente)
            modelBuilder.Entity<TipoUsuario>().HasData(
                new TipoUsuario { Id = 1, Nome = "Administrador" },
                new TipoUsuario { Id = 2, Nome = "Cliente" }
            );
        }
    }
}
