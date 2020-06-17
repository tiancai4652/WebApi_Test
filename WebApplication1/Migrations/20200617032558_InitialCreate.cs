using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication1.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    ID = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Spectrums",
                columns: table => new
                {
                    ID = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    DeviceID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectrums", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Spectrums_Devices_DeviceID",
                        column: x => x.DeviceID,
                        principalTable: "Devices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpectrumData",
                columns: table => new
                {
                    ID = table.Column<string>(nullable: false),
                    X = table.Column<float>(nullable: false),
                    Y = table.Column<float>(nullable: false),
                    SpectrumID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpectrumData", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SpectrumData_Spectrums_SpectrumID",
                        column: x => x.SpectrumID,
                        principalTable: "Spectrums",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpectrumData_SpectrumID",
                table: "SpectrumData",
                column: "SpectrumID");

            migrationBuilder.CreateIndex(
                name: "IX_Spectrums_DeviceID",
                table: "Spectrums",
                column: "DeviceID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpectrumData");

            migrationBuilder.DropTable(
                name: "Spectrums");

            migrationBuilder.DropTable(
                name: "Devices");
        }
    }
}
