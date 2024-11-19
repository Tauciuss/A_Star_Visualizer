using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace Road_Finder_1
{
    public partial class Form1 : Form
    {
        string path = "";
        List<City> cityList = new List<City>();
        private bool isClicked = false;
        int howManyCities = 0;
        public Form1()
        {
            InitializeComponent();
            resultBox.Controls.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Paint += mapPanel_paint;
        }

        private void mapPanel_paint(object sender, PaintEventArgs e)
        {
            if (isClicked)
            {
                howManyCities = 0;
                //Connections
                Graphics grap2 = e.Graphics;
                Pen pen2 = new Pen(Color.Black, 1);
                for (int i = 0; i < cityList.Count; i++)
                {
                    for (int j = 1; j < cityList.Count; j++)
                    {
                        for (int k = 0; k < cityList[j].GetConnectedCities().Count; k++)
                        {
                            if (cityList[i].GetName().Equals(cityList[j].GetConnectedCities()[k]))
                            {
                                grap2.DrawLine(pen2, cityList[i].Get_X() + 12, cityList[i].Get_Y() + 12,
                                    cityList[j].Get_X() + 13, cityList[j].Get_Y() + 13);
                                //grap2.DrawLine(pen2, cityList[i].Get_X(), cityList[i].Get_Y(),
                                //    cityList[j].Get_X(), cityList[j].Get_Y());
                            }

                            if(howManyCities < cityList[j].GetConnectedCities().Count)
                                howManyCities = cityList[j].GetConnectedCities().Count;
                        }
                    }
                }

                //City locations
                Graphics grap1 = e.Graphics;
                Pen pen1 = new Pen(Color.Red, 7);
                foreach (City city in cityList)
                {
                    grap1.DrawLine(pen1, city.Get_X() + 12, city.Get_Y() + 12, city.Get_X() + 13, city.Get_Y() + 13);
                    Label label = new Label();
                    label.Text = city.GetName();
                    label.AutoSize = true;
                    label.Location = new Point(city.Get_X(), city.Get_Y());
                    label.BackColor = Color.Transparent;
                    mapPanel.Controls.Add(label);
                }

                FindTheCity(e);                
            }
        }

        private void findRouteButton_Click(object sender, EventArgs e)
        {
            isClicked = true;
            mapPanel.Controls.Clear();
            this.Refresh();
            PathFinding();
            this.Invalidate();
            isClicked = false;            
        }

        private void PathFinding()
        {
            resultBox.Text = "";
            string startingCityName = startComboBox.SelectedItem.ToString();
            string destinationCity = destinationComboBox.SelectedItem.ToString();
            City shortest = null;
            List<string> cityPath = new List<string>();
            //The starting city shows which ways it can go and is marked as visited
            foreach(City city in cityList)
            {
                if(city.GetName() == startingCityName)
                {
                    shortest = city;
                    city.SetIsVisited(true);
                }
            }

            cityPath.Add(startingCityName);
            double min = 9999;
            string nearestCity = "";
            
            //From here, we need to go through all the cities
            foreach (City city1 in cityList) {
                //Thread.Sleep(100);
                min = 9999;
                List<City> connectedCities = GetConnectedCityList(shortest.GetConnectedCities());
                List<string> distances = CountDistances(shortest.GetName(), connectedCities);
                if (connectedCities.Count == 0)                
                    break;

                string temp = "";
                //going through the list of all connected cities that are allowed to visit
                for(int i = 0; i < distances.Count; i++) {
                    string[] cityDistance = distances[i].Split(';');

                    if(int.Parse(cityDistance[1]) < min)
                    {
                        min = int.Parse(cityDistance[1]);
                        temp = cityDistance[0];
                    }
                }
                foreach (City city in cityList)
                {
                    if(temp == city.GetName())
                    {
                        city.SetIsVisited(true);
                        shortest = city;
                    }    
                }

                cityPath.Add(temp);
                if (temp == destinationCity)                
                    break;
                
            }

            foreach(City city in cityList)
            {
                city.SetIsVisited(false);
            }

            foreach (string city in cityPath)
            {
                resultBox.Text += city;
                resultBox.Text += Environment.NewLine;
            }
            MessageBox.Show("Found the path.");
        }

        private List<string> CountDistances(string from, List<City> connected)
        {
            List<string> cities = new List<string>();
            string destinationCity = destinationComboBox.SelectedItem.ToString();

            double distance = 0;
            double realDistance = 0;

            City fromCity = null;
            foreach(City city in cityList)
            {
                if(from == city.GetName())
                {
                    fromCity = city;
                }
            }

            foreach(City connectedCity in connected)
            {
                if (connectedCity.GetIsVisited() != true)
                {
                    int x1 = fromCity.Get_X();
                    int y1 = fromCity.Get_Y();

                    int x2 = connectedCity.Get_X();
                    int y2 = connectedCity.Get_Y();

                    distance = Math.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));
                    string fromTo2 = CountDistanceFromTo(connectedCity.GetName(), destinationCity);
                    realDistance = double.Parse(fromTo2) + distance;
                    cities.Add(connectedCity.GetName() + ";" + Math.Round(realDistance));
                }
            }

            return cities;
        }
        
        private List<City> GetConnectedCityList( List<string> mainCity)
        {
            List<City> cityList2 = new List<City>();

            foreach(City city in cityList)
            {
                for(int i = 0; i < mainCity.Count; i++)
                {
                    if(mainCity[i] == city.GetName() && city.GetIsVisited() != true)
                    {
                        cityList2.Add(city);
                    }
                }
            }

            return cityList2;
        }

        private string CountDistanceFromTo(string from, string to)
        {
            //counts distance from this city to desired city            
            string cityDistance = "";

            City fromCity = null;
            City toCity = null;


            foreach (City city in cityList)
            {
                if (city.GetName() == to)
                {
                    toCity = city;
                }

                if (city.GetName() == from)
                {
                    fromCity = city;
                }
            }

            int x1 = fromCity.Get_X();
            int y1 = fromCity.Get_Y();
            int y2 = toCity.Get_Y();
            int x2 = toCity.Get_X();

            double distance = Math.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));

            cityDistance = Convert.ToInt32(distance).ToString();
            return cityDistance;
        }

        private void FindTheCity(PaintEventArgs e)
        {
            if (startComboBox.SelectedItem != null && destinationComboBox.SelectedItem != null)
            {
                string startingCity = startComboBox.SelectedItem.ToString();
                string destinationCity = destinationComboBox.SelectedItem.ToString();

                //Starting city color
                Graphics grap1 = e.Graphics;
                Pen pen1 = new Pen(Color.Blue, 1);

                //destination city color
                Graphics grap2 = e.Graphics;
                Pen pen2 = new Pen(Color.Red, 1);

                //Starting city
                foreach (City city in cityList)
                {
                    if (city.GetName().Equals(startingCity))
                    {
                        DrawCircle(grap1, pen1, city.Get_X() + 12, city.Get_Y() + 12, 15);
                    }
                }

                //Destination city
                foreach (City city in cityList)
                {
                    if (city.GetName().Equals(destinationCity))
                    {
                        DrawCircle(grap2, pen2, city.Get_X() + 12, city.Get_Y() + 12, 15);
                    }
                }

                int s = 0;                
                foreach (City city in cityList)
                {
                    for (int i = 0; i < city.GetConnectedCities().Count; i++)
                    {
                        foreach (City city2 in cityList)
                        {
                            if (city.GetConnectedCities()[i] == city2.GetName())
                            {
                                int x1 = city.Get_X();
                                int y1 = city.Get_Y();
                                int y2 = city2.Get_Y();
                                int x2 = city2.Get_X();

                                int midX = (x1 + x2) / 2;
                                int midY = (y1 + y2) / 2;

                                string distance = CountDistanceFromTo(city.GetName(), city2.GetName());

                                //Adding distancelabel on the map
                                Label label = new Label();
                                label.Text = distance;
                                label.AutoSize = true;
                                label.Location = new Point(midX + 1, midY + 1);
                                label.BackColor = Color.Transparent;
                                label.ForeColor = Color.Blue;
                                mapPanel.Controls.Add(label);
                            }
                        }
                    }
                    s++;
                }
            }
        }
       
        private void browseButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    path = openFileDialog.FileName;
                    pathTextBox.Text = path;

                    //Devides cities to the object
                    string[] lines = File.ReadAllLines(path);

                    foreach (string line in lines)
                    {
                        List<string> connectedCities = new List<string>();
                        string[] splitted = line.Split(';');
                        string name = splitted[0];
                        int xCoord = int.Parse(splitted[1]);
                        int yCoord = int.Parse(splitted[2]); ;
                        for (int i = 3; i < splitted.Length; i++)
                        {
                            connectedCities.Add(splitted[i]);
                        }
                        cityList.Add(new City(name, xCoord, yCoord, connectedCities));
                    }

                    //Adds the cities to the combo boxes
                    startComboBox.Items.Clear();
                    destinationComboBox.Items.Clear();

                    foreach(City city in cityList)
                        startComboBox.Items.Add(city.GetName());

                    foreach (City city in cityList)
                        destinationComboBox.Items.Add(city.GetName());


                    isClicked = true;
                    this.Invalidate();
                    this.Refresh();
                }
            }
        }
        private void DrawCircle(Graphics g, Pen pen, float centerX, float centerY, float radius)
        {
            g.DrawEllipse(pen, centerX - radius, centerY - radius, radius + radius, radius + radius);
        }

        private void createMapButton_Click(object sender, EventArgs e)
        {
            CreateMap createMap = new CreateMap();
            createMap.Show();
        }
    }
}
