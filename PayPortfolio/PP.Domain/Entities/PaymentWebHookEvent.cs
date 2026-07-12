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
		public string IdTransaction { get; private set; } = string.Empty;
		public string IdContract { get; private set; } = string.Empty;
		public decimal Amount { get; private set; }
		public DateTime PaymentDate { get; private set; }
		public string ReceivedStatus { get; private set; } = string.Empty;

		public string PayloadRaw { get; private set; } = string.Empty;

		public DateTime? ProcessedAt { get; private set; }

		public EProcessingStatus? ProcessingStatus { get; private set; }

		public string? ProcessingError { get; private set; }

		private PaymentWebHookEvent() { }

		public static PaymentWebHookEvent Create(
			string idTransaction,
			string idContract,
			decimal amount,
			DateTime paymentDate,
			string receivedStatus,
			string payloadRaw)
		{
			if (string.IsNullOrWhiteSpace(idTransaction))
				throw new ArgumentException("id_transaction é obrigatório.", nameof(idTransaction));
			if (string.IsNullOrWhiteSpace(idContract))
				throw new ArgumentException("id_contract é obrigatório.", nameof(idContract));

			return new PaymentWebHookEvent
			{
				IdTransaction = idTransaction,
				IdContract = idContract,
				Amount = amount,
				PaymentDate = paymentDate,
				ReceivedStatus = receivedStatus,
				PayloadRaw = payloadRaw,
				ProcessingStatus = EProcessingStatus.Received
			};
		}

		public void CheckInProcess()
		{
			ProcessingStatus = EProcessingStatus.InProcess;
			RegisterUpdate();
		}

		public void CheckProcessed()
		{
			ProcessingStatus = EProcessingStatus.Processed;
			ProcessedAt = DateTime.UtcNow;
			ProcessingError = null;
			RegisterUpdate();
		}

		public void CheckFailed(string error)
		{
			ProcessingStatus = EProcessingStatus.Failed;
			ProcessedAt = DateTime.UtcNow;
			ProcessingError = error;
			RegisterUpdate();
		}
	}
}
