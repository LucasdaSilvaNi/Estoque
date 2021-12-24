using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;
using System.Configuration;
using Sam.Common.Util;
using System.Collections;
using System.Linq.Expressions;
using System.Xml.Linq;


namespace Sam.Domain.Infrastructure
{
    public class AlmoxarifadoInfraestructure : BaseInfraestructure, IAlmoxarifadoService
    {
        private AlmoxarifadoEntity almoxarifado = new AlmoxarifadoEntity();

        public AlmoxarifadoEntity Entity
        {
            get { return almoxarifado; }
            set { almoxarifado = value; }
        }

        public IList<AlmoxarifadoEntity> Listar()
        {
            IList<AlmoxarifadoEntity> resultado = (from a in Db.TB_ALMOXARIFADOs
                                                   orderby (a.TB_ALMOXARIFADO_CODIGO)
                                                   select new AlmoxarifadoEntity
                                                   {
                                                       Id = a.TB_ALMOXARIFADO_ID,
                                                       Codigo = a.TB_ALMOXARIFADO_CODIGO,
                                                       Descricao = a.TB_ALMOXARIFADO_DESCRICAO,
                                                       EnderecoLogradouro = a.TB_ALMOXARIFADO_LOGRADOURO,
                                                       EnderecoNumero = a.TB_ALMOXARIFADO_NUMERO,
                                                       EnderecoCompl = a.TB_ALMOXARIFADO_COMPLEMENTO,
                                                       EnderecoBairro = a.TB_ALMOXARIFADO_BAIRRO,
                                                       EnderecoMunicipio = a.TB_ALMOXARIFADO_MUNICIPIO,
                                                       Uf = (new UFEntity(a.TB_UF_ID)),
                                                       EnderecoCep = a.TB_ALMOXARIFADO_CEP,
                                                       EnderecoTelefone = a.TB_ALMOXARIFADO_TELEFONE,
                                                       EnderecoFax = a.TB_ALMOXARIFADO_FAX,
                                                       Responsavel = a.TB_ALMOXARIFADO_RESPONSAVEL,
                                                       Uge = (new UGEEntity(a.TB_UGE_ID)),
                                                       //Orgao = (new OrgaoEntity(a.TB_ORGAO_ID)),
                                                       Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
                                                       RefInicial = a.TB_ALMOXARIFADO_MES_REF_INICIAL,
                                                       MesRef = a.TB_ALMOXARIFADO_MES_REF,
                                                       IndicadorAtividade = (new IndicadorAtividadeEntity(Convert.ToInt32(a.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE))),
                                                       //Ordem = a.ORDEM                                                  
                                                       IgnoraCalendarioSiafemParaReabertura = a.TB_ALMOXARIFADO_IGNORA_CALENDARIO_SIAFEM_PARA_REABERTURA
                                                   })
                                             .Skip(this.SkipRegistros)
                                             .Take(this.RegistrosPagina)
                                             .ToList<AlmoxarifadoEntity>();

            this.totalregistros = (from a in Db.TB_ALMOXARIFADOs
                                   select new
                                   {
                                       a.TB_ALMOXARIFADO_ID,
                                   }).Count();
            return resultado;
        }

        public IList<AlmoxarifadoEntity> ListarAlmoxarifadosNivelAcesso(int idGestor, List<AlmoxarifadoEntity> almoxarifadosNivelAcesso)
        {
            var result = (from a in Db.TB_ALMOXARIFADOs
                          where a.TB_GESTOR_ID == idGestor || idGestor == null || idGestor == 0// Gestor Id Opcional
                          select new AlmoxarifadoEntity
                          {
                              Id = a.TB_ALMOXARIFADO_ID,
                              Descricao = a.TB_ALMOXARIFADO_DESCRICAO
                          }).ToList();

            List<AlmoxarifadoEntity> list = new List<AlmoxarifadoEntity>();
            foreach (var r in result)
            {
                foreach (var a in almoxarifadosNivelAcesso)
                {
                    if (r.Id == a.Id)
                    {
                        list.Add(r);
                    }
                }
            }

            return list;
        }

        public IList<AlmoxarifadoEntity> ListarCodigo(int OrgaoId, int GestorId, string AlmoxCodigo)
        {

            var almox = AlmoxCodigo.Split(',').Select(Int32.Parse).ToList();


            IList<AlmoxarifadoEntity> resultado = (from a in Db.TB_ALMOXARIFADOs
                                                   join b in Db.TB_UFs on a.TB_UF_ID equals b.TB_UF_ID
                                                   //where (a.TB_ORGAO_ID == OrgaoId)
                                                   where (a.TB_GESTOR_ID == GestorId)
                                                   where almox.Contains(a.TB_ALMOXARIFADO_CODIGO)
                                                   orderby (a.TB_ALMOXARIFADO_CODIGO)
                                                   select new AlmoxarifadoEntity
                                                   {
                                                       Id = a.TB_ALMOXARIFADO_ID,
                                                       Codigo = a.TB_ALMOXARIFADO_CODIGO,
                                                       Descricao = a.TB_ALMOXARIFADO_DESCRICAO,
                                                       EnderecoLogradouro = a.TB_ALMOXARIFADO_LOGRADOURO,
                                                       EnderecoNumero = a.TB_ALMOXARIFADO_NUMERO,
                                                       EnderecoCompl = a.TB_ALMOXARIFADO_COMPLEMENTO,
                                                       EnderecoBairro = a.TB_ALMOXARIFADO_BAIRRO,
                                                       EnderecoMunicipio = a.TB_ALMOXARIFADO_MUNICIPIO,
                                                       Uf = (new UFEntity { Id = b.TB_UF_ID, Descricao = b.TB_UF_DESCRICAO, Sigla = b.TB_UF_SIGLA }),
                                                       EnderecoCep = a.TB_ALMOXARIFADO_CEP,
                                                       EnderecoTelefone = a.TB_ALMOXARIFADO_TELEFONE,
                                                       EnderecoFax = a.TB_ALMOXARIFADO_FAX,
                                                       Responsavel = a.TB_ALMOXARIFADO_RESPONSAVEL,
                                                       Uge = (new UGEEntity(a.TB_UGE_ID)),
                                                       //  Orgao = (new OrgaoEntity(a.TB_ORGAO_ID)),
                                                       //Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
                                                       RefInicial = a.TB_ALMOXARIFADO_MES_REF_INICIAL,
                                                       RefFaturamento = a.TB_ALMOXARIFADO_MES_REF_FATURAMENTO,
                                                       TipoAlmoxarifado = a.TB_ALMOXARIFADO_TIPO,
                                                       IndicAtividade = a.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE,
                                                       IndicadorAtividade = (new IndicadorAtividadeEntity(Convert.ToInt32(a.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE))),
                                                       //Ordem = a.ORDEM
                                                       IgnoraCalendarioSiafemParaReabertura = a.TB_ALMOXARIFADO_IGNORA_CALENDARIO_SIAFEM_PARA_REABERTURA
                                                   })
                                              .Skip(this.SkipRegistros)
                                              .Take(this.RegistrosPagina)
                                              .ToList<AlmoxarifadoEntity>();

            this.totalregistros = (from a in Db.TB_ALMOXARIFADOs
                                       //where (a.TB_ORGAO_ID == OrgaoId)
                                   where (a.TB_GESTOR_ID == GestorId)
                                   where almox.Contains(a.TB_ALMOXARIFADO_CODIGO)
                                   select new
                                   {
                                       a.TB_ALMOXARIFADO_ID,
                                   }).Count();
            return resultado;
        }

