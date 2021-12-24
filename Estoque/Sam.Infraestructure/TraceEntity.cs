using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Configuration;
using System.Diagnostics;

namespace Sam.Infrastructure
{
    public class TraceEntity
    {
        private const string debugSeperator = "-------------------------------------------------------------------------------";

        public static IEnumerable<T> TraceQuery<T>(IEnumerable<T> query)
        {
            if (query != null)
            {
                ObjectQuery<T> objectQuery = query as ObjectQuery<T>;
                if (objectQuery != null && Boolean.Parse(ConfigurationManager.AppSettings["Debugging"]))
                {
                    StringBuilder queryString = new StringBuilder();
                    queryString
                        .AppendLine(debugSeperator)
                        .AppendLine("QUERY GENERATED...")
                        .AppendLine("DATE:" + DateTime.Now.ToString())
                        .AppendLine(debugSeperator)
                        .AppendLine(objectQuery.ToTraceString())
                        .AppendLine(debugSeperator);

                    if (objectQuery.Parameters.Count > 0)
                    {
                        queryString.AppendLine("PARAMETERS...")
                        .AppendLine(debugSeperator);

                        foreach (ObjectParameter parameter in objectQuery.Parameters)
                        {
                            queryString.Append(String.Format("{0}({1}) \t- {2}", parameter.Name, parameter.ParameterType, parameter.Value)).Append(Environment.NewLine);
                        }
                    }
                    Console.WriteLine(queryString);
                    Trace.WriteLine(queryString);
                }
            }
            return query;
        }

        public static IQueryable<T> TraceQuery<T>(IQueryable<T> query)
        {
            if (query != null)
            {
                ObjectQuery<T> objectQuery = query as ObjectQuery<T>;
                if (objectQuery != null && Boolean.Parse(ConfigurationManager.AppSettings["Debugging"]))
                {
                    StringBuilder queryString = new StringBuilder();
                    queryString
                        .AppendLine(debugSeperator)
                        .AppendLine("QUERY GENERATED...")
                        .AppendLine("DATE:" + DateTime.Now.ToString())
                        .AppendLine(debugSeperator)
                        .AppendLine(objectQuery.ToTraceString())
                        .AppendLine(debugSeperator);                        

                    if (objectQuery.Parameters.Count > 0)
                    {
                        queryString.AppendLine("PARAMETERS...")
                        .AppendLine(debugSeperator);

                        foreach (ObjectParameter parameter in objectQuery.Parameters)
                        {
                            queryString.Append(String.Format("{0}({1}) \t- {2}", parameter.Name, parameter.ParameterType, parameter.Value)).Append(Environment.NewLine);
                        }
                    }
                    Console.WriteLine(queryString);
                    Trace.WriteLine(queryString);
                }
            }
            return query;
        }

        public static void TraceCount(string entityName)
        {
            if (!String.IsNullOrEmpty(entityName))
            {
                if (Boolean.Parse(ConfigurationManager.AppSettings["Debugging"]))
                {
                    StringBuilder queryString = new StringBuilder();
                    queryString
                        .AppendLine(debugSeperator)
                        .AppendLine("QUERY GENERATED...")
                        .AppendLine("DATE:" + DateTime.Now.ToString())
                        .AppendLine(debugSeperator)
                        .AppendLine(String.Format("SELECT COUNT(*) FROM {0}", entityName));

                    Console.WriteLine(queryString);
                    Trace.WriteLine(queryString);
                }
            }
        }

        public static void TraceContextStartDateTime()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(debugSeperator);
            sb.AppendLine(String.Format("{0}{1}", "START DATE: ", DateTime.Now.ToString()));            

            Console.WriteLine(sb.ToString());
            Trace.WriteLine(sb.ToString());
        }
    }
}
