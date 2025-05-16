CREATE TABLE [dbo].[AuditTable](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](255) NOT NULL,
	[KeyFieldID] [int] NOT NULL,
	[ActionType] [int] NOT NULL,
	[DateTimeStamp] [datetime] NOT NULL,
	[DataModel] [nvarchar](50) NOT NULL,
	[Changes] [nvarchar](255) NOT NULL,
	[ValueBefore] [nvarchar](255) NOT NULL,
	[ValueAfter] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_AuditTable] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]


