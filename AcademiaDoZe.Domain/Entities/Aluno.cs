//Rafael dos Santos Tavares
using AcademiaDoZe.Domain.Exceptions;
using AcademiaDoZe.Domain.Services;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{
    public class Aluno : Pessoa
    {
        private Aluno(string nome, string cpf, DateOnly dataNascimento, string telefone,
                        string email, Arquivo foto, string numero, string complemento, Logradouro endereco) : base(nome,cpf,dataNascimento,telefone,email,  foto,  numero,  complemento,  endereco)
        {
        }
        private Aluno(int id,string nome, string cpf, DateOnly dataNascimento, string telefone,
                string email, Arquivo foto, string numero, string complemento, Logradouro endereco) : base(nome, cpf, dataNascimento, telefone, email, foto, numero, complemento, endereco)
        {
            Id = id;
        }


        public static Aluno Criar(string nome, string cpf, DateOnly dataNascimento, string telefone,
                        string email, Arquivo foto, string numero, string complemento, Logradouro endereco)
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
            // if (foto == null) throw new DomainException("FOTO_OBRIGATORIO");
            if (endereco == null) throw new DomainException("LOGRADOURO_OBRIGATORIO");

            if (NormalizadoService.TextoVazioOuNulo(numero)) throw new DomainException("NUMERO_OBRIGATORIO");
            numero = NormalizadoService.LimparEspacos(numero);
            complemento = NormalizadoService.LimparEspacos(complemento);


            return new Aluno(nome, cpf, dataNascimento,  telefone,
                         email,  foto,  numero,  complemento,  endereco);
        }

        public static Aluno Criar(int id, string nome, string cpf, DateOnly dataNascimento, string telefone,
                string email, Arquivo foto, string numero, string complemento, Logradouro endereco)
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
            // if (foto == null) throw new DomainException("FOTO_OBRIGATORIO");
            if (endereco == null) throw new DomainException("LOGRADOURO_OBRIGATORIO");

            if (NormalizadoService.TextoVazioOuNulo(numero)) throw new DomainException("NUMERO_OBRIGATORIO");
            numero = NormalizadoService.LimparEspacos(numero);
            complemento = NormalizadoService.LimparEspacos(complemento);


            return new Aluno(id,nome, cpf, dataNascimento, telefone,
                         email, foto, numero, complemento, endereco);
        }
    }
}