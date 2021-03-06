	alter PROCEDURE [dbo].[PROC_TMP_CARGA_SIGEO_UGE] 
		@_MYID UNIQUEIDENTIFIER,
		@_RETURN SMALLINT OUTPUT
		AS 		
		--
        -- STORED PROCEDURE
        --     PROC_TMP_CARGA_SIGEO_UGE.
        --
        -- DESCRIPTION
        --     RESPONSÁVEL PELAS CARGAS DO UGE.
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
DECLARE @TB_UGE_CODIGO				INT,
	    @TB_UO_ID				    INT,
        @TB_UGE_DESCRICAO			VARCHAR(150),
        @TB_UGE_DESCRICAO_NOVA		VARCHAR(150) 

SET @MYID = @_MYID

DECLARE SIGEO_UGE_CURSOR CURSOR FOR 						
        SELECT DISTINCT
            CS.TB_UGE_CODIGO,
            CS.TB_UGE_DESCRICAO,
			CS.TB_UO_CODIGO

        FROM
            TB_TMP_CARGA_SIGEO CS 
        ORDER BY
            CS.TB_UGE_CODIGO

    BEGIN TRY 

	    SET @DS_MODULO = 'CARGA SIGEO - UGE' 	
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
            OPEN SIGEO_UGE_CURSOR FETCH NEXT 
            FROM SIGEO_UGE_CURSOR INTO	@TB_UGE_CODIGO,
										@TB_UGE_DESCRICAO ,
										@TB_UO_ID
										 
            WHILE @@FETCH_STATUS = 0 
            BEGIN

				SET
					@TB_UGE_DESCRICAO_NOVA = RTRIM(LTRIM(@TB_UGE_DESCRICAO))

				IF EXISTS 
				(
					SELECT
						* 
					FROM
						TB_UGE O 
					WHERE
						O.TB_UGE_CODIGO = @TB_UGE_CODIGO AND O.TB_UO_ID = @TB_UO_ID)

						BEGIN

							IF EXISTS 
							(
								SELECT
									* 
								FROM
									TB_UGE O 
								WHERE
									(O.TB_UGE_CODIGO = @TB_UGE_CODIGO AND O.TB_UO_ID = @TB_UO_ID)
									AND TB_UGE_DESCRICAO <> @TB_UGE_DESCRICAO_NOVA
							)
							--
							BEGIN
														
								UPDATE
									TB_UGE 
								SET
									TB_UGE_DESCRICAO = @TB_UGE_DESCRICAO_NOVA 
								WHERE
									TB_UGE_CODIGO = @TB_UGE_CODIGO AND TB_UO_ID = @TB_UGE_CODIGO		

								INSERT INTO
									TB_TMP_CARGA_LOG 
								VALUES
									(
										@MYID,
										@DS_MODULO,
										'A',
										'ANTES: [' + @TB_UGE_DESCRICAO + '] - APÓS: [' + @TB_UGE_DESCRICAO_NOVA + ']',
										GETDATE(),
										@QT_REGISTROS
									)
							END
						END
						
				ELSE

					BEGIN
						--NOVO REGISTRO

						INSERT INTO
							TB_UGE (TB_UO_ID, TB_UGE_CODIGO, TB_UGE_DESCRICAO, TB_UGE_TIPO, TB_UGE_STATUS, TB_UGE_INTEGRACAO_SIAFEM, TB_UGE_IMPLANTADO) 
						VALUES
							(
								@TB_UO_ID,
								@TB_UGE_CODIGO,
								@TB_UGE_DESCRICAO_NOVA,
								0,
								1,
								1,
								1
							)
							
							INSERT INTO
								TB_TMP_CARGA_LOG 
							VALUES
								(
									@MYID,
									@DS_MODULO,
									'N',
									'NOVO CÓDIGO: ' + CAST( @TB_UO_ID AS VARCHAR(6) ) + '/' + CAST( @TB_UGE_CODIGO AS VARCHAR(6) ) + ' - NOVA DESCRIÇÃO: ' + @TB_UGE_DESCRICAO_NOVA,
									GETDATE(),
									@QT_REGISTROS
								)
					END
			
			FETCH NEXT 
			FROM SIGEO_UGE_CURSOR INTO	@TB_UGE_CODIGO,
								        @TB_UGE_DESCRICAO,
										@TB_UO_ID
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
                'PROCESSAMENTO COM ERRO: ' + ERROR_MESSAGE() + CHAR(13)+CHAR(10) + ' - CÓDIGO: '+ CAST( @TB_UO_ID AS VARCHAR(6) ) + '/' + CAST( @TB_UGE_CODIGO AS VARCHAR(6) ) + ' - DESCRIÇÃO: ' + @TB_UGE_DESCRICAO_NOVA,
                GETDATE(),
                @QT_REGISTROS
            )
			SET @_RETURN = -1;
    END CATCH 					

    CLOSE SIGEO_UGE_CURSOR 					
    DEALLOCATE SIGEO_UGE_CURSOR 

	SET NOCOUNT OFF;

