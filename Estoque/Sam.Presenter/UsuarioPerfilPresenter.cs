using System;
using System.Collections.Generic;
using System.Collections;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Sam.View;
using Sam.Entity;
using Sam.Common.Util;
using Sam.Business;
using System.ComponentModel;
using Sam.Common;
using Sam.Common;

namespace Sam.Presenter
{
    public class UsuarioPerfilPresenter : CrudPresenter<IUsuarioPerfilView>
    {
        IUsuarioPerfilView view;
        EstruturaBusiness estrutura = new EstruturaBusiness();

        public IUsuarioPerfilView View
        {
            get { return view; }
            set { view = value; }
        }

        private Sam.Entity.Perfil perfil = new Sam.Entity.Perfil();

        public Sam.Entity.Perfil Perfil
        {
            get { return perfil; }
            set { perfil = value; }
        }

        private Login login = new Login();

        public Login Login
        {
            get { return login; }
            set { login = value; }
        }
 
        public UsuarioPerfilPresenter()
        {
        }

        public UsuarioPerfilPresenter(IUsuarioPerfilView _view)
            : base(_view)
        {
            this.View = _view;
        }

        private PerfilLoginNivelAcesso ReturnaNivelAcessoOrgao()
        {
            PerfilLoginNivelAcesso perfilLoginNivelAcesso = new PerfilLoginNivelAcesso();
            perfilLoginNivelAcesso.NivelAcesso = new NivelAcesso();

            if (View.OrgaoId.Value > 0)
            {
                perfilLoginNivelAcesso.Valor = Convert.ToInt32(View.OrgaoId.Value);
                perfilLoginNivelAcesso.NivelAcesso.NivelId = (short)NivelAcessoEnum.Orgao;
            }

            return perfilLoginNivelAcesso;
        }

        private PerfilLoginNivelAcesso ReturnaNivelAcessoDivisao()
        {
            PerfilLoginNivelAcesso perfilLoginNivelAcesso = new PerfilLoginNivelAcesso();
            perfilLoginNivelAcesso.NivelAcesso = new NivelAcesso();

            if (View.DivisaoId.Value > 0)
            {
                perfilLoginNivelAcesso.Valor = Convert.ToInt32(View.DivisaoId.Value);
                perfilLoginNivelAcesso.NivelAcesso.NivelId = (short)NivelAcessoEnum.DIVISAO;
            }

            return perfilLoginNivelAcesso;
        }

        private PerfilLoginNivelAcesso ReturnaNivelAcessoAlmoxarifado()
        {
            PerfilLoginNivelAcesso perfilLoginNivelAcesso = new PerfilLoginNivelAcesso();
            perfilLoginNivelAcesso.NivelAcesso = new NivelAcesso();

            if (View.AlmoxId.Value > 0)
            {
                perfilLoginNivelAcesso.Valor = Convert.ToInt32(View.AlmoxId.Value);
                perfilLoginNivelAcesso.NivelAcesso.NivelId = (short)NivelAcessoEnum.ALMOXARIFADO;
            }

            return perfilLoginNivelAcesso;
        }

        private PerfilLoginNivelAcesso ReturnaNivelAcessoGestor()
        {
            PerfilLoginNivelAcesso perfilLoginNivelAcesso = new PerfilLoginNivelAcesso();
            perfilLoginNivelAcesso.NivelAcesso = new NivelAcesso();

            if (View.GestorId.Value > 0)
            {
                perfilLoginNivelAcesso.Valor = Convert.ToInt32(View.GestorId.Value);
                perfilLoginNivelAcesso.NivelAcesso.NivelId = (short)NivelAcessoEnum.GESTOR;
            }

            return perfilLoginNivelAcesso;
        }

