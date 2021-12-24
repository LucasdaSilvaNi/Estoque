using System;
using System.Collections.Generic;
using System.Linq;
using Sam.Business.MovimentoFactory;
using Sam.Common.Util;
using Sam.Domain.Business;
using Sam.Domain.Entity;
using Sam.View;
using Sam.Infrastructure;
using System.Linq.Expressions;

namespace Sam.Presenter
{
    public class ConsultarEntradaMaterialPresenter : CrudPresenter<IConsultarEntradaView>
    {
        IConsultarEntradaView view;

        public IConsultarEntradaView View
        {
            get { return view; }
            set { view = value; }
        }

        public ConsultarEntradaMaterialPresenter(IConsultarEntradaView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public ConsultarEntradaMaterialPresenter()
        {
        }

        public void HabilitarControlesEdit()
        {
            this.View.BloqueiaEstornar = false;
            this.View.BloqueiaNovo = false;
            this.View.BloqueiaCancelar = false;
            this.View.BloqueiaNotaFornecimento = false;
        }

        #region Metodos


        public TB_MOVIMENTO SelectOneMovimento(int movimentoId)
        {
            Business.MovimentoBusiness business = new Business.MovimentoBusiness();
            return business.SelectOne(a => a.TB_MOVIMENTO_ID == movimentoId);
        }

        public IList<TB_MOVIMENTO> ConsultarMovimentos(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            var business = new Business.MovimentoBusiness();
            TB_MOVIMENTO mov = new TB_MOVIMENTO();

            int almoxId = (int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id;
            string mesRef = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;

            Expression<Func<TB_MOVIMENTO, bool>> where;
            //Montando Where
            if (Acesso.Transacoes.Perfis[0].IdPerfil == (int)GeralEnum.TipoPerfil.AdministradorGestor)
                where = a => a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID == almoxId
                && ((a.TB_MOVIMENTO_NUMERO_DOCUMENTO == this.View.MOVIMENTO_NUMERO_DOCUMENTO) || (this.View.MOVIMENTO_NUMERO_DOCUMENTO == string.Empty))
                && ((a.TB_MOVIMENTO_EMPENHO == this.View.EMPENHO_COD) || (this.View.EMPENHO_COD == string.Empty))
                && (a.TB_UGE_ID == this.View.UGE_ID || this.View.UGE_ID == 0)
                && ((a.TB_MOVIMENTO_DATA_DOCUMENTO >= this.View.MOVIMENTO_DATA_DOCUMENTO && a.TB_MOVIMENTO_DATA_DOCUMENTO <= this.View.MOVIMENTO_DATA_DOCUMENTO_ATE))
                && ((a.TB_MOVIMENTO_DATA_MOVIMENTO >= this.View.MOVIMENTO_DATA_MOVIMENTO && a.TB_MOVIMENTO_DATA_MOVIMENTO <= this.View.MOVIMENTO_DATA_MOVIMENTO_ATE))
                && ((a.TB_TIPO_MOVIMENTO_ID == this.View.TIPO_MOVIMENTO_ID) || (this.View.TIPO_MOVIMENTO_ID == 0))
                && (a.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada)//Apenas movimentos de entrada
                && (!a.TB_MOVIMENTO_ATIVO == this.View.Estornado); //Inativo é estornado
            else
                where = a => a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID == almoxId
                && (a.TB_MOVIMENTO_ANO_MES_REFERENCIA == mesRef)
                && ((a.TB_MOVIMENTO_NUMERO_DOCUMENTO == this.View.MOVIMENTO_NUMERO_DOCUMENTO) || (this.View.MOVIMENTO_NUMERO_DOCUMENTO == string.Empty))
                && ((a.TB_MOVIMENTO_EMPENHO == this.View.EMPENHO_COD) || (this.View.EMPENHO_COD == string.Empty))
                && (a.TB_UGE_ID == this.View.UGE_ID || this.View.UGE_ID == 0)
                && ((a.TB_MOVIMENTO_DATA_DOCUMENTO >= this.View.MOVIMENTO_DATA_DOCUMENTO && a.TB_MOVIMENTO_DATA_DOCUMENTO <= this.View.MOVIMENTO_DATA_DOCUMENTO_ATE))
                && ((a.TB_MOVIMENTO_DATA_MOVIMENTO >= this.View.MOVIMENTO_DATA_MOVIMENTO && a.TB_MOVIMENTO_DATA_MOVIMENTO <= this.View.MOVIMENTO_DATA_MOVIMENTO_ATE))
                && ((a.TB_TIPO_MOVIMENTO_ID == this.View.TIPO_MOVIMENTO_ID) || (this.View.TIPO_MOVIMENTO_ID == 0))
                && (a.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada)//Apenas movimentos de entrada
                && (!a.TB_MOVIMENTO_ATIVO == this.View.Estornado); //Inativo é estornado

            var result = business.ConsultaMovimento(startRowIndexParameterName, where);
            result.OrderBy(a => a.TB_MOVIMENTO_DATA_MOVIMENTO);
            this.TotalRegistrosGrid = business.TotalRegistros;
            return result;
        }

        public IList<UGEEntity> PopularListaUGE(int _orgaoId, int _gestorId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<UGEEntity> retorno = estrutura.ListarTodosCodPorGestor(_gestorId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public MovimentoEntity Estornar(MovimentoEntity mov)
        {
            var perfilLogado_ID = Acesso.Transacoes.Perfis[0].IdLogin;
            MovimentoBusiness estrutura = new MovimentoBusiness();

            estrutura.Movimento = mov;

            // atualizar saldo do BD via SALDO_SUBITEM
            //if (!estrutura.EstornarMovimentoEntrada())
            if (!estrutura.EstornarMovimentoEntrada(perfilLogado_ID, mov.InscricaoCE).Item1)
            {
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                this.View.ListaErros = estrutura.ListaErro;
                return null;
            }

            this.View.ExibirMensagem("Estorno de materiais concluído com sucesso.");
            this.View.PopularGrid();            
            return mov;
        }

        #endregion

        public override void Load()
        {
            this.View.BloqueiaNovo = true;
            
            this.View.MostrarPainelEdicao = false;            
        }

        public override void Cancelar()
        {            
            base.Cancelar();
            this.View.BloqueiaNovo = false;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, string _documento)
        {
            return this.TotalRegistrosGrid;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, IConsultarEntradaView ConsultaView)
        {
            return this.TotalRegistrosGrid;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            return this.TotalRegistrosGrid;
        }
        
        public void Imprimir()
        {
            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.EntradaMaterial;
            relatorioImpressao.Nome = "AlmoxEntradaMaterial.rdlc";
            relatorioImpressao.DataSet = "dsEntradaMaterial";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

        public void Imprimir(System.Collections.SortedList ParametrosRelatorio)
        {
            //RelatorioEntity.Id = (int)RelatorioEnum.EntradaMaterial;
            //RelatorioEntity.Nome = "AlmoxEntradaMaterial.rdlc";
            //RelatorioEntity.DataSet = "dsEntradaMaterial";
            //RelatorioEntity.Parametros = ParametrosRelatorio;            

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.EntradaMaterial;
            relatorioImpressao.Nome = "AlmoxEntradaMaterial.rdlc";
            relatorioImpressao.DataSet = "dsEntradaMaterial";

            relatorioImpressao.Parametros = ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;
        }
    }
}
