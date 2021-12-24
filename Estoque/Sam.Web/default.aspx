<%@ Page Title="SAM - Sistema de Administração de Materiais" Language="C#" AutoEventWireup="true"
    CodeBehind="default.aspx.cs" Inherits="Sam.Web._default" MasterPageFile="~/MasterPage/PrincipalFull.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <div id="content">
        <h1>
            Sistema SAM - Página Principal</h1>

            <fieldset class="fieldset">
            <p>
                O Sistema de Administração de Materiais da PRODESP foi desenvolvido especialmente
                para os diversos órgãos da administração pública do Estado de São Paulo e é integrado
                aos sistemas da administração financeira, utilizando a estrutura organizacional
                do SIAFEM, a codificação de materiais do SIAFISICO e BEC, informações de empenhos
                e planos de trabalho.
            </p>
            <p>
                O escopo do SAM é a gestão dos materiais consumíveis e dos permanentes, proporcionando
                apoio as rotinas operacionais, automatização de tarefas e o fornecimento de informações
                operacionais e gerenciais.
            </p>
        </fieldset>

        <div class="notificacaoBox" runat="server" id="pnlMensagemTodos" visible="false">
        <h2>Avisos Gerais</h2>
        <p>
            <asp:Label runat="server" ID="lblTituloMensagemTodos" Font-Bold="true"></asp:Label>
        </p>
        <p>
            <asp:Label runat="server" ID="lblMensagemTodos"></asp:Label>
        </p>
        </div>

        <div runat="server" class="notificacaoBox" id="pnlMensagemPerfil" visible="false">
        <h2>Notificações</h2>
        <p>
        <asp:Label runat="server" ID="lblTituloMensagemPerfil" Font-Bold="true"></asp:Label>        
        </p>
        <p>
        
            <asp:Label runat="server" ID="lblMensagemPerfil"></asp:Label>
        </p>
        </div>
        
        <div id="tabelas">
            <h2>
                Suporte ao Usuário</h2>
            <p>
                Em caso de dúvidas, problemas ou sugestões de melhorias, o sistema SAM possui uma equipe disponível para atendê-lo. Basta enviar um e-mail para <a href="mailto:suportesam@sp.gov.br">suportesam@sp.gov.br</a>.
            </p>
            <br />
            <br />
            <p>
                <font size="2"><b>P.S.1: E-Mails devem ser enviados, com o seguinte formato, no campo Assunto (ou Subject): 'Sistema SAM - Módulo Estoque - Ambiente Produção - UGE 000000'. Fora deste formato, não serão respondidos!</b></font>
            </p>
            <p>
                <font size="2"><b>P.S.2: Não copiar os analistas de suporte nos e-mails enviados ao SuporteSAM, pois os mesmos recebem todos os e-mails enviados ao SuporteSAM.</b></font>
            </p>
            <p>
                <font size="2"><b>P.S.3: E-mails enviados diretamente aos analistas não serão retornados.</b></font>
            </p>
            <br />
            <br />
            <p>
                Em caso de erro SIAFEM ou dúvidas em relação aos eventos utilizados entrar em contato com a Fazenda através do e-mail <a href='mailto:cscc@fazenda.sp.gov.br'>cscc@fazenda.sp.gov.br</a>.
            </p>
            <p>
                <font size="2" color="red"><b>
                    Para acesso ao ambiente de homologação do módulo de estoque, solicitamos utilizar a seguinte URL: <a href='http://sam.homologacao.sp.gov.br/estoque/'>http://sam.homologacao.sp.gov.br/Estoque/</a>.
                        <br />
                    P.S.4: Para o ambiente acima, deverá ser utilizado o seguinte formato, no campo Assunto (ou Subject): 'Sistema SAM - Módulo Estoque - Ambiente Homologação - UGE 000000'. Fora deste formato, não serão respondidos!
                </font></b>
            </p>
        </div>
        <div id="patrimonio" class="esconderControle">
            <h2>
                Patrimônio
            </h2>
            <p>
                As principais funções realizadas pelo módulo de patrimônio do SAM são registrar
                informações dos bens permanentes, acompanhar a localização e a responsabilidade,
                dar suporte a execução de inventários físicos, manter atualizada a vida contábil,
                transferir e dar baixa de bens do Patrimônio.</p>
        </div>
        <br />
        <br />
    </div>
</asp:Content>
