// Rafael dos Santos Tavares
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Exceptions;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{
    public class RegistroAcesso : Entity
    {
        public ETipoPessoaEnum TipoPessoa { get; private set; }
        public Pessoa Pessoa { get; }
        public DateTime DataHoraChegada { get; }
        public DateTime? DataHoraSaida { get; private set; }
        public TimeSpan? Duracao
        {
            get
            {
                if (DataHoraSaida.HasValue)
                {
                    return DataHoraSaida.Value - DataHoraChegada;
                }
                return null;
            }
        }

        private RegistroAcesso(ETipoPessoaEnum tipo, Pessoa pessoa, DateTime dataHora) : base()
        {
            TipoPessoa = tipo;
            Pessoa = pessoa;
            DataHoraChegada = dataHora;
        }

        public void RegistrarSaida()
        {
            // Garante que a hora de saída seja sempre maior ou igual à de chegada
            var agora = DateTime.Now;
            DataHoraSaida = agora > DataHoraChegada ? agora : DataHoraChegada;
        }

        public static RegistroAcesso Criar(ETipoPessoaEnum tipo, Pessoa pessoa, DateTime dataHora)
        {
           
            if (!Enum.IsDefined(typeof(ETipoPessoaEnum), tipo)) throw new DomainException("TIPO_OBRIGATORIO");

            if (pessoa == null) throw new DomainException("PESSOA_OBRIGATORIA");

            if (dataHora.Date < DateTime.Today) throw new DomainException("DATAHORA_INVALIDA");

            if (dataHora.TimeOfDay < new TimeSpan(6, 0, 0) || dataHora.TimeOfDay > new TimeSpan(22, 0, 0))
                throw new DomainException("DATAHORA_INTERVALO");

            // validações especificas de aluno ou colaborador
            if (pessoa is Aluno aluno)
            {
                // Validar se possui matrícula ativa - depende da persistência de aluno e matrícula.
                // Na entrada, mostrar quanto tempo ainda tem de plano - depende da persistência de aluno e matrícula.
                // Na saída, mostrar o tempo que permaneceu na academia - depende da persistência de aluno e matrícula.
            }
            else if (pessoa is Colaborador colaborador)
            {
                // Validar se já não ultrapassa o limite de: 8 horas se for ctl, 6 horas se for estágio.
                // Na saída, mostrar o tempo que permaneceu na academia, devendo ser somado todos os registros do dia.
            }

            // cria e retorna o objeto
            return new RegistroAcesso(tipo, pessoa, dataHora);
        }
    }
}