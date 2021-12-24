using System;
using System.Collections.Generic;
using System.Linq;

namespace Sam.Domain.Entity
{
    public class CargaEntity : BaseEntity
    {
        public CargaEntity()
        {
        }

        public CargaEntity(string arquivo)
        {
            NomeArquivo = arquivo;
        }

        public string NomeArquivo { get; set; }
        public string CaminhoDiretorio { get; set; }
        public string CaminhoDiretorioDestino { get; set; }
        public double tamanhoMax { get; set; }
        public List<Extensao> ExtensaoList { get; set; }
        public int TipoArquivo { get; set; }
    }

    public class Extensao
    {
        public string ExtensaoArquivo { get; set; }

        public Extensao(string ext)
        {
            ExtensaoArquivo = ext;
        }
    }

}
