namespace ModalMais.Conta.Service.Dtos
{
    public class AtualizaClienteRequest
    {
        public string CPF { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public string Celular { get; set; }
        public string Email { get; set; }
    }
}