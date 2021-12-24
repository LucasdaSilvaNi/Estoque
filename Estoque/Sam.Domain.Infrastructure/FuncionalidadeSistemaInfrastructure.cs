using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sam.Common.Util;
using Sam.Domain.Entity;
using Sam.ServiceInfraestructure;



namespace Sam.Domain.Infrastructure
{
    public partial class FuncionalidadeSistemaInfrastructure : BaseInfraestructure, IFuncionalidadeSistemaService
    {
        private FuncionalidadeSistemaEntity funcionalidadeSistema = new FuncionalidadeSistemaEntity();
        public FuncionalidadeSistemaEntity Entity
        {
            get { return funcionalidadeSistema; }
            set { funcionalidadeSistema = value; }
        }

        public FuncionalidadeSistemaEntity ObterFuncionalidadeSistema(int funcionalidadeSistemaID)
        {
            FuncionalidadeSistemaEntity objEntidade = null;
            IQueryable<TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMA> qryConsulta = null;
            Expression<Func<TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMA, bool>> expWhere;

            expWhere = (funcionalidadeSistema => funcionalidadeSistema.TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMA_ID == funcionalidadeSistemaID);

            qryConsulta = Db.TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMAs.Where(expWhere)
                                               .AsQueryable();

            #region debug SQL
            var _strSQL = qryConsulta.ToString();
            Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            #endregion

            objEntidade = qryConsulta.Select(_instanciadorDTOFuncionalidadeSistema())
                                     .FirstOrDefault();

            return objEntidade;
        }
        public IList<FuncionalidadeSistemaEntity> Imprimir()
        {
            throw new NotImplementedException();
        }

        public IList<FuncionalidadeSistemaEntity> Listar()
        {
            IList<FuncionalidadeSistemaEntity> lstRetorno = null;
            IQueryable<TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMA> qryConsulta = null;

            qryConsulta = Db.TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMAs.AsQueryable();

            #region debug SQL
            var _strSQL = qryConsulta.ToString();
            Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            #endregion

            lstRetorno = qryConsulta.Select(_instanciadorDTOFuncionalidadeSistema())
                                    .ToList();

            return lstRetorno;
        }

        public IList<FuncionalidadeSistemaEntity> Listar(int[] perfilIDs)
        {
            IList<FuncionalidadeSistemaEntity> lstRetorno = null;
            IQueryable<TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMA> qryConsulta = null;


            

            if (perfilIDs.HasElements() && !perfilIDs.Contains((int)Sam.Common.Util.GeralEnum.TipoPerfil.AdministradorGeral))
                qryConsulta = Db.TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMAs.AsQueryable().Where(funcionalidadeSistema => perfilIDs.Contains(funcionalidadeSistema.TB_PERFIL_ID) || funcionalidadeSistema.TB_PERFIL_ID == 0);
            else if (perfilIDs.HasElements() && perfilIDs.Contains((int)Sam.Common.Util.GeralEnum.TipoPerfil.AdministradorGeral))
                qryConsulta = Db.TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMAs.AsQueryable();


            #region debug SQL
            var _strSQL = qryConsulta.ToString();
            Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            #endregion

            lstRetorno = qryConsulta.Select(_instanciadorDTOFuncionalidadeSistema())
                                    //.DistinctBy(funcionalidadeSistema => funcionalidadeSistema.Descricao)
                                    .ToList();

            return lstRetorno;
        }

        public FuncionalidadeSistemaEntity LerRegistro()
        {
            throw new NotImplementedException();
        }

        public IList<FuncionalidadeSistemaEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }

        void ICrudBaseService<FuncionalidadeSistemaEntity>.Excluir()
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            return Db.TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMAs.Count();
        }

        public bool Excluir()
        {
            bool blnRetorno = false;
            TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMA rowTabela = this.Db.TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMAs.Where(funcionalidadeSistema => funcionalidadeSistema.TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMA_ID == this.Entity.Id).FirstOrDefault();

            if (rowTabela.IsNotNull())
            {
                this.Db.TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMAs.DeleteOnSubmit(rowTabela);
                this.Db.SubmitChanges();
            }

            blnRetorno = this.Db.TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMAs.Where(funcionalidadeSistema => funcionalidadeSistema.TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMA_ID == this.Entity.Id).FirstOrDefault().IsNull();

            return blnRetorno;
        }

        public void Salvar()
        {
            TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMA rowTabela = new TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMA();

            if (this.Entity.Id.HasValue && this.Entity.Id.Value != 0)
                rowTabela = this.Db.TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMAs.Where(a => a.TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMA_ID == this.Entity.Id.Value).FirstOrDefault();
            else
                this.Db.TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMAs.InsertOnSubmit(rowTabela);

            rowTabela.TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMA_DESCRICAO = this.Entity.Descricao;

            this.Db.SubmitChanges();

            this.Entity.Id = rowTabela.TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMA_ID;
        }

        public bool PodeExcluir()
        {
            throw new NotImplementedException();
        }

        public bool ExisteCodigoInformado()
        {
            return (this.Db.TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMAs.Where(funcionalidadeSistema => funcionalidadeSistema.TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMA_ID == this.Entity.Id)
                                                                      .CountReadUncommitted() == 1);
        }

        private Func<TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMA, FuncionalidadeSistemaEntity> _instanciadorDTOFuncionalidadeSistema()
        {
            Func<TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMA, FuncionalidadeSistemaEntity> _actionSeletor = null;

                _actionSeletor = (funcionalidadeSistema => new FuncionalidadeSistemaEntity()
                {
                    Id              = funcionalidadeSistema.TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMA_ID,
                    Descricao       = funcionalidadeSistema.TB_CHAMADO_SUPORTE_FUNCIONALIDADE_SISTEMA_DESCRICAO,
                });

            return _actionSeletor;
        }
    }
}
