using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;
using System.Data.SqlClient;
using Sam.Common;
using Sam.Common.Util;
using System.Transactions;
using Sam.Integracao.SIAF.Core;

namespace Sam.Domain.Business
{
    public partial class CatalogoBusiness : BaseBusiness
    {
        #region Item de Material

        private ItemMaterialEntity item = new ItemMaterialEntity();

        public ItemMaterialEntity ItemMaterial
        {
            get { return item; }
            set { item = value; }
        }

        public bool SalvarItemMaterial()
        {
            this.Service<IItemMaterialService>().Entity = this.ItemMaterial;
            this.ConsistirItemMaterial();
            if (this.Consistido)
            {
                this.Service<IItemMaterialService>().Salvar();
            }
            return this.Consistido;
        }

        public bool SalvarItemMaterialSiafisico()
        {
            try
            {
                this.Service<IItemMaterialService>().Entity = this.ItemMaterial;

                if (!this.Service<IItemMaterialService>().SalvarSiafisico())
                {
                    ListaErro.Add("Erro ao salvar o item de material!");
                    return false;
                }
                //return true;
                else
                {
                    this.Service<IItemMaterialService>().Entity.Status = true;
                    this.Service<IItemMaterialService>().Entity.Atividade = true;

                    this.Service<IItemMaterialService>().Salvar();
                    return true;
                }
            }
            catch (Exception e)
            {
                TratarErro(e);
                return false;
            }
        }


        public void SelectItemMaterial(int _materialId)
        {
            this.ItemMaterial = this.Service<IItemMaterialService>().Select(_materialId);
        }

        public void SelectItemMaterial(ItemMaterialEntity item)
        {
            this.ItemMaterial = this.Service<IItemMaterialService>().Select(item);
        }

        public ItemMaterialEntity SelectItemMaterialRetorno(int _materialId)
        {
            return this.Service<IItemMaterialService>().Select(_materialId);
        }

        public IList<ItemMaterialEntity> ListarItemSaldoByAlmox(int almoxarifado)
        {
            return this.Service<IItemMaterialService>().ListarItemSaldoByAlmox(almoxarifado);
        }

        public ItemMaterialEntity SelectPorItemMaterial(ItemMaterialEntity itemMat)
        {
            return this.Service<IItemMaterialService>().Select(itemMat);
        }

        public ItemMaterialEntity GetItemMaterialBySubItem(int subItemId)
        {
            return this.Service<IItemMaterialService>().GetItemMaterialBySubItem(subItemId);
        }

        public string NaturezaSubItem(int subItemId)
        {
            return this.Service<IItemMaterialService>().NaturezaSubItem(subItemId);
        }

        public ItemMaterialEntity GetItemMaterialByItem(int itemId)
        {
            return this.Service<IItemMaterialService>().GetItemMaterialByItem(itemId);
        }

