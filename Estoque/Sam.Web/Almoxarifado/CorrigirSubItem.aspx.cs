using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.Presenter;
using Sam.Domain.Entity;
using System.Collections;
using System.Threading;
using System.Xml;
using System.Text;
using Sam.View;


namespace Sam.Web.Almoxarifado
{
    public partial class CorrigirSubItem : PageBase
    {
        private readonly static AlmoxarifadoPresenter presenterAlmoxarifado = new AlmoxarifadoPresenter();
        private readonly static OrgaoPresenter presenterOrgao = new OrgaoPresenter();
        private readonly static UOPresenter presenterOU = new UOPresenter();
        private readonly static UGEPresenter presenterUGE = new UGEPresenter();
        private readonly static GestorPresenter presenterGestor = new GestorPresenter();
        public EventWaitHandle eventWaitHandle = new ManualResetEvent(false);
        MovimentoItemEntity movimentoItem = null;
        PTResMensalEntity ptResMensal = null;
        private Thread thredCorrigir = null;
        CorrigirSubItem Corrigir = null;


        private string path = "";

        public int result;

        private void PerformUserWorkItem(Object stateObject)
        {

            CorrigirSubItem state = stateObject as CorrigirSubItem;

            if (state != null)
            {

                CorrigirSubItens();

                state.result = 42;

                state.eventWaitHandle.Set(); // signal we're done

            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager.GetCurrent(this).AsyncPostBackTimeout = 90000;
            //Session.Timeout = 525600;
            //Response.Cache.SetExpires(DateTime.Now.AddMinutes(525600));

            if (!IsPostBack)
            {
                ComboDeOrgao();
               btnDownload.Enabled = false;


            }
            ThreadStart StartCorrigir = new ThreadStart(ExecutarCorrecao);
            thredCorrigir = new Thread(StartCorrigir);
        }
        
        
        
        public void childthreadcall()
        {
            try
            {
                lblmessage.Text = "<br />Child thread started <br/>";
                lblmessage.Text += "Child Thread: Coiunting to 10";
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(500);
                    lblmessage.Text += "<br/> in Child thread </br>";
                }
                lblmessage.Text += "<br/> child thread finished";
            }
            catch (ThreadAbortException e)
            {
                lblmessage.Text += "<br /> child thread - exception";
            }
            finally
            {
                lblmessage.Text += "<br /> child thread  - unable to catch the exception";
            }
        }
        


      
        


        private void CorrigirSubItens()
        {
            try
            {
                AlmoxarifadoEntity almoxarifado = new AlmoxarifadoEntity(int.Parse(drpAlmoxarifado.SelectedValue));
                String[] codigoAlmoxarifado = drpAlmoxarifado.SelectedItem.Text.Split('-');
                almoxarifado.Codigo = Convert.ToInt32(codigoAlmoxarifado[0]);

                movimentoItem = new MovimentoItemEntity();
                movimentoItem.Almoxarifado = almoxarifado;

                SubItemMaterialEntity material = new SubItemMaterialEntity();
                Int64 codigoSubItem = 0;
                Int64.TryParse(this.txtSubtItemCodigo.Text, out codigoSubItem);

                if (codigoSubItem > 0)
                    material.Codigo = codigoSubItem;

                GestorEntity gestor = new GestorEntity();
                Int32 gestorId = 0;
                Int32.TryParse(this.ddlGestor.SelectedValue, out gestorId);

                if (gestorId > 0)
                    gestor.Id = gestorId;

                MovimentoEntity movimento = new MovimentoEntity();
                movimento.Almoxarifado = almoxarifado;
                movimentoItem.SubItemMaterial = material;
                movimentoItem.Almoxarifado.Gestor = gestor;
                int ano = int.Parse(this.txtMesReferencia.Text.Substring(2, 4));
                int mes = int.Parse(this.txtMesReferencia.Text.Substring(0, 2));
                movimento.AnoMesReferencia = ano.ToString() + mes.ToString("00");
                movimento.DataMovimento = new DateTime(ano, mes, 1);

                movimentoItem.Movimento = movimento;
            }
            catch (Exception ex)
            {
                validarCorrecaoSubItem(ex.Message);
            }
        }

        
        private bool AlterarNLConsumo()
        {          

            try
            {
                if (string.IsNullOrWhiteSpace(txtMovimentoItemNL.Text) || string.IsNullOrWhiteSpace(txtIdPtresMensal.Text) ||
                     string.IsNullOrWhiteSpace(txtNL.Text))
                    throw new Exception();


                   AlmoxarifadoEntity almoxarifado = new AlmoxarifadoEntity(int.Parse(drpAlmoxarifado.SelectedValue));
                   String[] codigoAlmoxarifado = drpAlmoxarifado.SelectedItem.Text.Split('-');
                   almoxarifado.Codigo = Convert.ToInt32(codigoAlmoxarifado[0]);

                   movimentoItem = new MovimentoItemEntity();
                   movimentoItem.Almoxarifado = almoxarifado;
                   movimentoItem.Id = Convert.ToInt32(txtMovimentoItemNL.Text);
                   movimentoItem.NL_Consumo = txtNL.Text;

                   MovimentoEntity movimento = new MovimentoEntity();
                   movimento.Almoxarifado = almoxarifado;


                ptResMensal = new PTResMensalEntity();
                ptResMensal.Id = (Convert.ToInt32(txtIdPtresMensal.Text));
                ptResMensal.NlLancamento = txtNL.Text;


                movimentoItem.Movimento = movimento;
                return false;
            }
            catch (Exception ex)
            {
                validarMovNLConsumo(ex.Message);
                return true;
            }
        }

       


