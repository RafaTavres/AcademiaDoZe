using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Repositories
{
    public interface IPlanoRepository : IRepository<Plano>
    {
        Task<IEnumerable<Plano>> ObterAtivos();
    }
}