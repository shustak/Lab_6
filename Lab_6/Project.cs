using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace MunicipalManagement
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public decimal Budget { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        static SqlConnection connection;

        static Project()
        {
            var connString = ConfigurationManager.ConnectionStrings["MunicipalConnection"].ConnectionString;
            connection = new SqlConnection(connString);
        }

        public override string ToString()
        {
            return $"id={ProjectId} - name: {ProjectName} - budget: {Budget} - start date: {StartDate} - end date: {(EndDate.HasValue ? EndDate.Value.ToString("d") : "N/A")}";
        }

        public static async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            var projects = new List<Project>();
            var commandString = "SELECT ProjectId, ProjectName, Budget, StartDate, EndDate FROM Projects";
            SqlCommand getAllCommand = new SqlCommand(commandString, connection);

            await connection.OpenAsync();
            var reader = await getAllCommand.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    var project = new Project
                    {
                        ProjectId = reader.GetInt32(0),
                        ProjectName = reader.GetString(1),
                        Budget = reader.GetDecimal(2),
                        StartDate = reader.GetDateTime(3),
                        EndDate = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4)
                    };
                    projects.Add(project);
                }
            }
            await connection.CloseAsync();
            return projects;
        }

        public async Task InsertAsync()
        {
            var commandString = "INSERT INTO Projects (ProjectName, Budget, StartDate, EndDate) VALUES (@name, @budget, @startDate, @endDate)";
            SqlCommand insertCommand = new SqlCommand(commandString, connection);
            insertCommand.Parameters.AddRange(new SqlParameter[]
            {
                new SqlParameter("name", ProjectName),
                new SqlParameter("budget", Budget),
                new SqlParameter("startDate", StartDate),
                new SqlParameter("endDate", (object)EndDate ?? DBNull.Value),
            });
            await connection.OpenAsync();
            await insertCommand.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }

        public async Task UpdateAsync()
        {
            var commandString = "UPDATE Projects SET ProjectName=@name, Budget=@budget, StartDate=@startDate, EndDate=@endDate WHERE(ProjectId=@id)";
            SqlCommand updateCommand = new SqlCommand(commandString, connection);
            updateCommand.Parameters.AddRange(new SqlParameter[]
            {
                new SqlParameter("name", ProjectName),
                new SqlParameter("budget", Budget),
                new SqlParameter("startDate", StartDate),
                new SqlParameter("endDate", (object)EndDate ?? DBNull.Value),
                new SqlParameter("id", ProjectId),
            });
            await connection.OpenAsync();
            await updateCommand.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }

        public static async Task DeleteAsync(int id)
        {
            var commandString = "DELETE FROM Projects WHERE(ProjectId=@id)";
            SqlCommand deleteCommand = new SqlCommand(commandString, connection);
            deleteCommand.Parameters.AddWithValue("id", id);
            await connection.OpenAsync();
            await deleteCommand.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }
    }
}
