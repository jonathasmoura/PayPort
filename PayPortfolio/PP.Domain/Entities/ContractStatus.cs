using PP.Domain.Entities.EntityBase;
using System;

namespace PP.Domain.Entities
{
	public class ContractStatus : Base
	{
		public string IdContract { get; private set; } = string.Empty;
		public string CurrentStatus { get; private set; } = string.Empty;
		public decimal AmountPaid { get; private set; }
		public DateTime LastPaymentDate { get; private set; }
		public string LastTransactionId { get; private set; } = string.Empty;

		private ContractStatus() { }

		public static ContractStatus CreateNew(
			string idContract, string status, decimal amount, DateTime paymentDate, string transactionId)
		{
			return new ContractStatus
			{
				IdContract = idContract,
				CurrentStatus = status,
				AmountPaid = amount,
				LastPaymentDate = paymentDate,
				LastTransactionId = transactionId
			};
		}

		public void UpdateWithNewPayment(string status, decimal amount, DateTime paymentDate, string transactionId)
		{
			
			if (paymentDate < LastPaymentDate)
				return;

			CurrentStatus = status;
			AmountPaid = amount;
			LastPaymentDate = paymentDate;
			LastTransactionId = transactionId;
			RegisterUpdate();
		}
	}
}
