namespace AutomatedParkingSystems.Model
{
    public class ParkingZoneModel
    {
        public int ZoneId { get; set; }
        public string? ZoneTitle { get; set; }
        public int TotalSlots { get; set; }
        public int FilledSlots { get; set; }
        public int AvialableSlots { get; set; }

    }
}