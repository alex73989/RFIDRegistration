using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace RFIDRegistration.Models
{
    public class RFID_detail_Json_Attr
    {
        public RFID_detail_Json_Attr() 
        {
            Epoch_Valid = "";
            Rpc_Busy = "";
            Epoch_Sec = "";
            Rate_Control = "";
            Rate_Control_Period = "";
            Channel_No = "";
            Power_Max = "";
            Power_Min = "";
            CacheSpace_Max = "";
            RFID_Temp = "";
            RFID_Firmware_ID = "";
            Read_Period = "";
            Auto = "";
            Period = "";
            Cache_Tag_Remove = "";
            Cache_Period = "";
            Tag_Remove_Upd = "";
            Anthenna_Channel_Upd = "";
            EnbForce_Upd = "";
            Force_Update_Period = "";
            Self_Filter_Option = "";
            Self_Filter_Addr = "";
            Self_Filter_Len_Bit = "";
            Self_Filter_Data = "";
            Self_Filter_Invert = "";
            Enb_Tag_Data = "";
            Tag_Data_Mem_Bank = "";
            Tag_Data_Reader_Addr = "";
            Tag_Data_Word_Count = "";
            EnbTagPass = "";
            Tag_Pass = "";
            Dynamic_Q = "";
            Q = "";
            Se_Mode = "";
            Target = "";
            EPC_Extended = "";
            Demo_Mode = "";

        }

        /* ---------- Device System Attribute ---------- */

        [JsonProperty("deviceid")]
        public string Device_ID { get; set; }

        [JsonProperty("pktno")]
        public string Pktno { get; set; }

        // Device Wi-Fi Received Signal Strength
        [JsonProperty("rssi")]
        public string Wifi_Received_Signal_Strength { get; set; }

        /* ---------- System Only Attribute ---------- */

        [JsonProperty("d.sys.addr")]
        public string IP_Address { get; set; }

        [JsonProperty("d.sys.iface")]
        public string Connection_Type { get; set; }

        [JsonProperty("d.sys.ssid")]
        public string SSID { get; set; }

        // Firmware ID
        [JsonProperty("d.sys.fwid")]
        public string FWID { get; set; }

        [JsonProperty("d.sys.datecode")]
        public string Date_Code { get; set; }

        [JsonProperty("d.sys.model")]
        public string Model { get; set; }

        [JsonProperty("d.sys.name")]
        public string Device_Name { get; set; }

        [JsonProperty("d.sys.desc")]
        public string Device_Desc { get; set; }

        [JsonProperty("d.sys.perip")]
        public List<Peripharal> Perip_Items { get; set; }

        [JsonProperty("d.sys.linkup")]
        public string Connection { get; set; }

        [JsonProperty("d.sys.epochvalid")]
        public string Epoch_Valid { get; set; }

        [JsonProperty("d.sys.rpcbusy")]
        public string Rpc_Busy { get; set; }

        /* ---------- Share Attribute ---------- */

        // Current Epoch Times in second
        [JsonProperty("s.sys.epochsec")]
        public string Epoch_Sec { get; set; }

        /* ---------- Device Peripheral - RFID Reader ---------- */

        // Indicate Number of Available RFID Channel
        [JsonProperty("d.uhfrfid.ch")]
        public string Channel_No { get; set; }

        [JsonProperty("d.uhfrfid.pwrmax")]
        public string Power_Max { get; set; }

        [JsonProperty("d.uhfrfid.pwrmin")]
        public string Power_Min { get; set; }

        // Maximum Tag Cache Memory Size (In Number of Tag)
        [JsonProperty("d.uhfrfid.csmax")]
        public string CacheSpace_Max { get; set; }

        // Check each element indicates if the antenna is connected / using
        [JsonProperty("d.uhfrfid.antstate")]
        public string[] RFID_Mode { get; set; }

        // Check whether the Antenna Channel is available or shutdown
        [JsonProperty("d.uhfrfid.antison")]
        public string[] Anthenna_Output { get; set; }

        [JsonProperty("d.uhfrfid.temp")]
        public string RFID_Temp { get; set; }

        // RFID Reader Module Firmware ID
        [JsonProperty("d.uhfrfid.readerver")]
        public string RFID_Firmware_ID { get; set; }

        /* ---------- Tag Inventory and Cache Setting ---------- */

        /*  Period in millisecond for device to read the tag in multi tag reading loop
            Setting Range is 500ms - 10000ms */
        [JsonProperty("s.uhfrfid.devreadperiod")]
        public string Read_Period { get; set; }

        /* True = Continue Reading when r.uhfrfid.start = true 
            * and stop when r.uhfrfid.start = false
            * 
            * False = Only read upon request when r.uhfrfid.start = true
            * and stop when reach s.uhfrfid.period
        */
        [JsonProperty("s.uhfrfid.auto")]
        public string Auto { get; set; }

        // Reading Period in second when s.uhrfid.auto = false
        [JsonProperty("s.uhfrfid.period")]
        public string Period { get; set; }

        /*  Enable / Disable input trigger from on board input port to start
         *  the Tag Reading / inventory process when s.uhrfid.auto = false */
        [JsonProperty("s.uhfrfid.inptrig")]
        public string[] Input_Trigger { get; set; }

        [JsonProperty("s.uhfrfid.cachetagremove")]
        public string Cache_Tag_Remove { get; set; }

        /* Period for the detected tag to be stay in the Tag Cache
            * before remove it.
            */
        [JsonProperty("s.uhfrfid.cacheperiod")]
        public string Cache_Period { get; set; }

        [JsonProperty("s.uhfrfid.tagremoveupd")]
        public string Tag_Remove_Upd { get; set; }

        [JsonProperty("s.uhfrfid.antchangeupd")]
        public string Anthenna_Channel_Upd { get; set; }

        // While true, system will send out the tag info that currently in the cache memory
        [JsonProperty("s.uhfrfid.enbforceupd")]
        public string EnbForce_Upd { get; set; }

        [JsonProperty("s.uhfrfid.forceupdperiod")]
        public string Force_Update_Period { get; set; }

        /* ---------- Tag Selection / Singulation Filter Criteria ---------- */

        [JsonProperty("s.uhfrfid.selfilteroption")]
        public string Self_Filter_Option { get; set; }

        [JsonProperty("s.uhfrfid.selfilteraddr")]
        public string Self_Filter_Addr { get; set; }

        [JsonProperty("s.uhfrfid.selfilterlenbit")]
        public string Self_Filter_Len_Bit { get; set; }

        [JsonProperty("s.uhfrfid.selfilterdata")]
        public string Self_Filter_Data { get; set; }

        [JsonProperty("s.uhfrfid.selfilterinvert")]
        public string Self_Filter_Invert { get; set; }

        /* Auto Tag Data Reading */

        [JsonProperty("s.uhfrfid.enbtagdata")]
        public string Enb_Tag_Data { get; set; }

        [JsonProperty("s.uhfrfid.tagdatamembank")]
        public string Tag_Data_Mem_Bank { get; set; }

        [JsonProperty("s.uhfrfid.tagdatareadaddr")]
        public string Tag_Data_Reader_Addr { get; set; }

        [JsonProperty("s.uhfrfid.tagdatawordcount")]
        public string Tag_Data_Word_Count { get; set; }

        [JsonProperty("s.uhfrfid.enbtagpass")]
        public string EnbTagPass { get; set; }

        [JsonProperty("s.uhfrfid.tagpass")]
        public string Tag_Pass { get; set; }

        /* ----------- Tag Reading Mode Setting ---------- */

        [JsonProperty("s.uhfrfid.dynamicq")]
        public string Dynamic_Q { get; set; }

        // Reader Q Factor 0-8 (Default is 4)
        [JsonProperty("s.uhfrfid.q")]
        public string Q { get; set; }

        // Tag Session Mode to S0-S3
        [JsonProperty("s.uhfrfid.semode")]
        public string Se_Mode { get; set; }

        [JsonProperty("s.uhfrfid.target")]
        public string Target { get; set; }

        /* When Enable the tag ID provided in Telematics will have
            * format as below:
            * 
            * PC (2 byte) + EPC + CRC (2 byte)
            * PC = Protocol Control Word
            * EPC = Tag EPC Memory, word size is depend on the PC word setting
            * CRC = Tag CRC Word calculate by the tag
            */
        [JsonProperty("s.uhfrfid.epcextended")]
        public string EPC_Extended { get; set; }

        /* ---------- Reading Setting ----------- */

        /*  0 = Auto Mode
            1 = Enable Mode (might dmg antenna if without antenna connected)
            2 = Disable Mode
        */
        [JsonProperty("s.uhfrfid.antenb")]
        public int[] Anthenna_Mode { get; set; }

        [JsonProperty("s.uhfrfid.power")]
        public int[] RFID_Current_Power { get; set; }

        /* ---------- Demo ----------- */

        // If Enable Demo Mode, all the RFID will return immediately without any delay
        [JsonProperty("s.uhfrfid.demo")]
        public string Demo_Mode { get; set; }

        [JsonProperty("s.outputport.setup")]
        public List<OutputPortConfig> OutputSetup { get; set; }

        [JsonProperty("s.inputport.setup")]
        public List<InputPortConfig> InputSetup { get; set; }

        /* ========== Temperature Sensor ========== */

        [JsonProperty("s.envsensor.ratectrl")]
        public string Rate_Control { get; set; }

        [JsonProperty("s.envsensor.ratectrlperiod")]
        public string Rate_Control_Period { get; set; }

        /*[JsonProperty("t.envsensor.param")]
        public List<EvnSensorParam> Sensor_Data { get; set; }*/
    }

    public class Peripharal
    {
        [JsonProperty("cmdkey")]
        public string Cmd_Key { get; set; }

        [JsonProperty("feature")]
        public string Feature { get; set; }
    }

    public class OutputPortConfig
    {
        [JsonProperty("ch")]
        public string Channel { get; set; }

        [JsonProperty("onishigh")]
        public string On_isHigh { get; set; }

        [JsonProperty("mode")]
        public string Mode_Output { get; set; }

        // Pulse per seconds for on
        [JsonProperty("fperiodon")]
        public string F_Period_On { get; set; }

        [JsonProperty("fperiodoff")]
        public string F_Period_Off { get; set; }

        [JsonProperty("pulsecnt")]
        public string Pulse_Cnt { get; set; }
    }

    public class InputPortConfig
    {
        [JsonProperty("ch")]
        public string Channel { get; set; }

        [JsonProperty("onishigh")]
        public string On_isHigh { get; set; }

        [JsonProperty("mode")]
        public string Mode_Output { get; set; }

        [JsonProperty("debounce")]
        public string Debounce { get; set; }
    }

    public class RFID_Tag_Json
    {
        public RFID_Tag_Json()
        {
            Sec = "";
        }

        [JsonProperty("deviceid")]
        public string Device_ID { get; set; }

        [JsonProperty("rssi")]
        public string RSSI { get; set; }

        [JsonProperty("pktno")]
        public string Pkt_No { get; set; }

        [JsonProperty("sec")]
        public string Sec { get; set; }

        [JsonProperty("t.uhfrfid.temp")]
        public string Device_Temp { get; set; }

        [JsonProperty("r.sysconfig.result")]
        public string ConfigResult { get; set; }

        [JsonProperty("t.uhfrfid.param")]
        public List<Telematic_Scan> Telematics_Data { get; set; }

        [JsonProperty("t.envsensor.param")]
        public Telematic_EnvSensor Telematics_SensorData { get; set; }

        [JsonProperty("r.sysconfig.get")]
        public RPC_Config RPC_ConfigData { get; set; }

    }

    public class Telematic_Scan
    {
        [JsonProperty("ts")]
        public string Time_Stamp { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("ch")]
        public string Channel { get; set; }

        [JsonProperty("rssi")]
        public string RSSI { get; set; }

        [JsonProperty("tag")]
        public string Tag_ID { get; set; }
    }

    public class Telematic_EnvSensor
    {
        [JsonProperty("sht4x_temp")]
        public string Temperature { get; set; }

        [JsonProperty("sht4x_hum")]
        public string Humidity { get; set; }

        [JsonProperty("bmp280a_pressure")]
        public string Pressure { get; set; }
    }

    public class RPC_Config
    {
        [JsonProperty("wifi")]
        public Wifi WifiObj { get; set; }

        [JsonProperty("staticip")]
        public StaticIP StaticIPObj { get; set; }

        [JsonProperty("sntp")]
        public SNTP SNTPObj { get; set; }

        [JsonProperty("mqtt")]
        public MQTT MQTTObj { get; set; }
    }

    public class Wifi
    {
        [JsonProperty("wificonntype")]
        public string Wifi_Connection_Type { get; set; }

        [JsonProperty("ssid")]
        public string SSID { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("bssid")]
        public string BSSID { get; set; }
    }

    public class StaticIP
    {
        [JsonProperty("enable")]
        public string Enable { get; set; }
    }

    public class SNTP
    {
        [JsonProperty("enable")]
        public string Enable { get; set; }
    }

    public class MQTT
    {
        [JsonProperty("enable")]
        public string Enable { get; set; }

        [JsonProperty("addr")]
        public string Address { get; set; }

        [JsonProperty("port")]
        public string Port { get; set; }

        [JsonProperty("conntype")]
        public string Connection_Type { get; set; }

        [JsonProperty("anonylogin")]
        public string Anony_Login { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("qos")]
        public string QOS { get; set; }

        [JsonProperty("keepalive")]
        public string Keep_Alive { get; set; }

        [JsonProperty("topictype")]
        public string TopicType { get; set; }
    }
}
