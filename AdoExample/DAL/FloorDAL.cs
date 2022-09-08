using AutomatedParkingSystems.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedParkingSystems.DAL
{
    public class FloorDAL
    {
        private string _connectionString;
        
        public FloorDAL(IConfiguration iconfiguration)
        {
            _connectionString = iconfiguration.GetConnectionString("Default");
        }

        public List<FloorSlotsModel> GetFloorSlots(int floorId)
        {
            var listFloorSlots = new List<FloorSlotsModel>();

            try
            {
                SqlConnection con = new(_connectionString);
                SqlCommand cmd = new("GET_ZONE_SLOTS", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                SqlParameter param1 = new SqlParameter
                {
                    ParameterName = "@floorId",
                    SqlDbType = SqlDbType.NVarChar,
                    Value = floorId,
                    Direction = ParameterDirection.Input,
                };
                cmd.Parameters.Add(param1);


                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    listFloorSlots.Add(new FloorSlotsModel
                    {
                        slotId = Convert.ToInt32(rdr[0]),
                        FloorSlots = Convert.ToInt32(rdr[1]),
                    });
                }
                cmd.Dispose();
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return listFloorSlots;
        }


        public void ParkAndUnpark(int floorId, int slotId, string condition)
        {
            try
            {
                SqlConnection con = new(_connectionString);
                SqlCommand cmd = new("SET_PARKING_SLOT", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                SqlParameter param1 = new()
                {
                    ParameterName = "@floorId",
                    SqlDbType = SqlDbType.NVarChar,
                    Value = floorId,
                    Direction = ParameterDirection.Input
                };
                SqlParameter param2 = new()
                {
                    ParameterName = "@slotId",
                    SqlDbType = SqlDbType.NVarChar,
                    Value = slotId,
                    Direction = ParameterDirection.Input
                };
                SqlParameter param3 = new()
                {
                    ParameterName = "@cond",
                    SqlDbType = SqlDbType.NVarChar,
                    Value = condition,
                    Direction = ParameterDirection.Input
                };
                cmd.Parameters.Add(param1);
                cmd.Parameters.Add(param2);
                cmd.Parameters.Add(param3);


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
    }
}
