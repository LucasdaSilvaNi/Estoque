using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.View;
using Sam.Domain.Entity;
using Sam.Domain.Business;
using Sam.Common.Util;
using System.ComponentModel;
using System.Web;
using System.Web.UI.WebControls;
using Sam.Business;
using Sam.Infrastructure;
using System.Linq.Expressions;
using Sam.Business.Importacao;
using System.IO;

namespace Sam.Presenter
{
    public class CargaPresenter : CrudPresenter<ICargaView> 
    {
        ICargaView view;

        public ICargaView View
        {
            get { return view; }
            set { view = value; }
        }

        public CargaPresenter()
        {
        }

        public CargaPresenter(ICargaView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public void CarregarArquivoUpload(CargaEntity cargaEntity, FileUpload fulArquivo)
        {
            string dataFormatada = String.Format("{0:d/MM/yyyy HH:mm:ss}", DateTime.Now);
            dataFormatada = dataFormatada.Replace("/", "").Replace(" ", "_").Replace(":", "");

            try
            {
                double tamanho = 0;
                string extensao;

                if (fulArquivo.PostedFile != null)
                {
                    tamanho = Convert.ToDouble(fulArquivo.PostedFile.ContentLength) / 1024;
                    extensao = cargaEntity.NomeArquivo.Substring(cargaEntity.NomeArquivo.Length - 4).ToLower();

                    cargaEntity.NomeArquivo = String.Format("{0}_{1}", dataFormatada, System.IO.Path.GetFileName(Helper.RemoverAcentos(cargaEntity.NomeArquivo)));

                    if (tamanho > cargaEntity.tamanhoMax)
                        throw new Exception("Tamanho Máximo permitido é de " + cargaEntity.tamanhoMax + " kb!");

                    if (cargaEntity.ExtensaoList.Where(a => a.ExtensaoArquivo == extensao).Count() == 0)
                        throw new Exception("Extensão inválida, só são permitidas .xls!");

                    if (!File.Exists(cargaEntity.CaminhoDiretorio + cargaEntity.NomeArquivo))
                        fulArquivo.PostedFile.SaveAs(cargaEntity.CaminhoDiretorio + cargaEntity.NomeArquivo);
                    
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                bool existeDetalhe = ex.InnerException.IsNotNull();
                var strDescricaoDetalhe = (existeDetalhe) ? ex.InnerException.Message : string.Empty;
                var fmtMsgErro = (existeDetalhe) ? "Erro (access denied) no Upload: {0}\nDetalhe: {1}" : "Erro no Upload:";

                throw new Exception(String.Format(fmtMsgErro, ex.Message, strDescricaoDetalhe));
            }

            catch (Exception ex)
            {
                bool existeDetalhe = ex.IsNotNull();
                var strDescricaoDetalhe = (existeDetalhe) ? ex.Message : string.Empty;
                var fmtMsgErro = (existeDetalhe) ? "Erro genérico no Upload: {0}\nDetalhe: {1}" : "Erro no Upload:";

                throw new Exception(String.Format(fmtMsgErro, ex.Message, strDescricaoDetalhe));
            }

        }

        public List<TB_CONTROLE> ListarControleCarga(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            startRowIndexParameterName = startRowIndexParameterName / 10; //Correcao de um erro que está multiplicando a paginacao por 10.

            Expression<Func<TB_CONTROLE, DateTime?>> sort = a => a.TB_CONTROLE_DATA_OPERACAO;            

            ControleBusiness business = new ControleBusiness();
            var result = business.SelectSituacao(sort, maximumRowsParameterName, startRowIndexParameterName);
            this.TotalRegistrosGrid = business.TotalRegistros;
            return result;
        }

        public List<TB_TIPO_CONTROLE> SelectTipoControle()
        {
            ControleBusiness controleBusiness = new ControleBusiness();
            return controleBusiness.SelectTipoControle();
        }

        public List<CargaEntity> ListarArquivosPendente(string diretorio)
        {
            try
            {
                var listArquivos = ImportacaoBusiness.ListarArquivosDiretorio(diretorio);
                return listArquivos;
            }
            catch (Exception ex)
            {
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");

                List<string> erros = new List<string>();
                erros.Add(ex.Message);
                this.View.ListaErros = erros;
                return null;
            }            
        }

        public void CarregarArquivo(CargaEntity cargaEntity, FileUpload fulArquivo)
        {
            try
            {
                ImportacaoBusiness objBusiness = new ImportacaoBusiness();

                if (!ValidarArquivo())
                return;

                if (objBusiness.ExisteDiretorio(cargaEntity.CaminhoDiretorio, true) && objBusiness.ExisteDiretorio(cargaEntity.CaminhoDiretorioDestino, true))
                    CarregarArquivoUpload(cargaEntity, fulArquivo);

                int LoginId = Acesso.Transacoes.Perfis[0].IdLogin;                
                //cargaEntity.CaminhoDiretorio = fulArquivo.PostedFile.FileName;                
                objBusiness.ProcessarExcel(cargaEntity, LoginId);
                GravadoSucesso();
                
                //Copiar os arquivos para a pasta de histórico
                //CopiarArquivo(cargaEntity);
                this.View.ExibirMensagem("Arquivo Inserido com sucesso!");                
                this.View.PopularGrid();
            }
            catch (Exception ex)
            {
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");

                List<string> erros = new List<string>();
                erros = ex.Message.BreakLine(new char[]{'\n'}).ToList();

                this.View.ListaErros = erros;
            }
        }

        public void RemoverCarga(int controleId)
        {
            try
            {
                ControleBusiness controleBusiness = new ControleBusiness();
                var controle = controleBusiness.SelectOne(a => a.TB_CONTROLE_ID == controleId);
                controle.TB_CONTROLE_SITUACAO_ID = (int)GeralEnum.ControleSituacao.Cancelado;
                controleBusiness.Update(controle);
                GravadoSucesso();

                this.View.ExibirMensagem("Carga cancelada com sucesso!");
                View.PopularGrid();
            }
            catch (Exception ex)
            {
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");

                List<string> erros = new List<string>();
                erros.Add(ex.Message);
                this.View.ListaErros = erros;
            }
        }

        private void CopiarArquivo(CargaEntity cargaTable)
        {
            try
            {
                new ImportacaoBusiness().CopiarArquivo(cargaTable);                
                this.View.LerArquivosPasta();
            }
            catch (Exception ex)
            {
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");

                List<string> erros = new List<string>();
                erros.Add(ex.Message);
                this.View.ListaErros = erros;
            }
        }

        public void ImportarArquivo(int controleId)
        {
            try
            {
                bool isErro = new  Sam.Business.Importacao.ImportacaoBusiness().ExecutarCarga(controleId);

                if (isErro)
                    this.View.ExibirMensagem("Arquivo Importado finalizado com erro!");
                else
                    this.View.ExibirMensagem("Arquivo Importado finalizado com sucesso!");

                this.View.PopularGrid();
                GravadoSucesso();
                    
            }
            catch (Exception ex)
            {
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");

                List<string> erros = new List<string>();

                string TB_LOGERRO_MESSAGE;
                TB_LOGERRO_MESSAGE = ex.Message;
                erros.Add(TB_LOGERRO_MESSAGE);
                this.View.ListaErros = erros;
                new LogErro().GravarLogErro(ex);
            }            
        }

        public void ExportarErrosCarga(int controleId)
        {

            try
            {
                var listErros = new CargaBusiness().RetornarCargaErros(controleId);

                if (listErros.Count() == 0)
                {
                    this.View.ExibirMensagem("Não há erros para serem exportados.");
                    return;
                }

                var listErroDescricao = new CargaBusiness().RetornaCargaErroDescricao(listErros);

                if (listErros.Count() == 0 || listErroDescricao.Count() == 0)
                {
                    this.View.ExibirMensagem("Não há erros para serem exportados.");
                }
                else
                {
                    this.View.ExportarToExcel(listErros, listErroDescricao);
                    GravadoSucesso();
                }

            }
            catch (Exception ex)
            {
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");

                List<string> erros = new List<string>();
                erros.Add(ex.Message);
                this.View.ListaErros = erros;
            }

        }

        public void ExportarCarga(int controleId)
        {
            try
            {
                var listErros = new CargaBusiness().RetornarCarga(controleId);                

                if (listErros.Count() == 0)
                {
                    this.View.ExibirMensagem("Não há registros para serem exportados.");
                }
                else
                {
                    this.View.ExportarToExcel(listErros, null);
                    GravadoSucesso();
                }

            }
            catch (Exception ex)
            {
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");

                List<string> erros = new List<string>();
                erros.Add(ex.Message);
                this.View.ListaErros = erros;
            }

        }

        public bool ValidarArquivo()
        {
            List<string> erros = new List<string>();

            if (erros.Count > 0)
            {
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                this.View.ListaErros = erros;
                return false;
            }
            else
                return true;
        }

        public override void Load()
        {
            base.Load();
            View.BloqueiaNovo = false;            
            this.View.LerArquivosPasta();
            this.View.PopularGrid();
        }

        public override void Novo()
        {
            base.Novo();
            View.BloqueiaNovo = true;
            View.BloqueiaCancelar = false;
            View.BloqueiaGravar = false;
        }

        public override void Cancelar()
        {
            base.Cancelar();
            View.BloqueiaNovo = false;
            View.BloqueiaCancelar = true;
            View.BloqueiaGravar = true;
        }

        public override void GravadoSucesso()
        {
            base.GravadoSucesso();
            View.BloqueiaNovo = false;
            View.BloqueiaCancelar = true;
            View.BloqueiaGravar = true;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int _orgaoId, int _gestorId)
        {
            return this.TotalRegistrosGrid;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            return this.TotalRegistrosGrid;
        }

    }
}
