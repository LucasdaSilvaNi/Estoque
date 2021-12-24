using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Sam.View;
using Sam.Common.Util;
using Sam.Presenter;
using Sam.Entity;
using Sam.Domain.Entity;


namespace Sam.Web.Seguranca
{
    public partial class SEGAlterSenha : PageBase, IUsuarioView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UsuarioPresenter usu = new UsuarioPresenter(this);

            if (!IsPostBack)
            {
                txtUsuario.Text = usu.Acesso.Transacoes.Usuario.Cpf.ToString();
                txtUsuario.Enabled = false;
            }

            if (Convert.ToString(Session["RESET_TABCOUNTER"]) == "1")
            {
                Session["RESET_TABCOUNTER"] = "2";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "initTabCounter", "javascript:initTabCounter();", true);
            }
        }

        public string Id
        {
            get
            {
                string retorno = null;
                if (Session["ID"] != null)
                    retorno = Session["ID"].ToString();
                return retorno;
            }
            set
            {
                Session["ID"] = value;
            }
        }
        public int? CEP
        {
            get { return CEP; }
            set
            {
               CEP = value;
            }
        }

        public long? Telefone
        {
            get { return Telefone; }
            set { Telefone = value; }
        }

        public int? GestorId
        {
            get { return GestorId; }
            set
            {
               GestorId = value;
            }
        }

        public int? GestorPdId
        {
            get { return GestorPdId; }
            set
            {
                GestorPdId = value;
            }
        }

        public short? Numero
        {
            get { return Numero; }
            set { Numero = value; }
        }

        public string Complemento
        {
            get { return Complemento; }
            set { Complemento = value; }
        }

        public string Email
        {
            get { return Email; }
            set { Email = value; }
        }

        public string Codigo { get; set; }

        public string Descricao { get; set; }

        public void PopularGrid()
        {
            UsuarioPresenter usu = new UsuarioPresenter(this);
         }

        public int? UsuarioIdResponsavel { get; set; }

        public int? OrgaoId
        {
            get { return OrgaoId; }
            set
            {
                OrgaoId = value;
            }
        }

        public int? OrgaoPdId
        {
            get { return OrgaoPdId; }
            set
            {
                OrgaoPdId = value;
            }
        }

       
        public void PopularUf() 
        {
            UsuarioPresenter usu = new UsuarioPresenter();
            IList<UFEntity> UF = usu.PopularDadosUf();
        }

        public void   ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);            
        }

        public void   Redirecionar()
        {
            HttpContext.Current.Request.Cookies.Clear();
            HttpContext.Current.Response.Cookies.Clear();
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
            FormsAuthentication.SignOut();
            Response.Redirect(FormsAuthentication.LoginUrl, false);
        }       
        

        public IList ListaErros
        {
            set
            {
                this.ListInconsistencias.ExibirLista(value);
                this.ListInconsistencias.DataBind();
            }
        }

        public bool BloqueiaNovo
        {
            set { }
        }

        public bool BloqueiaGravar
        {
            set { } 
        }

        public bool BloqueiaCancelar
        {
            set { }
        }


        public string MsgSenha
        {
            get { return lblMsgSenha.Text; }
            set { lblMsgSenha.Text = value; }
        }
    

        public bool BloqueiaExcluir { set { } }

        public bool BloqueiaCodigo { set { } }

        public bool BloqueiaDescricao { set { } }

        public bool MostrarPainelEdicao
        {
            set 
            { 
               
            }
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
         
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            
        }

        public string CPF
        {
            get { return CPF; }
            set { CPF = value; }
        }

        public bool? Ativo
        {
            get { return Ativo; }
            set { Ativo = false; } 
        }

        public string OrgaoEmissor
        {
            get { return OrgaoEmissor; }
            set { OrgaoEmissor = value; }
        }

        public string Logradouro
        {
            get { return Logradouro; }
            set { Logradouro = value; }
        }

        public string Bairro
        {
            get { return Bairro; }
            set { Bairro = value; }
        }

        public string Municipio
        {
            get { return Municipio; }
            set { Municipio = value; }
        }

        public string NomeUsuario
        {
            get { return NomeUsuario; }
            set { NomeUsuario = value; }
        }

        public string RG
        {
            get { return RG;}
            set { RG = RG; }
        }

        public string UFEmissor
        {
            get { return UFEmissor; }
            set
            {
                UFEmissor = UFEmissor;
            }
        }

        
        public string Senha
        {
            get { return Senha; }
            set 
            {
                Senha = value;
            }
        }

        public string UF
        {
            get { return UF; }
            set
            {
                UF = value;
                }
            }

        public RelatorioEntity DadosRelatorio { get; set; }

        public SortedList ParametrosRelatorio
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int? PerfilId
        {
            get;set;
        }

        protected void btnAlterarSenha_Click(object sender, EventArgs e)
        {
            if ((txtNovaSenha.Text.Length < 8) || (txtNovaSenha.Text.Length > 20))
            {
                ExibirMensagem("Os campos de senha devem ter de 8 à 20 caracteres;");
                return;
            }

            //Aqui você pode incluir os caracteres que deseja que sejam retirados	
            char[] trim = { '=', '!', '@', '#', '$', '%', '¨', '&', '*', '(', ')', '_', '-', '+', '<', '>', ':', ';', '?', '/' };
            int pos;
            bool bAchou = false;
            if ((pos = this.txtNovaSenha.Text.IndexOfAny(trim)) >= 0)
            {
                bAchou = true;
            }

            if (bAchou)
            {
                ExibirMensagem("Use somente letras e/ou  números;");
                return;
            }

            if (txtSenhaAtual.Text == string.Empty)
            {
                ExibirMensagem("Senha Atual deve ser informada.");
                return;
            }

            if (txtNovaSenha.Text == string.Empty)
            {
                ExibirMensagem("Nova Senha deve ser informada.");
                return;
            }

            if (txtConfirmaSenha.Text == string.Empty)
            {
                ExibirMensagem("Confirmação da Senha deve ser informada.");
                return;
            }

            UsuarioPresenter usu = new UsuarioPresenter(this);

            //Compara a senha Criptografada

            string senhaCriptografada = Sam.Facade.FacadeLogin.CriptografarSenha(txtSenhaAtual.Text);

            if (usu.Acesso.Transacoes.Senha == senhaCriptografada)
            {
                if (txtNovaSenha.Text == txtConfirmaSenha.Text)
                {
                    //Nova senha Criptografada

                    string novaSenhaCriptografada = Sam.Facade.FacadeLogin.CriptografarSenha(txtNovaSenha.Text);
                    usu.AlterarSenha((int)usu.Acesso.Transacoes.Usuario.Id, novaSenhaCriptografada);

                    Response.Redirect(string.Concat(ResolveClientUrl("~/LogoffAutomatico.aspx"), "?lf=0&si=", Session["usuarioCahe"], "&li=", Session["IdLoginLogado"]), true);
                    //Redirecionar();
                }
                else
                    ExibirMensagem("Confirmação de senha não confere com a senha informada.");
            }
            else
            {
                ExibirMensagem("Senha Atual incorreta.");
            }            
          
         }

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Seguranca/TABMenu.aspx", false);
        }

        public void ExibirRelatorio()
        {
            throw new NotImplementedException();
        }
    }

}
