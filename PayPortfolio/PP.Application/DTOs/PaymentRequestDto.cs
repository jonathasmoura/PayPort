using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PP.Application.DTOs
{
	public class PaymentRequestDto
	{
		[JsonPropertyName("id_transaction")]
		public string IdTransaction { get; set; } = string.Empty;

		[JsonPropertyName("id_contract")]
		public string IdContract { get; set; } = string.Empty;

		[JsonPropertyName("amount")]
		public decimal Amount { get; set; }

		[JsonPropertyName("payment_date")]
		public DateTime PaymentDate { get; set; }

		[JsonPropertyName("status")]
		public string Status { get; set; } = string.Empty;
	}
}
