//Rafael dos Santos Tavares
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Exceptions;
using AcademiaDoZe.Domain.Services;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{
    public class Matricula : Entity
    {
        public Aluno Aluno { get;  }
        public Plano Plano { get;  }
        public DateOnly DataInicio { get;  }
        public DateOnly DataFim { get;  }
        public string Objetivo { get;  }
        public EMatriculaRestricoesEnum Restricoes { get;  }
        public string ObservacoesRestricoes { get;  }
        public Arquivo LaudoMedico { get;  }

        private Matricula(Aluno aluno,Plano plano, DateOnly dataInicio, DateOnly dataFim, string objetivo, EMatriculaRestricoesEnum restricoes = 0,string observacoesRestricoes = null, Arquivo laudoMedico = null)  
        {
            Aluno = aluno;
            Plano = plano;
            DataInicio = dataInicio;
            DataFim = dataFim;
            Objetivo = objetivo;
            Restricoes = restricoes;
            ObservacoesRestricoes = observacoesRestricoes;
            LaudoMedico = laudoMedico;
        }

        private Matricula(int id, Aluno aluno, Plano plano, DateOnly dataInicio, DateOnly dataFim, string objetivo, EMatriculaRestricoesEnum restricoes = 0, string observacoesRestricoes = null, Arquivo laudoMedico = null)
        {
            Id = id;
            Aluno = aluno;
            Plano = plano;
            DataInicio = dataInicio;
            DataFim = dataFim;
            Objetivo = objetivo;
            Restricoes = restricoes;
            ObservacoesRestricoes = observacoesRestricoes;
            LaudoMedico = laudoMedico;
        }

        public static Matricula Criar(Aluno aluno, Plano plano, DateOnly dataInicio, DateOnly dataFim, string objetivo,
            EMatriculaRestricoesEnum restricoes, string observacoesRestricoes = null, Arquivo laudoMedico = null)
        {
            if (aluno == null) throw new DomainException("ALUNO_INVALIDO");
            if (aluno.DataNascimento > DateOnly.FromDateTime(DateTime.Today.AddYears(-16)) && laudoMedico == null) throw new DomainException("MENOR16_LAUDO_OBRIGATORIO");
            if (plano == null) throw new DomainException("PLANO_INVALIDO");
            if (dataInicio == default) throw new DomainException("DATA_INICIO_OBRIGATORIO");
            // dataFim
            if (NormalizadoService.TextoVazioOuNulo(objetivo)) throw new DomainException("OBJETIVO_OBRIGATORIO");
            objetivo = NormalizadoService.LimparEspacos(objetivo);
            if (restricoes != EMatriculaRestricoesEnum.None && laudoMedico == null) throw new DomainException("RESTRICOES_LAUDO_OBRIGATORIO");
            observacoesRestricoes = NormalizadoService.LimparEspacos(observacoesRestricoes);

            return new Matricula(aluno, plano, dataInicio, dataFim,  objetivo, restricoes,  observacoesRestricoes,  laudoMedico );
        }


        public static Matricula Criar(int id, Aluno aluno, Plano plano, DateOnly dataInicio, DateOnly dataFim, string objetivo,
            EMatriculaRestricoesEnum restricoes, string observacoesRestricoes = null, Arquivo laudoMedico = null)
        {
            if (aluno == null) throw new DomainException("ALUNO_INVALIDO");
            if (aluno.DataNascimento > DateOnly.FromDateTime(DateTime.Today.AddYears(-16)) && laudoMedico == null) throw new DomainException("MENOR16_LAUDO_OBRIGATORIO");
            if (plano == null) throw new DomainException("PLANO_INVALIDO");
            if (dataInicio == default) throw new DomainException("DATA_INICIO_OBRIGATORIO");
            // dataFim
            if (NormalizadoService.TextoVazioOuNulo(objetivo)) throw new DomainException("OBJETIVO_OBRIGATORIO");
            objetivo = NormalizadoService.LimparEspacos(objetivo);
            if (restricoes != EMatriculaRestricoesEnum.None && laudoMedico == null) throw new DomainException("RESTRICOES_LAUDO_OBRIGATORIO");
            observacoesRestricoes = NormalizadoService.LimparEspacos(observacoesRestricoes);

            return new Matricula(id, aluno, plano, dataInicio, dataFim, objetivo, restricoes, observacoesRestricoes, laudoMedico);
        }
    }
}