using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Sam.Common.Util;
using Sam.Domain.Business;
using Sam.Domain.Entity;
using Sam.View;
using tipoPesquisa = Sam.Common.Util.GeralEnum.TipoPesquisa;
using statusAtendimentoChamado = Sam.Common.Enums.ChamadoSuporteEnums.StatusAtendimentoChamado;
using System.IO;
using System.Xml.Linq;
using Sam.Common;

namespace Sam.Presenter
{
    public class ChamadoSuportePresenter : CrudPresenter<IChamadoSuporteView> 
    {
        IChamadoSuporteView view;
        FileUpload fileUploader;
        HttpResponse webResponse;
        HttpContext webContext;
        ListBox listaNomesArquivosAnexos;

        private readonly string cstNomeTabela_ChamadosSuporte = "TB_CHAMADO_SUPORTE";
        private readonly string cstNomeAtributoCampo_AlteradoPor = "AlteradoPor";

        public IChamadoSuporteView View
        {
            get { return view; }
            set { view = value; }
        }

        public ChamadoSuportePresenter()
        {
        }
        public ChamadoSuportePresenter(IChamadoSuporteView _view) : base(_view)
        {
            this.View = _view;
        }

        public override void Load()
        {
            base.Load();
        }

        public bool Gravar(bool ehAdminGeral)
        {
            ChamadoSuporteBusiness objBusiness = null;
            ChamadoSuporteEntity objEntidade = null;

            objBusiness = new ChamadoSuporteBusiness();
            objEntidade = new ChamadoSuporteEntity();

            if (this.View.NumeroChamado > 0)
                objEntidade.Id = this.View.NumeroChamado;
            
            objEntidade.Almoxarifado = this.View.Almoxarifado;
            objEntidade.DataAbertura = this.View.DataAberturaChamado;
            objEntidade.DataFechamento = this.View.DataFechamentoChamado;
            objEntidade.CpfUsuario = this.View.CpfUsuario;
            objEntidade.CpfUsuarioLogado = this.View.CpfUsuarioLogado;
            objEntidade.NomeUsuario = this.View.NomeUsuario;
            objEntidade.EMailUsuario = this.View.EMailUsuario;
            objEntidade.SistemaModulo = this.View.SistemaModulo;
            objEntidade.Funcionalidade = new FuncionalidadeSistemaEntity(this.View.FuncionalidadeSistemaID);
            objEntidade.StatusChamadoAtendimentoProdesp = this.View.StatusChamadoAtendimentoProdesp;
            objEntidade.StatusChamadoAtendimentoUsuario = this.View.StatusChamadoAtendimentoUsuario;
            objEntidade.PerfilUsuarioAberturaChamado = this.View.PerfilUsuarioAberturaChamadoID;
            objEntidade.Responsavel = this.View.Responsavel;
            objEntidade.TipoChamado = this.View.TipoChamado;
            objEntidade.Observacoes = this.View.Observacoes;
            objEntidade.LogHistoricoAtendimento = this.View.LogHistoricoAtendimento;
            objEntidade.AmbienteSistema = this.View.AmbienteSistema;
            objEntidade.Divisao = this.View.Divisao;

            if (objEntidade.Divisao.IsNotNull() && objEntidade.Divisao.Id.HasValue)
                objEntidade.Divisao.Ua = this.View.Ua;


            if (this.View.Anexos.HasElements())
                objEntidade.Anexos = this.View.Anexos;


            bool chamadoGravado = objBusiness.GravarChamado(objEntidade, ehAdminGeral);
            bool? envioEmailOk = objBusiness.envioEmailOk;

            //Chamado não gravado por algum erro, e nem tentativa de envio de email realizada
            if (chamadoGravado == false && objBusiness.ListaErro.Any() && envioEmailOk == null)
            {
                this.View.MostrarPainelStatusAtualizar = false;
                this.View.MostrarPainelEdicao = true;
                this.View.ExibirMensagem("Registro com Inconsistências, verificar mensagens!");
                this.View.ListaErros = objBusiness.ListaErro;
                return false;
            }
            //Chamado gravado e tentativa de envio de email realizada
            else if (chamadoGravado == true && envioEmailOk != null)
            {
                string mensagem = string.Empty;

                //Envio de email realizado
                if (envioEmailOk == true)
                    mensagem = String.Format("Chamado Suporte #{0:D7} salvo com sucesso!", objEntidade.Id.Value);
                //Envio de email não realizado
                else
                    mensagem = String.Format("Chamado Suporte #{0:D7} salvo com sucesso! \\n\\nOBS: {1}", objEntidade.Id.Value, objBusiness.ListaErro[0]);

                this.View.NumeroChamado = objEntidade.Id.Value;
                this.View.PopularGrid();
                this.View.ExibirMensagem(mensagem);
                this.View.MostrarPainelEdicao = false;
                this.View.MostrarPainelStatusAtualizar = false;
                this.view.MostrarBotaoStatusAtualizar = true;
                return true;
            }


            return false;
        }

