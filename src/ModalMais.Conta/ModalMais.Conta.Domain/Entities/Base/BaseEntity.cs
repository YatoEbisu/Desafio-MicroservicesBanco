using System;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ModalMais.Conta.Domain.Entities
{
    public class BaseEntity
    {
        [BsonId] public ObjectId Id { get; set; }

        public static string GetTableName<T>()
        {
            try
            {
                return ((TableAttribute)typeof(T).GetCustomAttributes(typeof(TableAttribute), true)?[0]).Name;
            }
            catch (Exception ex)
            {
                throw new("Não foi encontrado o TableAttribute, verifique sua entidade.", ex);
            }
        }
    }
}