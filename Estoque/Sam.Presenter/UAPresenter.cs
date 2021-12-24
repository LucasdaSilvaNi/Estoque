using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.View;
using Sam.Domain.Entity;
using Sam.Domain.Business;
using Sam.Common.Util;
using System.ComponentModel;
using Sam.Business;
using Sam.Common;

namespace Sam.Presenter
{
   //[DataObject(true)]
   public class UAPresenter : CrudPresenter<IUAView>
    { 
        IUAView view;

        public IUAView View
        {
            get { return view; }
            set { view = value; }
        }

        public UAPresenter()
        {
 
        }

        public UAPresenter(IUAView _view): base(_view)
        {
            this.View = _view;
        }

        public IList<OrgaoEntity> PopularListaOrgao()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<OrgaoEntity> retorno = estrutura.ListarOrgaos();
            return retorno;
        }
    
        public IList<UOEntity> PopularListaUO(int _orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<UOEntity> retorno = estrutura.ListarUO(_orgaoId);
            return retorno;
        }
        
        public IList<UGEEntity> PopularListaUGE(int? _uoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<UGEEntity> retorno = estrutura.ListarUGE(_uoId);
            return retorno;
        }

       // [DataObjectMethod(DataObjectMethodType.Select, true)] 
        public IList<UnidadeEntity> PopularListaUnidade(int? _orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<UnidadeEntity> retorno = estrutura.ListarUnidade(_orgaoId);
            retorno.Insert(0, new UnidadeEntity(null) { Descricao = "- Selecione -" });
            return retorno;
        }

       //[DataObjectMethod(DataObjectMethodType.Select, true)]  
       public IList<CentroCustoEntity> PopularListaCentroCusto(int _orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<CentroCustoEntity> retorno = estrutura.ListarCentroCusto(_orgaoId);
            retorno.Insert(0, new CentroCustoEntity(null) { Descricao = "- Selecione -" });
            return retorno;
        }

