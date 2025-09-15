//Rafael dos Santos Tavares
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Exceptions;
using AcademiaDoZe.Domain.Services;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{
    public class Colaborador : Pessoa
    {
        public DateOnly DataAdmissao { get; }
        public ETipoColaboradorEnum Tipo { get; }
        public ETipoVinculoEnum Vinculo { get; }


        private Colaborador(string nome, string cpf, DateOnly dataNascimento, string telefone,
                        string email, Arquivo foto,string numero, string complemento, Logradouro endereco,
                        DateOnly dataAdmissao, ETipoColaboradorEnum tipo, ETipoVinculoEnum vinculo) : base(nome,cpf,dataNascimento, telefone, email, foto, numero, complemento, endereco)
        {
            DataAdmissao = dataAdmissao;
            Tipo = tipo;
            Vinculo = vinculo;
        }

        private Colaborador(int id, string nome, string cpf, DateOnly dataNascimento, string telefone,
                       string email, Arquivo foto, string numero, string complemento, Logradouro endereco,
                       DateOnly dataAdmissao, ETipoColaboradorEnum tipo, ETipoVinculoEnum vinculo) : base(nome, cpf, dataNascimento, telefone, email, foto, numero, complemento, endereco)
        {
            Id = id;
            DataAdmissao = dataAdmissao;
            Tipo = tipo;
            Vinculo = vinculo;
        }

        public static Colaborador Criar(string nome, string cpf, DateOnly dataNascimento, string telefone,
                       string email, Arquivo foto, string numero, string complemento, Logradouro endereco, DateOnly dataAdmissao, ETipoColaboradorEnum tipo, ETipoVinculoEnum vinculo)
        {
            if (NormalizadoService.TextoVazioOuNulo(nome)) throw new DomainException("NOME_OBRIGATORIO");
            nome = NormalizadoService.LimparEspacos(nome);
            if (NormalizadoService.TextoVazioOuNulo(cpf)) throw new DomainException("CPF_OBRIGATORIO");
            cpf = NormalizadoService.LimparEDigitos(cpf);
            if (cpf.Length != 11) throw new DomainException("CPF_DIGITOS");
            if (dataNascimento == default) throw new DomainException("DATA_NASCIMENTO_OBRIGATORIO");
            if (dataNascimento > DateOnly.FromDateTime(DateTime.Today.AddYears(-12))) throw new DomainException("DATA_NASCIMENTO_MINIMA_INVALIDA");
            if (NormalizadoService.TextoVazioOuNulo(telefone)) throw new DomainException("TELEFONE_OBRIGATORIO");
            telefone = NormalizadoService.LimparEDigitos(telefone);
            if (telefone.Length != 11) throw new DomainException("TELEFONE_DIGITOS");
            email = NormalizadoService.LimparEspacos(email);
            if (NormalizadoService.ValidarFormatoEmail(email)) throw new DomainException("EMAIL_FORMATO");
            if (endereco == null) throw new DomainException("LOGRADOURO_OBRIGATORIO");
            if (NormalizadoService.TextoVazioOuNulo(numero)) throw new DomainException("NUMERO_OBRIGATORIO");
            numero = NormalizadoService.LimparEspacos(numero);
            complemento = NormalizadoService.LimparEspacos(complemento);
            if (dataAdmissao == default) throw new DomainException("DATA_ADMISSAO_OBRIGATORIO");
            if (dataAdmissao > DateOnly.FromDateTime(DateTime.Today)) throw new DomainException("DATA_ADMISSAO_MAIOR_ATUAL");
            if (!Enum.IsDefined(tipo)) throw new DomainException("TIPO_COLABORADOR_INVALIDO");
            if (!Enum.IsDefined(vinculo)) throw new DomainException("VINCULO_COLABORADOR_INVALIDO");
            if (tipo == ETipoColaboradorEnum.Administrador && vinculo == ETipoVinculoEnum.Estagio) throw new DomainException("ADMINISTRADOR_CLT_INVALIDO");

            return new Colaborador(nome, cpf, dataNascimento, telefone,
                         email, foto, numero, complemento, endereco, dataAdmissao, tipo, vinculo);
        }

        public static Colaborador Criar(int id,string nome, string cpf, DateOnly dataNascimento, string telefone,
                       string email, Arquivo foto, string numero, string complemento, Logradouro endereco, DateOnly dataAdmissao, ETipoColaboradorEnum tipo, ETipoVinculoEnum vinculo)
        {
            if (NormalizadoService.TextoVazioOuNulo(nome)) throw new DomainException("NOME_OBRIGATORIO");
            nome = NormalizadoService.LimparEspacos(nome);
            if (NormalizadoService.TextoVazioOuNulo(cpf)) throw new DomainException("CPF_OBRIGATORIO");
            cpf = NormalizadoService.LimparEDigitos(cpf);
            if (cpf.Length != 11) throw new DomainException("CPF_DIGITOS");
            if (dataNascimento == default) throw new DomainException("DATA_NASCIMENTO_OBRIGATORIO");
            if (dataNascimento > DateOnly.FromDateTime(DateTime.Today.AddYears(-12))) throw new DomainException("DATA_NASCIMENTO_MINIMA_INVALIDA");
            if (NormalizadoService.TextoVazioOuNulo(telefone)) throw new DomainException("TELEFONE_OBRIGATORIO");
            telefone = NormalizadoService.LimparEDigitos(telefone);
            if (telefone.Length != 11) throw new DomainException("TELEFONE_DIGITOS");
            email = NormalizadoService.LimparEspacos(email);
            if (NormalizadoService.ValidarFormatoEmail(email)) throw new DomainException("EMAIL_FORMATO");
            if (endereco == null) throw new DomainException("LOGRADOURO_OBRIGATORIO");
            if (NormalizadoService.TextoVazioOuNulo(numero)) throw new DomainException("NUMERO_OBRIGATORIO");
            numero = NormalizadoService.LimparEspacos(numero);
            complemento = NormalizadoService.LimparEspacos(complemento);
            if (dataAdmissao == default) throw new DomainException("DATA_ADMISSAO_OBRIGATORIO");
            if (dataAdmissao > DateOnly.FromDateTime(DateTime.Today)) throw new DomainException("DATA_ADMISSAO_MAIOR_ATUAL");
            if (!Enum.IsDefined(tipo)) throw new DomainException("TIPO_COLABORADOR_INVALIDO");
            if (!Enum.IsDefined(vinculo)) throw new DomainException("VINCULO_COLABORADOR_INVALIDO");
            if (tipo == ETipoColaboradorEnum.Administrador && vinculo == ETipoVinculoEnum.Estagio) throw new DomainException("ADMINISTRADOR_CLT_INVALIDO");

            return new Colaborador(id,nome, cpf, dataNascimento, telefone,
                         email, foto, numero, complemento, endereco, dataAdmissao, tipo, vinculo);
        }
    }
}
  