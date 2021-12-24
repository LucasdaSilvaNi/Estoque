using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using Sam.Domain.Entity;
using Sam.Common.Enums;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Sam.Infrastructure
{
    public class AlmoxarifadoInfrastructure : AbstractCrud<TB_ALMOXARIFADO, SAMwebEntities>
    {
        public IList<AlmoxarifadoEntity> ListarAlmoxarifadosNivelAcesso(int idGestor, List<AlmoxarifadoEntity> almoxarifadosNivelAcesso)
        {
            var result = (from a in  Context.TB_ALMOXARIFADO
                          where a.TB_GESTOR_ID == idGestor || idGestor == null || idGestor == 0// Gestor Id Opcional
                          select new AlmoxarifadoEntity
                          {
                              Id = a.TB_ALMOXARIFADO_ID,
                              Descricao = a.TB_ALMOXARIFADO_DESCRICAO
                          }).ToList();

            List<AlmoxarifadoEntity> list = new List<AlmoxarifadoEntity>();
            foreach (var r in result)
            {
                foreach (var a in almoxarifadosNivelAcesso)
                {
                    if (r.Id == a.Id)
                    {
                        list.Add(r);
                    }
                }
            }

            return list;
        }

        public IList<TB_ALMOXARIFADO> ListarAlmoxarifadoPorGestorMovimentoPendente(int GestorId)
        {
            IQueryable<TB_ALMOXARIFADO> resultado = (from a in Context.TB_ALMOXARIFADO select a);
            var retorno = resultado.Cast<TB_ALMOXARIFADO>().ToList();

            var retornoLista = (from r in retorno
                                where r.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE==true && r.TB_GESTOR_ID==GestorId
                                group r by new
                                {
                                    r.TB_ALMOXARIFADO_ID,
                                    r.TB_ALMOXARIFADO_CODIGO,
                                    r.TB_ALMOXARIFADO_DESCRICAO,
                                } into am
                                select new TB_ALMOXARIFADO()
                                {
                                    TB_ALMOXARIFADO_ID = am.Key.TB_ALMOXARIFADO_ID,
                                    TB_ALMOXARIFADO_CODIGO = am.Key.TB_ALMOXARIFADO_CODIGO,
                                    //TB_ALMOXARIFADO_DESCRICAO = am.Key.TB_ALMOXARIFADO_CODIGO.ToString() + " - " +  am.Key.TB_ALMOXARIFADO_DESCRICAO,
                                    TB_ALMOXARIFADO_DESCRICAO = string.Format("{0} - {1}", am.Key.TB_ALMOXARIFADO_CODIGO.ToString().PadLeft(6, '0'), am.Key.TB_ALMOXARIFADO_DESCRICAO),
                                });

            return retornoLista.OrderBy(a => a.TB_ALMOXARIFADO_CODIGO).Cast<TB_ALMOXARIFADO>().ToList();
        }
        public IList<TB_ALMOXARIFADO> ListarAlmoxarifadoPorUge(int UgeId)
        {

            var retornoLista = (from r in Context.TB_ALMOXARIFADO
                                where r.TB_UGE_ID == UgeId
                                select r
                                //group r by new
                                //{
                                //    r.TB_ALMOXARIFADO_ID,
                                //    r.TB_ALMOXARIFADO_CODIGO,
                                //    r.TB_ALMOXARIFADO_DESCRICAO,
                                //} into am
                                //select new TB_ALMOXARIFADO()
                                //{
                                //    TB_ALMOXARIFADO_ID = am.Key.TB_ALMOXARIFADO_ID,
                                //    TB_ALMOXARIFADO_CODIGO = am.Key.TB_ALMOXARIFADO_CODIGO,
                                //    TB_ALMOXARIFADO_DESCRICAO = string.Format("{0} - {1}", am.Key.TB_ALMOXARIFADO_CODIGO.ToString().PadLeft(6, '0'), am.Key.TB_ALMOXARIFADO_DESCRICAO),
                                ).OrderBy(a => a.TB_ALMOXARIFADO_CODIGO).Cast<TB_ALMOXARIFADO>();

            var retorno = retornoLista.ToList();

            return retorno;
        }
        public IList<TB_ALMOXARIFADO> ListarAlmoxarifadoPorGestor(int GestorId)
        {
            IQueryable<TB_ALMOXARIFADO> resultado = (from a in Context.TB_ALMOXARIFADO select a);
            var retorno = resultado.Cast<TB_ALMOXARIFADO>().ToList();

            var retornoLista = (from r in retorno
                                where r.TB_GESTOR_ID == GestorId
                                group r by new
                                {
                                    r.TB_ALMOXARIFADO_ID,
                                    r.TB_ALMOXARIFADO_CODIGO,
                                    r.TB_ALMOXARIFADO_DESCRICAO,
                                } into am
                                select new TB_ALMOXARIFADO()
                                {
                                    TB_ALMOXARIFADO_ID = am.Key.TB_ALMOXARIFADO_ID,
                                    TB_ALMOXARIFADO_CODIGO = am.Key.TB_ALMOXARIFADO_CODIGO,
                                    TB_ALMOXARIFADO_DESCRICAO = string.Format("{0} - {1}", am.Key.TB_ALMOXARIFADO_CODIGO.ToString().PadLeft(6, '0'), am.Key.TB_ALMOXARIFADO_DESCRICAO),
                                });

            return retornoLista.OrderBy(a => a.TB_ALMOXARIFADO_CODIGO).Cast<TB_ALMOXARIFADO>().ToList();
        }

        public IList<TB_ALMOXARIFADO> ListarAlmoxarifadoAtivoPorGestor(int GestorId)
        {
            IQueryable<TB_ALMOXARIFADO> resultado = (from a in Context.TB_ALMOXARIFADO select a);
            var retorno = resultado.Cast<TB_ALMOXARIFADO>().ToList();

            var retornoLista = (from r in retorno
                                where r.TB_GESTOR_ID == GestorId
                                      && r.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE == true
                                group r by new
                                {
                                    r.TB_ALMOXARIFADO_ID,
                                    r.TB_ALMOXARIFADO_CODIGO,
                                    r.TB_ALMOXARIFADO_DESCRICAO,
                                } into am
                                select new TB_ALMOXARIFADO()
                                {
                                    TB_ALMOXARIFADO_ID = am.Key.TB_ALMOXARIFADO_ID,
                                    TB_ALMOXARIFADO_CODIGO = am.Key.TB_ALMOXARIFADO_CODIGO,
                                    TB_ALMOXARIFADO_DESCRICAO = string.Format("{0} - {1}", am.Key.TB_ALMOXARIFADO_CODIGO.ToString().PadLeft(6, '0'), am.Key.TB_ALMOXARIFADO_DESCRICAO),
                                });

            return retornoLista.OrderBy(a => a.TB_ALMOXARIFADO_CODIGO).Cast<TB_ALMOXARIFADO>().ToList();
        }

        public IList<TB_ALMOXARIFADO> ListarAlmoxarifadoAtivoPorGestor(int GestorId, RepositorioEnum.Repositorio Repositorio)
        {
            IQueryable<TB_ALMOXARIFADO> resultado = (from a in Context.TB_ALMOXARIFADO select a);
            var retorno = resultado.Cast<TB_ALMOXARIFADO>().ToList();

            var retornoLista = (from r in retorno
                                where r.TB_GESTOR_ID == GestorId
                                      && r.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE == true
                                      && r.TB_ALMOXARIFADO_TIPO == Repositorio.ToString()
                                group r by new
                                {
                                    r.TB_ALMOXARIFADO_ID,
                                    r.TB_ALMOXARIFADO_CODIGO,
                                    r.TB_ALMOXARIFADO_DESCRICAO,
                                } into am
                                select new TB_ALMOXARIFADO()
                                {
                                    TB_ALMOXARIFADO_ID = am.Key.TB_ALMOXARIFADO_ID,
                                    TB_ALMOXARIFADO_CODIGO = am.Key.TB_ALMOXARIFADO_CODIGO,
                                    TB_ALMOXARIFADO_DESCRICAO = string.Format("{0} - {1}", am.Key.TB_ALMOXARIFADO_CODIGO.ToString().PadLeft(6, '0'), am.Key.TB_ALMOXARIFADO_DESCRICAO),
                                });

            return retornoLista.OrderBy(a => a.TB_ALMOXARIFADO_CODIGO).Cast<TB_ALMOXARIFADO>().ToList();
        }

        public DataSet GerarExportacaoAlmoxarifadoStatusFechamento(int CodigoOrgao, int IdAlmoxarifado, string anoMesRef)
                                                      
        {
            DataSet dsRetorno = new DataSet();
            try
            {
                SqlConnection conexao = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
                SqlCommand cmd = new SqlCommand("SAM_RETORNA_EXPORTACAO_ALMOXARIFADO ", conexao);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;             
                cmd.Parameters.AddWithValue("@ID_ORGAO", CodigoOrgao);
               // cmd.Parameters.AddWithValue("@ID_ALMOXARIFADO", IdAlmoxarifado);
                cmd.Parameters.AddWithValue("@ANO_MES_REF", anoMesRef);
                string retorno;
                retorno = ConvertCommandParamatersToLiteralValues(cmd);
                SqlDataAdapter daRetorno = new SqlDataAdapter(cmd);
                daRetorno.Fill(dsRetorno);

                return dsRetorno;
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }
        private static string ConvertCommandParamatersToLiteralValues(SqlCommand cmd)
        {
            string query = cmd.CommandText;
            foreach (SqlParameter prm in cmd.Parameters)
            {
                switch (prm.SqlDbType)
                {
                    case SqlDbType.Bit:
                        int boolToInt = (bool)prm.Value ? 1 : 0;
                        query += Convert.ToString((bool)prm.Value ? 1 : 0) + ", ";
                        break;
                    case SqlDbType.Int:
                        query += Convert.ToString(prm.Value) + ", ";
                        break;
                    case SqlDbType.VarChar:
                        query += Convert.ToString(prm.Value) + ", ";
                        break;
                    default:
                        query += Convert.ToString(prm.Value) + ", ";
                        break;
                }
            }
            return query;
        }       
    }
}
