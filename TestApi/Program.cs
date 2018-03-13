using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApi
{
    public class Program
    {
        public static void Main(string[] args)
        {

        }

    }

    //public void TurnAngle(List<double> arrs)
    //{
    //    //return arrs;
    //}
    public class GPS
    {
        public double lng;
        public double lat;
        public GPS(double x, double y)
        {
            lng = x;
            lat = y;
        }
    }
}
