using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Sam.Domain.Entity;
using System.Collections;
using Sam.ServiceInfraestructure;
using Sam.Common.Util;
using System.Data.SqlClient;
using System.Net;
using System.Linq;
using System.Transactions;
using Sam.Integracao.SIAF.Core;


namespace Sam.Domain.Business
{
    public partial class EstruturaOrganizacionalBusiness : BaseBusiness
    {
        #region Fornecedor

        private FornecedorEntity fornecedor = new FornecedorEntity();
        public FornecedorEntity Fornecedor
        {
            get { return fornecedor; }
            set { fornecedor = value; }
        }

        public bool SalvarFornecedor()
        {
            this.Service<IFornecedorService>().Entity = this.Fornecedor;
            this.ConsistirFornecedor();

            if (this.Consistido)
            {
                this.Service<IFornecedorService>().Salvar();
            }

            return this.Consistido;
        }

        public void ConsistirFornecedor()
        {
            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<FornecedorEntity>(ref this.fornecedor);

            if (this.Service<IFornecedorService>().ExisteCodigoInformado())
            {
                this.ListaErro.Add("CPF/CNPJ já cadastrado.");
                return;
            }

            if (string.IsNullOrEmpty(this.Fornecedor.CpfCnpj))
                this.ListaErro.Add("É obrigatório informar o CPF/CNPJ.");

            if (string.IsNullOrEmpty(this.Fornecedor.Nome))
                ListaErro.Add("É obrigatório informar o Nome.");

            //if (string.IsNullOrEmpty(this.Fornecedor.Logradouro))
            //    ListaErro.Add("É obrigatório informar o Logradouro.");

            //if (string.IsNullOrEmpty(this.Fornecedor.Numero))
            //    ListaErro.Add("É obrigatório informar o Número.");

            //if (string.IsNullOrEmpty(this.Fornecedor.Bairro))
            //    ListaErro.Add("É obrigatório informar o Bairro.");

            //if (string.IsNullOrEmpty(this.Fornecedor.Cidade))
            //    ListaErro.Add("É obrigatório informar a Cidade.");

            //if (!this.Fornecedor.Uf.Id.HasValue)
            //    ListaErro.Add("É obrigatório informar a UF.");

            if (!string.IsNullOrEmpty(this.fornecedor.Email))
            {
                if (!TratamentoDados.ValidarEmail(this.Fornecedor.Email))
                    ListaErro.Add("E-mail inválido.");
            }

            if (this.Fornecedor.CpfCnpj.Length.Equals(11))
            {
                if (!TratamentoDados.ValidarCPF(this.Fornecedor.CpfCnpj))
                    ListaErro.Add("CPF inválido.");
            }
            else if (this.Fornecedor.CpfCnpj.Length.Equals(14))
            {
                if (!TratamentoDados.ValidarCNPJ(this.Fornecedor.CpfCnpj))
                    ListaErro.Add("CNPJ inválido.");
            }
        }

        public IList<FornecedorEntity> ListarFornecedor()
        {
            this.Service<IFornecedorService>().SkipRegistros = this.SkipRegistros;
            IList<FornecedorEntity> retorno = this.Service<IFornecedorService>().Listar();
            this.TotalRegistros = this.Service<IFornecedorService>().TotalRegistros();
            return retorno;
        }

        public IList<FornecedorEntity> ListarFornecedorPorPalavraChave(string _chave)
        {
            IList<FornecedorEntity> retorno = null;

            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    this.Service<IFornecedorService>().SkipRegistros = this.SkipRegistros;
                    //IList<FornecedorEntity> retorno = this.Service<IFornecedorService>().ListarFornecedorPorPalavraChave(_chave);
                    //IList<FornecedorEntity> retorno = this.Service<IFornecedorService>().ListarFornecedorPorPalavraChave(_chave, true);
                    retorno = this.Service<IFornecedorService>().ListarFornecedorPorPalavraChave(_chave, true);
                    this.TotalRegistros = this.Service<IFornecedorService>().TotalRegistros();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                //finally
                //{
                //    tras.Complete();
                //}
            }

