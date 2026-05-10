using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthZone.Migrations
{
    /// <inheritdoc />
    public partial class ModeliUloge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Korisnici",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Ime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prezime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrojKartona = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prioritet = table.Column<int>(type: "int", nullable: false),
                    Specijalizacija = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Korisnici", x => x.Id);
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
                    Cijena = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicinskeUsluge", x => x.UslugaId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_Korisnici_UserId",
                        column: x => x.UserId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_Korisnici_UserId",
                        column: x => x.UserId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_Korisnici_UserId",
                        column: x => x.UserId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_Korisnici_UserId",
                        column: x => x.UserId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                        name: "FK_ListeCekanja_Korisnici_DoktorID",
                        column: x => x.DoktorID,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                        name: "FK_Recenzije_Korisnici_DoktorID",
                        column: x => x.DoktorID,
                        principalTable: "Korisnici",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Recenzije_Korisnici_PacijentID",
                        column: x => x.PacijentID,
                        principalTable: "Korisnici",
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
                        name: "FK_Zahtjevi_Korisnici_DoktorID",
                        column: x => x.DoktorID,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                        name: "FK_Termini_Korisnici_DoktorID",
                        column: x => x.DoktorID,
                        principalTable: "Korisnici",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Termini_Korisnici_PacijentID",
                        column: x => x.PacijentID,
                        principalTable: "Korisnici",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Termini_MedicinskeUsluge_UslugaID",
                        column: x => x.UslugaID,
                        principalTable: "MedicinskeUsluge",
                        principalColumn: "UslugaId",
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
                        name: "FK_KorisniciNaListi_Korisnici_KorisnikID",
                        column: x => x.KorisnikID,
                        principalTable: "Korisnici",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KorisniciNaListi_ListeCekanja_ListaID",
                        column: x => x.ListaID,
                        principalTable: "ListeCekanja",
                        principalColumn: "ListaId");
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
                        name: "FK_Nalazi_Korisnici_PacijentID",
                        column: x => x.PacijentID,
                        principalTable: "Korisnici",
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
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Korisnici",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Korisnici",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

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
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

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
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "ListeCekanja");

            migrationBuilder.DropTable(
                name: "Termini");

            migrationBuilder.DropTable(
                name: "Korisnici");

            migrationBuilder.DropTable(
                name: "MedicinskeUsluge");
        }
    }
}