        public IList<AlmoxarifadoEntity> Listar(int OrgaoId, int GestorId)
        {

            IList<AlmoxarifadoEntity> resultado = (from a in Db.TB_ALMOXARIFADOs
                                                   join b in Db.TB_UFs on a.TB_UF_ID equals b.TB_UF_ID
                                                   //where (a.TB_ORGAO_ID == OrgaoId)
                                                   where (a.TB_GESTOR_ID == GestorId)
                                                   orderby (a.TB_ALMOXARIFADO_CODIGO)
                                                   select new AlmoxarifadoEntity
                                                   {
                                                       Id = a.TB_ALMOXARIFADO_ID,
                                                       Codigo = a.TB_ALMOXARIFADO_CODIGO,
                                                       Descricao = a.TB_ALMOXARIFADO_DESCRICAO,
                                                       EnderecoLogradouro = a.TB_ALMOXARIFADO_LOGRADOURO,
                                                       EnderecoNumero = a.TB_ALMOXARIFADO_NUMERO,
                                                       EnderecoCompl = a.TB_ALMOXARIFADO_COMPLEMENTO,
                                                       EnderecoBairro = a.TB_ALMOXARIFADO_BAIRRO,
                                                       EnderecoMunicipio = a.TB_ALMOXARIFADO_MUNICIPIO,
                                                       Uf = (new UFEntity { Id = b.TB_UF_ID, Descricao = b.TB_UF_DESCRICAO, Sigla = b.TB_UF_SIGLA }),
                                                       EnderecoCep = a.TB_ALMOXARIFADO_CEP,
                                                       EnderecoTelefone = a.TB_ALMOXARIFADO_TELEFONE,
                                                       EnderecoFax = a.TB_ALMOXARIFADO_FAX,
                                                       Responsavel = a.TB_ALMOXARIFADO_RESPONSAVEL,
                                                       Uge = (new UGEEntity(a.TB_UGE_ID)),
                                                       //  Orgao = (new OrgaoEntity(a.TB_ORGAO_ID)),
                                                       //Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
                                                       RefInicial = a.TB_ALMOXARIFADO_MES_REF_INICIAL,
                                                       RefFaturamento = a.TB_ALMOXARIFADO_MES_REF_FATURAMENTO,
                                                       TipoAlmoxarifado = a.TB_ALMOXARIFADO_TIPO,
                                                       IndicadorAtividade = (new IndicadorAtividadeEntity(Convert.ToInt32(a.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE))),
                                                       //Ordem = a.ORDEM
                                                       IgnoraCalendarioSiafemParaReabertura = a.TB_ALMOXARIFADO_IGNORA_CALENDARIO_SIAFEM_PARA_REABERTURA
                                                   })
                                              .Skip(this.SkipRegistros)
                                              .Take(this.RegistrosPagina)
                                              .ToList<AlmoxarifadoEntity>();

            this.totalregistros = (from a in Db.TB_ALMOXARIFADOs
                                       //where (a.TB_ORGAO_ID == OrgaoId)
                                   where (a.TB_GESTOR_ID == GestorId)
                                   select new
                                   {
                                       a.TB_ALMOXARIFADO_ID,
                                   }).Count();
            return resultado;
        }

        public IList<AlmoxarifadoEntity> ListarSelecionaAlmoxarifado(int OrgaoId, int GestorId, int AlmoxarifadoId)
        {

            IList<AlmoxarifadoEntity> resultado = (from a in Db.TB_ALMOXARIFADOs
                                                   join b in Db.TB_UFs on a.TB_UF_ID equals b.TB_UF_ID
                                                   where (a.TB_GESTOR_ID == GestorId && a.TB_ALMOXARIFADO_ID != AlmoxarifadoId)
                                                   orderby (a.TB_ALMOXARIFADO_CODIGO)
                                                   select new AlmoxarifadoEntity
                                                   {
                                                       Id = a.TB_ALMOXARIFADO_ID,
                                                       Codigo = a.TB_ALMOXARIFADO_CODIGO,
                                                       Descricao = a.TB_ALMOXARIFADO_DESCRICAO,
                                                       EnderecoLogradouro = a.TB_ALMOXARIFADO_LOGRADOURO,
                                                       EnderecoNumero = a.TB_ALMOXARIFADO_NUMERO,
                                                       EnderecoCompl = a.TB_ALMOXARIFADO_COMPLEMENTO,
                                                       EnderecoBairro = a.TB_ALMOXARIFADO_BAIRRO,
                                                       EnderecoMunicipio = a.TB_ALMOXARIFADO_MUNICIPIO,
                                                       Uf = (new UFEntity { Id = b.TB_UF_ID, Descricao = b.TB_UF_DESCRICAO, Sigla = b.TB_UF_SIGLA }),
                                                       EnderecoCep = a.TB_ALMOXARIFADO_CEP,
                                                       EnderecoTelefone = a.TB_ALMOXARIFADO_TELEFONE,
                                                       EnderecoFax = a.TB_ALMOXARIFADO_FAX,
                                                       Responsavel = a.TB_ALMOXARIFADO_RESPONSAVEL,
                                                       Uge = (new UGEEntity(a.TB_UGE_ID)),
                                                       RefInicial = a.TB_ALMOXARIFADO_MES_REF_INICIAL,
                                                       IndicadorAtividade = (new IndicadorAtividadeEntity(Convert.ToInt32(a.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE))),
                                                       IgnoraCalendarioSiafemParaReabertura = a.TB_ALMOXARIFADO_IGNORA_CALENDARIO_SIAFEM_PARA_REABERTURA
                                                   })
                                              .ToList<AlmoxarifadoEntity>();
            return resultado;
        }

        public IList<AlmoxarifadoEntity> ListarSelecionaAlmoxarifadoTake(int OrgaoId, int GestorId, int AlmoxarifadoId)
        {

            IList<AlmoxarifadoEntity> resultado = (from a in Db.TB_ALMOXARIFADOs
                                                   join b in Db.TB_UFs on a.TB_UF_ID equals b.TB_UF_ID
                                                   where (a.TB_GESTOR_ID == GestorId && a.TB_ALMOXARIFADO_ID != AlmoxarifadoId)
                                                   orderby (a.TB_ALMOXARIFADO_CODIGO)
                                                   select new AlmoxarifadoEntity
                                                   {
                                                       Id = a.TB_ALMOXARIFADO_ID,
                                                       Codigo = a.TB_ALMOXARIFADO_CODIGO,
                                                       Descricao = a.TB_ALMOXARIFADO_DESCRICAO,
                                                       EnderecoLogradouro = a.TB_ALMOXARIFADO_LOGRADOURO,
                                                       EnderecoNumero = a.TB_ALMOXARIFADO_NUMERO,
                                                       EnderecoCompl = a.TB_ALMOXARIFADO_COMPLEMENTO,
                                                       EnderecoBairro = a.TB_ALMOXARIFADO_BAIRRO,
                                                       EnderecoMunicipio = a.TB_ALMOXARIFADO_MUNICIPIO,
                                                       Uf = (new UFEntity { Id = b.TB_UF_ID, Descricao = b.TB_UF_DESCRICAO, Sigla = b.TB_UF_SIGLA }),
                                                       EnderecoCep = a.TB_ALMOXARIFADO_CEP,
                                                       EnderecoTelefone = a.TB_ALMOXARIFADO_TELEFONE,
                                                       EnderecoFax = a.TB_ALMOXARIFADO_FAX,
                                                       Responsavel = a.TB_ALMOXARIFADO_RESPONSAVEL,
                                                       Uge = (new UGEEntity(a.TB_UGE_ID)),
                                                       //  Orgao = (new OrgaoEntity(a.TB_ORGAO_ID)),
                                                       //Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
                                                       RefInicial = a.TB_ALMOXARIFADO_MES_REF_INICIAL,
                                                       //IndicadorAtividade = (new IndicadorAtividadeEntity(Convert.ToInt32(a.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE))),
                                                       //Ordem = a.ORDEM
                                                       //SubAlmoxarifado = (a.TB_ALMOXARIFADO_TIPO_ALMOXARIFADO == 1),
                                                       // AlmoxarifadoVinculadoId = ((a.TB_ALMOXARIFADO_VINCULADO_ID != null) ? a.TB_ALMOXARIFADO_VINCULADO_ID : 0),
                                                       //CCCentroCusto = a.TB_ALMOXARIFADO_CC_CENTRO_CUSTO
                                                       IgnoraCalendarioSiafemParaReabertura = a.TB_ALMOXARIFADO_IGNORA_CALENDARIO_SIAFEM_PARA_REABERTURA
                                                   })
                                              .Skip(this.SkipRegistros)
                                              .Take(this.RegistrosPagina)
                                              .ToList<AlmoxarifadoEntity>();

            this.totalregistros = (from a in Db.TB_ALMOXARIFADOs
                                   where (a.TB_ALMOXARIFADO_ID != AlmoxarifadoId)
                                   where (a.TB_GESTOR_ID == GestorId)
                                   select new
                                   {
                                       a.TB_ALMOXARIFADO_ID,
                                   }).Count();
            return resultado;
        }