        private PerfilLoginNivelAcesso ReturnaNivelAcessoUO()
        {
            PerfilLoginNivelAcesso perfilLoginNivelAcesso = new PerfilLoginNivelAcesso();
            perfilLoginNivelAcesso.NivelAcesso = new NivelAcesso();

            if (View.UoId.Value > 0)
            {
                perfilLoginNivelAcesso.Valor = Convert.ToInt32(View.UoId.Value);
                perfilLoginNivelAcesso.NivelAcesso.NivelId = (short)NivelAcessoEnum.UO;
            }

            return perfilLoginNivelAcesso;
        }

        private PerfilLoginNivelAcesso ReturnaNivelAcessoUA()
        {
            PerfilLoginNivelAcesso perfilLoginNivelAcesso = new PerfilLoginNivelAcesso();
            perfilLoginNivelAcesso.NivelAcesso = new NivelAcesso();

            if (View.UaId.Value > 0)
            {
                perfilLoginNivelAcesso.Valor = Convert.ToInt32(View.UaId.Value);
                perfilLoginNivelAcesso.NivelAcesso.NivelId = (short)NivelAcessoEnum.UA;
            }

            return perfilLoginNivelAcesso;
        }

        private PerfilLoginNivelAcesso ReturnaNivelAcessoUGE()
        {
            PerfilLoginNivelAcesso perfilLoginNivelAcesso = new PerfilLoginNivelAcesso();
            perfilLoginNivelAcesso.NivelAcesso = new NivelAcesso();

            if (View.UgeId.Value > 0)
            {
                perfilLoginNivelAcesso.Valor = Convert.ToInt32(View.UgeId.Value);
                perfilLoginNivelAcesso.NivelAcesso.NivelId = (short)NivelAcessoEnum.UGE;
            }

            return perfilLoginNivelAcesso;
        }

        
        private List<PerfilLoginNivelAcesso> RetornaPerfilNivelAcesso()
        {
            List<PerfilLoginNivelAcesso> PerfilNivelAcessoList = new List<PerfilLoginNivelAcesso>();

            if (View.PerfilId == (int)Sam.Common.Perfil.ADMINISTRADOR_GERAL)
            {
                PerfilNivelAcessoList.Add(ReturnaNivelAcessoOrgao());
                PerfilNivelAcessoList.Add(ReturnaNivelAcessoGestor());
                PerfilNivelAcessoList.Add(ReturnaNivelAcessoAlmoxarifado());
            }
            else
                if (View.PerfilId == (int)Sam.Common.Perfil.ADMINISTRADOR_GESTOR)
                {
                    PerfilNivelAcessoList.Add(ReturnaNivelAcessoOrgao());
                    PerfilNivelAcessoList.Add(ReturnaNivelAcessoGestor());
                    PerfilNivelAcessoList.Add(ReturnaNivelAcessoAlmoxarifado());
                }
                else

                    if (View.PerfilId == (int)Sam.Common.Perfil.ADMINISTRADOR_ORGAO)
                    {
                        PerfilNivelAcessoList.Add(ReturnaNivelAcessoOrgao());
                        PerfilNivelAcessoList.Add(ReturnaNivelAcessoGestor());
                        PerfilNivelAcessoList.Add(ReturnaNivelAcessoAlmoxarifado());
                    }
                    else

                        if (View.PerfilId == (int)Sam.Common.Perfil.OPERADOR_ALMOXARIFADO)
                        {
                            PerfilNivelAcessoList.Add(ReturnaNivelAcessoOrgao());
                            PerfilNivelAcessoList.Add(ReturnaNivelAcessoGestor());
                            PerfilNivelAcessoList.Add(ReturnaNivelAcessoAlmoxarifado());
                        }
                        else
                            if (view.PerfilId == Sam.Common.Perfil.COMERCIAL_PRODESP)
                            {
                                PerfilNivelAcessoList.Add(ReturnaNivelAcessoOrgao());
                                //PerfilNivelAcessoList.Add(ReturnaNivelAcessoGestor());
                            }
                            else
                                if (View.PerfilId == (int)Sam.Common.Perfil.REQUISITANTE || View.PerfilId == (int)Sam.Common.Perfil.REQUISITANTE_GERAL)
                                {
                                    PerfilNivelAcessoList.Add(ReturnaNivelAcessoOrgao());
                                    PerfilNivelAcessoList.Add(ReturnaNivelAcessoUO());
                                    PerfilNivelAcessoList.Add(ReturnaNivelAcessoUA());
                                    PerfilNivelAcessoList.Add(ReturnaNivelAcessoUGE());
                                    PerfilNivelAcessoList.Add(ReturnaNivelAcessoGestor());
                                    PerfilNivelAcessoList.Add(ReturnaNivelAcessoDivisao());
                                }
                                else
                                    if (View.PerfilId == (int)Sam.Common.Perfil.ADMINISTRADOR_PAT)
                                    {
                                        PerfilNivelAcessoList.Add(ReturnaNivelAcessoOrgao());
                                        PerfilNivelAcessoList.Add(ReturnaNivelAcessoGestor());
                                        PerfilNivelAcessoList.Add(ReturnaNivelAcessoAlmoxarifado());
                                    }
                                    else
                                        if (View.PerfilId == (int)Sam.Common.Perfil.ALMOXARIFADO_PAT)
                                        {
                                            PerfilNivelAcessoList.Add(ReturnaNivelAcessoOrgao());
                                            PerfilNivelAcessoList.Add(ReturnaNivelAcessoGestor());
                                            PerfilNivelAcessoList.Add(ReturnaNivelAcessoAlmoxarifado());
                                        }
                                        else
                                            if (View.PerfilId == (int)Sam.Common.Perfil.CONSULTA_GERAL_PAT)
                                            {
                                                PerfilNivelAcessoList.Add(ReturnaNivelAcessoOrgao());
                                                PerfilNivelAcessoList.Add(ReturnaNivelAcessoGestor());
                                                PerfilNivelAcessoList.Add(ReturnaNivelAcessoAlmoxarifado());
                                            }
                                            else
                                                if (View.PerfilId == (int)Sam.Common.Perfil.CONSULTA_UA_PAT)
                                                {
                                                    PerfilNivelAcessoList.Add(ReturnaNivelAcessoOrgao());
                                                    PerfilNivelAcessoList.Add(ReturnaNivelAcessoUO());
                                                    PerfilNivelAcessoList.Add(ReturnaNivelAcessoUA());
                                                    PerfilNivelAcessoList.Add(ReturnaNivelAcessoUGE());
                                                    PerfilNivelAcessoList.Add(ReturnaNivelAcessoGestor());
                                                    PerfilNivelAcessoList.Add(ReturnaNivelAcessoDivisao());
                                                }
                                                else
                                                    if (View.PerfilId == (int)Sam.Common.Perfil.CONSULTA_UGE_PAT)
                                                    {
                                                        PerfilNivelAcessoList.Add(ReturnaNivelAcessoOrgao());
                                                        PerfilNivelAcessoList.Add(ReturnaNivelAcessoGestor());
                                                        PerfilNivelAcessoList.Add(ReturnaNivelAcessoAlmoxarifado());
                                                    }
                                                    else
                                                        if (View.PerfilId == (int)Sam.Common.Perfil.CONSULTA_UGE_PAT)
                                                        {
                                                            PerfilNivelAcessoList.Add(ReturnaNivelAcessoOrgao());
                                                            PerfilNivelAcessoList.Add(ReturnaNivelAcessoGestor());
                                                            PerfilNivelAcessoList.Add(ReturnaNivelAcessoAlmoxarifado());
                                                        }
                                                        else
                                                            if (View.PerfilId == (int)Sam.Common.Perfil.NUCLEO_PATRIMONIO_PAT)
                                                            {
                                                                PerfilNivelAcessoList.Add(ReturnaNivelAcessoOrgao());
                                                                PerfilNivelAcessoList.Add(ReturnaNivelAcessoGestor());
                                                                PerfilNivelAcessoList.Add(ReturnaNivelAcessoAlmoxarifado());
                                                            }
                                                            else
                                                                if (View.PerfilId == (int)Sam.Common.Perfil.OPERADOR_ALMOXARIFADO)
                                                                {
                                                                    PerfilNivelAcessoList.Add(ReturnaNivelAcessoOrgao());
                                                                    PerfilNivelAcessoList.Add(ReturnaNivelAcessoGestor());
                                                                    PerfilNivelAcessoList.Add(ReturnaNivelAcessoAlmoxarifado());
                                                                }
                                                                else
                                                                    if (View.PerfilId == (int)Sam.Common.Perfil.OPERADOR_UA_PAT)
                                                                    {
                                                                        PerfilNivelAcessoList.Add(ReturnaNivelAcessoOrgao());
                                                                        PerfilNivelAcessoList.Add(ReturnaNivelAcessoGestor());
                                                                        PerfilNivelAcessoList.Add(ReturnaNivelAcessoAlmoxarifado());
                                                                    }
                                                                    else
                                                                        if (View.PerfilId == (int)Sam.Common.Perfil.OPERADOR_UGE_PAT)
                                                                        {
                                                                            PerfilNivelAcessoList.Add(ReturnaNivelAcessoOrgao());
                                                                            PerfilNivelAcessoList.Add(ReturnaNivelAcessoGestor());
                                                                            PerfilNivelAcessoList.Add(ReturnaNivelAcessoAlmoxarifado());
                                                                        }
                                                                        else
                                                                            if (View.PerfilId == (int)Sam.Common.Perfil.OPERADOR_UNICO_PAT)
                                                                            {
                                                                                PerfilNivelAcessoList.Add(ReturnaNivelAcessoOrgao());
                                                                                PerfilNivelAcessoList.Add(ReturnaNivelAcessoGestor());
                                                                                PerfilNivelAcessoList.Add(ReturnaNivelAcessoAlmoxarifado());
                                                                            }

                                                    //else
                                                                            //    if (View.PerfilId == (int)Sam.Common.Perfil.REQUISITANTE_GERAL)
                                                                            //    {
                                                                            //        PerfilNivelAcessoList.Add(ReturnaNivelAcessoOrgao());
                                                                            //        PerfilNivelAcessoList.Add(ReturnaNivelAcessoGestor());
                                                                            //        PerfilNivelAcessoList.Add(ReturnaNivelAcessoUO());
                                                                            //        //PerfilNivelAcessoList.Add(ReturnaNivelAcessoUA());
                                                                            //        //PerfilNivelAcessoList.Add(ReturnaNivelAcessoUGE());                
                                                                            //        //PerfilNivelAcessoList.Add(ReturnaNivelAcessoDivisao());
                                                                            //    }
                                                                            else
                                                                            {
                                                                                PerfilNivelAcessoList.Add(ReturnaNivelAcessoOrgao());
                                                                                PerfilNivelAcessoList.Add(ReturnaNivelAcessoGestor());
                                                                                PerfilNivelAcessoList.Add(ReturnaNivelAcessoAlmoxarifado());
                                                                            }

            return PerfilNivelAcessoList;
        }

