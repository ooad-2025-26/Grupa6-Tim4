using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthZone.Migrations
{
    /// <inheritdoc />
    public partial class InitialIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_Korisnici_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_Korisnici_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_Korisnici_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_Korisnici_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_KorisniciNaListi_Korisnici_KorisnikID",
                table: "KorisniciNaListi");

            migrationBuilder.DropForeignKey(
                name: "FK_ListeCekanja_Korisnici_DoktorID",
                table: "ListeCekanja");

            migrationBuilder.DropForeignKey(
                name: "FK_Nalazi_Korisnici_PacijentID",
                table: "Nalazi");

            migrationBuilder.DropForeignKey(
                name: "FK_Recenzije_Korisnici_DoktorID",
                table: "Recenzije");

            migrationBuilder.DropForeignKey(
                name: "FK_Recenzije_Korisnici_PacijentID",
                table: "Recenzije");

            migrationBuilder.DropForeignKey(
                name: "FK_Termini_Korisnici_DoktorID",
                table: "Termini");

            migrationBuilder.DropForeignKey(
                name: "FK_Termini_Korisnici_PacijentID",
                table: "Termini");

            migrationBuilder.DropForeignKey(
                name: "FK_Zahtjevi_Korisnici_DoktorID",
                table: "Zahtjevi");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Korisnici",
                table: "Korisnici");

            migrationBuilder.RenameTable(
                name: "Korisnici",
                newName: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "Specijalizacija",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Prioritet",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "BrojKartona",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KorisniciNaListi_AspNetUsers_KorisnikID",
                table: "KorisniciNaListi",
                column: "KorisnikID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ListeCekanja_AspNetUsers_DoktorID",
                table: "ListeCekanja",
                column: "DoktorID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Nalazi_AspNetUsers_PacijentID",
                table: "Nalazi",
                column: "PacijentID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Recenzije_AspNetUsers_DoktorID",
                table: "Recenzije",
                column: "DoktorID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Recenzije_AspNetUsers_PacijentID",
                table: "Recenzije",
                column: "PacijentID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Termini_AspNetUsers_DoktorID",
                table: "Termini",
                column: "DoktorID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Termini_AspNetUsers_PacijentID",
                table: "Termini",
                column: "PacijentID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Zahtjevi_AspNetUsers_DoktorID",
                table: "Zahtjevi",
                column: "DoktorID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_KorisniciNaListi_AspNetUsers_KorisnikID",
                table: "KorisniciNaListi");

            migrationBuilder.DropForeignKey(
                name: "FK_ListeCekanja_AspNetUsers_DoktorID",
                table: "ListeCekanja");

            migrationBuilder.DropForeignKey(
                name: "FK_Nalazi_AspNetUsers_PacijentID",
                table: "Nalazi");

            migrationBuilder.DropForeignKey(
                name: "FK_Recenzije_AspNetUsers_DoktorID",
                table: "Recenzije");

            migrationBuilder.DropForeignKey(
                name: "FK_Recenzije_AspNetUsers_PacijentID",
                table: "Recenzije");

            migrationBuilder.DropForeignKey(
                name: "FK_Termini_AspNetUsers_DoktorID",
                table: "Termini");

            migrationBuilder.DropForeignKey(
                name: "FK_Termini_AspNetUsers_PacijentID",
                table: "Termini");

            migrationBuilder.DropForeignKey(
                name: "FK_Zahtjevi_AspNetUsers_DoktorID",
                table: "Zahtjevi");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "Korisnici");

            migrationBuilder.AlterColumn<int>(
                name: "Specijalizacija",
                table: "Korisnici",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Prioritet",
                table: "Korisnici",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BrojKartona",
                table: "Korisnici",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Korisnici",
                table: "Korisnici",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_Korisnici_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "Korisnici",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_Korisnici_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "Korisnici",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_Korisnici_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "Korisnici",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_Korisnici_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "Korisnici",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KorisniciNaListi_Korisnici_KorisnikID",
                table: "KorisniciNaListi",
                column: "KorisnikID",
                principalTable: "Korisnici",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ListeCekanja_Korisnici_DoktorID",
                table: "ListeCekanja",
                column: "DoktorID",
                principalTable: "Korisnici",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Nalazi_Korisnici_PacijentID",
                table: "Nalazi",
                column: "PacijentID",
                principalTable: "Korisnici",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Recenzije_Korisnici_DoktorID",
                table: "Recenzije",
                column: "DoktorID",
                principalTable: "Korisnici",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Recenzije_Korisnici_PacijentID",
                table: "Recenzije",
                column: "PacijentID",
                principalTable: "Korisnici",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Termini_Korisnici_DoktorID",
                table: "Termini",
                column: "DoktorID",
                principalTable: "Korisnici",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Termini_Korisnici_PacijentID",
                table: "Termini",
                column: "PacijentID",
                principalTable: "Korisnici",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Zahtjevi_Korisnici_DoktorID",
                table: "Zahtjevi",
                column: "DoktorID",
                principalTable: "Korisnici",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