        public IList<AlmoxarifadoEntity> ListarAlmoxarifadoPorGestorTodosCod(int GestorId)
        {
            IList<AlmoxarifadoEntity> resultado = (from a in Db.TB_ALMOXARIFADOs
                                                   join b in Db.TB_UFs on a.TB_UF_ID equals b.TB_UF_ID
                                                   where (a.TB_GESTOR_ID == GestorId)
                                                   orderby (a.TB_ALMOXARIFADO_CODIGO)
                                                   select new AlmoxarifadoEntity
                                                   {
                                                       Id = a.TB_ALMOXARIFADO_ID,
                                                       Codigo = a.TB_ALMOXARIFADO_CODIGO,
                                                       Descricao = string.Format("{0} - {1}", a.TB_ALMOXARIFADO_CODIGO.ToString().PadLeft(6, '0'), a.TB_ALMOXARIFADO_DESCRICAO),
                                                       EnderecoLogradouro = a.TB_ALMOXARIFADO_LOGRADOURO,
                                                       EnderecoNumero = a.TB_ALMOXARIFADO_NUMERO,
                                                       EnderecoCompl = a.TB_ALMOXARIFADO_COMPLEMENTO,
                                                       EnderecoBairro = a.TB_ALMOXARIFADO_BAIRRO,
                                                       EnderecoMunicipio = a.TB_ALMOXARIFADO_MUNICIPIO,
                                                       Uf = (new UFEntity { Id = b.TB_UF_ID, Descricao = b.TB_UF_DESCRICAO, Sigla = b.TB_UF_SIGLA }),
                                                       EnderecoCep = a.TB_ALMOXARIFADO_CEP,
                                                       EnderecoTelefone = a.TB_ALMOXARIFADO_TELEFONE,
                                                       EnderecoFax = a.TB_ALMOXARIFADO_FAX,
                                                       Responsavel = a.TB_ALMOXARIFADO_RESPONSAVEL,
                                                       Uge = (new UGEEntity(a.TB_UGE_ID)),
                                                       //  Orgao = (new OrgaoEntity(a.TB_ORGAO_ID)),
                                                       Orgao = (new OrgaoEntity(a.TB_GESTOR.TB_ORGAO_ID)),
                                                       Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
                                                       RefInicial = a.TB_ALMOXARIFADO_MES_REF_INICIAL,
                                                       // IndicadorAtividade = (new IndicadorAtividadeEntity(Convert.ToInt32(a.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE))),
                                                       //Ordem = a.ORDEM
                                                       //SubAlmoxarifado = (a.TB_ALMOXARIFADO_TIPO_ALMOXARIFADO == 1),
                                                       //AlmoxarifadoVinculadoId = ((a.TB_ALMOXARIFADO_VINCULADO_ID != null) ? a.TB_ALMOXARIFADO_VINCULADO_ID : 0),
                                                       //CCCentroCusto = a.TB_ALMOXARIFADO_CC_CENTRO_CUSTO
                                                       IgnoraCalendarioSiafemParaReabertura = a.TB_ALMOXARIFADO_IGNORA_CALENDARIO_SIAFEM_PARA_REABERTURA,
                                                   })
                                              .ToList<AlmoxarifadoEntity>();
            return resultado;
        }

        /// <summary>
        /// Sobrecarga criada para casos em que precisa-se de todos os campos do objeto preenchidos
        /// </summary>
        /// <param name="GestorId"></param>
        /// <param name="pBlnPreencherObjeto"></param>
        /// <returns></returns>
        public IList<AlmoxarifadoEntity> ListarAlmoxarifadoPorGestorTodosCod(int GestorId, bool pBlnPreencherObjetos)
        {
            List<AlmoxarifadoEntity> lLstRetorno = null;

            string lStrSqlTracing = string.Empty;
            IQueryable lQryRetorno = (from Almoxarifado in Db.TB_ALMOXARIFADOs
                                      join UF in Db.TB_UFs on Almoxarifado.TB_UF_ID equals UF.TB_UF_ID
                                      where (Almoxarifado.TB_GESTOR_ID == GestorId)
                                      orderby (Almoxarifado.TB_ALMOXARIFADO_CODIGO)
                                      select new AlmoxarifadoEntity
                                      {
                                          Id = Almoxarifado.TB_ALMOXARIFADO_ID,
                                          Codigo = Almoxarifado.TB_ALMOXARIFADO_CODIGO,
                                          Descricao = string.Format("{0} - {1}", Almoxarifado.TB_ALMOXARIFADO_CODIGO.ToString().PadLeft(6, '0'), Almoxarifado.TB_ALMOXARIFADO_DESCRICAO),
                                          EnderecoLogradouro = Almoxarifado.TB_ALMOXARIFADO_LOGRADOURO,
                                          EnderecoNumero = Almoxarifado.TB_ALMOXARIFADO_NUMERO,
                                          EnderecoCompl = Almoxarifado.TB_ALMOXARIFADO_COMPLEMENTO,
                                          EnderecoBairro = Almoxarifado.TB_ALMOXARIFADO_BAIRRO,
                                          EnderecoMunicipio = Almoxarifado.TB_ALMOXARIFADO_MUNICIPIO,
                                          RefInicial = Almoxarifado.TB_ALMOXARIFADO_MES_REF_INICIAL,
                                          MesRef = Almoxarifado.TB_ALMOXARIFADO_MES_REF,
                                          EnderecoCep = Almoxarifado.TB_ALMOXARIFADO_CEP,
                                          EnderecoTelefone = Almoxarifado.TB_ALMOXARIFADO_TELEFONE,
                                          EnderecoFax = Almoxarifado.TB_ALMOXARIFADO_FAX,
                                          Responsavel = Almoxarifado.TB_ALMOXARIFADO_RESPONSAVEL,
                                          IndicadorAtividade = new IndicadorAtividadeEntity(Convert.ToInt32(Almoxarifado.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE)),

                                          Uf = new UFEntity() { Id = UF.TB_UF_ID, Descricao = UF.TB_UF_DESCRICAO, Sigla = UF.TB_UF_SIGLA },
                                          Uge = new UGEEntity() { Id = Almoxarifado.TB_UGE.TB_UGE_ID, Codigo = Almoxarifado.TB_UGE.TB_UGE_CODIGO, Descricao = Almoxarifado.TB_UGE.TB_UGE_DESCRICAO },
                                          Orgao = new OrgaoEntity() { Id = Almoxarifado.TB_GESTOR.TB_ORGAO_ID, Codigo = Almoxarifado.TB_GESTOR.TB_ORGAO.TB_ORGAO_CODIGO, Descricao = Almoxarifado.TB_GESTOR.TB_GESTOR_NOME },
                                          Gestor = new GestorEntity() { Id = Almoxarifado.TB_GESTOR_ID, CodigoGestao = Almoxarifado.TB_GESTOR.TB_GESTOR_CODIGO_GESTAO, Nome = Almoxarifado.TB_GESTOR.TB_GESTOR_NOME },
                                          IgnoraCalendarioSiafemParaReabertura = Almoxarifado.TB_ALMOXARIFADO_IGNORA_CALENDARIO_SIAFEM_PARA_REABERTURA,
                                      });


            lStrSqlTracing = lQryRetorno.ToString();
            lLstRetorno = lQryRetorno.Cast<AlmoxarifadoEntity>().ToList();

            return lLstRetorno;
        }

