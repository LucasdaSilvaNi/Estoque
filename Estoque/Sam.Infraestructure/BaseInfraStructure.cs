using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Sam.Infrastructure;


namespace Sam.Infrastructure
{
    public class BaseInfrastructure
    {
         SEGwebEntities db;
        SAMwebEntities dbSam;

        public SEGwebEntities Db
        {
            get
            {
                if (this.db == null)
                {
                    db = ContextManager.GetContext<SEGwebEntities>();
                }
                return db;

                //if (db == null)
                //    db = new SEGwebEntities(ConfigurationManager.ConnectionStrings["SEGwebEntities"].ConnectionString);

                //return db;
            }
        }

        public SAMwebEntities DbSam
        {
            get
            {

                if (this.dbSam == null)
                    dbSam = ContextManager.GetContext<SAMwebEntities>();
                return dbSam;

                //if (this.dbSam == null)
                //    this.dbSam
                //        = new SAMwebEntities(ConfigurationManager.ConnectionStrings["SAMwebEntities"].ConnectionString);
                //return dbSam;
            }
        }


        public int RegistrosPagina
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["TOP"].ToString());
            }
        }

        public int PularRegistros
        {
            get;
            set;
        }

        internal int totalregistros;

        public int TotalRegistros()
        {
            return totalregistros;
        }

        public virtual void ComitarTransacao()
        {}

        public virtual void RollbackTransacao()
        { }


   }
}
