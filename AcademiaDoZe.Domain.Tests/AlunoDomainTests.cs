//Rafael dos Santos Tavares
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Exceptions;
using AcademiaDoZe.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaDoZe.Domain.Tests
{
    public class AlunoDomainTests
    {
        // Arrange Helpers
        private Logradouro GetValidLogradouro() => Logradouro.Criar("12345678", "Rua A", "Centro", "Cidade", "SP", "Brasil");
        private Arquivo GetValidArquivo() => Arquivo.Criar(new byte[1], ".jpg");

        [Fact]
        public void CriarAluno_ComDadosValidos_DeveCriarObjeto()
        {
            var nome = "João da Silva"; var cpf = "12345678901"; var dataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-20)); var telefone = "11999999999";
            var email = "joao@email.com"; var endereco = GetValidLogradouro(); var numero = "123"; var complemento = "Apto 1"; var foto = GetValidArquivo();

            var aluno = Aluno.Criar(nome, cpf, dataNascimento, telefone, email, foto, numero, complemento, endereco);

            Assert.NotNull(aluno);
        }

        [Fact]
        public void CriarAluno_ComNomeVazio_DeveLancarExcecao()
        {
            var cpf = "12345678901"; var dataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-20)); var telefone = "11999999999";
            var email = "joao@email.com"; var endereco = GetValidLogradouro(); var numero = "123"; var complemento = "Apto 1"; var foto = GetValidArquivo();

            var ex = Assert.Throws<DomainException>(() =>
                Aluno.Criar("", cpf, dataNascimento, telefone, email, foto, numero, complemento, endereco));

            Assert.Equal("NOME_OBRIGATORIO", ex.Message);
        }

        [Fact]
        public void CriarAluno_ComCpfMenorQueOnzeDigitos_DeveLancarExcecao()
        {
            var nome = "João da Silva"; var cpf = "123"; var dataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-20)); var telefone = "11999999999";
            var email = "joao@email.com"; var endereco = GetValidLogradouro(); var numero = "123"; var complemento = "Apto 1"; var foto = GetValidArquivo();

            var ex = Assert.Throws<DomainException>(() =>
                Aluno.Criar(nome, cpf, dataNascimento, telefone, email, foto, numero, complemento, endereco));

            Assert.Equal("CPF_DIGITOS", ex.Message);
        }

        [Fact]
        public void CriarAluno_DeveNormalizarDadosCorretamente()
        {
            var nome = "  João da Silva  ";
            var cpf = "123.456.789-01";
            var telefone = "(11) 99999-9999";
            var email = "  joao@email.com ";
            var dataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-20));
            var numero = " 123  ";
            var complemento = "  Apto 1  ";
            var foto = GetValidArquivo();
            var endereco = GetValidLogradouro();

            var aluno = Aluno.Criar(nome, cpf, dataNascimento, telefone, email, foto, numero, complemento, endereco);

            Assert.Equal("João da Silva", aluno.Nome);
            Assert.Equal("12345678901", aluno.Cpf);
            Assert.Equal("11999999999", aluno.Telefone);
            Assert.Equal("joao@email.com", aluno.Email);
            Assert.Equal("123", aluno.Numero);
            Assert.Equal("Apto 1", aluno.Complemento);
        }
    }
}
