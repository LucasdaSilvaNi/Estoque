using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Sam.View
{
    public interface IEstruturaOrganizacionalView : IBaseView
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

        void PopularGrid();

        void ExibirMensagem(string _mensagem);

    }
}
