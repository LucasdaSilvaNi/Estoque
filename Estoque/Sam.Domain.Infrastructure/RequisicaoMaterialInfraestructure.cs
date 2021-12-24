using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;
using System.Configuration;


namespace Sam.Domain.Infrastructure
{
    public class RequisicaoMaterial : BaseInfraestructure, IDivisaoService
    {
        private DivisaoEntity Divisao = new DivisaoEntity();
        public DivisaoEntity Entity
        {
            get { return Divisao; }
            set { Divisao = value; }
        }

        public IList<DivisaoEntity> Listar()
        {
            IList<DivisaoEntity> resultado = (from a in Db.TB_DIVISAOs
                                             orderby a.TB_DIVISAO_CODIGO
                                             select new DivisaoEntity
                                             {
                                                 Id = a.TB_DIVISAO_ID,
                                                 Codigo = a.TB_DIVISAO_CODIGO,
                                                 Descricao = a.TB_DIVISAO_DESCRICAO,
                                                 Responsavel = new ResponsavelEntity(a.TB_RESPONSAVEL_ID),
                                                 Almoxarifado = new AlmoxarifadoEntity(a.TB_ALMOXARIFADO_ID),
                                                 EnderecoLogradouro = a.TB_DIVISAO_LOGRADOURO,
                                                 EnderecoNumero = a.TB_DIVISAO_NUMERO,
                                                 EnderecoCompl = a.TB_DIVISAO_COMPLEMENTO,
                                                 EnderecoBairro = a.TB_DIVISAO_BAIRRO,
                                                 EnderecoMunicipio = a.TB_DIVISAO_MUNICIPIO,
                                                 Uf = new UFEntity(a.TB_UF_ID),
                                                 EnderecoCep = a.TB_DIVISAO_CEP,
                                                 EnderecoTelefone = a.TB_DIVISAO_TELEFONE,
                                                 EnderecoFax = a.TB_DIVISAO_FAX,
                                                 Area = Convert.ToInt32(a.TB_DIVISAO_AREA),
                                                 NumeroFuncionarios = a.TB_DIVISAO_QTDE_FUNC,
                                                 IndicadorAtividade = (bool)a.TB_DIVISAO_INDICADOR_ATIVIDADE
                                             })
                                             .Skip(this.SkipRegistros)
                                             .Take(this.RegistrosPagina)
                                             .ToList<DivisaoEntity>();

            this.totalregistros = (from a in Db.TB_DIVISAOs
                                   select new
                                   {
                                       a.TB_DIVISAO_ID,
                                   }).Count();
            return resultado;
        }

