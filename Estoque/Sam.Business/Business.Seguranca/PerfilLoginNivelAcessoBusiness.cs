using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;
using Sam.Common.Util;
using Sam.Entity;
using Sam.Infrastructure;
using Sam.Common;
using System.Linq.Expressions;

namespace Sam.Business
{
    public class PerfilLoginNivelAcessoBusiness : BaseBusinessSeguranca
    {
        private Sam.Entity.PerfilLoginNivelAcesso perfilLoginNivelAcesso = new Sam.Entity.PerfilLoginNivelAcesso();

        public Sam.Entity.PerfilLoginNivelAcesso PerfilLoginNivelAcesso
        {
            get { return perfilLoginNivelAcesso; }
            set { perfilLoginNivelAcesso = value; }
        }

        private Sam.Entity.Perfil perfil = new Sam.Entity.Perfil();

        public Sam.Entity.Perfil Perfil
        {
            get { return perfil; }
            set { perfil = value; }
        }

        private Sam.Entity.Login login = new Sam.Entity.Login();

        public Sam.Entity.Login Login
        {
            get { return login; }
            set { login = value; }
        }

        public List<PerfilLoginNivelAcesso> EditarUsuarioPerfil(int loginPerfilId)
        {

            return new PerfilLoginNivelAcessoInfrastructure().EditarUsuarioPerfil(loginPerfilId);
        }

        public IList<DivisaoEntity> ListarDivisaoByUALogin(int uaId, int gestorId, int loginId)
        {
            return new PerfilLoginNivelAcessoInfrastructure().ListarDivisaoByUALogin(uaId, gestorId, loginId);
        }

        public bool Salvar()
        {
            try
            {
                PerfilLoginNivelAcessoInfrastructure infra = new PerfilLoginNivelAcessoInfrastructure();
                infra.Entity = this.PerfilLoginNivelAcesso;
                Validar();
                if (ListaErro.Count > 0)
                {
                    return false;
                }
                infra.Salvar();
                return true;
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception("Erro no sistema: " + e.Message);
                return false;//??
            }
        }


        public void Validar()
        {
            if (this.PerfilLoginNivelAcesso.Valor == 0)
            {

                if (this.PerfilLoginNivelAcesso.NivelAcesso.NivelId == NivelAcessoEnum.Orgao)
                    throw new Exception("Informar o órgão!");
                else if (this.PerfilLoginNivelAcesso.NivelAcesso.NivelId == NivelAcessoEnum.UO)
                    throw new Exception("Informar a UO!");
                else if (this.PerfilLoginNivelAcesso.NivelAcesso.NivelId == NivelAcessoEnum.UGE)
                    throw new Exception("Informar a UGE!");
                else if (this.PerfilLoginNivelAcesso.NivelAcesso.NivelId == NivelAcessoEnum.UA)
                    throw new Exception("Informar a UA!");
                else if (this.PerfilLoginNivelAcesso.NivelAcesso.NivelId == NivelAcessoEnum.GESTOR)
                    throw new Exception("Informar o Gestor!");
                else if (this.PerfilLoginNivelAcesso.NivelAcesso.NivelId == NivelAcessoEnum.ALMOXARIFADO)
                    throw new Exception("Informar o Almoxarifado!");
                else if (this.PerfilLoginNivelAcesso.NivelAcesso.NivelId == NivelAcessoEnum.DIVISAO)
                    throw new Exception("Informar o Divisão!");
            }
        }

        public bool Excluir()
        {
            try
            {
                PerfilLoginNivelAcessoInfrastructure infra = new PerfilLoginNivelAcessoInfrastructure();
                infra.Entity = this.PerfilLoginNivelAcesso;
                if (!ConsistirPerfilLoginNivelAcesso(this.PerfilLoginNivelAcesso.PerfilLoginId))
                {
                    throw new Exception("Perfil do usuário associado ao nível de acesso!");
                    return false;
                }
                infra.Excluir();
                return true;
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception("Erro no sistema: " + e.Message);
                return false;
            }
        }

        public bool ConsistirPerfilLoginNivelAcesso(int perfilLoginId)
        {
            PerfilLoginNivelAcessoInfrastructure infra = new PerfilLoginNivelAcessoInfrastructure();
            infra.Entity = this.PerfilLoginNivelAcesso;
            if (!infra.Listar(perfilLoginId))
            {
                throw new Exception("Existem nÃ­veis de acesso associados ao perfil do usuário.");
                return false;
            }
            return true;
        }

