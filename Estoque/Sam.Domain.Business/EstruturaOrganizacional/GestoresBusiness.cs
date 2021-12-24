using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Sam.Domain.Entity;
using System.Collections;
using Sam.ServiceInfraestructure;
using Sam.Common.Util;
using System.Data.SqlClient;
using System.Linq;

namespace Sam.Domain.Business
{
    public partial class EstruturaOrganizacionalBusiness : BaseBusiness
    {
        #region Gestores

        private GestorEntity gestor = new GestorEntity();

        public GestorEntity Gestor
        {
            get { return gestor; }
            set { gestor = value; }
        }

        public bool SalvarGestor()
        {
            this.ConsistirGestor();
            this.Service<IGestorService>().Entity = this.Gestor;

            if (this.Consistido)
            {
                this.Service<IGestorService>().Salvar();
            }

            return this.Consistido;
        }

        public bool ExcluirGestor()
        {
            this.Service<IGestorService>().Entity = this.Gestor;
            if (this.Consistido)
            {
                try
                {
                    this.Service<IGestorService>().Excluir();
                }
                catch (Exception ex)
                {
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

         public IList<GestorEntity> ListarGestor(int orgaoId)
        {
            this.Service<IGestorService>().SkipRegistros = this.SkipRegistros;
            IList<GestorEntity> retorno = this.Service<IGestorService>().Listar(orgaoId);
            this.TotalRegistros = this.Service<IGestorService>().TotalRegistros();

            return retorno;
        }

         public IList<GestorEntity> ListarGestor()
         {
             IList<GestorEntity> retorno = this.Service<IGestorService>().Listar();

             return retorno;
         }

        public IList<GestorEntity> ListarGestorTodosCod(int orgaoId)
        {            
            IList<GestorEntity> retorno = this.Service<IGestorService>().ListarTodosCod(orgaoId);
            return retorno;
        }

        public IList<GestorEntity> ListarGestorTodosCod(int orgaoId, int gestorId)
        {
            IList<GestorEntity> retorno = this.Service<IGestorService>().ListarTodosCod(orgaoId);
            return retorno;
        }

        public IList<GestorEntity> ListarGestorTodosCod()
        {
            return this.Service<IGestorService>().ListarTodosCod();
        }

        public IList<UnidadeEntity> ListarUnidadesTodosCod(int orgaoId)
        {
            IList<UnidadeEntity> retorno = this.Service<IUnidadeService>().ListarTodosCod(orgaoId);
            return retorno;
        }

        public IList<CentroCustoEntity> ListarCentroCustoTodosCodPorOrgao(int orgaoId)
        {
            IList<CentroCustoEntity> retorno = this.Service<ICentroCustoService>().ListarTodosCodPorOrgao(orgaoId);
            return retorno;
        }
        

        public IList<GestorEntity> ImprimirGestor(int orgaoId)
        {
            IList<GestorEntity> retorno = this.Service<IGestorService>().Imprimir(orgaoId);
            return retorno;
        }

        public GestorEntity SelecionarGestor(int GestorId)
        {
            GestorEntity gestor = this.Service<IGestorService>().Selecionar(GestorId);
            return gestor;
        }

        public int RetornaGestorOrganizacional(int? orgaoId, int? uoId, int? ugeId)
        {
            int gestorId = this.Service<IGestorService>().RetornaGestorOrganizacional(orgaoId, uoId, ugeId);
            return gestorId;
        }

        public void ConsistirGestor()
        {

            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<GestorEntity>(ref this.gestor);

            if (string.IsNullOrEmpty(this.Gestor.Nome.ToString()))
                this.ListaErro.Add("É obrigatório informar o Nome.");

            if (string.IsNullOrEmpty(this.Gestor.NomeReduzido.ToString()))
                this.ListaErro.Add("É obrigatório informar o Nome reduzido.");

            if (string.IsNullOrEmpty(this.Gestor.EnderecoLogradouro.ToString()))
                this.ListaErro.Add("É obrigatório informar o Logradouro.");

            if (string.IsNullOrEmpty(this.Gestor.EnderecoNumero.ToString()))
                this.ListaErro.Add("É obrigatório informar o Número.");

            if (string.IsNullOrEmpty(this.Gestor.EnderecoTelefone.ToString()))
                this.ListaErro.Add("É obrigatório informar o Telefone.");

            if (!this.Gestor.CodigoGestao.HasValue)
                this.ListaErro.Add("É obrigatório informar o Código de Gestão.");

            // pesquisa por nome do gestor
            this.SkipRegistros = 0;
            IList<GestorEntity> gestorE = this.Service<IGestorService>().Listar();

            if (!Gestor.Id.HasValue)
            {

                if (gestorE.Where(a => a.Nome.ToLower() == this.Gestor.Nome.ToLower()).FirstOrDefault() != null)
                    ListaErro.Add("Nome do gestor já existe.");

                //if (gestorE.Where(a => a.Orgao.Id == this.Gestor.Orgao.Id).FirstOrDefault() != null)
                //    ListaErro.Add("Órgão gestor já cadastrado.");
            }
            else
            {
                if (gestorE.Where(a => a.Id == this.Gestor.Id).FirstOrDefault() == null)
                {

                    if (gestorE.Where(a => a.Nome.ToLower() == this.Gestor.Nome.ToLower()).FirstOrDefault() != null)
                        ListaErro.Add("Nome do gestor já existe.");

                    if (gestorE.Where(a => a.Orgao.Id == this.Gestor.Orgao.Id).FirstOrDefault() != null)
                        ListaErro.Add("Órgão gestor já cadastrado.");
                }
            }

            // atualiza tabela de UA
            //IList<UAEntity> lstUa = new List<UAEntity>();

            //if(this.Gestor.Orgao != null && this.Gestor.Orgao.Id.HasValue)
            //    lstUa = this.Service<IUAService>().ListarUasTodosCod(this.Gestor.Orgao.Id.Value);
            //if (this.Gestor.Uo != null && this.Gestor.Uo.Id.HasValue)
            //    lstUa.Union(this.Service<IUAService>().ListarUasTodosCodPorUo(this.Gestor.Uo.Id.Value));
            //if (this.Gestor.Uge != null && this.Gestor.Uge.Id.HasValue)
            //    lstUa.Union(this.Service<IUAService>().ListarUasTodosCodPorUge(this.Gestor.Uge.Id.Value));
            
            //if(lstUa != null)
            //{
            //    foreach (UAEntity ua in lstUa) 
            //    {
            //        if (this.Gestor.Id != null)
            //            ua.Gestor = new GestorEntity(this.Gestor.Id.Value);
            //        else
            //            ua.Gestor = new GestorEntity();

            //        this.Service<IUAService>().Entity = ua;
            //        this.Service<IUAService>().Salvar();
            //    }
            //}

        }

        #endregion       
    }
}
