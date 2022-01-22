using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestauranteAPI.Models
{
    public class RestauranteContext : DbContext
    {
        public DbSet<Encomenda> Encomenda { get; set; }
        public DbSet<Pedido> Pedido { get; set; }
        public DbSet<Estafeta> Estafeta { get; set; }
        public DbSet<Item> Item { get; set; }
        public DbSet<Produto> Produto { get; set; }


        public RestauranteContext(DbContextOptions<RestauranteContext> opcoes) : base(opcoes) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Encomenda>().HasOne(e => e.Estafeta);
            modelBuilder.Entity<Encomenda>()
                .HasMany(e => e.Itens)
                .WithOne(e => e.Encomenda) // Fazem parte de um encomenda
                .HasForeignKey(e => e.EncomendaId);

            modelBuilder.Entity<Pedido>()
                .HasMany(p => p.Itens)
                .WithOne(p => p.Pedido)
                .HasForeignKey(p => p.PedidoId);

            modelBuilder.Entity<Item>().HasOne(e => e.Produto);


            modelBuilder.Entity<Produto>().HasData(
                new Produto { Id = 1, Nome = "Água", Preco = 0.60M, Iva = 0.06 },
                new Produto { Id = 2, Nome = "Coca-Cola", Preco = 1M, Iva = 0.10 },
                new Produto { Id = 3, Nome = "Sumol", Preco = 5M, Iva = 0.10 },
                new Produto { Id = 4, Nome = "Francesinha", Preco = 15M, Iva = 0.10 },
                new Produto { Id = 5, Nome = "Mac 'n' Cheese", Preco = 10M, Iva = 0.10 },
                new Produto { Id = 6, Nome = "Sushi de Baixa Qualidade", Preco = 3M, Iva = 0.10 },
                new Produto { Id = 7, Nome = "Sopa de pedra", Preco = 4M, Iva = 0.13 }
            );

            modelBuilder.Entity<Estafeta>().HasData(
                new Estafeta { Id = 1, Nome = "Pedro Reis", Disponivel = false },
                new Estafeta { Id = 2, Nome = "Rodrigo Nogueira", Disponivel = false },
                new Estafeta { Id = 3, Nome = "Diogo Silva", Disponivel = false },
                new Estafeta { Id = 4, Nome = "Diogu Anjos", Disponivel = true },
                new Estafeta { Id = 5, Nome = "Nicole Silva", Disponivel = true },
                new Estafeta { Id = 6, Nome = "Bernardo Pacheco", Disponivel = true },
                new Estafeta { Id = 7, Nome = "Ana Ferrão", Disponivel = true }
            );

            modelBuilder.Entity<Encomenda>().HasData(
                new Encomenda
                {
                    Id = 1,
                    DataHoraAbertura = new DateTime(2022, 3, 2, 15, 00, 00),
                    DataHoraFecho = adicionarDataHoraFechoAuto(new DateTime(2022, 3, 2, 15, 00, 00)),
                    PrecoTotal = 62m,
                    Estado = "Em Preparação",
                    Morada = "Rua do Gatos, 17",
                    EstafetaId = 1
                },
                new Encomenda
                {
                    Id = 2,
                    DataHoraAbertura = new DateTime(2022, 4, 3, 16, 00, 00),
                    DataHoraFecho = adicionarDataHoraFechoAuto(new DateTime(2022, 4, 3, 16, 00, 00)),
                    PrecoTotal = 70m,
                    Estado = "Em Preparação",
                    Morada = "Rua do Gatos, 19",
                    EstafetaId = 2
                },
                new Encomenda
                {
                    Id = 3,
                    DataHoraAbertura = new DateTime(2022, 5, 4, 17, 00, 00),
                    DataHoraFecho = adicionarDataHoraFechoAuto(new DateTime(2022, 5, 4, 17, 00, 00)),
                    PrecoTotal = 50m,
                    Estado = "Em Preparação",
                    Morada = "Rua do Gatos, 20",
                    EstafetaId = 3
                }
            );

            modelBuilder.Entity<Item>().HasData(
                new Item { Id = 1, ProdutoId = 2, Quantidade = 2, EncomendaId = 1},
                new Item { Id = 2, ProdutoId = 4, Quantidade = 2, EncomendaId = 1},

                new Item { Id = 3, ProdutoId = 5, Quantidade = 6, EncomendaId = 2},
                new Item { Id = 4, ProdutoId = 3, Quantidade = 2, EncomendaId = 2},

                new Item { Id = 5, ProdutoId = 3, Quantidade = 4, EncomendaId = 3},
                new Item { Id = 6, ProdutoId = 6, Quantidade = 10, EncomendaId = 3},

                new Item { Id = 7, ProdutoId = 7, Quantidade = 1, PedidoId = 1}
            );

            modelBuilder.Entity<Pedido>().HasData(
                new Pedido
                {
                    Id = 1,
                    NumeroMesa = 1,
                    Disponivel = false,
                    DataHoraAbertura = new DateTime(2022, 10, 9, 21, 00, 00),
                    DataHoraFecho = adicionarDataHoraFechoAuto(new DateTime(2022, 10, 9, 21, 00, 00)),
                    PrecoTotal = 4M,
                    Estado = "Em Preparação"
                }

            );

        }

        private DateTime adicionarDataHoraFechoAuto(DateTime data)
        {
            return data.AddMinutes(45);
        }


    }
}
