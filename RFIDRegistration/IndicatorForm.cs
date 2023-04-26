using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

// Json Serialization and Deserialization Library
using Newtonsoft.Json;

// MQTT Library
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Net.Http;
using RFIDRegistration.Models;
using System.Data.SqlClient;

namespace RFIDRegistration
{
    public partial class IndicatorForm : Form
    {
        /* ========== Global Variable ========== */

        //WIRIO3_68B6B329AB94
        //WIRIO3_58BF25A84F04

        /* ========== COM Port Comm ========== */
        public bool indicatorChoosen = false;
        public String DataOut, DataIn, CurrentLine;
        public StringBuilder Buffer = new StringBuilder();
        int countBracketLeft = 0;
        int countBracketRight = 0;
        delegate void SetTextCallBack(string text);

        // MQTT Variable
        MqttClient client;
        string clientID_One;
        bool ConnectionStatus = false;

        /* ========== COM Port Comm ========== */

        /* ========== MQTT ========== */
        public String DataOut_03, DataIn_03, CurrentLine_03;
        public StringBuilder Buffer_02 = new StringBuilder();
        delegate void SetTextCallBack_02(string text_02);

        // MQTT Variable
        MqttClient client_03;
        string clientID_One_03;
        bool ConnectionStatus_03 = false;
        bool RC_1 = false, RC_2 = false, RC_3 = false, RC_4 = false, RC_5 = false, RC_6 = false;

        /* ========== MQTT ========== */

        // RPC Config
        public static IndicatorForm instance;
        public TextBox RPCTxt;
        public Button PubAttb, PubRPCReq;

        // Tag Config
        public TagConfig tagConfig = new TagConfig();
        public TextBox TagConfigTxt;

        // Address of SQL Server and Database
        SqlConnect sqlConnect = new SqlConnect();

        private void IndicatorForm_Load(object sender, EventArgs e)
        {
            sqlConnect.Connection();
            sqlConnect.con.Close();

            if (lblStatusCom.Text == "Disconnected")
            {
                string[] ports = SerialPort.GetPortNames();
                cBoxComPort.Items.AddRange(ports);
            }

            if (indicatorChoosen == false)
            {
                this.ClientSize = new Size(1350, 760);
                this.Text = "RFID Reader Tag";
            }
            else
            {
                this.ClientSize = new Size(1350, 760);
            }

            /* ========== COM Port Comm Initialize ========== */

            // -- DTR CheckBox --
            chBoxDtrEnable.Checked = true;
            serialPort1.DtrEnable = false;

            // -- RTS CheckBox --
            chBoxRTSEnable.Checked = true;
            serialPort1.RtsEnable = false;

            // -- Button --
            btnOpen.Enabled = true;
            btnClose.Enabled = false;
            btnSendData.Enabled = false;
            btnClearDataOut.Enabled = false;
            cBoxSelectionCommand.Enabled = false;
            btnClearDataIn.Enabled = false;
            btnRPCConfig_USB.Enabled = false;

            cBoxBaudRate.SelectedIndex = 8;
            cBoxDataBits.SelectedIndex = 2;
            cBoxStopBits.SelectedIndex = 0;
            cBoxParityBits.SelectedIndex = 0;

            /* ========== COM Port Comm Initialize ========== */

            /* ========== Device_MQTT Initialize ========== */

            // -- Button --
            listView_MQTTList.Columns.Add("No.", 30, HorizontalAlignment.Center);
            listView_MQTTList.Columns.Add("WIRIO ID", 200, HorizontalAlignment.Center);
            listView_MQTTList.Columns.Add("IP Address", 200, HorizontalAlignment.Center);
            listView_MQTTList.Columns.Add("Connection Status", 120, HorizontalAlignment.Center);
            listView_MQTTList.GridLines = true;
            listView_MQTTList.View = View.Details;

            btnConnectMQTT_03.Enabled = false;
            btnDisconnectMQTT_03.Enabled = false;
            btnSubTele_03.Enabled = false;
            tBoxPublish_03.Enabled = false;

            listView_MQTTList.Items.Clear();

            /* ========== Device_MQTT Initialize ========== */
        }

        public IndicatorForm()
        {
            InitializeComponent();
            MQTTGroupBoxSelection();
            instance = this;
            RPCTxt = tBoxPublish_03;
            PubRPCReq = btnPubRPCReq_03;
            PubAttb = btnPubDevUpAttb_03;

            btnPubRPCReq_03.Enabled = false;
            btnPubDevUpAttb_03.Enabled = false;
        }

        public void MQTTGroupBoxSelection()
        {
            if (indicatorChoosen == true)
            {
                groupBox1.Enabled = true;
                groupBox16.Enabled = false;
            }
            else if (indicatorChoosen == false)
            {
                groupBox1.Enabled = false;
                groupBox16.Enabled = true;
            }
        }