        public IList<DivisaoEntity> Listar(int UoId, Int64 UgeId = default(Int64))
        {
            IQueryable<DivisaoEntity> retorno = (from a in Db.TB_DIVISAOs
                                                 orderby a.TB_DIVISAO_DESCRICAO
                                                 orderby a.TB_UA.TB_UGE.TB_UO.TB_UO_CODIGO
                                                 orderby a.TB_UA.TB_UGE.TB_UGE_CODIGO
                                                 orderby a.TB_UA.TB_UA_CODIGO
                                                 orderby a.TB_DIVISAO_CODIGO
                                                 select new DivisaoEntity
                                                 {
                                                     Id = a.TB_DIVISAO_ID,
                                                     Codigo = a.TB_DIVISAO_CODIGO,
                                                     Descricao = a.TB_DIVISAO_DESCRICAO,
                                                     Responsavel = a.TB_RESPONSAVEL_ID != null ? new ResponsavelEntity(a.TB_RESPONSAVEL_ID) : new ResponsavelEntity(),
                                                     Almoxarifado = a.TB_ALMOXARIFADO_ID != null ? new AlmoxarifadoEntity(a.TB_ALMOXARIFADO_ID) : new AlmoxarifadoEntity(),
                                                     EnderecoLogradouro = a.TB_DIVISAO_LOGRADOURO,
                                                     EnderecoNumero = a.TB_DIVISAO_NUMERO,
                                                     EnderecoCompl = a.TB_DIVISAO_COMPLEMENTO,
                                                     EnderecoBairro = a.TB_DIVISAO_BAIRRO,
                                                     EnderecoMunicipio = a.TB_DIVISAO_MUNICIPIO,
                                                     Uf = new UFEntity(a.TB_UF_ID),
                                                     EnderecoCep = a.TB_DIVISAO_CEP,
                                                     EnderecoTelefone = a.TB_DIVISAO_TELEFONE,
                                                     EnderecoFax = a.TB_DIVISAO_FAX,
                                                     Area = a.TB_DIVISAO_AREA.HasValue ? Convert.ToInt32(a.TB_DIVISAO_AREA) : 0,
                                                     NumeroFuncionarios = a.TB_DIVISAO_QTDE_FUNC,
                                                     IndicadorAtividade = (bool)a.TB_DIVISAO_INDICADOR_ATIVIDADE,
                                                     Ua = new UAEntity
                                                     {
                                                         Id = a.TB_UA.TB_UA_ID,
                                                         Codigo = a.TB_UA.TB_UA_CODIGO,
                                                         CodigoFormatado = a.TB_UA.TB_UA_CODIGO.ToString().PadLeft(7, '0'),
                                                         CodigoDescricao = string.Format("{0} - {1}", a.TB_UA.TB_UA_CODIGO.ToString().PadLeft(7, '0'), a.TB_UA.TB_UA_DESCRICAO),
                                                         Descricao = a.TB_UA.TB_UA_DESCRICAO,
                                                         Gestor = new GestorEntity
                                                         {
                                                             Id = a.TB_UA.TB_GESTOR.TB_GESTOR_ID,
                                                             Nome = a.TB_UA.TB_GESTOR.TB_GESTOR_NOME
                                                         },
                                                         Uge = new UGEEntity
                                                         {
                                                             Id = a.TB_UA.TB_UGE.TB_UGE_ID,
                                                             Codigo = a.TB_UA.TB_UGE.TB_UGE_CODIGO,
                                                             CodigoFormatado = a.TB_UA.TB_UGE.TB_UGE_CODIGO.ToString().PadLeft(6, '0'),
                                                             CodigoDescricao = string.Format("{0} - {1}", a.TB_UA.TB_UGE.TB_UGE_CODIGO.ToString().PadLeft(6, '0'), a.TB_UA.TB_UGE.TB_UGE_DESCRICAO),
                                                             Descricao = a.TB_UA.TB_UGE.TB_UGE_DESCRICAO,
                                                             Uo = new UOEntity
                                                             {
                                                                 Id = a.TB_UA.TB_UGE.TB_UO.TB_UO_ID,
                                                                 Codigo = a.TB_UA.TB_UGE.TB_UO.TB_UO_CODIGO,
                                                                 CodigoFormatado = a.TB_UA.TB_UGE.TB_UO.TB_UO_CODIGO.ToString().PadLeft(6, '0'),
                                                                 CodigoDescricao = string.Format("{0} - {1}", a.TB_UA.TB_UGE.TB_UO.TB_UO_CODIGO.ToString().PadLeft(6, '0'), a.TB_UA.TB_UGE.TB_UO.TB_UO_DESCRICAO),
                                                                 Descricao = a.TB_UA.TB_UGE.TB_UO.TB_UO_DESCRICAO,
                                                             }
                                                         }
                                                     }

                                                 });

            this.totalregistros = retorno.Count();

            if (this.Entity.Ua.Uge != null && this.Entity.Ua.Uge.Id != 0)
                retorno = retorno.Where(a => a.Ua.Uge.Id == this.Entity.Ua.Uge.Id && (a.Ua.Uge.Id == UgeId || UgeId == 0));

            if (this.Entity.Ua.Uge.Uo != null && this.Entity.Ua.Uge.Uo.Id != 0)
                retorno = retorno.Where(a => a.Ua.Uge.Uo.Id == this.Entity.Ua.Uge.Uo.Id && a.Ua.Uge.Uo.Id == UoId);


            return retorno.ToList<DivisaoEntity>();
        }

