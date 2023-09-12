using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AzureFunctionRed.Models
{
	public class Message
	{
		
		public int Id { get; set; }
		public string Content { get; set; }



	}
}
