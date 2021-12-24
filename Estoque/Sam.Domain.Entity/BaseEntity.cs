using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Sam.Domain.Entity
{
    [Serializable]
    public abstract class BaseEntity
    {
        private int? id;      
        private int? idTemp;

        protected string _codigoDescricao = null;

        public int? Id
        {
            get { return id; }
            set { id = value; }
        }
     
        public int? IdTemp
        {
            get { return idTemp; }
            set { idTemp = value; }
        }

        public virtual string CodigoDescricao { get; set; }

        public string CodigoFormatado { get; set; }
        public virtual string Descricao { get; set; }
        public virtual bool? Ativo { get; set; }
        public virtual List<string> Validate()
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(this, null, null);
            bool isValid = Validator.TryValidateObject(this, validationContext, validationResults, true);

            List<string> retorno = (from a in validationResults
                                    select a.ErrorMessage).ToList<string>();

            return retorno;
        }

        public virtual string concatenarCodigoDescricao(int codigo, string descricao, int tamanho)
        {
            string _formatacao = string.Concat("{0:D", tamanho, "} - {1}");
            return descricao == null || descricao.ToLower().Trim().Equals("todos") ?  string.IsNullOrEmpty(descricao) ? "" : descricao : string.Format(_formatacao, codigo, descricao);
        }
    }

    [Serializable]
    public class BaseEntityIEqualityComparer : IEqualityComparer<BaseEntity>
    {
        public bool Equals(BaseEntity x, BaseEntity y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(BaseEntity obj)
        {
            return (int)obj.Id;
        }
    }

}
