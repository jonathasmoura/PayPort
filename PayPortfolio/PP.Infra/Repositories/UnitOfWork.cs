using PP.Domain.Interfaces;
using PP.Infra.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Infra.Repositories
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly DbPayContext _context;

		public UnitOfWork(DbPayContext context)
		{
			_context = context;
		}

		public Task<int> SaveAsync(CancellationToken ct = default) => _context.SaveChangesAsync(ct);
		
	}
}
