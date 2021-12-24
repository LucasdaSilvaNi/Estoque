using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Web.UI;
using Microsoft.Reporting.WebForms;
using Sam.Common.Util;
using Sam.Domain.Entity;
using Sam.Presenter;
using formatoRelatorio = Sam.Common.Util.GeralEnum.FormatoExportacaoRelatorio;
using TipoPerfil = Sam.Common.Util.GeralEnum.TipoPerfil;
using Sam.Domain.Entity.Relatorios;
using System.Data;



namespace Sam.Web.Relatorios
{
    public partial class imprimirRelatorio : Page
    {
        private readonly string _chaveUsuarioImpressao = String.Format("impressao{0}", new PageBase().GetAcesso.LoginBase);
        private RelatorioEntity relatorioImpressao = null;
        private const string CONSTFORMATOSIAFEM = "FormatoSIAFEM";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    ProcessarRelatorios();
                }
                catch (ArgumentException argExc)
                {
                    Response.Redirect("ConsultaNaoGerada.html", false);
                }
                catch (Exception exc)
                {
                    Response.Redirect("ConsultaSemResultado.aspx", false);
                }
            }
        }

        private void GerarRelatorio<T>(T listaDataSource, List<ReportParameter> param)
        {
            var relatorioDestaUnidade = true;
            var existeAlmoxRelatorio = relatorioImpressao.Parametros.ExisteParametroValor("AlmoxId");
            var almoxLogadoId = (int)new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id;
            var tipoPerfilLogado = (int)new PageBase().GetAcesso.Transacoes.Perfis[0].IdPerfil;

            //if (existeAlmoxRelatorio)
            //    relatorioDestaUnidade &= (almoxLogadoId == ExtensionMethods.obterAlmoxarifadoId(listaDataSource as IList));
            try
            {
                if (relatorioImpressao.IsNotNull() && relatorioDestaUnidade)
                {
                    rptViewer.LocalReport.Refresh();
                    rptViewer.ShowParameterPrompts = false;

                    rptViewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                    rptViewer.LocalReport.ReportPath = Server.MapPath(Constante.ReportPath + relatorioImpressao.Nome);
                    rptViewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource(relatorioImpressao.DataSet, listaDataSource));

                    rptViewer.ShowPrintButton = true;
                    rptViewer.AsyncRendering = true;
                    rptViewer.SizeToReportContent = true;

                    if (tipoPerfilLogado != (int)TipoPerfil.AdministradorGeral)
                    {
                        DesabilitarFormatoParaExportacao(rptViewer, formatoRelatorio.Excel);
                        DesabilitarFormatoParaExportacao(rptViewer, formatoRelatorio.Word);
                    }

                    // Remove o parâmetro FormatoSiafem dos relatórios que não o possuem, 
                    // evitando que a exceção seja disparada
                    if (!rptViewer.LocalReport.GetParameters().Any(r => r.Name.ToLower() == CONSTFORMATOSIAFEM.ToLower()))
                    {
                        var _item = (from _param in param
                                     where _param.Name == CONSTFORMATOSIAFEM
                                    select _param).FirstOrDefault();

                        param.Remove(_item);
                    }

                    rptViewer.LocalReport.SetParameters(param);
                    rptViewer.DataBind();
                    rptViewer.ServerReport.Refresh();
                }
                else if (!relatorioDestaUnidade)
                {
                    throw new ArgumentException("Erro ao gerar relatório (outro almoxarifado/colisão sessões).");
                }
                else
                {
                    throw new Exception("Erro ao gerar relatório (instanciamento objeto saída).");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void GerarExportacao<T>(T listaDataSource, List<ReportParameter> param)
        {
            var relatorioDestaUnidade = true;
            var existeAlmoxRelatorio = relatorioImpressao.Parametros.ExisteParametroValor("AlmoxId");
            var almoxLogadoId = (int)new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id;
            var tipoPerfilLogado = (int)new PageBase().GetAcesso.Transacoes.Perfis[0].IdPerfil;

            try
            {
                if (relatorioImpressao.IsNotNull() && relatorioDestaUnidade)
                {
                    rptViewer.LocalReport.Refresh();
                    rptViewer.ShowParameterPrompts = false;

                    rptViewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                    rptViewer.LocalReport.ReportPath = Server.MapPath(Constante.ReportPath + relatorioImpressao.Nome);
                    rptViewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource(relatorioImpressao.DataSet, listaDataSource));

                    rptViewer.ShowPrintButton = true;
                    rptViewer.AsyncRendering = true;
                    rptViewer.SizeToReportContent = true;

                    if (tipoPerfilLogado != (int)TipoPerfil.AdministradorGeral)
                    {
                        //DesabilitarFormatoParaExportacao(rptViewer, formatoRelatorio.Excel);
                        DesabilitarFormatoParaExportacao(rptViewer, formatoRelatorio.Word);
                        DesabilitarFormatoParaExportacao(rptViewer, formatoRelatorio.PDF);
                    }

                    rptViewer.LocalReport.SetParameters(param);
                    rptViewer.DataBind();

                    rptViewer.ServerReport.Refresh();
                }
                else if (!relatorioDestaUnidade)
                {
                    throw new ArgumentException("Erro ao gerar relatório (outro almoxarifado/colisão sessões).");
                }
                else
                {
                    throw new Exception("Erro ao gerar relatório (instanciamento objeto saída).");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void ProcessarRelatorios()
        {
            if (relatorioImpressao.IsNotNull())
                this.relatorioImpressao = null;

            relatorioImpressao = PageBase.GetSession<RelatorioEntity>(_chaveUsuarioImpressao);
            var dadosAcessoUsuario = new PageBase().GetAcesso;

            if (relatorioImpressao.IsNotNull())
            {
                List<ReportParameter> param = new List<ReportParameter>();

                string strLoginUsuario = dadosAcessoUsuario.NomeUsuario;
                string strGestorUsuario = dadosAcessoUsuario.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Nome;

                relatorioImpressao.Parametros.ValidarParametro("NomeUsuario", strLoginUsuario);
                relatorioImpressao.Parametros.ValidarParametro("NomeGestor", strGestorUsuario);

                for (int i = 0; i < relatorioImpressao.Parametros.Count; i++)
                {
                    if (relatorioImpressao.Parametros.GetByIndex(i) != null)
                    {
                        param.Add(new ReportParameter(relatorioImpressao.Parametros.GetKey(i).ToString(),
                            relatorioImpressao.Parametros.GetByIndex(i).ToString()));
                    }
                }

                param.Add(new ReportParameter(CONSTFORMATOSIAFEM, (new PageBase().FormatoSIAFEM()).ToString()));


                switch (relatorioImpressao.Id)
                {
                    case (int)RelatorioEnum.Orgao:
                        {
                            OrgaoPresenter orgao = new OrgaoPresenter();
                            GerarRelatorio((IEnumerable<OrgaoEntity>)orgao.PopularDadosRelatorio(), param);
                        }
                        break;
                    case (int)RelatorioEnum.UO:
                        {
                            UOPresenter uo = new UOPresenter();
                            GerarRelatorio((IEnumerable<UOEntity>)uo.PopularDadosRelatorio(int.Parse(param[0].Values[0].ToString())), param);
                        }
                        break;
                    case (int)RelatorioEnum.UA:
                        {
                            UAPresenter ua = new UAPresenter();
                            GerarRelatorio((IEnumerable<UAEntity>)ua.PopularDadosRelatorio(int.Parse(param[1].Values[0].ToString())), param);
                        }
                        break;
                    case (int)RelatorioEnum.Gestor:
                        {
                            GestorPresenter gestor = new GestorPresenter();
                            GerarRelatorio((IEnumerable<GestorEntity>)gestor.PopularDadosRelatorio(int.Parse(param[0].Values[0].ToString())), param);
                        }
                        break;
                    case (int)RelatorioEnum.UGE:
                        {
                            UGEPresenter uge = new UGEPresenter();
                            GerarRelatorio((IEnumerable<UGEEntity>)uge.PopularDadosRelatorio(int.Parse(param[0].Values[0].ToString()), int.Parse(param[1].Values[0].ToString())), param);
                        }
                        break;

                    case (int)RelatorioEnum.Unidade:
                        {
                            UnidadePresenter unidade = new UnidadePresenter();
                            GerarRelatorio((IEnumerable<UnidadeEntity>)unidade.PopularDadosRelatorio(int.Parse(param[1].Values[0].ToString()), int.Parse(param[0].Values[0].ToString())), param);
                        }
                        break;

                    case (int)RelatorioEnum.CentroCusto:
                        {
                            CentroCustoPresenter centroCusto = new CentroCustoPresenter();
                            GerarRelatorio((IEnumerable<CentroCustoEntity>)centroCusto.PopularDadosRelatorio(int.Parse(param[1].Values[0].ToString()), int.Parse(param[0].Values[0].ToString())), param);
                        }
                        break;

                    case (int)RelatorioEnum.Classe:
                        {
                            ClassePresenter classe = new ClassePresenter();
                            GerarRelatorio((IEnumerable<ClasseEntity>)classe.PopularDadosRelatorio(int.Parse(param[0].Values[0].ToString())), param);
                        }
                        break;
                    case (int)RelatorioEnum.Responsavel:
                        {
                            ResponsavelPresenter responsavel = new ResponsavelPresenter();
                            GerarRelatorio((IEnumerable<ResponsavelEntity>)responsavel.PopularDadosRelatorio(int.Parse(param[1].Values[0].ToString()), int.Parse(param[0].Values[0].ToString())), param);
                        }
                        break;

                    case (int)RelatorioEnum.GrupoMaterial:
                        {
                            GrupoPresenter grupo = new GrupoPresenter();
                            GerarRelatorio((IEnumerable<GrupoEntity>)grupo.PopularDadosRelatorio(), param);
                        }
                        break;

                    case (int)RelatorioEnum.Divisao:
                        {
                            DivisaoPresenter divisao = new DivisaoPresenter();
                            GerarRelatorio((IEnumerable<DivisaoEntity>)divisao.PopularDadosRelatorio(int.Parse(param[0].Values[0].ToString()), int.Parse(param[1].Values[0].ToString())), param);
                        }
                        break;
                    case (int)RelatorioEnum.ConsultaDivisao:
                        {
                            DivisaoPresenter divisao = new DivisaoPresenter();
                            var UOId = param.Where(a => a.Name == "UOId").FirstOrDefault().Values[0].ToString();
                            var UGEId = param.Where(a => a.Name == "UGEId").FirstOrDefault().Values[0].ToString();
                            GerarRelatorio((IEnumerable<DivisaoEntity>)divisao.ImprimirDivisaoByGestor(new PageBase().GetAcesso.Transacoes.Perfis[0].GestorPadrao.Id.Value, TratamentoDados.TryParseInt32(UOId), TratamentoDados.TryParseInt32(UGEId)), param);
                        }
                        break;
                    case (int)RelatorioEnum.Almoxarifado:
                        {
                            AlmoxarifadoPresenter almoxarifado = new AlmoxarifadoPresenter();
                            GerarRelatorio((IEnumerable<AlmoxarifadoEntity>)almoxarifado.PopularDadosRelatorio(int.Parse(param[1].Values[0].ToString()), int.Parse(param[0].Values[0].ToString())), param);
                        }
                        break;

                    case (int)RelatorioEnum.Material:
                        {
                            MaterialPresenter material = new MaterialPresenter();
                            GerarRelatorio((IEnumerable<MaterialEntity>)material.PopularDadosRelatorio(int.Parse(param[0].Values[0].ToString())), param);
                        }
                        break;
                    case (int)RelatorioEnum.ItemMaterial:
                        {
                            ItemMaterialPresenter itemMaterial = new ItemMaterialPresenter();
                            GerarRelatorio((IEnumerable<ItemMaterialEntity>)itemMaterial.PopularDadosRelatorio(int.Parse(param[2].Values[0].ToString())), param);
                        }
                        break;

                    case (int)RelatorioEnum.NaturezaDespesa:
                        {
                            NaturezaDespesaPresenter naturezaDespesa = new NaturezaDespesaPresenter();
                            GerarRelatorio((IEnumerable<NaturezaDespesaEntity>)naturezaDespesa.PopularDadosRelatorio(), param);
                        }
                        break;
                    case (int)RelatorioEnum.UnidadeFornecimento:
                        {
                            UnidadeFornecimentoPresenter unidadeFornecimento = new UnidadeFornecimentoPresenter();
                            GerarRelatorio((IEnumerable<UnidadeFornecimentoEntity>)unidadeFornecimento.PopularDadosRelatorio(int.Parse(param[1].Values[0].ToString()), int.Parse(param[0].Values[0].ToString())), param);
                        }
                        break;

                    case (int)RelatorioEnum.UnidadeFornecimentoSiafem:
                        {
                            param.Clear();
                            UnidadeFornecimentoSiafemPresenter unidadeFornecimento = new UnidadeFornecimentoSiafemPresenter();
                            GerarRelatorio((IEnumerable<UnidadeFornecimentoSiafemEntity>)unidadeFornecimento.PopularDadosUnidadeFornecimentoTodosCod(), param);
                        }
                        break;

                    case (int)RelatorioEnum.SubitensMaterial:
                        {
                            SubItemMaterialPresenter subitensMaterial = new SubItemMaterialPresenter();
                            var itemId = param.Where(a => a.Name == "CodigoItem").FirstOrDefault().Values[0].ToString();
                            var gestorId = param.Where(a => a.Name == "CodigoGestor").FirstOrDefault().Values[0].ToString();
                            GerarRelatorio((IEnumerable<SubItemMaterialEntity>)subitensMaterial.PopularDadosRelatorio(Convert.ToInt32(itemId), Convert.ToInt32(gestorId)), param);
                        }
                        break;

                    case (int)RelatorioEnum.ConsultaSubitemMaterial:
                        {
                            SubItemMaterialPresenter subitensMaterial = new SubItemMaterialPresenter();
                            var naturezaCodigo = param.Where(a => a.Name == "NaturezaDespesa").FirstOrDefault().Values[0].ToString();
                            string itemCodigo = param.Where(a => a.Name == "ItemMaterialCodigo").FirstOrDefault().Values[0].ToString();
                            string subitemCodigo = param.Where(a => a.Name == "SubitemMaterialCodigo").FirstOrDefault().Values[0].ToString();

                            if (naturezaCodigo == "0")
                            {
                                naturezaCodigo = null;
                                int indexNaturezaCodigo = param.FindIndex(a => a.Name == "NaturezaDespesa");
                                param.RemoveAt(indexNaturezaCodigo);
                            }

                            GerarRelatorio((IEnumerable<SubItemMaterialEntity>)subitensMaterial.PopularDadosRelatorioConsulta(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value, TratamentoDados.TryParseInt32(naturezaCodigo), TratamentoDados.TryParseInt32(itemCodigo), TratamentoDados.TryParseLong(subitemCodigo)), param);
                        }
                        break;

                    case (int)RelatorioEnum.GerenciaCatalogo:
                        {
                            SubItemMaterialPresenter subitensMaterial = new SubItemMaterialPresenter();
                            var itemId = param.Where(a => a.Name == "CodigoItem").FirstOrDefault().Values[0].ToString();
                            var gestorId = param.Where(a => a.Name == "CodigoGestor").FirstOrDefault().Values[0].ToString();
                            var almoxId = param.Where(a => a.Name == "CodigoAlmoxarifado").FirstOrDefault().Values[0].ToString();
                            var materialId = param.Where(a => a.Name == "CodigoMaterial").FirstOrDefault().Values[0].ToString();
                            GerarRelatorio((IEnumerable<SubItemMaterialEntity>)subitensMaterial.PopularDadosRelatorioGerenciaCatalogo(
                                 Convert.ToInt32(materialId.ToString())
                                , Convert.ToInt32(itemId.ToString())
                                , Convert.ToInt32(gestorId.ToString())
                                , Convert.ToInt32(almoxId.ToString()))
                                , param);
                        }
                        break;

                    case (int)RelatorioEnum.ContaAuxiliar:
                        {
                            ContaAuxiliarPresenter contaAuxiliar = new ContaAuxiliarPresenter();
                            GerarRelatorio((IEnumerable<ContaAuxiliarEntity>)contaAuxiliar.PopularDadosRelatorio(), param);
                        }
                        break;

                    case (int)RelatorioEnum.Fornecedores:
                        {
                            FornecedorPresenter fornecedor = new FornecedorPresenter();
                            GerarRelatorio((IEnumerable<FornecedorEntity>)fornecedor.PopularDadosRelatorio(), param);
                        }
                        break;

                    case (int)RelatorioEnum.RelacaoItemSubitem:
                        {
                            RelacaoMaterialItemSubItemPresenter relacaoMaterialItemSubitem = new RelacaoMaterialItemSubItemPresenter();
                            var itemId = param.Where(a => a.Name == "CodigoItem").FirstOrDefault().Values[0].ToString();
                            var subItemId = param.Where(a => a.Name == "CodigoSubItem").FirstOrDefault().Values[0].ToString();
                            var gestorId = param.Where(a => a.Name == "CodigoGestor").FirstOrDefault().Values[0].ToString();

                            int nitemId = 0;
                            int nSubitemId = 0;
                            int nGestorId = 0;
                            if (int.TryParse(itemId.ToString(), out nitemId))
                                nitemId = int.Parse(itemId.ToString());

                            if (int.TryParse(subItemId.ToString(), out nSubitemId))
                                nSubitemId = int.Parse(subItemId.ToString());

                            if (int.TryParse(gestorId.ToString(), out nGestorId))
                                nGestorId = int.Parse(gestorId.ToString());

                            GerarRelatorio((IEnumerable<RelacaoMaterialItemSubItemEntity>)relacaoMaterialItemSubitem.
                                PopularDadosRelatorio(
                                nitemId,
                                nSubitemId,
                                nGestorId),
                                param);
                        }
                        break;

                    case (int)RelatorioEnum.FonteRecurso:
                        {
                            FontesRecursoPresenter fontesRecurso = new FontesRecursoPresenter();
                            GerarRelatorio((IEnumerable<FontesRecursoEntity>)fontesRecurso.PopularDadosRelatorio(), param);
                        }
                        break;

                    case (int)RelatorioEnum.Sigla:
                        {
                            SiglaPresenter sigla = new SiglaPresenter();
                            GerarRelatorio((IEnumerable<SiglaEntity>)sigla.PopularDadosRelatorio(int.Parse(param[1].Values[0].ToString()), int.Parse(param[0].Values[0].ToString())), param);
                        }
                        break;
                    case (int)RelatorioEnum.PTRes:
                        {
                            PTResPresenter ptRes = new PTResPresenter();
                            GerarRelatorio((IEnumerable<PTResEntity>)ptRes.PopularDadosRelatorio(), param);
                        }
                        break;
                    case (int)RelatorioEnum.Terceiro:
                        {
                            TerceiroPresenter terceiro = new TerceiroPresenter();
                            int orgaoId = int.Parse(param[1].Values[0].ToString());
                            int restorId = int.Parse(param[0].Values[0].ToString());
                            GerarRelatorio((IEnumerable<TerceiroEntity>)terceiro.PopularDadosRelatorio(orgaoId, restorId), param);
                        }
                        break;

                    case (int)RelatorioEnum.TipoIncorp:
                        {
                            TipoIncorpPresenter tipoIncorp = new TipoIncorpPresenter();
                            GerarRelatorio((IEnumerable<TipoIncorpEntity>)tipoIncorp.PopularDadosRelatorio(), param);
                        }
                        break;

                    case (int)RelatorioEnum.MotivoBaixa:
                        {
                            MotivoBaixaPresenter motivoBaixa = new MotivoBaixaPresenter();
                            GerarRelatorio((IEnumerable<MotivoBaixaEntity>)motivoBaixa.PopularDadosRelatorio(), param);
                        }
                        break;

                    case (int)RelatorioEnum.RequisicaoMaterial:
                        {
                            var movimentoId = param.Where(a => a.Name == "movimentoId").FirstOrDefault().Values[0].ToString();

                            RequisicaoMaterialPresenter requisicao = new RequisicaoMaterialPresenter();
                            GerarRelatorio((IEnumerable<MovimentoItemEntity>)requisicao.ListarNotaRequisicao(Convert.ToInt32(movimentoId)), param);
                        }
                        break;

                    case (int)RelatorioEnum.ConsultaEstoqueSintetico:
                        {

                            var ugeId = param.Where(a => a.Name == "UgeId").FirstOrDefault().Values[0].ToString();
                            var grupoId = param.Where(a => a.Name == "GrupoId").FirstOrDefault().Values[0].ToString();
                            var almoxId = param.Where(a => a.Name == "AlmoxId").FirstOrDefault().Values[0].ToString();
                            var comSemSaldo = param.Where(a => a.Name == "ComSemSaldo").FirstOrDefault().Values[0].ToString();
                            var _ordenarPor = param.Where(a => a.Name == "Ordenacao").FirstOrDefault().Values[0].ToString();
                            ConsultasPresenter consultas = new ConsultasPresenter();
                            IList<SaldoSubItemEntity> lista = consultas.PopularDadosRelatorio(Convert.ToInt32(ugeId), Convert.ToInt32(almoxId), Convert.ToInt32(grupoId), Convert.ToInt16(comSemSaldo), Convert.ToInt32(_ordenarPor));
                            GerarRelatorio(lista, param);
                        }
                        break;

                    case (int)RelatorioEnum.ConsultaEstoqueAnalitico:
                    case (int)RelatorioEnum.ConsultaEstoqueAnaliticoFichaPrateleira:
                        {
                            bool comEstorno = (relatorioImpressao.Id == (int)RelatorioEnum.ConsultaEstoqueAnalitico ? true : false);
                            var subItem = param.Where(a => a.Name == "SubItemMaterialId").FirstOrDefault().Values[0].ToString();
                            var ugeId = param.Where(a => a.Name == "UgeId").FirstOrDefault().Values[0].ToString();
                            var dataInicial = param.Where(a => a.Name == "DataInicial").FirstOrDefault().Values[0].ToString();
                            var dataFinal = param.Where(a => a.Name == "DataFinal").FirstOrDefault().Values[0].ToString();
                            var almoxId = param.Where(a => a.Name == "AlmoxId").FirstOrDefault().Values[0].ToString();

                            MovimentoItemPresenter movItem = new MovimentoItemPresenter();
                            IList<MovimentoItemEntity> lista = movItem.PopularMovimentacaoItem(TratamentoDados.TryParseInt32(almoxId), Convert.ToInt64(subItem), Convert.ToInt32(ugeId), Convert.ToDateTime(dataInicial), Convert.ToDateTime(dataFinal), comEstorno);
                            GerarRelatorio(lista, param);
                        }
                        break;

                    case (int)RelatorioEnum.ConsultaMovimentacaoEntrada:
                        {
                            var tipoMovimento = param.Where(a => a.Name == "TipoMovimentoId").FirstOrDefault().Values[0].ToString();
                            var tipoMovimentoAgrup = param.Where(a => a.Name == "TipoMovAgrupamentoId").FirstOrDefault().Values[0].ToString();
                            var fornecedorId = param.Where(a => a.Name == "FornecedorId").FirstOrDefault().Values[0].ToString();
                            var divisaoId = param.Where(a => a.Name == "DivisaoId").FirstOrDefault().Values[0].ToString();
                            var almoxId = param.Where(a => a.Name == "AlmoxId").FirstOrDefault().Values[0].ToString();
                            var dataInicial = param.Where(a => a.Name == "DataInicial").FirstOrDefault().Values[0].ToString();
                            var dataFinal = param.Where(a => a.Name == "DataFinal").FirstOrDefault().Values[0].ToString();
                            MovimentoItemPresenter movItem = new MovimentoItemPresenter();
                            IList<MovimentoItemEntity> lista = movItem.ImprimirMovimentacao(TratamentoDados.TryParseInt16(almoxId)
                                                                                            , TratamentoDados.TryParseInt16(tipoMovimento)
                                                                                            , TratamentoDados.TryParseInt16(tipoMovimentoAgrup)
                                                                                            , TratamentoDados.TryParseInt32(fornecedorId)
                                                                                            , TratamentoDados.TryParseInt32(divisaoId)
                                                                                            , Convert.ToDateTime(dataInicial)
                                                                                            , Convert.ToDateTime(dataFinal));
                            
                            GerarRelatorio(lista, param);
                        }
                        break;
                    case (int)RelatorioEnum.ConsultaMovimentacaoTransferencia:
                        {
                            var fornecedorId = param.Where(a => a.Name == "FornecedorId").FirstOrDefault().Values[0].ToString();
                            var divisaoId = param.Where(a => a.Name == "DivisaoId").FirstOrDefault().Values[0].ToString();
                            var almoxId = param.Where(a => a.Name == "AlmoxId").FirstOrDefault().Values[0].ToString();
                            var dataInicial = param.Where(a => a.Name == "DataInicial").FirstOrDefault().Values[0].ToString();
                            var dataFinal = param.Where(a => a.Name == "DataFinal").FirstOrDefault().Values[0].ToString();
                            
                            MovimentoItemPresenter movItem = new MovimentoItemPresenter();
                            IList<MovimentoItemEntity> lista = movItem.ImprimirMovimentacao(TratamentoDados.TryParseInt16(almoxId)
                                                                                            , null
                                                                                            , null
                                                                                            , TratamentoDados.TryParseInt32(fornecedorId)
                                                                                            , TratamentoDados.TryParseInt32(divisaoId)
                                                                                            , Convert.ToDateTime(dataInicial)
                                                                                            , Convert.ToDateTime(dataFinal)
                                                                                            , true);
                            
                            GerarRelatorio(lista, param);
                        }
                        break;


                    case (int)RelatorioEnum.ConsultaConsumoAlmox:
                        {
                            var almoxId = param.Where(a => a.Name == "AlmoxId").FirstOrDefault().Values[0].ToString();
                            var dataInicial = param.Where(a => a.Name == "DataInicial").FirstOrDefault().Values[0].ToString();
                            var dataFinal = param.Where(a => a.Name == "DataFinal").FirstOrDefault().Values[0].ToString();
                            SubItemMaterialPresenter sub = new SubItemMaterialPresenter();
                            IList<SubItemMaterialEntity> lista = sub.ImprimirConsumoAlmox(TratamentoDados.TryParseInt16(almoxId), Convert.ToDateTime(dataInicial), Convert.ToDateTime(dataFinal));
                            GerarRelatorio(lista, param);
                        }
                        break;

                    case (int)RelatorioEnum.ConsultaConsumoRequisitante:
                        {
                            var divisaoId = param.Where(a => a.Name == "DivisaoId").FirstOrDefault().Values[0].ToString();
                            var almoxId = param.Where(a => a.Name == "AlmoxId").FirstOrDefault().Values[0].ToString();
                            var dataInicial = param.Where(a => a.Name == "DataInicial").FirstOrDefault().Values[0].ToString();
                            var dataFinal = param.Where(a => a.Name == "DataFinal").FirstOrDefault().Values[0].ToString();

                            SubItemMaterialPresenter sub = new SubItemMaterialPresenter();
                            IList<SubItemMaterialEntity> lista = sub.ImprimirConsumoDivisao(TratamentoDados.TryParseInt16(divisaoId), TratamentoDados.TryParseInt32(almoxId), Convert.ToDateTime(dataInicial), Convert.ToDateTime(dataFinal));
                            GerarRelatorio(lista, param);
                        }
                        break;


                    case (int)RelatorioEnum.ConsultaSubitemMaterialAlmoxPorND:
                        {
                            SubItemMaterialPresenter objPresenter = new SubItemMaterialPresenter();
                            //var naturezaDespesaID = param.Where(a => a.Name == "NaturezaDespesa_ID").FirstOrDefault().Values[0].ToString();
                            int naturezaDespesaId = -1;
                            var paramNaturezaDespesa = param.Where(a => a.Name == "NaturezaDespesa_ID").FirstOrDefault();
                            if (paramNaturezaDespesa.IsNotNull()) { naturezaDespesaId = TratamentoDados.TryParseInt32(paramNaturezaDespesa.Values[0]).Value; }

                            //param.Remove(param.Where(a => a.Name == "NaturezaDespesa_ID").FirstOrDefault());
                            param.Remove(paramNaturezaDespesa);
                            IEnumerable<SubItemMaterialEntity> ienumSubItens = objPresenter.ListarSubitemMaterialAlmoxarifadoPorNaturezaDespesa(naturezaDespesaId);//.AsEnumerable();
                            GerarRelatorio(ienumSubItens, param);
                        }
                        break;
                    case (int)RelatorioEnum.ConsulmoSubitemAlmox:
                        {
                            long subitemCodigo = (Convert.ToInt64(param.Where(a => a.Name == "SubitemCodigo").FirstOrDefault().Values[0].ToString()));
                            var dataInicial = param.Where(a => a.Name == "DataInicial").FirstOrDefault().Values[0].ToString();
                            var dataFinal = param.Where(a => a.Name == "DataFinal").FirstOrDefault().Values[0].ToString();
                            var gestorId = param.Where(a => a.Name == "GestorId").FirstOrDefault().Values[0].ToString();
                            
                            MovimentoPresenter _movPresenter = new MovimentoPresenter();
                            IList<MovimentoEntity> lista = _movPresenter.ImprimirConsultaConsulmoAlmox(subitemCodigo
                                                                                                        , dataInicial.ToString()
                                                                                                        , dataFinal.ToString()
                                                                                                        , Convert.ToInt16(gestorId));
                            GerarRelatorio(lista, param);
                        }
                        break;
                    //case (int)RelatorioEnum.ConsultaPrevisaoConsumoAlmox:
                    //    {
                    //        var almoxId = param.Where(a => a.Name == "AlmoxId").FirstOrDefault().Values[0].ToString();
                    //        var dataInicial = param.Where(a => a.Name == "DataInicial").FirstOrDefault().Values[0].ToString();
                    //        var dataFinal = param.Where(a => a.Name == "DataFinal").FirstOrDefault().Values[0].ToString();
                    //        SubItemMaterialPresenter sub = new SubItemMaterialPresenter();
                    //        IList<SubItemMaterialEntity> lista = sub.ImprimirPrevisaoConsumoAlmox(TratamentoDados.TryParseInt16(almoxId), Convert.ToDateTime(dataInicial), Convert.ToDateTime(dataFinal));
                    //        GerarRelatorio(lista, param);
                    //    }                //    break;

                    case (int)RelatorioEnum.EntradaMaterial: //Refazer, está pegando NUMERO DOCUMENTO
                        {
                            var MovimentoId = param.Where(a => a.Name == "MovimentoId").FirstOrDefault().Values[0].ToString();
                            MovimentoItemPresenter movItem = new MovimentoItemPresenter();
                            IList<MovimentoItemEntity> lista = movItem.ListarMovimentoEntradaFornecimento((int)TratamentoDados.TryParseInt32(MovimentoId));

                            if (lista.HasElements<MovimentoItemEntity>())
                            {
                                UsuarioPresenter usuarioPresenter = new UsuarioPresenter();
                                var usuario = usuarioPresenter.SelecionaUsuarioPor_LoginID(lista[0].Movimento.IdLogin.Value);
                                param.Add(new ReportParameter("efetuadaPor", usuario.NomeUsuario));
                            }

                            GerarRelatorio(lista, param);
                        }
                        break;
                    case (int)RelatorioEnum.ConsumoImediato:
                        {
                            var movimentacaoMaterialID = param.Where(a => a.Name == "MovimentoId").FirstOrDefault().Values[0].ToString();
                            MovimentoPresenter objPresenter = new MovimentoPresenter();
                            MovimentoEntity movimentacaoMaterial = objPresenter.ObterMovimentacaoConsumoImediato((int)TratamentoDados.TryParseInt32(movimentacaoMaterialID));

                            if (movimentacaoMaterial.MovimentoItem.HasElements<MovimentoItemEntity>())
                            {
                                UsuarioPresenter usuarioPresenter = new UsuarioPresenter();
                                var usuario = usuarioPresenter.SelecionaUsuarioPor_LoginID(movimentacaoMaterial.MovimentoItem[0].Movimento.IdLogin.Value);
                                param.Add(new ReportParameter("efetuadaPor", usuario.NomeUsuario));
                                param.Add(new ReportParameter("DadosUA", String.Format("{0} - {1}", movimentacaoMaterial.UA.Codigo.Value.ToString("D6"), movimentacaoMaterial.UA.Descricao)));
                                param.Add(new ReportParameter("DadosUGE", String.Format("{0} - {1}", movimentacaoMaterial.UGE.Codigo.Value.ToString("D6"), movimentacaoMaterial.UGE.Descricao)));
                                param.Add(new ReportParameter("CodigoUGE", movimentacaoMaterial.UGE.Codigo.Value.ToString("D6")));
                                param.Add(new ReportParameter("DadosAlmoxarifado", String.Format("{0} - {1}", movimentacaoMaterial.Almoxarifado.Codigo.Value.ToString("D3"), movimentacaoMaterial.Almoxarifado.Descricao)));
                            }

                            GerarRelatorio(movimentacaoMaterial.MovimentoItem, param);
                        }
                        break;
                    case (int)RelatorioEnum.SaidaMaterial:
                        {
                            MovimentoEntity movimento = new MovimentoEntity();
                            movimento.Id = Common.Util.TratamentoDados.TryParseInt32(param.Where(a => a.Name == "MovimentoId").FirstOrDefault().Values[0].ToString());

                            SaidaMaterialPresenter movItem = new SaidaMaterialPresenter();
                            IList<MovimentoItemEntity> lista = movItem.ImprimirRelatorioSaida(movimento);

                            if (lista.HasElements<MovimentoItemEntity>())
                            {
                                UsuarioPresenter usuarioPresenter = new UsuarioPresenter();
                                var usuario = usuarioPresenter.SelecionaUsuarioPor_LoginID(lista[0].Movimento.IdLogin.Value);
                                param.Add(new ReportParameter("efetuadaPor", usuario.NomeUsuario));

                                string NomeReq = lista[0].Movimento.NomeRequisitante == null ? " - " : lista[0].Movimento.NomeRequisitante.ToString();
                                param.Add(new ReportParameter("requeridoPor", NomeReq));

                               
                               
                            }

                            GerarRelatorio(lista, param);
                        }
                        break;
                    //case (int)RelatorioEnum.MensalAnalitico:
                    case (int)RelatorioEnum.MensalAnaliticoConsumo:
                    case (int)RelatorioEnum.MensalAnaliticoPatrimonio:
                    {
                        string[] arrParamsToExclude = new string[] {"SubitensInativos","TransacaoPendentes"};

                        arrParamsToExclude.ToList().ForEach(_nomeParam => param.Remove(param.Where(__paramParaExclusao => __paramParaExclusao.Name == _nomeParam).FirstOrDefault()));

                        FechamentoMensalPresenter objPresenter = new FechamentoMensalPresenter();
                        var almoxId = param.Where(a => a.Name == "AlmoxId").FirstOrDefault().Values[0].ToString();
                        var dataInicial = param.Where(a => a.Name == "DataInicial").FirstOrDefault().Values[0].ToString();
                        DateTime d = DateTime.Parse(dataInicial);
                        int anoMesRef = Int32.Parse(d.ToString("yyyyMM"));
                        int almoxID = Int32.Parse(almoxId);

                        IList<relAnaliticoFechamentoMensalEntity> lista = objPresenter.ImprimirAnaliticoBalanceteMensal(almoxID, anoMesRef);
                        GerarRelatorio(lista, param);
                    }
                    break;
                    case (int)RelatorioEnum.MensalInventario:
                    //{
                    //    string[] arrParamsToExclude = new string[] {"SubitensInativos","TransacaoPendentes"};

                    //    arrParamsToExclude.ToList().ForEach(_nomeParam => param.Remove(param.Where(__paramParaExclusao => __paramParaExclusao.Name == _nomeParam).FirstOrDefault()));

                    //    FechamentoMensalPresenter objPresenter = new FechamentoMensalPresenter();
                    //    var almoxId = param.Where(a => a.Name == "AlmoxId").FirstOrDefault().Values[0].ToString();
                    //    var dataInicial = param.Where(a => a.Name == "DataInicial").FirstOrDefault().Values[0].ToString();
                    //    DateTime d = DateTime.Parse(dataInicial);
                    //    int anoMesRef = Int32.Parse(d.ToString("yyyyMM"));
                    //    int almoxID = Int32.Parse(almoxId);

                    //    IList<relInventarioFechamentoMensalEntity> lista = objPresenter.ImprimirInventarioMensal(almoxID, anoMesRef);
                    //    GerarRelatorio(lista, param);
                    //}
                    //break;
                    case (int)RelatorioEnum.MensalAnalitico:
                    case (int)RelatorioEnum.MensalBalancete:
                    case (int)RelatorioEnum.MensalBalanceteConsumo:
                    case (int)RelatorioEnum.BalanceteSimulacao:
                    case (int)RelatorioEnum.MensalBalancetePatrimonio:
                    case (int)RelatorioEnum.MensalGrupoClasseMaterial:
                        {
                            if ((relatorioImpressao.Id != (int)RelatorioEnum.MensalBalancete) && (relatorioImpressao.Id != (int)RelatorioEnum.BalanceteSimulacao))
                            {
                                param.Remove(param.Where(a => a.Name == "SubitensInativos").FirstOrDefault());
                                param.Remove(param.Where(a => a.Name == "TransacaoPendentes").FirstOrDefault());
                            }
                            else if ((relatorioImpressao.Id == (int)RelatorioEnum.BalanceteSimulacao) || (relatorioImpressao.Id == (int)RelatorioEnum.MensalBalancete))
                            {
                                param.Remove(param.Where(a => a.Name == "FormatoSIAFEM").FirstOrDefault());
                            }

                            FechamentoMensalPresenter fecha = new FechamentoMensalPresenter();
                            var almoxId = param.Where(a => a.Name == "AlmoxId").FirstOrDefault().Values[0].ToString();
                            var dataInicial = param.Where(a => a.Name == "DataInicial").FirstOrDefault().Values[0].ToString();
                            var dataFinal = param.Where(a => a.Name == "DataFinal").FirstOrDefault().Values[0].ToString();
                            DateTime d = Convert.ToDateTime(dataInicial);
                            int? anoMes = TratamentoDados.TryParseInt32(d.Year.ToString().PadLeft(4, '0') + d.Month.ToString().PadLeft(2, '0'));

                            String nome = relatorioImpressao.Nome;

                            bool saldoZero = false;
                            if (Session["chkSaldoMaiorZero"] != null)
                             saldoZero = (bool)Session["chkSaldoMaiorZero"];  



                            GeralEnum.SituacaoFechamento situacaoFechamento;

                            if (relatorioImpressao.Id == (int)RelatorioEnum.BalanceteSimulacao)
                                situacaoFechamento = GeralEnum.SituacaoFechamento.Simular;
                            else
                                situacaoFechamento = GeralEnum.SituacaoFechamento.Executar;

                            bool? tipoNatureza = null;
                            if (relatorioImpressao.Id == (int)RelatorioEnum.MensalBalanceteConsumo || relatorioImpressao.Id == (int)RelatorioEnum.MensalAnaliticoConsumo)
                                tipoNatureza = true;
                            if (relatorioImpressao.Id == (int)RelatorioEnum.MensalBalancetePatrimonio || relatorioImpressao.Id == (int)RelatorioEnum.MensalAnaliticoPatrimonio)
                                tipoNatureza = false;

                            IList<FechamentoMensalEntity> lista = fecha.ImprimirFechamentoMensal(TratamentoDados.TryParseInt32(almoxId), anoMes, tipoNatureza, situacaoFechamento,saldoZero);
                            GerarRelatorio(lista, param);
                        }
                        break;
                    case (int)RelatorioEnum.ConsultaMovimentacaoSaida:
                        {
                            var tipoMovimento = param.Where(a => a.Name == "TipoMovimentoId").FirstOrDefault().Values[0].ToString();
                            var tipoMovimentoAgrup = param.Where(a => a.Name == "TipoMovAgrupamentoId").FirstOrDefault().Values[0].ToString();
                            var fornecedorId = param.Where(a => a.Name == "FornecedorId").FirstOrDefault().Values[0].ToString();
                            var divisaoId = param.Where(a => a.Name == "DivisaoId").FirstOrDefault().Values[0].ToString();
                            var almoxId = param.Where(a => a.Name == "AlmoxId").FirstOrDefault().Values[0].ToString();
                            var dataInicial = param.Where(a => a.Name == "DataInicial").FirstOrDefault().Values[0].ToString();
                            var dataFinal = param.Where(a => a.Name == "DataFinal").FirstOrDefault().Values[0].ToString();
                            MovimentoItemPresenter movItem = new MovimentoItemPresenter();
                            IList<MovimentoItemEntity> lista = movItem.ImprimirMovimentacao(TratamentoDados.TryParseInt16(almoxId)
                                                                                            , TratamentoDados.TryParseInt16(tipoMovimento)
                                                                                            , TratamentoDados.TryParseInt16(tipoMovimentoAgrup)
                                                                                            , TratamentoDados.TryParseInt32(fornecedorId)
                                                                                            , TratamentoDados.TryParseInt32(divisaoId)
                                                                                            , Convert.ToDateTime(dataInicial)
                                                                                            , Convert.ToDateTime(dataFinal));



                            GerarRelatorio(lista, param);
                        }
                        break;
                    case (int)RelatorioEnum.ExportacaoCustos:
                        {
                            int almoxId = Convert.ToInt32(param.Where(a => a.Name == "AlmoxId").FirstOrDefault().Values[0].ToString());
                            string sGestorNome = param.Where(a => a.Name == "AlmoxId").FirstOrDefault().Values[0].ToString();
                            var dataInicial = param.Where(a => a.Name == "DataInicial").FirstOrDefault().Values[0].ToString();
                            int? intGestorId = Convert.ToInt32(param.Where(a => a.Name == "GestorId").FirstOrDefault().Values[0].ToString());
                            DateTime d = Convert.ToDateTime(dataInicial);
                            string anoMes = param.Where(a => a.Name == "AnoMesRef").FirstOrDefault().Values[0].ToString();
                            int ianoMes = anoMes == null ? 0 : Convert.ToInt32(anoMes);

                            FechamentoMensalPresenter fechamentoMensal = new FechamentoMensalPresenter();
                            //List<PTResMensalEntity> lista = fechamentoMensal.GerarExportacaoCustos(ianoMes, intGestorId).ToList();
                            //List<PTResMensalEntity> lista = fechamentoMensal.ProcessarNLConsumo(almoxId, ianoMes).ToList();
                            List<PTResMensalEntity> lista = fechamentoMensal.ProcessarNLsConsumoAlmox(intGestorId ?? -1, ianoMes, almoxId).ToList();

                            GerarExportacao(lista, param);
                        }
                        break;
                    case (int)RelatorioEnum.ExportacaoCustosConsumoImediato:
                        {
                            int almoxId = Convert.ToInt32(param.Where(a => a.Name == "AlmoxId").FirstOrDefault().Values[0].ToString());
                            string sGestorNome = param.Where(a => a.Name == "AlmoxId").FirstOrDefault().Values[0].ToString();
                            var dataInicial = param.Where(a => a.Name == "DataInicial").FirstOrDefault().Values[0].ToString();
                            int? intGestorId = Convert.ToInt32(param.Where(a => a.Name == "GestorId").FirstOrDefault().Values[0].ToString());
                            DateTime d = Convert.ToDateTime(dataInicial);
                            string anoMes = param.Where(a => a.Name == "AnoMesRef").FirstOrDefault().Values[0].ToString();
                            int ianoMes = anoMes == null ? 0 : Convert.ToInt32(anoMes);
                            int idPerfil = dadosAcessoUsuario.Transacoes.Perfis[0].IdPerfil;
                            int orgaoId = int.Parse(param.Where(a => a.Name == "OrgaoId").FirstOrDefault().Values[0]); 

                            FechamentoMensalPresenter fechamentoMensal = new FechamentoMensalPresenter();
                            //List<PTResMensalEntity> lista = fechamentoMensal.GerarExportacaoCustos(ianoMes, intGestorId).ToList();
                            //List<PTResMensalEntity> lista = fechamentoMensal.ProcessarNLConsumo(almoxId, ianoMes).ToList();
                            List<PTResMensalEntity> lista = fechamentoMensal.ProcessarNLsConsumoImediato(almoxId, ianoMes, orgaoId, idPerfil).ToList();

                            GerarExportacao(lista, param);
                        }
                        break;
                    case (int)RelatorioEnum.BalanceteAnual:
                        {
                            int almoxId = Convert.ToInt32(param.Where(a => a.Name == "AlmoxId").FirstOrDefault().Values[0].ToString());
                            string sGestorNome = param.Where(a => a.Name == "AlmoxId").FirstOrDefault().Values[0].ToString();
                            var dataInicial = param.Where(a => a.Name == "MesExtracaoInicial").FirstOrDefault().Values[0].ToString();
                            var dataFinal = param.Where(a => a.Name == "MesExtracaoFinal").FirstOrDefault().Values[0].ToString();
                            var mesAnterior = param.Where(a => a.Name == "mesAnterior").FirstOrDefault().Values[0].ToString();
                             
                            FechamentoMensalPresenter fechamentoPresenter = new FechamentoMensalPresenter();
                            List<FechamentoAnualEntity> dtLista =  fechamentoPresenter.GerarBalanceteAnual(almoxId, mesAnterior, dataInicial, dataFinal);

                            this.ImprimirBalanceteAnual(dtLista, param);

                            break;
                        }
                    case (int)RelatorioEnum.Usuarios:
                        {
                            UsuarioPresenter usuario = new UsuarioPresenter();
                            GerarRelatorio((IEnumerable<Entity.UsuarioRelatorio>)usuario.ListarTodosPerfisRelatorio(int.Parse(param[1].Values[0].ToString()),
                                                                                                                    int.Parse(param[0].Values[0].ToString()),
                                                                                                                    int.Parse(param[2].Values[0].ToString()),
                                                                                                                              param[6].Values[0]), param);
                        }
                        break;
                    case (int)RelatorioEnum.UsuariosSemPerfil:
                        {
                            UsuarioPresenter usuario = new UsuarioPresenter();
                            GerarRelatorio((IEnumerable<Entity.Usuario>)usuario.PopularDadosUsuarioGrid(int.Parse(param[1].Values[0].ToString()),
                                                                                                        int.Parse(param[0].Values[0].ToString()),
                                                                                                        0,
                                                                                                        0,
                                                                                                        int.Parse(param[2].Values[0].ToString()),
                                                                                                        _chaveUsuarioImpressao,
                                                                                                        100,
                                                                                                        param[6].Values[0]), param);
                        }
                        break;
                }
            }
            else
            {
                throw new Exception("Erro ao gerar relatório (coleta de dados).");
            }
        }

        public void DesabilitarFormatoParaExportacao(ReportViewer ctrlReportViewer, formatoRelatorio fmtExportacaoRelatorio)
        {
            System.Reflection.FieldInfo info;
            RenderingExtension formatoSaida = null;

            string strNomeFormato = GeralEnum.GetEnumDescription(fmtExportacaoRelatorio);
            if (ctrlReportViewer.LocalReport.ListRenderingExtensions().Select(formato => formato.Name.ToLowerInvariant()).ToList().Contains(strNomeFormato.ToLowerInvariant()))
            {
                formatoSaida = ctrlReportViewer.LocalReport.ListRenderingExtensions().Where(_formatoSaida => _formatoSaida.Name.ToLowerInvariant() == strNomeFormato.ToLowerInvariant()).FirstOrDefault();

                if (formatoSaida.IsNotNull())
                {
                    info = formatoSaida.GetType().GetField("m_isVisible", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    info.SetValue(formatoSaida, false);
                }
            }
        }


        private void ImprimirBalanceteAnual<T>(T lista, List<ReportParameter> param)
        {
            var relatorioDestaUnidade = true;
            var existeAlmoxRelatorio = relatorioImpressao.Parametros.ExisteParametroValor("AlmoxId");
            var almoxLogadoId = (int)new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id;
            var tipoPerfilLogado = (int)new PageBase().GetAcesso.Transacoes.Perfis[0].IdPerfil;

            try
            {
                if (relatorioImpressao.IsNotNull() && relatorioDestaUnidade)
                {
                    rptViewer.LocalReport.Refresh();
                    rptViewer.ShowParameterPrompts = false;

                    rptViewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                    rptViewer.LocalReport.ReportPath = Server.MapPath(Constante.ReportPath + relatorioImpressao.Nome);
                    rptViewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource(relatorioImpressao.DataSet, lista));

                    rptViewer.ShowPrintButton = true;
                    rptViewer.AsyncRendering = true;
                    rptViewer.SizeToReportContent = true;

                    if (tipoPerfilLogado != (int)TipoPerfil.AdministradorGeral)
                    {
                        DesabilitarFormatoParaExportacao(rptViewer, formatoRelatorio.Word);
                        DesabilitarFormatoParaExportacao(rptViewer, formatoRelatorio.PDF);
                    }

                    rptViewer.LocalReport.SetParameters(param);
                    rptViewer.DataBind();

                    rptViewer.ServerReport.Refresh();
                }
                else if (!relatorioDestaUnidade)
                {
                    throw new ArgumentException("Erro ao gerar relatório (outro almoxarifado/colisão sessões).");
                }
                else
                {
                    throw new Exception("Erro ao gerar relatório (instanciamento objeto saída).");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
