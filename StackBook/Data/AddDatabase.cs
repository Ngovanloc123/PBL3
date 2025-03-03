//using Microsoft.Data.SqlClient;

//namespace StackBook.Data
//{
//    public class AddDatabase
//    {
//        private static string connectionString = "Server=DESKTOP-A47VFML\\SQLEXPRESS;Database=StackBook;Trusted_Connection=True;TrustServerCertificate=True";
//        public void ConnectDatabase()
//        {
//            using (SqlConnection connect = new SqlConnection(connectionString))
//            {
//                connect.Open();
//                var categories = new (int, string)[]
//                {
//                    (1, "Literature & Fiction"),
//                    (2, "Science & Math"),
//                    (3, "Mystery, Thriller & Suspense"),
//                    (4, "Business & Money"),
//                    (5, "Computers & Technology"),
//                    (6, "Self-Help"),
//                    (7, "Health, Fitness & Dieting"),
//                    (8, "Science Fiction & Fantasy")
//                };

//                int totalRowsAffected = 0;
//                foreach (var category in categories)
//                {
//                    string query = "INSERT INTO Category (CategoryID, CategoryName) VALUES (@CategoryID, @CategoryName)";
//                    using (SqlCommand cmd = connect.CreateCommand())
//                    {
//                        cmd.Parameters.AddWithValue("@CategoryID", category.Item1);
//                        cmd.Parameters.AddWithValue("@CategoryName", category.Item2);
//                        int rowsAffected = cmd.ExecuteNonQuery();
//                        totalRowsAffected += rowsAffected;
//                    }
//                }
//            }
//        }
        
//    }
//}
