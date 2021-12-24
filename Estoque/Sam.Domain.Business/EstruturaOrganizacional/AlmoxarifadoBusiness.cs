using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Sam.Domain.Entity;
using Sam.ServiceInfraestructure;
using Sam.Common.Util;
using System.Data.SqlClient;


namespace Sam.Domain.Business
{
    public partial class EstruturaOrganizacionalBusiness : BaseBusiness
    {
        #region Almoxarifado

        private AlmoxarifadoEntity almoxarifado = new AlmoxarifadoEntity();
        public AlmoxarifadoEntity Almoxarifado
        {
            get { return almoxarifado; }
            set { almoxarifado = value; }
        }

        public bool SalvarAlmoxarifado()
        {
            this.Service<IAlmoxarifadoService>().Entity = this.Almoxarifado;
            
            // Se for inclusão, setar o mês referência igual ao mês referência inicial
            if (this.Almoxarifado.Id == null){
                this.Almoxarifado.MesRef = this.Almoxarifado.RefInicial;
            }

            this.ConsistirAlmoxarifado();
            if (this.Consistido)
            {
                this.Service<IAlmoxarifadoService>().Salvar();
            }
            return this.Consistido;
        }

        public IList<AlmoxarifadoEntity> ListarAlmoxarifado(int OrgaoId, int GestorId)
        {
            this.Service<IAlmoxarifadoService>().SkipRegistros = this.SkipRegistros;
            IList<AlmoxarifadoEntity> retorno = this.Service<IAlmoxarifadoService>().Listar(OrgaoId, GestorId);
            this.TotalRegistros = this.Service<IAlmoxarifadoService>().TotalRegistros();
            return retorno;
        }

        public IList<AlmoxarifadoEntity> ListarAlmoxarifado(int OrgaoId, int GestorId, string AlmoxCodigo)
        {
            this.Service<IAlmoxarifadoService>().SkipRegistros = this.SkipRegistros;
            IList<AlmoxarifadoEntity> retorno = (!string.IsNullOrEmpty(AlmoxCodigo)) ? this.Service<IAlmoxarifadoService>().ListarCodigo(OrgaoId, GestorId, AlmoxCodigo):this.Service<IAlmoxarifadoService>().Listar(OrgaoId, GestorId);
            this.TotalRegistros = this.Service<IAlmoxarifadoService>().TotalRegistros();
            return retorno;
        }


        public IList<AlmoxarifadoEntity> ListarComboAlmoxarifado(int OrgaoId, int GestorId)
        {            
           IList<AlmoxarifadoEntity> retorno = this.Service<IAlmoxarifadoService>().ListarAlmoxarifadoPorOrgaoGestor(OrgaoId, GestorId);           
           return retorno;            
        }

        public IList<AlmoxarifadoEntity> ListarAlmoxarifadoLista(IList<AlmoxarifadoEntity> listaalmox, int OrgaoId, int GestorId, int AlmoxarifadoId)
        {
            this.Service<IAlmoxarifadoService>().SkipRegistros = this.SkipRegistros;
            IList<AlmoxarifadoEntity> retorno = this.Service<IAlmoxarifadoService>().ListarSelecionaAlmoxarifado(OrgaoId, GestorId, AlmoxarifadoId);
            this.TotalRegistros = this.Service<IAlmoxarifadoService>().TotalRegistros();
            return retorno;
        }

        public IList<AlmoxarifadoEntity> ListarSelecionaAlmoxarifado(int OrgaoId, int GestorId, int AlmoxarifadoId)
        {
            IList<AlmoxarifadoEntity> retorno = this.Service<IAlmoxarifadoService>().ListarSelecionaAlmoxarifado(OrgaoId, GestorId, AlmoxarifadoId);         
            return retorno;
        }

        public IList<AlmoxarifadoEntity> ListarSelecionaAlmoxarifadoTake(int OrgaoId, int GestorId, int AlmoxarifadoId)
        {
            this.Service<IAlmoxarifadoService>().SkipRegistros = this.SkipRegistros;
            IList<AlmoxarifadoEntity> retorno = this.Service<IAlmoxarifadoService>().ListarSelecionaAlmoxarifadoTake(OrgaoId, GestorId, AlmoxarifadoId);
            this.TotalRegistros = this.Service<IAlmoxarifadoService>().TotalRegistros();
            return retorno;
        }

