using System;
using Sam.Common;
using System.Web.UI;
using Sam.Common.Util;
using Sam.Web.Financeiro;


namespace Sam.Web.Controles
{
    public partial class EntradaCampoCE : UserControl
    {
        public static string ChaveSessao_CampoValorCE = "valorCampoCE";
        public static string ChaveSessao_CampoValorNumDoc = "valorCampoNumDoc";
        public static string ChaveSessao_CampoValorGestor = "valorCampoGestor";
        public static string ChaveSessao_CampoValorUge = "valorCampoUge";
        public static string ChaveSessao_valorPreenchidoCampoCE = "valorPreenchidoCampoCE";

        public delegate void chamaEvento();
        public delegate void EventoCancelar();

        public event chamaEvento EvchamaEvento;
        public event EventoCancelar EvchamaEventoCancelar;
        public bool ExibirUge { get; set; }
        public string classExibirControle { get { return ExibirUge ? "" : "esconderControle"; } }

        public static bool valorPreenchidoCampoCE
        {
            get; set;
        }

        public string ValorCampoCE
        {
            get { return txtValorCE.Text; }
            set { txtValorCE.Text = value; }
        }

        public string NumeroDocumento
        {
            get { return hdnNumeroDocumento.Value; }
            set { hdnNumeroDocumento.Value = value; }
        }

        public string Uge
        {
            get { return txtValorUgeDestino.Text.Trim(); }
            set { txtValorUgeDestino.Text = value; }
        }

        public bool ProcessarFormulario
        {
            get
            {
                bool _retorno = false;
                bool.TryParse(hdnProcessar.Value, out _retorno);
                return _retorno;
            }
            set
            {
                hdnProcessar.Value = value.ToString();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PageBase.GetSession<string>(ChaveSessao_CampoValorCE) != ValorCampoCE && !ProcessarFormulario)
            {
                PageBase.SetSession<string>(ValorCampoCE, ChaveSessao_CampoValorCE);
            }

            if (ExibirUge)
            {
                ScriptManager.RegisterStartupScript(txtValorUgeDestino, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);
            }
        }

        protected void btnValorOK_Click(object sender, EventArgs e)
        {
            WebUtil webUtil = new WebUtil();
            PageBase.SetSession<string>(ValorCampoCE, ChaveSessao_CampoValorCE);
            PageBase.SetSession<string>(string.IsNullOrEmpty(Uge) ? null : Uge, ChaveSessao_CampoValorUge);
            ProcessarFormulario = true;

            webUtil.runJScript(this.Parent.Page, "CloseModalCampoSiafemCE();");
            //webUtil.runJScript(this.Parent.Page, "btnValorOK();");

            if ((!String.IsNullOrWhiteSpace(ValorCampoCE)))
                valorPreenchidoCampoCE = true;

            EvchamaEvento();
        }

        protected void btnCancelarEntradaValorCE_Click(object sender, EventArgs e)
        {
            ProcessarFormulario = false;
            EvchamaEventoCancelar();
        }

        protected void txtValorCE_TextChanged(object sender, EventArgs e)
        {
            ValorCampoCE = ValorCampoCE.Replace(" ", "");
            if (ValorCampoCE == "999")
                ValorCampoCE = Constante.CST_SIAFEM_CE_PADRAO;
        }
    }
}
