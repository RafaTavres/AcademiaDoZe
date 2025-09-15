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
    public class ColaboradorDomainTests
    {
        private Logradouro GetValidLogradouro() => Logradouro.Criar("12345678", "Rua A", "Centro", "Cidade", "SP", "Brasil");
        private Arquivo GetValidArquivo() => Arquivo.Criar(new byte[1], ".jpg");

        [Fact]
        public void CriarColaborador_ComDadosValidos_DeveCriarObjeto()
        {
            // Arrange
            var nome = "Maria Oliveira";
            var cpf = "12345678901";
            var dataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-25));
            var telefone = "11999999999";
            var email = "maria@email.com";
            var endereco = GetValidLogradouro();
            var numero = "50";
            var complemento = "Sala 2";
            var foto = GetValidArquivo();
            var dataAdmissao = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
            var tipo = ETipoColaboradorEnum.Instrutor;
            var vinculo = ETipoVinculoEnum.CLT;

            // Act
            var colaborador = Colaborador.Criar(nome, cpf, dataNascimento, telefone, email, foto, numero, complemento, endereco, dataAdmissao, tipo, vinculo);

            // Assert
            Assert.NotNull(colaborador);
        }

        [Fact]
        public void CriarColaborador_ComCpfTamanhoInvalido_DeveLancarExcecao()
        {
            // Arrange
            var nome = "Maria";
            var cpf = "123";
            var dataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-25));
            var telefone = "11999999999";
            var email = "maria@email.com";
            var endereco = GetValidLogradouro();
            var numero = "50";
            var complemento = "Sala 2";
            var foto = GetValidArquivo();
            var dataAdmissao = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

            // Act & Assert
            var ex = Assert.Throws<DomainException>(() =>
                Colaborador.Criar(
                    nome, cpf, dataNascimento, telefone, email, foto,
                    numero, complemento, endereco, dataAdmissao,
                    ETipoColaboradorEnum.Instrutor, ETipoVinculoEnum.CLT
                )
            );

            Assert.Equal("CPF_DIGITOS", ex.Message);
        }

        [Fact]
        public void CriarColaborador_DeveNormalizarDadosCorretamente()
        {
            // Arrange
            var nome = "  Maria Oliveira ";
            var cpf = "123.456.789-01";
            var dataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-30));
            var telefone = "(11) 99999-9999";
            var email = "  maria@email.com ";
            var endereco = GetValidLogradouro();
            var numero = "  50  ";
            var complemento = "  Sala 2 ";
            var foto = GetValidArquivo();
            var dataAdmissao = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
            var tipo = ETipoColaboradorEnum.Atendente;
            var vinculo = ETipoVinculoEnum.CLT;

            // Act
            var colaborador = Colaborador.Criar(nome, cpf, dataNascimento, telefone,
                email, foto, numero, complemento, endereco, dataAdmissao, tipo, vinculo);

            // Assert
            Assert.Equal("Maria Oliveira", colaborador.Nome);
            Assert.Equal("12345678901", colaborador.Cpf);
            Assert.Equal("11999999999", colaborador.Telefone);
            Assert.Equal("maria@email.com", colaborador.Email);
            Assert.Equal("50", colaborador.Numero);
            Assert.Equal("Sala 2", colaborador.Complemento);
        }

        [Fact]
        public void CriarColaborador_ComAdministradorEstagiario_DeveLancarExcecao()
        {
            // Arrange
            var nome = "Administrador";
            var cpf = "12345678901";
            var dataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-30));
            var telefone = "11999999999";
            var email = "admin@email.com";
            var endereco = GetValidLogradouro();
            var numero = "10";
            var complemento = "";
            var foto = GetValidArquivo();
            var dataAdmissao = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

            // Act & Assert
            var ex = Assert.Throws<DomainException>(() =>
                Colaborador.Criar(nome, cpf, dataNascimento, telefone, email, foto,
                    numero, complemento, endereco, dataAdmissao,
                    ETipoColaboradorEnum.Administrador, ETipoVinculoEnum.Estagio));

            Assert.Equal("ADMINISTRADOR_CLT_INVALIDO", ex.Message);
        }
    }
}