        //[DataObjectMethod(DataObjectMethodType.Select, true)] 
        public IList<IndicadorAtividadeEntity> PopularListaIndicadorAtividade()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<IndicadorAtividadeEntity> retorno = estrutura.ListarIndicadorAtividade();
            return retorno;
        }

        //[DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<UAEntity> PopularDados(int startRowIndexParameterName, int maximumRowsParameterName, int _ugeId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<UAEntity> retorno = estrutura.ListarUA(_ugeId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public IList<UAEntity> PopularDados(int startRowIndexParameterName, int maximumRowsParameterName, int _ugeId, string _uaCodigo)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.SkipRegistros = startRowIndexParameterName;

            int _codigoUa;
            int.TryParse(_uaCodigo, out _codigoUa);

            IList<UAEntity> retorno = estrutura.ListarUA(_ugeId, _codigoUa);

            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public IList<UAEntity> PopularDadosTodosCod(int? _orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<UAEntity> retorno = estrutura.ListarUasTodosCodPorOrgao(_orgaoId);
            return retorno;
        }

        public IList<UAEntity> PopularDadosTodosCodPorUo(int? _uoId) 
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            return estrutura.ListarUasTodosCodPorUo(_uoId);
        }

        /// <summary>
        /// Listar Todas as UA's ATIVAS filtrado por Uge
        /// </summary>
        /// <param name="UgeId">Identificador da UGE</param>
        /// <returns></returns>
        public IList<UAEntity> ListarUasTodosCodAtivoPorUge(int? _ugeId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            return estrutura.ListarUasTodosCodAtivoPorUge(_ugeId);
        }

        public IList<UAEntity> PopularDadosTodosCodPorUge(int? _ugeId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            return estrutura.ListarUasTodosCodPorUge(_ugeId);
        }

        public IList<UAEntity> PopularDadosTodosCodPorUge(int? _ugeId, bool filtrarPorAlmoxarifadoLogado)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();

            IList<UAEntity> retorno = null;

            if (filtrarPorAlmoxarifadoLogado)
            {
                var divisaoList = estrutura.ListarDivisaoPorAlmoxTodosCod((int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id);

                if (divisaoList == null)
                    return retorno;
                else
                {
                    retorno = estrutura.ListarUasTodosCodPorUge(_ugeId, divisaoList);
                }
            }
            else
            {
                retorno = estrutura.ListarUasTodosCodPorUge(_ugeId);
            }

            return retorno;
        }

        public IList<UAEntity> PopularDados()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<UAEntity> retorno = estrutura.ListarUA();
            return retorno;
        }

        public IList<UAEntity> PopularDadosTodas()
       {
           EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
           IList<UAEntity> retorno = estrutura.ListarTodasUA();
           return retorno;
       }

       public IList<UAEntity> PopularDadosRelatorio(int _ugeId)
       {
           EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
           IList<UAEntity> retorno = estrutura.ImprimirUA(_ugeId);
           return retorno;
       }
              
       public IList<UAEntity> ListarUaByUge(int ugeId)
       {
           EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
           IList<UAEntity> retorno = estrutura.ListarUA(ugeId);
           return retorno;
       }

       public IList<Sam.Domain.Entity.UAEntity> ListarUaById(int uaId)
       {
           Sam.Business.UaBusiness uaBusiness = new Sam.Business.UaBusiness();
           return uaBusiness.SelectOne(uaId);
       }      

       //[DataObjectMethod(DataObjectMethodType.Select, true)]
       public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int _ugeId, string _uaCodigo)
        {
            return this.TotalRegistrosGrid;
        }

        public override void Load()
        {
            base.Load();

            this.View.PopularListaOrgao();
            this.View.BloqueiaListaUnidade = false;
            this.View.BloqueiaUAVinculada = false;
            this.View.BloqueiaListaCentroCusto = false;
            this.View.BloqueiaListaIndicadorAtividade = false;
            this.View.BloqueiaNovo = false;
        }

        public override void Novo()
        {
            base.Novo();
            this.View.BloqueiaListaUnidade = true;
            this.view.BloqueiaCodigo = true;
            this.View.UAVinculada = string.Empty;
            this.View.BloqueiaUAVinculada = true;
            this.View.BloqueiaListaCentroCusto = true;
            this.View.BloqueiaListaIndicadorAtividade = true;
            this.View.BloqueiaNovo = false;
        }

        public override void Cancelar()
        {
            base.Cancelar();
            this.View.BloqueiaListaUnidade = false;
            this.View.BloqueiaUAVinculada = false;
            this.View.BloqueiaListaCentroCusto = false;
            this.View.BloqueiaListaIndicadorAtividade = false;
            //this.view.BloqueiaNovo = true;
        }

        public override void RegistroSelecionado()
        {
            base.RegistroSelecionado();
            this.view.BloqueiaCodigo = false;
            this.View.BloqueiaListaUnidade = true;
            this.View.BloqueiaUAVinculada = true;
            this.View.BloqueiaListaCentroCusto = true;
            this.View.BloqueiaListaIndicadorAtividade = true;
        }

        public override void GravadoSucesso()
        {
            base.GravadoSucesso();
            this.View.BloqueiaListaUnidade = false;
            this.View.UAVinculada = string.Empty;
            this.View.BloqueiaUAVinculada = false;
            this.View.BloqueiaListaCentroCusto = false;
            this.View.BloqueiaListaIndicadorAtividade = false;
        }

        public override void ExcluidoSucesso()
        {
            base.ExcluidoSucesso();
            this.View.BloqueiaListaUnidade = false;
            this.View.UAVinculada = string.Empty;
            this.View.BloqueiaUAVinculada = false;
            this.View.BloqueiaListaCentroCusto = false;
            this.View.BloqueiaListaIndicadorAtividade = false;
        }

        public void Gravar()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.Ua.Id = TratamentoDados.TryParseInt32(this.View.Id);
            estrutura.Ua.Orgao =(new OrgaoEntity(TratamentoDados.TryParseInt32(this.View.OrgaoId)));
            estrutura.Ua.Uge = (new UGEEntity(TratamentoDados.TryParseInt32(this.View.UgeId)));
            estrutura.Ua.Codigo = TratamentoDados.TryParseInt32(this.View.Codigo);
            estrutura.Ua.Unidade = (new UnidadeEntity(TratamentoDados.TryParseInt32(this.View.UnidadeId)));
            estrutura.Ua.Gestor = new GestorEntity(TratamentoDados.TryParseInt32(this.View.GestorId));
            estrutura.Ua.Descricao = this.View.Descricao;
            estrutura.Ua.UaVinculada = TratamentoDados.TryParseInt32(this.View.UAVinculada);
            estrutura.Ua.CentroCusto = (new CentroCustoEntity(TratamentoDados.TryParseInt32(this.View.CentroCustoId)));
            estrutura.Ua.IndicadorAtividade = Convert.ToBoolean(this.View.IndicadorAtividadeId);

            if (estrutura.SalvarUA())
            {
                this.View.PopularGrid();
                this.GravadoSucesso();
                this.View.ExibirMensagem("Registro salvo com sucesso!");
            }
            else
            {
                this.View.ExibirMensagem("Incosistências encontradas, verifique as mensagens!");
            }
            
            this.View.ListaErros = estrutura.ListaErro;
        }

        public void Excluir()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            EstruturaBusiness estruturaPerfil = new EstruturaBusiness();

            estrutura.Ua.Id= TratamentoDados.TryParseInt32(this.View.Id);

            if (estruturaPerfil.ConsultaEstruturaNivel(Convert.ToInt32(estrutura.Ua.Id), (int)Enuns.NivelAcessoEnum.UA) == 0)
            {
                if (estrutura.ExcluirUA())
                {
                    this.View.PopularGrid();
                    this.ExcluidoSucesso();
                    this.View.ExibirMensagem("Registro excluÃ­do com sucesso!");
                }
                else
                    estrutura.ListaErro.Add("Há registro associado a esta UA.");
            }
            else
                estrutura.ListaErro.Add("Há nivel associado a esta UA.");

            if (estrutura.ListaErro.Count > 0)
            {
                this.View.ExibirMensagem("Não foi possível excluir, verifique as mensagens.");
                this.View.ListaErros = estrutura.ListaErro;
            }            

        }

        public void Imprimir()
        {
            //RelatorioEntity.Id = 3;
            //RelatorioEntity.Nome = "rptUA.rdlc";
            //RelatorioEntity.DataSet = "dsUA";

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.UA;
            relatorioImpressao.Nome = "rptUA.rdlc";
            relatorioImpressao.DataSet = "dsUA";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

        public UAEntity ObterUA(int uaID)
        {
            EstruturaOrganizacionalBusiness objBusiness = new EstruturaOrganizacionalBusiness();
            UAEntity objEntidade = objBusiness.ObterUA(uaID);

            return objEntidade;
        }

        public UAEntity ObterUAPorCodigo(int uaCodigo, int gestorId)
        {
            EstruturaOrganizacionalBusiness objBusiness = new EstruturaOrganizacionalBusiness();
            UAEntity objEntidade = objBusiness.ObterUAPorCodigo(uaCodigo, gestorId);

            return objEntidade;
        }
    }
}
