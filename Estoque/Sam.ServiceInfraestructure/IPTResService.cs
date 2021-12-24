using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    //public interface IPTResService : ICatalogoBaseService,ICrudBaseService<PTResEntity>
    public interface IPTResService : ICrudBaseService<PTResEntity>
    {
        PTResEntity Listar(int pIntCodigoPtRes);
        IList<PTResEntity> Listar(int ugeCodigo, bool retornaListagem = true);
        PTResEntity ObterPTRes(int ptresID);
        PTResEntity ObterPTRes(int ptresCodigo, int ugeCodigo);
        IList<PTResEntity> ObterPTRes(int ptresCodigo, int ugeCodigo, int? ptresAcao = null);
        IList<PTResEntity> ObterPTResAcao(int ugeCodigo);

        //bool Salvar(ref PTResEntity pObjPtRes);
        //bool Salvar(PTResEntity pObjPtRes);
    }
}
