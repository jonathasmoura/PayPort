using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Application.DTOs
{
	public class PaymentResponseDto
	{
		public EReceivedResultDto Result { get; set; }
		public Guid? EventId { get; set; }
		public string Message { get; set; } = string.Empty;
		public IReadOnlyList<string> Errors { get; set; } = Array.Empty<string>();
	}
}