        public void GravarPerfilLogin()
        {
            PerfilLoginBusiness estruturaPerfil = new PerfilLoginBusiness();

            var listErros = View.ConsistirPerfil();

            if (listErros.Count == 0)
            {
                try
                {
                    PerfilLogin perfilLoginEntity = new PerfilLogin();
                    perfilLoginEntity.PerfilId = (int)View.PerfilId;
                    perfilLoginEntity.LoginId = (int)View.LoginId;

                    if (View.IsPerfilPadrao)
                    {
                        //Perfil Login                       
                        int _gestorId;
                        int.TryParse(View.GestorId.ToString(), out _gestorId);

                        perfilLoginEntity.OrgaoPadraoId = (int)View.OrgaoId;
                        perfilLoginEntity.AlmoxarifadoPadraoId = (int)View.AlmoxId;
                        perfilLoginEntity.DivisaoPadraoId = (int)View.DivisaoId;
                        perfilLoginEntity.GestorPadraoId = _gestorId;
                        perfilLoginEntity.OrgaoPadraoId = (int)View.OrgaoId;
                        perfilLoginEntity.UAPadraoId = (int)View.UaId;
                        perfilLoginEntity.UGEPadraoId = (int)View.UgeId;
                        perfilLoginEntity.UOPadraoId = (int)View.UoId;
                    }
                    else
                    {
                        //Caso não for padrão, gravar 0
                        perfilLoginEntity.OrgaoPadraoId = 0;
                        perfilLoginEntity.AlmoxarifadoPadraoId = 0;
                        perfilLoginEntity.DivisaoPadraoId = 0;
                        perfilLoginEntity.GestorPadraoId = 0;
                        perfilLoginEntity.OrgaoPadraoId = 0;
                        perfilLoginEntity.UAPadraoId = 0;
                        perfilLoginEntity.UGEPadraoId = 0;
                        perfilLoginEntity.UOPadraoId = 0;
                    }

                    perfilLoginEntity.PerfilLoginNivelAcessoList = RetornaPerfilNivelAcesso();

                    if (View.PerfilLoginId == null || View.PerfilLoginId == 0)
                    {
                        View.PerfilLoginId = null; //set null para persistir no banco corretamente                      
                        estruturaPerfil.SalvarLoginPerfil(perfilLoginEntity, View.IsPerfilPadrao);
                    }
                    else
                    {
                        perfilLoginEntity.IdPerfilLogin = (int)View.PerfilLoginId;
                        estruturaPerfil.AtualizarLoginPerfil(perfilLoginEntity, View.IsPerfilPadrao);
                    }

                    this.View.PopularGrid();
                    this.GravadoSucesso();
                    this.View.ExibirMensagem("Registro Salvo Com Sucesso.");
                }
                catch (Exception ex)
                {
                    this.View.ExibirMensagem("Inconsistências encontradas.");
                    List<string> erros = new List<string>();
                    erros.Add(ex.Message);
                    this.View.ListaErros = erros;
                }
            }
            else
            {
                this.View.ExibirMensagem("Inconsistências encontradas.");
                this.View.ListaErros = listErros;
            }
        }

