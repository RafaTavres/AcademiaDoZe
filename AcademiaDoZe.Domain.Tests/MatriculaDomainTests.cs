//Rafael dos Santos Tavares
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Exceptions;
using AcademiaDoZe.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaDoZe.Domain.Tests
{
    public class MatriculaDomainTests
    {

        private Aluno GetValidAluno()
        {
            var endereco = Logradouro.Criar("12345678", "Rua A", "Centro", "Cidade", "SP", "Brasil");
            var foto = Arquivo.Criar(new byte[1], ".jpg");
            return Aluno.Criar("Aluno Teste", "12345678901", DateOnly.FromDateTime(DateTime.Today.AddYears(-20)),
                "11999999999", "email@test.com", foto, "123", "Apto 1", endereco);
        }

        private Plano GetValidPlano()
        {
            // Supondo que você tenha um método Criar válido para Plano
            // Ajuste conforme sua implementação real
            return Plano.Criar("Plano Básico","plano simples" ,100, 60);
        }

        private Arquivo GetValidLaudo()
        {
            return Arquivo.Criar(new byte[1], ".pdf");
        }

        private Aluno GetValidAlunoComIdade(DateTime nascimento)
        {
            var endereco = Logradouro.Criar("12345678", "Rua A", "Centro", "Cidade", "SP", "Brasil");
            var foto = Arquivo.Criar(new byte[1], ".jpg");
            return Aluno.Criar("Aluno Teste", "12345678901", DateOnly.FromDateTime(nascimento),
                "11999999999", "email@test.com", foto, "123", "Apto 1", endereco);
        }

        [Fact]
        public void CriarMatricula_ComDadosValidos_DeveCriarObjeto()
        {
            var aluno = GetValidAluno();
            var plano = GetValidPlano();
            var dataInicio = DateOnly.FromDateTime(DateTime.Today);
            var dataFim = DateOnly.FromDateTime(DateTime.Today.AddMonths(1));
            var objetivo = " Emagrecer ";
            var restricoes = EMatriculaRestricoesEnum.None;

            var matricula = Matricula.Criar(aluno, plano, dataInicio, dataFim, objetivo, restricoes);

            Assert.NotNull(matricula);
            Assert.Equal("Emagrecer", matricula.Objetivo); // verificando normalização
        }

        [Fact]
        public void CriarMatricula_AlunoNulo_DeveLancarExcecao()
        {
            var plano = GetValidPlano();
            var dataInicio = DateOnly.FromDateTime(DateTime.Today);
            var dataFim = DateOnly.FromDateTime(DateTime.Today.AddMonths(1));
            var objetivo = "Objetivo";
            var restricoes = EMatriculaRestricoesEnum.None;

            var ex = Assert.Throws<DomainException>(() =>
                Matricula.Criar(null, plano, dataInicio, dataFim, objetivo, restricoes));

            Assert.Equal("ALUNO_INVALIDO", ex.Message);
        }

        [Fact]
        public void CriarMatricula_PlanoNulo_DeveLancarExcecao()
        {
            var aluno = GetValidAluno();
            var dataInicio = DateOnly.FromDateTime(DateTime.Today);
            var dataFim = DateOnly.FromDateTime(DateTime.Today.AddMonths(1));
            var objetivo = "Objetivo";
            var restricoes = EMatriculaRestricoesEnum.None;

            var ex = Assert.Throws<DomainException>(() =>
                Matricula.Criar(aluno, null, dataInicio, dataFim, objetivo, restricoes));

            Assert.Equal("PLANO_INVALIDO", ex.Message);
        }

        [Fact]
        public void CriarMatricula_Menor16SemLaudo_DeveLancarExcecao()
        {
            var aluno = GetValidAlunoComIdade(DateTime.Today.AddYears(-15)); // 15 anos
            var plano = GetValidPlano();
            var dataInicio = DateOnly.FromDateTime(DateTime.Today);
            var dataFim = DateOnly.FromDateTime(DateTime.Today.AddMonths(1));
            var objetivo = "Objetivo";
            var restricoes = EMatriculaRestricoesEnum.None;

            var ex = Assert.Throws<DomainException>(() =>
                Matricula.Criar(aluno, plano, dataInicio, dataFim, objetivo, restricoes));

            Assert.Equal("MENOR16_LAUDO_OBRIGATORIO", ex.Message);
        }

        [Fact]
        public void CriarMatricula_ComRestricoesSemLaudo_DeveLancarExcecao()
        {
            var aluno = GetValidAluno();
            var plano = GetValidPlano();
            var dataInicio = DateOnly.FromDateTime(DateTime.Today);
            var dataFim = DateOnly.FromDateTime(DateTime.Today.AddMonths(1));
            var objetivo = "Objetivo";
            var restricoes = EMatriculaRestricoesEnum.Labirintite;

            var ex = Assert.Throws<DomainException>(() =>
                Matricula.Criar(aluno, plano, dataInicio, dataFim, objetivo, restricoes));

            Assert.Equal("RESTRICOES_LAUDO_OBRIGATORIO", ex.Message);
        }

        [Fact]
        public void CriarMatricula_NormalizarObjetivoEObservacoes()
        {
            var aluno = GetValidAluno();
            var plano = GetValidPlano();
            var dataInicio = DateOnly.FromDateTime(DateTime.Today);
            var dataFim = DateOnly.FromDateTime(DateTime.Today.AddMonths(1));
            var objetivo = "  Perder peso  ";
            var restricoes = EMatriculaRestricoesEnum.None;
            var observacoes = "  Observação de teste  ";
            var laudo = GetValidLaudo();

            var matricula = Matricula.Criar(aluno, plano, dataInicio, dataFim, objetivo, restricoes, observacoes, laudo);

            Assert.Equal("Perder peso", matricula.Objetivo);
            Assert.Equal("Observação de teste", matricula.ObservacoesRestricoes);
        }
    }
}
