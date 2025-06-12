using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Okada.Models
{
    public class Earning
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string PhoneNumber {  get; set; }
        public decimal Amount {  get; set; }
        public DateTime Date {  get; set; }
    }
}
