//Rafael dos Santos Tavares
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace AcademiaDoZe.Infrastructure.Repositories
{
    public class PlanoRepository : BaseRepository<Plano>, IPlanoRepository
    {
        public PlanoRepository(string connectionString, DatabaseType databaseType) : base(connectionString, databaseType) { }

        public override async Task<Plano> Adicionar(Plano entity)
        {
            try
            {
                await using var connection = await GetOpenConnectionAsync();
                string query = _databaseType == DatabaseType.SqlServer
                ? $"INSERT INTO {TableName} (tipo, descricao, preco, duracao_em_dias, ativo) "
                + "OUTPUT INSERTED.id_plano "
                + "VALUES (@Tipo, @Descricao, @Preco, @DuracaoEmDias, @Ativo);"
                : $"INSERT INTO {TableName} (tipo, descricao, preco, duracao_em_dias, ativo) "
                + "VALUES (@Tipo, @Descricao, @Preco, @DuracaoEmDias, @Ativo); "
                + "SELECT LAST_INSERT_ID();";

                await using var command = DbProvider.CreateCommand(query, connection);
                command.Parameters.Add(DbProvider.CreateParameter("@Tipo", entity.Tipo, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Descricao", entity.Descricao, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Preco", entity.Preco, DbType.Decimal, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@DuracaoEmDias", entity.DuracaoEmDias, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Ativo", true, DbType.Boolean, _databaseType));

                var id = await command.ExecuteScalarAsync();
                if (id != null && id != DBNull.Value)
                {
                    var idProperty = typeof(Entity).GetProperty("Id");
                    idProperty?.SetValue(entity, Convert.ToInt32(id));

                    // LINHA CORRIGIDA: A linha abaixo foi removida.
                    // Ela era redundante e causava o erro, pois o INSERT na query já define "ativo" como true.
                }
                return entity;
            }
            catch (DbException ex) { throw new InvalidOperationException($"Erro ao adicionar plano: {ex.Message}", ex); }
        }

        public override async Task<Plano> Atualizar(Plano entity)
        {
            try
            {
                await using var connection = await GetOpenConnectionAsync();
                string query = $"UPDATE {TableName} "
                + "SET tipo = @Tipo, "
                + "descricao = @Descricao, "
                + "preco = @Preco, "
                + "duracao_em_dias = @DuracaoEmDias, "
                + "ativo = @Ativo "
                + $"WHERE {IdTableName} = @Id";

                await using var command = DbProvider.CreateCommand(query, connection);
                command.Parameters.Add(DbProvider.CreateParameter("@Id", entity.Id, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Tipo", entity.Tipo, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Descricao", entity.Descricao, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Preco", entity.Preco, DbType.Decimal, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@DuracaoEmDias", entity.DuracaoEmDias, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Ativo", entity.Ativo, DbType.Boolean, _databaseType));

                int rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected == 0)
                {
                    throw new InvalidOperationException($"Nenhum plano encontrado com o ID {entity.Id} para atualização.");
                }
                return entity;
            }
            catch (DbException ex) { throw new InvalidOperationException($"Erro ao atualizar plano com ID {entity.Id}: {ex.Message}", ex); }
        }

        public async Task<IEnumerable<Plano>> ObterAtivos()
        {
            try
            {
                await using var connection = await GetOpenConnectionAsync();
                string query = $"SELECT * FROM {TableName} WHERE ativo = @Ativo";
                await using var command = DbProvider.CreateCommand(query, connection);
                command.Parameters.Add(DbProvider.CreateParameter("@Ativo", true, DbType.Boolean, _databaseType));

                var planos = new List<Plano>();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    planos.Add(await MapAsync(reader));
                }
                return planos;
            }
            catch (DbException ex) { throw new InvalidOperationException($"Erro ao obter planos ativos: {ex.Message}", ex); }
        }

        protected override async Task<Plano> MapAsync(DbDataReader reader)
        {
            try
            {
                var plano = Plano.Criar(
                    tipo: reader["tipo"].ToString()!,
                    descricao: reader["descricao"].ToString()!,
                    preco: Convert.ToDecimal(reader["preco"]),
                    duracaoEmDias: Convert.ToInt32(reader["duracao_em_dias"])
                );

                var idProperty = typeof(Entity).GetProperty("Id");
                idProperty?.SetValue(plano, Convert.ToInt32(reader["id_plano"]));

                var ativoField = typeof(Plano).GetField("<Ativo>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
                if (ativoField != null)
                {
                    ativoField.SetValue(plano, Convert.ToBoolean(reader["ativo"]));
                }

                return plano;
            }
            catch (DbException ex) { throw new InvalidOperationException($"Erro ao mapear dados do plano: {ex.Message}", ex); }
        }
    }
}