using AutomatedParkingSystems.DAL;
using Microsoft.Extensions.Configuration;
using AutomatedParkingSystems.Model;


namespace AutomatedParkingSystems
{
    internal class Program
    {
        private static IConfiguration? _iconfiguration;
        static void Main(string[] args)
        {
            GetAppSettingsFile();


            char input = 'Y';
            while(input == 'Y' || input == 'y')
            {
                Console.Clear();
                Console.WriteLine("\t\t================= Menu ===================");
                Console.WriteLine("\t\t\t 1 -> Get Parking Avialability");
                Console.WriteLine("\t\t\t 2 -> Park Vehicle");
                Console.WriteLine("\t\t\t 3 -> Unpark Vehicle");

                Console.Write("\t\t\t 4 -> Exit\n\n\t\t\tEnter Choice => ");

                try
                {
                    int Choice = Convert.ToInt32(Console.ReadLine());
                    int flag = 0;
                    switch (Choice)
                    {
                        case 1:
                            PrintParkingData(Choice);
                            break;
                        case 2:
                            PrintParkingData(Choice);
                            break;
                        case 3:
                            flag = UnParkVehicle();
                            break;
                        case 4:
                            Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine("No Such Option Avialable....");
                            break;
                    }

                    if (flag == 1)
                        Console.Write("Do you want to park Another vehicle? (Y/N) : ");
                    else
                        Console.Write("Want to start the process again?? (Y/N) : ");
                    input = Convert.ToChar(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }
            }
        }

        static void GetAppSettingsFile()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true);
            _iconfiguration = builder.Build();
        }

        public static int checkSlots(List<ParkingZoneModel> zoneData)
        {
            var ZoneList = zoneData;
            int slotCount = 0;
            foreach (var zone in ZoneList)
                slotCount += zone.AvialableSlots;

            return slotCount;
        }

