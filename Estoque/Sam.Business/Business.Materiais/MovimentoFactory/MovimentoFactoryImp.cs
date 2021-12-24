using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;
using Sam.Business.MovimentoFactory;

namespace Sam.Business.MovimentoFactory
{
    internal class MovimentoFactoryImp : IMovimentoFactory
    {
        public void EntrarMaterial(ParametrizacaoEntrada param, MovimentoEntity movimento)
        {
            try
            {
                //if (param.tipoMovimento == Common.Util.GeralEnum.TipoMovimento.AquisicaoCompraEmpenho)
                if (param.tipoMovimento == Common.Util.GeralEnum.TipoMovimento.EntradaPorEmpenho)
                {
                    new MovimentoEmpenho().EntrarMaterial(movimento);
                }
                //else if (param.tipoMovimento == Common.Util.GeralEnum.TipoMovimento.AquisicaoAvulsa)
                else if (param.tipoMovimento == Common.Util.GeralEnum.TipoMovimento.EntradaAvulsa)
                {
                    new MovimentoAvulsa().EntrarMaterial(movimento);
                }
                else if (param.tipoMovimento == Common.Util.GeralEnum.TipoMovimento.EntradaPorDevolucao)
                {
                    new MovimentoDevolucao().EntrarMaterial(movimento);
                }
                else if (param.tipoMovimento == Common.Util.GeralEnum.TipoMovimento.EntradaPorDoacaoImplantado)
                {
                    new MovimentoDoacao().EntrarMaterial(movimento);
                }
                else if (param.tipoMovimento == Common.Util.GeralEnum.TipoMovimento.EntradaPorTransferencia)
                {
                    new MovimentoTransferencia().EntrarMaterial(movimento);
                }
                else if (param.tipoMovimento == Common.Util.GeralEnum.TipoMovimento.EntradaPorMaterialTransformado)
                {
                    new MovimentoTransformado().EntrarMaterial(movimento);
                }
                else
                {
                    throw new Exception("Tipo de movimento inválido");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public void EstornarMaterial(ParametrizacaoEntrada param, MovimentoEntity movimento)
        {
            //if (param.tipoMovimento == Common.Util.GeralEnum.TipoMovimento.AquisicaoCompraEmpenho)
            if (param.tipoMovimento == Common.Util.GeralEnum.TipoMovimento.EntradaPorEmpenho)
            {
                new MovimentoEmpenho().EstornarEntradaMaterial(movimento);
            }
            //else if (param.tipoMovimento == Common.Util.GeralEnum.TipoMovimento.AquisicaoAvulsa)
            else if (param.tipoMovimento == Common.Util.GeralEnum.TipoMovimento.EntradaAvulsa)
            {
                new MovimentoAvulsa().EstornarEntradaMaterial(movimento);
            }
            else if(param.tipoMovimento==Common.Util.GeralEnum.TipoMovimento.EntradaCovid19)
            {
                new MovimentoAvulsa().EstornarEntradaMaterial(movimento);
            }
        }

        public void ValidarMovimento(ParametrizacaoEntrada param, MovimentoEntity movimento)
        {
            throw new NotImplementedException();
        }

        public void SairMaterial(ParametrizacaoEntrada param, MovimentoEntity movimento)
        {
            if (param.tipoMovimento == Common.Util.GeralEnum.TipoMovimento.OutrasSaidas)
            {
                new MovimentoOutras().SairMaterial(movimento);
            }

            else
            {
                throw new Exception("Tipo de movimento inválido");
            }
        }

        public void EstornarSaidaMaterial(ParametrizacaoEntrada param, MovimentoEntity movimento)
        {
            if (param.tipoMovimento == Common.Util.GeralEnum.TipoMovimento.OutrasSaidas)
            {
                new MovimentoOutras().EstornarSaidaMaterial(movimento);
            }

            else
            {
                throw new Exception("Tipo de movimento inválido");
            }
        }

    }
}
