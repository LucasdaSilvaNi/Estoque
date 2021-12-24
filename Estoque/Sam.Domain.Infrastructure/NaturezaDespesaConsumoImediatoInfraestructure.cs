using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;
using Sam.Common.Util;
using System.Linq.Expressions;

namespace Sam.Domain.Infrastructure
{
    public class NaturezaDespesaConsumoImediatoInfraestructure : BaseInfraestructure, INaturezaDespesaConsumoImediatoService
    {
        public int totalregistros
        {
            get;
            set;
        }

        public int TotalRegistros()
        {
            return totalregistros;
        }


        public NaturezaDespesaConsumoImediatoEntity Entity { get; set; }

        public IList<NaturezaDespesaConsumoImediatoEntity> Listar()
        {
            IList<NaturezaDespesaConsumoImediatoEntity> listagemRegistros = this.Db.TB_NATUREZA_DESPESA_CONSUMO_IMEDIATOs.Select(_instanciadorDTONaturezaDespesaConsumoImediato())
                                                                                                                         .Take(this.RegistrosPagina)
                                                                                                                         .OrderBy(registroTabela => registroTabela.Codigo).
                                                                                                                         ToList();

            this.totalregistros = this.Db.TB_NATUREZA_DESPESA_CONSUMO_IMEDIATOs.Count();
            return listagemRegistros;
        }

        public bool ExcluirNaturezaDespesaConsumoImediato()
        {
            this.Excluir();
            return (this.Db.TB_NATUREZA_DESPESA_CONSUMO_IMEDIATOs.Count(a => a.TB_NATUREZA_DESPESA_CODIGO == this.Entity.Codigo) == 0);
        }
        public void Excluir()
        {
            TB_NATUREZA_DESPESA_CONSUMO_IMEDIATO naturezaDespesaConsumoImediato = this.Db.TB_NATUREZA_DESPESA_CONSUMO_IMEDIATOs.Where(a => a.TB_NATUREZA_DESPESA_CODIGO == this.Entity.Codigo).FirstOrDefault();
            this.Db.TB_NATUREZA_DESPESA_CONSUMO_IMEDIATOs.DeleteOnSubmit(naturezaDespesaConsumoImediato);
            this.Db.SubmitChanges();
        }

        public bool ExisteItemMaterialRelacionado()
        {
            bool blnStatusExclusao = false;

            try
            {
                TB_NATUREZA_DESPESA_CONSUMO_IMEDIATO naturezaDespesa = this.Db.TB_NATUREZA_DESPESA_CONSUMO_IMEDIATOs.Where(a => a.TB_NATUREZA_DESPESA_CODIGO == this.Entity.Codigo).FirstOrDefault();
                IList<TB_ITEM_NATUREZA_DESPESA> lstItemMaterialRelacionado = this.Db.TB_ITEM_NATUREZA_DESPESAs.Where(_itemMaterial => _itemMaterial.TB_NATUREZA_DESPESA_ID == this.Entity.Id).ToList();

                blnStatusExclusao = (naturezaDespesa.IsNotNull() && lstItemMaterialRelacionado.IsNullOrEmpty());
            }
            catch (Exception excErroExecucao)
            {
                throw excErroExecucao;
            }


            return blnStatusExclusao;
        }
        public void Salvar()
        {
            TB_NATUREZA_DESPESA_CONSUMO_IMEDIATO naturezaDespesaConsumoImediato = new TB_NATUREZA_DESPESA_CONSUMO_IMEDIATO();

            //if (this.Entity.Codigo > 0)

            naturezaDespesaConsumoImediato = this.Db.TB_NATUREZA_DESPESA_CONSUMO_IMEDIATOs.Where(a => a.TB_NATUREZA_DESPESA_CODIGO == this.Entity.Codigo).FirstOrDefault();

            if (naturezaDespesaConsumoImediato.IsNull())
            {
                naturezaDespesaConsumoImediato = new TB_NATUREZA_DESPESA_CONSUMO_IMEDIATO();
                this.Db.TB_NATUREZA_DESPESA_CONSUMO_IMEDIATOs.InsertOnSubmit(naturezaDespesaConsumoImediato);
            }


            naturezaDespesaConsumoImediato.TB_NATUREZA_DESPESA_CODIGO = this.Entity.Codigo;
            naturezaDespesaConsumoImediato.TB_NATUREZA_DESPESA_DESCRICAO = this.Entity.Descricao;
            naturezaDespesaConsumoImediato.TB_NATUREZA_DESPESA_ATIVA = this.Entity.Ativa;


            this.Db.SubmitChanges();
        }

