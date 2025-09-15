//Rafael dos Santos Tavares
using AcademiaDoZe.Domain.Enums;

namespace AcademiaDoZe.Domain.Entities
{
    public class Usuario : Entity
    {
        public string Nome { get; }
        public string Email { get; }
        public ETipoPermissaoEnum tipoAcesso { get; }

        private string _senhaHash;

        public Usuario(string nome, string email)
        {
            Nome = nome;
            Email = email;
        }

        public void DefinirSenha(string senhaEmTextoPlano)
        {
            if (string.IsNullOrWhiteSpace(senhaEmTextoPlano))
            {
                throw new ArgumentException("A senha não pode ser nula ou vazia.");
            }

            _senhaHash = BCrypt.Net.BCrypt.HashPassword(senhaEmTextoPlano);
        }
        public bool VerificarSenha(string senhaEmTextoPlano)
        {
            if (string.IsNullOrWhiteSpace(senhaEmTextoPlano) || string.IsNullOrWhiteSpace(_senhaHash))
            {
                return false;
            }

            return BCrypt.Net.BCrypt.Verify(senhaEmTextoPlano, _senhaHash);
        }

        public string ObterSenhaHash()
        {
            return _senhaHash;
        }

        public void CarregarSenhaHash(string hashDoBanco)
        {
            _senhaHash = hashDoBanco;
        }
    }
}
