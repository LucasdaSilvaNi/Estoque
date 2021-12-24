using System;
namespace Sam.Domain.Entity
{
    [Serializable]
    public class UAEntity : BaseEntity
    {
        public UAEntity()
        {
        }

        public UAEntity(int? id)
        {
           base.Id = id;
        }

        UOEntity uo;
        public UOEntity Uo
        {
            get { return uo; }
            set { uo = value; }
        }

        UGEEntity uge;
        public UGEEntity Uge
        {
            get { return uge; }
            set { uge = value; }
        }

        int? codigo;
        public int? Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        OrgaoEntity orgao;
        public OrgaoEntity Orgao
        {
            get { return orgao; }
            set { orgao = value; }
        }

        UnidadeEntity unidade;
        public UnidadeEntity Unidade
        {
            get { return unidade; }
            set { unidade = value; }
        }

        int? uaVinculada;
        public int? UaVinculada
        {
            get { return uaVinculada; }
            set { uaVinculada = value; }
        }

        CentroCustoEntity centroCusto;
        public CentroCustoEntity CentroCusto
        {
            get { return centroCusto; }
            set { centroCusto = value; }
        }

        public GestorEntity Gestor { get; set; }

        bool indicadorAtividade;
        public bool IndicadorAtividade
        {
            get { return indicadorAtividade; }
            set { indicadorAtividade = value; }
        }

        long? ordem;
        public long? Ordem
        {
            get { return ordem; }
            set { ordem = value; }
        }

        /*
         Solução Paleativa p/ solucionar bug do ReportViewer 10 
         ao exibir objetos aninhados
        */
        public string DescricaoUnidade 
        {
            get 
            {
                if (Unidade != null)
                    return Unidade.Descricao;
                else
                    return string.Empty;
            } 
        }

        public string DescricaoCentroCusto
        {
            get
            {
                if (CentroCusto != null)
                    return CentroCusto.Descricao;
                else
                    return string.Empty;
            }
        }
        public bool UaDefault { get; set; }


        public override string CodigoDescricao
        {
            get { return string.IsNullOrEmpty(_codigoDescricao) ? base.concatenarCodigoDescricao(Codigo.GetValueOrDefault(), Descricao, 7) : _codigoDescricao; }
            set { _codigoDescricao = value; }
        }
    }
}
