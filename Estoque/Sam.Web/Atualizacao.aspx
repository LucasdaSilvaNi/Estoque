<%@ Page Title="SAM - Sistema de Administração de Materiais" Language="C#" AutoEventWireup="true"
    CodeBehind="Atualizacao.aspx.cs" Inherits="Sam.Web.Atualizacao" MasterPageFile="~/MasterPage/PrincipalFull.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <div id="content">
        <h1> Sistema SAM - Atualizações</h1>
        <br />
        <fieldset class="fieldset" style="text-align:left">
        <p>
            <h4>Atualizações 1.3.64</h4>
            <h5>11/09/2013</h5>
            <ul type="disc">
                <li>0.1 - Opção de salvar  a Saída de Materiais como Rascunho.</li>
                <li>0.2 - Bloqueia a Edição e exclução dos Subitens caso tenho alguma movimentação.</li>
                <li>0.3 - Aumento do Timeout da Analise, simulação e fechamento para 3 minutos.</li>
                <li>0.4 - Não é permitido fazer uma requisição com o subitem duplicado.</li>
                <li>0.5 - A Saída de material Retroativa com saldo negativo irá avisar o código e descrição do SubItem.</li>
                <li>0.6 - Correção da geração do relatório "Gerência de Catálogo".</li>
                <li>0.7 - Correção na somatório dos valores dos Subitens da tela de Entrada de Materiais.</li>
                <li>0.8 - Correção validação da Data de movimentação da Entrada e Saída de Materiais conforme o ano mês ref.</li>
            </ul>
        </p>
        <hr />
        <p>
            <h4>Atualizações 1.3.59</h4>
            <h5>08/08/2013</h5>
            <ul type="disc">
                <li>0.1 - Correção do módulo Fechamento (rotinas).</li>
                <li>0.2 - Correção do módulo Fechamento (informativo de mês de referência do almoxarifado logado.</li>
                <li>0.3 - Correção de método de verificação de transferências pendentes.</li>
                <li>0.4 - Ordenação do grid de Análise de Fechamento, por ordem alfabética.
            </ul>
        </p>
        <hr />
        <p>
            <h4>Atualizações 1.3.58</h4>
            <h5>08/08/2013</h5>
            <ul type="disc">
                <li>0.1 - Correções nas rotinas de saída de material.</li>
                <li>0.2 - Inclusão de mensagens descritivas, na ocorrência de erros de saldo/estoque, ao executar análise/simulação de Fechamento Mensal.</li>
            </ul>
        </p>
        <hr />
        <p>
            <h4>Atualizações 1.3.57</h4>
            <h5>08/08/2013</h5>
            <ul type="disc">
                <li>0.1 - Correção do grid de dados da página de Gerência de Catálogo (ordenação por ordem alfabética e paginação).</li>
                <li>0.2 - Alteração no módulo de saída de materiais, para permitir processamento de requisições pendentes de qualquer mês.</li>
            </ul>
        </p>
        <hr />
        <p>
            <h4>Atualizações 1.3.56</h4>
            <h5>08/08/2013</h5>
            <ul type="disc">
                <li>0.1 - Correção no relatório de consulta sintética, ocorria a somatória do saldo de subitens que possuam a mesma descrição, porém códigos e Naturezaa de Despesa divergentes.</li>
                <li>0.2 - Inclusão de campo totalizando valores dos subitens inclusos na nota de transferência (saída).</li>
                <li>0.3 - Tratamento no cadastro de fornecedores, evitando o cadastramento de mais de um fornecedor com o mesmo CPF/CNPJ.</li>
                <li>0.4 - Correção do relatório analítico, para exibição do código do subitem (exibia código apenas do primeiro subitem de cada Natureza de Despesa).</li>
                <li>0.5 - Correção no grid da tela de saída de materiais, para alinhamento de todos os itens integrantes do movimento.</li>
                <li>0.6 - Ocultação do campo Instruções, na tela de criação/edição de requisições.</li>
            </ul>
        </p>
        <hr />
        <p>
            <h4>Atualizações 1.3.55</h4>
            <h5>29/07/2013</h5>
            <ul type="disc">
                <li>0.1 - Tratamento para não cadastrar mais de um fornecedor com o mesmo CNPJ/CPF.</li>
                <li>0.2 - Informativo de qual subitem está com saldo insuficiente para lançamento retroativo.</li>
                <li>0.3 - Inclusão de campo para somatória de itens constantes na nota de fornecimento.</li>
            </ul>
        </p>
        <hr />
        <p>
            <h4>Atualizações 1.3.54</h4>
            <h5>04/07/2013</h5>
            <ul type="disc">
                <li>0.1 - Correção de erro na edição de requisição, quando existem  documentos com o mesmo número.</li>                
                <li>0.2 - Correção na ordenação do relatório analítico, ocorria divergências nos valores nos recalculos retroativos.</li>
                <li>0.3 - Melhora na performance do relatório Analítico e Sintético.</li>
                <li>0.4 - Criação da nova funcionalidade, postar mensagens ao usuário na tela principal do sistema.</li>
                <li>0.5 - Correção nas pesquisas de Items e Subitem material, estava ocorrendo um problema ao carregar a unidade de fornecimento e o código dos itens na tela de Entrada e Saída de Materiais.</li>
                <li>0.6 - Aumento da integridade ao realizar o fechamento mensal. Possivelmente corrigido o problema de deadlook ao realizar a simulação do fechamento.</li>                
                <li>0.7 - Ordenação da pesquisa de documentos das telas de Entrada e Saída de material por Número Documento decrescente.</li>
                <li>0.8 - Criação da nova tela de Atualização.</li>
                <li>0.9 - O sistema não compartilhará informações de cache quando os usuários se autenticarem simultaneamente com o mesmo CPF.</li>
                <li>0.10 - Tratamento para correções de divergências minímas, no cálculo de valores (quantidades centesimais). Indicação de qual subitem está com saldo insuficiente para lançamento retroativo.</li>
            </ul>
        </p>
        <hr />
        <p>
            <h4>Atualizações 1.3.53</h4>
            <h5>02/07/2013</h5>          
            <ul type="disc">
                <li>Aumento da integridade, performance e confiabilidade das funcionalidades na tela de Entrada de Materiais.</li>
                <li>Aumento da integridade, performance e confiabilidade das funcionalidades na tela de Saída de Materiais.</li>
                <li>Aumento da velocidade de consulta das lupas de Itens de Materiais.</li>
                <li>Aumento da velocidade de consulta das lupas de SubItens de Materiais.</li>
                <li>Desabilitada temporariamente das funcionalidades Entrada e Saida por lote. (Em manutenção).</li>
                <li>Melhora significativa na performance do sistema.</li>
            </ul>
        </p>   
        </fieldset>
        <br />
        <br />
        </div>
</asp:Content>
