using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Sam.Common.Util;
using Sam.Configuracao;
using Sam.Domain.Entity;
using Sam.Domain.Infrastructure;

namespace Sam.Common.EMail.Core
{
    public class DespachanteEMail
    {
        private const string CST_MSG_ERRO_FALHA_ENVIO_EMAIL_HOST_AUSENTE = "Nome de host do servidor se encontra ausente!";
        private const string CST_MSG_ERRO_FALHA_ENVIO_EMAIL_SERVICO_SMTP_INDISPONIVEL = "Serviço de SMTP indisponível.";
        private const string CST_MSG_ERRO_FALHA_ENVIO_EMAIL_DESTINO_OCUPADO = "A caixa de correio de destino está em uso.";
        private const string CST_MSG_ERRO_FALHA_ENVIO_EMAIL_SEM_PERMISSAO = "O cliente não foi autenticado ou não tem permissão para enviar emails.";
        private const string CST_MSG_ERRO_FALHA_ENVIO_EMAIL_PARA_DESTINATARIO = "A mensagem não pode ser entregue para um ou mais destinatários.";
        private const string CST_MSG_ERRO_FALHA_ENVIO_EMAIL_DESTINO_COM_ESPACO_INSUFICIENTE = "A mensagem é muito grande para ser armazenada na caixa de correio de destino.";
        private const string CST_MSG_ERRO_MENSAGEM_EMAIL_NAO_GERADA = "Mensagem de e-mail não-gerada!";
        private const string CST_MSG_ERRO_SERVIDOR_EMAIL_NAO_INICIALIZADO = "Servidor E-Mail não inicializado!";
        private const string CST_MSG_ERRO_FALHA_ENVIO_EMAIL = "Envio não Realizado.";
        private const string CST_MSG_EMAIL_ENVIADO_SUCESSO = "E-Mail enviado com sucesso";


        byte[] arrByte = null;
        MemoryStream memStream = null;

        MailMessage msgEMail = null;
        MailAddress enderecoRemetente = null;
        SmtpClient ServidorEMail = null;

        public void MontarMensagem(string enderecoEmailRemetente, IEnumerable<string> enderecosEMailDestinatarios, IEnumerable<string> enderecosEMailEmCopia, IEnumerable<string> enderecosEMailEmCopiaOculta, string assuntoMensagem, string conteudoMensagem, IEnumerable<AnexoChamadoSuporte> anexosMensagem, bool ehMensagemHTML = false)
        {
            msgEMail = new MailMessage();
            enderecoRemetente = new MailAddress(enderecoEmailRemetente);

            msgEMail.Sender = enderecoRemetente;
            msgEMail.From = this.enderecoRemetente;
            msgEMail.ReplyToList.Add(new MailAddress("(e-mail do suporte)", "(nome do usuario do e-mail do suporte)"));

            if (enderecosEMailDestinatarios.HasElements())
                enderecosEMailDestinatarios.ToList().ForEach(destinatarioMsg => msgEMail.To.Add(destinatarioMsg));

            if (enderecosEMailEmCopia.HasElements())
                enderecosEMailEmCopia.ToList().ForEach(destinatarioEmCopia => msgEMail.CC.Add(destinatarioEmCopia));

            if (enderecosEMailEmCopiaOculta.HasElements())
                enderecosEMailEmCopiaOculta.ToList().ForEach(destinatarioEmCopiaOculta => msgEMail.Bcc.Add(destinatarioEmCopiaOculta));

            if (anexosMensagem.HasElements())
                anexosMensagem.ToList().ForEach(anexoMsg =>
                                                            {
                                                                arrByte = new byte[anexoMsg.ConteudoArquivo.Length];
                                                                memStream = new MemoryStream(arrByte);
                                                                memStream.Write(arrByte, 0, arrByte.Length);
                                                                msgEMail.Attachments.Add(new Attachment(memStream, anexoMsg.NomeArquivo, anexoMsg.ContentType));

                                                                arrByte = null;
                                                                memStream = null;
                                                            });

            msgEMail.Subject = assuntoMensagem;
            msgEMail.Body = conteudoMensagem;
            msgEMail.IsBodyHtml = ehMensagemHTML;
            msgEMail.BodyEncoding = UTF8Encoding.UTF8;
            msgEMail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
        }

        public void EnviarMensagem()
        {
            string msgErro = null;
            string msgErroDetalhada = null;
            var logErroInfra = new LogErroInfraestructure();

            msgEMail.Headers.Add("Read-Receipt-To", msgEMail.From.Address);
            msgEMail.Headers.Add("Disposition-Notification-To", msgEMail.From.Address);

            try
            {
                if (msgEMail.IsNull())
                    throw new Exception(CST_MSG_ERRO_MENSAGEM_EMAIL_NAO_GERADA);

                ServidorEMail = ConfiguracaoEMail.ObterServidorEnvioEMail();

                if (ServidorEMail.IsNull())
                    throw new Exception(CST_MSG_ERRO_SERVIDOR_EMAIL_NAO_INICIALIZADO);

                //Envio do email
                ServidorEMail.Send(msgEMail);
            }
            catch (InvalidOperationException ex)
            {
                msgErroDetalhada = new LogErroInfraestructure().MontaMsgCompletaErro(ex);
                msgErro = CST_MSG_ERRO_FALHA_ENVIO_EMAIL_HOST_AUSENTE;
            }
            catch (SmtpException smtpException)
            {
                switch (smtpException.StatusCode)
                {
                    case SmtpStatusCode.ServiceNotAvailable:
                        msgErro = CST_MSG_ERRO_FALHA_ENVIO_EMAIL_SERVICO_SMTP_INDISPONIVEL;
                        break;
                    case SmtpStatusCode.MailboxBusy:
                        msgErro = CST_MSG_ERRO_FALHA_ENVIO_EMAIL_DESTINO_OCUPADO;
                        break;
                    case SmtpStatusCode.ClientNotPermitted:
                        msgErro = CST_MSG_ERRO_FALHA_ENVIO_EMAIL_SEM_PERMISSAO;
                        break;
                    case SmtpStatusCode.MailboxUnavailable:
                        msgErro = CST_MSG_ERRO_FALHA_ENVIO_EMAIL_PARA_DESTINATARIO;
                        break;
                    case SmtpStatusCode.ExceededStorageAllocation:
                        msgErro = CST_MSG_ERRO_FALHA_ENVIO_EMAIL_DESTINO_COM_ESPACO_INSUFICIENTE;
                        break;
                    default:
                        msgErro = CST_MSG_ERRO_FALHA_ENVIO_EMAIL;
                        break;
                }

                msgErroDetalhada = new LogErroInfraestructure().MontaMsgCompletaErro(smtpException);
            }
            catch (Exception ex)
            {
                msgErroDetalhada = new LogErroInfraestructure().MontaMsgCompletaErro(ex);
                msgErro = CST_MSG_ERRO_FALHA_ENVIO_EMAIL;
            }

            if (msgErroDetalhada.IsNotNullAndNotEmpty())
            {
                logErroInfra.GravarMsgInfo("Disparo de Email", msgErroDetalhada);
                throw new Exception(msgErro);
            }
        }

        private void initServidorEnvioEMail()
        { ServidorEMail = ConfiguracaoEMail.ObterServidorEnvioEMail(); }

    }
}
