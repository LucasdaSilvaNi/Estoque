using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Entity;
using Sam.Entity;


namespace Sam.Infrastructure
{
    public class PerfilLoginInfrastructure : BaseInfrastructure
    {
        public void Excluir(int PerfilLoginId) 
        {
            TB_PERFIL_LOGIN perfilLoginTable = new TB_PERFIL_LOGIN();

            if (PerfilLoginId != 0)
            {
                perfilLoginTable = Db.TB_PERFIL_LOGIN.Where(a => a.TB_PERFIL_LOGIN_ID == PerfilLoginId).FirstOrDefault();
            }
            Db.DeleteObject(perfilLoginTable);
            Db.SaveChanges();
        }

        public void Gravar(PerfilLogin perfilLogin, bool IsPerfilPadrao)
        {
            TB_PERFIL_LOGIN perfilLoginTable = new TB_PERFIL_LOGIN();

            //Perfil Login
            perfilLoginTable.TB_ALMOXARIFADO_ID_PADRAO = perfilLogin.AlmoxarifadoPadraoId;
            perfilLoginTable.TB_DIVISAO_ID_PADRAO = perfilLogin.DivisaoPadraoId;
            perfilLoginTable.TB_GESTOR_ID_PADRAO = perfilLogin.GestorPadraoId;
            perfilLoginTable.TB_LOGIN_ID = perfilLogin.LoginId;
            perfilLoginTable.TB_ORGAO_ID_PADRAO = perfilLogin.OrgaoPadraoId;
            perfilLoginTable.TB_PERFIL_ID = Convert.ToInt16(perfilLogin.PerfilId);
            perfilLoginTable.TB_UA_ID_PADRAO = perfilLogin.UAPadraoId;
            perfilLoginTable.TB_UGE_ID_PADRAO = perfilLogin.UGEPadraoId;
            perfilLoginTable.TB_UO_ID_PADRAO = perfilLogin.UOPadraoId;
            perfilLoginTable.TB_PERFIL_LOGIN_ATIVO = IsPerfilPadrao;
            //PerfilNivelAcesso
            foreach (var perfilNivel in perfilLogin.PerfilLoginNivelAcessoList)
            {
                TB_LOGIN_NIVEL_ACESSO loginNivelAcesso = new TB_LOGIN_NIVEL_ACESSO();
                loginNivelAcesso.TB_LOGIN_NIVEL_ACESSO_VALOR = (int)perfilNivel.Valor;
                loginNivelAcesso.TB_NIVEL_ID = (Byte)perfilNivel.NivelAcesso.NivelId;
                perfilLoginTable.TB_LOGIN_NIVEL_ACESSO.Add(loginNivelAcesso);
            }
            
            Db.TB_PERFIL_LOGIN.AddObject(perfilLoginTable);

            if (IsPerfilPadrao)
            {
                //remove como padrão os perfils anteriores
                //List<TB_PERFIL_LOGIN> result = Db.TB_PERFIL_LOGIN.Where(a => a.TB_LOGIN_ID == (short)perfilLogin.LoginId).ToList();
                IQueryable<TB_PERFIL_LOGIN> result = Db.TB_PERFIL_LOGIN.Where(a => a.TB_LOGIN_ID == perfilLogin.LoginId).AsQueryable();
                foreach (var a in result.ToList().Where(a => a.TB_PERFIL_LOGIN_ID != perfilLogin.IdPerfilLogin))
                {
                    a.TB_ALMOXARIFADO_ID_PADRAO = 0;
                    a.TB_DIVISAO_ID_PADRAO = 0;
                    a.TB_GESTOR_ID_PADRAO = 0;
                    a.TB_UA_ID_PADRAO = 0;
                    a.TB_UGE_ID_PADRAO = 0;
                    a.TB_UO_ID_PADRAO = 0;
                    a.TB_ORGAO_ID_PADRAO = 0;
                    a.TB_PERFIL_LOGIN_ATIVO = false;
                }
            }
            
            Db.SaveChanges();
        }

