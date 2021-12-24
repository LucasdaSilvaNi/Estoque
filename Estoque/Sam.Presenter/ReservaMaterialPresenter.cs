using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.View;
using Sam.Domain.Entity;
using Sam.Domain.Business;
using Sam.Common.Util;

namespace Sam.Presenter
{
    public class ReservaMaterialPresenter : CrudPresenter<IReservaMaterialView> 
    {
 
        IReservaMaterialView view;

        public IReservaMaterialView View
        {
            get { return view; }
            set { view = value; }
        }

        public ReservaMaterialPresenter()
        {
        }

        public ReservaMaterialPresenter(IReservaMaterialView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public IList<GestorEntity> PopularListaGestor(int _orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<GestorEntity> retorno = estrutura.ListarGestor(_orgaoId);
            return retorno;
        }


        public IList<ReservaMaterialEntity> PopularDadosGrid(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            int almoxarifado = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id ?? 0;
            ReservaMaterialBusiness business = new ReservaMaterialBusiness();
            business.SkipRegistros = startRowIndexParameterName;
            IList<ReservaMaterialEntity> retorno = business.Listar(almoxarifado);
            this.TotalRegistrosGrid = business.TotalRegistros;
            return retorno;
        }

        public IList<UGEEntity> PopularListaUge(int subItem)
        {
            int almoxarifado = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value;
            int ugeId =  Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge.Id.Value;
            SaldoSubItemBusiness saldoSubItemBusiness = new SaldoSubItemBusiness();
            return saldoSubItemBusiness.ConsultarUgesBySubItemAlmox(almoxarifado, subItem, ugeId);
       
        }

        public IList<ReservaMaterialEntity> PopularDadosRelatorio(int almoxarifado)
        {
            ReservaMaterialBusiness business = new ReservaMaterialBusiness();
            IList<ReservaMaterialEntity> retorno = business.Imprimir(almoxarifado);
            return retorno;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            return this.TotalRegistrosGrid;
        }

       public override void Load()
        {
            this.View.PopularGrid();
            base.Load();
        }

        public void LerRegistro()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.Orgao.Id = TratamentoDados.TryParseInt32(this.View.Id);
            estrutura.LerOrgao();

            this.View.Id = estrutura.Orgao.Id.ToString();
            this.View.Codigo = estrutura.Orgao.Codigo.ToString();
            this.View.Descricao = estrutura.Orgao.Descricao;
        }

        public void Novo()
        {
            base.Novo();
            this.View.ItemMaterialId = string.Empty;
            this.View.SubItemMaterialId = string.Empty;
            this.View.UgeId = string.Empty;
            this.View.Obs = string.Empty;
            this.View.Quantidade = string.Empty;
        }

        public void Gravar()
        {
            ReservaMaterialBusiness reservaBusiness = new ReservaMaterialBusiness();
            reservaBusiness.Entity.Id = TratamentoDados.TryParseInt32(this.View.Id);
            List<string> erro = new List<string>();

            if(!string.IsNullOrEmpty(this.View.SubItemMaterialId))
                reservaBusiness.Entity.SubItemMaterial = new SubItemMaterialEntity { Id = Convert.ToInt32(this.View.SubItemMaterialId)};
            else
                erro.Add("É necessário infomar o subitem.");                
            
            reservaBusiness.Entity.Almoxarifado = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado;

            if (!string.IsNullOrEmpty(this.View.UgeId))
            
                reservaBusiness.Entity.Uge = new UGEEntity { Id = Convert.ToInt32(this.View.UgeId) };
            
            else
            
                erro.Add("É necessário selecionar a UGE.");

            if (!string.IsNullOrEmpty(this.View.Quantidade))            
                reservaBusiness.Entity.Quantidade = Convert.ToInt32(this.View.Quantidade);            
            else
                erro.Add("É necessário informar a quantidade.");    
            
            reservaBusiness.Entity.Obs = this.View.Obs;

            if (erro.Count > 0)
            {
                this.View.ListaErros = erro;
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
            }
            else
            {
                if (reservaBusiness.Salvar())
                {
                    this.View.PopularGrid();
                    this.GravadoSucesso();
                    this.View.ExibirMensagem("Registro salvo com sucesso!");
                }
                else
                    this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                this.View.ListaErros = reservaBusiness.ListaErro;
            }

        }

        public decimal CalcularSaldoReservaPorAlmoxSubMaterial(int almoxId, long subItemMaterialCodigo, DateTime dataInicial, DateTime dataFinal)
        {
            ReservaMaterialBusiness estrutura = new ReservaMaterialBusiness();
            DateTime[] periodo = new DateTime[] {dataInicial, dataFinal};
            return estrutura.ListarReservaPorPeriodoAlmoxSubItem(almoxId, subItemMaterialCodigo, periodo).Sum(a => a.Quantidade.Value);
        }


        public void Excluir()
        {
            ReservaMaterialBusiness reservaBusiness = new ReservaMaterialBusiness();
            reservaBusiness.Entity.Id = TratamentoDados.TryParseInt32(this.View.Id);
            if (reservaBusiness.Excluir())
            {
                this.View.PopularGrid();
                this.ExcluidoSucesso();
                this.View.ExibirMensagem("Registro excluído com sucesso!");
            }
            else
                this.View.ExibirMensagem("Não foi possível excluir, verifique as mensagens!");
            this.View.ListaErros = reservaBusiness.ListaErro;
        }

        public void Imprimir()
        {
            //RelatorioEntity.Id = (int)RelatorioEnum.Orgao;
            //RelatorioEntity.Nome = "rptOrgao.rdlc";
            //RelatorioEntity.DataSet = "dsOrgao";

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.Orgao;
            relatorioImpressao.Nome = "rptOrgao.rdlc";
            relatorioImpressao.DataSet = "dsOrgao";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

    }
}