        static void PrintParkingData(int choice)
        {
            int floorId = 1;
            var parkingDAL = new ParkingDAL(_iconfiguration);
            var ListParkingData = parkingDAL.GetList();

            if(checkSlots(ListParkingData) == 0)
            {
                Console.WriteLine("\n\n\n\n\t===============================================================================================\n\n");
                Console.WriteLine("\t\tSorry there is no parking slot Available in the entire Parking System.... \n\n");
                Console.WriteLine("\t===============================================================================================");
                return;
            }

            Console.WriteLine("\t\n\nParking data according to Floors : \n");
            Console.WriteLine("-------------------------------------------------------------------------------------");
            Console.WriteLine($" FloorId   \t| Floors  \t| Total Slots\t| Filled Slots\t| Available Slots");
            Console.WriteLine("-------------------------------------------------------------------------------------");

            ListParkingData.ForEach(item =>
            {
                if (item.AvialableSlots == 0)
                {
                    Console.Write($"     {item.ZoneId}  \t| {item.ZoneTitle}\t| ----------- Floor is Filled Completely ------------ \n");
                }
                else
                {
                    Console.Write($"     {item.ZoneId}  \t| {item.ZoneTitle}\t|    {item.TotalSlots}\t\t|    {item.FilledSlots}\t\t|    {item.AvialableSlots}\n");
                }
            });
            Console.WriteLine("-------------------------------------------------------------------------------------");


            if (choice == 1)
                return;

            
            int checkFlag = 0;
            do
            {
                try
                {
                    Console.Write("Enter Floor Id to park into the floor / Or press 'X' to Exit: ");
                    string input = Console.ReadLine();

                    if (input == "X" || input == "x")
                        Environment.Exit(0);
                    else
                    {
                        floorId = Convert.ToInt32(input);
                        if (floorId < 0 || floorId > ListParkingData.Count)
                        {
                            Console.WriteLine("NO such floor available... Check Floor Id and try again...\n\n");
                            continue;
                        }
                        else if (ListParkingData[floorId-1].AvialableSlots == 0)
                        {
                            Console.WriteLine("The Floor is Filled Completely, try to park in another slot.");
                            continue;
                        }
                        checkFlag = 1;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }
            } while (checkFlag == 0);


            PrintFloorSlots(floorId);
        }

        static void PrintFloorSlots(int floorId)
        {
            Console.Clear();
            int slotId = 1;
            var FloorDAL = new FloorDAL(_iconfiguration);
            var ListFloorSlotData = FloorDAL.GetFloorSlots(floorId);
            string str = "";


            Console.WriteLine($"\nHere are the slots available in Floor{floorId}\n");
            Console.Write($"\t\t  SlotId\t|  Availability\n");
            Console.WriteLine("\t\t-------------------------------");
            ListFloorSlotData.ForEach(item =>
            {
                if (item.FloorSlots == 1)
                    str = "  ----   ";
                else
                    str = "Available";

                Console.Write($"\t\t  {item.slotId}\t|  {str}\n");
            });
            Console.WriteLine("\t\t-------------------------------");


            int checkFlag = 0;
            do
            {
                try
                {
                    Console.Write("Enter Floor Id to park into the floor / Or press 'X' to Exit: ");
                    string input = Console.ReadLine();

                    if (input == "X" || input == "x")
                        Environment.Exit(0);
                    else
                    {
                        slotId = Convert.ToInt32(input);
                        if (slotId < 0 || slotId > ListFloorSlotData.Count)
                        {
                            Console.WriteLine("NO such slot available in the parking... Check Slot Id and try again...\n\n");
                            continue;
                        }
                        else if(ListFloorSlotData[slotId-1].FloorSlots == 1){
                            Console.WriteLine("The slot is already filled, try in another slot.");
                            continue;
                        }
                        checkFlag = 1;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }
            } while (checkFlag == 0);


            SetSlot(floorId, slotId, "park");
        }

        static void SetSlot(int floorId, int slotId, string condition)
        {
            Console.Clear();
            var FloorDAL = new FloorDAL(_iconfiguration);
            FloorDAL.ParkAndUnpark(floorId, slotId, condition);

            if (condition == "park")
            {
                Console.WriteLine(" +-------------------------------------------------------+");
                Console.WriteLine(" |\t\t\t\t\t\t\t |");
                Console.WriteLine($" |  Your vehicle will be parked at Floor-{floorId} at Slot-{slotId}\t |");
                Console.WriteLine(" |\t\t\t\t\t\t\t |");
                Console.WriteLine(" +-------------------------------------------------------+");

                Console.Write("\n\n\nEnter your Name : ");
                string Name = Console.ReadLine();

                Console.Write("\nEnter your Vehicle Number : ");
                string VehicleNo = Console.ReadLine();

                Console.Write("\nCreate 4 digit pin to use it while unparking : ");
                int pin = Convert.ToInt32(Console.ReadLine());


                PrintUserInfo(floorId, slotId, Name, VehicleNo, pin);
            }
            else
            {
                FloorDAL.ParkAndUnpark(floorId, slotId, condition);
            }

        }

        static void PrintUserInfo(int fId, int sId, string UserName, string vehicleNumber, int pin)
        {
            Console.Clear();
            Random rnd = new();
            int parkingId = rnd.Next(10, 10000000);

            Console.WriteLine($"\n\n\n\n \t\t+-----------------------------------------------+\n");
            Console.WriteLine($" \t\t Keep this slip untill you unpark your Vehicle \n");
            Console.WriteLine($" \t\t+-----------------------------------------------+");
            Console.WriteLine($" \t\t| ParkingId \t\t | {parkingId} \t\t |");
            Console.WriteLine($" \t\t+-----------------------------------------------+");
            Console.WriteLine($" \t\t| Name \t\t\t | {UserName}\t\t |");
            Console.WriteLine($" \t\t+-----------------------------------------------+");
            Console.WriteLine($" \t\t| Vehicle Number\t | {vehicleNumber}\t\t |");
            Console.WriteLine($" \t\t+-----------------------------------------------+");
            Console.WriteLine($" \t\t| Pin \t\t\t | {pin}\t\t\t |");
            Console.WriteLine($" \t\t+-----------------------------------------------+");

            var VehicleDataDAL = new VehicleDAL(_iconfiguration);
            VehicleDataDAL.SetParkingData(fId, sId, UserName, vehicleNumber, pin, parkingId);
        }

        static int UnParkVehicle()
        {
            Console.Clear();
            int checkFlag = 0;
            var vehicleDAL = new VehicleDAL(_iconfiguration);
            var floorDAL = new FloorDAL(_iconfiguration);
            var vehicleInfo = new List<ParkingDataBase>();
            do
            {
                try
                {
                    Console.Write("\n\nOr press 'X' to Exit\nEnter Parking ID for Unparking : ");
                    string input = Console.ReadLine();

                    if (input == "X" || input == "x")
                        Environment.Exit(0);
                    else
                    {
                        int parkingId = Convert.ToInt32(input);

                        Console.Write("\nEnter PIN : ");
                        int pin = Convert.ToInt32(Console.ReadLine());

                        vehicleInfo = vehicleDAL.UnparkVehicle(parkingId, pin);
                        if (vehicleInfo.Count < 1)
                        {
                            Console.WriteLine("Something went Wrong !!!! Credentials are not matching....");
                            Console.WriteLine("\n\nRetry Again...");
                            continue;
                        }
                        checkFlag = 1;
                        Console.WriteLine("\n\n\nOwner Validated Successfully....");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }
            } while (checkFlag == 0);

            int floorId = vehicleInfo[0].FloorId;
            int slotId = vehicleInfo[0].SlotId;

            floorDAL.ParkAndUnpark(floorId, slotId, "unpark");
            Console.WriteLine("\t\t+=========================\n");
            Console.WriteLine("\t\t\tVehicle Unparked\n");
            Console.WriteLine("\t\t\t  Drive Safely\n");
            Console.WriteLine("\t\t\tThankyou for Parking\n");
            Console.WriteLine("\t\t+=========================");

            return 1;
        }
    }
}