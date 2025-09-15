using AcademiaDoZe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaDoZe.Domain.Repositories
{
    public interface IColaboradorRepository : IRepository<Colaborador>
    {
        Task<Colaborador?> ObterPorCpf(string cpf);

        Task<bool> CpfJaExiste(string cpf, int? id = null);
    }
}
