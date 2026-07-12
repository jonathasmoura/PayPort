using System;

namespace PP.Application.DTOs
{
	public class ContractStatusDto
	{
		public string IdContract { get; set; } = string.Empty;
		public string CurrentStatus { get; set; } = string.Empty;
		public decimal AmountPaid { get; set; }
		public DateTime LastPaymentDate { get; set; }
		public string LastTransactionId { get; set; } = string.Empty;
		public bool IsActive { get; set; }
		public DateTime? UpdatedAt { get; set; }
	}
}
