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
    public partial class IndicadorBemProprioInfraestructure : BaseInfraestructure, IIndicadorBemProprioService
    {


        private IndicadorBemProprioEntity indicadorProprio = new IndicadorBemProprioEntity();

        public IndicadorBemProprioEntity Entity
        {
            get { return indicadorProprio; }

            set { indicadorProprio = value; }
        }

        public IList<IndicadorBemProprioEntity> Listar()
        {
            //IList<IndicadorBemProprioEntity> resultado = (from a in this.Db.TB_INDICADOR_BEM_PROPRIOs
            //                            orderby a.TB_INDICADOR_BEM_PROPRIO_CODIGO
            //                                             select new IndicadorBemProprioEntity
            //                             {
            //                                Id = a.TB_INDICADOR_BEM_PROPRIO_ID,
            //                                Codigo = a.TB_INDICADOR_BEM_PROPRIO_CODIGO,
            //                                Descricao = a.TB_INDICADOR_BEM_PROPRIO_DESCRICAO

            //                             }).ToList<IndicadorBemProprioEntity>();

          
            //return resultado;

            return new List<IndicadorBemProprioEntity>();
        }

        public IList<IndicadorBemProprioEntity> Imprimir()
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




        public IndicadorBemProprioEntity LerRegistro()
        {
            throw new NotImplementedException();
        }


        public IList<IndicadorBemProprioEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }
    }
}
