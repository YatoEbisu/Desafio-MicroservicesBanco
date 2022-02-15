namespace ModalMais.Conta.Domain.Entities
{
    public class Pix
    {
        public Pix(TipoChave tipo, string chave = null)
        {
            Tipo = tipo;
            Chave = chave; // == TipoChave.ChaveAleatoria ? Guid.NewGuid().ToString() : chave;
        }

        public string Chave { get; set; }
        public TipoChave Tipo { get; set; }
    }

    public enum TipoChave
    {
        Email = 1,
        CPF = 2,
        Celular = 3,
        ChaveAleatoria = 4
    }
}