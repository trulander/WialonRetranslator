using System;

namespace WialonRetranslator.DataAccess.Entities
{
    public class Point
    {
        public int PointId { get; set; }
        public DateTime PointTime { get; set; }
        public string DeviceId { get; set; }
        public string Latitude { get; set; }
        public string Longtitude { get; set; }
    }
}