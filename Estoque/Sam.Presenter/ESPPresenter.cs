using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.View;
using Sam.Entity;
using Sam.Common.Util;
using System.ComponentModel;
using Sam.Business;
using Sam.Infrastructure;
using System.Linq.Expressions;
using Sam.Business.Business.Seguranca;

namespace Sam.Presenter
{
    public class ESPPresenter : CrudPresenter<IESPView>, ITipoOperacao
    {
        #region [ View ]
        private IESPView _view;

        public new IESPView View
        {
            get { return _view; }
            set { _view = value; }
        }
        #endregion

        #region [ Operacao ]
        private enTipoOperacao _operacao;

        public enTipoOperacao Operacao
        {
            get { return this._operacao; }
            set { _operacao = value; }
        }
        #endregion

        #region [ Construtor ]
        public ESPPresenter()
        {

        }

        public ESPPresenter(IESPView view)
            : base(view)
        {
            this.View = view;
        }

        public ESPPresenter(IESPView _view, enTipoOperacao tipoOperacao)
            : base(_view)
        {
            this.View = _view;
            this.Operacao = tipoOperacao;
        }
        #endregion

        #region [ ListarESP ]
        public List<ESPEntity> ListarESP(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            ESPBusiness business = new ESPBusiness();
            business.SkipRegistros = startRowIndexParameterName;

            var result = business.ListarESP();
            this.TotalRegistrosGrid = business.TotalRegistros;

            return result;
        }
        #endregion

        #region [ Gravar ]
        public void Gravar()
        {
            ESPBusiness business = new ESPBusiness();
            TB_ESP ESP = new TB_ESP();

            try
            {
                if (!ValidarDadosView(_view))
                    return;

                ESP.TB_ESP_ID = (_operacao == enTipoOperacao.Update) ? _view.ID : 0;
                ESP.TB_ESP_CODIGO = _view.EspCodigo;
                ESP.TB_ESP_SISTEMA = _view.EspSistema;
                ESP.TB_GESTOR_ID = _view.GestorId;
                ESP.TB_ESP_INICIO_VIGENCIA = _view.DataInicioVigencia;
                ESP.TB_ESP_FIM_VIGENCIA = _view.DataFimVigencia ?? DateTime.MinValue ;
                ESP.TB_ESP_QTDE_REPOSITORIO_PRINCIPAL = _view.QtdeRepositorioPrincipal;
                ESP.TB_ESP_QTDE_REPOSITORIO_COMPLEMENTAR = _view.QtdeRepositorioComplementar;
                ESP.TB_ESP_QTDE_USUARIO_NIVEL_I = _view.QtdeUsuarioNivelI;
                ESP.TB_ESP_QTDE_USUARIO_NIVEL_II = _view.QtdeUsuarioNivelII;
                ESP.TB_ESP_TERMO = _view.TermoId;

                if (!Validar(ESP))
                    return;

                if (_operacao == enTipoOperacao.Update)
                    business.Update(ESP);   //update
                else
                    business.Insert(ESP);   //insert

                this.View.ExibirMensagem("Registro Salvo Com Sucesso.");
                GravadoSucesso();
            }
            catch (Exception ex)
            {
                business.ListaErro.Add(ex.Message);
                this.View.ListaErros = business.ListaErro;
            }
        }
        #endregion