        private bool AlterarMovimentoSubItem()
        {
            try
            {
                if (drpAlmoxarifado.SelectedIndex < 1 || string.IsNullOrWhiteSpace(txtMovimentoItem.Text) ||
                     string.IsNullOrWhiteSpace(ddlAcao.Text))
                    throw new Exception();
                else
                {
                    if(((ddlAcao.SelectedValue == "Ativar") && string.IsNullOrWhiteSpace(ddlAtivar.SelectedValue))
                        || (ddlAcao.SelectedValue == "Alterar" && string.IsNullOrWhiteSpace(txtQtdeSubItem.Text))
                         || (ddlAcao.SelectedValue == "AlterarValor" && string.IsNullOrWhiteSpace(txtValorSubItem.Text)))
                            throw new Exception();                
                }
                
               
                AlmoxarifadoEntity almoxarifado = new AlmoxarifadoEntity(int.Parse(drpAlmoxarifado.SelectedValue));
                String[] codigoAlmoxarifado = drpAlmoxarifado.SelectedItem.Text.Split('-');
                almoxarifado.Codigo = Convert.ToInt32(codigoAlmoxarifado[0]);

                movimentoItem = new MovimentoItemEntity();
                movimentoItem.Almoxarifado = almoxarifado;
                movimentoItem.Id = Convert.ToInt32(txtMovimentoItem.Text);
                
                MovimentoEntity movimento = new MovimentoEntity();
                movimento.Almoxarifado = almoxarifado;

                if (!string.IsNullOrWhiteSpace(ddlAtivar.SelectedValue) && idAtivacao.Visible)
                {
                    bool ativar = ddlAtivar.SelectedValue == "1" ? true : false;
                    movimentoItem.Ativo = ativar;
                }
                
                if (!string.IsNullOrWhiteSpace(txtQtdeSubItem.Text) && txtQtdeSubItem.Visible)
                {
                    movimentoItem.QtdeMov = Convert.ToDecimal(txtQtdeSubItem.Text);
                }

                if (!string.IsNullOrWhiteSpace(txtValorSubItem.Text) && txtValorSubItem.Visible)
                {
                    movimentoItem.ValorMov = Convert.ToDecimal(txtValorSubItem.Text);
                }
              
                movimentoItem.Movimento = movimento;
                return false;
            }
            catch (Exception ex)
            {
               validarMovSubItem(ex.Message);
               return true;
            }
        }

