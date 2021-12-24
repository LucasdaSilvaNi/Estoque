var clicked = false;
var logoff = false;
var confirmationMessage = 'Tem certeza em fechar o Browser!';
var pathLogoffAutomatico = '';

function CheckBrowser() {
    if (clicked == false) {
        //Browser closed   
    } else {
        //redirected
        clicked = false;
    }
}
function executeLogoff() {
    setPathLogoffAutomatico(_redirecionaFechaSessao);
    $.get(this.pathLogoffAutomatico, function (data) { });
}

function GetRequest() {
    var xmlHttp = null;
    try {
        // Firefox, Opera 8.0+, Safari
        xmlHttp = new XMLHttpRequest();
    }
    catch (e) {
        //Internet Explorer
        try {
            xmlHttp = new ActiveXObject("Msxml2.XMLHTTP");
        }
        catch (e) {
            xmlHttp = new ActiveXObject("Microsoft.XMLHTTP");
        }
    }
    return xmlHttp;
}

function addEventListenerSistema(element, eventName, eventHandler) {
    //W3C DOM
    if (element.addEventListener) {
        element.addEventListener(eventName, eventHandler, false);
        //IE < 8 DOM
    } else if (element.attachEvent) {
        element.attachEvent('on' + eventName, eventHandler);
    }
}

//limpar o cache do usuario
function LogoffUsuario() {
    if (logoff === true || getControlCloseWindow() == _duplicated) {
        initTabCounter();
        executeLogoff();
    }
}

function getLogoffMensagem(e) {
    if (getControlCloseWindow() == _duplicated && logoff == false) {
        updateContentFieldTabController('');
        store.set(_tabCounter, 1);;
        setPathLogoffAutomatico(_redirecionaSemFecharSessao);
        $.get(this.pathLogoffAutomatico, function (data) { });
    }

    // Fechando a aba ou browser no Chrome ou Firefox
    if (logoff && !navegadorMicrosoft())
    {
        initTabCounter();
        executeLogoff();
    }
}

function initilize() {
    // Cria o Handler para o evento sair (Fechar o Browser)
    addEventListenerSistema(window, 'beforeunload', function (e) {
        getLogoffMensagem(e);
    });

    addEventListenerSistema(window, 'unload', function (e) {
        LogoffUsuario();
    });
}

function setLogoff(value) {
    logoff = value;
}

function setPathLogoffAutomatico(pagina) {
    this.pathLogoffAutomatico = pagina;
}


function setLogoffFalseTeclaF5(e) {
    if ((e.which || e.keyCode) == 116)
        setLogoff(false);
}

function resetCronometro(tempo) {
    tempoTimeout = tempo;
}

/*
    e.returnValue =mensagem;  // Gecko and Trident
    return mensagem;// Gecko and WebKit
*/

function updateStorageValue(key, value) {
    store.set(key, value);
}

function incrementTabCounter() {
    var _counter = parseInt(getTabCounter());
    if (_counter < 0) _counter = 0;
    updateStorageValue(_tabCounter, _counter + 1);
}

function decrementTabCounter() {
    var _counter = parseInt(getTabCounter()) - 1;
    if (_counter < 0) _counter = 0;
    if (getControlCloseWindow() == _duplicated) _counter = 1;

    store.set(_tabCounter, _counter);
}

function getTabCounter() {
    return _tabCounter == 'tabCounter_' ? 0 : store.get(_tabCounter);
}

function getControlCloseWindow() {
    // Caso não tenha realizado o login, retornará branco 
    return _tabController == 'tabController_' ? '' : store.get(_tabController);
}

function initTabCounter() {
    store.set(_tabCounter, 0);
    store.set(_tabController, '');
}

function limparCampoControleAba() {
    if (getControlCloseWindow() == 'pronto') {
        updateContentFieldTabController('');
    }
}

function verificarAbaDuplicada() {
    if (navegadorMicrosoft())
        setPathLogoffAutomatico(_redirecionaSemFecharSessao);
    else
        setPathLogoffAutomatico(_redirecionaFechaSessao);

    if (getControlCloseWindow() == '' && getTabCounter() <= 1) {
        updateContentFieldTabController('pronto');
    }
    else {
        if (window.location.href.toLowerCase().indexOf("login.aspx") < 0) {
            if (window.location.href.toLocaleLowerCase().lastIndexOf('/') == window.location.href.length - 1 && _sessaoAtiva != '2')
                return;

            updateContentFieldTabController(_duplicated);

            var _msg = "Não é permitido trabalhar com 2 ou mais abas simultâneamente.\n\n";

            if (navegadorMicrosoft() == false) _msg = _msg + 'Sua conexão foi encerrada!!!';

            alert(_msg);
            realClose();
        }
        else {
            updateContentFieldTabController('');
        }
    }
}