        public void LerRegistro(int? _idLogin, int _idLoginPerfil, int idPerfil)
        {
            if (_idLogin.HasValue)
            {
                PerfilLoginNivelAcessoBusiness estruturaPerfil = new PerfilLoginNivelAcessoBusiness();

                var PerfilNivelAcesso = estruturaPerfil.EditarUsuarioPerfil(_idLoginPerfil);

                this.View.PerfilId = idPerfil;
                this.View.OrgaoId = PerfilNivelAcesso.Where(a => a.NivelAcesso.IdNivelAcesso == (int)Sam.Common.NivelAcessoEnum.Orgao).FirstOrDefault().Valor;

                //Valida se o perfil é requisitante
                if (View.PerfilId == (int)Sam.Common.Perfil.REQUISITANTE)
                {
                    this.View.UoId = PerfilNivelAcesso.Where(a => a.NivelAcesso.IdNivelAcesso == (int)Sam.Common.NivelAcessoEnum.UO).FirstOrDefault().Valor;
                    this.View.UgeId = PerfilNivelAcesso.Where(a => a.NivelAcesso.IdNivelAcesso == (int)Sam.Common.NivelAcessoEnum.UGE).FirstOrDefault().Valor;
                    this.View.UaId = PerfilNivelAcesso.Where(a => a.NivelAcesso.IdNivelAcesso == (int)Sam.Common.NivelAcessoEnum.UA).FirstOrDefault().Valor;
                    this.View.GestorId = PerfilNivelAcesso.Where(a => a.NivelAcesso.IdNivelAcesso == (int)Sam.Common.NivelAcessoEnum.GESTOR).FirstOrDefault().Valor;

                    //Requisitante tem a Divisão
                    this.View.DivisaoId = PerfilNivelAcesso.Where(a => a.NivelAcesso.IdNivelAcesso == (int)Sam.Common.NivelAcessoEnum.DIVISAO).FirstOrDefault().Valor;
                }

                //TODO Eduardo Almeida: Altração de código Requisitante Geral.
                else if (view.PerfilId == (int)Sam.Common.Perfil.REQUISITANTE_GERAL)
                {
                    this.View.GestorId = PerfilNivelAcesso.Where(a => a.NivelAcesso.IdNivelAcesso == (int)Sam.Common.NivelAcessoEnum.GESTOR).FirstOrDefault().Valor;
                    this.View.UoId = PerfilNivelAcesso.Where(a => a.NivelAcesso.IdNivelAcesso == (int)Sam.Common.NivelAcessoEnum.UO).FirstOrDefault().Valor;
                }
                else
                {
                    this.View.GestorId = PerfilNivelAcesso.Where(a => a.NivelAcesso.IdNivelAcesso == (int)Sam.Common.NivelAcessoEnum.GESTOR).FirstOrDefault().Valor;
                    //Requisitante não tem almoxarifado
                    this.View.AlmoxId = PerfilNivelAcesso.Where(a => a.NivelAcesso.IdNivelAcesso == (int)Sam.Common.NivelAcessoEnum.ALMOXARIFADO).FirstOrDefault().Valor;
                }

                this.View.BloqueiaCancelar = false;
                this.View.BloqueiaGravar = false;
                this.View.BloqueiaNovo = true;
                this.View.MostrarPainelEdicao = true;
            }
        }