        public ItemMaterialEntity GetItemMaterialSiafisico(int itemCodigo)
        { 
            ItemMaterialEntity item = new ItemMaterialEntity();
            item.Codigo = itemCodigo;

            // usando o login do usuário SAM para consulta
            //string sRetorno = Siafem.recebeMsg(Siafem.userNameConsulta, Siafem.passConsulta, DateTime.Today.Year.ToString(), "", Siafem.wsSFCOConsultaI(itemCodigo.ToString()));
            //string sRetorno = Siafem.recebeMsg(Siafem.userNameConsulta, Siafem.passConsulta, DateTime.Today.Year.ToString(), "", Siafem.SiafisicoDocConsultaI(itemCodigo.ToString()), true);
            string sRetorno = Siafem.recebeMsg(Siafem.userNameConsulta, Siafem.passConsulta, DateTime.Today.Year.ToString(), "", GeradorEstimuloSIAF.SiafisicoDocConsultaI(itemCodigo.ToString()), true);
            string resposta = null;
            string strNomeMensagem = null;
            if (Siafem.VerificarErroMensagem(sRetorno, out strNomeMensagem, out resposta))
            {
                //this.ListaErro.Add(resposta);
                if (!String.IsNullOrWhiteSpace(resposta))
                    resposta.BreakLine(Environment.NewLine.ToCharArray()).ToList().ForEach(linhaErro => this.ListaErro.Add(linhaErro));
                return null;
            }

            // descarregando os dados do item de material no Siafisico
            item.Material = new MaterialEntity();
            string material = XmlUtil.getXmlValue(sRetorno,
                "/MSG/SISERRO/Doc_Retorno/SFCODOC/SiafisicoDocConsultaI/documento/Material");
            item.Material.Codigo = Convert.ToInt32(material.Substring(0, 9));
            item.Material.Descricao = material.Substring(9, material.Length - 9);

            item.Material.Classe = new ClasseEntity();
            string classe = XmlUtil.getXmlValue(sRetorno,
                "/MSG/SISERRO/Doc_Retorno/SFCODOC/SiafisicoDocConsultaI/documento/Classe");
            item.Material.Classe.Codigo = Convert.ToInt32(classe.Substring(2, 2).PadRight(3, ' '));
            item.Material.Classe.Descricao = classe.Substring(5, classe.Length - 5);

            item.Material.Classe.Grupo = new GrupoEntity();
            string grupo = XmlUtil.getXmlValue(sRetorno,
                "/MSG/SISERRO/Doc_Retorno/SFCODOC/SiafisicoDocConsultaI/documento/Grupo");
            item.Material.Classe.Grupo.Codigo = Convert.ToInt32(grupo.Substring(0, 2));
            item.Material.Classe.Grupo.Descricao = grupo.Substring(3, grupo.Length - 3);

            string itemMat = XmlUtil.getXmlValue(sRetorno,
                "/MSG/SISERRO/Doc_Retorno/SFCODOC/SiafisicoDocConsultaI/documento/Item");
            item.Codigo = Convert.ToInt32(itemMat.Substring(0, 9));
            item.Descricao = itemMat.Substring(10, itemMat.Length - 10);

            string natDesp1 = XmlUtil.getXmlValue(sRetorno,
                "/MSG/SISERRO/Doc_Retorno/SFCODOC/SiafisicoDocConsultaI/documento/Natureza");
            item.NatDespSiafisicoCodigo1 = Convert.ToInt32(natDesp1);

            string natDesp2 = XmlUtil.getXmlValue(sRetorno,
                "/MSG/SISERRO/Doc_Retorno/SFCODOC/SiafisicoDocConsultaI/documento/Natureza2");
            item.NatDespSiafisicoCodigo2 = Convert.ToInt32(natDesp2);

            string natDesp3 = XmlUtil.getXmlValue(sRetorno,
                "/MSG/SISERRO/Doc_Retorno/SFCODOC/SiafisicoDocConsultaI/documento/Natureza3");
            item.NatDespSiafisicoCodigo3 = Convert.ToInt32(natDesp3);

            string natDesp4 = XmlUtil.getXmlValue(sRetorno,
                "/MSG/SISERRO/Doc_Retorno/SFCODOC/SiafisicoDocConsultaI/documento/Natureza4");
            item.NatDespSiafisicoCodigo4 = Convert.ToInt32(natDesp4);

            string natDesp5 = XmlUtil.getXmlValue(sRetorno,
                "/MSG/SISERRO/Doc_Retorno/SFCODOC/SiafisicoDocConsultaI/documento/Natureza5");
            item.NatDespSiafisicoCodigo5 = Convert.ToInt32(natDesp5);

            return item;
        }


        public IList<ItemMaterialEntity> ListarItemMaterialBySubItem(int _subItem, int _gestorId)
        {
            this.Service<IItemMaterialService>().SkipRegistros = this.SkipRegistros;
            IList<ItemMaterialEntity> retorno = this.Service<IItemMaterialService>().ListarBySubItem(_subItem, _gestorId);
            this.TotalRegistros = this.Service<IItemMaterialService>().TotalRegistros();
            return retorno;
        }

        public ItemMaterialEntity GetItemMaterialNaturezaDespesa()
        {
            this.Service<IItemMaterialService>().Entity = new ItemMaterialEntity();
            this.Service<IItemMaterialService>().Entity = this.ItemMaterial;
            return this.Service<IItemMaterialService>().GetItemMaterialNaturezaDespesa();
        }

