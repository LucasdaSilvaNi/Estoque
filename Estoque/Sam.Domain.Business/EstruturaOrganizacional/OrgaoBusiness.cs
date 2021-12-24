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

        #region Órgão

        private OrgaoEntity orgao = new OrgaoEntity();

        public OrgaoEntity Orgao
        {
            get { return orgao; }
            set { orgao = value; }
        }

        public bool SalvarOrgao()
        {
            this.Service<IOrgaoService>().Entity = this.Orgao;
            this.ConsistirOrgao();
            if (this.Consistido)
            {
                this.Service<IOrgaoService>().Salvar();
            }
            return this.Consistido;
        }

        public IList<OrgaoEntity> ListarOrgaos()
        {
            this.Service<IOrgaoService>().SkipRegistros = this.SkipRegistros;
            IList<OrgaoEntity> retorno = this.Service<IOrgaoService>().Listar();
            this.TotalRegistros = this.Service<IOrgaoService>().TotalRegistros();
            return retorno;
        }

        public IList<UOEntity> ListarUOs(int OrgaoId)
        {
            IList<UOEntity> retorno = this.Service<IUOService>().ListarTodosCod(OrgaoId);
            return retorno;
        }

        public IList<OrgaoEntity> ListarOrgaosTodosCod(int? orgaoId)
        {
            IList<OrgaoEntity> retorno = this.Service<IOrgaoService>().ListarTodosCod(orgaoId);
            return retorno;
        }
        public IList<OrgaoEntity> ListarOrgaosTodosCod()
        {
            IList<OrgaoEntity> retorno = this.Service<IOrgaoService>().ListarTodosCod();
            return retorno;
        }
        //public IList<UsuarioEntity> ListarGetIdUsuarioOrgao(int? idOrgao)
        //{
        //    IList<UsuarioEntity> retorno = this.Service<UsuarioBusiness>().ListarGetIdUsuarioOrgao(idOrgao);
        //    return retorno;
        //}

        public IList<UOEntity> ListarUosTodosCod(int OrgaoId)
        {
            IList<UOEntity> retorno = this.Service<IUOService>().ListarTodosCod(OrgaoId);
            return retorno;
        }

        public IList<UOEntity> ListarUosTodosCod(int OrgaoId, IList<DivisaoEntity> divisaoList)
        {
            IList<UOEntity> retorno = this.Service<IUOService>().ListarTodosCod(OrgaoId, divisaoList);
            return retorno;
        }


        public IList<UOEntity> ListarUosTodosCod()
        {
            return this.Service<IUOService>().ListarTodosCod();
        }

        public IList<UGEEntity> ListarUgesTodosCod(int OrgaoId)
        {
            IList<UGEEntity> retorno = this.Service<IUGEService>().ListarUgesTodosCod(OrgaoId);
            return retorno;
        }

        public IList<UGEEntity> ListarUgesComAlmoxarifado(int OrgaoId)
        {
            IList<UGEEntity> retorno = this.Service<IUGEService>().ListarUgeComAlmoxarifado(OrgaoId);
            return retorno;
        }
        public IList<UGEEntity> ListarUgesTodosCodPorUo(int UoId)
        {
            IList<UGEEntity> retorno = this.Service<IUGEService>().ListarUgesTodosCodPorUo(UoId);
            return retorno;
        }

        public IList<UGEEntity> ListarUgesTodosCodPorUo(int UoId, IList<DivisaoEntity> divisaoList)
        {
            IList<UGEEntity> retorno = this.Service<IUGEService>().ListarUgesTodosCodPorUo(UoId, divisaoList);
            return retorno;
        }

        public IList<UGEEntity> ListarUgesTodosCodPorGestor(int GestorId)
        {
            return this.Service<IUGEService>().ListarTodosCodPorGestor(GestorId);
        }

        public OrgaoEntity LerOrgao()
        {
            this.Service<IOrgaoService>().SkipRegistros = this.SkipRegistros;
            OrgaoEntity retorno = this.Service<IOrgaoService>().LerRegistro();

            return retorno;
        }

        public IList<OrgaoEntity> ImprimirOrgaos()
        {
            IList<OrgaoEntity> retorno = this.Service<IOrgaoService>().Imprimir();
            return retorno;
        }

        public bool ExcluirOrgao()
        {
            this.Service<IOrgaoService>().Entity = this.Orgao;
            if (this.Consistido)
            {
                try
                {
                    this.Service<IOrgaoService>().Excluir();
                }
                catch (Exception ex)
                {
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        public void ConsistirOrgao()
        {

            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<OrgaoEntity>(ref this.orgao);

            if (!this.Orgao.Codigo.HasValue)
            {
                this.ListaErro.Add("É obrigatório informar o Código.");
            }
            else
            {
                if (this.Orgao.Codigo == 0)
                    this.ListaErro.Add("Código do órgão inválido.");
            }

            if (string.IsNullOrEmpty(this.Orgao.Descricao))
                this.ListaErro.Add("É obrigatório informar a Descrição.");


            if (this.ListaErro.Count == 0)
            {
                if (this.Service<IOrgaoService>().ExisteCodigoInformado())
                    this.ListaErro.Add("Código já existente.");
            }

            //Listar UA(s) Ativas Associadas a UGE          
            if (this.Orgao.Ativo == false)
            {
                IList<OrgaoEntity> _listar = this.ListarUoPorOrgao(this.Orgao.Codigo);
                if (_listar != null && _listar.Count > 0)
                {
                    this.ListaErro.Add("Existe(m) UO(s) ativa(s) associada(s) a esse Órgão.");
                }
            }
        }
        public IList<OrgaoEntity> ListarUoPorOrgao(int? orgaoId)
        {
            return this.Service<IOrgaoService>().ListarUoPorOrgao(orgaoId);
        }

        public IList<OrgaoEntity> ListarOrgaosPorGestao(int codigoGestao, bool gerarComCodigoDescricao = true)
        {
            IList<OrgaoEntity> lstOrgaos = null;
            bool excluirOrgaosGestaoDoRetorno = false;

            lstOrgaos = this.Service<IOrgaoService>().ListarOrgaosPorGestao(codigoGestao, excluirOrgaosGestaoDoRetorno, gerarComCodigoDescricao);

            return lstOrgaos;
        }
        public IList<OrgaoEntity> ListarOrgaosPorGestaoImplantado(int codigoGestao, bool gerarComCodigoDescricao = true)
        {
            IList<OrgaoEntity> lstOrgaos = null;
            bool excluirOrgaosGestaoDoRetorno = false;

            lstOrgaos = this.Service<IOrgaoService>().ListarOrgaosPorGestaoImplantado(codigoGestao, excluirOrgaosGestaoDoRetorno, gerarComCodigoDescricao);

            return lstOrgaos;
        }
        public IList<OrgaoEntity> ListarOrgaosExcetoPorGestao(int codigoGestao, bool gerarComCodigoDescricao = true)
        {
            IList<OrgaoEntity> lstOrgaos = null;
            bool excluirOrgaosGestaoDoRetorno = true;

            lstOrgaos = this.Service<IOrgaoService>().ListarOrgaosPorGestao(codigoGestao, excluirOrgaosGestaoDoRetorno, gerarComCodigoDescricao);

            return lstOrgaos;
        }
        public IList<OrgaoEntity> ListarOrgaosExcetoPorGestaoImplantado(int codigoGestao, bool gerarComCodigoDescricao = true)
        {
            IList<OrgaoEntity> lstOrgaos = null;
            bool excluirOrgaosGestaoDoRetorno = true;

            lstOrgaos = this.Service<IOrgaoService>().ListarOrgaosPorGestaoImplantado(codigoGestao, excluirOrgaosGestaoDoRetorno, gerarComCodigoDescricao);

            return lstOrgaos;
        }
        public OrgaoEntity ListarCodigoOrgao(int idOrgao)
        {
            OrgaoEntity lstOrgaos = null;

            lstOrgaos = this.Service<IOrgaoService>().ListarCodigoOrgao(idOrgao);

            return lstOrgaos;
        }
        #endregion

    }
}
