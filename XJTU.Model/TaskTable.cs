using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XJTU.Model
{
    public class TaskTable : BaseEntity
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Sensor { get; set; }
        public string Destination { get; set; }
        public int State { get; set; }
    }
}
