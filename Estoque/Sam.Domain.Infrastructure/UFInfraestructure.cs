using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using System.Configuration;
using System.Collections;
using Sam.Domain.Entity;


namespace Sam.Domain.Infrastructure
{
    public partial class UFInfraestructure : BaseInfraestructure, IUFService
    {
        #region ICrudBaseService<UFEntity> Members

        private UFEntity uf = new UFEntity();
        
        public UFEntity Entity
        {
            get { return uf; }

            set { uf = value; }
        }

        public IList<UFEntity> Listar()
        {
            IList<UFEntity> resultado = (from a in this.Db.TB_UFs
                                        orderby a.TB_UF_ID
                                         select new UFEntity
                                         {
                                             Id = a.TB_UF_ID,
                                             Descricao = a.TB_UF_DESCRICAO,
                                             Sigla = a.TB_UF_SIGLA,
                                            
                                         }).ToList<UFEntity>();

          
            return resultado;
        }

        public IList<UFEntity> Imprimir()
        {
            throw new NotImplementedException();
        }

        public void Salvar()
        {
            throw new NotImplementedException();
        }

        public bool ExisteCodigoInformado()
        {
            throw new NotImplementedException();
        }

        public void Excluir()
        {
            throw new NotImplementedException();
        }

        public bool PodeExcluir()
        {

            throw new NotImplementedException();
        }

        #endregion


        public UFEntity LerRegistro()
        {
            throw new NotImplementedException();
        }


        public IList<UFEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }
    }
}
