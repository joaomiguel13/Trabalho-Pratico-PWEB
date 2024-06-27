using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trabalho_Pratico.Data.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ContaAtiva",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataNascimento",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "LocadorId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NIF",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PrimeiroNome",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UltimoNome",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "CategoriasHabitacao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasHabitacao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locador",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstadoSubscricao = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locador", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Localizacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localizacoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Habitacao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Foto = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnoConstrucao = table.Column<int>(type: "int", nullable: false),
                    Morada = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AreaTotal = table.Column<int>(type: "int", nullable: false),
                    Disponivel = table.Column<bool>(type: "bit", nullable: false),
                    PrecoPorNoite = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    LocalizacaoId = table.Column<int>(type: "int", nullable: false),
                    LocadorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Habitacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Habitacao_CategoriasHabitacao_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "CategoriasHabitacao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Habitacao_Locador_LocadorId",
                        column: x => x.LocadorId,
                        principalTable: "Locador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Habitacao_Localizacoes_LocalizacaoId",
                        column: x => x.LocalizacaoId,
                        principalTable: "Localizacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Arrendamento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataEntrada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataSaida = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Confirmado = table.Column<bool>(type: "bit", nullable: false),
                    HabitacaoId = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReservaEstadoHabitacaoEntradaId = table.Column<int>(type: "int", nullable: true),
                    ArrendamentoEstadoHabitacaoSaidaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arrendamento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Arrendamento_AspNetUsers_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Arrendamento_Habitacao_HabitacaoId",
                        column: x => x.HabitacaoId,
                        principalTable: "Habitacao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArrendamentoEstadoHabitacaoEntrada",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaTotal = table.Column<int>(type: "int", nullable: false),
                    DanosHabitacao = table.Column<bool>(type: "bit", nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FuncionarioId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ArrendamentoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArrendamentoEstadoHabitacaoEntrada", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArrendamentoEstadoHabitacaoEntrada_Arrendamento_ArrendamentoId",
                        column: x => x.ArrendamentoId,
                        principalTable: "Arrendamento",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ArrendamentoEstadoHabitacaoEntrada_AspNetUsers_FuncionarioId",
                        column: x => x.FuncionarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ArrendamentoEstadoHabitacaoSaida",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaTotal = table.Column<int>(type: "int", nullable: false),
                    DanosHabitacao = table.Column<bool>(type: "bit", nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FuncionarioId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ArrendamentoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArrendamentoEstadoHabitacaoSaida", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArrendamentoEstadoHabitacaoSaida_Arrendamento_ArrendamentoId",
                        column: x => x.ArrendamentoId,
                        principalTable: "Arrendamento",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ArrendamentoEstadoHabitacaoSaida_AspNetUsers_FuncionarioId",
                        column: x => x.FuncionarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Avaliacao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocadorId = table.Column<int>(type: "int", nullable: false),
                    ArrendamentoId = table.Column<int>(type: "int", nullable: true),
                    ClassificacaoReserva = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avaliacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Avaliacao_Arrendamento_ArrendamentoId",
                        column: x => x.ArrendamentoId,
                        principalTable: "Arrendamento",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Avaliacao_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Avaliacao_Locador_LocadorId",
                        column: x => x.LocadorId,
                        principalTable: "Locador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LocadorId",
                table: "AspNetUsers",
                column: "LocadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Arrendamento_ClienteId",
                table: "Arrendamento",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Arrendamento_HabitacaoId",
                table: "Arrendamento",
                column: "HabitacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_ArrendamentoEstadoHabitacaoEntrada_ArrendamentoId",
                table: "ArrendamentoEstadoHabitacaoEntrada",
                column: "ArrendamentoId",
                unique: true,
                filter: "[ArrendamentoId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ArrendamentoEstadoHabitacaoEntrada_FuncionarioId",
                table: "ArrendamentoEstadoHabitacaoEntrada",
                column: "FuncionarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ArrendamentoEstadoHabitacaoSaida_ArrendamentoId",
                table: "ArrendamentoEstadoHabitacaoSaida",
                column: "ArrendamentoId",
                unique: true,
                filter: "[ArrendamentoId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ArrendamentoEstadoHabitacaoSaida_FuncionarioId",
                table: "ArrendamentoEstadoHabitacaoSaida",
                column: "FuncionarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacao_ApplicationUserId",
                table: "Avaliacao",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacao_ArrendamentoId",
                table: "Avaliacao",
                column: "ArrendamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacao_LocadorId",
                table: "Avaliacao",
                column: "LocadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Habitacao_CategoriaId",
                table: "Habitacao",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Habitacao_LocadorId",
                table: "Habitacao",
                column: "LocadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Habitacao_LocalizacaoId",
                table: "Habitacao",
                column: "LocalizacaoId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Locador_LocadorId",
                table: "AspNetUsers",
                column: "LocadorId",
                principalTable: "Locador",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Locador_LocadorId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ArrendamentoEstadoHabitacaoEntrada");

            migrationBuilder.DropTable(
                name: "ArrendamentoEstadoHabitacaoSaida");

            migrationBuilder.DropTable(
                name: "Avaliacao");

            migrationBuilder.DropTable(
                name: "Arrendamento");

            migrationBuilder.DropTable(
                name: "Habitacao");

            migrationBuilder.DropTable(
                name: "CategoriasHabitacao");

            migrationBuilder.DropTable(
                name: "Locador");

            migrationBuilder.DropTable(
                name: "Localizacoes");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LocadorId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ContaAtiva",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DataNascimento",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LocadorId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NIF",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PrimeiroNome",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UltimoNome",
                table: "AspNetUsers");
        }
    }
}
