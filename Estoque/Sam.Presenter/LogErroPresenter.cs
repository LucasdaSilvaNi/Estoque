using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Facade;
using Sam.View;
using Sam.Domain.Business;
using Sam.Domain.Entity;
using Sam.Common.Util;
using Sam.Entity;

namespace Sam.Presenter
{
    public class LogErroPresenter
    {
        ILogErroView _view;

        public ILogErroView View
        {
            get { return _view; }
            set { _view = value; }
        }

        public LogErroPresenter()
        {
        }

        public LogErroPresenter(ILogErroView view)
        {
            this.View = view;
        }

        public int TotalRegistrosGrid
        {
            get;
            set;
        }

        public void GravarLogErro(Exception ex)
        {
            
        }
        //public int QtdRegistros;
        public IList<LogErroEntity> ListarLogErro(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            LogErroEntity logParam = new LogErroEntity();
            logParam.QtdRegistros = 1000;

            LogErro business = new LogErro();

            IList<LogErroEntity> logErros = business.ListarLogErro(logParam, startRowIndexParameterName, maximumRowsParameterName);
            this.TotalRegistrosGrid = business.QtdRegistros;
            return logErros;
        }
        
        public int TotalRegistros(int maximumRowsParameterName, int startRowIndexParameterName)
        {
            return this.TotalRegistrosGrid;
        }


        public IList<LogErroEntity> PopularListaLogErro(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            LogErroEntity logParam = new LogErroEntity();
            logParam.QtdRegistros = 1000;

            LogErro business = new LogErro();

            IList<LogErroEntity> logErros = business.PopularListaLogErro(logParam
                                                                        , startRowIndexParameterName
                                                                        , maximumRowsParameterName);
            this.TotalRegistrosGrid = business.QtdRegistros;
            return logErros;
        }


    }      
}
