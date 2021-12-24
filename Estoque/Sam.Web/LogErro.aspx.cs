using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using Sam.View;
using Sam.Common.Util;
using Sam.Presenter;
using System.Collections.Generic;
using Sam.Domain.Entity;


namespace Sam.Web
{

    public partial class LogErro : Page, ILogErroView
    {
        public int Id
        {
            get
            {
                return 1000;
            }
            set
            {

            }
        }

        public string Message
        {
            get
            {
                return string.Empty;
            }
            set
            {

            }

        }

        public string StrackTrace
        {
            get
            {
                return string.Empty;
            }
            set
            {

            }
        }

        public DateTime Data
        {
            get
            {
                return DateTime.Now;
            }
            set
            {

            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.gridLogErro.DataSourceID = "sourceDados";
            }

        }

        /// <summary>
        /// Popula a lista de erros.
        /// </summary>
        /// <param name="startRowIndexParameterName"></param>
        /// <param name="maximumRowsParameterName"></param>
        /// <returns></returns>
        public IList<LogErroEntity> PopularListaLogErro(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            var presenter = new LogErroPresenter();
            var result = presenter.PopularListaLogErro(startRowIndexParameterName, maximumRowsParameterName);
            this.TotalRegistrosGrid = presenter.TotalRegistrosGrid;
            return result;
        }

        /// <summary>
        /// Evento disparado ao clicar na troca de páginas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gridLogErro_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridLogErro.PageIndex = e.NewPageIndex;
        }

        int TotalRegistrosGrid = 0;
        /// <summary>
        /// Retorna o total de registros para o grid
        /// </summary>
        /// <param name="maximumRowsParameterName"></param>
        /// <param name="startRowIndexParameterName"></param>
        /// <returns></returns>
        public int TotalRegistros(int maximumRowsParameterName, int startRowIndexParameterName)
        {
            return this.TotalRegistrosGrid;
        }
    }
}
