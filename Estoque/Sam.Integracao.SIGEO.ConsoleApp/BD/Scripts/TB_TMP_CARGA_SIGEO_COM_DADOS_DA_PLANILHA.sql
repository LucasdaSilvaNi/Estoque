/****** Object:  Table [dbo].[TB_TMP_CARGA_SIGEO]    Script Date: 29/03/2021 11:39:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_TMP_CARGA_SIGEO](
	[TB_ORGAO_CODIGO] [int] NULL,
	[TB_ORGAO_DESCRICAO] [varchar](120) NULL,
	[TB_UO_CODIGO] [int] NULL,
	[TB_UO_DESCRICAO] [varchar](150) NULL,
	[TB_UGE_CODIGO] [int] NULL,
	[TB_UGE_DESCRICAO] [varchar](150) NULL,
	[TB_UA_CODIGO] [int] NULL,
	[TB_UA_DESCRICAO] [varchar](150) NULL,
	[NIVEL] [varchar](3) NULL,
	[STATUS_UA] [varchar](10) NULL,
	[POSICAO_ATUAL] [varchar](5) NULL
) ON [PRIMARY]
GO