//Rafael dos Santos Tavares
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services
{
    internal class MatriculaService : IMatriculaService
    {
        private readonly Func<IMatriculaRepository> _repoFactory;
        public MatriculaService(Func<IMatriculaRepository> repoFactory)
        {
            _repoFactory = repoFactory ?? throw new ArgumentNullException(nameof(repoFactory));
        }
        public async Task<MatriculaDTO> AdicionarAsync(MatriculaDTO matriculaDto)
        {
            var matricula = matriculaDto.ToEntity();
            // Salva no repositório
            await _repoFactory().Adicionar(matricula);
            // Retorna o DTO atualizado com o ID gerado
            return matricula.ToDto();
        }

        public async Task<MatriculaDTO> AtualizarAsync(MatriculaDTO matriculaDto)
        {
            // Verifica se o colaborador existe

            var matriculaExistente = await _repoFactory().ObterPorId(matriculaDto.Id) ?? throw new KeyNotFoundException($"Matricula ID {matriculaDto.Id} não encontrado.");

            // a partir dos dados do dto e do existente, cria uma nova instância com os valores atualizados

            var alunoAtualizado = matriculaExistente.UpdateFromDto(matriculaDto);
            // Atualiza no repositório
            await _repoFactory().Atualizar(alunoAtualizado);
            return alunoAtualizado.ToDto();
        }

        public async Task<IEnumerable<MatriculaDTO>> ObterAtivasAsync(int alunoId = 0)
        {
            var matriculas = await _repoFactory().ObterAtivas();
            return [.. matriculas.Select(c => c.ToDto())];
        }

        public async Task<MatriculaDTO> ObterPorAlunoIdAsync(int alunoId)
        {
            var matricula = await _repoFactory().ObterPorId(alunoId);
            return matricula.ToDto();
        }

        public async Task<MatriculaDTO> ObterPorIdAsync(int id)
        {
            var matricula = await _repoFactory().ObterPorId(id);
            return (matricula != null) ? matricula.ToDto() : null!;
        }

        public async Task<IEnumerable<MatriculaDTO>> ObterTodasAsync()
        {
            var matriculas = await _repoFactory().ObterTodos();
            return [.. matriculas.Select(c => c.ToDto())];
        }

        public async Task<IEnumerable<MatriculaDTO>> ObterVencendoEmDiasAsync(int dias)
        {
            var matriculas = await _repoFactory().ObterVencendoEmDias(dias);
            return [.. matriculas.Select(c => c.ToDto())];
        }

        public async Task<bool> RemoverAsync(int id)
        {
            var matricula = await _repoFactory().ObterPorId(id);

            if (matricula == null)

            {
                return false;
            }
            await _repoFactory().Remover(id);

            return true;
        }
    }
}
