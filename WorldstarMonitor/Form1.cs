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

            System.IO.StreamReader filet = new System.IO.StreamReader("totallist.txt");
            while ((line = filet.ReadLine()) != null)
            {
                this.totalCoinListBox.Items.Add(line.ToString());
                counter++;
            }
            filet.Close();

            //Initiate the QuickPriceListBox with 0 value
            for (int i = 0; i < 5; i++)
            {
                this.quickPriceListBox.Items.Add("0");
            }

            //Set the miner combo box to the first mining tool in the index
            this.minerSelector.SelectedIndex = 0;

            quickStatRatio.ReadOnly = true;
            quickStatVolume.ReadOnly = true;
            quickLastTradeTime.ReadOnly = true;
            quickCryptsyMarketID.ReadOnly = true;
            quickMarketSymbol.ReadOnly = true;

            //Make my donation box hidden so its a copyable label basically
            donateTextBox.BorderStyle = 0;
            donateTextBox.BackColor = this.BackColor;
            donateTextBox.TabStop = false;

            //Last time checked, loading data for the initialization
            this.lastUpdateTimeLabel.Text = "Loading Data";

            //Labels to display Current Bitcoin price from Btce and coinbase
            this.coinbasePriceLabel.Text = "0000.00" + " USD";
            this.btcePriceLabel.Text = "0000.00" + " USD";

            //Blockchain.info restapi query labels
            this.totalBitcoinLabel.Text = "0";
            this.totalMarketCapLabel.Text = "0";
            this.dailyAVGPriceLabel.Text = "0";
            this.dailyTransactionTotalLabe