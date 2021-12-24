using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Sam.Domain.Entity;
using System.Collections;
using Sam.ServiceInfraestructure;
using Sam.Common.Util;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace Sam.Domain.Business
{
    public partial class EstruturaOrganizacionalBusiness : BaseBusiness
    {
        #region Divisão

        private DivisaoEntity divisao = new DivisaoEntity();
        public DivisaoEntity Divisao
        {
            get { return divisao; }
            set { divisao = value; }
        }

        public bool SalvarDivisao()
        {
            this.Service<IDivisaoService>().Entity = this.Divisao;
            this.ConsistirDivisao();

            if (this.Consistido)
            {
                this.Service<IDivisaoService>().Salvar();
            }

            return this.Consistido;
        }

        public void ConsistirDivisao()
        {
            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<DivisaoEntity>(ref this.divisao);

            if(!this.Divisao.Ua.Id.HasValue)
                this.ListaErro.Add("É obrigatório informar a UA.");

            if (!this.Divisao.Codigo.HasValue)
            {
                this.ListaErro.Add("É obrigatório informar o Código.");
            }
            else 
            {
                if (this.Divisao.Codigo == 0)
                    this.ListaErro.Add("Código da Divisão inválido!");
            }

            if(this.Divisao.Descricao == null || this.Divisao.Descricao == "" )
                this.ListaErro.Add("É obrigatório informar a Descrição.");

            if(!this.Divisao.Almoxarifado.Id.HasValue)
                this.ListaErro.Add("É obrigatorio informar o Almoxarifado.");

            if(this.Divisao.EnderecoLogradouro == null || this.Divisao.EnderecoLogradouro  == "" )
                this.ListaErro.Add("É obrigatório informar o Endereço.");

            if(this.Divisao.EnderecoNumero == null || this.Divisao.EnderecoNumero == "" )
                this.ListaErro.Add("É obrigatório informar o Número.");
           
            if(this.Divisao.EnderecoBairro == null || this.Divisao.EnderecoBairro == "" )
                this.ListaErro.Add("É obrigatório informar o Bairro.");
            
            if(this.Divisao.EnderecoMunicipio == null || this.Divisao.EnderecoMunicipio == "" )
                this.ListaErro.Add("É obrigatório informar o Municipio.");
            
            if(!this.Divisao.Uf.Id.HasValue)
                this.ListaErro.Add("É obrigatório informar a UF.");

            if (this.Divisao.EnderecoBairro == null || this.Divisao.EnderecoBairro == "")
                this.ListaErro.Add("É obrigatório informar o Bairro.");

            if (this.Divisao.EnderecoMunicipio == null || this.Divisao.EnderecoMunicipio == "")
                this.ListaErro.Add("É obrigatório informar o Municipio.");

            if (!this.Divisao.Uf.Id.HasValue)
                this.ListaErro.Add("É obrigatório informar a UF.");

            if (this.Divisao.EnderecoCep == null || this.Divisao.EnderecoCep == "")
                this.ListaErro.Add("É obrigatório informar o CEP.");

            if (this.Divisao.IndicadorAtividade.Value && !this.Service<IUAService>().ObterUA(this.divisao.Ua.Id.Value).IndicadorAtividade)
                this.ListaErro.Add("Não é permitido status ATIVO para Divisão associada a UA INATIVA");

            if (this.ListaErro.Count == 0)
            {
                var _divisao = this.Service<IDivisaoService>();
                _divisao.Entity = this.Divisao;

                if (_divisao.ExisteCodigoInformado())
                    this.ListaErro.Add("Código já existente.");
            }
        }


        public IList<DivisaoEntity> ListarDivisao(int OrgaoId, int UaId)
        {
            this.Service<IDivisaoService>().SkipRegistros = this.SkipRegistros;
            IList <DivisaoEntity> retorno = this.Service<IDivisaoService>().Listar(OrgaoId, UaId);
            this.TotalRegistros = this.Service<IDivisaoService>().TotalRegistros();
            return retorno;
        }

        public IList<DivisaoEntity> ListarDivisao(int UoId, Int64 UgeId)
        {
            this.Service<IDivisaoService>().SkipRegistros = this.SkipRegistros;
            IList<DivisaoEntity> retorno = this.Service<IDivisaoService>().Listar(UoId, UgeId);
            this.TotalRegistros = this.Service<IDivisaoService>().TotalRegistros();
            return retorno;
        }

        public IList<DivisaoEntity> ListarDivisaoPorUgeTodosCod(int _ugeId)
        {
            return this.Service<IDivisaoService>().ListarPorUgeTodosCod(_ugeId);
        }

        public IList<DivisaoEntity> ListarDivisaoPorAlmoxTodosCod(int _almoxId)
        {
            return this.Service<IDivisaoService>().ListarPorAlmoxTodosCod(_almoxId);
        }

        public IList<DivisaoEntity> ListarDivisaoByUA(int uaId, int gestorID)
        {
            return this.Service<IDivisaoService>().ListarDivisaoByUA(uaId, gestorID);
        }

        public IList<DivisaoEntity> ListarDivisaoByUA(int uaId, int gestorID, AlmoxarifadoEntity almoxarifado)
        {
            return this.Service<IDivisaoService>().ListarDivisaoByUA(uaId, gestorID, almoxarifado);
        }

        public IList<DivisaoEntity> ListarDivisao()
        {
            this.Service<IDivisaoService>().SkipRegistros = this.SkipRegistros;
            IList<DivisaoEntity> retorno = this.Service<IDivisaoService>().Listar();
            this.TotalRegistros = this.Service<IDivisaoService>().TotalRegistros();
            return retorno;
        }

        public IList<DivisaoEntity> ListarDivisaoByGestor(int gestorId, int? UOId, int? UGEId)
        {
            IList<DivisaoEntity> retorno = this.Service<IDivisaoService>().ListarDivisaoByGestor(gestorId, UOId, UGEId);            
            return retorno;
        }

        public IList<DivisaoEntity> ListarDivisaoTodosCod()
        {
            this.Service<IDivisaoService>().Entity = this.Divisao;
            return this.Service<IDivisaoService>().ListarTodosCod();
        }

        public void SelectDivisao(int _id)
        {
            this.Divisao = this.Service<IDivisaoService>().Select(_id);
        }


        public DivisaoEntity ObterDivisaoUA(int codigoUA, int codigoDivisaoUA)
        {
            DivisaoEntity objEntidade = null;

            objEntidade = this.Service<IDivisaoService>().ObterDivisaoUA(codigoUA, codigoDivisaoUA);
            return objEntidade;
        }

        public IList<DivisaoEntity> ImprimirDivisao(int OrgaoId, int UaId)
        {
            IList<DivisaoEntity> retorno = this.Service<IDivisaoService>().Imprimir(OrgaoId, UaId);
            return retorno;
        }

        public bool ExcluirDivisao()
        {
            this.Service<IDivisaoService>().Entity = this.Divisao;
            if (this.Consistido)
            {
                try
                {
                    this.Service<IDivisaoService>().Excluir();
                }
                catch (Exception ex)
                {
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        #endregion
    }
}
