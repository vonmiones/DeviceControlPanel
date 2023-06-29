using System;

namespace DeviceControlPanel.Classes
{
    public interface IDevice
    {
        int Id { get; set; }
        string DeviceName { get; set; }
        string Description { get; set; }
        string Alias { get; set; }
        string Ip { get; set; }
        int Port { get; set; }
        string Serial { get; set; }
        string Tag { get; set; }
        int MachineId { get; set; }
        string Model { get; set; }
        string Processor { get; set; }
        string Version { get; set; }
        string Kind { get; set; }
        int CategoryId { get; set; }
        int TypeId { get; set; }
        DateTime DtCreated { get; set; }
        DateTime DtUpdated { get; set; }
        DateTime DtDeleted { get; set; }
        string Status { get; set; }
        string Remarks { get; set; }
    }

    public class Device : IDevice
    {
        public int Id { get; set; }
        public string DeviceName { get; set; }
        public string Description { get; set; }
        public string Alias { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        public string Serial { get; set; }
        public string Tag { get; set; }
        public int MachineId { get; set; }
        public string Model { get; set; }
        public string Processor { get; set; }
        public string Version { get; set; }
        public string Kind { get; set; }
        public int CategoryId { get; set; }
        public int TypeId { get; set; }
        public DateTime DtCreated { get; set; }
        public DateTime DtUpdated { get; set; }
        public DateTime DtDeleted { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
    }

    public class DeviceTap
    {
        public string sdwEnrollNumber { get; set; }
        public int idwVerifyMode { get; set; }
        public int idwInOutMode { get; set; }
        public int idwYear { get; set; }
        public int idwMonth { get; set; }
        public int idwDay { get; set; }
        public int idwHour { get; set; }
        public int idwMinute { get; set; }
        public int idwSecond { get; set; }
        public int idwWorkcode { get; set; }
    }
}
