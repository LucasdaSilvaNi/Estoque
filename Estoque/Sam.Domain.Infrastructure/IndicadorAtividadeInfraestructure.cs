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
    public partial class IndicadorAtividadeInfraestructure : BaseInfraestructure, IIndicadorAtividadeService
    {


        private IndicadorAtividadeEntity indicadorAtividade = new IndicadorAtividadeEntity();

        public IndicadorAtividadeEntity Entity
        {
            get { return indicadorAtividade; }

            set { indicadorAtividade = value; }
        }

        public IList<IndicadorAtividadeEntity> Listar()
        {
            //IList<IndicadorAtividadeEntity> resultado = (from a in this.Db.TB_INDICADOR_ATIVIDADEs
            //                                             orderby a.TB_INDICADOR_ATIVIDADE_CODIGO
            //                                             select new IndicadorAtividadeEntity
            //                             {
            //                                 Id = a.TB_INDICADOR_ATIVIDADE_ID,
            //                                 Codigo = a.TB_INDICADOR_ATIVIDADE_CODIGO,
            //                                 Descricao = a.TB_INDICADOR_ATIVIDADE_DESCRICAO

            //                             }).ToList<IndicadorAtividadeEntity>();


            return new List<IndicadorAtividadeEntity>();
            ////return resultado;
        }


        public IList<IndicadorAtividadeEntity> Imprimir()
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




        public IndicadorAtividadeEntity LerRegistro()
        {
            throw new NotImplementedException();
        }


        public IList<IndicadorAtividadeEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }
    }
}
