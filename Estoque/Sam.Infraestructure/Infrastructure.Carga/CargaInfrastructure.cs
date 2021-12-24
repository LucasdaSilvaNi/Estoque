using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using Sam.Common.Util;

namespace Sam.Infrastructure
{
    public class CargaInfrastructure : AbstractCrud<TB_CARGA, SAMwebEntities>
    {
        public IEnumerable<TB_CARGA> RetornarCargaErros(TB_CONTROLE controleItem)
        {
            var result = new List<TB_CARGA>();

            switch (controleItem.TB_TIPO_CONTROLE_ID)
            {
                case (int)GeralEnum.TipoControle.Almoxarifado:

                    result = (from carga in Context.TB_CARGA.AsEnumerable<TB_CARGA>()
                              join controle in Context.TB_CONTROLE on carga.TB_CONTROLE_ID equals controle.TB_CONTROLE_ID
                              where carga.TB_CARGA_VALIDO == false
                              where carga.TB_CONTROLE_ID == controleItem.TB_CONTROLE_ID
                              select new TB_CARGA
                              {
                                  TB_CARGA_ID = carga.TB_CARGA_ID,
                                  TB_CONTROLE_ID = carga.TB_CONTROLE_ID,                                  
                                  TB_ALMOXARIFADO_CODIGO = carga.TB_ALMOXARIFADO_CODIGO,
                                  TB_UGE_CODIGO = carga.TB_UGE_CODIGO,
                                  TB_GESTOR_ID = carga.TB_GESTOR_ID,
                                  TB_ALMOXARIFADO_ID = carga.TB_ALMOXARIFADO_ID,
                                  TB_CARGA_SEQ = carga.TB_CARGA_SEQ,
                                  TB_GESTOR_NOME_REDUZIDO = carga.TB_GESTOR_NOME_REDUZIDO,
                                  TB_CARGA_VALIDO = carga.TB_CARGA_VALIDO,
                                  TB_ALMOXARIFADO_BAIRRO = carga.TB_ALMOXARIFADO_BAIRRO,
                                  TB_ALMOXARIFADO_CEP = carga.TB_ALMOXARIFADO_CEP,
                                  TB_ALMOXARIFADO_COMPLEMENTO = carga.TB_ALMOXARIFADO_COMPLEMENTO,
                                  TB_ALMOXARIFADO_DESCRICAO = carga.TB_ALMOXARIFADO_DESCRICAO,
                                  TB_ALMOXARIFADO_FAX = carga.TB_ALMOXARIFADO_FAX,
                                  TB_ALMOXARIFADO_INDICADOR_ATIVIDADE = carga.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE,
                                  TB_ALMOXARIFADO_LOGRADOURO = carga.TB_ALMOXARIFADO_LOGRADOURO,
                                  TB_ALMOXARIFADO_MUNICIPIO = carga.TB_ALMOXARIFADO_MUNICIPIO,
                                  TB_ALMOXARIFADO_NUMERO = carga.TB_ALMOXARIFADO_NUMERO,
                                  TB_ALMOXARIFADO_MES_REF_INICIAL = carga.TB_ALMOXARIFADO_MES_REF_INICIAL,
                                  TB_ALMOXARIFADO_TELEFONE = carga.TB_ALMOXARIFADO_TELEFONE,
                                  TB_ALMOXARIFADO_RESPONSAVEL = carga.TB_ALMOXARIFADO_RESPONSAVEL,
                                  TB_CONTROLE = new TB_CONTROLE()
                                  {
                                      TB_TIPO_CONTROLE_ID = controle.TB_TIPO_CONTROLE_ID,
                                  },
                                  
                              }).AsEnumerable<TB_CARGA>().ToList();

                    break;

                case (int)GeralEnum.TipoControle.Responsavel:
                    result = (from carga in Context.TB_CARGA.AsEnumerable<TB_CARGA>()
                              join controle in Context.TB_CONTROLE on carga.TB_CONTROLE_ID equals controle.TB_CONTROLE_ID
                              where carga.TB_CARGA_VALIDO == false
                              where carga.TB_CONTROLE_ID == controleItem.TB_CONTROLE_ID
                              select new TB_CARGA
                              {
                                  TB_CARGA_ID = carga.TB_CARGA_ID,
                                  TB_CONTROLE_ID = carga.TB_CONTROLE_ID,
                                  TB_RESPONSAVEL_ID = carga.TB_RESPONSAVEL_ID,
                                  TB_RESPONSAVEL_CODIGO = carga.TB_RESPONSAVEL_CODIGO,
                                  TB_RESPONSAVEL_NOME = carga.TB_RESPONSAVEL_NOME,
                                  TB_RESPONSAVEL_CARGO = carga.TB_RESPONSAVEL_CARGO,
                                  TB_CARGA_SEQ = carga.TB_CARGA_SEQ,
                                  TB_GESTOR_NOME_REDUZIDO = carga.TB_GESTOR_NOME_REDUZIDO,
                                  TB_RESPONSAVEL_ENDERECO = carga.TB_RESPONSAVEL_ENDERECO,
                                  TB_GESTOR_ID = carga.TB_GESTOR_ID,
                                  
                                  TB_CONTROLE = new TB_CONTROLE()
                                  {
                                      TB_TIPO_CONTROLE_ID = controle.TB_TIPO_CONTROLE_ID,
                                  },
                                

                              }).AsEnumerable<TB_CARGA>().ToList();

                    break;
                case (int)GeralEnum.TipoControle.Usuario:

                    result = (from carga in Context.TB_CARGA.AsEnumerable<TB_CARGA>()
                              join controle in Context.TB_CONTROLE on carga.TB_CONTROLE_ID equals controle.TB_CONTROLE_ID
                              where carga.TB_CARGA_VALIDO == false
                              where carga.TB_CONTROLE_ID == controleItem.TB_CONTROLE_ID
                              select new TB_CARGA
                              {
                                  TB_CARGA_ID = carga.TB_CARGA_ID,
                                  TB_CONTROLE_ID = carga.TB_CONTROLE_ID,
                                  TB_GESTOR_ID = carga.TB_GESTOR_ID,
                                  TB_CARGA_SEQ = carga.TB_CARGA_SEQ,
                                  TB_GESTOR_NOME_REDUZIDO = carga.TB_GESTOR_NOME_REDUZIDO,
                                  TB_CARGA_VALIDO = carga.TB_CARGA_VALIDO,
                                  TB_USUARIO_CPF = carga.TB_USUARIO_CPF,
                                  TB_USUARIO_NOME_USUARIO = carga.TB_USUARIO_NOME_USUARIO,
                                  TB_USUARIO_NUM_RG = carga.TB_USUARIO_NUM_RG,
                                  TB_USUARIO_RG_UF = carga.TB_USUARIO_RG_UF,
                                  TB_ALMOXARIFADO_FAX = carga.TB_ALMOXARIFADO_FAX,
                                  TB_USUARIO_ORGAO_EMISSOR = carga.TB_USUARIO_ORGAO_EMISSOR,
                                  TB_USUARIO_END_RUA = carga.TB_USUARIO_END_RUA,
                                  TB_USUARIO_END_NUMERO = carga.TB_USUARIO_END_NUMERO,
                                  TB_USUARIO_END_COMPL = carga.TB_USUARIO_END_COMPL,
                                  TB_USUARIO_END_BAIRRO = carga.TB_USUARIO_END_BAIRRO,
                                  TB_USUARIO_END_MUNIC = carga.TB_USUARIO_END_MUNIC,
                                  TB_USUARIO_END_UF = carga.TB_USUARIO_END_UF,
                                  TB_USUARIO_END_CEP = carga.TB_USUARIO_END_CEP,
                                  TB_USUARIO_END_FONE = carga.TB_USUARIO_END_FONE,
                                  TB_USUARIO_EMAIL = carga.TB_USUARIO_EMAIL,
                                  TB_CONTROLE = new TB_CONTROLE()
                                  {
                                      TB_TIPO_CONTROLE_ID = controle.TB_TIPO_CONTROLE_ID,
                                  },

                              }).AsEnumerable<TB_CARGA>().ToList();
                    													

                    break;

                case (int)GeralEnum.TipoControle.PerfilRequisitante:

                    result = (from carga in Context.TB_CARGA.AsEnumerable<TB_CARGA>()
                              join controle in Context.TB_CONTROLE on carga.TB_CONTROLE_ID equals controle.TB_CONTROLE_ID
                              where carga.TB_CARGA_VALIDO == false
                              where carga.TB_CONTROLE_ID == controleItem.TB_CONTROLE_ID
                              select new TB_CARGA
                              {
                                  TB_CARGA_ID = carga.TB_CARGA_ID,
                                  TB_CONTROLE_ID = carga.TB_CONTROLE_ID,
                                  TB_GESTOR_ID = carga.TB_GESTOR_ID,
                                  TB_CARGA_SEQ = carga.TB_CARGA_SEQ,
                                  TB_USUARIO_CPF = carga.TB_USUARIO_CPF,
                                  TB_GESTOR_NOME_REDUZIDO = carga.TB_GESTOR_NOME_REDUZIDO,
                                  TB_CARGA_VALIDO = carga.TB_CARGA_VALIDO,
                                  TB_UA_CODIGO = carga.TB_UA_CODIGO,
                                  TB_DIVISAO_CODIGO = carga.TB_DIVISAO_CODIGO,
                                  TB_CONTROLE = new TB_CONTROLE()
                                  {
                                      TB_TIPO_CONTROLE_ID = controle.TB_TIPO_CONTROLE_ID,
                                  },

                              }).AsEnumerable<TB_CARGA>().ToList();


                    break;
                default:

                    result = (from carga in Context.TB_CARGA.AsEnumerable<TB_CARGA>()
                              join controle in Context.TB_CONTROLE on carga.TB_CONTROLE_ID equals controle.TB_CONTROLE_ID
                              where carga.TB_CARGA_VALIDO == false
                              where carga.TB_CONTROLE_ID == controleItem.TB_CONTROLE_ID
                              select new TB_CARGA
                              {
                                  TB_CARGA_ID = carga.TB_CARGA_ID,
                                  TB_CONTROLE_ID = carga.TB_CONTROLE_ID,
                                  TB_ITEM_MATERIAL_CODIGO = carga.TB_ITEM_MATERIAL_CODIGO,
                                  TB_SUBITEM_MATERIAL_CODIGO = carga.TB_SUBITEM_MATERIAL_CODIGO,
                                  TB_SUBITEM_MATERIAL_DESCRICAO = carga.TB_SUBITEM_MATERIAL_DESCRICAO,
                                  TB_NATUREZA_DESPESA_CODIGO = carga.TB_NATUREZA_DESPESA_CODIGO,
                                  TB_UNIDADE_FORNECIMENTO_CODIGO = carga.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                  TB_SUBITEM_MATERIAL_LOTE = carga.TB_SUBITEM_MATERIAL_LOTE,
                                  TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE = carga.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                  TB_INDICADOR_DISPONIVEL_DESCRICAO = carga.TB_INDICADOR_DISPONIVEL_DESCRICAO,
                                  TB_SUBITEM_MATERIAL_ESTOQUE_MIN = carga.TB_SUBITEM_MATERIAL_ESTOQUE_MIN,
                                  TB_SUBITEM_MATERIAL_ESTOQUE_MAX = carga.TB_SUBITEM_MATERIAL_ESTOQUE_MAX,
                                  TB_CONTA_AUXILIAR_CODIGO = carga.TB_CONTA_AUXILIAR_CODIGO,
                                  TB_ALMOXARIFADO_CODIGO = carga.TB_ALMOXARIFADO_CODIGO,
                                  TB_UGE_CODIGO = carga.TB_UGE_CODIGO,
                                  TB_SALDO_SUBITEM_SALDO_QTDE = carga.TB_SALDO_SUBITEM_SALDO_QTDE,
                                  TB_SALDO_SUBITEM_SALDO_VALOR = carga.TB_SALDO_SUBITEM_SALDO_VALOR,
                                  TB_SALDO_SUBITEM_LOTE_DT_VENC = carga.TB_SALDO_SUBITEM_LOTE_DT_VENC,
                                  TB_SALDO_SUBITEM_LOTE_IDENT = carga.TB_SALDO_SUBITEM_LOTE_IDENT,
                                  TB_SALDO_SUBITEM_LOTE_FAB = carga.TB_SALDO_SUBITEM_LOTE_FAB,
                                  TB_ITEM_MATERIAL_ID = carga.TB_ITEM_MATERIAL_ID,
                                  TB_GESTOR_ID = carga.TB_GESTOR_ID,
                                  TB_SUBITEM_MATERIAL_ID = carga.TB_SUBITEM_MATERIAL_ID,
                                  TB_NATUREZA_DESPESA_ID = carga.TB_NATUREZA_DESPESA_ID,
                                  TB_UNIDADE_FORNECIMENTO_ID = carga.TB_UNIDADE_FORNECIMENTO_ID,
                                  TB_INDICADOR_DISPONIVEL_ID = carga.TB_INDICADOR_DISPONIVEL_ID,
                                  TB_ALMOXARIFADO_ID = carga.TB_ALMOXARIFADO_ID,
                                  TB_CARGA_SEQ = carga.TB_CARGA_SEQ,
                                  TB_GESTOR_NOME_REDUZIDO = carga.TB_GESTOR_NOME_REDUZIDO,
                                  TB_CONTA_AUXILIAR_ID = carga.TB_CONTA_AUXILIAR_ID,
                                  TB_CARGA_VALIDO = carga.TB_CARGA_VALIDO,
                                  TB_DIVISAO_AREA = carga.TB_DIVISAO_AREA,
                                  TB_DIVISAO_BAIRRO = carga.TB_DIVISAO_BAIRRO,
                                  TB_DIVISAO_CEP = carga.TB_DIVISAO_CEP,
                                  TB_DIVISAO_CODIGO = carga.TB_DIVISAO_CODIGO,
                                  TB_DIVISAO_COMPLEMENTO = carga.TB_DIVISAO_COMPLEMENTO,
                                  TB_DIVISAO_DESCRICAO = carga.TB_DIVISAO_DESCRICAO,
                                  TB_DIVISAO_FAX = carga.TB_DIVISAO_FAX,
                                  TB_DIVISAO_INDICADOR_ATIVIDADE = carga.TB_DIVISAO_INDICADOR_ATIVIDADE,
                                  TB_DIVISAO_LOGRADOURO = carga.TB_DIVISAO_LOGRADOURO,
                                  TB_DIVISAO_MUNICIPIO = carga.TB_DIVISAO_MUNICIPIO,
                                  TB_DIVISAO_NUMERO = carga.TB_DIVISAO_NUMERO,
                                  TB_DIVISAO_QTDE_FUNC = carga.TB_DIVISAO_QTDE_FUNC,
                                  TB_DIVISAO_TELEFONE = carga.TB_DIVISAO_TELEFONE,
                                  TB_UF_SIGLA = carga.TB_UF_SIGLA,
                                  TB_RESPONSAVEL_CODIGO = carga.TB_RESPONSAVEL_CODIGO,
                                  TB_UA_CODIGO = carga.TB_UA_CODIGO,
                                  TB_CONTROLE = new TB_CONTROLE()
                                  {
                                      TB_TIPO_CONTROLE_ID = controle.TB_TIPO_CONTROLE_ID,
                                  },
                                
                              }).AsEnumerable<TB_CARGA>().ToList();

                    break;
            }
            TraceEntity.TraceQuery(result.AsEnumerable());
            return result.ToList(); 
        }

