using System;

namespace ModalMais.Transferencia.Api.Entities.Base
{
    public abstract class BaseEntity
    {
        public BaseEntity()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
    }
}