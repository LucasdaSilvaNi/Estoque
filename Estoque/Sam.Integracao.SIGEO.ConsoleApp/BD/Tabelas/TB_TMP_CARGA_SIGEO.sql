/****** Object:  Table [dbo].[TB_TMP_CARGA_SIGEO]    Script Date: 13/04/2021 15:42:17 ******/
DROP TABLE [dbo].[TB_TMP_CARGA_SIGEO]
GO

/****** Object:  Table [dbo].[TB_TMP_CARGA_SIGEO]    Script Date: 13/04/2021 15:42:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TB_TMP_CARGA_SIGEO](
	[TB_LINHA] [int] NOT NULL,
	[TB_ORGAO_CODIGO] [int] NULL,
	[TB_ORGAO_DESCRICAO] [varchar](120) NULL,
	[TB_UO_CODIGO] [int] NULL,
	[TB_UO_DESCRICAO] [varchar](150) NULL,
	[TB_UGE_CODIGO] [int] NULL,
	[TB_UGE_DESCRICAO] [varchar](150) NULL,
	[TB_UA_CODIGO] [int] NULL,
	[TB_UA_DESCRICAO] [varchar](200) NULL,
	[NIVEL] [varchar](3) NULL,
	[STATUS_UA] [varchar](10) NULL,
	[POSICAO_ATUAL] [varchar](5) NULL,
 CONSTRAINT [PK_TB_TMP_CARGA_SIGEO] PRIMARY KEY CLUSTERED 
(
	[TB_LINHA] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
