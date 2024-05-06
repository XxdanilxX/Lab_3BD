using MySql.Data.MySqlClient;

namespace Lab_3BD
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Getting Connection ...");
            MySqlConnection conn = DBUtils.GetDBConnection();

            try
            {
                Console.WriteLine("Opening Connection ...");
                conn.Open();
                Console.WriteLine("Connection successful!");

                QueryOrders(conn);

                // Додаємо нове замовлення
                InsertOrder(conn, "2024-04-30", 1, "Добриво A", 1, "Пільга 1", 100.50, "2024-05-10");

                Console.WriteLine("\nЗамовлення після додавання нового запису:");
                QueryOrders(conn);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }

            Console.ReadLine();
        }


        private static void QueryOrders(MySqlConnection conn)
        {
            string orderId, orderDate, productName, deliveryDate;
            string sql = "SELECT Замовлення.код_замовлення, Замовлення.дата_заповнення, Добрива.назва_добрива, Замовлення.дата_постачання " +
                         "FROM Замовлення " +
                         "JOIN Добрива ON Замовлення.код_добрива = Добрива.код_добрива";

            MySqlCommand cmd = new MySqlCommand();

            cmd.Connection = conn;
            cmd.CommandText = sql;

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                Console.WriteLine("------------------------------------------------------------------------------------");
                Console.WriteLine("Код замовлення | Дата заповнення | Назва добрива | Дата постачання");
                Console.WriteLine("------------------------------------------------------------------------------------");

                while (reader.Read())
                {
                    orderId = reader["код_замовлення"].ToString();
                    orderDate = reader["дата_заповнення"].ToString();
                    productName = reader["назва_добрива"].ToString();
                    deliveryDate = reader["дата_постачання"].ToString();

                    Console.WriteLine($"{orderId,-15} | {orderDate,-15} | {productName,-15} | {deliveryDate,-15}");
                }

                Console.WriteLine("------------------------------------------------------------------------------------");
            }
        }

        private static void InsertOrder(MySqlConnection conn, string orderDate, int customerId, string productName, int fertilizerId, string privilege, double area, string deliveryDate)
        {
            string sql = "INSERT INTO Замовлення (дата_заповнення, код_замовника, назва_добрива, код_добрива, категорія_пільг, площа_для_обробки, дата_постачання) " +
                         "VALUES (@orderDate, @customerId, @productName, @fertilizerId, @privilege, @area, @deliveryDate)";

            MySqlCommand cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@orderDate", orderDate);
            cmd.Parameters.AddWithValue("@customerId", customerId);
            cmd.Parameters.AddWithValue("@productName", productName);
            cmd.Parameters.AddWithValue("@fertilizerId", fertilizerId);
            cmd.Parameters.AddWithValue("@privilege", privilege);
            cmd.Parameters.AddWithValue("@area", area);
            cmd.Parameters.AddWithValue("@deliveryDate", deliveryDate);

            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                Console.WriteLine($"Додано нове замовлення: {productName}");
            }
            else
            {
                Console.WriteLine("Помилка при додаванні нового замовлення.");
            }
        }
    }
}
