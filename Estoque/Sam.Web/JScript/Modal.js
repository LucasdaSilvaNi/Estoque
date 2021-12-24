$(function () {
    // Dialog
    $('#dialog').dialog({
        autoOpen: false,
        position: 'top',
        width: 620,
        modal: true,
        open: function (type, data) { $(this).parent().appendTo("form"); },
        buttons: {

        }
    });


    // Dialog Link
    $('#dialog_link').click(function () {
        $('#dialog').dialog('open');
        return false;
    });
});


function CloseModal() {
    $('#dialog').dialog('close');
}

function OpenModal() {
    $('#dialog').dialog('open');
}


$(function () {
    // Dialog
    $('#dialogInfo').dialog({
        autoOpen: false,
        position: 'center top',
        width: 450,
        modal: true,
        open: function (type, data) { $(this).parent().appendTo("form"); },
        buttons: {
            Fechar: function () {
                $(this).dialog("close");
            }
        }
    });

    // Dialog Link
    $('#dialog_link').click(function () {
        $('#dialogInfo').dialog('open');
        return false;
    });
});


function OpenCalendar(campo) {
    $('#ctl00_cphBody_hdnCalendarioEscolhido')[0].value = campo;
    var dataSelecionada = $('#ctl00_cphBody_' + campo)[0].value;

    $('#dialogCalendar').dialog({
        autoOpen: false,
        position: 'center top',
        width: 330,
        modal: true,
        open: function (type, data) { $(this).parent().appendTo("form"); $('.ui-widget-overlay').css('width', '100%'); $('body').css('overflow', 'hidden'); loadDate($('#dialogCalendar').data('dataSelecionada')); },
        close: function (type, data) { $("body").css("overflow", "auto"); },
        buttons: { }
    });

    // Calendar Link
    $('#dialogCalendar').data('dataSelecionada', dataSelecionada).dialog('open');
}

function CloseCalendar() {
    // Calendar Link
    $('#dialogCalendar').dialog('close');
}

function CloseModalNotification() {
    $('#dialogInfo').dialog('close');
}

function OpenModalNotification() {
    $('#dialogInfo').dialog('open');
}



$(function () {
    // Dialog
    $('#dialogItem').dialog({
        autoOpen: false,
        position: 'top',
        width: 700,
        modal: true,
        open: function (type, data) { $(this).parent().appendTo("form"); $('.ui-widget-overlay').css('width', '100%'); $('body').css('overflow', 'hidden'); },
        close: function (type, data) { $("body").css("overflow", "auto"); },
        buttons: {

        }
    });


    // Dialog Link
    $('#dialog_link').click(function () {
        $('#dialogItem').dialog('open');
        return false;
    });
});

function CloseModalItem() {
    $('#dialogItem').dialog('close');
}

function OpenModalItem() {
    $('#dialogItem').dialog('open');
}



$(function () {
    // Dialog
    $('#dialogSubItem').dialog({
        autoOpen: false,
        position: 'top',
        width: 620,
        modal: true,
        open: function (type, data) { $(this).parent().appendTo("form"); $('.ui-widget-overlay').css('width', '100%'); $('body').css('overflow', 'hidden'); },
        close: function (type, data) { $("body").css("overflow", "auto"); },
        buttons: {

        }
    });

    // Dialog Link
    $('#dialog_link').click(function () {
        $('#dialogSubItem').dialog('open');
        return false;
    });
});

function CloseModalSubItem() {
    $('#dialogSubItem').dialog('close');
}

function OpenModalSubItem() {
    $('#dialogSubItem').dialog('open');
}

//EntradaCampoCE
$(function () {
    // Dialog
    $('#dialogEntradaCampoCE').dialog({
        autoOpen: false,
        position: 'center',
        width: 300,
        modal: true,
        open: function (type, data) { $(this).parent().appendTo("form"); },
        buttons: { }
    });
    // Dialog Link
    $('#dialog_link').click(function () {
        $('#dialogEntradaCampoCE').dialog('open');
        return false;
    });
});
function CloseModalCampoSiafemCE() {
    $('#dialogEntradaCampoCE').dialog('close');
}
function OpenModalCampoSiafemCE() {
    $('#dialogEntradaCampoCE').dialog('open');
}

$(function () {
    // Dialog
    $('#dialogSenhaWS').dialog({
        autoOpen: false,
        position: 'center',
        width: 300,
        modal: true,
        open: function (type, data) { $(this).parent().appendTo("form"); },
        buttons: {

        }
    });


    // Dialog Link
    $('#dialog_link').click(function () {
        $('#dialogSenhaWS').dialog('open');
        return false;
    });
});

function CloseModalSenhaWs() {
    $('#dialogSenhaWS').dialog('close');
}

function OpenModalSenhaWs() {
    $('#dialogSenhaWS').dialog('open');
}

$(function () {
    // Dialog
    $('#dialogDoc').dialog({
        autoOpen: false,
        position: 'top',
        width: 780,
        modal: true,
        open: function (type, data) { $(this).parent().appendTo("form"); },
        buttons: {

        }
    });


    // Dialog Link
    $('#dialog_link').click(function () {
        $('#dialogDoc').dialog('open');
        return false;
    });
});

