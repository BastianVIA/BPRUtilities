using System.Data;
using System.Data.SqlClient;
using BPRUtilities.Models;

namespace BPRUtilities.Services;

public class FixPCBAUidService : IFixPCBAUidService
{
    private IGetPCBAUidService getPcbaUidService;

    public FixPCBAUidService(IGetPCBAUidService getPcbaUidService)
    {
        this.getPcbaUidService = getPcbaUidService;
    }

    public async Task ExecuteService()
    {
        var pcbaUidsToFix = getPcbaUidService.GetBadPCBAs();
        var pcbas = getPcbaUidService.GetPCBAs();
        var pcbaIndex = 0;
        
        const string myConnectionString = "Server=localhost;Database=ActuatorsDB;Trusted_Connection=True;TrustServerCertificate=True;";
        var actuators = new List<Actuator>();

        using var myConnection = new SqlConnection();
        myConnection.ConnectionString = myConnectionString;
        myConnection.Open();

        foreach (var pcbaUid in pcbaUidsToFix)
        {
            using var actuatorCommand =
                new SqlCommand(
                    "SELECT WorkOrderNumber, SerialNumber FROM [ActuatorsDB].[dbo].[Actuators] WHERE PCBAUid = '" +
                    pcbaUid + "'", myConnection);
            using var actuatorReader = actuatorCommand.ExecuteReader();
            actuatorReader.Read();
            actuators.Add(new Actuator()
            {
                WorkOrderNumber = actuatorReader.GetInt32(0).ToString(),
                SerialNumber = actuatorReader.GetInt32(1),
                PCBAUid = pcbas[pcbaIndex].Uid
            });
            pcbaIndex++;
        }

        for (int i = 0; i < pcbaIndex; i++)
        {
            SqlCommand command = new SqlCommand(
                "INSERT INTO PCBAs (Uid, ItemNumber, Software, ManufacturerNumber, ProductionDateCode) VALUES (@0, @1, @2, @3, @4)",
                myConnection);

            command.Parameters.Add(new SqlParameter("0", pcbas[i].Uid));
            command.Parameters.Add(new SqlParameter("1", pcbas[i].ItemNumber));
            command.Parameters.Add(new SqlParameter("2", pcbas[i].Software));
            command.Parameters.Add(new SqlParameter("3", pcbas[i].ManufacturerNumber));
            command.Parameters.Add(new SqlParameter("4", pcbas[i].ProductionDateCode));

            command.ExecuteNonQuery();
        }

        foreach (var actuator in actuators)
        {
            SqlCommand command = new SqlCommand(
                "UPDATE Actuators SET PCBAUid = @0 " +
                "WHERE WorkOrderNumber = @1 AND SerialNumber = @2;",
                myConnection);

            command.Parameters.Add(new SqlParameter("0", actuator.PCBAUid));
            command.Parameters.Add(new SqlParameter("1", actuator.WorkOrderNumber));
            command.Parameters.Add(new SqlParameter("2", actuator.SerialNumber));

            command.ExecuteNonQuery();
        }
    }
}