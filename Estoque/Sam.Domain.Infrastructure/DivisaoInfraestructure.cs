using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;
using System.Configuration;
using System.Linq.Expressions;
using Sam.Common.Util;


namespace Sam.Domain.Infrastructure
{
    public class DivisaoInfraestructure : BaseInfraestructure,IDivisaoService
    {
        private DivisaoEntity Divisao = new DivisaoEntity();
        public DivisaoEntity Entity
        {
            get { return Divisao; }
            set { Divisao = value; }
        }

        public IList<DivisaoEntity> ListarDivisaoByGestor(int gestorId, int? UOId, int? UGEId)
        {
            IList<DivisaoEntity> resultado = (from a in Db.TB_DIVISAOs
                                              where a.TB_UA.TB_GESTOR_ID == gestorId
                                              where a.TB_UA.TB_UGE_ID == (int)UGEId
                                              where a.TB_UA.TB_UGE.TB_UO_ID == (int)UOId
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
                                                  IndicadorAtividade = (bool)a.TB_DIVISAO_INDICADOR_ATIVIDADE,

                                                   Ua = new UAEntity
                        {
                            Id = a.TB_UA.TB_UA_ID,
                            Codigo = a.TB_UA.TB_UA_CODIGO,
                            CodigoFormatado = a.TB_UA.TB_UA_CODIGO.ToString().PadLeft(7, '0'),
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
                                Descricao = a.TB_UA.TB_UGE.TB_UGE_DESCRICAO,
                                Uo = new UOEntity 
                                    {
                                        Id = a.TB_UA.TB_UGE.TB_UO.TB_UO_ID,
                                        Codigo = a.TB_UA.TB_UGE.TB_UO.TB_UO_CODIGO,
                                        CodigoFormatado = a.TB_UA.TB_UGE.TB_UO.TB_UO_CODIGO.ToString().PadLeft(6, '0'),
                                        Descricao = a.TB_UA.TB_UGE.TB_UO.TB_UO_DESCRICAO,
                                    }
                            }
                        }
                                              }).ToList<DivisaoEntity>();
            return resultado;
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

