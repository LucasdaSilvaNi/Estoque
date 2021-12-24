using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Entity;
using Sam.Domain.Entity;
using System.Linq.Expressions;


namespace Sam.Infrastructure
{

    public class PerfilLoginNivelAcessoInfrastructure : BaseInfrastructure
    {

        private Sam.Entity.PerfilLoginNivelAcesso perfilLoginNivelAcesso = new Sam.Entity.PerfilLoginNivelAcesso();

        public Sam.Entity.PerfilLoginNivelAcesso Entity
        {
            get { return perfilLoginNivelAcesso; }
            set { perfilLoginNivelAcesso = value; }
        }

        public List<Sam.Entity.PerfilLoginNivelAcesso> EditarUsuarioPerfil(int loginPerfilId)
        {
            List<Sam.Entity.PerfilLoginNivelAcesso> resposta = null;
            resposta = (from ac in Db.TB_LOGIN_NIVEL_ACESSO
                        where ac.TB_PERFIL_LOGIN_ID.Equals(loginPerfilId)
                        select
                            new
                                Sam.Entity.PerfilLoginNivelAcesso
                            {
                                NivelAcesso = new NivelAcesso { IdNivelAcesso = ac.TB_NIVEL_ID },
                                Valor = ac.TB_LOGIN_NIVEL_ACESSO_VALOR
                            }).ToList();

            if (resposta.Count > 0)
                return resposta;
            else
                return null;

        }

        public IList<DivisaoEntity> ListarDivisaoByUALogin(int uaId, int gestorId, int loginId)
        {

            var pesoPerfil = (from b in Db.TB_LOGIN_NIVEL_ACESSO
                                where b.TB_PERFIL_LOGIN.TB_LOGIN_ID == loginId
                                where b.TB_PERFIL_LOGIN.TB_PERFIL.TB_PERFIL_PESO >= 60
                                select new PerfilLoginNivelAcesso
                                {
                                    Id = b.TB_PERFIL_LOGIN_ID
                                }).Count();

            var divisoes = (from a in DbSam.TB_DIVISAO
                             .Where(a => a.TB_UA_ID == uaId)
                             where a.TB_ALMOXARIFADO.TB_GESTOR_ID == gestorId                             
                             select new DivisaoEntity
                             {
                                 Id = a.TB_DIVISAO_ID,
                                 Codigo = a.TB_DIVISAO_CODIGO,
                                 Descricao = a.TB_DIVISAO_DESCRICAO,
                                 Almoxarifado = (from almox in DbSam.TB_ALMOXARIFADO
                                                 where a.TB_ALMOXARIFADO_ID == almox.TB_ALMOXARIFADO_ID
                                                 select new AlmoxarifadoEntity
                                                 {
                                                     Id = almox.TB_ALMOXARIFADO_ID
                                                 }).FirstOrDefault()
                             }).ToList();

            if (pesoPerfil == 0)//Não ÃƒÂ© administrador
            {
                var almoxPerfils = (from b in Db.TB_LOGIN_NIVEL_ACESSO
                                    where b.TB_PERFIL_LOGIN.TB_LOGIN_ID == loginId
                                    select new PerfilLoginNivelAcesso
                                    {
                                        AlmoxId = b.TB_LOGIN_NIVEL_ACESSO_VALOR
                                    }).ToList();

                List<DivisaoEntity> divisoesPerfil = new List<DivisaoEntity>();
                foreach (DivisaoEntity div in divisoes)
                {
                    foreach (PerfilLoginNivelAcesso perfilAlmox in almoxPerfils)
                    {
                        if (div.Almoxarifado.Id == perfilAlmox.AlmoxId)
                            divisoesPerfil.Add(div);
                    }
                }
                return divisoesPerfil;
            }
            else
            {
                return divisoes;
            }

            
        }

