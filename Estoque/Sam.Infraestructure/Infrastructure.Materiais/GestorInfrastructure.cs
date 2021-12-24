using Sam.Domain.Entity;
using Sam.Common.Util;
using System.Collections.Generic;
using System.Linq;


namespace Sam.Infrastructure
{
    public class GestorInfrastructure : AbstractCrud<TB_GESTOR, SAMwebEntities>
    {
        internal GestorEntity MapearDTO(TB_GESTOR rowTabela)
        {
            GestorEntity objEntidade = new GestorEntity();

            objEntidade.Id                  = rowTabela.TB_GESTOR_ID;
            objEntidade.Nome                = rowTabela.TB_GESTOR_NOME;
            objEntidade.NomeReduzido        = rowTabela.TB_GESTOR_NOME_REDUZIDO;
            objEntidade.EnderecoLogradouro  = rowTabela.TB_GESTOR_LOGRADOURO;
            objEntidade.EnderecoNumero      = rowTabela.TB_GESTOR_NUMERO;
            objEntidade.EnderecoCompl       = rowTabela.TB_GESTOR_COMPLEMENTO;
            objEntidade.EnderecoTelefone    = rowTabela.TB_GESTOR_TELEFONE;
            objEntidade.CodigoGestao        = rowTabela.TB_GESTOR_CODIGO_GESTAO;

            return objEntidade;
        }
        internal TB_GESTOR MapearEntity(GestorEntity objEntidade)
        {
            TB_GESTOR rowTabela = new TB_GESTOR();


            rowTabela.TB_GESTOR_NOME           = objEntidade.Nome;
            rowTabela.TB_GESTOR_NOME_REDUZIDO  = objEntidade.NomeReduzido;
            rowTabela.TB_GESTOR_LOGRADOURO     = objEntidade.EnderecoLogradouro;
            rowTabela.TB_GESTOR_NUMERO         = objEntidade.EnderecoNumero;
            rowTabela.TB_GESTOR_COMPLEMENTO    = objEntidade.EnderecoCompl;
            rowTabela.TB_GESTOR_TELEFONE       = objEntidade.EnderecoTelefone;

            if (objEntidade.CodigoGestao.HasValue)
                rowTabela.TB_GESTOR_CODIGO_GESTAO = objEntidade.CodigoGestao.Value;

            rowTabela.TB_ORGAO_ID = objEntidade.Orgao.Id.Value;
            rowTabela.TB_UGE_ID   = objEntidade.Uge.Id.HasValue ? objEntidade.Uge.Id : null;
            rowTabela.TB_UO_ID    = objEntidade.Uo.Id.HasValue ? objEntidade.Uo.Id : null;

            if (objEntidade.Logotipo != null)
                rowTabela.TB_GESTOR_IMAGEM = objEntidade.Logotipo;

            return rowTabela;
        }

        public GestorEntity ObterID(int rowTabelaID)
        {
            GestorEntity objRetorno = null;

            var rowTabela = this.SelectOne(Gestor => Gestor.TB_GESTOR_ID == rowTabelaID);
            if (rowTabela.IsNotNull())
                objRetorno = MapearDTO(rowTabela);

            return objRetorno;
        }

        public IList<TB_GESTOR> ListarGestor(int OrgaoId)
        {
            IQueryable<TB_GESTOR> resultado = (from a in Context.TB_GESTOR select a);
            var retorno = resultado.Cast<TB_GESTOR>().ToList();

            var retornoLista = (from r in retorno
                                where r.TB_ORGAO_ID == OrgaoId
                                group r by new
                                {
                                    r.TB_GESTOR_ID,
                                    r.TB_GESTOR_NOME_REDUZIDO,
                                    r.TB_GESTOR_NOME,
                                } into am
                                select new TB_GESTOR()
                                {
                                    TB_GESTOR_ID = am.Key.TB_GESTOR_ID,
                                    TB_GESTOR_NOME_REDUZIDO = am.Key.TB_GESTOR_NOME_REDUZIDO,
                                    TB_GESTOR_NOME = am.Key.TB_GESTOR_NOME,
                                });

            return retornoLista.OrderBy(a => a.TB_GESTOR_NOME_REDUZIDO).Cast<TB_GESTOR>().ToList();
        }
    }
}