        public AlmoxarifadoEntity SelecionarAlmoxarifadoPorGestor(int AlmoxarifadoId)
        {

            var result = (from almox in Db.TB_ALMOXARIFADOs
                          where almox.TB_ALMOXARIFADO_ID == AlmoxarifadoId
                          select new AlmoxarifadoEntity
                          {
                              Id = almox.TB_ALMOXARIFADO_ID,
                              Descricao = almox.TB_ALMOXARIFADO_DESCRICAO,
                              RefInicial = almox.TB_ALMOXARIFADO_MES_REF_INICIAL,
                              MesRef = almox.TB_ALMOXARIFADO_MES_REF,
                              Uge = (from u in Db.TB_UGEs
                                     where almox.TB_UGE_ID == u.TB_UGE_ID || almox.TB_UGE_ID == null
                                     select new UGEEntity
                                     {
                                         Id = almox.TB_UGE_ID,
                                         Codigo = almox.TB_UGE.TB_UGE_CODIGO,
                                         Descricao = almox.TB_UGE.TB_UGE_DESCRICAO

                                     }).FirstOrDefault(),
                              Gestor = (from g in Db.TB_GESTORs
                                        where almox.TB_GESTOR_ID == g.TB_GESTOR_ID || g.TB_GESTOR_ID == null
                                        select new GestorEntity
                                        {
                                            Id = almox.TB_GESTOR.TB_GESTOR_ID,
                                            CodigoGestao = almox.TB_GESTOR.TB_GESTOR_CODIGO_GESTAO,
                                            Nome = almox.TB_GESTOR.TB_GESTOR_NOME,
                                            NomeReduzido = almox.TB_GESTOR.TB_GESTOR_NOME_REDUZIDO,
                                            Orgao = new OrgaoEntity()
                                            {
                                                Id = almox.TB_GESTOR.TB_ORGAO.TB_ORGAO_ID,
                                                Codigo = almox.TB_GESTOR.TB_ORGAO.TB_ORGAO_CODIGO,
                                                Descricao = almox.TB_GESTOR.TB_ORGAO.TB_ORGAO_DESCRICAO
                                            }
                                        }).FirstOrDefault(),
                              //SubAlmoxarifado = (almox.TB_ALMOXARIFADO_TIPO_ALMOXARIFADO == 1),
                              //AlmoxarifadoVinculadoId = ((almox.TB_ALMOXARIFADO_VINCULADO_ID != null) ? almox.TB_ALMOXARIFADO_VINCULADO_ID : 0),
                              //CCCentroCusto = almox.TB_ALMOXARIFADO_CC_CENTRO_CUSTO
                              IgnoraCalendarioSiafemParaReabertura = almox.TB_ALMOXARIFADO_IGNORA_CALENDARIO_SIAFEM_PARA_REABERTURA
                          }).FirstOrDefault();

            return result;
        }

        public IList<AlmoxarifadoEntity> ListarAlmoxarifadoPorOrgaoTodosCod(int orgaoId)
        {
            IList<AlmoxarifadoEntity> resultado = (from a in Db.TB_ALMOXARIFADOs
                                                   join b in Db.TB_GESTORs on a.TB_GESTOR_ID equals b.TB_GESTOR_ID
                                                   where (b.TB_ORGAO_ID == orgaoId)
                                                   orderby a.TB_ALMOXARIFADO_CODIGO ascending
                                                   select new AlmoxarifadoEntity
                                                   {
                                                       Id = a.TB_ALMOXARIFADO_ID,
                                                       Codigo = a.TB_ALMOXARIFADO_CODIGO,
                                                       Descricao = string.Format("{0} - {1}", a.TB_ALMOXARIFADO_CODIGO.ToString().PadLeft(6, '0'), a.TB_ALMOXARIFADO_DESCRICAO),
                                                   })
                                              .ToList<AlmoxarifadoEntity>();
            return resultado;
        }

        public IList<AlmoxarifadoEntity> ListarAlmoxarifadoPorOrgaoMesRef(int orgaoId, string AnoMesReferencia)
        {         
            IList<AlmoxarifadoEntity> resultado = (from a in Db.TB_ALMOXARIFADOs
                                                   join b in Db.TB_GESTORs on a.TB_GESTOR_ID equals b.TB_GESTOR_ID  
                                                   join u in Db.TB_UGEs on a.TB_UGE_ID equals u.TB_UGE_ID                                                
                                                   where (b.TB_ORGAO_ID == orgaoId) && a.TB_ALMOXARIFADO_MES_REF== AnoMesReferencia
                                                        && u.TB_UGE_IMPLANTADO==true && a.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE==true
                                                                && u.TB_UGE_STATUS==true
                                                   orderby (a.TB_ALMOXARIFADO_CODIGO)
                                                   select new AlmoxarifadoEntity
                                                   {
                                                       Id = a.TB_ALMOXARIFADO_ID,
                                                       Codigo = a.TB_ALMOXARIFADO_CODIGO,
                                                       Descricao = string.Format("{0} - {1}", a.TB_ALMOXARIFADO_CODIGO.ToString().PadLeft(6, '0'), a.TB_ALMOXARIFADO_DESCRICAO.ToUpper()),
                                                   })
                                              .Distinct().ToList<AlmoxarifadoEntity>();
            return resultado;
        }

        public IList<AlmoxarifadoEntity> ListarAlmoxarifadoPorOrgaoGestor(int orgaoId, int GestorId)
        {
            IList<AlmoxarifadoEntity> resultado = (from a in Db.TB_ALMOXARIFADOs
                                                   where (a.TB_GESTOR.TB_GESTOR_ID == GestorId)
                                                   orderby (a.TB_ALMOXARIFADO_CODIGO)
                                                   select new AlmoxarifadoEntity
                                                   {
                                                       Id = a.TB_ALMOXARIFADO_ID,
                                                       Codigo = a.TB_ALMOXARIFADO_CODIGO,
                                                       Descricao = string.Format("{0} - {1}", a.TB_ALMOXARIFADO_CODIGO.ToString().PadLeft(6, '0'), a.TB_ALMOXARIFADO_DESCRICAO),
                                                   })
                                              .ToList<AlmoxarifadoEntity>();
            return resultado;
        }

        public IList<AlmoxarifadoEntity> Imprimir(int OrgaoId, int GestorId)
        {
            IList<AlmoxarifadoEntity> resultado = (from a in Db.TB_ALMOXARIFADOs
                                                   join b in Db.TB_UFs on a.TB_UF_ID equals b.TB_UF_ID
                                                   //where (a.TB_ORGAO_ID == OrgaoId)
                                                   where (a.TB_GESTOR_ID == GestorId)
                                                   orderby (a.TB_ALMOXARIFADO_ID)
                                                   select new AlmoxarifadoEntity
                                                   {
                                                       Id = a.TB_ALMOXARIFADO_ID,
                                                       Codigo = a.TB_ALMOXARIFADO_CODIGO,
                                                       Descricao = a.TB_ALMOXARIFADO_DESCRICAO,
                                                       EnderecoLogradouro = a.TB_ALMOXARIFADO_LOGRADOURO,
                                                       EnderecoNumero = a.TB_ALMOXARIFADO_NUMERO,
                                                       EnderecoCompl = a.TB_ALMOXARIFADO_COMPLEMENTO,
                                                       EnderecoBairro = a.TB_ALMOXARIFADO_BAIRRO,
                                                       EnderecoMunicipio = a.TB_ALMOXARIFADO_MUNICIPIO,
                                                       Uf = (new UFEntity { Id = b.TB_UF_ID, Descricao = b.TB_UF_DESCRICAO, Sigla = b.TB_UF_SIGLA }),
                                                       EnderecoCep = a.TB_ALMOXARIFADO_CEP,
                                                       EnderecoTelefone = a.TB_ALMOXARIFADO_TELEFONE,
                                                       EnderecoFax = a.TB_ALMOXARIFADO_FAX,
                                                       Responsavel = a.TB_ALMOXARIFADO_RESPONSAVEL,
                                                       Uge = (new UGEEntity(a.TB_UGE_ID)),
                                                       //Orgao = (new OrgaoEntity(a.TB_ORGAO_ID)),
                                                       //Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
                                                       RefInicial = a.TB_ALMOXARIFADO_MES_REF_INICIAL,
                                                       IndicadorAtividade = (new IndicadorAtividadeEntity(TratamentoDados.TryParseInt32(a.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE.ToString()))),
                                                       //Ordem = a.ORDEM
                                                       IgnoraCalendarioSiafemParaReabertura = a.TB_ALMOXARIFADO_IGNORA_CALENDARIO_SIAFEM_PARA_REABERTURA,
                                                   })
                                              .ToList<AlmoxarifadoEntity>();

            return new List<AlmoxarifadoEntity>();
        }