        private void LimparDados()
        {
           
            txtMovimentoItem.Text = string.Empty;
            ddlAcao.SelectedValue = "";
            idAtivacao.Visible = false;
            ddlAtivar.SelectedValue = "";
            idQtde.Visible = false;
            idValor.Visible = false;
            txtQtdeSubItem.Text = string.Empty;
            txtValorSubItem.Text = string.Empty;
            txtMovimentoId.Text = string.Empty;
            ddlBloquear.SelectedValue = "";

        }



        private string ExecutarAlteracaoPtResMensal()
        {
            PTResMensalEntity resMensal = ptResMensal;
            try
            {
                PTResMensalPresenter presenter = new PTResMensalPresenter();
                Tuple<string, bool> retorno = presenter.AlterarPtREsNL(ptResMensal);

                if (retorno.Item2)
                    LimparDados();

                throw new Exception(retorno.Item1);

            }
            catch (Exception ex)
            {

                thredCorrigir.Abort();
                List<string> lstErro = new List<string>();

                lstErro.Add(ex.Message);
                return lstErro.FirstOrDefault();
            }
            finally
            {
                thredCorrigir.Abort();
            }
        }

        private void ExecutarAlteracaoSubItem(string mgsNlConsumo)
        {
            MovimentoItemEntity movimento = movimentoItem;
            try
            {
                MovimentoPresenter presenter = new MovimentoPresenter();
                Tuple<string, bool> retorno = presenter.AlterarMovimentoSubItem(movimento);

                if (retorno.Item2)
                    LimparDados();

                   throw new Exception(retorno.Item1);

            }
            catch (Exception ex)
            {
                
                thredCorrigir.Abort();
                List<string> lstErro = new List<string>();

                lstErro.Add(ex.Message);
                if (!string.IsNullOrEmpty(mgsNlConsumo))
                    lstErro.Add(mgsNlConsumo);

                ListaErros = lstErro;
            }
            finally
            {
                thredCorrigir.Abort();
            }
        }

        private void ExecutarMovimentoBloquear(int MovId, bool bloquear)
        {
            MovimentoItemEntity movimento = movimentoItem;
            try
            {
                MovimentoPresenter presenter = new MovimentoPresenter();
                Tuple<string, bool> retorno = presenter.AtualizarMovimentoBloquear(MovId,  bloquear);

                if (retorno.Item2)
                    LimparDados();

                   throw new Exception(retorno.Item1);

            }
            catch (Exception ex)
            {
                
                thredCorrigir.Abort();
                List<string> lstErro = new List<string>();

                lstErro.Add(ex.Message);
               
                ListaErros = lstErro;
            }
            finally
            {
                thredCorrigir.Abort();
            }
        }

        
        private void ExecutarCorrecao()
        {
            MovimentoItemEntity movimento = movimentoItem;
            try
            {
                MovimentoPresenter presenter = new MovimentoPresenter();
                presenter.CorrigirSubItem(movimento);

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(movimento.Destino))
                {
                    foreach (MovimentoItemEntity item in presenter.listaMovimentoItem)
                    {
                        string line;
                        if (item.Movimento.Instrucoes != null && item.Movimento.Instrucoes.Trim().Length > 0)
                        {
                            line = item.Movimento.Instrucoes + "\n";
                            file.WriteLine(line);
                            line = "Almoxarifado:" + item.Movimento.Almoxarifado.Codigo.ToString() + "| Movimento:" + item.Movimento.Id.Value.ToString() + "| MovimentoItem:" + item.Id.Value.ToString() + "| Número Documento: " + item.Movimento.NumeroDocumento + "| Data Movimento:" + item.Movimento.DataMovimento.Value.ToString("dd/MM/yyyy") + " | SubItem:" + item.SubItemMaterial.Id.Value.ToString() + ":" + item.SubItemMaterial.Codigo.ToString() + "| Tipo Movimento:" + item.Movimento.TipoMovimento.Id.ToString();
                        }
                        else
                            line = "Almoxarifado:" + item.Movimento.Almoxarifado.Codigo.ToString() + "| Movimento:" + item.Movimento.Id.Value.ToString() + "| MovimentoItem:" + item.Id.Value.ToString() + "| Número Documento: " + item.Movimento.NumeroDocumento + "| Data Movimento:" + item.Movimento.DataMovimento.Value.ToString("dd/MM/yyyy") + " | SubItem:" + item.SubItemMaterial.Id.Value.ToString() + ":" + item.SubItemMaterial.Codigo.ToString() + "| Tipo Movimento:" + item.Movimento.TipoMovimento.Id.ToString();

                        file.WriteLine(line);

                    }
                }
                //manualResetEvent[0].Set();
               

            }
            catch (Exception ex)
            {
                if (!getArquivoExiste(movimento.Destino))
                {
                    System.IO.StreamWriter file = new System.IO.StreamWriter(movimento.Destino);
                }
                thredCorrigir.Abort();

                List<string> lstErro = new List<string>();

                lstErro.Add(ex.Message);
                ListaErros = lstErro;
            }
            finally
            {
                thredCorrigir.Abort();
            }
        }

