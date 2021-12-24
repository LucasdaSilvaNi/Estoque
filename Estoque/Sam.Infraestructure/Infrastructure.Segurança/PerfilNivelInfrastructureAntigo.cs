using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Entity;
using Sam.Entity;

namespace Sam.Infrastructure
{
    public class PerfilNivelInfrastructureAntigo : BaseInfrastructure
    {
        public List<PerfilNivel> LerNivel(int PerfilId)
        {
            List<PerfilNivel> resposta = null;

            resposta = (from perfil_nivel in Db.TB_PERFIL_NIVEL
                           join per in Db.TB_PERFIL on perfil_nivel.TB_PERFIL_ID equals per.TB_PERFIL_ID into i_pn_p
                                from j_pn_p in i_pn_p.DefaultIfEmpty()
                           join nivel in Db.TB_NIVEL on perfil_nivel.TB_NIVEL_ID equals nivel.TB_NIVEL_ID into i_pn_n
                                from j_pn_n in i_pn_n.DefaultIfEmpty()
                           where j_pn_p.TB_PERFIL_ID == PerfilId

                           select new PerfilNivel
                                      {
                                          PerfilId = j_pn_p.TB_PERFIL_ID,
                                          PerfilDescricao = j_pn_p.TB_PERFIL_DESCRICAO,
                                          NivelId = j_pn_n.TB_NIVEL_ID,
                                          NivelDescricao = j_pn_n.TB_NIVEL_DESCRICAO
                                      }).ToList();

            return resposta;
        }

    }
}
