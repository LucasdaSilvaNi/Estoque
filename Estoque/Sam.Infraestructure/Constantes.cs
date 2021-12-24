using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Sam.Common
{
    public class Constantes
    {
        //Evento do Grid
        public static string SelectEvent
        {
            get { return "Select"; }
        }
        public static string DeleteEvent
        {
            get { return "Delete"; }
        }
        public static string PerfilEvent
        {
            get { return "Perfil"; }
        }        

        public static string LinkDeleteName
        {
            get { return "linkIDDelete"; }
        }

        public static string LinkEditName
        {
            get { return "linkIDEdit"; }
        }

        public static string TituloPainelEdit
        {
            get { return "Editar Registro"; }
        }

        public static string TituloPainelNovo
        {
            get { return "Novo Registro"; }
        }

        public static string CrudException
        {
            get { return "crudException"; }
        }

        public static string RelatorioException
        {
            get { return "relatorioException"; }
        }

        

        #region Relatorio

        static readonly string pathApplication = HttpContext.Current.Request.ApplicationPath == "/" ? "" : HttpContext.Current.Request.ApplicationPath;
        public static string ReportPath = pathApplication + "/Relatorios/";
        public static string ReportUrl = pathApplication + "/Relatorios/imprimirRelatorio.aspx";
        public static string ImageUrl = pathApplication + "/Relatorios/obterImagem.ashx";
        public static string ReportScript = "<script language='javascript'>" + "window.open('" + ReportUrl + "', 'CustomPopUp', " + "'width=800,height=600,toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=yes, resizable=yes, copyhistory=no, top=0, left=0')" + "</script>";

        #endregion
    }
}