        public IList<Domain.Entity.UFEntity> PopularDadosUf()
        {
            Domain.Business.EstruturaOrganizacionalBusiness estrut = new Domain.Business.EstruturaOrganizacionalBusiness();
            return estrut.ListarUF();
        }

        public IList<Sam.Entity.Perfil> PopularDadosUsuarioGrid(int _loginId)
        {
            PerfilBusiness perf = new PerfilBusiness(_loginId);
            return perf.RecuperarPerfil();
        }

        public IList<Sam.Entity.Perfil> PopularDadosUsuarioPerfilGrid(int _loginId)
        {
            PerfilBusiness perf = new PerfilBusiness(_loginId);
            return perf.RecuperarUsuarioPerfil();
        }

        public IList<PerfilNivel> PopularPerfilNivel(int PerfilId)
        {
            PerfilNivelBusiness perf = new PerfilNivelBusiness();
            return perf.LerNivelAceso(PerfilId);
        }

        public void Excluir()
        {
            PerfilLoginNivelAcessoBusiness estruturaPerfilLoginNivelAcesso = new PerfilLoginNivelAcessoBusiness();
            PerfilLoginBusiness estrutura = new PerfilLoginBusiness();

            try
            {
                estrutura.ExcluirPerfilNivelAcesso(Perfil.PerfilLoginId);
            }
            catch (Exception ex)
            {
                estrutura.ListaErro.Add(ex.Message);

                this.View.ExibirMensagem("Inconsistências encontradas.");
                this.View.ListaErros = estrutura.ListaErro;
                return;
            }

            this.View.ExibirMensagem("Perfil do usuário excluído com sucesso.");
            this.View.PopularGrid();
            Cancelar();
        }

