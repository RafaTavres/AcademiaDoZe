//Rafael dos Santos Tavares
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Exceptions;

namespace AcademiaDoZe.Domain.Tests
{
    public class LogradouroDomainTests
    {
        [Fact]
        public void CriarLogradouro_Valido_NaoDeveLancarExcecao()
        {
            var logradouro = Logradouro.Criar("12345670", "Rua A", "Centro", "Cidade", "SP", "Brasil");
            Assert.NotNull(logradouro); // validando criação, não deve lançar exceção e não deve ser nulo
        }
        [Fact]
        public void CriarLogradouro_Invalido_DeveLancarExcecao()
        {
            // validando a criação de logradouro com CEP inválido, deve lançar exceção
            Assert.Throws<DomainException>(() => Logradouro.Criar("123", "Rua A", "Centro", "Cidade", "SP", "Brasil"));
        }
        [Fact]
        public void CriarLogradouro_Valido_VerificarNormalizado()
        {
            var logradouro = Logradouro.Criar("12.3456-78 ", " Rua A ", " Centro ", " Cidade ", "S P", "Brasil ");
            Assert.Equal("12345678", logradouro.Cep); // validando normalização
            Assert.Equal("Rua A", logradouro.Nome);
            Assert.Equal("Centro", logradouro.Bairro);
            Assert.Equal("Cidade", logradouro.Cidade);
            Assert.Equal("SP", logradouro.Estado);
            Assert.Equal("Brasil", logradouro.Pais);

        }
        [Fact]
        public void CriarLogradouro_Invalido_VerificarMessageExcecao()
        {
            var exception = Assert.Throws<DomainException>(() => Logradouro.Criar("12345670", "", "Centro", "Cidade", "SP", "Brasil"));
            Assert.Equal("NOME_OBRIGATORIO", exception.Message); // validando a mensagem de exceção
        }

        [Fact]
        public void CriarLogradouro_ComCepVazio_DeveLancarExcecao()
        {
            var ex = Assert.Throws<DomainException>(() =>
                Logradouro.Criar("", "Rua A", "Centro", "Cidade", "SP", "Brasil"));

            Assert.Equal("CEP_OBRIGATORIO", ex.Message);
        }

        [Fact]
        public void CriarLogradouro_ComBairroVazio_DeveLancarExcecao()
        {
            var ex = Assert.Throws<DomainException>(() =>
                Logradouro.Criar("12345678", "Rua A", "", "Cidade", "SP", "Brasil"));

            Assert.Equal("BAIRRO_OBRIGATORIO", ex.Message);
        }

        [Fact]
        public void CriarLogradouro_ComCidadeVazia_DeveLancarExcecao()
        {
            var ex = Assert.Throws<DomainException>(() =>
                Logradouro.Criar("12345678", "Rua A", "Centro", "", "SP", "Brasil"));

            Assert.Equal("CIDADE_OBRIGATORIO", ex.Message);
        }

        [Fact]
        public void CriarLogradouro_ComEstadoVazio_DeveLancarExcecao()
        {
            var ex = Assert.Throws<DomainException>(() =>
                Logradouro.Criar("12345678", "Rua A", "Centro", "Cidade", "", "Brasil"));

            Assert.Equal("ESTADO_OBRIGATORIO", ex.Message);
        }

        [Fact]
        public void CriarLogradouro_ComPaisVazio_DeveLancarExcecao()
        {
            var ex = Assert.Throws<DomainException>(() =>
                Logradouro.Criar("12345678", "Rua A", "Centro", "Cidade", "SP", ""));

            Assert.Equal("PAIS_OBRIGATORIO", ex.Message);
        }
    }
}