        public IList<AlmoxarifadoEntity> AlmoxarifadoTodosCod()
        {
            IList<AlmoxarifadoEntity> resultado = (from a in Db.TB_ALMOXARIFADOs
                                                   orderby (a.TB_ALMOXARIFADO_CODIGO)
                                                   select new AlmoxarifadoEntity
                                                   {
                                                       Id = a.TB_ALMOXARIFADO_ID,
                                                       Codigo = a.TB_ALMOXARIFADO_CODIGO,
                                                       Descricao = string.Format("{0} - {1}", a.TB_ALMOXARIFADO_CODIGO.ToString().PadLeft(2, '0'), a.TB_ALMOXARIFADO_DESCRICAO),
                                                       EnderecoLogradouro = a.TB_ALMOXARIFADO_LOGRADOURO,
                                                       EnderecoNumero = a.TB_ALMOXARIFADO_NUMERO,
                                                       EnderecoCompl = a.TB_ALMOXARIFADO_COMPLEMENTO,
                                                       EnderecoBairro = a.TB_ALMOXARIFADO_BAIRRO,
                                                       EnderecoMunicipio = a.TB_ALMOXARIFADO_MUNICIPIO,
                                                       EnderecoCep = a.TB_ALMOXARIFADO_CEP,
                                                       EnderecoTelefone = a.TB_ALMOXARIFADO_TELEFONE,
                                                       EnderecoFax = a.TB_ALMOXARIFADO_FAX,
                                                       Responsavel = a.TB_ALMOXARIFADO_RESPONSAVEL,
                                                       Uge = new UGEEntity(a.TB_UGE_ID),
                                                       Gestor = new GestorEntity(a.TB_GESTOR_ID),
                                                       RefInicial = a.TB_ALMOXARIFADO_MES_REF_INICIAL,
                                                       IndicadorAtividade = (new IndicadorAtividadeEntity(Convert.ToInt32(a.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE))),
                                                       IgnoraCalendarioSiafemParaReabertura = a.TB_ALMOXARIFADO_IGNORA_CALENDARIO_SIAFEM_PARA_REABERTURA
                                                   })
                                            .ToList<AlmoxarifadoEntity>();

            return resultado;
        }

        public IList<AlmoxarifadoEntity> AlmoxarifadoTodosCod(int OrgaoId, int GestorId)
        {
            IEnumerable<AlmoxarifadoEntity> resultado = (from a in Db.TB_ALMOXARIFADOs
                                                         orderby (a.TB_ALMOXARIFADO_CODIGO)
                                                         select new AlmoxarifadoEntity
                                                         {
                                                             Id = a.TB_ALMOXARIFADO_ID,
                                                             Codigo = a.TB_ALMOXARIFADO_CODIGO,
                                                             Descricao = string.Format("{0} - {1}", a.TB_ALMOXARIFADO_CODIGO.ToString().PadLeft(2, '0'), a.TB_ALMOXARIFADO_DESCRICAO),
                                                             EnderecoLogradouro = a.TB_ALMOXARIFADO_LOGRADOURO,
                                                             EnderecoNumero = a.TB_ALMOXARIFADO_NUMERO,
                                                             EnderecoCompl = a.TB_ALMOXARIFADO_COMPLEMENTO,
                                                             EnderecoBairro = a.TB_ALMOXARIFADO_BAIRRO,
                                                             EnderecoMunicipio = a.TB_ALMOXARIFADO_MUNICIPIO,
                                                             EnderecoCep = a.TB_ALMOXARIFADO_CEP,
                                                             EnderecoTelefone = a.TB_ALMOXARIFADO_TELEFONE,
                                                             EnderecoFax = a.TB_ALMOXARIFADO_FAX,
                                                             Responsavel = a.TB_ALMOXARIFADO_RESPONSAVEL,
                                                             Uge = new UGEEntity(a.TB_UGE_ID),
                                                             Gestor = new GestorEntity(a.TB_GESTOR_ID),
                                                             RefInicial = a.TB_ALMOXARIFADO_MES_REF_INICIAL,
                                                             IndicadorAtividade = (new IndicadorAtividadeEntity(Convert.ToInt32(a.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE))),
                                                             IgnoraCalendarioSiafemParaReabertura = a.TB_ALMOXARIFADO_IGNORA_CALENDARIO_SIAFEM_PARA_REABERTURA
                                                         })
                                            .ToList<AlmoxarifadoEntity>();

            if (GestorId != 0)
                resultado = resultado.Where(a => a.Gestor.Id == GestorId);

            return resultado.ToList();
        }

        public AlmoxarifadoEntity ObterAlmoxarifado(int? idAlmoxarifado)
        {
            AlmoxarifadoEntity lObjRetorno = null;

            if (idAlmoxarifado.HasValue)
            {
                lObjRetorno = (from rowTabela in Db.TB_ALMOXARIFADOs
                               where rowTabela.TB_ALMOXARIFADO_ID == idAlmoxarifado
                               select new AlmoxarifadoEntity
                               {
                                   Id = rowTabela.TB_ALMOXARIFADO_ID,
                                   Codigo = rowTabela.TB_ALMOXARIFADO_CODIGO,
                                   Descricao = rowTabela.TB_ALMOXARIFADO_DESCRICAO,
                                   EnderecoLogradouro = rowTabela.TB_ALMOXARIFADO_LOGRADOURO,
                                   EnderecoNumero = rowTabela.TB_ALMOXARIFADO_NUMERO,
                                   EnderecoCompl = rowTabela.TB_ALMOXARIFADO_COMPLEMENTO,
                                   EnderecoBairro = rowTabela.TB_ALMOXARIFADO_BAIRRO,
                                   EnderecoMunicipio = rowTabela.TB_ALMOXARIFADO_MUNICIPIO,
                                   EnderecoCep = rowTabela.TB_ALMOXARIFADO_CEP,
                                   EnderecoTelefone = rowTabela.TB_ALMOXARIFADO_TELEFONE,
                                   EnderecoFax = rowTabela.TB_ALMOXARIFADO_FAX,
                                   Responsavel = rowTabela.TB_ALMOXARIFADO_RESPONSAVEL,
                                   MesRef = rowTabela.TB_ALMOXARIFADO_MES_REF,
                                   Uge = new UGEEntity(rowTabela.TB_UGE_ID) { Codigo = rowTabela.TB_UGE.TB_UGE_CODIGO },
                                   Gestor = new GestorEntity(rowTabela.TB_GESTOR_ID) { CodigoGestao = rowTabela.TB_GESTOR.TB_GESTOR_CODIGO_GESTAO, Nome = rowTabela.TB_GESTOR.TB_GESTOR_NOME, NomeReduzido = rowTabela.TB_GESTOR.TB_GESTOR_NOME_REDUZIDO },
                                   RefInicial = rowTabela.TB_ALMOXARIFADO_MES_REF_INICIAL,
                                   IndicAtividade = rowTabela.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE,
                                   //IndicadorAtividade = new IndicadorAtividadeEntity(Convert.ToInt32(rowTabela.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE)),
                                   //SubAlmoxarifado = (rowTabela.TB_ALMOXARIFADO_TIPO_ALMOXARIFADO == 1),
                                   //AlmoxarifadoVinculadoId = ((rowTabela.TB_ALMOXARIFADO_VINCULADO_ID != null) ? rowTabela.TB_ALMOXARIFADO_VINCULADO_ID : 0),
                                   //CCCentroCusto = rowTabela.TB_ALMOXARIFADO_CC_CENTRO_CUSTO
                                   IgnoraCalendarioSiafemParaReabertura = rowTabela.TB_ALMOXARIFADO_IGNORA_CALENDARIO_SIAFEM_PARA_REABERTURA
                               }).FirstOrDefault();
            }

            return lObjRetorno;
        }

        public AlmoxarifadoEntity ObterAlmoxarifadoUGE(int codigoUGE, int codigoAlmoxarifado)
        {
            AlmoxarifadoEntity objEntidade = null;
            Expression<Func<TB_ALMOXARIFADO, bool>> expWhere = null;
            IQueryable<TB_ALMOXARIFADO> qryConsulta = null;



            expWhere = (almoxaConsultado => almoxaConsultado.TB_UGE.TB_UGE_CODIGO == codigoUGE
                                          && almoxaConsultado.TB_ALMOXARIFADO_CODIGO == codigoAlmoxarifado);

            qryConsulta = Db.TB_ALMOXARIFADOs.AsQueryable();
            objEntidade = qryConsulta.Where(expWhere)
                                     .Select(_instanciadorDTOAlmoxarifado())
                                     .FirstOrDefault();


            return objEntidade;
        }

