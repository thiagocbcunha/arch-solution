USE [VerxTransaction]

--- Transaction
CREATE TABLE [dbo].[Transaction] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [TransactionId] UNIQUEIDENTIFIER NOT NULL,
    [Amount] DECIMAL(18, 2) NOT NULL,
    [TransactionDate] DATETIME2 NOT NULL
);

--- TransactionEvent
CREATE TABLE [dbo].[TransactionEvent] (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [VersionNum] [int] NOT NULL,
    [Currency] NVARCHAR(10) NOT NULL,
    [Description] NVARCHAR(255) NOT NULL,
    [SenderAccountId] NVARCHAR(100) NOT NULL,
    [ReceiverAccountId] NVARCHAR(100) NOT NULL,
    [CreateBy] NVARCHAR(50) NOT NULL,
    [CreateDate] DATETIME2 NOT NULL,
    CONSTRAINT [PK_TransactionEvent] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[VersionNum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[TransactionEvent] WITH CHECK ADD CONSTRAINT [FK_TransactionEvent_Transaction] FOREIGN KEY([Id]) REFERENCES [dbo].[Transaction] ([Id]) ON DELETE CASCADE
ALTER TABLE [dbo].[TransactionEvent] WITH CHECK ADD CONSTRAINT [DF_TransactionEvent_CreateDate] DEFAULT (getdate()) FOR [CreateDate]
ALTER TABLE [dbo].[TransactionEvent] WITH CHECK ADD CONSTRAINT [DF_TransactionEvent_CreateBy] DEFAULT (suser_sname()) FOR [CreateBy]
ALTER TABLE [dbo].[TransactionEvent] WITH CHECK ADD CONSTRAINT [DF_TransactionEvent_Id] DEFAULT (newid()) FOR [Id]
ALTER TABLE [dbo].[TransactionEvent] WITH CHECK ADD CONSTRAINT [DF_TransactionEvent_VersionNum] DEFAULT ((1)) FOR [VersionNum]

EXEC sp_addextendedproperty 
    @name = 'MS_Description',
    @value = 'Tabela de transações financeiras',
    @level0type = 'SCHEMA', @level0name = 'dbo',
    @level1type = 'TABLE', @level1name = 'Transaction';

EXEC sp_addextendedproperty 
    @name = 'MS_Description',
    @value = 'Identificador único da transação',
    @level0type = 'SCHEMA', @level0name = 'dbo',
    @level1type = 'TABLE', @level1name = 'Transaction',
    @level2type = 'COLUMN', @level2name = 'Id';

EXEC sp_addextendedproperty 
    @name = 'MS_Description',
    @value = 'Identificador externo da transação',
    @level0type = 'SCHEMA', @level0name = 'dbo',
    @level1type = 'TABLE', @level1name = 'Transaction',
    @level2type = 'COLUMN', @level2name = 'TransactionId';

EXEC sp_addextendedproperty 
    @name = 'MS_Description',
    @value = 'Valor da transação',
    @level0type = 'SCHEMA', @level0name = 'dbo',
    @level1type = 'TABLE', @level1name = 'Transaction',
    @level2type = 'COLUMN', @level2name = 'Amount';

EXEC sp_addextendedproperty 
    @name = 'MS_Description',
    @value = 'Data da transação',
    @level0type = 'SCHEMA', @level0name = 'dbo',
    @level1type = 'TABLE', @level1name = 'Transaction',
    @level2type = 'COLUMN', @level2name = 'TransactionDate';

EXEC sp_addextendedproperty 
    @name = 'MS_Description',
    @value = 'Tabela de eventos relacionados a uma transação',
    @level0type = 'SCHEMA', @level0name = 'dbo',
    @level1type = 'TABLE', @level1name = 'TransactionEvent';

EXEC sp_addextendedproperty 
    @name = 'MS_Description',
    @value = 'Identificador único do evento de transação',
    @level0type = 'SCHEMA', @level0name = 'dbo',
    @level1type = 'TABLE', @level1name = 'TransactionEvent',
    @level2type = 'COLUMN', @level2name = 'Id';

EXEC sp_addextendedproperty 
    @name = 'MS_Description',
    @value = 'Número da versão do evento',
    @level0type = 'SCHEMA', @level0name = 'dbo',
    @level1type = 'TABLE', @level1name = 'TransactionEvent',
    @level2type = 'COLUMN', @level2name = 'VersionNum';

EXEC sp_addextendedproperty 
    @name = 'MS_Description',
    @value = 'Moeda da transação',
    @level0type = 'SCHEMA', @level0name = 'dbo',
    @level1type = 'TABLE', @level1name = 'TransactionEvent',
    @level2type = 'COLUMN', @level2name = 'Currency';

EXEC sp_addextendedproperty 
    @name = 'MS_Description',
    @value = 'Descrição do evento de transação',
    @level0type = 'SCHEMA', @level0name = 'dbo',
    @level1type = 'TABLE', @level1name = 'TransactionEvent',
    @level2type = 'COLUMN', @level2name = 'Description';

EXEC sp_addextendedproperty 
    @name = 'MS_Description',
    @value = 'Identificador da conta remetente',
    @level0type = 'SCHEMA', @level0name = 'dbo',
    @level1type = 'TABLE', @level1name = 'TransactionEvent',
    @level2type = 'COLUMN', @level2name = 'SenderAccountId';

EXEC sp_addextendedproperty 
    @name = 'MS_Description',
    @value = 'Identificador da conta destinatária',
    @level0type = 'SCHEMA', @level0name = 'dbo',
    @level1type = 'TABLE', @level1name = 'TransactionEvent',
    @level2type = 'COLUMN', @level2name = 'ReceiverAccountId';