        private void validarCorrecaoSubItem(string mensagem)
        {
            List<string> lstErro = new List<string>();



            if (drpAlmoxarifado.SelectedIndex < 1)
                lstErro.Add("Favor informar o almoxarifado!");
            else if (this.txtMesReferencia.Text.Trim().Length < 1)
                lstErro.Add("Favor informar a Mês de Referência");
            else
                lstErro.Add(mensagem);

            ListaErros = lstErro;
        }

        private void validarMovNLConsumo(string mensagem)
        {
            List<string> lstErro = new List<string>();


            if (string.IsNullOrWhiteSpace(txtIdPtresMensal.Text))
                lstErro.Add("Favor informar Inserir o ID PtResMensal");
            if (string.IsNullOrWhiteSpace(txtMovimentoItemNL.Text))
                lstErro.Add("Favor informar ID MovimentoItem");
            if (string.IsNullOrWhiteSpace(txtNL.Text))
                lstErro.Add("Favor informar NL Consumo");
           
            if (!string.IsNullOrWhiteSpace(mensagem) && lstErro.Count == 0)
                lstErro.Add(mensagem);

            ListaErros = lstErro;

        }

        private void validarMovSubItem(string mensagem)
        {
            List<string> lstErro = new List<string>();

            if (drpAlmoxarifado.SelectedIndex < 1)
                lstErro.Add("Favor informar o almoxarifado!");
            if (string.IsNullOrWhiteSpace(txtMovimentoItem.Text))
                lstErro.Add("Favor informar Inserir o MovimentoItem");
            if (string.IsNullOrWhiteSpace(ddlAcao.Text))
                lstErro.Add("Favor informar Selecionar Ação a ser realizada");
            if ((ddlAcao.SelectedValue == "Ativar") && string.IsNullOrWhiteSpace(ddlAtivar.SelectedValue))
                 lstErro.Add("Favor informar Selecionar Ativar Mov");
            if ((ddlAcao.SelectedValue == "Alterar") && string.IsNullOrWhiteSpace(txtQtdeSubItem.Text))
                 lstErro.Add("Favor informar Qtde Mov");
            if ((ddlAcao.SelectedValue == "AlterarValor") && string.IsNullOrWhiteSpace(txtValorSubItem.Text))
                lstErro.Add("Favor informar Valor Mov");
            if (!string.IsNullOrWhiteSpace(mensagem) && lstErro.Count == 0)
                lstErro.Add(mensagem);
                    
            ListaErros = lstErro;
                   
        }

        private bool validarMovBloquear()
        {
            List<string> lstErro = new List<string>();

            if (string.IsNullOrWhiteSpace(txtMovimentoId.Text))
                lstErro.Add("Favor informar Inserir o MovimentoId");
            if (string.IsNullOrEmpty(ddlBloquear.SelectedValue))
                lstErro.Add("Favor informar Selecionar Ação a ser realizada");
          

            ListaErros = lstErro;

            if (lstErro.Count > 0)
                return true;
            else
                return false;

        }

