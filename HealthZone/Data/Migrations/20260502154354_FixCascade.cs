using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthZone.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BrojKartona",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ime",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Prezime",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Prioritet",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Specijalizacija",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ListeCekanja",
                columns: table => new
                {
                    ListaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoktorID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListeCekanja", x => x.ListaId);
                    table.ForeignKey(
                        name: "FK_ListeCekanja_AspNetUsers_DoktorID",
                        column: x => x.DoktorID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicinskeUsluge",
                columns: table => new
                {
                    UslugaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Vrsta = table.Column<int>(type: "int", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cijena = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicinskeUsluge", x => x.UslugaId);
                });

            migrationBuilder.CreateTable(
                name: "Recenzije",
                columns: table => new
                {
                    RecenzijaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Komentar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OcjenaLjubaznosti = table.Column<int>(type: "int", nullable: false),
                    OcjenaProfesionalnosti = table.Column<int>(type: "int", nullable: false),
                    OcjenaUsluge = table.Column<int>(type: "int", nullable: false),
                    PacijentID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DoktorID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recenzije", x => x.RecenzijaId);
                    table.ForeignKey(
                        name: "FK_Recenzije_AspNetUsers_DoktorID",
                        column: x => x.DoktorID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Recenzije_AspNetUsers_PacijentID",
                        column: x => x.PacijentID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Zahtjevi",
                columns: table => new
                {
                    ZahtjevId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DoktorID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VrstaZahtjeva = table.Column<int>(type: "int", nullable: false),
                    Kategorija = table.Column<int>(type: "int", nullable: false),
                    Hitnost = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zahtjevi", x => x.ZahtjevId);
                    table.ForeignKey(
                        name: "FK_Zahtjevi_AspNetUsers_DoktorID",
                        column: x => x.DoktorID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KorisniciNaListi",
                columns: table => new
                {
                    KorisnikNaListiID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ListaID = table.Column<int>(type: "int", nullable: false),
                    KorisnikID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KorisniciNaListi", x => x.KorisnikNaListiID);
                    table.ForeignKey(
                        name: "FK_KorisniciNaListi_AspNetUsers_KorisnikID",
                        column: x => x.KorisnikID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KorisniciNaListi_ListeCekanja_ListaID",
                        column: x => x.ListaID,
                        principalTable: "ListeCekanja",
                        principalColumn: "ListaId");
                });

            migrationBuilder.CreateTable(
                name: "Termini",
                columns: table => new
                {
                    TerminId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Vrijeme = table.Column<TimeOnly>(type: "time", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DoktorID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PacijentID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UslugaID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Termini", x => x.TerminId);
                    table.ForeignKey(
                        name: "FK_Termini_AspNetUsers_DoktorID",
                        column: x => x.DoktorID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Termini_AspNetUsers_PacijentID",
                        column: x => x.PacijentID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Termini_MedicinskeUsluge_UslugaID",
                        column: x => x.UslugaID,
                        principalTable: "MedicinskeUsluge",
                        principalColumn: "UslugaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Nalazi",
                columns: table => new
                {
                    NalazId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dijagnoza = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Terapija = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TerminID = table.Column<int>(type: "int", nullable: false),
                    PacijentID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nalazi", x => x.NalazId);
                    table.ForeignKey(
                        name: "FK_Nalazi_AspNetUsers_PacijentID",
                        column: x => x.PacijentID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Nalazi_Termini_TerminID",
                        column: x => x.TerminID,
                        principalTable: "Termini",
                        principalColumn: "TerminId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifikacije",
                columns: table => new
                {
                    NotifikacijaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Poruka = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatumSlanja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    TerminID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifikacije", x => x.NotifikacijaId);
                    table.ForeignKey(
                        name: "FK_Notifikacije_Termini_TerminID",
                        column: x => x.TerminID,
                        principalTable: "Termini",
                        principalColumn: "TerminId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KorisniciNaListi_KorisnikID",
                table: "KorisniciNaListi",
                column: "KorisnikID");

            migrationBuilder.CreateIndex(
                name: "IX_KorisniciNaListi_ListaID",
                table: "KorisniciNaListi",
                column: "ListaID");

            migrationBuilder.CreateIndex(
                name: "IX_ListeCekanja_DoktorID",
                table: "ListeCekanja",
                column: "DoktorID");

            migrationBuilder.CreateIndex(
                name: "IX_Nalazi_PacijentID",
                table: "Nalazi",
                column: "PacijentID");

            migrationBuilder.CreateIndex(
                name: "IX_Nalazi_TerminID",
                table: "Nalazi",
                column: "TerminID");

            migrationBuilder.CreateIndex(
                name: "IX_Notifikacije_TerminID",
                table: "Notifikacije",
                column: "TerminID");

            migrationBuilder.CreateIndex(
                name: "IX_Recenzije_DoktorID",
                table: "Recenzije",
                column: "DoktorID");

            migrationBuilder.CreateIndex(
                name: "IX_Recenzije_PacijentID",
                table: "Recenzije",
                column: "PacijentID");

            migrationBuilder.CreateIndex(
                name: "IX_Termini_DoktorID",
                table: "Termini",
                column: "DoktorID");

            migrationBuilder.CreateIndex(
                name: "IX_Termini_PacijentID",
                table: "Termini",
                column: "PacijentID");

            migrationBuilder.CreateIndex(
                name: "IX_Termini_UslugaID",
                table: "Termini",
                column: "UslugaID");

            migrationBuilder.CreateIndex(
                name: "IX_Zahtjevi_DoktorID",
                table: "Zahtjevi",
                column: "DoktorID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KorisniciNaListi");

            migrationBuilder.DropTable(
                name: "Nalazi");

            migrationBuilder.DropTable(
                name: "Notifikacije");

            migrationBuilder.DropTable(
                name: "Recenzije");

            migrationBuilder.DropTable(
                name: "Zahtjevi");

            migrationBuilder.DropTable(
                name: "ListeCekanja");

            migrationBuilder.DropTable(
                name: "Termini");

            migrationBuilder.DropTable(
                name: "MedicinskeUsluge");

            migrationBuilder.DropColumn(
                name: "BrojKartona",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Ime",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Prezime",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Prioritet",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Specijalizacija",
                table: "AspNetUsers");
        }
    }
}
