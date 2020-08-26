using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MenuFramework.DAL
{
    public class ParkDao
    {
        private string connectionString;
        private List<Park> parks = new List<Park>()
        {
            new Park(1, "Cuyahoga Valley", "Ohio"),
            new Park(2, "Acadia", "Maine"),
            new Park(3, "Yosemite", "California"),
        };
        public ParkDao(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IEnumerable<Park> GetList()
        {
            return parks;
        } 

        public void Add(Park park)
        {
            parks.Add(park);
        }

        public void Update(Park park)
        {
            Park parkToUpdate = parks.Find(p => p.ParkId == park.ParkId);
            if (parkToUpdate != null)
            {
                parkToUpdate.Name = park.Name;
                parkToUpdate.State = park.State;
            }
        }

        internal void Delete(int parkId)
        {
            Park parkToDelete = parks.Find(p => p.ParkId == parkId);
            if (parkToDelete != null)
            {
                parks.Remove(parkToDelete);
            }
        
        }
    }
}