        public IList<AlmoxarifadoEntity> ListarAlmoxarifado(int OrgaoId)
        {
            this.Service<IAlmoxarifadoService>().SkipRegistros = this.SkipRegistros;
            IList<AlmoxarifadoEntity> retorno = this.Service<IAlmoxarifadoService>().ListarAlmoxarifadoPorOrgaoTodosCod(OrgaoId);
            this.TotalRegistros = this.Service<IAlmoxarifadoService>().TotalRegistros();
            return retorno;
        }
        public IList<AlmoxarifadoEntity> ListarAlmoxarifadoPorOrgaoMesRef(int OrgaoId,string AnoMesReferencia)
        {
            this.Service<IAlmoxarifadoService>().SkipRegistros = this.SkipRegistros;
            IList<AlmoxarifadoEntity> retorno = this.Service<IAlmoxarifadoService>().ListarAlmoxarifadoPorOrgaoMesRef(OrgaoId, AnoMesReferencia);
            this.TotalRegistros = this.Service<IAlmoxarifadoService>().TotalRegistros();
            return retorno;
        }
        public IList<AlmoxarifadoEntity> ListarAlmoxarifadoPorGestorTodosCod(int GestorId)
        {
            return this.Service<IAlmoxarifadoService>().ListarAlmoxarifadoPorGestorTodosCod(GestorId);
        }

        public IList<AlmoxarifadoEntity> ListarAlmoxarifadoPorGestorTodosCod(int GestorId, bool pBlnPreencherObjetos)
        {
            return this.Service<IAlmoxarifadoService>().ListarAlmoxarifadoPorGestorTodosCod(GestorId, pBlnPreencherObjetos);
        }

        public IList<AlmoxarifadoEntity> ListarAlmoxarifadoPorUGE(int ugeID)
        {
            this.Service<IAlmoxarifadoService>().SkipRegistros = this.SkipRegistros;
            IList<AlmoxarifadoEntity> retorno = this.Service<IAlmoxarifadoService>().ListarAlmoxarifadoPorUGE(ugeID);
            this.TotalRegistros = this.Service<IAlmoxarifadoService>().TotalRegistros();
            return retorno;
        }

        public IList<AlmoxarifadoEntity> ListarAlmoxarifado()
        {
            this.Service<IAlmoxarifadoService>().SkipRegistros = this.SkipRegistros;
            IList<AlmoxarifadoEntity> retorno = this.Service<IAlmoxarifadoService>().Listar();
            this.TotalRegistros = this.Service<IAlmoxarifadoService>().TotalRegistros();
            return retorno;
        }

        public IList<AlmoxarifadoEntity> AlmoxarifadoTodosCod()
        {
            IList<AlmoxarifadoEntity> retorno = this.Service<IAlmoxarifadoService>().AlmoxarifadoTodosCod();            
            return retorno;
        }

        public IList<AlmoxarifadoEntity> AlmoxarifadoTodosCod(int OrgaoId, int GestorId)
        {
            IList<AlmoxarifadoEntity> retorno = this.Service<IAlmoxarifadoService>().AlmoxarifadoTodosCod(OrgaoId, GestorId);
            return retorno;
        }

        public IList<UGEEntity> ListarUGEGestor(int GestorId)
        {
            this.Service<IAlmoxarifadoService>().SkipRegistros = this.SkipRegistros;
            IList<UGEEntity> retorno = Service<IAlmoxarifadoService>().ListarUGEGestor(GestorId);
            this.TotalRegistros = this.Service<IAlmoxarifadoService>().TotalRegistros();
            return retorno;
        }

        public IList<AlmoxarifadoEntity> ImprimirAlmoxarifado(int OrgaoId, int GestorId)
        {
            IList<AlmoxarifadoEntity> retorno = this.Service<IAlmoxarifadoService>().Imprimir(OrgaoId, GestorId);
            return retorno;
        }

        public IList<AlmoxarifadoEntity> ImprmirAlmoxarifado(int GestorId)
        {
            this.Service<IAlmoxarifadoService>().Entity = new AlmoxarifadoEntity();
            this.Service<IAlmoxarifadoService>().Entity.Gestor = new GestorEntity(GestorId);
            IList<AlmoxarifadoEntity> retorno = this.Service<IAlmoxarifadoService>().Imprimir();
            return retorno;
        }

        public AlmoxarifadoEntity GetAlmoxarifadoByDivisao(int divisaoId)
        {
            return this.Service<IAlmoxarifadoService>().GetAlmoxarifadoByDivisao(divisaoId);
        }

