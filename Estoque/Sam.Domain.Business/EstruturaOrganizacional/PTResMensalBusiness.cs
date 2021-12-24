using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Linq;
using Sam.Domain.Entity;
using Sam.ServiceInfraestructure;
using Sam.Common.Util;
using System.Data.SqlClient;
using System.Xml;
using Sam.Common;
using System.Transactions;

namespace Sam.Domain.Business
{
    public partial class EstruturaOrganizacionalBusiness : BaseBusiness
    {
        #region PTResMensal

        private PTResMensalEntity pTResM = new PTResMensalEntity();

        public PTResMensalEntity PTResM
        {
            get { return pTResM; }
            set { pTResM = value; }
        }

        public bool SalvarPTResMensal()
        {
            this.Service<IPTResMensalService>().Entity = this.PTResM;
            this.ConsistirPTRes();
            if (this.Consistido)
            {
                this.Service<IPTResService>().Salvar();
            }
            return this.Consistido;
        }

        public IList<PTResMensalEntity> ListarPTResMensal()
        {
            this.Service<IPTResMensalService>().SkipRegistros = this.SkipRegistros;
            IList<PTResMensalEntity> retorno = this.Service<IPTResMensalService>().Listar();
            this.TotalRegistros = this.Service<IPTResService>().TotalRegistros();
            return retorno;
        }

        public IList<PTResMensalEntity> ImprimirPTResMensal()
        {
            IList<PTResMensalEntity> retorno = this.Service<IPTResMensalService>().Imprimir();
            return retorno;
        }

        public string wsSIAFDetPTRESMensal(string UG, string gestao, string mes, string diaMesInicial, string diaMesFinal)
        {
            //UG = "200147";

            System.Text.StringBuilder sMensagemEstimulo = new System.Text.StringBuilder();
            sMensagemEstimulo.AppendLine("<SIAFDOC>");
            sMensagemEstimulo.AppendLine("    <cdMsg>SIAFDetPTRES</cdMsg>");
            sMensagemEstimulo.AppendLine("    <SiafemDocDetPTRES>");
            sMensagemEstimulo.AppendLine("        <documento>");
            sMensagemEstimulo.AppendLine("            <UG>" + UG + "</UG>");  // + uge.Codigo.ToString() + "</UG>");
            sMensagemEstimulo.AppendLine("            <Gestao>" + gestao + "</Gestao>");
            sMensagemEstimulo.AppendLine("            <Mes>" + mes + "</Mes>"); // + MesExtenso.Mes[Convert.ToInt16(MesRef.Substring(4, 2).ToString())] + "</Mes>"); 
            sMensagemEstimulo.AppendLine("            <DiaMesInicial>" + diaMesInicial + "</DiaMesInicial>"); //+ MesExtenso.Mes[Convert.ToInt16(MesRef.Substring(4, 2).ToString())] + "</DiaMesInicial>");
            sMensagemEstimulo.AppendLine("            <DiaMesFinal>" + diaMesFinal + "</DiaMesFinal>"); // + ultimoDiaMes + MesExtenso.Mes[Convert.ToInt16(MesRef.Substring(4, 2).ToString())] + "</DiaMesFinal>"); 
            sMensagemEstimulo.AppendLine("        </documento>");
            sMensagemEstimulo.AppendLine("    </SiafemDocDetPTRES>");
            sMensagemEstimulo.AppendLine("</SIAFDOC>");
            return sMensagemEstimulo.ToString();
        }


        public IList<PTResMensalEntity> CarregarListaPTResM(int ugeCodigo)
        {
            #region Variaveis
            IList<PTResMensalEntity> lstPTRes = null;
            #endregion Variaveis

            try
            {
                using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    try
                    {
                        lstPTRes = this.Service<IPTResMensalService>().Listar();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        tras.Complete();
                    }
                }

                return lstPTRes;
            }
            catch (Exception e)
            {
                new LogErro().GravarLogErro(e);
                this.ListaErro.Add("Erro no sistema: " + e.Message);
                return null;
            }

            return lstPTRes;
        }

        public IList<PTResMensalEntity> CarregarListaPTResMAcao(int ugeCodigo)
        {
            #region Variaveis
            IList<PTResMensalEntity> lstPTRes = null;
            #endregion Variaveis

            try
            {
                using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    try
                    {
                        lstPTRes = null;// this.Service<IPTResMensalService>().ObterNLConsumoPaga();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        tras.Complete();
                    }
                }

                return lstPTRes;
            }
            catch (Exception e)
            {
                new LogErro().GravarLogErro(e);
                this.ListaErro.Add("Erro no sistema: " + e.Message);
                return null;
            }

            return lstPTRes;
        }

        /// <summary>
        /// Método criado para trazer lista de PTRes, em cima de retorno da mensagem SIAFDetaContaGen, para uma determinada UGE
        /// </summary>
        /// <param name="xmlListNodes"></param>
        /// <returns></returns>
        private IList<string> processarListaPTResMensal(XmlNodeList xmlListNodes)
        {
            List<string> listaEmpenhos = new List<string>();

            listaEmpenhos = xmlListNodes.Cast<XmlNode>()
                                        .AsQueryable<XmlNode>()
                                        .SelectMany(nodes => nodes.ChildNodes.Cast<XmlNode>())
                                        .Where(node2 => (node2.Name == "ContaCorrente"))
                //.Where(node2 => ((node2.InnerText.Contains(" 339030") || (node2.InnerText.Contains(" 449052")))))
                                        .Select(node2 => node2.InnerText.BreakLineForEmpenho())
                                        .ToList<string>();
            //.ForEach(codigoEmpenho => listaEmpenhos.Add(codigoEmpenho));

            return listaEmpenhos;
        }

        public bool ExcluirPTResMensal()
        {
            this.Service<IPTResMensalService>().Entity = this.PTResM;
            if (this.Consistido)
            {
                try
                {
                    this.Service<IPTResMensalService>().Excluir();
                }
                catch (Exception ex)
                {
                    new LogErro().GravarLogErro(ex);
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        public void ConsistirPTResM()
        {
            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<PTResMensalEntity>(ref this.pTResM);

            if (!this.PTRes.Codigo.HasValue)
                this.ListaErro.Add("É obrigatório informar o Código.");

            if (string.IsNullOrEmpty(this.PTRes.Descricao))
                this.ListaErro.Add("É obrigatório informar a Descrição.");

            if (this.ListaErro.Count == 0)
            {
                if (this.Service<IPTResService>().ExisteCodigoInformado())
                    this.ListaErro.Add("Código já existente.");
            }
        }
        //public PTResMensalEntity ObterPTResMensal(int ptresID)
        //{
        //    return this.Service<IPTResMensalService>().ObterPTResM(ptresID);
        //}

        #endregion
    }
}
