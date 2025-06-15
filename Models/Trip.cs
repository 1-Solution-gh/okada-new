using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Okada.Models
{
    public class Trip
    {
        public int Id { get; set; }

        public string PhoneNumber { get; set; }
        public string Destination {  get; set; }
        public string Status {  get; set; }
        public DateTime Date { get; set; }
    }
}
