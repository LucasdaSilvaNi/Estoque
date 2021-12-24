using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;
namespace Sam.ServiceInfraestructure
{
    public interface IMaterialService : ICatalogoBaseService, ICrudBaseService<MaterialEntity>
    {
        /// <summary>
        /// Recupera a Lista de Materiais à partir da PK de uma Classe
        /// </summary>
        /// <param name="_classeId"></param>
        /// <returns></returns>
        IList<MaterialEntity> Listar(int _classeId);
        IList<MaterialEntity> ListarTodosCod(int _classeId);
        IList<MaterialEntity> ListarTodosCod(int _classeId, AlmoxarifadoEntity almoxarifado);
        MaterialEntity Select(int _materialId);
        IList<MaterialEntity> Imprimir(int _classeId);
        MaterialEntity ObterMaterial(int codigoMaterial);
    }
}
