using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleRentSystem.Migrations
{
    public partial class create : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VehicleGallery_Vehicles_VehicleId",
                table: "VehicleGallery");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Category_CategoryId",
                table: "Vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleGallery",
                table: "VehicleGallery");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.RenameTable(
                name: "VehicleGallery",
                newName: "VehicleGalleries");

            migrationBuilder.RenameTable(
                name: "Category",
                newName: "Categories");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleGallery_VehicleId",
                table: "VehicleGalleries",
                newName: "IX_VehicleGalleries_VehicleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleGalleries",
                table: "VehicleGalleries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleGalleries_Vehicles_VehicleId",
                table: "VehicleGalleries",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Categories_CategoryId",
                table: "Vehicles",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VehicleGalleries_Vehicles_VehicleId",
                table: "VehicleGalleries");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Categories_CategoryId",
                table: "Vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleGalleries",
                table: "VehicleGalleries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "VehicleGalleries",
                newName: "VehicleGallery");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "Category");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleGalleries_VehicleId",
                table: "VehicleGallery",
                newName: "IX_VehicleGallery_VehicleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleGallery",
                table: "VehicleGallery",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleGallery_Vehicles_VehicleId",
                table: "VehicleGallery",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Category_CategoryId",
                table: "Vehicles",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
