using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using Sam.Common.Util;


namespace Sam.Business.Importacao
{
    public class ImportacaoPerfil
    {

        public bool InsertImportacao(Infrastructure.TB_CONTROLE entityList)
        {
            try
            {
                bool isErro = new PerfilBusiness().InsertListControleImportacao(entityList);
                AtualizaControle(isErro, entityList.TB_CONTROLE_ID);
                return isErro;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        /// <summary>
        /// Atualiza o status do controle conforme o retorno do erro
        /// </summary>
        /// <param name="isErro">Se retornou erro</param>
        /// <param name="controleId">Id do controle</param>
        protected void AtualizaControle(bool isErro, int controleId)
        {
            //Busca o objeto atualizado para fazer o update
            var controleUpdate = new ControleBusiness().SelectOne(a => a.TB_CONTROLE_ID == controleId);

            if (isErro)
                controleUpdate.TB_CONTROLE_SITUACAO_ID = (int)GeralEnum.ControleSituacao.FinalizadaErro;
            else
                controleUpdate.TB_CONTROLE_SITUACAO_ID = (int)GeralEnum.ControleSituacao.FinalizadoSucesso;

            new ControleBusiness().Update(controleUpdate);
        }

    }
}
