using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDRegistration.Models
{
    public class RPCGet
    {
        [JsonProperty("deviceid")]
        public string Device_ID { get; set; }

        [JsonProperty("rssi")]
        public string RSSI { get; set; }

        [JsonProperty("pktno")]
        public string Pkt_No { get; set; }

        [JsonProperty("r.sysconfig.get")]
        public List<SysConfigGet> System_Settings { get; set; }

        [JsonProperty("r.sysconfig.result")]
        public string System_Result { get; set; }

        [JsonProperty("certname_mqtt")]
        public string CertName_MQTT { get; set; }
    }

    public class SysConfigGet
    {
        [JsonProperty("wifi")]
        public List<Wifi_Get> Wifi { get; set; }

        [JsonProperty("ethernet")]
        public List<Ethernet_Get> Ethernet { get; set; }

        [JsonProperty("staticip")]
        public List<StaticIP_Get> StaticIP { get; set; }

        [JsonProperty("sntp")]
        public List<SNTP_Get> SNTP { get; set; }

        [JsonProperty("mqtt")]
        public List<MQTT_Get> MQTT { get; set; }
    }

    public class Wifi_Get
    {
        [JsonProperty("wificonntype")]
        public string Wifi_Connection_Type { get; set; }

        [JsonProperty("ssid")]
        public string SSID { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("bssid")]
        public string BSSID { get; set; }

        [JsonProperty("eapidtype")]
        public string EAPID_Type { get; set; }

        [JsonProperty("eapid")]
        public string EAPID { get; set; }

        [JsonProperty("phase2type")]
        public string Phase_2_Type { get; set; }
    }

    public class Ethernet_Get
    {
        [JsonProperty("enable")]
        public string Enable { get; set; }
    }

    public class StaticIP_Get
    {
        [JsonProperty("enable")]
        public string Enable { get; set; }

        [JsonProperty("ip")]
        public string IP { get; set; }

        [JsonProperty("netmask")]
        public string Netmask { get; set; }

        [JsonProperty("gateway")]
        public string Gateway { get; set; }

        [JsonProperty("dns1")]
        public string DNS_1 { get; set; }

        [JsonProperty("dns2")]
        public string DNS_2 { get; set; }
    }

    public class SNTP_Get
    {
        [JsonProperty("enable")]
        public string Enable { get; set; }

        [JsonProperty("addr")]
        public string Server_Address { get; set; }

        [JsonProperty("tz")]
        public string Time_Zone { get; set; }
    }

    public class MQTT_Get
    {
        [JsonProperty("enable")]
        public string Enable { get; set; }

        [JsonProperty("addr")]
        public string Broker_Address { get; set; }

        [JsonProperty("port")]
        public string Port_Number { get; set; }

        [JsonProperty("conntype")]
        public string Connection_Type { get; set; }

        [JsonProperty("wsprefix")]
        public string Web_Socket_Prefix { get; set; }

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
        public string Topic_Type { get; set; }

        [JsonProperty("tpp_tele")]
        public string Topic_Pub_Telemetry { get; set; }

        [JsonProperty("tpp_attb")]
        public string Topic_Pub_Attribute { get; set; }

        [JsonProperty("tps_attb")]
        public string Topic_Sub_Attribute { get; set; }

        [JsonProperty("tps_req")]
        public string Topic_Sub_RPCRequest { get; set; }

        [JsonProperty("tpp_reply")]
        public string Topic_Pub_RPCReply { get; set; }
    }
}
