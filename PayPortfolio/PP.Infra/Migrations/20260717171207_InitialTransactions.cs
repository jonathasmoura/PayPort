using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PP.Infra.Migrations
{
    /// <inheritdoc />
    public partial class InitialTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContractsStatus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdContract = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CurrentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastPaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastTransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ActivationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InactivationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractsStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentWebHookEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdTransaction = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdContract = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceivedStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PayloadRaw = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessingStatus = table.Column<int>(type: "int", nullable: true),
                    ProcessingError = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ActivationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InactivationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentWebHookEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContractStatus_IsActive",
                table: "ContractsStatus",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "UX_ContractsStatus_IdContract",
                table: "ContractsStatus",
                column: "IdContract",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentWebHookEvent_IsActive",
                table: "PaymentWebHookEvents",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentWebHookEvents_IdContract",
                table: "PaymentWebHookEvents",
                column: "IdContract");

            migrationBuilder.CreateIndex(
                name: "UX_PaymentWebHookEvents_IdTransaction",
                table: "PaymentWebHookEvents",
                column: "IdTransaction",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractsStatus");

            migrationBuilder.DropTable(
                name: "PaymentWebHookEvents");
        }
    }
}
