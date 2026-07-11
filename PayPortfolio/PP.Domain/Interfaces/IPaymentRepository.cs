using PP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Domain.Interfaces
{
	public interface IPaymentRepository
	{
		Task<bool> ExistsTransactionAsync(string idTransaction, CancellationToken ct = default);
		Task IncludeAsync(PaymentWebHookEvent eventPayment, CancellationToken ct = default);
		Task<PaymentWebHookEvent?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
		Task UpdateAsync(PaymentWebHookEvent eventPayment, CancellationToken ct = default);
		Task<IReadOnlyList<PaymentWebHookEvent>> ListLatestsAsync(int quantity, CancellationToken ct = default);
	}
}
