using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Account.API.Migrations
{
    /// <inheritdoc />
    public partial class V1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InboxState",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConsumerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Received = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceiveCount = table.Column<int>(type: "int", nullable: false),
                    ExpirationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Consumed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Delivered = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSequenceNumber = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxState", x => x.Id);
                    table.UniqueConstraint("AK_InboxState_MessageId_ConsumerId", x => new { x.MessageId, x.ConsumerId });
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessage",
                columns: table => new
                {
                    SequenceNumber = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnqueueTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SentTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Headers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Properties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InboxMessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InboxConsumerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OutboxId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    MessageType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConversationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InitiatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DestinationAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ResponseAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    FaultAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ExpirationTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessage", x => x.SequenceNumber);
                });

            migrationBuilder.CreateTable(
                name: "OutboxState",
                columns: table => new
                {
                    OutboxId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Delivered = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSequenceNumber = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxState", x => x.OutboxId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdentityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    IsEmailVerified = table.Column<bool>(type: "bit", nullable: false),
                    NewEmail = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsPhoneNumberVerified = table.Column<bool>(type: "bit", nullable: false),
                    NewPhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccountStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address_AddressLine1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Address_AddressLine2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Address_AddressLine3 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Address_City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Address_State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Address_PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Address_Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TimeZoneId = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    UserType = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    BusinessName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: true),
                    TaxId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    WebsiteUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rating = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: true),
                    MechanicUser_DeviceId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PersonalInfo_FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PersonalInfo_MiddleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PersonalInfo_LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PersonalInfo_DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PersonalInfo_Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonalInfo_ProfilePictureUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DeviceId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BusinessLicenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "varchar(max)", nullable: false),
                    VerificationStatus = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessLicenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessLicenses_Users_BusinessUserId",
                        column: x => x.BusinessUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Certifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MechanicUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CertificationId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CertificationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ValidDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Specializations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certifications_Users_MechanicUserId",
                        column: x => x.MechanicUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DrivingLicenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FrontSideImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    BackSideImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    VerificationStatus = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrivingLicenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DrivingLicenses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoyaltyPrograms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoyaltyUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoyaltyPoints = table.Column<int>(type: "int", nullable: false),
                    LoyaltyTier = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoyaltyPrograms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoyaltyPrograms_Users_LoyaltyUserId",
                        column: x => x.LoyaltyUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NationalIdentities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdentityNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FrontSideImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    BackSideImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    VerificationStatus = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NationalIdentities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NationalIdentities_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReferralPrograms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferralCode = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValidDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RewardPoint = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferralPrograms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReferralPrograms_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReferredPrograms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferrerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferredUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferralCode = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    ReferredDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferredPrograms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReferredPrograms_Users_ReferrerId",
                        column: x => x.ReferrerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Staffs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BusinessUserId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false),
                    BusinessUserCode = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NewPhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsPhoneNumberVerified = table.Column<bool>(type: "bit", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    NewEmail = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: true),
                    IsEmailVerified = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Address_AddressLine1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Address_AddressLine2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Address_AddressLine3 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Address_City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Address_State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Address_PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Address_Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TimeZoneId = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staffs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Staffs_Users_BusinessUserId",
                        column: x => x.BusinessUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessLicenses_BusinessUserId",
                table: "BusinessLicenses",
                column: "BusinessUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessLicenses_LicenseNumber_VerificationStatus",
                table: "BusinessLicenses",
                columns: new[] { "LicenseNumber", "VerificationStatus" },
                unique: true,
                filter: "[VerificationStatus] = 'Accepted'");

            migrationBuilder.CreateIndex(
                name: "IX_Certifications_CertificationId",
                table: "Certifications",
                column: "CertificationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Certifications_MechanicUserId",
                table: "Certifications",
                column: "MechanicUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DrivingLicenses_UserId_VerificationStatus",
                table: "DrivingLicenses",
                columns: new[] { "UserId", "VerificationStatus" },
                unique: true,
                filter: "[VerificationStatus] = 'Accepted' ");

            migrationBuilder.CreateIndex(
                name: "IX_InboxState_Delivered",
                table: "InboxState",
                column: "Delivered");

            migrationBuilder.CreateIndex(
                name: "IX_LoyaltyPrograms_LoyaltyUserId",
                table: "LoyaltyPrograms",
                column: "LoyaltyUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NationalIdentities_UserId_VerificationStatus",
                table: "NationalIdentities",
                columns: new[] { "UserId", "VerificationStatus" },
                unique: true,
                filter: "[VerificationStatus] =  'Accepted' ");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_EnqueueTime",
                table: "OutboxMessage",
                column: "EnqueueTime");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_ExpirationTime",
                table: "OutboxMessage",
                column: "ExpirationTime");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_InboxMessageId_InboxConsumerId_SequenceNumber",
                table: "OutboxMessage",
                columns: new[] { "InboxMessageId", "InboxConsumerId", "SequenceNumber" },
                unique: true,
                filter: "[InboxMessageId] IS NOT NULL AND [InboxConsumerId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_OutboxId_SequenceNumber",
                table: "OutboxMessage",
                columns: new[] { "OutboxId", "SequenceNumber" },
                unique: true,
                filter: "[OutboxId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxState_Created",
                table: "OutboxState",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_ReferralPrograms_ReferralCode",
                table: "ReferralPrograms",
                column: "ReferralCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReferralPrograms_UserId",
                table: "ReferralPrograms",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReferredPrograms_ReferrerId",
                table: "ReferredPrograms",
                column: "ReferrerId");

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_BusinessUserId",
                table: "Staffs",
                column: "BusinessUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_Email",
                table: "Staffs",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_PhoneNumber",
                table: "Staffs",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_Username",
                table: "Staffs",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Code",
                table: "Users",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                table: "Users",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessLicenses");

            migrationBuilder.DropTable(
                name: "Certifications");

            migrationBuilder.DropTable(
                name: "DrivingLicenses");

            migrationBuilder.DropTable(
                name: "InboxState");

            migrationBuilder.DropTable(
                name: "LoyaltyPrograms");

            migrationBuilder.DropTable(
                name: "NationalIdentities");

            migrationBuilder.DropTable(
                name: "OutboxMessage");

            migrationBuilder.DropTable(
                name: "OutboxState");

            migrationBuilder.DropTable(
                name: "ReferralPrograms");

            migrationBuilder.DropTable(
                name: "ReferredPrograms");

            migrationBuilder.DropTable(
                name: "Staffs");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
