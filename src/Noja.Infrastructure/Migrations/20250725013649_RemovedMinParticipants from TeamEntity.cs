﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Noja.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedMinParticipantsfromTeamEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinParticipants",
                table: "Teams");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MinParticipants",
                table: "Teams",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
