using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sam.Infrastructure
{
    public class SistemaInfrastructureAntigo : BaseInfrastructure, ICrudBase<Sam.Entity.Sistema>
    {

        private Sam.Entity.Sistema sistema = new Sam.Entity.Sistema();

        public Sam.Entity.Sistema Entity
        {
            get { return sistema; }
            set { sistema = value; }
        }

        public IList<Sam.Entity.Sistema> Listar()
        {
            return (from a in Db.TB_SISTEMA
                    orderby a.TB_SISTEMA_DESCRICAO
                    select new Sam.Entity.Sistema
                    {
                        SistemaId = a.TB_SISTEMA_ID,
                        Descricao = a.TB_SISTEMA_DESCRICAO,
                        Sigla = a.TB_SISTEMA_SIGLA,
                        Ativo = a.TB_SISTEMA_ATIVO
                    }).ToList();
        }

        public IList<Sam.Entity.Sistema> ListAllCode()
        {
            throw new NotImplementedException();
        }

        public Sam.Entity.Sistema LerRegistro()
        {
            throw new NotImplementedException();
        }

        public IList<Sam.Entity.Sistema> Imprimir()
        {
            throw new NotImplementedException();
        }

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
    }
}
