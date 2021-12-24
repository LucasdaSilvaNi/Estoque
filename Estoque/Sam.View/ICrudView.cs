using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.Common;
using Sam.Common;
using Sam.Entity;

namespace Sam.View
{
    public interface ICrudView : IBaseView
    {
        string Id
        {
            get;
            set;
        }

        string Codigo
        {
            get;
            set;
        }

        string Descricao
        {
            get;
            set;
        }

        void PopularGrid();     
           

        void ExibirMensagem(string _mensagem);

        IList ListaErros
        {
            set;
        }

        bool BloqueiaNovo
        {
             set;
        }

        bool BloqueiaGravar
        {
            set;
        }

        bool BloqueiaExcluir
        {
            set;
        }

        bool BloqueiaCancelar
        {
            set;
        }

        bool BloqueiaCodigo
        {
            set;
        }

        bool BloqueiaDescricao
        {
            set;
        }

        bool MostrarPainelEdicao
        {
            set;
        }
    }
}

