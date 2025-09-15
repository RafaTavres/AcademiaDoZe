//Rafael dos Santos Tavares
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Exceptions;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{
    public class Plano : Entity
    {
        public string Tipo { get;  }
        public string Descricao { get;  }
        public decimal Preco { get;  }
        public int DuracaoEmDias { get;  }
        public bool Ativo { get;  }

        private Plano(string tipo, string descricao, decimal preco, int duracaoEmDias) : base()
        {
            Tipo = tipo;
            Descricao = descricao;
            Preco = preco;
            DuracaoEmDias = duracaoEmDias;
        }
        private Plano(int id, string tipo, string descricao, decimal preco, int duracaoEmDias) : base()
        {
            Id = id;
            Tipo = tipo;
            Descricao = descricao;
            Preco = preco;
            DuracaoEmDias = duracaoEmDias;
        }

        public static Plano Criar(string tipo, string descricao, decimal preco, int duracaoEmDias)
        {
            if (string.IsNullOrEmpty(tipo)) throw new DomainException("TIPO OBRIGATÓRIO");
            if (string.IsNullOrEmpty(descricao)) throw new DomainException("DESCRIÇÂP OBRIGATÓRIO");
            if (string.IsNullOrEmpty(preco.ToString())) throw new DomainException("PRECO OBRIGATÓRIO");
            if (string.IsNullOrEmpty(duracaoEmDias.ToString())) throw new DomainException("DURACAO EM DIAS OBRIGATÓRIO");

            return new Plano(tipo, descricao, preco, duracaoEmDias);
        }


        public static Plano Criar(int id,string tipo, string descricao, decimal preco, int duracaoEmDias)
        {
            if (string.IsNullOrEmpty(tipo)) throw new DomainException("TIPO OBRIGATÓRIO");
            if (string.IsNullOrEmpty(descricao)) throw new DomainException("DESCRIÇÂP OBRIGATÓRIO");
            if (string.IsNullOrEmpty(preco.ToString())) throw new DomainException("PRECO OBRIGATÓRIO");
            if (string.IsNullOrEmpty(duracaoEmDias.ToString())) throw new DomainException("DURACAO EM DIAS OBRIGATÓRIO");

            return new Plano(id,tipo, descricao, preco, duracaoEmDias);
        }
    }
}