        public void Atualizar(PerfilLogin perfilLogin, bool IsPerfilPadrao)
        {
            //Exclui os registros
            List<TB_LOGIN_NIVEL_ACESSO> loginNivelAcessoExcluir = Db.TB_LOGIN_NIVEL_ACESSO.Where(a => a.TB_PERFIL_LOGIN_ID == perfilLogin.IdPerfilLogin).ToList();
            TB_PERFIL_LOGIN perfilLoginTables = Db.TB_PERFIL_LOGIN.Where(a => a.TB_PERFIL_LOGIN_ID == perfilLogin.IdPerfilLogin).FirstOrDefault();

            //Exclui todos os Niveis de acesso associados ao perfil
            foreach (var loginNivel in loginNivelAcessoExcluir)
            {
                Db.TB_LOGIN_NIVEL_ACESSO.DeleteObject(loginNivel);
            }

            //Exclui o perfil
            Db.TB_PERFIL_LOGIN.DeleteObject(perfilLoginTables);

            if (IsPerfilPadrao)
            {
                //remove como padrão os perfils anteriores
                //List<TB_PERFIL_LOGIN> result = Db.TB_PERFIL_LOGIN.Where(a => a.TB_LOGIN_ID == (short)perfilLogin.LoginId).ToList();
                IQueryable<TB_PERFIL_LOGIN> result = Db.TB_PERFIL_LOGIN.Where(a => a.TB_LOGIN_ID == perfilLogin.LoginId).AsQueryable();
                foreach (var a in result.ToList().Where(a => a.TB_PERFIL_LOGIN_ID != perfilLogin.IdPerfilLogin))
                {
                    a.TB_ALMOXARIFADO_ID_PADRAO = 0;
                    a.TB_DIVISAO_ID_PADRAO = 0;
                    a.TB_GESTOR_ID_PADRAO = 0;
                    a.TB_UA_ID_PADRAO = 0;
                    a.TB_UGE_ID_PADRAO = 0;
                    a.TB_UO_ID_PADRAO = 0;
                    a.TB_ORGAO_ID_PADRAO = 0;
                    a.TB_PERFIL_LOGIN_ATIVO = false;
                }
            }

            //Insere um novo perfil
            TB_PERFIL_LOGIN perfilLoginTable = new TB_PERFIL_LOGIN();

            //Perfil Login
            perfilLoginTable.TB_ALMOXARIFADO_ID_PADRAO = perfilLogin.AlmoxarifadoPadraoId;
            perfilLoginTable.TB_DIVISAO_ID_PADRAO = perfilLogin.DivisaoPadraoId;
            perfilLoginTable.TB_GESTOR_ID_PADRAO = perfilLogin.GestorPadraoId;
            perfilLoginTable.TB_LOGIN_ID = perfilLogin.LoginId;
            perfilLoginTable.TB_ORGAO_ID_PADRAO = perfilLogin.OrgaoPadraoId;
            perfilLoginTable.TB_PERFIL_ID = Convert.ToInt16(perfilLogin.PerfilId);
            perfilLoginTable.TB_UA_ID_PADRAO = perfilLogin.UAPadraoId;
            perfilLoginTable.TB_UGE_ID_PADRAO = perfilLogin.UGEPadraoId;
            perfilLoginTable.TB_UO_ID_PADRAO = perfilLogin.UOPadraoId;
            perfilLoginTable.TB_PERFIL_LOGIN_ATIVO = IsPerfilPadrao;
            //PerfilNivelAcesso
            foreach (var perfilNivel in perfilLogin.PerfilLoginNivelAcessoList)
            {
                TB_LOGIN_NIVEL_ACESSO loginNivelAcesso = new TB_LOGIN_NIVEL_ACESSO();
                loginNivelAcesso.TB_LOGIN_NIVEL_ACESSO_VALOR = (int)perfilNivel.Valor;
                loginNivelAcesso.TB_NIVEL_ID = (Byte)perfilNivel.NivelAcesso.NivelId;
                perfilLoginTable.TB_LOGIN_NIVEL_ACESSO.Add(loginNivelAcesso);
            }

            Db.TB_PERFIL_LOGIN.AddObject(perfilLoginTable);


            Db.SaveChanges();
        }

