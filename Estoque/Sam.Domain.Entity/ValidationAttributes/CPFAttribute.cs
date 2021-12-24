using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Sam.Domain.Entity.ValidationAttributes
{
    [Serializable]
    public class CPFAttribute : ValidationAttribute
    {
        public CPFAttribute()
        {
        }

        public override bool IsValid(object value)
        {
            return ValidaCPF(value as string);
        }


        /// <summary>
        /// Valida um CPF
        /// </summary>
        /// <param name="strCpf">Número do CPF (incluindo os digitos)</param>
        /// <returns>True CPF válido ou False CPF inválido</returns>
        private bool ValidaCPF(string strCpf)
        {
            try
            {
                int d1, d2;
                int digito1, digito2, resto;
                int digitoCPF;
                string nDigResult;

                if (strCpf.Length != 11 ||
                    Convert.ToInt64(strCpf) == 0)
                    return false;

                d1 = d2 = 0;
                digito1 = digito2 = resto = 0;

                for (int nCount = 1; nCount < strCpf.Length - 1; nCount++)
                {
                    digitoCPF = Convert.ToInt32(strCpf.Substring(nCount - 1, 1));

                    //multiplique a ultima casa por 2 a seguinte por 3 a seguinte por 4 e assim por diante.
                    d1 = d1 + (11 - nCount) * digitoCPF;

                    //para o segundo digito repita o procedimento incluindo o primeiro digito calculado no passo anterior.
                    d2 = d2 + (12 - nCount) * digitoCPF;
                };

                //Primeiro resto da divisão por 11.
                resto = (d1 % 11);

                //Se o resultado for 0 ou 1 o digito é 0 caso contrário o digito é 11 menos o resultado anterior.
                if (resto < 2)
                    digito1 = 0;
                else
                    digito1 = 11 - resto;

                d2 += 2 * digito1;

                //Segundo resto da divisão por 11.
                resto = (d2 % 11);

                //Se o resultado for 0 ou 1 o digito é 0 caso contrário o digito é 11 menos o resultado anterior.
                if (resto < 2)
                    digito2 = 0;
                else
                    digito2 = 11 - resto;

                //Digito verificador do CPF que está sendo validado.
                String nDigVerific = strCpf.Substring(strCpf.Length - 2, 2);

                //Concatenando o primeiro resto com o segundo.
                nDigResult = digito1.ToString() + digito2.ToString();

                //comparar o digito verificador do cpf com o primeiro resto + o segundo resto.
                return nDigVerific.Equals(nDigResult);
            }
            catch
            {
                return false;
            }
        }
    }
}
