//Rafael dos Santos Tavares
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaDoZe.Infrastructure.Tests
{
    public class ColaboradorInfrastructureTests : TestBase
    {
        [Fact]
        public async Task Colaborador_LogradouroPorId_CpfJaExiste_Adicionar()
        {
            // com base em logradouroID, acessar logradourorepository e obter o logradouro

            var logradouroId = 4;
            var repoLogradouroObterPorId = new LogradouroRepository(ConnectionString, DatabaseType);
            Logradouro? logradouro = await repoLogradouroObterPorId.ObterPorId(logradouroId);
            // cria um arquivo de exemplo

            Arquivo arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 },".pdf");

            var _cpf = "12345678900";
            // verifica se cpf já existe

            var repoColaboradorCpf = new ColaboradorRepository(ConnectionString, DatabaseType);

            var cpfExistente = await repoColaboradorCpf.CpfJaExiste(_cpf);
            Assert.False(cpfExistente, "CPF já existe no banco de dados.");
            var colaborador = Colaborador.Criar(
            "zé",
            _cpf,

            new DateOnly(2010, 10, 09),
            "49999999999",
            "ze@com.br",
            arquivo,
            "123",
            "complemento casa",
            logradouro!,
            

            new DateOnly(2024, 05, 04),
            ETipoColaboradorEnum.Administrador,
            ETipoVinculoEnum.CLT

            );
            // Adicionar

            var repoColaboradorAdicionar = new ColaboradorRepository(ConnectionString, DatabaseType);
            var colaboradorInserido = await repoColaboradorAdicionar.Adicionar(colaborador);
            Assert.NotNull(colaboradorInserido);
            Assert.True(colaboradorInserido.Id > 0);         
        }
        [Fact]
        public async Task Colaborador_ObterPorCpf_Atualizar()
        {
            var _cpf = "12345678900";
            Arquivo arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 }, ".pdf");
            var repoColaboradorObterPorCpf = new ColaboradorRepository(ConnectionString, DatabaseType);
            var colaboradoresExistentes = await repoColaboradorObterPorCpf.ObterPorCpf(_cpf);
            Assert.NotNull(colaboradoresExistentes);

            // criar novo colaborador com os mesmos dados, editando o que quiser
            var colaboradorExistente = colaboradoresExistentes.First();
            var colaboradorAtualizado = Colaborador.Criar(

            "zé dos testes 123",
            colaboradorExistente.Cpf,
            colaboradorExistente.DataNascimento,
            colaboradorExistente.Telefone,
            colaboradorExistente.Email,
            arquivo,
            colaboradorExistente.Numero,
            colaboradorExistente.Complemento,
            colaboradorExistente.Endereco,
            colaboradorExistente.DataAdmissao,
            colaboradorExistente.Tipo,
            colaboradorExistente.Vinculo
            );
            // Usar reflexão para definir o ID

            var idProperty = typeof(Entity).GetProperty("Id");

            idProperty?.SetValue(colaboradorAtualizado, colaboradorExistente.Id);
            // Teste de Atualização

            var repoColaboradorAtualizar = new ColaboradorRepository(ConnectionString, DatabaseType);
            var resultadoAtualizacao = await repoColaboradorAtualizar.Atualizar(colaboradorAtualizado);
            Assert.NotNull(resultadoAtualizacao);

            Assert.Equal("zé dos testes 123", resultadoAtualizacao.Nome);

        }
        [Fact]
        public async Task Colaborador_ObterPorCpf_Remover_ObterPorId()
        {
            var _cpf = "12345678900";
            var repoColaboradorObterPorCpf = new ColaboradorRepository(ConnectionString, DatabaseType);
            var colaboradoresExistentes = await repoColaboradorObterPorCpf.ObterPorCpf(_cpf);
            Assert.NotNull(colaboradoresExistentes);

            // Remover
            var colaboradorExistente = colaboradoresExistentes.First();
            var repoColaboradorRemover = new ColaboradorRepository(ConnectionString, DatabaseType);
            var resultadoRemover = await repoColaboradorRemover.Remover(colaboradorExistente.Id);
            Assert.True(resultadoRemover);

            var repoColaboradorObterPorId = new ColaboradorRepository(ConnectionString, DatabaseType);
            var resultadoRemovido = await repoColaboradorObterPorId.ObterPorId(colaboradorExistente.Id);
            Assert.Null(resultadoRemovido);
        }
        [Fact]
        public async Task Colaborador_ObterTodos()
        {
            var repoColaboradorRepository = new ColaboradorRepository(ConnectionString, DatabaseType);
            var resultado = await repoColaboradorRepository.ObterTodos();
            Assert.NotNull(resultado);
        }
    }
}
