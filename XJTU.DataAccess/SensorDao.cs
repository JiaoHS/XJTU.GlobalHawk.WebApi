using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XJTU.DataAccess.BaseDao;
using XJTU.DataAccess.Contract;
using XJTU.Model;

namespace XJTU.DataAccess
{
    public class SensorDao : BaseDao<Sensor>, ISensorDao
    {
        protected override void Init()
        {
            Dao = BeecellRootDao.Instance;
            TableName = "Sensor.";
        }
    }
}