        public List<Sam.Entity.PerfilLoginNivelAcesso> ListarPerfilLoginNivelAcesso(int idLogin, string NivelAcessoDesc)
        {
            PerfilLoginNivelAcessoInfrastructure infra = new PerfilLoginNivelAcessoInfrastructure();
            return infra.ListarPerfilLoginNivelAcesso(idLogin, NivelAcessoDesc);
        }

        public List<Sam.Entity.PerfilLoginNivelAcesso> ListarPerfilLoginNivelAcesso(int idLogin, string NivelAcessoDesc, short idPerfil)
        {
            return new PerfilLoginNivelAcessoInfrastructure().ListarPerfilLoginNivelAcesso(idLogin, NivelAcessoDesc, idPerfil);
        }

        //Checar uso
        public List<Sam.Entity.PerfilLoginNivelAcesso> ListarPerfilLoginNivelAcesso(int idLogin, string NivelAcessoDesc, Int16 idPerfil, IList<int> perfisLoginsIds)
        {            
            return new PerfilLoginNivelAcessoInfrastructure().ListarPerfilLoginNivelAcesso(idLogin, NivelAcessoDesc, idPerfil,  perfisLoginsIds);
        }

        //Checar uso
        public IList<int> ListarPerfilLoginIds(int idLogin, string NivelAcessoDesc, Int16 idPerfil)
        {
            return new PerfilLoginNivelAcessoInfrastructure().ListarPerfilLoginIds(idLogin, NivelAcessoDesc, idPerfil);
        }

        public List<Sam.Entity.PerfilLoginNivelAcesso> ListarPerfilLoginNivelAcessoUser(int IdUsuario)
        {
            PerfilLoginNivelAcessoInfrastructure infra = new PerfilLoginNivelAcessoInfrastructure();
            return infra.ListarPerfilLoginNivelAcessoUser(IdUsuario);
        }

        public List<Sam.Entity.PerfilLoginNivelAcesso> ListarPerfilLoginNivelAcesso(int perfilLoginId)
        {
            PerfilLoginNivelAcessoInfrastructure infra = new PerfilLoginNivelAcessoInfrastructure();
            return infra.ListarPerfilLoginNivelAcessoPerfilLoginId(perfilLoginId);
        }
                
        public IList<AlmoxarifadoEntity> ListarPerfilLoginNivelAcesso(int idLogin, int idGestor)
        {
            //Retorna os almoxarifados do nivel de acesso e pesquisa os almoxarifados 
            PerfilLoginNivelAcessoInfrastructure infra = new PerfilLoginNivelAcessoInfrastructure();
            var nivelAcessoAlmox = infra.ListarPerfilLoginNivelAcesso(idLogin, "ALMOXARIFADO");

            List<AlmoxarifadoEntity> almoxs = new List<AlmoxarifadoEntity>();

            foreach (var n in nivelAcessoAlmox)
            {
                AlmoxarifadoEntity almox = new AlmoxarifadoEntity();
                almox.Id = n.AlmoxId;
                almoxs.Add(almox);
            }

            return new AlmoxarifadoInfrastructure().ListarAlmoxarifadosNivelAcesso(idGestor, almoxs);
        }

        public IList<UAEntity> ListarUAByPerfil(int idLogin)
        {
            List<Sam.Entity.PerfilLoginNivelAcesso> perfilUa = ListarPerfilLoginNivelAcesso(idLogin, "UA");

            PerfilLoginNivelAcessoInfrastructure infra = new PerfilLoginNivelAcessoInfrastructure();
            IList<UAEntity> listUA = infra.ListarUA();

            List<UAEntity> listUaPerfil = new List<UAEntity>();

            foreach (PerfilLoginNivelAcesso nivelAcesso in perfilUa)
            {
                listUaPerfil.Add(listUA.Where(a => a.Id == (int)nivelAcesso.Valor).FirstOrDefault());
            }

            return listUaPerfil;
        }

        // ### Requisitante Geral ### 
        public IList<UAEntity> ListarUAByUgeId(int idUGE)
        {
            PerfilLoginNivelAcessoInfrastructure infra = new PerfilLoginNivelAcessoInfrastructure();
            List<UAEntity> listaUas = infra.ListarUA().Where(x => x.Uge.Id == idUGE).ToList();
            return listaUas;
        }

