using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Sam.Domain.Entity;
using Sam.ServiceInfraestructure;
using Sam.Common.Util;
using System.Data.SqlClient;

namespace Sam.Domain.Business
{
    public partial class EstruturaOrganizacionalBusiness : BaseBusiness
    {
        #region FontesRecurso

        FontesRecursoEntity fontesRecurso = new FontesRecursoEntity();
        public FontesRecursoEntity FontesRecurso
        {
            get { return fontesRecurso; }
            set { fontesRecurso = value; }
        }

        public IList<FontesRecursoEntity> ListarFontesRecurso()
        {
            this.Service<IFontesRecursoService>().SkipRegistros = this.SkipRegistros;
            IList<FontesRecursoEntity> retorno = this.Service<IFontesRecursoService>().Listar();
            this.TotalRegistros = this.Service<IFontesRecursoService>().TotalRegistros();
            return retorno;
        }

        public IList<FontesRecursoEntity> ImprimirFontesRecurso()
        {
            IList<FontesRecursoEntity> retorno = this.Service<IFontesRecursoService>().Imprimir();
            return retorno;
        }

        public bool SalvarFontesRecurso()
        {
            this.Service<IFontesRecursoService>().Entity = FontesRecurso;
            this.ConsistirFontesRecurso();
            if (this.Consistido)
            {
                this.Service<IFontesRecursoService>().Salvar();
            }

            return this.Consistido;
        }

        public bool ExcluirFontesRecurso()
        {
            this.Service<IFontesRecursoService>().Entity = this.FontesRecurso;
            if (this.Consistido)
            {
                try
                {
                    this.Service<IFontesRecursoService>().Excluir();
                }
                catch (Exception ex)
                {
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        public void ConsistirFontesRecurso()
        {
            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<FontesRecursoEntity>(ref this.fontesRecurso);

            if (!this.FontesRecurso.Codigo.HasValue)
            {
                this.ListaErro.Add("É obrigatório informar o Código.");
            }

            if (string.IsNullOrEmpty(this.FontesRecurso.Descricao))
            {
                this.ListaErro.Add("É obrigatório informar a Descrição.");
            }

            if (this.ListaErro.Count == 0)
            {
                if (this.Service<IFontesRecursoService>().ExisteCodigoInformado())
                {
                    this.ListaErro.Add("Código já existente.");
                }
            }
        }

        #endregion
    }
}