        public IList<ItemMaterialEntity> ListarItemMaterialBySubItem(int _subItem)
        {
            this.Service<IItemMaterialService>().SkipRegistros = this.SkipRegistros;
            IList<ItemMaterialEntity> retorno = this.Service<IItemMaterialService>().ListarBySubItem(_subItem);
            this.TotalRegistros = this.Service<IItemMaterialService>().TotalRegistros();
            return retorno;
        }

        public IList<ItemMaterialEntity> ListarAlmox(int _materialId, int _gestorId, int _almoxarifadoId)
        {
            this.Service<IItemMaterialService>().SkipRegistros = this.SkipRegistros;
            IList<ItemMaterialEntity> retorno = this.Service<IItemMaterialService>().ListarAlmox(_materialId, _gestorId, _almoxarifadoId);
            this.TotalRegistros = this.Service<IItemMaterialService>().TotalRegistros();
            return retorno;
        }

        public IList<ItemMaterialEntity> ListarItemMaterial(int _materialId)
        {
            this.Service<IItemMaterialService>().SkipRegistros = this.SkipRegistros;
            IList<ItemMaterialEntity> retorno = this.Service<IItemMaterialService>().Listar(_materialId);
            this.TotalRegistros = this.Service<IItemMaterialService>().TotalRegistros();
            return retorno;
        }

        public IList<ItemMaterialEntity> ListarSubItemCod(int _materialId)
        {
            IList<ItemMaterialEntity> retorno = this.Service<IItemMaterialService>().ListarSubItemCod(_materialId);
            return retorno;
        }

        public IList<ItemMaterialEntity> ListarItemMaterialTodosCod(int _materialId)
        {
            IList<ItemMaterialEntity> retorno = this.Service<IItemMaterialService>().ListarTodosCod(_materialId, true);
            return retorno;
        }

        public IList<ItemMaterialEntity> ListarItemMaterialCod(int _materialId)
        {
            IList<ItemMaterialEntity> retorno = this.Service<IItemMaterialService>().ListarTodosCod(_materialId, false);
            return retorno;
        }

        public IList<ItemMaterialEntity> ListarItemMaterialPorPalavraChaveTodosCod(int? _id, int? _codigo, string _descricao, int? _almoxId, int? _gestorId)
        {

            IList<ItemMaterialEntity> retorno = this.Service<IItemMaterialService>().ListarPorPalavraChaveTodosCod(_id, _codigo, _descricao, _almoxId, _gestorId);
            this.TotalRegistros = this.Service<IItemMaterialService>().TotalRegistros();
            return retorno;
        }


        public IList<ItemMaterialEntity> ListarItemMaterialPorCodigoSiafisico(int? _codigo)
        {
            IList<ItemMaterialEntity> retorno = null;

            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    //IList<ItemMaterialEntity> retorno = this.Service<IItemMaterialService>().ListarItemMaterialPorCodigoSiafisico(_codigo);
                    retorno = this.Service<IItemMaterialService>().ListarItemMaterialPorCodigoSiafisico(_codigo);
                    this.TotalRegistros = this.Service<IItemMaterialService>().TotalRegistros();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    tras.Complete();
                }
            }

