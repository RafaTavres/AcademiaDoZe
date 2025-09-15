using AcademiaDoZe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaDoZe.Domain.Repositories
{
    public interface IAcessoRepository : IRepository<RegistroAcesso>
    {
        Task<IEnumerable<RegistroAcesso>> GetAcessosPorAlunoPeriodo(int? alunoId = null, DateOnly? inicio = null, DateOnly? fim = null);
        Task<IEnumerable<RegistroAcesso>> GetAcessosPorColaboradorPeriodo(int? colaboradorId = null, DateOnly? inicio = null, DateOnly? fim = null);
        Task<Dictionary<TimeOnly, int>> GetHorarioMaisProcuradoPorMes(int mes);
        Task<Dictionary<int, TimeSpan>> GetPermanenciaMediaPorMes(int mes);
        Task<IEnumerable<Aluno>> GetAlunosSemAcessoNosUltimosDias(int dias);
    }
}
