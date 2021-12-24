using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using Sam.Entity;
using Sam.Domain.Business;
using Sam.Domain.Entity;
using Sam.Common.Util;
using TipoPerfil = Sam.Common.Util.GeralEnum.TipoPerfil;
using Sam.Business.Business.Seguranca;



namespace Sam.Business
{
    public class PerfilBusiness : BaseBusinessSeguranca, ICrudBaseBusiness<TB_PERFIL>
    {

        private Sam.Entity.Usuario usuario = new Sam.Entity.Usuario();

        public Sam.Entity.Usuario Usuario
        {
            get { return usuario; }
            set { usuario = value; }
        }

        List<string> listaErro = new List<string>();

        public List<string> ListaErro
        {
            get { return listaErro; }
            set { listaErro = value; }
        }



        public List<GeralEnum.CargaErro> listCargaErro = new List<GeralEnum.CargaErro>();
        public List<GeralEnum.CargaErro> ListCargaErro
        {
            get { return listCargaErro; }
            set { listCargaErro = value; }
        }
        public bool ConsistidoCargaErro
        {
            get
            {
                return this.ListCargaErro.Count == 0;
            }
        }

        #region Métodos Customizados

        public List<TB_PERFIL> ListarPerfil()
        {
            //Exemplo para sort
            Expression<Func<TB_PERFIL, string>> sort = a => a.TB_PERFIL_DESCRICAO;

            //Exemplo para Where
            Expression<Func<TB_PERFIL, bool>> where = a => a.TB_PERFIL_ID != 0;

            PerfilInfrastructure persistencia = new PerfilInfrastructure();
            var result = persistencia.SelectWhere(sort, false, where, this.SkipRegistros);

            this.TotalRegistros = persistencia.TotalRegistros;

            return result;
        }

        public void Insert(TB_PERFIL Perfil, int perfilNivel)
        {
            PerfilInfrastructure Infrastructure = new PerfilInfrastructure();
            Perfil.TB_PERFIL_ID = Infrastructure.GetNovoPerfilId();

            if (perfilNivel == (int)Sam.Common.Util.GeralEnum.PerfilNivel.TipoPadao)
            {
                // Nível Órgão
                Perfil.TB_PERFIL_NIVEL.Add(criarPerfilNivel(Perfil.TB_PERFIL_ID, 1));

                // Nível Gestor
                Perfil.TB_PERFIL_NIVEL.Add(criarPerfilNivel(Perfil.TB_PERFIL_ID, 6));

                // Nível Almoxarifado
                Perfil.TB_PERFIL_NIVEL.Add(criarPerfilNivel(Perfil.TB_PERFIL_ID, 7));

                #region Excluir Código Antigo
                //TB_PERFIL_NIVEL nivelOrgao = new TB_PERFIL_NIVEL();
                //nivelOrgao.TB_NIVEL_ID = 1;
                //Perfil.TB_PERFIL_NIVEL.Add(nivelOrgao);

                //TB_PERFIL_NIVEL nivelGestor = new TB_PERFIL_NIVEL();
                //nivelGestor.TB_NIVEL_ID = 6;
                //Perfil.TB_PERFIL_NIVEL.Add(nivelGestor);

                //TB_PERFIL_NIVEL nivelAlmoxarifado = new TB_PERFIL_NIVEL();
                //nivelAlmoxarifado.TB_NIVEL_ID = 7;
                //Perfil.TB_PERFIL_NIVEL.Add(nivelAlmoxarifado);
                #endregion
            }
            else if (perfilNivel == (int)Sam.Common.Util.GeralEnum.PerfilNivel.TipoRequisitante)
            {
                // Nível Órgão
                Perfil.TB_PERFIL_NIVEL.Add(criarPerfilNivel(Perfil.TB_PERFIL_ID, 1));

                // Nível UO
                Perfil.TB_PERFIL_NIVEL.Add(criarPerfilNivel(Perfil.TB_PERFIL_ID, 2));

                // Nível UGE
                Perfil.TB_PERFIL_NIVEL.Add(criarPerfilNivel(Perfil.TB_PERFIL_ID, 3));

                // Nível UA
                Perfil.TB_PERFIL_NIVEL.Add(criarPerfilNivel(Perfil.TB_PERFIL_ID, 4));

                // Nível Gestor
                Perfil.TB_PERFIL_NIVEL.Add(criarPerfilNivel(Perfil.TB_PERFIL_ID, 6));

                // Nível Div
                Perfil.TB_PERFIL_NIVEL.Add(criarPerfilNivel(Perfil.TB_PERFIL_ID, 9));

                #region Excluir Código Antigo
                //TB_PERFIL_NIVEL nivelOrgao = new TB_PERFIL_NIVEL();
                //nivelOrgao.TB_NIVEL_ID = 1;
                //Perfil.TB_PERFIL_NIVEL.Add(nivelOrgao);

                //TB_PERFIL_NIVEL nivelUO = new TB_PERFIL_NIVEL();
                //nivelUO.TB_NIVEL_ID = 2;
                //Perfil.TB_PERFIL_NIVEL.Add(nivelUO);

                //TB_PERFIL_NIVEL nivelUGE = new TB_PERFIL_NIVEL();
                //nivelUGE.TB_NIVEL_ID = 3;
                //Perfil.TB_PERFIL_NIVEL.Add(nivelUGE);

                //TB_PERFIL_NIVEL nivelUA = new TB_PERFIL_NIVEL();
                //nivelUA.TB_NIVEL_ID = 4;
                //Perfil.TB_PERFIL_NIVEL.Add(nivelUA);

                //TB_PERFIL_NIVEL nivelGestor = new TB_PERFIL_NIVEL();
                //nivelGestor.TB_NIVEL_ID = 6;
                //Perfil.TB_PERFIL_NIVEL.Add(nivelGestor);

                //TB_PERFIL_NIVEL nivelDiv = new TB_PERFIL_NIVEL();
                //nivelDiv.TB_NIVEL_ID = 9;
                //Perfil.TB_PERFIL_NIVEL.Add(nivelDiv);
                #endregion
            }

            Infrastructure.Insert(Perfil);
            Infrastructure.SaveChanges();
        }

