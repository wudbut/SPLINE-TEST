// Worldstar Monitor
// Use: Conglomerates market data and helps one acquire information on the market quickly.
// By: juju
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Forms.DataVisualization.Charting;
using System.Resources;
using System.Reflection;

namespace WorldStarMonitor
{
    public partial class Form1 : Form
    {
        public static string satoshify(string tobesatoshied)
        {
            StringBuilder str = new StringBuilder(tobesatoshied);
            //If the number is even satoshiable
            if (tobesatoshied.Count() > 7)
            {
                int indexOfPoint = tobesatoshied.Count() - 8;
                str.Insert(indexOfPoint, '.');
            }
            return str.ToString();
        }
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        public Form1()
        {
            InitializeComponent();

            //Make donate box readonly
            donateTextBox.ReadOnly = true;

            //Load in all the 3 files, Market List, Coin List, and total List
            int counter = 0;
            string line;
            // Read the file and display it line by line.
            System.IO.StreamReader file = new System.IO.StreamReader("marketlist.txt");
            while ((line = file.ReadLine()) != null)
            {
                this.marketListBox.Items.Add(line.ToString());
                counter++;
            }
            file.Close();

            this.marketTotalListBox.Items.Clear();

            System.IO.StreamReader filec = new System.IO.StreamReader("coinlist.txt");
            while ((line = filec.ReadLine()) != null)
            {
                this.marketTotalListBox.Items.Add(line.ToString());
                counter++;
            }
            filec.Close();

            System.IO.StreamReader