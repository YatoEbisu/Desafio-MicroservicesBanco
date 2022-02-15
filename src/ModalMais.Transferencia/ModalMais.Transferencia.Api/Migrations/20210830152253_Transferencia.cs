using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ModalMais.Transferencia.Api.Migrations
{
    public partial class Transferencia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transferencias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoChave = table.Column<int>(type: "integer", nullable: false),
                    Chave = table.Column<string>(type: "varchar", nullable: false),
                    Valor = table.Column<double>(type: "numeric(14,2)", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false),
                    NumeroBanco = table.Column<string>(type: "varchar", nullable: false),
                    NumeroConta = table.Column<string>(type: "varchar", nullable: false),
                    Agencia = table.Column<string>(type: "varchar", nullable: false),
                    Nome = table.Column<string>(type: "varchar", nullable: false),
                    Sobrenome = table.Column<string>(type: "text", nullable: false),
                    DataTransferencia = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transferencias", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transferencias");
        }
    }
}