        public bool AtualizarStatus(List<ChamadoSuporteEntity> chamadosSuporteEntity, IChamadoSuporteView chamadoSuporteView, out IEnumerable<string> mensagensErro)
        {
            ChamadoSuporteBusiness _objBusiness = new ChamadoSuporteBusiness();
            ChamadoSuportePresenter _presenter = new ChamadoSuportePresenter(chamadoSuporteView);
            mensagensErro = Enumerable.Empty<string>();

            bool _retornoProcessamento = _objBusiness.GravarChamadosEmLote(chamadosSuporteEntity);

            if (!_retornoProcessamento)
                mensagensErro = _objBusiness.ListaErro;

            return _retornoProcessamento;
        }

        public void Excluir()
        {
            ChamadoSuporteBusiness objBusiness = new ChamadoSuporteBusiness();
            ChamadoSuporteEntity objEntidade = new ChamadoSuporteEntity();

            objBusiness.Entity.Id = this.View.NumeroChamado;
            objBusiness.Excluir();

            if (objBusiness.ListaErro.Count == 0)
            {
                if (this.View.IsNotNull())
                    this.View.ListaErros = objBusiness.ListaErro;

                this.View.PopularGrid();
                this.ExcluidoSucesso();
                this.View.ExibirMensagem("Registro excluído com Sucesso.");
            }
        }
        public void Imprimir()
        {
            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.Almoxarifado;
            relatorioImpressao.Nome = "rptAlmoxarifado.rdlc";
            relatorioImpressao.DataSet = "dsAlmoxarifado";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

        public override void RegistroSelecionado()
        {
            this.View.BloqueiaNovo = true;

            base.RegistroSelecionado();
        }
        public override void GravadoSucesso()
        {
            this.View.LimparCamposView();
            this.View.MostrarPainelEdicao = false;
            this.View.MostrarPainelStatusAtualizar = false;
            this.view.MostrarBotaoStatusAtualizar = true;
            this.View.BloqueiaCancelar = true;

            base.GravadoSucesso();
        }
        public override void ExcluidoSucesso()
        {
            this.View.LimparCamposView();

            base.ExcluidoSucesso();
        }
      
        public override void Novo()
        {
            this.View.LimparCamposView();
            //base.Novo();

            this.View.BloqueiaGravar = false;
            this.View.BloqueiaCancelar = false;
            this.View.BloqueiaCodigo = true;
            this.View.BloqueiaDescricao = true;
            this.View.BloqueiaExcluir = true;
            this.View.BloqueiaNovo = false;

            this.View.MostrarPainelEdicao = true;
            this.View.MostrarPainelStatusAtualizar = false;
            this.View.MostrarBotaoStatusAtualizar = false;
        }

        public void StatusAtualizar()
        {
            this.View.LimparCamposView();
            //base.Novo();

            this.View.BloqueiaGravar = false;
            this.View.BloqueiaCancelar = false;
            this.View.BloqueiaCodigo = true;
            this.View.BloqueiaDescricao = true;
            this.View.BloqueiaExcluir = true;
            this.View.BloqueiaNovo = false;

            this.View.MostrarPainelEdicao = false;
            this.View.MostrarPainelStatusAtualizar = true;
            this.View.MostrarBotaoStatusAtualizar = false;
        }

        public override void Cancelar()
        {
            this.View.LimparCamposView();
            this.View.MostrarBotaoStatusAtualizar = true;

            base.Cancelar();
        }

        public IList<ChamadoSuporteEntity> SelecionarChamados(tipoPesquisa tipoPesquisa, statusAtendimentoChamado statusChamadoAtendimentoProdesp, statusAtendimentoChamado statusChamadoAtendimentoUsuario, long linhaRegistroID, bool? chamadosAtivos)
        {
            IList<ChamadoSuporteEntity> lstRetorno = null;
            ChamadoSuporteBusiness objBusiness = null;

            objBusiness = new ChamadoSuporteBusiness();

            if (this.View.IsNotNull())
                this.View.ListaErros = objBusiness.ListaErro;
            
            lstRetorno = objBusiness.SelecionarChamados(tipoPesquisa, statusChamadoAtendimentoProdesp, statusChamadoAtendimentoUsuario, linhaRegistroID, chamadosAtivos);

            if (lstRetorno.IsNotNullAndNotEmpty())
                lstRetorno.ToList().ForEach(chamado => chamado.NomeUsuario = (Sam.Business.UsuarioBusiness.RecuperarInformacoesUsuario(chamado.CpfUsuario.ToString()).IsNotNullAndNotEmpty() ? Sam.Business.UsuarioBusiness.RecuperarInformacoesUsuario(chamado.CpfUsuario.ToString()).FirstOrDefault().NomeUsuario : null));

            return lstRetorno;
        }
        public ChamadoSuporteEntity ObterChamadoSuporte(int chamadoSuporteID)
        {
            ChamadoSuporteEntity objEntidade = null;

            ChamadoSuporteBusiness objBusiness = new ChamadoSuporteBusiness();
            objEntidade = objBusiness.ObterChamadoSuporte(chamadoSuporteID);

            if (objEntidade.CpfUsuario != 0 && String.IsNullOrWhiteSpace(objEntidade.NomeUsuario))
            {
                var listaInfoUsuario = Sam.Business.UsuarioBusiness.RecuperarInformacoesUsuario(objEntidade.CpfUsuario.ToString());

                if (listaInfoUsuario.HasElements())
                    objEntidade.NomeUsuario = listaInfoUsuario.FirstOrDefault().NomeUsuario;
            }

            return objEntidade;
        }
        public int ObterNumeroChamados()
        {
            int _numRetorno = -1;
            ChamadoSuporteBusiness objBusiness = null;

            objBusiness = new ChamadoSuporteBusiness();
            _numRetorno = objBusiness.ObterNumeroChamados();

            return _numRetorno;
        }

        private bool extensaoValida(string nomeArquivo)
        {
            bool blnRetorno = false;
            ChamadoSuporteBusiness objBusiness = null;

            objBusiness = new ChamadoSuporteBusiness();
            blnRetorno = objBusiness.ExtensaoValida(nomeArquivo);

            return blnRetorno;
        }
        public bool VerificarSeArquivoValido()
        {
            this.fileUploader = (FileUpload)this.View.FileUploader;
            bool blnRetorno = false;

            blnRetorno = (this.fileUploader.HasFile || !String.IsNullOrWhiteSpace(this.fileUploader.FileName));

            if (!blnRetorno)
                return blnRetorno;

            blnRetorno &= this.extensaoValida(this.fileUploader.FileName);

            return blnRetorno;
        }
        public bool ArquivoJaAnexado()
        {
            bool blnRetorno = false;

            blnRetorno = (this.View.ListaArquivosAnexados.Contains(fileUploader.FileName));

            return blnRetorno;
        }
        private bool validaTamanhoTotalAnexos(AnexoChamadoSuporte arquivoUploaded)
        {
            bool blnRetorno = false;
            decimal tamanhoTotalAnexos = 0.00m;

            tamanhoTotalAnexos += ((this.View.Anexos.IsNotNullAndNotEmpty()) ? ((this.View.Anexos.Sum(anexo => (decimal)anexo.ConteudoArquivo.Length) / 1024.00m) / 1024.00m) : 0.00m);
            blnRetorno = ((tamanhoTotalAnexos + arquivoUploaded.TamanhoMB) > 10.00m);

            return blnRetorno;
        }

        public void AnexarArquivo()
        {
            if (this.View.Anexos.IsNullOrEmpty())
                this.View.Anexos = new List<AnexoChamadoSuporte>();

            if (String.IsNullOrWhiteSpace(this.View.ArquivoSelecionadoParaUpload))
            {
                this.View.ExibirMensagem("Favor selecionar arquivo para upload!");
            }
            else if (this.VerificarSeArquivoValido() && !this.ArquivoJaAnexado() && this.comprimentoNomeArquivoValido())
            {
                if (this.View.Anexos.Count < 3)
                {
                    this.CarregarArquivoUpload();

                    this.View.IsUploadingFile = true;
                }
                else if (this.View.Anexos.Count == 3)
                {
                    this.View.BloqueiaBotaoAdicionarAnexo = true;
                    this.View.ExibirMensagem("Limite de arquivos excedido!");
                }
            }
            else if (!this.comprimentoNomeArquivoValido())
            {
                this.View.ExibirMensagem(String.Format("Arquivo possui comprimento de nome superior a {0} caracteres!", Constante.CST_COMPRIMENTO_MAXIMO_NOME_ARQUIVO_ANEXO_CHAMADO_SUPORTE));
            }
            else if (this.ArquivoJaAnexado())
            {
                this.View.ExibirMensagem("Arquivo já adicionado a este chamado!");
            }
            else if (!this.VerificarSeArquivoValido())
            {
                this.View.ExibirMensagem("Extensão de arquivo não-permitida!");
            }

        }

        private bool comprimentoNomeArquivoValido()
        {
            this.fileUploader = (FileUpload)this.View.FileUploader;
            bool blnRetorno = false;

            blnRetorno = (this.fileUploader.FileName.BreakLine(0).Length <= Constante.CST_COMPRIMENTO_MAXIMO_NOME_ARQUIVO_ANEXO_CHAMADO_SUPORTE);

            if (!blnRetorno)
                return blnRetorno;

            blnRetorno &= this.extensaoValida(this.fileUploader.FileName);

            return blnRetorno;
        }

        public void CarregarArquivoUpload()
        {
            AnexoChamadoSuporte arquivoParaAnexar = new AnexoChamadoSuporte();

            this.fileUploader = (FileUpload)this.View.FileUploader;
            
            var arquivoUploaded = this.fileUploader.PostedFile;
            var streamArquivo = arquivoUploaded.InputStream;
            byte[] conteudoArquivo = null;


            conteudoArquivo = new byte[arquivoUploaded.ContentLength];
            streamArquivo.Read(conteudoArquivo, 0, arquivoUploaded.ContentLength);


            arquivoParaAnexar.NomeArquivo = Path.GetFileName(arquivoUploaded.FileName); //Incluso devido a comportamento diferenciado do controle FileUpload, no IE7 (sempre a M$!)
            arquivoParaAnexar.ConteudoArquivo = conteudoArquivo;

            if(validaTamanhoTotalAnexos(arquivoParaAnexar))
                this.View.ExibirMensagem("Tamanho total dos anexos não pode exceder 10MB!");

            this.InserirAnexo(arquivoParaAnexar);
        }

        public void InserirAnexo(AnexoChamadoSuporte arquivoAnexo)
        {
            AnexoChamadoSuporte anexoEmMemoria = null;

            listaNomesArquivosAnexos = (ListBox)this.View.RelacaoArquivosAnexados;
            if (arquivoAnexo.IsNotNull())
            {
                anexoEmMemoria = arquivoAnexo;

                if (this.View.Anexos.IsNullOrEmpty())
                    this.View.Anexos = new List<AnexoChamadoSuporte>();

                this.View.Anexos.Add(arquivoAnexo);
                this.listaNomesArquivosAnexos.Items.Add(new ListItem(arquivoAnexo.NomeArquivo));
            }

            this.View.BloqueiaBotaoAdicionarAnexo = (this.View.Anexos.Count == 3);
            this.View.BloqueiaBotaoDownloadAnexos = (this.View.Anexos.Count == 0);
        }
        public void RemoverAnexo()
        {
            AnexoChamadoSuporte anexoEmMemoria = null;
            string nomeArquivoAnexo = null;

            var terceiroAnexo = (this.View.ListaArquivosAnexados.Count < 3);

            nomeArquivoAnexo = this.View.AnexoSelecionado;
            listaNomesArquivosAnexos = (ListBox)this.View.RelacaoArquivosAnexados;
            if (this.View.IsNotNull() && this.View.Anexos.IsNotNullAndNotEmpty() && !String.IsNullOrWhiteSpace(nomeArquivoAnexo))
            {
                anexoEmMemoria = this.View.Anexos.Where(anexo => anexo.NomeArquivo == nomeArquivoAnexo).FirstOrDefault();

                if (anexoEmMemoria.IsNotNull())
                {
                    this.View.Anexos.Remove(anexoEmMemoria);
                    this.listaNomesArquivosAnexos.Items.Remove(anexoEmMemoria.NomeArquivo);
                }

                this.View.BloqueiaBotaoAdicionarAnexo = !terceiroAnexo;
                this.View.BloqueiaBotaoDownloadAnexos = (this.View.Anexos.Count == 0);
            }
        }
        private byte[] processarAnexoParaDownload()
        {
            byte[] arquivoParaDownload = null;
            AnexoChamadoSuporte anexoEmMemoria = null;
            string nomeArquivoAnexo = null;


            nomeArquivoAnexo = this.View.AnexoSelecionado;
            listaNomesArquivosAnexos = (ListBox)this.View.RelacaoArquivosAnexados;

            if (this.View.IsNotNull() && this.View.Anexos.IsNotNullAndNotEmpty() && !String.IsNullOrWhiteSpace(nomeArquivoAnexo))
                anexoEmMemoria = this.View.Anexos.Where(anexo => anexo.NomeArquivo == nomeArquivoAnexo).FirstOrDefault();

            if (anexoEmMemoria.IsNotNull())
                arquivoParaDownload = anexoEmMemoria.ConteudoArquivo;

            return arquivoParaDownload;
        }

        public string ProcessarHistoricoAtendimento(string xmlLog)
        {
            string strRetorno = null;
            ChamadoSuporteBusiness objBusiness = null;

            objBusiness = new ChamadoSuporteBusiness();
            strRetorno = objBusiness.ProcessarHistoricoAtendimento(xmlLog);

            return strRetorno;
        }


        public bool UltimaInteracaoDoUsuario(string xmlLog)
        {
            if (!String.IsNullOrWhiteSpace(xmlLog) && !XmlUtil.IsXML(xmlLog))
                throw new Exception("Formato inválido");

            #region Variaveis
            DateTime dataAlteracao = new DateTime(0);

            IList<XElement> listaAlteracoes = null;
            XElement ultimaRelacaoAlteracoes = null;
            XDocument docXml = null;
            bool blnRetorno = false;
            #endregion

            docXml = XDocument.Parse(xmlLog);
            listaAlteracoes = docXml.Element(cstNomeTabela_ChamadosSuporte).Elements().ToList();

            if (listaAlteracoes.HasElements() && listaAlteracoes.Count() >= 3)
            {
                var _listaAdminGeral = new UsuarioPresenter().ListarAdminGeral();
                
                //Iteracao Usuario
                ultimaRelacaoAlteracoes = listaAlteracoes[listaAlteracoes.Count() - 1];

                blnRetorno = ultimaRelacaoAlteracoes.Attribute(cstNomeAtributoCampo_AlteradoPor).IsNotNull() ?
                              _listaAdminGeral.Count(admin => admin.Cpf.Contains((ultimaRelacaoAlteracoes as XElement).Attribute(cstNomeAtributoCampo_AlteradoPor).Value)) == 0 : false;
            }

            return blnRetorno;
        }



        public void IniciarDownload()
        {
            byte[] arquivoParaDownload = null;

            arquivoParaDownload = this.processarAnexoParaDownload();
            if (this.View.IsNotNull() && arquivoParaDownload.IsNotNull())
            {
                this.webContext = (HttpContext)this.View.WebContext;

                webContext.Response.Clear();
                webContext.Response.ClearHeaders();
                webContext.Response.ClearContent();
                webContext.Response.AppendHeader("content-length", arquivoParaDownload.Length.ToString());
                webContext.Response.ContentType = this.obterContentTypeAnexo();
                webContext.Response.AppendHeader("content-disposition", String.Format(@"attachment; filename=""{0}""", this.View.AnexoSelecionado));
                webContext.Response.BinaryWrite(arquivoParaDownload);

                webContext.ApplicationInstance.CompleteRequest();
            }
        }

        private string obterContentTypeAnexo()
        {
            string strRetorno = null;
            string nomeAnexo = null;
            ChamadoSuporteBusiness objBusiness = null;

            objBusiness = new ChamadoSuporteBusiness();
            nomeAnexo = this.View.AnexoSelecionado;
            strRetorno = objBusiness.ObterContentTypePorExtensao(nomeAnexo);

            return strRetorno;
        }
    }
}

