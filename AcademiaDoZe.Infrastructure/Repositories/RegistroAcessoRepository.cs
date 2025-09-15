//Rafael dos Santos Tavares
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text;

namespace AcademiaDoZe.Infrastructure.Repositories
{
    public class RegistroAcessoRepository : BaseRepository<RegistroAcesso>, IAcessoRepository
    {
        public RegistroAcessoRepository(string connectionString, DatabaseType databaseType) : base(connectionString, databaseType) { }

        public override async Task<RegistroAcesso> Adicionar(RegistroAcesso entity)
        {
            try
            {
                await using var connection = await GetOpenConnectionAsync();
                string query = _databaseType == DatabaseType.SqlServer
                ? $"INSERT INTO {TableName} (pessoa_tipo, pessoa_id, data_hora, data_hora_saida) "
                + "OUTPUT INSERTED.id_registroacesso "
                + "VALUES (@TipoPessoa, @PessoaId, @DataHoraChegada, NULL);"
                : $"INSERT INTO {TableName} (pessoa_tipo, pessoa_id, data_hora, data_hora_saida) "
                + "VALUES (@TipoPessoa, @PessoaId, @DataHoraChegada, NULL); "
                + "SELECT LAST_INSERT_ID();";

                await using var command = DbProvider.CreateCommand(query, connection);
                command.Parameters.Add(DbProvider.CreateParameter("@TipoPessoa", (int)entity.TipoPessoa, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@PessoaId", entity.Pessoa.Id, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@DataHoraChegada", entity.DataHoraChegada, DbType.DateTime, _databaseType));

                var id = await command.ExecuteScalarAsync();
                if (id != null && id != DBNull.Value)
                {
                    var idProperty = typeof(Entity).GetProperty("Id");
                    idProperty?.SetValue(entity, Convert.ToInt32(id));
                }
                return entity;
            }
            catch (DbException ex) { throw new InvalidOperationException($"Erro ao adicionar registro de acesso: {ex.Message}", ex); }
        }

        public override async Task<RegistroAcesso> Atualizar(RegistroAcesso entity)
        {
            try
            {
                await using var connection = await GetOpenConnectionAsync();
                string query = $"UPDATE {TableName} "
                + "SET pessoa_tipo = @TipoPessoa, "
                + "pessoa_id = @PessoaId, "
                + "data_hora = @DataHoraChegada, "
                + "data_hora_saida = @DataHoraSaida "
                + $"WHERE {IdTableName} = @Id";

                await using var command = DbProvider.CreateCommand(query, connection);
                command.Parameters.Add(DbProvider.CreateParameter("@Id", entity.Id, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@TipoPessoa", (int)entity.TipoPessoa, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@PessoaId", entity.Pessoa.Id, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@DataHoraChegada", entity.DataHoraChegada, DbType.DateTime, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@DataHoraSaida", (object)entity.DataHoraSaida ?? DBNull.Value, DbType.DateTime, _databaseType));

                int rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected == 0)
                {
                    throw new InvalidOperationException($"Nenhum registro de acesso encontrado com o ID {entity.Id} para atualização.");
                }
                return entity;
            }
            catch (DbException ex) { throw new InvalidOperationException($"Erro ao atualizar registro de acesso com ID {entity.Id}: {ex.Message}", ex); }
        }

