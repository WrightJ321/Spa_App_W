using Microsoft.EntityFrameworkCore;

namespace SpaApp_2.Data
{

	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions<DataContext> options) : base(options)
		{

		}
		
	}
}
