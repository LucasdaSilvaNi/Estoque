using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.View;
using Sam.Domain.Entity;
using Sam.Common.Util;
using Sam.Domain.Business;
using System.ComponentModel;
using Sam.Infrastructure;
using Sam.Business;

namespace Sam.Presenter
{
    //[DataObject(true)]
    public class UOPresenter : CrudPresenter<IUOView>
    {
        IUOView view;

        public IUOView View
        {
            get { return view; }
            set { view = value; }
        }

        public UOPresenter()
        {
        }

        public UOPresenter(IUOView _view)
            : base(_view)
        {
            this.View = _view;
        }



       // [DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<UOEntity> PopularDadosUO(int startRowIndexParameterName, int maximumRowsParameterName, int _orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();

            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<UOEntity> retorno = estrutura.ListarUO(_orgaoId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public IList<UOEntity> PopularDadosRelatorio(int _orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<UOEntity> retorno = estrutura.ImprimirUO(_orgaoId);
            return retorno;
        }
       
      //  [DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<UOEntity> PopularDadosUO()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<UOEntity> retorno = estrutura.ListarUO();
            
            return retorno;
        }

        public IList<UOEntity> PopularDadosUO(int _orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();

            IList<UOEntity> retorno = estrutura.ListarUO(_orgaoId);

            return retorno;
        }

        public IList<UOEntity> PopularDadosTodosCod(int _orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<UOEntity> retorno = estrutura.ListarUosTodosCod(_orgaoId);

            return retorno;
        }

        public IList<UOEntity> PopularDadosTodosCod(int _orgaoId, bool filtrarPorAlmoxarifadoLogado)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            
            IList<UOEntity> retorno = null;

            if (filtrarPorAlmoxarifadoLogado)
            {
                var divisaoList = estrutura.ListarDivisaoPorAlmoxTodosCod((int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id);

                if (divisaoList == null)
                    return retorno;
                else
                {
                    retorno = estrutura.ListarUosTodosCod(_orgaoId, divisaoList);
                }
            }
            else
            {
                retorno = estrutura.ListarUosTodosCod(_orgaoId);
            }

            return retorno;
        }

       // [DataObjectMethod(DataObjectMethodType.Select)]
        public IList<OrgaoEntity> PopularListaOrgao()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<OrgaoEntity> retorno = estrutura.ListarOrgaos();
            return retorno;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public int TotalRegistros(int startRowIndexParameterName,int maximumRowsParameterName, int _orgaoId)
        {
            return this.TotalRegistrosGrid;
        }

        public override void Load()
        {
            base.Load();
            this.View.PopularListaOrgao();
            this.View.PopularGrid();
        }

        public void Gravar()
        {

            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.UO.Id = TratamentoDados.TryParseInt32(this.View.Id);

            int codigo, orgaoId;

            int.TryParse(this.View.Codigo, out codigo);
            int.TryParse(this.View.OrgaoId, out orgaoId);

            estrutura.UO.Codigo = codigo;
            estrutura.UO.Descricao = this.View.Descricao;
            estrutura.UO.Orgao = (new OrgaoEntity(orgaoId));
            estrutura.UO.Ativo = Convert.ToBoolean(this.view.Ativo);
            if (estrutura.SalvarUO())
            {
                this.View.PopularGrid();
                this.GravadoSucesso();
                this.View.ExibirMensagem("Registro salvo com sucesso!");
            }
            else
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
            this.View.ListaErros = estrutura.ListaErro;

        }

        public void Excluir()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.UO.Id = TratamentoDados.TryParseInt32(this.View.Id);
            if (estrutura.ExcluirUO())
            {
                this.View.PopularGrid();
                this.ExcluidoSucesso();
                this.View.ExibirMensagem("Registro excluído com sucesso!");
            }
            else
            {
                this.View.ExibirMensagem("Não foi possível excluir, verifique as mensagens!");
            }
            this.View.ListaErros = estrutura.ListaErro;
        }

        public void Imprimir()
        {
            //RelatorioEntity.Id = (int)RelatorioEnum.UO;
            //RelatorioEntity.Nome = "rptUO.rdlc";
            //RelatorioEntity.DataSet = "dsUO";

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.UO;
            relatorioImpressao.Nome = "rptUO.rdlc";
            relatorioImpressao.DataSet = "dsUO";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

        //TODO Eduardo Almeida: Altração de código Requisitante Geral.
        public IList<UOEntity> ListarUOByOrgao(int orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<UOEntity> retorno = estrutura.ListarUO(orgaoId);
            return retorno;
        }



        public IList<TB_UO> ListarUo_2(int OrgaoId)
        {
            UoBusiness business = new UoBusiness();
            return business.ListarOrgao(OrgaoId);
        }
        
    }
}
