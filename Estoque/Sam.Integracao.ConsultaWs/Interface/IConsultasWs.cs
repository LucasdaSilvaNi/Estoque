
using System.ServiceModel;
using System.ServiceModel.Web;
using Sam.Integracao.ConsultaWs.Parametro;



namespace Sam.Integracao.ConsultaWs.Interface
{
    [ServiceContract]
    public interface IConsultasWs
    {
        // GeraRelacaoMovimentacaoMaterialWs 
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, UriTemplate = "GeraRelacaoMovimentacaoMaterialWs")]
        //string GeraRelacaoMovimentacaoMaterialWs(ConsultaSubitemWs item);
        string GeraRelacaoMovimentacaoMaterialWs(DadosRelatorioWs dadosRelatorio);

        [WebInvoke(Method = "OPTIONS", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, UriTemplate = "GeraRelacaoMovimentacaoMaterialWs")]
        //string GeraRelacaoMovimentacaoMaterialWsOptions(ConsultaSubitemWs item);
        string GeraRelacaoMovimentacaoMaterialWsOptions(DadosRelatorioWs dadosRelatorio);

        // BuscaSubItemMaterialRequisicaoParaWs
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, UriTemplate = "BuscaSubItemMaterialRequisicaoParaWs")]
        string BuscaSubItemMaterialRequisicaoParaWs(ConsultaSubitemWs item);


        [WebInvoke(Method = "OPTIONS", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, UriTemplate = "BuscaSubItemMaterialRequisicaoParaWs")]
        string BuscaSubItemMaterialRequisicaoParaWsOptions(ConsultaSubitemWs item);

        // CriaRequisicaoMaterial
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, UriTemplate = "CriaRequisicaoMaterialWs")]
        string CriaRequisicaoMaterialWs(DadosMovimentacaoWs dadosMovimentacaoWs);


        [WebInvoke(Method = "OPTIONS", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, UriTemplate = "CriaRequisicaoMaterialWsOptions")]
        string CriaRequisicaoMaterialWsOptions(DadosMovimentacaoWs dadosMovimentacaoWs);
    }
}
