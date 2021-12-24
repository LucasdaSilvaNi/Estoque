using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Sam.Domain.Entity
{
    public class CargaErroDescricao
    {
        public CargaErroDescricao(string seq, string SubItemCod, string descricao)
        {
            TB_CARGA_SEQ = seq;
            TB_ERRO_DESCRICAO = descricao;
            TB_SUBITEM_MATERIAL_CODIGO = SubItemCod;
        }

        public string TB_CARGA_SEQ { get; set; }
        public string TB_SUBITEM_MATERIAL_CODIGO { get; set; }
        public string TB_ERRO_DESCRICAO { get; set; }
    }

    public class CargaErro 
    {
        public string TB_CARGA_SEQ { get; set; }
        public string TB_ITEM_MATERIAL_CODIGO { get; set; }
        public string TB_SUBITEM_MATERIAL_CODIGO { get; set; }
        public string TB_SUBITEM_MATERIAL_DESCRICAO { get; set; }
        public string TB_NATUREZA_DESPESA_CODIGO { get; set; }
        public string TB_UNIDADE_FORNECIMENTO_CODIGO { get; set; }
        public string TB_SUBITEM_MATERIAL_LOTE { get; set; }
        public string TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE { get; set; }
        public string TB_INDICADOR_DISPONIVEL_DESCRICAO { get; set; }
        public string TB_SUBITEM_MATERIAL_ESTOQUE_MIN { get; set; }
        public string TB_SUBITEM_MATERIAL_ESTOQUE_MAX { get; set; }
        public string TB_CONTA_AUXILIAR_CODIGO { get; set; }
        public string TB_ALMOXARIFADO_CODIGO { get; set; }
        public string TB_UGE_CODIGO { get; set; }
        public string TB_SALDO_SUBITEM_SALDO_QTDE { get; set; }
        public string TB_SALDO_SUBITEM_SALDO_VALOR { get; set; }
        public string TB_SALDO_SUBITEM_LOTE_DT_VENC { get; set; }
        public string TB_SALDO_SUBITEM_LOTE_IDENT { get; set; }
        public string TB_SALDO_SUBITEM_LOTE_FAB { get; set; }        
        public string TB_GESTOR_NOME_REDUZIDO { get; set; }
        public string TB_ERRO_DESCRICAO { get; set; }
    }

    public class CargaErroDivisao
    { 
     public string TB_CARGA_SEQ { get; set; }
     public string TB_GESTOR_NOME_REDUZIDO { get; set; }    
     public string TB_RESPONSAVEL_CODIGO { get; set; }   
     public string TB_ALMOXARIFADO_CODIGO { get; set; }  
     public string TB_DIVISAO_INDICADOR_ATIVIDADE  { get; set; }  	
     public string TB_UF_SIGLA  { get; set; }  	
     public string TB_UA_CODIGO  { get; set; }  	
     public string TB_DIVISAO_CODIGO  { get; set; }  	
     public string TB_DIVISAO_DESCRICAO  { get; set; }
     public string TB_DIVISAO_LOGRADOURO { get; set; }
     public string TB_ERRO_DESCRICAO { get; set; }
    
    }


    public class CargaErroAlmoxarifado
    {
        public string TB_CARGA_SEQ { get; set; }
        public string TB_UGE_CODIGO { get; set; }
        public string TB_GESTOR_NOME_REDUZIDO { get; set; }
        public string TB_ALMOXARIFADO_CODIGO { get; set; }
        public string TB_ALMOXARIFADO_DESCRICAO { get; set; }
        public string TB_ALMOXARIFADO_LOGRADOURO { get; set; }
        public string TB_ALMOXARIFADO_NUMERO { get; set; }
        public string TB_ALMOXARIFADO_COMPLEMENTO { get; set; }
        public string TB_ALMOXARIFADO_BAIRRO { get; set; }
        public string TB_ALMOXARIFADO_MUNICIPIO { get; set; }
        public string TB_UF_SIGLA { get; set; }
        public string TB_ALMOXARIFADO_CEP { get; set; }
        public string TB_ALMOXARIFADO_TELEFONE { get; set; }
        public string TB_ALMOXARIFADO_FAX { get; set; }
        public string TB_ALMOXARIFADO_RESPONSAVEL { get; set; }
        public string TB_ALMOXARIFADO_MES_REF_INICIAL { get; set; }
        public string TB_ALMOXARIFADO_INDICADOR_ATIVIDADE { get; set; }       
        public string TB_ERRO_DESCRICAO { get; set; }

    }

    public class CargaErroResponsavel
    {
        public string TB_CARGA_SEQ { get; set; }
        public string TB_GESTOR_NOME_REDUZIDO { get; set; }
        public string TB_RESPONSAVEL_CODIGO { get; set; }        
        public string TB_RESPONSAVEL_NOME { get; set; }
        public string TB_RESPONSAVEL_CARGO { get; set; }
        public string TB_RESPONSAVEL_ENDERECO { get; set; }
        public string TB_ERRO_DESCRICAO { get; set; }
    }

    public class CargaErroUsuario
    {
        public string TB_CARGA_SEQ { get; set; }        
        public string TB_USUARIO_CPF { get; set; }
        public string TB_USUARIO_NOME_USUARIO { get; set; }
        public string TB_USUARIO_NUM_RG { get; set; }
        public string TB_USUARIO_RG_UF { get; set; }
        public string TB_USUARIO_ORGAO_EMISSOR { get; set; }
        public string TB_USUARIO_END_RUA { get; set; }
        public string TB_USUARIO_END_NUMERO { get; set; }
        public string TB_USUARIO_END_COMPL { get; set; }
        public string TB_USUARIO_END_BAIRRO { get; set; }
        public string TB_USUARIO_END_MUNIC { get; set; }
        public string TB_USUARIO_END_UF { get; set; }
        public string TB_USUARIO_END_CEP { get; set; }
        public string TB_USUARIO_END_FONE { get; set; }
        public string TB_USUARIO_EMAIL { get; set; }
        public string TB_GESTOR_NOME_REDUZIDO { get; set; }
        public string TB_ERRO_DESCRICAO { get; set; }
    }

    public class CargaErroPerfilRequisitante
    {
        public string TB_CARGA_SEQ { get; set; }
        public string TB_USUARIO_CPF { get; set; }
        public string TB_GESTOR_NOME_REDUZIDO { get; set; }
        public string TB_UA_CODIGO { get; set; }
        public string TB_DIVISAO_CODIGO { get; set; }       
        public string TB_ERRO_DESCRICAO { get; set; }
    }

 

}

