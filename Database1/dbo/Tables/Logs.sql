CREATE TABLE [dbo].[Logs] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [Timestamp] DATETIME2 (7)  CONSTRAINT [DF_Logs_Timestamp] DEFAULT (getdate()) NOT NULL,
    [Message]   NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_Logs] PRIMARY KEY CLUSTERED ([ID] ASC)
);

