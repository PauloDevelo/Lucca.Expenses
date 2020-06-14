CREATE TABLE [dbo].[UserInfo] (
    [Id]        UNIQUEIDENTIFIER NOT NULL,
    [FirstName] NCHAR (100)      NOT NULL,
    [LastName]  NCHAR (200)      NOT NULL,
    [Currency]  CHAR (3)         NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

