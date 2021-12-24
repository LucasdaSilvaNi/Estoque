using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;
using System.Data.SqlClient;
using System.Transactions;

namespace Sam.Domain.Business
{
    public partial class CatalogoBusiness : BaseBusiness
    {
        #region Classe

        private ClasseEntity classe = new ClasseEntity();

        public ClasseEntity Classe
        {
            get { return classe; }
            set { classe = value; }
        }

        public bool SalvarClasse()
        {
            this.Service<IClasseService>().Entity = this.Classe;
            this.ConsistirClasse();
            if (this.Consistido)
            {
                this.Service<IClasseService>().Salvar();
            }
            return this.Consistido;
        }

        public IList<ClasseEntity> ListarClasse(int _grupoId)
        {
            this.Service<IClasseService>().SkipRegistros = this.SkipRegistros;

            IList<ClasseEntity> retorno = this.Service<IClasseService>().Listar(_grupoId);
            this.TotalRegistros = this.Service<IClasseService>().TotalRegistros();
            return retorno;
        }

        public IList<ClasseEntity> ListarClasseTodosCod(int _grupoId)
        {
            //bool blnRetornarTodas = (_grupoId == 99); //Será ativado após reunião a ser definida com cliente-piloto.
            //IList<ClasseEntity> retorno = this.Service<IClasseService>().ListarTodosCod(_grupoId, blnRetornarTodas);
            IList<ClasseEntity> retorno = this.Service<IClasseService>().ListarTodosCod(_grupoId);
            return retorno;
        }

        public void SelectClasse(int _classeId)
        {
            this.Classe = this.Service<IClasseService>().Select(_classeId);

        }

        public IList<ClasseEntity> ListarClasse()
        {
            IList<ClasseEntity> retorno = this.Service<IClasseService>().Listar();
            this.TotalRegistros = this.Service<IClasseService>().TotalRegistros();
            return retorno;
        }

        public IList<ClasseEntity> ImprimirClasse(int _grupoId)
        {
            IList<ClasseEntity> retorno = this.Service<IClasseService>().Imprimir(_grupoId);
            return retorno;
        }

        public bool ExcluirClasse()
        {
            this.Service<IClasseService>().Entity = this.Classe;
            if (this.Consistido)
            {
                try
                {
                    this.Service<IClasseService>().Excluir();
                }
                catch (Exception ex)
                {
                    new LogErro().GravarLogErro(ex);
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        public void ConsistirClasse()
        {

            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<ClasseEntity>(ref this.classe);

            if (this.Classe.Codigo < 1)
                this.ListaErro.Add("É obrigatório informar o Código.");

            if (this.Classe.Codigo.ToString().Substring((Classe.Codigo.ToString().Length - 2), 2) == "00")
                this.ListaErro.Add("É obrigatório informar o Código.");

            if (string.IsNullOrEmpty(this.Classe.Descricao))
                this.ListaErro.Add("É obrigatório informar a Descrição.");


            if (this.ListaErro.Count == 0)
            {
                if (this.Service<IClasseService>().ExisteCodigoInformado())
                    this.ListaErro.Add("Código já existente.");
            }
        }

        public ClasseEntity ObterClasse(int codigoClasseMaterial)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    return this.Service<IClasseService>().ObterClasse(codigoClasseMaterial);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    tras.Complete();
                }
            }
        }

        #endregion
    }
}
