using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Models.System_DTOs
{
    public class HealthInfoDTO
    {

        public List<HealthHourInfo> Hourínfo { get; set; }

        public HealthInfoDTO(List<HealthHourInfo> Hourínfo) {

            this.Hourínfo = Hourínfo;
        }


    }
}
