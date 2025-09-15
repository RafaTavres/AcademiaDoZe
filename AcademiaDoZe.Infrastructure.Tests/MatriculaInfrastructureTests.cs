//Rafael dos Santos Tavares
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Repositories;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace AcademiaDoZe.Infrastructure.Tests
{
    public class MatriculaInfrastructureTests : TestBase
    {
        private async Task<Aluno> CriarAlunoDeTesteTemporario()
        {
            var repo = new AlunoRepository(ConnectionString, DatabaseType);
            var logradouroRepo = new LogradouroRepository(ConnectionString, DatabaseType);
            var logradouro = await logradouroRepo.ObterPorId(4);
            Assert.NotNull(logradouro);

            var aluno = Aluno.Criar(
                "Aluno Para Matricula",
                $"12121212121",
                new DateOnly(1995, 1, 1),
                "49911110000",
                "matricula@teste.com",
                null, "1", null, logradouro);

            return await repo.Adicionar(aluno);
        }

        private async Task<Plano> CriarPlanoDeTesteTemporario()
        {
            // Agora utiliza o PlanoRepository que você forneceu
            var repo = new PlanoRepository(ConnectionString, DatabaseType);
            var plano = Plano.Criar($"Plano Teste","plano feito para os testes", 99.90m, 30);
            return await repo.Adicionar(plano);
        }

        private Matricula CriarMatriculaDeTesteInstance(Aluno aluno, Plano plano, DateOnly dataFim)
        {
            return Matricula.Criar(
                aluno,
                plano,
                DateOnly.FromDateTime(DateTime.Today),
                dataFim,
                "Hipertrofia",
                EMatriculaRestricoesEnum.None,
                null,
                null
            );
        }


        [Fact]
        public async Task Adicionar_DeveInserirNovaMatriculaComSucesso()
        {
            // Arrange
            Aluno aluno = null;
            Plano plano = null;
            Matricula matriculaInserida = null;

            try
            {
                aluno = await CriarAlunoDeTesteTemporario();
                plano = await CriarPlanoDeTesteTemporario();
                var matricula = CriarMatriculaDeTesteInstance(aluno, plano, DateOnly.FromDateTime(DateTime.Today.AddMonths(1)));

                // Act
                var repoAdicionarMatricula = new MatriculaRepository(ConnectionString, DatabaseType);
                matriculaInserida = await repoAdicionarMatricula.Adicionar(matricula);

                // Assert
                Assert.NotNull(matriculaInserida);
                Assert.True(matriculaInserida.Id > 0);
            }
            finally
            {
                // Cleanup
                if (matriculaInserida?.Id > 0) await new MatriculaRepository(ConnectionString, DatabaseType).Remover(matriculaInserida.Id);
                if (aluno?.Id > 0) await new AlunoRepository(ConnectionString, DatabaseType).Remover(aluno.Id);
                if (plano?.Id > 0) await new PlanoRepository(ConnectionString, DatabaseType).Remover(plano.Id);
            }
        }

        [Fact]
        public async Task Atualizar_DeveModificarDadosDaMatricula()
        {
            // Arrange
            Aluno aluno = null;
            Plano plano = null;
            Matricula matriculaInserida = null;

            try
            {
                aluno = await CriarAlunoDeTesteTemporario();
                plano = await CriarPlanoDeTesteTemporario();
                var matriculaOriginal = CriarMatriculaDeTesteInstance(aluno, plano, DateOnly.FromDateTime(DateTime.Today.AddMonths(1)));
                var repoAdicionarMatricula = new MatriculaRepository(ConnectionString, DatabaseType);
                matriculaInserida = await repoAdicionarMatricula.Adicionar(matriculaOriginal);

                var matriculaParaAtualizar = Matricula.Criar(
                    aluno, plano, matriculaInserida.DataInicio, matriculaInserida.DataFim,
                    "Objetivo Atualizado", EMatriculaRestricoesEnum.None, "Observação atualizada", null);

                var idProperty = typeof(Entity).GetProperty("Id");
                idProperty?.SetValue(matriculaParaAtualizar, matriculaInserida.Id);

                // Act
                var repoAtualizarMatricula = new MatriculaRepository(ConnectionString, DatabaseType);
                await repoAtualizarMatricula.Atualizar(matriculaParaAtualizar);

                // Assert
                var repoObterPorIdMatricula = new MatriculaRepository(ConnectionString, DatabaseType);
                var matriculaVerificacao = await repoObterPorIdMatricula.ObterPorId(matriculaInserida.Id);
                Assert.NotNull(matriculaVerificacao);
                Assert.Equal("Objetivo Atualizado", matriculaVerificacao.Objetivo);
                Assert.Equal(EMatriculaRestricoesEnum.None, matriculaVerificacao.Restricoes);
            }
            finally
            {
                // Cleanup
                if (matriculaInserida?.Id > 0) await new MatriculaRepository(ConnectionString, DatabaseType).Remover(matriculaInserida.Id);
                if (aluno?.Id > 0) await new AlunoRepository(ConnectionString, DatabaseType).Remover(aluno.Id);
                if (plano?.Id > 0) await new PlanoRepository(ConnectionString, DatabaseType).Remover(plano.Id);
            }
        }

        [Fact]
        public async Task ObterPorAluno_DeveRetornarMatriculasDoAlunoCorreto()
        {
            // Arrange
            Aluno aluno = null;
            Plano plano = null;
            Matricula matricula1 = null;
            Matricula matricula2 = null;

            try
            {
                aluno = await CriarAlunoDeTesteTemporario();
                plano = await CriarPlanoDeTesteTemporario();
                var repoAdicionarMatricula = new MatriculaRepository(ConnectionString, DatabaseType);
                matricula1 = await repoAdicionarMatricula.Adicionar(CriarMatriculaDeTesteInstance(aluno, plano, DateOnly.FromDateTime(DateTime.Today.AddDays(10))));
                var repoSegundaAdicionarMatricula = new MatriculaRepository(ConnectionString, DatabaseType);
                matricula2 = await repoSegundaAdicionarMatricula.Adicionar(CriarMatriculaDeTesteInstance(aluno, plano, DateOnly.FromDateTime(DateTime.Today.AddDays(20))));

                // Act
                var repoObterPorAlunoMatricula = new MatriculaRepository(ConnectionString, DatabaseType);
                var resultado = await repoObterPorAlunoMatricula.ObterPorAluno(aluno.Id);

                // Assert
                Assert.NotNull(resultado);
                Assert.Equal(2, resultado.Count());
            }
            finally
            {
                // Cleanup
                if (matricula1?.Id > 0) await new MatriculaRepository(ConnectionString, DatabaseType).Remover(matricula1.Id);
                if (matricula2?.Id > 0) await new MatriculaRepository(ConnectionString, DatabaseType).Remover(matricula2.Id);
                if (aluno?.Id > 0) await new AlunoRepository(ConnectionString, DatabaseType).Remover(aluno.Id);
                if (plano?.Id > 0) await new PlanoRepository(ConnectionString, DatabaseType).Remover(plano.Id);
            }
        }

        [Fact]
        public async Task ObterAtivas_DeveRetornarApenasMatriculasVigentes()
        {
            // Arrange
            Aluno aluno = null;
            Plano plano = null;
            Matricula ativa = null;
            Matricula vencida = null;

            try
            {
                aluno = await CriarAlunoDeTesteTemporario();
                plano = await CriarPlanoDeTesteTemporario();

                // Cria uma matrícula ativa e uma vencida
                var repoAdicionarMatricula = new MatriculaRepository(ConnectionString, DatabaseType);
                ativa = await repoAdicionarMatricula.Adicionar(CriarMatriculaDeTesteInstance(aluno, plano, DateOnly.FromDateTime(DateTime.Today.AddDays(1))));
                var repoSegundaAdicionarMatricula = new MatriculaRepository(ConnectionString, DatabaseType);
                vencida = await repoSegundaAdicionarMatricula.Adicionar(CriarMatriculaDeTesteInstance(aluno, plano, DateOnly.FromDateTime(DateTime.Today.AddDays(-1))));

                // Act
                var repoObterAtivasMatricula = new MatriculaRepository(ConnectionString, DatabaseType);
                var resultado = await repoObterAtivasMatricula.ObterAtivas();

                // Assert
                Assert.NotNull(resultado);
                Assert.Contains(resultado, m => m.Id == ativa.Id);
            }
            finally
            {
                // Cleanup
                if (ativa?.Id > 0) await new MatriculaRepository(ConnectionString, DatabaseType).Remover(ativa.Id);
                if (vencida?.Id > 0) await new MatriculaRepository(ConnectionString, DatabaseType).Remover(vencida.Id);
                if (aluno?.Id > 0) await new AlunoRepository(ConnectionString, DatabaseType).Remover(aluno.Id);
                if (plano?.Id > 0) await new PlanoRepository(ConnectionString, DatabaseType).Remover(plano.Id);
            }
        }
    }
}