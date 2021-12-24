using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using Sam.Domain.Entity;
using LinqToExcel;
using Sam.Common.Util;
using System.IO;
using Sam.Business.Business;

namespace Sam.Business.Importacao
{
    public class ImportacaoBusiness
    {
        /// <summary>
        /// Faz a leitura do arquivo excel e insere na tabela Carga
        /// </summary>
        /// <param name="cargaEntity">Dados para realizar a leitura do excel</param>
        /// <param name="loginId">Login do usuário logado</param>
        /// <returns></returns>
        public IQueryable<TB_CARGA> ProcessarExcel(CargaEntity cargaEntity, int loginId)
        {
            return new ImportacaoCargaControle().ProcessarExcel(cargaEntity, loginId);
        }

        /// <summary>
        /// Executa a carga após ser inserida na tabela TB_CARGA
        /// </summary>
        /// <param name="controleId">Id do controle</param>
        /// <returns>Executado com sucesso ou com erros</returns>
        public bool ExecutarCarga(int controleId)
        {
            SubItemMaterialBusiness subItemBusiness = new SubItemMaterialBusiness();
            ControleBusiness controleBusiness = new ControleBusiness();

            try
            {
                //Retorna o objeto controle com todos as cargas
                bool LazyLoadingEnabled = true;
                var controle = new ControleBusiness().QueryAll(a => a.TB_CONTROLE_ID == controleId, LazyLoadingEnabled).FirstOrDefault();

                //Verifica qual tipo de importação irá realizar

                switch (controle.TB_TIPO_CONTROLE_ID)
                {
                    case (int)GeralEnum.TipoControle.SubItemMaterial:
                        return new ImportacaoSubItemMaterial().InsertImportacao(controle);
                    case (int)GeralEnum.TipoControle.Divisao:
                        return new ImportacaoDivisao().InsertImportacao(controle);
                    case (int)GeralEnum.TipoControle.InventarioAlmoxarifado:
                        return new ImportacaoInventario().InsertImportacao(controle);
                    case (int)GeralEnum.TipoControle.CatalogoAlmoxarifado:
                        return new ImportacaoCatalogo().InsertImportacao(controle);
                    case (int)GeralEnum.TipoControle.GrupoMaterial:
                        return new ImportacaoGrupoMaterial().InsertImportacao(controle);
                    case (int)GeralEnum.TipoControle.ClasseMaterial:
                        return new ImportacaoClasseMaterial().InsertImportacao(controle);
                    case (int)GeralEnum.TipoControle.Material:
                        return new ImportacaoMaterial().InsertImportacao(controle);
                    case (int)GeralEnum.TipoControle.NaturezaDespesa:
                        return new ImportacaoNaturezaDespesa().InsertImportacao(controle);
                    case (int)GeralEnum.TipoControle.ItemMaterial:
                        return new ImportacaoMaterial().InsertImportacao(controle);
                    case (int)GeralEnum.TipoControle.ItemNatureza:
                        return new ImportacaoNaturezaDespesa().InsertImportacao(controle);
                    case (int)GeralEnum.TipoControle.UnidadeAdm:
                        return new ImportacaoUA().InsertImportacao(controle);
                    case (int)GeralEnum.TipoControle.Almoxarifado:
                        return new ImportacaoAlmoxarifado().InsertImportacao(controle);
                    case (int)GeralEnum.TipoControle.Responsavel:
                        return new ImportacaoResponsavel().InsertImportacao(controle);
                    case (int)GeralEnum.TipoControle.Usuario:
                        return new ImportacaoUsuario().InsertImportacao(controle);
                    case (int)GeralEnum.TipoControle.PerfilRequisitante:
                        return new ImportacaoPerfil().InsertImportacao(controle);
                    default:
                        throw new Exception("Tipo de carga não foi implementado");
                }
                //if (controle.TB_TIPO_CONTROLE_ID == (int)GeralEnum.TipoControle.SubItemMaterial)
                //{
                //    return new ImportacaoSubItemMaterial().InsertImportacao(controle);
                //}
                //else if (controle.TB_TIPO_CONTROLE_ID == (int)GeralEnum.TipoControle.Divisao)
                //{
                //    return new ImportacaoDivisao().InsertImportacao(controle);
                //}
                //else if (controle.TB_TIPO_CONTROLE_ID == (int)GeralEnum.TipoControle.InventarioAlmoxarifado)
                //{
                //    return new ImportacaoInventario().InsertImportacao(controle);
                //}
                //else if (controle.TB_TIPO_CONTROLE_ID == (int)GeralEnum.TipoControle.CatalogoAlmoxarifado)
                //{
                //    return new ImportacaoCatalogo().InsertImportacao(controle);
                //}

                //else if (controle.TB_TIPO_CONTROLE_ID == (int)GeralEnum.TipoControle.GrupoMaterial)
                //{
                //    return new ImportacaoGrupoMaterial().InsertImportacao(controle);
                //}
                //else if (controle.TB_TIPO_CONTROLE_ID == (int)GeralEnum.TipoControle.ClasseMaterial)
                //{
                //    return new ImportacaoClasseMaterial().InsertImportacao(controle);
                //}
                //else if (controle.TB_TIPO_CONTROLE_ID == (int)GeralEnum.TipoControle.Material)
                //{
                //    return new ImportacaoMaterial().InsertImportacao(controle);
                //}
                //else if (controle.TB_TIPO_CONTROLE_ID == (int)GeralEnum.TipoControle.NaturezaDespesa)
                //{
                //    return new ImportacaoNaturezaDespesa().InsertImportacao(controle);
                //}
                //else if (controle.TB_TIPO_CONTROLE_ID == (int)GeralEnum.TipoControle.ItemMaterial)
                //{
                //    return new ImportacaoMaterial().InsertImportacao(controle);
                //}
                //else if (controle.TB_TIPO_CONTROLE_ID == (int)GeralEnum.TipoControle.ItemNatureza)
                //{
                //    return new ImportacaoNaturezaDespesa().InsertImportacao(controle);
                //}
                //else if (controle.TB_TIPO_CONTROLE_ID == (int)GeralEnum.TipoControle.UnidadeAdm)
                //{
                //    return new ImportacaoUA().InsertImportacao(controle);
                //}
                //else
                //{
                //    throw new Exception("Tipo de carga não foi implementado");
                //}
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public static List<CargaEntity> ListarArquivosDiretorio(string diretorio)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(diretorio);

            List<CargaEntity> arquivoList = new List<CargaEntity>();

            // lista arquivos do diretorio corrente
            foreach (FileInfo file in dirInfo.GetFiles())
            {
                // aqui no caso estou guardando o nome completo do arquivo em em controle ListBox
                // voce faz como quiser

                arquivoList.Add(new CargaEntity(file.Name));
            }

            return arquivoList;

        }

        public void RemoverArquivo(CargaEntity cargaTable)
        {
            try
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(cargaTable.CaminhoDiretorio + cargaTable.NomeArquivo);
                fi.Delete();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public void CopiarArquivo(CargaEntity cargaTable)
        {
            try
            {
                if (ExisteDiretorio(cargaTable.CaminhoDiretorio, true) || ExisteDiretorio(cargaTable.CaminhoDiretorioDestino, true))
                {
                    System.IO.FileInfo fi = new System.IO.FileInfo(cargaTable.CaminhoDiretorio + cargaTable.NomeArquivo);
                    System.IO.File.Move(cargaTable.CaminhoDiretorio + cargaTable.NomeArquivo, cargaTable.CaminhoDiretorioDestino + cargaTable.NomeArquivo);
                }
                else
                {
                    throw new Exception("Diretórios utilizados para processo de carga inexistentes no servidor.\nFavor contatar administrador do sistema.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public bool ExisteDiretorio(string fullPathDir, bool forceCreatedir)
        {
            var blnExisteDiretorio = false;

            try
            {
                blnExisteDiretorio = System.IO.Directory.Exists(fullPathDir);

                if (!blnExisteDiretorio && forceCreatedir)
                {
                    System.IO.Directory.CreateDirectory(fullPathDir);
                    blnExisteDiretorio = true;
                }
            }
            catch (IOException ioExc)
            {
                throw new Exception(String.Format("Erro IO: {0}", ioExc.Message));
            }
            catch (Exception excGenerico)
            {
                throw new Exception(String.Format("Erro genérico: {0}", excGenerico.Message));
            }

            return blnExisteDiretorio;
        }
    }
}