        public void Salvar() 
        {
            TB_LOGIN_NIVEL_ACESSO logNivAcessoTable = new TB_LOGIN_NIVEL_ACESSO();
            TB_PERFIL_LOGIN perfilLoginTable = new TB_PERFIL_LOGIN();
            TB_NIVEL nivelTable = new TB_NIVEL();

            if (this.Entity.Id != 0)
            {
                logNivAcessoTable = this.Db.TB_LOGIN_NIVEL_ACESSO.Where(a => a.TB_LOGIN_NIVEL_ACESSO_ID == this.Entity.IdLoginNivelAcesso).FirstOrDefault();
                Db.Attach(logNivAcessoTable); // modo de alteração
            }
            else
                Db.TB_LOGIN_NIVEL_ACESSO.AddObject(logNivAcessoTable); // modo de inclusão

            if (this.Entity.PerfilLoginId != 0)
                perfilLoginTable = Db.TB_PERFIL_LOGIN.FirstOrDefault(a => a.TB_PERFIL_LOGIN_ID == Entity.PerfilLoginId);
            else
                Db.AddToTB_PERFIL_LOGIN(perfilLoginTable);

            if (this.Entity.NivelAcesso.NivelId != 0)
                nivelTable = Db.TB_NIVEL.FirstOrDefault(a => a.TB_NIVEL_ID == this.Entity.NivelAcesso.NivelId);
            else
                Db.AddToTB_NIVEL(nivelTable);

            logNivAcessoTable.TB_LOGIN_NIVEL_ACESSO_VALOR = this.Entity.Valor ?? 0;
            logNivAcessoTable.TB_NIVEL_ID = Convert.ToByte(this.Entity.NivelAcesso.NivelId);

            logNivAcessoTable.TB_NIVELReference.Value = nivelTable;
            logNivAcessoTable.TB_PERFIL_LOGINReference.Value = perfilLoginTable;
            this.Db.SaveChanges();

        }

        public void Excluir() 
        {
            TB_LOGIN_NIVEL_ACESSO logNivAcessoTable = new TB_LOGIN_NIVEL_ACESSO();
            if (this.Entity.IdLoginNivelAcesso != 0)
            {
                logNivAcessoTable = this.Db.TB_LOGIN_NIVEL_ACESSO.Where(a => a.TB_LOGIN_NIVEL_ACESSO_ID == this.Entity.IdLoginNivelAcesso).FirstOrDefault();
            }
            Db.DeleteObject(logNivAcessoTable);
            Db.SaveChanges();
        }

        public bool Listar(int PerfilLoginId) 
        {
            // verificará se existe associação com o perfil
            var result = (from a in Db.TB_LOGIN_NIVEL_ACESSO
                          where a.TB_PERFIL_LOGIN_ID == PerfilLoginId
                          select a)
                          .FirstOrDefault();

            if (result != null)
                return false;

            return true;

        }

        public List<Sam.Entity.PerfilLoginNivelAcesso> ListarPerfilLoginNivelAcesso(int idLogin, string NivelAcessoDesc)
        {
            var result = (from a in Db.TB_LOGIN_NIVEL_ACESSO
                          join n in Db.TB_NIVEL on a.TB_NIVEL_ID equals n.TB_NIVEL_ID
                          join b in Db.TB_PERFIL_LOGIN on a.TB_PERFIL_LOGIN_ID equals b.TB_PERFIL_LOGIN_ID
                          where b.TB_LOGIN_ID == idLogin
                          where n.TB_NIVEL_DESCRICAO == NivelAcessoDesc //hardcode porque a tabela está como identity
                          select new Sam.Entity.PerfilLoginNivelAcesso
                          {
                              Id = a.TB_LOGIN_NIVEL_ACESSO_ID,
                              AlmoxId = a.TB_LOGIN_NIVEL_ACESSO_VALOR,
                              Valor = a.TB_LOGIN_NIVEL_ACESSO_VALOR
                          }).ToList();

            return result;
                          
        }

