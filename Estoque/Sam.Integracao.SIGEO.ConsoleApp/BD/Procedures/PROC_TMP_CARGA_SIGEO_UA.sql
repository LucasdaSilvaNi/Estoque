	alter PROCEDURE [dbo].[PROC_TMP_CARGA_SIGEO_UA] 
		@_MYID UNIQUEIDENTIFIER,
		@_RETURN SMALLINT OUTPUT
		AS 		
		--
        -- STORED PROCEDURE
        --     PROC_TMP_CARGA_SIGEO_UA.
        --
        -- DESCRIPTION
        --     RESPONSÁVEL PELAS CARGAS DO UA.
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
        --     15/04/2021 - VERSÃO INICIAL
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
DECLARE @TB_UA_CODIGO				INT,
	    @TB_UGE_ID				    INT,
		@TB_GESTOR_ID				INT,
        @TB_UA_DESCRICAO			VARCHAR(150),
        @TB_UA_DESCRICAO_NOVA		VARCHAR(150) 

SET @MYID = @_MYID

DECLARE SIGEO_UA_CURSOR CURSOR FOR 						
        SELECT DISTINCT
            CS.TB_UA_CODIGO,
            CS.TB_UA_DESCRICAO,
			--CS.TB_UGE_CODIGO,
			G.TB_GESTOR_ID

        FROM
            TB_TMP_CARGA_SIGEO CS INNER JOIN TB_GESTOR G ON 
			    --CS.TB_UGE_CODIGO   = G.TB_UGE_ID    --O CAMPO ESTA NULL
			    CS.TB_UO_CODIGO    = G.TB_UO_ID 
			AND CS.TB_ORGAO_CODIGO = G.TB_ORGAO_ID  

        ORDER BY
            CS.TB_UA_CODIGO

    BEGIN TRY 

	    SET @DS_MODULO = 'CARGA SIGEO - UA' 	
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
            OPEN SIGEO_UA_CURSOR FETCH NEXT 
            FROM SIGEO_UA_CURSOR INTO	@TB_UA_CODIGO,
										@TB_UA_DESCRICAO ,
										@TB_UGE_ID,
										@TB_GESTOR_ID
										 
            WHILE @@FETCH_STATUS = 0 
            BEGIN

				SET
					@TB_UA_DESCRICAO_NOVA = RTRIM(LTRIM(@TB_UA_DESCRICAO))

				IF EXISTS 
				(
					SELECT
						* 
					FROM
						TB_UA O 
					WHERE
						O.TB_UA_CODIGO = @TB_UA_CODIGO AND O.TB_UGE_ID = @TB_UGE_ID)

						BEGIN

							IF EXISTS 
							(
								SELECT
									* 
								FROM
									TB_UA O 
								WHERE
									(O.TB_UA_CODIGO = @TB_UA_CODIGO AND O.TB_UGE_ID = @TB_UGE_ID)
									AND TB_UA_DESCRICAO <> @TB_UA_DESCRICAO_NOVA
							)
							--
							BEGIN
														
								UPDATE
									TB_UA 
								SET
									TB_UA_DESCRICAO = @TB_UA_DESCRICAO_NOVA 
								WHERE
									TB_UA_CODIGO = @TB_UA_CODIGO AND TB_UGE_ID = @TB_UGE_ID		

								INSERT INTO
									TB_TMP_CARGA_LOG 
								VALUES
									(
										@MYID,
										@DS_MODULO,
										'A',
										'ANTES: [' + @TB_UA_DESCRICAO + '] - APÓS: [' + @TB_UA_DESCRICAO_NOVA + ']',
										GETDATE(),
										@QT_REGISTROS
									)
							END
						END
						
				ELSE

					BEGIN
						--NOVO REGISTRO

						INSERT INTO	TB_UA (	 
								TB_UGE_ID
								,TB_UA_CODIGO	
								,TB_UA_DESCRICAO
								,TB_UA_VINCULADA
								,TB_UA_INDICADOR_ATIVIDADE
								,TB_UNIDADE_ID
								,TB_CENTRO_CUSTO_ID
								,TB_GESTOR_ID) 
						VALUES
							(
								@TB_UGE_ID,
								@TB_UA_CODIGO,
								@TB_UA_DESCRICAO_NOVA,
								NULL,
								0,
								NULL,
								NULL,
								@TB_GESTOR_ID
							)
							
							INSERT INTO
								TB_TMP_CARGA_LOG 
							VALUES
								(
									@MYID,
									@DS_MODULO,
									'N',
									'NOVO CÓDIGO: ' + CAST( @TB_UGE_ID AS VARCHAR(6) ) + '/' + CAST( @TB_UA_CODIGO AS VARCHAR(6) ) + ' - NOVA DESCRIÇÃO: ' + @TB_UA_DESCRICAO_NOVA,
									GETDATE(),
									@QT_REGISTROS
								)
					END
			
			FETCH NEXT 
			FROM SIGEO_UA_CURSOR INTO	@TB_UA_CODIGO,
								        @TB_UA_DESCRICAO,
										@TB_UGE_ID,
										@TB_GESTOR_ID
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
                'PROCESSAMENTO COM ERRO: ' + ERROR_MESSAGE() + CHAR(13)+CHAR(10) + ' - CÓDIGO: '+ CAST( @TB_UGE_ID AS VARCHAR(6) ) + '/' + CAST( @TB_UA_CODIGO AS VARCHAR(6) ) + ' - DESCRIÇÃO: ' + @TB_UA_DESCRICAO_NOVA,
                GETDATE(),
                @QT_REGISTROS
            )
			SET @_RETURN = -1;
    END CATCH 					

    CLOSE SIGEO_UA_CURSOR 					
    DEALLOCATE SIGEO_UA_CURSOR 

	SET NOCOUNT OFF;

