using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    //Class to hold the health data for an hour
    //Can be expanded to hold more types of data currently only has stepdata.
    public class HealthHourInfo
    {
        public DateTime startTime {  get; set; }
        public DateTime endTime { get; set; }
        public Double dataCount { get; set; }
        
        public String metricName { get; set; }


        public HealthHourInfo(DateTime startTime, DateTime endTime, Double dataCount, String metricName) { 
            this.startTime = startTime;
            this.endTime = endTime;
            this.dataCount = dataCount;
            this.metricName = metricName;
        }

        public override string ToString()
        {
            return $"Start time: {startTime}, End time: {endTime}, Value of {metricName}: {dataCount}";
        }
    }
}