        public bool ExcluirAlmoxarifado()
        {
            this.Service<IAlmoxarifadoService>().Entity = this.Almoxarifado;
            this.ConsistirExclusaoAlmoxarifado();
            if (this.Consistido)
            {
                try
                {
                    this.Service<IAlmoxarifadoService>().Excluir();
                }
                catch (Exception ex)
                {
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        public void ConsistirExclusaoAlmoxarifado()
        {
            this.Service<IAlmoxarifadoService>().Entity = this.Almoxarifado;
            if (!this.Service<IAlmoxarifadoService>().PodeExcluir())
                this.ListaErro.Add("Não é possível excluir o Almoxarifado, existem registros associados a ele.");
        }

        public void ConsistirAlmoxarifado()
        {
            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<AlmoxarifadoEntity>(ref this.almoxarifado);

            //Fazer validação
            if (!this.Almoxarifado.Orgao.Id.HasValue)
                this.ListaErro.Add("É obrigatório informar o Órgão.");

            if (!this.Almoxarifado.Gestor.Id.HasValue)
                this.ListaErro.Add("É obrigatório informar o Gestor.");

            if (!this.Almoxarifado.Codigo.HasValue)
                this.ListaErro.Add("É obrigatório informar o Código.");
            else if (this.Almoxarifado.Codigo.Value < 1 )
                this.ListaErro.Add("Informe um código maior que 0.");

            if (string.IsNullOrWhiteSpace(this.Almoxarifado.Descricao.Trim().ToString()))
                this.ListaErro.Add("É obrigatório informar a Descrição.");

            if (string.IsNullOrWhiteSpace(this.Almoxarifado.EnderecoLogradouro.Trim().ToString()))
                this.ListaErro.Add("É obrigatório informar o Endereço.");

            if (string.IsNullOrWhiteSpace(this.Almoxarifado.EnderecoNumero.Trim().ToString()))
                this.ListaErro.Add("É obrigatório informar o Número.");

            if (string.IsNullOrWhiteSpace(this.Almoxarifado.EnderecoBairro.Trim().ToString()))
                this.ListaErro.Add("É obrigatório informar o Bairro.");

            if (string.IsNullOrWhiteSpace(this.Almoxarifado.EnderecoMunicipio.Trim().ToString()))
                this.ListaErro.Add("É obrigatório informar o Município.");

            if (!this.Almoxarifado.Uf.Id.HasValue)
                this.ListaErro.Add("É obrigatório informar a UF.");

            if (string.IsNullOrWhiteSpace(this.Almoxarifado.EnderecoCep.Trim().ToString()))
                this.ListaErro.Add("É obrigatório informar o CEP.");

            if (string.IsNullOrWhiteSpace(this.Almoxarifado.Responsavel.Trim().ToString()))
                this.ListaErro.Add("É obrigatório informar o Responsável.");

            if (!this.Almoxarifado.Uge.Id.HasValue)
                this.ListaErro.Add("É obrigatório informar o UGE.");

            if (string.IsNullOrWhiteSpace(this.Almoxarifado.RefInicial.Trim().ToString()))
                this.ListaErro.Add("É obrigatório informar o Mês Ref. Inicial.");
            else
            {
                int mesAno = Convert.ToInt32(this.Almoxarifado.RefInicial.ToString());
                int mes = Convert.ToInt32(this.Almoxarifado.RefInicial.Substring(4,2));
                int agora = Convert.ToInt32(string.Concat(DateTime.Now.Year, DateTime.Now.Month.ToString("00")));
                
                if (mes > 12 )
                    this.ListaErro.Add("Mês inválido.");

                if (mesAno < 1900)
                    this.ListaErro.Add("Ano inválido.");

                if (mesAno > agora)
                    this.ListaErro.Add("Mês Ref Inicial deve ser menor ou igual ao ano/mês corrente.");

            }

            if (!this.Almoxarifado.IndicadorAtividade.Id.HasValue)
                this.ListaErro.Add("É obrigatório informar o Indicador de Atividade.");


            if (this.ListaErro.Count == 0)
            {
                if (this.Service<IAlmoxarifadoService>().ExisteCodigoInformado())
                    this.ListaErro.Add("Código já existente.");
            }
        }

        private bool existeMovimentacaoMaterialRetroativaAMesReferenciaInicial(int almoxarifadoId)
        {
            return this.Service<IAlmoxarifadoService>().ExisteMovimentacaoMaterialRetroativaAMesReferenciaAtual(almoxarifadoId);
        }

        public AlmoxarifadoEntity SelecionarAlmoxarifadoPorGestor(int almoxarifadoId)
        {
            return this.Service<IAlmoxarifadoService>().SelecionarAlmoxarifadoPorGestor(almoxarifadoId);
        }

        public AlmoxarifadoEntity ObterAlmoxarifado(int AlmoxarifadoID)
        {
            AlmoxarifadoEntity lObjRetorno = this.Service<IAlmoxarifadoService>().ObterAlmoxarifado(AlmoxarifadoID);
            return lObjRetorno;
        }

        public AlmoxarifadoEntity ObterAlmoxarifadoUGE(int codigoUGE, int codigoAlmoxarifado)
        {
            AlmoxarifadoEntity lObjRetorno = this.Service<IAlmoxarifadoService>().ObterAlmoxarifadoUGE(codigoUGE, codigoAlmoxarifado);
            return lObjRetorno;
        }

        public bool ExisteMovimentacaoMaterialRetroativaAMesReferenciaAtual(int almoxarifadoId)
        {
          return this.Service<IAlmoxarifadoService>().ExisteMovimentacaoMaterialRetroativaAMesReferenciaAtual(almoxarifadoId);
        }

        public string ObtemMesReferenciaAtual(int almoxarifadoId)
        {
            return this.Service<IAlmoxarifadoService>().ObtemMesReferenciaAtual(almoxarifadoId);
        }

        #endregion
    }
}
