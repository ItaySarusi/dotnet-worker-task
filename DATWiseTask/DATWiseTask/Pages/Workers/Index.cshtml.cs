using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace DATWiseTask.Pages.Workers
{
    public class IndexModel : PageModel
    {
        public List<WorkerInfo> WorkerList = new List<WorkerInfo>();
        public void OnGet()
        {
            try
            {
                String connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=DATWise;Integrated Security=True";

                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT * FROM Workers";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using(SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                WorkerInfo workerInfo = new WorkerInfo();
                                workerInfo.id = "" + reader.GetInt32(0); //Add "" so it will be converted in .NET into a string but in the DB it will be an integer
                                workerInfo.name = reader.GetString(1);
                                workerInfo.workerId = reader.GetString(2);
                                
                                WorkerList.Add(workerInfo);
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }
    }
    public class WorkerInfo
    {
        public String ?id { get; set; }
        public String ?name { get; set; }
        public String ?workerId { get; set; }
    }
}
