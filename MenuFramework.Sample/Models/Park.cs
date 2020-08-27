using System;
using System.Collections.Generic;
using System.Text;

namespace MenuFramework.Sample.Models
{
    public class Park
    {
        public int ParkId { get; set; }
        public string Name { get; set; }
        public string State { get; set; }

        public Park(int id, string name, string state)
        {
            ParkId = id;
            Name = name;
            State = state;
        }

        public override string ToString()
        {
            return $"{Name}, {State}";
        }
    }
}
