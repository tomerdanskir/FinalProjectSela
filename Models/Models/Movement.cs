using Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Movement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MovementId { get; set; }

        public Spots? Src { get; set; }
        public Spots Dst { get; set; }

        public DateTime MovementDateTime{ get; set; }

        public int PlainOnMoveId { get; set; }

        public virtual Plane PlainOnMove { get; set; }


        public Movement()
        {

        }

        public Movement(Plane p, Spots? srcSpot, Spots dstSpot)
        {
            //PlainOnMove = p;
            PlainOnMoveId = p.PlaneId;
            Src = srcSpot;
            Dst = dstSpot;
        }
    }
}
