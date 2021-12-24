using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;



namespace Sam.Domain.Infrastructure
{
    public class IndicadorDisponivelInfraestructure : BaseInfraestructure, IIndicadorDisponivelService
    {
        public void Excluir()
        {
            throw new NotImplementedException();
        }

        public void Salvar()
        {
            throw new NotImplementedException();
        }

        public bool PodeExcluir()
        {
            throw new NotImplementedException();
        }

        public bool ExisteCodigoInformado()
        {
            throw new NotImplementedException();
        }

        IList<IndicadorDisponivelEntity> Listar()
        {
            IList<IndicadorDisponivelEntity> resultado = (from a in Db.TB_INDICADOR_DISPONIVELs
                                                          orderby a.TB_INDICADOR_DISPONIVEL_ID
                                                          select new IndicadorDisponivelEntity
                                                          {
                                                              Id = a.TB_INDICADOR_DISPONIVEL_ID,
                                                              Descricao = a.TB_INDICADOR_DISPONIVEL_DESCRICAO
                                                          }).ToList();
            return resultado;
        }



        public IndicadorDisponivelEntity Entity { get; set; }

        IList<IndicadorDisponivelEntity> ICrudBaseService<IndicadorDisponivelEntity>.Listar()
        {
            throw new NotImplementedException();
        }

        public IList<IndicadorDisponivelEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }

        public IndicadorDisponivelEntity LerRegistro()
        {
            throw new NotImplementedException();
        }

        public IList<IndicadorDisponivelEntity> Imprimir()
        {
            throw new NotImplementedException();
        }
    }
}
