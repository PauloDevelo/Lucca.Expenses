CREATE TABLE [dbo].[Expenses] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [UserId]      UNIQUEIDENTIFIER NOT NULL,
    [PurchasedOn] DATETIME2 (7)    NOT NULL,
    [Comment]     NVARCHAR (MAX)   NOT NULL,
    [Category]    NCHAR (100)      NOT NULL,
    [Amount]      SMALLMONEY       NOT NULL,
    [Currency]    CHAR (3)         NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Expenses_UserInfo] FOREIGN KEY ([UserId]) REFERENCES [dbo].[UserInfo] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);

