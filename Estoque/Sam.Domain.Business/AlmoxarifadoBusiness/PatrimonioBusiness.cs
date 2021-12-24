using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Sam.Domain.Business
{
    public static class PatrimonioBusiness
    {
        public static dynamic GetManagerUnit(int ugeCode)
        {
            try
            {
                string URI = string.Format("{0}/{1}", ConfigurationManager.GetSection("AcessoPatrimonio").ToString(), ugeCode);
                string key = ConfigurationManager.GetSection("ChavePatrimonio").ToString();
                dynamic data = null;

                //using (var client = new HttpClient())
                //{
                //    client.DefaultRequestHeaders.Add("sam_patrimonio_estoque", key);
                //    using (var response = client.GetAsync(URI).Result)
                //    {
                //        if (response.IsSuccessStatusCode)
                //        {
                //            var ProdutoJsonString = response.Content.ReadAsStringAsync().Result;
                //            data = JsonConvert.DeserializeObject<dynamic>(ProdutoJsonString);

                //        }
                //        else
                //        {
                //            throw new Exception(response.ReasonPhrase);
                //        }
                //    }
                //}
                return data;
            }catch(Exception ex)
            {
                throw ex;
            }
        }
        public static dynamic GetManagerUnitDataReferencia(int ugeCode)
        {
            try
            {
                string URI = string.Format("{0}/{1}/{2}", ConfigurationManager.AppSettings.GetValues("AcessoPatrimonio").FirstOrDefault().ToString(), ugeCode, "DataReferencia");
                string key = ConfigurationManager.AppSettings.GetValues("ChavePatrimonio").FirstOrDefault().ToString();
                dynamic data = null;

                using (var client = new System.Net.Http.HttpClient())
                {
                    client.DefaultRequestHeaders.Add("sam_patrimonio_estoque", key);
                    using (var response = client.GetAsync(URI).Result)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var ProdutoJsonString = response.Content.ReadAsStringAsync().Result;
                            data = JsonConvert.DeserializeObject<dynamic>(ProdutoJsonString);

                        }
                        else
                        {
                            throw new Exception(response.ReasonPhrase);
                        }
                    }
                }
                return data;
            }catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
