using System;
using PP.Domain.Entities;
using PP.Domain.Enums;
using Xunit;

namespace PP.Application.Tests
{
	public class PaymentWebHookEventTests
	{
		[Fact]
		public void Create_WithValidParameters_SetsPropertiesAndStatusReceived()
		{
			var idTransaction = "txn-123";
			var idContract = "ctr-456";
			var amount = 100.50m;
			var paymentDate = new DateTime(2024, 1, 1);
			var receivedStatus = "RECEIVED";
			var payloadRaw = "{\"foo\":\"bar\"}";

			var evt = PaymentWebHookEvent.Create(idTransaction, idContract, amount, paymentDate, receivedStatus, payloadRaw);

			Assert.Equal(idTransaction, evt.IdTransaction);
			Assert.Equal(idContract, evt.IdContract);
			Assert.Equal(amount, evt.Amount);
			Assert.Equal(paymentDate, evt.PaymentDate);
			Assert.Equal(receivedStatus, evt.ReceivedStatus);
			Assert.Equal(payloadRaw, evt.PayloadRaw);
			Assert.Equal(EProcessingStatus.Received, evt.ProcessingStatus);
		}

		[Fact]
		public void Create_WithNullOrWhiteSpaceIdTransaction_ThrowsArgumentException()
		{
			Assert.Throws<ArgumentException>(() => PaymentWebHookEvent.Create("", "ctr", 1m, DateTime.Now, "s", "p"));
		}

		[Fact]
		public void Create_WithNullOrWhiteSpaceIdContract_ThrowsArgumentException()
		{
			Assert.Throws<ArgumentException>(() => PaymentWebHookEvent.Create("txn", "\t", 1m, DateTime.Now, "s", "p"));
		}

		[Fact]
		public void CreateFailed_WithError_SetsFailedStatusAndProcessingError()
		{
			var evt = PaymentWebHookEvent.CreateFailed("txn", "ctr", 10m, DateTime.Now, "s", "p", "some error");

			Assert.Equal(EProcessingStatus.Failed, evt.ProcessingStatus);
			Assert.Equal("some error", evt.ProcessingError);
			Assert.True(evt.ProcessedAt.HasValue);
		}

		[Fact]
		public void CheckInProcess_UpdatesStatusAndUpdatedAt()
		{
			var evt = PaymentWebHookEvent.Create("txn", "ctr", 5m, DateTime.Now, "s", "p");
			Assert.Null(evt.ProcessingError);

			evt.CheckInProcess();

			Assert.Equal(EProcessingStatus.InProcess, evt.ProcessingStatus);
			Assert.True(evt.UpdatedAt.HasValue);
		}

		[Fact]
		public void CheckProcessed_ClearsErrorAndSetsProcessedAt()
		{
			var evt = PaymentWebHookEvent.CreateFailed("txn", "ctr", 5m, DateTime.Now, "s", "p", "err");

			evt.CheckProcessed();

			Assert.Equal(EProcessingStatus.Processed, evt.ProcessingStatus);
			Assert.Null(evt.ProcessingError);
			Assert.True(evt.ProcessedAt.HasValue);
		}

		[Fact]
		public void CheckFailed_SetsErrorAndProcessedAt()
		{
			var evt = PaymentWebHookEvent.Create("txn", "ctr", 5m, DateTime.Now, "s", "p");

			evt.CheckFailed("error-x");

			Assert.Equal(EProcessingStatus.Failed, evt.ProcessingStatus);
			Assert.Equal("error-x", evt.ProcessingError);
			Assert.True(evt.ProcessedAt.HasValue);
		}
	}
}
