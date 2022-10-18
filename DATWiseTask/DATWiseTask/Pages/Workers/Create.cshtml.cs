using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace DATWiseTask.Pages.Workers
{
    public class CreateModel : PageModel
    {
        public WorkerInfo workerInfo = new WorkerInfo();
        public String errorMessage = "";
        public List<WorkerInfo> WorkerList = new List<WorkerInfo>();
        public void OnGet()
        {

        }
        public void OnPost()
        {
            //Connecting to the Database
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
                            WorkerInfo workerInfo = new WorkerInfo();
                            workerInfo.id = "" + reader.GetInt32(0); //Add "" so it will be converted in .NET into a string but in the DB it will be an integer
                            workerInfo.name = reader.GetString(1);
                            workerInfo.workerId = "" + reader.GetString(2); //Add "" so it will be converted in .NET into a string but in the DB it will be an integer

                            WorkerList.Add(workerInfo);
                        }
                    }
                }
            }
            //Finished connecting
            workerInfo.name = Request.Form["Name"];
            workerInfo.workerId = Request.Form["WorkerId"];
            //Form Validations:
            if (WorkerList.Any(t => t.workerId == Request.Form["WorkerId"]))
            {
                errorMessage = "WorkerId already exists in the system";
                return;
            }
            if (workerInfo.name.Length == 0 || workerInfo.workerId.Length == 0)
            {
                errorMessage = "All Fields are required";
                return;
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "INSERT INTO Workers " +
                          "(name, workerId) VALUES " +
                          "(@name, @workerId);";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
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

            //Send Email to Barak:
            string body = "New worker name: " + workerInfo.name + "\nNew worker ID: " + workerInfo.workerId;
            string fromMail = "itaysarusi67@gmail.com";
            string ToMail = "itaysarusi67@gmail.com"; //Switch to Barak's Email --> barak@datwise.com
            string FromPassword = "jkdyhipewwazirjs";
            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = "New worker added to the system !";
            message.Body = body;
            message.To.Add(new MailAddress(ToMail));

            SmtpClient smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, FromPassword),
                EnableSsl = true
            };
            smtp.Send(message);

            //if everything is ok:
            workerInfo.name = ""; workerInfo.workerId = "";
            //Redirect back to workers list:
            Response.Redirect("/Workers/Index");
        }
        
    }
}