        public void HttpClientConnection(Object object_)
        {
            if (indicatorChoosen == false)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:60243/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    var response = client.PostAsJsonAsync("api/rfidconnection", object_).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Send Successfully");
                    }
                    else
                    {
                        MessageBox.Show("Error Code " + response.StatusCode + " : Message - " + response.ReasonPhrase);
                    }
                }
            }
            else
            {
                using (var client = new HttpClient())
                {
                    //client.BaseAddress = new Uri("http://mxdashboard.local/");
                    client.BaseAddress = new Uri("http://localhost:60243/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    var response = client.PostAsJsonAsync("api/batterydetail", object_).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        //MessageBox.Show("Send Successfully");
                    }
                    else
                    {
                        MessageBox.Show("Error Code " + response.StatusCode + " : Message - " + response.ReasonPhrase);
                    }
                }
            }
        }

        /* ========== COM Port Functions ========== */

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            DataIn = serialPort1.ReadExisting();

            /*  Checking whether the Json Data format with Bracket Left and Right
                Is the same or not, If different the Data will wait for the next Buffer
                and Append into StringBuilder until the Bracket Left and Right are the same. 
            */
            foreach (char c in DataIn)
            {
                if (c == '{')
                {
                    // Add Left Bracket once
                    countBracketLeft++;
                }
                else if (c == '}')
                {
                    // Add Right Bracket once
                    countBracketRight++;
                }

                if (countBracketLeft == countBracketRight)
                {
                    Buffer.Append(c);
                    CurrentLine = Buffer.ToString();
                    Buffer.Clear(); // Must define clear everytime for flushing purpose to prevent duplicate
                    Eval_String(CurrentLine);
                }
                else if (countBracketLeft != countBracketRight)
                {
                    Buffer.Append(c);
                }
            }
        }

        private void Eval_String(string s)
        {
            if (sqlConnect.con.State == ConnectionState.Closed)
            {
                sqlConnect.con.Open();
            }
            
            if (this.tBoxDataIn.InvokeRequired)
            {
                SetTextCallBack d = new SetTextCallBack(Eval_String);
                this.Invoke(d, new object[] { s });

                if (indicatorChoosen == false)
                {
                    // RFID Reader Tag :
                    RFID_detail_Json_Attr RFID_Details_Inv = JsonConvert.DeserializeObject<RFID_detail_Json_Attr>(s);

                    if (RFID_Details_Inv.Device_ID != null)
                    {
                        if (s.Contains("attributes"))
                        {
                            if (s.Contains("d.sys.addr") && s.Contains("d.sys.fwid"))
                            {
                                if (sqlConnect.con.State == System.Data.ConnectionState.Open)
                                {
                                    SqlDataAdapter DA_1 = new SqlDataAdapter("Insert_DetailList", sqlConnect.con);
                                    DA_1.SelectCommand.CommandType = CommandType.StoredProcedure;

                                    SqlDataAdapter DA_2 = new SqlDataAdapter("Insert_RFID_Attributes", sqlConnect.con);
                                    DA_2.SelectCommand.CommandType = CommandType.StoredProcedure;

                                    DA_1.SelectCommand.Parameters.Add("@device_id", SqlDbType.VarChar).Value = RFID_Details_Inv.Device_ID;
                                    DA_1.SelectCommand.Parameters.Add("@pkt_no", SqlDbType.VarChar).Value = RFID_Details_Inv.Pktno;
                                    DA_1.SelectCommand.Parameters.Add("@rssi", SqlDbType.VarChar).Value = RFID_Details_Inv.Wifi_Received_Signal_Strength;
                                    DA_1.SelectCommand.Parameters.Add("@ip_address", SqlDbType.VarChar).Value = RFID_Details_Inv.IP_Address;
                                    DA_1.SelectCommand.Parameters.Add("@connection", SqlDbType.VarChar).Value = RFID_Details_Inv.Connection;
                                    DA_1.SelectCommand.Parameters.Add("@connection_type", SqlDbType.VarChar).Value = RFID_Details_Inv.Connection_Type;
                                    DA_1.SelectCommand.Parameters.Add("@ssid", SqlDbType.VarChar).Value = RFID_Details_Inv.SSID;
                                    DA_1.SelectCommand.Parameters.Add("@fwid", SqlDbType.VarChar).Value = RFID_Details_Inv.FWID;
                                    DA_1.SelectCommand.Parameters.Add("@date_code", SqlDbType.VarChar).Value = RFID_Details_Inv.Date_Code;

                                    // Device Desc ::
                                    DA_1.SelectCommand.Parameters.Add("@device_model", SqlDbType.VarChar).Value = RFID_Details_Inv.Model;
                                    DA_1.SelectCommand.Parameters.Add("@device_name", SqlDbType.VarChar).Value = RFID_Details_Inv.Device_Name;
                                    DA_1.SelectCommand.Parameters.Add("@device_desc", SqlDbType.VarChar).Value = RFID_Details_Inv.Device_Desc;

                                    // Epoch ::
                                    DA_1.SelectCommand.Parameters.Add("@epoch_valid", SqlDbType.VarChar).Value = RFID_Details_Inv.Epoch_Valid;
                                    DA_1.SelectCommand.Parameters.Add("@epoch_sec", SqlDbType.VarChar).Value = RFID_Details_Inv.Epoch_Sec;

                                    DA_1.SelectCommand.Parameters.Add("@rpc_busy", SqlDbType.VarChar).Value = RFID_Details_Inv.Rpc_Busy;
                                    DA_1.SelectCommand.Parameters.Add("@channel_no", SqlDbType.VarChar).Value = RFID_Details_Inv.Channel_No;
                                    DA_1.SelectCommand.Parameters.Add("@power_max", SqlDbType.VarChar).Value = RFID_Details_Inv.Power_Max;
                                    DA_1.SelectCommand.Parameters.Add("@power_min", SqlDbType.VarChar).Value = RFID_Details_Inv.Power_Min;
                                    DA_1.SelectCommand.Parameters.Add("@cachespace_max", SqlDbType.VarChar).Value = RFID_Details_Inv.CacheSpace_Max;

                                    // Anthenna Output ::
                                    if (s.Contains("d.uhfrfid.antison"))
                                    {
                                        DA_1.SelectCommand.Parameters.Add("@anthenna_output", SqlDbType.VarChar).Value
                                        = RFID_Details_Inv.Anthenna_Output[0] + "," + RFID_Details_Inv.Anthenna_Output[1] + "," + RFID_Details_Inv.Anthenna_Output[2] + "," + RFID_Details_Inv.Anthenna_Output[3];
                                    }
                                    else
                                    {
                                        DA_1.SelectCommand.Parameters.Add("@anthenna_output", SqlDbType.VarChar).Value = "";
                                    }

                                    // RFID Mode ::
                                    if (s.Contains("d.uhfrfid.antstate"))
                                    {
                                        DA_1.SelectCommand.Parameters.Add("@rfid_mode", SqlDbType.VarChar).Value
                                        = RFID_Details_Inv.RFID_Mode[0] + "," + RFID_Details_Inv.RFID_Mode[1] + "," + RFID_Details_Inv.RFID_Mode[2] + "," + RFID_Details_Inv.RFID_Mode[3];
                                    }
                                    else
                                    {
                                        DA_1.SelectCommand.Parameters.Add("@rfid_mode", SqlDbType.VarChar).Value = "";
                                    }

                                    DA_1.SelectCommand.Parameters.Add("@rfid_temp", SqlDbType.VarChar).Value = RFID_Details_Inv.RFID_Temp;
                                    DA_1.SelectCommand.Parameters.Add("@rfid_firmware_id", SqlDbType.VarChar).Value = RFID_Details_Inv.RFID_Firmware_ID;
                                    DA_1.SelectCommand.Parameters.Add("@read_period", SqlDbType.VarChar).Value = RFID_Details_Inv.Read_Period;
                                    DA_1.SelectCommand.Parameters.Add("@auto", SqlDbType.VarChar).Value = RFID_Details_Inv.Auto;
                                    DA_1.SelectCommand.Parameters.Add("@period", SqlDbType.VarChar).Value = RFID_Details_Inv.Period;

                                    // Input Trigger ::
                                    if (s.Contains("s.uhfrfid.inptrig"))
                                    {
                                        DA_1.SelectCommand.Parameters.Add("@input_trigger", SqlDbType.VarChar).Value = RFID_Details_Inv.Input_Trigger[0] + "," + RFID_Details_Inv.Input_Trigger[1];
                                    }
                                    else
                                    {
                                        DA_1.SelectCommand.Parameters.Add("@input_trigger", SqlDbType.VarChar).Value = "";
                                    }

                                    DA_1.SelectCommand.Parameters.Add("@cache_tag_remove", SqlDbType.VarChar).Value = RFID_Details_Inv.Cache_Tag_Remove;
                                    DA_1.SelectCommand.Parameters.Add("@cache_period", SqlDbType.VarChar).Value = RFID_Details_Inv.Cache_Period;
                                    DA_1.SelectCommand.Parameters.Add("@tag_remove_upd", SqlDbType.VarChar).Value = RFID_Details_Inv.Tag_Remove_Upd;
                                    DA_1.SelectCommand.Parameters.Add("@anthenna_channel_upd", SqlDbType.VarChar).Value = RFID_Details_Inv.Anthenna_Channel_Upd;
                                    DA_1.SelectCommand.Parameters.Add("@enbforce_upd", SqlDbType.VarChar).Value = RFID_Details_Inv.EnbForce_Upd;
                                    DA_1.SelectCommand.Parameters.Add("@force_update_period", SqlDbType.VarChar).Value = RFID_Details_Inv.Force_Update_Period;

                                    // Filter Options ::
                                    DA_1.SelectCommand.Parameters.Add("@self_filter_option", SqlDbType.VarChar).Value = RFID_Details_Inv.Self_Filter_Option;
                                    DA_1.SelectCommand.Parameters.Add("@self_filter_addr", SqlDbType.VarChar).Value = RFID_Details_Inv.Self_Filter_Addr;
                                    DA_1.SelectCommand.Parameters.Add("@self_filter_len_bit", SqlDbType.VarChar).Value = RFID_Details_Inv.Self_Filter_Len_Bit;
                                    DA_1.SelectCommand.Parameters.Add("@self_filter_data", SqlDbType.VarChar).Value = RFID_Details_Inv.Self_Filter_Data;
                                    DA_1.SelectCommand.Parameters.Add("@self_filter_invert", SqlDbType.VarChar).Value = RFID_Details_Inv.Self_Filter_Invert;

                                    // Tag Data ::
                                    DA_1.SelectCommand.Parameters.Add("@enb_tag_data", SqlDbType.VarChar).Value = RFID_Details_Inv.Enb_Tag_Data;
                                    DA_1.SelectCommand.Parameters.Add("@tag_data_mem_bank", SqlDbType.VarChar).Value = RFID_Details_Inv.Tag_Data_Mem_Bank;
                                    DA_1.SelectCommand.Parameters.Add("@tag_data_reader_addr", SqlDbType.VarChar).Value = RFID_Details_Inv.Tag_Data_Reader_Addr;
                                    DA_1.SelectCommand.Parameters.Add("@tag_data_word_count", SqlDbType.VarChar).Value = RFID_Details_Inv.Tag_Data_Word_Count;
                                    DA_1.SelectCommand.Parameters.Add("@enbtagpass", SqlDbType.VarChar).Value = RFID_Details_Inv.EnbTagPass;
                                    DA_1.SelectCommand.Parameters.Add("@tag_pass", SqlDbType.VarChar).Value = RFID_Details_Inv.Tag_Pass;

                                    DA_1.SelectCommand.Parameters.Add("@dynamic_q", SqlDbType.VarChar).Value = RFID_Details_Inv.Dynamic_Q;
                                    DA_1.SelectCommand.Parameters.Add("@q", SqlDbType.VarChar).Value = RFID_Details_Inv.Q;
                                    DA_1.SelectCommand.Parameters.Add("@se_mode", SqlDbType.VarChar).Value = RFID_Details_Inv.Se_Mode;
                                    DA_1.SelectCommand.Parameters.Add("@target", SqlDbType.VarChar).Value = RFID_Details_Inv.Target;
                                    DA_1.SelectCommand.Parameters.Add("@epc_extended", SqlDbType.VarChar).Value = RFID_Details_Inv.EPC_Extended;

                                    // Anthenna Mode ::
                                    if (s.Contains("s.uhfrfid.antenb"))
                                    {
                                        DA_1.SelectCommand.Parameters.Add("@anthenna_mode", SqlDbType.VarChar).Value
                                        = RFID_Details_Inv.Anthenna_Mode[0] + "," + RFID_Details_Inv.Anthenna_Mode[1] + "," + RFID_Details_Inv.Anthenna_Mode[2] + "," + RFID_Details_Inv.Anthenna_Mode[3];
                                    }
                                    else
                                    {
                                        DA_1.SelectCommand.Parameters.Add("@anthenna_mode", SqlDbType.VarChar).Value = "";
                                    }

                                    // Current Power ::
                                    if (s.Contains("s.uhfrfid.power"))
                                    {
                                        DA_1.SelectCommand.Parameters.Add("@rfid_current_power", SqlDbType.VarChar).Value
                                        = RFID_Details_Inv.RFID_Current_Power[0] + "," + RFID_Details_Inv.RFID_Current_Power[1] + "," + RFID_Details_Inv.RFID_Current_Power[2] + "," + RFID_Details_Inv.RFID_Current_Power[3];
                                    }
                                    else
                                    {
                                        DA_1.SelectCommand.Parameters.Add("@rfid_current_power", SqlDbType.VarChar).Value = "";
                                    }

                                    DA_1.SelectCommand.Parameters.Add("@demo_mode", SqlDbType.VarChar).Value = RFID_Details_Inv.Demo_Mode;
                                    // Temp Sensor ::
                                    DA_1.SelectCommand.Parameters.Add("@rate_control", SqlDbType.VarChar).Value = RFID_Details_Inv.Rate_Control;
                                    DA_1.SelectCommand.Parameters.Add("@rate_control_period", SqlDbType.VarChar).Value = RFID_Details_Inv.Rate_Control_Period;

                                    DA_1.SelectCommand.ExecuteNonQuery();

                                    for (int x = 0; x < RFID_Details_Inv.Perip_Items.Count; x++)
                                    {
                                        DA_2.SelectCommand.Parameters.Clear();
                                        DA_2.SelectCommand.Parameters.Add("@Option", SqlDbType.VarChar).Value = "Insert_PeripharalItems";
                                        DA_2.SelectCommand.Parameters.Add("@cmdkey", SqlDbType.VarChar).Value = RFID_Details_Inv.Perip_Items[x].Cmd_Key;
                                        DA_2.SelectCommand.Parameters.Add("@feature", SqlDbType.VarChar).Value = RFID_Details_Inv.Perip_Items[x].Feature;
                                        DA_2.SelectCommand.Parameters.Add("@ipp_ch", SqlDbType.VarChar).Value = "";
                                        DA_2.SelectCommand.Parameters.Add("@ipp_onishigh", SqlDbType.VarChar).Value = "";
                                        DA_2.SelectCommand.Parameters.Add("@ipp_mode", SqlDbType.VarChar).Value = "";
                                        DA_2.SelectCommand.Parameters.Add("@ipp_debounce", SqlDbType.VarChar).Value = "";
                                        DA_2.SelectCommand.Parameters.Add("@opp_ch", SqlDbType.VarChar).Value = "";
                                        DA_2.SelectCommand.Parameters.Add("@opp_onishigh", SqlDbType.VarChar).Value = "";
                                        DA_2.SelectCommand.Parameters.Add("@opp_mode", SqlDbType.VarChar).Value = "";
                                        DA_2.SelectCommand.Parameters.Add("@opp_fperiodon", SqlDbType.VarChar).Value = "";
                                        DA_2.SelectCommand.Parameters.Add("@opp_fperiodoff", SqlDbType.VarChar).Value = "";
                                        DA_2.SelectCommand.Parameters.Add("@opp_pulsecnt", SqlDbType.VarChar).Value = "";
                                        DA_2.SelectCommand.ExecuteNonQuery();
                                    }

                                    if (s.Contains("s.outputport.setup"))
                                    {
                                        for (int y = 0; y < RFID_Details_Inv.OutputSetup.Count; y++)
                                        {
                                            DA_2.SelectCommand.Parameters.Clear();
                                            DA_2.SelectCommand.Parameters.Add("@Option", SqlDbType.VarChar).Value = "Insert_OutputConfig";
                                            DA_2.SelectCommand.Parameters.Add("@cmdkey", SqlDbType.VarChar).Value = "";
                                            DA_2.SelectCommand.Parameters.Add("@feature", SqlDbType.VarChar).Value = "";
                                            DA_2.SelectCommand.Parameters.Add("@ipp_ch", SqlDbType.VarChar).Value = "";
                                            DA_2.SelectCommand.Parameters.Add("@ipp_onishigh", SqlDbType.VarChar).Value = "";
                                            DA_2.SelectCommand.Parameters.Add("@ipp_mode", SqlDbType.VarChar).Value = "";
                                            DA_2.SelectCommand.Parameters.Add("@ipp_debounce", SqlDbType.VarChar).Value = "";
                                            DA_2.SelectCommand.Parameters.Add("@opp_ch", SqlDbType.VarChar).Value = RFID_Details_Inv.OutputSetup[y].Channel;
                                            DA_2.SelectCommand.Parameters.Add("@opp_onishigh", SqlDbType.VarChar).Value = RFID_Details_Inv.OutputSetup[y].On_isHigh;
                                            DA_2.SelectCommand.Parameters.Add("@opp_mode", SqlDbType.VarChar).Value = RFID_Details_Inv.OutputSetup[y].Mode_Output;
                                            DA_2.SelectCommand.Parameters.Add("@opp_fperiodon", SqlDbType.VarChar).Value = RFID_Details_Inv.OutputSetup[y].F_Period_On;
                                            DA_2.SelectCommand.Parameters.Add("@opp_fperiodoff", SqlDbType.VarChar).Value = RFID_Details_Inv.OutputSetup[y].F_Period_Off;
                                            DA_2.SelectCommand.Parameters.Add("@opp_pulsecnt", SqlDbType.VarChar).Value = RFID_Details_Inv.OutputSetup[y].Pulse_Cnt;
                                            DA_2.SelectCommand.ExecuteNonQuery();
                                        }
                                    }

                                    if (s.Contains("s.inputport.setup"))
                                    {
                                        for (int z = 0; z < RFID_Details_Inv.InputSetup.Count; z++)
                                        {
                                            DA_2.SelectCommand.Parameters.Clear();
                                            DA_2.SelectCommand.Parameters.Add("@Option", SqlDbType.VarChar).Value = "Insert_InputConfig";
                                            DA_2.SelectCommand.Parameters.Add("@cmdkey", SqlDbType.VarChar).Value = "";
                                            DA_2.SelectCommand.Parameters.Add("@feature", SqlDbType.VarChar).Value = "";
                                            DA_2.SelectCommand.Parameters.Add("@ipp_ch", SqlDbType.VarChar).Value = RFID_Details_Inv.InputSetup[z].Channel;
                                            DA_2.SelectCommand.Parameters.Add("@ipp_onishigh", SqlDbType.VarChar).Value = RFID_Details_Inv.InputSetup[z].On_isHigh;
                                            DA_2.SelectCommand.Parameters.Add("@ipp_mode", SqlDbType.VarChar).Value = RFID_Details_Inv.InputSetup[z].Mode_Output;
                                            DA_2.SelectCommand.Parameters.Add("@ipp_debounce", SqlDbType.VarChar).Value = RFID_Details_Inv.InputSetup[z].Debounce;
                                            DA_2.SelectCommand.Parameters.Add("@opp_ch", SqlDbType.VarChar).Value = "";
                                            DA_2.SelectCommand.Parameters.Add("@opp_onishigh", SqlDbType.VarChar).Value = "";
                                            DA_2.SelectCommand.Parameters.Add("@opp_mode", SqlDbType.VarChar).Value = "";
                                            DA_2.SelectCommand.Parameters.Add("@opp_fperiodon", SqlDbType.VarChar).Value = "";
                                            DA_2.SelectCommand.Parameters.Add("@opp_fperiodoff", SqlDbType.VarChar).Value = "";
                                            DA_2.SelectCommand.Parameters.Add("@opp_pulsecnt", SqlDbType.VarChar).Value = "";
                                            DA_2.SelectCommand.ExecuteNonQuery();
                                        }
                                    }

                                }
                                else
                                {
                                    tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                                            + "-----------------------------------------" + Environment.NewLine
                                            + "SQL Connection is Failure, Please Try Again!!"
                                            + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                                            + "-----------------------------------------");
                                }
                            }

                        }
                        else if (s.Contains("telematics"))
                        {
                            if (s.Contains("t.uhfrfid.temp"))
                            {
                                RFID_Tag_Json TagJson = JsonConvert.DeserializeObject<RFID_Tag_Json>(s);

                                if (sqlConnect.con.State == System.Data.ConnectionState.Open)
                                {
                                    SqlDataAdapter DA = new SqlDataAdapter("Insert_RFID_Temp", sqlConnect.con);
                                    DA.SelectCommand.CommandType = CommandType.StoredProcedure;

                                    DA.SelectCommand.Parameters.Add("@device_id", SqlDbType.VarChar).Value = TagJson.Device_ID;
                                    DA.SelectCommand.Parameters.Add("@rssi", SqlDbType.VarChar).Value = TagJson.RSSI;
                                    DA.SelectCommand.Parameters.Add("@pkt_no", SqlDbType.VarChar).Value = TagJson.Pkt_No;
                                    DA.SelectCommand.Parameters.Add("@device_temp", SqlDbType.VarChar).Value = TagJson.Device_Temp;

                                    DA.SelectCommand.ExecuteNonQuery();
                                }
                                else
                                {
                                    tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                                        + "-----------------------------------------" + Environment.NewLine
                                        + "SQL Connection is Failure, Please Try Again!!"
                                        + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                                        + "-----------------------------------------");
                                }
                            }
                            else if (s.Contains("t.uhfrfid.param"))
                            {
                                RFID_Tag_Json TagJson = JsonConvert.DeserializeObject<RFID_Tag_Json>(s);

                                if (sqlConnect.con.State == System.Data.ConnectionState.Open)
                                {
                                    if (TagJson.Telematics_Data.Count > 0)
                                    {
                                        for (int countTeleData = 0; countTeleData < TagJson.Telematics_Data.Count(); countTeleData++)
                                        {
                                            SqlDataAdapter DA = new SqlDataAdapter("Insert_ScanResult", sqlConnect.con);
                                            DA.SelectCommand.CommandType = CommandType.StoredProcedure;

                                            DA.SelectCommand.Parameters.Add("@device_id", SqlDbType.VarChar).Value = TagJson.Device_ID;
                                            DA.SelectCommand.Parameters.Add("@tag_rssi", SqlDbType.VarChar).Value = TagJson.RSSI;
                                            DA.SelectCommand.Parameters.Add("@pkt_no", SqlDbType.VarChar).Value = TagJson.Pkt_No;

                                            DA.SelectCommand.Parameters.Add("@time_stamp", SqlDbType.VarChar).Value = TagJson.Telematics_Data[countTeleData].Time_Stamp;
                                            DA.SelectCommand.Parameters.Add("@state", SqlDbType.VarChar).Value = TagJson.Telematics_Data[countTeleData].State;
                                            DA.SelectCommand.Parameters.Add("@channel", SqlDbType.VarChar).Value = TagJson.Telematics_Data[countTeleData].Channel;
                                            DA.SelectCommand.Parameters.Add("@tele_rssi", SqlDbType.VarChar).Value = TagJson.Telematics_Data[countTeleData].RSSI;
                                            DA.SelectCommand.Parameters.Add("@tag_id", SqlDbType.VarChar).Value = TagJson.Telematics_Data[countTeleData].Tag_ID;

                                            DA.SelectCommand.ExecuteNonQuery();
                                        }
                                    }
                                }
                                else
                                {
                                    tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                                        + "-----------------------------------------" + Environment.NewLine
                                        + "SQL Connection is Failure, Please Try Again!!"
                                        + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                                        + "-----------------------------------------");
                                }
                            }
                            else if (s.Contains("t.envsensor.param"))
                            {
                                RFID_Tag_Json TagJson = JsonConvert.DeserializeObject<RFID_Tag_Json>(s);

                                if (sqlConnect.con.State == System.Data.ConnectionState.Open)
                                {
                                    SqlDataAdapter DA = new SqlDataAdapter("Insert_SensorResult", sqlConnect.con);
                                    DA.SelectCommand.CommandType = CommandType.StoredProcedure;

                                    DA.SelectCommand.Parameters.Add("@device_id", SqlDbType.VarChar).Value = TagJson.Device_ID;
                                    DA.SelectCommand.Parameters.Add("@rssi", SqlDbType.VarChar).Value = TagJson.RSSI;
                                    DA.SelectCommand.Parameters.Add("@pkt_no", SqlDbType.VarChar).Value = TagJson.Pkt_No;
                                    DA.SelectCommand.Parameters.Add("@sec", SqlDbType.VarChar).Value = TagJson.Sec;
                                    DA.SelectCommand.Parameters.Add("@temperature", SqlDbType.VarChar).Value = TagJson.Telematics_SensorData.Temperature;
                                    DA.SelectCommand.Parameters.Add("@humidity", SqlDbType.VarChar).Value = TagJson.Telematics_SensorData.Humidity;
                                    DA.SelectCommand.Parameters.Add("@pressure", SqlDbType.VarChar).Value = TagJson.Telematics_SensorData.Pressure;

                                    DA.SelectCommand.ExecuteNonQuery();
                                }
                                else
                                {
                                    tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                                        + "-----------------------------------------" + Environment.NewLine
                                        + "SQL Connection is Failure, Please Try Again!!"
                                        + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                                        + "-----------------------------------------");
                                }

                            }
                        }
                        else if (s.Contains("rpcreply"))
                        {
                            if (s.Contains("r.sysconfig.get") && s.Contains("r.sysconfig.result\":true"))
                            {
                                RFID_Tag_Json TagJson = JsonConvert.DeserializeObject<RFID_Tag_Json>(s);

                                if (sqlConnect.con.State == System.Data.ConnectionState.Open)
                                {
                                    SqlDataAdapter DA = new SqlDataAdapter("Insert_RPCConfig", sqlConnect.con);
                                    DA.SelectCommand.CommandType = CommandType.StoredProcedure;

                                    DA.SelectCommand.Parameters.Add("@device_id", SqlDbType.VarChar).Value = TagJson.Device_ID;
                                    DA.SelectCommand.Parameters.Add("@rssi", SqlDbType.VarChar).Value = TagJson.RSSI;
                                    DA.SelectCommand.Parameters.Add("@pkt_no", SqlDbType.VarChar).Value = TagJson.Pkt_No;
                                    DA.SelectCommand.Parameters.Add("@wifi_conn_type", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.WifiObj.Wifi_Connection_Type;
                                    DA.SelectCommand.Parameters.Add("@wifi_ssid", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.WifiObj.SSID;
                                    DA.SelectCommand.Parameters.Add("@wifi_password", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.WifiObj.Password;
                                    DA.SelectCommand.Parameters.Add("@wifi_bssid", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.WifiObj.BSSID;
                                    DA.SelectCommand.Parameters.Add("@staticip_enable", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.StaticIPObj.Enable;
                                    DA.SelectCommand.Parameters.Add("@sntp_enable", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.SNTPObj.Enable;
                                    DA.SelectCommand.Parameters.Add("@mqtt_enable", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.MQTTObj.Enable;
                                    DA.SelectCommand.Parameters.Add("@mqtt_addr", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.MQTTObj.Address;
                                    DA.SelectCommand.Parameters.Add("@mqtt_port", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.MQTTObj.Port;
                                    DA.SelectCommand.Parameters.Add("@mqtt_conntype", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.MQTTObj.Connection_Type;
                                    DA.SelectCommand.Parameters.Add("@mqtt_anonylogin", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.MQTTObj.Anony_Login;
                                    DA.SelectCommand.Parameters.Add("@mqtt_username", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.MQTTObj.Username;
                                    DA.SelectCommand.Parameters.Add("@mqtt_password", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.MQTTObj.Password;
                                    DA.SelectCommand.Parameters.Add("@mqtt_qos", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.MQTTObj.QOS;
                                    DA.SelectCommand.Parameters.Add("@mqtt_keepalive", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.MQTTObj.Keep_Alive;
                                    DA.SelectCommand.Parameters.Add("@mqtt_topictype", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.MQTTObj.TopicType;
                                    DA.SelectCommand.Parameters.Add("@result", SqlDbType.VarChar).Value = TagJson.ConfigResult;

                                    DA.SelectCommand.ExecuteNonQuery();
                                }
                                else
                                {
                                    tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                                        + "-----------------------------------------" + Environment.NewLine
                                        + "SQL Connection is Failure, Please Try Again!!"
                                        + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                                        + "-----------------------------------------");
                                }
                            }
                        }

                    }
                    else
                    {
                        tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                            + "-----------------------------------------" + Environment.NewLine
                            + " Device Verification Failure! Please Try Again" + Environment.NewLine
                            + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                            + "-----------------------------------------");
                    }
                }
                else
                {
                    if (s.Contains("rpcreply"))
                    {
                        if (s.Contains("r.enow.list"))
                        {
                            Battery_detail_Json_Layer battery_1 = JsonConvert.DeserializeObject<Battery_detail_Json_Layer>(s);

                            Battery battery_ = new Battery
                            {
                                device_id = battery_1.RPC_Reply.Device_ID,
                                pkt_no = battery_1.RPC_Reply.Pkt_No,
                                battery_name = battery_1.RPC_Reply.Enow_List[0].Node_ID,
                                battery_level = battery_1.RPC_Reply.Enow_List[0].Battery_level,
                                battery_status = battery_1.RPC_Reply.Enow_List[0].Battery_Status,
                            };
                            HttpClientConnection(battery_);
                        }
                    }

                    else if (s.Contains("telematics"))
                    {
                        if (s.Contains("t.enow.button"))
                        {
                            Battery_detail_Json_tele battery_1 = JsonConvert.DeserializeObject<Battery_detail_Json_tele>(s);

                            Battery battery_ = new Battery
                            {
                                device_id = battery_1.Telematics.Device_ID,
                                pkt_no = battery_1.Telematics.Pkt_No,
                                battery_name = battery_1.Telematics.Enow_Button[0].Node_ID,
                                battery_level = battery_1.Telematics.Enow_Button[0].Battery_level,
                                battery_status = battery_1.Telematics.Enow_Button[0].Battery_Status,
                            };
                            HttpClientConnection(battery_);
                        }
                        else if (s.Contains("t.enow.battstate"))
                        {
                            Battery_detail_Json_tele battery_1 = JsonConvert.DeserializeObject<Battery_detail_Json_tele>(s);

                            Battery battery_ = new Battery
                            {
                                device_id = battery_1.Telematics.Device_ID,
                                pkt_no = battery_1.Telematics.Pkt_No,
                                battery_name = battery_1.Telematics.Enow_BattState[0].Node_ID,
                                battery_level = battery_1.Telematics.Enow_BattState[0].Battery_level,
                                battery_status = battery_1.Telematics.Enow_BattState[0].Battery_Status,
                            };
                            HttpClientConnection(battery_);
                        }
                        else if (s.Contains("t.enow.disconnected"))
                        {
                            Battery_detail_Json_tele battery_1 = JsonConvert.DeserializeObject<Battery_detail_Json_tele>(s);

                            Battery battery_ = new Battery
                            {
                                device_id = battery_1.Telematics.Device_ID,
                                pkt_no = battery_1.Telematics.Pkt_No,
                                battery_name = battery_1.Telematics.Enow_Disconnected[0].Node_ID,
                                battery_level = battery_1.Telematics.Enow_Disconnected[0].Battery_level,
                                battery_status = battery_1.Telematics.Enow_Disconnected[0].Battery_Status,
                            };
                            HttpClientConnection(battery_);
                        }
                        else if (s.Contains("t.enow.connected"))
                        {
                            Battery_detail_Json_tele battery_1 = JsonConvert.DeserializeObject<Battery_detail_Json_tele>(s);

                            Battery battery_ = new Battery
                            {
                                device_id = battery_1.Telematics.Device_ID,
                                pkt_no = battery_1.Telematics.Pkt_No,
                                battery_name = battery_1.Telematics.Enow_Connected[0].Node_ID,
                                battery_level = battery_1.Telematics.Enow_Connected[0].Battery_level,
                                battery_status = battery_1.Telematics.Enow_Connected[0].Battery_Status,
                            };
                            HttpClientConnection(battery_);
                        }
                        else if (s.Contains("t.enow.indicator"))
                        {
                            string newS = s.ToString();
                            Battery_detail_Json_tele battery_1 = JsonConvert.DeserializeObject<Battery_detail_Json_tele>(newS);

                            Battery battery_ = new Battery
                            {
                                device_id = battery_1.Telematics.Device_ID,
                                pkt_no = battery_1.Telematics.Pkt_No,
                                battery_name = battery_1.Telematics.Enow_Indicator[0].Node_ID,
                                battery_level = battery_1.Telematics.Enow_Indicator[0].Battery_level,
                                battery_status = battery_1.Telematics.Enow_Indicator[0].Battery_Status,
                                indicator_color = battery_1.Telematics.Enow_Indicator[0].RGB_Color[0].ToString() + "," + battery_1.Telematics.Enow_Indicator[0].RGB_Color[1].ToString() + "," + battery_1.Telematics.Enow_Indicator[0].RGB_Color[2].ToString(),
                                rgb_mode = battery_1.Telematics.Enow_Indicator[0].RGB_Mode,
                                indicator_buzz_mode = battery_1.Telematics.Enow_Indicator[0].Buz_Mode,
                                period = battery_1.Telematics.Enow_Indicator[0].Period,
                            };
                            HttpClientConnection(battery_);
                        }
                    }
                }
            }
            else
            {

                tBoxDataIn.Text = tBoxDataIn.Text.Insert(0, Environment.NewLine
                        + "-----------------------------------------" + Environment.NewLine
                        + "===== Received Message =====" + Environment.NewLine
                        + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                        + "Message: " + s + Environment.NewLine
                        + "-----------------------------------------" + Environment.NewLine);
            }

            if (sqlConnect.con.State == ConnectionState.Open)
            {
                sqlConnect.con.Close();
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = cBoxComPort.Text;
                serialPort1.BaudRate = Convert.ToInt32(cBoxBaudRate.Text);
                serialPort1.DataBits = Convert.ToInt32(cBoxDataBits.Text);
                serialPort1.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cBoxStopBits.Text);
                serialPort1.Parity = (Parity)Enum.Parse(typeof(Parity), cBoxParityBits.Text);

                serialPort1.Open();
                progressBar1.Value = 100;

                // -- Button --
                // Prevent "Open" Button able to click when serial port already activate
                btnOpen.Enabled = false;
                btnClose.Enabled = true;
                btnSendData.Enabled = true;
                btnClearDataOut.Enabled = true;
                cBoxSelectionCommand.Enabled = true;
                btnClearDataIn.Enabled = true;
                gBoxComPortControl.Enabled = false;
                btnRPCConfig_USB.Enabled = true;

                lblStatusCom.Text = "Connected";
            }

            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // -- Button --
                btnOpen.Enabled = true;
                btnClose.Enabled = false;
                btnSendData.Enabled = false;
                btnClearDataOut.Enabled = false;
                cBoxSelectionCommand.Enabled = false;
                btnClearDataIn.Enabled = false;
                gBoxComPortControl.Enabled = true;
                btnRPCConfig_USB.Enabled = false;

                lblStatusCom.Text = "Disconnected";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                progressBar1.Value = 0;

                // -- Button --
                btnOpen.Enabled = true;
                btnClose.Enabled = false;
                btnSendData.Enabled = false;
                btnClearDataOut.Enabled = false;
                cBoxSelectionCommand.Enabled = false;
                btnClearDataIn.Enabled = false;
                gBoxComPortControl.Enabled = true;
                btnRPCConfig_USB.Enabled = false;

                lblStatusCom.Text = "Disconnected";
            }
        }

        // Send Data to Device that is connected, start and stop communication..
        private void chBoxDtrEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (chBoxDtrEnable.Checked)
            {
                serialPort1.DtrEnable = true;
                //MessageBox.Show("DTR Enable", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                serialPort1.DtrEnable = false;
            }
        }

        // Ready to Receive Data, controlling flow data, handshaking as well
        private void chBoxRTSEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (chBoxRTSEnable.Checked)
            {
                serialPort1.RtsEnable = true;
                //MessageBox.Show("RTS Enable", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                serialPort1.DtrEnable = false;
            }
        }

        private void btnClearDataOut_Click(object sender, EventArgs e)
        {
            if (tBoxDataOut.Text != string.Empty)
            {
                tBoxDataOut.Text = string.Empty;
            }
        }

        private void btnClearDataIn_Click(object sender, EventArgs e)
        {
            if (tBoxDataIn.Text != string.Empty)
            {
                tBoxDataIn.Text = string.Empty;
            }
        }

        private void btnSendData_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                DataOut = tBoxDataOut.Text;
                serialPort1.Write(DataOut);
            }
        }

        private void cBoxSelectionCommand_SelectedIndexChanged(object sender, EventArgs e)
        {
            string DataComboBoxCommand = cBoxSelectionCommand.Text;

            if (indicatorChoosen == false)
            {
                switch (DataComboBoxCommand)
                {
                    case "None":
                        tBoxDataOut.Text = "";
                        btnSendData.Enabled = false;
                        break;

                    case "Attb: Get All Attribute":
                        tBoxDataOut.Text = "{\"attribute\":{}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Set Internal Reading Period 500ms":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.devreadperiod\":500}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Set Internal Reading Period 1000ms":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.devreadperiod\":1000}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Enb. Auto Mode":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.auto\":1}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Dis. Auto Mode":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.auto\":0}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Menu Mode Period 5 second":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.auto\":0,\"s.uhfrfid.period\":5}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Menu Mode Period 10 second":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.auto\":0,\"s.uhfrfid.period\":10}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Cache period 800ms":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.cachetagremove\":1,\"s.uhfrfid.cacheperiod\":800}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Cache period 1100ms":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.cachetagremove\":1,\"s.uhfrfid.cacheperiod\":1100}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Cache period 1500ms":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.cachetagremove\":1,\"s.uhfrfid.cacheperiod\":1500}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Cache period 4000ms":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.cachetagremove\":1,\"s.uhfrfid.cacheperiod\":4000}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Cache period 10000ms":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.cachetagremove\":1,\"s.uhfrfid.cacheperiod\":10000}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Cache do not remove tag":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.cachetagremove\":0}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Enb Tag remove update":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.tagremoveupd\":1}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Dis Tag remove update":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.tagremoveupd\":0}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Enb. Ant Ch Update":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.antchangeupd\":1}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Dis. Ant Ch Update":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.antchangeupd\":0}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Enb. Force Update 4sec":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.enbforceupd\":1,\"s.uhfrfid.forceupdperiod\":4}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Enb. Force Update 10sec":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.enbforceupd\":1,\"s.uhfrfid.forceupdperiod\":10}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Dis. Force Update":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.enbforceupd\":0}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Enb Dynamic Q":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.dynamicq\":1}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Dis Dynamic Q":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.dynamicq\":0}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Set Q Value 4":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.dynamicq\":0,\"s.uhfrfid.q\":4}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Set Q Value 8":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.dynamicq\":0,\"s.uhfrfid.q\":8}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Set Tag Session 0":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.semode\":0}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Set Tag Session 1":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.semode\":1}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Set Tag Session 2":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.semode\":2}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Set Tag Session 3":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.semode\":3}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Set Tag Target A":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.target\":0}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Set Tag Target B":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.target\":1}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Set Tag Target A-B":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.target\":2}}";
                        btnSendData.Enabled = true;
                        break;

                    case "RFID Attb: Set Tag Target B-A":
                        tBoxDataOut.Text = "{\"attribute\":{\"s.uhfrfid.target\":0}}";
                        btnSendData.Enabled = true;
                        break;


                    default:
                        tBoxDataOut.Text = "";
                        btnSendData.Enabled = false;
                        break;
                }
            }
            else
            {
                switch (DataComboBoxCommand)
                {
                    case "None":
                        tBoxDataOut.Text = "";
                        break;

                    case "Attb: Get All Attribute":
                        tBoxDataOut.Text = "{\"attribute\":{}}";
                        break;

                    case "RPC: Get List Device":
                        tBoxDataOut.Text = "{\"rpcreq\":{\"r.enow.list\":{}}}";
                        break;

                    case "Doom: LED Red Blink, Buz Off, Period 10s":
                        tBoxDataOut.Text = "{\"rpcreq\":{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D1D194\",\"ch\":0,\"rgbmode\":2,\"rgbcolor\":[255,0,0],\"buzmode\":0,\"period\":10}]}}";
                        break;

                    case "Doom: LED Green Blink, Buz off, Period Default":
                        tBoxDataOut.Text = "{\"rpcreq\":{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D1D194\",\"ch\":0,\"rgbmode\":2,\"rgbcolor\":[0,255,0],\"buzmode\":0}]}}";
                        break;

                    case "Doom: LED Blue Blink, Buz off, Period Default":
                        tBoxDataOut.Text = "{\"rpcreq\":{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D1D194\",\"ch\":0,\"rgbmode\":2,\"rgbcolor\":[0,0,255],\"buzmode\":0}]}}";
                        break;

                    case "Doom: LED White Blink, Buz off, Period Default":
                        tBoxDataOut.Text = "{\"rpcreq\":{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D1D194\",\"ch\":0,\"rgbmode\":2,\"rgbcolor\":[255,255,255],\"buzmode\":0}]}}";
                        break;

                    case "Doom: LED White On, Buz Off, Period Default":
                        tBoxDataOut.Text = "{\"rpcreq\":{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D1D194\",\"ch\":0,\"rgbmode\":1,\"rgbcolor\":[255,255,255],\"buzmode\":0}]}}";
                        break;

                    case "Doom: Previous Setting LED On, Period Default":
                        tBoxDataOut.Text = "{\"rpcreq\":{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D1D194\",\"ch\":0,\"rgbmode\":1}]}}";
                        break;

                    case "Doom: Previous Setting LED Blink, Period Default":
                        tBoxDataOut.Text = "{\"rpcreq\":{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D1D194\",\"ch\":0,\"rgbmode\":2}]}}";
                        break;

                    case "Doom: LED Off, Period Default":
                        tBoxDataOut.Text = "{\"rpcreq\":{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D1D194\",\"ch\":0,\"rgbmode\":0}]}}";
                        break;

                    case "Doom: Buz On, Period Default":
                        tBoxDataOut.Text = "{\"rpcreq\":{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D1D194\",\"ch\":0,\"buzmode\":1}]}}";
                        break;

                    case "Doom: Buz blink, Period Default":
                        tBoxDataOut.Text = "{\"rpcreq\":{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D1D194\",\"ch\":0,\"buzmode\":2}]}}";
                        break;

                    case "Doom: Buz Off, Period Default":
                        tBoxDataOut.Text = "{\"rpcreq\":{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D1D194\",\"ch\":0,\"buzmode\":0}]}}";
                        break;

                    case "Doom: Buz & LED Off, Period Default":
                        tBoxDataOut.Text = "{\"rpcreq\":{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D1D194\",\"ch\":0,\"rgbmode\":0,\"buzmode\":0}]}}";
                        break;

                    case "WL: LED Red Blink, Buz Off, Period 10s":
                        tBoxDataOut.Text = "{\"rpcreq\":{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D00A60\",\"ch\":0,\"rgbmode\":2,\"rgbcolor\":[255,0,0],\"buzmode\":0,\"period\":10}]}}";
                        break;

                    case "WL: LED Green Blink, Buz off, Period Default":
                        tBoxDataOut.Text = "{\"rpcreq\":{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D00A60\",\"ch\":0,\"rgbmode\":2,\"rgbcolor\":[0,255,0],\"buzmode\":0}]}}";
                        break;

                    case "WL: LED Yellow Blink, Buz off, Period Default":
                        tBoxDataOut.Text = "{\"rpcreq\":{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D00A60\",\"ch\":0,\"rgbmode\":2,\"rgbcolor\":[0,0,255],\"buzmode\":0}]}}";
                        break;

                    case "WL: LED Yellow On, Buz Off, Period Default":
                        tBoxDataOut.Text = "{\"rpcreq\":{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D00A60\",\"ch\":0,\"rgbmode\":1,\"rgbcolor\":[0,0,255],\"buzmode\":0}]}}";
                        break;

                    case "WL: Previous Setting LED On, Period Default":
                        tBoxDataOut.Text = "{\"rpcreq\":{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D00A60\",\"ch\":0,\"rgbmode\":1}]}}";
                        break;

                    case "WL: Previous Setting LED Blink, Period Default":
                        tBoxDataOut.Text = "{\"rpcreq\":{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D00A60\",\"ch\":0,\"rgbmode\":2}]}}";
                        break;

                    case "WL: LED Off, Period Default":
                        tBoxDataOut.Text = "{\"rpcreq\":{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D00A60\",\"ch\":0,\"rgbmode\":0}]}}";
                        break;

                    case "WL: Buz On, Period Default":
                        tBoxDataOut.Text = "{\"rpcreq\":{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D00A60\",\"ch\":0,\"buzmode\":1}]}}";
                        break;

                    case "WL: Buz blink, Period Default":
                        tBoxDataOut.Text = "{\"rpcreq\":{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D00A60\",\"ch\":0,\"buzmode\":2}]}}";
                        break;

                    case "WL: Buz Off, Period Default":
                        tBoxDataOut.Text = "{\"rpcreq\":{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D00A60\",\"ch\":0,\"buzmode\":0}]}}";
                        break;

                    case "WL: Buz & LED Off, Period Default":
                        tBoxDataOut.Text = "{\"rpcreq\":{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D00A60\",\"ch\":0,\"rgbmode\":0,\"buzmode\":0}]}}";
                        break;

                }
            }
        }

        private void EventPublished(object sender, MqttMsgPublishEventArgs e)
        {
            try
            {
                string ReceivedMessage = Encoding.UTF8.GetString(e.Message);
                if (tBoxDataIn.InvokeRequired)
                {
                    tBoxDataIn.Invoke((MethodInvoker)delegate
                    {
                        tBoxDataIn.Text = tBoxDataIn.Text.Insert(0, Environment.NewLine
                        + "-----------------------------------------" + Environment.NewLine
                        + "===== Received Message =====" + Environment.NewLine
                        + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                        + "Topic:" + e.Topic + Environment.NewLine
                        + "Message: " + ReceivedMessage + Environment.NewLine
                        + "-----------------------------------------");
                    });
                }
                else
                {
                    tBoxDataIn.Text = tBoxDataIn.Text.Insert(0, Environment.NewLine
                    + "-----------------------------------------" + Environment.NewLine
                    + "===== Received Message =====" + Environment.NewLine
                    + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                    + "Topic:" + e.Topic + Environment.NewLine
                    + "Message: " + ReceivedMessage + Environment.NewLine
                    + "-----------------------------------------");
                }

                if (indicatorChoosen == false)
                {

                }
                else
                {
                    if (e.Topic.Contains("/rpc/response/"))
                    {
                        if (ReceivedMessage.Contains("r.enow.list"))
                        {
                            Battery_detail_Json battery_1 = JsonConvert.DeserializeObject<Battery_detail_Json>(ReceivedMessage);

                            Battery battery_ = new Battery
                            {
                                device_id = battery_1.Device_ID,
                                pkt_no = battery_1.Pkt_No,
                                battery_name = battery_1.Enow_List[0].Node_ID,
                                battery_level = battery_1.Enow_List[0].Battery_level,
                                battery_status = battery_1.Enow_List[0].Battery_Status,
                            };
                            //HttpClientConnection(battery_);
                        }
                    }

                    else if (e.Topic.Contains("/telemetry"))
                    {
                        if (ReceivedMessage.Contains("t.enow.button"))
                        {
                            Battery_detail_Json battery_1 = JsonConvert.DeserializeObject<Battery_detail_Json>(ReceivedMessage);

                            Battery battery_ = new Battery
                            {
                                device_id = battery_1.Device_ID,
                                pkt_no = battery_1.Pkt_No,
                                battery_name = battery_1.Enow_Button[0].Node_ID,
                                battery_level = battery_1.Enow_Button[0].Battery_level,
                                battery_status = battery_1.Enow_Button[0].Battery_Status,
                            };
                            //HttpClientConnection(battery_);
                        }
                        else if (ReceivedMessage.Contains("t.enow.battstate"))
                        {
                            Battery_detail_Json battery_1 = JsonConvert.DeserializeObject<Battery_detail_Json>(ReceivedMessage);

                            Battery battery_ = new Battery
                            {
                                device_id = battery_1.Device_ID,
                                pkt_no = battery_1.Pkt_No,
                                battery_name = battery_1.Enow_BattState[0].Node_ID,
                                battery_level = battery_1.Enow_BattState[0].Battery_level,
                                battery_status = battery_1.Enow_BattState[0].Battery_Status,
                            };
                            //HttpClientConnection(battery_);
                        }
                        else if (ReceivedMessage.Contains("t.enow.disconnected"))
                        {
                            Battery_detail_Json battery_1 = JsonConvert.DeserializeObject<Battery_detail_Json>(ReceivedMessage);

                            Battery battery_ = new Battery
                            {
                                device_id = battery_1.Device_ID,
                                pkt_no = battery_1.Pkt_No,
                                battery_name = battery_1.Enow_Disconnected[0].Node_ID,
                                battery_level = battery_1.Enow_Disconnected[0].Battery_level,
                                battery_status = battery_1.Enow_Disconnected[0].Battery_Status,
                            };
                            //HttpClientConnection(battery_);
                        }
                        else if (ReceivedMessage.Contains("t.enow.connected"))
                        {
                            Battery_detail_Json battery_1 = JsonConvert.DeserializeObject<Battery_detail_Json>(ReceivedMessage);

                            Battery battery_ = new Battery
                            {
                                device_id = battery_1.Device_ID,
                                pkt_no = battery_1.Pkt_No,
                                battery_name = battery_1.Enow_Connected[0].Node_ID,
                                battery_level = battery_1.Enow_Connected[0].Battery_level,
                                battery_status = battery_1.Enow_Connected[0].Battery_Status,
                            };
                            //HttpClientConnection(battery_);
                        }
                        else if (ReceivedMessage.Contains("t.enow.indicator"))
                        {
                            string newS = ReceivedMessage.ToString();
                            Battery_detail_Json battery_1 = JsonConvert.DeserializeObject<Battery_detail_Json>(newS);

                            Battery battery_ = new Battery
                            {
                                device_id = battery_1.Device_ID,
                                pkt_no = battery_1.Pkt_No,
                                battery_name = battery_1.Enow_Indicator[0].Node_ID,
                                battery_level = battery_1.Enow_Indicator[0].Battery_level,
                                battery_status = battery_1.Enow_Indicator[0].Battery_Status,
                                indicator_color = battery_1.Enow_Indicator[0].RGB_Color[0].ToString() + "," + battery_1.Enow_Indicator[0].RGB_Color[1].ToString() + "," + battery_1.Enow_Indicator[0].RGB_Color[2].ToString(),
                                rgb_mode = battery_1.Enow_Indicator[0].RGB_Mode,
                                indicator_buzz_mode = battery_1.Enow_Indicator[0].Buz_Mode,
                                period = battery_1.Enow_Indicator[0].Period,
                            };
                            //HttpClientConnection(battery_);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnRPCConfig_USB_Click(object sender, EventArgs e)
        {
            RPCConfig rPCConfig = new RPCConfig
            {
                USBC = true
            };
            rPCConfig.ShowDialog();
        }

        /* ========== COM Port Functions ========== */

        /* ========== MQTT Functions ========== */

        private void btnConnectMQTT_03_Click(object sender, EventArgs e)
        {
            if (tBoxIPAddress_03.Text != "")
            {
                try
                {
                    client_03 = new MqttClient(tBoxIPAddress_03.Text, 1883, false, null, null, MqttSslProtocols.TLSv1_2);
                    clientID_One_03 = Guid.NewGuid().ToString();
                    // Use Unique ID as Client ID, each time we start the application.
                    client_03.Connect(clientID_One_03);
                    tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                        + "-----------------------------------------" + Environment.NewLine
                        + "Client Connected to MQTT" + Environment.NewLine
                        + "Client ID: " + clientID_One_03 + Environment.NewLine
                        + "Broker Address: " + tBoxIPAddress_03.Text.ToString() + Environment.NewLine
                        + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                        + "-----------------------------------------");

                    btnConnectMQTT_03.Enabled = false;

                    // Register a callback-function for implementation
                    client_03.MqttMsgPublishReceived += new MqttClient.MqttMsgPublishEventHandler(EventPublished_03);
                    tBoxDeviceID_03.Enabled = false;
                    tBoxIPAddress_03.Enabled = false;
                    ConnectionStatus_03 = true;

                }
                catch (Exception ex)
                {
                    ConnectionStatus_03 = false;
                    btnConnectMQTT_03.Enabled = true;

                    tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                        + "-----------------------------------------" + Environment.NewLine
                        + "Client Connection Failed" + Environment.NewLine
                        + "Broker Address: " + tBoxIPAddress_03.Text + Environment.NewLine
                        + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                        + "-----------------------------------------");
                    //MessageBox.Show(ex.ToString());
                    MessageBox.Show("Please try again, Connection failure.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("IP Address Cannot Be Blank!");
            }

        }

        private void btnDisconnectMQTT_03_Click(object sender, EventArgs e)
        {
            try
            {
                if (tBoxDeviceID_03.Text != null)
                {
                    ListViewItem item = listView_MQTTList.FindItemWithText(tBoxDeviceID_03.Text);
                    if (item != null)
                    {
                        for (int i = 0; i < listView_MQTTList.Items.Count; i++)
                        {
                            if (listView_MQTTList.Items[i].Selected)
                            {
                                listView_MQTTList.Items[i].Remove();
                                i--;
                            }
                        }

                        tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                        + "-----------------------------------------" + Environment.NewLine
                        + "Client ID: " + clientID_One_03 + Environment.NewLine
                        + "Topic Successfully Unsubscribed To MQTT" + Environment.NewLine
                        + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                        + "-----------------------------------------");
                    }
                    else
                    {
                        MessageBox.Show("Please Select a valid Device ID!");
                    }

                    string TopicName_1st = "W3/" + tBoxDeviceID_03.Text + "/telemetry";
                    string TopicName_2nd = "W3/" + tBoxDeviceID_03.Text + "/attributes";
                    string TopicName_3rd = "W3/" + tBoxDeviceID_03.Text + "/rpc/response/+";

                    client_03.Unsubscribe(new string[] { TopicName_1st, TopicName_2nd, TopicName_3rd });

                    if (listView_MQTTList.Items.Count == 1)
                    {
                        client_03.Disconnect();

                        tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                        + "-----------------------------------------" + Environment.NewLine
                        + "Client ID: " + clientID_One_03 + Environment.NewLine
                        + "Client Successfully Disconnect To MQTT" + Environment.NewLine
                        + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                        + "-----------------------------------------");
                    }
                    
                    ConnectionStatus_03 = false;
                    tBoxDeviceID_03.Text = "";
                    tBoxIPAddress_03.Text = "";
                }
                else
                {
                    MessageBox.Show("Please Select a valid Device ID!");
                }

            }
            catch (Exception ex)
            {
                ConnectionStatus_03 = true;

                tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                    + "-----------------------------------------" + Environment.NewLine
                    + "Client ID: " + clientID_One_03 + Environment.NewLine
                    + "Client Unable Disconnected!" + Environment.NewLine
                    + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                    + "-----------------------------------------");
                //MessageBox.Show(ex.ToString());
                MessageBox.Show("Disconnect unsuccessful, please try again..", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSubTele_03_Click(object sender, EventArgs e)
        {
            //  PC Disconnected , Connected, Disconnected, Device Disconnected
            if (ConnectionStatus_03)
            {
                if (tBoxDeviceID_03.Text != "")
                {
                    ListViewItem item = listView_MQTTList.FindItemWithText(tBoxDeviceID_03.Text);
                    if (item == null)
                    {
                        //  W3/WIRIO3_XXX/telemetry
                        //  Set Topic Name for MQTT
                        string TopicName_1st = "W3/" + tBoxDeviceID_03.Text + "/telemetry";
                        string TopicName_2nd = "W3/" + tBoxDeviceID_03.Text + "/attributes";
                        string TopicName_3rd = "W3/" + tBoxDeviceID_03.Text + "/rpc/response/+";

                        //  Subcribe to Topic with QoS Level 2 (Exactly Once)
                        client_03.Subscribe(
                            new string[]
                            {
                            TopicName_1st,
                            TopicName_2nd,
                            TopicName_3rd
                            },
                            new byte[]
                            {
                            MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,
                            MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,
                            MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE
                            });

                        string TopicName = "W3/" + tBoxDeviceID_03.Text + "/attributes";
                        client_03.Publish(TopicName, Encoding.UTF8.GetBytes("{}"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);

                        string[] mqttData = { (listView_MQTTList.Items.Count + 1).ToString(), tBoxDeviceID_03.Text, tBoxIPAddress_03.Text, "PC Connected" };
                        var listViewItem = new ListViewItem(mqttData);

                        listViewItem.SubItems[3].ForeColor = Color.Orange;
                        listViewItem.UseItemStyleForSubItems = false;

                        listView_MQTTList.Items.Add(listViewItem);

                        tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                        + "-----------------------------------------" + Environment.NewLine
                        + "Device ID: " + tBoxDeviceID_03.Text + Environment.NewLine
                        + "Client ID: " + clientID_One_03 + Environment.NewLine
                        + "Subscribe All Topic Successfully!" + Environment.NewLine
                        + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                        + "-----------------------------------------");

                        tBoxDeviceID_03.Enabled = true;
                        tBoxIPAddress_03.Enabled = true;
                        tBoxDeviceID_03.Text = string.Empty;
                        tBoxIPAddress_03.Text = string.Empty;
                    }
                    else
                    {
                        tBoxDeviceID_03.Text = string.Empty;
                        tBoxIPAddress_03.Text = string.Empty;
                        MessageBox.Show("Current Device ID Has Been Subscribed!");
                    }
                }
                else
                {
                    MessageBox.Show("Please Enter a valid Device ID!");
                }
            }
            else
            {
                MessageBox.Show("Please Connect to MQTT Broker First!");
            }

        }

        private void listView_MQTTList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView_MQTTList.SelectedItems.Count > 0)
            {
                ListViewItem item = listView_MQTTList.SelectedItems[0];
                tBoxDeviceID_03.Text = item.SubItems[1].Text;
                tBoxIPAddress_03.Enabled = false;
                tBoxIPAddress_03.Text = item.SubItems[2].Text;
            }
            else
            {
                tBoxIPAddress_03.Enabled = true;
                tBoxDeviceID_03.Text = string.Empty;
                tBoxIPAddress_03.Text = string.Empty;
            }
        }

        private void btnPubDevUp_03_Click(object sender, EventArgs e)
        {
            if (ConnectionStatus_03)
            {
                if (tBoxDeviceID_03.Text != "")
                {
                    // Set Topic Name for MQTT
                    //W3 / WIRIO3_943CC64D77F0 / attributes
                    string TopicName = "W3/" + tBoxDeviceID_03.Text + "/attributes";

                    if (tBoxPublish_03.Text != "")
                    {
                        // Publish a message with QoS Level 2 (Exactly Once)
                        client_03.Publish(TopicName, Encoding.UTF8.GetBytes(tBoxPublish_03.Text), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
                    }
                    else
                    {
                        MessageBox.Show("Please Enter a Text for Publish!");
                    }
                }
                else
                {
                    MessageBox.Show("Please Enter a valid Device ID!");
                }
            }
            else
            {
                MessageBox.Show("Please Connect to MQTT Broker First!");
            }
        }

        private void btnPubRPCReq_03_Click(object sender, EventArgs e)
        {
            if (ConnectionStatus_03)
            {
                if (tBoxDeviceID_03.Text != "")
                {
                    // Set Topic Name for MQTT
                    string TopicName = "W3/" + tBoxDeviceID_03.Text + "/rpc/request/0";

                    if (tBoxPublish_03.Text != "")
                    {
                        // Publish a message with QoS Level 2 (Exactly Once)
                        client_03.Publish(TopicName, Encoding.UTF8.GetBytes(tBoxPublish_03.Text), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
                    }
                    else
                    {
                        MessageBox.Show("Please Enter a Text for Publish!");
                    }
                }
                else
                {
                    MessageBox.Show("Please Enter a valid Device ID!");
                }
            }
            else
            {
                MessageBox.Show("Please Connect to MQTT Broker First!");
            }
        }

        private void btnReceiverConnect_01_Click(object sender, EventArgs e)
        {
            if (ConnectionStatus_03)
            {
                if (tBoxDeviceID_03.Text != "")
                {
                    if (btnReceiverConnect_01.Text == "Connect")
                    {
                        if (!gBoxReceiverControl_02.Text.Contains(tBoxDeviceID_03.Text) && !gBoxReceiverControl_03.Text.Contains(tBoxDeviceID_03.Text) &&
                            !gBoxReceiverControl_04.Text.Contains(tBoxDeviceID_03.Text) && !gBoxReceiverControl_05.Text.Contains(tBoxDeviceID_03.Text) &&
                            !gBoxReceiverControl_06.Text.Contains(tBoxDeviceID_03.Text))
                        {
                            gBoxReceiverControl_01.Text = tBoxDeviceID_03.Text;
                            RC_1 = true;
                            btnReceiverConnect_01.Text = "Disconnect";

                            tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                            + "-----------------------------------------" + Environment.NewLine
                            + tBoxDeviceID_03.Text + " Successfully Connect to Receiver Control!" + Environment.NewLine
                            + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                            + "-----------------------------------------");
                        }
                        else
                        {
                            MessageBox.Show("Current WIRIO Device has been using one of the Receiver Control!");
                        }

                    }
                    else if (btnReceiverConnect_01.Text == "Disconnect")
                    {
                        tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                        + "-----------------------------------------" + Environment.NewLine
                        + gBoxReceiverControl_01.Text + " Successfully Disconnect to Receiver Control!" + Environment.NewLine
                        + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                        + "-----------------------------------------");

                        gBoxReceiverControl_01.Text = "Receiver Control (Unused)";
                        RC_1 = false;
                        btnReceiverConnect_01.Text = "Connect";
                        tBoxReceiverOutput_01.Text = string.Empty;
                    }
                }
                else
                {
                    MessageBox.Show("Please Enter a valid Device ID!");
                }
            }
            else
            {
                MessageBox.Show("Please Connect to MQTT Broker First!");
            }
        }

        private void btnReceiverConnect_02_Click(object sender, EventArgs e)
        {
            if (ConnectionStatus_03)
            {
                if (tBoxDeviceID_03.Text != "")
                {
                    if (btnReceiverConnect_02.Text == "Connect")
                    {
                        if (!gBoxReceiverControl_01.Text.Contains(tBoxDeviceID_03.Text) && !gBoxReceiverControl_03.Text.Contains(tBoxDeviceID_03.Text) &&
                            !gBoxReceiverControl_04.Text.Contains(tBoxDeviceID_03.Text) && !gBoxReceiverControl_05.Text.Contains(tBoxDeviceID_03.Text) &&
                            !gBoxReceiverControl_06.Text.Contains(tBoxDeviceID_03.Text))
                        {
                            gBoxReceiverControl_02.Text = tBoxDeviceID_03.Text;
                            RC_2 = true;
                            btnReceiverConnect_02.Text = "Disconnect";

                            tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                            + "-----------------------------------------" + Environment.NewLine
                            + tBoxDeviceID_03.Text + " Successfully Connect to Receiver Control!" + Environment.NewLine
                            + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                            + "-----------------------------------------");
                        }
                        else
                        {
                            MessageBox.Show("Current WIRIO Device has been using one of the Receiver Control!");
                        }

                    }
                    else if (btnReceiverConnect_02.Text == "Disconnect")
                    {
                        tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                        + "-----------------------------------------" + Environment.NewLine
                        + gBoxReceiverControl_02.Text + " Successfully Disconnect to Receiver Control!" + Environment.NewLine
                        + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                        + "-----------------------------------------");

                        gBoxReceiverControl_02.Text = "Receiver Control (Unused)";
                        RC_2 = false;
                        btnReceiverConnect_02.Text = "Connect";
                        tBoxReceiverOutput_02.Text = string.Empty;
                    }
                }
                else
                {
                    MessageBox.Show("Please Enter a valid Device ID!");
                }
            }
            else
            {
                MessageBox.Show("Please Connect to MQTT Broker First!");
            }
        }

        private void btnReceiverConnect_03_Click(object sender, EventArgs e)
        {
            if (ConnectionStatus_03)
            {
                if (tBoxDeviceID_03.Text != "")
                {
                    if (btnReceiverConnect_03.Text == "Connect")
                    {
                        if (!gBoxReceiverControl_01.Text.Contains(tBoxDeviceID_03.Text) && !gBoxReceiverControl_02.Text.Contains(tBoxDeviceID_03.Text) &&
                            !gBoxReceiverControl_04.Text.Contains(tBoxDeviceID_03.Text) && !gBoxReceiverControl_05.Text.Contains(tBoxDeviceID_03.Text) &&
                            !gBoxReceiverControl_06.Text.Contains(tBoxDeviceID_03.Text))
                        {
                            gBoxReceiverControl_03.Text = tBoxDeviceID_03.Text;
                            RC_3 = true;
                            btnReceiverConnect_03.Text = "Disconnect";

                            tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                            + "-----------------------------------------" + Environment.NewLine
                            + tBoxDeviceID_03.Text + " Successfully Connect to Receiver Control!" + Environment.NewLine
                            + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                            + "-----------------------------------------");
                        }
                        else
                        {
                            MessageBox.Show("Current WIRIO Device has been using one of the Receiver Control!");
                        }

                    }
                    else if (btnReceiverConnect_03.Text == "Disconnect")
                    {
                        tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                        + "-----------------------------------------" + Environment.NewLine
                        + gBoxReceiverControl_03.Text + " Successfully Disconnect to Receiver Control!" + Environment.NewLine
                        + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                        + "-----------------------------------------");

                        gBoxReceiverControl_03.Text = "Receiver Control (Unused)";
                        RC_3 = false;
                        btnReceiverConnect_03.Text = "Connect";
                        tBoxReceiverOutput_03.Text = string.Empty;
                    }
                }
                else
                {
                    MessageBox.Show("Please Enter a valid Device ID!");
                }
            }
            else
            {
                MessageBox.Show("Please Connect to MQTT Broker First!");
            }
        }

        private void btnReceiverConnect_04_Click(object sender, EventArgs e)
        {
            if (ConnectionStatus_03)
            {
                if (tBoxDeviceID_03.Text != "")
                {
                    if (btnReceiverConnect_04.Text == "Connect")
                    {
                        if (!gBoxReceiverControl_01.Text.Contains(tBoxDeviceID_03.Text) && !gBoxReceiverControl_02.Text.Contains(tBoxDeviceID_03.Text) &&
                            !gBoxReceiverControl_03.Text.Contains(tBoxDeviceID_03.Text) && !gBoxReceiverControl_05.Text.Contains(tBoxDeviceID_03.Text) &&
                            !gBoxReceiverControl_06.Text.Contains(tBoxDeviceID_03.Text))
                        {
                            gBoxReceiverControl_04.Text = tBoxDeviceID_03.Text;
                            RC_4 = true;
                            btnReceiverConnect_04.Text = "Disconnect";

                            tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                            + "-----------------------------------------" + Environment.NewLine
                            + tBoxDeviceID_03.Text + " Successfully Connect to Receiver Control!" + Environment.NewLine
                            + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                            + "-----------------------------------------");
                        }
                        else
                        {
                            MessageBox.Show("Current WIRIO Device has been using one of the Receiver Control!");
                        }
                    }
                    else if (btnReceiverConnect_04.Text == "Disconnect")
                    {
                        tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                        + "-----------------------------------------" + Environment.NewLine
                        + gBoxReceiverControl_04.Text + " Successfully Disconnect to Receiver Control!" + Environment.NewLine
                        + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                        + "-----------------------------------------");

                        gBoxReceiverControl_04.Text = "Receiver Control (Unused)";
                        RC_4 = false;
                        btnReceiverConnect_04.Text = "Connect";
                        tBoxReceiverOutput_04.Text = string.Empty;
                    }
                }
                else
                {
                    MessageBox.Show("Please Enter a valid Device ID!");
                }
            }
            else
            {
                MessageBox.Show("Please Connect to MQTT Broker First!");
            }
        }

        private void btnReceiverConnect_05_Click(object sender, EventArgs e)
        {
            if (ConnectionStatus_03)
            {
                if (tBoxDeviceID_03.Text != "")
                {
                    if (btnReceiverConnect_05.Text == "Connect")
                    {
                        if (!gBoxReceiverControl_01.Text.Contains(tBoxDeviceID_03.Text) && !gBoxReceiverControl_02.Text.Contains(tBoxDeviceID_03.Text) &&
                            !gBoxReceiverControl_03.Text.Contains(tBoxDeviceID_03.Text) && !gBoxReceiverControl_04.Text.Contains(tBoxDeviceID_03.Text) &&
                            !gBoxReceiverControl_06.Text.Contains(tBoxDeviceID_03.Text))
                        {
                            gBoxReceiverControl_05.Text = tBoxDeviceID_03.Text;
                            RC_5 = true;
                            btnReceiverConnect_05.Text = "Disconnect";

                            tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                            + "-----------------------------------------" + Environment.NewLine
                            + tBoxDeviceID_03.Text + " Successfully Connect to Receiver Control!" + Environment.NewLine
                            + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                            + "-----------------------------------------");
                        }
                        else
                        {
                            MessageBox.Show("Current WIRIO Device has been using one of the Receiver Control!");
                        }

                    }
                    else if (btnReceiverConnect_05.Text == "Disconnect")
                    {
                        tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                        + "-----------------------------------------" + Environment.NewLine
                        + gBoxReceiverControl_05.Text + " Successfully Disconnect to Receiver Control!" + Environment.NewLine
                        + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                        + "-----------------------------------------");

                        gBoxReceiverControl_05.Text = "Receiver Control (Unused)";
                        RC_5 = false;
                        btnReceiverConnect_05.Text = "Connect";
                        tBoxReceiverOutput_05.Text = string.Empty;
                    }
                }
                else
                {
                    MessageBox.Show("Please Enter a valid Device ID!");
                }
            }
            else
            {
                MessageBox.Show("Please Connect to MQTT Broker First!");
            }
        }

        private void btnReceiverConnect_06_Click(object sender, EventArgs e)
        {
            if (ConnectionStatus_03)
            {
                if (tBoxDeviceID_03.Text != "")
                {
                    if (btnReceiverConnect_06.Text == "Connect")
                    {
                        if (!gBoxReceiverControl_01.Text.Contains(tBoxDeviceID_03.Text) && !gBoxReceiverControl_02.Text.Contains(tBoxDeviceID_03.Text) &&
                            !gBoxReceiverControl_03.Text.Contains(tBoxDeviceID_03.Text) && !gBoxReceiverControl_04.Text.Contains(tBoxDeviceID_03.Text) &&
                            !gBoxReceiverControl_06.Text.Contains(tBoxDeviceID_03.Text))
                        {
                            gBoxReceiverControl_06.Text = tBoxDeviceID_03.Text;
                            RC_6 = true;
                            btnReceiverConnect_06.Text = "Disconnect";

                            tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                            + "-----------------------------------------" + Environment.NewLine
                            + tBoxDeviceID_03.Text + " Successfully Connect to Receiver Control!" + Environment.NewLine
                            + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                            + "-----------------------------------------");
                        }
                        else
                        {
                            MessageBox.Show("Current WIRIO Device has been using one of the Receiver Control!");
                        }
                    }
                    else if (btnReceiverConnect_06.Text == "Disconnect")
                    {
                        tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                        + "-----------------------------------------" + Environment.NewLine
                        + gBoxReceiverControl_06.Text + " Successfully Disconnect to Receiver Control!" + Environment.NewLine
                        + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                        + "-----------------------------------------");

                        gBoxReceiverControl_06.Text = "Receiver Control (Unused)";
                        RC_6 = false;
                        btnReceiverConnect_06.Text = "Connect";
                        tBoxReceiverOutput_06.Text = string.Empty;
                    }
                }
                else
                {
                    MessageBox.Show("Please Enter a valid Device ID!");
                }
            }
            else
            {
                MessageBox.Show("Please Connect to MQTT Broker First!");
            }
        }

        private void cBoxSelectionMQTTRFID_01_SelectedIndexChanged(object sender, EventArgs e)
        {
            string DataMQTTCommands = cBoxSelectionMQTTRFID_01.Text;

            switch (DataMQTTCommands)
            {
                case "None":
                    tBoxPublish_03.Text = "";
                    btnPubRPCReq_03.Enabled = false;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                case "Attb: Get All Attribute":
                    tBoxPublish_03.Text = "{}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;


                // RFID Tag Inventory / Cache Settings ::
                case "RFID Attb: Enable Auto Mode":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.auto\":1}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Disable Auto Mode":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.auto\":0}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Disable Auto Mode, Menu Mode Period 5 second":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.auto\":0,\"s.uhfrfid.period\":5}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Disable Auto Mode, Menu Mode Period 10 second":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.auto\":0,\"s.uhfrfid.period\":10}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Set Internal Reading Period 500ms":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.devreadperiod\":500}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Set Internal Reading Period 1000ms":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.devreadperiod\":1000}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Cache do not remove tag":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.cacheperiod\":0}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Cache Remove Tag, Cache period 800ms":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.cachetagremove\":1,\"s.uhfrfid.cacheperiod\":800}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Cache Remove Tag, Cache period 1100ms":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.cachetagremove\":1,\"s.uhfrfid.cacheperiod\":1100}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Cache Remove Tag, Cache period 2000ms":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.cachetagremove\":1,\"s.uhfrfid.cacheperiod\":2000}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Cache Remove Tag, Cache Period 4000ms":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.cachetagremove\":1,\"s.uhfrfid.cacheperiod\":4000}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Cache Remove Tag, Cache period 10000ms":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.cachetagremove\":1,\"s.uhfrfid.cacheperiod\":10000}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Enable Tag Remove Update":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.tagremoveupd\":1}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Disable Tag Remove Update":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.tagremoveupd\":0}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Enable Antenna Channel Update":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.antchangeupd\":1}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Disable Antenna Channel Update":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.antchangeupd\":0}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Enable Force Update 4sec":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.enbforceupd\":1,\"s.uhfrfid.forceupdperiod\":4}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Enable Force Update 10sec":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.enbforceupd\":1,\"s.uhfrfid.forceupdperiod\":10}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Disable Force Update":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.enbforceupd\":0}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;


                // RFID Tag Mode Setting
                case "RFID Attb: Enable Dynamic Q":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.dynamicq\":1}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Disable Dynamic Q":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.dynamicq\":0}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Set Q Value 4":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.dynamicq\":0,\"s.uhfrfid.q\":4}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Set Q Value 8":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.dynamicq\":0,\"s.uhfrfid.q\":8}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Set Tag Session 0":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.semode\":0}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Set Tag Session 1":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.semode\":1}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Set Tag Session 2":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.semode\":2}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Set Tag Session 3":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.semode\":3}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Set Tag Target A":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.target\":0}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Set Tag Target B":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.target\":1}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Set Tag Target A-B":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.target\":2}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Set Tag Target B-A":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.target\":3}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Enable Tag Password = 0xff,0x2d,0x20,0x5a":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.enbtagpass\":1,\"s.uhfrfid.tagpass\":\"ff2d205A\"}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Disable Tag Password":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.enbtagpass\":0}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Enable EPC Extended Info":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.epcextended\":1}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Disable EPC Extended Info":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.epcextended\":0}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;


                // RFID Reader Setting
                case "RFID Attb: Antenna Auto On Mode":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.antenb\":[0,0,0,0]}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Antenna CH3 Off": // Can be more specific which channel to turn off manually
                    tBoxPublish_03.Text = "{\"s.uhfrfid.antenb\":[0,0,2,0]}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Set Power to 1dbm": // Power can specific which channel to different power
                    tBoxPublish_03.Text = "{\"s.uhfrfid.power\":[1,1,1,1]}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Set Power to 20dbm":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.power\":[20,20,20,20]}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Set Power to 28dbm":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.power\":[28,28,28,28]}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Set Power to 33dbm":
                    tBoxPublish_03.Text = "{ \"s.uhfrfid.power\":[33,33,33,33]}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;


                // RFID External Trigger
                case "RFID Attb: Enable Input Port 0 Trigger":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.inptrig\":[1,0],\"s.inputport.setup\":[{\"ch\":0,\"mode\":1,\"onishigh\":false}]}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Enable Input Port 1 Trigger":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.inptrig\":[0,1],\"s.inputport.setup\":[{\"ch\":1,\"mode\":1,\"onishigh\":false}]}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Enable Input Port 0 & 1 Trigger":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.inptrig\":[1,1],\"s.inputport.setup\":[{\"ch\":0,\"mode\":1,\"onishigh\":false},{\"ch\":1,\"mode\":1,\"onishigh\":false}]}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Disable All Input Trigger":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.inptrig\":[0,0]}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                // RFID Demo Mode and Testing
                case "RFID Attb: Set Enable Demo Mode":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.demo\":1}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Set Disable Demo Mode":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.demo\":0}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                // Filter Setting
                case "RFID Attb: Set Disable Selection Filter and invert=0":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.selfilteroption\":0,\"s.uhfrfid.selfilterinvert\":0}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Set Filter Option EPC Bank, Add=20, BitLengh=4, Data=E0, invert=0":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.selfilteroption\":4,\"s.uhfrfid.selfilteraddr\":\"20\",\"s.uhfrfid.selfilterlenbit\":4,\"s.uhfrfid.selfilterdata\":\"e0\",\"s.uhfrfid.selfilterinvert\":0}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Set Filter Option EPC Bank, Add=20, BitLengh=4, Data=E0, invert=1":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.selfilteroption\":4,\"s.uhfrfid.selfilteraddr\":\"20\",\"s.uhfrfid.selfilterlenbit\":4,\"s.uhfrfid.selfilterdata\":\"e0\",\"s.uhfrfid.selfilterinvert\":1}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Set Filter Option By EPC, BitLengh=4, Data=E0, invert=0":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.selfilteroption\":1,\"s.uhfrfid.selfilterlenbit\":4,\"s.uhfrfid.selfilterdata\":\"e0\",\"s.uhfrfid.selfilterinvert\":0}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Set Filter Option By EPC, BitLengh=96, Data=E28068940000501B512658F4, invert=0":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.selfilteroption\":1,\"s.uhfrfid.selfilterlenbit\":96,\"s.uhfrfid.selfilterdata\":\"E28068940000501B512658F4\",\"s.uhfrfid.selfilterinvert\":0}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                // Auto Read Tag Data
                case "RFID Attb: Set Disable Read TagData":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.enbtagdata\":0}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Set Read TagData, EPC Bank Addr=0, cnt=6":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.enbtagdata\":1,\"s.uhfrfid.tagdatamembank\":1,\"s.uhfrfid.tagdatareadaddr\":\"0\",\"s.uhfrfid.tagdatawordcount\":6}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Set Read TagData TID Bank, Addr=0, cnt=3":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.enbtagdata\":1,\"s.uhfrfid.tagdatamembank\":2,\"s.uhfrfid.tagdatareadaddr\":\"0\",\"s.uhfrfid.tagdatawordcount\":3}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Set Read TagData USER, Addr=0, cnt=6":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.enbtagdata\":1,\"s.uhfrfid.tagdatamembank\":3,\"s.uhfrfid.tagdatareadaddr\":\"0\",\"s.uhfrfid.tagdatawordcount\":6}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID Attb: Set Read TagData RESERVED Bank Password":
                    tBoxPublish_03.Text = "{\"s.uhfrfid.enbtagdata\":1,\"s.uhfrfid.tagdatamembank\":0,\"s.uhfrfid.tagdatareadaddr\":\"0\",\"s.uhfrfid.tagdatawordcount\":4}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                // RFID RPC Request
                case "RFID RpcReq: Start Reading":
                    tBoxPublish_03.Text = "{\"r.uhfrfid.start\":1}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                case "RFID RpcReq: Stop Reading":
                    tBoxPublish_03.Text = "{\"r.uhfrfid.start\":0}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                case "RFID RpcReq: Tag Reload":
                    tBoxPublish_03.Text = "{\"r.uhfrfid.reload\":1}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                // Tag Reading
                case "RFID RpcReq: Read EPC Bank, any tag, addr=0,wordcnt=5":
                    tBoxPublish_03.Text = "{\"r.uhfrfid.readtag\":{\"readoption\":1,\"readaddr\":\"0\",\"wordcount\":5}}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                case "RFID RpcReq: Read User Bank, any tag, addr=0,wordcnt=5":
                    tBoxPublish_03.Text = "{\"r.uhfrfid.readtag\":{\"readoption\":3,\"readaddr\":\"0\",\"wordcount\":5}}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                case "RFID RpcReq: Read User Bank, Filter EPC=\"EA000001\", addr=0,wordcnt=5":
                    tBoxPublish_03.Text = "{\"r.uhfrfid.readtag\":{\"readoption\":3,\"readaddr\":\"0\",\"wordcount\":5,\"tagselection\":{\"option\":1,\"lenbit\":32,\"data\":\"EA000001\"}}}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                case "RFID RpcReq: Read Reserved Bank, Filter EPC=\"EA000001\", addr=0,wordcnt=4":
                    tBoxPublish_03.Text = "{\"r.uhfrfid.readtag\":{\"readoption\":0,\"readaddr\":\"0\",\"wordcount\":4,\"tagselection\":{\"option\":1,\"lenbit\":32,\"data\":\"EA000001\"}}}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                case "RFID RpcReq: Read Reserved Bank, Filter EPC=\"EA000001\", Pw=\"11112222\",addr=0,wordcnt=4":
                    tBoxPublish_03.Text = "{\"r.uhfrfid.readtag\":{\"readoption\":0,\"readaddr\":\"0\",\"wordcount\":4,\"password\":\"11112222\",\"tagselection\":{\"option\":1,\"lenbit\":32,\"data\":\"EA000001\"}}}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                // Tag Writting
                case "RFID RpcReq: Write EPC, any tag, data=\"EA000001\"":
                    tBoxPublish_03.Text = "{\"r.uhfrfid.writetag\":{\"writeoption\":4,\"writedata\":\"ea000001\"}}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                case "RFID RpcReq: Write EPC, any tag, data=\"EA000002\"":
                    tBoxPublish_03.Text = "{\"r.uhfrfid.writetag\":{\"writeoption\":4,\"writedata\":\"ea000002\"}}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                case "RFID RpcReq: Write EPC, any tag, data=\"EA000001\" Passwd=\"11112222\"":
                    tBoxPublish_03.Text = "{\"r.uhfrfid.writetag\":{\"writeoption\":4,\"writedata\":\"ea000001\",\"password\": \"11112222\"}}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                case "RFID RpcReq: Write EPC, any tag, data=\"EA000002\", passwd=\"11112222\"":
                    tBoxPublish_03.Text = "{\"r.uhfrfid.writetag\":{\"writeoption\":4,\"writedata\":\"ea000002\",\"password\":\"11112222\"}}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                case "RFID RpcReq: Write USER, any tag, addr=0, data=\"aa55bb66cc77dd88ee99ff11\"":
                    tBoxPublish_03.Text = "{\"r.uhfrfid.writetag\":{\"writeoption\":3,\"timeout\":1000,\"writeaddr\":\"0\",\"writedata\":\"aa55bb66cc77dd88ee99ff11\"}}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                case "RFID RpcReq: Write USER, any tag, addr=1, data=\"aaaabbbbccccdddd\"":
                    tBoxPublish_03.Text = "{\"r.uhfrfid.writetag\":{\"writeoption\":3,\"timeout\":1000,\"writeaddr\":\"1\",\"writedata\":\"aaaabbbbccccdddd\"}}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                case "RFID RpcReq: Write USER, Filter EPC=\"EA000001\", addr=1, data=\"aaaabbbbccccdddd\"":
                    tBoxPublish_03.Text = "{\"r.uhfrfid.writetag\":{\"writeoption\":3,\"timeout\":1000,\"writeaddr\":\"1\",\"writedata\":\"11aa22bb33cc44dd\",\"tagselection\":{\"option\":1,\"lenbit\":32,\"data\":\"EA000001\"}}}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                case "RFID RpcReq: Write USER, Filter EPC=\"EA000001\", pass=\"11112222\", addr=1, data=\"aaaabbbbccccdddd\"":
                    tBoxPublish_03.Text = "{\"r.uhfrfid.writetag\":{\"writeoption\":3,\"timeout\":1000,\"writeaddr\":\"1\",\"writedata\":\"11aa22bb33cc44dd\",\"password\":\"11112222\",\"tagselection\":{\"option\":1,\"lenbit\":32,\"data\":\"EA000001\"}}}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                case "RFID RpcReq: Write USER, Filter EPC=\"EA000001\", pass=\"11112222\", addr=0, data=\"aabbccddeeff\"":
                    tBoxPublish_03.Text = "{\"r.uhfrfid.writetag\":{\"writeoption\":3,\"timeout\":1000,\"writeaddr\":\"0\",\"writedata\":\"aabbccddeeff\",\"password\":\"11112222\",\"tagselection\":{\"option\":1,\"lenbit\":32,\"data\":\"EA000001\"}}}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                // Tag Writting (Set Password / Lock)
                case "RFID RpcReq: Set Password on tag=\"EA000001\", kill Pw=\"aaaabbbb\", access pw=\"11112222\"":
                    tBoxPublish_03.Text = "{\"r.uhfrfid.writetag\":{\"writeoption\":0,\"writeaddr\":\"0\",\"writedata\":\"aaaabbbb11112222\",\"tagselection\":{\"option\":1,\"lenbit\":32,\"data\":\"EA000001\"}}}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                case "RFID RpcReq: Set Password on tag any tag, kill Pw=\"aaaabbbb\", access pw=\"11112222\"":
                    tBoxPublish_03.Text = "{\"r.uhfrfid.writetag\":{\"writeoption\":0,\"writeaddr\":\"0\",\"writedata\":\"aaaabbbb11112222\"}}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                case "RFID RpcReq: Clear AccPw and KillPw on tag=\"EA000001\"":
                    tBoxPublish_03.Text = "{\"r.uhfrfid.writetag\":{\"writeoption\":0,\"writeaddr\":\"0\",\"writedata\":\"0000000000000000\",\"tagselection\":{\"option\":1,\"lenbit\":32,\"data\":\"EA000001\"}}}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                case "RFID RpcReq: Lock EPC on tag=\"EA0000001\", Password=\"11112222\"":
                    tBoxPublish_03.Text = "{\"r.uhfrfid.writetag\":{\"writeoption\":5,\"lockmask\":\"0020\",\"lockaction\":\"0020\",\"password\":\"11112222\",\"tagselection\":{\"option\":1,\"lenbit\":32,\"data\":\"EA000001\"}}}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                case "RFID RpcReq: UnLock EPC on tag=\"EA0000001\", Password=\"11112222\"":
                    tBoxPublish_03.Text = "{\"r.uhfrfid.writetag\":{\"writeoption\":5,\"lockmask\":\"0020\",\"lockaction\":\"0000\",\"password\":\"11112222\",\"tagselection\":{\"option\":1,\"lenbit\":32,\"data\":\"EA000001\"}}}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                case "RFID RpcReq: Lock Acc/Kill mem on tag=\"EA0000001\", Password=\"11112222\"":
                    tBoxPublish_03.Text = "{\"r.uhfrfid.writetag\":{\"writeoption\":5,\"lockmask\":\"0280\",\"lockaction\":\"0280\",\"password\":\"11112222\",\"tagselection\":{\"option\":1,\"lenbit\":32,\"data\":\"EA000001\"}}}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                case "RFID RpcReq: Unlock Acc/Kill mem on tag=\"EA0000001\", Password=\"11112222\"":
                    tBoxPublish_03.Text = "{\"r.uhfrfid.writetag\":{\"writeoption\":5,\"lockmask\":\"0280\",\"lockaction\":\"0000\",\"password\":\"11112222\",\"tagselection\":{\"option\":1,\"lenbit\":32,\"data\":\"EA000001\"}}}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                // Tag Writting Perma-Lock and Kill Tag. Make sure the password is already set
                case "RFID RpcReq: Perma-Lock EPC, AccPass and KillPass on tag=\"EA0000001\", Password=\"11112222\"":
                    tBoxPublish_03.Text = "{\"r.uhfrfid.writetag\":{\"writeoption\":5,\"lockmask\":\"03f0\",\"lockaction\":\"03f0\",\"password\":\"11112222\",\"tagselection\":{\"option\":1,\"lenbit\":32,\"data\":\"EA000001\"}}}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "RFID RpcReq: Kill Tag EPC=\"EA000001\", KillPass=\"aaaabbbb\"":
                    tBoxPublish_03.Text = "{\"r.uhfrfid.writetag\":{\"writeoption\":6,\"password\":\"aaaabbbb\",\"tagselection\":{\"option\":1,\"lenbit\":32,\"data\":\"EA000001\"}}}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                // Remote IO Setting
                case "Input Attb: Set Debounce = 10":
                    tBoxPublish_03.Text = "{\"s.inputport.setup\":[{\"ch\":0,\"debounce\":10}]}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "Input Attb: CH0, CH1 Set to Input port mode, Input Low is on":
                    tBoxPublish_03.Text = "{\"s.inputport.setup\":[{\"ch\":0,\"mode\":1,\"onishigh\":false},{\"ch\":1,\"mode\":1,\"onishigh\":false}]}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "Input Attb: CH0,CH1 Set to Push Button Input, Low is on":
                    tBoxPublish_03.Text = "{\"s.inputport.setup\":[{\"ch\":0,\"mode\":0,\"onishigh\":false},{\"ch\":1,\"mode\":0,\"onishigh\":false}]}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "Output Attb: CH0, Pulse Mode, 0.3/0.5, Cnt:4, On is High":
                    tBoxPublish_03.Text = "{\"s.outputport.setup\":[{\"ch\":0,\"mode\":1,\"onishigh\":true,\"fperiodon\":3,\"fperiodoff\":5,\"pulsecnt\":4}]}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "Output Attb: CH1, Pulse Mode, 0.3/0.5, Cnt:2, On is High":
                    tBoxPublish_03.Text = "{\"s.outputport.setup\":[{\"ch\":1,\"mode\":1,\"onishigh\":true,\"fperiodon\":3,\"fperiodoff\":5,\"pulsecnt\":2}]}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "Output Attb: CH0 Std Mode, On is High":
                    tBoxPublish_03.Text = "{\"s.outputport.setup\":[{\"ch\":0,\"mode\":0,\"onishigh\":true}]}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                case "Output Attb: CH1 Std Mode, On is High":
                    tBoxPublish_03.Text = "{\"s.outputport.setup\":[{\"ch\":1,\"mode\":0,\"onishigh\":true}]}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;


                // Remote IO RPC Command
                case "Output RpcReq: CH0 On":
                    tBoxPublish_03.Text = "{\"r.outputport.param\":[{\"ch\":0,\"ison\":true}]}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                case "Output RpcReq: CH0 Off":
                    tBoxPublish_03.Text = "{\"r.outputport.param\":[{\"ch\":0,\"ison\":0}]}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                case "Output RpcReq: CH1 On":
                    tBoxPublish_03.Text = "{\"r.outputport.param\":[{\"ch\":1,\"ison\":true}]}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                case "Output RpcReq: CH1 Off":
                    tBoxPublish_03.Text = "{\"r.outputport.param\":[{\"ch\":1,\"ison\":0}]}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                // System Attribute Setting
                case "Sys Attb: Set Device EPOC Time to 2022-Aug-26 3:16:23 GMT+0 (Example)":
                    tBoxPublish_03.Text = "{\"s.sys.epochsec\":1661483783}";
                    btnPubDevUpAttb_03.Enabled = true;
                    btnPubRPCReq_03.Enabled = false;
                    break;

                // System RPC Command
                case "Sys RpcReq: Reboot":
                    tBoxPublish_03.Text = "{\"r.sys.reboot\":1}";
                    btnPubRPCReq_03.Enabled = true;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;

                default:
                    tBoxPublish_03.Text = "";
                    btnPubRPCReq_03.Enabled = false;
                    btnPubDevUpAttb_03.Enabled = false;
                    break;
            }
        }

        private void cBoxSelectionMQTTIND_01_SelectedIndexChanged(object sender, EventArgs e)
        {
            string DataMQTTCommands = cBoxSelectionMQTTIND_01.Text;

            switch (DataMQTTCommands)
            {
                case "None":
                    tBoxPublish_03.Text = "";
                    break;

                case "Attb: Get All Attribute":
                    tBoxPublish_03.Text = "{}";
                    btnPubRPCReq_04.Enabled = false;
                    btnPubDevUpAttb_04.Enabled = true;
                    break;

                case "RPC: Get List Device":
                    tBoxPublish_03.Text = "{\"r.enow.list\":{}}";
                    btnPubRPCReq_04.Enabled = true;
                    btnPubDevUpAttb_04.Enabled = false;
                    break;

                case "Doom: LED Red Blink, Buz Off, Period 10s":
                    tBoxPublish_03.Text = "{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D1D194\",\"ch\":0,\"rgbmode\":2,\"rgbcolor\":[255,0,0],\"buzmode\":0,\"period\":10}]}";
                    btnPubRPCReq_04.Enabled = true;
                    btnPubDevUpAttb_04.Enabled = false;
                    break;

                case "Doom: LED Green Blink, Buz off, Period Default":
                    tBoxPublish_03.Text = "{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D1D194\",\"ch\":0,\"rgbmode\":2,\"rgbcolor\":[0,255,0],\"buzmode\":0}]}";
                    btnPubRPCReq_04.Enabled = true;
                    btnPubDevUpAttb_04.Enabled = false;
                    break;

                case "Doom: LED Blue Blink, Buz off, Period Default":
                    tBoxPublish_03.Text = "{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D1D194\",\"ch\":0,\"rgbmode\":2,\"rgbcolor\":[0,0,255],\"buzmode\":0}]}";
                    btnPubRPCReq_04.Enabled = true;
                    btnPubDevUpAttb_04.Enabled = false;
                    break;

                case "Doom: LED White Blink, Buz off, Period Default":
                    tBoxPublish_03.Text = "{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D1D194\",\"ch\":0,\"rgbmode\":2,\"rgbcolor\":[255,255,255],\"buzmode\":0}]}";
                    btnPubRPCReq_04.Enabled = true;
                    btnPubDevUpAttb_04.Enabled = false;
                    break;

                case "Doom: LED White On, Buz Off, Period Default":
                    tBoxPublish_03.Text = "{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D1D194\",\"ch\":0,\"rgbmode\":1,\"rgbcolor\":[255,255,255],\"buzmode\":0}]}";
                    btnPubRPCReq_04.Enabled = true;
                    btnPubDevUpAttb_04.Enabled = false;
                    break;

                case "Doom: Previous Setting LED On, Period Default":
                    tBoxPublish_03.Text = "{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D1D194\",\"ch\":0,\"rgbmode\":1}]}";
                    btnPubRPCReq_04.Enabled = true;
                    btnPubDevUpAttb_04.Enabled = false;
                    break;

                case "Doom: Previous Setting LED Blink, Period Default":
                    tBoxPublish_03.Text = "{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D1D194\",\"ch\":0,\"rgbmode\":2}]}";
                    btnPubRPCReq_04.Enabled = true;
                    btnPubDevUpAttb_04.Enabled = false;
                    break;

                case "Doom: LED Off, Period Default":
                    tBoxPublish_03.Text = "{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D1D194\",\"ch\":0,\"rgbmode\":0}]}";
                    btnPubRPCReq_04.Enabled = true;
                    btnPubDevUpAttb_04.Enabled = false;
                    break;

                case "Doom: Buz On, Period Default":
                    tBoxPublish_03.Text = "{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D1D194\",\"ch\":0,\"buzmode\":1}]}";
                    btnPubRPCReq_04.Enabled = true;
                    btnPubDevUpAttb_04.Enabled = false;
                    break;

                case "Doom: Buz blink, Period Default":
                    tBoxPublish_03.Text = "{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D1D194\",\"ch\":0,\"buzmode\":2}]}";
                    btnPubRPCReq_04.Enabled = true;
                    btnPubDevUpAttb_04.Enabled = false;
                    break;

                case "Doom: Buz Off, Period Default":
                    tBoxPublish_03.Text = "{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D1D194\",\"ch\":0,\"buzmode\":0}]}";
                    btnPubRPCReq_04.Enabled = true;
                    btnPubDevUpAttb_04.Enabled = false;
                    break;

                case "Doom: Buz & LED Off, Period Default":
                    tBoxPublish_03.Text = "{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D1D194\",\"ch\":0,\"rgbmode\":0,\"buzmode\":0}]}";
                    btnPubRPCReq_04.Enabled = true;
                    btnPubDevUpAttb_04.Enabled = false;
                    break;

                case "WL: LED Red Blink, Buz Off, Period 10s":
                    tBoxPublish_03.Text = "{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D00A60\",\"ch\":0,\"rgbmode\":2,\"rgbcolor\":[255,0,0],\"buzmode\":0,\"period\":10}]}";
                    btnPubRPCReq_04.Enabled = true;
                    btnPubDevUpAttb_04.Enabled = false;
                    break;

                case "WL: LED Green Blink, Buz off, Period Default":
                    tBoxPublish_03.Text = "{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D00A60\",\"ch\":0,\"rgbmode\":2,\"rgbcolor\":[0,255,0],\"buzmode\":0}]}";
                    btnPubRPCReq_04.Enabled = true;
                    btnPubDevUpAttb_04.Enabled = false;
                    break;

                case "WL: LED Yellow Blink, Buz off, Period Default":
                    tBoxPublish_03.Text = "{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D00A60\",\"ch\":0,\"rgbmode\":2,\"rgbcolor\":[0,0,255],\"buzmode\":0}]}";
                    btnPubRPCReq_04.Enabled = true;
                    btnPubDevUpAttb_04.Enabled = false;
                    break;

                case "WL: LED Yellow On, Buz Off, Period Default":
                    tBoxPublish_03.Text = "{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D00A60\",\"ch\":0,\"rgbmode\":1,\"rgbcolor\":[0,0,255],\"buzmode\":0}]}";
                    btnPubRPCReq_04.Enabled = true;
                    btnPubDevUpAttb_04.Enabled = false;
                    break;

                case "WL: Previous Setting LED On, Period Default":
                    tBoxPublish_03.Text = "{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D00A60\",\"ch\":0,\"rgbmode\":1}]}";
                    btnPubRPCReq_04.Enabled = true;
                    btnPubDevUpAttb_04.Enabled = false;
                    break;

                case "WL: Previous Setting LED Blink, Period Default":
                    tBoxPublish_03.Text = "{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D00A60\",\"ch\":0,\"rgbmode\":2}]}";
                    btnPubRPCReq_04.Enabled = true;
                    btnPubDevUpAttb_04.Enabled = false;
                    break;

                case "WL: LED Off, Period Default":
                    tBoxPublish_03.Text = "{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D00A60\",\"ch\":0,\"rgbmode\":0}]}";
                    btnPubRPCReq_04.Enabled = true;
                    btnPubDevUpAttb_04.Enabled = false;
                    break;

                case "WL: Buz On, Period Default":
                    tBoxPublish_03.Text = "{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D00A60\",\"ch\":0,\"buzmode\":1}]}";
                    btnPubRPCReq_04.Enabled = true;
                    btnPubDevUpAttb_04.Enabled = false;
                    break;

                case "WL: Buz blink, Period Default":
                    tBoxPublish_03.Text = "{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D00A60\",\"ch\":0,\"buzmode\":2}]}";
                    btnPubRPCReq_04.Enabled = true;
                    btnPubDevUpAttb_04.Enabled = false;
                    break;

                case "WL: Buz Off, Period Default":
                    tBoxPublish_03.Text = "{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D00A60\",\"ch\":0,\"buzmode\":0}]}";
                    btnPubRPCReq_04.Enabled = true;
                    btnPubDevUpAttb_04.Enabled = false;
                    break;

                case "WL: Buz & LED Off, Period Default":
                    tBoxPublish_03.Text = "{\"r.enow.indicator\":[{\"nodeid\":\"7CDFA1D00A60\",\"ch\":0,\"rgbmode\":0,\"buzmode\":0}]}";
                    btnPubRPCReq_04.Enabled = true;
                    btnPubDevUpAttb_04.Enabled = false;
                    break;

                default:
                    tBoxPublish_03.Text = "";
                    btnPubRPCReq_04.Enabled = false;
                    btnPubDevUpAttb_04.Enabled = false;
                    break;
            }
        }

        private void cBoxSelectionMQTTCommandsType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string SelectionMQTTType = cBoxSelectionMQTTCommandsType.Text;

            switch (SelectionMQTTType)
            {
                case "None":
                    cBoxSelectionMQTTRFID_01.Items.Clear();
                    cBoxSelectionMQTTRFID_01.ResetText();
                    break;

                case "RFID Tag Inventory / Cache Settings":
                    cBoxSelectionMQTTRFID_01.Items.Clear();
                    cBoxSelectionMQTTRFID_01.Items.AddRange(new string[] {
                        "Attb: Get All Attribute",
                        "RFID Attb: Set Internal Reading Period 500ms",
                        "RFID Attb: Set Internal Reading Period 1000ms",
                        "RFID Attb: Enable Auto Mode",
                        "RFID Attb: Disable Auto Mode",
                        "RFID Attb: Disable Auto Mode, Menu Mode Period 5 second",
                        "RFID Attb: Disable Auto Mode, Menu Mode Period 10 second",
                        "RFID Attb: Cache Remove Tag, Cache period 800ms",
                        "RFID Attb: Cache Remove Tag, Cache period 1100ms",
                        "RFID Attb: Cache Remove Tag, Cache period 2000ms",
                        "RFID Attb: Cache Remove Tag, Cache Period 4000ms",
                        "RFID Attb: Cache Remove Tag, Cache period 10000ms",
                        "RFID Attb: Cache do not remove tag",
                        "RFID Attb: Enable Tag Remove Update",
                        "RFID Attb: Disable Tag Remove Update",
                        "RFID Attb: Enable Antenna Channel Update",
                        "RFID Attb: Disable Antenna Channel Update",
                        "RFID Attb: Enable Force Update 4sec",
                        "RFID Attb: Enable Force Update 10sec",
                        "RFID Attb: Disable Force Update"
                    });
                    break;

                case "RFID Tag Mode Setting":
                    cBoxSelectionMQTTRFID_01.Items.Clear();
                    cBoxSelectionMQTTRFID_01.Items.AddRange(new string[] {
                        "RFID Attb: Enable Dynamic Q",
                        "RFID Attb: Disable Dynamic Q",
                        "RFID Attb: Set Q Value 4",
                        "RFID Attb: Set Q Value 8",
                        "RFID Attb: Set Tag Session 0",
                        "RFID Attb: Set Tag Session 1",
                        "RFID Attb: Set Tag Session 2",
                        "RFID Attb: Set Tag Session 3",
                        "RFID Attb: Set Tag Target A",
                        "RFID Attb: Set Tag Target B",
                        "RFID Attb: Set Tag Target A-B",
                        "RFID Attb: Set Tag Target B-A",
                        "RFID Attb: Enable Tag Password = 0xff,0x2d,0x20,0x5a",
                        "RFID Attb: Disable Tag Password",
                        "RFID Attb: Enable EPC Extended Info",
                        "RFID Attb: Disable EPC Extended Info"
                    });
                    break;

                case "RFID Reader Setting":
                    cBoxSelectionMQTTRFID_01.Items.Clear();
                    cBoxSelectionMQTTRFID_01.Items.AddRange(new string[] {
                        "RFID Attb: Antenna Auto On Mode",
                        "RFID Attb: Antenna CH3 Off",
                        "RFID Attb: Set Power to 33dbm",
                        "RFID Attb: Set Power to 28dbm",
                        "RFID Attb: Set Power to 20dbm",
                        "RFID Attb: Set Power to 1dbm"
                    });
                    break;

                case "RFID External Trigger":
                    cBoxSelectionMQTTRFID_01.Items.Clear();
                    cBoxSelectionMQTTRFID_01.Items.AddRange(new string[] {
                        "RFID Attb: Enable Input Port 0 Trigger",
                        "RFID Attb: Enable Input Port 1 Trigger",
                        "RFID Attb: Enable Input Port 0 & 1 Trigger",
                        "RFID Attb: Disable All Input Trigger"
                    });
                    break;

                case "RFID Demo and Testing":
                    cBoxSelectionMQTTRFID_01.Items.Clear();
                    cBoxSelectionMQTTRFID_01.Items.AddRange(new string[] {
                        "RFID Attb: Set Enable Demo Mode",
                        "RFID Attb: Set Disable Demo Mode"
                    });
                    break;

                case "Filter Setting":
                    cBoxSelectionMQTTRFID_01.Items.Clear();
                    cBoxSelectionMQTTRFID_01.Items.AddRange(new string[] {
                        "RFID Attb: Set Disable Selection Filter and invert=0",
                        "RFID Attb: Set Filter Option EPC Bank, Add=20, BitLengh=4, Data=E0, invert=0",
                        "RFID Attb: Set Filter Option EPC Bank, Add=20, BitLengh=4, Data=E0, invert=1",
                        "RFID Attb: Set Filter Option By EPC, BitLengh=4, Data=E0, invert=0",
                        "RFID Attb: Set Filter Option By EPC, BitLengh=96, Data=E28068940000501B512658F4, invert=0"
                    });
                    break;

                case "Auto Read Tag Data":
                    cBoxSelectionMQTTRFID_01.Items.Clear();
                    cBoxSelectionMQTTRFID_01.Items.AddRange(new string[] {
                        "RFID Attb: Set Disable Read TagData",
                        "RFID Attb: Set Read TagData, EPC Bank Addr=0, cnt=6",
                        "RFID Attb: Set Read TagData TID Bank, Addr=0, cnt=3",
                        "RFID Attb: Set Read TagData USER, Addr=0, cnt=6",
                        "RFID Attb: Set Read TagData RESERVED Bank Password",
                    });
                    break;

                case "RFID RPC Command":
                    cBoxSelectionMQTTRFID_01.Items.Clear();
                    cBoxSelectionMQTTRFID_01.Items.AddRange(new string[] {
                        "Attb: Get All Attribute",
                        "RFID RpcReq: Start Reading",
                        "RFID RpcReq: Stop Reading",
                        "RFID RpcReq: Tag Reload"
                    });
                    break;

                case "Tag Reading":
                    cBoxSelectionMQTTRFID_01.Items.Clear();
                    cBoxSelectionMQTTRFID_01.Items.AddRange(new string[] {
                        "RFID RpcReq: Read EPC Bank, any tag, addr=0,wordcnt=5",
                        "RFID RpcReq: Read User Bank, any tag, addr=0,wordcnt=5",
                        "RFID RpcReq: Read User Bank, Filter EPC=\"EA000001\", addr=0,wordcnt=5",
                        "RFID RpcReq: Read Reserved Bank, Filter EPC=\"EA000001\", addr=0,wordcnt=4",
                        "RFID RpcReq: Read Reserved Bank, Filter EPC=\"EA000001\", Pw=\"11112222\",addr=0,wordcnt=4"
                    });
                    break;

                case "Tag Writting":
                    cBoxSelectionMQTTRFID_01.Items.Clear();
                    cBoxSelectionMQTTRFID_01.Items.AddRange(new string[] {
                        "RFID RpcReq: Write EPC, any tag, data=\"EA000001\"",
                        "RFID RpcReq: Write EPC, any tag, data=\"EA000002\"",
                        "RFID RpcReq: Write EPC, any tag, data=\"EA000001\" Passwd=\"11112222\"",
                        "RFID RpcReq: Write EPC, any tag, data=\"EA000002\", passwd=\"11112222\"",
                        "RFID RpcReq: Write USER, any tag, addr=0, data=\"aa55bb66cc77dd88ee99ff11\"",
                        "RFID RpcReq: Write USER, any tag, addr=1, data=\"aaaabbbbccccdddd\"",
                        "RFID RpcReq: Write USER, Filter EPC=\"EA000001\", addr=1, data=\"aaaabbbbccccdddd\"",
                        "RFID RpcReq: Write USER, Filter EPC=\"EA000001\", pass=\"11112222\", addr=1, data=\"aaaabbbbccccdddd\"",
                        "RFID RpcReq: Write USER, Filter EPC=\"EA000001\", pass=\"11112222\", addr=0, data=\"aabbccddeeff\""
                    });
                    break;

                case "Tag Writting (Set Password / Lock)":
                    cBoxSelectionMQTTRFID_01.Items.Clear();
                    cBoxSelectionMQTTRFID_01.Items.AddRange(new string[] {
                        "RFID RpcReq: Set Password on tag=\"EA000001\", kill Pw=\"aaaabbbb\", access pw=\"11112222\"",
                        "RFID RpcReq: Set Password on tag any tag, kill Pw=\"aaaabbbb\", access pw=\"11112222\"",
                        "RFID RpcReq: Clear AccPw and KillPw on tag=\"EA000001\"",
                        "RFID RpcReq: Lock EPC on tag=\"EA0000001\", Password=\"11112222\"",
                        "RFID RpcReq: UnLock EPC on tag=\"EA0000001\", Password=\"11112222\"",
                        "RFID RpcReq: Lock Acc/Kill mem on tag=\"EA0000001\", Password=\"11112222\"",
                        "RFID RpcReq: Unlock Acc/Kill mem on tag=\"EA0000001\", Password=\"11112222\""
                    });
                    break;

                case "Tag Writting Perma-Lock and Kill Tag. Make sure the password is already set":
                    cBoxSelectionMQTTRFID_01.Items.Clear();
                    cBoxSelectionMQTTRFID_01.Items.AddRange(new string[] {
                        "RFID RpcReq: Perma-Lock EPC, AccPass and KillPass on tag=\"EA0000001\", Password=\"11112222\"",
                        "RFID RpcReq: Kill Tag EPC=\"EA000001\", KillPass=\"aaaabbbb\""
                    });
                    break;

                case "Remote I/O Setting":
                    cBoxSelectionMQTTRFID_01.Items.Clear();
                    cBoxSelectionMQTTRFID_01.Items.AddRange(new string[] {
                        "Input Attb: Set debounce = 10",
                        "Input Attb: CH0, CH1 Set to Input port mode, Input Low is on",
                        "Input Attb: CH0,CH1 Set to Push Button Input, Low is on",
                        "Output Attb: CH0, Pulse Mode, 0.3/0.5, Cnt:4, On is High",
                        "Output Attb: CH1, Pulse Mode, 0.3/0.5, Cnt:2, On is High",
                        "Output Attb: CH0 Std Mode, On is High",
                        "Output Attb: CH1 Std Mode, On is High"
                    });
                    break;

                case "Remote I/O RPC Command":
                    cBoxSelectionMQTTRFID_01.Items.Clear();
                    cBoxSelectionMQTTRFID_01.Items.AddRange(new string[] {
                        "Output RpcReq: CH0 On",
                        "Output RpcReq: CH0 Off",
                        "Output RpcReq: CH1 On",
                        "Output RpcReq: CH1 Off"
                    });
                    break;

                case "System Attribute Setting":
                    cBoxSelectionMQTTRFID_01.Items.Clear();
                    cBoxSelectionMQTTRFID_01.Items.AddRange(new string[] {
                        "Sys Attb: Set Device EPOC Time to 2022-Aug-26 3:16:23 GMT+0"
                    });
                    break;

                case "System RPC Command":
                    cBoxSelectionMQTTRFID_01.Items.Clear();
                    cBoxSelectionMQTTRFID_01.Items.AddRange(new string[] {
                        "Sys RpcReq: Reboot"
                    });
                    break;
            }
        }

        public void btnRPCConfig_03_Click(object sender, EventArgs e)
        {
            if (tBoxDeviceID_03.Text != String.Empty && tBoxIPAddress_03.Text != String.Empty)
            {
                RPCConfig rpcConfig = new RPCConfig
                {
                    GetIPAddress = tBoxIPAddress_03.Text,
                    GetWirioID = tBoxDeviceID_03.Text,
                    USBC = false,
                };
                rpcConfig.ShowDialog();
            }
            else
            {
                MessageBox.Show("Device ID or IP Address cannot be empty!");
            }
            
        }

        private void tBoxDeviceID_03_TextChanged(object sender, EventArgs e)
        {
            if (tBoxDeviceID_03.Text != string.Empty)
            {
                ListViewItem item = listView_MQTTList.FindItemWithText(tBoxDeviceID_03.Text);
                if (item == null)
                {
                    btnConnectMQTT_03.Enabled = true;
                    btnDisconnectMQTT_03.Enabled = false;
                    btnSubTele_03.Enabled = true;
                }
                else
                {
                    btnConnectMQTT_03.Enabled = false;
                    btnDisconnectMQTT_03.Enabled = true;
                    btnSubTele_03.Enabled = false;
                }
            }
            else if (tBoxDeviceID_03.Text == string.Empty)
            {
                tBoxIPAddress_03.Enabled = true;
                btnConnectMQTT_03.Enabled = false;
                btnDisconnectMQTT_03.Enabled = false;
                btnSubTele_03.Enabled = false;
            }
        }

        private void btnPubRPCReq_04_Click(object sender, EventArgs e)
        {
            if (ConnectionStatus_03)
            {
                if (tBoxDeviceID_03.Text != "")
                {
                    // Set Topic Name for MQTT
                    string TopicName = "W3/" + tBoxDeviceID_03.Text + "/rpc/request/0";

                    if (tBoxPublish_03.Text != "")
                    {
                        // Publish a message with QoS Level 2 (Exactly Once)
                        client_03.Publish(TopicName, Encoding.UTF8.GetBytes(tBoxPublish_03.Text), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
                    }
                    else
                    {
                        MessageBox.Show("Please Enter a Text for Publish!");
                    }
                }
                else
                {
                    MessageBox.Show("Please Enter a valid Device ID!");
                }
            }
            else
            {
                MessageBox.Show("Please Connect to MQTT Broker First!");
            }
        }

        private void btn_TagConfig_Click(object sender, EventArgs e)
        {
            if (tBoxDeviceID_03.Text != String.Empty && tBoxIPAddress_03.Text != String.Empty)
            {
                tagConfig.Show();
            }
            else
            {
                MessageBox.Show("Device ID or IP Address cannot be empty!");
            }
        }

        private void btnPubDevUpAttb_04_Click(object sender, EventArgs e)
        {
            if (ConnectionStatus_03)
            {
                if (tBoxDeviceID_03.Text != "")
                {
                    // Set Topic Name for MQTT
                    //W3 / WIRIO3_943CC64D77F0 / attributes
                    string TopicName = "W3/" + tBoxDeviceID_03.Text + "/attributes";

                    if (tBoxPublish_03.Text != "")
                    {
                        // Publish a message with QoS Level 2 (Exactly Once)
                        client_03.Publish(TopicName, Encoding.UTF8.GetBytes(tBoxPublish_03.Text), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
                    }
                    else
                    {
                        MessageBox.Show("Please Enter a Text for Publish!");
                    }
                }
                else
                {
                    MessageBox.Show("Please Enter a valid Device ID!");
                }
            }
            else
            {
                MessageBox.Show("Please Connect to MQTT Broker First!");
            }
        }

        private void EventPublished_03(object sender, MqttMsgPublishEventArgs e)
        {
            sqlConnect.con.Open();
            
            try
            {
                string ReceivedMessage = Encoding.UTF8.GetString(e.Message);

                if (RC_1 && e.Topic.Contains(gBoxReceiverControl_01.Text))
                {
                    if (tBoxReceiverOutput_01.InvokeRequired)
                    {
                        tBoxReceiverOutput_01.Invoke((MethodInvoker)delegate
                        {
                            tBoxReceiverOutput_01.Text = tBoxReceiverOutput_01.Text.Insert(0, Environment.NewLine
                                + "-----------------------------------------" + Environment.NewLine
                                + "===== Received Message =====" + Environment.NewLine
                                + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                                + "Topic:" + e.Topic + Environment.NewLine
                                + "Message: " + ReceivedMessage + Environment.NewLine
                                + "-----------------------------------------");
                        });
                    }
                    else
                    {
                        tBoxReceiverOutput_01.Text = tBoxReceiverOutput_01.Text.Insert(0, Environment.NewLine
                                + "-----------------------------------------" + Environment.NewLine
                                + "===== Received Message =====" + Environment.NewLine
                                + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                                + "Topic:" + e.Topic + Environment.NewLine
                                + "Message: " + ReceivedMessage + Environment.NewLine
                                + "-----------------------------------------");
                    }

                }
                else if (RC_2 && e.Topic.Contains(gBoxReceiverControl_02.Text))
                {
                    if (tBoxReceiverOutput_02.InvokeRequired)
                    {
                        tBoxReceiverOutput_02.Invoke((MethodInvoker)delegate
                        {
                            tBoxReceiverOutput_02.Text = tBoxReceiverOutput_02.Text.Insert(0, Environment.NewLine
                                + "-----------------------------------------" + Environment.NewLine
                                + "===== Received Message =====" + Environment.NewLine
                                + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                                + "Topic:" + e.Topic + Environment.NewLine
                                + "Message: " + ReceivedMessage + Environment.NewLine
                                + "-----------------------------------------");
                        });
                    }
                    else
                    {
                        tBoxReceiverOutput_02.Text = tBoxReceiverOutput_02.Text.Insert(0, Environment.NewLine
                            + "-----------------------------------------" + Environment.NewLine
                            + "===== Received Message =====" + Environment.NewLine
                            + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                            + "Topic:" + e.Topic + Environment.NewLine
                            + "Message: " + ReceivedMessage + Environment.NewLine
                            + "-----------------------------------------");
                    }
                }
                else if (RC_3 && e.Topic.Contains(gBoxReceiverControl_03.Text))
                {
                    if (tBoxReceiverOutput_03.InvokeRequired)
                    {
                        tBoxReceiverOutput_03.Invoke((MethodInvoker)delegate
                        {
                            tBoxReceiverOutput_03.Text = tBoxReceiverOutput_03.Text.Insert(0, Environment.NewLine
                                + "-----------------------------------------" + Environment.NewLine
                                + "===== Received Message =====" + Environment.NewLine
                                + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                                + "Topic:" + e.Topic + Environment.NewLine
                                + "Message: " + ReceivedMessage + Environment.NewLine
                                + "-----------------------------------------");
                        });
                    }
                    else
                    {
                        tBoxReceiverOutput_03.Text = tBoxReceiverOutput_03.Text.Insert(0, Environment.NewLine
                            + "-----------------------------------------" + Environment.NewLine
                            + "===== Received Message =====" + Environment.NewLine
                            + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                            + "Topic:" + e.Topic + Environment.NewLine
                            + "Message: " + ReceivedMessage + Environment.NewLine
                            + "-----------------------------------------");
                    }
                }
                else if (RC_4 && e.Topic.Contains(gBoxReceiverControl_04.Text))
                {
                    if (tBoxReceiverOutput_04.InvokeRequired)
                    {
                        tBoxReceiverOutput_04.Invoke((MethodInvoker)delegate
                        {
                            tBoxReceiverOutput_04.Text = tBoxReceiverOutput_04.Text.Insert(0, Environment.NewLine
                                + "-----------------------------------------" + Environment.NewLine
                                + "===== Received Message =====" + Environment.NewLine
                                + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                                + "Topic:" + e.Topic + Environment.NewLine
                                + "Message: " + ReceivedMessage + Environment.NewLine
                                + "-----------------------------------------");
                        });
                    }
                    else
                    {
                        tBoxReceiverOutput_04.Text = tBoxReceiverOutput_04.Text.Insert(0, Environment.NewLine
                            + "-----------------------------------------" + Environment.NewLine
                            + "===== Received Message =====" + Environment.NewLine
                            + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                            + "Topic:" + e.Topic + Environment.NewLine
                            + "Message: " + ReceivedMessage + Environment.NewLine
                            + "-----------------------------------------");
                    }
                }
                else if (RC_5 && e.Topic.Contains(gBoxReceiverControl_05.Text))
                {
                    if (tBoxReceiverOutput_05.InvokeRequired)
                    {
                        tBoxReceiverOutput_05.Invoke((MethodInvoker)delegate
                        {
                            tBoxReceiverOutput_05.Text = tBoxReceiverOutput_05.Text.Insert(0, Environment.NewLine
                                + "-----------------------------------------" + Environment.NewLine
                                + "===== Received Message =====" + Environment.NewLine
                                + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                                + "Topic:" + e.Topic + Environment.NewLine
                                + "Message: " + ReceivedMessage + Environment.NewLine
                                + "-----------------------------------------");
                        });
                    }
                    else
                    {
                        tBoxReceiverOutput_05.Text = tBoxReceiverOutput_05.Text.Insert(0, Environment.NewLine
                            + "-----------------------------------------" + Environment.NewLine
                            + "===== Received Message =====" + Environment.NewLine
                            + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                            + "Topic:" + e.Topic + Environment.NewLine
                            + "Message: " + ReceivedMessage + Environment.NewLine
                            + "-----------------------------------------");
                    }
                }
                else if (RC_6 && e.Topic.Contains(gBoxReceiverControl_06.Text))
                {
                    if (tBoxReceiverOutput_06.InvokeRequired)
                    {
                        tBoxReceiverOutput_06.Invoke((MethodInvoker)delegate
                        {
                            tBoxReceiverOutput_06.Text = tBoxReceiverOutput_06.Text.Insert(0, Environment.NewLine
                                + "-----------------------------------------" + Environment.NewLine
                                + "===== Received Message =====" + Environment.NewLine
                                + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                                + "Topic:" + e.Topic + Environment.NewLine
                                + "Message: " + ReceivedMessage + Environment.NewLine
                                + "-----------------------------------------");
                        });
                    }
                    else
                    {
                        tBoxReceiverOutput_06.Text = tBoxReceiverOutput_06.Text.Insert(0, Environment.NewLine
                            + "-----------------------------------------" + Environment.NewLine
                            + "===== Received Message =====" + Environment.NewLine
                            + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                            + "Topic:" + e.Topic + Environment.NewLine
                            + "Message: " + ReceivedMessage + Environment.NewLine
                            + "-----------------------------------------");
                    }
                }

                if (tBoxOverallReceiver.InvokeRequired)
                {
                    tBoxOverallReceiver.Invoke((MethodInvoker)delegate
                    {
                        tBoxOverallReceiver.Text = tBoxOverallReceiver.Text.Insert(0,
                            Environment.NewLine
                            + "-----------------------------------------" + Environment.NewLine
                            + "===== Received Message =====" + Environment.NewLine
                            + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                            + "Topic:" + e.Topic + Environment.NewLine
                            + "Message: " + ReceivedMessage + Environment.NewLine
                            + "-----------------------------------------"
                            );

                        if (indicatorChoosen == false)
                        {
                            // RFID Reader Tag :
                            RFID_detail_Json_Attr RFID_Details_Inv = JsonConvert.DeserializeObject<RFID_detail_Json_Attr>(ReceivedMessage);

                            if (RFID_Details_Inv.Device_ID != null)
                            {
                                ListViewItem item = listView_MQTTList.FindItemWithText(RFID_Details_Inv.Device_ID);

                                if (item.SubItems[1].Text.Equals(RFID_Details_Inv.Device_ID))
                                {
                                    if (RFID_Details_Inv.Connection == "true")
                                    {
                                        item.SubItems[3].Text = "Connected";
                                        item.SubItems[3].ForeColor = Color.Green;
                                    }
                                    else if (RFID_Details_Inv.Connection == "false")
                                    {
                                        item.SubItems[3].Text = "Disconnected";
                                        item.SubItems[3].ForeColor = Color.Red;
                                    }
                                }

                                if (e.Topic.Contains("/attributes"))
                                {
                                    if (ReceivedMessage.Contains("d.sys.addr") && ReceivedMessage.Contains("d.sys.fwid"))
                                    {
                                        if (sqlConnect.con.State == System.Data.ConnectionState.Open)
                                        {
                                            SqlDataAdapter DA_1 = new SqlDataAdapter("Insert_DetailList", sqlConnect.con);
                                            DA_1.SelectCommand.CommandType = CommandType.StoredProcedure;

                                            SqlDataAdapter DA_2 = new SqlDataAdapter("Insert_RFID_Attributes", sqlConnect.con);
                                            DA_2.SelectCommand.CommandType = CommandType.StoredProcedure;

                                            DA_1.SelectCommand.Parameters.Add("@device_id", SqlDbType.VarChar).Value = RFID_Details_Inv.Device_ID;
                                            DA_1.SelectCommand.Parameters.Add("@pkt_no", SqlDbType.VarChar).Value = RFID_Details_Inv.Pktno;
                                            DA_1.SelectCommand.Parameters.Add("@rssi", SqlDbType.VarChar).Value = RFID_Details_Inv.Wifi_Received_Signal_Strength;
                                            DA_1.SelectCommand.Parameters.Add("@ip_address", SqlDbType.VarChar).Value = RFID_Details_Inv.IP_Address;
                                            DA_1.SelectCommand.Parameters.Add("@connection", SqlDbType.VarChar).Value = RFID_Details_Inv.Connection;
                                            DA_1.SelectCommand.Parameters.Add("@connection_type", SqlDbType.VarChar).Value = RFID_Details_Inv.Connection_Type;
                                            DA_1.SelectCommand.Parameters.Add("@ssid", SqlDbType.VarChar).Value = RFID_Details_Inv.SSID;
                                            DA_1.SelectCommand.Parameters.Add("@fwid", SqlDbType.VarChar).Value = RFID_Details_Inv.FWID;
                                            DA_1.SelectCommand.Parameters.Add("@date_code", SqlDbType.VarChar).Value = RFID_Details_Inv.Date_Code;

                                            // Device Desc ::
                                            DA_1.SelectCommand.Parameters.Add("@device_model", SqlDbType.VarChar).Value = RFID_Details_Inv.Model;
                                            DA_1.SelectCommand.Parameters.Add("@device_name", SqlDbType.VarChar).Value = RFID_Details_Inv.Device_Name;
                                            DA_1.SelectCommand.Parameters.Add("@device_desc", SqlDbType.VarChar).Value = RFID_Details_Inv.Device_Desc;

                                            // Epoch ::
                                            DA_1.SelectCommand.Parameters.Add("@epoch_valid", SqlDbType.VarChar).Value = RFID_Details_Inv.Epoch_Valid;
                                            DA_1.SelectCommand.Parameters.Add("@epoch_sec", SqlDbType.VarChar).Value = RFID_Details_Inv.Epoch_Sec;

                                            DA_1.SelectCommand.Parameters.Add("@rpc_busy", SqlDbType.VarChar).Value = RFID_Details_Inv.Rpc_Busy;
                                            DA_1.SelectCommand.Parameters.Add("@channel_no", SqlDbType.VarChar).Value = RFID_Details_Inv.Channel_No;
                                            DA_1.SelectCommand.Parameters.Add("@power_max", SqlDbType.VarChar).Value = RFID_Details_Inv.Power_Max;
                                            DA_1.SelectCommand.Parameters.Add("@power_min", SqlDbType.VarChar).Value = RFID_Details_Inv.Power_Min;
                                            DA_1.SelectCommand.Parameters.Add("@cachespace_max", SqlDbType.VarChar).Value = RFID_Details_Inv.CacheSpace_Max;

                                            // Anthenna Output ::
                                            if (ReceivedMessage.Contains("d.uhfrfid.antison"))
                                            {
                                                DA_1.SelectCommand.Parameters.Add("@anthenna_output", SqlDbType.VarChar).Value
                                                = RFID_Details_Inv.Anthenna_Output[0] + "," + RFID_Details_Inv.Anthenna_Output[1] + "," + RFID_Details_Inv.Anthenna_Output[2] + "," + RFID_Details_Inv.Anthenna_Output[3];
                                            }
                                            else
                                            {
                                                DA_1.SelectCommand.Parameters.Add("@anthenna_output", SqlDbType.VarChar).Value = "";
                                            }

                                            // RFID Mode ::
                                            if (ReceivedMessage.Contains("d.uhfrfid.antstate"))
                                            {
                                                DA_1.SelectCommand.Parameters.Add("@rfid_mode", SqlDbType.VarChar).Value
                                                = RFID_Details_Inv.RFID_Mode[0] + "," + RFID_Details_Inv.RFID_Mode[1] + "," + RFID_Details_Inv.RFID_Mode[2] + "," + RFID_Details_Inv.RFID_Mode[3];
                                            }
                                            else
                                            {
                                                DA_1.SelectCommand.Parameters.Add("@rfid_mode", SqlDbType.VarChar).Value = "";
                                            }

                                            DA_1.SelectCommand.Parameters.Add("@rfid_temp", SqlDbType.VarChar).Value = RFID_Details_Inv.RFID_Temp;
                                            DA_1.SelectCommand.Parameters.Add("@rfid_firmware_id", SqlDbType.VarChar).Value = RFID_Details_Inv.RFID_Firmware_ID;
                                            DA_1.SelectCommand.Parameters.Add("@read_period", SqlDbType.VarChar).Value = RFID_Details_Inv.Read_Period;
                                            DA_1.SelectCommand.Parameters.Add("@auto", SqlDbType.VarChar).Value = RFID_Details_Inv.Auto;
                                            DA_1.SelectCommand.Parameters.Add("@period", SqlDbType.VarChar).Value = RFID_Details_Inv.Period;

                                            // Input Trigger ::
                                            if (ReceivedMessage.Contains("s.uhfrfid.inptrig"))
                                            {
                                                DA_1.SelectCommand.Parameters.Add("@input_trigger", SqlDbType.VarChar).Value = RFID_Details_Inv.Input_Trigger[0] + "," + RFID_Details_Inv.Input_Trigger[1];
                                            }
                                            else
                                            {
                                                DA_1.SelectCommand.Parameters.Add("@input_trigger", SqlDbType.VarChar).Value = "";
                                            }

                                            DA_1.SelectCommand.Parameters.Add("@cache_tag_remove", SqlDbType.VarChar).Value = RFID_Details_Inv.Cache_Tag_Remove;
                                            DA_1.SelectCommand.Parameters.Add("@cache_period", SqlDbType.VarChar).Value = RFID_Details_Inv.Cache_Period;
                                            DA_1.SelectCommand.Parameters.Add("@tag_remove_upd", SqlDbType.VarChar).Value = RFID_Details_Inv.Tag_Remove_Upd;
                                            DA_1.SelectCommand.Parameters.Add("@anthenna_channel_upd", SqlDbType.VarChar).Value = RFID_Details_Inv.Anthenna_Channel_Upd;
                                            DA_1.SelectCommand.Parameters.Add("@enbforce_upd", SqlDbType.VarChar).Value = RFID_Details_Inv.EnbForce_Upd;
                                            DA_1.SelectCommand.Parameters.Add("@force_update_period", SqlDbType.VarChar).Value = RFID_Details_Inv.Force_Update_Period;

                                            // Filter Options ::
                                            DA_1.SelectCommand.Parameters.Add("@self_filter_option", SqlDbType.VarChar).Value = RFID_Details_Inv.Self_Filter_Option;
                                            DA_1.SelectCommand.Parameters.Add("@self_filter_addr", SqlDbType.VarChar).Value = RFID_Details_Inv.Self_Filter_Addr;
                                            DA_1.SelectCommand.Parameters.Add("@self_filter_len_bit", SqlDbType.VarChar).Value = RFID_Details_Inv.Self_Filter_Len_Bit;
                                            DA_1.SelectCommand.Parameters.Add("@self_filter_data", SqlDbType.VarChar).Value = RFID_Details_Inv.Self_Filter_Data;
                                            DA_1.SelectCommand.Parameters.Add("@self_filter_invert", SqlDbType.VarChar).Value = RFID_Details_Inv.Self_Filter_Invert;

                                            // Tag Data ::
                                            DA_1.SelectCommand.Parameters.Add("@enb_tag_data", SqlDbType.VarChar).Value = RFID_Details_Inv.Enb_Tag_Data;
                                            DA_1.SelectCommand.Parameters.Add("@tag_data_mem_bank", SqlDbType.VarChar).Value = RFID_Details_Inv.Tag_Data_Mem_Bank;
                                            DA_1.SelectCommand.Parameters.Add("@tag_data_reader_addr", SqlDbType.VarChar).Value = RFID_Details_Inv.Tag_Data_Reader_Addr;
                                            DA_1.SelectCommand.Parameters.Add("@tag_data_word_count", SqlDbType.VarChar).Value = RFID_Details_Inv.Tag_Data_Word_Count;
                                            DA_1.SelectCommand.Parameters.Add("@enbtagpass", SqlDbType.VarChar).Value = RFID_Details_Inv.EnbTagPass;
                                            DA_1.SelectCommand.Parameters.Add("@tag_pass", SqlDbType.VarChar).Value = RFID_Details_Inv.Tag_Pass;

                                            DA_1.SelectCommand.Parameters.Add("@dynamic_q", SqlDbType.VarChar).Value = RFID_Details_Inv.Dynamic_Q;
                                            DA_1.SelectCommand.Parameters.Add("@q", SqlDbType.VarChar).Value = RFID_Details_Inv.Q;
                                            DA_1.SelectCommand.Parameters.Add("@se_mode", SqlDbType.VarChar).Value = RFID_Details_Inv.Se_Mode;
                                            DA_1.SelectCommand.Parameters.Add("@target", SqlDbType.VarChar).Value = RFID_Details_Inv.Target;
                                            DA_1.SelectCommand.Parameters.Add("@epc_extended", SqlDbType.VarChar).Value = RFID_Details_Inv.EPC_Extended;

                                            // Anthenna Mode ::
                                            if (ReceivedMessage.Contains("s.uhfrfid.antenb"))
                                            {
                                                DA_1.SelectCommand.Parameters.Add("@anthenna_mode", SqlDbType.VarChar).Value
                                                = RFID_Details_Inv.Anthenna_Mode[0] + "," + RFID_Details_Inv.Anthenna_Mode[1] + "," + RFID_Details_Inv.Anthenna_Mode[2] + "," + RFID_Details_Inv.Anthenna_Mode[3];
                                            }
                                            else
                                            {
                                                DA_1.SelectCommand.Parameters.Add("@anthenna_mode", SqlDbType.VarChar).Value = "";
                                            }

                                            // Current Power ::
                                            if (ReceivedMessage.Contains("s.uhfrfid.power"))
                                            {
                                                DA_1.SelectCommand.Parameters.Add("@rfid_current_power", SqlDbType.VarChar).Value
                                                = RFID_Details_Inv.RFID_Current_Power[0] + "," + RFID_Details_Inv.RFID_Current_Power[1] + "," + RFID_Details_Inv.RFID_Current_Power[2] + "," + RFID_Details_Inv.RFID_Current_Power[3];
                                            }
                                            else
                                            {
                                                DA_1.SelectCommand.Parameters.Add("@rfid_current_power", SqlDbType.VarChar).Value = "";
                                            }

                                            DA_1.SelectCommand.Parameters.Add("@demo_mode", SqlDbType.VarChar).Value = RFID_Details_Inv.Demo_Mode;
                                            // Temp Sensor ::
                                            DA_1.SelectCommand.Parameters.Add("@rate_control", SqlDbType.VarChar).Value = RFID_Details_Inv.Rate_Control;
                                            DA_1.SelectCommand.Parameters.Add("@rate_control_period", SqlDbType.VarChar).Value = RFID_Details_Inv.Rate_Control_Period;

                                            DA_1.SelectCommand.ExecuteNonQuery();

                                            for (int x = 0; x < RFID_Details_Inv.Perip_Items.Count; x++)
                                            {
                                                DA_2.SelectCommand.Parameters.Clear();
                                                DA_2.SelectCommand.Parameters.Add("@Option", SqlDbType.VarChar).Value = "Insert_PeripharalItems";
                                                DA_2.SelectCommand.Parameters.Add("@cmdkey", SqlDbType.VarChar).Value = RFID_Details_Inv.Perip_Items[x].Cmd_Key;
                                                DA_2.SelectCommand.Parameters.Add("@feature", SqlDbType.VarChar).Value = RFID_Details_Inv.Perip_Items[x].Feature;
                                                DA_2.SelectCommand.Parameters.Add("@ipp_ch", SqlDbType.VarChar).Value = "";
                                                DA_2.SelectCommand.Parameters.Add("@ipp_onishigh", SqlDbType.VarChar).Value = "";
                                                DA_2.SelectCommand.Parameters.Add("@ipp_mode", SqlDbType.VarChar).Value = "";
                                                DA_2.SelectCommand.Parameters.Add("@ipp_debounce", SqlDbType.VarChar).Value = "";
                                                DA_2.SelectCommand.Parameters.Add("@opp_ch", SqlDbType.VarChar).Value = "";
                                                DA_2.SelectCommand.Parameters.Add("@opp_onishigh", SqlDbType.VarChar).Value = "";
                                                DA_2.SelectCommand.Parameters.Add("@opp_mode", SqlDbType.VarChar).Value = "";
                                                DA_2.SelectCommand.Parameters.Add("@opp_fperiodon", SqlDbType.VarChar).Value = "";
                                                DA_2.SelectCommand.Parameters.Add("@opp_fperiodoff", SqlDbType.VarChar).Value = "";
                                                DA_2.SelectCommand.Parameters.Add("@opp_pulsecnt", SqlDbType.VarChar).Value = "";
                                                DA_2.SelectCommand.ExecuteNonQuery();
                                            }

                                            if (ReceivedMessage.Contains("s.outputport.setup"))
                                            {
                                                for (int y = 0; y < RFID_Details_Inv.OutputSetup.Count; y++)
                                                {
                                                    DA_2.SelectCommand.Parameters.Clear();
                                                    DA_2.SelectCommand.Parameters.Add("@Option", SqlDbType.VarChar).Value = "Insert_OutputConfig";
                                                    DA_2.SelectCommand.Parameters.Add("@cmdkey", SqlDbType.VarChar).Value = "";
                                                    DA_2.SelectCommand.Parameters.Add("@feature", SqlDbType.VarChar).Value = "";
                                                    DA_2.SelectCommand.Parameters.Add("@ipp_ch", SqlDbType.VarChar).Value = "";
                                                    DA_2.SelectCommand.Parameters.Add("@ipp_onishigh", SqlDbType.VarChar).Value = "";
                                                    DA_2.SelectCommand.Parameters.Add("@ipp_mode", SqlDbType.VarChar).Value = "";
                                                    DA_2.SelectCommand.Parameters.Add("@ipp_debounce", SqlDbType.VarChar).Value = "";
                                                    DA_2.SelectCommand.Parameters.Add("@opp_ch", SqlDbType.VarChar).Value = RFID_Details_Inv.OutputSetup[y].Channel;
                                                    DA_2.SelectCommand.Parameters.Add("@opp_onishigh", SqlDbType.VarChar).Value = RFID_Details_Inv.OutputSetup[y].On_isHigh;
                                                    DA_2.SelectCommand.Parameters.Add("@opp_mode", SqlDbType.VarChar).Value = RFID_Details_Inv.OutputSetup[y].Mode_Output;
                                                    DA_2.SelectCommand.Parameters.Add("@opp_fperiodon", SqlDbType.VarChar).Value = RFID_Details_Inv.OutputSetup[y].F_Period_On;
                                                    DA_2.SelectCommand.Parameters.Add("@opp_fperiodoff", SqlDbType.VarChar).Value = RFID_Details_Inv.OutputSetup[y].F_Period_Off;
                                                    DA_2.SelectCommand.Parameters.Add("@opp_pulsecnt", SqlDbType.VarChar).Value = RFID_Details_Inv.OutputSetup[y].Pulse_Cnt;
                                                    DA_2.SelectCommand.ExecuteNonQuery();
                                                }
                                            }

                                            if (ReceivedMessage.Contains("s.inputport.setup"))
                                            {
                                                for (int z = 0; z < RFID_Details_Inv.InputSetup.Count; z++)
                                                {
                                                    DA_2.SelectCommand.Parameters.Clear();
                                                    DA_2.SelectCommand.Parameters.Add("@Option", SqlDbType.VarChar).Value = "Insert_InputConfig";
                                                    DA_2.SelectCommand.Parameters.Add("@cmdkey", SqlDbType.VarChar).Value = "";
                                                    DA_2.SelectCommand.Parameters.Add("@feature", SqlDbType.VarChar).Value = "";
                                                    DA_2.SelectCommand.Parameters.Add("@ipp_ch", SqlDbType.VarChar).Value = RFID_Details_Inv.InputSetup[z].Channel;
                                                    DA_2.SelectCommand.Parameters.Add("@ipp_onishigh", SqlDbType.VarChar).Value = RFID_Details_Inv.InputSetup[z].On_isHigh;
                                                    DA_2.SelectCommand.Parameters.Add("@ipp_mode", SqlDbType.VarChar).Value = RFID_Details_Inv.InputSetup[z].Mode_Output;
                                                    DA_2.SelectCommand.Parameters.Add("@ipp_debounce", SqlDbType.VarChar).Value = RFID_Details_Inv.InputSetup[z].Debounce;
                                                    DA_2.SelectCommand.Parameters.Add("@opp_ch", SqlDbType.VarChar).Value = "";
                                                    DA_2.SelectCommand.Parameters.Add("@opp_onishigh", SqlDbType.VarChar).Value = "";
                                                    DA_2.SelectCommand.Parameters.Add("@opp_mode", SqlDbType.VarChar).Value = "";
                                                    DA_2.SelectCommand.Parameters.Add("@opp_fperiodon", SqlDbType.VarChar).Value = "";
                                                    DA_2.SelectCommand.Parameters.Add("@opp_fperiodoff", SqlDbType.VarChar).Value = "";
                                                    DA_2.SelectCommand.Parameters.Add("@opp_pulsecnt", SqlDbType.VarChar).Value = "";
                                                    DA_2.SelectCommand.ExecuteNonQuery();
                                                }
                                            }

                                        }
                                        else
                                        {
                                            tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                                                    + "-----------------------------------------" + Environment.NewLine
                                                    + "SQL Connection is Failure, Please Try Again!!"
                                                    + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                                                    + "-----------------------------------------");
                                        }
                                    }

                                }
                                else if (e.Topic.Contains("/telemetry"))
                                {
                                    if (ReceivedMessage.Contains("t.uhfrfid.temp"))
                                    {
                                        RFID_Tag_Json TagJson = JsonConvert.DeserializeObject<RFID_Tag_Json>(ReceivedMessage);

                                        if (sqlConnect.con.State == System.Data.ConnectionState.Open)
                                        {
                                            SqlDataAdapter DA = new SqlDataAdapter("Insert_RFID_Temp", sqlConnect.con);
                                            DA.SelectCommand.CommandType = CommandType.StoredProcedure;

                                            DA.SelectCommand.Parameters.Add("@device_id", SqlDbType.VarChar).Value = TagJson.Device_ID;
                                            DA.SelectCommand.Parameters.Add("@rssi", SqlDbType.VarChar).Value = TagJson.RSSI;
                                            DA.SelectCommand.Parameters.Add("@pkt_no", SqlDbType.VarChar).Value = TagJson.Pkt_No;
                                            DA.SelectCommand.Parameters.Add("@device_temp", SqlDbType.VarChar).Value = TagJson.Device_Temp;

                                            DA.SelectCommand.ExecuteNonQuery();
                                        }
                                        else
                                        {
                                            tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                                                + "-----------------------------------------" + Environment.NewLine
                                                + "SQL Connection is Failure, Please Try Again!!"
                                                + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                                                + "-----------------------------------------");
                                        }
                                    }
                                    else if (ReceivedMessage.Contains("t.uhfrfid.param"))
                                    {
                                        RFID_Tag_Json TagJson = JsonConvert.DeserializeObject<RFID_Tag_Json>(ReceivedMessage);

                                        if (sqlConnect.con.State == System.Data.ConnectionState.Open)
                                        {
                                            if (TagJson.Telematics_Data.Count > 0)
                                            {
                                                for (int countTeleData = 0; countTeleData < TagJson.Telematics_Data.Count(); countTeleData++)
                                                {
                                                    SqlDataAdapter DA = new SqlDataAdapter("Insert_ScanResult", sqlConnect.con);
                                                    DA.SelectCommand.CommandType = CommandType.StoredProcedure;

                                                    DA.SelectCommand.Parameters.Add("@device_id", SqlDbType.VarChar).Value = TagJson.Device_ID;
                                                    DA.SelectCommand.Parameters.Add("@tag_rssi", SqlDbType.VarChar).Value = TagJson.RSSI;
                                                    DA.SelectCommand.Parameters.Add("@pkt_no", SqlDbType.VarChar).Value = TagJson.Pkt_No;

                                                    DA.SelectCommand.Parameters.Add("@time_stamp", SqlDbType.VarChar).Value = TagJson.Telematics_Data[countTeleData].Time_Stamp;
                                                    DA.SelectCommand.Parameters.Add("@state", SqlDbType.VarChar).Value = TagJson.Telematics_Data[countTeleData].State;
                                                    DA.SelectCommand.Parameters.Add("@channel", SqlDbType.VarChar).Value = TagJson.Telematics_Data[countTeleData].Channel;
                                                    DA.SelectCommand.Parameters.Add("@tele_rssi", SqlDbType.VarChar).Value = TagJson.Telematics_Data[countTeleData].RSSI;
                                                    DA.SelectCommand.Parameters.Add("@tag_id", SqlDbType.VarChar).Value = TagJson.Telematics_Data[countTeleData].Tag_ID;

                                                    DA.SelectCommand.ExecuteNonQuery();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                                                + "-----------------------------------------" + Environment.NewLine
                                                + "SQL Connection is Failure, Please Try Again!!"
                                                + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                                                + "-----------------------------------------");
                                        }
                                    }
                                    else if (ReceivedMessage.Contains("t.envsensor.param"))
                                    {
                                        RFID_Tag_Json TagJson = JsonConvert.DeserializeObject<RFID_Tag_Json>(ReceivedMessage);

                                        if (sqlConnect.con.State == System.Data.ConnectionState.Open)
                                        {
                                            SqlDataAdapter DA = new SqlDataAdapter("Insert_SensorResult", sqlConnect.con);
                                            DA.SelectCommand.CommandType = CommandType.StoredProcedure;

                                            DA.SelectCommand.Parameters.Add("@device_id", SqlDbType.VarChar).Value = TagJson.Device_ID;
                                            DA.SelectCommand.Parameters.Add("@rssi", SqlDbType.VarChar).Value = TagJson.RSSI;
                                            DA.SelectCommand.Parameters.Add("@pkt_no", SqlDbType.VarChar).Value = TagJson.Pkt_No;
                                            DA.SelectCommand.Parameters.Add("@sec", SqlDbType.VarChar).Value = TagJson.Sec;
                                            DA.SelectCommand.Parameters.Add("@temperature", SqlDbType.VarChar).Value = TagJson.Telematics_SensorData.Temperature;
                                            DA.SelectCommand.Parameters.Add("@humidity", SqlDbType.VarChar).Value = TagJson.Telematics_SensorData.Humidity;
                                            DA.SelectCommand.Parameters.Add("@pressure", SqlDbType.VarChar).Value = TagJson.Telematics_SensorData.Pressure;

                                            DA.SelectCommand.ExecuteNonQuery();
                                        }
                                        else
                                        {
                                            tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                                                + "-----------------------------------------" + Environment.NewLine
                                                + "SQL Connection is Failure, Please Try Again!!"
                                                + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                                                + "-----------------------------------------");
                                        }

                                    }
                                }
                                else if (e.Topic.Contains("/rpc/response/"))
                                {
                                    if (ReceivedMessage.Contains("r.sysconfig.get") && ReceivedMessage.Contains("r.sysconfig.result\":true"))
                                    {
                                        RFID_Tag_Json TagJson = JsonConvert.DeserializeObject<RFID_Tag_Json>(ReceivedMessage);

                                        if (sqlConnect.con.State == System.Data.ConnectionState.Open)
                                        {
                                            SqlDataAdapter DA = new SqlDataAdapter("Insert_RPCConfig", sqlConnect.con);
                                            DA.SelectCommand.CommandType = CommandType.StoredProcedure;

                                            DA.SelectCommand.Parameters.Add("@device_id", SqlDbType.VarChar).Value = TagJson.Device_ID;
                                            DA.SelectCommand.Parameters.Add("@rssi", SqlDbType.VarChar).Value = TagJson.RSSI;
                                            DA.SelectCommand.Parameters.Add("@pkt_no", SqlDbType.VarChar).Value = TagJson.Pkt_No;
                                            DA.SelectCommand.Parameters.Add("@wifi_conn_type", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.WifiObj.Wifi_Connection_Type;
                                            DA.SelectCommand.Parameters.Add("@wifi_ssid", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.WifiObj.SSID;
                                            DA.SelectCommand.Parameters.Add("@wifi_password", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.WifiObj.Password;
                                            DA.SelectCommand.Parameters.Add("@wifi_bssid", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.WifiObj.BSSID;
                                            DA.SelectCommand.Parameters.Add("@staticip_enable", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.StaticIPObj.Enable;
                                            DA.SelectCommand.Parameters.Add("@sntp_enable", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.SNTPObj.Enable;
                                            DA.SelectCommand.Parameters.Add("@mqtt_enable", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.MQTTObj.Enable;
                                            DA.SelectCommand.Parameters.Add("@mqtt_addr", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.MQTTObj.Address;
                                            DA.SelectCommand.Parameters.Add("@mqtt_port", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.MQTTObj.Port;
                                            DA.SelectCommand.Parameters.Add("@mqtt_conntype", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.MQTTObj.Connection_Type;
                                            DA.SelectCommand.Parameters.Add("@mqtt_anonylogin", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.MQTTObj.Anony_Login;
                                            DA.SelectCommand.Parameters.Add("@mqtt_username", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.MQTTObj.Username;
                                            DA.SelectCommand.Parameters.Add("@mqtt_password", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.MQTTObj.Password;
                                            DA.SelectCommand.Parameters.Add("@mqtt_qos", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.MQTTObj.QOS;
                                            DA.SelectCommand.Parameters.Add("@mqtt_keepalive", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.MQTTObj.Keep_Alive;
                                            DA.SelectCommand.Parameters.Add("@mqtt_topictype", SqlDbType.VarChar).Value = TagJson.RPC_ConfigData.MQTTObj.TopicType;
                                            DA.SelectCommand.Parameters.Add("@result", SqlDbType.VarChar).Value = TagJson.ConfigResult;

                                            DA.SelectCommand.ExecuteNonQuery();
                                        }
                                        else
                                        {
                                            tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                                                + "-----------------------------------------" + Environment.NewLine
                                                + "SQL Connection is Failure, Please Try Again!!"
                                                + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                                                + "-----------------------------------------");
                                        }
                                    }
                                }

                            }
                            else
                            {
                                tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                                    + "-----------------------------------------" + Environment.NewLine
                                    + " Device Verification Failure! Please Try Again" + Environment.NewLine
                                    + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                                    + "-----------------------------------------");
                            }
                        }
                        else
                        {
                            // Indicator Battery :
                            Battery_detail_Json battery_1 = JsonConvert.DeserializeObject<Battery_detail_Json>(ReceivedMessage);

                            if (battery_1.Device_ID != null && ReceivedMessage.Contains(battery_1.Device_ID))
                            {
                                ListViewItem item = listView_MQTTList.FindItemWithText(battery_1.Device_ID);

                                if (e.Topic.Contains("/rpc/response/"))
                                {
                                    if (ReceivedMessage.Contains("r.enow.list"))
                                    {
                                        Battery battery_ = new Battery
                                        {
                                            device_id = battery_1.Device_ID,
                                            pkt_no = battery_1.Pkt_No,
                                            battery_name = battery_1.Enow_List[0].Node_ID,
                                            battery_level = battery_1.Enow_List[0].Battery_level,
                                            battery_status = battery_1.Enow_List[0].Battery_Status,
                                        };

                                        if (item.SubItems[1].Text.Equals(battery_1.Device_ID))
                                        {
                                            if (battery_1.Enow_List[0].Battery_Status == "0")
                                            {
                                                item.SubItems[3].Text = "Connected";
                                                item.SubItems[3].ForeColor = Color.Green;
                                            }
                                            else if (battery_1.Enow_List[0].Battery_Status == "1")
                                            {
                                                item.SubItems[3].Text = "Disconnected";
                                                item.SubItems[3].ForeColor = Color.Red;
                                            }
                                        }

                                        //HttpClientConnection(battery_);
                                    }
                                }

                                else if (e.Topic.Contains("/telemetry"))
                                {
                                    if (ReceivedMessage.Contains("t.enow.button"))
                                    {
                                        Battery battery_ = new Battery
                                        {
                                            device_id = battery_1.Device_ID,
                                            pkt_no = battery_1.Pkt_No,
                                            battery_name = battery_1.Enow_Button[0].Node_ID,
                                            battery_level = battery_1.Enow_Button[0].Battery_level,
                                            battery_status = battery_1.Enow_Button[0].Battery_Status,
                                        };
                                        //HttpClientConnection(battery_);
                                    }
                                    else if (ReceivedMessage.Contains("t.enow.battstate"))
                                    {
                                        Battery battery_ = new Battery
                                        {
                                            device_id = battery_1.Device_ID,
                                            pkt_no = battery_1.Pkt_No,
                                            battery_name = battery_1.Enow_BattState[0].Node_ID,
                                            battery_level = battery_1.Enow_BattState[0].Battery_level,
                                            battery_status = battery_1.Enow_BattState[0].Battery_Status,
                                        };
                                        //HttpClientConnection(battery_);
                                    }
                                    else if (ReceivedMessage.Contains("t.enow.disconnected"))
                                    {
                                        Battery battery_ = new Battery
                                        {
                                            device_id = battery_1.Device_ID,
                                            pkt_no = battery_1.Pkt_No,
                                            battery_name = battery_1.Enow_Disconnected[0].Node_ID,
                                            battery_level = battery_1.Enow_Disconnected[0].Battery_level,
                                            battery_status = battery_1.Enow_Disconnected[0].Battery_Status,
                                        };
                                        //HttpClientConnection(battery_);
                                    }
                                    else if (ReceivedMessage.Contains("t.enow.connected"))
                                    {
                                        Battery battery_ = new Battery
                                        {
                                            device_id = battery_1.Device_ID,
                                            pkt_no = battery_1.Pkt_No,
                                            battery_name = battery_1.Enow_Connected[0].Node_ID,
                                            battery_level = battery_1.Enow_Connected[0].Battery_level,
                                            battery_status = battery_1.Enow_Connected[0].Battery_Status,
                                        };
                                        //HttpClientConnection(battery_);
                                    }
                                    else if (ReceivedMessage.Contains("t.enow.indicator"))
                                    {
                                        Battery battery_ = new Battery
                                        {
                                            device_id = battery_1.Device_ID,
                                            pkt_no = battery_1.Pkt_No,
                                            battery_name = battery_1.Enow_Indicator[0].Node_ID,
                                            battery_level = battery_1.Enow_Indicator[0].Battery_level,
                                            battery_status = battery_1.Enow_Indicator[0].Battery_Status,
                                            indicator_color = battery_1.Enow_Indicator[0].RGB_Color[0].ToString() + "," + battery_1.Enow_Indicator[0].RGB_Color[1].ToString() + "," + battery_1.Enow_Indicator[0].RGB_Color[2].ToString(),
                                            rgb_mode = battery_1.Enow_Indicator[0].RGB_Mode,
                                            indicator_buzz_mode = battery_1.Enow_Indicator[0].Buz_Mode,
                                            period = battery_1.Enow_Indicator[0].Period,
                                        };
                                        //HttpClientConnection(battery_);
                                    }
                                }
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            sqlConnect.con.Close();
        }

        private void btnClearReceiverTxt_01_Click(object sender, EventArgs e)
        {
            tBoxReceiverOutput_01.Text = string.Empty;
            tBoxLog_03.Text = tBoxLog_03.Text.Insert(0,Environment.NewLine
                        + "-----------------------------------------" + Environment.NewLine
                        + gBoxReceiverControl_01.Text + " Text Clear Successfully!" + Environment.NewLine
                        + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                        + "-----------------------------------------");
        }

        private void btnClearReceiverTxt_02_Click(object sender, EventArgs e)
        {
            tBoxReceiverOutput_02.Text = string.Empty;
            tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                        + "-----------------------------------------" + Environment.NewLine
                        + gBoxReceiverControl_02.Text + " Text Clear Successfully!" + Environment.NewLine
                        + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                        + "-----------------------------------------");
        }

        private void btnClearReceiverTxt_03_Click(object sender, EventArgs e)
        {
            tBoxReceiverOutput_03.Text = string.Empty;
            tBoxLog_03.Text += tBoxLog_03.Text.Insert(0, Environment.NewLine
                        + "-----------------------------------------" + Environment.NewLine
                        + gBoxReceiverControl_03.Text + " Text Clear Successfully!" + Environment.NewLine
                        + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                        + "-----------------------------------------");
        }

        private void btnClearReceiverTxt_04_Click(object sender, EventArgs e)
        {
            tBoxReceiverOutput_04.Text = string.Empty;
            tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                        + "-----------------------------------------" + Environment.NewLine
                        + gBoxReceiverControl_04.Text + " Text Clear Successfully!" + Environment.NewLine
                        + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                        + "-----------------------------------------");
        }

        private void btnClearReceiverTxt_05_Click(object sender, EventArgs e)
        {
            tBoxReceiverOutput_05.Text = string.Empty;
            tBoxLog_03.Text += tBoxLog_03.Text.Insert(0, Environment.NewLine
                        + "-----------------------------------------" + Environment.NewLine
                        + gBoxReceiverControl_05.Text + " Text Clear Successfully!" + Environment.NewLine
                        + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                        + "-----------------------------------------");
        }

        private void btnClearReceiverTxt_06_Click(object sender, EventArgs e)
        {
            tBoxReceiverOutput_06.Text = string.Empty;
            tBoxLog_03.Text += tBoxLog_03.Text.Insert(0, Environment.NewLine
                        + "-----------------------------------------" + Environment.NewLine
                        + gBoxReceiverControl_06.Text + " Text Clear Successfully!" + Environment.NewLine
                        + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                        + "-----------------------------------------");
        }

        private void btnClearOverallReceiver_Click(object sender, EventArgs e)
        {
            tBoxOverallReceiver.Text = string.Empty;
            tBoxLog_03.Text = tBoxLog_03.Text.Insert(0, Environment.NewLine
                        + "-----------------------------------------" + Environment.NewLine
                        + " Text Clear Successfully!" + Environment.NewLine
                        + "Date & Time :" + DateTime.Now.ToString("[dd-MM-yyyy] hh:mm:ss") + Environment.NewLine
                        + "-----------------------------------------");
        }

        /* ========== MQTT Functions ========== */
    }
}