function CloseModalDoc() {
    $('#dialogDoc').dialog('close');
}

function OpenModalDoc() {
    $('#dialogDoc').dialog('open');
}

function RetornaCodigo(obj) {

    var codigoSelecionado = obj.innerHTML;
    var descricaoSelecionado = obj.title;

    $("#ctl00_cphBody_txtSubItem").attr("value", codigoSelecionado);
    $("#ctl00_cphBody_txtSubItemC").attr("value", codigoSelecionado);
    $("#ctl00_cphBody_txtDescricao").attr("value", descricaoSelecionado.split(";;")[1]);
    $("#ctl00_cphBody_idSubItem").attr("value", descricaoSelecionado.split(";;")[0]);
    $("#ctl00_cphBody_txtUnidadeForn").attr("value", descricaoSelecionado.split(";;")[2]);
    $("#ctl00_cphBody_txtUnidadeFornecimento").attr("value", descricaoSelecionado.split(";;")[2]);

    if ($("#ctl00_cphBody_txtItemMaterial") != null) {
        $("#ctl00_cphBody_txtItemMaterial").attr("value", descricaoSelecionado.split(";;")[3]);
    }

    CloseModal();
    CloseModalItem();
    CloseModalSubItem();
}

function RetornaCodigoItem(obj) {

    var codigoSelecionado = obj.innerHTML;
    var descricaoSelecionado = obj.title;

    $("#ctl00_cphBody_txtItem").attr("value", codigoSelecionado);
    $("#ctl00_cphBody_itemMaterialId").attr("value", descricaoSelecionado.split(";;")[0]);



    CloseModal();
    CloseModalItem();
}

function RetornaCodigoRequisicao(obj2) {

    $("#ctl00_cphBody_txtRequisicao").attr("value", obj2.innerHTML);
    $("#ctl00_cphBody_hdfMovimentoId").attr("value", obj2.title);
    $("#ctl00_cphBody_txtObservacoes").attr("value", obj2.attributes["value-obs"].value);
    if ($("#ctl00_cphBody_txtDataMovimento").attr("value") == "" || $("#ctl00_cphBody_txtDataMovimento").attr("value") == undefined)
        $("#ctl00_cphBody_txtDataMovimento").attr("value", obj2.attributes["value-obs2"].value);

    $("#ctl00_cphBody_txtDescricaoAvulsa").attr("value", obj2.attributes["value-obs3"].value);


    CloseModal();
}

function RetornaCodigoDocumento(obj3) {

    var obj = obj3.title;

    $("#ctl00_cphBody_txtDocumentoAvulso").attr("value", obj3.innerHTML);
    $("#ctl00_cphBody_hdfMovimentoId").attr("value", obj.split(";;")[0]);
    $("#ctl00_cphBody_txtAlmoxarifadoTransf").attr("value", obj.split(";;")[1]);
    $("#ctl00_cphBody_hdfAlmoxTransId").attr("value", obj.split(";;")[2]);
    $("#ctl00_cphBody_txtOrgao_Transferencia").attr("value", obj3.attributes["value-gerdesc"].value);
    
    CloseModalDoc();
}

function btnOkSenha() {
    $('#ctl00_cphBody_btnSenhaEmpenho').click();
}

function DesabilitarDuploClick() {
    $('form').submit(function () {
        if (typeof jQuery.data(this, "disabledOnSubmit") == 'undefined' || typeof jQuery.data(this, "disabledOnSubmit") == 'object') {
            jQuery.data(this, "disabledOnSubmit", { submited: true });
            $('input[type=submit], input[type=button]', this).each(function () {
                $(this).attr("disabled", "disabled");
            });
            return true;
        } else {
            return false;
        }
    });
}

function CloseModalConversaoUnidadeFornecimento() {
    $('#dialogConversaoUnidadeFornecimento').dialog('close');
}

function OpenModalConversaoUnidadeFornecimento() {
    $('#dialogConversaoUnidadeFornecimento').dialog('open');
}

$(function () {
    // Dialog
    $('#dialogConversaoUnidadeFornecimento').dialog({
        autoOpen: false,
        position: 'top',
        width: 780,
        modal: true,
        open: function (type, data) { $(this).parent().appendTo("form"); },
        buttons: {

        }
    });

    // Dialog Link
    $('#dialog_link').click(function () {
        $('#dialogConversaoUnidadeFornecimento').dialog('open');
        return false;
    });
});

$(function () {
    // Dialog
    $('#dialogInfoEmpenhoUnidadeFornecimento').dialog({
        autoOpen: false,
        position: 'center top',
        width: 450,
        modal: true,
        open: function (type, data) { $(this).parent().appendTo("form"); },
        buttons: {
            Fechar: function () {
                $(this).dialog("close");
            }
        }
    });

    // Dialog Link
    $('#dialog_link').click(function () {
        $('#dialogInfoEmpenhoUnidadeFornecimento').dialog('open');
        return false;
    });
});


function CloseModalInfoEmpenhoUnidadeFornecimento() {
    $('#dialogInfoEmpenhoUnidadeFornecimento').dialog('close');
}

function OpenModalInfoEmpenhoUnidadeFornecimento() {
    $('#dialogInfoEmpenhoUnidadeFornecimento').dialog('open');
}