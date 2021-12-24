using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Integracao.ConsultaWs.Constante
{
    public static class MensagemErro
    {
        internal const string FMT_MSGERRO__PARAMETRO_INFORMADO_INCORRETAMENTE = "Parâmetro(s) {0} informado(s) incorretamente. Favor verificar dado(s) informado(s).";

        internal const string FMT_MSGERRO__SUBITEM_NUMERO_MAXIMO_MOVIMENTACAO = "NUMERO MAXIMO DE SUBITENS POR MOVIMENTACAO ({0}), EXCEDIDO!";
        internal const string FMT_MSGERRO__SUBITEM_QTDE_INVALIDA_MOVIMENTACAO = "'Relação Subitens Requisição' (QTDE. INVALIDA INFORMADA, PARA SUBITEM {0})";
        internal const string FMT_MSGERRO__SUBITEM_CODIGO_REPETIDO = "'Relação Subitens Requisição' (SUBITEM {0} REQUISITADO MAIS DE UMA VEZ. PARA REQUISITAR NOVAMENTE O MESMO MATERIAL, UTILIZAR NOVA REQUISICAO DE MATERIAL)";
        internal const string FMT_MSGERRO__SUBITEM_NAO_EXISTE_CATALOGO_ALMOXARIFADO = "'Relação Subitens Requisição' (SUBITEM {0} NAO EXISTENTE NO CATALOGO DO ALMOXARIFADO {1:D3})";
        internal const string FMT_MSGERRO__SUBITEM_CODIGO_INVALIDO_INFORMADO = "'Relação Subitens Requisição' (CODIGO INVALIDO DE SUBITEM INFORMADO. VALOR INFORMADO: '{0}')";

        internal const string FMT_MSGERRO__PTRES_NAO_VALIDO_SUBITENS_MOVIMENTACAO = "'Relação Subitens Requisição' (PTRES {0} NAO-VALIDO, PARA UGE {1:D6})";
        internal const string FMT_MSGERRO__PTRES_CODIGO_INVALIDO_SUBITENS_MOVIMENTACAO = "'Relação Subitens Requisição' (CODIGO INVALIDO DE PTRES INFORMADO. VALOR INFORMADO: '{0}', PARA SUBITEM {1})";

        internal const string FMT_MSGERRO__UA_INFORMADA_NAO_PERTENCE_A_UGE = "'Código UA' (UA INFORMADA ({0}) NAO PERTENCENTE A UGE {1})";
        internal const string FMT_MSGERRO__DIVISAO_INFORMADA_NAO_PERTENCE_A_UA = "'Código Divisao' (DIVISAO INFORMADA ({0}) NAO PERTENCENTE A UA {1})";
        internal const string FMT_MSGERRO__UGE_INFORMADA_NAO_PERTENCE_A_ORGAO = "'Código Órgão' UGE INFORMADA ({0}) NAO PERTENCENTE AO ORGAO {1})";




        public const string MSG_ERRO__CONSULTA_NAO_RETORNOU_DADOS = "CONSULTA NAO RETORNOU DADOS";
        public const string MSG_ERRO__ACESSO_NEGADO__LOGIN_SENHA_INVALIDOS = "ACESSO NEGADO. FAVOR VERIFICAR LOGIN/SENHA INFORMADOS!";
        public const string MSG_ERRO__REGISTRO_MOVIMENTACAO = "ERRO AO REGISTRAR MOVIMENTACAO!";
        public const string MSG_ERRO__PARAMETRO_INCOMPATIVEL_MENSAGEM_INTEGRACAO_INFORMADA = "PARAMETROS INCOMPATIVEIS COM MENSAGEM DE INTEGRACAO INFORMADA!";
        public const string MSG_ERRO__PROCESSAMENTO_SOLICITACAO = "ERRO AO PROCESSAR SOLICITACAO";
        public const string MSG_ERRO__OBJETO_OPERATIONCONTEXT_ESTADO_INVALIDO = "Objeto OperationContext em estado inválido";
    }
}
