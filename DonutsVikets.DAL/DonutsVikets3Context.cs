using Microsoft.EntityFrameworkCore;
using DonutsVikets.Models;
using System.Security.Cryptography;
using System.Text;

namespace DonutsVikets.DAL.Data
{
    public class DonutsVikets3Context : DbContext
    {
        public DonutsVikets3Context(DbContextOptions<DonutsVikets3Context> options)
            : base(options)
        {
        }

        // DbSets (conforme os models fornecidos)
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<TipoUsuario> TipoUsuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemPedido> ItensPedido { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=DonutsVikets3Context;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //---------------------------------------------------------
            // SEED: TipoUsuario
            //---------------------------------------------------------
            modelBuilder.Entity<TipoUsuario>().HasData(
                new TipoUsuario { Id = 1, Nome = "Administrador" },
                new TipoUsuario { Id = 2, Nome = "Cliente" }
            );

            //---------------------------------------------------------
            // SEED: Usuário Administrador (senha = 123456)
            //---------------------------------------------------------
            var hash = HashPassword("123456");

            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Nome = "Admin",
                    Email = "admin@teste.com",
                    SenhaHash = hash,
                    TipoUsuarioId = 1
                }
            );

            //---------------------------------------------------------
            // SEED: Categorias (conforme modelo real)
            //---------------------------------------------------------
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, Nome = "Clássicos", Descricao = "Donuts tradicionais" },
                new Categoria { Id = 2, Nome = "Recheados", Descricao = "Donuts recheados" },
                new Categoria { Id = 3, Nome = "Premium", Descricao = "Donuts especiais premium" }
            );

            //---------------------------------------------------------
            // SEED: Produtos (conforme propriedades reais das models)
            //---------------------------------------------------------
            modelBuilder.Entity<Produto>().HasData(
    new Produto
    {
        Id = 1,
        Nome = "Donut Tradicional",
        Descricao = "Simples e delicioso",
        Preco = 5.50m,
        ImagemUrl = "",     // ← REQUIRED! Agora ok
        CategoriaId = 1
    },
    new Produto
    {
        Id = 2,
        Nome = "Donut Recheado Chocolate",
        Descricao = "Recheado com chocolate",
        Preco = 8.90m,
        ImagemUrl = "",     // ← REQUIRED
        CategoriaId = 2
    },
    new Produto
    {
        Id = 3,
        Nome = "Donut Caramelo Premium",
        Descricao = "Caramelo artesanal",
        Preco = 12.50m,
        ImagemUrl = "",     // ← REQUIRED
        CategoriaId = 3
    }
);


            // SEED: Pedido de exemplo (Data fixa — EF Core exige isso)
            var dataFixa = new DateTime(2024, 01, 01, 12, 00, 00);

            modelBuilder.Entity<Pedido>().HasData(
                new Pedido
                {
                    Id = 1,
                    DataPedido = dataFixa,
                    Status = "Pago",
                    UsuarioId = 1
                }
            );


            //---------------------------------------------------------
            // SEED: Itens do Pedido
            //---------------------------------------------------------
            modelBuilder.Entity<ItemPedido>().HasData(
                new ItemPedido
                {
                    Id = 1,
                    PedidoId = 1,
                    ProdutoId = 1,
                    Quantidade = 2,
                    PrecoUnitario = 5.50m
                },
                new ItemPedido
                {
                    Id = 2,
                    PedidoId = 1,
                    ProdutoId = 2,
                    Quantidade = 1,
                    PrecoUnitario = 8.90m
                }
            );
        }

        //---------------------------------------------------------
        // Função auxiliar para gerar hash SHA256
        //---------------------------------------------------------
        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash).ToLower();
        }
    }
}