        public IList<DivisaoEntity> Listar(int OrgaoId, int UaId)
        {
            IList<DivisaoEntity> resultado = (from a in Db.TB_DIVISAOs  
                                              join b in Db.TB_UFs on a.TB_UF_ID equals b.TB_UF_ID
                                              where (a.TB_UA_ID == UaId)
                                              orderby a.TB_DIVISAO_CODIGO
                                              select new DivisaoEntity
                                              {
                                                  Id = a.TB_DIVISAO_ID,
                                                  Codigo = a.TB_DIVISAO_CODIGO,
                                                  Descricao = a.TB_DIVISAO_DESCRICAO,
                                                  Responsavel = new ResponsavelEntity(a.TB_RESPONSAVEL_ID),
                                                  Almoxarifado = new AlmoxarifadoEntity(a.TB_ALMOXARIFADO_ID),
                                                  EnderecoLogradouro = a.TB_DIVISAO_LOGRADOURO,
                                                  EnderecoNumero = a.TB_DIVISAO_NUMERO,
                                                  EnderecoCompl = a.TB_DIVISAO_COMPLEMENTO,
                                                  EnderecoBairro = a.TB_DIVISAO_BAIRRO,
                                                  EnderecoMunicipio = a.TB_DIVISAO_MUNICIPIO,
                                                  Uf = new UFEntity { Id = b.TB_UF_ID, Descricao = b.TB_UF_DESCRICAO, Sigla = b.TB_UF_SIGLA },
                                                  EnderecoCep = a.TB_DIVISAO_CEP,
                                                  EnderecoTelefone = a.TB_DIVISAO_TELEFONE,
                                                  EnderecoFax = a.TB_DIVISAO_FAX,
                                                  Area = Convert.ToInt32(a.TB_DIVISAO_AREA),
                                                  NumeroFuncionarios = a.TB_DIVISAO_QTDE_FUNC,
                                                  IndicadorAtividade = (bool)a.TB_DIVISAO_INDICADOR_ATIVIDADE
                                              })
                                              .Skip(this.SkipRegistros)
                                              .Take(this.RegistrosPagina)
                                              .ToList<DivisaoEntity>();

            this.totalregistros = (from a in Db.TB_DIVISAOs
                                   where (a.TB_UA_ID == UaId)
                                   select new
                                   {
                                       a.TB_DIVISAO_ID,
                                   }).Count();
            return resultado;
        }

        public IList<DivisaoEntity>Imprimir()
        {
            IList<DivisaoEntity> resultado = (from a in Db.TB_DIVISAOs
                                              orderby a.TB_DIVISAO_CODIGO
                                              select new DivisaoEntity
                                              {
                                                  Id = a.TB_DIVISAO_ID,
                                                  Codigo = a.TB_DIVISAO_CODIGO,
                                                  Descricao = a.TB_DIVISAO_DESCRICAO,
                                                  Responsavel = new ResponsavelEntity(a.TB_RESPONSAVEL_ID),
                                                  Almoxarifado = new AlmoxarifadoEntity(a.TB_ALMOXARIFADO_ID),
                                                  EnderecoLogradouro = a.TB_DIVISAO_LOGRADOURO,
                                                  EnderecoNumero = a.TB_DIVISAO_NUMERO,
                                                  EnderecoCompl = a.TB_DIVISAO_COMPLEMENTO,
                                                  EnderecoBairro = a.TB_DIVISAO_BAIRRO,
                                                  EnderecoMunicipio = a.TB_DIVISAO_MUNICIPIO,
                                                  Uf = new UFEntity(a.TB_UF_ID),
                                                  EnderecoCep = a.TB_DIVISAO_CEP,
                                                  EnderecoTelefone = a.TB_DIVISAO_TELEFONE,
                                                  EnderecoFax = a.TB_DIVISAO_FAX,
                                                  Area = Convert.ToInt32(a.TB_DIVISAO_AREA),
                                                  NumeroFuncionarios = a.TB_DIVISAO_QTDE_FUNC,
                                                  IndicadorAtividade = (bool)a.TB_DIVISAO_INDICADOR_ATIVIDADE
                                              })
                                             .ToList<DivisaoEntity>();

          
            return resultado;
        }

