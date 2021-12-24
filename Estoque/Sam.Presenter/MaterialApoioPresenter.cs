using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.View;
using Sam.Entity;
using Sam.Common.Util;
using System.ComponentModel;
using Sam.Business;
using Sam.Infrastructure;
using System.Linq.Expressions;

namespace Sam.Presenter
{
    public class MaterialApoioPresenter : CrudPresenter<IMaterialApoioView>
    {
        IMaterialApoioView view;
        public IMaterialApoioView View
        {
            get { return view; }
            set { view = value; }
        }

        MaterialApoioBusiness objBusiness = new MaterialApoioBusiness();

        public MaterialApoioPresenter()
        {
        }
        public MaterialApoioPresenter(IMaterialApoioView _view) : base(_view)
        {
            this.View = _view;
        }

        public List<MaterialApoioEntity> ListarRecursosPorPerfil(int Perfil_ID)
        {
           return objBusiness.ListarRecursosPorPerfil(Perfil_ID).ToList();
        }

        public MaterialApoioEntity ObterDadosMaterialApoio(int MaterialApoio_ID)
        {
            return objBusiness.ObterDadosMaterialApoio(MaterialApoio_ID);
        }
    }
}