            return retorno;
        }

        public IList<ItemMaterialEntity> ImprimirItemMaterial(int _materialId)
        {
            IList<ItemMaterialEntity> retorno = this.Service<IItemMaterialService>().Imprimir(_materialId);
            return retorno;
        }

        public bool ExcluirItemMaterial()
        {
            this.Service<IItemMaterialService>().Entity = this.ItemMaterial;
            if (this.Consistido)
            {
                try
                {
                    this.Service<IItemMaterialService>().Excluir();
                }
                catch (Exception ex)
                {
                    TratarErro(ex);
                }
            }
            return this.Consistido;

        }

        public void ConsistirExclusaoItemMaterial()
        {
            this.Service<IItemMaterialService>().Entity = this.ItemMaterial;
            if (!this.Service<IItemMaterialService>().PodeExcluir())
                this.ListaErro.Add("Não é possível excluir o Item, existem registros associados a ele.");
        }

        public void ConsistirPesquisaItem(int? codigo, string descricao)
        {
            if ((codigo == null) && String.IsNullOrEmpty(descricao))
                this.ListaErro.Add("É obrigatório informar o campo de pesquisa.");            
        }

        public void ConsistirItemMaterial()
        {

            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<ItemMaterialEntity>(ref this.item);

            if (this.ItemMaterial.Material.Id == null || this.ItemMaterial.Material.Id.Value < 1)
                this.ListaErro.Add("É obrigatório informar o Material.");

            if (this.ItemMaterial.Codigo < 1)
                this.ListaErro.Add("É obrigatório informar o Código.");

            if (string.IsNullOrEmpty(this.ItemMaterial.Descricao))
                this.ListaErro.Add("É obrigatório informar a Descrição.");

            //if (this.ItemMaterial.Atividade == null)
            //    this.ListaErro.Add("É obrigatório informar o Indicador de Atividade.");



            if (this.ListaErro.Count == 0)
            {
                if (this.Service<IItemMaterialService>().ExisteCodigoInformado())
                    this.ListaErro.Add("Código já existente.");
            }
        }

        public ItemMaterialEntity RecuperarCadastroItemMaterialDoSiafisico(string pStrCodigoItemMaterial, bool pBlnSalvarNovoRegistro = false)
        {
            EstruturaOrganizacionalBusiness objEstrutura    = new EstruturaOrganizacionalBusiness();
            ItemMaterialEntity              objItemMaterial = new ItemMaterialEntity();

            string lStrUsuario        = string.Empty;
            string lStrSenha          = string.Empty;
            string lStrAnoBase        = string.Empty;
            string lStrUnidadeGestora = string.Empty;
            string lStrMsgEstimulo    = string.Empty;
            string lStrRetornoWs      = string.Empty;
            string lStrErroTratado    = string.Empty;
            string lStrNomeMensagem   = string.Empty;

            string lStrXmlPatternConsulta = "/MSG/SISERRO/Doc_Retorno/SFCODOC/SiafisicoDocConsultaI/documento/";

            lStrUsuario     = Siafem.userNameConsulta;
            lStrSenha       = Siafem.passConsulta;
            lStrAnoBase     = DateTime.Now.Year.ToString();
            //lStrMsgEstimulo = Siafem.wsSFCOConsultaI(pStrCodigoItemMaterial);
            //lStrMsgEstimulo = Siafem.SiafisicoDocConsultaI(pStrCodigoItemMaterial);
            lStrMsgEstimulo = GeradorEstimuloSIAF.SiafisicoDocConsultaI(pStrCodigoItemMaterial);

            try
            {
                lStrRetornoWs   = Siafem.recebeMsg(lStrUsuario, lStrSenha, lStrAnoBase, lStrUnidadeGestora, lStrMsgEstimulo, false);
                //lStrRetornoWs = FakeReturnXml.SiafisicoDocConsultaI_ItemMaterial_004152603();

                if (Siafem.VerificarErroMensagem(lStrRetornoWs, out lStrNomeMensagem, out lStrErroTratado))
                {
                    //lStrErroTratado.BreakLine(Environment.NewLine.ToCharArray()).ToList().ForEach(linhaErro => this.ListaErro.Add(linhaErro));
                    //this.ListaErro.Add(String.Format("Erro SIAFISICO (retornado) ao efetuar importação de Item Material do SIAFISICO: {0}.", lStrErroTratado));
                    if (this.ListaErro.IsNull())
                        this.ListaErro = new List<string>();
                    
                    this.ListaErro.Add(lStrErroTratado);
                    return null;
                }
                else
                {
                    string lStrStatusOperacao = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "StatusOperacao"));
                    string lStrClasse         = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "Classe"));
                    string lStrClasse1        = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "Classe1"));
                    string lStrGrupo          = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "Grupo"));
                    string lStrGrupo1         = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "Grupo1"));
                    string lStrItem           = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "Item"));
                    string lStrItem1          = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "Item1"));
                    string lStrMaterial       = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "Material"));
                    string lStrNatureza       = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "Natureza"));
                    string lStrNatureza2      = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "Natureza2"));
                    string lStrNatureza3      = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "Natureza3"));
                    string lStrNatureza4      = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "Natureza4"));
                    string lStrNatureza5      = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "Natureza5"));
                    //string lStrMsgRetorno     = Sam.Common.XmlUtil.getXmlValue(lStrRetornoWs, String.Format("{0}{1}", lStrXmlPatternConsulta, "MsgRetorno"));

                    objItemMaterial.Atividade = true;
                    objItemMaterial.Codigo    = Int32.Parse(pStrCodigoItemMaterial);
                    objItemMaterial.Descricao = String.Format("{0} {1}", lStrItem, lStrItem1).Replace(lStrItem.BreakLine(0), "").Trim();
                    
                    objItemMaterial.NaturezaDespesa = new List<NaturezaDespesaEntity>();

                    var objBusiness = new CatalogoBusiness();

                    ClasseEntity   objClasseMaterial        = objBusiness.ObterClasse(Int32.Parse(lStrClasse.BreakLine(0)));
                    GrupoEntity    objGrupoMaterial         = objBusiness.ObterGrupoMaterial(Int32.Parse(lStrGrupo.BreakLine(0)));
                    MaterialEntity objMaterial_ItemMaterial = objBusiness.ObterMaterial(Int32.Parse(lStrMaterial.BreakLine(0)));

                    using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                    {
                        try
                        {
                            #region Natureza Despesa
                            var srvNaturezaDespesaMaterial = this.Service<INaturezaDespesaService>();

                            string[] arrNaturezaDespesaCodigo = new string[] { lStrNatureza, lStrNatureza2, lStrNatureza3, lStrNatureza4, lStrNatureza5 };
                            NaturezaDespesaEntity natDespesa = null;

                            var _iCodigoNaturezaDespesa = 0;

                            arrNaturezaDespesaCodigo.ToList().ForEach(codigoNaturezaDespesa =>
                            {
                                if (!String.IsNullOrWhiteSpace(codigoNaturezaDespesa) && (Int32.TryParse(codigoNaturezaDespesa, out _iCodigoNaturezaDespesa) && _iCodigoNaturezaDespesa != 0))
                                {
                                    natDespesa = objBusiness.ObterNaturezaDespesa(codigoNaturezaDespesa);

                                    if (natDespesa.IsNotNull())
                                        objItemMaterial.NaturezaDespesa.Add(natDespesa);
                                    else
                                        this.ListaErro.AddRange(String.Format("Natureza de Despesa {0} não cadastrada no sistema (Item de Material SIAFISICO: {1} - {2}).\nFavor solicitar cadastro da Natureza de Despesa ao Suporte SAM, e efetuar novamente a importação.", codigoNaturezaDespesa, objItemMaterial.Codigo.ToString().PadLeft(9, '0'), objItemMaterial.Descricao).Split('\n'));
                                }
                            });

                            objItemMaterial.NaturezaDespesa.ForEach(NaturezaDespesa =>
                            {
                                if (NaturezaDespesa.IsNotNull() && NaturezaDespesa.Codigo != 0 && !String.IsNullOrWhiteSpace(NaturezaDespesa.Descricao))
                                {
                                    srvNaturezaDespesaMaterial.Entity = NaturezaDespesa;
                                    srvNaturezaDespesaMaterial.Salvar();
                                }
                            });
                            #endregion Natureza Despesa
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                        finally
                        {
                            tras.Complete();
                        }
                    }

                    using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                    {
                        try
                        {
                            #region Material (ItemMaterial)
                            var srvClasseMaterial = this.Service<IClasseService>();

                            if (objClasseMaterial == null)
                            {
                                srvClasseMaterial.Entity = new ClasseEntity()
                                                                             {
                                                                                 Codigo = Int32.Parse(lStrClasse.BreakLine(0)),
                                                                                 Descricao = String.Format("{0} {1}", lStrClasse, lStrClasse1).Replace(lStrClasse.BreakLine(0), "").Trim()
                                                                             };
                                if (pBlnSalvarNovoRegistro) // salvar automaticamente
                                {
                                    srvClasseMaterial.Salvar();
                                    objItemMaterial.ClasseId = srvClasseMaterial.Entity.Id.Value;
                                }
                            }
                            else
                            {
                                objItemMaterial.ClasseId = objClasseMaterial.Id.Value;
                            }
                            #endregion Material (ItemMaterial)
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                        finally
                        {
                            tras.Complete();
                        }
                    }

                    using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                    {
                        try
                        {
                            #region GrupoMaterial
                            var srvGrupoMaterial = this.Service<IGrupoService>();
                            if (objGrupoMaterial == null)
                            {
                                srvGrupoMaterial.Entity = new GrupoEntity()
                                                                            {
                                                                                Codigo = Int32.Parse(lStrClasse.BreakLine(0)),
                                                                                Descricao = String.Format("{0} {1}", lStrGrupo, lStrGrupo1).Replace(lStrGrupo.BreakLine(0), "").Trim()
                                                                            };
                                if (pBlnSalvarNovoRegistro) // salvar automaticamente
                                {
                                    srvGrupoMaterial.Salvar();
                                    objItemMaterial.GrupoId = srvGrupoMaterial.Entity.Id.Value;
                                }
                            }
                            else
                            {
                                objItemMaterial.GrupoId = objGrupoMaterial.Id.Value;
                            }
                            #endregion GrupoMaterial
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                        finally
                        {
                            tras.Complete();
                        }
                    }

                    using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                    {
                        try
                        {
                            #region Material (ItemMaterial)
                            var srvMaterial_ItemMaterial = this.Service<IMaterialService>();
                            if (objMaterial_ItemMaterial == null)
                            {
                                srvMaterial_ItemMaterial.Entity = new MaterialEntity()
                                                                                       {
                                                                                           Codigo = Int32.Parse(lStrMaterial.BreakLine(0)),
                                                                                           Descricao = lStrMaterial.Replace(lStrMaterial.BreakLine(0), "").Trim(),
                                                                                           Classe = objClasseMaterial
                                                                                       };
                                if (pBlnSalvarNovoRegistro)
                                {
                                    srvMaterial_ItemMaterial.Salvar();
                                    objItemMaterial.Material = srvMaterial_ItemMaterial.Entity;
                                }
                            }
                            else
                            {
                                objItemMaterial.Material = objMaterial_ItemMaterial;
                            }

                            // inserir o relacionamento classe e grupo ao objeto Item de Material
                            if (!pBlnSalvarNovoRegistro)
                            {
                                objItemMaterial.Material = new MaterialEntity()
                                                            {
                                                                Codigo = Int32.Parse(lStrMaterial.BreakLine(0)),
                                                                Descricao = lStrMaterial.Replace(lStrMaterial.BreakLine(0), "").Trim()
                                                            };
                                objItemMaterial.Material.Classe = objClasseMaterial;
                                objItemMaterial.Material.Classe.Grupo = objGrupoMaterial;
                                objItemMaterial.NatDespSiafisicoCodigo1 = Convert.ToInt32(lStrNatureza);
                                objItemMaterial.NatDespSiafisicoCodigo2 = Convert.ToInt32(lStrNatureza2);
                                objItemMaterial.NatDespSiafisicoCodigo3 = Convert.ToInt32(lStrNatureza3);
                                objItemMaterial.NatDespSiafisicoCodigo4 = Convert.ToInt32(lStrNatureza4);
                                objItemMaterial.NatDespSiafisicoCodigo5 = Convert.ToInt32(lStrNatureza5);

                            }
                            #endregion Material (ItemMaterial)
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                        finally
                        {
                            tras.Complete();
                        }
                    }

                    using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                    {
                        try
                        {
                            #region ItemMaterial

                            if (pBlnSalvarNovoRegistro) // verificar se salva ou não o item siafisico automaticamente
                            {
                                var srvItemMaterial = this.Service<IItemMaterialService>();
                                {
                                    srvItemMaterial.Entity = objItemMaterial;
                                    srvItemMaterial.Salvar();
                                    objItemMaterial.NaturezaDespesa.ForEach(NaturezaDespesa => { srvItemMaterial.SalvarRelacaoItemNaturezaDespesa(objItemMaterial.Id.Value, NaturezaDespesa.Id.Value); });

                                    //srvItemMaterial.Salvar();
                                    //objItemMaterial = srvItemMaterial.ListarItemMaterialPorCodigoSiafisico(objItemMaterial.Codigo).FirstOrDefault();
                                }
                            }
                            #endregion ItemMaterial
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                        finally
                        {
                            tras.Complete();
                        }
                    }
                    //Verificar regra para mais de uma natureza de despesa, ou se será utilizada a do empenho.
                    //Salvar registro ItemMaterial x NaturezaDespesa
                }
            }
            catch (Exception lExcExcecaoGeral)
            {
                new LogErro().GravarLogErro(lExcExcecaoGeral);
                this.ListaErro.Add(String.Format("Erro ao cadastrar automaticamente item do sistema SIAFISICO: '{0}'\n{1}", lExcExcecaoGeral.Message, "Acionar administrador do sistema"));
                return null;
            }

            return objItemMaterial;
        }

        public bool ExisteRelacaoItemNaturezaDespesa(int iItemMaterial_ID, int iNaturezaDespesa_ID)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    return this.Service<IItemMaterialService>().ExisteRelacaoItemNaturezaDespesa(iItemMaterial_ID, iNaturezaDespesa_ID);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    tras.Complete();
                }
            }
        }

        public bool ExisteRelacaoItemNaturezaDespesa(int iItemMaterial_ID, string strCodigoNaturezaDespesa)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    return this.Service<IItemMaterialService>().ExisteRelacaoItemNaturezaDespesa(iItemMaterial_ID, strCodigoNaturezaDespesa);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    tras.Complete();
                }
            }
        }

        public bool AtualizaRelacaoItemNaturezaDespesa(int iItemMaterial_ID, string strCodigoNaturezaDespesa)
        {
            bool blnNaturezaDespesaExisteParaItem = false;
            bool blnRetorno = false;
            bool blnCadastroAutoNaturezaDespesa = false;

            var serviceItemMaterial = this.Service<IItemMaterialService>();

            try
            {
                using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    try
                    {
                        blnNaturezaDespesaExisteParaItem = serviceItemMaterial.ExisteRelacaoItemNaturezaDespesa(iItemMaterial_ID, strCodigoNaturezaDespesa);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        tras.Complete();
                    }
                }
                

                if (!blnNaturezaDespesaExisteParaItem)
                {
                    serviceItemMaterial.SalvarRelacaoItemNaturezaDespesa(iItemMaterial_ID, strCodigoNaturezaDespesa, true);
                }
                else if (!blnNaturezaDespesaExisteParaItem && blnCadastroAutoNaturezaDespesa)
                {
                    this.CadastraNaturezaDespesaNova(strCodigoNaturezaDespesa);
                    serviceItemMaterial.SalvarRelacaoItemNaturezaDespesa(iItemMaterial_ID, strCodigoNaturezaDespesa);
                }
            }
            catch (Exception excErroParaPropagar)
            {
                //throw new ArgumentException(String.Format("Natureza de Despesa {0} não cadastrada no sistema!", strCodigoNaturezaDespesa), excErroParaPropagar);
                throw excErroParaPropagar;
            }

            return blnRetorno;
        }

        public void CadastraNaturezaDespesaNova(string strCodigoNaturezaDespesa)
        {
            var servicoInfra = this.Service<INaturezaDespesaService>();

            this.NaturezaDespesa = new NaturezaDespesaEntity() { Codigo = Int32.Parse(strCodigoNaturezaDespesa), Descricao = String.Format("Natureza de Despesa cadastrada automaticamente - #{0}", strCodigoNaturezaDespesa), Natureza = true };

            servicoInfra.Entity = this.NaturezaDespesa;
            servicoInfra.Salvar();

            servicoInfra.Entity.Id = this.NaturezaDespesa.Id;

            return;
        }

        public ItemMaterialEntity ObterItemMaterial(int iCodigoItemMaterial)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    return this.Service<IItemMaterialService>().ObterItemMaterial(iCodigoItemMaterial);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    tras.Complete();
                }
            }
        }
        #endregion
    }
}
