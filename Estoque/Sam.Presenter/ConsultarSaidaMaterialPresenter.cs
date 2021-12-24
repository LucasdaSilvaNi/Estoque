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
using Sam.Business;

namespace Sam.Presenter
{
    public class ConsultarSaidaMaterialPresenter : CrudPresenter<IConsultarSaidaView>
    {
        IConsultarSaidaView view;

        public IConsultarSaidaView View
        {
            get { return view; }
            set { view = value; }
        }

        public ConsultarSaidaMaterialPresenter(IConsultarSaidaView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public ConsultarSaidaMaterialPresenter()
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
                && (a.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)//Apenas movimentos de entrada
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
                && (a.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)//Apenas movimentos de entrada
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

        public void Estornar(MovimentoEntity movimento)
        {
            var perfilLogado_ID = Acesso.Transacoes.Perfis[0].IdLogin;
            Sam.Domain.Business.MovimentoBusiness movimentoBusiness = new Sam.Domain.Business.MovimentoBusiness();

            movimentoBusiness.Movimento = movimento;            
            movimentoBusiness.Movimento.Almoxarifado.Id = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id ?? 0;

            //if (!movimentoBusiness.EstornarMovimentoSaida())
            if (!movimentoBusiness.EstornarMovimentoSaida(perfilLogado_ID, movimento.InscricaoCE).Item1)
            {
                // validar (não salva em banco)
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                this.View.ListaErros = movimentoBusiness.ListaErro;
            }
            else
            {
                //Estorno com sucesso
                this.View.ExibirMensagem("Estorno de materiais concluído com sucesso.");
                this.Cancelar();
            }
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
            //RelatorioEntity.Id = (int)RelatorioEnum.SaidaMaterial;
            //RelatorioEntity.Nome = "AlmoxSaidaMaterial.rdlc";
            //RelatorioEntity.DataSet = "dsSaidaMaterial";
            //RelatorioEntity.Parametros = this.View.ParametrosRelatorio;

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.SaidaMaterial;
            relatorioImpressao.Nome = "AlmoxSaidaMaterial.rdlc";
            relatorioImpressao.DataSet = "dsSaidaMaterial";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

        public void Imprimir(System.Collections.SortedList ParametrosRelatorio)
        {
            //RelatorioEntity.Id = (int)RelatorioEnum.SaidaMaterial;
            //RelatorioEntity.Nome = "AlmoxSaidaMaterial.rdlc";
            //RelatorioEntity.DataSet = "dsSaidaMaterial";
            //RelatorioEntity.Parametros = this.View.ParametrosRelatorio;

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.SaidaMaterial;
            relatorioImpressao.Nome = "AlmoxSaidaMaterial.rdlc";
            relatorioImpressao.DataSet = "dsSaidaMaterial";

            relatorioImpressao.Parametros = ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();         
        }

        public IList<TB_SERIALIZACAO> ConsultarRascunhos(int p)
        {
            SerializacaoBusiness business = new SerializacaoBusiness();

            int idLogin = Acesso.Transacoes.Perfis[0].IdLogin;
            return business.SelectWhere(a => a.TB_LOGIN_ID == idLogin);
        }

        public void ExcluirRascunho(TB_SERIALIZACAO serializacao)
        {
            Sam.Business.SerializacaoBusiness serializacaoBusiness = new Sam.Business.SerializacaoBusiness();
            serializacaoBusiness.Delete(serializacao);
            this.View.ExibirMensagem("Rascunho excluido com sucesso!");
            this.view.PopularGrid();
        }
    }
}
