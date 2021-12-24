using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Sam.View;
using Sam.Entity;
using Sam.Domain.Entity;
using Sam;
using System.Web.Security;
using Sam.Domain.Business;
using Sam.Common.Util;
using System.Xml.Linq;

namespace Sam.Presenter
{
    public class CrudPresenter<T> : BasePresenter where T : ICrudView
    {

        public CrudPresenter()
        { }

        private T view;

        private string usuario;
        public string Usuario
        {
            get
            {
                try
                {
                    return ((System.Web.Security.FormsIdentity)(HttpContext.Current.User.Identity)).Ticket.UserData;
                }
                catch (InvalidCastException)
                {
                    Sam.Domain.Business.LogErro.GravarStackTrace();

                    throw new InvalidCastException("Erro ao obter token de usuário");
                }
                catch (Exception e)
                {
                    new Presenter.LogErroPresenter().GravarLogErro(e);
                    throw new Exception("Erro interno", e);
                }
            }
            set { usuario = value; }
        }

        private Acesso acesso;
        public Acesso Acesso
        {
            get { return (Acesso)HttpContext.Current.Cache[Usuario]; }
            set { acesso = value; }
        }

        private int perfil;
        public int Perfil
        {
            get { return Acesso.Transacoes.Perfis[0].IdPerfil; }
            set { perfil = value; }
        }

        public IList<AlmoxarifadoEntity> FiltrarNivelAcesso(IList<AlmoxarifadoEntity> lista)
        {

            IList<AlmoxarifadoEntity> filtro = this.Acesso.Estruturas.Almoxarifado;
            if ((this.Perfil != (short)Sam.Common.Perfil.ADMINISTRADOR_GERAL && this.Perfil != (short)Sam.Common.Perfil.ADMINISTRADOR_ORGAO
                 && this.Perfil != (short)Sam.Common.EnumPerfil.ADMINISTRADOR_FINANCEIRO_SEFAZ) && filtro != null)
            {
                lista = this.FiltrarNivelAcesso(lista.Cast<BaseEntity>().ToList(), filtro.Cast<BaseEntity>().ToList()).Cast<AlmoxarifadoEntity>().ToList();
            }

            return lista;
            ;
        }

        public IList<OrgaoEntity> FiltrarNivelAcesso(IList<OrgaoEntity> lista)
        {
            IList<OrgaoEntity> filtro = this.Acesso.Estruturas.Orgao;

            if (this.Perfil != (short)Sam.Common.Perfil.ADMINISTRADOR_GERAL
                && this.Perfil != (short)Sam.Common.EnumPerfil.ADMINISTRADOR_FINANCEIRO_SEFAZ)
            {
                lista = this.FiltrarNivelAcesso(lista.Cast<BaseEntity>().ToList(), filtro.Cast<BaseEntity>().ToList()).Cast<OrgaoEntity>().ToList();
            }

            return lista;
        }

        public IList<GestorEntity> FiltrarNivelAcesso(IList<GestorEntity> lista)
        {
            IList<GestorEntity> filtro = this.Acesso.Estruturas.Gestor;

            //Se for administrador gestor
            //Fitrar gestor
            if (this.Perfil == (short)Sam.Common.Perfil.ADMINISTRADOR_GESTOR
                && this.Perfil != (short)Sam.Common.EnumPerfil.ADMINISTRADOR_FINANCEIRO_SEFAZ)
            {
                lista = this.FiltrarNivelAcesso(lista.Cast<BaseEntity>().ToList(), filtro.Cast<BaseEntity>().ToList()).Cast<GestorEntity>().ToList();
            }

            return lista;
        }

        private IList<BaseEntity> FiltrarNivelAcesso(IList<BaseEntity> lista, IList<BaseEntity> filtro)
        {
            if (filtro != null)
            {
                lista = lista.Intersect(filtro, new BaseEntityIEqualityComparer()).ToList<BaseEntity>();
            }
            return lista;
        }

        public T View
        {
            get { return (T)view; }
            set { view = value; }
        }

        public CrudPresenter(T _view)
        {
            this.view = _view;
        }

        public int TotalRegistrosGrid
        {
            get;
            set;
        }

        public virtual void Load()
        {
            this.View.BloqueiaGravar = false;
            this.View.BloqueiaExcluir = false;
            this.View.BloqueiaCancelar = true;
            this.View.BloqueiaCodigo = true;
            this.View.BloqueiaDescricao = false;
            this.View.BloqueiaNovo = true;
            this.View.MostrarPainelEdicao = false;
        }

        public virtual void RegistroSelecionado()
        {
            this.View.BloqueiaGravar = true;
            this.View.BloqueiaExcluir = true;
            this.View.BloqueiaCancelar = true;
            this.View.BloqueiaCodigo = true;
            this.View.BloqueiaDescricao = true;
            this.View.BloqueiaNovo = false;
            this.View.MostrarPainelEdicao = true;
        }

