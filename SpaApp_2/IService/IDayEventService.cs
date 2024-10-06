using SpaApp_2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace SpaApp_2.IService
{
	public interface IDayEventService
	{
		DayEvent SaveOrUpdate(DayEvent odayEvent);
		DayEvent GetEvent(DateTime eventDate);

		List<DayEvent> GetEvents(DateTime fromDate, DateTime toDate);
		string Delete(int id);


	}
}
