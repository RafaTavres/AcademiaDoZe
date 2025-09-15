using AcademiaDoZe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaDoZe.Domain.Repositories
{
    public interface IMatriculaRepository : IRepository<Matricula>
    {
        Task<IEnumerable<Matricula>> ObterPorAluno(int alunoId);

        Task<IEnumerable<Matricula>> ObterAtivas();
        Task<IEnumerable<Matricula>> ObterVencendoEmDias(int dias);
    }
}
