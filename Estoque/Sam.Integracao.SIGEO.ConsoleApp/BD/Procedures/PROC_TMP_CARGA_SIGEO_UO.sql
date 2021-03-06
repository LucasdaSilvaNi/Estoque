/****** Object:  StoredProcedure [dbo].[PROC_TMP_CARGA_SIGEO_UO]    Script Date: 14/04/2021 13:18:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
		ALTER PROCEDURE [dbo].[PROC_TMP_CARGA_SIGEO_UO] 
		@_MYID UNIQUEIDENTIFIER,
		@_RETURN SMALLINT OUTPUT
		AS 		
		--
        -- STORED PROCEDURE
        --     PROC_TMP_CARGA_SIGEO_UO.
        --
        -- DESCRIPTION
        --     RESPONSÁVEL PELAS CARGAS DO UO.
        --
        -- PARAMETERS
        --     NENHUM
        --
        -- RETURN VALUE
        --         -1 ERRO.
        --          1 SEM ERRO.
		--          0 SEM NECESSIDADE DE PROCESSAR
        --
        -- CHANGE HISTORY
        --     14/04/2021 - VERSÃO INICIAL
        --
        ---- LEGENDA
        ------ (I) INICIO DO PROCESSAMENTO
        ------ (F) FINAL DO PROCESSAMENTO
        ------ (E) PROCESSAMENTO COM ERRO
        ------ (A) DESCRIÇÃO ALTERADA 
        ------ (N) NOVO CÓDIGO CADASTRADO 
        ------ (X) NENHUM REGISTRO A PROCESSSAR
        SET NOCOUNT ON;
		--

DECLARE @MYID						UNIQUEIDENTIFIER;
DECLARE @DT_PROC_INICIAL			TIMESTAMP;
DECLARE @DT_PROC_FINAL				TIMESTAMP;
DECLARE @QT_REGISTROS				INT = 0;
DECLARE @DS_MODULO					VARCHAR(50);
DECLARE @TB_UO_CODIGO				INT,
	    @TB_ORGAO_CODIGO			INT,
        @TB_UO_DESCRICAO			VARCHAR(150),
        @TB_UO_DESCRICAO_NOVA		VARCHAR(150) 

SET @MYID = @_MYID

DECLARE SIGEO_UO_CURSOR CURSOR FOR 						
        SELECT DISTINCT
            CS.TB_UO_CODIGO,
            CS.TB_UO_DESCRICAO,
			CS.TB_ORGAO_CODIGO

        FROM
            TB_TMP_CARGA_SIGEO CS 
        ORDER BY
            CS.TB_UO_CODIGO

    BEGIN TRY 

	    SET @DS_MODULO = 'CARGA SIGEO - UO' 	
        SET
            @QT_REGISTROS = 
            (
                SELECT
                    ISNULL(COUNT(*), 0) 
                FROM
                    TB_TMP_CARGA_SIGEO
            )

        IF @QT_REGISTROS = 0 
        BEGIN
            --
            INSERT INTO
                TB_TMP_CARGA_LOG 
            VALUES
                (
                    @MYID,
                    @DS_MODULO,
                    'X',
                    'NENHUM REGISTRO A PROCESSSAR',
                    GETDATE(),
                    @QT_REGISTROS
                )
			SET @_RETURN = 0;
            RETURN 
        END
        --
		INSERT INTO
			TB_TMP_CARGA_LOG 
		VALUES
			(
            @MYID,
            @DS_MODULO,
            'I',
            'INICIO DO PROCESSAMENTO',
            GETDATE(),
            @QT_REGISTROS      
			)
									--
            OPEN SIGEO_UO_CURSOR FETCH NEXT 
            FROM SIGEO_UO_CURSOR INTO	@TB_UO_CODIGO,
										@TB_UO_DESCRICAO ,
										@TB_ORGAO_CODIGO
										 
            WHILE @@FETCH_STATUS = 0 
            BEGIN

				SET
					@TB_UO_DESCRICAO_NOVA = RTRIM(LTRIM(@TB_UO_DESCRICAO))

				IF EXISTS 
				(
					SELECT
						* 
					FROM
						TB_UO O 
					WHERE
						O.TB_UO_CODIGO = @TB_UO_CODIGO AND O.TB_ORGAO_ID = @TB_ORGAO_CODIGO)

						BEGIN

							IF EXISTS 
							(
								SELECT
									* 
								FROM
									TB_UO O 
								WHERE
									(O.TB_UO_CODIGO = @TB_UO_CODIGO AND O.TB_ORGAO_ID = @TB_ORGAO_CODIGO)
									AND TB_UO_DESCRICAO <> @TB_UO_DESCRICAO_NOVA
							)
							--
							BEGIN
														
								UPDATE
									TB_UO 
								SET
									TB_UO_DESCRICAO = @TB_UO_DESCRICAO_NOVA 
								WHERE
									TB_UO_CODIGO = @TB_UO_CODIGO AND TB_ORGAO_ID = @TB_ORGAO_CODIGO		

								INSERT INTO
									TB_TMP_CARGA_LOG 
								VALUES
									(
										@MYID,
										@DS_MODULO,
										'A',
										'ANTES: [' + @TB_UO_DESCRICAO + '] - APÓS: [' + @TB_UO_DESCRICAO_NOVA + ']',
										GETDATE(),
										@QT_REGISTROS
									)
							END
						END
						
				ELSE

					BEGIN
						--NOVO REGISTRO

						INSERT INTO
							TB_UO (TB_ORGAO_ID, TB_UO_CODIGO, TB_UO_DESCRICAO, TB_UO_STATUS) 
						VALUES
							(
								@TB_ORGAO_CODIGO,
								@TB_UO_CODIGO,
								@TB_UO_DESCRICAO_NOVA,
								1
							)
							
							INSERT INTO
								TB_TMP_CARGA_LOG 
							VALUES
								(
									@MYID,
									@DS_MODULO,
									'N',
									'NOVO CÓDIGO: ' + CAST( @TB_ORGAO_CODIGO AS VARCHAR(6) ) + '/' + CAST( @TB_UO_CODIGO AS VARCHAR(6) ) + ' - NOVA DESCRIÇÃO: ' + @TB_UO_DESCRICAO_NOVA,
									GETDATE(),
									@QT_REGISTROS
								)
					END
			
			FETCH NEXT 
			FROM SIGEO_UO_CURSOR INTO	@TB_UO_CODIGO,
								        @TB_UO_DESCRICAO,
										@TB_ORGAO_CODIGO
            END
                
            INSERT INTO
                TB_TMP_CARGA_LOG 
            VALUES
                (
                    @MYID,
                    @DS_MODULO,
                    'F',
                    'FINAL DO PROCESSAMENTO',
                    GETDATE(),
                    @QT_REGISTROS
                )
			SET @_RETURN = 1;
	END TRY 

    BEGIN CATCH 						

        INSERT INTO
            TB_TMP_CARGA_LOG 
        VALUES
            (
                @MYID,
                @DS_MODULO,
                'E',
                'PROCESSAMENTO COM ERRO: ' + ERROR_MESSAGE() + CHAR(13)+CHAR(10) + ' - CÓDIGO: '+ CAST( @TB_ORGAO_CODIGO AS VARCHAR(6) ) + '/' + CAST( @TB_UO_CODIGO AS VARCHAR(6) ) + ' - DESCRIÇÃO: ' + @TB_UO_DESCRICAO_NOVA,
                GETDATE(),
                @QT_REGISTROS
            )
			SET @_RETURN = -1;
    END CATCH 					

    CLOSE SIGEO_UO_CURSOR 					
    DEALLOCATE SIGEO_UO_CURSOR 

	SET NOCOUNT OFF;