        public virtual void GravadoSucesso()
        {
            this.View.BloqueiaGravar = false;
            this.View.BloqueiaExcluir = false;
            this.View.BloqueiaCancelar = false;
            this.View.BloqueiaCodigo = false;
            this.View.BloqueiaDescricao = false;
            this.View.BloqueiaNovo = true;
            this.View.Codigo = string.Empty;
            this.View.Descricao = string.Empty;
            this.View.Id = null;
            this.View.ListaErros = null;
            this.View.MostrarPainelEdicao = false;
            this.View.PopularGrid();
        }

        public virtual void ExcluidoSucesso()
        {
            this.View.BloqueiaGravar = false;
            this.View.BloqueiaExcluir = false;
            this.View.BloqueiaCancelar = false;
            this.View.BloqueiaCodigo = false;
            this.View.BloqueiaDescricao = false;
            this.View.BloqueiaNovo = false;
            this.View.Codigo = string.Empty;
            this.View.Descricao = string.Empty;
            this.View.Id = null;
            this.View.ListaErros = null;
            this.View.MostrarPainelEdicao = false;
            this.View.PopularGrid();
        }

        public virtual void Novo()
        {
            this.View.BloqueiaGravar = true;
            this.View.BloqueiaCancelar = true;
            this.View.BloqueiaCodigo = true;
            this.View.BloqueiaDescricao = true;
            this.View.BloqueiaExcluir = false;
            this.View.Codigo = string.Empty;
            this.View.Descricao = string.Empty;
            this.View.Id = null;
            this.View.MostrarPainelEdicao = true;
            this.View.BloqueiaNovo = false;
        }

        public virtual void Cancelar()
        {
            this.View.BloqueiaGravar = false;
            this.View.BloqueiaExcluir = false;
            this.View.BloqueiaCancelar = false;
            this.View.BloqueiaCodigo = false;
            this.View.BloqueiaDescricao = false;
            this.View.BloqueiaNovo = true;
            this.View.Id = null;
            this.View.Codigo = string.Empty;
            this.View.Descricao = string.Empty;

            this.View.MostrarPainelEdicao = false;
        }

        public virtual void InserirMensagemEmSessao()
        { }

        public virtual bool VerificaStatusFechadoMesReferenciaSIAFEM(string anoMesReferencia, bool exibirMensagemErro = false)
        {
            bool blnRetorno = true;
            bool mesFiscalFechadoSIAFEM = true;
            DateTime dtFechamentoSIAFEM = new DateTime();
            CalendarioFechamentoMensalEntity objDataFechamentoMensal = null;
            CalendarioFechamentoMensalBusiness objBusinessCalendarioSIAFEM = null;

            try
            {
                if (this.View.IsNotNull() && !String.IsNullOrWhiteSpace(anoMesReferencia))
                {
                    var anoReferencia = Int32.Parse(anoMesReferencia.Substring(0, 4));
                    var mesReferencia = Int32.Parse(anoMesReferencia.Substring(4, 2));
                    objBusinessCalendarioSIAFEM = new CalendarioFechamentoMensalBusiness();
                    mesFiscalFechadoSIAFEM = objBusinessCalendarioSIAFEM.StatusFechadoMesReferenciaSIAFEM(mesReferencia, anoReferencia, exibirMensagemErro);
                    objDataFechamentoMensal = objBusinessCalendarioSIAFEM.ObterDataFechamentoMensalSIAFEM(mesReferencia, anoReferencia);
                    dtFechamentoSIAFEM = objDataFechamentoMensal.DataFechamentoDespesa;

                    if (!mesFiscalFechadoSIAFEM)
                    {
                        dtFechamentoSIAFEM = dtFechamentoSIAFEM.AddHours(19);
                        //dtFechamentoSIAFEM = Convert.ToDateTime("19/08/2019 19:00:00");
                        blnRetorno = (DateTime.Now >= dtFechamentoSIAFEM);
                    }
                    else if ((!exibirMensagemErro && mesFiscalFechadoSIAFEM) || (objBusinessCalendarioSIAFEM.ListaErro.IsNotNullAndNotEmpty()))
                    {
                        var msgErro = String.Format("Mês/ano referência ({0:D2}/{1:D4})* fechado em {2} 19:00.", mesReferencia, anoReferencia, dtFechamentoSIAFEM.ToString("dd/MM/yyyy"));
                        this.View.ListaErros = new List<string>() { msgErro };
                    }
                }
            }
            catch (Exception excErro)
            {
                if (objBusinessCalendarioSIAFEM.ListaErro.IsNotNullAndNotEmpty())
                    this.View.ListaErros = new List<string>(objBusinessCalendarioSIAFEM.ListaErro);

                new LogErro().GravarLogErro(excErro);
                return true;
            }


            return blnRetorno;
        }

        public CalendarioFechamentoMensalEntity PegarPendenciaFechamento(int mesReferencia, int anoReferencia)
        {
            CalendarioFechamentoMensalEntity objDataFechamentoMensal = null;
            CalendarioFechamentoMensalBusiness objBusinessCalendarioSIAFEM = new CalendarioFechamentoMensalBusiness();
            try
            {
                objDataFechamentoMensal = objBusinessCalendarioSIAFEM.PegarPendenciaFechamento(mesReferencia, anoReferencia);
            }
            catch (Exception excErro)
            {
                throw excErro;
            }

            return objDataFechamentoMensal;
        }

    }
}