        // ### Requisitante Geral ###
        public IList<UGEEntity> ListarUGEByPerfil(int idLogin)
        {
            List<Sam.Entity.PerfilLoginNivelAcesso> perfilUge = ListarPerfilLoginNivelAcesso(idLogin, "UGE");
            IList<UGEEntity> listaTodasUGEs = new PerfilLoginNivelAcessoInfrastructure().ListarUGE();
            List<UGEEntity> listaUgePerfil = new List<UGEEntity>();

            foreach (PerfilLoginNivelAcesso nivelAcesso in perfilUge)
            {
                listaUgePerfil.Add(listaTodasUGEs.Where(x => x.Id == (int)nivelAcesso.Valor).FirstOrDefault());
            }
            return listaUgePerfil;
        }

        // ### Requisitante Geral ###
        public IList<UOEntity> ListarUOByPerfil(int idLogin)
        {
            List<Sam.Entity.PerfilLoginNivelAcesso> perfilUO = ListarPerfilLoginNivelAcesso(idLogin, "UO");
            PerfilLoginNivelAcessoInfrastructure infra = new PerfilLoginNivelAcessoInfrastructure();
            IList<UOEntity> listaTodasUOs = infra.ListarUO();
            List<UOEntity> listaUOPerfil = new List<UOEntity>();

            foreach (PerfilLoginNivelAcesso nivelAcesso in perfilUO)
            {
                listaUOPerfil.Add(listaTodasUOs.Where(x => x.Id == (int)nivelAcesso.Valor).FirstOrDefault());
            }

            return listaUOPerfil;
        }

        public IList<MovimentoEntity> ListarMovimentoByPerfil(int idLogin, short idPerfil, string numeroDocumento)
        {
            IList<MovimentoEntity> result;
            IList<MovimentoEntity> listaMovimentos = new Sam.Domain.Business.MovimentoBusiness().ListarRequisicaoByNumeroDocumento(numeroDocumento);
            
            switch (idPerfil)
            {
                //Excecao - Sem tratamento para perfilID = 4 que nao contem valores folha TB_NIVEL(Almoxarifado/ Divisão).    
                case (int)GeralEnum.TipoPerfil.AdministradorGeral:
                    result = listaMovimentos;
                    break;

                case (int)GeralEnum.TipoPerfil.Requisitante:
                    result = ListarMovimentoByPerfilDivisao(idLogin, idPerfil, numeroDocumento, listaMovimentos);
                    break;

                case (int)GeralEnum.TipoPerfil.OperadorAlmoxarifado:
                case (int)GeralEnum.TipoPerfil.AdministradorGestor:
                case (int)GeralEnum.TipoPerfil.AdministradorOrgao:
                    result =  ListarMovimentoByPerfilAlmoxarifado(idLogin, idPerfil, numeroDocumento, listaMovimentos);
                    break;

                case (int)GeralEnum.TipoPerfil.RequisitanteGeral:
                    result = ListarMovimentoByPerfilUO(idLogin, idPerfil, numeroDocumento, listaMovimentos);
                    break;

                default:
                    result = new List<MovimentoEntity>();
                    break;
            }

            return result;
        }

        private IList<MovimentoEntity> ListarMovimentoByPerfilAlmoxarifado(int idLogin, int idPerfil, string numeroDocumento, IList<MovimentoEntity> listaMovimentos)
        {
            const string nivelDeAcesso = "ALMOXARIFADO";
            IList<MovimentoEntity> listaMovimentosPerfil = new List<MovimentoEntity>();
            List<PerfilLoginNivelAcesso> perfilLoginNivelAcesso = ListarPerfilLoginNivelAcesso(idLogin, nivelDeAcesso);

            foreach (PerfilLoginNivelAcesso nivelAcesso in perfilLoginNivelAcesso)
            {
                var movimentoTemp = listaMovimentos.Where(x => x.Almoxarifado.Id == (int)nivelAcesso.Valor).FirstOrDefault();
                if (movimentoTemp.IsNotNull() && !listaMovimentosPerfil.Contains(movimentoTemp))
                    listaMovimentosPerfil.Add(movimentoTemp);
            }

            return listaMovimentosPerfil;
        }

