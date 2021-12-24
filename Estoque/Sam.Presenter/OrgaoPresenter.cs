using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.View;
using Sam.Domain.Business;
using Sam.Common.Util;
using Sam.Domain.Entity;
using System.Collections;
using Sam.Infrastructure;
using Sam.Business;

namespace Sam.Presenter
{
    public class OrgaoPresenter : CrudPresenter<IOrgaoView>
    {
        public OrgaoPresenter()
        {
        }
        public OrgaoPresenter(IOrgaoView _view)
            : base(_view)
        {
            this.View = _view;
        }
        public IList<OrgaoEntity> PopularDados(int startRowIndexParameterName, int maximumRowsParameterName)
        {
      
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<OrgaoEntity> retorno = estrutura.ListarOrgaos();
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }
        public IList<OrgaoEntity> PopularDadosTodosCod(int? orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<OrgaoEntity> retorno = estrutura.ListarOrgaosTodosCod(orgaoId);
            //retorno = base.FiltrarNivelAcesso(retorno);
            return retorno;
        }

        public IList<OrgaoEntity> PopularDadosTodosCod()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<OrgaoEntity> retorno = estrutura.ListarOrgaosTodosCod();
            //retorno = base.FiltrarNivelAcesso(retorno);
            return retorno;
        }

        //public IList<OrgaoEntity> PopularGetIdUsuarioOrgao(int? idOrgao)
        //{
        //    EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
        //    IList<OrgaoEntity> retorno = estrutura.ListarGetIdUsuarioOrgao(idOrgao);

        //   return retorno;
        //}

        public IList<OrgaoEntity> PopularOrgao(int? orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<OrgaoEntity> retorno = estrutura.ListarOrgaosTodosCod(orgaoId);
            retorno = base.FiltrarNivelAcesso(retorno);
            return retorno;
        }
        public IList<OrgaoEntity> PopularOrgao()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<OrgaoEntity> retorno = estrutura.ListarOrgaosTodosCod();
           retorno = base.FiltrarNivelAcesso(retorno);
            return retorno;
        }

        public IList<TB_ORGAO> ListarOrgao()
        {
            OrgaoBusiness business = new OrgaoBusiness();
            return business.ListarOrgao();
        }

        public IList<OrgaoEntity> PopularDadosRelatorio()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<OrgaoEntity> retorno = estrutura.ImprimirOrgaos();
            return retorno;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            return this.TotalRegistrosGrid;
        }

        public override void Load()
        {
            base.Load();
            this.View.PopularGrid();
            this.PopularDadosRelatorio();
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

        public OrgaoEntity LerRegistro(int orgaoId)
        {
            OrgaoBusiness orgaoBuss = new OrgaoBusiness();

            OrgaoEntity orgao = new OrgaoEntity();
            var objOrgaoBanco = orgaoBuss.RecuperaOrgao(orgaoId);

            orgao.Id = objOrgaoBanco.TB_ORGAO_ID;
            orgao.Codigo = objOrgaoBanco.TB_ORGAO_CODIGO;
            orgao.Descricao = objOrgaoBanco.TB_ORGAO_DESCRICAO;

            return orgao;
        }

        public void Gravar()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.Orgao.Codigo = TratamentoDados.TryParseInt32(this.View.Codigo);
            estrutura.Orgao.Descricao = this.View.Descricao;
            estrutura.Orgao.Id = TratamentoDados.TryParseInt32(this.View.Id);
            estrutura.Orgao.Ativo = Convert.ToBoolean(this.View.Ativo);
            estrutura.Orgao.Implantado = Convert.ToBoolean(this.View.Implantado);
            estrutura.Orgao.IntegracaoSIAFEM= Convert.ToBoolean(this.View.IntegracaoSIAFEM);

            if (estrutura.SalvarOrgao())
            {
                //this.View.PopularGrid();
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
            estrutura.Orgao.Id = TratamentoDados.TryParseInt32(this.View.Id);
            if (estrutura.ExcluirOrgao())
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
            //RelatorioEntity.Id = (int)RelatorioEnum.Orgao;
            //RelatorioEntity.Nome = "rptOrgao.rdlc";
            //RelatorioEntity.DataSet = "dsOrgao";
            //RelatorioEntity.Parametros = this.View.ParametrosRelatorio;

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.Orgao;
            relatorioImpressao.Nome = "rptOrgao.rdlc";
            relatorioImpressao.DataSet = "dsOrgao";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

        /// <summary>
        /// Método retorna lista de Orgaos cuja Gestao seja igual da Gestao informada.
        /// </summary>
        /// <param name="codigoGestao">Codigo da Gestao a ser consultado</param>
        /// <param name="gerarComCodigoDescricao">Se campo Descricao será devolvido no formato "CodigoOrgao - NomeOrgao"</param>
        /// <returns></returns>
        public IList<OrgaoEntity> ListarOrgaosPorGestao(int codigoGestao, bool gerarComCodigoDescricao = true)
        {
            IList<OrgaoEntity> lstOrgaosMesmaGestao = null;
            EstruturaOrganizacionalBusiness eoBusiness = null;

            eoBusiness = new EstruturaOrganizacionalBusiness();
            lstOrgaosMesmaGestao = eoBusiness.ListarOrgaosPorGestao(codigoGestao, gerarComCodigoDescricao);

            return lstOrgaosMesmaGestao;
        }
        public IList<OrgaoEntity> ListarOrgaosPorGestaoImplantado(int codigoGestao, bool gerarComCodigoDescricao = true)
        {
            IList<OrgaoEntity> lstOrgaosMesmaGestao = null;
            EstruturaOrganizacionalBusiness eoBusiness = null;

            eoBusiness = new EstruturaOrganizacionalBusiness();
            lstOrgaosMesmaGestao = eoBusiness.ListarOrgaosPorGestaoImplantado(codigoGestao, gerarComCodigoDescricao);

            return lstOrgaosMesmaGestao;
        }

        /// <summary>
        /// Método retorna lista de Orgaos cuja Gestao seja diferente da Gestao informada.
        /// </summary>
        /// <param name="codigoGestao">Codigo da Gestao a ser consultado</param>
        /// <param name="gerarComCodigoDescricao">Se campo Descricao será devolvido no formato "CodigoOrgao - NomeOrgao"</param>
        /// <returns></returns>
        public IList<OrgaoEntity> ListarOrgaosExcetoPorGestao(int codigoGestao, bool gerarComCodigoDescricao = true)
        {
            IList<OrgaoEntity> lstOrgaosMesmaGestao = null;
            EstruturaOrganizacionalBusiness eoBusiness = null;

            eoBusiness = new EstruturaOrganizacionalBusiness();
            lstOrgaosMesmaGestao = eoBusiness.ListarOrgaosExcetoPorGestao(codigoGestao, gerarComCodigoDescricao);

            return lstOrgaosMesmaGestao;
        }
        public IList<OrgaoEntity> ListarOrgaosExcetoPorGestaoImplantado(int codigoGestao, bool gerarComCodigoDescricao = true)
        {
            IList<OrgaoEntity> lstOrgaosMesmaGestao = null;
            EstruturaOrganizacionalBusiness eoBusiness = null;

            eoBusiness = new EstruturaOrganizacionalBusiness();
            lstOrgaosMesmaGestao = eoBusiness.ListarOrgaosExcetoPorGestaoImplantado(codigoGestao, gerarComCodigoDescricao);

            return lstOrgaosMesmaGestao;
        }
    }
}
