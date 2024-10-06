using Dapper;
using Microsoft.Data.SqlClient;
using SpaApp_2.Data;
using SpaApp_2.IService;
using System.Data;

namespace SpaApp_2.Service
{
	public class DayEventService : IDayEventService
	{
		DayEvent _oDayEvent = new DayEvent();
		List<DayEvent> _oDayEvents = new List<DayEvent>();
		public IConfiguration Configuration { get; set; }
		public DayEventService(IConfiguration configuration)
		{
			Configuration = configuration;
		}
		public string Delete(int id)
		{
			string message = "";
			try
			{
				_oDayEvent = new DayEvent()
				{
					DayEventId = id

				};
				using (IDbConnection connect = new SqlConnection(Configuration.GetConnectionString("SpaDb")))
				{
					if (connect.State == ConnectionState.Closed)
					{
						connect.Open();
					}
					var oDayEvents = connect.Query<DayEvent>("SP_DayEvent", this.SetParameters(_oDayEvent, Convert.ToInt32(OperationType.Delete)), commandType: CommandType.StoredProcedure);

					message = "Deleted";

				}



			}
			catch (Exception ex)
			{
				message = ex.Message;
			}
			return message;
		}

		public DayEvent GetEvent(DateTime eventDate)
		{
			_oDayEvent = new DayEvent();
			using (IDbConnection connect = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
			{
				if (connect.State == ConnectionState.Closed)
				{
					connect.Open();
				}
				string sql = string.Format("SELECT * FROM DayEvent WHERE EventDate = '{0}'", eventDate.ToString("dd-MMM-yyyy"));

				var oDayEvents = connect.Query<DayEvent>(sql).ToList();

				if (oDayEvents != null && oDayEvents.Count() > 0)
				{
					_oDayEvent = oDayEvents.SingleOrDefault();
				}
				else
				{
					_oDayEvent.EventDate = eventDate;
					_oDayEvent.FromDate = eventDate;
					_oDayEvent.ToDate = eventDate;
				}

			}
			return _oDayEvent;
		}

		public List<DayEvent> GetEvents(DateTime fromDate, DateTime toDate)
		{
			_oDayEvents = new List<DayEvent>();
			using (IDbConnection connect = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
			{
				if (connect.State == ConnectionState.Closed)
				{
					connect.Open();
				}
				string sql = string.Format("SELECT * FROM DayEvent WHERE EventDate BETWEEN '{0}' AND '{1}'", fromDate.ToString("dd-MMM-yyyy"), toDate.ToString("dd-MMM-yyyy"));

				var oDayEvents = connect.Query<DayEvent>(sql).ToList();
				if (oDayEvents != null && oDayEvents.Count() > 0)
				{
					_oDayEvents = oDayEvents;
				}

			}
			return _oDayEvents;
		}

		public DayEvent SaveOrUpdate(DayEvent oDayEvent)
		{
			_oDayEvent = new DayEvent();
			try
			{
				int operationType = Convert.ToInt32(oDayEvent.DayEventId == 0 ? OperationType.Insert : OperationType.Update);

				using (IDbConnection connect = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
				{
					if (connect.State == ConnectionState.Closed)
					{
						connect.Open();

					}
					var oDayEvents = connect.Query<DayEvent>("SP_DayEvent", this.SetParameters(oDayEvent, operationType), commandType: CommandType.StoredProcedure);
					if (oDayEvents != null && oDayEvents.Count() > 0)
					{
						_oDayEvent = oDayEvents.FirstOrDefault();
					}

				}


			}
			catch (Exception ex)
			{
				_oDayEvent.Message = ex.Message;
			}
			return _oDayEvent;

		}
		private DynamicParameters SetParameters(DayEvent oDayEvent, int operationType)
		{
			DynamicParameters parameters = new DynamicParameters();
			parameters.Add("@DayEventId", oDayEvent.DayEventId);
			parameters.Add("FirstName", oDayEvent.FirstName);
			parameters.Add("LastName", oDayEvent.LastName);
			parameters.Add("Phone", oDayEvent.Phone);
			parameters.Add("Note", oDayEvent.Note);
			parameters.Add("@EventDate", oDayEvent.EventDate);
			parameters.Add("@OperationType", operationType);
			return parameters;
		}
	}
}