        public void Imprimir()
        {

        }

        public bool PermitidoGravarRequisitanteGeral(int userId, int novaUoId)
        {
            bool permitidoGravar = true;
            var requisitanteGeralId = (int)Sam.Common.Perfil.REQUISITANTE_GERAL;

            List<Sam.Entity.Perfil> recuperaPerfiLogin_RequisitanteGerais = Login.Perfis.Where(x => x.IdPerfil == requisitanteGeralId).ToList();             
            List<perfilLoginNivelUsuarioLogado> loginNivelAcessoUsuarioLogado = new List<perfilLoginNivelUsuarioLogado>();
            perfilLoginNivelUsuarioLogado nivelAcessoNovo = new perfilLoginNivelUsuarioLogado()
            {
                UoId = novaUoId,
                PerfilLoginId = Convert.ToInt32(this.view.Id)
            };

            foreach (var perfil in recuperaPerfiLogin_RequisitanteGerais)
            {
                if (perfil.PerfilLoginNivelAcesso.Count > 0)
                {
                    foreach (var perfilLoginNivelAcessoPorUO in perfil.PerfilLoginNivelAcesso.Where(x => x.NivelAcesso.DescricaoNivel == "UO"))
                    {
                        loginNivelAcessoUsuarioLogado.Add(
                                                    new perfilLoginNivelUsuarioLogado()
                                                    {
                                                        UoId = (int)perfilLoginNivelAcessoPorUO.Valor,
                                                        PerfilLoginId = perfilLoginNivelAcessoPorUO.PerfilLoginId
                                                    });
                    }
                }
            }

            //Lista de TB_LOGIN_NIVEL_ACESSO por PERFIL_ID existe a nova UO? Caso negativo, permiti gravar.
            if (loginNivelAcessoUsuarioLogado.Exists(a => a.UoId == nivelAcessoNovo.UoId))
            {
                //Se na lista os atributos UOId e perfilLoginId forem iguais, permiti gravar (EDITAR).
                if (!loginNivelAcessoUsuarioLogado.Exists(x => x.PerfilLoginId == nivelAcessoNovo.PerfilLoginId && x.UoId == nivelAcessoNovo.UoId))
                {
                    permitidoGravar = false;

                    this.View.ExibirMensagem("Inconsistências encontradas.");
                    this.View.ListaErros = new List<string>() { "A UO selecioanda já está atrelada a esse login." };
                }
            }

            return permitidoGravar;

        }