        public List<Sam.Entity.PerfilLoginNivelAcesso> ListarPerfilLoginNivelAcesso(int idLogin, string NivelAcessoDesc, short idPerfil)
        {
            var result = (from a in Db.TB_LOGIN_NIVEL_ACESSO
                          join n in Db.TB_NIVEL on a.TB_NIVEL_ID equals n.TB_NIVEL_ID
                          join b in Db.TB_PERFIL_LOGIN on a.TB_PERFIL_LOGIN_ID equals b.TB_PERFIL_LOGIN_ID
                          where b.TB_LOGIN_ID == idLogin
                          where b.TB_PERFIL_ID == idPerfil
                          where n.TB_NIVEL_DESCRICAO == NivelAcessoDesc 
                          select new Sam.Entity.PerfilLoginNivelAcesso
                          {
                              Id = a.TB_LOGIN_NIVEL_ACESSO_ID,
                              AlmoxId = a.TB_LOGIN_NIVEL_ACESSO_VALOR,
                              Valor = a.TB_LOGIN_NIVEL_ACESSO_VALOR
                          }).ToList();

            return result;

        }

        public IList<int> ListarPerfilLoginIds(int idLogin, string NivelAcessoDesc, short idPerfil)
        {
            List<int> result = (from a in Db.TB_LOGIN_NIVEL_ACESSO
                          join n in Db.TB_NIVEL on a.TB_NIVEL_ID equals n.TB_NIVEL_ID
                          join b in Db.TB_PERFIL_LOGIN on a.TB_PERFIL_LOGIN_ID equals b.TB_PERFIL_LOGIN_ID
                          where b.TB_LOGIN_ID == idLogin
                          where b.TB_PERFIL_ID == idPerfil
                          where n.TB_NIVEL_DESCRICAO == NivelAcessoDesc
                          select a.TB_PERFIL_LOGIN_ID).ToList();                          

            return result;

        }

        public List<Sam.Entity.PerfilLoginNivelAcesso> ListarPerfilLoginNivelAcesso(int idLogin, string NivelAcessoDesc, Int16 idPerfil, IList<int> perfisLoginsIds)
        {
            var result = (from a in Db.TB_LOGIN_NIVEL_ACESSO
                          join n in Db.TB_NIVEL on a.TB_NIVEL_ID equals n.TB_NIVEL_ID
                          join b in Db.TB_PERFIL_LOGIN on a.TB_PERFIL_LOGIN_ID equals b.TB_PERFIL_LOGIN_ID
                          where b.TB_LOGIN_ID == idLogin
                          where b.TB_PERFIL_ID == idPerfil
                          where n.TB_NIVEL_DESCRICAO == NivelAcessoDesc
                          where perfisLoginsIds.Contains(a.TB_PERFIL_LOGIN_ID)
                          select new Sam.Entity.PerfilLoginNivelAcesso
                          {
                              Id = a.TB_LOGIN_NIVEL_ACESSO_ID,
                              AlmoxId = a.TB_LOGIN_NIVEL_ACESSO_VALOR,
                              Valor = a.TB_LOGIN_NIVEL_ACESSO_VALOR
                          }).ToList();

            return result;

        }


        public List<Sam.Entity.PerfilLoginNivelAcesso> ListarPerfilLoginNivelAcesso(int idLogin)
        {
            var result = (from a in Db.TB_LOGIN_NIVEL_ACESSO
                          join b in Db.TB_PERFIL_LOGIN on a.TB_PERFIL_LOGIN_ID equals b.TB_PERFIL_LOGIN_ID
                          where b.TB_LOGIN_ID == idLogin                          
                          select new Sam.Entity.PerfilLoginNivelAcesso
                          {
                              Id = a.TB_LOGIN_NIVEL_ACESSO_ID,
                              AlmoxId = a.TB_LOGIN_NIVEL_ACESSO_VALOR,
                              Valor = a.TB_LOGIN_NIVEL_ACESSO_VALOR,                              
                          }).ToList();

            return result;

        }