        protected void btnCorrigir_Click(object sender, EventArgs e)
        {
            Corrigir = new CorrigirSubItem();
            CorrigirSubItens();
            path = Server.MapPath("") + @"\" + "Correcao" + movimentoItem.Almoxarifado.Id.Value.ToString() + ".txt";
            
            if(getArquivoExiste(path))
                deletaArquivo(path);

            Session.Add("NOMEPATH", "Correcao" + movimentoItem.Almoxarifado.Id.Value.ToString());
            Session.Add(Session["NOMEPATH"].ToString(), path);
           
            movimentoItem.Destino = path;
            Corrigir.movimentoItem = movimentoItem;
            //Thread t = new Thread(Corrigir.ExecutarCorrecao);
            //t.IsBackground = true;
            //t.Start(Corrigir);

            // ThreadStart StartCorrigir = new ThreadStart(ExecutarCorrecao);
            // thredCorrigir = new Thread(StartCorrigir);
            //manualResetEvent = new ManualResetEvent[1];
            //manualResetEvent[0] = new ManualResetEvent(false);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "CorrigirAlmoxarifado", "alert('A Correção dos subitens do almoxarifado:" + this.drpAlmoxarifado.SelectedItem.Text +" vai inicializar, por favor espere a mensagem de conclusão para selecionar outro almoxarifado!');", true);
            timerArquivo.Enabled = true;
            this.drpAlmoxarifado.Enabled = false;
            btnCorrigir.Enabled = false;
            var constante = Sam.Common.Util.Constante.CST_ANO_MES_DATA_CORTE_SAP;
            thredCorrigir.Start();
            //WaitHandle.WaitAll(manualResetEvent);

            //btnDownload.Enabled = true;
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "Correção", "alert('Correção executada com sucesso!');", true);
            // DoWorkSynchronous();
            // ThreadPool.QueueUserWorkItem(CorrigirSubItens);
        }

        public object DoWorkSynchronous()
        {

            CorrigirSubItem state = new CorrigirSubItem();

            ThreadPool.QueueUserWorkItem(PerformUserWorkItem, state);

            state.eventWaitHandle.WaitOne();

            Console.WriteLine(state.result);

            return state.result;

        }

        public IList ListaErros
        {
            set
            {
                if (value != null)
                {
                    this.ListInconsistencias.ExibirLista(value);
                    this.ListInconsistencias.DataBind();
                }
            }
        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            path = Session[Session["NOMEPATH"].ToString()].ToString();
            DownloadFile(path);
        }
        private void DownloadFile(string filePath)
        {
            // Get the physical Path of the file
            string filepath = filePath;
            System.IO.FileInfo file = new System.IO.FileInfo(filepath);

            if (file.Exists)
            {
                Response.ClearContent();
                // Add the file name and attachment
                Response.AddHeader("Content-Disposition", "attachment; filename=Download.txt");
                // Add the file size into the response header
                Response.AddHeader("Content-Length", file.Length.ToString());
                // Set the ContentType
                Response.ContentType = GetFileExtension(file.Extension.ToLower());
                // Write the file into the response
                Response.TransmitFile(file.FullName);
                Response.End();
            }
            this.drpAlmoxarifado.Enabled = true;
            btnDownload.BackColor = btnCorrigir.BackColor;
        }
        private string GetFileExtension(string fileExtension)
        {
            switch (fileExtension)
            {
                case ".htm":
                case ".html":
                case ".log":
                    return "text/HTML";
                case ".txt":
                    return "text/plain";
                case ".doc":
                    return "application/ms-word";
                case ".tiff":
                case ".tif":
                    return "image/tiff";
                case ".asf":
                    return "video/x-ms-asf";
                case ".avi":
                    return "video/avi";
                case ".zip":
                    return "application/zip";
                case ".xls":
                case ".csv":
                    return "application/vnd.ms-excel";
                case ".gif":
                    return "image/gif";
                case ".jpg":
                case "jpeg":
                    return "image/jpeg";
                case ".bmp":
                    return "image/bmp";
                case ".wav":
                    return "audio/wav";
                case ".mp3":
                    return "audio/mpeg3";
                case ".mpg":
                case "mpeg":
                    return "video/mpeg";
                case ".rtf":
                    return "application/rtf";
                case ".asp":
                    return "text/asp";
                case ".pdf":
                    return "application/pdf";
                case ".fdf":
                    return "application/vnd.fdf";
                case ".ppt":
                    return "application/mspowerpoint";
                case ".dwg":
                    return "image/vnd.dwg";
                case ".msg":
                    return "application/msoutlook";
                case ".xml":
                case ".sdxl":
                    return "application/xml";
                case ".xdp":
                    return "application/vnd.adobe.xdp+xml";
                default:
                    return "application/octet-stream";
            }
        }

        protected void timerArquivo_Tick(object sender, EventArgs e)
        {
            try
            {
                string filepath = Session[Session["NOMEPATH"].ToString()].ToString();
                System.IO.FileInfo file = new System.IO.FileInfo(filepath);

                if (file.Exists)
                {

                    if (file.Length > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "Correção", "alert('Correção executada, com erros a ser corrigidos, faça download do arquivo de error para continuar a correção!');", true);
                        btnDownload.Enabled = true;
                        btnDownload.BackColor = System.Drawing.Color.DarkBlue;
                        this.drpAlmoxarifado.Enabled = false;
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "Correção", "alert('Correção executada com sucesso, sem erros!');", true);
                        this.drpAlmoxarifado.Enabled = true;
                        file.Delete();

                    }

                    timerArquivo.Enabled = false;
                    btnCorrigir.Enabled = true;

                    file = null;
                }
            }
            catch (Exception ex)
            {
                timerArquivo.Enabled = false;
                btnCorrigir.Enabled = true;
                this.drpAlmoxarifado.Enabled = true;
                throw ex;
            }
        }

        private Boolean getArquivoExiste(String path)
        {
            string filepath = path;
            System.IO.FileInfo file = new System.IO.FileInfo(filepath);
            if (file.Exists)
            {
                file = null;
                return true;
            }
            file = null;
            return false;

        }

        private void deletaArquivo(String path)
        {
            string filepath = path;
            System.IO.FileInfo file = new System.IO.FileInfo(filepath);

            if (file.Exists)
                file.Delete();

            file = null;
        }

        protected void btnSalvarAlm_Click(object sender, EventArgs e)
        {
            
            if (!validarMovBloquear())
            {
                bool bloquear = ddlBloquear.SelectedValue == "1" ? true : false;

                ExecutarMovimentoBloquear(Convert.ToInt32(txtMovimentoId.Text), bloquear);
            }
        }

        protected void btnSalvarPTR_Click(object sender, EventArgs e)
        {

            if (!AlterarNLConsumo())
            {

                ExecutarAlteracaoSubItem(ExecutarAlteracaoPtResMensal());
            }
        }


        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            
            if(!AlterarMovimentoSubItem())
                ExecutarAlteracaoSubItem(string.Empty);
        }

        protected void ddlAcao_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlAcao.SelectedValue == "Ativar")
            {
                idAtivacao.Visible = true;
                idQtde.Visible = false;
                idValor.Visible = false;
            }
            else if (ddlAcao.SelectedValue == "Alterar")
            {
                idAtivacao.Visible = false;
                idQtde.Visible = true;
                idValor.Visible = false;
            }
            else if (ddlAcao.SelectedValue == "AlterarValor")
            {
                idAtivacao.Visible = false;
                idQtde.Visible = false;
                idValor.Visible = true;
            }
            else
            {
                idAtivacao.Visible = false;
                idQtde.Visible = false;
                idValor.Visible = false;
            }

        }

        #region Novos Filtros


        private void ComboDeOrgao()
        {
            ddlOrgao.Items.Clear();
            ddlOrgao.DataTextField = "TB_ORGAO_DESCRICAO";
            ddlOrgao.DataValueField = "TB_ORGAO_ID";
            ddlOrgao.Items.Add("- Selecione -");
            ddlOrgao.Items[0].Value = "0";
            ddlOrgao.AppendDataBoundItems = true;

            //refatorar esse método PopularListaAlmoxarifado, LINQ não deve ser usado na camada de VIEW
            this.ddlOrgao.DataSource = presenterOrgao.ListarOrgao();
            this.ddlOrgao.DataBind();
        }

        private void ComboDeUO(int OrgaoId)
        {
            ddlUO.Items.Clear();
            ddlUO.DataTextField = "TB_UO_DESCRICAO";
            ddlUO.DataValueField = "TB_UO_ID";
            ddlUO.Items.Add("- Selecione -");
            ddlUO.Items[0].Value = "0";
            ddlUO.AppendDataBoundItems = true;

           // refatorar esse método PopularListaAlmoxarifado, LINQ não deve ser usado na camada de VIEW
            this.ddlUO.DataSource = presenterOU.ListarUo_2(OrgaoId);
            this.ddlUO.DataBind();
       }


        private void ComboDeUGE(int UoId)
        {
            ddlUGE.Items.Clear();
            ddlUGE.DataTextField = "TB_UGE_DESCRICAO";
            ddlUGE.DataValueField = "TB_UGE_ID";
            ddlUGE.Items.Add("- Selecione -");
            ddlUGE.Items[0].Value = "0";
            ddlUGE.AppendDataBoundItems = true;

            // refatorar esse método PopularListaAlmoxarifado, LINQ não deve ser usado na camada de VIEW
            this.ddlUGE.DataSource = presenterUGE.ListarUGE(UoId);
            this.ddlUGE.DataBind();
        }

        private void ComboDeGestor(int OrgaoId)
        {
            ddlGestor.Items.Clear();
            ddlGestor.DataTextField = "TB_GESTOR_NOME_REDUZIDO";
            ddlGestor.DataValueField = "TB_GESTOR_ID";
            ddlGestor.Items.Add("- Selecione -");
            ddlGestor.Items[0].Value = "0";
            ddlGestor.AppendDataBoundItems = true;

            // refatorar esse método PopularListaAlmoxarifado, LINQ não deve ser usado na camada de VIEW
            this.ddlGestor.DataSource = presenterGestor.ListarGestor(OrgaoId);
            this.ddlGestor.DataBind();
        }

        private void ComboDeAlmoxarifado(int GestorId)
        {
            drpAlmoxarifado.Items.Clear();
            drpAlmoxarifado.DataTextField = "TB_ALMOXARIFADO_DESCRICAO";
            drpAlmoxarifado.DataValueField = "TB_ALMOXARIFADO_ID";
            drpAlmoxarifado.Items.Add("- Selecione -");
            drpAlmoxarifado.Items[0].Value = "0";
            drpAlmoxarifado.AppendDataBoundItems = true;

            //refatorar esse método PopularListaAlmoxarifado, LINQ não deve ser usado na camada de VIEW
           // this.drpAlmoxarifado.DataSource = presenterAlmoxarifado.ListarAlmoxarifadoPorUge(UgeId);
            this.drpAlmoxarifado.DataSource = presenterAlmoxarifado.ListarAlmoxarifadoPorGestor(GestorId);
            this.drpAlmoxarifado.DataBind();
        }


        protected void ddlOrgao_SelectedIndexChanged(object sender, EventArgs e)
        {           
            ddlUO.Items.Clear();
            ddlGestor.Items.Clear();
            //ddlUGE.Items.Clear();
            drpAlmoxarifado.Items.Clear();
           // ComboDeUO(Convert.ToInt32(ddlOrgao.SelectedValue));
            ComboDeGestor(Convert.ToInt32(ddlOrgao.SelectedValue));
        }


        protected void ddlUO_SelectedIndexChanged(object sender, EventArgs e)
        {           
            ddlUGE.Items.Clear();
            drpAlmoxarifado.Items.Clear();
            ComboDeUGE(Convert.ToInt32(ddlUO.SelectedValue));
        }

        protected void ddlUGE_SelectedIndexChanged(object sender, EventArgs e)
        {
            drpAlmoxarifado.Items.Clear();
            ComboDeAlmoxarifado(Convert.ToInt32(ddlUGE.SelectedValue));
        }

        #endregion

        protected void ddlGestor_SelectedIndexChanged(object sender, EventArgs e)
        {
            drpAlmoxarifado.Items.Clear();
            ComboDeAlmoxarifado(Convert.ToInt32(ddlGestor.SelectedValue));
        }

        

       
        

    }
}