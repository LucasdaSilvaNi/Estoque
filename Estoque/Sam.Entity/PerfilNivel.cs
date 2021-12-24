using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Entity
{
    [Serializable]
    public class PerfilNivel 
    {

        public PerfilNivel()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 PerfilId { get; set; }
        public string PerfilDescricao { get; set; }
        public Int32 NivelId { get; set; }
        public string NivelDescricao { get; set; }

    }
}