        public List<Sam.Entity.PerfilLoginNivelAcesso> ListarPerfilLoginNivelAcessoUser(int IdUsuario)
        {
            var result = (from a in Db.TB_LOGIN_NIVEL_ACESSO                          
                          join b in Db.TB_PERFIL_LOGIN on a.TB_PERFIL_LOGIN_ID equals b.TB_PERFIL_LOGIN_ID
                          join l in Db.TB_LOGIN on b.TB_LOGIN_ID equals l.TB_LOGIN_ID
                          where l.TB_LOGIN_ID == IdUsuario                          
                          select new Sam.Entity.PerfilLoginNivelAcesso
                          {
                              Id = a.TB_LOGIN_NIVEL_ACESSO_ID,
                              AlmoxId = a.TB_LOGIN_NIVEL_ACESSO_VALOR,
                              Valor = a.TB_LOGIN_NIVEL_ACESSO_VALOR
                          }).ToList();

            return result;

        }
        public List<Sam.Entity.PerfilLoginNivelAcesso> ListarPerfilLoginNivelAcessoPerfilLoginId(int perfilLoginId)
        {
            var result = (from a in Db.TB_LOGIN_NIVEL_ACESSO                          
                          where a.TB_PERFIL_LOGIN_ID == perfilLoginId
                          select new Sam.Entity.PerfilLoginNivelAcesso
                          {
                              Id = a.TB_LOGIN_NIVEL_ACESSO_ID,
                              AlmoxId = a.TB_LOGIN_NIVEL_ACESSO_VALOR,
                              Valor = a.TB_LOGIN_NIVEL_ACESSO_VALOR,
                              NivelAcesso = (from n in  Db.TB_NIVEL
                                             where n.TB_NIVEL_ID == a.TB_NIVEL_ID
                                             select new NivelAcesso
                                             {
                                                 NivelId = n.TB_NIVEL_ID,
                                                 Id = n.TB_NIVEL_ID
                                             }).FirstOrDefault()                              
                          }).ToList();

            return result;
        }

        public IList<UAEntity> ListarUA()
        {
            IList<UAEntity> resultado = (from a in DbSam.TB_UA
                                         orderby a.TB_UA_CODIGO
                                         select new UAEntity
                                         {
                                             Id = a.TB_UA_ID,
                                             Codigo = a.TB_UA_CODIGO,                                             
                                             Descricao = a.TB_UA_DESCRICAO,
                                             UaVinculada = a.TB_UA_VINCULADA,                                             
                                             IndicadorAtividade = a.TB_UA_INDICADOR_ATIVIDADE,                                             
                                         }).ToList<UAEntity>();
            return resultado;
        }

        //TODO Eduardo Almeida: Altração de Código Requisitante Geral.
        public IList<UGEEntity> ListarUGE()
        {
            IList<UGEEntity> resultado = (from a in DbSam.TB_UGE
                                          orderby a.TB_UGE_CODIGO
                                          select new UGEEntity
                                          {
                                              Id = a.TB_UGE_ID,
                                              Codigo = a.TB_UGE_CODIGO,
                                              Descricao = a.TB_UGE_DESCRICAO,
                                              TipoUGE = a.TB_UGE_TIPO
                                          }).ToList<UGEEntity>();
            return resultado;
        }

        //TODO Eduardo Almeida: Altração de Código Requisitante Geral.
        public IList<UOEntity> ListarUO()
        {
            IList<UOEntity> resultado = (from a in DbSam.TB_UO
                                         orderby a.TB_UO_CODIGO
                                         select new UOEntity
                                         {
                                             Id = a.TB_UO_ID,
                                             //Orgao = new OrgaoEntity(a.TB_ORGAO_ID),
                                             Codigo = a.TB_UO_CODIGO,
                                             CodigoDescricao = a.TB_UO_DESCRICAO,
                                             Descricao = a.TB_UO_DESCRICAO
                                         }).ToList();
            return resultado;
        }

        //TODO Eduardo Almeida: Altração de Código Requisitante Geral.
        public IList<OrgaoEntity> ListarOrgao()
        {
            IList<OrgaoEntity> resultado = (from a in DbSam.TB_ORGAO
                                            orderby a.TB_ORGAO_CODIGO
                                            select new OrgaoEntity
                                            {
                                                Id = a.TB_ORGAO_ID,
                                                Codigo = a.TB_ORGAO_CODIGO,
                                                Descricao = a.TB_ORGAO_DESCRICAO
                                            }).ToList();
            return resultado;
        }
    
    }
}
