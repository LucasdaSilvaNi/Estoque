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
using Sam.Infrastructure.Infrastructure.Segurança;
using System.Data;
using System.Data.SqlClient;
using Sam.Common.Enums;
using Sam.Common.Util;

namespace Sam.Business.Business.Seguranca
{
    public class ESPBusiness : BaseBusiness, ICrudBaseBusiness<TB_ESP>
    {
        #region [ Implementação métodos da Interface ]
        #region [ Consistir ]
        public void Consistir(TB_ESP entity)
        {
            try
            {
                DateTime _dtIniVige, _dtFimVige;
                DateTime.TryParse(entity.TB_ESP_INICIO_VIGENCIA.ToString(), out _dtIniVige);
                DateTime.TryParse(entity.TB_ESP_FIM_VIGENCIA.ToString(), out _dtFimVige);

                if (entity.TB_ESP_CODIGO == 0)
                    throw new Exception("O campo Código é de preenchimento obritagório");

                if (_dtIniVige == DateTime.MinValue)
                    throw new Exception("O campo Data Início Vigência é de preenchimento obritagório");

                if (_dtFimVige == DateTime.MinValue)
                    throw new Exception("O campo Data Fim Vigência é de preenchimento obritagório");

                if (entity.TB_ESP_INICIO_VIGENCIA <= DateTime.MinValue)
                    throw new Exception("O campo Data Início Vigência é de preenchimento obritagório");

                if (entity.TB_ESP_FIM_VIGENCIA <= DateTime.MinValue)
                    throw new Exception("O campo Data Fim Vigência é de preenchimento obritagório");

                if (entity.TB_ESP_FIM_VIGENCIA <= entity.TB_ESP_INICIO_VIGENCIA)
                    throw new Exception("O campo Data Fim Vigência não pode ser inferior ou igual a Data Início Vigência");
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);

                if (e.InnerException != null)
                    throw new Exception(e.Message, e.InnerException);

                throw new Exception(e.Message, e);
            }
        }
        #endregion