        public DivisaoEntity Select(int _id) 
        {
            DivisaoEntity resultado = (from a in Db.TB_DIVISAOs
                                       where (a.TB_DIVISAO_ID == _id)
                                       select new DivisaoEntity
                                       {
                                           Id = a.TB_DIVISAO_ID,
                                           Codigo = a.TB_DIVISAO_CODIGO,
                                           Descricao = a.TB_DIVISAO_DESCRICAO,
                                           Responsavel = new ResponsavelEntity(a.TB_RESPONSAVEL_ID),
                                           Almoxarifado = new AlmoxarifadoEntity(a.TB_ALMOXARIFADO_ID),
                                           EnderecoLogradouro = a.TB_DIVISAO_LOGRADOURO,
                                           EnderecoNumero = a.TB_DIVISAO_NUMERO,
                                           EnderecoCompl = a.TB_DIVISAO_COMPLEMENTO,
                                           EnderecoBairro = a.TB_DIVISAO_BAIRRO,
                                           EnderecoMunicipio = a.TB_DIVISAO_MUNICIPIO,
                                           Uf = new UFEntity(a.TB_UF_ID),
                                           EnderecoCep = a.TB_DIVISAO_CEP,
                                           EnderecoTelefone = a.TB_DIVISAO_TELEFONE,
                                           EnderecoFax = a.TB_DIVISAO_FAX,
                                           Area = Convert.ToInt32(a.TB_DIVISAO_AREA),
                                           NumeroFuncionarios = a.TB_DIVISAO_QTDE_FUNC,
                                           IndicadorAtividade = (bool)a.TB_DIVISAO_INDICADOR_ATIVIDADE
                                       }).FirstOrDefault();

            return resultado;
        }


        public IList<DivisaoEntity> Imprimir(int OrgaoId, int UaId)
        {
            IList<DivisaoEntity> resultado = (from a in Db.TB_DIVISAOs
                                              join b in Db.TB_UFs on a.TB_UF_ID equals b.TB_UF_ID
                                              where (a.TB_UA_ID == UaId)
                                              orderby a.TB_DIVISAO_CODIGO
                                              select new DivisaoEntity
                                              {
                                                  Id = a.TB_DIVISAO_ID,
                                                  Codigo = a.TB_DIVISAO_CODIGO,
                                                  Descricao = a.TB_DIVISAO_DESCRICAO,
                                                  Responsavel = new ResponsavelEntity(a.TB_RESPONSAVEL_ID),
                                                  Almoxarifado = new AlmoxarifadoEntity(a.TB_ALMOXARIFADO_ID),
                                                  EnderecoLogradouro = a.TB_DIVISAO_LOGRADOURO,
                                                  EnderecoNumero = a.TB_DIVISAO_NUMERO,
                                                  EnderecoCompl = a.TB_DIVISAO_COMPLEMENTO,
                                                  EnderecoBairro = a.TB_DIVISAO_BAIRRO,
                                                  EnderecoMunicipio = a.TB_DIVISAO_MUNICIPIO,
                                                  Uf = new UFEntity { Id = b.TB_UF_ID, Descricao = b.TB_UF_DESCRICAO, Sigla = b.TB_UF_SIGLA },
                                                  EnderecoCep = a.TB_DIVISAO_CEP,
                                                  EnderecoTelefone = a.TB_DIVISAO_TELEFONE,
                                                  EnderecoFax = a.TB_DIVISAO_FAX,
                                                  Area = Convert.ToInt32(a.TB_DIVISAO_AREA),
                                                  NumeroFuncionarios = a.TB_DIVISAO_QTDE_FUNC,
                                                  IndicadorAtividade = (bool)a.TB_DIVISAO_INDICADOR_ATIVIDADE
                                              })
                                              .ToList<DivisaoEntity>();

       
            return resultado;
        }

        public void Excluir()
        {
            TB_DIVISAO tbDivisao = this.Db.TB_DIVISAOs
                .Where(a => a.TB_DIVISAO_ID == this.Entity.Id).FirstOrDefault();
            this.Db.TB_DIVISAOs.DeleteOnSubmit(tbDivisao);
            this.Db.SubmitChanges();
        }

