using System.Data.SqlClient;
using System.Text.RegularExpressions;
using BPRUtilities.Models;
using Microsoft.Data.Sqlite;

namespace BPRUtilities.Services;

public class SQLiteToMSSQLService : ISQLiteToMSSQLService
{
    public void ExecuteService()
    {
        var linakConnectionString = "Data Source=C:\\Users\\Bastian\\Downloads\\Configuration_POLASD-S4S85602.db";
        var myConnectionString =
            "Server=localhost;Database=LINAK-DB;Trusted_Connection=True;TrustServerCertificate=True;";
        var pcbas = new List<PCBA>();
        var actuators = new List<Actuator>();
        var orders = new List<Order>();
        var woNumbers = new List<string>();

        var serialNo = 0;
        var uid = 0;
        var sw = "";
        var conf = "";
        var confLabel = "";
        var refNo = 0;
        var pcbaItemNo = "";
        var passed = 0;
        var failed = 0;
        var date = "";
        var time = "";

        using (var connection = new SqliteConnection(linakConnectionString))
        {
            connection.Open();

            SqliteCommand commandWo =
                new SqliteCommand("select name from sqlite_master where type='table'", connection);

            using (var reader = commandWo.ExecuteReader())
            {
                while (reader.Read())
                {
                    var tableName = reader.GetString(0);
                    if (tableName.Length != 10)
                    {
                        continue;
                    }

                    Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
                    Match result = re.Match(tableName);

                    string alphaPart = result.Groups[1].Value;
                    string woNo = result.Groups[2].Value;

                    if (woNo is "30390465" or "30891648" or "30982330" or "31035611" or "30961092")
                    {
                        continue;
                    }

                    woNumbers.Add(woNo);
                }
            }

            foreach (var woNo in woNumbers)
            {
                SqliteCommand command = new SqliteCommand("select * from PO" + woNo, connection);

                var manufacturerNo = getRandomManufacturerNo();
                var productionDateCode = getRandomProductionDateCode();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        serialNo = reader.GetInt32(1);
                        uid = reader.GetInt32(3);
                        sw = reader.GetString(4);
                        conf = reader.GetString(5);
                        confLabel = reader.GetString(6);
                        refNo = reader.GetInt32(7);
                        pcbaItemNo = reader.GetString(8);
                        passed = reader.GetInt32(9);
                        failed = reader.GetInt32(10);
                        date = reader.GetString(11);
                        time = reader.GetString(12);

                        if (uid == 0 || sw == "" || conf == "" || serialNo == 0 || uid == 411493 || uid == 442790 ||
                            uid == 442792
                            || uid == 467293 || uid == 467292 || uid == 481022 || uid == 481021 || uid == 486831 ||
                            uid == 486832
                            || uid == 521723 || uid == 521724 || uid == 583467 || uid == 704665 || uid == 704666 ||
                            uid == 743476
                            || uid == 746336 || uid == 773863)
                        {
                            continue;
                        }

                        var pcba = new PCBA
                        {
                            Uid = uid, ItemNumber = pcbaItemNo, Software = sw,
                            ManufacturerNumber = manufacturerNo, ProductionDateCode = productionDateCode
                        };

                        var actuator = new Actuator
                        {
                            WorkOrderNumber = woNo, SerialNumber = serialNo, PCBAUid = uid, CreationDate = date, 
                            CreationTime = time, SumOfPassedTests = passed, SumOfFailedTests = failed
                        };
                        
                        pcbas.Add(pcba);
                        actuators.Add(actuator);
                    }

                }
                var order = new Order
                    { WorkOrderNumber = woNo, Configuration = conf, ReferenceNumber = refNo };
                orders.Add(order);
            }


            using (SqlConnection myConnection = new SqlConnection())
            {
                myConnection.ConnectionString = myConnectionString;
                myConnection.Open();
                var count = 0;
                var onePercent = orders.Count / 100;
                var percentage = 0;
                foreach (var order in orders)
                {
                    SqlCommand command = new SqlCommand(
                        "INSERT INTO Orders (WorkOrderNumber, Configuration, ReferenceNumber) VALUES (@0, @1, @2)",
                        myConnection);

                    command.Parameters.Add(new SqlParameter("0", order.WorkOrderNumber));
                    command.Parameters.Add(new SqlParameter("1", order.Configuration));
                    command.Parameters.Add(new SqlParameter("2", order.ReferenceNumber));

                    command.ExecuteNonQuery();
                    count++;

                    if (count % onePercent == 0)
                    {
                        percentage++;
                        Console.WriteLine("Orders progress: " + percentage + "% done");
                    }
                }

                Console.WriteLine("Done with Orders");

                count = 0;
                onePercent = pcbas.Count / 100;
                percentage = 0;
                foreach (var pcba in pcbas)
                {

                    SqlCommand command = new SqlCommand(
                        "INSERT INTO PCBAs (Uid, ItemNumber, Software, ManufacturerNumber, ProductionDateCode) VALUES (@0, @1, @2, @3, @4)",
                        myConnection);

                    command.Parameters.Add(new SqlParameter("0", pcba.Uid));
                    command.Parameters.Add(new SqlParameter("1", pcba.ItemNumber));
                    command.Parameters.Add(new SqlParameter("2", pcba.Software));
                    command.Parameters.Add(new SqlParameter("3", pcba.ManufacturerNumber));
                    command.Parameters.Add(new SqlParameter("4", pcba.ProductionDateCode));

                    command.ExecuteNonQuery();
                    count++;

                    if (count % onePercent == 0)
                    {
                        percentage++;
                        Console.WriteLine("PCBAs progress: " + percentage + "% done");
                    }
                }

                Console.WriteLine("Done with PCBAs");

                count = 0;
                onePercent = actuators.Count / 100;
                percentage = 0;
                foreach (var actuator in actuators)
                {
                    SqlCommand command = new SqlCommand(
                        "INSERT INTO Actuators (WorkOrderNumber, SerialNumber, PCBAUid, CreationDate, CreationTime, SumOfPassedTests, SumOfFailedTests) VALUES (@0, @1, @2, @3, @4, @5, @6)",
                        myConnection);

                    command.Parameters.Add(new SqlParameter("0", actuator.WorkOrderNumber));
                    command.Parameters.Add(new SqlParameter("1", actuator.SerialNumber));
                    command.Parameters.Add(new SqlParameter("2", actuator.PCBAUid));
                    command.Parameters.Add(new SqlParameter("3", actuator.CreationDate));
                    command.Parameters.Add(new SqlParameter("4", actuator.CreationTime));
                    command.Parameters.Add(new SqlParameter("5", actuator.SumOfPassedTests));
                    command.Parameters.Add(new SqlParameter("6", actuator.SumOfFailedTests));

                    command.ExecuteNonQuery();
                    count++;
                    if (count % onePercent == 0)
                    {
                        percentage++;
                        Console.WriteLine("Actuators progress: " + percentage + "% done");
                    }
                }

                Console.WriteLine("Done with Actuators");
            }
        }
    }
    
    int getRandomManufacturerNo()
    {
        var manuNo = new List<int> { 9937, 7157, 1042, 7507 };
        Random random = new Random();
        return manuNo[random.Next(0, 4)];
    }

    int getRandomProductionDateCode()
    {
        var PDC = new List<int> { 0220, 0720, 1120, 1620, 2320, 2620, 3320, 4020, 0321, 0721, 1221, 2621 };
        Random random = new Random();
        return PDC[random.Next(0, 12)];
    }
}