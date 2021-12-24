
function WebForm_FireDefaultButton(event, target) {
    if (event.keyCode == 13 && !(event.srcElement && (event.srcElement.tagName.toLowerCase() == "textarea"))) {
        var defaultButton;
        if (__nonMSDOMBrowser) {
            defaultButton = document.getElementById(target);
        }
        else {
            defaultButton = document.all[target];
        }
        if (defaultButton && typeof (defaultButton.click) != "undefined") {
            defaultButton.click();
            event.cancelBubble = true;
            if (event.stopPropagation) event.stopPropagation();
            return false;
        }
    }
    return true;
}

function SomenteNumero(e) {

    $(".QtdeEmpenho").floatnumber(',', 3);
    $(".txtQtdeLote").floatnumber(',', 3);

    var tecla = (window.event) ? event.keyCode : e.which;
    if ((tecla > 47 && tecla < 58)) return true;
    else {
        if (tecla == 8 || tecla == 0) return true;
        else return false;
    }
}

function SomenteNumeroApenas(e) {

    var tecla = (window.event) ? event.keyCode : e.which;
    if ((tecla > 47 && tecla < 58)) return true;
    else {
        if (tecla == 8 || tecla == 0) return true;
        else return false;
    }
}

function CompararSaldoFornecido() {

    var saldo = parseInt($("#ctl00_cphBody_txtSaldo").val());
    var fornecido = parseInt($("#ctl00_cphBody_txtQtdeMov").val());

    if (!isNaN(fornecido)) {
        if (saldo >= fornecido) {
            $("#ctl00_cphBody_txtQtdeMov").attr("value", fornecido);
        }
        else {
            $("#ctl00_cphBody_txtQtdeMov").attr("value", saldo);
        }
        
    }
}

function SomenteNumeroDecimal(e) {

    $(".QtdeEmpenho").floatnumber(',', 3);
    $(".txtQtdeLote").floatnumber(',', 3);
    $(".txtEstoqueMaximo").floatnumber(',', 3);
    

    var tecla = (window.event) ? event.keyCode : e.which;
    //if ((tecla > 47 && tecla < 58) || tecla == 188 || tecla == 110 || tecla == 44) return true;
    if ((tecla > 47 && tecla < 58) || tecla == 44) return true;
    else {
        if (tecla == 8 || tecla == 0) return true;
        else return false;
    }
}

function CompararInserirSaldoFornecido() {

    var saldo = parseFloat($("#ctl00_cphBody_txtSaldo").val());
    var fornecido = parseInt($("#ctl00_cphBody_txtQtdeMov").val());

    var total = 0.000;
    total = parseFloat(total);
    var subtotal = "";

    $('.txtQtdeLote').each(function () {


        var obj = $(this);
        var valor = obj.val();
        var subSaldo = obj.attr('data-saldo');
        subSaldo = subSaldo.replace(",", ".");
        subSaldo = parseFloat(subSaldo);
        subtotal = 0;

        if (valor != "") {

            subtotal = valor.replace(",", ".");
            subtotal = parseFloat(subtotal);

            if (subtotal > subSaldo) {
                obj.val(subSaldo);
                subtotal = subSaldo;
            }
        }

        if (!isNaN(valor) || valor != "") {
            total += subtotal;
        }
    });

    if (total > 0) {
        if ($('.txtQtdeLote').length > 0) {

            total = total.toFixed(3);

            $("#ctl00_cphBody_txtQtdeMov").attr("value", total.replace(".", ","));

        } else {
            if (saldo >= fornecido) {
                $("#ctl00_cphBody_txtQtdeMov").attr("value", fornecido);
            }
            else {
                $("#ctl00_cphBody_txtQtdeMov").attr("value", saldo);
            }
        }

    }
}

function SetDate(dataEscolhida)
{
    var campo = $('#ctl00_cphBody_hdnCalendarioEscolhido')[0].value;
    $("#ctl00_cphBody_" + campo).attr("value", dataEscolhida);
    CloseCalendar();
}