        private TB_PERFIL_NIVEL criarPerfilNivel( short perfilId, byte nivelId)
        {
            TB_PERFIL_NIVEL _novoRegistro = new TB_PERFIL_NIVEL() { TB_PERFIL_ID = perfilId, TB_NIVEL_ID = nivelId };

            return _novoRegistro;
        }

        #endregion

        #region Métodos Genéricos

        /// <summary>
        /// Insere um novo objeto consistido
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Insert(TB_PERFIL entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    PerfilInfrastructure infraestrutura = new PerfilInfrastructure();
                    infraestrutura.Insert(entity);
                    infraestrutura.SaveChanges();
                }
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        /// <summary>
        /// Atualiza o objeto consitido
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Update(TB_PERFIL entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    PerfilInfrastructure infraestrutura = new PerfilInfrastructure();
                    infraestrutura.Update(entity);
                    infraestrutura.SaveChanges();
                }
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        /// <summary>
        /// Exclui o objeto não relacionado
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Delete(TB_PERFIL entity)
        {
            try
            {
                PerfilInfrastructure infraestrutura = new PerfilInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_PERFIL_ID == entity.TB_PERFIL_ID);
                infraestrutura.Delete(entity);
                infraestrutura.SaveChanges();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        /// <summary>
        /// Exclui todos os objetos relecionados
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void DeleteRelatedEntries(TB_PERFIL entity)
        {
            try
            {
                PerfilInfrastructure infraestrutura = new PerfilInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_PERFIL_ID == entity.TB_PERFIL_ID);
                infraestrutura.DeleteRelatedEntries(entity);

                infraestrutura.SaveChanges();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        /// <summary>
        /// Eclui os objetos relacionados com excessão a lista de ignorados
        /// </summary>
        /// <param name="entity">Objetos</param>
        /// <param name="keyListOfIgnoreEntites">Lista de tabelas ignoradas</param>
        public void DeleteRelatedEntries(TB_PERFIL entity, ObservableCollection<string> keyListOfIgnoreEntites)
        {
            try
            {
                PerfilInfrastructure infraestrutura = new PerfilInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_PERFIL_ID == entity.TB_PERFIL_ID);
                infraestrutura.DeleteRelatedEntries(entity, keyListOfIgnoreEntites);
                infraestrutura.SaveChanges();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        /// <summary>
        /// Retorna o primeiro registro encontrado na condição
        /// </summary>
        /// <param name="where">Expressão lambda where</param>
        /// <returns>Objeto</returns>
        public TB_PERFIL SelectOne(Expression<Func<TB_PERFIL, bool>> where)
        {
            try
            {
                PerfilInfrastructure infraestrutura = new PerfilInfrastructure();
                return infraestrutura.SelectOne(where);
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna todos os registros encontrados sem filtro
        /// </summary>
        /// <returns>Lista de objetos</returns>
        public List<TB_PERFIL> SelectAll()
        {
            try
            {
                PerfilInfrastructure infraestrutura = new PerfilInfrastructure();
                return infraestrutura.SelectAll();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna todos os objetos encontrados, ordenados e com paginação. Utilizado em Listas paginadas.
        /// </summary>
        /// <param name="sortExpression">Expressão de ordenação</param>
        /// <param name="maximumRows">Número máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">índice da paginação</param>
        /// <returns>Lista de objetos</returns>
        public List<TB_PERFIL> SelectAll(Expression<Func<TB_PERFIL, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                PerfilInfrastructure infraestrutura = new PerfilInfrastructure();
                var result = infraestrutura.SelectAll(sortExpression, startRowIndex);
                this.TotalRegistros = infraestrutura.TotalRegistros;

                return result;
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna uma lista de objetos encontrados na condição, ordenados e com paginação. Utilizado em Listas paginadas.
        /// </summary>
        /// <param name="where">Expressão lambda where</param>
        /// <returns>Lista de objetos</returns>
        public List<TB_PERFIL> SelectWhere(Expression<Func<TB_PERFIL, bool>> where)
        {
            try
            {
                PerfilInfrastructure infraestrutura = new PerfilInfrastructure();
                return infraestrutura.SelectWhere(where);
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna uma lista de objetos encontrados na condição, ordenados e paginados
        /// </summary>
        /// <param name="sortExpression">Expressão de ordenação para campos INT</param>
        /// <param name="desc">Ordem decrescente</param>
        /// <param name="where">Expressão lambda where</param>
        /// <param name="maximumRows">Número máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">índice da paginação</param>
        /// <returns>Lista de objetos</returns>
        public List<TB_PERFIL> SelectWhere(Expression<Func<TB_PERFIL, int>> sortExpression, bool desc, Expression<Func<TB_PERFIL, bool>> where, int startRowIndex)
        {
            try
            {
                PerfilInfrastructure infraestrutura = new PerfilInfrastructure();
                var result = infraestrutura.SelectWhere(sortExpression, desc, where, startRowIndex);
                this.TotalRegistros = infraestrutura.TotalRegistros;

                return result;
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna uma lista de objetos encontrados na condição, ordenados e paginados
        /// </summary>
        /// <param name="sortExpression">Expressão de ordenação para campos STRING</param>
        /// <param name="desc">Ordem decrescente</param>
        /// <param name="where">Expressão lambda where</param>
        /// <param name="maximumRows">Número máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">índice da paginação</param>
        /// <returns>Lista de objetos</returns>
        public List<TB_PERFIL> SelectWhere(Expression<Func<TB_PERFIL, string>> sortExpression, bool desc, Expression<Func<TB_PERFIL, bool>> where, int startRowIndex)
        {
            try
            {
                PerfilInfrastructure infraestrutura = new PerfilInfrastructure();
                var result = infraestrutura.SelectWhere(sortExpression, desc, where, startRowIndex);
                this.TotalRegistros = infraestrutura.TotalRegistros;

                return result;
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna todos os registros encontrados sem filtro
        /// </summary>
        /// <returns>IQueryable de objetos</returns>
        public IQueryable<TB_PERFIL> QueryAll()
        {
            try
            {
                PerfilInfrastructure infraestrutura = new PerfilInfrastructure();
                return infraestrutura.QueryAll();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna todos os registros encontrados sem filtro
        /// </summary>
        /// <param name="sortExpression">Expressão de ordenação</param>
        /// <param name="desc">Ordem decrescente</param>        
        /// <param name="maximumRows">Número máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">índice da paginação</param>
        /// <returns>IQueryable de objetos</returns>
        public IQueryable<TB_PERFIL> QueryAll(Expression<Func<TB_PERFIL, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                PerfilInfrastructure infraestrutura = new PerfilInfrastructure();
                var result = infraestrutura.QueryAll(sortExpression, startRowIndex);
                this.TotalRegistros = infraestrutura.TotalRegistros;

                return result;
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna o total de registros encontrados na tabela
        /// </summary>
        /// <returns>Total de registros inteiro</returns>
        public int GetCount()
        {
            try
            {
                PerfilInfrastructure infraestrutura = new PerfilInfrastructure();
                return infraestrutura.GetCount();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna o total de registros na condição
        /// </summary>
        /// <param name="where">Expressão lambda where</param>
        /// <returns>Total de registros inteiro</returns>
        public int GetCount(Expression<Func<TB_PERFIL, bool>> where)
        {
            try
            {
                PerfilInfrastructure infraestrutura = new PerfilInfrastructure();
                return infraestrutura.GetCount(where);
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        #endregion

        #region Métodos Outros

        /// <summary>
        /// Valida os campos da entidade antes da persistencia
        /// </summary>
        /// <param name="entity">Objeto a ser validado</param>
        public void Consistir(TB_PERFIL entity)
        {
            try
            {
                if (string.IsNullOrEmpty(entity.TB_PERFIL_DESCRICAO))
                {
                    throw new Exception("O campo Descrição é de preenchimento obritagório");
                }

            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        #endregion

        #region Métodos Segurança

        public PerfilBusiness()
        {
        }

        private Sam.Entity.Perfil perfil = new Sam.Entity.Perfil();

        public Sam.Entity.Perfil Perfil
        {
            get { return perfil; }
            set { perfil = value; }
        }

        public PerfilBusiness(string login)
            : base(login)
        { }

        public PerfilBusiness(int loginId)
            : base(loginId)
        { }

        public List<Perfil> RecuperarPerfil()
        {
            return new PerfilInfrastructureAntigo().RecuperarPerfil(base._loginId);
        }

        public bool ValidarPerfilConsultaWs(string cpf, string senhaCriptografada, string codigoOrgao)
        {
            return new PerfilInfrastructureAntigo().ValidarPerfilConsultaWs(cpf, senhaCriptografada, codigoOrgao);
        }

        public List<Perfil> RecuperarUsuarioPerfil()
        {
            //return new PerfilInfrastructureAntigo().RecuperarPerfil_Novo(base._loginId);

            var infraSegurancaAntiga = new PerfilInfrastructureAntigo();
            var listaPerfis = infraSegurancaAntiga.RecuperarPerfil_Novo(base._loginId);
            EstruturaOrganizacionalBusiness eoBusiness = new EstruturaOrganizacionalBusiness();
            AlmoxarifadoEntity dtoAlmox = null;
            DivisaoEntity dtoDivisao = null;
            int almoxID = 0;
            int divisaoID = 0;
            int divisaoPadraoRequisitanteGeralID = 0;
            string[] estruturaSAMInfos = null;
            string prefixoTipoEstruturaOrganizacional = null;
            string prefixoEstruturaSAM = null;
            string descricaoTipoPerfil = null;
            char chrSeparador = ',';
            foreach (var perfil in listaPerfis)
            {
                switch (perfil.IdPerfil)
                {
                    case (int)TipoPerfil.OperadorAlmoxarifado: almoxID = perfil.Almoxarifado.Id.Value; break;
                    case (int)TipoPerfil.Requisitante: divisaoID = perfil.Divisao.Id.Value; break;
                    case (int)TipoPerfil.AdministradorGestor: almoxID = perfil.Almoxarifado.Id.Value; break;
                    case (int)TipoPerfil.AdministradorOrgao: almoxID = perfil.Almoxarifado.Id.Value; break;
                    case (int)TipoPerfil.Financeiro: almoxID = perfil.Almoxarifado.Id.Value; break;
                    case (int)TipoPerfil.ConsultaRelatorio: almoxID = perfil.Almoxarifado.Id.Value; break;
                    case (int)TipoPerfil.RequisitanteGeral: divisaoPadraoRequisitanteGeralID = perfil.Divisao.Id.Value; break;
                    case (int)TipoPerfil.AdministradorFinanceiroSEFAZ: almoxID = perfil.Almoxarifado.Id.Value; break;
                    case (int)TipoPerfil.AdministradorGeral: almoxID = perfil.Almoxarifado.Id.Value; break;
                    default:
                        break;
                }


                if (almoxID != 0)
                {
                    dtoAlmox = eoBusiness.ObterAlmoxarifado(almoxID);
                    estruturaSAMInfos = new string[] { dtoAlmox.Uge.Codigo.Value.ToString("D6"), dtoAlmox.Descricao };

                    prefixoTipoEstruturaOrganizacional = "UGE";
                    prefixoEstruturaSAM = "Almoxarifado";
                }
                else if (divisaoID != 0)
                {
                    eoBusiness.SelectDivisao(divisaoID);
                    dtoDivisao = eoBusiness.Divisao;
                    estruturaSAMInfos = new string[] { dtoDivisao.Ua.Codigo.Value.ToString("D8"), dtoDivisao.Descricao, dtoDivisao.Ua.Uge.Codigo.Value.ToString("D6"), dtoDivisao.Almoxarifado.Descricao };
                    //estruturaSAMInfos = new string[] { dtoDivisao.Ua.Codigo.Valeue.ToString("D8"), dtoDivisao.Ua.Descricao, dtoDivisao.Ua.Uge.Codigo.Value.ToString("D6"), dtoDivisao.Almoxarifado.Descricao };

                    prefixoTipoEstruturaOrganizacional = String.Format(@"{0}: {1}, {2}: ""{3}"", {4}", "UGE", estruturaSAMInfos[2], "Almoxarifado", estruturaSAMInfos[3], "UA");
                    prefixoEstruturaSAM = "Divisão";
                }
                else if (divisaoPadraoRequisitanteGeralID != 0)
                {
                    eoBusiness.SelectDivisao(divisaoPadraoRequisitanteGeralID);
                    dtoDivisao = eoBusiness.Divisao;
                    estruturaSAMInfos = new string[] { dtoDivisao.Ua.Uge.Uo.Codigo.ToString("D5"), dtoDivisao.Ua.Uge.Uo.Descricao };

                    prefixoTipoEstruturaOrganizacional = "UO";
                    prefixoEstruturaSAM = "";
                    chrSeparador = '\0';
                }

                descricaoTipoPerfil = perfil.Descricao.BreakLine("->", 0).Trim();

                var _estruturaSAMInfos = estruturaSAMInfos.IsNull() ? "" : estruturaSAMInfos[0];
                var _estruturaSAMInfosA = estruturaSAMInfos.IsNull() ? "" : estruturaSAMInfos[1];

                perfil.Descricao = String.Format(@"{0} ({1}: {2}{3} {4}: ""{5}"")", descricaoTipoPerfil, prefixoTipoEstruturaOrganizacional ?? "", _estruturaSAMInfos, chrSeparador, prefixoEstruturaSAM ?? "", _estruturaSAMInfosA);


                almoxID = 0;
                divisaoID = 0;
                descricaoTipoPerfil = null;
            }

            return listaPerfis;
        }

        public List<Perfil> ListarPerfis(int? Peso)
        {
            if (Peso == null)
                return new PerfilInfrastructureAntigo().ListarPerfis();
            else
                return new PerfilInfrastructureAntigo().ListarPerfisPeso(Peso);
        }

        public void LiberarPerfil(List<Transacao> transacoes, Perfil perfil)
        {
            PerfilInfrastructureAntigo.CriarTransacoes(transacoes, perfil, true);
        }

        public void CopiarPerfil(Perfil origem, Perfil destino)
        {
            destino.Transacoes = origem.Transacoes;
            Infrastructure.PerfilInfrastructureAntigo.CriarPerfil(destino);
        }

        public void InativarUsuario()
        {
            Infrastructure.PerfilInfrastructureAntigo.InativarUsuario(base._usuario);
        }

        public void AtribuirPermissaoUsuario(List<Perfil> perfil, Usuario login)
        {
            Infrastructure.PerfilInfrastructureAntigo.AtribuirPermissoes(perfil, login);
        }

        public IList<PerfilLoginNivelAcesso> ListarPerfilLoginNivelAcesso(int _perfilId, int _perfilLoginId)
        {
            return new Infrastructure.PerfilInfrastructureAntigo().ListarPerfilLoginNivelAcesso(_perfilLoginId, _perfilId);
        }

        public bool SalvarPerfil()
        {
            try
            {
                Infrastructure.PerfilInfrastructureAntigo infra = new PerfilInfrastructureAntigo();
                infra.Entity = this.Perfil;
                infra.Salvar();
                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Erro no sistema: " + e.Message);
                return false;
            }
        }


        public bool InsertListControleImportacao(TB_CONTROLE entityList)
        {

            StringBuilder seq = new StringBuilder();
            PerfilLoginBusiness estruturaPerfil = new PerfilLoginBusiness();
            Login loginPerfil = new Login();


            try
            {
                bool isErro = false;
                CargaBusiness cargaBusiness = new CargaBusiness();

                foreach (var carga in entityList.TB_CARGA)
                {

                    seq.Clear();
                    seq.Append(carga.TB_CARGA_SEQ);

                    PerfilLogin perfilLoginEntity = new PerfilLogin();
                    UgeBusiness business = new UgeBusiness();
                    int? uoId = null;

                    if (carga.TB_UGE_ID != null)
                    {
                         uoId = business.ObterUGE((int)carga.TB_UGE_ID).FirstOrDefault().TB_UO_ID;
                    }


                    if (carga.TB_USUARIO_ID != null)
                    {
                        int usuarioId = (int)carga.TB_USUARIO_ID;
                        loginPerfil = PopularDadosLoginId(usuarioId);                      

                       
                    }

                    if (ConsistirPerfil(carga, uoId, loginPerfil))
                    {
                        //Perfil Login      
                                              
                        
                        perfilLoginEntity.PerfilId = (int)TipoPerfil.Requisitante;
                        perfilLoginEntity.LoginId = (int)loginPerfil.Id;


                        perfilLoginEntity.OrgaoPadraoId = (int)carga.TB_ORGAO_ID;
                        perfilLoginEntity.AlmoxarifadoPadraoId = 0;
                        perfilLoginEntity.DivisaoPadraoId = (int)carga.TB_DIVISAO_ID;
                        perfilLoginEntity.GestorPadraoId = (int)carga.TB_GESTOR_ID;
                        perfilLoginEntity.OrgaoPadraoId = (int)carga.TB_ORGAO_ID;
                        perfilLoginEntity.UAPadraoId = (int)carga.TB_UA_ID;
                        perfilLoginEntity.UGEPadraoId = (int)carga.TB_UGE_ID;
                        perfilLoginEntity.UOPadraoId = (int)uoId;




                        List<PerfilLoginNivelAcesso> PerfilNivelAcessoList = new List<PerfilLoginNivelAcesso>();
                        PerfilNivelAcessoList.Add(ReturnaNivelAcesso((int)carga.TB_ORGAO_ID, (int)Sam.Common.NivelAcessoEnum.Orgao));
                        PerfilNivelAcessoList.Add(ReturnaNivelAcesso((int)uoId, (int)Sam.Common.NivelAcessoEnum.UO));
                        PerfilNivelAcessoList.Add(ReturnaNivelAcesso((int)carga.TB_UA_ID, (int)Sam.Common.NivelAcessoEnum.UA));
                        PerfilNivelAcessoList.Add(ReturnaNivelAcesso((int)carga.TB_UGE_ID, (int)Sam.Common.NivelAcessoEnum.UGE));
                        PerfilNivelAcessoList.Add(ReturnaNivelAcesso((int)carga.TB_GESTOR_ID, (int)Sam.Common.NivelAcessoEnum.GESTOR));
                        PerfilNivelAcessoList.Add(ReturnaNivelAcesso((int)carga.TB_DIVISAO_ID, (int)Sam.Common.NivelAcessoEnum.DIVISAO));





                        perfilLoginEntity.PerfilLoginNivelAcessoList = PerfilNivelAcessoList;


                        estruturaPerfil.SalvarLoginPerfil(perfilLoginEntity, true);

                    }
                    else
                    {
                        foreach (GeralEnum.CargaErro erroEnum in ListCargaErro)
                        {
                            TB_CARGA_ERRO cargaErro = new TB_CARGA_ERRO();
                            CargaErroInfrastructure infraCargaErro = new CargaErroInfrastructure();

                            carga.TB_CARGA_VALIDO = false;
                            cargaBusiness.Update(carga);

                            cargaErro.TB_CARGA_ID = carga.TB_CARGA_ID;
                            cargaErro.TB_ERRO_ID = (int)erroEnum;

                            infraCargaErro.Insert(cargaErro);
                            infraCargaErro.SaveChanges();
                            infraCargaErro.Dispose();



                            isErro = true;
                        }
                    }


                }

                return isErro;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }


        }

        public bool ConsistirPerfil(TB_CARGA carga, int? uoId, Login loginPerfil)
        {

            listaErro = new List<string>();
            listCargaErro = new List<GeralEnum.CargaErro>();

            

           

            if (string.IsNullOrWhiteSpace(carga.TB_USUARIO_CPF))
            {
                this.ListaErro.Add("CPF obrigatório!");
                ListCargaErro.Add(GeralEnum.CargaErro.CPFObrigatorio);
            }

            if (carga.TB_USUARIO_ID == null)
            {
                this.ListaErro.Add("CPF inválido!");
                ListCargaErro.Add(GeralEnum.CargaErro.CPFInvalido);
            }

            if (string.IsNullOrWhiteSpace(carga.TB_UA_CODIGO) || carga.TB_UA_ID == null)
            {
                this.ListaErro.Add("Código da UA inválido.!");
                ListCargaErro.Add(GeralEnum.CargaErro.CodigoUAInvalido);
            }

            if (string.IsNullOrWhiteSpace(carga.TB_DIVISAO_CODIGO) || carga.TB_DIVISAO_ID == null)
            {
                this.ListaErro.Add("Código da Divisão inválido.");
                ListCargaErro.Add(GeralEnum.CargaErro.DivisaoInvalido);
            }

            if (string.IsNullOrWhiteSpace(carga.TB_GESTOR_NOME_REDUZIDO) || carga.TB_GESTOR_ID == null)
            {
                this.ListaErro.Add("Gestor inválido.");
                ListCargaErro.Add(GeralEnum.CargaErro.GestorInvalido);
            }

            if (carga.TB_ORGAO_ID == null || carga.TB_UGE_ID == null || uoId == null)
            {
                this.ListaErro.Add("Estrutura Orgão e/ou UGE e/ou UO está(ão) inválido(s).");
                ListCargaErro.Add(GeralEnum.CargaErro.EstruturaOrgaoUGEUOInvalido);
            }


            if (loginPerfil.Perfis != null)
            {
                foreach (var item in loginPerfil.Perfis)
                {
                    if (item.IdPerfil == (int)TipoPerfil.Requisitante)
                    {
                        if ((carga.TB_UA_ID != null) && (carga.TB_DIVISAO_ID != null) &&
                            (item.PerfilLoginNivelAcesso[2].Valor != null) && (item.PerfilLoginNivelAcesso[5].Valor != null))
                        {
                            if (item.PerfilLoginNivelAcesso[2].Valor == (int)carga.TB_UA_ID && item.PerfilLoginNivelAcesso[5].Valor == (int)carga.TB_DIVISAO_ID)
                            {
                                this.ListaErro.Add("Perfil Requisitante cadastrado!");
                                ListCargaErro.Add(GeralEnum.CargaErro.PerfilRequisitanteCadastrado);
                            }
                        }

                    }
                }
            }


            if (this.ListaErro.Count > 0)
            {
                return false;
            }

            return true;
        }

        private PerfilLoginNivelAcesso ReturnaNivelAcesso(int valorId, int nivelId)
        {
            PerfilLoginNivelAcesso perfilLoginNivelAcesso = new PerfilLoginNivelAcesso();
            perfilLoginNivelAcesso.NivelAcesso = new NivelAcesso();

            if (valorId > 0)
            {
                perfilLoginNivelAcesso.Valor = Convert.ToInt32(valorId);
                perfilLoginNivelAcesso.NivelAcesso.NivelId = (short)nivelId;
            }

            return perfilLoginNivelAcesso;
        }




        public Login PopularDadosLoginId(int _userId)
        {
            Sam.Business.LoginBusiness log = new Sam.Business.LoginBusiness("", "");
            return log.RecuperarInformacoesLoginPorUserId(_userId);

        }




        #endregion
    }
}
