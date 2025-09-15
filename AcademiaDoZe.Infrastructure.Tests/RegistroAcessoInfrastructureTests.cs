//Rafael dos Santos Tavares
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace AcademiaDoZe.Infrastructure.Tests
{
    public class RegistroAcessoInfrastructureTests : TestBase
    {
        #region Helpers

        private async Task<Aluno> CriarAlunoDeTesteTemporario()
        {
            var repoAdicionarAluno = new AlunoRepository(ConnectionString, DatabaseType);
            var repoObterPorIdLogradouro = new LogradouroRepository(ConnectionString, DatabaseType);
            var logradouro = await repoObterPorIdLogradouro.ObterPorId(4);
            Assert.NotNull(logradouro);
            var random = new Random();

            var aluno = Aluno.Criar(
                "Aluno Para Matricula",
                $"{random.NextInt64(0, 100_000_000_000L).ToString("D11")}",
                new DateOnly(1995, 1, 1),
                "49911110000",
                "matricula@teste.com",
                null, "1", null, logradouro);

            return await repoAdicionarAluno.Adicionar(aluno);
        }

        private async Task<Colaborador> CriarColaboradorDeTesteTemporario()
        {
            var repoAdicionarColaborador = new ColaboradorRepository(ConnectionString, DatabaseType);
            var repoObterPorIdLogradouro = new LogradouroRepository(ConnectionString, DatabaseType);
            var logradouro = await repoObterPorIdLogradouro.ObterPorId(4);
            Assert.NotNull(logradouro);
            Arquivo arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 }, ".pdf");

            var colaborador = Colaborador.Criar("Colab Acesso Teste", "12121212121",
                new DateOnly(1990, 1, 1), "49922220000", "colab.acesso@teste.com", arquivo, "2", null, logradouro,
                DateOnly.FromDateTime(DateTime.Today), ETipoColaboradorEnum.Instrutor, ETipoVinculoEnum.CLT);

            return await repoAdicionarColaborador.Adicionar(colaborador);
        }

        #endregion

        [Fact]
        public async Task Adicionar_E_Atualizar_DeveRegistrarChegadaESaida()
        {
            // Arrange
            Aluno aluno = null;
            RegistroAcesso registroChegada = null;
            var horaChegadaValida = DateTime.Today.AddHours(10);

            try
            {
                aluno = await CriarAlunoDeTesteTemporario();
                var registro = RegistroAcesso.Criar(ETipoPessoaEnum.Aluno, aluno, horaChegadaValida);

                var repoAdicionarRegistroAcesso = new RegistroAcessoRepository(ConnectionString, DatabaseType);
                registroChegada = await repoAdicionarRegistroAcesso.Adicionar(registro);

                Assert.NotNull(registroChegada);
                Assert.True(registroChegada.Id > 0);
                Assert.Null(registroChegada.DataHoraSaida);

                var saidaProperty = typeof(RegistroAcesso).GetProperty("DataHoraSaida", BindingFlags.Public | BindingFlags.Instance);
                saidaProperty?.SetValue(registroChegada, horaChegadaValida.AddHours(1));

                var repoAtualizarRegistroAcesso = new RegistroAcessoRepository(ConnectionString, DatabaseType);
                await repoAtualizarRegistroAcesso.Atualizar(registroChegada);

                var repoObterPorIdRegistroAcesso = new RegistroAcessoRepository(ConnectionString, DatabaseType);
                var registroVerificacao = await repoObterPorIdRegistroAcesso.ObterPorId(registroChegada.Id);
                Assert.NotNull(registroVerificacao);
                Assert.NotNull(registroVerificacao.DataHoraSaida);
            }
            finally
            {
                if (registroChegada?.Id > 0) await new RegistroAcessoRepository(ConnectionString, DatabaseType).Remover(registroChegada.Id);
                if (aluno?.Id > 0) await new AlunoRepository(ConnectionString, DatabaseType).Remover(aluno.Id);
            }
        }

        [Fact]
        public async Task GetAcessosPorAlunoPeriodo_DeveFiltrarCorretamente()
        {
            // Arrange
            Aluno alunoA = null;
            Aluno alunoB = null;
            var registros = new List<RegistroAcesso>();

            try
            {
                alunoA = await CriarAlunoDeTesteTemporario();
                alunoB = await CriarAlunoDeTesteTemporario();

               
                registros.Add(await new RegistroAcessoRepository(ConnectionString, DatabaseType).Adicionar(RegistroAcesso.Criar(ETipoPessoaEnum.Aluno, alunoA, DateTime.Today.AddHours(9)))); 
                registros.Add(await new RegistroAcessoRepository(ConnectionString, DatabaseType).Adicionar(RegistroAcesso.Criar(ETipoPessoaEnum.Aluno, alunoA, DateTime.Today.AddHours(11))));
                registros.Add(await new RegistroAcessoRepository(ConnectionString, DatabaseType).Adicionar(RegistroAcesso.Criar(ETipoPessoaEnum.Aluno, alunoB, DateTime.Today.AddHours(10))));

                // Act: Buscar todos os registros do Aluno A para a data de hoje.
                var resultado = await new RegistroAcessoRepository(ConnectionString, DatabaseType).GetAcessosPorAlunoPeriodo(alunoA.Id, DateOnly.FromDateTime(DateTime.Today));

                // Assert
                Assert.NotNull(resultado);
                // CORREÇÃO: O esperado agora são 2 registros para o aluno A.
                Assert.Equal(2, resultado.Count());
                Assert.True(resultado.All(r => r.Pessoa.Id == alunoA.Id));
            }
            finally
            {
                // Cleanup
                foreach (var r in registros) await new RegistroAcessoRepository(ConnectionString, DatabaseType).Remover(r.Id);
                if (alunoA?.Id > 0) await new AlunoRepository(ConnectionString, DatabaseType).Remover(alunoA.Id);
                if (alunoB?.Id > 0) await new AlunoRepository(ConnectionString, DatabaseType).Remover(alunoB.Id);
            }
        }

        [Fact]
        public async Task GetAcessosPorColaboradorPeriodo_DeveFiltrarCorretamente()
        {
            // Arrange
            Colaborador colabA = null;
            var registros = new List<RegistroAcesso>();

            try
            {
                colabA = await CriarColaboradorDeTesteTemporario();
                // Usando horário válido.
                registros.Add(await new RegistroAcessoRepository(ConnectionString, DatabaseType).Adicionar(RegistroAcesso.Criar(ETipoPessoaEnum.Colaborador, colabA, DateTime.Today.AddHours(8))));

                // Act
                var resultado = await new RegistroAcessoRepository(ConnectionString, DatabaseType).GetAcessosPorColaboradorPeriodo(colabA.Id, DateOnly.FromDateTime(DateTime.Today));

                // Assert
                Assert.NotNull(resultado);
                Assert.Single(resultado);
            }
            finally
            {
                // Cleanup
                if (registros.Any()) await new RegistroAcessoRepository(ConnectionString, DatabaseType).Remover(registros.First().Id);
                if (colabA?.Id > 0) await new ColaboradorRepository(ConnectionString, DatabaseType).Remover(colabA.Id);
            }
        }

        [Fact]
        public async Task GetAlunosSemAcessoNosUltimosDias_DeveEncontrarAlunoInativo()
        {
            // Arrange
            var repo = new RegistroAcessoRepository(ConnectionString, DatabaseType);
            Aluno alunoAtivo = null;
            Aluno alunoInativo = null;
            RegistroAcesso registro = null;

            try
            {
                alunoAtivo = await CriarAlunoDeTesteTemporario();
                alunoInativo = await CriarAlunoDeTesteTemporario();
                registro = await new RegistroAcessoRepository(ConnectionString, DatabaseType).Adicionar(RegistroAcesso.Criar(ETipoPessoaEnum.Aluno, alunoAtivo, DateTime.Today.AddHours(14)));

                // Act
                var resultado = await new RegistroAcessoRepository(ConnectionString, DatabaseType).GetAlunosSemAcessoNosUltimosDias(5);

                // Assert
                Assert.NotNull(resultado);
                Assert.Contains(resultado, a => a.Id == alunoInativo.Id);
                Assert.DoesNotContain(resultado, a => a.Id == alunoAtivo.Id);
            }
            finally
            {
                // Cleanup
                if (registro?.Id > 0) await new RegistroAcessoRepository(ConnectionString, DatabaseType).Remover(registro.Id);
                if (alunoAtivo?.Id > 0) await new AlunoRepository(ConnectionString, DatabaseType).Remover(alunoAtivo.Id);
                if (alunoInativo?.Id > 0) await new AlunoRepository(ConnectionString, DatabaseType).Remover(alunoInativo.Id);
            }
        }
    }
}