using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_UI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Amenities",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IconName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amenities", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CancellationPolicies",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PlatformCommissionRate = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CancellationPolicies", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CouponCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(4,1)", nullable: false),
                    MaximumDiscountAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(CONVERT([date],getdate()))"),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaxUsageCount = table.Column<int>(type: "int", nullable: true),
                    CurrentUsageCount = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DimDates",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Quarter = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    MonthName = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    DayOfMonth = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    DayName = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    IsHoliday = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimDates", x => x.ID);
                    table.UniqueConstraint("AK_DimDates_Date", x => x.Date);
                });

            migrationBuilder.CreateTable(
                name: "ItemTypes",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Scores",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scores", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTypes",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IconName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TransactionTypes",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Username = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Gender = table.Column<byte>(type: "tinyint", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProfilePicture = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Attractions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    AreaID = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attractions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Attractions_Areas",
                        column: x => x.AreaID,
                        principalTable: "Areas",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "CancellationRefundFees",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CancellationPolicyID = table.Column<long>(type: "bigint", nullable: false),
                    DaysLeft = table.Column<int>(type: "int", nullable: false),
                    PenaltyPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CancellationRefundFees", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CancellationRefundFees_CancellationPolicies",
                        column: x => x.CancellationPolicyID,
                        principalTable: "CancellationPolicies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    ServiceTypeID = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Duration = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DayOfWeek = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DayOfMonth = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DailyCap = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    BookingCap = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Services_ServiceTypes",
                        column: x => x.ServiceTypeID,
                        principalTable: "ServiceTypes",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Guests",
                columns: table => new
                {
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    LoyaltyPoints = table.Column<int>(type: "int", nullable: false),
                    PreferredLanguage = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true, defaultValue: "en"),
                    NationalID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NationalIDVerified = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guests", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_Guests_Users",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Hosts",
                columns: table => new
                {
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    BusinessLicense = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TaxCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    VerifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Rating = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    TotalReviews = table.Column<int>(type: "int", nullable: false),
                    JoinedAsHostDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hosts", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_Hosts_Users",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    TransactionTypeID = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GatewayReturnID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Transactions_DimDates",
                        column: x => x.TransactionDate,
                        principalTable: "DimDates",
                        principalColumn: "Date");
                    table.ForeignKey(
                        name: "FK_Transactions_TransactionTypes",
                        column: x => x.TransactionTypeID,
                        principalTable: "TransactionTypes",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Transactions_Users",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "HostBankAccounts",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    HostUserID = table.Column<long>(type: "bigint", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountNumber = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    AccountHolder = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostBankAccounts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HostBankAccounts_Hosts",
                        column: x => x.HostUserID,
                        principalTable: "Hosts",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    HostUserID = table.Column<long>(type: "bigint", nullable: false),
                    ItemTypeID = table.Column<long>(type: "bigint", nullable: false),
                    AreaID = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    NumberOfBeds = table.Column<int>(type: "int", nullable: false),
                    NumberOfBedrooms = table.Column<int>(type: "int", nullable: false),
                    NumberOfBathrooms = table.Column<int>(type: "int", nullable: false),
                    ExactAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ApproximateAddress = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    HostRules = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    MinimumNights = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    MaximumNights = table.Column<int>(type: "int", nullable: false, defaultValue: 365),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Items_Areas",
                        column: x => x.AreaID,
                        principalTable: "Areas",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Items_Hosts",
                        column: x => x.HostUserID,
                        principalTable: "Hosts",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_Items_ItemTypes",
                        column: x => x.ItemTypeID,
                        principalTable: "ItemTypes",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    GuestUserID = table.Column<long>(type: "bigint", nullable: false),
                    ItemID = table.Column<long>(type: "bigint", nullable: false),
                    CheckInDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckOutDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumberOfGuests = table.Column<int>(type: "int", nullable: false),
                    PricePerNight = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    FinalPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CancellationPolicyID = table.Column<long>(type: "bigint", nullable: false),
                    BookingStatus = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false, defaultValue: "Pending"),
                    BookingDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    SpecialRequests = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TransactionID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Bookings_CancellationPolicies",
                        column: x => x.CancellationPolicyID,
                        principalTable: "CancellationPolicies",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Bookings_CheckInDate_DimDates",
                        column: x => x.CheckInDate,
                        principalTable: "DimDates",
                        principalColumn: "Date");
                    table.ForeignKey(
                        name: "FK_Bookings_CheckOutDate_DimDates",
                        column: x => x.CheckOutDate,
                        principalTable: "DimDates",
                        principalColumn: "Date");
                    table.ForeignKey(
                        name: "FK_Bookings_Guests",
                        column: x => x.GuestUserID,
                        principalTable: "Guests",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_Bookings_Items",
                        column: x => x.ItemID,
                        principalTable: "Items",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Bookings_Transactions",
                        column: x => x.TransactionID,
                        principalTable: "Transactions",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ItemAmenities",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    ItemID = table.Column<long>(type: "bigint", nullable: false),
                    AmenityID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemAmenities", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ItemAmenities_Amenities",
                        column: x => x.AmenityID,
                        principalTable: "Amenities",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ItemAmenities_Items",
                        column: x => x.ItemID,
                        principalTable: "Items",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemAttractions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    ItemID = table.Column<long>(type: "bigint", nullable: false),
                    AttractionID = table.Column<long>(type: "bigint", nullable: false),
                    Distance = table.Column<decimal>(type: "decimal(5,1)", nullable: true),
                    DurationOnFoot = table.Column<int>(type: "int", nullable: true),
                    DurationByCar = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemAttractions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ItemAttractions_Attractions",
                        column: x => x.AttractionID,
                        principalTable: "Attractions",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ItemAttractions_Items",
                        column: x => x.ItemID,
                        principalTable: "Items",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemAvailability",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemID = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemAvailability", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ItemAvailability_DimDates",
                        column: x => x.Date,
                        principalTable: "DimDates",
                        principalColumn: "Date");
                    table.ForeignKey(
                        name: "FK_ItemAvailability_Items",
                        column: x => x.ItemID,
                        principalTable: "Items",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemPictures",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    ItemID = table.Column<long>(type: "bigint", nullable: false),
                    PictureFileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemPictures", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ItemPictures_Items",
                        column: x => x.ItemID,
                        principalTable: "Items",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemPrices",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    ItemID = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CancellationPolicyID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemPrices", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ItemPrices_CancellationPolicies",
                        column: x => x.CancellationPolicyID,
                        principalTable: "CancellationPolicies",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ItemPrices_DimDates",
                        column: x => x.Date,
                        principalTable: "DimDates",
                        principalColumn: "Date");
                    table.ForeignKey(
                        name: "FK_ItemPrices_Items",
                        column: x => x.ItemID,
                        principalTable: "Items",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemScores",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    ItemID = table.Column<long>(type: "bigint", nullable: false),
                    ScoreID = table.Column<long>(type: "bigint", nullable: false),
                    Value = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemScores", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ItemScores_Items",
                        column: x => x.ItemID,
                        principalTable: "Items",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ItemScores_Scores",
                        column: x => x.ScoreID,
                        principalTable: "Scores",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ItemScores_Users",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "AddonServices",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    BookingID = table.Column<long>(type: "bigint", nullable: true),
                    CouponID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddonServices", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AddonServices_Bookings",
                        column: x => x.BookingID,
                        principalTable: "Bookings",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_AddonServices_Coupons",
                        column: x => x.CouponID,
                        principalTable: "Coupons",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_AddonServices_Users",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "BookingCoupons",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    BookingID = table.Column<long>(type: "bigint", nullable: false),
                    CouponID = table.Column<long>(type: "bigint", nullable: false),
                    DiscountApplied = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    AppliedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingCoupons", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BookingCoupons_Bookings",
                        column: x => x.BookingID,
                        principalTable: "Bookings",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_BookingCoupons_Coupons",
                        column: x => x.CouponID,
                        principalTable: "Coupons",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "BookingStatusHistory",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    BookingID = table.Column<long>(type: "bigint", nullable: false),
                    OldStatus = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    NewStatus = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    ChangedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ChangedByUserID = table.Column<long>(type: "bigint", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingStatusHistory", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BookingStatusHistory_Bookings",
                        column: x => x.BookingID,
                        principalTable: "Bookings",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_BookingStatusHistory_ChangedByUser",
                        column: x => x.ChangedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    BookingID = table.Column<long>(type: "bigint", nullable: false),
                    ReviewerID = table.Column<long>(type: "bigint", nullable: false),
                    RevieweeID = table.Column<long>(type: "bigint", nullable: true),
                    ItemID = table.Column<long>(type: "bigint", nullable: true),
                    Rating = table.Column<byte>(type: "tinyint", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Reviews_Bookings",
                        column: x => x.BookingID,
                        principalTable: "Bookings",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Reviews_Items",
                        column: x => x.ItemID,
                        principalTable: "Items",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Reviews_Reviewee",
                        column: x => x.RevieweeID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Reviews_Reviewer",
                        column: x => x.ReviewerID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "BookingDetails",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    BookingID = table.Column<long>(type: "bigint", nullable: false),
                    ItemPriceID = table.Column<long>(type: "bigint", nullable: false),
                    isRefund = table.Column<bool>(type: "bit", nullable: false),
                    RefundDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RefundCancellationPolicyID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BookingDetails_Bookings",
                        column: x => x.BookingID,
                        principalTable: "Bookings",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_BookingDetails_ItemPrices",
                        column: x => x.ItemPriceID,
                        principalTable: "ItemPrices",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_BookingDetails_RefundPolicy",
                        column: x => x.RefundCancellationPolicyID,
                        principalTable: "CancellationPolicies",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "AddonServiceDetails",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    AddonServiceID = table.Column<long>(type: "bigint", nullable: false),
                    ServiceID = table.Column<long>(type: "bigint", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    NumberOfPeople = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    isRefund = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddonServiceDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AddonServiceDetails_AddonServices",
                        column: x => x.AddonServiceID,
                        principalTable: "AddonServices",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_AddonServiceDetails_Services",
                        column: x => x.ServiceID,
                        principalTable: "Services",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddonServiceDetails_AddonServiceID",
                table: "AddonServiceDetails",
                column: "AddonServiceID");

            migrationBuilder.CreateIndex(
                name: "IX_AddonServiceDetails_ServiceID",
                table: "AddonServiceDetails",
                column: "ServiceID");

            migrationBuilder.CreateIndex(
                name: "UQ_AddonServiceDetails_GUID",
                table: "AddonServiceDetails",
                column: "GUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AddonServices_BookingID",
                table: "AddonServices",
                column: "BookingID");

            migrationBuilder.CreateIndex(
                name: "IX_AddonServices_CouponID",
                table: "AddonServices",
                column: "CouponID");

            migrationBuilder.CreateIndex(
                name: "IX_AddonServices_UserID",
                table: "AddonServices",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "UQ_AddonServices_GUID",
                table: "AddonServices",
                column: "GUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Amenities_GUID",
                table: "Amenities",
                column: "GUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Areas_GUID",
                table: "Areas",
                column: "GUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attractions_AreaID",
                table: "Attractions",
                column: "AreaID");

            migrationBuilder.CreateIndex(
                name: "UQ_Attractions_GUID",
                table: "Attractions",
                column: "GUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingCoupons_CouponID",
                table: "BookingCoupons",
                column: "CouponID");

            migrationBuilder.CreateIndex(
                name: "UQ_BookingCoupons_GUID",
                table: "BookingCoupons",
                column: "GUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_BookingCoupons_Pair",
                table: "BookingCoupons",
                columns: new[] { "BookingID", "CouponID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingDetails_BookingID",
                table: "BookingDetails",
                column: "BookingID");

            migrationBuilder.CreateIndex(
                name: "IX_BookingDetails_ItemPriceID",
                table: "BookingDetails",
                column: "ItemPriceID");

            migrationBuilder.CreateIndex(
                name: "IX_BookingDetails_RefundCancellationPolicyID",
                table: "BookingDetails",
                column: "RefundCancellationPolicyID");

            migrationBuilder.CreateIndex(
                name: "UQ_BookingDetails_GUID",
                table: "BookingDetails",
                column: "GUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CancellationPolicyID",
                table: "Bookings",
                column: "CancellationPolicyID");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CheckInDate",
                table: "Bookings",
                column: "CheckInDate");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CheckOutDate",
                table: "Bookings",
                column: "CheckOutDate");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_GuestUserID",
                table: "Bookings",
                column: "GuestUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ItemID",
                table: "Bookings",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TransactionID",
                table: "Bookings",
                column: "TransactionID");

            migrationBuilder.CreateIndex(
                name: "UQ_Bookings_GUID",
                table: "Bookings",
                column: "GUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingStatusHistory_BookingID",
                table: "BookingStatusHistory",
                column: "BookingID");

            migrationBuilder.CreateIndex(
                name: "IX_BookingStatusHistory_ChangedByUserID",
                table: "BookingStatusHistory",
                column: "ChangedByUserID");

            migrationBuilder.CreateIndex(
                name: "UQ_BookingStatusHistory_GUID",
                table: "BookingStatusHistory",
                column: "GUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_CancellationPolicies_GUID",
                table: "CancellationPolicies",
                column: "GUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CancellationRefundFees_CancellationPolicyID",
                table: "CancellationRefundFees",
                column: "CancellationPolicyID");

            migrationBuilder.CreateIndex(
                name: "UQ_CancellationRefundFees_GUID",
                table: "CancellationRefundFees",
                column: "GUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Coupons_CouponCode",
                table: "Coupons",
                column: "CouponCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Coupons_GUID",
                table: "Coupons",
                column: "GUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_DimDates_Date",
                table: "DimDates",
                column: "Date",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HostBankAccounts_HostUserID",
                table: "HostBankAccounts",
                column: "HostUserID");

            migrationBuilder.CreateIndex(
                name: "UQ_HostBankAccounts_GUID",
                table: "HostBankAccounts",
                column: "GUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemAmenities_AmenityID",
                table: "ItemAmenities",
                column: "AmenityID");

            migrationBuilder.CreateIndex(
                name: "UQ_ItemAmenities_GUID",
                table: "ItemAmenities",
                column: "GUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_ItemAmenities_Pair",
                table: "ItemAmenities",
                columns: new[] { "ItemID", "AmenityID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemAttractions_AttractionID",
                table: "ItemAttractions",
                column: "AttractionID");

            migrationBuilder.CreateIndex(
                name: "UQ_ItemAttractions_GUID",
                table: "ItemAttractions",
                column: "GUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_ItemAttractions_Pair",
                table: "ItemAttractions",
                columns: new[] { "ItemID", "AttractionID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemAvailability_Date",
                table: "ItemAvailability",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "UQ_ItemAvailability_ItemDate",
                table: "ItemAvailability",
                columns: new[] { "ItemID", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemPictures_ItemID",
                table: "ItemPictures",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "UQ_ItemPictures_GUID",
                table: "ItemPictures",
                column: "GUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemPrices_CancellationPolicyID",
                table: "ItemPrices",
                column: "CancellationPolicyID");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPrices_Date",
                table: "ItemPrices",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "UQ_ItemPrices_GUID",
                table: "ItemPrices",
                column: "GUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_ItemPrices_ItemDate",
                table: "ItemPrices",
                columns: new[] { "ItemID", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_AreaID",
                table: "Items",
                column: "AreaID");

            migrationBuilder.CreateIndex(
                name: "IX_Items_HostUserID",
                table: "Items",
                column: "HostUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemTypeID",
                table: "Items",
                column: "ItemTypeID");

            migrationBuilder.CreateIndex(
                name: "UQ_Items_GUID",
                table: "Items",
                column: "GUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemScores_ItemID",
                table: "ItemScores",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_ItemScores_ScoreID",
                table: "ItemScores",
                column: "ScoreID");

            migrationBuilder.CreateIndex(
                name: "UQ_ItemScores_GUID",
                table: "ItemScores",
                column: "GUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_ItemScores_Pair",
                table: "ItemScores",
                columns: new[] { "UserID", "ItemID", "ScoreID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_ItemTypes_GUID",
                table: "ItemTypes",
                column: "GUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BookingID",
                table: "Reviews",
                column: "BookingID");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ItemID",
                table: "Reviews",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_RevieweeID",
                table: "Reviews",
                column: "RevieweeID");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReviewerID",
                table: "Reviews",
                column: "ReviewerID");

            migrationBuilder.CreateIndex(
                name: "UQ_Reviews_GUID",
                table: "Reviews",
                column: "GUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Scores_GUID",
                table: "Scores",
                column: "GUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceTypeID",
                table: "Services",
                column: "ServiceTypeID");

            migrationBuilder.CreateIndex(
                name: "UQ_Services_GUID",
                table: "Services",
                column: "GUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_ServiceTypes_GUID",
                table: "ServiceTypes",
                column: "GUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionDate",
                table: "Transactions",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionTypeID",
                table: "Transactions",
                column: "TransactionTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserID",
                table: "Transactions",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "UQ_Transactions_GUID",
                table: "Transactions",
                column: "GUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_TransactionTypes_GUID",
                table: "TransactionTypes",
                column: "GUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Users_GUID",
                table: "Users",
                column: "GUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddonServiceDetails");

            migrationBuilder.DropTable(
                name: "BookingCoupons");

            migrationBuilder.DropTable(
                name: "BookingDetails");

            migrationBuilder.DropTable(
                name: "BookingStatusHistory");

            migrationBuilder.DropTable(
                name: "CancellationRefundFees");

            migrationBuilder.DropTable(
                name: "HostBankAccounts");

            migrationBuilder.DropTable(
                name: "ItemAmenities");

            migrationBuilder.DropTable(
                name: "ItemAttractions");

            migrationBuilder.DropTable(
                name: "ItemAvailability");

            migrationBuilder.DropTable(
                name: "ItemPictures");

            migrationBuilder.DropTable(
                name: "ItemScores");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "AddonServices");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "ItemPrices");

            migrationBuilder.DropTable(
                name: "Amenities");

            migrationBuilder.DropTable(
                name: "Attractions");

            migrationBuilder.DropTable(
                name: "Scores");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Coupons");

            migrationBuilder.DropTable(
                name: "ServiceTypes");

            migrationBuilder.DropTable(
                name: "CancellationPolicies");

            migrationBuilder.DropTable(
                name: "Guests");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.DropTable(
                name: "Hosts");

            migrationBuilder.DropTable(
                name: "ItemTypes");

            migrationBuilder.DropTable(
                name: "DimDates");

            migrationBuilder.DropTable(
                name: "TransactionTypes");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
