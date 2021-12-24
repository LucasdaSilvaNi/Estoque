using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.Configuration;
using Sam.Infrastructure;

namespace Sam.Integracao.SIGEO.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ImportExcel();
        }

        public static void GravarLogErro(Exception ex)
        {
            LogErroInfrastructure infraestrutura = new LogErroInfrastructure();

            TB_LOGERRO logErro = new TB_LOGERRO();
            logErro.TB_LOGERRO_DATA = DateTime.Now;
            logErro.TB_LOGERRO_MESSAGE = ex.Message;
            logErro.TB_LOGERRO_STACKTRACE = ex.StackTrace;

            infraestrutura.Insert(logErro);
            infraestrutura.SaveChanges();
        }

        static void ImportExcel()
        {

            Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(System.Configuration.ConfigurationManager.AppSettings["SIGEO_EXCEL"]);
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;

            int rowCount = xlRange.Rows.Count;
            string conn = System.Configuration.ConfigurationManager.ConnectionStrings["SIGEO_CONN"].ConnectionString;
            StringBuilder sb = new StringBuilder();

            try
            {

                using (SqlConnection openCon = new SqlConnection(conn))
                {
                    using (SqlCommand query = new SqlCommand("truncate table TB_TMP_CARGA_SIGEO"))
                    {
                        query.Connection = openCon;
                        openCon.Open();
                        query.ExecuteNonQuery();
                    }

                }

                for (int i = 2; i <= rowCount; i++)
                {
                    sb.Clear();
                    if (!String.IsNullOrEmpty((string)xlRange.Cells[i, ColumnIndex: 1].Value.Replace("'", "''")))
                    {
                        using (SqlConnection openCon = new SqlConnection(conn))
                        {

                            sb.Append("INSERT INTO TB_TMP_CARGA_SIGEO (");
                            sb.Append(" TB_LINHA"); // A Linha do Linha do Excel não é necessária
                            sb.Append(",TB_ORGAO_CODIGO");
                            sb.Append(",TB_ORGAO_DESCRICAO");
                            sb.Append(",TB_UO_CODIGO");
                            sb.Append(",TB_UO_DESCRICAO");
                            sb.Append(",TB_UGE_CODIGO");
                            sb.Append(",TB_UGE_DESCRICAO");
                            sb.Append(",TB_UA_CODIGO");
                            sb.Append(",TB_UA_DESCRICAO");
                            sb.Append(",NIVEL");
                            sb.Append(",STATUS_UA");
                            sb.Append(",POSICAO_ATUAL ");

                            sb.Append(") VALUES (");

                            sb.Append(i);
                            sb.Append(", " + (string)xlRange.Cells[i, ColumnIndex: 1].Value.Replace("'", "''"));
                            sb.Append(", '" + (string)xlRange.Cells[i, ColumnIndex: 2].Value.Replace("'", "''") + "'");
                            sb.Append(", " + (string)xlRange.Cells[i, ColumnIndex: 3].Value.Replace("'", "''"));
                            sb.Append(", '" + (string)xlRange.Cells[i, ColumnIndex: 4].Value.Replace("'", "''") + "'");
                            sb.Append(", " + (string)xlRange.Cells[i, ColumnIndex: 5].Value.Replace("'", "''"));
                            sb.Append(", '" + (string)xlRange.Cells[i, ColumnIndex: 6].Value.Replace("'", "''") + "'");
                            sb.Append(", " + (string)xlRange.Cells[i, ColumnIndex: 7].Value.Replace("'", "''"));
                            sb.Append(", '" + (string)xlRange.Cells[i, ColumnIndex: 8].Value.Replace("'", "''") + "'");
                            sb.Append(", '" + (string)xlRange.Cells[i, ColumnIndex: 9].Value.Replace("'", "''") + "'");
                            sb.Append(", '" + (string)xlRange.Cells[i, ColumnIndex: 10].Value.Replace("'", "''") + "'");
                            sb.Append(", '" + (string)xlRange.Cells[i, ColumnIndex: 11].Value.Replace("'", "''") + "'");
                            sb.Append(");");

                            using (SqlCommand query = new SqlCommand(sb.ToString()))
                            {
                                query.Connection = openCon;
                                openCon.Open();
                                query.ExecuteNonQuery();

                            }
                        }

                        Console.WriteLine("Inserindo linha: " + i.ToString() + " de: " + rowCount.ToString());
                    }

                }

                Console.WriteLine("Executando Procedures de Carga ");
                using (SqlConnection openCon = new SqlConnection(conn))
                {
                    using (SqlCommand query = new SqlCommand("PROC_TMP_CARGA_SIGEO"))
                    {
                        query.Connection = openCon;
                        openCon.Open();
                        query.CommandType = CommandType.StoredProcedure;
                        query.ExecuteNonQuery();
                    }

                }
            }

            catch (Exception ex)
            {
                GravarLogErro(ex);
            }

            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                Marshal.ReleaseComObject(xlRange);
                Marshal.ReleaseComObject(xlWorksheet);

                xlWorkbook.Close();
                Marshal.ReleaseComObject(xlWorkbook);

                xlApp.Quit();
                Marshal.ReleaseComObject(xlApp);

                sb = null;
            }
        }

    }
}