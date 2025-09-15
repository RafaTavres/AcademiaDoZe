//Rafael dos Santos Tavares
using AcademiaDoZe.Application.Tests;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AcademiaDoZe._Application.Tests.AplicationTests
{
    public class AlunoApplicationTests
    {
        // Configurações de conexão
        const string connectionString = "Server=localhost,1433;Database=db_academia_do_ze;User Id=sa;Password=abcBolinhas12345;TrustServerCertificate=True;Encrypt=True;";
        const EAppDatabaseType databaseType = EAppDatabaseType.SqlServer;

        [Fact(Timeout = 60000)]
        public async Task AlunoService_Integracao_Adicionar_Obter_Atualizar_Remover()
        {
            var services = DependencyInjection.ConfigureServices(connectionString, databaseType);
            var provider = DependencyInjection.BuildServiceProvider(services);

            var alunoService = provider.GetRequiredService<IAlunoService>();
            var logradouroService = provider.GetRequiredService<ILogradouroService>();

            var cpfUnico = new Random().Next(100000000, 999999999).ToString("D11");
            var cepUnico = new Random().Next(99500000, 99599999).ToString();

            var logradouro = await logradouroService.ObterPorIdAsync(5);

            LogradouroDTO? logradouroCriado = null;
            AlunoDTO? alunoCriado = null;

            try
            {
                // Arrange - Passo 2: Preparar o DTO do Aluno
                var alunoDto = new AlunoDTO
                {
                    Nome = "Aluno Teste",
                    Cpf = cpfUnico,
                    DataNascimento = new DateOnly(2000, 1, 1),
                    Telefone = "54912345678",
                    Email = "aluno.teste@email.com",
                    Endereco = logradouro,
                    Numero = "123",
                    Complemento = "Apto 101"
                };

                // Act - Adicionar Aluno
                alunoCriado = await alunoService.AdicionarAsync(alunoDto);
                Assert.NotNull(alunoCriado);
                Assert.True(alunoCriado.Id > 0);
                Assert.Equal(cpfUnico, alunoCriado.Cpf);

                // Act - Obter por CPF
                var obtidoPorCpf = await alunoService.ObterPorCpfAsync(cpfUnico);
                Assert.NotNull(obtidoPorCpf);
                Assert.Equal("Aluno Teste", obtidoPorCpf.Nome);

                // Act - Atualizar Aluno
                var atualizarDto = new AlunoDTO
                {
                    Id = alunoCriado.Id,
                    Nome = "Aluno Teste Atualizado", // Nome alterado
                    Cpf = alunoCriado.Cpf,
                    DataNascimento = alunoCriado.DataNascimento,
                    Telefone = "54987654321", // Telefone alterado
                    Endereco = alunoCriado.Endereco,
                    Numero = "123B"
                };
                var atualizado = await alunoService.AtualizarAsync(atualizarDto);
                Assert.NotNull(atualizado);
                Assert.Equal("Aluno Teste Atualizado", atualizado.Nome);
                Assert.Equal("54987654321", atualizado.Telefone);

                // Act - Remover Aluno
                var removido = await alunoService.RemoverAsync(alunoCriado.Id);
                Assert.True(removido);

                // Assert - Conferir remoção
                var aposRemocao = await alunoService.ObterPorIdAsync(alunoCriado.Id);
                Assert.Null(aposRemocao);
            }
            finally
            {
                // Clean-up defensivo: remove os registros criados, mesmo se o teste falhar
                if (alunoCriado is not null)
                {
                    try { await alunoService.RemoverAsync(alunoCriado.Id); } catch { }
                }
                if (logradouroCriado is not null)
                {
                    try { await logradouroService.RemoverAsync(logradouroCriado.Id); } catch { }
                }
            }
        }
    }
}