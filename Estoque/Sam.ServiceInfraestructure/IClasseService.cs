using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    public interface IClasseService : ICatalogoBaseService, ICrudBaseService<ClasseEntity>
    {
        /// <summary>
        /// Recupera uma lista baseada na FK Grupo ID
        /// </summary>
        /// <param name="_grupoId">Identificação do Grupo</param>
        /// <returns></returns>
        IList<ClasseEntity> Listar(int _grupoId);
        IList<ClasseEntity> ListarTodosCod(int _grupoId);
        IList<ClasseEntity> ListarTodosCod(int _grupoId, bool blnRetornarTodas);
        IList<ClasseEntity> Imprimir(int _grupoId);
        ClasseEntity Select(int _classeId);
        ClasseEntity ObterClasse(int codigoClasseMaterial);
    }
}
