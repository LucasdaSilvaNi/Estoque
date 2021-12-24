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
using System.Web;
using Sam.Infrastructure;
using Sam.Business;

namespace Sam.Presenter
{
    public class AlmoxarifadoPresenter : CrudPresenter<IAlmoxarifadoView> 
    {
        IAlmoxarifadoView view;

        public IAlmoxarifadoView View
        {
            get { return view; }
            set { view = value; }
        }

        public AlmoxarifadoPresenter()
        {
        }

        public AlmoxarifadoPresenter(IAlmoxarifadoView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public IList<AlmoxarifadoEntity> PopularDadosAlmoxarifado(int startRowIndexParameterName, int maximumRowsParameterName,
            int _orgaoId, int _gestorId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<AlmoxarifadoEntity> retorno = estrutura.ListarAlmoxarifado(_orgaoId, _gestorId);            
            this.TotalRegistrosGrid = estrutura.TotalRegistros;

            return retorno;
        }

        public IList<AlmoxarifadoEntity> PopularListaAmoxarifadoPerfil(int startRowIndexParameterName, int maximumRowsParameterName, int _orgaoId, int _gestorId, int _almoxarifadoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.SkipRegistros = startRowIndexParameterName;

            var idPerfil = Acesso.Transacoes.Perfis[0].IdPerfil;

            if (idPerfil == (int)Sam.Common.Perfil.ADMINISTRADOR_GERAL || idPerfil == (int)Sam.Common.Perfil.ADMINISTRADOR_ORGAO)
            {
                //Se for administrador geral ou de orgão, mostrar todos os almoxarifados cadatrados do gestor

                //10 primeiros almoxarifados do gestor
                var almoxarifados = estrutura.ListarSelecionaAlmoxarifadoTake(_orgaoId, _gestorId, _almoxarifadoId);
                this.TotalRegistrosGrid = estrutura.TotalRegistros;
                return almoxarifados.ToList();
            }
            else
            {
                //Se for diferente dos administradores, filtrar os almoxarifados do gestor pelo nivel de acesso do perfil

                string nivelAcessoDesc = "ALMOXARIFADO";//Campo Identity no banco
                var nivelAcesso = new Sam.Facade.FacadePerfil().ListarPerfilLoginNivelAcesso(Acesso.Transacoes.Perfis[0].IdLogin, nivelAcessoDesc);

                //Todos os almoxarifados do gestor
                var almoxarifados = estrutura.ListarSelecionaAlmoxarifado(_orgaoId, _gestorId, _almoxarifadoId);
                
                List<AlmoxarifadoEntity> almoxList = new List<AlmoxarifadoEntity>();
                foreach (var almox in almoxarifados)
                {
                    if (nivelAcesso.Where(a => a.AlmoxId == almox.Id).ToList().Count > 0)
                        almoxList.Add(almox);
                }

                //Retorna paginando os registros
                TotalRegistrosGrid = almoxList.Count();
                almoxList = almoxList.Skip(startRowIndexParameterName).Take(maximumRowsParameterName).ToList();
              //  almoxList = almoxList.Skip(startRowIndexParameterName).Take(10).ToList();
                

                return almoxList;
            }
        }

        public AlmoxarifadoEntity GetAlmoxarifadoByDivisao(int divisaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            AlmoxarifadoEntity retorno = estrutura.GetAlmoxarifadoByDivisao(divisaoId);
            return retorno;            
        }

        public IList<AlmoxarifadoEntity> PopularListalmoxarifado(int startRowIndexParameterName, 
               int maximumRowsParameterName, int orgaoId, int gestorId,int almoxarifadoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<AlmoxarifadoEntity> retorno = estrutura.ListarAlmoxarifadoLista(Acesso.Estruturas.Almoxarifado, orgaoId, gestorId, almoxarifadoId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public IList<AlmoxarifadoEntity> PopularDadosAlmoxarifado(int startRowIndexParameterName, int maximumRowsParameterName,
           int _orgaoId, int _gestorId, int _almoxarifadoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<AlmoxarifadoEntity> retorno = estrutura.ListarSelecionaAlmoxarifado(_orgaoId, _gestorId, _almoxarifadoId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;

            return retorno;
        }

        public IList<AlmoxarifadoEntity> AlmoxarifadoTodosCod()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<AlmoxarifadoEntity> retorno = estrutura.AlmoxarifadoTodosCod();
            return retorno;
        }
        public IList<AlmoxarifadoEntity> AlmoxarifadoTodosCod(int _OrgaoId, int _GestorId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<AlmoxarifadoEntity> retorno = estrutura.AlmoxarifadoTodosCod(_OrgaoId, _GestorId);
            return retorno;
        }
        public IList<AlmoxarifadoEntity> PopularDadosRelatorio(int _orgaoId, int _gestorId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            //IList<AlmoxarifadoEntity> retorno = estrutura.ImprimirAlmoxarifado(_orgaoId, _gestorId);
            IList<AlmoxarifadoEntity> retorno = estrutura.ImprmirAlmoxarifado(_gestorId);
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

        public IList<UGEEntity> PopularListaUge(int gestorId)
        {
            if (gestorId == 0)
            {
                if (Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado != null)
                {
                    gestorId = (int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id;
                }
            }

            if (gestorId != 0)
            {
                EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
                IList<UGEEntity> retorno = estrutura.ListarUgesTodosCodPorGestor(gestorId);
                return retorno;
            }
            else
            {
                return new List<UGEEntity>();
            }
            
        }

        public IList<AlmoxarifadoEntity> PopularListaAlmoxarifado(int _gestorId)
        {
            if (_gestorId == 0)
                _gestorId = (int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id;

            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<AlmoxarifadoEntity> retorno = estrutura.ListarAlmoxarifadoPorGestorTodosCod(_gestorId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public IList<AlmoxarifadoEntity> PopularListaAlmoxarifadoMenosAlmoxLogado(int _gestorId)
        {
            //if (_gestorId == 0)//Alterado para sempre pegar o gestor logado
            //_gestorId = (int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id;
            _gestorId = (int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id;

            int? almoxLogadoId = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id;

            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<AlmoxarifadoEntity> retorno = estrutura.ListarAlmoxarifadoPorGestorTodosCod(_gestorId);
            
            //Remove o almoxarifado logado
            if (almoxLogadoId != null)
                retorno = retorno.Where(a => a.Id != (int)almoxLogadoId).ToList<AlmoxarifadoEntity>();
            
            return retorno;
        }
        public IList<TB_ALMOXARIFADO> ListarAlmoxarifadoPorGestorMovimentoPendente(int GestorId)
        {
            AlmoxarifadoBusiness business = new AlmoxarifadoBusiness();
            return business.ListarAlmoxarifadoPorGestorMovimentoPendente(GestorId);
        }

        public IList<ResponsavelEntity> PopularListaResponsavel()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<ResponsavelEntity> retorno = estrutura.ListarResponsavel();
            return retorno;
        }

        public IList<UFEntity> PopularListaUf()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<UFEntity> retorno = estrutura.ListarUF();
            return retorno;
        }

        public IList<IndicadorAtividadeEntity> PopularListaIndicadorAtividade()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<IndicadorAtividadeEntity> retorno = estrutura.ListarIndicadorAtividade();
            return retorno;
        }

        public IList<OrgaoEntity> PopularListaOrgaoTodosCod()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<OrgaoEntity> lista = estrutura.ListarOrgaosTodosCod();
            lista = base.FiltrarNivelAcesso(lista);
            return lista;
        }

        public IList<GestorEntity> PopularListaGestorTodosCod(int _orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<GestorEntity> lista = estrutura.ListarGestorTodosCod(_orgaoId);
            lista = base.FiltrarNivelAcesso(lista);
            return lista;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int _orgaoId, int _gestorId, int _almoxarifadoId)
        {
            return this.TotalRegistrosGrid;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int _orgaoId, int _gestorId)
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
            this.View.BloqueiaEnderecoMunicipio = false;
            this.View.BloqueiaEnderecoCep = false;
            this.View.BloqueiaEnderecoTelefone = false;
            this.View.BloqueiaEnderecoFax = false;
            this.View.BloqueiaRefInicial = false;
            this.View.BloqueiaListaUge = false;
            this.View.BloqueiaListaUf = false;
            this.View.BloqueiaListaIndicadorAtividade = false;
            this.View.BloqueiaResponsavel = false;
           
            this.View.PopularListaOrgao();
            this.View.PopularListaGestor(int.MinValue);
            this.View.PopularListaUge();
            this.View.PopularListaUf();
            this.View.PopularListaIndicadorAtividade();
            this.View.PopularGrid();
        }

        public void Gravar()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.Almoxarifado.Orgao = new OrgaoEntity(TratamentoDados.TryParseInt32(this.View.OrgaoId));
            estrutura.Almoxarifado.Gestor = new GestorEntity(TratamentoDados.TryParseInt32(this.View.GestorId));
            estrutura.Almoxarifado.Id = TratamentoDados.TryParseInt32(this.View.Id);
            estrutura.Almoxarifado.Codigo = TratamentoDados.TryParseInt32(this.View.Codigo);
            estrutura.Almoxarifado.Descricao = this.View.Descricao;
            estrutura.Almoxarifado.EnderecoLogradouro = this.View.EnderecoLogradouro;
            estrutura.Almoxarifado.EnderecoNumero = this.View.EnderecoNumero;
            estrutura.Almoxarifado.EnderecoCompl = this.View.EnderecoComplemento;
            estrutura.Almoxarifado.EnderecoBairro = this.View.EnderecoBairro;
            estrutura.Almoxarifado.EnderecoMunicipio = this.View.EnderecoMunicipio;
            estrutura.Almoxarifado.Uf = new UFEntity(TratamentoDados.TryParseInt32(this.View.UfId));
            estrutura.Almoxarifado.EnderecoCep = TratamentoDados.RetirarMascara(this.View.EnderecoCep);
            estrutura.Almoxarifado.EnderecoTelefone = TratamentoDados.RetirarMascara(this.View.EnderecoTelefone);
            estrutura.Almoxarifado.EnderecoFax = TratamentoDados.RetirarMascara(this.View.EnderecoFax);
            estrutura.Almoxarifado.Responsavel = this.View.Responsavel;
            estrutura.Almoxarifado.Uge = 
                new UGEEntity(TratamentoDados.TryParseInt32(this.View.UgeId));
            // inverte para mes/ano
//            string mesAno = this.View.RefInicial.Substring(5, 2) + "/" + this.View.RefInicial.Substring(0, 4);

            estrutura.Almoxarifado.RefInicial = TratamentoDados.TryParseMesAno(this.View.RefInicial);
            //if (this.View.Id != "")
            if (!String.IsNullOrWhiteSpace(this.View.Id) && !String.IsNullOrWhiteSpace(this.view.Id))
            {
                //Busca o mês referência do banco
                int almoxId = (int)TratamentoDados.TryParseInt32(this.view.Id);
                estrutura.Almoxarifado.MesRef = new EstruturaOrganizacionalBusiness().ObterAlmoxarifado(almoxId).MesRef;

                //Caso esteja nulo, pega o mês referência Inacial
                if (String.IsNullOrEmpty(estrutura.Almoxarifado.MesRef))
                    estrutura.Almoxarifado.MesRef = TratamentoDados.TryParseMesAno(this.View.RefInicial);
            }
            else
                estrutura.Almoxarifado.MesRef = TratamentoDados.TryParseMesAno(this.View.RefInicial);

            estrutura.Almoxarifado.IndicadorAtividade =
                new IndicadorAtividadeEntity(TratamentoDados.TryParseInt32(this.View.IndicadorAtividadeId));

            if (estrutura.SalvarAlmoxarifado())
            {
                this.View.PopularGrid();
                this.GravadoSucesso();
                this.View.ExibirMensagem("Registro Salvo Com Sucesso.");
            }
            else
                this.View.ExibirMensagem("Registro com Inconsistências, verificar mensagens!");
            
            this.View.ListaErros = estrutura.ListaErro;
        }

        public void Excluir()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            estrutura.Almoxarifado.Id = TratamentoDados.TryParseInt32(this.View.Id);

            if (estrutura.ExcluirAlmoxarifado())
            {
                this.View.PopularGrid();
                this.ExcluidoSucesso();
                this.View.ExibirMensagem("Registro excluÃ­do com Sucesso.");
            }
            else
                this.View.ExibirMensagem("Não foi possível excluir, verifique as mensagens.");
            this.View.ListaErros = estrutura.ListaErro;
        }

        public void Imprimir()
        {
            //RelatorioEntity.Id = (int)RelatorioEnum.Almoxarifado;
            //RelatorioEntity.Nome = "rptAlmoxarifado.rdlc";
            //RelatorioEntity.DataSet = "dsAlmoxarifado";
            //RelatorioEntity.Parametros = this.View.ParametrosRelatorio;

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.Almoxarifado;
            relatorioImpressao.Nome = "rptAlmoxarifado.rdlc";
            relatorioImpressao.DataSet = "dsAlmoxarifado";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

        public override void RegistroSelecionado()
        {
            this.View.BloqueiaEnderecoLogradouro = true;
            this.View.BloqueiaEnderecoNumero = true;
            this.View.BloqueiaEnderecoComplemento = true;
            this.View.BloqueiaEnderecoBairro = true;
            this.View.BloqueiaEnderecoMunicipio = true;
            this.View.BloqueiaEnderecoComplemento = true;
            this.View.BloqueiaEnderecoCep = true;
            this.View.BloqueiaEnderecoTelefone = true;
            this.View.BloqueiaEnderecoFax = true;
            //this.View.BloqueiaRefInicial = true;
            this.View.BloqueiaListaUge = true;
            this.View.BloqueiaListaUf = true;
            this.View.BloqueiaResponsavel = true;
            this.View.BloqueiaListaIndicadorAtividade = true;
            base.RegistroSelecionado();
        }

        public override void GravadoSucesso()
        {
            this.View.UfId = " - Selecione - ";
            this.View.UgeId = " - Selecione - ";
            this.View.IndicadorAtividadeId = " - Selecione - ";
            this.View.EnderecoLogradouro = string.Empty;
            this.View.BloqueiaEnderecoLogradouro = false;
            this.View.EnderecoNumero = string.Empty;
            this.View.BloqueiaEnderecoNumero = false;
            this.View.EnderecoComplemento = string.Empty;
            this.View.BloqueiaEnderecoComplemento = false;
            this.View.EnderecoBairro = string.Empty;
            this.View.BloqueiaEnderecoBairro = false;
            this.View.EnderecoMunicipio = string.Empty;
            this.View.BloqueiaEnderecoMunicipio = false;
            this.View.BloqueiaListaUf = false;
            this.View.EnderecoCep = string.Empty;
            this.View.BloqueiaEnderecoCep = false;
            this.View.EnderecoTelefone = string.Empty;
            this.View.BloqueiaEnderecoTelefone = false;
            this.View.EnderecoFax = string.Empty;
            this.View.BloqueiaEnderecoFax = false;
            this.view.Responsavel = string.Empty;
            this.View.BloqueiaResponsavel = false;
            this.View.RefInicial = string.Empty;
            this.View.BloqueiaRefInicial = false;
            this.View.BloqueiaListaIndicadorAtividade = false;
            this.View.BloqueiaListaUge = false;
            base.GravadoSucesso();
        }

        public override void ExcluidoSucesso()
        {
            this.View.UfId = " - Selecione - ";
            this.View.UgeId = " - Selecione - ";
            this.View.IndicadorAtividadeId = " - Selecione - ";
            this.View.EnderecoLogradouro = string.Empty;
            this.View.BloqueiaEnderecoLogradouro = false;
            this.View.EnderecoNumero = string.Empty;
            this.View.BloqueiaEnderecoNumero = false;
            this.View.EnderecoComplemento = string.Empty;
            this.View.BloqueiaEnderecoComplemento = false;
            this.View.EnderecoBairro = string.Empty;
            this.View.BloqueiaEnderecoBairro = false;
            this.View.EnderecoMunicipio = string.Empty;
            this.View.BloqueiaEnderecoMunicipio = false;
            this.View.EnderecoCep = string.Empty;
            this.View.BloqueiaEnderecoCep = false;
            this.View.EnderecoTelefone = string.Empty;
            this.View.BloqueiaEnderecoTelefone = false;
            this.View.EnderecoFax = string.Empty;
            this.View.BloqueiaEnderecoFax = false;
            this.View.RefInicial = string.Empty;
            this.View.BloqueiaRefInicial = false;
            this.View.Responsavel = string.Empty;
            this.View.BloqueiaResponsavel = false;


            this.View.BloqueiaListaUge = false;
            this.View.BloqueiaListaUf = false;
            this.View.BloqueiaListaIndicadorAtividade = false;
            base.ExcluidoSucesso();
        }
        
        public override void Novo()
        {
            this.View.UfId = " - Selecione - ";
            this.View.UgeId = " - Selecione - ";
            this.View.IndicadorAtividadeId = " - Selecione - ";
            this.View.EnderecoLogradouro = string.Empty;
            this.View.BloqueiaEnderecoLogradouro = true;
            this.View.EnderecoNumero = string.Empty;
            this.View.BloqueiaEnderecoNumero = true;
            this.View.EnderecoComplemento = string.Empty;
            this.View.BloqueiaEnderecoComplemento = true;
            this.View.EnderecoBairro = string.Empty;
            this.View.BloqueiaEnderecoBairro = true;
            this.View.EnderecoMunicipio = string.Empty;
            this.View.BloqueiaEnderecoMunicipio = true;
            this.View.EnderecoCep = string.Empty;
            this.View.BloqueiaEnderecoCep = true;
            this.View.EnderecoTelefone = string.Empty;
            this.View.BloqueiaEnderecoTelefone = true;
            this.View.EnderecoFax = string.Empty;
            this.View.BloqueiaEnderecoFax = true;
            this.View.RefInicial = string.Empty;
            this.View.BloqueiaRefInicial = true;
            this.View.Responsavel = string.Empty;
            this.View.BloqueiaResponsavel = true;

            this.View.BloqueiaListaUge = true;
            this.View.BloqueiaListaUf = true;
            this.View.BloqueiaListaIndicadorAtividade = true;
            base.Novo();
        }

        public override void Cancelar()
        {
            this.View.EnderecoLogradouro = string.Empty;
            this.View.EnderecoNumero = string.Empty;
            this.View.EnderecoComplemento = string.Empty;
            this.View.EnderecoBairro = string.Empty;
            this.View.EnderecoMunicipio = string.Empty;
            this.View.EnderecoCep = string.Empty;
            this.View.EnderecoTelefone = string.Empty;
            this.View.EnderecoFax = string.Empty;
            this.View.RefInicial = string.Empty;
            this.View.Responsavel = string.Empty;

            this.View.UfId = " - Selecione - ";
            this.View.UgeId = " - Selecione - ";
            this.View.IndicadorAtividadeId = " - Selecione - ";
            
            this.View.BloqueiaEnderecoLogradouro = false;
            this.View.BloqueiaEnderecoNumero = false;
            this.View.BloqueiaEnderecoComplemento = false;
            this.View.BloqueiaEnderecoBairro = false;
            this.View.BloqueiaEnderecoMunicipio = false;
            this.View.BloqueiaEnderecoComplemento = false;
            this.View.BloqueiaEnderecoCep = false;
            this.View.BloqueiaEnderecoTelefone = false;
            this.View.BloqueiaEnderecoFax = false;
            this.View.BloqueiaRefInicial = false;
            
            this.View.BloqueiaListaUge = false;
            this.View.BloqueiaListaUf = false;
            this.View.BloqueiaResponsavel = false;
            this.View.BloqueiaListaIndicadorAtividade = false;
            base.Cancelar();
        }

        public AlmoxarifadoEntity SelecionarAlmoxarifadoPorGestor(int almoxarifadoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            AlmoxarifadoEntity retorno = estrutura.SelecionarAlmoxarifadoPorGestor(almoxarifadoId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public AlmoxarifadoEntity ObterAlmoxarifado(int AlmoxarifadoID)
        {
            EstruturaOrganizacionalBusiness objBusiness = new EstruturaOrganizacionalBusiness();
            AlmoxarifadoEntity objEntidade = objBusiness.ObterAlmoxarifado(AlmoxarifadoID);

            return objEntidade;
        }
    }
}
