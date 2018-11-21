using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace souvenirs.Migrations
{
    public partial class EnableSouvenirDeleteRestrict : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Souvenir_SouvenirID",
                table: "OrderItems");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Souvenir_SouvenirID",
                table: "OrderItems",
                column: "SouvenirID",
                principalTable: "Souvenir",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Souvenir_SouvenirID",
                table: "OrderItems");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Souvenir_SouvenirID",
                table: "OrderItems",
                column: "SouvenirID",
                principalTable: "Souvenir",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
