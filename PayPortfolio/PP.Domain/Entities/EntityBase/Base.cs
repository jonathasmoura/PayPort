using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Domain.Entities.EntityBase
{
	public class Base
	{
		public Base()
		{
			Id = Guid.NewGuid();
			IsActive = true;
			Created = DateTime.UtcNow;
			ActivationDate = Created;
		}

		public Guid Id { get; set; }
		public bool IsActive { get; set; }
		public DateTime? ActivationDate { get; set; }
		public DateTime? InactivationDate { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public DateTime Created { get; set; }
	}
}
