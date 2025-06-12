using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Okada.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string JwtToken { get; set; }
        public string RiderFullName { get; set; }
        public string RiderDOB { get; set; }
        public string RiderAddress { get; set; }
        public string RiderLicenseNumber { get; set; }
        public string RiderLicenseExpiry { get; set; } 
    }
}
