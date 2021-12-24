using Sam.Common.Enums;
using Sam.Common.Enums.ChamadoSuporteEnums;
using Sam.Common.Util;
using Sam.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Sam.Infrastructure.Infrastructure.Segurança
{
    public class ESPInfrastructure : AbstractCrud<TB_ESP, SAMwebEntities>
    {
        private const string FORMATODATABR = "dd/MM/yyyy";

        public Sam.Entity.ESPEntity esp = new ESPEntity();

        public Sam.Entity.ESPEntity Entity
        {
            get { return esp; }
            set { esp = value; }
        }

        /// <summary>
        /// Data que será verificada a ESP
        /// </summary>
        public DateTime DataAvaliacao { get; set; }

        /// <summary>
        /// Código do Gestor
        /// </summary>
        public int GestorID { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool ConsistirPesquisa()
        {
            StringBuilder _erros = new StringBuilder();

            if (GestorID == 0)
                _erros.AppendLine("Obrigatório informar o Código do Gestor");

            if (DataAvaliacao == DateTime.MinValue)
                _erros.AppendLine("Obrigatório informar a Data de Avaliação");

            if (_erros.Length > 0)
                throw new ArgumentException(_erros.ToString());

            return _erros.Length == 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Expression<Func<TB_ESP, bool>> ObterClausulaWhereConsulta()
        {
            Expression<Func<TB_ESP, bool>> expWhere;
            expWhere = _esp => _esp.TB_GESTOR_ID == GestorID
                                    && (_esp.TB_ESP_INICIO_VIGENCIA <= DataAvaliacao
                                        && _esp.TB_ESP_FIM_VIGENCIA >= DataAvaliacao);

            return expWhere;
        }

        /// <summary>
        /// Retorna o total de usuários contratados na ESP vigente na Data informada
        /// </summary>
        /// <returns>Total de USUÁRIOS</returns>
        public int ObterTotalUsuarioNivel(PerfilNivelAcessoEnum.PerfilNivelAcesso usuarioNivel)
        {
            ConsistirPesquisa();

            var _result = from t in base.QueryAll(ObterClausulaWhereConsulta())
                          group t by t.TB_GESTOR_ID into _total
                          select new
                          {
                              totalNvI = _total.Sum(a => a.TB_ESP_QTDE_USUARIO_NIVEL_I),
                              totalNvII = _total.Sum(a => a.TB_ESP_QTDE_USUARIO_NIVEL_II)
                          };

            if (_result.SingleOrDefault() == null)
                return 0;

            return usuarioNivel == PerfilNivelAcessoEnum.PerfilNivelAcesso.Nivel_I ? _result.SingleOrDefault().totalNvI : _result.SingleOrDefault().totalNvII;
        }

        /// <summary>
        /// Retorna o total de repositórios contratados na ESP vigente na Data informada
        /// </summary>
        /// <param name="repositorio"></param>
        /// <returns></returns>
        public int ObterTotalRepositorio(RepositorioEnum.Repositorio repositorio)
        {
            ConsistirPesquisa();

            var _result = from t in base.QueryAll(ObterClausulaWhereConsulta())
                          group t by t.TB_GESTOR_ID into _total
                          select new
                          {
                              totalRepoPrinc = _total.Sum(a => a.TB_ESP_QTDE_REPOSITORIO_PRINCIPAL),
                              totalRepoCompl = _total.Sum(a => a.TB_ESP_QTDE_REPOSITORIO_COMPLEMENTAR)
                          };

            if (_result.SingleOrDefault() == null)
                return 0;

            return repositorio == RepositorioEnum.Repositorio.Principal ? _result.SingleOrDefault().totalRepoPrinc : _result.SingleOrDefault().totalRepoCompl;
        }

        public List<ESPEntity> ListarESP(int pularRegistros)
        {
            //Exemplo para sort
            Expression<Func<TB_ESP, int>> sort = a => a.TB_ESP_CODIGO.Value;

            //Exemplo para Where
            Expression<Func<TB_ESP, bool>> where = a => a.TB_ESP_ID != 0;

            var _esp = this.SelectWhere(sort, false, where, pularRegistros);
            var _result = (from _tbESP in _esp
                           select new ESPEntity()
                           {
                               Id = _tbESP.TB_ESP_ID,
                               ESPCodigo = _tbESP.TB_ESP_CODIGO.Value,
                               TermoId = byte.Parse(_tbESP.TB_ESP_TERMO.ToString()),
                               DataInicioVigencia = _tbESP.TB_ESP_INICIO_VIGENCIA.HasValue ? DateTime.Parse(_tbESP.TB_ESP_INICIO_VIGENCIA.ToString()).ToString(FORMATODATABR) : "",
                               //DataFimVigencia = _tbESP.TB_ESP_FIM_VIGENCIA.HasValue ? DateTime.Parse(_tbESP.TB_ESP_FIM_VIGENCIA.ToString()).ToString(FORMATODATABR) : "",
                               DataFimVigencia = DateTime.Parse(_tbESP.TB_ESP_FIM_VIGENCIA.ToString()).ToString(FORMATODATABR),
                               QtdeRepositorioPrincipal = _tbESP.TB_ESP_QTDE_REPOSITORIO_PRINCIPAL,
                               QtdeRepositorioComplementar = _tbESP.TB_ESP_QTDE_REPOSITORIO_COMPLEMENTAR,
                               QtdeUsuarioNivelI = _tbESP.TB_ESP_QTDE_USUARIO_NIVEL_I,
                               QtdeUsuarioNivelII = _tbESP.TB_ESP_QTDE_USUARIO_NIVEL_II,
                               ESPSistema = _tbESP.TB_ESP_SISTEMA,
                               ESPSistemaDescricao = _tbESP.TB_ESP_SISTEMA == "EST" ? EnumUtils.GetEnumDescription(SistemaModulo.Estoque) : EnumUtils.GetEnumDescription(SistemaModulo.Patrimonio),
                               Gestor = (from _gestorA in new GestorInfrastructure().QueryAll()
                                         where _tbESP.TB_GESTOR_ID == _gestorA.TB_GESTOR_ID
                                         select new Gestor() { GestorId = _gestorA.TB_GESTOR_ID, GestorDescricao = _gestorA.TB_GESTOR_NOME }
                                         ).SingleOrDefault<Gestor>()
                           }
                          )
                           .OrderBy(o => o.ESPCodigo)
                            .ThenBy(t => t.ESPSistema)
                            .ThenBy(g => g.Gestor.GestorDescricao)
                           .ToList<ESPEntity>();

            return _result;
        }
    }
}