        public IList<AlmoxarifadoEntity> ListarAlmoxarifadoPorUGE(int ugeID)
        {
            IList<AlmoxarifadoEntity> lstAlmoxarifadosDaUGE = null;
            IQueryable<TB_ALMOXARIFADO> qryConsulta = null;

            qryConsulta = (from rowTabela in Db.TB_ALMOXARIFADOs
                           select rowTabela).AsQueryable();

            if (ugeID != 0)
                qryConsulta = qryConsulta.Where(almoxarifado => almoxarifado.TB_UGE_ID == ugeID);
            else
                return (new List<AlmoxarifadoEntity>(0));

            lstAlmoxarifadosDaUGE = qryConsulta.Select(_instanciadorDTOAlmoxarifado())
                                               .ToList();

            this.totalregistros = qryConsulta.Count();

            return lstAlmoxarifadosDaUGE;
        }

        public IList<AlmoxarifadoEntity> Imprimir()
        {
            IList<AlmoxarifadoEntity> resultado = (from a in Db.TB_ALMOXARIFADOs
                                                   join b in Db.TB_UFs on a.TB_UF_ID equals b.TB_UF_ID
                                                   where (a.TB_GESTOR_ID == this.Entity.Gestor.Id)
                                                   orderby (a.TB_ALMOXARIFADO_CODIGO)
                                                   select new AlmoxarifadoEntity
                                                   {
                                                       Id = a.TB_ALMOXARIFADO_ID,
                                                       Codigo = a.TB_ALMOXARIFADO_CODIGO,
                                                       Descricao = a.TB_ALMOXARIFADO_DESCRICAO,
                                                       EnderecoLogradouro = a.TB_ALMOXARIFADO_LOGRADOURO,
                                                       EnderecoNumero = a.TB_ALMOXARIFADO_NUMERO,
                                                       EnderecoCompl = a.TB_ALMOXARIFADO_COMPLEMENTO,
                                                       EnderecoBairro = a.TB_ALMOXARIFADO_BAIRRO,
                                                       EnderecoMunicipio = a.TB_ALMOXARIFADO_MUNICIPIO,
                                                       Uf = (new UFEntity { Id = b.TB_UF_ID, Descricao = b.TB_UF_DESCRICAO, Sigla = b.TB_UF_SIGLA }),
                                                       EnderecoCep = a.TB_ALMOXARIFADO_CEP,
                                                       EnderecoTelefone = a.TB_ALMOXARIFADO_TELEFONE,
                                                       EnderecoFax = a.TB_ALMOXARIFADO_FAX,
                                                       Responsavel = a.TB_ALMOXARIFADO_RESPONSAVEL,
                                                       Uge = (new UGEEntity(a.TB_UGE_ID)),
                                                       RefInicial = a.TB_ALMOXARIFADO_MES_REF_INICIAL,

                                                       IndicadorAtividade = (new IndicadorAtividadeEntity(TratamentoDados.TryParseInt32(a.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE.ToString()))),
                                                   })
                                              .ToList<AlmoxarifadoEntity>();

            return resultado;
        }

        public void Excluir()
        {
            TB_ALMOXARIFADO tbAlmoxarifado = this.Db.TB_ALMOXARIFADOs
                .Where(a => a.TB_ALMOXARIFADO_ID == this.Entity.Id).FirstOrDefault();
            this.Db.TB_ALMOXARIFADOs.DeleteOnSubmit(tbAlmoxarifado);
            this.Db.SubmitChanges();
        }

        public void Salvar()
        {
            TB_ALMOXARIFADO tbAlmoxarifado = new TB_ALMOXARIFADO();

            if (this.Entity.Id.HasValue)
                tbAlmoxarifado = this.Db.TB_ALMOXARIFADOs.Where(a => a.TB_ALMOXARIFADO_ID == this.Entity.Id).FirstOrDefault();
            else
                Db.TB_ALMOXARIFADOs.InsertOnSubmit(tbAlmoxarifado);


            tbAlmoxarifado.TB_ALMOXARIFADO_MES_REF = this.Entity.MesRef;
            tbAlmoxarifado.TB_GESTOR_ID = this.Entity.Gestor.Id.HasValue ? Convert.ToInt32(this.Entity.Gestor.Id.Value) : 0;
            tbAlmoxarifado.TB_UGE_ID = this.Entity.Uge.Id.Value;
            tbAlmoxarifado.TB_ALMOXARIFADO_CODIGO = this.Entity.Codigo.Value;
            tbAlmoxarifado.TB_ALMOXARIFADO_DESCRICAO = this.Entity.Descricao;
            tbAlmoxarifado.TB_ALMOXARIFADO_LOGRADOURO = this.Entity.EnderecoLogradouro;
            tbAlmoxarifado.TB_ALMOXARIFADO_NUMERO = this.Entity.EnderecoNumero;
            tbAlmoxarifado.TB_ALMOXARIFADO_COMPLEMENTO = this.Entity.EnderecoCompl;
            tbAlmoxarifado.TB_ALMOXARIFADO_BAIRRO = this.Entity.EnderecoBairro;
            tbAlmoxarifado.TB_ALMOXARIFADO_MUNICIPIO = this.Entity.EnderecoMunicipio;

            if (this.Entity.TipoAlmoxarifado != null)
                tbAlmoxarifado.TB_ALMOXARIFADO_TIPO = this.Entity.TipoAlmoxarifado;

            if (this.Entity.RefFaturamento != null)
                tbAlmoxarifado.TB_ALMOXARIFADO_MES_REF_FATURAMENTO = this.Entity.RefFaturamento;

            if (this.Entity.Uf != null)
                tbAlmoxarifado.TB_UF_ID = this.Entity.Uf.Id.Value;

            tbAlmoxarifado.TB_ALMOXARIFADO_CEP = this.Entity.EnderecoCep;
            tbAlmoxarifado.TB_ALMOXARIFADO_TELEFONE = this.Entity.EnderecoTelefone;
            tbAlmoxarifado.TB_ALMOXARIFADO_FAX = this.Entity.EnderecoFax;
            tbAlmoxarifado.TB_ALMOXARIFADO_RESPONSAVEL = this.Entity.Responsavel;
            tbAlmoxarifado.TB_ALMOXARIFADO_MES_REF_INICIAL = this.Entity.RefInicial;
            tbAlmoxarifado.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE = Convert.ToBoolean(this.Entity.IndicadorAtividade.Id.HasValue ? this.Entity.IndicadorAtividade.Id.Value : 0);
            tbAlmoxarifado.TB_ALMOXARIFADO_IGNORA_CALENDARIO_SIAFEM_PARA_REABERTURA = this.Entity.IgnoraCalendarioSiafemParaReabertura;

            this.Db.SubmitChanges();
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = true;

            if (this.Entity.Id.HasValue)
            {
                retorno = this.Db.TB_ALMOXARIFADOs
                    .Where(a => a.TB_ALMOXARIFADO_CODIGO == this.Entity.Codigo.Value)
                    //.Where(a => a.TB_ORGAO_ID == this.Entity.Orgao.Id.Value)
                    .Where(a => a.TB_GESTOR_ID == this.Entity.Gestor.Id.Value)
                    .Where(a => a.TB_ALMOXARIFADO_ID != this.Entity.Id.Value).Count() > 0;
            }
            else
                retorno = this.Db.TB_ALMOXARIFADOs
                    // .Where(a => a.TB_ORGAO_ID == this.Entity.Orgao.Id.Value)
                    .Where(a => a.TB_GESTOR_ID == this.Entity.Gestor.Id.Value)
                    .Where(a => a.TB_ALMOXARIFADO_CODIGO == this.Entity.Codigo).Count() > 0;

            return retorno;
        }

        public AlmoxarifadoEntity LerRegistro()
        {
            throw new NotImplementedException();
        }

