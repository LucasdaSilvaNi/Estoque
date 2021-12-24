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
        #region PTRes

        private PTResEntity pTRes = new PTResEntity();

        public PTResEntity PTRes
        {
            get { return pTRes; }
            set { pTRes = value; }
        }

        public bool SalvarPTRes()
        {
            this.Service<IPTResService>().Entity = this.PTRes;
            this.ConsistirPTRes();
            if (this.Consistido)
            {
                this.Service<IPTResService>().Salvar();
            }
            return this.Consistido;
        }

        public IList<PTResEntity> ListarPTRes()
        {
            this.Service<IPTResService>().SkipRegistros = this.SkipRegistros;
            IList<PTResEntity> retorno = this.Service<IPTResService>().Listar();
            this.TotalRegistros = this.Service<IPTResService>().TotalRegistros();
            return retorno;
        }

        public IList<PTResEntity> ImprimirPTRes()
        {
            IList<PTResEntity> retorno = this.Service<IPTResService>().Imprimir();
            return retorno;
        }

        public string wsSIAFDetPTRES(string UG, string gestao, string mes, string diaMesInicial, string diaMesFinal)
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


        public IList<PTResEntity> CarregarListaPTRes(int ugeCodigo)
        {
            #region Variaveis
            IList<PTResEntity> lstPTRes = null;
            #endregion Variaveis

            try
            {
                using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    try
                    {
                       lstPTRes = this.Service<IPTResService>().Listar(ugeCodigo, true);
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

        public IList<PTResEntity> CarregarListaPTResAcao(int ugeCodigo)
        {
            #region Variaveis
            IList<PTResEntity> lstPTRes = null;
            #endregion Variaveis

            try
            {
                using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    try
                    {
                        lstPTRes = this.Service<IPTResService>().ObterPTResAcao(ugeCodigo);
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
        private IList<string> processarListaPTRes(XmlNodeList xmlListNodes)
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

        public bool ExcluirPTRes()
        {
            this.Service<IPTResService>().Entity = this.PTRes;
            if (this.Consistido)
            {
                try
                {
                    this.Service<IPTResService>().Excluir();
                }
                catch (Exception ex)
                {
                    new LogErro().GravarLogErro(ex);
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        public void ConsistirPTRes()
        {
            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<PTResEntity>(ref this.pTRes);

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
        public PTResEntity ObterPTRes(int ptresID)
        {
            return this.Service<IPTResService>().ObterPTRes(ptresID);
        }

        #endregion
    }
}
