using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Road_Finder_1
{
    internal class City
    {
        private string Name { get; set; }
        private int xCoord { get; set; }
        private int yCoord { get; set; }

        private List<string> connectedCities { get; set; }

        private bool isVisited = false;

        public City(string name, int x, int y, List<string> cities)
        {
            Name = name;
            xCoord = x;
            yCoord = y;
            connectedCities = cities;
        }
        

        public string GetName()
        {
            return Name;
        }

        public int Get_X()
        {
            return xCoord;
        }

        public int Get_Y()
        {
            return yCoord;
        }

        public List<string> GetConnectedCities()
        {
            return connectedCities;
        }

        public void SetIsVisited(bool visted)
        {
            isVisited = visted;
        }

        public bool GetIsVisited()
        {
            return isVisited;
        }

    }
}
