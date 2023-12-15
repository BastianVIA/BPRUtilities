using System.Data.SqlClient;
using BPRUtilities.Models;

namespace BPRUtilities.Services;

public class GetPCBAUidService : IGetPCBAUidService
{
    public List<int> GetPCBAUids()
    {
        const string myConnectionString = "Server=localhost;Database=LINAK-DB;Trusted_Connection=True;TrustServerCertificate=True;";
        var pcbaUids = new List<int>();

        using var myConnection = new SqlConnection();
        myConnection.ConnectionString = myConnectionString;
        myConnection.Open();

        using var command = new SqlCommand("SELECT Uid FROM PCBAs", myConnection);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            pcbaUids.Add(reader.GetInt32(0));
        }

        return pcbaUids;
    }
    
    public List<PCBA> GetPCBAs()
    {
        const string myConnectionString = "Server=localhost;Database=LINAK-DB;Trusted_Connection=True;TrustServerCertificate=True;";
        var pcbas = new List<PCBA>();

        using var myConnection = new SqlConnection();
        myConnection.ConnectionString = myConnectionString;
        myConnection.Open();

        using var command = new SqlCommand("SELECT * FROM PCBAs WHERE Uid > 600000 AND Uid < 630000", myConnection);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            pcbas.Add(new PCBA()
            {
                Uid = reader.GetInt32(0),
                ItemNumber = reader.GetString(1),
                ManufacturerNumber = reader.GetInt32(2),
                Software = reader.GetString(3),
                ProductionDateCode = reader.GetInt32(4)
            });
        }

        return pcbas;
    }

    public List<int> GetBadPCBAs()
    {
        const string myConnectionString = "Server=localhost;Database=ActuatorsDB;Trusted_Connection=True;TrustServerCertificate=True;";
        var pcbaUids = new List<int>();

        using var myConnection = new SqlConnection();
        myConnection.ConnectionString = myConnectionString;
        myConnection.Open();

        using var command = new SqlCommand("SELECT PCBAUid FROM [ActuatorsDB].[dbo].[Actuators] GROUP BY PCBAUid HAVING COUNT(*) > 1", myConnection);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            pcbaUids.Add(Int32.Parse(reader.GetString(0)));
        }

        return pcbaUids;
    }
}