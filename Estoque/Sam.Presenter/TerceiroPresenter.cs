using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.View;
using Sam.Domain.Entity;
using Sam.Common.Util;
using Sam.Domain.Business;
using System.Collections;
using System.ComponentModel;

namespace Sam.Presenter
{
    public class TerceiroPresenter : CrudPresenter<ITerceiroView>
    {
        ITerceiroView view;

        public ITerceiroView View
        {
            get { return view; }
            set { view = value; }
        }

        public TerceiroPresenter()
        {
        }

        public TerceiroPresenter(ITerceiroView _view) : base(_view)
        {
            this.View = _view;
        }

        public IList<TerceiroEntity> PopularDadosTerceiro(int startRowIndexParameterName,
                int maximumRowsParameterName, int _orgaoId, int _gestorId)
        {

            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();

            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<TerceiroEntity> retorno = estrutura.ListarTerceiro(_orgaoId, _gestorId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public IList<TerceiroEntity> PopularDadosRelatorio(int _orgaoId, int _gestorId)
        {

            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<TerceiroEntity> retorno = estrutura.ImprimirTerceiro(_orgaoId, _gestorId);
            return retorno;
        }

        public IList<OrgaoEntity> PopularListaOrgao()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<OrgaoEntity> retorno = estrutura.ListarOrgaos();
            return retorno;
        }

        public IList<GestorEntity> PopularListaGestor(int _orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<GestorEntity> retorno = estrutura.ListarGestor(_orgaoId);
            return retorno;
        }

        public IList<UFEntity> PopularListaUF()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<UFEntity> retorno = estrutura.ListarUF();
            return retorno;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int _orgaoId, int _gestorId)
        {
            return this.TotalRegistrosGrid;
        }
        public override void Load()
        {
            base.Load();
            this.View.PopularListaOrgao();
            this.View.PopularListaGestor(int.MinValue);
            this.View.PopularListaUF();
            this.View.PopularGrid();
        }

        public void Gravar()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.Terceiro.Id = TratamentoDados.TryParseInt32(this.view.Id);
            estrutura.Terceiro.Cnpj = TratamentoDados.RetirarMascara(this.View.Codigo);
            estrutura.Terceiro.Nome = this.View.Descricao;
            estrutura.Terceiro.Logradouro = this.View.EnderecoLogradouro;
            estrutura.Terceiro.Numero = this.View.EnderecoNumero;
            estrutura.Terceiro.Complemento = this.View.EnderecoComplemento;
            estrutura.Terceiro.Bairro = this.View.EnderecoBairro;
            estrutura.Terceiro.Cidade = this.View.EnderecoCidade;
            estrutura.Terceiro.Uf = (new UFEntity( TratamentoDados.TryParseInt32(this.View.UfId)));
            estrutura.Terceiro.Cep = TratamentoDados.RetirarMascara(this.View.EnderecoCep);
            estrutura.Terceiro.Telefone = TratamentoDados.RetirarMascara(this.View.EnderecoTelefone);
            estrutura.Terceiro.Fax = TratamentoDados.RetirarMascara(this.View.EnderecoFax);
            estrutura.Terceiro.Orgao = (new OrgaoEntity(TratamentoDados.TryParseInt32(this.View.OrgaoId)));
            estrutura.Terceiro.Gestor = (new GestorEntity(TratamentoDados.TryParseInt32(this.View.GestorId)));

            if (estrutura.SalvarTerceiro())
            {
                this.View.PopularGrid();
                this.GravadoSucesso();
                this.View.ExibirMensagem("Registro salvo com sucesso!");
            }
            else
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
            this.View.ListaErros = estrutura.ListaErro;

        }

        public void Imprimir()
        {
            //RelatorioEntity.Id = (int)RelatorioEnum.Terceiro;
            //RelatorioEntity.Nome = "rptTerceiro.rdlc";
            //RelatorioEntity.DataSet = "dsTerceiro";
            //RelatorioEntity.Parametros = this.View.ParametrosRelatorio;

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.Terceiro;
            relatorioImpressao.Nome = "rptTerceiro.rdlc";
            relatorioImpressao.DataSet = "dsTerceiro";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

        public void Excluir()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.Terceiro.Id = TratamentoDados.TryParseInt32(this.View.Id);
            if (estrutura.ExcluirTerceiro())
            {
                this.View.PopularGrid();
                this.ExcluidoSucesso();
                this.View.ExibirMensagem("Registro excluído com sucesso!");
            }
            else
                this.View.ExibirMensagem("Não foi possível excluir, verifique as mensagens!");
            this.View.ListaErros = estrutura.ListaErro;
        }

