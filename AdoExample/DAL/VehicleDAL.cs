using AutomatedParkingSystems.Model;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace AutomatedParkingSystems.DAL
{
    public class VehicleDAL
    {
        private string _connectionString;

        public VehicleDAL(IConfiguration iconfiguration)
        {
            _connectionString = iconfiguration.GetConnectionString("Default");
        }

        public void SetParkingData(int floorId, int slotId, string Name, string VehicleNo, int Pin, int OrderNum)
        {
            try
            {
                SqlConnection con = new(_connectionString);
                SqlCommand cmd = new("SET_PARKING_INFO", con)
                {
                    CommandType = CommandType.StoredProcedure,
                };

                SqlParameter param1 = new()
                {
                    ParameterName = "@floorId",
                    SqlDbType = SqlDbType.Int,
                    Value = floorId,
                    Direction = ParameterDirection.Input
                };
                SqlParameter param2 = new()
                {
                    ParameterName = "@slotId",
                    SqlDbType = SqlDbType.Int,
                    Value = slotId,
                    Direction = ParameterDirection.Input
                };
                SqlParameter param3 = new()
                {
                    ParameterName = "@Name",
                    SqlDbType = SqlDbType.VarChar,
                    Value = Name,
                    Direction = ParameterDirection.Input
                };
                SqlParameter param4 = new()
                {
                    ParameterName = "@VehicleNo",
                    SqlDbType = SqlDbType.VarChar,
                    Value = VehicleNo,
                    Direction = ParameterDirection.Input
                };
                SqlParameter param5 = new()
                {
                    ParameterName = "@Pin",
                    SqlDbType = SqlDbType.Int,
                    Value = Pin,
                    Direction = ParameterDirection.Input
                };
                SqlParameter param6 = new()
                {
                    ParameterName = "@OrderNum",
                    SqlDbType = SqlDbType.NVarChar,
                    Value = OrderNum,
                    Direction = ParameterDirection.Input
                };
                cmd.Parameters.Add(param1);
                cmd.Parameters.Add(param2);
                cmd.Parameters.Add(param3);
                cmd.Parameters.Add(param4);
                cmd.Parameters.Add(param5);
                cmd.Parameters.Add(param6);

                con.Open();

                cmd.ExecuteReader();

                cmd.Dispose();
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public List<ParkingDataBase> UnparkVehicle(int parkingId, int Pin)
        {
            var parkingInfo = new List<ParkingDataBase>();

            try
            {
                SqlConnection con = new(_connectionString);
                SqlCommand cmd = new("VALIDATE_OWNER", con)
                {
                    CommandType = CommandType.StoredProcedure,
                };

                SqlParameter param1 = new()
                {
                    ParameterName = "@parkingId",
                    SqlDbType = SqlDbType.VarChar,
                    Value = parkingId,
                    Direction = ParameterDirection.Input
                };
                SqlParameter param2 = new()
                {
                    ParameterName = "@Pin",
                    SqlDbType = SqlDbType.VarChar,
                    Value = Pin,
                    Direction = ParameterDirection.Input
                };
                cmd.Parameters.Add(param1);
                cmd.Parameters.Add(param2);

                con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    parkingInfo.Add(new ParkingDataBase
                    {
                        OrderId = Convert.ToInt32(rdr[1]),
                        PersonName = rdr[2].ToString(),
                        FloorId = Convert.ToInt32(rdr[3]),
                        SlotId = Convert.ToInt32(rdr[4]),
                        V_Number = rdr[5].ToString(),
                        ParkingTime = Convert.ToDateTime(rdr[6]),
                        ParkingPin = Convert.ToInt32(rdr[8])
                    });
                }

                rdr.Close();
                cmd.Dispose();

                if (parkingInfo.Count > 0 && parkingInfo[0].ParkingPin == Pin)
                {
                    SqlCommand cmd2 = new("Update_UNPARKING_TIME", con)
                    {
                        CommandType = CommandType.StoredProcedure,
                    };
                    SqlParameter parkId = new()
                    {
                        ParameterName = "@orderId",
                        SqlDbType = SqlDbType.Int,
                        Value = parkingId,
                        Direction = ParameterDirection.Input
                    };
                    SqlParameter parkPin = new()
                    {
                        ParameterName = "@pin",
                        SqlDbType = SqlDbType.Int,
                        Value = Pin,
                        Direction = ParameterDirection.Input
                    };
                    cmd2.Parameters.Add(parkId);
                    cmd2.Parameters.Add(parkPin);
                    cmd2.ExecuteNonQuery();

                    rdr.Close();
                    cmd2.Dispose();
                }

                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return parkingInfo;
        }
    }
}
