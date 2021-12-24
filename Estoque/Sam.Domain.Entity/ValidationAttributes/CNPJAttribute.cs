using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Sam.Domain.Entity.ValidationAttributes
{
    public class CNPJAttribute : ValidationAttribute
    {
        public CNPJAttribute()
        {
        }

        public override bool IsValid(object value)
        {
            return ValidarCNPJ(value as string);
        }


        /// <summary>
        /// Valida o CNPJ
        /// </summary>
        /// <param name="strCnpj">CNPJ a ser validado</param>
        /// <returns>True CNPJ válido ou False CNPJ inválido</returns>
        private bool ValidarCNPJ(string strCnpj)
        {
            try
            {
                int soma = 0, dig;

                if (strCnpj.Length != 14)
                    return false;

                string cnpj_calc = strCnpj.Substring(0, 12);

                char[] chr_cnpj = strCnpj.ToCharArray();

                /* Primeira parte */
                for (int i = 0; i < 4; i++)
                    if (chr_cnpj[i] - 48 >= 0 && chr_cnpj[i] - 48 <= 9)
                        soma += (chr_cnpj[i] - 48) * (6 - (i + 1));
                for (int i = 0; i < 8; i++)
                    if (chr_cnpj[i + 4] - 48 >= 0 && chr_cnpj[i + 4] - 48 <= 9)
                        soma += (chr_cnpj[i + 4] - 48) * (10 - (i + 1));
                dig = 11 - (soma % 11);

                cnpj_calc += (dig == 10 || dig == 11) ?
                    "0" : dig.ToString();

                /* Segunda parte */
                soma = 0;
                for (int i = 0; i < 5; i++)
                    if (chr_cnpj[i] - 48 >= 0 && chr_cnpj[i] - 48 <= 9)
                        soma += (chr_cnpj[i] - 48) * (7 - (i + 1));
                for (int i = 0; i < 8; i++)
                    if (chr_cnpj[i + 5] - 48 >= 0 && chr_cnpj[i + 5] - 48 <= 9)
                        soma += (chr_cnpj[i + 5] - 48) * (10 - (i + 1));
                dig = 11 - (soma % 11);
                cnpj_calc += (dig == 10 || dig == 11) ?
                    "0" : dig.ToString();

                return strCnpj.Equals(cnpj_calc);
            }
            catch
            {
                return false;
            }
        }
    }
}
