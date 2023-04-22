using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;


namespace search_functionality
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Document> documents = SearchDocument("Contenido 1", false);
            foreach (Document document in documents)
            {
                Console.WriteLine("Document ID: " + document.Id);
                Console.WriteLine("Title: " + document.Title);
                Console.WriteLine("Content: " + document.Content);

            }


            //List<Document> documents = SearchDocument("Contenido 2", true);

            //foreach (Document document in documents)
            //{
            //    Console.WriteLine("Document ID: " + document.Id);
            //    Console.WriteLine("Title: " + document.Title);
            //    Console.WriteLine("Content: " + document.Content);
            //    Console.WriteLine();
            //}
        }

        public static List<Document> SearchDocument(string query, bool matchAll)
        {
            query = query.Trim().ToLower();

            string[] words = query.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

            string sql = "SELECT id, title, content from document where";

            if (matchAll)
            {
                sql += string.Join(" AND ", words.Select(word => $"(LOWER(title) LIKE '%{word}%' OR LOWER(content) LIKE '%{word}%')"));
            }
            else
            {
                sql += string.Join(" OR ", words.Select(word => $"(LOWER(title) LIKE '%{word}%' OR LOWER(content) LIKE '%{word}%')"));
            }

            using (SqlConnection conn = new SqlConnection(Conexion.cConexion))
            {
                conn.Open();

                using( SqlCommand command = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        var documents = new List<Document>();
                        while (reader.Read())
                        {
                            documents.Add(new Document
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Content = reader.GetString(2)
                            });
                        }
                        return documents;
                    }
                }
            }
        }
    }
}