        private IList<MovimentoEntity> ListarMovimentoByPerfilDivisao(int idLogin, int idPerfil, string numeroDocumento, IList<MovimentoEntity> listaMovimentos)
        {
            const string nivelDeAcesso = "DIVISÃO";
            IList<MovimentoEntity> listaMovimentosPerfil = new List<MovimentoEntity>();
            List<PerfilLoginNivelAcesso> perfilLoginNivelAcesso = ListarPerfilLoginNivelAcesso(idLogin, nivelDeAcesso);

            foreach (PerfilLoginNivelAcesso nivelAcesso in perfilLoginNivelAcesso)
            {
                var movimentoTemp = listaMovimentos.Where(x => x.Divisao.Id == (int)nivelAcesso.Valor).FirstOrDefault();
                if (movimentoTemp.IsNotNull() && !listaMovimentosPerfil.Contains(movimentoTemp))
                    listaMovimentosPerfil.Add(movimentoTemp);
            }

            return listaMovimentosPerfil;
        }

        private IList<MovimentoEntity> ListarMovimentoByPerfilUO(int idLogin, short idPerfil, string numeroDocumento, IList<MovimentoEntity> listaMovimentos)
        {
            const string nivelDeAcesso = "UO";
            IList<MovimentoEntity> listaMovimentosPerfil = new List<MovimentoEntity>();

            //Percorrer UOs e trazer lista de DivisÃƒÂµes com permissão, abaixo desta UO.
            var perfilLoginRequisitanteGeral = ListarPerfilLoginNivelAcesso(idLogin, nivelDeAcesso, idPerfil);
                        
            //Popula DivisÃƒÂµes abaixo da UO perfil Requisitante Geral.
            List<DivisaoEntity> listaDivisoesComAcesso = new List<DivisaoEntity>();
            foreach (var uo in perfilLoginRequisitanteGeral)
            {
                Expression<Func<TB_DIVISAO, bool>> expression = _ => _.TB_UA.TB_UGE.TB_UO.TB_UO_ID == uo.Valor;
                var tempListDivisoes = new DivisaoBusiness().SelectSimplesExpression(expression);
                if (tempListDivisoes.Count > 0)
                {
                    foreach (var divisao in tempListDivisoes)
                    {
                        listaDivisoesComAcesso.Add(divisao);
                    }
                }
            }
                     
            //Valida se a lista de Movimento pode ser lida pelo usuário logado. (compara as DivisÃƒÂµes de movimentos e as divisÃƒÂµes que se tem acesso)
            if (listaDivisoesComAcesso.IsNotNull())
            {
                foreach(var divisao in listaDivisoesComAcesso)
                {  
                    var movimentoTemp = listaMovimentos.Where(x => x.Divisao.Id.Value == (int)divisao.Id).FirstOrDefault();
                    if (movimentoTemp.IsNotNull() && !listaMovimentosPerfil.Contains(movimentoTemp))
                        listaMovimentosPerfil.Add(movimentoTemp);
                }
            }

            return listaMovimentosPerfil;
        }
       
        // ### Requisitante Geral ###
        public IList<OrgaoEntity> ListarOrgaoByPerfil(int idLogin)
        {
            List<PerfilLoginNivelAcesso> perfilOrgao = ListarPerfilLoginNivelAcesso(idLogin, "Orgão");
            IList<OrgaoEntity> listaTodosOrgaos = new PerfilLoginNivelAcessoInfrastructure().ListarOrgao();
            List<OrgaoEntity> listaOrgaoPerfil = new List<OrgaoEntity>();

            //Adiciona a lista somente os orgãos que o usuario logado tem acesso.
            foreach (PerfilLoginNivelAcesso nivelAcesso in perfilOrgao)
            {
                listaOrgaoPerfil.Add(listaTodosOrgaos.Where(x => x.Id == (int)nivelAcesso.Valor).FirstOrDefault());
            }

            //Concatena Codigo + Orgão.
            List<OrgaoEntity> listaFormatada = new List<OrgaoEntity>();
            foreach (var item in listaOrgaoPerfil.Distinct().ToList())
            {
                listaFormatada.Add(
                            new OrgaoEntity()
                            {
                                Id = item.Id,
                                Codigo = item.Codigo,
                                Descricao = string.Format("{0} - {1}", item.Codigo.ToString().PadLeft(2, '0'), item.Descricao)
                            }
                );
            }

            return listaFormatada;
        }
    }
}