function loadDate(dataSelecionada)
{
    var _arrData = [0, 0, 0];

    if (dataSelecionada != null && dataSelecionada != '01/01/0001')
        _arrData = dataSelecionada.split('/');
    else
    {
        var _date = new Date();
        _arrData[0] = _date.getDay();
        _arrData[1] = _date.getMonth() + 1;
        _arrData[2] = _date.getYear();
    }


    $("#ctl00_cphBody_uc2Calendar_Ddmonth").attr("value", (_arrData[1] < 10 ? "0" : "") + parseInt(_arrData[1]));
    $("#ctl00_cphBody_uc2Calendar_Ddyear").attr("value", _arrData[2]);
}

function preencheZeros(param, tamanho) {
    var contador = param.value.length;
    if (contador > 0) {
        if (param.value.length != tamanho) {
            do {
                param.value = "0" + param.value;
                contador += 1;

            } while (contador < tamanho)
        } 
    }
}

// função que limita o tamanho de caracteres no texto multiline
function limitarTamanhoTexto(campo, limiteMaximo) {
    var countfield = campo.value.length;
    if (campo.value.length > limiteMaximo)
        campo.value = campo.value.substring(0, limiteMaximo);
    else
        return campo.value
}

//Desativa o enter das telas
function onEnter(evt) {
    return ((evt.keyCode ? evt.keyCode : evt.charCode ? evt.charCode : evt.which ? evt.which : void 0) != 13);
}

// Adiciona Mascaras no sistema
        
//        $('.mask-data').mask('99/99/9999'); //data 
//        $('.mask-hora').mask('99:99'); //hora 
//        $('.mask-fone').mask('(999) 999-9999'); //telefone 
//        $('.mask-rg').mask('99.999.999-9'); //RG 
//        $('.mask-ag').mask('9999-9'); //Agência 
//        $('.mask-ag').mask('9.999-9'); //Conta 
//        $('.mask-zero2').mask('00'); //2 zero
//        $('.mask-zero4').mask('0000'); //4 zero
//        

function mvalor(v) {
    v = v.replace(/\D/g, ""); //Remove tudo o que não é dígito
    v = v.replace(/(\d)(\d{8})$/, "$1.$2"); //coloca o ponto dos milhões
    v = v.replace(/(\d)(\d{5})$/, "$1.$2"); //coloca o ponto dos milhares

    v = v.replace(/(\d)(\d{3})$/, "$1,$3"); //coloca a virgula antes dos 2 últimos dígitos
    return v;
}

function registerEvent(sTargetID, sEventName, fnHandler) {
    var oTarget = document.getElementById(sTargetID);
    if (oTarget != null) {
        if (oTarget.addEventListener) {
            oTarget.addEventListener(sEventName, fnHandler, false);
        } else {
            var sOnEvent = "on" + sEventName;
            if (oTarget.attachEvent) {
                oTarget.attachEvent(sOnEvent, fnHandler);
            }
        }
    }
}

function realClose() {
    var tab = window.open(window.location, "_top");
    tab.close();

    var win = window.open("", "_top", "", "true");
    win.opener = true;
    win.close();

    // Caso não consiga fechar a janela no Chrome ou Firefox, será direcionado para a página de login
    // O Fechamento da janela ou aba neste Browser não é possível por uma questão de segurança
    if (navegadorMicrosoft() == false)
        executeLogoff();
}

//var inFormOrLink;
//$('a').live('click', function () { inFormOrLink = true; });
//$('form').bind('submit', function () { inFormOrLink = true; });

//$(window).bind('beforeunload', function (eventObject) {
//    var returnValue = undefined;
//    if (!inFormOrLink) {
//        //returnValue = "Do you really want to close?";
//        console.log("Quer sair do site ???")
//    }
//    eventObject.returnValue = returnValue;
//    return returnValue;
//});

function navegadorMicrosoft() {
    var ua = window.navigator.userAgent.toUpperCase();

    return (ua.indexOf("MSIE ") >= 0 || ua.indexOf("EDGE ") >= 0);
}

function bloqueiaCopiarEColar() {
    var ctrl = window.event.ctrlKey;
    var tecla = window.event.keyCode;
    if (ctrl && tecla == 67) {
        event.keyCode = 0;
        event.returnValue = false;
    }
    if (ctrl && tecla == 86) {
        event.keyCode = 0;
        event.returnValue = false;
    }
}