        public void Salvar()
        {
            TB_DIVISAO tbDivisao = new TB_DIVISAO();

            if (this.Entity.Id.HasValue)
            {
                tbDivisao = this.Db.TB_DIVISAOs.Where(a => a.TB_DIVISAO_ID == this.Entity.Id).FirstOrDefault();
            }
            else
                Db.TB_DIVISAOs.InsertOnSubmit(tbDivisao);

            tbDivisao.TB_UA_ID = this.Entity.Ua.Id.Value;
            tbDivisao.TB_DIVISAO_CODIGO = this.Entity.Codigo.Value;
            tbDivisao.TB_DIVISAO_DESCRICAO = this.Entity.Descricao;
            tbDivisao.TB_RESPONSAVEL_ID = this.Entity.Responsavel.Id.Value;
            tbDivisao.TB_ALMOXARIFADO_ID = this.Entity.Almoxarifado.Id.Value;
            tbDivisao.TB_DIVISAO_LOGRADOURO = this.Entity.EnderecoLogradouro;
            tbDivisao.TB_DIVISAO_NUMERO = this.Entity.EnderecoNumero;
            tbDivisao.TB_DIVISAO_COMPLEMENTO = this.Entity.EnderecoCompl;
            tbDivisao.TB_DIVISAO_BAIRRO = this.Entity.EnderecoBairro;
            tbDivisao.TB_DIVISAO_MUNICIPIO = this.Entity.EnderecoMunicipio;
            tbDivisao.TB_UF_ID = this.Entity.Uf.Id.Value;
            tbDivisao.TB_DIVISAO_CEP = this.Entity.EnderecoCep;
            tbDivisao.TB_DIVISAO_TELEFONE = this.Entity.EnderecoTelefone;
            tbDivisao.TB_DIVISAO_FAX = this.Entity.EnderecoFax;
            tbDivisao.TB_DIVISAO_AREA = this.Entity.Area;
            tbDivisao.TB_DIVISAO_QTDE_FUNC = this.Entity.NumeroFuncionarios;
            tbDivisao.TB_DIVISAO_INDICADOR_ATIVIDADE = this.Entity.IndicadorAtividade;

            this.Db.SubmitChanges();
        }

        public bool PodeExcluir()
        {
            bool retorno = true;

            //TB_DIVISAO tbDivisao = this.Db.TB_DIVISAOs
            //    .Where(a => a.TB_INDICADOR_ATIVIDADE_ID == this.Entity.Id).FirstOrDefault();

            //if (tbDivisao.
                //return false;

            return retorno;
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = true;

            if (this.Entity.Id.HasValue)
            {
                retorno = this.Db.TB_DIVISAOs
                    .Where(a => a.TB_DIVISAO_CODIGO == this.Entity.Codigo.Value)
                    .Where(a => a.TB_DIVISAO_ID != this.Entity.Id.Value)
                    .Where(a => a.TB_UA_ID == this.Entity.Ua.Id)
                    .Count() > 0;
            }
            else
                retorno = this.Db.TB_DIVISAOs
                    .Where(a => a.TB_DIVISAO_CODIGO == this.Entity.Codigo)
                    .Where(a => a.TB_UA_ID == this.Entity.Ua.Id)
                    .Count() > 0;

            return retorno;
        }


        public DivisaoEntity LerRegistro()
        {
            throw new NotImplementedException();
        }


        public IList<DivisaoEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }


        public IList<DivisaoEntity> ListarPorUgeTodosCod(int UgeId)
        {
            throw new NotImplementedException();
        }


        public IList<DivisaoEntity> ListarPorAlmoxTodosCod(int AlmoxId)
        {
            throw new NotImplementedException();
        }


        public IList<DivisaoEntity> ListarDivisaoByUA(int uaId, int gestorId)
        {
            throw new NotImplementedException();
        }

        public IList<DivisaoEntity> ListarDivisaoByUA(int uaId, int gestorId, AlmoxarifadoEntity almoxarifado)
        {
            throw new NotImplementedException();
        }

        public IList<DivisaoEntity> ListarDivisaoByGestor(int gestorId, int? UOId, int? UGEId)
        {
            throw new NotImplementedException();
        }

        public DivisaoEntity ObterDivisaoUA(int codigoUA, int codigoDivisaoUA)
        {
            throw new NotImplementedException();
        }
    }
}
