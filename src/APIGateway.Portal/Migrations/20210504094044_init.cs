using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace APIGateway.Portal.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Aggregates",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ReRouteKeys = table.Column<string>(nullable: true),
                    ReRouteKeysConfig = table.Column<string>(nullable: true),
                    UpstreamPathTemplate = table.Column<string>(nullable: true),
                    UpstreamHost = table.Column<string>(nullable: true),
                    ReRouteIsCaseSensitive = table.Column<bool>(nullable: false),
                    Aggregator = table.Column<string>(nullable: true),
                    Priority = table.Column<int>(nullable: false),
                    Enable = table.Column<bool>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    DeletedTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aggregates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GlobalConfiguration",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    RequestIdKey = table.Column<string>(nullable: true),
                    ServiceDiscoveryProvider = table.Column<string>(nullable: true),
                    RateLimitOptions = table.Column<string>(nullable: true),
                    QoSOptions = table.Column<string>(nullable: true),
                    BaseUrl = table.Column<string>(nullable: true),
                    LoadBalancerOptions = table.Column<string>(nullable: true),
                    DownstreamScheme = table.Column<string>(nullable: true),
                    HttpHandlerOptions = table.Column<string>(nullable: true),
                    DownstreamHttpVersion = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    DeletedTime = table.Column<DateTime>(nullable: true),
                    Enable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalConfiguration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReRoute",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    GlobalConfigurationId = table.Column<string>(nullable: true),
                    DownstreamPathTemplate = table.Column<string>(nullable: true),
                    UpstreamPathTemplate = table.Column<string>(nullable: true),
                    UpstreamHttpMethod = table.Column<string>(nullable: true),
                    DownstreamHttpMethod = table.Column<string>(nullable: true),
                    AddHeadersToRequest = table.Column<string>(nullable: true),
                    UpstreamHeaderTransform = table.Column<string>(nullable: true),
                    DownstreamHeaderTransform = table.Column<string>(nullable: true),
                    AddClaimsToRequest = table.Column<string>(nullable: true),
                    RouteClaimsRequirement = table.Column<string>(nullable: true),
                    AddQueriesToRequest = table.Column<string>(nullable: true),
                    ChangeDownstreamPathTemplate = table.Column<string>(nullable: true),
                    RequestIdKey = table.Column<string>(nullable: true),
                    FileCacheOptions = table.Column<string>(nullable: true),
                    ReRouteIsCaseSensitive = table.Column<bool>(nullable: false),
                    ServiceName = table.Column<string>(nullable: true),
                    ServiceNamespace = table.Column<string>(nullable: true),
                    DownstreamScheme = table.Column<string>(nullable: true),
                    QoSOptions = table.Column<string>(nullable: true),
                    LoadBalancerOptions = table.Column<string>(nullable: true),
                    RateLimitOptions = table.Column<string>(nullable: true),
                    AuthenticationOptions = table.Column<string>(nullable: true),
                    HttpHandlerOptions = table.Column<string>(nullable: true),
                    DownstreamHostAndPorts = table.Column<string>(nullable: true),
                    UpstreamHost = table.Column<string>(nullable: true),
                    Key = table.Column<string>(nullable: true),
                    DelegatingHandlers = table.Column<string>(nullable: true),
                    Priority = table.Column<int>(nullable: false),
                    Timeout = table.Column<int>(nullable: false),
                    DangerousAcceptAnyServerCertificateValidator = table.Column<bool>(nullable: false),
                    SecurityOptions = table.Column<string>(nullable: true),
                    DownstreamHttpVersion = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    DeletedTime = table.Column<DateTime>(nullable: true),
                    Enable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReRoute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReRoute_GlobalConfiguration_GlobalConfigurationId",
                        column: x => x.GlobalConfigurationId,
                        principalTable: "GlobalConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReRoute_GlobalConfigurationId",
                table: "ReRoute",
                column: "GlobalConfigurationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Aggregates");

            migrationBuilder.DropTable(
                name: "ReRoute");

            migrationBuilder.DropTable(
                name: "GlobalConfiguration");
        }
    }
}