        #region override

        public override void GravadoSucesso()
        {
            base.GravadoSucesso();
            this.View.ApagarSessaoIDs();
            this.View.BloqueiaGravar = true;
            this.View.BloqueiaExcluir = true;
            this.View.BloqueiaCancelar = true;
            this.View.BloqueiaCodigo = false;
            this.View.BloqueiaDescricao = false;
            this.View.BloqueiaNovo = false;
            this.View.Codigo = string.Empty;
            this.View.Descricao = string.Empty;
            this.View.Id = null;
            this.View.ListaErros = null;
            this.View.MostrarPainelEdicao = false;
        }

        public override void Load()
        {
            this.View.BloqueiaGravar = true;
            this.View.BloqueiaExcluir = true;
            this.View.BloqueiaCancelar = true;
            this.View.BloqueiaNovo = false;
            this.View.MostrarPainelEdicao = false;
        }

        public override void Novo()
        {
            this.View.BloqueiaGravar = false;
            this.View.BloqueiaCancelar = false;
            this.View.Descricao = string.Empty;
            this.View.Id = null;
            this.View.PerfilLoginId = 0;
            Perfil.IdPerfil = Convert.ToInt16(this.View.PerfilId.Value);
            this.View.OrgaoId = null;
            this.View.UoId = null;
            this.View.UgeId = null;
            this.View.UaId = null;
            this.View.GestorId = null;
            this.View.AlmoxId = null;
            this.View.DivisaoId = null;

            this.View.MostrarPainelEdicao = true;
            this.View.BloqueiaNovo = true;
        }

        public override void Cancelar()
        {
            this.View.BloqueiaGravar = true;
            this.View.BloqueiaCancelar = true;
            this.View.BloqueiaNovo = false;
            this.View.Id = null;
            this.View.Codigo = string.Empty;
            this.View.Descricao = string.Empty;
            this.View.MostrarPainelEdicao = false;
        }

        #endregion
    }

    public class perfilLoginNivelUsuarioLogado
    {
        public int PerfilLoginId { get; set; }
        public int UoId { get; set; }
    }
}
