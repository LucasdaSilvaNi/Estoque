using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using System.Web.Handlers;
using Sam.View;
using Sam.Common.Util;
using Sam.Presenter;
using Sam.Domain.Entity;

namespace Sam.Web.Seguranca
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    public class obterImagem : IHttpHandler
    {
        
        public void ProcessRequest(HttpContext context)
        {
            int? gestorId = TratamentoDados.TryParseInt32(context.Request.QueryString["id"].ToString());
            int? tipoImagem = TratamentoDados.TryParseInt32(context.Request.QueryString["tipoImagem"].ToString());

            switch (tipoImagem)
            {
                case (int)ImagemEnum.Gestor:
                    {
                        GestorPresenter gestor = new GestorPresenter();
                        GestorEntity entity = gestor.SelecionarRegistro((Int32)gestorId);

                        if (entity.Logotipo != null)
                        {
                            context.Response.BinaryWrite(entity.Logotipo);
                            context.Response.End();
                        }
                    }
                    break;
            }
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}
