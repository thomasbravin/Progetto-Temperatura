CREATE TABLE [dbo].[Dati]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [DataMessaggio] DATETIME NOT NULL 
                DEFAULT CURRENT_TIMESTAMP, 
    [Message] DECIMAL(5, 2) NULL,
)
