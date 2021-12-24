using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam;
using Sam.View;
using Sam.Domain.Entity;
using Sam.Domain.Business;
using Sam.Common.Util;
using System.ComponentModel;
using Sam.Business;
using Sam.Infrastructure;


namespace Sam.Presenter
{
    public class UGEPresenter : CrudPresenter<IUGEView>
    {
        IUGEView view;

        public IUGEView View
        {
            get { return view; }
            set { view = value; }
        }

        public UGEPresenter()
        {

        }

        public UGEPresenter(IUGEView _view)
            : base(_view)
        {
            this.View = _view;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<UGEEntity> PopularDadosUGE(int startRowIndexParameterName, int maximumRowsParameterName, int _uoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<UGEEntity> retorno = estrutura.ListarUGE(_uoId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<UGEEntity> ListarUGESaldoTodosCod(int subItemId, int almoxarifadoId)
        {
            almoxarifadoId = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id ?? 0;
            var ugeId = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge.Id;

            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<UGEEntity> retorno = estrutura.ListarUGESaldoTodosCod(subItemId, almoxarifadoId, ugeId.Value);
            return retorno;
        }

        public IList<UGEEntity> PopularDadosRelatorio(int _orgaoId, int _uoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<UGEEntity> retorno = estrutura.ImprimirUGE(_orgaoId, _uoId);
            return retorno;
        }

        public IList<UGEEntity> PopularDadosUGE()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<UGEEntity> retorno = estrutura.ListarUGE();
            return retorno;
        }

        public UGEEntity CarregarRegistroUGE(int _ugeId) 
        { 
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            return estrutura.ListarUgesTodosCod().Where(a => a.Id == _ugeId).FirstOrDefault();//Refazer
        }

        public IList<OrgaoEntity> PopularListaOrgao()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<OrgaoEntity> retorno = estrutura.ListarOrgaos();
            return retorno;
        }

        public IList<UOEntity> PopularListaUo(int _orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<UOEntity> retorno = estrutura.ListarUO(_orgaoId);
            return retorno;
        }

        public IList<TipoUGEEntity> PopularListaTipoUGE()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<TipoUGEEntity> retorno = estrutura.ListarTipoUGE();
            return retorno;
        }

        public IList<TB_UGE> ListarUGE(int UOId)
        {
            UgeBusiness business = new UgeBusiness();
            return business.ListarUGE(UOId);
        }

             
        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int _uoId)
        {
            return this.TotalRegistrosGrid;
        }

        public override void Load()
        {
            base.Load();
            this.View.PopularListaOrgao();
            this.View.PopularListaUo(int.MinValue);
            this.View.PopularGrid();

            this.View.BloqueiaListaTipoUge = false;
        }

        public IList<UGEEntity> PopularDadosTodosCod(int _orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<UGEEntity> retorno = estrutura.ListarUgesTodosCod(_orgaoId);
            return retorno;
        }
        public IList<UGEEntity> PopularDadosUgeCodigo(int _orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<UGEEntity> retorno = estrutura.ListarUgesComAlmoxarifado(_orgaoId);
            return retorno;
        }
        public IList<UGEEntity> PopularDadosTodosCodPorUo(int _uoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<UGEEntity> retorno = estrutura.ListarUgesTodosCodPorUo(_uoId);
            return retorno;
        }

        public IList<UGEEntity> PopularDadosTodosCodPorUo(int _uoId, bool filtrarPorAlmoxarifadoLogado)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();

            IList<UGEEntity> retorno = null;

            if (filtrarPorAlmoxarifadoLogado)
            {
                var divisaoList = estrutura.ListarDivisaoPorAlmoxTodosCod((int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id);

                if (divisaoList == null)
                    return retorno;
                else
                {
                    retorno = estrutura.ListarUgesTodosCodPorUo(_uoId, divisaoList);
                }
            }
            else
            {
                retorno = estrutura.ListarUgesTodosCodPorUo(_uoId);
            }

            return retorno;
        }

        public IList<UGEEntity> PopularDadosTodosCodPorGestor(int _gestorId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            return estrutura.ListarUgesTodosCodPorGestor(_gestorId);
        }

        public IList<UGEEntity> PopularDadosComSaldo(int? _gestorId, int? _almoxId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            return estrutura.ListarUGESaldoSubItemComSaldo(_gestorId.Value, _almoxId.Value);
        }

        public IList<UGEEntity> PopularUGEsComSaldoParaSubItem(long? _subItemCodigo, int? _almoxId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            return estrutura.ListarUGEsComSaldoParaSubitem(_subItemCodigo.Value, _almoxId.Value);
        }

        //# Requisitante Geral.
        public IList<UGEEntity> ListarUGEsByUoId(int uoid)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            return estrutura.ListarUGE(uoid);
        }

        public IList<UGEEntity> ListarUGEById(int ugeId)
        {
            return new UgeBusiness().SelectById(ugeId);
        }

        public void Gravar()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.UGE.Id = TratamentoDados.TryParseInt32(this.view.Id);
            estrutura.UGE.Codigo = TratamentoDados.TryParseInt32(this.View.Codigo);
            estrutura.UGE.Descricao = this.View.Descricao;
            estrutura.UGE.Orgao = new OrgaoEntity(TratamentoDados.TryParseInt32(this.View.OrgaoId));
            estrutura.UGE.TipoUGE = this.View.UgeTipoId;
            estrutura.UGE.Uo = new UOEntity(TratamentoDados.TryParseInt32(this.View.UoId));
            estrutura.UGE.Ativo = Convert.ToBoolean(this.view.UgeAtivo);
            estrutura.UGE.IntegracaoSIAFEM = Convert.ToBoolean(this.view.UgeIntegracaoSIAFEM);
            estrutura.UGE.Implantado= Convert.ToBoolean(this.view.UgeImplantado);

            if (estrutura.SalvarUGE())
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
            estrutura.UGE.Id = TratamentoDados.TryParseInt32(this.View.Id);
            if (estrutura.ExcluirUGE())
            {
                this.View.PopularGrid();
                this.ExcluidoSucesso();
                this.View.ExibirMensagem("Registro excluído com sucesso!");
            }
            else
                this.View.ExibirMensagem("Não foi possível excluir, verifique as mensagens!");
            this.View.ListaErros = estrutura.ListaErro;
        }

        public void Imprimir()
        {          
            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.UGE;
            relatorioImpressao.Nome = "rptUGE.rdlc";
            relatorioImpressao.DataSet = "dsUGE";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

        public override void Novo()
        {
            this.View.BloqueiaListaTipoUge = true;
            base.Novo();
        }

        public override void GravadoSucesso()
        {
            this.View.BloqueiaListaTipoUge = false;
            base.GravadoSucesso();
        }

        public override void ExcluidoSucesso()
        {
            this.View.BloqueiaListaTipoUge = false;
            base.ExcluidoSucesso();
        }

        public override void Cancelar()
        {
            this.View.BloqueiaListaTipoUge = false;
            base.Cancelar();
        }

        public override void RegistroSelecionado()
        {
            this.View.BloqueiaListaTipoUge = true;
            base.RegistroSelecionado();
        }
    }
}
