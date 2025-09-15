//Rafael dos Santos Tavares
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{
    public abstract class Pessoa : Entity
    {
        public string Nome { get; }
        public string Cpf { get; }
        public DateOnly DataNascimento { get; }
        public string Telefone { get; }
        public string Email { get; }
        public Arquivo Foto { get; }
        public Logradouro Endereco { get; }
        public string Numero { get; }
        public string Complemento { get; }

        public Pessoa(string nome, string cpf, DateOnly dataNascimento, string telefone,
                        string email, Arquivo foto, string numero, string complemento, Logradouro endereco)
        {
            Nome = nome;
            Cpf = cpf;
            DataNascimento = dataNascimento;
            Telefone = telefone;
            Email = email;
            Foto = foto;
            Numero = numero;
            Complemento = complemento;
            Endereco = endereco;
        }

    }
}
