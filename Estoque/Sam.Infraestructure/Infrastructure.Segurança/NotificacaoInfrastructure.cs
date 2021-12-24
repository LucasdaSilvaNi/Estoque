using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Linq.Expressions;

namespace Sam.Infrastructure
{
    public class NotificacaoInfrastructure : AbstractCrud<TB_NOTIFICACAO, SEGwebEntities>
    {
        public IList<TB_NOTIFICACAO> ListarNotificacao(Expression<Func<TB_NOTIFICACAO, bool>> where, int SkipRegistros)
        {
            Context.ContextOptions.LazyLoadingEnabled = true;

            var result = (from not in Context.TB_NOTIFICACAO
                          select not).Where(where).OrderBy(a => a.TB_NOTIFICACAO_TITULO).Skip(SkipRegistros).ToList();

            this.TotalRegistros = (from not in Context.TB_NOTIFICACAO
                                   select not).Where(where).Count();

            foreach (var r in result)
            {
                if (r.TB_PERFIL == null)
                {
                    r.TB_PERFIL_DESCRICAO = "Todos os Perfis";
                }
                else
                {
                    r.TB_PERFIL_DESCRICAO = r.TB_PERFIL.TB_PERFIL_DESCRICAO;
                }
            }

            return result;
        }
    }
}
