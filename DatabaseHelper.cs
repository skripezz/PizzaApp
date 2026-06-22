using System;
using System.Data;
using System.Data.SqlClient;

namespace PizzaApp.Helpers
{
    public static class DatabaseHelper
    {
        private static string connectionString = 
            @"Data Source=localhost;Initial Catalog=PizzaProduction;Integrated Security=True";

        public static DataTable ExecuteQuery(string query)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        public static int ExecuteNonQuery(string query)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static bool ValidateUser(string login, string password)
        {
            string query = $"SELECT * FROM Пользователи WHERE логин='{login}' AND пароль='{password}' AND заблокирован=0";
            DataTable dt = ExecuteQuery(query);
            return dt.Rows.Count > 0;
        }

        public static string GetUserRole(string login)
        {
            string query = $"SELECT роль FROM Пользователи WHERE логин='{login}'";
            DataTable dt = ExecuteQuery(query);
            return dt.Rows.Count > 0 ? dt.Rows[0]["роль"].ToString() : null;
        }

        public static bool IsUserBlocked(string login)
        {
            string query = $"SELECT заблокирован FROM Пользователи WHERE логин='{login}'";
            DataTable dt = ExecuteQuery(query);
            return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0]["заблокирован"]);
        }

        public static void BlockUser(string login)
        {
            string query = $"UPDATE Пользователи SET заблокирован=1 WHERE логин='{login}'";
            ExecuteNonQuery(query);
        }

        public static void UnblockUser(string login)
        {
            string query = $"UPDATE Пользователи SET заблокирован=0 WHERE логин='{login}'";
            ExecuteNonQuery(query);
        }

        public static void AddUser(string login, string password, string role)
        {
            string query = $"INSERT INTO Пользователи (логин, пароль, роль, заблокирован) VALUES ('{login}', '{password}', '{role}', 0)";
            ExecuteNonQuery(query);
        }

        public static bool UserExists(string login)
        {
            string query = $"SELECT * FROM Пользователи WHERE логин='{login}'";
            DataTable dt = ExecuteQuery(query);
            return dt.Rows.Count > 0;
        }

        public static DataTable GetAllUsers()
        {
            return ExecuteQuery("SELECT * FROM Пользователи");
        }

        public static void DeleteUser(string login)
        {
            string query = $"DELETE FROM Пользователи WHERE логин='{login}'";
            ExecuteNonQuery(query);
        }
    }
}