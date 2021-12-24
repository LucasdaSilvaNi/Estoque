using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    public class RelatorioEntity
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string DataSet { get; set; }

        private SortedList parametros;
        public SortedList Parametros
        {
            get
            {
                if (parametros == null)
                    parametros = new SortedList(StringComparer.InvariantCultureIgnoreCase);

                return parametros;
            }
            set { parametros = value; }
        }
    }

    //public static class RelatorioEntity
    //{
    //    public static int Id { get; set; }
    //    public static string Nome { get; set; }
    //    public static string DataSet { get; set; }

    //    private static SortedList parametros;
    //    public static SortedList Parametros
    //    {
    //        get
    //        {
    //            if (parametros == null)
    //                //parametros = new SortedList();
    //                parametros = new SortedList(StringComparer.InvariantCultureIgnoreCase);

    //            return parametros;
    //        }
    //        set { parametros = value; }
    //    }
    //}
}
