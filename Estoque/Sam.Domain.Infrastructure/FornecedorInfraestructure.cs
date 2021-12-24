using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sam.Domain.Entity;
using Sam.ServiceInfraestructure;
using Sam.Common.Util;


namespace Sam.Domain.Infrastructure
{
    public partial class FornecedorInfraestructure : BaseInfraestructure, IFornecedorService
    {
        #region ICrudBaseService<FornecedorEntity> Members

        private FornecedorEntity fornecedor = new FornecedorEntity();
        
        public FornecedorEntity Entity
        {
            get { return fornecedor; }

            set { fornecedor = value; }
        }

        public IList<FornecedorEntity> ListarFornecedorPorPalavraChaveTodosCod(string Chave)
        {
            IList<FornecedorEntity> resultado = (from a in this.Db.TB_FORNECEDORs
                                                 where a.TB_FORNECEDOR_CPFCNPJ.Contains(Chave) || a.TB_FORNECEDOR_NOME.Contains(Chave)
                                                 orderby a.TB_FORNECEDOR_NOME
                                                 select new FornecedorEntity
                                                 {
                                                     Id = a.TB_FORNECEDOR_ID,
                                                     CpfCnpj = String.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(a.TB_FORNECEDOR_CPFCNPJ)),
                                                     Nome = a.TB_FORNECEDOR_NOME,
                                                     Logradouro = a.TB_FORNECEDOR_LOGRADOURO,
                                                     Numero = a.TB_FORNECEDOR_NUMERO,
                                                     Complemento = a.TB_FORNECEDOR_COMPLEMENTO,
                                                     Bairro = a.TB_FORNECEDOR_BAIRRO,
                                                     Cep = a.TB_FORNECEDOR_CEP,
                                                     Cidade = a.TB_FORNECEDOR_CIDADE,
                                                     Uf = new UFEntity(a.TB_UF_ID),
                                                     Telefone = a.TB_FORNECEDOR_TELEFONE,
                                                     Fax = a.TB_FORNECEDOR_FAX,
                                                     Email = a.TB_FORNECEDOR_EMAIL,
                                                     InformacoesComplementares = a.TB_FORNECEDOR_INFO_COMPLEMENTAR
                                                 })
                                           .ToList<FornecedorEntity>();

            this.totalregistros = resultado.Count();
            return resultado;
        }

        public IList<FornecedorEntity> ListarFornecedorPorPalavraChave(string Chave)
        {
            IList<FornecedorEntity> resultado = (from a in this.Db.TB_FORNECEDORs
                                                 where a.TB_FORNECEDOR_CPFCNPJ.Contains(Chave) || a.TB_FORNECEDOR_NOME.Contains(Chave)
                                                 orderby a.TB_FORNECEDOR_NOME
                                                 select new FornecedorEntity
                                                 {
                                                     Id = a.TB_FORNECEDOR_ID,
                                                     CpfCnpj = a.TB_FORNECEDOR_CPFCNPJ.Length == 11 ? String.Format(@"{0:000\.000\.000\-00}", long.Parse(a.TB_FORNECEDOR_CPFCNPJ)) : String.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(a.TB_FORNECEDOR_CPFCNPJ)),
                                                     Nome = a.TB_FORNECEDOR_NOME,
                                                     Logradouro = a.TB_FORNECEDOR_LOGRADOURO,
                                                     Numero = a.TB_FORNECEDOR_NUMERO,
                                                     Complemento = a.TB_FORNECEDOR_COMPLEMENTO,
                                                     Bairro = a.TB_FORNECEDOR_BAIRRO,
                                                     Cep = a.TB_FORNECEDOR_CEP,
                                                     Cidade = a.TB_FORNECEDOR_CIDADE,
                                                     Uf = new UFEntity(a.TB_UF_ID),
                                                     Telefone = a.TB_FORNECEDOR_TELEFONE,
                                                     Fax = a.TB_FORNECEDOR_FAX,
                                                     Email = a.TB_FORNECEDOR_EMAIL,
                                                     InformacoesComplementares = a.TB_FORNECEDOR_INFO_COMPLEMENTAR}).Skip(this.SkipRegistros)
                                           .Take(this.RegistrosPagina)
                                           .ToList<FornecedorEntity>();