        public async Task<IEnumerable<RegistroAcesso>> GetAcessosPorAlunoPeriodo(int? alunoId = null, DateOnly? inicio = null, DateOnly? fim = null)
        {
            var queryBuilder = new StringBuilder($"SELECT * FROM {TableName} WHERE pessoa_tipo = @TipoPessoa");

            if (alunoId.HasValue)
                queryBuilder.Append(" AND pessoa_id = @PessoaId");
            if (inicio.HasValue)
                queryBuilder.Append(" AND CAST(data_hora AS DATE) >= @Inicio");
            if (fim.HasValue)
                queryBuilder.Append(" AND CAST(data_hora AS DATE) <= @Fim");

            await using var connection = await GetOpenConnectionAsync();
            await using var command = DbProvider.CreateCommand(queryBuilder.ToString(), connection);

            command.Parameters.Add(DbProvider.CreateParameter("@TipoPessoa", (int)ETipoPessoaEnum.Aluno, DbType.Int32, _databaseType));
            if (alunoId.HasValue)
                command.Parameters.Add(DbProvider.CreateParameter("@PessoaId", alunoId.Value, DbType.Int32, _databaseType));
            if (inicio.HasValue)
                command.Parameters.Add(DbProvider.CreateParameter("@Inicio", inicio.Value.ToDateTime(TimeOnly.MinValue), DbType.Date, _databaseType));
            if (fim.HasValue)
                command.Parameters.Add(DbProvider.CreateParameter("@Fim", fim.Value.ToDateTime(TimeOnly.MinValue), DbType.Date, _databaseType));

            var registros = new List<RegistroAcesso>();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                registros.Add(await MapAsync(reader));
            }
            return registros;
        }

        public async Task<IEnumerable<RegistroAcesso>> GetAcessosPorColaboradorPeriodo(int? colaboradorId = null, DateOnly? inicio = null, DateOnly? fim = null)
        {
            var queryBuilder = new StringBuilder($"SELECT * FROM {TableName} WHERE pessoa_tipo = @TipoPessoa");

            if (colaboradorId.HasValue)
                queryBuilder.Append(" AND pessoa_id = @PessoaId");
            if (inicio.HasValue)
                queryBuilder.Append(" AND CAST(data_hora AS DATE) >= @Inicio");
            if (fim.HasValue)
                queryBuilder.Append(" AND CAST(data_hora AS DATE) <= @Fim");

            await using var connection = await GetOpenConnectionAsync();
            await using var command = DbProvider.CreateCommand(queryBuilder.ToString(), connection);

            command.Parameters.Add(DbProvider.CreateParameter("@TipoPessoa", (int)ETipoPessoaEnum.Colaborador, DbType.Int32, _databaseType));
            if (colaboradorId.HasValue)
                command.Parameters.Add(DbProvider.CreateParameter("@PessoaId", colaboradorId.Value, DbType.Int32, _databaseType));
            if (inicio.HasValue)
                command.Parameters.Add(DbProvider.CreateParameter("@Inicio", inicio.Value.ToDateTime(TimeOnly.MinValue), DbType.Date, _databaseType));
            if (fim.HasValue)
                command.Parameters.Add(DbProvider.CreateParameter("@Fim", fim.Value.ToDateTime(TimeOnly.MinValue), DbType.Date, _databaseType));

            var registros = new List<RegistroAcesso>();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                registros.Add(await MapAsync(reader));
            }
            return registros;
        }

        public async Task<Dictionary<TimeOnly, int>> GetHorarioMaisProcuradoPorMes(int mes)
        {
            string query;
            if (_databaseType == DatabaseType.SqlServer)
            {
                query = "SELECT DATEPART(hour, data_hora) as Hora, COUNT(*) as Quantidade " +
                        $"FROM {TableName} " +
                        "WHERE DATEPART(month, data_hora) = @Mes " +
                        "GROUP BY DATEPART(hour, data_hora) " +
                        "ORDER BY Quantidade DESC";
            }
            else
            {
                query = "SELECT EXTRACT(HOUR FROM data_hora) as Hora, COUNT(*) as Quantidade " +
                        $"FROM {TableName} " +
                        "WHERE EXTRACT(MONTH FROM data_hora) = @Mes " +
                        "GROUP BY Hora " +
                        "ORDER BY Quantidade DESC";
            }

            var resultado = new Dictionary<TimeOnly, int>();
            await using var connection = await GetOpenConnectionAsync();
            await using var command = DbProvider.CreateCommand(query, connection);
            command.Parameters.Add(DbProvider.CreateParameter("@Mes", mes, DbType.Int32, _databaseType));

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var hora = Convert.ToInt32(reader["Hora"]);
                var quantidade = Convert.ToInt32(reader["Quantidade"]);
                resultado.Add(new TimeOnly(hora, 0), quantidade);
            }
            return resultado;
        }

        public async Task<Dictionary<int, TimeSpan>> GetPermanenciaMediaPorMes(int mes)
        {
            string query = "";
            if (_databaseType == DatabaseType.SqlServer)
            {
                query = "SELECT pessoa_id, AVG(CAST(DATEDIFF(second, data_hora, data_hora_saida) AS BIGINT)) as DuracaoMediaSegundos ";
            }
            else if (_databaseType == DatabaseType.MySql)
            {
                query = "SELECT pessoa_id, AVG(TIMESTAMPDIFF(SECOND, data_hora, data_hora_saida)) as DuracaoMediaSegundos ";
            }

            query += $"FROM {TableName} " +
                     "WHERE data_hora_saida IS NOT NULL AND EXTRACT(MONTH FROM data_hora) = @Mes " +
                     "GROUP BY pessoa_id";

            var resultado = new Dictionary<int, TimeSpan>();
            await using var connection = await GetOpenConnectionAsync();
            await using var command = DbProvider.CreateCommand(query, connection);
            command.Parameters.Add(DbProvider.CreateParameter("@Mes", mes, DbType.Int32, _databaseType));

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var pessoaId = Convert.ToInt32(reader["pessoa_id"]);
                var segundos = Convert.ToInt64(reader["DuracaoMediaSegundos"]);
                resultado.Add(pessoaId, TimeSpan.FromSeconds(segundos));
            }
            return resultado;
        }

        public async Task<IEnumerable<Aluno>> GetAlunosSemAcessoNosUltimosDias(int dias)
        {
            var alunoRepository = new AlunoRepository(_connectionString, _databaseType);
            var dataLimite = DateTime.Today.AddDays(-dias);

            var query = $"SELECT a.* FROM tb_aluno a WHERE NOT EXISTS (" +
                        $"SELECT 1 FROM {TableName} r " +
                        $"WHERE r.pessoa_id = a.id_aluno " +
                        $"AND r.pessoa_tipo = @TipoPessoa " +
                        $"AND r.data_hora >= @DataLimite)";

            var alunos = new List<Aluno>();
            await using var connection = await GetOpenConnectionAsync();
            await using var command = DbProvider.CreateCommand(query, connection);
            command.Parameters.Add(DbProvider.CreateParameter("@TipoPessoa", (int)ETipoPessoaEnum.Aluno, DbType.Int32, _databaseType));
            command.Parameters.Add(DbProvider.CreateParameter("@DataLimite", dataLimite, DbType.DateTime, _databaseType));

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                alunos.Add(await alunoRepository.MapPublicAsync(reader));
            }
            return alunos;
        }

        protected override async Task<RegistroAcesso> MapAsync(DbDataReader reader)
        {
            try
            {
                var tipoPessoa = (ETipoPessoaEnum)Convert.ToInt32(reader["pessoa_tipo"]);
                var pessoaId = Convert.ToInt32(reader["pessoa_id"]);
                Pessoa pessoa;

                if (tipoPessoa == ETipoPessoaEnum.Aluno)
                {
                    var alunoRepo = new AlunoRepository(_connectionString, _databaseType);
                    pessoa = await alunoRepo.ObterPorId(pessoaId) ?? throw new InvalidOperationException($"Aluno com ID {pessoaId} não encontrado.");
                }
                else
                {
                    var colaboradorRepo = new ColaboradorRepository(_connectionString, _databaseType);
                    pessoa = await colaboradorRepo.ObterPorId(pessoaId) ?? throw new InvalidOperationException($"Colaborador com ID {pessoaId} não encontrado.");
                }

                var registro = RegistroAcesso.Criar(
                    tipo: tipoPessoa,
                    pessoa: pessoa,
                    dataHora: Convert.ToDateTime(reader["data_hora"])
                );

                var idProperty = typeof(Entity).GetProperty("Id");
                idProperty?.SetValue(registro, Convert.ToInt32(reader[$"{IdTableName}"]));

                if (reader["data_hora_saida"] != DBNull.Value)
                {
                    var dataSaidaProperty = typeof(RegistroAcesso).GetProperty("DataHoraSaida", BindingFlags.Public | BindingFlags.Instance);
                    dataSaidaProperty?.SetValue(registro, Convert.ToDateTime(reader["data_hora_saida"]));
                }

                return registro;
            }
            catch (DbException ex) { throw new InvalidOperationException($"Erro ao mapear dados do registro de acesso: {ex.Message}", ex); }
        }


    }
}