            return retorno;
        }

        public IList<FornecedorEntity> ListarFornecedorTodosCod() 
        {
            return this.Service<IFornecedorService>().ListarTodosCod();
        }

        public IList<FornecedorEntity> ListarFornecedorComEmpenhosPendentes(int almoxID, string anoMesRef)
        {
            IList<FornecedorEntity> lstRetorno = null;

            try
            {
                lstRetorno = this.Service<IFornecedorService>().ListarFornecedorComEmpenhosPendentes(almoxID, anoMesRef);
                this.TotalRegistros = this.Service<IFornecedorService>().TotalRegistros();
            }
            catch (Exception excErroConsulta)
            {
                Exception excErroParaPropagacao = new Exception(String.Format("Erro ao retornar lista de fornecedores com empenhos pendentes, MesRef: '{0}', almoxID: '{1}'.", anoMesRef, almoxID), excErroConsulta);

                this.ListaErro = new List<string>(){ excErroParaPropagacao.Message };
                throw excErroParaPropagacao;
            }

            return lstRetorno;
        }

        public IList<FornecedorEntity> ImprimirFornecedor()
        {
            IList<FornecedorEntity> retorno = this.Service<IFornecedorService>().Imprimir();
            return retorno;
        }

        public bool ExcluirFornecedor()
        {
            this.Service<IFornecedorService>().Entity = this.Fornecedor;
            if (this.Consistido)
            {
                try
                {
                    this.Service<IFornecedorService>().Excluir();
                }
                catch (Exception ex)
                {
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        public FornecedorEntity RecuperarCadastroFornecedorDoSiafisico(long lngCnpjCpfUge, int iUgeID)
        {
            EstruturaOrganizacionalBusiness lObjOrganizacao = new EstruturaOrganizacionalBusiness();
            FornecedorEntity                lObjFornecedor  = new FornecedorEntity();

            string lStrUsuario        = string.Empty;
            string lStrSenha          = string.Empty;
            string lStrAnoBase        = string.Empty;
            string lStrUnidadeGestora = string.Empty;
            string lStrMsgEstimulo    = string.Empty;
            string lStrRetornoWs      = string.Empty;
            string lStrErroTratado    = string.Empty;

            string lStrXmlPatternConsulta = "/MSG/SISERRO/Doc_Retorno/SFCODOC/SiafisicoDocConsultaF/documento/";

            lStrAnoBase = DateTime.Now.ToString("yyyy");
            lStrUnidadeGestora = this.Service<IUGEService>().Listar(uge => uge.Id == iUgeID).FirstOrDefault().Codigo.ToString();

            lStrUsuario = Siafem.userNameConsulta;
            lStrSenha   = Siafem.passConsulta;


            try
            {
                //lStrMsgEstimulo = Siafem.wsSFCOConsultaF(pStrCnpjCpf);
                //lStrRetornoWs   = Siafem.recebeMsg(lStrUsuario, lStrSenha, lStrAnoBase, lStrUnidadeGestora, lStrMsgEstimulo, false);
                //lStrErroTratado = Siafem.trataErro(lStrRetornoWs);

                //if (!String.IsNullOrEmpty(lStrErroTratado))
                //{
                //    this.ListaErro.Add(lStrErroTratado);
                //    return null;
                //}
                string strNomeMensagem = null;

                //lStrMsgEstimulo = Siafem.SiafisicoDocConsultaF(lngCnpjCpfUge);
                lStrMsgEstimulo = GeradorEstimuloSIAF.SiafisicoDocConsultaF(lngCnpjCpfUge);
                lStrRetornoWs = Siafem.recebeMsg(lStrUsuario, lStrSenha, lStrAnoBase, lStrUnidadeGestora, lStrMsgEstimulo, false);
                if(Siafem.VerificarErroMensagem(lStrRetornoWs, out strNomeMensagem, out lStrErroTratado))
                {
                    //this.ListaErro.Add(lStrErroTratado);
                    lStrErroTratado.BreakLine(Environment.NewLine.ToCharArray()).ToList().ForEach(linhaErro => this.ListaErro.Add(linhaErro));
                    return null;
                }
                else
                {
                    string lStrStatusOperacao = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "StatusOperacao"));
                    string lStrCgcCpf         = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "CgcCpf"));
                    string lStrRazaoSocial    = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "RazaoSocial"));
                    string lStrEndereco       = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "Endereco"));
                    string lStrCidade         = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "Cidade"));
                    string lStrUF             = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "UF"));
                    string lStrCEP            = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "CEP"));
                    string lStrDDD1           = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "DDD1"));
                    string lStrDDD2           = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "DDD2"));
                    string lStrFone1          = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "Fone1"));
                    string lStrFone2          = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "Fone2"));
                    string lStrRamal1         = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "Ramal1"));
                    string lStrRamal2         = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "Ramal2"));
                    string lStrFax            = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "Fax"));
                    string lStrMsgRetorno     = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "MsgRetorno"));
                    
                    string lStrNumeroEndereco      = null;
                    string lStrComplementoEndereco = null;
                    

                    lStrDDD1 = (lStrDDD1.Length == 3) ? lStrDDD1 = lStrDDD1.Substring(1, 2) : lStrDDD1 = lStrDDD1.Substring(0, 2);
                    //lStrDDD2 = (lStrDDD2.Length == 3) ? lStrDDD2 = lStrDDD2.Substring(1, 2) : lStrDDD2 = lStrDDD2.Substring(0, 2);

                    string lStrTelefone1Formatado = string.Format("{0} {1}", lStrDDD1, lStrFone1);
                    //string lStrTelefone2Formatado = string.Format("{0} {1}", lStrDDD2, lStrFone2);

                    lStrComplementoEndereco   = lStrEndereco.BreakLine(',', 1).Trim();
                    lStrNumeroEndereco        = lStrComplementoEndereco.BreakLine('-',0).Trim();
                    lStrComplementoEndereco   = lStrComplementoEndereco.Replace(lStrNumeroEndereco, "").Replace("-", "").Trim();
                    lStrEndereco              = lStrEndereco.Replace(lStrNumeroEndereco,"").Replace(lStrComplementoEndereco,"").Replace(",","").Replace("-","").Trim();

                    lObjFornecedor.Nome       = lStrRazaoSocial;
                    lObjFornecedor.CpfCnpj    = lStrCgcCpf;
                    lObjFornecedor.Cidade     = lStrCidade;
                    lObjFornecedor.Cep        = lStrCEP;
                    lObjFornecedor.Telefone   = lStrTelefone1Formatado;
                    lObjFornecedor.Fax        = lStrFax;
                    lObjFornecedor.Logradouro = lStrEndereco;
                    
                    lObjFornecedor.Uf         = lObjOrganizacao.ListarUF().Where(UF => UF.Sigla == lStrUF).FirstOrDefault();
                }

            }
            catch (Exception excErroConsulta)
            {
                Exception excErroParaPropagacao = new Exception("Erro ao processar novo cadastro de fornecedor (integração CADFOR/SIAFISICO).", excErroConsulta);
                throw excErroParaPropagacao;
            }

            return lObjFornecedor;
        }

        public string ObterCpfCnpjFornecedor(int pIntFornecedorId)
        {
            FornecedorEntity lObjFornecedor = null;
            string           lStrRetorno    = String.Empty;

            lObjFornecedor = this.Service<IFornecedorService>().LerRegistro(pIntFornecedorId);

            if (lObjFornecedor != null)
                lStrRetorno = lObjFornecedor.CpfCnpj;

            return lStrRetorno;
        }

        #endregion

    }
}
