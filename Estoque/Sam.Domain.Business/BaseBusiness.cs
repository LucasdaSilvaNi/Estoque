using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;
using Microsoft.Practices.Unity;
using System.Data.SqlClient;
using Sam.Common.Util;
using System.Transactions;
using System.Diagnostics;


namespace Sam.Domain.Business
{
    public abstract class BaseBusiness
    {

        public static decimal _valorZero = 0.0000m; 

        public int SkipRegistros { get; set; }

        public int TotalRegistros
        {
            get;
            set;
        }

        List<string> listaErro = new List<string>();
        public List<string> ListaErro
        {
            get { return listaErro; }
            set { listaErro = value; }
        }

        public bool Consistido
        {
            get
            {
                return this.ListaErro.Count == 0;
            }
        }

        IUnityContainer container;
        UnityConfigurationSection section;


        object service;
        [DebuggerStepThrough]
        public T Service<T>()
        {
            if (service == null)
                service = this.Create<T>();


            try
            {
                return (T)service;
            }
            catch
            {
                service = this.Create<T>();

                return (T)service;
            }
        
        }

        [DebuggerStepThrough]
        [InjectionMethod]
        public T Create<T>()
        {

            container = new UnityContainer();
            section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");

            section.Containers.Default.Configure(container);

            T bus = container.Resolve<T>();

            return (T)bus;

        }
        public void TratarErro(Exception message)
        {
            new LogErro().GravarLogErro(message);
            string mensagem = message.Message;
            if (message.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                mensagem = "Esse registro não pode ser excluido! Existem informações relacionadas.";
            this.ListaErro.Add(mensagem);
        }

        #region DualModeView
        public string fmtFracionarioMaterialQtde
        { get; set; }
        public string fmtFracionarioMaterialValorUnitario
        { get; private set; }
        public string fmtDataFormatoBrasileiro
        { get { return "dd/MM/yyyy"; } }
        public string fmtDataFormatoBrasileiroNumeral
        { get { return "ddMMyyyy"; } }
        public string fmtDataHoraFormatoBrasileiro
        { get { return "dd/MM/yyyy HH:mm:ss"; } }
        public int numCasasDecimaisMaterialQtde
        { get; private set; }
        public int numCasasDecimaisValorUnitario
        { get; private set; }
        public bool CarregarFormatos(string anoMesRef)
        {
            bool blnRetorno = false;

            try
            {
                var strFormatoQtdeMaterial = string.Empty;
                var strFormatoValorUnitario = string.Empty;
                var _anoMesRef = Int32.Parse(anoMesRef);

                if (_anoMesRef >= Constante.CST_ANO_MES_DATA_CORTE_SAP)
                {
                    strFormatoQtdeMaterial = "#,##0.000";
                    strFormatoValorUnitario = "#,##0.0000";
                    numCasasDecimaisMaterialQtde = 3;
                    numCasasDecimaisValorUnitario = 4;
                }
                else
                {
                    strFormatoQtdeMaterial = "#0";
                    strFormatoValorUnitario = "#,##0.00";
                    numCasasDecimaisMaterialQtde = 0;
                    numCasasDecimaisValorUnitario = 2;
                }

                this.fmtFracionarioMaterialQtde = strFormatoQtdeMaterial;
                this.fmtFracionarioMaterialValorUnitario = strFormatoValorUnitario;

                blnRetorno = true;
            }
            catch (Exception)
            { }

            return blnRetorno;
        }
        public bool FormatosCarregados()
        {
            return (fmtFracionarioMaterialQtde.IsNotNull() && fmtFracionarioMaterialValorUnitario.IsNotNull());
        }
        #endregion

        #region Funcoes-base para acesso concorrente
        public TransactionScope ObterConfiguracoesPadraoDeConsulta(bool leituraSuja = true)
        {
            TransactionScope transScope = null;

            if (leituraSuja)
                transScope = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted });
            else
                transScope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted });

            return transScope;
        }

        #endregion
    }
}
