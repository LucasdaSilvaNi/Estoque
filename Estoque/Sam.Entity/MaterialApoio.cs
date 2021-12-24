using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Entity
{
    [Serializable]
    public class MaterialApoioEntity : BaseSeguranca
    {
        //public int? MaterialApoioId { get; set; }
        public short? Id { get; set; }
        public string PathArquivo { get; set; }
        public string NomeArquivo { get; set; }
        public int?   Codigo { get; set; }
        public string TipoRecurso { get; set; }
        public string Descricao { get; set; }
        public string DescricaoDetalhada { get; set; }
        public Perfil Perfil { get; set; }
    }
}
