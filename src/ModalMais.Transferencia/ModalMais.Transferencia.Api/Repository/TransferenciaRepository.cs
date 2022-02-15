using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ModalMais.Transferencia.Api.Data;
using ModalMais.Transferencia.Api.Entities;
using ModalMais.Transferencia.Api.Interfaces;

namespace ModalMais.Transferencia.Api.Repository
{
    public class TransferenciaRepository : ITransferenciaRepository
    {
        private readonly TransferenciaPixContext _context;
        private readonly DapperContext _dapper;

        public TransferenciaRepository(TransferenciaPixContext context, DapperContext dapper)
        {
            _context = context;
            _dapper = dapper;
        }

        public async Task Add(TransferenciaPix obj)
        {
            _context.Transferencias.Add(obj);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TransferenciaPix>> Find(Expression<Func<TransferenciaPix, bool>> predicate)
        {
            return await _context.Transferencias.AsNoTracking().Where(predicate).ToListAsync();
        }

        public async Task<List<TransferenciaPix>> ObterTransferencias(string numeroConta, string agencia,
            DateTime? dataInicial = null, DateTime? dataFinal = null)
        {
            const string query =
                "SELECT * FROM \"Transferencias\" WHERE \"Agencia\" = @Agencia AND \"NumeroConta\" = @NumeroConta AND date(\"DataTransferencia\") >= date(@DataInicial) AND date(\"DataTransferencia\") <= date(@DataFinal)";

            object data;
            if (dataInicial != null && dataFinal != null)
                data = new
                {
                    NumeroConta = numeroConta,
                    Agencia = agencia,
                    DataInicial = dataInicial.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    DataFinal = dataFinal.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                };
            else
                data = new
                {
                    NumeroConta = numeroConta,
                    Agencia = agencia,
                    DataInicial = DateTime.Now.Date.AddDays(-3),
                    DataFinal = DateTime.Now.Date
                };

            return (await _dapper.Run<TransferenciaPix>(query, data)).ToList();
        }

        public async Task<decimal> ObterTotalDiarioPorConta(string conta)
        {
            //SELECT coalesce(SUM("Valor"), 0) FROM "Transferencias" WHERE "NumeroConta" = '1234'
            const string query =
                "SELECT coalesce(SUM(\"Valor\"), 0) FROM \"Transferencias\" WHERE \"NumeroConta\" = @NumeroConta AND date(\"DataTransferencia\") >= date(@Data)";
            var data = new
            {
                NumeroConta = conta,
                Data = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
            };
            return await _dapper.Single<decimal>(query, data);
        }
    }
}