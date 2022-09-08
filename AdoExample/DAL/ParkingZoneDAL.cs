using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using AutomatedParkingSystems.Model;

namespace AutomatedParkingSystems.DAL
{
    public class ParkingDAL
    {
        private string _connectionString;
        public ParkingDAL(IConfiguration iconfiguration)
        {
            _connectionString = iconfiguration.GetConnectionString("Default");
        }
        
        public List<ParkingZoneModel> GetList()
        {
            var listParkingZoneModel = new List<ParkingZoneModel>();

            try
            {
                
                SqlConnection con = new(_connectionString);
                SqlCommand cmd = new("GET_PARKING_DATA", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    listParkingZoneModel.Add(new ParkingZoneModel
                    {
                        ZoneId = Convert.ToInt32(rdr[0]),
                        ZoneTitle = rdr[1].ToString(),
                        TotalSlots = Convert.ToInt32(rdr[2]),
                        FilledSlots = Convert.ToInt32(rdr[3]),
                        AvialableSlots = Convert.ToInt32(rdr[4])
                    });
                }
                cmd.Dispose();
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return listParkingZoneModel;
        }

    }
}