using System;



namespace Sam.Domain.Entity 
{
    [Serializable]
    public class PTResEntity : BaseEntity
    {
        public PTResEntity() { }

        public PTResEntity(int _Id)
		{ this.Id = _Id; }

        int? id;
        public virtual int? Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        int? codigo;
        public int? Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        string descricao;
        public virtual string Descricao
        {
            get { return descricao; }
            set { descricao = value; }
        }

        public string CodigoDescricao
        {
            get
            {
                if (this.codigo > 0 && !String.IsNullOrWhiteSpace(this.descricao))
                    return String.Format("{0} - {1}", this.codigo.ToString().PadLeft(6, '0'), this.descricao);
                else
                    return String.Empty;
            }
        }

        public string CodigoGestao { get; set; }
        public int? CodigoUGE { get; set; }
        public int? CodigoUO { get; set; }
        public int? AnoDotacao { get; set; }

        public string Ativo { get; set; }
        public string CodigoPT { get; set; }

		private ProgramaTrabalho _programaTrabalho;
        public ProgramaTrabalho @ProgramaTrabalho
        {
            get { return _programaTrabalho; }
            set { _programaTrabalho = value; }
        }
    }

    [Serializable]
    public class ProgramaTrabalho
    {
        private ProgramaTrabalho()
        { }

        public ProgramaTrabalho(string strCodigoPT)
        {
            long _codigoPT = -1;

            if (!String.IsNullOrWhiteSpace(strCodigoPT) && Int64.TryParse(strCodigoPT, out _codigoPT))
            {
                var strPT = _codigoPT.ToString().PadLeft(17, '0');
                this.Funcao = strPT.Substring(0, 2);
                this.SubFuncao = strPT.Substring(2, 3);
                this.Programa = strPT.Substring(5, 4);
                this.TipoProjetoAtividade = strPT.Substring(9, 1);
                this.ProjetoAtividade = strPT.Substring(9, 4);
                this.Acao = strPT.Substring(13, 4);
            }
        }

        public ProgramaTrabalho(long codigoPT)
        {
            var strPT = codigoPT.ToString().PadLeft(17, '0');
            this.Funcao = strPT.Substring(0, 2);
            this.SubFuncao = strPT.Substring(2, 3);
            this.Programa = strPT.Substring(5, 4);
            this.TipoProjetoAtividade = strPT.Substring(9, 1);
            this.ProjetoAtividade = strPT.Substring(9, 4);
            this.Acao = strPT.Substring(13, 4);
        }

        public string Funcao { private set; get; }
        public string SubFuncao { private set; get; }
        public string Programa { private set; get; }
        public string TipoProjetoAtividade { private set; get; }
        public string ProjetoAtividade { private set; get; }
        public string Acao { private set; get; }
    }
}
