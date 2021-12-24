using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Sam.Domain.Entity;
using System.Collections;
using Sam.ServiceInfraestructure;
using Sam.Common.Util;
using System.Data.SqlClient;

namespace Sam.Domain.Business
{
    public partial class EstruturaOrganizacionalBusiness : BaseBusiness
    {       
        #region Terceiro

        private TerceiroEntity terceiro = new TerceiroEntity();

        public TerceiroEntity Terceiro
        {
            get { return terceiro; }
            set { terceiro = value; }
        }

        public bool SalvarTerceiro()
        {
            this.Service<ITerceiroService>().Entity = this.Terceiro;
            this.ConsistirTerceiro();
            if (this.Consistido)
            {
                this.Service<ITerceiroService>().Salvar();
            }
            return this.Consistido;
        }

        public IList<TerceiroEntity> ListarTerceiro(int OrgaoId, int GestorId)
        {
            this.Service<ITerceiroService>().SkipRegistros = this.SkipRegistros;
            IList<TerceiroEntity> retorno = this.Service<ITerceiroService>().Listar(OrgaoId,GestorId);
            this.TotalRegistros = this.Service<ITerceiroService>().TotalRegistros();
            return retorno;
        }

        public IList<TerceiroEntity> ListarTerceiro()
        {
            this.Service<ITerceiroService>().SkipRegistros = this.SkipRegistros;
            IList<TerceiroEntity> retorno = this.Service<ITerceiroService>().Listar();
            this.TotalRegistros = this.Service<ITerceiroService>().TotalRegistros();
            return retorno;
        }

        public IList<TerceiroEntity> ImprimirTerceiro()
        {
            IList<TerceiroEntity> retorno = this.Service<ITerceiroService>().Imprimir();
            return retorno;
        }

        public IList<TerceiroEntity> ImprimirTerceiro(int OrgaoId, int GestorId)
        {
            IList<TerceiroEntity> retorno = this.Service<ITerceiroService>().Imprimir(OrgaoId, GestorId);
            return retorno;
        }

        public bool ExcluirTerceiro()
        {
            this.Service<ITerceiroService>().Entity = this.Terceiro;
            if (this.Consistido)
            {
                try
                {
                    this.Service<ITerceiroService>().Excluir();
                }
                catch (Exception ex)
                {
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        public void ConsistirTerceiro()
        {
            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<TerceiroEntity>(ref this.terceiro);

            if (!this.Terceiro.Orgao.Id.HasValue)
                this.ListaErro.Add("É obrigatório informar o Órgão.");
            
            if (!this.Terceiro.Gestor.Id.HasValue)
                this.ListaErro.Add("É obrigatório informar o Gestor.");

            if (string.IsNullOrEmpty(this.Terceiro.Cnpj))
                this.ListaErro.Add("É obrigatório informar o CNPJ.");
            else if (!TratamentoDados.ValidarCNPJ( TratamentoDados.RetirarMascara(this.Terceiro.Cnpj)))
            {
                this.ListaErro.Add("CNPJ Inválido.");
            }
            
            if (string.IsNullOrEmpty(this.Terceiro.Nome))
                this.ListaErro.Add("É obrigatório informar o Nome.");

            if (string.IsNullOrEmpty(this.Terceiro.Logradouro))
                this.ListaErro.Add("É obrigatório informar o Logradouro.");

            if (string.IsNullOrEmpty(this.Terceiro.Numero))
                this.ListaErro.Add("É obrigatório informar o Número.");

            if (string.IsNullOrEmpty(this.Terceiro.Bairro))
                this.ListaErro.Add("É obrigatório informar o Bairro.");

            if (string.IsNullOrEmpty(this.Terceiro.Cidade))
                this.ListaErro.Add("É obrigatório informar o Município.");

            if (!this.Terceiro.Uf.Id.HasValue)
                this.ListaErro.Add("É obrigatório informar a UF.");

            if (string.IsNullOrEmpty(this.Terceiro.Cep))
                this.ListaErro.Add("É obrigatório informar o CEP.");

            if (this.ListaErro.Count == 0)
            {
                if (this.Service<ITerceiroService>().ExisteCodigoInformado())
                    this.ListaErro.Add("Código já existente.");
            }
        }

        #endregion
    }
}
