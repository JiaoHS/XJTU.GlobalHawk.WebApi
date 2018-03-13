using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XJTU.DataAccess.Contract;
using XJTU.Model;
using XJTU.Service.Contract;

namespace XJTU.Service
{
    public class SensorService : BaseService<Sensor>, ISensorService
    {
        public SensorService(ISensorDao dao)
        {
            this.Instance = dao;
        }
    }
}