        //Listar todos Where
        public IList<AlmoxarifadoEntity> Listar(System.Linq.Expressions.Expression<Func<AlmoxarifadoEntity, bool>> where)
        {
            IList<AlmoxarifadoEntity> resultado = (from a in Db.TB_ALMOXARIFADOs
                                                   orderby (a.TB_ALMOXARIFADO_CODIGO)
                                                   select new AlmoxarifadoEntity
                                                   {
                                                       Id = a.TB_ALMOXARIFADO_ID,
                                                       Codigo = a.TB_ALMOXARIFADO_CODIGO,
                                                       Descricao = a.TB_ALMOXARIFADO_DESCRICAO,
                                                       EnderecoLogradouro = a.TB_ALMOXARIFADO_LOGRADOURO,
                                                       EnderecoNumero = a.TB_ALMOXARIFADO_NUMERO,
                                                       EnderecoCompl = a.TB_ALMOXARIFADO_COMPLEMENTO,
                                                       EnderecoBairro = a.TB_ALMOXARIFADO_BAIRRO,
                                                       EnderecoMunicipio = a.TB_ALMOXARIFADO_MUNICIPIO,
                                                       Uf = (new UFEntity(a.TB_UF_ID)),
                                                       EnderecoCep = a.TB_ALMOXARIFADO_CEP,
                                                       EnderecoTelefone = a.TB_ALMOXARIFADO_TELEFONE,
                                                       EnderecoFax = a.TB_ALMOXARIFADO_FAX,
                                                       Responsavel = a.TB_ALMOXARIFADO_RESPONSAVEL,
                                                       Uge = (new UGEEntity(a.TB_UGE_ID)),
                                                       Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
                                                       RefInicial = a.TB_ALMOXARIFADO_MES_REF_INICIAL,
                                                       MesRef = a.TB_ALMOXARIFADO_MES_REF,
                                                       IndicadorAtividade = (new IndicadorAtividadeEntity(Convert.ToInt32(a.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE))),
                                                       IgnoraCalendarioSiafemParaReabertura = a.TB_ALMOXARIFADO_IGNORA_CALENDARIO_SIAFEM_PARA_REABERTURA,
                                                   }).Where(where).ToList();
            return resultado;
        }

        public IList<AlmoxarifadoEntity> ListarTodosCod()
        {
            IList<AlmoxarifadoEntity> resultado = (from a in Db.TB_ALMOXARIFADOs
                                                   orderby (a.TB_ALMOXARIFADO_CODIGO)
                                                   select new AlmoxarifadoEntity
                                                   {
                                                       Id = a.TB_ALMOXARIFADO_ID,
                                                       Codigo = a.TB_ALMOXARIFADO_CODIGO,
                                                       Descricao = a.TB_ALMOXARIFADO_DESCRICAO,
                                                       EnderecoLogradouro = a.TB_ALMOXARIFADO_LOGRADOURO,
                                                       EnderecoNumero = a.TB_ALMOXARIFADO_NUMERO,
                                                       EnderecoCompl = a.TB_ALMOXARIFADO_COMPLEMENTO,
                                                       EnderecoBairro = a.TB_ALMOXARIFADO_BAIRRO,
                                                       EnderecoMunicipio = a.TB_ALMOXARIFADO_MUNICIPIO,
                                                       Uf = (new UFEntity(a.TB_UF_ID)),
                                                       EnderecoCep = a.TB_ALMOXARIFADO_CEP,
                                                       EnderecoTelefone = a.TB_ALMOXARIFADO_TELEFONE,
                                                       EnderecoFax = a.TB_ALMOXARIFADO_FAX,
                                                       Responsavel = a.TB_ALMOXARIFADO_RESPONSAVEL,
                                                       Uge = (new UGEEntity(a.TB_UGE_ID)),
                                                       Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
                                                       RefInicial = a.TB_ALMOXARIFADO_MES_REF_INICIAL,
                                                       MesRef = a.TB_ALMOXARIFADO_MES_REF,
                                                       IndicadorAtividade = (new IndicadorAtividadeEntity(Convert.ToInt32(a.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE))),
                                                   }).ToList();
            return resultado;
        }

        public bool PodeExcluir()
        {
            return true;
        }

        public IList<UGEEntity> ListarUGEGestor(int gestorId)
        {
            GestorEntity gestor = (from g in Db.TB_GESTORs
                                   where g.TB_GESTOR_ID == gestorId
                                   select new GestorEntity
                                   {
                                       Orgao = new OrgaoEntity { Id = g.TB_ORGAO_ID },
                                       Uo = new UOEntity { Id = g.TB_UO_ID },
                                       Uge = new UGEEntity { Id = g.TB_UGE_ID }
                                   }
                                  ).FirstOrDefault();

            IList<UGEEntity> resultado = null;
            if (gestor != null)
            {
                resultado = (from a in Db.TB_UGEs
                             join b in Db.TB_UOs on a.TB_UO_ID equals b.TB_UO_ID
                             join c in Db.TB_ORGAOs on b.TB_ORGAO_ID equals c.TB_ORGAO_ID
                             where (gestor.Uge.Id == null ? a.TB_UGE_ID == a.TB_UGE_ID : a.TB_UGE_ID == gestor.Uge.Id)
                             where (gestor.Uo.Id == null ? b.TB_UO_ID == b.TB_UO_ID : b.TB_UO_ID == gestor.Uo.Id)
                             where (gestor.Orgao.Id == null ? c.TB_ORGAO_ID == c.TB_ORGAO_ID : c.TB_ORGAO_ID == gestor.Orgao.Id)
                             orderby (a.TB_UGE_ID)
                             select new UGEEntity
                             {
                                 Id = a.TB_UGE_ID,
                                 Codigo = a.TB_UGE_CODIGO,
                                 Descricao = a.TB_UGE_DESCRICAO
                             }).ToList<UGEEntity>();
            }

            return resultado;
        }

        public AlmoxarifadoEntity GetAlmoxarifadoByDivisao(int divisaoId)
        {
            var result = (from a in Db.TB_DIVISAOs
                          where a.TB_DIVISAO_ID == divisaoId
                          select new AlmoxarifadoEntity
                          {
                              Id = a.TB_ALMOXARIFADO_ID,
                              Codigo = a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_CODIGO,
                              Descricao = a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_DESCRICAO,
                              EnderecoLogradouro = a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_LOGRADOURO,
                              EnderecoNumero = a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_NUMERO,
                              EnderecoCompl = a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_COMPLEMENTO,
                              EnderecoBairro = a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_BAIRRO,
                              EnderecoMunicipio = a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_MUNICIPIO,
                              Uf = (new UFEntity(a.TB_ALMOXARIFADO.TB_UF_ID)),
                              EnderecoCep = a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_CEP,
                              EnderecoTelefone = a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_TELEFONE,
                              EnderecoFax = a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_FAX,
                              Responsavel = a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_RESPONSAVEL,
                              Uge = (new UGEEntity(a.TB_ALMOXARIFADO.TB_UGE_ID)),
                              Gestor = (new GestorEntity(a.TB_ALMOXARIFADO.TB_GESTOR_ID)),
                              RefInicial = a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_MES_REF_INICIAL,
                              MesRef = a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_MES_REF,
                          }).FirstOrDefault();

            return result;
        }


        private Func<TB_ALMOXARIFADO, AlmoxarifadoEntity> _instanciadorDTOAlmoxarifado()
        {
            Func<TB_ALMOXARIFADO, AlmoxarifadoEntity> _actionSeletor = null;

            _actionSeletor = rowTabela => new AlmoxarifadoEntity
            {
                Id = rowTabela.TB_ALMOXARIFADO_ID,
                Codigo = rowTabela.TB_ALMOXARIFADO_CODIGO,
                CodigoDescricao = string.Format("{0:D6} - {1}", rowTabela.TB_ALMOXARIFADO_CODIGO, rowTabela.TB_ALMOXARIFADO_DESCRICAO),
                Descricao = rowTabela.TB_ALMOXARIFADO_DESCRICAO,
                EnderecoLogradouro = rowTabela.TB_ALMOXARIFADO_LOGRADOURO,
                EnderecoNumero = rowTabela.TB_ALMOXARIFADO_NUMERO,
                EnderecoCompl = rowTabela.TB_ALMOXARIFADO_COMPLEMENTO,
                EnderecoBairro = rowTabela.TB_ALMOXARIFADO_BAIRRO,
                EnderecoMunicipio = rowTabela.TB_ALMOXARIFADO_MUNICIPIO,
                EnderecoCep = rowTabela.TB_ALMOXARIFADO_CEP,
                EnderecoTelefone = rowTabela.TB_ALMOXARIFADO_TELEFONE,
                EnderecoFax = rowTabela.TB_ALMOXARIFADO_FAX,
                Responsavel = rowTabela.TB_ALMOXARIFADO_RESPONSAVEL,
                MesRef = rowTabela.TB_ALMOXARIFADO_MES_REF,
                Uge = new UGEEntity(rowTabela.TB_UGE_ID) { Codigo = rowTabela.TB_UGE.TB_UGE_CODIGO, CodigoDescricao = String.Format("{0:D6} - {1}", rowTabela.TB_UGE.TB_UGE_CODIGO, rowTabela.TB_UGE.TB_UGE_DESCRICAO) },
                Gestor = new GestorEntity(rowTabela.TB_GESTOR_ID) { CodigoGestao = rowTabela.TB_GESTOR.TB_GESTOR_CODIGO_GESTAO, Nome = rowTabela.TB_GESTOR.TB_GESTOR_NOME, NomeReduzido = rowTabela.TB_GESTOR.TB_GESTOR_NOME_REDUZIDO },
                RefInicial = rowTabela.TB_ALMOXARIFADO_MES_REF_INICIAL,
                IgnoraCalendarioSiafemParaReabertura = rowTabela.TB_ALMOXARIFADO_IGNORA_CALENDARIO_SIAFEM_PARA_REABERTURA,
            };

            return _actionSeletor;
        }

