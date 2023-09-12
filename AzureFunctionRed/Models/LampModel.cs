using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionRed.Models
{
    public class LampModel
    {
        
        public int Id { get; set; }
	    public bool isOn { get; set; } = false;
        public string DeviceName { get; set; } = null!;
        public string DeviceColor { get; set; } = null!;
        public DateTime Created { get; set; }

    }
}