        public IList<DivisaoEntity> ListarDivisaoByUA(int uaId, int gestorId)
        {
            var resultado = (from a in Db.TB_DIVISAOs
                             where a.TB_UA_ID == uaId
                             where a.TB_ALMOXARIFADO.TB_GESTOR_ID == gestorId
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
                                 EnderecoCep = a.TB_DIVISAO_CEP,
                                 EnderecoTelefone = a.TB_DIVISAO_TELEFONE,
                                 EnderecoFax = a.TB_DIVISAO_FAX,
                                 Area = Convert.ToInt32(a.TB_DIVISAO_AREA),
                                 NumeroFuncionarios = a.TB_DIVISAO_QTDE_FUNC,
                                 IndicadorAtividade = (bool)a.TB_DIVISAO_INDICADOR_ATIVIDADE
                             }).ToList<DivisaoEntity>();
            return resultado;
        }

        public IList<DivisaoEntity> ListarDivisaoByUA(int uaId, int gestorId, AlmoxarifadoEntity almoxarifado)
        {
            var resultado = (from a in ListarDivisaoByUA(uaId, gestorId)
                             where a.Almoxarifado.Id == almoxarifado.Id
                             select a).ToList<DivisaoEntity>();
            return resultado;
        }

        public IList<DivisaoEntity> Listar(int OrgaoId, int UaId)
        {
            IList<DivisaoEntity> resultado = (from a in Db.TB_DIVISAOs
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
                                                  Uf = new UFEntity { Id = a.TB_UF.TB_UF_ID, Descricao = a.TB_UF.TB_UF_DESCRICAO, Sigla = a.TB_UF.TB_UF_SIGLA },
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

        public IList<DivisaoEntity> ListarPorAlmoxTodosCod(int AlmoxId)
        {
            IList<DivisaoEntity> resultado = (from a in Db.TB_DIVISAOs
                                              join ua in Db.TB_UAs on a.TB_UA_ID equals ua.TB_UA_ID
                                              join uge in Db.TB_UGEs on ua.TB_UGE_ID equals uge.TB_UGE_ID 
                                              join b in Db.TB_UFs on a.TB_UF_ID equals b.TB_UF_ID
                                              where (a.TB_ALMOXARIFADO_ID == AlmoxId)
                                              where (a.TB_DIVISAO_INDICADOR_ATIVIDADE == true)
                                              where (ua.TB_UA_INDICADOR_ATIVIDADE == true)
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
                                                  Area = a.TB_DIVISAO_AREA ?? 0,
                                                  NumeroFuncionarios = a.TB_DIVISAO_QTDE_FUNC ?? 0,
                                                  IndicadorAtividade = (bool?)a.TB_DIVISAO_INDICADOR_ATIVIDADE 
                                              }).ToList<DivisaoEntity>();

            return resultado;
        }

        public IList<DivisaoEntity> ListarPorUgeTodosCod(int UgeId)
        {
            IList<DivisaoEntity> resultado = (from a in Db.TB_DIVISAOs
                                              join ua in Db.TB_UAs on a.TB_UA_ID equals ua.TB_UA_ID
                                              join uge in Db.TB_UGEs on ua.TB_UGE_ID equals uge.TB_UGE_ID 
                                              join b in Db.TB_UFs on a.TB_UF_ID equals b.TB_UF_ID
                                              where (ua.TB_UGE_ID == UgeId)
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
                                                  Area = a.TB_DIVISAO_AREA ?? 0,
                                                  NumeroFuncionarios = a.TB_DIVISAO_QTDE_FUNC,
                                                  IndicadorAtividade = (bool)a.TB_DIVISAO_INDICADOR_ATIVIDADE
                                              }).ToList<DivisaoEntity>();

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
                                                  Area = a.TB_DIVISAO_AREA ?? 0,
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
                                       let uaDivisao = a.TB_UA
                                       let ugeUaDivisao = uaDivisao.TB_UGE
                                       let uoUgeUaDivisao = ugeUaDivisao.TB_UO
                                       let almoxDivisao = a.TB_ALMOXARIFADO
                                       select new DivisaoEntity
                                       {
                                           Id = a.TB_DIVISAO_ID,
                                           Codigo = a.TB_DIVISAO_CODIGO,
                                           Descricao = a.TB_DIVISAO_DESCRICAO,
                                           Responsavel = new ResponsavelEntity(a.TB_RESPONSAVEL_ID),
                                           //Almoxarifado = new AlmoxarifadoEntity(a.TB_ALMOXARIFADO_ID),
                                           Almoxarifado = new AlmoxarifadoEntity()
                                                                                   {
                                                                                       Id = almoxDivisao.TB_ALMOXARIFADO_ID,
                                                                                       Codigo = almoxDivisao.TB_ALMOXARIFADO_CODIGO,
                                                                                       Descricao = almoxDivisao.TB_ALMOXARIFADO_DESCRICAO
                                                                                   },
                                           EnderecoLogradouro = a.TB_DIVISAO_LOGRADOURO,
                                           EnderecoNumero = a.TB_DIVISAO_NUMERO,
                                           EnderecoCompl = a.TB_DIVISAO_COMPLEMENTO,
                                           EnderecoBairro = a.TB_DIVISAO_BAIRRO,
                                           EnderecoMunicipio = a.TB_DIVISAO_MUNICIPIO,
                                           Uf = new UFEntity(a.TB_UF_ID),
                                           EnderecoCep = a.TB_DIVISAO_CEP,
                                           EnderecoTelefone = a.TB_DIVISAO_TELEFONE,
                                           EnderecoFax = a.TB_DIVISAO_FAX,
                                           Area = a.TB_DIVISAO_AREA ?? 0,
                                           NumeroFuncionarios = a.TB_DIVISAO_QTDE_FUNC,
                                           IndicadorAtividade = (bool)a.TB_DIVISAO_INDICADOR_ATIVIDADE,
                                           Ua = new UAEntity() { 
                                                                 Id = uaDivisao.TB_UA_ID,
                                                                 Codigo = uaDivisao.TB_UA_CODIGO,
                                                                 Descricao = uaDivisao.TB_UA_DESCRICAO,
                                                                 Uge = new UGEEntity()
                                                                                        {
                                                                                            Id = ugeUaDivisao.TB_UGE_ID,
                                                                                            Codigo = ugeUaDivisao.TB_UGE_CODIGO,
                                                                                            Descricao = ugeUaDivisao.TB_UGE_DESCRICAO,
                                                                                            Uo = new UOEntity()
                                                                                                                { 
                                                                                                                    Id = uoUgeUaDivisao.TB_UO_ID,
                                                                                                                    Codigo = uoUgeUaDivisao.TB_UO_CODIGO,
                                                                                                                    Descricao = uoUgeUaDivisao.TB_UO_DESCRICAO
                                                                                                                }
                                                                                        }
                                                               }
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
                                                  Area = a.TB_DIVISAO_AREA ?? 0,
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
            if (this.Entity.Responsavel.Id.HasValue)
                tbDivisao.TB_RESPONSAVEL_ID = this.Entity.Responsavel.Id.Value;
            else 
                tbDivisao.TB_RESPONSAVEL_ID = null;
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
                                Descricao = a.TB_UA.TB_UGE.TB_UGE_DESCRICAO,
                                Uo = new UOEntity 
                                    {
                                        Id = a.TB_UA.TB_UGE.TB_UO.TB_UO_ID,
                                        Codigo = a.TB_UA.TB_UGE.TB_UO.TB_UO_CODIGO,
                                        CodigoFormatado = a.TB_UA.TB_UGE.TB_UO.TB_UO_CODIGO.ToString().PadLeft(6, '0'),
                                        Descricao = a.TB_UA.TB_UGE.TB_UO.TB_UO_DESCRICAO,
                                    }
                            }
                        }

                    });
            
            if(this.Entity.Ua.Uge != null && this.Entity.Ua.Uge.Id != 0)
                retorno = retorno.Where(a => a.Ua.Uge.Id == this.Entity.Ua.Uge.Id);

            if (this.Entity.Ua.Uge.Uo != null && this.Entity.Ua.Uge.Uo.Id != 0)
                retorno = retorno.Where(a => a.Ua.Uge.Uo.Id == this.Entity.Ua.Uge.Uo.Id);

            return retorno.ToList<DivisaoEntity>();
        }



        public IList<DivisaoEntity> Listar(int UoId, Int64 UgeId = default(Int64))
        {
            IQueryable<DivisaoEntity> retorno = (from a    in Db.TB_DIVISAOs
                                                 join _ua  in Db.TB_UAs  on a.TB_UA_ID    equals _ua.TB_UA_ID
                                                 join _uge in Db.TB_UGEs on _ua.TB_UGE_ID equals _uge.TB_UGE_ID
                                                 join _uo  in Db.TB_UOs  on _uge.TB_UO_ID equals _uo.TB_UO_ID
                                                 where    _uo.TB_UO_ID == UoId
                                                       && (    (_uge.TB_UGE_ID == UgeId && UgeId > 0)
                                                            || (UgeId == 0))
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
                                                     Ua = (from _uaA in Db.TB_UAs
                                                           where _uaA.TB_UA_ID == _ua.TB_UA_ID
                                                           select new UAEntity
                                                         {
                                                             Id = a.TB_UA.TB_UA_ID,
                                                             Codigo = a.TB_UA.TB_UA_CODIGO,
                                                             CodigoFormatado = a.TB_UA.TB_UA_CODIGO.ToString().PadLeft(7, '0'),
                                                             Descricao = a.TB_UA.TB_UA_DESCRICAO/*,
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
                                                             }*/
                                                         }).FirstOrDefault<UAEntity>()
                                                 });

            this.totalregistros = retorno.Count();

            return retorno.ToList<DivisaoEntity>();
        }


        public IList<DivisaoEntity> ListarTodosByExpression(Expression<Func<DivisaoEntity, bool>> _where)
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
                                                             Descricao = a.TB_UA.TB_UGE.TB_UGE_DESCRICAO,
                                                             Uo = new UOEntity
                                                             {
                                                                 Id = a.TB_UA.TB_UGE.TB_UO.TB_UO_ID,
                                                                 Codigo = a.TB_UA.TB_UGE.TB_UO.TB_UO_CODIGO,
                                                                 CodigoFormatado = a.TB_UA.TB_UGE.TB_UO.TB_UO_CODIGO.ToString().PadLeft(6, '0'),
                                                                 Descricao = a.TB_UA.TB_UGE.TB_UO.TB_UO_DESCRICAO,
                                                             }
                                                         }
                                                     }

                                                 });            

            return retorno.Where(_where).ToList<DivisaoEntity>();
        }

        public DivisaoEntity ObterDivisaoUA(int codigoUA, int codigoDivisaoUA)
        {
            DivisaoEntity objEntidade = null;
            Expression<Func<TB_DIVISAO, bool>> expWhere = null;
            IQueryable<TB_DIVISAO> qryConsulta = null;



            expWhere = (divisaoConsultada => divisaoConsultada.TB_UA.TB_UA_CODIGO == codigoUA
                                          && divisaoConsultada.TB_DIVISAO_CODIGO == codigoDivisaoUA);

            qryConsulta = Db.TB_DIVISAOs.AsQueryable();
            objEntidade = qryConsulta.Where(expWhere)
                                     .Select(_instanciadorDTODivisao())
                                     .FirstOrDefault();


            return objEntidade;
        }


        private Func<TB_DIVISAO, DivisaoEntity> _instanciadorDTODivisao()
        {
            Func<TB_DIVISAO, DivisaoEntity> _actionSeletor = null;

            _actionSeletor = rowTabela => new DivisaoEntity
                                                            {
                                                                Id = rowTabela.TB_DIVISAO_ID,
                                                                Codigo = rowTabela.TB_DIVISAO_CODIGO,
                                                                CodigoDescricao = String.Format("{0:D7} - {1}", rowTabela.TB_DIVISAO_CODIGO, rowTabela.TB_DIVISAO_DESCRICAO),
                                                                Descricao = rowTabela.TB_DIVISAO_DESCRICAO,
                                                                EnderecoLogradouro = rowTabela.TB_DIVISAO_LOGRADOURO,
                                                                Area = rowTabela.TB_DIVISAO_AREA,
                                                                EnderecoNumero = rowTabela.TB_DIVISAO_NUMERO,
                                                                EnderecoCompl = rowTabela.TB_DIVISAO_COMPLEMENTO,
                                                                EnderecoBairro = rowTabela.TB_DIVISAO_BAIRRO,
                                                                EnderecoMunicipio = rowTabela.TB_DIVISAO_MUNICIPIO,
                                                                EnderecoCep = rowTabela.TB_DIVISAO_CEP,
                                                                EnderecoTelefone = rowTabela.TB_DIVISAO_TELEFONE,
                                                                EnderecoFax = rowTabela.TB_DIVISAO_FAX,
                                                                IndicadorAtividade = rowTabela.TB_DIVISAO_INDICADOR_ATIVIDADE,
                                                                NumeroFuncionarios = rowTabela.TB_DIVISAO_QTDE_FUNC,

                                                                Responsavel = ((rowTabela.TB_RESPONSAVEL_ID.IsNotNull())? (new ResponsavelEntity() { Id = rowTabela.TB_RESPONSAVEL.TB_RESPONSAVEL_ID, Descricao = rowTabela.TB_RESPONSAVEL.TB_RESPONSAVEL_NOME }) : null),
                                                                Ua = ((rowTabela.TB_UA_ID != 0)? (new UAEntity(rowTabela.TB_UA_ID) { Codigo = rowTabela.TB_UA.TB_UA_CODIGO, CodigoDescricao = String.Format("{0:D7} - {1}", rowTabela.TB_UA.TB_UA_CODIGO, rowTabela.TB_UA.TB_UA_DESCRICAO) }) : null),
                                                                Almoxarifado = ((rowTabela.TB_ALMOXARIFADO_ID.IsNotNull())? (new AlmoxarifadoEntity() { Id = rowTabela.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID, Codigo = rowTabela.TB_ALMOXARIFADO.TB_ALMOXARIFADO_CODIGO, CodigoDescricao = String.Format("{0:D3} - {1}", rowTabela.TB_ALMOXARIFADO.TB_ALMOXARIFADO_CODIGO, rowTabela.TB_ALMOXARIFADO.TB_ALMOXARIFADO_DESCRICAO), MesRef = rowTabela.TB_ALMOXARIFADO.TB_ALMOXARIFADO_MES_REF, Uge = ((rowTabela.TB_ALMOXARIFADO.TB_UGE_ID.IsNotNull()) ? (new UGEEntity() { Id = rowTabela.TB_ALMOXARIFADO.TB_UGE.TB_UGE_ID, Codigo = rowTabela.TB_ALMOXARIFADO.TB_UGE.TB_UGE_CODIGO, Descricao = rowTabela.TB_ALMOXARIFADO.TB_UGE.TB_UGE_DESCRICAO }) : null) }) : null),
                                                                Uf =  ((rowTabela.TB_UF_ID.IsNotNull()) ? (new UFEntity() { Id = rowTabela.TB_UF.TB_UF_ID, Descricao = rowTabela.TB_UF.TB_UF_DESCRICAO, Sigla = rowTabela.TB_UF.TB_UF_SIGLA }) : null),
                                                            };

            return _actionSeletor;
        }


    }
}