        public override void Novo()
        {
            
            this.View.UfId = " - Selecione - ";
            this.View.EnderecoLogradouro = string.Empty;
            this.View.BloqueiaEnderecoLogradouro = true;
            this.View.EnderecoNumero = string.Empty;
            this.View.BloqueiaEnderecoNumero = true;
            this.View.EnderecoComplemento = string.Empty;
            this.View.BloqueiaEnderecoComplemento = true;
            this.View.EnderecoBairro = string.Empty;
            this.View.BloqueiaEnderecoBairro = true;
            this.View.EnderecoCidade = string.Empty;
            this.View.BloqueiaEnderecoCidade = true;
            this.View.EnderecoCep = string.Empty;
            this.View.BloqueiaEnderecoCep = true;
            this.View.EnderecoTelefone = string.Empty;
            this.View.BloqueiaEnderecoTelefone = true;
            this.View.EnderecoFax = string.Empty;
            this.View.BloqueiaEnderecoFax = true;
            
            this.View.BloqueiaListaUF = true;
            base.Novo();
        }

        public override void GravadoSucesso()
        {
            this.View.UfId = " - Selecione - ";
          
            this.View.EnderecoLogradouro = string.Empty;
            this.View.BloqueiaEnderecoLogradouro = false;
            this.View.EnderecoNumero = string.Empty;
            this.View.BloqueiaEnderecoNumero = false;
            this.View.EnderecoComplemento = string.Empty;
            this.View.BloqueiaEnderecoComplemento = false;
            this.View.EnderecoBairro = string.Empty;
            this.View.BloqueiaEnderecoBairro = false;
            this.View.EnderecoCidade = string.Empty;
            this.View.BloqueiaEnderecoCidade = false;
            this.View.BloqueiaListaUF = false;
            this.View.EnderecoCep = string.Empty;
            this.View.BloqueiaEnderecoCep = false;
            this.View.EnderecoTelefone = string.Empty;
            this.View.BloqueiaEnderecoTelefone = false;
            this.View.EnderecoFax = string.Empty;
            this.View.BloqueiaEnderecoFax = false;
            this.View.BloqueiaListaUF = false;
            base.GravadoSucesso();
        }

        public override void ExcluidoSucesso()
        {
            this.View.UfId = " - Selecione - ";
            this.View.EnderecoLogradouro = string.Empty;
            this.View.BloqueiaEnderecoLogradouro = false;
            this.View.EnderecoNumero = string.Empty;
            this.View.BloqueiaEnderecoNumero = false;
            this.View.EnderecoComplemento = string.Empty;
            this.View.BloqueiaEnderecoComplemento = false;
            this.View.EnderecoBairro = string.Empty;
            this.View.BloqueiaEnderecoBairro = false;
            this.View.EnderecoCidade = string.Empty;
            this.View.BloqueiaEnderecoCidade = false;
            this.View.EnderecoCep = string.Empty;
            this.View.BloqueiaEnderecoCep = false;
            this.View.EnderecoTelefone = string.Empty;
            this.View.BloqueiaEnderecoTelefone = false;
            this.View.EnderecoFax = string.Empty;
            this.View.BloqueiaEnderecoFax = false;
           
            this.View.BloqueiaListaUF = false;
            base.ExcluidoSucesso();
        }

        public override void Cancelar()
        {
            this.View.EnderecoLogradouro = string.Empty;
            this.View.EnderecoNumero = string.Empty;
            this.View.EnderecoComplemento = string.Empty;
            this.View.EnderecoBairro = string.Empty;
            this.View.EnderecoCidade = string.Empty;
            this.View.EnderecoCep = string.Empty;
            this.View.EnderecoTelefone = string.Empty;
            this.View.EnderecoFax = string.Empty;
            
            this.View.UfId = " - Selecione - ";
           
            this.View.BloqueiaEnderecoLogradouro = false;
            this.View.BloqueiaEnderecoNumero = false;
            this.View.BloqueiaEnderecoComplemento = false;
            this.View.BloqueiaEnderecoBairro = false;
            this.View.BloqueiaEnderecoCidade = false;
            this.View.BloqueiaEnderecoComplemento = false;
            this.View.BloqueiaEnderecoCep = false;
            this.View.BloqueiaEnderecoTelefone = false;
            this.View.BloqueiaEnderecoFax = false;
            
            this.View.BloqueiaListaUF = false;
           
            base.Cancelar();
        }

        public override void RegistroSelecionado()
        {
            this.View.BloqueiaEnderecoLogradouro = true;
            this.View.BloqueiaEnderecoNumero = true;
            this.View.BloqueiaEnderecoComplemento = true;
            this.View.BloqueiaEnderecoBairro = true;
            this.View.BloqueiaEnderecoCidade = true;
            this.View.BloqueiaEnderecoComplemento = true;
            this.View.BloqueiaEnderecoCep = true;
            this.View.BloqueiaEnderecoTelefone = true;
            this.View.BloqueiaEnderecoFax = true;
            this.View.BloqueiaListaUF = true;
            base.RegistroSelecionado();
        }
    }
}
