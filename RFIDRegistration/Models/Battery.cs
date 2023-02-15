using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RFIDRegistration.Models
{
    public class Battery
    {
        public string device_id { get; set; }
        public string pkt_no { get; set; }
        public string battery_name { get; set; }
        public string battery_level { get; set; }
        public string battery_status { get; set; }
        public string indicator_color { get; set; }
        public string rgb_mode { get; set; }
        public string indicator_buzz_mode { get; set; }
        public string period { get; set; }
    }

    public class Battery_detail_Json_Layer
    {
        [JsonProperty("rpcreply")]
        public Battery_detail_Json RPC_Reply { get; set; }
    }

    public class Battery_detail_Json_tele
    {
        [JsonProperty("telematics")]
        public Battery_detail_Json Telematics { get; set; }
    }

    public class Battery_detail_Json
    {
        [JsonProperty("deviceid")]
        public string Device_ID { get; set; }

        [JsonProperty("pktno")]
        public string Pkt_No { get; set; }

        [JsonProperty("r.enow.result")]
        public string Enow_Result { get; set; }

        [JsonProperty("r.enow.list")]
        public List<Information> Enow_List { get; set; }

        [JsonProperty("t.enow.button")]
        public List<Information> Enow_Button { get; set; }

        [JsonProperty("t.enow.battstate")]
        public List<Information> Enow_BattState { get; set; }

        [JsonProperty("t.enow.disconnected")]
        public List<Information> Enow_Disconnected { get; set; }

        [JsonProperty("t.enow.connected")]
        public List<Information> Enow_Connected { get; set; }

        [JsonProperty("t.enow.indicator")]
        public List<Information> Enow_Indicator { get; set; }
    }

    public class Information
    {
        [JsonProperty("nodeid")]
        public string Node_ID { get; set; }

        [JsonProperty("appid")]
        public string App_ID { get; set; }

        [JsonProperty("fwver")]
        public string Fwver { get; set; }

        [JsonProperty("pollrate")]
        public string Poll_Rate { get; set; }

        [JsonProperty("battlevel")]
        public string Battery_level { get; set; }

        [JsonProperty("battCharging")]
        public string Battery_Status { get; set; }

        [JsonProperty("ch")]
        public string Channel { get; set; }

        [JsonProperty("btn")]
        public string Btn { get; set; }

        [JsonProperty("rgbmode")]
        public string RGB_Mode { get; set; }

        [JsonProperty("period")]
        public string Period { get; set; }

        [JsonProperty("rgbcolor")]
        public int[] RGB_Color { get; set; }

        [JsonProperty("buzmode")]
        public string Buz_Mode { get; set; }
    }
}
