using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Sam.View;
using Sam.Common.Util;
using Sam.Presenter;
using Sam.Entity;
using Sam.Domain.Entity;
using System.IO;
using System.ComponentModel;
using Sam.Infrastructure;
using Sam.Business;
using System.Data;

namespace Sam.Web.Seguranca
{
    public partial class SEGUsuario : PageBase, IUsuarioView
    {
        private static Acesso acesso = null;
        bool gerarExcel = false;

        private int? orgaoId;
        private int? usuarioId;

        private IList<Common.EnumPerfil> permissaoRelatorio = new List<Common.EnumPerfil> { Common.EnumPerfil.ADMINISTRADOR_GERAL,
                                                                                            Common.EnumPerfil.ADMINISTRADOR_FINANCEIRO_SEFAZ,
                                                                                            Common.EnumPerfil.COMERCIAL_PRODESP,
                                                                                            Common.EnumPerfil.ADMINISTRADOR_ORGAO };



        public static string Ticket
        {
            get
            {
                try
                {
                    return ((System.Web.Security.FormsIdentity)(HttpContext.Current.User.Identity)).Ticket.UserData;
                }
                catch (InvalidCastException)
                {
                    return null;
                }
            }

        }

        public SortedList ParametrosRelatorio
        {
            get
            {
                SortedList paramList = new SortedList();

                paramList.Add("CodigoGestor", ddlGestor.SelectedValue.ToString());
                paramList.Add("CodigoOrgao", ddlOrgao.SelectedValue.ToString());
                paramList.Add("DescricaoOrgao", this.ddlOrgao.SelectedItem.Text);
                paramList.Add("NomeGestor", ddlGestor.SelectedItem.Text);
                paramList.Add("Pesquisa", txtPesquisar.Text);

                UsuarioPresenter usu = new UsuarioPresenter(this);
                if (this.ddlPerfil.Text != "- Sem Perfil -")
                    paramList.Add("CodigoPerfil", ddlPerfil.SelectedValue.ToString() == "- Todos -" ? "0" : ddlPerfil.SelectedValue.ToString());
                else
                    paramList.Add("CodigoPerfil", "-1");

                return paramList;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            UsuarioPresenter usu = new UsuarioPresenter(this);

            if (!IsPostBack)
            {
                usu.Load();
                PopularOrgao();
                PopularGestor();
                PopularUge();
                PopularPerfil();


                //if (permissaoRelatorio.Contains((Common.EnumPerfil)acesso.Transacoes.Perfis[0].IdPerfil))
                //    btnImprimir.Visible = true;
                btnImprimir.Visible = false;
            }

            //ScriptManager.RegisterStartupScript(this.txtCEP, GetType(), "cep", "$('.cep').mask('99999-999');", true);
            ScriptManager.RegisterStartupScript(this.txtCPF, GetType(), "cpf", "$('.cpf').mask('999.999.999-99');", true);
            ScriptManager.RegisterStartupScript(this.txtTelefone, GetType(), "telefone", "$('.telefone').mask('(99)9999-9999');", true);
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

        public string Codigo { get; set; }

        public string Descricao { get; set; }

        #region codigoantigogerarexcel
        public void PopularGrid()
        {


                if (!gerarExcel)
                {
                    PopularGridPaginacao(0); //Aqui vai ser zero pq é sempre a primeira página
                    //var retorno = usu.PopularDadosUsuarioGrid(Orgao, Gestor, Uge, Almoxarifado, Perfil, (string)acesso.Cpf, (int)acesso.Transacoes.Perfis[0].Peso, pesquisa);
                    //gridUsuario.DataSource = retorno;
                    //gridUsuario.DataBind();
                }

         }
        public void GeradorExcel()
        {

            int Orgao = 0;
            int Gestor = 0;
            int? Perfil = 0;
            PopularEXCEL();
            int Uge = 0;
            int Almoxarifado = 0;
            int pagina = 0;
            acesso = HttpContext.Current.Cache[Ticket] as Acesso;

            if (this.ddlOrgao.Text != null && this.ddlOrgao.Text != "")
            {
                Orgao = Convert.ToInt32(this.ddlOrgao.SelectedItem.Value);
            }

            if (this.ddlGestor.Text != null && this.ddlGestor.Text != "")
            {
                Gestor = Convert.ToInt32(this.ddlGestor.SelectedItem.Value);
            }

            if (this.ddlPerfil.Text != null && this.ddlPerfil.Text != "" && this.ddlPerfil.Text != "- Todos -" && this.ddlPerfil.Text != "- Sem Perfil -")
            {
                Perfil = Convert.ToInt32(this.ddlPerfil.SelectedItem.Value);
            }


            UsuarioPresenter usu = new UsuarioPresenter(this);
            if (this.ddlPerfil.Text == "- Todos -" || this.ddlPerfil.Text == "")
            {
                Perfil = 0;
            }
            if (this.cboUge.Text != null && cboUge.SelectedValue != "" && cboUge.Text != "- Todos -")
            {
                Uge = Convert.ToInt32(this.cboUge.SelectedItem.Value);
            }
            if (this.ddlPerfil.Text == "- Sem Perfil -")
            {
                Perfil = -1;
                Uge = 0;
                cboUge.SelectedIndex = 0;
                ddlAlmoxarifado.Items.Clear();
            }
            if (this.ddlAlmoxarifado.Text != null && ddlAlmoxarifado.SelectedValue != "" && ddlAlmoxarifado.Text != "- Todos -" && ddlAlmoxarifado.Text != " ")
            {
                Almoxarifado = Convert.ToInt32(this.ddlAlmoxarifado.SelectedItem.Value);
            }
            string pesquisa = txtPesquisar.Text.Trim();

            //var listaExportacao = usu.ListarPerfilLoginNivelAcessoExportacao(Orgao, Gestor, Uge, Almoxarifado);                   
            // var listaExportacao = usu.PopularDadosUsuarioSomenteGrid(Orgao, Gestor, Uge, Almoxarifado, Perfil, (string)acesso.Cpf, (int)acesso.Transacoes.Perfis[0].Peso, pesquisa, gerarExcel);
            IList<Usuario> listaExportacao = null;
            listaExportacao = (IList<Usuario>)Session["retornoexcel"];


            if (listaExportacao.Count > 0)
            {
                Response.Clear();
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment;filename=EXPORTACAO_USUARIO.xls");
                Response.ContentEncoding = System.Text.Encoding.GetEncoding("Windows-1252");
                Response.Charset = "ISO-8859-1";
                EnableViewState = false;
                //Status = item.Ativo==true?"Ativo":"Inativo"
                // IList<Sam.Entity.Usuario> xtp = new IList<Sam.Entity.Usuario>;


                using (StringWriter sw = new StringWriter())
                {
                    using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                    {
                        UGEPresenter uge = new UGEPresenter();
                        Perfil PF = new Perfil();

                        // List<Sam.Entity.Perfil> listagem = null;



                        if (Perfil == 2 && Almoxarifado == 3)
                        {
                            var lista =
                              (from item in listaExportacao
                               where item.Login.Perfil.Descricao == "Requisitante" && Almoxarifado.ToString() == AlmoxarifadoId.ToString()
                                   //where item.UgeId == Uge
                                   select new
                               {
                                   NomeUsuario = item.NomeUsuario.ToUpper(),
                                   CPF = item.Cpf,
                                   Email = item.Email,
                                       //Telefone=item.Telefone,
                                       Telefone = item.Fone,
                                   Perfil = item.Login.Perfil == null ? "" : item.Login.Perfil.DescricaoComCodigoEstrutura,
                                       //Perfil = item.Complemento,
                                       //Status = item.Ativo == true ? "Ativo" : "Inativo"
                                       Status = item.UsuarioAtivo == true ? "Ativo" : "Inativo"
                               }).OrderBy(a => a.NomeUsuario).ToList();// add where do perfil aqui
                            GridView gv = new GridView();
                            gv.DataSource = lista;
                            gv.DataBind();
                            gv.RenderControl(htw);
                            Response.Write(sw.ToString());
                            HttpContext.Current.Response.Flush();
                            HttpContext.Current.Response.SuppressContent = true;
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                        }

                        else if (Uge > 0)
                        {

                            var lista =
                              (from item in listaExportacao
                                       //where item.Perfil == 2
                                       //where item.UgeId == Uge
                                   select new
                               {
                                   NomeUsuario = item.NomeUsuario.ToUpper(),
                                   CPF = item.Cpf,
                                   Email = item.Email,
                                       //Telefone=item.Telefone,
                                       Telefone = item.Fone,
                                   Perfil = item.Login.Perfil == null ? "" : item.Login.Perfil.DescricaoComCodigoEstrutura,
                                       //Perfil = item.Complemento,
                                       //Status = item.Ativo == true ? "Ativo" : "Inativo"
                                       Status = item.UsuarioAtivo == true ? "Ativo" : "Inativo"
                               }).OrderBy(a => a.NomeUsuario).ToList();// add where do perfil aqui
                            GridView gv = new GridView();
                            gv.DataSource = lista;
                            gv.DataBind();
                            gv.RenderControl(htw);
                            Response.Write(sw.ToString());
                            HttpContext.Current.Response.Flush();
                            HttpContext.Current.Response.SuppressContent = true;
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                        }

                        else
                        {
                            var lista =
                                   (from item in listaExportacao
                                            //where item.Perfil == 2
                                            //where item.UgeId == Uge
                                        select new
                                    {
                                        NomeUsuario = item.NomeUsuario.ToUpper(),
                                        CPF = item.Cpf,
                                        Email = item.Email,
                                            //Telefone=item.Telefone,
                                            Telefone = item.Fone,
                                        Perfil = item.Login.Perfil == null ? "" : item.Login.Perfil.DescricaoComCodigoEstrutura,
                                            //Perfil = item.Complemento,
                                            //Status = item.Ativo == true ? "Ativo" : "Inativo"
                                            Status = item.UsuarioAtivo == true ? "Ativo" : "Inativo"
                                    }).OrderBy(a => a.NomeUsuario).ToList();// add where do perfil aqui
                            GridView gv = new GridView();
                            gv.DataSource = lista;
                            gv.DataBind();
                            gv.RenderControl(htw);
                            Response.Write(sw.ToString());
                            HttpContext.Current.Response.Flush();
                            HttpContext.Current.Response.SuppressContent = true;
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                        }
                    }
                }
            }
        }
        #endregion

     /*   public void PopularGrid()
        {
            try
            {

                if (!gerarExcel)
                {
                    PopularGridPaginacao(0); //Aqui vai ser zero pq é sempre a primeira página
                    //var retorno = usu.PopularDadosUsuarioGrid(Orgao, Gestor, Uge, Almoxarifado, Perfil, (string)acesso.Cpf, (int)acesso.Transacoes.Perfis[0].Peso, pesquisa);
                    //gridUsuario.DataSource = retorno;
                    //gridUsuario.DataBind();
                }
                else
                {
                    int Orgao = 0;
                    int Gestor = 0;
                    int? Perfil = 0;

                    int Uge = 0;
                    int Almoxarifado = 0;
                    int pagina = 0;
                    acesso = HttpContext.Current.Cache[Ticket] as Acesso;

                    if (this.ddlOrgao.Text != null && this.ddlOrgao.Text != "")
                    {
                        Orgao = Convert.ToInt32(this.ddlOrgao.SelectedItem.Value);
                    }

                    if (this.ddlGestor.Text != null && this.ddlGestor.Text != "")
                    {
                        Gestor = Convert.ToInt32(this.ddlGestor.SelectedItem.Value);
                    }

                    if (this.ddlPerfil.Text != null && this.ddlPerfil.Text != "" && this.ddlPerfil.Text != "- Todos -" && this.ddlPerfil.Text != "- Sem Perfil -")
                    {
                        Perfil = Convert.ToInt32(this.ddlPerfil.SelectedItem.Value);
                    }


                    UsuarioPresenter usu = new UsuarioPresenter(this);
                    if (this.ddlPerfil.Text == "- Todos -" || this.ddlPerfil.Text == "")
                    {
                        Perfil = 0;
                    }
                    if (this.cboUge.Text != null && cboUge.SelectedValue != "" && cboUge.Text != "- Todos -")
                    {
                        Uge = Convert.ToInt32(this.cboUge.SelectedItem.Value);
                    }
                    if (this.ddlPerfil.Text == "- Sem Perfil -")
                    {
                        Perfil = -1;
                        Uge = 0;
                        cboUge.SelectedIndex = 0;
                        ddlAlmoxarifado.Items.Clear();
                    }
                    if (this.ddlAlmoxarifado.Text != null && ddlAlmoxarifado.SelectedValue != "" && ddlAlmoxarifado.Text != "- Todos -" && ddlAlmoxarifado.Text != " ")
                    {
                        Almoxarifado = Convert.ToInt32(this.ddlAlmoxarifado.SelectedItem.Value);
                    }
                    string pesquisa = txtPesquisar.Text.Trim();

                    //var listaExportacao = usu.ListarPerfilLoginNivelAcessoExportacao(Orgao, Gestor, Uge, Almoxarifado);                   
                    // var listaExportacao = usu.PopularDadosUsuarioSomenteGrid(Orgao, Gestor, Uge, Almoxarifado, Perfil, (string)acesso.Cpf, (int)acesso.Transacoes.Perfis[0].Peso, pesquisa, gerarExcel);
                    IList<Usuario> listaExportacao = null;
                    listaExportacao = (IList<Usuario>)Session["excel"];


                    if (listaExportacao.Count > 0)
                    {
                        Response.Clear();
                        Response.ContentType = "application/vnd.ms-excel";
                        Response.AddHeader("Content-Disposition", "attachment;filename=EXPORTACAO_USUARIO.xls");
                        Response.ContentEncoding = System.Text.Encoding.GetEncoding("Windows-1252");
                        Response.Charset = "ISO-8859-1";
                        EnableViewState = false;
                        //Status = item.Ativo==true?"Ativo":"Inativo"
                        // IList<Sam.Entity.Usuario> xtp = new IList<Sam.Entity.Usuario>;


                        using (StringWriter sw = new StringWriter())
                        {
                            using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                            {
                                UGEPresenter uge = new UGEPresenter();
                                Perfil PF = new Perfil();

                                // List<Sam.Entity.Perfil> listagem = null;



                                if (Perfil == 2 && Almoxarifado == 3)
                                {
                                    var lista =
                                      (from item in listaExportacao
                                       where item.Login.Perfil.Descricao == "Requisitante" && Almoxarifado.ToString() == AlmoxarifadoId.ToString()
                                       //where item.UgeId == Uge
                                       select new
                                       {
                                           NomeUsuario = item.NomeUsuario.ToUpper(),
                                           CPF = item.Cpf,
                                           Email = item.Email,
                                           //Telefone=item.Telefone,
                                           Telefone = item.Fone,
                                           Perfil = item.Login.Perfil == null ? "" : item.Login.Perfil.DescricaoComCodigoEstrutura,
                                           //Perfil = item.Complemento,
                                           //Status = item.Ativo == true ? "Ativo" : "Inativo"
                                           Status = item.UsuarioAtivo == true ? "Ativo" : "Inativo"
                                       }).OrderBy(a => a.NomeUsuario).ToList();// add where do perfil aqui
                                    GridView gv = new GridView();
                                    gv.DataSource = lista;
                                    gv.DataBind();
                                    gv.RenderControl(htw);
                                    Response.Write(sw.ToString());
                                    HttpContext.Current.Response.Flush();
                                    HttpContext.Current.Response.SuppressContent = true;
                                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                                }

                                else if (Uge > 0)
                                {

                                    var lista =
                                      (from item in listaExportacao
                                           //where item.Perfil == 2
                                           //where item.UgeId == Uge
                                       select new
                                       {
                                           NomeUsuario = item.NomeUsuario.ToUpper(),
                                           CPF = item.Cpf,
                                           Email = item.Email,
                                           //Telefone=item.Telefone,
                                           Telefone = item.Fone,
                                           Perfil = item.Login.Perfil == null ? "" : item.Login.Perfil.DescricaoComCodigoEstrutura,
                                           //Perfil = item.Complemento,
                                           //Status = item.Ativo == true ? "Ativo" : "Inativo"
                                           Status = item.UsuarioAtivo == true ? "Ativo" : "Inativo"
                                       }).OrderBy(a => a.NomeUsuario).ToList();// add where do perfil aqui
                                    GridView gv = new GridView();
                                    gv.DataSource = lista;
                                    gv.DataBind();
                                    gv.RenderControl(htw);
                                    Response.Write(sw.ToString());
                                    HttpContext.Current.Response.Flush();
                                    HttpContext.Current.Response.SuppressContent = true;
                                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                                }

                                else
                                {
                                    var lista =
                                           (from item in listaExportacao
                                                //where item.Perfil == 2
                                                //where item.UgeId == Uge
                                            select new
                                            {
                                                NomeUsuario = item.NomeUsuario.ToUpper(),
                                                CPF = item.Cpf,
                                                Email = item.Email,
                                                //Telefone=item.Telefone,
                                                Telefone = item.Fone,
                                                Perfil = item.Login.Perfil == null ? "" : item.Login.Perfil.DescricaoComCodigoEstrutura,
                                                //Perfil = item.Complemento,
                                                //Status = item.Ativo == true ? "Ativo" : "Inativo"
                                                Status = item.UsuarioAtivo == true ? "Ativo" : "Inativo"
                                            }).OrderBy(a => a.NomeUsuario).ToList();// add where do perfil aqui
                                    GridView gv = new GridView();
                                    gv.DataSource = lista;
                                    gv.DataBind();
                                    gv.RenderControl(htw);
                                    Response.Write(sw.ToString());
                                    HttpContext.Current.Response.Flush();
                                    HttpContext.Current.Response.SuppressContent = true;
                                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                                }
                            }
                        }
                    }


                    else { return; }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }*/



        
        public void PopularGridPaginacao(int page = 0)
        {
            try
            {
                int Orgao = 0;
                int Gestor = 0;
                int? Perfil = 0;
                int Uge = 0;
                int Almoxarifado = 0;

                acesso = HttpContext.Current.Cache[Ticket] as Acesso;

                if (this.ddlOrgao.Text != null && this.ddlOrgao.Text != "")
                {
                    Orgao = Convert.ToInt32(this.ddlOrgao.SelectedItem.Value);
                }

                if (this.ddlGestor.Text != null && this.ddlGestor.Text != "")
                {
                    Gestor = Convert.ToInt32(this.ddlGestor.SelectedItem.Value);
                }

                if (this.ddlPerfil.Text != null && this.ddlPerfil.Text != "" && this.ddlPerfil.Text != "- Todos -" && this.ddlPerfil.Text != "- Sem Perfil -")
                {
                    Perfil = Convert.ToInt32(this.ddlPerfil.SelectedItem.Value);
                }


                UsuarioPresenter usu = new UsuarioPresenter(this);
                if (this.ddlPerfil.Text == "- Todos -" || this.ddlPerfil.Text == "")
                {
                    Perfil = 0;
                }
                if (this.cboUge.Text != null && cboUge.SelectedValue != "" && cboUge.Text != "- Todos -")
                {
                    Uge = Convert.ToInt32(this.cboUge.SelectedItem.Value);
                }
                if (this.ddlPerfil.Text == "- Sem Perfil -")
                {
                    Perfil = -1;
                    Uge = 0;
                    cboUge.SelectedIndex = 0;
                    ddlAlmoxarifado.Items.Clear();
                }
                if (this.ddlAlmoxarifado.Text != null && ddlAlmoxarifado.SelectedValue != "" && ddlAlmoxarifado.Text != "- Todos -" && ddlAlmoxarifado.Text != " ")
                {
                    Almoxarifado = Convert.ToInt32(this.ddlAlmoxarifado.SelectedItem.Value);
                }

                string pesquisa = txtPesquisar.Text.Trim();

                LimpaGrid();
                var retorno = usu.PopularDadosUsuarioSomenteGrid(Orgao, Gestor, Uge, Almoxarifado, Perfil, (string)acesso.Cpf, (int)acesso.Transacoes.Perfis[0].Peso, pesquisa, gerarExcel, page);
          
                gridUsuario.DataSource = retorno;
                gridUsuario.DataBind();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void PopularEXCEL(int page = 0)
        {
            try
            {
                int Orgao = 0;
                int Gestor = 0;
                int? Perfil = 0;
                int Uge = 0;
                int Almoxarifado = 0;

                acesso = HttpContext.Current.Cache[Ticket] as Acesso;

                if (this.ddlOrgao.Text != null && this.ddlOrgao.Text != "")
                {
                    Orgao = Convert.ToInt32(this.ddlOrgao.SelectedItem.Value);
                }

                if (this.ddlGestor.Text != null && this.ddlGestor.Text != "")
                {
                    Gestor = Convert.ToInt32(this.ddlGestor.SelectedItem.Value);
                }

                if (this.ddlPerfil.Text != null && this.ddlPerfil.Text != "" && this.ddlPerfil.Text != "- Todos -" && this.ddlPerfil.Text != "- Sem Perfil -")
                {
                    Perfil = Convert.ToInt32(this.ddlPerfil.SelectedItem.Value);
                }


                UsuarioPresenter usu = new UsuarioPresenter(this);
                if (this.ddlPerfil.Text == "- Todos -" || this.ddlPerfil.Text == "")
                {
                    Perfil = 0;
                }
                if (this.cboUge.Text != null && cboUge.SelectedValue != "" && cboUge.Text != "- Todos -")
                {
                    Uge = Convert.ToInt32(this.cboUge.SelectedItem.Value);
                }
                if (this.ddlPerfil.Text == "- Sem Perfil -")
                {
                    Perfil = -1;
                    Uge = 0;
                    cboUge.SelectedIndex = 0;
                    ddlAlmoxarifado.Items.Clear();
                }
                if (this.ddlAlmoxarifado.Text != null && ddlAlmoxarifado.SelectedValue != "" && ddlAlmoxarifado.Text != "- Todos -" && ddlAlmoxarifado.Text != " ")
                {
                    Almoxarifado = Convert.ToInt32(this.ddlAlmoxarifado.SelectedItem.Value);
                }

                string pesquisa = txtPesquisar.Text.Trim();


                var retornoexcel = usu.PopularDadosUsuarioSomenteGridExcel(Orgao, Gestor, Uge, Almoxarifado, Perfil, (string)acesso.Cpf, (int)acesso.Transacoes.Perfis[0].Peso, pesquisa, gerarExcel, page);

                Session["retornoexcel"] = retornoexcel;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public void WriteTsv<T>(IEnumerable<T> data, TextWriter output)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            foreach (PropertyDescriptor prop in props)
            {
                output.Write(prop.DisplayName); // header
                output.Write("\t");
            }
            output.WriteLine();
            foreach (T item in data)
            {
                foreach (PropertyDescriptor prop in props)
                {
                    output.Write(prop.Converter.ConvertToString(
                         prop.GetValue(item)));
                    output.Write("\t");
                }
                output.WriteLine();
            }
        }
        public void PopularOrgao()//conferir a o user presenter acesso estrutura orgao
        {
            UsuarioPresenter usuarioprese = new UsuarioPresenter(this);
            OrgaoPresenter org = new OrgaoPresenter();
            //this.usuarioId = GetAcesso.Transacoes.Usuario.Id;           
            var usulogado = GetAcesso.Transacoes.Perfis[0].Descricao;
            orgaoId = GetAcesso.Transacoes.Perfis[0].OrgaoPadrao.Id.Value;

            if (usulogado == "Administrador Geral")
            {
                ddlOrgao.DataSource = org.PopularDadosTodosCod();
                ddlOrgao.DataBind();
            }
            else
            {
                ddlOrgao.DataSource = org.PopularDadosTodosCod(orgaoId);
                ddlOrgao.DataBind();
            }
        }


        public void PopularGestor()
        {
            GestorPresenter gestor = new GestorPresenter();
            UsuarioPresenter usuarioprese = new UsuarioPresenter(this);

            ddlGestor.Items.Clear();
            if (orgaoId > 0)
            {
                ddlGestor.DataSource = gestor.PopularDadosGestorTodosCod(orgaoId.Value);
                ddlGestor.DataBind();
            }
            else
            {
                ddlGestor.DataSource = gestor.PopularDadosGestorTodosCod(orgaoId.Value);
                ddlGestor.DataBind();
            }
        }

        public void PopularUge()
        {
            cboUge.Items.Clear();
            if (orgaoId > 0)
            {
                UGEPresenter uge = new UGEPresenter();
                cboUge.Items.Add("- Todos -");
                cboUge.AppendDataBoundItems = true;
                cboUge.DataSource = uge.PopularDadosUgeCodigo(orgaoId.Value).Distinct().OrderBy(a => a.Codigo);
                cboUge.DataBind();
                ddlPerfil.SelectedIndex = 0;
            }
            else
            {
                UGEPresenter uge = new UGEPresenter();
                cboUge.Items.Add("- Todos -");
                cboUge.AppendDataBoundItems = true;
                cboUge.DataSource = uge.PopularDadosUgeCodigo(orgaoId.Value).Distinct().OrderBy(a => a.Codigo);
                cboUge.DataBind();
                ddlPerfil.SelectedIndex = 0;
            }

        }
        public void PopularAlmoxarifado()
        {
            ddlAlmoxarifado.Items.Clear();
            if (UgeId > 0)
            {
                AlmoxarifadoPresenter almoxarifado = new AlmoxarifadoPresenter();
                ddlAlmoxarifado.AppendDataBoundItems = true;
                var retorno = almoxarifado.ListarAlmoxarifadoPorUge(UgeId.Value);
                if (retorno.Count == 0)
                {
                    ddlAlmoxarifado.Items.Add(" ");

                    LimpaGrid();
                    return;
                }
                ddlAlmoxarifado.DataSource = almoxarifado.ListarAlmoxarifadoPorUge(UgeId.Value);
                ddlAlmoxarifado.DataBind();

            }
            else { ddlAlmoxarifado.SelectedValue = ""; }

        }
        public void PopularPerfil()
        {
            ddlPerfil.Items.Clear();
            ddlPerfil.Items.Add("- Todos -");
            ddlPerfil.Items.Add("- Sem Perfil -");
            ddlPerfil.Text = "- Todos -";
            PerfilPresenter perfil = new PerfilPresenter();

            if (Ticket == null)
                return;

            acesso = HttpContext.Current.Cache[Ticket] as Acesso;
            int? Peso = null;
            if (acesso != null)
            {
                Peso = (int)acesso.Transacoes.Perfis[0].Peso;
            }

            ddlPerfil.DataSource = perfil.PopularDadosPerfil(Peso);
            ddlPerfil.DataBind();
        }

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

        public bool BloqueiaNovo
        {
            set { btnNovo.Enabled = !value; }
        }

        public bool BloqueiaGravar
        {
            set { btnGravar.Enabled = !value; }
        }

        public bool BloqueiaCancelar
        {
            set { btnCancelar.Enabled = !value; }
        }

        public bool BloqueiaExcluir { set { } }

        public bool BloqueiaCodigo { set { } }

        public bool BloqueiaDescricao { set { } }

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

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            //this.PerfilId = int.Parse(this.ddlPerfil.SelectedValue);

            UsuarioPresenter usu = new UsuarioPresenter(this);
            if (!regExEmail.IsValid)
            {
                ExibirMensagem("Email inválido!");
                return;
            }
            usu.Gravar();

        }

        protected void btnPesquisar_Click(object sender, EventArgs e)//gridUsuario_PageIndexChanging
        {
            ResetGridPesquisa();
            PopularGrid();
        }

        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            gerarExcel = true;

            //PopularGrid();
            GeradorExcel();

        }
        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            UsuarioPresenter usu = new UsuarioPresenter(this);
            usu.Excluir();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            UsuarioPresenter usu = new UsuarioPresenter(this);
            usu.Cancelar();
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            UsuarioPresenter usu = new UsuarioPresenter(this);
            usu.Novo();
            txtCPF.Focus();
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            UsuarioPresenter usu = new UsuarioPresenter(this);
            usu.Imprimir(this.ddlPerfil.Text == "- Sem Perfil -" ? RelatorioEnum.UsuariosSemPerfil : RelatorioEnum.Usuarios);
        }


        public string CPF
        {
            get { return txtCPF.Text; }
            set { txtCPF.Text = value; }
        }

        public bool? Ativo
        {
            get { return TratamentoDados.TryParseBool(ddlAtivo.SelectedValue); }
            set
            {
                ListItem item = ddlAtivo.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlAtivo.ClearSelection();
                    item.Selected = true;
                }
            }
        }

        public string NomeUsuario
        {
            get { return txtNome.Text.Trim(); }
            set { txtNome.Text = value; }
        }
        public string Senha
        {
            get { return txtSenha.Text; }
            set
            {
                txtSenha.TextMode = TextBoxMode.SingleLine;
                txtSenha.Text = value;
            }
        }

        public long? Telefone
        {
            get { return TratamentoDados.TryParseLong(TratamentoDados.RetirarMascara(txtTelefone.Text)); }
            set { txtTelefone.Text = value.ToString(); }
        }

        public int? UsuarioIdResponsavel { get; set; }

        public string MsgSenha
        {
            get { return lblMsgSenha.Text; }
            set { lblMsgSenha.Text = value; }
        }

        public int? OrgaoId
        {
            get { return TratamentoDados.TryParseInt32(ddlOrgao.SelectedValue); }
            set
            {
                ListItem item = ddlOrgao.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    item.Selected = true;
                }
            }
        }

