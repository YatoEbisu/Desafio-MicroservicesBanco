using Microsoft.EntityFrameworkCore;
using ModalMais.Transferencia.Api.Entities;

namespace ModalMais.Transferencia.Api.Data
{
    public class TransferenciaPixContext : DbContext
    {
        public TransferenciaPixContext(DbContextOptions<TransferenciaPixContext> options) : base(options)
        {
        }

        public DbSet<TransferenciaPix> Transferencias { get; set; }
    }
}