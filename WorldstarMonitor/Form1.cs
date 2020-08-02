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
            this.dailyTransactionTotalLabel.Text = "0";
            this.dailyBitcoinSentLabel.Text = "0";
            this.blockHeightLabel.Text = "0";
            this.blockRewardLabel.Text = "0";
            this.networkHashrateLabel.Text = "0";
            this.probabilitySolveLabel.Text = "0";

            //Address Storage Fields populated from settings file
            this.dwallet.Text = Properties.Settings.Default.dwallet;
            this.dexchange.Text = Properties.Settings.Default.dexchange;
            this.ddonation.Text = Properties.Settings.Default.ddonation;

            this.bwallet.Text = Properties.Settings.Default.bwallet;
            this.bexchange.Text = Properties.Settings.Default.bexchange;
            this.bdonation.Text = Properties.Settings.Default.bdonation;

            this.lwallet.Text = Properties.Settings.Default.lwallet;
            this.lexchange.Text = Properties.Settings.Default.lexchange;
            this.ldonation.Text = Properties.Settings.Default.ldonation;

            //Extra Address Storage Fields populated from settings file
            this.a1.Text = Properties.Settings.Default.a1;
            this.a2.Text = Properties.Settings.Default.a2;
            this.a3.Text = Properties.Settings.Default.a3;
            this.a4.Text = Properties.Settings.Default.a4;
            this.a5.Text = Properties.Settings.Default.a5;
            this.a6.Text = Properties.Settings.Default.a6;
            this.a7.Text = Properties.Settings.Default.a7;
            this.a8.Text = Properties.Settings.Default.a8;
            this.a9.Text = Properties.Settings.Default.a9;
            this.a10.Text = Properties.Settings.Default.a10;
            this.a11.Text = Properties.Settings.Default.a11;

            //Mining command storage fields populated from settings file
            this.mc1.Text = Properties.Settings.Default.mc1;
            this.mc2.Text = Properties.Settings.Default.mc2;
            this.mc3.Text = Properties.Settings.Default.mc3;
            this.mc4.Text = Properties.Settings.Default.mc4;
            this.mc5.Text = Properties.Settings.Default.mc5;
            this.mc6.Text = Properties.Settings.Default.mc6;
            this.mc7.Text = Properties.Settings.Default.mc7;
            this.mc8.Text = Properties.Settings.Default.mc8;
            
            //Mining command coin name storage fields populated from settings field
            this.mcn1.Text = Properties.Settings.Default.mcn1;
            this.mcn2.Text = Properties.Settings.Default.mcn2;
            this.mcn3.Text = Properties.Settings.Default.mcn3;
            this.mcn4.Text = Properties.Settings.Default.mcn4;
            this.mcn5.Text = Properties.Settings.Default.mcn5;
            this.mcn6.Text = Properties.Settings.Default.mcn6;
            this.mcn7.Text = Properties.Settings.Default.mcn7;
            this.mcn8.Text = Properties.Settings.Default.mcn8;

            this.marketListBox.SetSelected(0, true);
            //If the textbox for wallet addr is blank put in text


            restCalls restReturnData = new restCalls
            {
                coinbasevalue = "",
                btcevalue = "",
                time = "",
                label = "LTCBTC"
            };

            RESTAPIworker.WorkerReportsProgress = true;
            RESTAPIworker.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
            
            //Send argument to our RESTAPI Worker Thread
            RESTAPIworker.RunWorkerAsync(restReturnData);

            //Generate a timer that has an event handler of a tick interval of 10 seconds to update the application
            timer.Tick += new EventHandler(timer_Tick); // Everytime timer ticks, timer_Tick will be called
            timer.Interval = (100) * (1);             // Timer will tick every 1 second
            timer.Enabled = true;                       // Enable the timer
            timer.Start();
        }

        class restCalls
        {
            //Current selected market data return
            public string marketid { get; set; }
            public string label { get; set; }
            public string lasttradeprice { get; set; }
            public string volume { get; set; }
            public string lasttradetime { get; set; }
            public string primaryname { get; set; }
            public string primarycode { get; set; }
            public string secondaryname { get; set; }
            public string secondarycode { get; set; }
            public List<Recenttrade> recenttrades { get; set; }
            public List<Sellorder> sellorders { get; set; }
            public List<Buyorder> buyor