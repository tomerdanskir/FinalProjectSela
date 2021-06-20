using Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Plane
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        int _PlaneId;

        Countries _src;
        Countries _dst;
        int _serialID;
        Purpose _PlanesPurpose;
        
        DateTime _receivedOnSystemDateTime;
        bool _startedProcess;

        [NotMapped]
        Spots? _currentSpot;
        [NotMapped]
        DateTime _spotArrivalDateTime;
        bool _finishedProcess;
        DateTime? _finishedOnDateTime;
        bool _isEmergency;

        public Plane()
        {

        }
        public Plane(Countries src, Countries dst, int serialId, Purpose purpose)
        {
            _src = src;
            _dst = dst;
            _serialID = serialId;
            _PlanesPurpose = purpose;
            _receivedOnSystemDateTime = DateTime.Now;
            _startedProcess = false;
            _currentSpot = null;
            _spotArrivalDateTime = DateTime.Now;
            FinishedProcess = false;
            _finishedOnDateTime = null;
            _isEmergency = false;
        }

        public override string ToString()
        {
            return $"{Enum.GetName(typeof(Purpose), PlanesPurpose)} Plane N. {SerialID} from {Src} to {Dst}";
        }

        public Countries Src { get => _src; set => _src = value; }
        public Countries Dst { get => _dst; set => _dst = value; }
        public int SerialID { get => _serialID; set => _serialID = value; }
        public Purpose PlanesPurpose { get => _PlanesPurpose; set => _PlanesPurpose = value; }
        public DateTime ReceivedOnSystemDateTime { get => _receivedOnSystemDateTime; set => _receivedOnSystemDateTime = value; }
        public bool StartedProcess { get => _startedProcess; set => _startedProcess = value; }
        [NotMapped]
        public Spots? CurrentSpot { get => _currentSpot; set => _currentSpot = value; }
        [NotMapped]
        public DateTime SpotArrivalDateTime { get => _spotArrivalDateTime; set => _spotArrivalDateTime = value; }
        public int PlaneId { get => _PlaneId; set => _PlaneId = value; }
        public bool FinishedProcess { get => _finishedProcess; set => _finishedProcess = value; }
        public DateTime? FinishedOnDateTime { get => _finishedOnDateTime; set => _finishedOnDateTime = value; }
        public bool IsEmergency { get => _isEmergency; set => _isEmergency = value; }
    }
}
