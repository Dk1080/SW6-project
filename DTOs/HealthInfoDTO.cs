﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class HealthInfoDTO
    {

        public List<HealthHourInfo> hourInfos { get; set; }

        public HealthInfoDTO(List<HealthHourInfo> hourInfos) {

            this.hourInfos = hourInfos;
        }


    }
}