        #region [ Validar ]
        private bool ValidarDadosView(IESPView viewESP)
        {
            List<string> erros = new List<string>();

            if (viewESP.EspCodigo == 0)
                erros.Add("O campo ESP não pode receber o valor Zero");

            if (viewESP.DataInicioVigencia == null || viewESP.DataInicioVigencia == DateTime.MinValue)
                erros.Add("O campo Data Início Vigência é de preenchimento obrigatório");

            if (viewESP.DataFimVigencia == null || viewESP.DataFimVigencia == DateTime.MinValue)
                erros.Add("O campo Data Fim Vigência é de preenchimento obrigatório");

            if (erros.Count > 0)
            {
                this.View.ExibirMensagem("Registro com Inconsistências, verificar mensagens!");
                this.View.ListaErros = erros;
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool Validar(TB_ESP tbESP)
        {
            List<string> erros = new List<string>();

            if (tbESP.TB_ESP_CODIGO == 0)
                erros.Add("O campo ESP não pode receber o valor Zero");

            if (erros.Count > 0)
            {
                this.View.ExibirMensagem("Registro com Inconsistências, verificar mensagens!");
                this.View.ListaErros = erros;
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region [ Excluir ]
        public void Excluir()
        {
            TB_ESP _esp = new TB_ESP();
            ESPBusiness business = new ESPBusiness();

            try
            {
                _esp.TB_ESP_ID = (short)_view.ID;

                if (_esp.TB_ESP_ID > 0)
                {

                    business.Delete(_esp);

                    this.View.ExibirMensagem("Registro Salvo Com Sucesso.");
                    ExcluidoSucesso();
                }
            }
            catch (Exception ex)
            {
                business.ListaErro.Add(ex.Message);
                this.View.ListaErros = business.ListaErro;
            }
        }
        #endregion

        #region [ TotalRegistros ]
        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            return this.TotalRegistrosGrid;
        }
        #endregion

        #region [ Override Métodos da Classe Base ]


        #region [ Cancelar ]
        public override void Cancelar()
        {
            this.View.BloqueiaGravar = true;
            this.View.BloqueiaCancelar = true;
            this.View.BloqueiaNovo = false;
            this.View.Id = null;
            this.View.Codigo = string.Empty;
            this.View.Descricao = string.Empty;
            this.View.MostrarPainelEdicao = false;
        }
        #endregion

        #region [ GravadoSucesso ]
        public override void GravadoSucesso()
        {
            this.View.BloqueiaGravar = true;
            this.View.BloqueiaExcluir = true;
            this.View.BloqueiaCancelar = true;
            this.View.BloqueiaCodigo = true;
            this.View.BloqueiaDescricao = true;
            this.View.BloqueiaNovo = false;
            this.View.Codigo = string.Empty;
            this.View.Descricao = string.Empty;
            this.View.Id = null;
            this.View.ListaErros = null;
            this.View.MostrarPainelEdicao = false;
            this.View.PopularGrid();
        }
        #endregion

        #region [ Load ]
        public override void Load()
        {
            this.View.BloqueiaGravar = true;
            this.View.BloqueiaExcluir = true;
            this.View.BloqueiaCancelar = true;
            this.View.BloqueiaNovo = false;
            this.View.MostrarPainelEdicao = false;
        }
        #endregion

        #region [ Novo ]
        public override void Novo()
        {
            this.View.BloqueiaGravar = false;
            this.View.BloqueiaCancelar = false;
            this.View.Descricao = string.Empty;
            this.Limpar();
            this.View.MostrarPainelEdicao = true;
            this.View.BloqueiaNovo = true;
        }
        #endregion

        #region [ ExcluidoSucesso ]
        public override void ExcluidoSucesso()
        {
            this.View.BloqueiaGravar = false;
            this.View.BloqueiaExcluir = false;
            this.View.BloqueiaCancelar = false;
            this.View.BloqueiaCodigo = false;
            this.View.BloqueiaDescricao = false;
            this.View.BloqueiaNovo = false;
            this.View.Codigo = string.Empty;
            this.View.Descricao = string.Empty;
            this.View.Id = null;
            this.View.ListaErros = null;
            this.View.MostrarPainelEdicao = false;
            this.View.PopularGrid();
        }
        #endregion

        #endregion

        #region [ Limpar ]
        public void Limpar()
        {
            this.View.ID = default(int);
            this.View.EspCodigo = default(int);
            this.View.EspSistema = default(string);
            this.View.GestorId = default(int);
            this.View.QtdeRepositorioPrincipal = default(int);
            this.View.QtdeRepositorioComplementar = default(int);
            this.View.QtdeUsuarioNivelI = default(int);
            this.View.QtdeUsuarioNivelII = default(int);
            this.View.TermoId = default(int);
            this.View.DataInicioVigencia = null;
            this.View.DataFimVigencia = null;
        }
        #endregion

        public override void RegistroSelecionado()
        {
            base.RegistroSelecionado();
            View.BloqueiaExcluir = false;

            var _esp = SelectOne(Int32.Parse(this._view.Id));

            if (_esp != null)
            {
                this._view.ID = _esp.TB_ESP_ID;
                this._view.EspCodigo = _esp.TB_ESP_CODIGO.Value;
                this._view.EspSistema = _esp.TB_ESP_SISTEMA;
                this._view.GestorId = _esp.TB_GESTOR_ID;
                this._view.DataInicioVigencia = _esp.TB_ESP_INICIO_VIGENCIA;
                this._view.DataFimVigencia = _esp.TB_ESP_FIM_VIGENCIA;
                this._view.QtdeRepositorioPrincipal = _esp.TB_ESP_QTDE_REPOSITORIO_PRINCIPAL;
                this._view.QtdeRepositorioComplementar = _esp.TB_ESP_QTDE_REPOSITORIO_COMPLEMENTAR;
                this._view.QtdeUsuarioNivelI = _esp.TB_ESP_QTDE_USUARIO_NIVEL_I;
                this._view.QtdeUsuarioNivelII = _esp.TB_ESP_QTDE_USUARIO_NIVEL_II;
                this._view.TermoId = (byte)_esp.TB_ESP_TERMO;

                this._view.BloqueiaGravar = false;
                this._view.BloqueiaNovo = true;
                this._view.BloqueiaCancelar = false;
            }
        }


        public TB_ESP SelectOne(int id)
        {
            ESPBusiness business = new ESPBusiness();
            Expression<Func<TB_ESP, bool>> where = a => a.TB_ESP_ID == id;

            return business.SelectOne(where);
        }

    }
}
