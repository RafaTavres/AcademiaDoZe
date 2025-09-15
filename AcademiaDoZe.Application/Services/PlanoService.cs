//Rafael dos Santos Tavares
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services
{
    public class PlanoService : IPlanoService
    {
        private readonly Func<IPlanoRepository> _repoFactory;

        public PlanoService(Func<IPlanoRepository> repoFactory)
        {
            _repoFactory = repoFactory ?? throw new ArgumentNullException(nameof(repoFactory));
        }

        public async Task<PlanoDTO> AdicionarAsync(PlanoDTO planoDto)
        {
            var plano = planoDto.ToEntity();
            await _repoFactory().Adicionar(plano);
            return plano.ToDto();
        }

        public async Task<PlanoDTO> AtualizarAsync(PlanoDTO planoDto)
        {
            // Validação: Garante que o plano a ser atualizado existe
            var planoExistente = await _repoFactory().ObterPorId(planoDto.Id)
                ?? throw new KeyNotFoundException($"Plano com ID {planoDto.Id} não encontrado.");

            // Cria uma nova instância da entidade com os dados atualizados a partir do DTO
            var planoAtualizado = planoExistente.UpdateFromDto(planoDto);

            await _repoFactory().Atualizar(planoAtualizado);
            return planoAtualizado.ToDto();
        }

        public async Task<PlanoDTO> ObterPorIdAsync(int id)
        {
            var plano = await _repoFactory().ObterPorId(id);
            return (plano != null) ? plano.ToDto() : null!;
        }

        public async Task<IEnumerable<PlanoDTO>> ObterTodosAsync()
        {
            var planos = await _repoFactory().ObterTodos();
            return planos.Select(p => p.ToDto()).ToList();
        }

        public async Task<IEnumerable<PlanoDTO>> ObterPlanosAtivosAsync()
        {
            var planosAtivos = await _repoFactory().ObterAtivos();
            return planosAtivos.Select(p => p.ToDto()).ToList();
        }

        public async Task<bool> RemoverAsync(int id)
        {
            var plano = await _repoFactory().ObterPorId(id);
            if (plano == null)
            {
                return false; // Retorna false se o plano não foi encontrado
            }

            await _repoFactory().Remover(id);
            return true;
        }
    }
}