        public void ExcluirPerfilNivelAcesso(int perfilLoginId)
        {
            List<TB_LOGIN_NIVEL_ACESSO> loginNivelAcesso = Db.TB_LOGIN_NIVEL_ACESSO.Where(a => a.TB_PERFIL_LOGIN_ID == perfilLoginId).ToList();
            TB_PERFIL_LOGIN perfilLoginTables = Db.TB_PERFIL_LOGIN.Where(a => a.TB_PERFIL_LOGIN_ID == perfilLoginId).FirstOrDefault();

            //Exclui todos os Niveis de acesso associados ao perfil
            foreach (var loginNivel in loginNivelAcesso)
            {
                Db.TB_LOGIN_NIVEL_ACESSO.DeleteObject(loginNivel);
                
            }

            //Exclui o perfil
            Db.TB_PERFIL_LOGIN.DeleteObject(perfilLoginTables);

            Db.SaveChanges();
        }

        public PerfilLogin GetPerfilLogin(int PerfilId, int LoginId)
        {
            TB_PERFIL_LOGIN perfils = Db.TB_PERFIL_LOGIN.Where(a => a.TB_PERFIL_ID  == PerfilId && a.TB_LOGIN_ID == LoginId).FirstOrDefault();

            return ConverterTB_PERFIL_LOGINParaPerfilLogin(perfils);
        }
        public List<PerfilLogin> GetPerfilLogins(int PerfilId, int LoginId)
        {
            List<TB_PERFIL_LOGIN> perfils = Db.TB_PERFIL_LOGIN.Where(a => a.TB_PERFIL_ID == PerfilId && a.TB_LOGIN_ID == LoginId).ToList();

            if (perfils == null)
                return null;

            return perfils.ConvertAll(new Converter<TB_PERFIL_LOGIN, PerfilLogin>(ConverterTB_PERFIL_LOGINParaPerfilLogin));

        }
        private PerfilLogin ConverterTB_PERFIL_LOGINParaPerfilLogin(TB_PERFIL_LOGIN login)
        {
            if (login == null)
                return null;

            return new PerfilLogin()
            {
                AlmoxarifadoPadraoId = (login.TB_ALMOXARIFADO_ID_PADRAO.HasValue ? login.TB_ALMOXARIFADO_ID_PADRAO.Value : 0),
                DivisaoPadraoId = (login.TB_DIVISAO_ID_PADRAO.HasValue ? login.TB_DIVISAO_ID_PADRAO.Value : 0),
                GestorPadraoId = (login.TB_GESTOR_ID_PADRAO.HasValue ? login.TB_GESTOR_ID_PADRAO.Value : 0),
                Id = login.TB_PERFIL_LOGIN_ID,
                PerfilId = login.TB_PERFIL_ID,
                LoginId = login.TB_LOGIN_ID,
                OrgaoPadraoId = (login.TB_ORGAO_ID_PADRAO.HasValue ? login.TB_ORGAO_ID_PADRAO.Value : 0),
                UAPadraoId = (login.TB_UA_ID_PADRAO.HasValue ? login.TB_UA_ID_PADRAO.Value : 0),
                UGEPadraoId = (login.TB_UGE_ID_PADRAO.HasValue ? login.TB_UGE_ID_PADRAO.Value : 0),
                UOPadraoId = (login.TB_UO_ID_PADRAO.HasValue ? login.TB_UO_ID_PADRAO.Value : 0),

            };
        }
    }
}
