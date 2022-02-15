namespace ModalMais.Conta.Domain.Entities
{
    public class Cliente
    {
        public Cliente(string cpf, string nome, string sobrenome, string celular, string email)
        {
            CPF = cpf;
            Nome = nome;
            Sobrenome = sobrenome;
            Celular = celular;
            Email = email;
        }

        public string CPF { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public string Celular { get; set; }
        public string Email { get; set; }
    }
}