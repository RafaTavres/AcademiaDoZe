//Rafael dos Santos Tavares
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Data;
using System.Data;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Repositories
{
    public class MatriculaRepository : BaseRepository<Matricula>, IMatriculaRepository
    {
        public MatriculaRepository(string connectionString, DatabaseType databaseType) : base(connectionString, databaseType) { }

        public override async Task<Matricula> Adicionar(Matricula entity)
        {
            try
            {
                await using var connection = await GetOpenConnectionAsync();
                string query = _databaseType == DatabaseType.SqlServer
                ? $"INSERT INTO {TableName} (aluno_id, plano_id, data_inicio, data_fim, objetivo, restricao_medica, obs_restricao, laudo_medico) "
                + "OUTPUT INSERTED.id_matricula "
                + "VALUES (@AlunoId, @PlanoId, @DataInicio, @DataFim, @Objetivo, @Restricoes, @ObservacoesRestricoes, @LaudoMedico);"
                : $"INSERT INTO {TableName} (aluno_id, plano_id, data_inicio, data_fim, objetivo, restricao_medica, obs_restricao, laudo_medico) "
                + "VALUES (@AlunoId, @PlanoId, @DataInicio, @DataFim, @Objetivo, @Restricoes, @ObservacoesRestricoes, @LaudoMedico); "
                + "SELECT LAST_INSERT_ID();";

                await using var command = DbProvider.CreateCommand(query, connection);
                command.Parameters.Add(DbProvider.CreateParameter("@AlunoId", entity.Aluno.Id, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@PlanoId", entity.Plano.Id, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@DataInicio", entity.DataInicio, DbType.Date, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@DataFim", entity.DataFim, DbType.Date, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Objetivo", entity.Objetivo, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Restricoes", (int)entity.Restricoes, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@ObservacoesRestricoes", (object)entity.ObservacoesRestricoes ?? DBNull.Value, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@LaudoMedico", (object)entity.LaudoMedico?.Conteudo ?? DBNull.Value, DbType.Binary, _databaseType));

                var id = await command.ExecuteScalarAsync();
                if (id != null && id != DBNull.Value)
                {
                    var idProperty = typeof(Entity).GetProperty("Id");
                    idProperty?.SetValue(entity, Convert.ToInt32(id));
                }
                return entity;
            }
            catch (DbException ex) { throw new InvalidOperationException($"Erro ao adicionar matrícula: {ex.Message}", ex); }
        }

        public override async Task<Matricula> Atualizar(Matricula entity)
        {
            try
            {
                await using var connection = await GetOpenConnectionAsync();
                string query = $"UPDATE {TableName} "
                + "SET aluno_id = @AlunoId, "
                + "plano_id = @PlanoId, "
                + "data_inicio = @DataInicio, "
                + "data_fim = @DataFim, "
                + "objetivo = @Objetivo, "
                + "restricao_medica = @Restricoes, "
                + "obs_restricao = @ObservacoesRestricoes, "
                + "laudo_medico = @LaudoMedico "
                + $"WHERE {IdTableName} = @Id";

                await using var command = DbProvider.CreateCommand(query, connection);
                command.Parameters.Add(DbProvider.CreateParameter("@Id", entity.Id, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@AlunoId", entity.Aluno.Id, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@PlanoId", entity.Plano.Id, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@DataInicio", entity.DataInicio, DbType.Date, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@DataFim", entity.DataFim, DbType.Date, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Objetivo", entity.Objetivo, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Restricoes", (int)entity.Restricoes, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@ObservacoesRestricoes", (object)entity.ObservacoesRestricoes ?? DBNull.Value, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@LaudoMedico", (object)entity.LaudoMedico?.Conteudo ?? DBNull.Value, DbType.Binary, _databaseType));

                int rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected == 0)
                {
                    throw new InvalidOperationException($"Nenhuma matrícula encontrada com o ID {entity.Id} para atualização.");
                }
                return entity;
            }
            catch (DbException ex) { throw new InvalidOperationException($"Erro ao atualizar matrícula com ID {entity.Id}: {ex.Message}", ex); }
        }

        #region Métodos da Interface IMatriculaRepository

        public async Task<IEnumerable<Matricula>> ObterPorAluno(int alunoId)
        {
            try
            {
                await using var connection = await GetOpenConnectionAsync();
                string query = $"SELECT * FROM {TableName} WHERE aluno_id = @AlunoId";
                await using var command = DbProvider.CreateCommand(query, connection);
                command.Parameters.Add(DbProvider.CreateParameter("@AlunoId", alunoId, DbType.Int32, _databaseType));

                var matriculas = new List<Matricula>();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    matriculas.Add(await MapAsync(reader));
                }
                return matriculas;
            }
            catch (DbException ex) { throw new InvalidOperationException($"Erro ao obter matrículas para o aluno ID {alunoId}: {ex.Message}", ex); }
        }

        public async Task<IEnumerable<Matricula>> ObterAtivas()
        {
            try
            {
                await using var connection = await GetOpenConnectionAsync();
                string query = $"SELECT * FROM {TableName} WHERE data_fim >= @DataAtual";
                await using var command = DbProvider.CreateCommand(query, connection);
                command.Parameters.Add(DbProvider.CreateParameter("@DataAtual", DateTime.Today, DbType.Date, _databaseType));

                var matriculas = new List<Matricula>();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    matriculas.Add(await MapAsync(reader));
                }
                return matriculas;
            }
            catch (DbException ex) { throw new InvalidOperationException($"Erro ao obter matrículas ativas: {ex.Message}", ex); }
        }

        public async Task<IEnumerable<Matricula>> ObterVencendoEmDias(int dias)
        {
            if (dias < 0) throw new ArgumentException("O número de dias não pode ser negativo.", nameof(dias));

            try
            {
                await using var connection = await GetOpenConnectionAsync();
                var dataAtual = DateTime.Today;
                var dataLimite = dataAtual.AddDays(dias);

                string query = $"SELECT * FROM {TableName} WHERE data_fim >= @DataAtual AND data_fim <= @DataLimite";
                await using var command = DbProvider.CreateCommand(query, connection);
                command.Parameters.Add(DbProvider.CreateParameter("@DataAtual", dataAtual, DbType.Date, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@DataLimite", dataLimite, DbType.Date, _databaseType));

                var matriculas = new List<Matricula>();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    matriculas.Add(await MapAsync(reader));
                }
                return matriculas;
            }
            catch (DbException ex) { throw new InvalidOperationException($"Erro ao obter matrículas vencendo em {dias} dias: {ex.Message}", ex); }
        }

        #endregion

        public async Task<bool> VerificarMatriculaAtiva(int alunoId)
        {
            try
            {
                await using var connection = await GetOpenConnectionAsync();
                string query = $"SELECT COUNT(1) FROM {TableName} WHERE aluno_id = @AlunoId AND data_fim >= @DataAtual";
                await using var command = DbProvider.CreateCommand(query, connection);
                command.Parameters.Add(DbProvider.CreateParameter("@AlunoId", alunoId, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@DataAtual", DateTime.Today, DbType.Date, _databaseType));

                var count = await command.ExecuteScalarAsync();
                return Convert.ToInt32(count) > 0;
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException($"Erro ao verificar matrícula ativa para o aluno ID {alunoId}: {ex.Message}", ex);
            }
        }

        protected override async Task<Matricula> MapAsync(DbDataReader reader)
        {
            try
            {
                var alunoId = Convert.ToInt32(reader["aluno_id"]);
                var planoId = Convert.ToInt32(reader["plano_id"]);

                var alunoRepository = new AlunoRepository(_connectionString, _databaseType);
                var aluno = await alunoRepository.ObterPorId(alunoId) ?? throw new InvalidOperationException($"Aluno com ID {alunoId} não encontrado.");

                var planoRepository = new PlanoRepository(_connectionString, _databaseType);
                var plano = await planoRepository.ObterPorId(planoId) ?? throw new InvalidOperationException($"Plano com ID {planoId} não encontrado.");

                var matricula = Matricula.Criar(
                    aluno: aluno,
                    plano: plano,
                    dataInicio: DateOnly.FromDateTime(Convert.ToDateTime(reader["data_inicio"])),
                    dataFim: DateOnly.FromDateTime(Convert.ToDateTime(reader["data_fim"])),
                    objetivo: reader["objetivo"].ToString()!,
                    restricoes: (EMatriculaRestricoesEnum)Convert.ToInt32(reader["restricao_medica"]),
                    observacoesRestricoes: reader["obs_restricao"]?.ToString(),
                    laudoMedico: reader["laudo_medico"] is DBNull ? null : Arquivo.Criar((byte[])reader["laudo_medico"], "pdf")
                );

                var idProperty = typeof(Entity).GetProperty("Id");
                idProperty?.SetValue(matricula, Convert.ToInt32(reader["id_matricula"]));

                return matricula;
            }
            catch (DbException ex) { throw new InvalidOperationException($"Erro ao mapear dados da matrícula: {ex.Message}", ex); }
        }
    }
}