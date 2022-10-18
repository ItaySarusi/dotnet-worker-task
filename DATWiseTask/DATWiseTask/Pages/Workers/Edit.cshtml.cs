using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace DATWiseTask.Pages.Workers
{
    public class EditModel : PageModel
    {
        public WorkerInfo workerInfo = new WorkerInfo();
        public String errorMessage = "";
        public String successMessage = "";
        public List<WorkerInfo> WorkerList = new List<WorkerInfo>();
        //allows us to see and use the data of the client
        public void OnGet()
        {
            String id = Request.Query["id"];
            try
            {
                String connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=DATWise;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT * FROM Workers WHERE id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if(reader.Read())
                            {
                                workerInfo.id = "" + reader.GetInt32(0);
                                workerInfo.name = reader.GetString(1);
                                workerInfo.workerId = reader.GetString(2);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }
        //actually updates the data of the worker
        public void OnPost()
        {
            try
            {
                String connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=DATWise;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT * FROM Workers";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                WorkerInfo workerInfo1 = new WorkerInfo();
                                workerInfo1.id = "" + reader.GetInt32(0);
                                workerInfo1.name = reader.GetString(1);
                                workerInfo1.workerId = reader.GetString(2);

                                WorkerList.Add(workerInfo1);
                            }
                        }
                    }
                }
                workerInfo.id = Request.Form["id"];
                workerInfo.name = Request.Form["name"];
                workerInfo.workerId = Request.Form["WorkerId"];
                //Form Validations:
                if (workerInfo.name.Length == 0 || workerInfo.workerId.Length == 0)
                {
                    errorMessage = "All Fields are required";
                    return;
                }
                //if the worker ID is not changed (but already exists in the database for the same worker) allow it...
                if (WorkerList.Any(t => t.workerId == Request.Form["WorkerId"] && t.id == Request.Form["id"]))
                {
                    errorMessage = ""; //not changed...
                }
                //but if the worker ID is changed and exists in the database but for a different worker, don't allow it...
                else if (WorkerList.Any(t => t.workerId == Request.Form["WorkerId"] && t.id != Request.Form["id"]))
                {
                    errorMessage = "WorkerId already exists in the system";
                    return;
                }
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "UPDATE Workers " +
                                  "SET name=@name, workerId=@workerId " +
                                  "WHERE id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", workerInfo.id);
                        command.Parameters.AddWithValue("@name", workerInfo.name);
                        command.Parameters.AddWithValue("@workerId", workerInfo.workerId);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
            Response.Redirect("/Workers/Index");
        }
    }
}
