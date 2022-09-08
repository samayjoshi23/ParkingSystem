namespace AutomatedParkingSystems.Model
{
    public class ParkingDataBase
    {
        public int OrderId { get; set; }
        public int FloorId { get; set; }
        public int SlotId { get; set; }
        public string PersonName { get; set; }
        public string V_Number { get; set; }
        public DateTime ParkingTime { get; set; }
        public DateTime UnParkingTime { get; set; }
        public int ParkingPin { get; set; }
    }
}