            this.totalregistros = (from a in this.Db.TB_FORNECEDORs
                                   where a.TB_FORNECEDOR_CPFCNPJ.Contains(Chave) || a.TB_FORNECEDOR_NOME.Contains(Chave)
                                   select new
                                   {
                                       Id = a.TB_FORNECEDOR_ID,
                                   }).Count();
            return resultado;
        }

        public IList<FornecedorEntity> ListarFornecedorPorPalavraChave(string strChavePesquisa, bool blnConsultaOtimizada)
        {
            if (!blnConsultaOtimizada)
                return this.ListarFornecedorPorPalavraChave(strChavePesquisa);

            long   iCNPJ_CPF = 0;
            string lStrSQL   = string.Empty;

            IList<FornecedorEntity>               lstRetorno  = null;
            Expression<Func<TB_FORNECEDOR, bool>> expWhere    = null;
            IQueryable<TB_FORNECEDOR>             qryConsulta = null;

            try
            {
                qryConsulta = (from Fornecedor in this.Db.TB_FORNECEDORs
                               orderby Fornecedor.TB_FORNECEDOR_NOME
                               select Fornecedor).AsQueryable<TB_FORNECEDOR>();

                if (Int64.TryParse(strChavePesquisa, out iCNPJ_CPF))
                {
                    expWhere = (Fornecedor => Fornecedor.TB_FORNECEDOR_CPFCNPJ.Contains(iCNPJ_CPF.ToString()));
                    qryConsulta = qryConsulta.Where(expWhere);
                }
                //else
                else if (iCNPJ_CPF == 0 && !String.IsNullOrWhiteSpace(strChavePesquisa))
                {
                    expWhere = (Fornecedor => Fornecedor.TB_FORNECEDOR_NOME.Contains(strChavePesquisa));
                    qryConsulta = qryConsulta.Where(expWhere);
                }

                lStrSQL = qryConsulta.ToString();
                this.Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => lStrSQL = lStrSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

                lstRetorno = new List<FornecedorEntity>();
                qryConsulta.ToList().ForEach(rowTabela => lstRetorno.Add( new FornecedorEntity
                                                                          {
                                                                              Id                        = rowTabela.TB_FORNECEDOR_ID,
                                                                              CpfCnpj                   = rowTabela.TB_FORNECEDOR_CPFCNPJ.Length == 11 ? String.Format(@"{0:000\.000\.000\-00}", long.Parse(rowTabela.TB_FORNECEDOR_CPFCNPJ.BreakLine('-',0))) : String.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(rowTabela.TB_FORNECEDOR_CPFCNPJ.BreakLine('-',0))),
                                                                              Nome                      = rowTabela.TB_FORNECEDOR_NOME,
                                                                              Logradouro                = rowTabela.TB_FORNECEDOR_LOGRADOURO,
                                                                              Numero                    = rowTabela.TB_FORNECEDOR_NUMERO,
                                                                              Complemento               = rowTabela.TB_FORNECEDOR_COMPLEMENTO,
                                                                              Bairro                    = rowTabela.TB_FORNECEDOR_BAIRRO,
                                                                              Cep                       = rowTabela.TB_FORNECEDOR_CEP,
                                                                              Cidade                    = rowTabela.TB_FORNECEDOR_CIDADE,
                                                                              Uf                        = new UFEntity(rowTabela.TB_UF_ID),
                                                                              Telefone                  = rowTabela.TB_FORNECEDOR_TELEFONE,
                                                                              Fax                       = rowTabela.TB_FORNECEDOR_FAX,
                                                                              Email                     = rowTabela.TB_FORNECEDOR_EMAIL,
                                                                              InformacoesComplementares = rowTabela.TB_FORNECEDOR_INFO_COMPLEMENTAR
                                                                          }));
                this.totalregistros = lstRetorno.Count();
            }
            catch(Exception excErroConsulta)
            {
                Exception excErroParaPropagacao = new Exception("Erro ao obter dados do fornecedor (BD).", excErroConsulta);
                throw excErroParaPropagacao;
            }

            return lstRetorno;
        }

        public IList<FornecedorEntity> Listar()
        {
            IList<FornecedorEntity> resultado = (from a in this.Db.TB_FORNECEDORs
                                                 orderby a.TB_FORNECEDOR_NOME
                                                 select new FornecedorEntity
                                                 {
                                                     Id = a.TB_FORNECEDOR_ID,
                                                     CpfCnpj = a.TB_FORNECEDOR_CPFCNPJ,
                                                     Nome = a.TB_FORNECEDOR_NOME,
                                                     Logradouro = a.TB_FORNECEDOR_LOGRADOURO,
                                                     Numero = a.TB_FORNECEDOR_NUMERO,
                                                     Complemento = a.TB_FORNECEDOR_COMPLEMENTO,
                                                     Bairro = a.TB_FORNECEDOR_BAIRRO,
                                                     Cep = a.TB_FORNECEDOR_CEP,
                                                     Cidade = a.TB_FORNECEDOR_CIDADE,
                                                     Uf = new UFEntity(a.TB_UF_ID),
                                                     Telefone = a.TB_FORNECEDOR_TELEFONE,
                                                     Fax = a.TB_FORNECEDOR_FAX,
                                                     Email = a.TB_FORNECEDOR_EMAIL,
                                                     InformacoesComplementares = a.TB_FORNECEDOR_INFO_COMPLEMENTAR
                                                 }).Skip(this.SkipRegistros)
                                           .Take(this.RegistrosPagina)
                                           .ToList<FornecedorEntity>();

            this.totalregistros = (from a in this.Db.TB_FORNECEDORs
                                   select new
                                   {
                                       Id = a.TB_FORNECEDOR_ID,
                                   }).Count();
            return resultado;
        }

        public IList<FornecedorEntity> Imprimir()
        {
            IList<FornecedorEntity> resultado = (from a in this.Db.TB_FORNECEDORs
                                                 join b in this.Db.TB_UFs on a.TB_UF_ID equals b.TB_UF_ID
                                                 orderby a.TB_FORNECEDOR_NOME
                                                 select new FornecedorEntity
                                                 {
                                                     Id = a.TB_FORNECEDOR_ID,
                                                     CpfCnpj = a.TB_FORNECEDOR_CPFCNPJ,
                                                     Nome = a.TB_FORNECEDOR_NOME,
                                                     Logradouro = a.TB_FORNECEDOR_LOGRADOURO,
                                                     Numero = a.TB_FORNECEDOR_NUMERO,
                                                     Complemento = a.TB_FORNECEDOR_COMPLEMENTO,
                                                     Bairro = a.TB_FORNECEDOR_BAIRRO,
                                                     Cep = a.TB_FORNECEDOR_CEP,
                                                     Cidade = a.TB_FORNECEDOR_CIDADE,
                                                     Uf = new UFEntity { Id = b.TB_UF_ID, Descricao = b.TB_UF_DESCRICAO, Sigla = b.TB_UF_SIGLA },
                                                     Telefone = a.TB_FORNECEDOR_TELEFONE,
                                                     Fax = a.TB_FORNECEDOR_FAX,
                                                     Email = a.TB_FORNECEDOR_EMAIL,
                                                     InformacoesComplementares = a.TB_FORNECEDOR_INFO_COMPLEMENTAR
                                                 }).ToList<FornecedorEntity>();

            return resultado;
        }

        public void Salvar()
        {
            TB_FORNECEDOR tbFornecedor = new TB_FORNECEDOR();

            if (this.Entity.Id.HasValue)
                tbFornecedor = this.Db.TB_FORNECEDORs.Where(a => a.TB_FORNECEDOR_ID == this.Entity.Id.Value).FirstOrDefault();
            else
                this.Db.TB_FORNECEDORs.InsertOnSubmit(tbFornecedor);

            tbFornecedor.TB_FORNECEDOR_CPFCNPJ = this.Entity.CpfCnpj;
            tbFornecedor.TB_FORNECEDOR_NOME = this.Entity.Nome;
            tbFornecedor.TB_FORNECEDOR_LOGRADOURO = this.Entity.Logradouro;
            tbFornecedor.TB_FORNECEDOR_NUMERO = this.Entity.Numero;
            tbFornecedor.TB_FORNECEDOR_COMPLEMENTO = this.Entity.Complemento;
            tbFornecedor.TB_FORNECEDOR_BAIRRO = this.Entity.Bairro;
            tbFornecedor.TB_FORNECEDOR_CEP = this.Entity.Cep;
            tbFornecedor.TB_FORNECEDOR_CIDADE = this.Entity.Cidade;
            tbFornecedor.TB_UF_ID = this.Entity.Uf.Id;
            tbFornecedor.TB_FORNECEDOR_TELEFONE = this.Entity.Telefone;
            tbFornecedor.TB_FORNECEDOR_FAX = this.Entity.Fax;
            tbFornecedor.TB_FORNECEDOR_EMAIL = this.Entity.Email;
            tbFornecedor.TB_FORNECEDOR_INFO_COMPLEMENTAR = this.Entity.InformacoesComplementares;

            this.Db.SubmitChanges();

            this.Entity.Id = tbFornecedor.TB_FORNECEDOR_ID;
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = false;
            if (this.Entity.Id.HasValue)
            {
                retorno = this.Db.TB_FORNECEDORs
                .Where(a => a.TB_FORNECEDOR_CPFCNPJ == this.Entity.CpfCnpj)
                .Where(a => a.TB_FORNECEDOR_ID != this.Entity.Id.Value)
                .Count() > 0;
            }
            else
            {
                retorno = this.Db.TB_FORNECEDORs
                .Where(a => a.TB_FORNECEDOR_CPFCNPJ == this.Entity.CpfCnpj)
                .Count() > 0;
            }
            return retorno;
        }

        public void Excluir()
        {
            TB_FORNECEDOR tbFornercedor
                = this.Db.TB_FORNECEDORs.Where(a => a.TB_FORNECEDOR_ID == this.Entity.Id.Value).FirstOrDefault();
            this.Db.TB_FORNECEDORs.DeleteOnSubmit(tbFornercedor);
            this.Db.SubmitChanges();
        }

        public bool PodeExcluir()
        {
            bool retorno = true;

            TB_FORNECEDOR tbFornecedor = this.Db.TB_FORNECEDORs.Where(a => a.TB_FORNECEDOR_ID == this.Entity.Id.Value).FirstOrDefault();
                        
            return retorno;
        }

        #endregion

        public FornecedorEntity LerRegistro()
        {
            throw new NotImplementedException();
        }

        public FornecedorEntity LerRegistro(int pIntFornecedorID)
        {
            FornecedorEntity lObjRetorno = (from a in this.Db.TB_FORNECEDORs
                                            where a.TB_FORNECEDOR_ID == pIntFornecedorID
                                            select new FornecedorEntity()
                                            {
                                                Id = a.TB_FORNECEDOR_ID,
                                                CpfCnpj = a.TB_FORNECEDOR_CPFCNPJ,
                                                Nome = a.TB_FORNECEDOR_NOME,
                                                Logradouro = a.TB_FORNECEDOR_LOGRADOURO,
                                                Numero = a.TB_FORNECEDOR_NUMERO,
                                                Complemento = a.TB_FORNECEDOR_COMPLEMENTO,
                                                Bairro = a.TB_FORNECEDOR_BAIRRO,
                                                Cep = a.TB_FORNECEDOR_CEP,
                                                Cidade = a.TB_FORNECEDOR_CIDADE,
                                                Uf = new UFEntity(a.TB_UF_ID),
                                                Telefone = a.TB_FORNECEDOR_TELEFONE,
                                                Fax = a.TB_FORNECEDOR_FAX,
                                                Email = a.TB_FORNECEDOR_EMAIL,
                                                InformacoesComplementares = a.TB_FORNECEDOR_INFO_COMPLEMENTAR
                                            }).FirstOrDefault();
            return lObjRetorno;
        }

        public IList<FornecedorEntity> ListarTodosCod()
        {
            IList<FornecedorEntity> resultado = (from a in this.Db.TB_FORNECEDORs
                                                 orderby a.TB_FORNECEDOR_NOME
                                                 select new FornecedorEntity
                                                 {
                                                     Id = a.TB_FORNECEDOR_ID,
                                                     CpfCnpj = string.Format(@"{0:00\.000\.000\/0000\-00}", a.TB_FORNECEDOR_CPFCNPJ),
                                                     Nome = string.Format(@"{0:00\.000\.000\/0000\-00} - {1}", a.TB_FORNECEDOR_CPFCNPJ, a.TB_FORNECEDOR_NOME),
                                                     Logradouro = a.TB_FORNECEDOR_LOGRADOURO,
                                                     Numero = a.TB_FORNECEDOR_NUMERO,
                                                     Complemento = a.TB_FORNECEDOR_COMPLEMENTO,
                                                     Bairro = a.TB_FORNECEDOR_BAIRRO,
                                                     Cep = a.TB_FORNECEDOR_CEP,
                                                     Cidade = a.TB_FORNECEDOR_CIDADE,
                                                     Uf = new UFEntity(a.TB_UF_ID),
                                                     Telefone = a.TB_FORNECEDOR_TELEFONE,
                                                     Fax = a.TB_FORNECEDOR_FAX,
                                                     Email = a.TB_FORNECEDOR_EMAIL,
                                                     InformacoesComplementares = a.TB_FORNECEDOR_INFO_COMPLEMENTAR
                                                 })
                                           .ToList<FornecedorEntity>();
            return resultado;
        }

        public IList<FornecedorEntity> Listar(System.Linq.Expressions.Expression<Func<FornecedorEntity, bool>> where)
        {
            IList<FornecedorEntity> resultado = (from a in this.Db.TB_FORNECEDORs
                                                 orderby a.TB_FORNECEDOR_NOME
                                                 select new FornecedorEntity
                                                 {
                                                     Id = a.TB_FORNECEDOR_ID,
                                                     CpfCnpj = string.Format(@"{0:00\.000\.000\/0000\-00}", a.TB_FORNECEDOR_CPFCNPJ),
                                                     Nome = string.Format(@"{0:00\.000\.000\/0000\-00} - {1}", a.TB_FORNECEDOR_CPFCNPJ, a.TB_FORNECEDOR_NOME),
                                                     Logradouro = a.TB_FORNECEDOR_LOGRADOURO,
                                                     Numero = a.TB_FORNECEDOR_NUMERO,
                                                     Complemento = a.TB_FORNECEDOR_COMPLEMENTO,
                                                     Bairro = a.TB_FORNECEDOR_BAIRRO,
                                                     Cep = a.TB_FORNECEDOR_CEP,
                                                     Cidade = a.TB_FORNECEDOR_CIDADE,
                                                     Uf = new UFEntity(a.TB_UF_ID),
                                                     Telefone = a.TB_FORNECEDOR_TELEFONE,
                                                     Fax = a.TB_FORNECEDOR_FAX,
                                                     Email = a.TB_FORNECEDOR_EMAIL,
                                                     InformacoesComplementares = a.TB_FORNECEDOR_INFO_COMPLEMENTAR
                                                 }).Where(where)
                                           .ToList<FornecedorEntity>();
            return resultado;
        }

        public IList<FornecedorEntity> ListarFornecedorComEmpenhosPendentes(int almoxID, string anoMesRef)
        {
            IList<FornecedorEntity> lstRetorno = null;
            IQueryable<FornecedorEntity> qryConsulta = null;

            //int tipoMovID = (int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho;
            int tipoMovID = (int)GeralEnum.TipoMovimento.EntradaPorEmpenho;

            qryConsulta = (	from fornecedores in this.Db.TB_FORNECEDORs
  						  	join movimentacoes in this.Db.TB_MOVIMENTOs on fornecedores.TB_FORNECEDOR_ID equals movimentacoes.TB_FORNECEDOR_ID into _left_movimentacoes
						 
						 	from _movimentacoes in _left_movimentacoes.DefaultIfEmpty()
							join tipoMov in this.Db.TB_TIPO_MOVIMENTOs on _movimentacoes.TB_TIPO_MOVIMENTO_ID equals tipoMov.TB_TIPO_MOVIMENTO_ID into _left_tipoMov
							
							where (_movimentacoes.TB_ALMOXARIFADO_ID == almoxID)
							where (_movimentacoes.TB_MOVIMENTO_ANO_MES_REFERENCIA == anoMesRef)
							where (_movimentacoes.TB_MOVIMENTO_ATIVO == true)
							where (_movimentacoes.TB_TIPO_MOVIMENTO_ID == tipoMovID)
							where ( (_movimentacoes.TB_MOVIMENTO_EMPENHO != null) || (_movimentacoes.TB_MOVIMENTO_EMPENHO != "") )
							orderby fornecedores.TB_FORNECEDOR_NOME, _movimentacoes.TB_MOVIMENTO_ANO_MES_REFERENCIA, _movimentacoes.TB_MOVIMENTO_EMPENHO
							select new Sam.Domain.Entity.FornecedorEntity
																		{
																			Id = fornecedores.TB_FORNECEDOR_ID,
																			CpfCnpj = string.Format(@"{0:00\.000\.000\/0000\-00}", fornecedores.TB_FORNECEDOR_CPFCNPJ),
																			Nome = string.Format(@"{0:00\.000\.000\/0000\-00} - {1}", fornecedores.TB_FORNECEDOR_CPFCNPJ, fornecedores.TB_FORNECEDOR_NOME),
																			Logradouro = fornecedores.TB_FORNECEDOR_LOGRADOURO,
																			Numero = fornecedores.TB_FORNECEDOR_NUMERO,
																			Complemento = fornecedores.TB_FORNECEDOR_COMPLEMENTO,
																			Bairro = fornecedores.TB_FORNECEDOR_BAIRRO,
																			Cep = fornecedores.TB_FORNECEDOR_CEP,
																			Cidade = fornecedores.TB_FORNECEDOR_CIDADE,
																			Uf = new Sam.Domain.Entity.UFEntity(fornecedores.TB_UF_ID),
																			Telefone = fornecedores.TB_FORNECEDOR_TELEFONE,
																			Fax = fornecedores.TB_FORNECEDOR_FAX,
																			Email = fornecedores.TB_FORNECEDOR_EMAIL,
																			InformacoesComplementares = fornecedores.TB_FORNECEDOR_INFO_COMPLEMENTAR
																		}).AsQueryable();
            
            qryConsulta = qryConsulta.DistinctBy(_fornecedor => _fornecedor.CpfCnpj)
                                     .OrderByDescending(_fornecedor => _fornecedor.Nome)
                                     .AsQueryable();

            lstRetorno = qryConsulta.ToList();


            return lstRetorno;
        }
    }
}
