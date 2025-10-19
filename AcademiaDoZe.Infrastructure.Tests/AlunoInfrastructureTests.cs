//Rafael dos Santos Tavares
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AcademiaDoZe.Infrastructure.Tests
{
    public class AlunoInfrastructureTests : TestBase
    {
        // Helper para criar uma instância de Aluno para os testes
        private async Task<Aluno> CriarAlunoDeTesteInstance(string cpf)
        {
            var logradouroId = 4; // Certifique-se que este ID existe no seu banco de teste
            var repoLogradouro = new LogradouroRepository(ConnectionString, DatabaseType);
            Logradouro? logradouro = await repoLogradouro.ObterPorId(logradouroId);

            Assert.NotNull(logradouro); // Se o logradouro não existir, o teste para aqui.

            Arquivo arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 }, ".pdf");

            return Aluno.Criar(
                "Aluno Teste Limpeza",
                cpf,
                new DateOnly(2000, 5, 20),
                "49912345678",
                "teste.limpeza@email.com",
                arquivo,
                "123",
                "Apto 456",
                logradouro
            );
        }

        [Fact]
        public async Task Adicionar_DeveInserirEApagarComSucesso()
        {
            // Arrange
            var repo = new AlunoRepository(ConnectionString, DatabaseType);
            var cpf = "10000000001";
            var alunoParaAdicionar = await CriarAlunoDeTesteInstance(cpf);
            Aluno alunoInserido = null;

            try
            {
                // Act
                alunoInserido = await repo.Adicionar(alunoParaAdicionar);

                // Assert
                Assert.NotNull(alunoInserido);
                Assert.True(alunoInserido.Id > 0);
            }
            finally
            {
                // Cleanup: Garante que o aluno seja removido, mesmo se o Assert falhar.
                if (alunoInserido?.Id > 0)
                {
                    var newRepo = new AlunoRepository(ConnectionString, DatabaseType);
                    await newRepo.Remover(alunoInserido.Id);
                }
            }
        }

        [Fact]
        public async Task Atualizar_DeveModificarDadosTemporariamente()
        {
            // Arrange
            var repoAdicionarAluno = new AlunoRepository(ConnectionString, DatabaseType);
            var cpf = "10000000002";
            var alunoOriginal = await CriarAlunoDeTesteInstance(cpf);
            Aluno alunoInserido = null;

            try
            {
                alunoInserido = await repoAdicionarAluno.Adicionar(alunoOriginal);
                Assert.NotNull(alunoInserido);

                var alunoParaAtualizar = Aluno.Criar(
                    "Nome Foi Atualizado",
                    alunoInserido.Cpf,
                    alunoInserido.DataNascimento,
                    "49987654321",
                    "email.atualizado@teste.com",
                    alunoInserido.Foto,
                    "321-A",
                    alunoInserido.Complemento,
                    alunoInserido.Endereco);

                var idProperty = typeof(Entity).GetProperty("Id");
                idProperty?.SetValue(alunoParaAtualizar, alunoInserido.Id);

                // Act
                var repoAtualizarAluno = new AlunoRepository(ConnectionString, DatabaseType);
                await repoAtualizarAluno.Atualizar(alunoParaAtualizar);

                // Assert
                var repoObterPorIdAluno = new AlunoRepository(ConnectionString, DatabaseType);
                var alunoVerificacao = await repoObterPorIdAluno.ObterPorId(alunoInserido.Id);
                Assert.NotNull(alunoVerificacao);
                Assert.Equal("Nome Foi Atualizado", alunoVerificacao.Nome);
                Assert.Equal("49987654321", alunoVerificacao.Telefone);
            }
            finally
            {
                // Cleanup
                if (alunoInserido?.Id > 0)
                {
                    var repoRemoverAluno = new AlunoRepository(ConnectionString, DatabaseType);
                    await repoRemoverAluno.Remover(alunoInserido.Id);
                }
            }
        }

        [Fact]
        public async Task Remover_DeveExcluirOAlunoCorretamente()
        {
            // Arrange
            var cpf = "10000000003";
            var alunoParaRemover = await CriarAlunoDeTesteInstance(cpf);

            // Adiciona o aluno que será removido no teste.
            var repoAdicionarAluno = new AlunoRepository(ConnectionString, DatabaseType);
            var alunoInserido = await repoAdicionarAluno.Adicionar(alunoParaRemover);
            Assert.NotNull(alunoInserido);

            // Act
            var repoRemoverAluno = new AlunoRepository(ConnectionString, DatabaseType);
            var resultadoRemocao = await repoRemoverAluno.Remover(alunoInserido.Id);

            // Assert
            Assert.True(resultadoRemocao);
            var repoObterPorIdAluno = new AlunoRepository(ConnectionString, DatabaseType);
            var alunoVerificacao = await repoObterPorIdAluno.ObterPorId(alunoInserido.Id);
            Assert.Null(alunoVerificacao);
            // Cleanup: Não é necessário um bloco finally aqui, pois o propósito do teste é a remoção.
        }

        [Fact]
        public async Task ObterPorCpf_DeveEncontrarAlunoExistente()
        {
            // Arrange
            var cpf = "10000000004";
            var alunoParaTeste = await CriarAlunoDeTesteInstance(cpf);
            Aluno alunoInserido = null;

            try
            {
                var repoAdicionarAluno = new AlunoRepository(ConnectionString, DatabaseType);
                alunoInserido = await repoAdicionarAluno.Adicionar(alunoParaTeste);
                Assert.NotNull(alunoInserido);

                // Act
                var repoObterPorCpfAluno = new AlunoRepository(ConnectionString, DatabaseType);
                var alunosEncontrados = await repoObterPorCpfAluno.ObterPorCpf(cpf);

                // Assert
                Assert.NotNull(alunosEncontrados);
                Assert.Equal(alunoInserido.Id, alunosEncontrados.First().Id);
            }
            finally
            {
                // Cleanup
                if (alunoInserido?.Id > 0)
                {
                    var repoRemoverALuno = new AlunoRepository(ConnectionString, DatabaseType);
                    await repoRemoverALuno.Remover(alunoInserido.Id);
                }
            }
        }

        [Fact]
        public async Task CpfJaExiste_DeveRetornarVerdadeiroParaCpfExistente()
        {
            // Arrange
            var cpf = "10000000005";
            var alunoParaTeste = await CriarAlunoDeTesteInstance(cpf);
            Aluno alunoInserido = null;

            try
            {
                var repoAdicionarAluno = new AlunoRepository(ConnectionString, DatabaseType);
                alunoInserido = await repoAdicionarAluno.Adicionar(alunoParaTeste);
                Assert.NotNull(alunoInserido);

                // Act
                var repoCpfJaExisteAluno = new AlunoRepository(ConnectionString, DatabaseType);
                var existe = await repoCpfJaExisteAluno.CpfJaExiste(cpf);

                // Assert
                Assert.True(existe);
            }
            finally
            {
                // Cleanup
                if (alunoInserido?.Id > 0)
                {
                    var repoRemoverALuno = new AlunoRepository(ConnectionString, DatabaseType);
                    await repoRemoverALuno.Remover(alunoInserido.Id);
                }
            }
        }
    }
}