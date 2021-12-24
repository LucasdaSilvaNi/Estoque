using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;
using Sam.Domain.Entity.Relatorios;
using System.Data;

namespace Sam.ServiceInfraestructure
{
    public interface IFechamentoMensalService: ICatalogoBaseService,ICrudBaseService<FechamentoMensalEntity>
    {
        void Salvar(IList<FechamentoMensalEntity> ListaFechamento);
        void Salvar(IList<FechamentoMensalEntity> pIListaFechamentos, int pIntAlmoxarifadoID, int pIntAnoMesReferencia);

        IList<FechamentoMensalEntity> Listar(int pIntAlmoxarifadoID, bool agruparFechamentos);
        IList<string> ListarMesesFechados(int almoxId);
        void AtualizarMesRefAlmoxarifadoFechamento(int almoxarifadoId, string AnoMesRef);
        int? ListarUltimoFechamento(int? almoxarifadoId);
        bool ContemFechamento(int almoxarifadoId);
        AlmoxarifadoEntity ObterAlmoxarifado(int? idAlmoxarifado);
        IList<SubItemMaterialEntity> VerificarSubitensInativos(int almoxId);
        List<FechamentoAnualEntity> GerarBalanceteAnual(int idAlmoxarifado, string mesrefAnoAnterior, string mesRefInicial, string mesRefFinal);
        IList<relInventarioFechamentoMensalEntity> _xpImprimirInventarioBalanceteMensal(int almoxID, int anoMesRef);
        IList<relAnaliticoFechamentoMensalEntity> ImprimirAnaliticoBalanceteMensal(int almoxID, int anoMesRef);

        void ExecutarFechamento(Int32 AlmoxarifadoId, Int32 mesAnoReferencia, int usuarioSamLoginId);
        void ExecutarSimulacao(Int32 AlmoxarifadoId, Int32 mesAnoReferencia, int usuarioSamLoginId);
        IList<String> fechamentoErro { get; set; }
        AlmoxarifadoEntity ListarStatusAlmoxarifado(int? almoxId);
    }
}
