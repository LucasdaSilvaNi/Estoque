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

        #region Responsáveis

        private ResponsavelEntity responsavel = new ResponsavelEntity();

        public ResponsavelEntity Responsavel
        {
            get { return responsavel; }
            set { responsavel = value; }
        }

        public bool SalvarResponsavel()
        {
            this.Service<IResponsavelService>().Entity = this.Responsavel;
            this.ConsistirResponsavel();
            if (this.Consistido)
            {
                this.Service<IResponsavelService>().Salvar();
            }
            return this.Consistido;
        }

        public IList<ResponsavelEntity> ListarResponsavel(int GestorId)
        {
            this.Service<IResponsavelService>().SkipRegistros = this.SkipRegistros;
            IList<ResponsavelEntity> retorno = this.Service<IResponsavelService>().Listar(GestorId);
            this.TotalRegistros = this.Service<IResponsavelService>().TotalRegistros();
            return retorno;
        }

        public IList<ResponsavelEntity> ListarResponsavelPorUa(int UaId)
        {
            // procurar o código do gestor pela tabela UA
            IList<ResponsavelEntity> retorno = this.Service<IResponsavelService>().ListarTodosCodUa();
            return retorno;
        }


        public IList<GestorEntity> ListarResponsavelTodosCod(int orgaoId)
        {
            IList<GestorEntity> retorno = this.Service<IGestorService>().ListarTodosCod(orgaoId);
            return retorno;
        }

        public IList<ResponsavelEntity> ListarResponsavelTodosCodPorOrgao(int orgaoId)
        {
            IList<ResponsavelEntity> retorno = this.Service<IResponsavelService>().ListarTodosCodPorOrgao(orgaoId);
            return retorno;
        }

        public IList<ResponsavelEntity> ListarResponsavelPorOrgaoGestor(int orgaoId, int gestorId)
        {
            IList<ResponsavelEntity> retorno = this.Service<IResponsavelService>().ListarTodosPorOrgaoGestor(orgaoId, gestorId);
            return retorno;
        }

        public IList<ResponsavelEntity> ListarResponsavel()
        {
            this.Service<IResponsavelService>().SkipRegistros = this.SkipRegistros;
            IList<ResponsavelEntity> retorno = this.Service<IResponsavelService>().Listar();
            this.TotalRegistros = this.Service<IResponsavelService>().TotalRegistros();
            return retorno;
        }

        public IList<ResponsavelEntity> ImprimirResponsavel(int OrgaoId, int GestorId)
        {
            IList<ResponsavelEntity> retorno = this.Service<IResponsavelService>().Imprimir(OrgaoId, GestorId);
            return retorno;
        }

        public bool ExcluirResponsavel()
        {
            this.Service<IResponsavelService>().Entity = this.Responsavel;
            if (this.Consistido)
            {
                try
                {
                    this.Service<IResponsavelService>().Excluir();
                }
                catch (Exception ex)
                {
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        public void ConsistirResponsavel()
        {

            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<ResponsavelEntity>(ref this.responsavel);

            if (!this.Responsavel.Gestor.Id.HasValue)
                this.ListaErro.Add("É obrigatório informar o Gestor.");

            if (!this.Responsavel.Codigo.HasValue)
                this.ListaErro.Add("É obrigatório informar o Código.");
            
            if (this.Responsavel.Codigo==0)
                this.ListaErro.Add("Código do Responsável inválido!");

            if (string.IsNullOrEmpty(this.Responsavel.Descricao))
                this.ListaErro.Add("É obrigatório informar o Nome.");

            if (string.IsNullOrEmpty(this.Responsavel.Cargo))
                this.ListaErro.Add("É obrigatório informar o Cargo.");

            if (string.IsNullOrEmpty(this.Responsavel.Endereco))
                this.ListaErro.Add("É obrigatório informar o Endereço.");

            if (this.ListaErro.Count == 0)
            {
                if (this.Service<IResponsavelService>().ExisteCodigoInformado())
                    this.ListaErro.Add("Código já existente.");
            }
        }

        #endregion

    }
}
