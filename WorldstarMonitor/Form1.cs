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
            public List<Buyorder> buyorders { get; set; }

            //Portfolio Data return
            public string ltcbtclasttradeprice { get; set; }
            public string dogebtclasttradeprice { get; set; }
            public string moonbtclasttradeprice { get; set; }
            public string ftcbtclasttradeprice { get; set; }
            public string frcbtclasttradeprice { get; set; }
            public string cgbbtclasttradeprice { get; set; }
            public string dvcbtclasttradeprice { get; set; }
            public string fortytwobtclasttradeprice { get; set; }
            public string tipsltclasttradeprice { get; set; }
            public string drkbtclasttradeprice { get; set; }
            public string lotbtclasttradeprice { get; set; }
            public string xpmbtclasttradeprice { get; set; }
            public string zetbtclasttradeprice { get; set; }
            public string frkbtclasttradeprice { get; set; }
            public string btbbtclasttradeprice { get; set; }

            //Coinbase and btce USD Btc price
            public string coinbasevalue { get; set; }
            public string btcevalue { get; set; }

            //Blockchain Rest Stuff
            public string totalbc { get; set; } //- https://blockchain.info/q/totalbc
            public string difficulty { get; set; }// - https://blockchain.info/q/getdifficulty
            public string rewardtotal { get; set; } //- https://blockchain.info/q/bcperblock
            public string blockheight { get; set; }// - https://blockchain.info/q/getblockcount
            public string hrprice { get; set; }// - https://blockchain.info/q/24hrprice
            public string hrtransactions { get; set; }// - https://blockchain.info/q/24hrtransactioncount
            public string hrbtcsent { get; set; }// - https://blockchain.info/q/24hrbtcsent
            public string nethashrate { get; set; } //- https://blockchain.info/q/hashrate
            public string marketcap { get; set; } //- https://blockchain.info/q/marketcap
            public string probability { get; set; } //- https://blockchain.info/q/probability
            public string addrbalance { get; set; } //- https://blockchain.info/q/addressbalance/
            
            //Time the last update occured
            public string time { get; set; }

        }
        public static void cmd_DataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine("Output from other process");
            Console.WriteLine(e.Data);
            //Form1.textBox61.Text = e.Data.ToString();
        }
        //Progress Bar percentage update
        void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            RESTAPIWorkerProgress.Value = e.ProgressPercentage;
            this.RESTAPIWorkerPercentageLabel.Text = e.ProgressPercentage + "%";
        }
        public static void cmd_Error(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine("Error from other process");
            Console.WriteLine(e.Data);
        }
        public static void launchminer(string miningcommand)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo = new ProcessStartInfo("cmd.exe", "/c " + miningcommand);
            Process.Start(startInfo);
        }
        public static string InvokeStringMethod(string typeName, string methodName)
        {
            // Get the Type for the class
            Type calledType = Type.GetType(typeName);

            // Invoke the method itself. The string returned by the method winds up in s
            string s = (string)calledType.InvokeMember(
                            methodName,
                            BindingFlags.InvokeMethod | BindingFlags.Public |
                                BindingFlags.Static,
                            null,
                            null,
                            null);

            // Return the string that was returned by the called method.
            return s;
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //
            // e.Argument always contains whatever was sent to the background worker
            // in RunWorkerAsync. We can simply cast it to its original type.
            //
            RESTAPIworker.ReportProgress(1);
            RESTAPIworker.ReportProgress(10);
            restCalls argumentTest = e.Argument as restCalls;
            //else make sure that they enter an address that is atleast the length of a real address
            RESTAPIworker.ReportProgress(15);


            RESTAPIworker.ReportProgress(20);
            RESTAPIworker.ReportProgress(25);
            WorldStarMonitor.Form1.RootObject allmarketdata = REST_GET("api.php?method=marketdatav2");
            string currentMarketSelected = argumentTest.label;
            if (currentMarketSelected == "42BTC")
            {
                currentMarketSelected = "fortytwoBTC";
            }

            var property = allmarketdata.@return.markets.GetType().GetProperty(currentMarketSelected);
            dynamic market = property.GetMethod.Invoke(allmarketdata.@return.markets, null);

            argumentTest.label = market.label;
            argumentTest.lasttradeprice = market.lasttradeprice;
            argumentTest.lasttradetime = market.lasttradetime;
            argumentTest.marketid = market.marketid;
            argumentTest.primarycode = market.primarycode;
            argumentTest.primaryname = market.primaryname;
            argumentTest.recenttrades = market.recenttrades;
            argumentTest.secondarycode = market.secondarycode;
            argumentTest.secondaryname = market.secondaryname;
            argumentTest.sellorders = market.sellorders;
            argumentTest.volume = market.volume;
            argumentTest.buyorders = market.buyorders;

            //Pass/SET in the lasttradeprice of ALL 