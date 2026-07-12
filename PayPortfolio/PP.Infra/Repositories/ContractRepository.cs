using Microsoft.EntityFrameworkCore;
using PP.Domain.Entities;
using PP.Domain.Interfaces;
using PP.Infra.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Infra.Repositories
{
	public class ContractRepository : IContractRepository
	{
		private readonly DbPayContext _context;

		public ContractRepository(DbPayContext context)
		{
			_context = context;
		}

		public async Task<IReadOnlyList<ContractStatus>> GetAllAsync(CancellationToken ct = default)
		{
			return await _context.ContractsStatus
				.AsNoTracking()
				.OrderByDescending(c => c.UpdatedAt ?? c.Created)
				.ToListAsync(ct);
		}

		public async Task<ContractStatus?> GetByContractAsync(string idContract, CancellationToken ct = default)
		{
			return await _context.ContractsStatus
				.IgnoreQueryFilters()
				.FirstOrDefaultAsync(c => c.IdContract == idContract, ct);
		}

		public async Task IncludeAsync(ContractStatus status, CancellationToken ct = default)
		{
			_context.ContractsStatus.Add(status);
			await _context.SaveChangesAsync(ct);
		}

		public async Task UpdateAsync(ContractStatus status, CancellationToken ct = default)
		{
			_context.ContractsStatus.Update(status);
			await _context.SaveChangesAsync(ct);
		}
	}
}