        public bool PodeExcluir()
        {

            return ExisteItemMaterialRelacionado();
        }
        public bool ExisteCodigoInformado()
        {
            bool codigoExistente = false;
            if (this.Entity.Codigo > 0)
            {
                codigoExistente = this.Db.TB_NATUREZA_DESPESA_CONSUMO_IMEDIATOs
                                      .Where(naturezaDespesaConsumoImediato => (naturezaDespesaConsumoImediato.TB_NATUREZA_DESPESA_CODIGO == this.Entity.Codigo)
                                                                            && (naturezaDespesaConsumoImediato.TB_NATUREZA_DESPESA_DESCRICAO != this.Entity.Descricao)
                                                                            && (naturezaDespesaConsumoImediato.TB_NATUREZA_DESPESA_ATIVA == Entity.Ativa))
                                                                            //&& (naturezaDespesaConsumoImediato.TB_NATUREZA_DESPESA_ATIVA == false))
                                      .Count() > 0;
            }

            return codigoExistente;
        }


        public NaturezaDespesaConsumoImediatoEntity LerRegistro()
        {
            throw new NotImplementedException();
        }
        public IList<NaturezaDespesaConsumoImediatoEntity> Imprimir()
        {
            throw new NotImplementedException();
        }
        public NaturezaDespesaConsumoImediatoEntity ObterNaturezaDespesaConsumoImediato(int codigoNaturezaDespesa)
        {
            NaturezaDespesaConsumoImediatoEntity registroTabela = null;
            IQueryable<TB_NATUREZA_DESPESA_CONSUMO_IMEDIATO> qryConsulta = null;
            Expression<Func<TB_NATUREZA_DESPESA_CONSUMO_IMEDIATO, bool>> expWhere;


            qryConsulta = this.Db.TB_NATUREZA_DESPESA_CONSUMO_IMEDIATOs.AsQueryable();
            expWhere = (naturezaDespesaConsumoImediato => naturezaDespesaConsumoImediato.TB_NATUREZA_DESPESA_CODIGO == codigoNaturezaDespesa);

            registroTabela = qryConsulta.Where(expWhere)
                                        .Select(_instanciadorDTONaturezaDespesaConsumoImediato())
                                        .FirstOrDefault();


            return registroTabela;
        }


        public IList<NaturezaDespesaConsumoImediatoEntity> ListarTodosCod()
        {
            IQueryable<TB_NATUREZA_DESPESA_CONSUMO_IMEDIATO> qryConsulta = null;
            IList<NaturezaDespesaConsumoImediatoEntity> listaEntidades = null;


            qryConsulta = this.Db.TB_NATUREZA_DESPESA_CONSUMO_IMEDIATOs.AsQueryable();
            listaEntidades = qryConsulta.Select(_instanciadorDTONaturezaDespesaConsumoImediato())
                                        .ToList();

            this.totalregistros = listaEntidades.Count();
            return listaEntidades;
        }
        public IList<String> ListarNaturezasDespesaConsumoImediato()
        {
            IList<String> listagemCodigosNaturezasDespesaConsumoImediato = null;
            IList<NaturezaDespesaConsumoImediatoEntity> listagemRegistros = null;

            listagemRegistros = ListarTodosCod();

            if (listagemRegistros.HasElements())
                listagemCodigosNaturezasDespesaConsumoImediato = listagemRegistros.Where(registroTabela => registroTabela.Ativa == true)
                                                                                  .Select(registroTabela => registroTabela.Codigo.ToString().PadLeft(8, '0'))
                                                                                  .ToList();

            this.totalregistros = listagemRegistros.Count();
            return listagemCodigosNaturezasDespesaConsumoImediato;
        }

        


        private Func<TB_NATUREZA_DESPESA_CONSUMO_IMEDIATO, NaturezaDespesaConsumoImediatoEntity> _instanciadorDTONaturezaDespesaConsumoImediato()
        {
            Func<TB_NATUREZA_DESPESA_CONSUMO_IMEDIATO, NaturezaDespesaConsumoImediatoEntity> _actionSeletor = null;

            _actionSeletor = (rowTabela => new NaturezaDespesaConsumoImediatoEntity()
                                                                                    {
                                                                                        Codigo = rowTabela.TB_NATUREZA_DESPESA_CODIGO,
                                                                                        Descricao = rowTabela.TB_NATUREZA_DESPESA_DESCRICAO,
                                                                                        Ativa = rowTabela.TB_NATUREZA_DESPESA_ATIVA
                                                                                    });

            return _actionSeletor;
        }
    }
}
