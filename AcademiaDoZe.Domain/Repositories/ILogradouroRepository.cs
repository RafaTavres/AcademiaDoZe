using AcademiaDoZe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaDoZe.Domain.Repositories
{
    public interface ILogradouroRepository : IRepository<Logradouro>
    {
        Task<Logradouro?> ObterPorCep(string cep);

        Task<IEnumerable<Logradouro>> ObterPorCidade(string cidade);
    }
}