        public List<TB_CARGA> RetornarCarga(TB_CONTROLE controleItem)
        {
            var result = new List<TB_CARGA>();

            switch (controleItem.TB_TIPO_CONTROLE_ID)
            {
                case (int)GeralEnum.TipoControle.Almoxarifado:

                    result = (from carga in Context.TB_CARGA.AsEnumerable()
                              join controle in Context.TB_CONTROLE on carga.TB_CONTROLE_ID equals controle.TB_CONTROLE_ID
                              where carga.TB_CONTROLE_ID == controleItem.TB_CONTROLE_ID
                              select new TB_CARGA
                              {
                                  TB_CARGA_ID = carga.TB_CARGA_ID,
                                  TB_CONTROLE_ID = carga.TB_CONTROLE_ID,
                                  TB_ALMOXARIFADO_CODIGO = carga.TB_ALMOXARIFADO_CODIGO,
                                  TB_UGE_CODIGO = carga.TB_UGE_CODIGO,
                                  TB_GESTOR_ID = carga.TB_GESTOR_ID,
                                  TB_ALMOXARIFADO_ID = carga.TB_ALMOXARIFADO_ID,
                                  TB_CARGA_SEQ = carga.TB_CARGA_SEQ,
                                  TB_GESTOR_NOME_REDUZIDO = carga.TB_GESTOR_NOME_REDUZIDO,
                                  TB_CARGA_VALIDO = carga.TB_CARGA_VALIDO,
                                  TB_ALMOXARIFADO_BAIRRO = carga.TB_ALMOXARIFADO_BAIRRO,
                                  TB_ALMOXARIFADO_CEP = carga.TB_ALMOXARIFADO_CEP,
                                  TB_ALMOXARIFADO_COMPLEMENTO = carga.TB_ALMOXARIFADO_COMPLEMENTO,
                                  TB_ALMOXARIFADO_DESCRICAO = carga.TB_ALMOXARIFADO_DESCRICAO,
                                  TB_ALMOXARIFADO_FAX = carga.TB_ALMOXARIFADO_FAX,
                                  TB_ALMOXARIFADO_INDICADOR_ATIVIDADE = carga.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE,
                                  TB_ALMOXARIFADO_LOGRADOURO = carga.TB_ALMOXARIFADO_LOGRADOURO,
                                  TB_ALMOXARIFADO_MUNICIPIO = carga.TB_ALMOXARIFADO_MUNICIPIO,
                                  TB_ALMOXARIFADO_NUMERO = carga.TB_ALMOXARIFADO_NUMERO,
                                  TB_ALMOXARIFADO_MES_REF_INICIAL = carga.TB_ALMOXARIFADO_MES_REF_INICIAL,
                                  TB_ALMOXARIFADO_TELEFONE = carga.TB_ALMOXARIFADO_TELEFONE,
                                  TB_ALMOXARIFADO_RESPONSAVEL = carga.TB_ALMOXARIFADO_RESPONSAVEL,
                                  TB_CONTROLE = new TB_CONTROLE()
                                  {
                                      TB_TIPO_CONTROLE_ID = controle.TB_TIPO_CONTROLE_ID,
                                  },
                              }).ToList();
               
                    break;

                case (int)GeralEnum.TipoControle.Responsavel:
                    result = (from carga in Context.TB_CARGA.AsEnumerable<TB_CARGA>()
                              join controle in Context.TB_CONTROLE on carga.TB_CONTROLE_ID equals controle.TB_CONTROLE_ID
                              where carga.TB_CONTROLE_ID == controleItem.TB_CONTROLE_ID
                              select new TB_CARGA
                              {
                                  TB_CARGA_ID = carga.TB_CARGA_ID,
                                  TB_CONTROLE_ID = carga.TB_CONTROLE_ID,
                                  TB_RESPONSAVEL_ID = carga.TB_RESPONSAVEL_ID,
                                  TB_RESPONSAVEL_CODIGO = carga.TB_RESPONSAVEL_CODIGO,
                                  TB_RESPONSAVEL_NOME = carga.TB_RESPONSAVEL_NOME,
                                  TB_RESPONSAVEL_CARGO = carga.TB_RESPONSAVEL_CARGO,
                                  TB_CARGA_SEQ = carga.TB_CARGA_SEQ,
                                  TB_GESTOR_NOME_REDUZIDO = carga.TB_GESTOR_NOME_REDUZIDO,
                                  TB_RESPONSAVEL_ENDERECO = carga.TB_RESPONSAVEL_ENDERECO,
                                  TB_GESTOR_ID = carga.TB_GESTOR_ID,

                                  TB_CONTROLE = new TB_CONTROLE()
                                  {
                                      TB_TIPO_CONTROLE_ID = controle.TB_TIPO_CONTROLE_ID,
                                  },


                              }).AsEnumerable<TB_CARGA>().ToList();

                    break;
                case (int)GeralEnum.TipoControle.Usuario:

                    result = (from carga in Context.TB_CARGA.AsEnumerable<TB_CARGA>()
                              join controle in Context.TB_CONTROLE on carga.TB_CONTROLE_ID equals controle.TB_CONTROLE_ID
                              where carga.TB_CONTROLE_ID == controleItem.TB_CONTROLE_ID
                              select new TB_CARGA
                              {
                                  TB_CARGA_ID = carga.TB_CARGA_ID,
                                  TB_CONTROLE_ID = carga.TB_CONTROLE_ID,
                                  TB_GESTOR_ID = carga.TB_GESTOR_ID,
                                  TB_CARGA_SEQ = carga.TB_CARGA_SEQ,
                                  TB_GESTOR_NOME_REDUZIDO = carga.TB_GESTOR_NOME_REDUZIDO,
                                  TB_CARGA_VALIDO = carga.TB_CARGA_VALIDO,
                                  TB_USUARIO_CPF = carga.TB_USUARIO_CPF,
                                  TB_USUARIO_NOME_USUARIO = carga.TB_USUARIO_NOME_USUARIO,
                                  TB_USUARIO_NUM_RG = carga.TB_USUARIO_NUM_RG,
                                  TB_USUARIO_RG_UF = carga.TB_USUARIO_RG_UF,
                                  TB_ALMOXARIFADO_FAX = carga.TB_ALMOXARIFADO_FAX,
                                  TB_USUARIO_ORGAO_EMISSOR = carga.TB_USUARIO_ORGAO_EMISSOR,
                                  TB_USUARIO_END_RUA = carga.TB_USUARIO_END_RUA,
                                  TB_USUARIO_END_NUMERO = carga.TB_USUARIO_END_NUMERO,
                                  TB_USUARIO_END_COMPL = carga.TB_USUARIO_END_COMPL,
                                  TB_USUARIO_END_BAIRRO = carga.TB_USUARIO_END_BAIRRO,
                                  TB_USUARIO_END_MUNIC = carga.TB_USUARIO_END_MUNIC,
                                  TB_USUARIO_END_UF = carga.TB_USUARIO_END_UF,
                                  TB_USUARIO_END_CEP = carga.TB_USUARIO_END_CEP,
                                  TB_USUARIO_END_FONE = carga.TB_USUARIO_END_FONE,
                                  TB_USUARIO_EMAIL = carga.TB_USUARIO_EMAIL,
                                  TB_CONTROLE = new TB_CONTROLE()
                                  {
                                      TB_TIPO_CONTROLE_ID = controle.TB_TIPO_CONTROLE_ID,
                                  },

                              }).AsEnumerable<TB_CARGA>().ToList();


                    break;

                case (int)GeralEnum.TipoControle.PerfilRequisitante:

                     result = (from carga in Context.TB_CARGA.AsEnumerable<TB_CARGA>()
                              join controle in Context.TB_CONTROLE on carga.TB_CONTROLE_ID equals controle.TB_CONTROLE_ID
                              where carga.TB_CONTROLE_ID == controleItem.TB_CONTROLE_ID
                              select new TB_CARGA
                              {
                                  TB_CARGA_ID = carga.TB_CARGA_ID,
                                  TB_CONTROLE_ID = carga.TB_CONTROLE_ID,
                                  TB_GESTOR_ID = carga.TB_GESTOR_ID,
                                  TB_CARGA_SEQ = carga.TB_CARGA_SEQ,
                                  TB_USUARIO_CPF = carga.TB_USUARIO_CPF,
                                  TB_GESTOR_NOME_REDUZIDO = carga.TB_GESTOR_NOME_REDUZIDO,
                                  TB_CARGA_VALIDO = carga.TB_CARGA_VALIDO,
                                  TB_UA_CODIGO = carga.TB_UA_CODIGO,
                                  TB_DIVISAO_CODIGO = carga.TB_DIVISAO_CODIGO,
                                  
                                  TB_CONTROLE = new TB_CONTROLE()
                                  {
                                      TB_TIPO_CONTROLE_ID = controle.TB_TIPO_CONTROLE_ID,
                                  },

                              }).AsEnumerable<TB_CARGA>().ToList();


                    break;


                default:
                    result = (from carga in Context.TB_CARGA.AsEnumerable()
                              join controle in Context.TB_CONTROLE on carga.TB_CONTROLE_ID equals controle.TB_CONTROLE_ID
                              where carga.TB_CONTROLE_ID == controleItem.TB_CONTROLE_ID
                              select new TB_CARGA
                              {
                                  TB_CARGA_ID = carga.TB_CARGA_ID,
                                  TB_CONTROLE_ID = carga.TB_CONTROLE_ID,
                                  TB_ITEM_MATERIAL_CODIGO = carga.TB_ITEM_MATERIAL_CODIGO,
                                  TB_SUBITEM_MATERIAL_CODIGO = carga.TB_SUBITEM_MATERIAL_CODIGO,
                                  TB_SUBITEM_MATERIAL_DESCRICAO = carga.TB_SUBITEM_MATERIAL_DESCRICAO,
                                  TB_NATUREZA_DESPESA_CODIGO = carga.TB_NATUREZA_DESPESA_CODIGO,
                                  TB_UNIDADE_FORNECIMENTO_CODIGO = carga.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                  TB_SUBITEM_MATERIAL_LOTE = carga.TB_SUBITEM_MATERIAL_LOTE,
                                  TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE = carga.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                  TB_INDICADOR_DISPONIVEL_DESCRICAO = carga.TB_INDICADOR_DISPONIVEL_DESCRICAO,
                                  TB_SUBITEM_MATERIAL_ESTOQUE_MIN = carga.TB_SUBITEM_MATERIAL_ESTOQUE_MIN,
                                  TB_SUBITEM_MATERIAL_ESTOQUE_MAX = carga.TB_SUBITEM_MATERIAL_ESTOQUE_MAX,
                                  TB_CONTA_AUXILIAR_CODIGO = carga.TB_CONTA_AUXILIAR_CODIGO,
                                  TB_ALMOXARIFADO_CODIGO = carga.TB_ALMOXARIFADO_CODIGO,
                                  TB_UGE_CODIGO = carga.TB_UGE_CODIGO,
                                  TB_SALDO_SUBITEM_SALDO_QTDE = carga.TB_SALDO_SUBITEM_SALDO_QTDE,
                                  TB_SALDO_SUBITEM_SALDO_VALOR = carga.TB_SALDO_SUBITEM_SALDO_VALOR,
                                  TB_SALDO_SUBITEM_LOTE_DT_VENC = carga.TB_SALDO_SUBITEM_LOTE_DT_VENC,
                                  TB_SALDO_SUBITEM_LOTE_IDENT = carga.TB_SALDO_SUBITEM_LOTE_IDENT,
                                  TB_SALDO_SUBITEM_LOTE_FAB = carga.TB_SALDO_SUBITEM_LOTE_FAB,
                                  TB_ITEM_MATERIAL_ID = carga.TB_ITEM_MATERIAL_ID,
                                  TB_GESTOR_ID = carga.TB_GESTOR_ID,
                                  TB_SUBITEM_MATERIAL_ID = carga.TB_SUBITEM_MATERIAL_ID,
                                  TB_NATUREZA_DESPESA_ID = carga.TB_NATUREZA_DESPESA_ID,
                                  TB_UNIDADE_FORNECIMENTO_ID = carga.TB_UNIDADE_FORNECIMENTO_ID,
                                  TB_INDICADOR_DISPONIVEL_ID = carga.TB_INDICADOR_DISPONIVEL_ID,
                                  TB_ALMOXARIFADO_ID = carga.TB_ALMOXARIFADO_ID,
                                  TB_CARGA_SEQ = carga.TB_CARGA_SEQ,
                                  TB_GESTOR_NOME_REDUZIDO = carga.TB_GESTOR_NOME_REDUZIDO,
                                  TB_CONTA_AUXILIAR_ID = carga.TB_CONTA_AUXILIAR_ID,
                                  TB_CARGA_VALIDO = carga.TB_CARGA_VALIDO,
                                  TB_DIVISAO_AREA = carga.TB_DIVISAO_AREA,
                                  TB_DIVISAO_BAIRRO = carga.TB_DIVISAO_BAIRRO,
                                  TB_DIVISAO_CEP = carga.TB_DIVISAO_CEP,
                                  TB_DIVISAO_CODIGO = carga.TB_DIVISAO_CODIGO,
                                  TB_DIVISAO_COMPLEMENTO = carga.TB_DIVISAO_COMPLEMENTO,
                                  TB_DIVISAO_DESCRICAO = carga.TB_DIVISAO_DESCRICAO,
                                  TB_DIVISAO_FAX = carga.TB_DIVISAO_FAX,
                                  TB_DIVISAO_INDICADOR_ATIVIDADE = carga.TB_DIVISAO_INDICADOR_ATIVIDADE,
                                  TB_DIVISAO_LOGRADOURO = carga.TB_DIVISAO_LOGRADOURO,
                                  TB_DIVISAO_MUNICIPIO = carga.TB_DIVISAO_MUNICIPIO,
                                  TB_DIVISAO_NUMERO = carga.TB_DIVISAO_NUMERO,
                                  TB_DIVISAO_QTDE_FUNC = carga.TB_DIVISAO_QTDE_FUNC,
                                  TB_DIVISAO_TELEFONE = carga.TB_DIVISAO_TELEFONE,
                                  TB_UF_SIGLA = carga.TB_UF_SIGLA,
                                  TB_RESPONSAVEL_CODIGO = carga.TB_RESPONSAVEL_CODIGO,
                                  TB_UA_CODIGO = carga.TB_UA_CODIGO,
                                  TB_CONTROLE = new TB_CONTROLE()
                                  {
                                      TB_TIPO_CONTROLE_ID = controle.TB_TIPO_CONTROLE_ID,
                                  },
                              }).ToList();
               
                    break;
            }

            TraceEntity.TraceQuery(result).ToList();
            return result.ToList();
        }

