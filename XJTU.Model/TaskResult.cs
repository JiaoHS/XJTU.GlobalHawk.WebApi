using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XJTU.Model
{
    public class TaskResult:BaseEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime Time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Destination { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Link { get; set; }
    }
}