        #region [ Delete ]
        public void Delete(TB_ESP entity)
        {
            try
            {
                ESPInfrastructure infraestrutura = new ESPInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_ESP_ID == entity.TB_ESP_ID);
                infraestrutura.Delete(entity);
                infraestrutura.SaveChanges();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public void DeleteRelatedEntries(TB_ESP entity)
        {
            try
            {
                ESPInfrastructure infraestrutura = new ESPInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_ESP_ID == entity.TB_ESP_ID);
                infraestrutura.DeleteRelatedEntries(entity);

                infraestrutura.SaveChanges();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public void DeleteRelatedEntries(TB_ESP entity, ObservableCollection<string> keyListOfIgnoreEntites)
        {
            try
            {
                ESPInfrastructure infraestrutura = new ESPInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_ESP_ID == entity.TB_ESP_ID);
                infraestrutura.DeleteRelatedEntries(entity, keyListOfIgnoreEntites);
                infraestrutura.SaveChanges();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
            }
        }
        #endregion

        #region [ GetCount ]
        public int GetCount()
        {
            try
            {
                ESPInfrastructure infraestrutura = new ESPInfrastructure();
                return infraestrutura.GetCount();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        public int GetCount(Expression<Func<TB_ESP, bool>> where)
        {
            try
            {
                ESPInfrastructure infraestrutura = new ESPInfrastructure();
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

        #region [ Insert ]
        public void Insert(TB_ESP entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    ESPInfrastructure infraestrutura = new ESPInfrastructure();
                    entity.TB_ESP_ID = 0;
                    infraestrutura.Insert(entity);
                    infraestrutura.SaveChanges();
                }
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);

                if (erroUniqueKey(e))
                    throw new Exception(Constante.CST_MSG_ERRO_ESP_DUPLICIDA, e.InnerException);
                else
                    throw new Exception(e.Message, e.InnerException);
            }
        }
        #endregion

        #region [ QueryAll ]
        public IQueryable<TB_ESP> QueryAll()
        {
            try
            {
                ESPInfrastructure infraestrutura = new ESPInfrastructure();
                return infraestrutura.QueryAll();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        public IQueryable<TB_ESP> QueryAll(Expression<Func<TB_ESP, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                ESPInfrastructure infraestrutura = new ESPInfrastructure();
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
        #endregion

        #region [ SelectAll ]
        public List<TB_ESP> SelectAll()
        {
            try
            {
                ESPInfrastructure infraestrutura = new ESPInfrastructure();
                return infraestrutura.SelectAll();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        public List<TB_ESP> SelectAll(Expression<Func<TB_ESP, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                ESPInfrastructure infraestrutura = new ESPInfrastructure();
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
        #endregion

        #region [ SelectOne ]
        public TB_ESP SelectOne(Expression<Func<TB_ESP, bool>> where)
        {
            try
            {
                ESPInfrastructure infraestrutura = new ESPInfrastructure();
                return infraestrutura.SelectOne(where);
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }
        #endregion

        #region [ SelectWhere ]
        public List<TB_ESP> SelectWhere(Expression<Func<TB_ESP, bool>> where)
        {
            try
            {
                ESPInfrastructure _infra = new ESPInfrastructure();
                return _infra.SelectWhere(where);
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        public List<TB_ESP> SelectWhere(Expression<Func<TB_ESP, string>> sortExpression, bool desc, Expression<Func<TB_ESP, bool>> where, int startRowIndex)
        {
            try
            {
                ESPInfrastructure infraestrutura = new ESPInfrastructure();
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

        public List<TB_ESP> SelectWhere(Expression<Func<TB_ESP, int>> sortExpression, bool desc, Expression<Func<TB_ESP, bool>> where, int startRowIndex)
        {
            try
            {
                ESPInfrastructure infraestrutura = new ESPInfrastructure();
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
        #endregion

        #region [ Update ]
        public void Update(TB_ESP entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    ESPInfrastructure infraestrutura = new ESPInfrastructure();
                    infraestrutura.Update(entity);
                    infraestrutura.SaveChanges();
                }
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);

                if (erroUniqueKey(e))
                    throw new Exception(Constante.CST_MSG_ERRO_ESP_DUPLICIDA, e.InnerException);
                else
                    throw new Exception(e.Message, e.InnerException);
            }
        }
        #endregion
        #endregion

        public bool erroUniqueKey(Exception e)
        {
            return e.InnerException != null 
                && e.InnerException.GetType().FullName == "System.Data.SqlClient.SqlException" 
                && ((System.Runtime.InteropServices.ExternalException)e.InnerException).ErrorCode == -2146232060;
        }

        #region [ ListarESP ]
        public List<ESPEntity> ListarESP()
        {
            ESPInfrastructure _espInfra = new ESPInfrastructure();
            var _result = _espInfra.ListarESP(this.SkipRegistros);

            this.TotalRegistros = _espInfra.TotalRegistros;

            return _result;
        }
        #endregion

        public int ObterTotalUsuarios(int gestorId, DateTime dataAvaliacao)
        {
            var _usuarioNivelI = ObterTotalUsuarios(gestorId, dataAvaliacao, PerfilNivelAcessoEnum.PerfilNivelAcesso.Nivel_I);
            var _usuarioNivelII = ObterTotalUsuarios(gestorId, dataAvaliacao, PerfilNivelAcessoEnum.PerfilNivelAcesso.Nivel_II);

            return _usuarioNivelI + _usuarioNivelII;
        }

        public int ObterTotalUsuarios(int gestorId, DateTime dataAvaliacao, PerfilNivelAcessoEnum.PerfilNivelAcesso usuarioNivel)
        {
            ESPInfrastructure _esp = new ESPInfrastructure();
            _esp.GestorID = gestorId;
            _esp.DataAvaliacao = dataAvaliacao;

            return _esp.ObterTotalUsuarioNivel(usuarioNivel);
        }

        public int ObterTotalRepositorio(int gestorId, DateTime dataAvaliacao, RepositorioEnum.Repositorio repositorio)
        {
            ESPInfrastructure _esp = new ESPInfrastructure();
            _esp.GestorID = gestorId;
            _esp.DataAvaliacao = dataAvaliacao;

            return _esp.ObterTotalRepositorio(repositorio);
        }
    }
}