        public int? UgeId
        {
            get { return TratamentoDados.TryParseInt32(cboUge.SelectedValue); }
            set
            {
                ListItem item = cboUge.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    // ddlUge.ClearSelection();
                    item.Selected = true;
                }
            }
        }
        public int? AlmoxarifadoId
        {
            get { return TratamentoDados.TryParseInt32(ddlAlmoxarifado.SelectedValue); }
            set
            {
                ListItem item = ddlAlmoxarifado.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    item.Selected = true;
                }
            }
        }
        public int? OrgaoPdId
        {
            get { return TratamentoDados.TryParseInt32(ddlOrgao.SelectedValue); }
            set
            {
                ListItem item = ddlOrgao.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    item.Selected = true;
                }
            }
        }


        public int? GestorId
        {
            get { return TratamentoDados.TryParseInt32(ddlGestor.SelectedValue); }
            set
            {
                ListItem item = ddlGestor.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    item.Selected = true;
                }
            }
        }


        public int? GestorPdId
        {
            get { return TratamentoDados.TryParseInt32(ddlGestor.SelectedValue); }
            set
            {
                ListItem item = ddlGestor.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    item.Selected = true;
                }
            }
        }

        protected void ddlOrgao_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ddlOrgao.Text != "")
            {
                orgaoId = Convert.ToInt32(ddlOrgao.SelectedValue);
                PopularGestor();
                PopularUge();
                PopularPerfil();
            }
        }

        protected void gridUsuario_SelectedIndexChanged(object sender, EventArgs e)
        {
            Label lblId = (Label)gridUsuario.Rows[gridUsuario.SelectedIndex].FindControl("lblId");
            Label lblCpf = (Label)gridUsuario.Rows[gridUsuario.SelectedIndex].FindControl("lblCpf");
            Label lblNome = (Label)gridUsuario.Rows[gridUsuario.SelectedIndex].FindControl("lblNome");

            this.Id = lblId.Text;

            UsuarioPresenter usu = new UsuarioPresenter(this);
            usu.LerRegistro(this.Id);

            txtCPF.Focus();
        }

        public string Email
        {
            get { return txtEmail.Text.Trim(); }
            set { txtEmail.Text = value; }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        public int? PerfilId
        {
            get;
            set;
        }

        protected void btnGerarSenha_Click(object sender, EventArgs e)
        {
            UsuarioPresenter usu = new UsuarioPresenter(this);
            usu.GerarSenha();
        }

        protected void gridUsuario_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "perfil")
            {
                int index = int.Parse((string)e.CommandArgument);
                GridViewRow row = gridUsuario.Rows[index];
                Label lblId = (Label)gridUsuario.Rows[index].FindControl("lblId");
                Label lblCpf = (Label)gridUsuario.Rows[index].FindControl("lblCpf");
                Label lblNome = (Label)gridUsuario.Rows[index].FindControl("lblNome");

                if (lblId != null)
                {
                    UsuarioEntity usuarioEditado = new UsuarioEntity();

                    usuarioEditado.Id = Convert.ToInt32(lblId.Text);
                    usuarioEditado.Nome = lblNome.Text;
                    usuarioEditado.Cpf = lblCpf.Text;

                    SetSession<UsuarioEntity>(usuarioEditado, "usuarioEditado");
                    Response.Redirect("SEGUsuarioPerfil.aspx", false);

                    //Response.Redirect("SEGUsuarioPerfil.aspx?userId=" + lblId.Text + "&userCpf=" + lblCpf.Text + "&userNome=" + Server.UrlEncode(lblNome.Text));
                }
            }
        }

        protected void gridUsuario_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            PopularGridPaginacao(e.NewPageIndex);
            gridUsuario.PageIndex = e.NewPageIndex;
            gridUsuario.DataBind();
        }

        protected void ddlGestor_SelectedIndexChanged1(object sender, EventArgs e)
        {
            PopularUge();

        }

        protected void cboUge_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopularAlmoxarifado();
            // PopularGrid();
        }

        protected void ddlAlmoxarifado_SelectedIndexChanged(object sender, EventArgs e)
        {

            // PopularGrid();
        }
        protected void ddlPerfil_SelectedIndexChanged(object sender, EventArgs e)
        {

            //PopularGrid();
        }

        protected void inicializarCombos(DropDownList ddl)
        {
            ddl.Items.Clear();
            ddl.Items.Add("- Selecione -");
            ddl.Items[0].Value = "0";
            ddl.AppendDataBoundItems = true;
        }

        public void ExibirRelatorio()
        {
            SetSession<RelatorioEntity>(this.DadosRelatorio, base.ChaveImpressaoUsuario);
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), Constante.ReportScript, false);
        }

        protected void ddlPdPerfil_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //Verifica o nivel de acesso com base no perfil do usuario logado
        public void AcessoPerfil()
        {
            acesso = HttpContext.Current.Cache[Ticket] as Acesso;

            if (acesso.Transacoes.Perfil.IdPerfil == (int)Sam.Common.Perfil.ADMINISTRADOR_GERAL)
            {
                this.ddlOrgao.Enabled = true;
                this.ddlGestor.Enabled = true;
            }
            else if (acesso.Transacoes.Perfil.IdPerfil == (int)Sam.Common.Perfil.ADMINISTRADOR_GESTOR)
            {
                this.ddlOrgao.SelectedValue = acesso.Transacoes.Usuario.OrgaoPadrao.ToString();
                this.ddlOrgao.Enabled = false;
                this.ddlGestor.Enabled = true;
            }
            else
            {
                this.ddlOrgao.Enabled = false;
                this.ddlGestor.Enabled = false;
            }
        }
        public void LimpaGrid()
        {

            gridUsuario.DataSource = null;
            gridUsuario.DataBind();
        }
        public void ResetGridPesquisa()
        {
            //metodo para restaurar o grid quando o usuario selecionar outro perfil na combo.
            DataTable ds = new DataTable();
            ds.Reset();
            gridUsuario.DataSource = ds;
            gridUsuario.DataBind();
        }
    }
}
