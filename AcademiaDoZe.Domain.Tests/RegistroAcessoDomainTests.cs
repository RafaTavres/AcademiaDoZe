//Rafael dos Santos Tavares
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Exceptions;
using AcademiaDoZe.Domain.ValueObjects;
using System;
using Xunit;

namespace AcademiaDoZe.Domain.Tests
{
    public class RegistroAcessoDomainTests
    {
        private Aluno GetValidAluno()
        {
            var endereco = Logradouro.Criar("12345678", "Rua A", "Centro", "Cidade", "SP", "Brasil");
            var foto = Arquivo.Criar(new byte[1], ".jpg");
            return Aluno.Criar("Aluno Teste", "12345678901", DateOnly.FromDateTime(DateTime.Today.AddYears(-20)),
                "11999999999", "email@test.com", foto, "123", "Apto 1", endereco);
        }

        private Colaborador GetValidColaborador()
        {
            var endereco = Logradouro.Criar("12345678", "Rua B", "Centro", "Cidade", "SP", "Brasil");
            return Colaborador.Criar(
                "Colaborador Teste",
                "98765432100",
                DateOnly.FromDateTime(DateTime.Today.AddYears(-30)),
                "11988888888",
                "colaborador@email.com",
                Arquivo.Criar(new byte[1], ".jpg"),
                "456",
                "Sala 1",
                endereco,
                DateOnly.FromDateTime(DateTime.Today.AddYears(-1)),
                ETipoColaboradorEnum.Instrutor,
                ETipoVinculoEnum.CLT
            );
        }


        [Fact]
        public void CriarRegistroAcesso_ComDadosValidos_DeveCriarObjeto()
        {
            var pessoa = GetValidAluno();
            var dataHora = DateTime.Today.AddHours(8);

            var registro = RegistroAcesso.Criar(ETipoPessoaEnum.Aluno, pessoa, dataHora);

            Assert.NotNull(registro);
            Assert.Equal(ETipoPessoaEnum.Aluno, registro.TipoPessoa);
            Assert.Equal(pessoa, registro.Pessoa);
            Assert.Equal(dataHora, registro.DataHoraChegada);
            Assert.Null(registro.DataHoraSaida);
        }

        [Fact]
        public void CriarRegistroAcesso_TipoPessoaInvalido_DeveLancarExcecao()
        {
            var pessoa = GetValidAluno();
            var dataHora = DateTime.Today.AddHours(8);
            var tipoInvalido = (ETipoPessoaEnum)999;

            var ex = Assert.Throws<DomainException>(() => RegistroAcesso.Criar(tipoInvalido, pessoa, dataHora));
            Assert.Equal("TIPO_OBRIGATORIO", ex.Message);
        }

        [Fact]
        public void CriarRegistroAcesso_PessoaNula_DeveLancarExcecao()
        {
            var dataHora = DateTime.Today.AddHours(8);
            var ex = Assert.Throws<DomainException>(() => RegistroAcesso.Criar(ETipoPessoaEnum.Aluno, null, dataHora));
            Assert.Equal("PESSOA_OBRIGATORIA", ex.Message);
        }

        [Fact]
        public void CriarRegistroAcesso_DataHoraAnteriorHoje_DeveLancarExcecao()
        {
            var pessoa = GetValidAluno();
            var dataHora = DateTime.Today.AddDays(-1).AddHours(10); // Ontem às 10:00

            var ex = Assert.Throws<DomainException>(() => RegistroAcesso.Criar(ETipoPessoaEnum.Aluno, pessoa, dataHora));
            Assert.Equal("DATAHORA_INVALIDA", ex.Message);
        }

        [Theory]
        [InlineData(5, 59)]  // Antes das 6:00
        [InlineData(22, 1)]  // Depois das 22:00
        public void CriarRegistroAcesso_HorarioForaDoIntervalo_DeveLancarExcecao(int hour, int minute)
        {
            var pessoa = GetValidAluno();
            var dataHora = DateTime.Today.AddHours(hour).AddMinutes(minute);

            var ex = Assert.Throws<DomainException>(() => RegistroAcesso.Criar(ETipoPessoaEnum.Aluno, pessoa, dataHora));
            Assert.Equal("DATAHORA_INTERVALO", ex.Message);
        }

        [Fact]
        public void RegistrarSaida_DeveAtualizarDataHoraSaida()
        {
            var pessoa = GetValidAluno();
            var dataHora = DateTime.Today.AddHours(8);
            var registro = RegistroAcesso.Criar(ETipoPessoaEnum.Aluno, pessoa, dataHora);

            registro.RegistrarSaida();

            Assert.NotNull(registro.DataHoraSaida);
            Assert.True(registro.DataHoraSaida >= registro.DataHoraChegada);
        }

        [Fact]
        public void CriarRegistroAcesso_ComColaboradorValido_DeveCriarObjeto()
        {
            var colaborador = GetValidColaborador();
            var dataHora = DateTime.Today.AddHours(9);

            var registro = RegistroAcesso.Criar(ETipoPessoaEnum.Colaborador, colaborador, dataHora);

            Assert.NotNull(registro);
            Assert.Equal(ETipoPessoaEnum.Colaborador, registro.TipoPessoa);
            Assert.Equal(colaborador, registro.Pessoa);
            Assert.Equal(dataHora, registro.DataHoraChegada);
            Assert.Null(registro.DataHoraSaida);
        }

        [Fact]
        public void CriarRegistroAcesso_ColaboradorNulo_DeveLancarExcecao()
        {
            var dataHora = DateTime.Today.AddHours(10);
            var ex = Assert.Throws<DomainException>(() => RegistroAcesso.Criar(ETipoPessoaEnum.Colaborador, null, dataHora));
            Assert.Equal("PESSOA_OBRIGATORIA", ex.Message);
        }

        [Fact]
        public void CriarRegistroAcesso_ColaboradorDataHoraForaIntervalo_DeveLancarExcecao()
        {
            var colaborador = GetValidColaborador();
            var dataHora = DateTime.Today.AddHours(23); // fora do intervalo 6h-22h

            var ex = Assert.Throws<DomainException>(() => RegistroAcesso.Criar(ETipoPessoaEnum.Colaborador, colaborador, dataHora));
            Assert.Equal("DATAHORA_INTERVALO", ex.Message);
        }

        [Fact]
        public void RegistrarSaida_Colaborador_DeveAtualizarDataHoraSaida()
        {
            var colaborador = GetValidColaborador();
            var dataHora = DateTime.Today.AddHours(8);
            var registro = RegistroAcesso.Criar(ETipoPessoaEnum.Colaborador, colaborador, dataHora);

            registro.RegistrarSaida();

            Assert.NotNull(registro.DataHoraSaida);
            Assert.True(registro.DataHoraSaida >= registro.DataHoraChegada);
        }
    }
}
