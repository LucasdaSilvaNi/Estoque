/****** Object:  StoredProcedure [dbo].[PROC_TMP_CARGA_SIGEO]    Script Date: 13/04/2021 15:18:27 ******/
DROP PROCEDURE [dbo].[PROC_TMP_CARGA_SIGEO]
GO

/****** Object:  StoredProcedure [dbo].[PROC_TMP_CARGA_SIGEO]    Script Date: 13/04/2021 15:18:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

		CREATE PROCEDURE [dbo].[PROC_TMP_CARGA_SIGEO] 
		AS 		
		--
        -- STORED PROCEDURE
        --    PROC_TMP_CARGA_SIGEO.
        --
        -- DESCRIPTION
        --     RESPONSÁVEL PELAS CARGAS DA ORGANIZAÇÃO.
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
        --     06/04/2021 - VERSÃO INICIAL
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

DECLARE @MYID						UNIQUEIDENTIFIER = NEWID();
DECLARE @DT_PROC_INICIAL			TIMESTAMP;
DECLARE @DT_PROC_FINAL				TIMESTAMP;
DECLARE @QT_REGISTROS				INT = 0;
DECLARE @DS_MODULO					VARCHAR(50);
DECLARE @TB_ORGAO_CODIGO			INT,
        @TB_ORGAO_DESCRICAO			VARCHAR(150),
        @TB_ORGAO_DESCRICAO_NOVA	VARCHAR(150) 

DECLARE @_RETURN SMALLINT;

DECLARE SIGEO_ORGAO_CURSOR CURSOR FOR 						
        SELECT DISTINCT
            CS.TB_ORGAO_CODIGO,
            CS.TB_ORGAO_DESCRICAO 
        FROM
            TB_TMP_CARGA_SIGEO CS 
        ORDER BY
            CS.TB_ORGAO_CODIGO

    BEGIN TRY 

	    SET @DS_MODULO = 'CARGA SIGEO - ORGÃO' 	
        SET
            @QT_REGISTROS = 
            (
                SELECT
                    ISNULL(COUNT(*), 0) 
                FROM
                    TB_TMP_CARGA_SIGEO
            )
            --

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
            OPEN SIGEO_ORGAO_CURSOR FETCH NEXT 
            FROM SIGEO_ORGAO_CURSOR INTO @TB_ORGAO_CODIGO,
										 @TB_ORGAO_DESCRICAO 	
										 
            WHILE @@FETCH_STATUS = 0 
            BEGIN

				SET
					@TB_ORGAO_DESCRICAO_NOVA = RTRIM(LTRIM(@TB_ORGAO_DESCRICAO))

				IF EXISTS 
				(
					SELECT
						* 
					FROM
						TB_ORGAO O 
					WHERE
						O.TB_ORGAO_CODIGO = @TB_ORGAO_CODIGO)

						BEGIN

							IF EXISTS 
							(
								SELECT
									* 
								FROM
									TB_ORGAO O 
								WHERE
									O.TB_ORGAO_CODIGO = @TB_ORGAO_CODIGO 
									AND TB_ORGAO_DESCRICAO <> @TB_ORGAO_DESCRICAO_NOVA
							)
							--
							BEGIN
														
								UPDATE
									TB_ORGAO 
								SET
									TB_ORGAO_DESCRICAO = @TB_ORGAO_DESCRICAO_NOVA 
								WHERE
									TB_ORGAO_CODIGO = @TB_ORGAO_CODIGO 		

								INSERT INTO
									TB_TMP_CARGA_LOG 
								VALUES
									(
										@MYID,
										@DS_MODULO,
										'A',
										'ANTES: [' + @TB_ORGAO_DESCRICAO + '] - APÓS: [' + @TB_ORGAO_DESCRICAO_NOVA + ']',
										GETDATE(),
										@QT_REGISTROS
									)
							END
						END
						
				ELSE

					BEGIN
						--NOVO REGISTRO

						INSERT INTO
							TB_ORGAO (TB_ORGAO_CODIGO , TB_ORGAO_DESCRICAO , TB_ORGAO_STATUS , TB_ORGAO_IMPLANTADO , TB_ORGAO_INTEGRACAO_SIAFEM) 
						VALUES
							(
								@TB_ORGAO_CODIGO,
								@TB_ORGAO_DESCRICAO_NOVA,
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
									'NOVO CÓDIGO: ' + CAST( @TB_ORGAO_CODIGO AS VARCHAR(6) ) + ' - NOVA DESCRIÇÃO: ' + @TB_ORGAO_DESCRICAO_NOVA,
									GETDATE(),
									@QT_REGISTROS
								)
					END
			
			FETCH NEXT 
			FROM SIGEO_ORGAO_CURSOR INTO @TB_ORGAO_CODIGO,
								         @TB_ORGAO_DESCRICAO 
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
                'PROCESSAMENTO COM ERRO: ' + ERROR_MESSAGE() + CHAR(13)+CHAR(10) + ' - CÓDIGO: '+ CAST( @TB_ORGAO_CODIGO AS VARCHAR(6) ) + ' - DESCRIÇÃO: ' + @TB_ORGAO_DESCRICAO_NOVA,
                GETDATE(),
                @QT_REGISTROS
            )
			SET @_RETURN = -1;
    END CATCH 					

    CLOSE SIGEO_ORGAO_CURSOR 					
    DEALLOCATE SIGEO_ORGAO_CURSOR 

	SET NOCOUNT OFF;

	IF @_RETURN = 1
	BEGIN
		SET @_RETURN = 0
		EXEC PROC_TMP_CARGA_SIGEO_UO @MYID, @_RETURN;
	END

	IF @_RETURN = 1
	BEGIN
		SET @_RETURN = 0
		EXEC PROC_TMP_CARGA_SIGEO_UGE @MYID, @_RETURN;
	END

		IF @_RETURN = 1
	BEGIN
		SET @_RETURN = 0
		EXEC PROC_TMP_CARGA_SIGEO_UA @MYID, @_RETURN;
	END


	GO
