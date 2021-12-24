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
    public partial class CalendarioFechamentoMensalInfrastructure : BaseInfraestructure, ICalendarioFechamentoMensalService
    {
        private CalendarioFechamentoMensalEntity dataFechamento = new CalendarioFechamentoMensalEntity();
        public CalendarioFechamentoMensalEntity Entity
        {
            get { return dataFechamento; }
            set { dataFechamento = value; }
        }

        public IList<CalendarioFechamentoMensalEntity> ObterDatasFechamentoMensalPorAnoReferencia(int anoReferencia)
        {
            IList<CalendarioFechamentoMensalEntity> lstRetorno = null;
            IQueryable<TB_CALENDARIO_FECHAMENTO_MENSAL> qryConsulta = null;
            Expression<Func<TB_CALENDARIO_FECHAMENTO_MENSAL, bool>> expWhere;

            expWhere = (mesFechamento => mesFechamento.TB_CALENDARIO_FECHAMENTO_MENSAL_ANO_REFERENCIA == anoReferencia);

            qryConsulta = Db.TB_CALENDARIO_FECHAMENTO_MENSALs.Where(expWhere)
                                               .AsQueryable();

            #region debug SQL
            var _strSQL = qryConsulta.ToString();
            Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            #endregion

            lstRetorno = qryConsulta.Select(_instanciadorDTOCalendarioFechamentoMensal())
                                     .ToList();

            return lstRetorno;
        }
        public CalendarioFechamentoMensalEntity ObterDataFechamentoMensal(int mesReferencia, int anoReferencia)
        {
            CalendarioFechamentoMensalEntity objEntidade = null;
            IQueryable<TB_CALENDARIO_FECHAMENTO_MENSAL> qryConsulta = null;
            Expression<Func<TB_CALENDARIO_FECHAMENTO_MENSAL, bool>> expWhere;

            expWhere = (mesFechamento => mesFechamento.TB_CALENDARIO_FECHAMENTO_MENSAL_MES_REFERENCIA == mesReferencia
                                      && mesFechamento.TB_CALENDARIO_FECHAMENTO_MENSAL_ANO_REFERENCIA == anoReferencia);

            qryConsulta = (from mesFechamento in Db.TB_CALENDARIO_FECHAMENTO_MENSALs
                           select mesFechamento).AsQueryable();


            #region debug SQL
            var _strSQL = qryConsulta.ToString();
            Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            #endregion

            objEntidade = qryConsulta.Where(expWhere)
                                     .Select(_instanciadorDTOCalendarioFechamentoMensal())
                                     .FirstOrDefault();

            return objEntidade;
        }
        
        public IList<CalendarioFechamentoMensalEntity> Imprimir()
        {
            throw new NotImplementedException();
        }

        public IList<CalendarioFechamentoMensalEntity> Listar()
        {
            IList<CalendarioFechamentoMensalEntity> lstRetorno = null;
            IQueryable<TB_CALENDARIO_FECHAMENTO_MENSAL> qryConsulta = null;

            qryConsulta = Db.TB_CALENDARIO_FECHAMENTO_MENSALs.AsQueryable();
            qryConsulta = qryConsulta.OrderBy(datasFechamento => datasFechamento.TB_CALENDARIO_FECHAMENTO_MENSAL_ANO_REFERENCIA)
                                     .ThenBy(datasFechamento => datasFechamento.TB_CALENDARIO_FECHAMENTO_MENSAL_MES_REFERENCIA);

            lstRetorno = qryConsulta.Select(_instanciadorDTOCalendarioFechamentoMensal())
                                    .ToList();

            return lstRetorno;
        }

        public IList<CalendarioFechamentoMensalEntity> Listar(int Ano)
        {
            IList<CalendarioFechamentoMensalEntity> lstRetorno = Listar();

            return (from _lista in lstRetorno
                    where _lista.AnoReferencia == Ano
                    orderby _lista.AnoReferencia, _lista.MesReferencia
                    select _lista).DefaultIfEmpty().ToList();
        }

        public IList<int> ListarAno()
        {
            IList<CalendarioFechamentoMensalEntity> lstRetorno = Listar();

            return (from _lista in lstRetorno
                    orderby _lista.AnoReferencia descending
                    select _lista.AnoReferencia).DefaultIfEmpty().Distinct().ToList<int>();
        }

        public CalendarioFechamentoMensalEntity LerRegistro()
        {
            throw new NotImplementedException();
        }

        public IList<CalendarioFechamentoMensalEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }

        void ICrudBaseService<CalendarioFechamentoMensalEntity>.Excluir()
        {
            throw new NotImplementedException();
        }

        public bool ExisteCodigoInformado()
        {
            throw new NotImplementedException();
        }

        public bool Excluir()
        {
            bool blnRetorno = false;
            TB_CALENDARIO_FECHAMENTO_MENSAL rowTabela = this.Db.TB_CALENDARIO_FECHAMENTO_MENSALs.Where(mesFechamento => mesFechamento.TB_CALENDARIO_FECHAMENTO_MENSAL_ID == this.Entity.Id).FirstOrDefault();

            if (rowTabela.IsNotNull())
            {
                this.Db.TB_CALENDARIO_FECHAMENTO_MENSALs.DeleteOnSubmit(rowTabela);
                this.Db.SubmitChanges();
            }

            blnRetorno = this.Db.TB_CALENDARIO_FECHAMENTO_MENSALs.Where(mesFechamento => mesFechamento.TB_CALENDARIO_FECHAMENTO_MENSAL_ID == this.Entity.Id).FirstOrDefault().IsNull();

            return blnRetorno;
        }

        public void Salvar()
        {
            TB_CALENDARIO_FECHAMENTO_MENSAL rowTabela = new TB_CALENDARIO_FECHAMENTO_MENSAL();

            if (this.Entity.Id.HasValue && this.Entity.Id.Value != 0)
                rowTabela = this.Db.TB_CALENDARIO_FECHAMENTO_MENSALs.Where(a => a.TB_CALENDARIO_FECHAMENTO_MENSAL_ID == this.Entity.Id.Value).FirstOrDefault();
            else
                this.Db.TB_CALENDARIO_FECHAMENTO_MENSALs.InsertOnSubmit(rowTabela);

            rowTabela.TB_CALENDARIO_FECHAMENTO_MENSAL_MES_REFERENCIA = this.Entity.MesReferencia;
            rowTabela.TB_CALENDARIO_FECHAMENTO_MENSAL_ANO_REFERENCIA = this.Entity.AnoReferencia;
            rowTabela.TB_CALENDARIO_FECHAMENTO_MENSAL_DATA_FECHAMENTO_DESPESA = this.Entity.DataFechamentoDespesa;

            this.Db.SubmitChanges();
        }

        public bool PodeExcluir()
        {
            throw new NotImplementedException();
        }

        public bool ExisteMesReferenciaInformado()
        {
            return (this.Db.TB_CALENDARIO_FECHAMENTO_MENSALs.Where(mesFechamento => mesFechamento.TB_CALENDARIO_FECHAMENTO_MENSAL_MES_REFERENCIA == this.Entity.MesReferencia
                                                                                 && mesFechamento.TB_CALENDARIO_FECHAMENTO_MENSAL_ANO_REFERENCIA == this.Entity.AnoReferencia)
                                                            .Count() == 1);
        }

        private Func<TB_CALENDARIO_FECHAMENTO_MENSAL, CalendarioFechamentoMensalEntity> _instanciadorDTOCalendarioFechamentoMensal()
        {
            Func<TB_CALENDARIO_FECHAMENTO_MENSAL, CalendarioFechamentoMensalEntity> _actionSeletor = null;

                _actionSeletor = (mesReferencia => new CalendarioFechamentoMensalEntity()
                {
                    Id                    = mesReferencia.TB_CALENDARIO_FECHAMENTO_MENSAL_ID,
                    AnoReferencia         = mesReferencia.TB_CALENDARIO_FECHAMENTO_MENSAL_ANO_REFERENCIA,
                    MesReferencia         = mesReferencia.TB_CALENDARIO_FECHAMENTO_MENSAL_MES_REFERENCIA,
                    DataFechamentoDespesa = mesReferencia.TB_CALENDARIO_FECHAMENTO_MENSAL_DATA_FECHAMENTO_DESPESA,

                    CodigoDescricao = (new DateTime(mesReferencia.TB_CALENDARIO_FECHAMENTO_MENSAL_ANO_REFERENCIA, mesReferencia.TB_CALENDARIO_FECHAMENTO_MENSAL_MES_REFERENCIA, 01).ToString("MMMyyyy"))
                });

            return _actionSeletor;
        }
    }
}
