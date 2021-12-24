using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.Entity;
using Sam.Presenter;
using Sam.Common;
using Sam.Common.Util;
using System.IO;
using System.Web.Security;

namespace Sam.Web
{
    public partial class Download : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                    Response.Redirect("login.aspx", false);

                string ticket = ((System.Web.Security.FormsIdentity)(User.Identity)).Ticket.UserData;

                var acesso = Cache[ticket] as Acesso;

                if (acesso.Transacoes.TrocarSenha == true)
                {
                    Response.Redirect("~/Seguranca/SEGAlterSenha.aspx", false);
                }

                if (!IsPostBack)
                {
                    PopularListaRecursosDeApoio();
                }
            }
            catch (Exception ex)
            {
                new Presenter.LogErroPresenter().GravarLogErro(ex);
                Response.Redirect(FormsAuthentication.LoginUrl, false);
            }
        }

        private void PopularListaRecursosDeApoio()
        {
            MaterialApoioPresenter    lObjPresenter = new MaterialApoioPresenter();
            List<MaterialApoioEntity> listaMaterialApoio = new List<MaterialApoioEntity>();

            var PerfilLogado = new PageBase().GetAcesso.Transacoes.Perfis[0];

            listaMaterialApoio = lObjPresenter.ListarRecursosPorPerfil(PerfilLogado.IdPerfil);

            PopularGridDocumentos(listaMaterialApoio);
            PopularGridVideos(listaMaterialApoio);
        }

        private void PopularGridVideos(List<MaterialApoioEntity> listaRecursos)
        {
            if (listaRecursos != null && listaRecursos.Count > 0)
            {
                List<MaterialApoioEntity> listaMaterialApoioVideos = listaRecursos.Where(MaterialApoio => MaterialApoio.TipoRecurso.ToLowerInvariant() == "avi").ToList();

                grdVideos.Visible = true;
                grdVideos.DataSource = listaMaterialApoioVideos;
                grdVideos.DataBind();
            }
        }

        private void PopularGridDocumentos(List<MaterialApoioEntity> listaRecursos)
        {
            if (listaRecursos != null && listaRecursos.Count > 0)
            {
                List<MaterialApoioEntity> listaMaterialApoioDocumentos = listaRecursos.Where(MaterialApoio => MaterialApoio.TipoRecurso.ToLowerInvariant() == "pdf").ToList();

                grdDocumentos.Visible = true;
                grdDocumentos.DataSource = listaMaterialApoioDocumentos;
                grdDocumentos.DataBind();
            }
        }


        protected void linkCodigo_Command(object sender, CommandEventArgs e)
        {
            MaterialApoioPresenter objPresenter = null;
            MaterialApoioEntity    objEntidade  = null;

            string strUrlDownload    = null;
            int    iMaterialApoio_ID = 0;

            if (e.CommandName == "Select")
            {
                iMaterialApoio_ID = Int32.Parse(e.CommandArgument.ToString().Trim());

                if (iMaterialApoio_ID != 0)
                {
                    objPresenter = new MaterialApoioPresenter();
                    objEntidade  = objPresenter.ObterDadosMaterialApoio(iMaterialApoio_ID);

                    strUrlDownload = String.Format("{0}{1}\\{2}\\{3}", Constante.FullPhysicalPathApp, Constante.MaterialApoioFolder, objEntidade.PathArquivo, objEntidade.NomeArquivo);
                    ForceDownload(strUrlDownload, null);
                }
            }
        }

        protected string RetornaPathFileIconeTipoRecurso(string strTipoRecurso)
        {
            string strRetorno = null;

            if (!String.IsNullOrWhiteSpace(strTipoRecurso))
            {
                switch (strTipoRecurso.ToLowerInvariant())
                {
                    case "pdf": strRetorno = "download_pdf.png"; break;
                    case "avi": strRetorno = "dowload_video.png"; break;

                    default: strRetorno = "download_arquivo_qualquer.png"; break;
                }

                strRetorno = String.Format("../{0}/{1}", Constante.ImagesFolder, strRetorno);
            }

            return strRetorno;
        }

        /// <summary>
        /// Rotina para forçar o download de arquivos
        /// </summary>
        /// <param name="caminhoArquivo">Caminho para o arquivo no sistema de arquivos</param>
        /// <param name="contentType">Content-Type do arquivo (opcional)</param>
        protected void ForceDownload(string caminhoArquivo, string contentType)
        {
            if (contentType == null)
                contentType = "application/octet-stream";

            FileInfo arquivo = new FileInfo(caminhoArquivo);
            Response.Clear();
            Response.AddHeader("Content-Disposition", "attachment; filename=" + arquivo.Name);
            Response.AddHeader("Content-Length", arquivo.Length.ToString());
            Response.ContentType = contentType;
            Response.WriteFile(arquivo.FullName);
            Response.End();
        }
    }
}
