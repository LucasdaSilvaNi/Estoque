using System.Collections.Generic;
using Sam.View;
using Sam.Domain.Business;
using Sam.Domain.Entity;
using tipoPesquisa = Sam.Common.Util.GeralEnum.TipoPesquisa;
using Sam.Business.Business.Seguranca;
using System.Data;
using System;
using Sam.Business.ResumoSituacaoBusiness;

namespace Sam.Presenter
{
    public class ResumoSituacaoPresenter : CrudPresenter<IResumoSituacaoView>
    {
        public IList<OrgaoEntity> PopularListaOrgaoTodosCod()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<OrgaoEntity> lista = estrutura.ListarOrgaosTodosCod();
            return lista;
        }
        public IList<AlmoxarifadoEntity> ListaAlmoxarifadoPorOgao(int orgaoId)
        {
            IList<AlmoxarifadoEntity> lista = null;
            ResumoSituacaoBusiness eoBusiness = null;
            eoBusiness = new ResumoSituacaoBusiness();
            lista = eoBusiness.ListarAlmoxarifadoPorOrgao(orgaoId);
            return lista;
        }        
        public IList<MovimentoEntity> ListarRequisicaoByPendete(int maximumRowsParameterName, int startRowIndexParameterName, int almoxarifadoId, int divisaoId, int tipoMovimento, int tipoDeOperacao)
        {
            string mesRef = null;
            bool requisicoesParaEstorno = false;

            if (tipoDeOperacao == (int)Common.Util.GeralEnum.TipoRequisicao.Estorno)
            {
                mesRef = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;
                requisicoesParaEstorno = true;
            }
            MovimentoBusiness estrutura = new MovimentoBusiness();
            IList<MovimentoEntity> retorno = estrutura.ListarRequisicaoByAlmoxarifado(almoxarifadoId, divisaoId, tipoMovimento, mesRef, requisicoesParaEstorno, "");
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }
        public IList<NotaLancamentoPendenteSIAFEMEntity> ListarNotasLancamentosPendentes(tipoPesquisa tipoPesquisa, long tabelaPesquisaID, bool? pendenciasAtivas = true)
        {
            NotaLancamentoPendenteSIAFEMBusiness objBusiness = null;
            IList<NotaLancamentoPendenteSIAFEMEntity> lstRetorno = null;

            objBusiness = new NotaLancamentoPendenteSIAFEMBusiness();
            lstRetorno = objBusiness.ListarNotasLancamentosPendentes(tipoPesquisa, tabelaPesquisaID, pendenciasAtivas);

            this.TotalRegistrosGrid = objBusiness.TotalRegistros;

            return lstRetorno;
        }
        public IList<SubItemMaterialEntity> ListarItensEstoque(int filtro, int idAlmoxarifado)
        {

            ResumoSituacaoBusiness estrutura = new ResumoSituacaoBusiness();
            IList<SubItemMaterialEntity> retorno = estrutura.ListarItensEstoque(filtro, idAlmoxarifado);
            return retorno;
        }
        public List<Entity.UsuarioLogadoEntity> ListarUsuariosOnlinePorOrgao(int orgaoId)
        {
            ResumoSituacaoBusiness business = new ResumoSituacaoBusiness();
            return business.ListarUsuariosOnlinePorOrgao(orgaoId);
        }
        public DataSet GerarExportacaoAlmoxarifadoStatusFechamento(int codigoOrgao, int idAlmoxarifado, string Periodo)
        {
            DataSet dsRetorno = new DataSet();
            try
            {
                ResumoSituacaoBusiness business = new ResumoSituacaoBusiness();
               
                dsRetorno = business.GerarExportacaoAlmoxarifadoStatusFechamento(codigoOrgao, idAlmoxarifado, Periodo);

                return dsRetorno;

            }
            catch (Exception)
            {

                throw;
            }
        }
        public OrgaoEntity ListarCodigoOrgao(int orgaoId)
        {
            OrgaoEntity lista = null;
            EstruturaOrganizacionalBusiness eoBusiness = null;
            eoBusiness = new EstruturaOrganizacionalBusiness();
            lista = eoBusiness.ListarCodigoOrgao(orgaoId);
            return lista;
        }
        public IList<AlmoxarifadoEntity> ListarAlmoxarifadoStatusFechamento(int idAlmoxarifado, int orgaoId)
        {
            IList<AlmoxarifadoEntity> lista = null;
            ResumoSituacaoBusiness eoBusiness = new ResumoSituacaoBusiness();
            lista = eoBusiness.ListarAlmoxarifadoStatusFechamento(idAlmoxarifado, orgaoId);
            return lista;
        }

        public IList<UGEEntity> ListarUgeImplantada(int idOrgao)
        {          
            IList<UGEEntity> lista = null;
            ResumoSituacaoBusiness eoBusiness = new ResumoSituacaoBusiness();
            lista = eoBusiness.ListarUgeImplantada(idOrgao);
            return lista;
        }
        public IList<AlmoxarifadoEntity> ListarInicioFechamento(int orgaoId)
        {
            IList<AlmoxarifadoEntity> lista = null;
            ResumoSituacaoBusiness eoBusiness = new ResumoSituacaoBusiness();
            lista = eoBusiness.ListarInicioFechamento(orgaoId);
            return lista;
        }
    }
}
