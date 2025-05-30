using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Krakenar.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddLoggingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Logging");

            migrationBuilder.CreateTable(
                name: "Logs",
                schema: "Logging",
                columns: table => new
                {
                    LogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CorrelationId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Method = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Destination = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    AdditionalInformation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperationType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    OperationName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ActivityType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ActivityData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusCode = table.Column<int>(type: "int", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HasErrors = table.Column<bool>(type: "bit", nullable: false),
                    StartedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: true),
                    RealmId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ApiKeyId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ActorId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.LogId);
                });

            migrationBuilder.CreateTable(
                name: "LogEvents",
                schema: "Logging",
                columns: table => new
                {
                    LogEventId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LogId = table.Column<long>(type: "bigint", nullable: false),
                    LogUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogEvents", x => x.LogEventId);
                    table.ForeignKey(
                        name: "FK_LogEvents_Logs_LogId",
                        column: x => x.LogId,
                        principalSchema: "Logging",
                        principalTable: "Logs",
                        principalColumn: "LogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LogExceptions",
                schema: "Logging",
                columns: table => new
                {
                    LogExceptionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LogId = table.Column<long>(type: "bigint", nullable: false),
                    LogUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HResult = table.Column<int>(type: "int", nullable: false),
                    HelpLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StackTrace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TargetSite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogExceptions", x => x.LogExceptionId);
                    table.ForeignKey(
                        name: "FK_LogExceptions_Logs_LogId",
                        column: x => x.LogId,
                        principalSchema: "Logging",
                        principalTable: "Logs",
                        principalColumn: "LogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogEvents_LogId_EventId",
                schema: "Logging",
                table: "LogEvents",
                columns: new[] { "LogId", "EventId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogEvents_LogUid",
                schema: "Logging",
                table: "LogEvents",
                column: "LogUid");

            migrationBuilder.CreateIndex(
                name: "IX_LogExceptions_Id",
                schema: "Logging",
                table: "LogExceptions",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogExceptions_LogId",
                schema: "Logging",
                table: "LogExceptions",
                column: "LogId");

            migrationBuilder.CreateIndex(
                name: "IX_LogExceptions_LogUid",
                schema: "Logging",
                table: "LogExceptions",
                column: "LogUid");

            migrationBuilder.CreateIndex(
                name: "IX_LogExceptions_Type",
                schema: "Logging",
                table: "LogExceptions",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_ActivityType",
                schema: "Logging",
                table: "Logs",
                column: "ActivityType");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_ActorId",
                schema: "Logging",
                table: "Logs",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_ApiKeyId",
                schema: "Logging",
                table: "Logs",
                column: "ApiKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_CorrelationId",
                schema: "Logging",
                table: "Logs",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_Duration",
                schema: "Logging",
                table: "Logs",
                column: "Duration");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_EndedOn",
                schema: "Logging",
                table: "Logs",
                column: "EndedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_HasErrors",
                schema: "Logging",
                table: "Logs",
                column: "HasErrors");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_Id",
                schema: "Logging",
                table: "Logs",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Logs_IsCompleted",
                schema: "Logging",
                table: "Logs",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_Level",
                schema: "Logging",
                table: "Logs",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_Method",
                schema: "Logging",
                table: "Logs",
                column: "Method");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_OperationName",
                schema: "Logging",
                table: "Logs",
                column: "OperationName");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_OperationType",
                schema: "Logging",
                table: "Logs",
                column: "OperationType");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_RealmId",
                schema: "Logging",
                table: "Logs",
                column: "RealmId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_SessionId",
                schema: "Logging",
                table: "Logs",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_StartedOn",
                schema: "Logging",
                table: "Logs",
                column: "StartedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_StatusCode",
                schema: "Logging",
                table: "Logs",
                column: "StatusCode");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UserId",
                schema: "Logging",
                table: "Logs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogEvents",
                schema: "Logging");

            migrationBuilder.DropTable(
                name: "LogExceptions",
                schema: "Logging");

            migrationBuilder.DropTable(
                name: "Logs",
                schema: "Logging");
        }
    }
}
