using PP.Domain.Entities.EntityBase;
using PP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Domain.Entities
{
	public class PaymentWebHookEvent : Base
	{
		public string IdTransacao { get; private set; } = string.Empty;
		public string IdContrato { get; private set; } = string.Empty;
		public decimal Valor { get; private set; }
		public DateTime DataPagamento { get; private set; }
		public string StatusRecebido { get; private set; } = string.Empty;

		public string PayloadRaw { get; private set; } = string.Empty;

		public DateTime? ProcessadoEm { get; private set; }

		public EProcessingStatus? ProcessingStatus { get; private set; }

		public string? ErroProcessamento { get; private set; }

		private PaymentWebHookEvent() { }

		public static PaymentWebHookEvent Criar(
			string idTransacao,
			string idContrato,
			decimal valor,
			DateTime dataPagamento,
			string statusRecebido,
			string payloadRaw)
		{
			if (string.IsNullOrWhiteSpace(idTransacao))
				throw new ArgumentException("id_transacao é obrigatório.", nameof(idTransacao));
			if (string.IsNullOrWhiteSpace(idContrato))
				throw new ArgumentException("id_contrato é obrigatório.", nameof(idContrato));

			return new PaymentWebHookEvent
			{
				IdTransacao = idTransacao,
				IdContrato = idContrato,
				Valor = valor,
				DataPagamento = dataPagamento,
				StatusRecebido = statusRecebido,
				PayloadRaw = payloadRaw,
				ProcessingStatus = EProcessingStatus.Recebido
			};
		}

		public void MarcarEmProcessamento()
		{
			ProcessingStatus = EProcessingStatus.EmProcessamento;
			RegistrarAtualizacao();
		}

		public void MarcarProcessado()
		{
			ProcessingStatus = EProcessingStatus.Processado;
			ProcessadoEm = DateTime.UtcNow;
			ErroProcessamento = null;
			RegistrarAtualizacao();
		}

		public void MarcarFalha(string erro)
		{
			ProcessingStatus = EProcessingStatus.Falhou;
			ProcessadoEm = DateTime.UtcNow;
			ErroProcessamento = erro;
			RegistrarAtualizacao();
		}
	}
}
