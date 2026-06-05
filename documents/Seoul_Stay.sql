-- ============================================================
    -- DATABASE: Seoul_Stay
    -- ============================================================
    USE master;
    GO
    IF EXISTS (SELECT name FROM sys.databases WHERE name = N'Seoul_Stay')
        ALTER DATABASE Seoul_Stay
        SET SINGLE_USER
        WITH ROLLBACK IMMEDIATE;
    GO

    DROP DATABASE Seoul_Stay;
    GO

    CREATE DATABASE Seoul_Stay
    GO

    USE Seoul_Stay
    GO

    SET ANSI_NULLS ON
    GO
    SET QUOTED_IDENTIFIER ON
    GO

    -- ============================================================
    -- LOOKUP TABLES
    -- ============================================================
    CREATE TABLE [dbo].[ItemTypes](
        [ID]          [bigint]           IDENTITY(1,1) NOT NULL,
        [GUID]        [uniqueidentifier] NOT NULL DEFAULT NEWID(),
        [Name]        [nvarchar](50)     NOT NULL,
        [Icon]        [nvarchar](100)    NULL,
        [Description] [nvarchar](500)    NULL,
        CONSTRAINT [PK_ItemTypes]      PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_ItemTypes_GUID] UNIQUE ([GUID])
    )
    GO

    CREATE TABLE [dbo].[TransactionTypes](
        [ID]   [bigint]           IDENTITY(1,1) NOT NULL,
        [GUID] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
        [Name] [nvarchar](50)     NOT NULL,
        CONSTRAINT [PK_TransactionTypes]      PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_TransactionTypes_GUID] UNIQUE ([GUID])
    )
    GO

    CREATE TABLE [dbo].[Areas](
        [ID]   [bigint]           IDENTITY(1,1) NOT NULL,
        [GUID] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
        [Name] [nvarchar](100)    NOT NULL,
        CONSTRAINT [PK_Areas]      PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_Areas_GUID] UNIQUE ([GUID])
    )
    GO

    -- ============================================================
    -- DIM DATES
    -- ============================================================
    CREATE TABLE [dbo].[DimDates](
        [ID]         [bigint]      NOT NULL,
        [Date]       [date]        NOT NULL,
        [Year]       [int]         NOT NULL,
        [Quarter]    [int]         NOT NULL,
        [Month]      [int]         NOT NULL,
        [MonthName]  [varchar](10) NOT NULL,
        [DayOfMonth] [int]         NOT NULL,
        [DayOfWeek]  [int]         NOT NULL,
        [DayName]    [varchar](10) NOT NULL,
        [IsHoliday]  [bit]         NOT NULL DEFAULT 0,
        CONSTRAINT [PK_DimDates]      PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_DimDates_Date] UNIQUE ([Date])
    )
    GO

    -- ============================================================
    -- AMENITIES & ATTRACTIONS
    -- ============================================================
    CREATE TABLE [dbo].[Amenities](
        [ID]       [bigint]           IDENTITY(1,1) NOT NULL,
        [GUID]     [uniqueidentifier] NOT NULL DEFAULT NEWID(),
        [Name]     [nvarchar](100)    NOT NULL,
        [IconName] [nvarchar](100)    NULL,
        CONSTRAINT [PK_Amenities]      PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_Amenities_GUID] UNIQUE ([GUID])
    )
    GO

    CREATE TABLE [dbo].[Attractions](
        [ID]      [bigint]           IDENTITY(1,1) NOT NULL,
        [GUID]    [uniqueidentifier] NOT NULL DEFAULT NEWID(),
        [AreaID]  [bigint]           NOT NULL,
        [Name]    [nvarchar](150)    NOT NULL,
        [Address] [nvarchar](500)    NOT NULL,
        CONSTRAINT [PK_Attractions]      PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_Attractions_GUID] UNIQUE ([GUID])
    )
    GO

    -- ============================================================
    -- CANCELLATION POLICIES & REFUND FEES
    -- ============================================================
    CREATE TABLE [dbo].[CancellationPolicies](
        [ID]                     [bigint]           IDENTITY(1,1) NOT NULL,
        [GUID]                   [uniqueidentifier] NOT NULL DEFAULT NEWID(),
        [Name]                   [nvarchar](100)    NOT NULL,
        [PlatformCommissionRate] [decimal](5,2)     NOT NULL,
        CONSTRAINT [PK_CancellationPolicies]      PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_CancellationPolicies_GUID] UNIQUE ([GUID]),
        CONSTRAINT [CK_CancellationPolicies_Rate] CHECK ([PlatformCommissionRate] BETWEEN 0 AND 100)
    )
    GO

    CREATE TABLE [dbo].[CancellationRefundFees](
        [ID]                   [bigint]           IDENTITY(1,1) NOT NULL,
        [GUID]                 [uniqueidentifier] NOT NULL DEFAULT NEWID(),
        [CancellationPolicyID] [bigint]           NOT NULL,
        [DaysLeft]             [int]              NOT NULL,
        [PenaltyPercentage]    [decimal](5,2)     NOT NULL,
        CONSTRAINT [PK_CancellationRefundFees]          PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_CancellationRefundFees_GUID]     UNIQUE ([GUID]),
        CONSTRAINT [CK_CancellationRefundFees_DaysLeft] CHECK ([DaysLeft] >= 0),
        CONSTRAINT [CK_CancellationRefundFees_Penalty]  CHECK ([PenaltyPercentage] BETWEEN 0 AND 100)
    )
    GO

    -- ============================================================
    -- COUPONS
    -- ============================================================
    CREATE TABLE [dbo].[Coupons](
        [ID]                    [bigint]           IDENTITY(1,1) NOT NULL,
        [GUID]                  [uniqueidentifier] NOT NULL DEFAULT NEWID(),
        [CouponCode]            [nvarchar](50)     NOT NULL,
        [DiscountPercent]       [decimal](4,1)     NOT NULL,
        [MaximumDiscountAmount] [decimal](10,2)    NOT NULL,
        [StartDate]             [date]             NOT NULL DEFAULT CAST(GETDATE() AS date),
        [ExpirationDate]        [date]             NULL,
        [MaxUsageCount]         [int]              NULL,
        [CurrentUsageCount]     [int]              NOT NULL DEFAULT 0,
        [IsActive]              [bit]              NOT NULL DEFAULT 1,
        CONSTRAINT [PK_Coupons]            PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_Coupons_GUID]       UNIQUE ([GUID]),
        CONSTRAINT [UQ_Coupons_CouponCode] UNIQUE ([CouponCode]),
        CONSTRAINT [CK_Coupons_Discount]   CHECK ([DiscountPercent] BETWEEN 0 AND 100),
        CONSTRAINT [CK_Coupons_Usage]      CHECK ([CurrentUsageCount] >= 0),
        CONSTRAINT [CK_Coupons_DateRange]  CHECK ([ExpirationDate] IS NULL OR [ExpirationDate] > [StartDate])
    )
    GO

    -- ============================================================
    -- USERS (core identity)
    -- ============================================================
    CREATE TABLE [dbo].[Users](
        [ID]             [bigint]           IDENTITY(1,1) NOT NULL,
        [GUID]           [uniqueidentifier] NOT NULL DEFAULT NEWID(),
        [Username]       [varchar](50)      NOT NULL,
        [Password]       [varchar](255)     NOT NULL,
        [FullName]       [nvarchar](100)    NOT NULL,
        [Email]          [varchar](100)     NOT NULL,
        [PhoneNumber]    [varchar](20)      NULL,
        [Gender]         [tinyint]          NOT NULL DEFAULT 0,
        [BirthDate]      [date]             NULL,
        [Country]        [nvarchar](50)     NULL,
        [ProfilePicture] [varchar](500)     NULL,
        [IsAdmin]        [bit]              NOT NULL DEFAULT 0,
        [CreatedDate]    [datetime]         NOT NULL DEFAULT GETDATE(),
        [IsActive]       [bit]              NOT NULL DEFAULT 1,
        CONSTRAINT [PK_Users]          PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_Users_GUID]     UNIQUE ([GUID]),
        CONSTRAINT [UQ_Users_Username] UNIQUE ([Username]),
        CONSTRAINT [UQ_Users_Email]    UNIQUE ([Email]),
        CONSTRAINT [CK_Users_Gender]   CHECK ([Gender] IN (0, 1, 2, 3))
    )
    GO

    -- ============================================================
    -- GUEST PROFILE
    -- ============================================================
    CREATE TABLE [dbo].[Guests](
        [UserID]            [bigint]        NOT NULL,
        [LoyaltyPoints]     [int]           NOT NULL DEFAULT 0,
        [PreferredLanguage] [varchar](10)   NULL DEFAULT 'en',
        [NationalID]        [nvarchar](50)  NULL,
        [NationalIDVerified][bit]           NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Guests]             PRIMARY KEY CLUSTERED ([UserID] ASC),
        CONSTRAINT [CK_Guests_Loyalty]     CHECK ([LoyaltyPoints] >= 0)
    )
    GO

    -- ============================================================
    -- HOST PROFILE
    -- ============================================================
    CREATE TABLE [dbo].[Hosts](
        [UserID]          [bigint]         NOT NULL,
        [BusinessLicense] [nvarchar](100)  NULL,
        [TaxCode]         [nvarchar](50)   NULL,
        [IsVerified]      [bit]            NOT NULL DEFAULT 0,
        [VerifiedDate]    [datetime]       NULL,
        [Rating]          [decimal](3,2)   NULL,
        [TotalReviews]    [int]            NOT NULL DEFAULT 0,
        [JoinedAsHostDate][datetime]       NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Hosts]          PRIMARY KEY CLUSTERED ([UserID] ASC),
        CONSTRAINT [CK_Hosts_Rating]   CHECK ([Rating] IS NULL OR [Rating] BETWEEN 1.00 AND 5.00),
        CONSTRAINT [CK_Hosts_Reviews]  CHECK ([TotalReviews] >= 0)
    )
    GO

    -- ============================================================
    -- HOST BANK ACCOUNTS
    -- ============================================================
    CREATE TABLE [dbo].[HostBankAccounts](
        [ID]            [bigint]           IDENTITY(1,1) NOT NULL,
        [GUID]          [uniqueidentifier] NOT NULL DEFAULT NEWID(),
        [HostUserID]    [bigint]           NOT NULL,
        [BankName]      [nvarchar](100)    NOT NULL,
        [AccountNumber] [varchar](100)     NOT NULL,
        [AccountHolder] [nvarchar](100)    NOT NULL,
        [IsPrimary]     [bit]              NOT NULL DEFAULT 0,
        [IsVerified]    [bit]              NOT NULL DEFAULT 0,
        [CreatedDate]   [datetime]         NOT NULL DEFAULT GETDATE(),
        [IsActive]      [bit]              NOT NULL DEFAULT 1,
        CONSTRAINT [PK_HostBankAccounts]      PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_HostBankAccounts_GUID] UNIQUE ([GUID])
    )
    GO

    -- ============================================================
    -- SCORES & ITEM SCORES (from Session6)
    -- ============================================================
    CREATE TABLE [dbo].[Scores](
        [ID]   [bigint]           IDENTITY(1,1) NOT NULL,
        [GUID] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
        [Name] [nvarchar](50)     NOT NULL,
        CONSTRAINT [PK_Scores]      PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_Scores_GUID] UNIQUE ([GUID])
    )
    GO

    CREATE TABLE [dbo].[ItemScores](
        [ID]      [bigint]           IDENTITY(1,1) NOT NULL,
        [GUID]    [uniqueidentifier] NOT NULL DEFAULT NEWID(),
        [UserID]  [bigint]           NOT NULL,
        [ItemID]  [bigint]           NOT NULL,
        [ScoreID] [bigint]           NOT NULL,
        [Value]   [bigint]           NOT NULL,
        CONSTRAINT [PK_ItemScores]        PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_ItemScores_GUID]   UNIQUE ([GUID]),
        CONSTRAINT [UQ_ItemScores_Pair]   UNIQUE ([UserID], [ItemID], [ScoreID]),
        CONSTRAINT [CK_ItemScores_Value]  CHECK ([Value] BETWEEN 1 AND 5)
    )
    GO

    -- ============================================================
    -- ITEMS
    -- ============================================================
    CREATE TABLE [dbo].[Items](
        [ID]                 [bigint]           IDENTITY(1,1) NOT NULL,
        [GUID]               [uniqueidentifier] NOT NULL DEFAULT NEWID(),
        [HostUserID]         [bigint]           NOT NULL,
        [ItemTypeID]         [bigint]           NOT NULL,
        [AreaID]             [bigint]           NOT NULL,
        [Title]              [nvarchar](100)    NOT NULL,
        [Capacity]           [int]              NOT NULL,
        [NumberOfBeds]       [int]              NOT NULL,
        [NumberOfBedrooms]   [int]              NOT NULL,
        [NumberOfBathrooms]  [int]              NOT NULL,
        [ExactAddress]       [nvarchar](500)    NOT NULL,
        [ApproximateAddress] [nvarchar](250)    NOT NULL,
        [Description]        [nvarchar](2000)   NOT NULL,
        [HostRules]          [nvarchar](2000)   NOT NULL,
        [MinimumNights]      [int]              NOT NULL DEFAULT 1,
        [MaximumNights]      [int]              NOT NULL DEFAULT 365,
        [CreatedDate]        [datetime]         NOT NULL DEFAULT GETDATE(),
        [IsActive]           [bit]              NOT NULL DEFAULT 1,
        CONSTRAINT [PK_Items]          PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_Items_GUID]     UNIQUE ([GUID]),
        CONSTRAINT [CK_Items_Capacity] CHECK ([Capacity] >= 1),
        CONSTRAINT [CK_Items_Nights]   CHECK ([MaximumNights] >= [MinimumNights] AND [MinimumNights] >= 1)
    )
    GO

    -- ============================================================
    -- ITEM AMENITIES & ATTRACTIONS & PICTURES & PRICES
    -- ============================================================
    CREATE TABLE [dbo].[ItemAmenities](
        [ID]        [bigint]           IDENTITY(1,1) NOT NULL,
        [GUID]      [uniqueidentifier] NOT NULL DEFAULT NEWID(),
        [ItemID]    [bigint]           NOT NULL,
        [AmenityID] [bigint]           NOT NULL,
        CONSTRAINT [PK_ItemAmenities]      PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_ItemAmenities_GUID] UNIQUE ([GUID]),
        CONSTRAINT [UQ_ItemAmenities_Pair] UNIQUE ([ItemID], [AmenityID])
    )
    GO

    CREATE TABLE [dbo].[ItemAttractions](
        [ID]             [bigint]           IDENTITY(1,1) NOT NULL,
        [GUID]           [uniqueidentifier] NOT NULL DEFAULT NEWID(),
        [ItemID]         [bigint]           NOT NULL,
        [AttractionID]   [bigint]           NOT NULL,
        [Distance]       [decimal](5,1)     NULL,
        [DurationOnFoot] [int]              NULL,
        [DurationByCar]  [int]              NULL,
        CONSTRAINT [PK_ItemAttractions]      PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_ItemAttractions_GUID] UNIQUE ([GUID]),
        CONSTRAINT [UQ_ItemAttractions_Pair] UNIQUE ([ItemID], [AttractionID])
    )
    GO

    CREATE TABLE [dbo].[ItemPictures](
        [ID]              [bigint]           IDENTITY(1,1) NOT NULL,
        [GUID]            [uniqueidentifier] NOT NULL DEFAULT NEWID(),
        [ItemID]          [bigint]           NOT NULL,
        [PictureFileName] [nvarchar](500)    NOT NULL,
        [DisplayOrder]    [int]              NOT NULL DEFAULT 0,
        CONSTRAINT [PK_ItemPictures]      PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_ItemPictures_GUID] UNIQUE ([GUID])
    )
    GO

    CREATE TABLE [dbo].[ItemPrices](
        [ID]                   [bigint]           IDENTITY(1,1) NOT NULL,
        [GUID]                 [uniqueidentifier] NOT NULL DEFAULT NEWID(),
        [ItemID]               [bigint]           NOT NULL,
        [Date]                 [date]             NOT NULL,
        [Price]                [decimal](10,2)    NOT NULL,
        [CancellationPolicyID] [bigint]           NOT NULL,
        CONSTRAINT [PK_ItemPrices]          PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_ItemPrices_GUID]     UNIQUE ([GUID]),
        CONSTRAINT [UQ_ItemPrices_ItemDate] UNIQUE ([ItemID], [Date]),
        CONSTRAINT [CK_ItemPrices_Price]    CHECK ([Price] > 0)
    )
    GO

    CREATE TABLE [dbo].[ItemAvailability](
        [ID]          [bigint] IDENTITY(1,1) NOT NULL,
        [ItemID]      [bigint] NOT NULL,
        [Date]        [date]   NOT NULL,
        [IsAvailable] [bit]    NOT NULL DEFAULT 1,
        CONSTRAINT [PK_ItemAvailability]         PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_ItemAvailability_ItemDate] UNIQUE ([ItemID], [Date])
    )
    GO

    -- ============================================================
    -- TRANSACTIONS
    -- ============================================================
    CREATE TABLE [dbo].[Transactions](
        [ID]                [bigint]           IDENTITY(1,1) NOT NULL,
        [GUID]              [uniqueidentifier] NOT NULL DEFAULT NEWID(),
        [UserID]            [bigint]           NOT NULL,
        [TransactionTypeID] [bigint]           NOT NULL,
        [Amount]            [decimal](18,2)    NOT NULL,
        [TransactionDate]   [date]             NOT NULL,
        [GatewayReturnID]   [nvarchar](100)    NOT NULL,
        CONSTRAINT [PK_Transactions]        PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_Transactions_GUID]   UNIQUE ([GUID]),
        CONSTRAINT [CK_Transactions_Amount] CHECK ([Amount] > 0)
    )
    GO

    -- ============================================================
    -- BOOKINGS (core)
    -- ============================================================
    CREATE TABLE [dbo].[Bookings](
        [ID]                   [bigint]           IDENTITY(1,1) NOT NULL,
        [GUID]                 [uniqueidentifier] NOT NULL DEFAULT NEWID(),
        [GuestUserID]          [bigint]           NOT NULL,
        [ItemID]               [bigint]           NOT NULL,
        [CheckInDate]          [date]             NOT NULL,
        [CheckOutDate]         [date]             NOT NULL,
        [NumberOfGuests]       [int]              NOT NULL,
        [PricePerNight]        [decimal](10,2)    NOT NULL,
        [TotalPrice]           [decimal](10,2)    NOT NULL,
        [DiscountAmount]       [decimal](10,2)    NOT NULL DEFAULT 0,
        [FinalPrice]           [decimal](10,2)    NOT NULL,
        [CancellationPolicyID] [bigint]           NOT NULL,
        [BookingStatus]        [varchar](20)      NOT NULL DEFAULT 'Pending',
        [BookingDate]          [datetime]         NOT NULL DEFAULT GETDATE(),
        [SpecialRequests]      [nvarchar](1000)   NULL,
        [TransactionID]        [bigint]           NULL,
        CONSTRAINT [PK_Bookings]            PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_Bookings_GUID]       UNIQUE ([GUID]),
        CONSTRAINT [CK_Bookings_Dates]      CHECK ([CheckOutDate] > [CheckInDate]),
        CONSTRAINT [CK_Bookings_Guests]     CHECK ([NumberOfGuests] >= 1),
        CONSTRAINT [CK_Bookings_Discount]   CHECK ([DiscountAmount] >= 0),
        CONSTRAINT [CK_Bookings_FinalPrice] CHECK ([FinalPrice] >= 0),
        CONSTRAINT [CK_Bookings_Status]     CHECK ([BookingStatus] IN (
            'Pending', 'Confirmed', 'CheckedIn', 'Completed', 'Cancelled', 'Refunded'
        ))
    )
    GO

    -- ============================================================
    -- BOOKING COUPONS & STATUS HISTORY
    -- ============================================================
    CREATE TABLE [dbo].[BookingCoupons](
        [ID]              [bigint]           IDENTITY(1,1) NOT NULL,
        [GUID]            [uniqueidentifier] NOT NULL DEFAULT NEWID(),
        [BookingID]       [bigint]           NOT NULL,
        [CouponID]        [bigint]           NOT NULL,
        [DiscountApplied] [decimal](10,2)    NOT NULL,
        [AppliedDate]     [datetime]         NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_BookingCoupons]      PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_BookingCoupons_GUID] UNIQUE ([GUID]),
        CONSTRAINT [UQ_BookingCoupons_Pair] UNIQUE ([BookingID], [CouponID])
    )
    GO

    CREATE TABLE [dbo].[BookingStatusHistory](
        [ID]              [bigint]           IDENTITY(1,1) NOT NULL,
        [GUID]            [uniqueidentifier] NOT NULL DEFAULT NEWID(),
        [BookingID]       [bigint]           NOT NULL,
        [OldStatus]       [varchar](20)      NULL,
        [NewStatus]       [varchar](20)      NOT NULL,
        [ChangedDate]     [datetime]         NOT NULL DEFAULT GETDATE(),
        [ChangedByUserID] [bigint]           NULL,
        [Notes]           [nvarchar](500)    NULL,
        CONSTRAINT [PK_BookingStatusHistory]      PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_BookingStatusHistory_GUID] UNIQUE ([GUID]),
        CONSTRAINT [CK_BSH_OldStatus] CHECK ([OldStatus] IS NULL OR [OldStatus] IN (
            'Pending', 'Confirmed', 'CheckedIn', 'Completed', 'Cancelled', 'Refunded'
        )),
        CONSTRAINT [CK_BSH_NewStatus] CHECK ([NewStatus] IN (
            'Pending', 'Confirmed', 'CheckedIn', 'Completed', 'Cancelled', 'Refunded'
        ))
    )
    GO

    -- ============================================================
    -- BOOKING DETAILS (from Session6) – một booking có thể nhiều đêm
    -- ============================================================
    CREATE TABLE [dbo].[BookingDetails](
        [ID]                          [bigint]           IDENTITY(1,1) NOT NULL,
        [GUID]                        [uniqueidentifier] NOT NULL DEFAULT NEWID(),
        [BookingID]                   [bigint]           NOT NULL,
        [ItemPriceID]                 [bigint]           NOT NULL,
        [isRefund]                    [bit]              NOT NULL DEFAULT 0,
        [RefundDate]                  [date]             NULL,
        [RefundCancellationPolicyID]  [bigint]           NULL,
        CONSTRAINT [PK_BookingDetails]        PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_BookingDetails_GUID]   UNIQUE ([GUID])
    )
    GO

    -- ============================================================
    -- REVIEWS (giữ từ v3 – đánh giá chung)
    -- ============================================================
    CREATE TABLE [dbo].[Reviews](
        [ID]          [bigint]           IDENTITY(1,1) NOT NULL,
        [GUID]        [uniqueidentifier] NOT NULL DEFAULT NEWID(),
        [BookingID]   [bigint]           NOT NULL,
        [ReviewerID]  [bigint]           NOT NULL,
        [RevieweeID]  [bigint]           NULL,
        [ItemID]      [bigint]           NULL,
        [Rating]      [tinyint]          NOT NULL,
        [Comment]     [nvarchar](2000)   NULL,
        [CreatedDate] [datetime]         NOT NULL DEFAULT GETDATE(),
        [IsActive]    [bit]              NOT NULL DEFAULT 1,
        CONSTRAINT [PK_Reviews]        PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_Reviews_GUID]   UNIQUE ([GUID]),
        CONSTRAINT [CK_Reviews_Rating] CHECK ([Rating] BETWEEN 1 AND 5),
        CONSTRAINT [CK_Reviews_NotSelf] CHECK (
            [ReviewerID] <> [RevieweeID] OR [RevieweeID] IS NULL
        ),
        CONSTRAINT [CK_Reviews_Target] CHECK (
            ([RevieweeID] IS NOT NULL AND [ItemID] IS NULL) OR
            ([RevieweeID] IS NULL     AND [ItemID] IS NOT NULL)
        )
    )
    GO

    -- ============================================================
    -- SERVICE TYPES & SERVICES (from Session6)
    -- ============================================================
    CREATE TABLE [dbo].[ServiceTypes](
        [ID]          [bigint]           IDENTITY(1,1) NOT NULL,
        [GUID]        [uniqueidentifier] NOT NULL DEFAULT NEWID(),
        [Name]        [nvarchar](150)    NOT NULL,
        [IconName]    [nvarchar](50)     NULL,
        [Description] [nvarchar](250)    NULL,
        CONSTRAINT [PK_ServiceTypes]      PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_ServiceTypes_GUID] UNIQUE ([GUID])
    )
    GO

    CREATE TABLE [dbo].[Services](
        [ID]            [bigint]           IDENTITY(1,1) NOT NULL,
        [GUID]          [uniqueidentifier] NOT NULL DEFAULT NEWID(),
        [ServiceTypeID] [bigint]           NOT NULL,
        [Name]          [nvarchar](150)    NOT NULL,
        [Price]         [decimal](10,2)    NOT NULL,
        [Duration]      [bigint]           NULL,
        [Description]   [nvarchar](1000)   NULL,
        [DayOfWeek]     [nvarchar](100)    NULL,
        [DayOfMonth]    [nvarchar](100)    NULL,
        [DailyCap]      [bigint]           NOT NULL DEFAULT 1,
        [BookingCap]    [bigint]           NOT NULL DEFAULT 1,
        CONSTRAINT [PK_Services]      PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_Services_GUID] UNIQUE ([GUID])
    )
    GO

    -- ============================================================
    -- ADDON SERVICES & ADDON SERVICE DETAILS (from Session6)
    -- ============================================================
    CREATE TABLE [dbo].[AddonServices](
        [ID]        [bigint]           IDENTITY(1,1) NOT NULL,
        [GUID]      [uniqueidentifier] NOT NULL DEFAULT NEWID(),
        [UserID]    [bigint]           NOT NULL,
        [BookingID] [bigint]           NULL,
        [CouponID]  [bigint]           NULL,
        CONSTRAINT [PK_AddonServices]      PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_AddonServices_GUID] UNIQUE ([GUID])
    )
    GO

    CREATE TABLE [dbo].[AddonServiceDetails](
        [ID]              [bigint]           IDENTITY(1,1) NOT NULL,
        [GUID]            [uniqueidentifier] NOT NULL DEFAULT NEWID(),
        [AddonServiceID]  [bigint]           NOT NULL,
        [ServiceID]       [bigint]           NOT NULL,
        [Price]           [decimal](10,2)    NOT NULL,
        [FromDate]        [datetime]         NOT NULL,
        [Notes]           [nvarchar](250)    NULL,
        [NumberOfPeople]  [bigint]           NOT NULL DEFAULT 1,
        [isRefund]        [bit]              NOT NULL DEFAULT 0,
        CONSTRAINT [PK_AddonServiceDetails]      PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [UQ_AddonServiceDetails_GUID] UNIQUE ([GUID])
    )
    GO
    -- ============================================================
    -- INVOICE TO GENERATE BILLING
    -- ============================================================
    CREATE TABLE [dbo].[Invoices] (
    [ID]            BIGINT           IDENTITY(1,1) NOT NULL,
    [GUID]          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [BookingID]     BIGINT           NOT NULL,
    [InvoiceCode]   NVARCHAR(50)     NOT NULL,  -- INV-000001
    [IssueDate]     DATETIME         NOT NULL DEFAULT GETDATE(),
    [DueDate]       DATETIME         NULL,
    [Status]        VARCHAR(20)      NOT NULL DEFAULT 'Pending',
    -- CHECK: 'Pending', 'Paid', 'Cancelled', 'Expired'
    [TotalAmount]   DECIMAL(10,2)    NOT NULL,
    [PaidDate]      DATETIME         NULL,
    [Notes]         NVARCHAR(500)    NULL,
    CONSTRAINT [PK_Invoices]         PRIMARY KEY ([ID]),
    CONSTRAINT [UQ_Invoices_GUID]    UNIQUE ([GUID]),
    CONSTRAINT [UQ_Invoices_Code]    UNIQUE ([InvoiceCode]),
    CONSTRAINT [UQ_Invoices_Booking] UNIQUE ([BookingID]),  -- 1 booking = 1 invoice
    CONSTRAINT [FK_Invoices_Booking] FOREIGN KEY ([BookingID]) REFERENCES [Bookings]([ID]),
    CONSTRAINT [CK_Invoices_Status]  CHECK ([Status] IN ('Pending','Paid','Cancelled','Expired'))
    )
    GO
    -- ============================================================
    -- FOREIGN KEY CONSTRAINTS
    -- ============================================================
    ALTER TABLE [dbo].[Attractions] ADD CONSTRAINT [FK_Attractions_Areas] FOREIGN KEY ([AreaID]) REFERENCES [dbo].[Areas]([ID])
    GO
    ALTER TABLE [dbo].[CancellationRefundFees] ADD CONSTRAINT [FK_CancellationRefundFees_CancellationPolicies] FOREIGN KEY ([CancellationPolicyID]) REFERENCES [dbo].[CancellationPolicies]([ID]) ON DELETE CASCADE ON UPDATE CASCADE
    GO
    ALTER TABLE [dbo].[Guests] ADD CONSTRAINT [FK_Guests_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users]([ID]) ON DELETE CASCADE
    GO
    ALTER TABLE [dbo].[Hosts] ADD CONSTRAINT [FK_Hosts_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users]([ID]) ON DELETE CASCADE
    GO
    ALTER TABLE [dbo].[HostBankAccounts] ADD CONSTRAINT [FK_HostBankAccounts_Hosts] FOREIGN KEY ([HostUserID]) REFERENCES [dbo].[Hosts]([UserID])
    GO
    ALTER TABLE [dbo].[Items] ADD CONSTRAINT [FK_Items_Hosts] FOREIGN KEY ([HostUserID]) REFERENCES [dbo].[Hosts]([UserID])
    GO
    ALTER TABLE [dbo].[Items] ADD CONSTRAINT [FK_Items_Areas] FOREIGN KEY ([AreaID]) REFERENCES [dbo].[Areas]([ID])
    GO
    ALTER TABLE [dbo].[Items] ADD CONSTRAINT [FK_Items_ItemTypes] FOREIGN KEY ([ItemTypeID]) REFERENCES [dbo].[ItemTypes]([ID])
    GO
    ALTER TABLE [dbo].[ItemAmenities] ADD CONSTRAINT [FK_ItemAmenities_Items] FOREIGN KEY ([ItemID]) REFERENCES [dbo].[Items]([ID]) ON DELETE CASCADE
    GO
    ALTER TABLE [dbo].[ItemAmenities] ADD CONSTRAINT [FK_ItemAmenities_Amenities] FOREIGN KEY ([AmenityID]) REFERENCES [dbo].[Amenities]([ID])
    GO
    ALTER TABLE [dbo].[ItemAttractions] ADD CONSTRAINT [FK_ItemAttractions_Items] FOREIGN KEY ([ItemID]) REFERENCES [dbo].[Items]([ID]) ON DELETE CASCADE
    GO
    ALTER TABLE [dbo].[ItemAttractions] ADD CONSTRAINT [FK_ItemAttractions_Attractions] FOREIGN KEY ([AttractionID]) REFERENCES [dbo].[Attractions]([ID])
    GO
    ALTER TABLE [dbo].[ItemPictures] ADD CONSTRAINT [FK_ItemPictures_Items] FOREIGN KEY ([ItemID]) REFERENCES [dbo].[Items]([ID]) ON DELETE CASCADE
    GO
    ALTER TABLE [dbo].[ItemPrices] ADD CONSTRAINT [FK_ItemPrices_Items] FOREIGN KEY ([ItemID]) REFERENCES [dbo].[Items]([ID]) ON DELETE CASCADE
    GO
    ALTER TABLE [dbo].[ItemPrices] ADD CONSTRAINT [FK_ItemPrices_CancellationPolicies] FOREIGN KEY ([CancellationPolicyID]) REFERENCES [dbo].[CancellationPolicies]([ID])
    GO
    ALTER TABLE [dbo].[ItemPrices] ADD CONSTRAINT [FK_ItemPrices_DimDates] FOREIGN KEY ([Date]) REFERENCES [dbo].[DimDates]([Date])
    GO
    ALTER TABLE [dbo].[ItemAvailability] ADD CONSTRAINT [FK_ItemAvailability_Items] FOREIGN KEY ([ItemID]) REFERENCES [dbo].[Items]([ID]) ON DELETE CASCADE
    GO
    ALTER TABLE [dbo].[ItemAvailability] ADD CONSTRAINT [FK_ItemAvailability_DimDates] FOREIGN KEY ([Date]) REFERENCES [dbo].[DimDates]([Date])
    GO
    ALTER TABLE [dbo].[Transactions] ADD CONSTRAINT [FK_Transactions_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users]([ID])
    GO
    ALTER TABLE [dbo].[Transactions] ADD CONSTRAINT [FK_Transactions_TransactionTypes] FOREIGN KEY ([TransactionTypeID]) REFERENCES [dbo].[TransactionTypes]([ID])
    GO
    ALTER TABLE [dbo].[Transactions] ADD CONSTRAINT [FK_Transactions_DimDates] FOREIGN KEY ([TransactionDate]) REFERENCES [dbo].[DimDates]([Date])
    GO

    -- Bookings
    ALTER TABLE [dbo].[Bookings] ADD CONSTRAINT [FK_Bookings_Guests] FOREIGN KEY ([GuestUserID]) REFERENCES [dbo].[Guests]([UserID])
    GO
    ALTER TABLE [dbo].[Bookings] ADD CONSTRAINT [FK_Bookings_Items] FOREIGN KEY ([ItemID]) REFERENCES [dbo].[Items]([ID])
    GO
    ALTER TABLE [dbo].[Bookings] ADD CONSTRAINT [FK_Bookings_CancellationPolicies] FOREIGN KEY ([CancellationPolicyID]) REFERENCES [dbo].[CancellationPolicies]([ID])
    GO
    ALTER TABLE [dbo].[Bookings] ADD CONSTRAINT [FK_Bookings_Transactions] FOREIGN KEY ([TransactionID]) REFERENCES [dbo].[Transactions]([ID])
    GO
    ALTER TABLE [dbo].[Bookings] ADD CONSTRAINT [FK_Bookings_CheckInDate_DimDates] FOREIGN KEY ([CheckInDate]) REFERENCES [dbo].[DimDates]([Date])
    GO
    ALTER TABLE [dbo].[Bookings] ADD CONSTRAINT [FK_Bookings_CheckOutDate_DimDates] FOREIGN KEY ([CheckOutDate]) REFERENCES [dbo].[DimDates]([Date])
    GO

    ALTER TABLE [dbo].[BookingCoupons] ADD CONSTRAINT [FK_BookingCoupons_Bookings] FOREIGN KEY ([BookingID]) REFERENCES [dbo].[Bookings]([ID])
    GO
    ALTER TABLE [dbo].[BookingCoupons] ADD CONSTRAINT [FK_BookingCoupons_Coupons] FOREIGN KEY ([CouponID]) REFERENCES [dbo].[Coupons]([ID])
    GO
    ALTER TABLE [dbo].[BookingStatusHistory] ADD CONSTRAINT [FK_BookingStatusHistory_Bookings] FOREIGN KEY ([BookingID]) REFERENCES [dbo].[Bookings]([ID])
    GO
    ALTER TABLE [dbo].[BookingStatusHistory] ADD CONSTRAINT [FK_BookingStatusHistory_ChangedByUser] FOREIGN KEY ([ChangedByUserID]) REFERENCES [dbo].[Users]([ID])
    GO

    -- BookingDetails (new from Session6)
    ALTER TABLE [dbo].[BookingDetails] ADD CONSTRAINT [FK_BookingDetails_Bookings] FOREIGN KEY ([BookingID]) REFERENCES [dbo].[Bookings]([ID])
    GO
    ALTER TABLE [dbo].[BookingDetails] ADD CONSTRAINT [FK_BookingDetails_ItemPrices] FOREIGN KEY ([ItemPriceID]) REFERENCES [dbo].[ItemPrices]([ID])
    GO
    ALTER TABLE [dbo].[BookingDetails] ADD CONSTRAINT [FK_BookingDetails_RefundPolicy] FOREIGN KEY ([RefundCancellationPolicyID]) REFERENCES [dbo].[CancellationPolicies]([ID])
    GO

    -- Reviews
    ALTER TABLE [dbo].[Reviews] ADD CONSTRAINT [FK_Reviews_Bookings] FOREIGN KEY ([BookingID]) REFERENCES [dbo].[Bookings]([ID])
    GO
    ALTER TABLE [dbo].[Reviews] ADD CONSTRAINT [FK_Reviews_Reviewer] FOREIGN KEY ([ReviewerID]) REFERENCES [dbo].[Users]([ID])
    GO
    ALTER TABLE [dbo].[Reviews] ADD CONSTRAINT [FK_Reviews_Reviewee] FOREIGN KEY ([RevieweeID]) REFERENCES [dbo].[Users]([ID])
    GO
    ALTER TABLE [dbo].[Reviews] ADD CONSTRAINT [FK_Reviews_Items] FOREIGN KEY ([ItemID]) REFERENCES [dbo].[Items]([ID])
    GO

    -- ItemScores (from Session6)
    ALTER TABLE [dbo].[ItemScores] ADD CONSTRAINT [FK_ItemScores_Items] FOREIGN KEY ([ItemID]) REFERENCES [dbo].[Items]([ID])
    GO
    ALTER TABLE [dbo].[ItemScores] ADD CONSTRAINT [FK_ItemScores_Scores] FOREIGN KEY ([ScoreID]) REFERENCES [dbo].[Scores]([ID])
    GO
    ALTER TABLE [dbo].[ItemScores] ADD CONSTRAINT [FK_ItemScores_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users]([ID])
    GO

    -- Services
    ALTER TABLE [dbo].[Services] ADD CONSTRAINT [FK_Services_ServiceTypes] FOREIGN KEY ([ServiceTypeID]) REFERENCES [dbo].[ServiceTypes]([ID])
    GO
    ALTER TABLE [dbo].[AddonServices] ADD CONSTRAINT [FK_AddonServices_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users]([ID])
    GO
    ALTER TABLE [dbo].[AddonServices] ADD CONSTRAINT [FK_AddonServices_Bookings] FOREIGN KEY ([BookingID]) REFERENCES [dbo].[Bookings]([ID])
    GO
    ALTER TABLE [dbo].[AddonServices] ADD CONSTRAINT [FK_AddonServices_Coupons] FOREIGN KEY ([CouponID]) REFERENCES [dbo].[Coupons]([ID])
    GO
    ALTER TABLE [dbo].[AddonServiceDetails] ADD CONSTRAINT [FK_AddonServiceDetails_AddonServices] FOREIGN KEY ([AddonServiceID]) REFERENCES [dbo].[AddonServices]([ID])
    GO
    ALTER TABLE [dbo].[AddonServiceDetails] ADD CONSTRAINT [FK_AddonServiceDetails_Services] FOREIGN KEY ([ServiceID]) REFERENCES [dbo].[Services]([ID])
    GO

    -- ============================================================
    -- VIEWS
    -- ============================================================
    CREATE VIEW [dbo].[vw_UserRoles] AS
    SELECT
        u.[ID]         AS UserID,
        u.[Username],
        u.[FullName],
        u.[Email],
        u.[IsAdmin],
        CASE WHEN h.[UserID] IS NOT NULL THEN 1 ELSE 0 END AS IsHost,
        CASE WHEN g.[UserID] IS NOT NULL THEN 1 ELSE 0 END AS IsGuest,
        h.[IsVerified]      AS HostVerified,
        h.[Rating]          AS HostRating,
        g.[LoyaltyPoints]   AS GuestLoyaltyPoints
    FROM [dbo].[Users] u
    LEFT JOIN [dbo].[Hosts]  h ON h.[UserID] = u.[ID]
    LEFT JOIN [dbo].[Guests] g ON g.[UserID] = u.[ID]
    GO

    CREATE VIEW [dbo].[vw_GuestBookingSummary] AS
    SELECT
        g.[UserID],
        u.[FullName],
        g.[LoyaltyPoints],
        COUNT(b.[ID])                                         AS TotalBookings,
        SUM(CASE WHEN b.[BookingStatus] = 'Completed'
                THEN 1 ELSE 0 END)                           AS CompletedBookings,
        SUM(CASE WHEN b.[BookingStatus] = 'Cancelled'
                THEN 1 ELSE 0 END)                           AS CancelledBookings,
        ISNULL(SUM(CASE WHEN b.[BookingStatus] = 'Completed'
                        THEN b.[FinalPrice] END), 0)           AS TotalSpent
    FROM [dbo].[Guests] g
    JOIN [dbo].[Users]  u ON u.[ID] = g.[UserID]
    LEFT JOIN [dbo].[Bookings] b ON b.[GuestUserID] = g.[UserID]
    GROUP BY g.[UserID], u.[FullName], g.[LoyaltyPoints]
    GO

    CREATE VIEW [dbo].[vw_HostPerformance] AS
    SELECT
        h.[UserID]              AS HostUserID,
        u.[FullName],
        h.[IsVerified],
        h.[Rating],
        h.[TotalReviews],
        COUNT(DISTINCT i.[ID])  AS TotalListings,
        COUNT(DISTINCT b.[ID])  AS TotalBookingsReceived,
        ISNULL(SUM(CASE WHEN b.[BookingStatus] = 'Completed'
                        THEN b.[FinalPrice] END), 0) AS TotalRevenue
    FROM [dbo].[Hosts] h
    JOIN [dbo].[Users] u   ON u.[ID]         = h.[UserID]
    LEFT JOIN [dbo].[Items] i ON i.[HostUserID] = h.[UserID]
    LEFT JOIN [dbo].[Bookings] b ON b.[ItemID]  = i.[ID]
    GROUP BY h.[UserID], u.[FullName], h.[IsVerified], h.[Rating], h.[TotalReviews]
    GO

    -- ============================================================
    -- SAMPLE DATA
    -- ============================================================

    -- DimDates (2022–2027)
    DECLARE @d DATE = '2022-01-01'
    DECLARE @end DATE = '2027-12-31'
    WHILE @d <= @end
    BEGIN
        INSERT INTO [dbo].[DimDates] ([ID],[Date],[Year],[Quarter],[Month],[MonthName],[DayOfMonth],[DayOfWeek],[DayName],[IsHoliday])
        VALUES (
            CAST(FORMAT(@d,'yyyyMMdd') AS bigint), @d,
            YEAR(@d), DATEPART(QUARTER,@d), MONTH(@d), DATENAME(MONTH,@d),
            DAY(@d), DATEPART(WEEKDAY,@d), DATENAME(WEEKDAY,@d), 0
        )
        SET @d = DATEADD(DAY,1,@d)
    END
    GO

    -- ItemTypes
    SET IDENTITY_INSERT [dbo].[ItemTypes] ON
    INSERT [dbo].[ItemTypes] ([ID],[GUID],[Name],[Icon],[Description]) VALUES
    (1, N'60b72778-02fd-4602-a7a7-84f9ae17a6c2', N'Apartment',     N'icon-apartment.png',     N'A self-contained housing unit that occupies part of a building.'),
    (2, N'dffe3bca-92b1-4760-8371-1cf145fd772c', N'House',          N'icon-house.png',          N'A standalone residential building with private entrance and yard.'),
    (3, N'e62aa132-bcc2-40df-872f-e553ba156acb', N'Secondary unit', N'icon-secondary-unit.png', N'A smaller, secondary dwelling unit on the same lot as a main house.'),
    (4, N'5dc77099-57fe-4000-b321-63ba98194d66', N'Unique space',   N'icon-unique-space.png',   N'An unusual or creative space such as a cabin, treehouse, or converted building.'),
    (5, N'727443d0-d486-4d3b-a3bb-2af3abe39dd7', N'Boutique hotel', N'icon-boutique-hotel.png', N'A small, stylish hotel with individually decorated rooms and personalized service.')
    SET IDENTITY_INSERT [dbo].[ItemTypes] OFF
    GO

    -- TransactionTypes
    SET IDENTITY_INSERT [dbo].[TransactionTypes] ON
    INSERT [dbo].[TransactionTypes] ([ID],[GUID],[Name]) VALUES
    (1, N'aa000001-0000-0000-0000-000000000001', N'Payment'),
    (2, N'aa000002-0000-0000-0000-000000000002', N'Refund'),
    (3, N'aa000003-0000-0000-0000-000000000003', N'Payout')
    SET IDENTITY_INSERT [dbo].[TransactionTypes] OFF
    GO

    -- Areas
    SET IDENTITY_INSERT [dbo].[Areas] ON
    INSERT [dbo].[Areas] ([ID],[GUID],[Name]) VALUES
    (1,  N'2c64db74-3d74-46f6-bf61-2e6869e49a59', N'Dobong-gu'),
    (2,  N'4f7bbac7-6d44-4662-bcd5-174bec2e0d59', N'Dongdaemun-gu'),
    (3,  N'e65838a5-3e1c-4631-8909-d76601d9d859', N'Eunpyeong-gu'),
    (4,  N'417668a8-6796-41cb-bc7a-687c66cd06eb', N'Gangbuk-gu'),
    (5,  N'4306fc47-6862-4979-900d-9f3fd3d727c9', N'Gangdong-gu'),
    (6,  N'6073b706-c1f4-4dde-94d4-4cfd7e9033ff', N'Gangnam-gu'),
    (7,  N'f34a7e3c-be5b-4eaf-8b60-a01c02cbcf42', N'Gangseo-gu'),
    (8,  N'c75476cc-36f0-4047-9f76-3c679d9f7720', N'Geumcheon-gu'),
    (9,  N'40ba3129-4fbf-4ac4-88f7-5a7f7b7682fe', N'Guro-gu'),
    (10, N'21117c16-473f-44a8-927b-afa8f5e5a578', N'Jongno-gu'),
    (11, N'0191510c-86d9-4b7c-8c37-53897b89a51c', N'Jung-gu'),
    (12, N'c826d5c8-353a-43e7-a8db-e596785a5709', N'Jungnang-gu'),
    (13, N'e127ad40-e033-48bc-a37c-4fb7d49fdd65', N'Mapo-gu'),
    (14, N'63a610a1-5de4-4c09-b113-39766145bb17', N'Nowon-gu'),
    (15, N'3723c5bc-0643-4526-8f14-d21fb36275cc', N'Seocho-gu'),
    (16, N'89f6e1fd-7e66-4716-a6a9-e6535cd77129', N'Seodaemun-gu'),
    (17, N'0cebd35e-77d5-4d3f-bf2d-61e82fa4688b', N'Seongbuk-gu'),
    (18, N'5a1a36c0-5111-4701-a4cc-5f3d4dff2f71', N'Seongdong-gu'),
    (19, N'df0af61b-7e9e-4bd5-84dc-6d5f448bbd52', N'Songpa-gu'),
    (20, N'f91550e6-21a0-4841-a0e7-98cd24945263', N'Yangcheon-gu'),
    (21, N'37211b04-ea72-45f0-9cc8-e5ba28f51943', N'Yeongdeungpo-gu'),
    (22, N'4a12b92d-5965-4784-a5ce-87ce3e0ada03', N'Yongsan-gu'),
    (23, N'bb000023-0000-0000-0000-000000000023', N'Gwanak-gu'),
    (24, N'bb000024-0000-0000-0000-000000000024', N'Gwangjin-gu'),
    (25, N'bb000025-0000-0000-0000-000000000025', N'Dongjak-gu')
    SET IDENTITY_INSERT [dbo].[Areas] OFF
    GO

    -- Amenities
    SET IDENTITY_INSERT [dbo].[Amenities] ON
    INSERT [dbo].[Amenities] ([ID],[GUID],[Name],[IconName]) VALUES
    (1,  N'37647c60-15aa-4a41-8af3-2583496919c9', N'Parking',            N'001-home.png'),
    (2,  N'488716b5-9323-4335-85eb-e17b78b5ba64', N'Free Wifi',           N'028-connection.png'),
    (3,  N'af861aa0-3828-4845-b013-2e0bb33a36e2', N'Room Service',        N'274-checkmark2.png'),
    (4,  N'55091928-b7e2-4bd4-a42b-f80a94e4e1dc', N'Breakfast',           N'164-spoon-knife.png'),
    (5,  N'0f502995-17f2-4f4d-8c80-7545f6b93ec7', N'Hair Styling Tools',  N'347-scissors.png'),
    (6,  N'62161419-a17b-49f9-bca8-a9063bd78b21', N'Sound System',        N'295-volume-high.png'),
    (7,  N'4db627db-e0d8-4d7c-8549-5ace684b706f', N'Safety Box',          N'143-key2.png'),
    (8,  N'129418ae-b767-4abf-9283-063f14c3fe86', N'Business Facilities', N'085-printer.png'),
    (9,  N'271b3a3f-eb49-4fe9-b8a0-f61337cfc343', N'Laundry Services',    N'341-radio-checked.png'),
    (10, N'dbf027c9-8aae-4990-a5ce-01b319d80fe1', N'Daily Newspaper',     N'005-newspaper.png'),
    (11, N'd5010dae-314a-4cd8-bf6a-fbc53a8005d1', N'Entertainment',       N'018-music.png')
    SET IDENTITY_INSERT [dbo].[Amenities] OFF
    GO

    -- Attractions (chỉ lấy 22 dòng mẫu như v3, vì dữ liệu Session6 trùng đến 99 dòng)
    SET IDENTITY_INSERT [dbo].[Attractions] ON
    INSERT [dbo].[Attractions] ([ID],[GUID],[AreaID],[Name],[Address]) VALUES
    (1,  N'670c374a-5fe5-4065-9f1e-18ed5d7232d6',  1, N'Dobongsan',               N'서울특별시 도봉구 도봉동'),
    (2,  N'c21b75a4-089b-4f5b-bc24-fd89139dbf08',  2, N'Cheongnyangni Station',   N'서울특별시 동대문구 전농동'),
    (3,  N'500919eb-eb16-4456-816e-7fe606a8b2e4',  2, N'Gyeongdong Market',       N'서울특별시 동대문구 제기동'),
    (4,  N'eb14eefb-e3b3-45f2-96ef-c9199d043cd3',  2, N'Hongneung Park',          N'서울특별시 동대문구 청량리동'),
    (5,  N'436f48e0-27e8-48c5-b25e-d1a1bb7d5ef9',  3, N'Bongsan Park',            N'서울특별시 은평구 봉산동'),
    (6,  N'5124200e-dff6-40b8-a017-b93d69c18673',  3, N'Gupabal Fall',            N'서울특별시 은평구 구파발동'),
    (7,  N'571ceb35-139a-42bf-bde0-ef892b820428',  4, N'Bukhansan National Park', N'서울특별시 강북구 우이동'),
    (8,  N'e1bab1d7-1b86-46a4-a6d2-0888ccbc0f72',  4, N'Dream Forest',            N'서울특별시 강북구 번동'),
    (9,  N'43ccbfce-9485-4cf1-aff0-8ccf1e23fa21',  5, N'Cheonhodong Park',        N'서울특별시 강동구 천호동'),
    (10, N'3d52550e-2722-4205-a259-4a076988dcbd',  5, N'Dunchun-dong Marsh',      N'서울특별시 강동구 둔촌동'),
    (11, N'48be80bf-b6e7-4f80-b5d6-60195fa2490f',  5, N'Kildong Ecological Park', N'서울특별시 강동구 길동'),
    (12, N'c787224d-d300-444a-9cda-b0a57774fd61',  5, N'Saetmaeul Park',          N'서울특별시 강동구 암사동'),
    (13, N'05fbe1c9-ce6b-49f5-a907-4930b68a8459',  6, N'Bongeunsa Temple',        N'서울특별시 강남구 삼성동'),
    (14, N'9f7ebf02-134d-406b-99a2-8070b57c4cab',  6, N'COEX Mall',               N'서울특별시 강남구 삼성동'),
    (15, N'68a11502-6f31-4d0b-823e-9bb00078013a',  6, N'Dosan Park',              N'서울특별시 강남구 신사동'),
    (16, N'b12281c8-6674-4bc4-964c-08a42962ce34',  6, N'Gangnam Station',         N'서울특별시 강남구 역삼동'),
    (17, N'b3c593e9-1e54-4e2e-a1a7-7f289213d988',  6, N'Garosu-gil',              N'서울특별시 강남구 신사동'),
    (18, N'37a5d8d7-6c76-47ab-8afe-c2fc248b8469',  6, N'Kukkiwon',                N'서울특별시 강남구 역삼동'),
    (19, N'de4533ae-571f-4ea6-9469-b55548e1215b',  6, N'Teheranno',               N'서울특별시 강남구 테헤란로'),
    (20, N'763d45bc-035c-4bfc-92cf-2b19c63b6c94',  6, N'Yangjaecheon',            N'서울특별시 강남구 양재동'),
    (21, N'08be9b58-c945-4913-b80f-4600320d6823',  7, N'Gimpo International Airport', N'서울특별시 강서구 공항동'),
    (22, N'1c4f1f28-e448-4cf6-95a0-c64ad3539fe7',  7, N'Heojun Museum',           N'서울특별시 강서구 가양동')
    SET IDENTITY_INSERT [dbo].[Attractions] OFF
    GO

    -- CancellationPolicies
    SET IDENTITY_INSERT [dbo].[CancellationPolicies] ON
    INSERT [dbo].[CancellationPolicies] ([ID],[GUID],[Name],[PlatformCommissionRate]) VALUES
    (1, N'6cc083f6-c039-4ac2-acb2-12a0058b0f73', N'Flexible',  7.00),
    (2, N'490c3c20-8974-4e69-96bf-3066c25d06ba', N'Moderate', 15.00),
    (3, N'7bfa6503-471f-45c1-ab22-796db8f9d71d', N'Strict',   25.00)
    SET IDENTITY_INSERT [dbo].[CancellationPolicies] OFF
    GO

    -- CancellationRefundFees
    SET IDENTITY_INSERT [dbo].[CancellationRefundFees] ON
    INSERT [dbo].[CancellationRefundFees] ([ID],[GUID],[CancellationPolicyID],[DaysLeft],[PenaltyPercentage]) VALUES
    (1,  N'a036741a-5582-44a0-a20d-5484ce3fa570', 1,7,  5.00),(2,  N'4d5e9b0f-f751-42af-a00a-2423503d857d', 1,6, 10.00),
    (3,  N'335c5e75-0dc6-4a80-9b35-647b37e4bafc', 1,5, 15.00),(4,  N'372e4722-c717-4a05-8ecb-a4dfedd91bff', 1,4, 15.00),
    (5,  N'844a1700-c757-4b61-90bb-9450f94cb421', 1,3, 20.00),(6,  N'c67f3725-ba45-4f72-9b1e-b9cced1b1ac0', 1,2, 20.00),
    (7,  N'67082379-db2f-4d21-a9b8-fc8fdf53b0e9', 1,1, 30.00),(8,  N'ea887a9b-ccb4-4593-ba4f-63fa62606c1e', 1,0, 40.00),
    (9,  N'36bd39c8-4c11-4a28-ab04-0a87562bd3bf', 2,7, 10.00),(10, N'34336eec-c4de-4a73-8e9f-7577e133e9a0', 2,6, 15.00),
    (11, N'3341968c-c849-4d06-a558-0061e01ef7fc', 2,5, 20.00),(12, N'9da45846-7d13-4322-a763-712189cefdc8', 2,4, 20.00),
    (13, N'410f8f56-12f4-4279-a7a8-420a8e7b60ad', 2,3, 30.00),(14, N'06dff31f-6fbb-4fc4-8988-ce6a2747174b', 2,2, 35.00),
    (15, N'cdf6b818-8052-4ca4-91eb-84a566b92eba', 2,1, 40.00),(16, N'754a6481-5d39-4b36-b370-4c7220f5aa06', 2,0, 45.00),
    (17, N'725341c0-538b-44c1-b02a-92914a67ca42', 3,7, 20.00),(18, N'14ed982b-9ea1-4d5d-aa75-eaf86ff3bdc7', 3,6, 25.00),
    (19, N'8f9b8930-b65c-44b5-bb78-5fbae6afea29', 3,5, 30.00),(20, N'9a5f70cb-befb-4d4b-a974-37757866f862', 3,4, 35.00),
    (21, N'4299d7aa-d692-4dac-b952-0a9caae24bdc', 3,3, 40.00),(22, N'55ab294f-d38c-47e1-8f3c-830b8f26cf56', 3,2, 50.00),
    (23, N'fa09e103-f8d1-4863-bdb8-fb8cb26be048', 3,1, 90.00),(24, N'1fc75f78-462a-4436-9a86-96a419a9db44', 3,0,100.00)
    SET IDENTITY_INSERT [dbo].[CancellationRefundFees] OFF
    GO

    -- Coupons
    SET IDENTITY_INSERT [dbo].[Coupons] ON
    INSERT [dbo].[Coupons] ([ID],[GUID],[CouponCode],[DiscountPercent],[MaximumDiscountAmount],
                            [StartDate],[ExpirationDate],[MaxUsageCount],[IsActive]) VALUES
    (1, N'cf3292fe-8636-400f-8c3f-2b5c5551f004', N'Welcome15', 15.0, 10.00,  '2022-01-01','2027-12-31', NULL, 1),
    (2, N'0ce0b94f-112c-42dc-a110-314c5ba1308b', N'Holiday',    5.0, 100.00, '2022-01-01','2027-12-31',  500, 1),
    (3, N'4a33cb47-ac38-48f8-88d2-97f103ad75a7', N'Seoul',      2.0, 200.00, '2022-01-01', NULL,         NULL, 1)
    SET IDENTITY_INSERT [dbo].[Coupons] OFF
    GO

    -- Users
    SET IDENTITY_INSERT [dbo].[Users] ON
    INSERT [dbo].[Users] ([ID],[GUID],[Username],[Password],[FullName],[Email],
                        [PhoneNumber],[Gender],[BirthDate],[Country],[IsAdmin],[IsActive]) VALUES
    (1,  N'3fa2814d-a355-4169-9a23-b3476f8c7083', N'sonia1980', N'1980',   N'Sonia Edvard',
        N'sonia@seoulstay.com',    N'+82-10-0001-0001', 2, '1980-12-02', N'Korea',    1, 1),
    (2,  N'b36658f2-e2e7-4a89-a60f-0861b77640cb', N'sirvard',   N'9090',   N'Nerses Sirvard',
        N'sirvard@email.com',      N'+82-10-0002-0002', 1, '1975-01-01', N'Armenia',  0, 1),
    (3,  N'2fd71d03-5d51-4015-a4b2-2f337692d919', N'mahdi',     N'1234',   N'Mahdi Jokar',
        N'mahdi@email.com',        N'+82-10-0003-0003', 1, '1985-02-03', N'Iran',     0, 1),
    (4,  N'd049f07f-85dd-4e2c-b0ae-7f3339f75483', N'minseo',    N'7890',   N'Min-Seo Young-Ho',
        N'minseo@email.com',       N'+82-10-0004-0004', 2, '1990-04-04', N'Korea',    0, 1),
    (6,  N'64fdd587-2976-4e62-aa4d-341cfcdfcba2', N'minju',     N'0000',   N'Minju Olp',
        N'minju@email.com',        N'+82-10-0006-0006', 2, '1991-05-06', N'Korea',    0, 1),
    (7,  N'3fe65e94-4f44-4c15-b88f-549a92e832b4', N'bayan',     N'9580',   N'Bayan Karim',
        N'bayan@email.com',        N'+1-555-0007',      2, '1992-09-09', N'USA',      0, 1),
    (8,  N'cc000008-0000-0000-0000-000000000008', N'tomkim',    N'pass01',  N'Tom Kim',
        N'tom@email.com',          N'+82-10-0008-0008', 1, '1995-11-05', N'Korea',    0, 1),
    (9,  N'cc000009-0000-0000-0000-000000000009', N'saralee',   N'pass02',  N'Sara Lee',
        N'sara@email.com',         N'+44-20-0009-0009', 2, '1993-04-18', N'UK',       0, 1),
    (10, N'cc000010-0000-0000-0000-000000000010', N'jiwon',     N'pass03',  N'Park Jiwon',
        N'jiwon@email.com',        N'+82-10-0010-0010', 2, '1997-08-20', N'Korea',    0, 1),
    (11, N'cc000011-0000-0000-0000-000000000011', N'yusuf',     N'pass04',  N'Yusuf Al-Rashid',
        N'yusuf@email.com',        N'+971-50-0011',     1, '1988-03-12', N'UAE',      0, 1),
    (12, N'cc000012-0000-0000-0000-000000000012', N'claire',    N'pass05',  N'Claire Dubois',
        N'claire@email.com',       N'+33-6-0012-0012',  2, '1994-07-30', N'France',   0, 1),
    (13, N'cc000013-0000-0000-0000-000000000013', N'alexwang',  N'pass06',  N'Alex Wang',
        N'alexwang@email.com',     N'+86-138-0013',     1, '1991-12-15', N'China',    0, 1)
    SET IDENTITY_INSERT [dbo].[Users] OFF
    GO

    -- Hosts
    INSERT [dbo].[Hosts] ([UserID],[BusinessLicense],[TaxCode],[IsVerified],[VerifiedDate],[Rating],[TotalReviews]) VALUES
    (2,  NULL,              NULL,            1, '2022-06-01', 4.80, 42),
    (3,  N'BL-2022-00123', N'TC-202200123', 1, '2022-07-15', 4.65, 87),
    (4,  NULL,              NULL,            1, '2022-08-01', 4.90, 23),
    (6,  N'BL-2021-00456', N'TC-202100456', 1, '2022-05-10', 4.70, 61)
    GO

    -- Guests
    INSERT [dbo].[Guests] ([UserID],[LoyaltyPoints],[PreferredLanguage],[NationalID],[NationalIDVerified]) VALUES
    (2,  1200, 'hy', NULL, 0),
    (3,  800,  'fa', NULL, 0),
    (4,  950,  'ko', N'KR-1990-04-04-MINSEO', 1),
    (6,  400,  'ko', N'KR-1991-05-06-MINJU',  1),
    (7,  200,  'en', NULL, 0),
    (8,  150,  'ko', N'KR-1995-11-05-TOMKIM', 1),
    (9,  300,  'en', NULL, 0),
    (10, 500,  'ko', N'KR-1997-08-20-JIWON',  1),
    (11, 75,   'ar', NULL, 0),
    (12, 250,  'fr', NULL, 0),
    (13, 180,  'zh', NULL, 0)
    GO

    -- HostBankAccounts
    SET IDENTITY_INSERT [dbo].[HostBankAccounts] ON
    INSERT [dbo].[HostBankAccounts] ([ID],[HostUserID],[BankName],[AccountNumber],[AccountHolder],[IsPrimary],[IsVerified]) VALUES
    (1, 2, N'Kookmin Bank',  N'123-456-78901234', N'Nerses Sirvard',  1, 1),
    (2, 3, N'Shinhan Bank',  N'110-345-67890123', N'Mahdi Jokar',     1, 1),
    (3, 3, N'Kakao Bank',    N'3333-01-2345678',  N'Mahdi Jokar',     0, 0),
    (4, 4, N'Woori Bank',    N'1002-345-678901',  N'Min-Seo Young-Ho',1, 1),
    (5, 6, N'Hana Bank',     N'159-910-1234567',  N'Minju Olp',       1, 1)
    SET IDENTITY_INSERT [dbo].[HostBankAccounts] OFF
    GO

    -- Scores
    SET IDENTITY_INSERT [dbo].[Scores] ON
    INSERT [dbo].[Scores] ([ID],[GUID],[Name]) VALUES
    (1, N'a659f251-5f26-404a-80f4-17dac4ff2ecd', N'Location'),
    (2, N'f3725fbe-1a7f-4133-8d8d-bdc7ed74000c', N'Cleanliness'),
    (3, N'9f87423a-cd27-4d61-80be-1627f9c4d4bb', N'Value for money'),
    (4, N'18183ad4-2112-4a6b-b190-c1ce72f49896', N'Facilities'),
    (5, N'784ce0dd-7927-4e2b-8270-b8896e9c1774', N'Comfort')
    SET IDENTITY_INSERT [dbo].[Scores] OFF
    GO

    -- Items
    SET IDENTITY_INSERT [dbo].[Items] ON
    INSERT [dbo].[Items] ([ID],[GUID],[HostUserID],[ItemTypeID],[AreaID],[Title],[Capacity],[NumberOfBeds],
        [NumberOfBedrooms],[NumberOfBathrooms],[ExactAddress],[ApproximateAddress],[Description],[HostRules],
        [MinimumNights],[MaximumNights]) VALUES
    (2,  N'a09423d2-25e5-4ca2-978f-03af1d7bf7fd', 2,1,15, N'Superior Cozy Home J',       5,5,1,1,
        N'562-1 Punggok-ri, Gagok-myeon, Seocho-gu', N'Seocho-gu',
        N'With 1 bedroom, this air-conditioned apartment features 1 bathroom with a bidet.',
        N'Pets are not allowed.', 1,10000),
    (3,  N'63de0bd5-c079-4036-be6e-b9bc9afa5e56', 4,5,1, N'Lee Jae Guesthouse',          4,3,1,0,
        N'583 Jisan-ri, Habuk-myeon, Dobong-gu', N'Dobong-gu',
        N'Comfortable as your home. Free WiFi, 6.4 km from Hongik University.',
        N'Smoking is not allowed.', 1,10000),
    (4,  N'aca50fbb-def0-4657-af83-3a30cde5db51', 3,1,2, N'Lee Jae Sweet House',         3,3,1,0,
        N'126-3 Gaeksa-ri, Paengseong-eup, Dongdaemun-gu', N'Dongdaemun-gu',
        N'Located in Seoul, 2.9 km from Ewha Women''s University.',
        N'Pets are not allowed.', 1,10000),
    (5,  N'158a1a1e-e75d-492b-9f76-65c8fbb539d5', 2,3,15, N'Hongdae Min House 2',        5,5,3,1,
        N'123 Nogok-ri, Yangseong-myeon, Seocho-gu', N'Seocho-gu',
        N'In the Mapo-gu district of Seoul, close to Hongik University Station.',
        N'Pets are not allowed.', 1,10000),
    (7,  N'8cb82712-d0b8-420e-a6c6-84c9a5ec0300', 3,2,1, N'Lovely House Dobong',         5,5,2,0,
        N'198-10 Uijeongbu 1(il)-dong, Dobong-gu', N'Dobong-gu',
        N'Fully equipped kitchen with a microwave, private bathroom.',
        N'Pets are not allowed.', 1,10000),
    (8,  N'950ef823-36f2-4c27-8286-e47d9d62ae9f', 6,3,15, N'HONGDAE HELEN''S HOUSE',    6,6,3,2,
        N'200-9 Mui-ri, Bongpyeong-myeon, Seocho-gu', N'Seocho-gu',
        N'Located in Seoul with Hongik University Station nearby.',
        N'Pets are not allowed. Smoking is not allowed.', 1,10000),
    (9,  N'87e8d2de-7460-46fe-bbbe-fd4ca0fb35df', 6,1,4, N'Residence Unicorn',           7,7,2,2,
        N'1135-6 Ingye-dong, Gangbuk-gu', N'Gangbuk-gu',
        N'Ewha Women''s University is 3.5 km from the apartment.',
        N'Pets are not allowed.', 1,10000),
    (10, N'817c6887-ce04-4b27-8b9f-fbbff8b94f92', 6,1,4, N'Eve Hongdae Studio',          2,1,1,0,
        N'228-87 Micheon-ri, Munui-myeon, Gangbuk-gu', N'Gangbuk-gu',
        N'Cozy studio with city views, free WiFi.',
        N'Pets are not allowed.', 1,10000),
    (11, N'666af311-cb9a-48c3-bd97-7aede1416663', 6,1,5, N'Samsung Coex Gangnam Suite',  2,1,1,1,
        N'900-13 Hwajeong-dong, Gangnam-gu', N'Gangnam-gu',
        N'Air-conditioned, flat-screen TV, living room.',
        N'Pets are not allowed. Smoking is not allowed.', 1,10000),
    (18, N'496c5c1a-3cb3-4777-ab59-dd9400a231cd', 3,5,2, N'Hongdae Residence',           5,4,1,0,
        N'74 Sunae 3(sam)-dong, Dongdaemun-gu', N'Dongdaemun-gu',
        N'Cozy and affordable. Close to public transport.',
        N'Pets are not allowed.', 1,10000),
    (19, N'd1639191-64a3-451a-b0a2-6bd1c37ee92d', 3,5,1, N'Hongdae Residence-2',         10,7,5,3,
        N'130 Gangnam-dong, Dobong-gu', N'Dobong-gu',
        N'Spacious 5-bedroom property. Perfect for large groups.',
        N'Pets are not allowed.', 1,10000),
    (23, N'303e83b2-ac84-4464-a5fe-f6b9c8ad0e65', 4,2,15, N'Laon House Yeonnam',         3,2,2,0,
        N'640-2 Gak-ri, Ochang-eup, Seocho-gu', N'Seocho-gu',
        N'Quiet neighborhood, close to cafes and local markets.',
        N'Smoking is not allowed.', 1,10000),
    (24, N'dc2b584b-c91e-4a21-8af5-cbe03cd0e442', 4,5,15, N'Coopie House 2',             5,4,1,1,
        N'36-1 Indong-ri, Gangdong-myeon, Seocho-gu', N'Seocho-gu',
        N'Well-equipped kitchen, private bathroom.',
        N'Pets are not allowed.', 1,10000),
    (25, N'1d35340d-77e2-41f7-a341-2067600c9340', 4,3,15, N'Orakai Insadong Suites',     10,8,5,3,
        N'337 Ungjin-dong, Seocho-gu', N'Seocho-gu',
        N'Luxury suites in the heart of Seoul.',
        N'Pets are not allowed.', 1,10000)
    SET IDENTITY_INSERT [dbo].[Items] OFF
    GO

    -- ItemAmenities
    SET IDENTITY_INSERT [dbo].[ItemAmenities] ON
    INSERT [dbo].[ItemAmenities] ([ID],[GUID],[ItemID],[AmenityID]) VALUES
    (1, N'd71e48f1-a38f-40e5-9185-54635f8276b6', 5,1),(2, N'ca76fcd9-120c-42e9-8e09-fe90010050bf', 7,1),
    (3, N'ddea27c2-b94a-410d-b047-2b695b237b9a',18,1),(4, N'd209e504-4358-48c1-804e-3f8d4d3d43a5', 5,2),
    (5, N'9f510a52-a328-4d02-b027-20c0a32a035b', 9,2),(6, N'f5adfcfd-12ea-4b7b-bf2a-ba5efb95d676',18,2),
    (7, N'692e5c75-7af9-461b-81fe-6b210f848081', 5,3),(8, N'a3cac984-9b12-4b60-99eb-35caa3af4b8e', 7,3),
    (9, N'a1549613-4487-4a6c-b114-cf7e0a4177dc', 7,4),(10,N'b42dfdb6-06c4-4885-b9db-eaddb1aa6a80', 9,4),
    (11,N'07780b14-8d8c-4e50-8e49-50685fd544b0',18,4),(12,N'52936b74-624b-4689-8920-a67db694c256',25,4)
    SET IDENTITY_INSERT [dbo].[ItemAmenities] OFF
    GO

    -- ItemPrices
    SET IDENTITY_INSERT [dbo].[ItemPrices] ON
    INSERT [dbo].[ItemPrices] ([ID],[GUID],[ItemID],[Date],[Price],[CancellationPolicyID]) VALUES
    (1,  N'dfe5de3b-8b77-43c2-bfff-046dc70a5e9e',18,'2022-10-10',100.00,1),
    (2,  N'd85c42a1-417d-40df-b623-c442fe49abe1',18,'2022-10-11',100.00,2),
    (3,  N'e66af364-98c4-4295-9780-9bbf4e11d01a',18,'2022-10-12',100.00,2),
    (4,  N'0d3b2c92-9d60-48af-a65d-7a7438f23169',18,'2022-10-13',100.00,2),
    (5,  N'c4a90090-e2d7-44be-9d30-e07421241afb',18,'2022-10-14',100.00,1),
    (6,  N'0cb62368-696d-415c-a73b-2d673142d44b',18,'2022-10-15',100.00,1),
    (7,  N'38ef2158-a3e8-4f82-8144-c6dc3d122013',18,'2022-10-16',100.00,1),
    (8,  N'cd7b6b28-dbbb-4663-8866-ba4905417261',18,'2022-10-17',100.00,2),
    (9,  N'1029d7b0-0d2c-4b77-a822-be4ee4c0686b',18,'2022-10-18',140.00,3),
    (10, N'a334171b-350f-4561-a29a-caaf78456710',18,'2022-10-19',145.00,2),
    (11, N'6b2d6323-f03c-471d-bfd9-75cbee33ae0f',18,'2022-10-20',140.00,1),
    (12, N'0a578d91-0643-4372-b1c9-9adac5462c1d',19,'2022-10-22',200.00,2),
    (13, N'45897c24-7fc2-4625-b105-ea5015207e74',19,'2022-10-23',210.00,2),
    (14, N'8f28a345-8ae1-4f27-ba55-92813218d37b',19,'2022-10-24',190.00,1),
    (15, N'de716cf0-997a-4882-b543-833c4642085d',19,'2022-10-25',190.00,1),
    (16, N'878bf0ee-36ff-4928-948b-1c80598b482c',19,'2022-10-26',190.00,1),
    (17, N'9010adcc-c977-4480-96e0-d321ca3cea1f',19,'2022-10-27',190.00,3),
    (18, N'5f29010b-4239-4fbe-bee4-899ab2aee960',19,'2022-10-28',190.00,3),
    (19, N'b2dbf15e-2df2-4e50-b154-5a145ba6b45e',19,'2022-10-29',190.00,1),
    (20, N'b9618ae7-8bc1-4cac-a696-12c6c54ebcf7',19,'2022-10-30',270.00,1),
    (21, N'04599602-bddb-4cd9-9298-21350193bee1',19,'2022-10-31',290.00,1)
    SET IDENTITY_INSERT [dbo].[ItemPrices] OFF
    GO

    -- Transactions
    SET IDENTITY_INSERT [dbo].[Transactions] ON
    INSERT [dbo].[Transactions] ([ID],[UserID],[TransactionTypeID],[Amount],[TransactionDate],[GatewayReturnID]) VALUES
    (1, 8, 1, 300.00, '2022-10-12', N'GW-2022-001'),
    (2, 9, 1, 570.00, '2022-10-22', N'GW-2022-002'),
    (3, 10, 1, 200.00, '2022-10-14', N'GW-2022-003'),
    (4, 11, 1, 400.00, '2022-10-25', N'GW-2022-004'),
    (5, 12, 1, 145.00, '2022-10-19', N'GW-2022-005'),
    (6, 3, 1, 190.00, '2022-10-23', N'GW-2022-006')
    SET IDENTITY_INSERT [dbo].[Transactions] OFF
    GO

    -- Bookings
    SET IDENTITY_INSERT [dbo].[Bookings] ON
    INSERT [dbo].[Bookings] ([ID],[GuestUserID],[ItemID],[CheckInDate],[CheckOutDate],
        [NumberOfGuests],[PricePerNight],[TotalPrice],[DiscountAmount],[FinalPrice],
        [CancellationPolicyID],[BookingStatus],[TransactionID]) VALUES
    (1,  8, 18,'2022-10-12','2022-10-15', 2,100.00,300.00,  0.00,300.00,1,'Completed',1),
    (2,  9, 19,'2022-10-22','2022-10-25', 4,190.00,570.00,  0.00,570.00,2,'Completed',2),
    (3, 10, 18,'2022-10-14','2022-10-16', 1,100.00,200.00, 10.00,190.00,1,'Completed',3),
    (4, 11, 19,'2022-10-25','2022-10-27', 3,190.00,380.00, 20.00,360.00,2,'Confirmed',4),
    (5, 12, 18,'2022-10-18','2022-10-19', 1,140.00,140.00,  0.00,140.00,3,'Cancelled',NULL),
    (6, 3, 23,'2022-10-23','2022-10-24', 2,190.00,190.00,  0.00,190.00,2,'Completed',6)
    SET IDENTITY_INSERT [dbo].[Bookings] OFF
    GO

    -- BookingCoupons
    SET IDENTITY_INSERT [dbo].[BookingCoupons] ON
    INSERT [dbo].[BookingCoupons] ([ID],[GUID],[BookingID],[CouponID],[DiscountApplied]) VALUES
    (1, N'bc000001-0000-0000-0000-000000000001', 3, 1, 10.00),
    (2, N'bc000002-0000-0000-0000-000000000002', 4, 2, 20.00)
    SET IDENTITY_INSERT [dbo].[BookingCoupons] OFF
    GO

    -- BookingStatusHistory
    SET IDENTITY_INSERT [dbo].[BookingStatusHistory] ON
    INSERT [dbo].[BookingStatusHistory] ([ID],[BookingID],[OldStatus],[NewStatus],[ChangedByUserID]) VALUES
    (1, 1, NULL,        N'Pending',   1),
    (2, 1, N'Pending',  N'Confirmed', 1),
    (3, 1, N'Confirmed',N'CheckedIn', 1),
    (4, 1, N'CheckedIn',N'Completed', 1),
    (5, 5, NULL,        N'Pending',   1),
    (6, 5, N'Pending',  N'Cancelled', 12),
    (7, 6, NULL,        N'Pending',   1),
    (8, 6, N'Pending',  N'Confirmed', 1),
    (9, 6, N'Confirmed',N'Completed', 1)
    SET IDENTITY_INSERT [dbo].[BookingStatusHistory] OFF
    GO

    -- BookingDetails (from Session6 data, reused)
    SET IDENTITY_INSERT [dbo].[BookingDetails] ON
    INSERT [dbo].[BookingDetails] ([ID],[GUID],[BookingID],[ItemPriceID],[isRefund],[RefundDate],[RefundCancellationPolicyID]) VALUES
    (1, N'4140c8d7-0acf-4e3b-98ab-c7990ecb90e4', 1, 2, 1, '2022-10-08', 1),
    (2, N'73bb1dac-a6f7-4082-8a6d-2f471091083a', 2, 11, 1, '2022-10-11', 2),
    (3, N'31cd25f0-90ff-4c18-98bd-5d38c2574f08', 2, 12, 0, NULL, NULL),
    (4, N'ed50d988-527c-4cf2-91c7-7820052661e9', 3, 7, 0, NULL, NULL),
    (5, N'eed666dd-8cc8-41e2-9291-57ef825ffdf0', 4, 8, 0, NULL, NULL),
    (6, N'97af1858-71ef-40b7-b4dd-cfafc73afcda', 4, 9, 0, NULL, NULL),
    (7, N'3065fa73-016a-43c0-8dfd-8c3837f07c47', 5, 15, 0, NULL, NULL),
    (8, N'1a5c1b4d-62f6-45a5-84e1-da3babfd16b8', 6, 21, 1, '2022-10-01', 3)
    SET IDENTITY_INSERT [dbo].[BookingDetails] OFF
    GO

    -- Reviews
    SET IDENTITY_INSERT [dbo].[Reviews] ON
    INSERT [dbo].[Reviews] ([ID],[BookingID],[ReviewerID],[RevieweeID],[ItemID],[Rating],[Comment]) VALUES
    (1, 1,  8, NULL, 18, 5, N'Amazing stay! Clean, cozy and great location.'),
    (2, 2,  9, NULL, 19, 4, N'Very spacious. Great for families.'),
    (3, 3, 10, NULL, 18, 5, N'Perfect little studio. Will come back!'),
    (4, 1,  8,  3, NULL, 5, N'Host Mahdi was very responsive and helpful.'),
    (5, 2,  9,  3, NULL, 4, N'Good communication throughout.'),
    (6, 6,  3, NULL, 23, 5, N'Lovely quiet place. Will recommend to guests!')
    SET IDENTITY_INSERT [dbo].[Reviews] OFF
    GO

    -- ItemScores (from Session6)
    SET IDENTITY_INSERT [dbo].[ItemScores] ON
    INSERT [dbo].[ItemScores] ([ID],[GUID],[UserID],[ItemID],[ScoreID],[Value]) VALUES
    (4, N'e9048f72-8df8-4a9b-b574-4c445e544008', 7, 19, 2, 5),
    (5, N'4733939d-e692-456c-b0de-1d4911268733', 7, 19, 3, 5),
    (6, N'f6c62d9c-adba-4624-bb49-f2cce1d4e5f1', 4, 19, 3, 2),
    (7, N'f0b38882-7c05-4fdb-b53a-c81df937b711', 4, 18, 4, 4),
    (8, N'f024efda-48b1-47ba-bd5e-0ceb3a51d1e4', 6, 18, 2, 4),
    (9, N'39be56f8-fd61-4950-bc65-36376eb725f1', 2, 18, 2, 1),
    (10, N'b7c94858-c77f-4f27-935e-3ac73523bd1d', 4, 19, 4, 3),
    (11, N'4e092c9e-3020-414e-b699-70fdc9bbc607', 2, 18, 1, 5),
    (12, N'6a4233f9-3515-4dd0-9380-73bc991bef6a', 4, 18, 1, 4),
    (13, N'64416f52-5180-4879-b7d1-3e8d05ce3652', 4, 19, 5, 5),
    (14, N'3b2c4252-1291-47b7-b9df-9f19c90aad16', 4, 18, 1, 1),
    (15, N'8dcb8c85-543a-430e-8363-f9f6eb2f89d1', 6, 19, 5, 5),
    (16, N'7e5d33e5-d410-4dea-91a8-f074d897dfeb', 4, 19, 2, 1),
    (17, N'c2a9be34-808e-49de-96be-3078f959a54b', 4, 18, 1, 2),
    (18, N'5fa56f38-f852-4a97-8230-5309bcff1c9d', 2, 18, 4, 4),
    (19, N'4272dbe0-9beb-488c-9de1-e8a875bcce97', 4, 19, 1, 3),
    (20, N'454d9168-1018-4a32-b6be-ba80fedb7415', 4, 19, 5, 2),
    (21, N'87c162f2-ef8b-4164-a6f4-33dd1bc9db24', 6, 18, 5, 5),
    (22, N'ebdf6a0f-51ed-42ae-9ec8-a6739c11d9ee', 2, 19, 2, 4),
    (23, N'1af2f89e-0c3e-44df-af34-16a02b5866d2', 6, 19, 5, 2),
    (24, N'a1c2a3d9-f913-4e80-ad1e-1a981286dd0e', 2, 19, 4, 4),
    (25, N'e7eaecca-4697-420b-a48b-712c5b26a546', 7, 18, 5, 4),
    (26, N'd3e073eb-6dbf-4899-810b-36babcf4b7f7', 4, 18, 2, 1),
    (27, N'8d7481ff-dac3-4bc2-aa24-83dbb8d5bda5', 7, 18, 5, 3),
    (28, N'a0359aa0-063d-4b24-8e57-4e8f4ab38ccf', 2, 18, 3, 5),
    (29, N'c2fea44a-5768-4bce-8d06-1a54bdd07d6a', 6, 19, 4, 3),
    (30, N'cd9454ad-605a-40df-b1d9-801f378e7180', 4, 18, 5, 1),
    (31, N'b53fa930-ae42-469a-a7a9-8b6f5d15e2d2', 4, 19, 4, 4),
    (32, N'c61858e5-9852-47d9-8bf6-85a0d76e32bd', 2, 18, 3, 3),
    (33, N'c764093c-d06a-4848-94a1-bed8c85eaa58', 4, 18, 3, 3),
    (34, N'affb8108-0654-480d-af17-0e5310aad5c1', 4, 18, 2, 4),
    (35, N'c333a3e3-0373-4b87-bd2a-927738812ec1', 2, 19, 3, 4),
    (36, N'e8094d61-f999-41c2-a7c2-4d513b5fcaf8', 7, 18, 4, 4),
    (37, N'0839a31f-70bd-42f9-998a-c3a443c70d88', 7, 18, 3, 4),
    (38, N'3898f58f-0170-4d9f-8c27-36f403247339', 4, 18, 1, 1)
    SET IDENTITY_INSERT [dbo].[ItemScores] OFF
    GO

    -- ServiceTypes
    SET IDENTITY_INSERT [dbo].[ServiceTypes] ON
    INSERT [dbo].[ServiceTypes] ([ID],[GUID],[Name],[IconName],[Description]) VALUES
    (1, N'2d743a10-266e-43af-b932-249f06115052', N'City tours', N'207-eye.png', N'The City Tour services provide transportation for tourists over a fixed route that lets passengers get on and off an unlimited number of times per day'),
    (2, N'ddd1bd05-0afb-4943-b199-bbc4e81b4886', N'Attraction tickets', N'016-camera.png', N' We issue gate-ready theme park tickets and can deliver'),
    (3, N'91ee94cd-ebde-4e9c-971f-7b78364d471d', N'Airport Transfer', N'176-airplane.png', N'Airport Taxi/Reservation/Booking Private Car, Luxuary, Van at the Best Price.'),
    (4, N'59633a8e-109c-4914-848d-5b4ad2069699', N'Catering services', N'164-spoon-knife.png', N'Serving food to guests at events'),
    (5, N'38f3ccf7-194e-43a7-a468-f69bd1c66485', N'Safety box', N'100-drive.png', N'A safe deposit box is an individually secured container—usually a metal box—housed in the vault of a federally insured bank or credit union.')
    SET IDENTITY_INSERT [dbo].[ServiceTypes] OFF
    GO

    -- Services
    SET IDENTITY_INSERT [dbo].[Services] ON
    INSERT [dbo].[Services] ([ID],[GUID],[ServiceTypeID],[Name],[Price],[Duration],[Description],[DayOfWeek],[DayOfMonth],[DailyCap],[BookingCap]) VALUES
    (37, N'65217ab4-a5b4-4194-a7e1-edbb1861b08e', 1, N'Seoul Special PACK ', CAST(390.00 AS Decimal(10, 2)), 2, N'Two day we will visit the famous historic and cultural, National museum of korea,Nansam Tower,Gyeongbokgung and …', N' ', N'*', 29, 1),
    (38, N'cac808cf-8ec2-4997-bd0a-bf49ded0ecdd', 1, N'Night Pack', CAST(98.00 AS Decimal(10, 2)), 1, N'Night couse 19:30PM ()Gwanghwamun ', N' ', N'*', 29, 1),
    (39, N'3d1af41f-d9e7-4de4-b937-d78d0ecc57ba', 1, N'Tour D. 야경코스', CAST(140.00 AS Decimal(10, 2)), 1, N'The night course offers a beautiful view of Seoul with the Han River in its arms.', N' ', N'*', 42, 1),
    (40, N'c857ebe7-4a1f-4c7b-84e8-1977432e439a', 2, N'Hakgojae Gallery', CAST(25.00 AS Decimal(10, 2)), 1, N'Daily 10:00 – 18:00, Closed Mondays, +82-2-720-1524', N'2-7', N'  ', 200, 1),
    (41, N'70388b47-8eaf-40b8-acd9-8bd06be8ccee', 2, N'Songeun Art Cube', CAST(42.00 AS Decimal(10, 2)), 1, N'Daily 11:00 - 19:00, Closed Sundays', N'1-6', N'  ', 150, 1),
    (42, N'136b3fff-e5fa-4187-ad74-8a7ce62a9889', 2, N'Changdeokgung Palace', CAST(60.00 AS Decimal(10, 2)), 1, N'Changdeokgung Palace is the second UNESCO World Heritage Site in Seoul. It was selected as a representative palace for its notable beauty in the history of palace architecture in East Asia and for its excellent arrangement with the surrounding naturalenvironment. Changdeokgung Palace was built by King Taejong in 1405.', N' ', N'*', 150, 1),
    (43, N'64ac9882-f1b6-4c8c-805e-90d09ce3c253', 2, N'Gyeongbokgung Palace', CAST(30.00 AS Decimal(10, 2)), 1, N'Gyeongbokgung Palace was the first and largest of the royal palaces built during the Joseon Dynasty. Built in 1395, Gyeongbokgung Palace was located at the heart of the newly appointed capital of Seoul (then known as Hanyang) and represented the sovereignty of the Joseon Dynasty. The largest of the Five Grand Palaces (the others being Gyeonghuigung Palace, Deoksugung Palace, Changgyeonggung Palace, Changdeokgung Palace), Gyeongbokgung served as the main palace of the Joseon Dynasty.', N' ', N'1,3-4', 3, 1),
    (44, N'410a3db9-3c1e-4855-9b14-ad87201c3e55', 3, N'Cars (Sedan)', CAST(28.00 AS Decimal(10, 2)), 3, N'Airportransferservices providers use the latest models of cars available ', N'*', N' ', 55, 3),
    (45, N'23818975-d995-4c21-b4e6-501a89b3615f', 3, N'Minibus', CAST(65.00 AS Decimal(10, 2)), 15, N'Transfer services with minibuses,Minibuses can have up to 15 seats', N'*', N' ', 15, 15),
    (46, N'4cc6f5fd-bd9f-48af-948d-4b3ce1415332', 3, N'Limousine', CAST(110.00 AS Decimal(10, 2)), 4, N'A limousine or limo for short, is a large, chauffeur-driven luxury vehicle with a partition between the driver compartment and the passenger compartment.', N' ', N'1,2,3,15-20', 2, 4),
    (47, N'5b90e2d4-d124-4887-b1f6-7634873a33a5', 3, N'Bus', CAST(107.00 AS Decimal(10, 2)), 37, N'Transfer services with buses', N'1,3,5', N' ', 2, 37)
    SET IDENTITY_INSERT [dbo].[Services] OFF
    GO

    -- AddonServices
    SET IDENTITY_INSERT [dbo].[AddonServices] ON
    INSERT [dbo].[AddonServices] ([ID],[GUID],[UserID],[BookingID],[CouponID]) VALUES
    (1, N'68144ae1-749b-4e9f-86dc-be4b2d95d226', 3, NULL, 2),
    (2, N'2d8feccc-63e2-4b1f-82f3-80ada470f5f1', 3, NULL, NULL),
    (3, N'2a50b37d-558a-4867-bc4d-1d75a6004ee6', 2, NULL, NULL),
    (4, N'44376037-ae83-424f-990d-39aac977af66', 2, NULL, 3),
    (8, N'e7718204-582a-40f3-b59c-6514b2229220', 7, NULL, NULL),
    (9, N'bafeb37d-9f8e-4ccd-8518-f681721640ba', 7, NULL, NULL),
    (10, N'e381b1f4-935b-4a32-b7d9-58940ab4cbc6', 7, NULL, NULL),
    (11, N'95a2a6f7-2c68-49e2-a07b-ae33fed2503a', 7, NULL, NULL),
    (12, N'3dfead6c-20d0-4ef1-a5a8-d62a77d26762', 7, NULL, NULL),
    (13, N'67699bf4-a1fe-4313-9474-c1f2770ec039', 7, NULL, NULL),
    (14, N'e2c57438-fdb1-4fee-a8ba-e6d94b5c2b69', 7, NULL, NULL),
    (15, N'57f155a6-f948-4194-9077-369aaa7c1c49', 7, NULL, NULL),
    (16, N'e406471b-b21c-417e-91b1-9ca8a4b0d1a4', 7, NULL, NULL),
    (17, N'52aee0ce-8ea0-4641-9771-aab3db82772a', 7, NULL, NULL),
    (18, N'e50fe419-9c67-4dc4-9789-23ebdf940891', 7, NULL, NULL),
    (19, N'6e32e473-2da4-4119-883d-78a6f132d25a', 7, NULL, NULL),
    (20, N'2972ebaf-f955-43a0-8169-1c62023ca569', 7, NULL, NULL),
    (21, N'038f2f4d-342b-43a4-9509-4293f828c349', 7, NULL, NULL),
    (22, N'9c80e9c7-7ff3-4292-9f3a-062f1232ef64', 7, NULL, NULL)
    SET IDENTITY_INSERT [dbo].[AddonServices] OFF
    GO

    -- AddonServiceDetails
    SET IDENTITY_INSERT [dbo].[AddonServiceDetails] ON
    INSERT [dbo].[AddonServiceDetails] ([ID],[GUID],[AddonServiceID],[ServiceID],[Price],[FromDate],[Notes],[NumberOfPeople],[isRefund]) VALUES
    (1, N'02e20a1f-3272-452d-ad9d-b66941d4df97', 1, 45, CAST(864.50 AS Decimal(10, 2)), CAST(N'2022-10-15T00:00:00.000' AS DateTime), N' ---- ', 205, 0),
    (2, N'4cd9ec78-8f6a-45ca-a342-32bac85a32e4', 2, 45, CAST(65.00 AS Decimal(10, 2)), CAST(N'2022-10-15T00:00:00.000' AS DateTime), N'   ***  ', 5, 0),
    (3, N'b6fb076b-a792-4322-8669-b8b23fbeed12', 3, 39, CAST(5740.00 AS Decimal(10, 2)), CAST(N'2022-10-15T00:00:00.000' AS DateTime), N' ---- ', 41, 0),
    (4, N'17952690-5c76-4f1a-a622-855392e8ec02', 4, 39, CAST(5550.00 AS Decimal(10, 2)), CAST(N'2022-10-15T00:00:00.000' AS DateTime), N' **** ', 40, 0),
    (5, N'9a56918d-bad4-465e-ace5-ae3f4ea7e86a', 8, 43, CAST(90.00 AS Decimal(10, 2)), CAST(N'2022-08-01T00:00:00.000' AS DateTime), N'   ***  ', 3, 0),
    (6, N'f5d06714-ccaf-45d6-89bd-c2e7396108a1', 9, 43, CAST(90.00 AS Decimal(10, 2)), CAST(N'2022-08-03T00:00:00.000' AS DateTime), N'   ***  ', 3, 0),
    (7, N'7d98ad32-5490-4989-895c-720bf1d9da11', 10, 43, CAST(90.00 AS Decimal(10, 2)), CAST(N'2022-08-04T00:00:00.000' AS DateTime), N'   ***  ', 3, 0),
    (8, N'7bd122ab-ecca-4c64-a8fe-37b0bc194a72', 11, 43, CAST(90.00 AS Decimal(10, 2)), CAST(N'2022-07-01T00:00:00.000' AS DateTime), N'   ***  ', 3, 0),
    (9, N'b0e8d48e-2daf-4af6-be83-dc2ba4bbfa32', 12, 43, CAST(90.00 AS Decimal(10, 2)), CAST(N'2022-07-03T00:00:00.000' AS DateTime), N'   ***  ', 3, 0),
    (10, N'4476e390-3063-4f31-bc02-5d3798c8acd3', 13, 43, CAST(90.00 AS Decimal(10, 2)), CAST(N'2022-07-04T00:00:00.000' AS DateTime), N'   ***  ', 3, 0),
    (11, N'6d9b9824-f038-4c72-9edb-8e6a73ff0af0', 14, 43, CAST(90.00 AS Decimal(10, 2)), CAST(N'2022-06-01T00:00:00.000' AS DateTime), N'   ***  ', 3, 0),
    (12, N'16258fd1-f5d1-4136-aac7-98d3cc1fe80f', 15, 43, CAST(90.00 AS Decimal(10, 2)), CAST(N'2022-06-03T00:00:00.000' AS DateTime), N'   ***  ', 3, 0),
    (13, N'55e7898c-0a19-4ef9-af79-4b04bdfaa4da', 16, 43, CAST(90.00 AS Decimal(10, 2)), CAST(N'2022-06-04T00:00:00.000' AS DateTime), N'   ***  ', 3, 0),
    (14, N'85059347-dc0f-40cf-a049-a666f68ca919', 17, 43, CAST(90.00 AS Decimal(10, 2)), CAST(N'2022-04-01T00:00:00.000' AS DateTime), N'   ***  ', 3, 0),
    (15, N'4007e34c-023a-401f-b83f-19fb8eef57c9', 18, 43, CAST(90.00 AS Decimal(10, 2)), CAST(N'2022-04-03T00:00:00.000' AS DateTime), N'   ***  ', 3, 0),
    (16, N'24c5072b-aa52-4b33-91e3-bed0d057b62b', 19, 43, CAST(90.00 AS Decimal(10, 2)), CAST(N'2022-04-04T00:00:00.000' AS DateTime), N'   ***  ', 3, 0),
    (17, N'e7b49284-0cbb-405b-977d-04144d868c55', 20, 43, CAST(90.00 AS Decimal(10, 2)), CAST(N'2022-12-01T00:00:00.000' AS DateTime), N'   ***  ', 3, 0),
    (18, N'cfd23b72-49ec-4d85-802f-18f8c7abe39c', 21, 43, CAST(90.00 AS Decimal(10, 2)), CAST(N'2022-12-03T00:00:00.000' AS DateTime), N'   ***  ', 3, 0),
    (19, N'901da38a-5f1b-49ee-b005-90ad10940856', 22, 43, CAST(90.00 AS Decimal(10, 2)), CAST(N'2022-12-04T00:00:00.000' AS DateTime), N'   ***  ', 3, 0)
    SET IDENTITY_INSERT [dbo].[AddonServiceDetails] OFF
    GO

    PRINT '====================================================='
    PRINT ' Seoul_Stay v4 (merged) created successfully.'
    PRINT ' Total tables: 30+'
    PRINT ' Ready for both Module 1 and Module 3 requirements.'
    PRINT '====================================================='
    GO