        //Essa query é simples e rapida e pode ser utilizada como exemplo.
        public IEnumerable<TB_CARGA_ERRO> RetornaCargaErroDescricao(IEnumerable<TB_CARGA> cargaList)
        {
            var result = (from cargaErro in Context.TB_CARGA_ERRO.ToList()
                          join carga in Context.TB_CARGA on cargaErro.TB_CARGA_ID equals carga.TB_CARGA_ID
                          join erro in Context.TB_ERRO on cargaErro.TB_ERRO_ID equals erro.TB_ERRO_ID
                          join controle in Context.TB_CONTROLE on carga.TB_CONTROLE_ID equals controle.TB_CONTROLE_ID
                          where carga.TB_CONTROLE_ID == cargaList.FirstOrDefault().TB_CONTROLE_ID
                          where carga.TB_CARGA_VALIDO == false
                          select new TB_CARGA_ERRO
                          {
                              TB_CARGA_ERRO_ID = cargaErro.TB_CARGA_ERRO_ID,
                              TB_CARGA_ID = cargaErro.TB_CARGA_ID,
                              TB_ERRO_ID = cargaErro.TB_ERRO_ID,
                              TB_CARGA = carga,
                              TB_ERRO = erro
                          });
            TraceEntity.TraceQuery(result);
            return result.ToList();
        }

        public List<TB_CARGA> RetornaCargaPrecoMedio(int controleId)
        {

            var result = (from a in Context.TB_CARGA.AsEnumerable()
                          where a.TB_SUBITEM_MATERIAL_ID == 26322
                          group a by a.TB_SUBITEM_MATERIAL_ID into aGroup                          
                          select new TB_CARGA
                          {
                              TB_SUBITEM_MATERIAL_ID = aGroup.Sum(b => b.TB_SUBITEM_MATERIAL_ID),
                              TB_SALDO_SUBITEM_SALDO_QTDE = aGroup.Sum(b => Convert.ToDecimal(b.TB_SALDO_SUBITEM_SALDO_QTDE)).ToString()
                          });
            TraceEntity.TraceQuery(result);
            return result.ToList();
        }
    }
}
