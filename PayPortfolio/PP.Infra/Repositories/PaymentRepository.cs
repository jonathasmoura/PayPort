using Microsoft.EntityFrameworkCore;
using PP.Domain.Entities;
using PP.Domain.Exceptions;
using PP.Domain.Interfaces;
using PP.Infra.DataContexts;
using PP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Infra.Repositories
{
	public class PaymentRepository : IPaymentRepository
	{
		private readonly DbPayContext _context;

		public PaymentRepository(DbPayContext context)
		{
			_context = context;
		}

		public async Task<bool> ExistsTransactionAsync(string idTransaction, CancellationToken ct = default)
		{
			return await _context.PaymentWebHookEvents
				.IgnoreQueryFilters()
				.AsNoTracking()
				.AnyAsync(e => e.IdTransaction == idTransaction, ct);
		}

		public async Task IncludeAsync(PaymentWebHookEvent eventPayment, CancellationToken ct = default)
		{
			_context.PaymentWebHookEvents.Add(eventPayment);

			try
			{
				await _context.SaveChangesAsync(ct);
			}
			catch (DbUpdateException ex) when (ItsUniqueKeyViolation(ex))
			{
				_context.Entry(eventPayment).State = EntityState.Detached;
				throw new DuplicateTransactionException(eventPayment.IdTransaction);
			}
		}

		public async Task<IReadOnlyList<PaymentWebHookEvent>> ListLatestsAsync(int quantity, EProcessingStatus? processingStatusFilter = null, CancellationToken ct = default)
		{
			var query = _context.PaymentWebHookEvents
				.IgnoreQueryFilters()
				.AsNoTracking()
				.AsQueryable();

			if (processingStatusFilter.HasValue)
			{
				query = query.Where(e => e.ProcessingStatus == processingStatusFilter.Value);
			}

			return await query
				.OrderByDescending(e => e.Created)
				.Take(quantity)
				.ToListAsync(ct);
		}

		public async Task<PaymentWebHookEvent?> GetByIdAsync(Guid id, CancellationToken ct = default)
		{
			return await _context.PaymentWebHookEvents
				.IgnoreQueryFilters()
				.FirstOrDefaultAsync(e => e.Id == id, ct);

		}

		public async Task UpdateAsync(PaymentWebHookEvent eventPayment, CancellationToken ct = default)
		{
			_context.PaymentWebHookEvents.Update(eventPayment);
			await _context.SaveChangesAsync(ct);
		}

		private static bool ItsUniqueKeyViolation(DbUpdateException ex)
		{
			return ex.InnerException?.Message.Contains("UX_PaymentWebHookEvents_IdTransaction") == true;
		}
	}
}
