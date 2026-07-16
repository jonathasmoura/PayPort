using System;

namespace PP.Application.DTOs
{
	public class EventLogDto
	{
		public Guid Id { get; set; }
		public string IdTransaction { get; set; } = string.Empty;
		public string IdContract { get; set; } = string.Empty;
		public decimal Amount { get; set; }
		public string ReceivedStatus { get; set; } = string.Empty;
		public string ProcessingStatus { get; set; } = string.Empty;
		public bool IsActive { get; set; }
		public DateTime ReceivedAt { get; set; }
		public DateTime? ProcessedAt { get; set; }
		public string? ProcessingError { get; set; }
	}
}
