using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using System.Configuration;
using System.Collections;
using Sam.Domain.Entity;


namespace Sam.Domain.Infrastructure
{
    public partial class EmpenhoLicitacaoInfrastructure : BaseInfraestructure, IEmpenhoLicitacaoService
    {
        private EmpenhoLicitacaoEntity EmpenhoLicitacao = new EmpenhoLicitacaoEntity();

        public EmpenhoLicitacaoEntity Entity
        {
            get { return EmpenhoLicitacao; }
            set { EmpenhoLicitacao = value; }
        }

        public IList<EmpenhoLicitacaoEntity> Listar()
        {
            return (from a in Db.TB_EMPENHO_LICITACAOs
                    select new EmpenhoLicitacaoEntity
                    {
                        Id = a.TB_EMPENHO_LICITACAO_ID,
                        Descricao = a.TB_EMPENHO_LICITACAO_DESCRICAO,
                        CodigoDescricao = string.Format("{0} - {1}", a.TB_EMPENHO_LICITACAO_ID, a.TB_EMPENHO_LICITACAO_DESCRICAO)
                    }).ToList();
        }

        public IList<EmpenhoLicitacaoEntity> Listar(System.Linq.Expressions.Expression<Func<EmpenhoLicitacaoEntity, bool>> where)
        {
            return (from a in Db.TB_EMPENHO_LICITACAOs
                    select new EmpenhoLicitacaoEntity
                    {
                        Id = a.TB_EMPENHO_LICITACAO_ID,
                        Descricao = a.TB_EMPENHO_LICITACAO_DESCRICAO,
                        CodigoDescricao = string.Format("{0} - {1}", a.TB_EMPENHO_LICITACAO_ID, a.TB_EMPENHO_LICITACAO_DESCRICAO)
                    }).Where(where).ToList();
        }

        public IList<EmpenhoLicitacaoEntity> ListarTodosCod()
        {
            return (from a in Db.TB_EMPENHO_LICITACAOs
                    select new EmpenhoLicitacaoEntity
                    {
                        Id = a.TB_EMPENHO_LICITACAO_ID,
                        Descricao = string.Format("{0} - {1}", a.TB_EMPENHO_LICITACAO_ID, a.TB_EMPENHO_LICITACAO_DESCRICAO)
                    }).ToList();
        }

        public EmpenhoLicitacaoEntity LerRegistro()
        {
            return Listar().Where(a => a.Id == Entity.Id).FirstOrDefault();
        }

        public EmpenhoLicitacaoEntity LerRegistro(MovimentoEntity pObjMovimento)
        {
            return (from EmpenhoLicitacao in Db.TB_EMPENHO_LICITACAOs
                    where (EmpenhoLicitacao.TB_EMPENHO_LICITACAO_ID == pObjMovimento.EmpenhoLicitacao.Id)
                    select new EmpenhoLicitacaoEntity
                    {
                        Id = EmpenhoLicitacao.TB_EMPENHO_LICITACAO_ID,
                        Descricao = EmpenhoLicitacao.TB_EMPENHO_LICITACAO_DESCRICAO,
                        CodigoDescricao = string.Format("{0} - {1}", EmpenhoLicitacao.TB_EMPENHO_LICITACAO_ID, EmpenhoLicitacao.TB_EMPENHO_LICITACAO_DESCRICAO),
                        CodigoFormatado = EmpenhoLicitacao.TB_EMPENHO_LICITACAO_DESCRICAO
                    }).FirstOrDefault();
        }

        public EmpenhoLicitacaoEntity LerTipoLicitacaoDoMovimento(int pIntMovimentoID)
        {
            return (from EmpenhoLicitacao in Db.TB_EMPENHO_LICITACAOs
                    join Movimentos in Db.TB_MOVIMENTOs on EmpenhoLicitacao.TB_EMPENHO_LICITACAO_ID equals Movimentos.TB_EMPENHO_LICITACAO_ID
                    where (Movimentos.TB_MOVIMENTO_ID == pIntMovimentoID)
                    //where (EmpenhoLicitacao.TB_EMPENHO_LICITACAO_ID == pIntMovimentoID)
                    select new EmpenhoLicitacaoEntity
                    {
                        Id = EmpenhoLicitacao.TB_EMPENHO_LICITACAO_ID,
                        Descricao = EmpenhoLicitacao.TB_EMPENHO_LICITACAO_DESCRICAO,
                        CodigoDescricao = string.Format("{0} - {1}", EmpenhoLicitacao.TB_EMPENHO_LICITACAO_ID, EmpenhoLicitacao.TB_EMPENHO_LICITACAO_DESCRICAO),
                        CodigoFormatado = EmpenhoLicitacao.TB_EMPENHO_LICITACAO_ID.ToString().PadLeft(5, '0')
                    }).FirstOrDefault();
        }

        public EmpenhoLicitacaoEntity ObterTipoLicitacao(int pIntLicitacaoID)
        {
            return (from EmpenhoLicitacao in Db.TB_EMPENHO_LICITACAOs
                    where (EmpenhoLicitacao.TB_EMPENHO_LICITACAO_ID == pIntLicitacaoID)
                    select new EmpenhoLicitacaoEntity
                    {
                        Id = EmpenhoLicitacao.TB_EMPENHO_LICITACAO_ID,
                        Descricao = EmpenhoLicitacao.TB_EMPENHO_LICITACAO_DESCRICAO,
                        CodigoDescricao = string.Format("{0} - {1}", EmpenhoLicitacao.TB_EMPENHO_LICITACAO_ID, EmpenhoLicitacao.TB_EMPENHO_LICITACAO_DESCRICAO),
                        CodigoFormatado = EmpenhoLicitacao.TB_EMPENHO_LICITACAO_ID.ToString().PadLeft(5, '0')
                    }).FirstOrDefault();
        }

        public EmpenhoLicitacaoEntity Listar(int _id)
        {
            return Listar().Where(a => a.Id == _id).FirstOrDefault();
        }

        public IList<EmpenhoLicitacaoEntity> Imprimir()
        {
            throw new NotImplementedException();
        }

        public void Excluir()
        {
            throw new NotImplementedException();
        }

        public void Salvar()
        {
            throw new NotImplementedException();
        }

        public bool PodeExcluir()
        {
            throw new NotImplementedException();
        }

        public bool ExisteCodigoInformado()
        {
            throw new NotImplementedException();
        }
    }
}
