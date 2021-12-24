using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EstoquePatrimonio.Security.Facade;
using EstoquePatrimonio.View;
using EstoquePatrimonio.Domain.Business;
using EstoquePatrimonio.Domain.Entity;
using EstoquePatrimonio.Common.Util;
using EstoquePatrimonio.Security.Domain.Entity;

namespace EstoquePatrimonio.Presenter
{
    public class TesteWebservicePresenter : IStatusTesteWebserviceView
    {
        private clsTesteWebservice lObjTesterWsProd;
        private clsTesteWebservice lObjTesterWsHml;

        public TesteWebservicePresenter()
        { }

        public void TestarWebservices()
        {

            #region WS Produção

            lObjTesterWsProd = new clsTesteWebservice(clsTesteWebservice.PRODUCAO);
            lObjTesterWsProd.ObterStatusServico();

            if (!String.IsNullOrEmpty(lObjTesterWsProd.Erro))
            {
                lblMsgDesenvolvimento.Text += lObjTesterWsProd.Erro;
                lblMsgDesenvolvimento.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                lblMsgDesenvolvimento.Text += lObjTesterWsProd.Messagem;
                lblMsgDesenvolvimento.ForeColor = System.Drawing.Color.Blue;
            }

            #endregion WS Produção

            #region WS Homologação

            lObjTesterWsHml = new clsTesteWebservice(clsTesteWebservice.HOMOLOGACAO);
            lObjTesterWsHml.ObterStatusServico();

            if (!String.IsNullOrEmpty(lObjTesterWsHml.Erro))
            {
                lblMsgHomologacao.Text += lObjTesterWsHml.Erro;
                lblMsgHomologacao.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                lblMsgHomologacao.Text += lObjTesterWsHml.Messagem;
                lblMsgHomologacao.ForeColor = System.Drawing.Color.Blue;
            }

            #endregion WS Homologação

        }
        public void LimparControles()
        {
            Master.FindControl("pnlBarraGestor").Visible = false;
            Master.FindControl("pnlUsuario").Visible = false;
        }

    }      
}
