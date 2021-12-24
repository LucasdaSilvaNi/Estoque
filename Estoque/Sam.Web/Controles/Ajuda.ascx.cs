using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.Domain.Entity;
using Sam.Presenter;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Web.Hosting;

namespace Sam.Web.Controles
{
    public partial class Ajuda : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //CarregarTextoAjuda(); Desativado
            }
            catch
            {
            }
        }

        private void CarregarTextoAjuda()
        {
            var _urlCompleta = string.Format("{0}://{1}:{2}{3}"
                           , Request.ServerVariables["HTTPS"] != null
                                       ? Request.ServerVariables["SERVER_PROTOCOL"].ToString().ToLower().Substring(0, 4)
                                       : Request.ServerVariables["SERVER_PROTOCOL"].ToString().ToLower().Substring(0, 5)
                           , Request.ServerVariables["SERVER_NAME"].ToString()
                           , Request.ServerVariables["SERVER_PORT"].ToString()
                           , Request.ServerVariables["SCRIPT_NAME"].ToString());

            string urlTransacao = _urlCompleta.Substring(_urlCompleta.LastIndexOf("/") + 1).ToLower();

            string filePath = HttpContext.Current.Request.MapPath("~/Controles/AjudaTexto.xml");

            //string filePath = String.Format("{0}{1}", HostingEnvironment.ApplicationPhysicalPath, @"Controles\AjudaTexto.xml");

            XElement xmlEl = XElement.Load(filePath);

            
            var xmlResult = (from a in xmlEl.Elements("pagina")
                             where a.Element("nomePagina").ToString().ToLower().Contains(urlTransacao)
                             select a).FirstOrDefault();

            if (xmlResult != null)
            {
                string tituloAjuda = xmlResult.Element("tituloAjuda").Value;
                string corpo = xmlResult.Element("corpo").ToString();

                lblTituloAjuda.Text = tituloAjuda;
                divCorpo.InnerHtml = corpo;


                //lblCorpo2.Text = Corpo2;
            }
        }
    }

 
}
