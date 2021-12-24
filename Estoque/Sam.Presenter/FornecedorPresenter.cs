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
    public class FornecedorPresenter : CrudPresenter<IFornecedorView>
    {
        IFornecedorView view;

        public IFornecedorView View
        {
            get { return view; }
            set { view = value; }
        }

        public FornecedorPresenter()
        {
        }

        public FornecedorPresenter(IFornecedorView _view):base(_view)
        {
            this.View = _view;
        }

        public IList<FornecedorEntity> PopularDadosFornecedor(int startRowIndexParameterName,
                int maximumRowsParameterName )
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<FornecedorEntity> retorno = estrutura.ListarFornecedor();
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public IList<FornecedorEntity> PopularDadosFornecedorTodosCod() 
        { 
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            return estrutura.ListarFornecedorTodosCod();
        }

        public IList<FornecedorEntity> ListarFornecedorComEmpenhosPendentes(int almoxID, string anoMesRef)
        {
            IList<FornecedorEntity> lstRetorno = null;
            EstruturaOrganizacionalBusiness objBusiness = new EstruturaOrganizacionalBusiness();

            try
            {
                lstRetorno = objBusiness.ListarFornecedorComEmpenhosPendentes(almoxID, anoMesRef);
                
                if(this.View.IsNotNull())
                this.View.ListaErros = objBusiness.ListaErro;
            }
            catch (Exception excErroConsulta)
            {
                Exception excErroParaPropagacao = new Exception(String.Format("Erro ao retornar lista de fornecedores com empenhos pendentes, MesRef: '{0}', almoxID: '{1}'.", anoMesRef, almoxID), excErroConsulta);

                if (this.View.IsNotNull())
                    this.View.ListaErros = new List<string>() { (excErroParaPropagacao.InnerException.IsNotNull() ? excErroParaPropagacao.InnerException.Message : excErroParaPropagacao.Message) };

                throw excErroParaPropagacao;
            }

            return lstRetorno;
        }

        public IList<FornecedorEntity> PopularDadosRelatorio()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<FornecedorEntity> retorno = estrutura.ImprimirFornecedor();
            return retorno;
        }
        
        public IList<UFEntity> PopularListaUf()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<UFEntity> retorno = estrutura.ListarUF();
            return retorno;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName )
        {
            return this.TotalRegistrosGrid;
        }
        public override void Load()
        {
            base.Load();
            this.View.BloqueiaEnderecoLogradouro = false;
            this.View.BloqueiaEnderecoNumero = false;
            this.View.BloqueiaEnderecoComplemento = false;
            this.View.BloqueiaEnderecoBairro = false;
            this.View.BloqueiaEnderecoCep = false;
            this.View.BloqueiaEnderecoCidade = false;
            this.View.BloqueiaListaUF = false;
            this.View.BloqueiaEnderecoTelefone = false;
            this.View.BloqueiaEnderecoFax = false;
            this.View.BloqueiaEmail = false;
            this.View.BloqueiaInformacoesComplementares = false;

            this.View.PopularGrid();
            this.View.PopularListaUF();
        }

        public override void Novo()
        {
            this.View.UfId = " - Selecione - ";
            this.View.EnderecoLogradouro = string.Empty;
            this.View.EnderecoNumero = string.Empty;
            this.View.EnderecoComplemento = string.Empty;
            this.View.EnderecoBairro = string.Empty;
            this.View.EnderecoCep = string.Empty;
            this.View.EnderecoCidade = string.Empty;
            this.View.UfId = string.Empty;
            this.View.EnderecoTelefone = string.Empty;
            this.View.EnderecoFax = string.Empty;
            this.View.Email = string.Empty;
            this.View.InformacoesComplementares = string.Empty;

            this.View.BloqueiaEnderecoLogradouro = true;
            this.View.BloqueiaEnderecoNumero = true;
            this.View.BloqueiaEnderecoComplemento = true;
            this.View.BloqueiaEnderecoBairro = true;
            this.View.BloqueiaEnderecoCep = true;
            this.View.BloqueiaEnderecoCidade = true;
            this.View.BloqueiaListaUF = true;
            this.View.BloqueiaEnderecoTelefone = true;
            this.View.BloqueiaEnderecoFax = true;
            this.View.BloqueiaEmail = true;
            this.View.BloqueiaInformacoesComplementares = true;
            base.Novo();
        }

        public override void GravadoSucesso()
        {
            this.View.UfId = " - Selecione - ";
            this.View.EnderecoLogradouro = string.Empty;
            this.View.EnderecoNumero = string.Empty;
            this.View.EnderecoComplemento = string.Empty;
            this.View.EnderecoBairro = string.Empty;
            this.View.EnderecoCep = string.Empty;
            this.View.EnderecoCidade = string.Empty;
            this.View.UfId = string.Empty;
            this.View.EnderecoTelefone = string.Empty;
            this.View.EnderecoFax = string.Empty;
            this.View.Email = string.Empty;
            this.View.InformacoesComplementares = string.Empty;

            this.View.BloqueiaEnderecoLogradouro = false;
            this.View.BloqueiaEnderecoNumero = false;
            this.View.BloqueiaInformacoesComplementares = false;
            this.View.BloqueiaEnderecoBairro = false;
            this.View.BloqueiaEnderecoCep = false;
            this.View.BloqueiaEnderecoCidade = false;
            this.View.BloqueiaListaUF = false;
            this.View.BloqueiaEnderecoTelefone = false;
            this.View.BloqueiaEnderecoFax = false;
            this.View.BloqueiaEmail = false;
            this.View.BloqueiaInformacoesComplementares = false;
            base.GravadoSucesso();
        }

        public IList<FornecedorEntity> PopularFornecedorPorPalavraChave(int startRowIndexParameterName,
                int maximumRowsParameterName, string _chave)
        {

            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();

            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<FornecedorEntity> retorno = estrutura.ListarFornecedorPorPalavraChave(_chave);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public IList<FornecedorEntity> PopularFornecedorPorPalavraChave(string _chave)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<FornecedorEntity> retorno = estrutura.ListarFornecedorPorPalavraChave(_chave);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }


        public override void ExcluidoSucesso()
        {
            this.View.UfId = " - Selecione - ";
            this.View.EnderecoLogradouro = string.Empty;
            this.View.EnderecoNumero = string.Empty;
            this.View.EnderecoComplemento = string.Empty;
            this.View.EnderecoBairro = string.Empty;
            this.View.EnderecoCep = string.Empty;
            this.View.EnderecoCidade = string.Empty;
            this.View.UfId = string.Empty;
            this.View.EnderecoTelefone = string.Empty;
            this.View.EnderecoFax = string.Empty;
            this.View.Email = string.Empty;
            this.View.InformacoesComplementares = string.Empty;

            this.View.BloqueiaEnderecoLogradouro = false;
            this.View.BloqueiaEnderecoNumero = false;
            this.View.BloqueiaInformacoesComplementares = false;
            this.View.BloqueiaEnderecoBairro = false;
            this.View.BloqueiaEnderecoCep = false;
            this.View.BloqueiaEnderecoCidade = false;
            this.View.BloqueiaListaUF = false;
            this.View.BloqueiaEnderecoTelefone = false;
            this.View.BloqueiaEnderecoFax = false;
            this.View.BloqueiaEmail = false;
            this.View.BloqueiaInformacoesComplementares = false;
            base.ExcluidoSucesso();
        }

        public override void Cancelar()
        {
            this.View.UfId = " - Selecione - ";
            
            this.View.EnderecoLogradouro = string.Empty;
            this.View.EnderecoNumero = string.Empty;
            this.View.EnderecoComplemento = string.Empty;
            this.View.EnderecoBairro = string.Empty;
            this.View.EnderecoCep = string.Empty;
            this.View.EnderecoCidade = string.Empty;
            this.View.UfId = string.Empty;
            this.View.EnderecoTelefone = string.Empty;
            this.View.EnderecoFax = string.Empty;
            this.View.Email = string.Empty;
            this.View.InformacoesComplementares = string.Empty;

            this.View.BloqueiaEnderecoLogradouro = false;
            this.View.BloqueiaEnderecoNumero = false;
            this.View.BloqueiaEnderecoComplemento = false;
            this.View.BloqueiaEnderecoBairro = false;
            this.View.BloqueiaEnderecoCep = false;
            this.View.BloqueiaEnderecoCidade = false;
            this.View.BloqueiaListaUF = false;
            this.View.BloqueiaEnderecoTelefone = false;
            this.View.BloqueiaEnderecoFax = false;
            this.View.BloqueiaEmail = false;
            this.View.BloqueiaInformacoesComplementares = false;
            base.Cancelar();
        }

        public override void RegistroSelecionado()
        {
            this.View.BloqueiaEnderecoLogradouro = true;
            this.View.BloqueiaEnderecoNumero = true;
            this.View.BloqueiaEnderecoComplemento = true;
            this.View.BloqueiaEnderecoBairro = true;
            this.View.BloqueiaEnderecoCep = true;
            this.View.BloqueiaEnderecoCidade = true;
            this.View.BloqueiaListaUF = true;
            this.View.BloqueiaEnderecoTelefone = true;
            this.View.BloqueiaEnderecoFax = true;
            this.View.BloqueiaEmail = true;
            this.View.BloqueiaInformacoesComplementares = true;
            base.RegistroSelecionado();
        }

        public void Gravar()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.Fornecedor.Id = TratamentoDados.TryParseInt32(this.view.Id);
            estrutura.Fornecedor.CpfCnpj = TratamentoDados.RetirarMascara(this.View.Codigo);
            estrutura.Fornecedor.Nome = this.View.Descricao;
            estrutura.Fornecedor.Logradouro = this.View.EnderecoLogradouro;
            estrutura.Fornecedor.Numero = this.View.EnderecoNumero;
            estrutura.Fornecedor.Complemento = this.View.EnderecoComplemento;
            estrutura.Fornecedor.Bairro = this.View.EnderecoBairro;
            estrutura.Fornecedor.Cep = TratamentoDados.RetirarMascara(this.View.EnderecoCep);
            estrutura.Fornecedor.Cidade = this.View.EnderecoCidade;
            estrutura.Fornecedor.Uf = new UFEntity(TratamentoDados.TryParseInt32(this.View.UfId));
            estrutura.Fornecedor.Telefone = TratamentoDados.RetirarMascara(this.View.EnderecoTelefone);
            estrutura.Fornecedor.Fax = TratamentoDados.RetirarMascara(this.View.EnderecoFax);
            estrutura.Fornecedor.Email = this.View.Email;
            estrutura.Fornecedor.InformacoesComplementares = this.View.InformacoesComplementares;

            if (estrutura.SalvarFornecedor())
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
            estrutura.Fornecedor.Id = TratamentoDados.TryParseInt32(this.View.Id);
            
            if (estrutura.ExcluirFornecedor())
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
            //RelatorioEntity.Id = (int)RelatorioEnum.Fornecedores;
            //RelatorioEntity.Nome = "rptFornecedor.rdlc";
            //RelatorioEntity.DataSet = "dsFornecedor";
            //RelatorioEntity.Parametros = this.View.ParametroRelatorio;

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.Fornecedores;
            relatorioImpressao.Nome = "rptFornecedor.rdlc";
            relatorioImpressao.DataSet = "dsFornecedor";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

        public string ObterCpfCnpjFornecedor(int pIntFornecedorId)
        {
            string lStrRetorno = string.Empty;

            EstruturaOrganizacionalBusiness lObjBusiness = new EstruturaOrganizacionalBusiness();
            lStrRetorno = lObjBusiness.ObterCpfCnpjFornecedor(pIntFornecedorId);

            return lStrRetorno;
        }
    }
}