        //Busca todos os almoxarifados ativos por órgão
        public IList<AlmoxarifadoEntity> ListarAlmoxarifadoPorOrgao(int orgaoId)
        {
            IList<AlmoxarifadoEntity> resultado = (from a in Db.TB_ALMOXARIFADOs
                                                   join b in Db.TB_GESTORs on a.TB_GESTOR_ID equals b.TB_GESTOR_ID
                                                   where (b.TB_ORGAO_ID == orgaoId && a.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE == true)
                                                   orderby (a.TB_ALMOXARIFADO_CODIGO)
                                                   select new AlmoxarifadoEntity
                                                   {
                                                       Id = a.TB_ALMOXARIFADO_ID,
                                                       Codigo = a.TB_ALMOXARIFADO_CODIGO,
                                                       Descricao = string.Format("{0} - {1}", a.TB_ALMOXARIFADO_CODIGO.ToString().PadLeft(6, '0'), a.TB_ALMOXARIFADO_DESCRICAO).ToUpper(),
                                                   })
                                              .ToList<AlmoxarifadoEntity>().OrderBy(c => c.Descricao).ToList();
            return resultado;
        }
        //Lista Almoxarifado com pendência de fechamento
        public IList<AlmoxarifadoEntity> ListarAlmoxarifadoStatusFechamento(int idAlmoxarifado, int orgaoId)
        {
            IList<AlmoxarifadoEntity> resultado;
            if (idAlmoxarifado != 0)
            {
                resultado = (from a in Db.TB_ALMOXARIFADOs
                             join b in Db.TB_GESTORs on a.TB_GESTOR_ID equals b.TB_GESTOR_ID
                             join f in Db.TB_FECHAMENTOs on a.TB_ALMOXARIFADO_ID equals f.TB_ALMOXARIFADO_ID
                             where (b.TB_ORGAO_ID == orgaoId && a.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE == true) && a.TB_ALMOXARIFADO_ID == idAlmoxarifado
                             group new { a, f } by new { a.TB_ALMOXARIFADO_CODIGO, a.TB_ALMOXARIFADO_DESCRICAO } into g
                             select new AlmoxarifadoEntity
                             {
                                 Codigo = g.Key.TB_ALMOXARIFADO_CODIGO,
                                 Descricao = g.Key.TB_ALMOXARIFADO_DESCRICAO.ToUpper(),
                                 MesRef = g.Max(x => x.f.TB_FECHAMENTO_ANO_MES_REF).ToString()
                             })
                                                       .ToList<AlmoxarifadoEntity>().OrderBy(c => c.Codigo).ToList();
            }
            else
            {

                resultado = (from a in Db.TB_ALMOXARIFADOs
                             join b in Db.TB_GESTORs on a.TB_GESTOR_ID equals b.TB_GESTOR_ID
                             join f in Db.TB_FECHAMENTOs on a.TB_ALMOXARIFADO_ID equals f.TB_ALMOXARIFADO_ID
                             where (b.TB_ORGAO_ID == orgaoId && a.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE == true)
                             group new { a, f } by new { a.TB_ALMOXARIFADO_CODIGO, a.TB_ALMOXARIFADO_DESCRICAO } into g
                             select new AlmoxarifadoEntity
                             {
                                 Codigo = g.Key.TB_ALMOXARIFADO_CODIGO,
                                 Descricao = g.Key.TB_ALMOXARIFADO_DESCRICAO.ToUpper(),
                                 MesRef = g.Max(x => x.f.TB_FECHAMENTO_ANO_MES_REF).ToString()
                             })
                                                   .ToList<AlmoxarifadoEntity>().OrderBy(c => c.Codigo).ToList();
            }
            return resultado;


        }

        public IList<AlmoxarifadoEntity> ListarInicioFechamento(int orgaoId)
        {
            IList<AlmoxarifadoEntity> resultado;

            resultado = (from a in Db.TB_ALMOXARIFADOs
                         join b in Db.TB_GESTORs on a.TB_GESTOR_ID equals b.TB_GESTOR_ID
                         join f in Db.TB_FECHAMENTOs on a.TB_ALMOXARIFADO_ID equals f.TB_ALMOXARIFADO_ID
                         where (b.TB_ORGAO_ID == orgaoId && a.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE == true)
                         group new { f.TB_FECHAMENTO_ANO_MES_REF } by new { a.TB_ALMOXARIFADO_CODIGO, a.TB_ALMOXARIFADO_DESCRICAO } into g
                         select new AlmoxarifadoEntity
                         {
                             MesRef = g.Min(a => a.TB_FECHAMENTO_ANO_MES_REF).ToString()
                         }).ToList();
            var anoMesRef = resultado.Min(c => c.MesRef);
            resultado = resultado.Where(c => c.MesRef == anoMesRef).ToList();
            resultado = resultado.DistinctBy(r => r.MesRef).ToList();
            return resultado;
        }

        public bool ExisteMovimentacaoMaterialRetroativaAMesReferenciaAtual(int almoxarifadoId)
        {
            bool existeMovimentacaoMaterialRetroativaAMesReferenciaInicial = false;
            string mesReferenciaAtualAlmox = "";
            TB_ALMOXARIFADO rowTabelaAlmoxarifado = null;

            if (almoxarifadoId > 0)
            {
                rowTabelaAlmoxarifado = this.Db.TB_ALMOXARIFADOs.Where(almoxSAM => almoxSAM.TB_ALMOXARIFADO_ID == almoxarifadoId).FirstOrDefault();
                if (rowTabelaAlmoxarifado.IsNotNull())
                {
                    int contadorMovimentacao = 0;
                    IQueryable<string> qryConsulta = null;
                    Expression<Func<TB_MOVIMENTO, bool>> expWhere;


                    expWhere = (movMaterial => movMaterial.TB_MOVIMENTO_ATIVO == true
                                            && movMaterial.TB_ALMOXARIFADO_ID == almoxarifadoId);

                    qryConsulta = Db.TB_MOVIMENTOs.Where(expWhere)
                                                  .Select(movMaterial => (movMaterial.TB_MOVIMENTO_ANO_MES_REFERENCIA))
                                                  .OrderBy(_mesReferenciaMovimentacao => _mesReferenciaMovimentacao)
                                                  .AsQueryable();

                    
                    mesReferenciaAtualAlmox = rowTabelaAlmoxarifado.TB_ALMOXARIFADO_MES_REF;
                    try
                    {
                        qryConsulta.ToList().ForEach(movMaterial =>
                                                                    {
                                                                        contadorMovimentacao += ((Int32.Parse(movMaterial) < Int32.Parse(mesReferenciaAtualAlmox)) ? 1 : 0);
                                                                        if (contadorMovimentacao > 0)
                                                                            throw new Exception(String.Format("Encontrada Movimentacao Material com Mes-Referencia menor que '{0}'", mesReferenciaAtualAlmox));
                                                                    });
                    }
                    catch (Exception exc)
                    {
                        existeMovimentacaoMaterialRetroativaAMesReferenciaInicial = (contadorMovimentacao > 0);
                    }
                }
            }

            return existeMovimentacaoMaterialRetroativaAMesReferenciaInicial;
        }

        public string ObtemMesReferenciaAtual(int almoxarifadoId)
        {
            string mesReferenciaAtual = null;

            if (almoxarifadoId > 0)
            {
                var dtoRegistroTabela = this.Db.TB_ALMOXARIFADOs.Where(registroTabela => registroTabela.TB_ALMOXARIFADO_ID == almoxarifadoId).FirstOrDefault();
                if (dtoRegistroTabela.IsNotNull())
                    mesReferenciaAtual = dtoRegistroTabela.TB_ALMOXARIFADO_MES_REF;
            }


            return mesReferenciaAtual;
        }
    }
}
