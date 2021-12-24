using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data.Linq;
using System.Web.UI.WebControls;
using Sam.View;
using Sam.Common.Util;
using Sam.Presenter;
using System.IO;
using Sam.Domain.Entity;
using Sam.Infrastructure;

namespace Sam.Web.Carga
{
    public partial class ImportarCarga : PageBase, ICargaView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                new CargaPresenter(this).Load();
            }
        }

        public void CarregarArquivoExcel()
        {
            CargaEntity cargaTable = new CargaEntity();
            //cargaTable.CaminhoDiretorio = this.Server.MapPath("arquivosPendentes\\");
            //cargaTable.CaminhoDiretorioDestino = this.Server.MapPath("arquivosHistorico\\");
            cargaTable.CaminhoDiretorio = string.Format(@"{0}{1}\", Constante.FullPhysicalPathApp, @"Carga\arquivosPendentes");
            cargaTable.CaminhoDiretorioDestino = string.Format(@"{0}{1}\", Constante.FullPhysicalPathApp, @"Carga\arquivosHistorico");
            cargaTable.NomeArquivo = fulExcel.PostedFile.FileName;
            cargaTable.tamanhoMax = 5000;
            cargaTable.ExtensaoList = new List<Extensao>();
            cargaTable.ExtensaoList.Add(new Extensao(".xls"));
            cargaTable.ExtensaoList.Add(new Extensao(".xlsx"));
            cargaTable.TipoArquivo = this.TipoArquivo;
            new CargaPresenter(this).CarregarArquivo(cargaTable, fulExcel);
        }

        public void RemoverCarga(int controleId)
        {
            new CargaPresenter(this).RemoverCarga(controleId);
        }

        public void ExecutarCarga(int controleId)
        {
            new CargaPresenter(this).ImportarArquivo(controleId);
        }

        public void ExportarErrosCarga(int controleId)
        {
            new CargaPresenter(this).ExportarErrosCarga(controleId);
        }

        public void ExportarCarga(int controleId)
        {
            new CargaPresenter(this).ExportarCarga(controleId);
        }

        public void LerArquivosPasta()
        {
        }

        protected void btnImportar_Click(object sender, EventArgs e)
        {
            CarregarArquivoExcel();
        }

        public string Id
        {
            get
            {
                return string.Empty;
            }
            set
            {

            }
        }

        public string Codigo
        {
            get
            {
                return string.Empty;
            }
            set
            {

            }
        }

        public string Descricao
        {
            get
            {
                return string.Empty;
            }
            set
            {

            }
        }

        public void PopularGrid()
        {
            grdPendentes.DataSourceID = "sourceGrid";
        }

        #region Mensagens de Erro

        public void ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);
        }

        public IList ListaErros
        {
            set
            {
                this.ListInconsistencias.ExibirLista(value);
                this.ListInconsistencias.DataBind();
            }
        }

        #endregion

        public bool BloqueiaNovo
        {
            set { btnNovo.Enabled = !value; }
        }

        public bool BloqueiaGravar
        {
            set { btnImportar.Enabled = !value; }
        }

        public bool BloqueiaExcluir
        {
            set { }
        }

        public bool BloqueiaCancelar
        {
            set { btnCancelar.Enabled = !value; }
        }

        public bool BloqueiaCodigo
        {
            set { }
        }

        public bool BloqueiaDescricao
        {
            set { }
        }

        public bool MostrarPainelEdicao
        {
            set
            {
                if (value == true)
                    pnlEditar.CssClass = "mostrarControle";
                else
                    pnlEditar.CssClass = "esconderControle";
            }
        }

        protected void grdPendentes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Excluir")
            {
                int controleId = Convert.ToInt32(e.CommandArgument.ToString());
                this.RemoverCarga(controleId);
            }
            if (e.CommandName == "Importar")
            {
                int controleId = Convert.ToInt32(e.CommandArgument.ToString());
                this.ExecutarCarga(controleId);
            }
            if (e.CommandName == "Erro")
            {

                int controleId = Convert.ToInt32(e.CommandArgument.ToString());
                this.ExportarErrosCarga(controleId);
            }
            if (e.CommandName == "Exportar")
            {
                int controleId = Convert.ToInt32(e.CommandArgument.ToString());
                this.ExportarCarga(controleId);
            }
        }

        public void ExportarToExcel(IEnumerable<TB_CARGA> listErros, IEnumerable<TB_CARGA_ERRO> listErroDescricao)
        {

            IList<CargaErroDescricao> listCargaErroGrid = new List<CargaErroDescricao>();
            List<CargaErro> listCargaGrid = new List<CargaErro>();
            List<CargaErroDivisao> listCargaGridD = new List<CargaErroDivisao>();
            List<CargaErroAlmoxarifado> listCargaGridA = new List<CargaErroAlmoxarifado>();
            List<CargaErroResponsavel> listCargaGridR = new List<CargaErroResponsavel>();
            List<CargaErroUsuario> listCargaGridU = new List<CargaErroUsuario>();
            List<CargaErroPerfilRequisitante> listCargaGridPR = new List<CargaErroPerfilRequisitante>();
            var grdExport2 = new GridView();
            var grdExport = new GridView();


            //Monta a entidade para a lista de erros
            foreach (var erro in listErros)
            {
                CargaErro cargaErro = new CargaErro();
                CargaErroDivisao cargaErroDivisao = new CargaErroDivisao();
                CargaErroAlmoxarifado cargaErroAlmox = new CargaErroAlmoxarifado();
                CargaErroResponsavel cargaErroResp = new CargaErroResponsavel();
                CargaErroUsuario cargaErroUsuario = new CargaErroUsuario();
                CargaErroPerfilRequisitante cargaErroPerfilRequisitante = new CargaErroPerfilRequisitante();

                switch (erro.TB_CONTROLE.TB_TIPO_CONTROLE_ID)
                {
                    case (int)GeralEnum.TipoControle.Divisao:

                        cargaErroDivisao.TB_CARGA_SEQ = erro.TB_CARGA_SEQ;
                        cargaErroDivisao.TB_GESTOR_NOME_REDUZIDO = erro.TB_GESTOR_NOME_REDUZIDO;
                        cargaErroDivisao.TB_RESPONSAVEL_CODIGO = erro.TB_RESPONSAVEL_CODIGO;
                        cargaErroDivisao.TB_ALMOXARIFADO_CODIGO = erro.TB_ALMOXARIFADO_CODIGO;
                        cargaErroDivisao.TB_DIVISAO_INDICADOR_ATIVIDADE = erro.TB_DIVISAO_INDICADOR_ATIVIDADE;
                        cargaErroDivisao.TB_UF_SIGLA = erro.TB_UF_SIGLA;
                        cargaErroDivisao.TB_UA_CODIGO = erro.TB_UA_CODIGO;
                        cargaErroDivisao.TB_DIVISAO_CODIGO = erro.TB_DIVISAO_CODIGO;
                        cargaErroDivisao.TB_DIVISAO_DESCRICAO = erro.TB_DIVISAO_DESCRICAO;
                        cargaErroDivisao.TB_DIVISAO_LOGRADOURO = erro.TB_DIVISAO_LOGRADOURO;

                        break;

                    case (int)GeralEnum.TipoControle.Responsavel:

                        cargaErroResp.TB_CARGA_SEQ = erro.TB_CARGA_SEQ;
                        cargaErroResp.TB_RESPONSAVEL_CODIGO = erro.TB_RESPONSAVEL_CODIGO;
                        cargaErroResp.TB_RESPONSAVEL_NOME = erro.TB_RESPONSAVEL_NOME;
                        cargaErroResp.TB_RESPONSAVEL_CARGO = erro.TB_RESPONSAVEL_CARGO;
                        cargaErroResp.TB_RESPONSAVEL_ENDERECO = erro.TB_RESPONSAVEL_ENDERECO;
                        cargaErroResp.TB_GESTOR_NOME_REDUZIDO = erro.TB_GESTOR_NOME_REDUZIDO;
                        

                        break;

                    case (int)GeralEnum.TipoControle.Almoxarifado:

                        cargaErroAlmox.TB_CARGA_SEQ = erro.TB_CARGA_SEQ;
                        cargaErroAlmox.TB_UGE_CODIGO = erro.TB_UGE_CODIGO;
                        cargaErroAlmox.TB_GESTOR_NOME_REDUZIDO = erro.TB_GESTOR_NOME_REDUZIDO;
                        cargaErroAlmox.TB_ALMOXARIFADO_CODIGO = erro.TB_ALMOXARIFADO_CODIGO;
                        cargaErroAlmox.TB_ALMOXARIFADO_DESCRICAO = erro.TB_ALMOXARIFADO_DESCRICAO;
                        cargaErroAlmox.TB_ALMOXARIFADO_LOGRADOURO = erro.TB_ALMOXARIFADO_LOGRADOURO;
                        cargaErroAlmox.TB_ALMOXARIFADO_NUMERO = erro.TB_ALMOXARIFADO_NUMERO;
                        cargaErroAlmox.TB_ALMOXARIFADO_COMPLEMENTO = erro.TB_ALMOXARIFADO_COMPLEMENTO;
                        cargaErroAlmox.TB_ALMOXARIFADO_BAIRRO = erro.TB_ALMOXARIFADO_BAIRRO;
                        cargaErroAlmox.TB_ALMOXARIFADO_MUNICIPIO = erro.TB_ALMOXARIFADO_MUNICIPIO;
                        cargaErroAlmox.TB_UF_SIGLA = erro.TB_UF_SIGLA;
                        cargaErroAlmox.TB_ALMOXARIFADO_CEP = erro.TB_ALMOXARIFADO_CEP;
                        cargaErroAlmox.TB_ALMOXARIFADO_TELEFONE = erro.TB_ALMOXARIFADO_TELEFONE;
                        cargaErroAlmox.TB_ALMOXARIFADO_FAX = erro.TB_ALMOXARIFADO_FAX;
                        cargaErroAlmox.TB_ALMOXARIFADO_RESPONSAVEL = erro.TB_ALMOXARIFADO_RESPONSAVEL;
                        cargaErroAlmox.TB_ALMOXARIFADO_MES_REF_INICIAL = erro.TB_ALMOXARIFADO_MES_REF_INICIAL;
                        cargaErroAlmox.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE = erro.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE;

                        break;
                    case (int)GeralEnum.TipoControle.Usuario:

                        cargaErroUsuario.TB_CARGA_SEQ = erro.TB_CARGA_SEQ;
                        cargaErroUsuario.TB_USUARIO_CPF = erro.TB_USUARIO_CPF;
                        cargaErroUsuario.TB_GESTOR_NOME_REDUZIDO = erro.TB_GESTOR_NOME_REDUZIDO;
                        cargaErroUsuario.TB_USUARIO_NOME_USUARIO = erro.TB_USUARIO_NOME_USUARIO;
                        cargaErroUsuario.TB_USUARIO_NUM_RG = erro.TB_USUARIO_NUM_RG;
                        cargaErroUsuario.TB_USUARIO_ORGAO_EMISSOR = erro.TB_USUARIO_ORGAO_EMISSOR;
                        cargaErroUsuario.TB_USUARIO_END_NUMERO = erro.TB_USUARIO_END_NUMERO;
                        cargaErroUsuario.TB_USUARIO_END_COMPL = erro.TB_USUARIO_END_COMPL;
                        cargaErroUsuario.TB_USUARIO_END_BAIRRO = erro.TB_USUARIO_END_BAIRRO;
                        cargaErroUsuario.TB_USUARIO_END_MUNIC = erro.TB_USUARIO_END_MUNIC;
                        cargaErroUsuario.TB_USUARIO_END_UF = erro.TB_USUARIO_END_UF;
                        cargaErroUsuario.TB_USUARIO_END_CEP = erro.TB_USUARIO_END_CEP;
                        cargaErroUsuario.TB_USUARIO_END_FONE = erro.TB_USUARIO_END_FONE;
                        cargaErroUsuario.TB_USUARIO_EMAIL = erro.TB_USUARIO_EMAIL;
                        cargaErroUsuario.TB_USUARIO_END_RUA = erro.TB_USUARIO_END_RUA;
                        cargaErroUsuario.TB_USUARIO_RG_UF = erro.TB_USUARIO_RG_UF;


                        break;

                    case (int)GeralEnum.TipoControle.PerfilRequisitante:

                        cargaErroPerfilRequisitante.TB_CARGA_SEQ = erro.TB_CARGA_SEQ;
                        cargaErroPerfilRequisitante.TB_USUARIO_CPF = erro.TB_USUARIO_CPF;
                        cargaErroPerfilRequisitante.TB_GESTOR_NOME_REDUZIDO = erro.TB_GESTOR_NOME_REDUZIDO;
                        cargaErroPerfilRequisitante.TB_UA_CODIGO = erro.TB_UA_CODIGO;
                        cargaErroPerfilRequisitante.TB_DIVISAO_CODIGO = erro.TB_DIVISAO_CODIGO;           
                


                        break;

                    default:
                        cargaErro.TB_CARGA_SEQ = erro.TB_CARGA_SEQ;
                        cargaErro.TB_GESTOR_NOME_REDUZIDO = erro.TB_GESTOR_NOME_REDUZIDO;
                        cargaErro.TB_ALMOXARIFADO_CODIGO = erro.TB_ALMOXARIFADO_CODIGO;
                        cargaErro.TB_CONTA_AUXILIAR_CODIGO = erro.TB_CONTA_AUXILIAR_CODIGO;
                        cargaErro.TB_INDICADOR_DISPONIVEL_DESCRICAO = erro.TB_INDICADOR_DISPONIVEL_DESCRICAO;
                        cargaErro.TB_ITEM_MATERIAL_CODIGO = erro.TB_ITEM_MATERIAL_CODIGO;
                        cargaErro.TB_NATUREZA_DESPESA_CODIGO = erro.TB_NATUREZA_DESPESA_CODIGO;
                        cargaErro.TB_SALDO_SUBITEM_LOTE_DT_VENC = erro.TB_SALDO_SUBITEM_LOTE_DT_VENC;
                        cargaErro.TB_SALDO_SUBITEM_LOTE_FAB = erro.TB_SALDO_SUBITEM_LOTE_FAB;
                        cargaErro.TB_SALDO_SUBITEM_LOTE_IDENT = erro.TB_SALDO_SUBITEM_LOTE_IDENT;
                        cargaErro.TB_SALDO_SUBITEM_SALDO_QTDE = erro.TB_SALDO_SUBITEM_SALDO_QTDE;
                        cargaErro.TB_SALDO_SUBITEM_SALDO_VALOR = erro.TB_SALDO_SUBITEM_SALDO_VALOR;
                        cargaErro.TB_SUBITEM_MATERIAL_CODIGO = erro.TB_SUBITEM_MATERIAL_CODIGO;
                        cargaErro.TB_SUBITEM_MATERIAL_DESCRICAO = erro.TB_SUBITEM_MATERIAL_DESCRICAO;
                        cargaErro.TB_SUBITEM_MATERIAL_ESTOQUE_MAX = erro.TB_SUBITEM_MATERIAL_ESTOQUE_MAX;
                        cargaErro.TB_SUBITEM_MATERIAL_ESTOQUE_MIN = erro.TB_SUBITEM_MATERIAL_ESTOQUE_MIN;
                        cargaErro.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE = erro.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE;
                        cargaErro.TB_SUBITEM_MATERIAL_LOTE = erro.TB_SUBITEM_MATERIAL_LOTE;
                        cargaErro.TB_UGE_CODIGO = erro.TB_UGE_CODIGO;
                        cargaErro.TB_UNIDADE_FORNECIMENTO_CODIGO = erro.TB_UNIDADE_FORNECIMENTO_CODIGO;
                        break;

                }


                if (listErroDescricao != null)
                {
                    //Monta a entidade para a lista de erros Descrição
                    foreach (var erro2 in listErroDescricao)
                    {
                        if (cargaErro.TB_CARGA_SEQ == erro2.TB_CARGA.TB_CARGA_SEQ || cargaErroDivisao.TB_CARGA_SEQ == erro2.TB_CARGA.TB_CARGA_SEQ
                            || cargaErroAlmox.TB_CARGA_SEQ == erro2.TB_CARGA.TB_CARGA_SEQ || cargaErroResp.TB_CARGA_SEQ == erro2.TB_CARGA.TB_CARGA_SEQ
                            || cargaErroUsuario.TB_CARGA_SEQ == erro2.TB_CARGA.TB_CARGA_SEQ || cargaErroPerfilRequisitante.TB_CARGA_SEQ == erro2.TB_CARGA.TB_CARGA_SEQ)
                        {
                            cargaErro.TB_ERRO_DESCRICAO = erro2.TB_ERRO.TB_ERRO_DESCRICAO;
                            cargaErroDivisao.TB_ERRO_DESCRICAO = erro2.TB_ERRO.TB_ERRO_DESCRICAO;
                            cargaErroAlmox.TB_ERRO_DESCRICAO = erro2.TB_ERRO.TB_ERRO_DESCRICAO;
                            cargaErroResp.TB_ERRO_DESCRICAO = erro2.TB_ERRO.TB_ERRO_DESCRICAO;
                            cargaErroUsuario.TB_ERRO_DESCRICAO = erro2.TB_ERRO.TB_ERRO_DESCRICAO;
                            cargaErroPerfilRequisitante.TB_ERRO_DESCRICAO = erro2.TB_ERRO.TB_ERRO_DESCRICAO;
                            break;
                        }
                    }

                    grdExport2.DataSource = listCargaErroGrid;
                    grdExport2.DataBind();
                }


                switch (erro.TB_CONTROLE.TB_TIPO_CONTROLE_ID)
                {
                    case (int)GeralEnum.TipoControle.Divisao:

                        listCargaGridD.Add(cargaErroDivisao);
                        grdExport.DataSource = listCargaGridD;
                        break;


                    case (int)GeralEnum.TipoControle.Almoxarifado:

                        listCargaGridA.Add(cargaErroAlmox);
                        grdExport.DataSource = listCargaGridA;
                        break;

                    case (int)GeralEnum.TipoControle.Responsavel:

                        listCargaGridR.Add(cargaErroResp);
                        grdExport.DataSource = listCargaGridR;
                        break;

                    case (int)GeralEnum.TipoControle.Usuario:

                        listCargaGridU.Add(cargaErroUsuario);
                        grdExport.DataSource = listCargaGridU;
                        break;

                    case (int)GeralEnum.TipoControle.PerfilRequisitante:

                        listCargaGridPR.Add(cargaErroPerfilRequisitante);
                        grdExport.DataSource = listCargaGridPR;
                        break;
                    default:

                        listCargaGrid.Add(cargaErro);
                        grdExport.DataSource = listCargaGrid;
                        break;
                }



            }


            grdExport.DataBind();

            //if (listErroDescricao != null)
            //{
            //    //Monta a entidade para a lista de erros Descrição
            //    foreach (var erro in listErroDescricao)
            //    {
            //        listCargaErroGrid.Add(new CargaErroDescricao(
            //            erro.TB_CARGA.TB_CARGA_SEQ
            //            , erro.TB_CARGA.TB_SUBITEM_MATERIAL_CODIGO
            //            , erro.TB_ERRO.TB_ERRO_DESCRICAO));
            //    }

            //    grdExport2.DataSource = listCargaErroGrid;
            //    grdExport2.DataBind();
            //}

            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment;filename=tblCarga.xls");
            Response.Charset = "";

            System.IO.StringWriter stringWrite = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);

            grdExport.RenderControl(htmlWrite);

            if (listErroDescricao != null)
                grdExport2.RenderControl(htmlWrite);

            Response.Write(stringWrite.ToString());

            Response.Flush();
            //HttpContext.Current.ApplicationInstance.CompleteRequest();
            Response.End();
        }

        protected void grdPendentes_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex >= 0)
            {
                ((ImageButton)e.Row.Cells[3].FindControl("linkIDExp")).Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
                ((ImageButton)e.Row.Cells[4].FindControl("linkIDErros")).Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
                ((ImageButton)e.Row.Cells[5].FindControl("linkIDImp")).Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
                ((ImageButton)e.Row.Cells[6].FindControl("linkIDExcluir")).Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            }
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            new CargaPresenter(this).Novo();
        }

        public int TipoArquivo
        {
            get
            {
                return Convert.ToInt32(ddlTipoControle.SelectedValue);
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            new CargaPresenter(this).Cancelar();
        }

        protected void grdPendentes_SelectedIndexChanged(object sender, EventArgs e)
        {
            HiddenField Hidden = (HiddenField)sender;

            string selectedEmployee = (string)Hidden.ID;
        }

        protected void grdPendentes_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            HiddenField Hidden = (HiddenField)sender;

            string selectedEmployee = (string)Hidden.ID;

        }

        protected void grdPendentes_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            HiddenField Hidden = (HiddenField)sender;

            string selectedEmployee = (string)Hidden.ID;
        }
    }
}
