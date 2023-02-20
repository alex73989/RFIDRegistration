using Newtonsoft.Json;
using RFIDRegistration.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Collections.Specialized.BitVector32;

namespace RFIDRegistration
{
    public partial class RPCConfig : Form
    {
        // WIRIO3_58BF25A85088
        // WIRIO3_68B6B329DEAC
        public string GetData, GetIPAddress, GetWirioID;
        public static RPCConfig instance;

        public RPCConfig()
        {
            InitializeComponent();
            InitializeTextBox();
            instance = this;
        }

        public void ResetWifiPersonal()
        {
            tBox_wp_ssid.Text = "";
            tBox_wp_password.Text = "";
            tBox_wp_bassid.Text = "";
        }
        
        public void ResetWifiTTLS()
        {
            tBox_wttls_ssid.Text = "";
            tBox_wttls_username.Text = "";
            tBox_wttls_password.Text = "";
            tBox_wttls_bassid.Text = "";
            tBox_wttls_eapid.Text = "";

            cBox_wttls_EAPIDType.Items.Clear();
            cBox_wttls_Phase2Type.Items.Clear();

            cBox_wttls_EAPIDType.Items.AddRange(new string[] 
            {
                "0 = Anonymous",
                "1 = Same as Username",
                "2 = Custom"
            });

            cBox_wttls_Phase2Type.Items.AddRange(new string[]
            {
                "0 = PEAP",
                "1 = EAP",
                "2 = EAP-MsCHAPv2",
                "3 = EAP-MsCHAP",
                "4 = EAP-TTLS / PAP",
                "5 = EAP_CHAP"
            });
        }

        public void ResetWifiTLS()
        {
            tBox_wtls_ssid.Text = "";
            tBox_wtls_bassid.Text = "";
            cBox_wtls_EAPIDType.Items.Clear();

            cBox_wtls_EAPIDType.Items.AddRange(new string[]
            {
                "0 = Anonymous",
                "1 = Not Valid",
                "2 = Custom"
            });
        }

        public void ResetStaticIP()
        {
            rB_static_staticip.Checked = true;
            rB_static_dhcp.Checked = false;

            tBox_static_ip.Text = "";
            tBox_static_netmask.Text = "";
            tBox_static_gateway.Text = "";
            tBox_static_dns1.Text = "";
            tBox_static_dns2.Text = "";
        }

        public void ResetSNTP()
        {
            tBox_sntp_serveradrs.Text = "";
            tBox_sntp_devicetz.Text = "";
        }

        public void ResetMQTT()
        {
            rB_mqtt_anonyenable.Checked = true;
            rB_mqtt_anonydisable.Checked = false;
            rB_mqtt_brokerdisable.Checked = true;
            rB_mqtt_brokerenable.Checked = false;

            tBox_mqtt_serveradrs.Text = "";
            tBox_mqtt_port.Text = "";
            tBox_mqtt_wsprefix.Text = "";
            tBox_mqtt_username.Text = "";
            tBox_mqtt_password.Text = "";
            tBox_mqtt_keepalive.Text = "";
            tBox_mqtt_pubtele.Text = "";
            tBox_mqtt_pubatt.Text = "";
            tBox_mqtt_subatt.Text = "";
            tBox_mqtt_subrpcreq.Text = "";
            tBox_mqtt_subrpcreply.Text = "";

            cBox_mqtt_connectiontype.Items.Clear();
            cBox_mqtt_qos.Items.Clear();
            cBox_mqtt_topictype.Items.Clear();

            cBox_mqtt_connectiontype.Items.AddRange(new string[]
            {
                "0 = TCP Stream without encryption",
                "1 = WebSocket without encryption",
                "2 = TCP Stream with Self-Sign Cert",
                "3 = WebSocket with Self-Sign Cert",
                "4 = TCP Stream with CA Cert",
                "5 = WebSocket with CA Cert"
            });

            cBox_mqtt_qos.Items.AddRange(new string[]
            {
                "QoS Lvl 1",
                "QoS Lvl 2",
                "QoS Lvl 3"
            });

            cBox_mqtt_topictype.Items.AddRange(new string[]
            {
                "0 = WiRIO3",
                "1 = Thinkboard",
                "2 = Custom"
            });
        }

        public void ResetLoadCertWifiCA()
        {
            tBox_loadcertwifica_name.Text = "";
            tBox_loadcertwifica_cert.Text = "";
        }

        public void ResetLoadCertUser()
        {
            tBox_loadcertuser_name.Text = "";
            tBox_loadcertwifica_name.Text = "";
        }

        public void ResetLoadCertKey()
        {
            tBox_loadcertkey_name.Text = "";
            tBox_loadcertkey_cert.Text = "";
            tBox_loadcertkey_password.Text = "";
        }

        public void ResetMQTTCA()
        {
            tBox_mqttca_name.Text = "";
            tBox_mqttca_cert.Text = "";
        }

        private void btn_wp_clear_Click(object sender, EventArgs e)
        {
            ResetWifiPersonal();
            SetLogText("(WIFI Personal) Reset Successfully!");
        }

        private void btn_wttls_clear_Click(object sender, EventArgs e)
        {
            ResetWifiTTLS();
            SetLogText("(WIFI TTLS) Reset Successfully!");
        }

        private void btn_wtls_clear_Click(object sender, EventArgs e)
        {
            ResetWifiTLS();
            SetLogText("(WIFI TLS) Reset Successfully!");
        }

        private void btn_static_clear_Click(object sender, EventArgs e)
        {
            ResetStaticIP();
            SetLogText("(Static IP) Reset Successfully!");
        }

        private void btn_sntp_clear_Click(object sender, EventArgs e)
        {
            ResetSNTP();
            SetLogText("(SNTP) Reset Successfully!");
        }

        private void btn_mqtt_clear_Click(object sender, EventArgs e)
        {
            ResetMQTT();
            SetLogText("(MQTT) Reset Successfully!");
        }

        private void btn_loadcertwifica_clear_Click(object sender, EventArgs e)
        {
            ResetLoadCertWifiCA();
            SetLogText("(LoadCert WifiCA) Reset Successfully!");
        }

        private void btn_loadcertuser_clear_Click(object sender, EventArgs e)
        {
            ResetLoadCertUser();
            SetLogText("(LoadCert User) Reset Successfully!");
        }

        private void btn_loadcertkey_clear_Click(object sender, EventArgs e)
        {
            ResetLoadCertKey();
            SetLogText("(LoadCert Key) Reset Successfully!");
        }

        private void btn_mqttca_clear_Click(object sender, EventArgs e)
        {
            ResetMQTTCA();
            SetLogText("(LoadCert MQTTCA) Reset Successfully!");
        }

        private void rB_static_staticip_CheckedChanged(object sender, EventArgs e)
        {
            if (rB_static_staticip.Checked == true)
            {
                tBox_static_ip.Enabled = true;
                tBox_static_netmask.Enabled = true;
                tBox_static_gateway.Enabled = true;
                tBox_static_dns1.Enabled = true;
                tBox_static_dns2.Enabled = true;
            }
        }

        private void rB_static_dhcp_CheckedChanged(object sender, EventArgs e)
        {
            if (rB_static_dhcp.Checked == true)
            {
                tBox_static_ip.Enabled = false;
                tBox_static_netmask.Enabled = false;
                tBox_static_gateway.Enabled = false;
                tBox_static_dns1.Enabled = false;
                tBox_static_dns2.Enabled = false;
            }
        }

        private void rB_mqtt_anonyenable_CheckedChanged(object sender, EventArgs e)
        {
            if (rB_mqtt_anonyenable.Checked == true)
            {
                tBox_mqtt_username.Enabled = false;
                tBox_mqtt_password.Enabled = false;
                rB_mqtt_anonydisable.Checked = false;
            }
        }

        private void rB_mqtt_anonydisable_CheckedChanged(object sender, EventArgs e)
        {
            if (rB_mqtt_anonydisable.Checked == true)
            {
                tBox_mqtt_username.Enabled = true;
                tBox_mqtt_password.Enabled = true;
                rB_mqtt_anonyenable.Checked = false;
            }
        }

        private void rB_mqtt_brokerenable_CheckedChanged(object sender, EventArgs e)
        {
            if (rB_mqtt_brokerenable.Checked == true)
            {
                rB_mqtt_brokerdisable.Checked = false;
            }
        }

        private void rB_mqtt_brokerdisable_CheckedChanged(object sender, EventArgs e)
        {
            if (rB_mqtt_brokerdisable.Checked == true)
            {
                rB_mqtt_brokerenable.Checked = false;
            }

        }

        private void btn_ClearCommand_Click(object sender, EventArgs e)
        {
            tBox_commanddelivered.Text = "";
            SetLogText("Command Delivered Text Cleared!");
        }

        private void tBox_commanddelivered_TextChanged(object sender, EventArgs e)
        {
            if(tBox_commanddelivered.Text != null)
            {
                btn_Submit.Enabled = true;
                btn_ClearCommand.Enabled = true;
            }
            else if (tBox_commanddelivered.Text == null)
            {
                btn_Submit.Enabled = false;
                btn_ClearCommand.Enabled = false;
            }
        }

        public void InitializeTextBox()
        {
            ResetWifiPersonal();
            ResetWifiTTLS();
            ResetWifiTLS();
            ResetStaticIP();
            ResetMQTT();
            ResetLoadCertWifiCA();
            ResetLoadCertUser();
            ResetLoadCertKey();
            ResetMQTTCA();

            btn_Submit.Enabled = false;

            tBox_wttls_eapid.Enabled = false;
            tBox_wtls_eapid.Enabled = false;

            tBox_mqtt_pubtele.Enabled = false;
            tBox_mqtt_pubatt.Enabled = false;
            tBox_mqtt_subatt.Enabled = false;
            tBox_mqtt_subrpcreq.Enabled = false;
            tBox_mqtt_subrpcreply.Enabled = false;
        }

        private void btn_wp_proceed_Click(object sender, EventArgs e)
        {
            if (tBox_wp_ssid.Text != null && tBox_wp_password.Text != null && tBox_wp_bassid.Text != null)
            {
                tBox_commanddelivered.Text = "{\"r.sysconfig.para\":" +
                    "{\"section\":\"wifi_personal\" " +
                    ",\"ssid\":\"" + tBox_wp_ssid.Text.ToString() + "\"" +
                    ",\"password\":\"" + tBox_wp_password.Text.ToString() + "\"" +
                    ",\"bssid\":\"" + tBox_wp_bassid.Text.ToString() + "\"" +
                    "}}";
            }
            else
            {
                SetLogText("(WIFI Personal) Unable to proceed the data with null, Please try again!");
                tBox_commanddelivered.Text = "";
            }
        }

        private void btn_wttls_proceed_Click(object sender, EventArgs e)
        {
            if (tBox_wttls_ssid.Text != null && tBox_wttls_username.Text != null
                && tBox_wttls_password.Text != null && tBox_wttls_bassid.Text != null
                && cBox_wttls_EAPIDType.Text != null && cBox_wttls_Phase2Type.Text != null)
            {
                tBox_commanddelivered.Text = "{\"r.sysconfig.para\":" +
                    "{\"section\":\"wifi_ttls\" " +
                    ",\"ssid\":\"" + tBox_wttls_ssid.Text.ToString() + "\"" +
                    ",\"username\":\"" + tBox_wttls_username.Text.ToString() + "\"" +
                    ",\"password\":\"" + tBox_wttls_password.Text.ToString() + "\"" +
                    ",\"bssid\":\"" + tBox_wttls_bassid.Text.ToString() + "\"";

                if (cBox_wttls_EAPIDType.Text == "0 = Anonymous")
                {
                    tBox_commanddelivered.Text += ",\"eapidtype\":\"0\"";
                }
                else if(cBox_wttls_EAPIDType.Text == "1 = Same as Username")
                {
                    tBox_commanddelivered.Text += ",\"eapidtype\":\"1\"";
                }
                else if(cBox_wttls_EAPIDType.Text == "2 = Custom")
                {
                    tBox_commanddelivered.Text += ",\"eapidtype\":\"2\"" +
                    ",\"eapid\":\"" + tBox_wttls_eapid.Text.ToString() + "\"";
                }

                if (cBox_wttls_Phase2Type.Text == "0 = PEAP")
                {
                    tBox_commanddelivered.Text += ",\"phase2type\":\"0\"" +
                    "}}";
                }
                else if (cBox_wttls_Phase2Type.Text == "1 = EAP")
                {
                    tBox_commanddelivered.Text += ",\"phase2type\":\"1\"" +
                    "}}";
                }
                else if(cBox_wttls_Phase2Type.Text == "2 = EAP-MsCHAPv2")
                {
                    tBox_commanddelivered.Text += ",\"phase2type\":\"2\"" +
                    "}}";
                }
                else if (cBox_wttls_Phase2Type.Text == "3 = EAP-MsCHAP")
                {
                    tBox_commanddelivered.Text += ",\"phase2type\":\"3\"" +
                    "}}";
                }
                else if (cBox_wttls_Phase2Type.Text == "4 = EAP-TTLS / PAP")
                {
                    tBox_commanddelivered.Text += ",\"phase2type\":\"4\"" +
                    "}}";
                }
                else if (cBox_wttls_Phase2Type.Text == "5 = EAP_CHAP")
                {
                    tBox_commanddelivered.Text += ",\"phase2type\":\"5\"" +
                    "}}";
                }

            }
            else
            {
                SetLogText("(WIFI TTLS) Unable to proceed the data with null, Please try again!");
                tBox_commanddelivered.Text = "";
            }
        }

        private void btn_wtls_proceed_Click(object sender, EventArgs e)
        {
            if(tBox_wtls_ssid.Text != null && tBox_wtls_bassid.Text != null
                && cBox_wtls_EAPIDType.Text != null)
            {
                tBox_commanddelivered.Text = "{\"r.sysconfig.para\":" +
                    "{\"section\":\"wifi_tls\" " +
                    ",\"ssid\":\"" + tBox_wtls_ssid.Text.ToString() + "\"" +
                    ",\"bassid\":\"" + tBox_wtls_bassid.Text.ToString() + "\"";

                if (cBox_wtls_EAPIDType.Text == "0 = Anonymous")
                {
                    tBox_commanddelivered.Text += ",\"eapidtype\":\"0\"" +
                    "}}";
                }
                else if (cBox_wtls_EAPIDType.Text == "1 = Not Valid")
                {
                    tBox_commanddelivered.Text += ",\"eapidtype\":\"1\"" +
                    "}}";
                }
                else if(cBox_wtls_EAPIDType.Text == "2 = Custom")
                {
                    tBox_commanddelivered.Text += ",\"eapidtype\":\"2\"" +
                        ",\"eapid\":\"" + tBox_wtls_eapid.Text.ToString() + "\"" +
                    "}}";
                }
            }
            else
            {
                SetLogText("(WIFI TLS) Unable to proceed the data with null, Please try again!");
                tBox_commanddelivered.Text = "";
            }
        }

        private void btn_ethernet_proceed_Click(object sender, EventArgs e)
        {
            if (rB_ethernet_enable.Checked == true)
            {
                tBox_commanddelivered.Text = "{\"r.sysconfig.para\":" +
                    "{\"section\":\"ethernet\" " +
                    ",\"enable\":\"1\" }}";
            }
            else if(rB_ethernet_enable.Checked == false)
            {
                tBox_commanddelivered.Text = "{\"r.sysconfig.para\":" +
                    "{\"section\":\"ethernet\" " +
                    ",\"enable\":\"0\" }}";
            }
        }

        private void btn_static_proceed_Click(object sender, EventArgs e)
        {
            if ( (rB_static_staticip.Checked == true || rB_static_dhcp.Checked == true) 
                && tBox_static_ip.Text != null && tBox_static_netmask.Text != null
                && tBox_static_gateway.Text != null && tBox_static_dns1.Text != null
                && tBox_static_dns2.Text != null)
            {
                if (rB_static_staticip.Checked == true)
                {
                    tBox_commanddelivered.Text = "{\"r.sysconfig.para\":" +
                    "{\"section\":\"staticip\" " +
                    ",\"enable\":\"1\"" + // Enable = 1 / true will enable the Static IP
                    ",\"ip\":\"" + tBox_static_ip.Text.ToString() + "\"" +
                    ",\"netmask\":\"" + tBox_static_netmask.Text.ToString() + "\"" +
                    ",\"gateway\":\"" + tBox_static_gateway.Text.ToString() + "\"" +
                    ",\"dns1\":\"" + tBox_static_dns1.Text.ToString() + "\"" +
                    ",\"dns2\":\"" + tBox_static_dns2.Text.ToString() + "\"" +
                    "}}";
                }
                else if (rB_static_dhcp.Checked == true)
                {
                    tBox_commanddelivered.Text = "{\"r.sysconfig.para\":" +
                    "{\"section\":\"staticip\" " +
                    ",\"enable\":\"0\" " + // Enable = 0 / false will enable the DHCP
                    "}}";
                }
            }
            else
            {
                SetLogText("(Static IP) Unable to proceed the data with null, Please try again!");
                tBox_commanddelivered.Text = "";
            }
        }

        private void btn_sntp_proceed_Click(object sender, EventArgs e)
        {
            if ((rB_sntp_enable.Checked == true || rB_sntp_disable.Checked == true) 
                && tBox_sntp_serveradrs.Text != null && tBox_sntp_devicetz.Text != null)
            {
                if (rB_sntp_enable.Checked == true)
                {
                    tBox_commanddelivered.Text = "{\"r.sysconfig.para\":" +
                    "{\"section\":\"sntp\" " +
                    ",\"enable\":\"1\"" +
                    ",\"addr\":\"" + tBox_sntp_serveradrs.Text.ToString() + "\"" +
                    ",\"tz\":\"" + tBox_sntp_devicetz.Text.ToString() + "\"" +
                    "}}";
                }
                else if (rB_sntp_disable.Checked == true)
                {
                    tBox_commanddelivered.Text = "{\"r.sysconfig.para\":" +
                    "{\"section\":\"sntp\" " +
                    ",\"enable\":\"0\" " +
                    "}}";
                }
            }
            else
            {
                SetLogText("(SNTP) Unable to proceed the data with null, Please try again!");
                tBox_commanddelivered.Text = "";
            }
        }

        private void btn_mqtt_proceed_Click(object sender, EventArgs e)
        {
            if(rB_mqtt_brokerenable.Checked == true)
            {
                // port 1883
                tBox_commanddelivered.Text = "{\"r.sysconfig.para\":" +
                    "{\"section\":\"mqtt\" " +
                    ",\"enable\":\"1\" " +
                    ",\"addr\":\"" + tBox_mqtt_serveradrs.Text.ToString() + "\"" +
                    ",\"port\":\"" + tBox_mqtt_port.Text.ToString() + "\"" ;

                if (cBox_mqtt_connectiontype.Text == "0 = TCP Stream without encryption")
                {
                    tBox_commanddelivered.Text += ",\"conntype\":\"0\"";
                }
                else if (cBox_mqtt_connectiontype.Text == "1 = WebSocket without encryption")
                {
                    tBox_commanddelivered.Text += ",\"conntype\":\"1\"";
                }
                else if (cBox_mqtt_connectiontype.Text == "2 = TCP Stream with Self-Sign Cert")
                {
                    tBox_commanddelivered.Text += ",\"conntype\":\"2\"";
                }
                else if (cBox_mqtt_connectiontype.Text == "3 = WebSocket with Self-Sign Cert")
                {
                    tBox_commanddelivered.Text += ",\"conntype\":\"3\"";
                }
                else if (cBox_mqtt_connectiontype.Text == "4 = TCP Stream with CA Cert")
                {
                    tBox_commanddelivered.Text += ",\"conntype\":\"4\"";
                }
                else if (cBox_mqtt_connectiontype.Text == "5 = WebSocket with CA Cert")
                {
                    tBox_commanddelivered.Text += ",\"conntype\":\"5\"";
                }

                tBox_commanddelivered.Text += ",\"wsprefix\":\"" + tBox_sntp_serveradrs.Text.ToString() + "\"" ;

                if (rB_mqtt_anonyenable.Checked == false && tBox_mqtt_username.Text != null && tBox_wp_password.Text != null)
                {
                    tBox_commanddelivered.Text += ",\"anonylogin\":\"0\"" ;
                }
                else if (tBox_mqtt_username.Text == null)
                {
                    SetLogText("(MQTT) Unable to proceed the data with null in Username, Please try again!");
                }
                else if (tBox_mqtt_password.Text == null)
                {
                    SetLogText("(MQTT) Unable to proceed the data with null in Password, Please try again!");
                }

                if (cBox_mqtt_qos.Text != null)
                {
                    if (cBox_mqtt_qos.Text == "QoS Lvl 1")
                    {
                        tBox_commanddelivered.Text += ",\"qos\":\"1\"";
                    }
                    else if (cBox_mqtt_qos.Text == "QoS Lvl 2")
                    {
                        tBox_commanddelivered.Text += ",\"qos\":\"2\"";
                    }
                    else if (cBox_mqtt_qos.Text == "QoS Lvl 3")
                    {
                        tBox_commanddelivered.Text += ",\"qos\":\"3\"";
                    }
                }
                else
                {
                    SetLogText("(MQTT) Unable to proceed the data with null in QoS, Please try again!");
                }
                // minimum 30seconds
                tBox_commanddelivered.Text += ",\"keepalive\":\"" + tBox_mqtt_keepalive.Text.ToString() + "\"" ;

                if (cBox_mqtt_topictype.Text == "0 = WiRIO3")
                {
                    tBox_commanddelivered.Text += ",\"topictype\":\"0\"";
                }
                else if (cBox_mqtt_topictype.Text == "1 = Thinkboard")
                {
                    tBox_commanddelivered.Text += ",\"topictype\":\"1\"";
                }
                else if (cBox_mqtt_topictype.Text == "2 = Custom")
                {
                    tBox_commanddelivered.Text += ",\"topictype\":\"2\"" +
                        ",\"tpp_tele\":\"" + tBox_mqtt_pubtele.Text.ToString() + "\"" +
                        ",\"tpp_attb\":\"" + tBox_mqtt_pubatt.Text.ToString() + "\"" +
                        ",\"tps_attb\":\"" + tBox_mqtt_subatt.Text.ToString() + "\"" +
                        ",\"tpp_tele\":\"" + tBox_mqtt_subrpcreq.Text.ToString() + "\"" +
                        ",\"tpp_tele\":\"" + tBox_mqtt_subrpcreply.Text.ToString() + "\"" ;
                }

                tBox_commanddelivered.Text += "}}";
            }
            else if (rB_mqtt_brokerdisable.Checked == true)
            {
                SetLogText("(MQTT) Unable to proceed the data, Please Enable the MQTT Broker Setting!");
                tBox_commanddelivered.Text = "";
            }
        }

        private void btn_loadcertwifica_proceed_Click(object sender, EventArgs e)
        {
            if (tBox_loadcertwifica_cert.Text != null && tBox_loadcertwifica_name.Text != null)
            {
                tBox_commanddelivered.Text = "{\"r.sysconfig.para\":" +
                    "{\"section\":\"loadcert_wifica\" " +
                    ",\"name\":\"" + tBox_loadcertwifica_name.Text.ToString() + "\"" +
                    ",\"cert\":\"" + tBox_loadcertwifica_cert.Text.ToString() + "\" }}";
            }
            else
            {
                SetLogText("(LoadCert WifiCA) Unable to proceed the data with null, Please Try Again!");
                tBox_commanddelivered.Text = "";
            }
        }

        private void btn_loadcertuser_proceed_Click(object sender, EventArgs e)
        {
            if (tBox_loadcertuser_cert.Text != null && tBox_loadcertuser_name.Text != null)
            {
                tBox_commanddelivered.Text = "{\"r.sysconfig.para\":" +
                    "{\"section\":\"loadcert_user\" " +
                    ",\"name\":\"" + tBox_loadcertuser_name.Text.ToString() + "\"" +
                    ",\"cert\":\"" + tBox_loadcertuser_cert.Text.ToString() + "\" }}";
            }
            else
            {
                SetLogText("(LoadCert User) Unable to proceed the data with null, Please Try Again!");
                tBox_commanddelivered.Text = "";
            }
        }

        private void btn_loadcertkey_proceed_Click(object sender, EventArgs e)
        {
            if (tBox_loadcertkey_cert.Text != null && tBox_loadcertkey_name.Text != null)
            {
                tBox_commanddelivered.Text = "{\"r.sysconfig.para\":" +
                    "{\"section\":\"loadcert_key\" " +
                    ",\"name\":\"" + tBox_loadcertkey_name.Text.ToString() + "\"" +
                    ",\"cert\":\"" + tBox_loadcertkey_cert.Text.ToString() + "\"" +
                    ",\"password\":\"" + tBox_loadcertkey_password.Text.ToString() + "\" }}";
            }
            else
            {
                SetLogText("(LoadCert Key) Unable to proceed the data with null, Please Try Again!");
                tBox_commanddelivered.Text = "";
            }
        }

        private void btn_mqttca_proceed_Click(object sender, EventArgs e)
        {
            if (tBox_mqttca_cert.Text != null && tBox_mqttca_name.Text != null)
            {
                tBox_commanddelivered.Text = "{\"r.sysconfig.para\":" +
                    "{\"section\":\"loadcert_mqttca\" " +
                    ",\"name\":\"" + tBox_mqttca_name.Text.ToString() + "\"" +
                    ",\"cert\":\"" + tBox_mqttca_cert.Text.ToString() + "\" }}";
            }
            else
            {
                SetLogText("(LoadCert MQTTCA) Unable to proceed the data with null, Please Try Again!");
                tBox_commanddelivered.Text = "";
            }
        }

        private void btn_GetCurrentNewSetting_Click(object sender, EventArgs e)
        {
            tBox_commanddelivered.Text = "{\"r.sysconfig.get\":0}";
        }

        private void btn_SaveSettingsReboot_Click(object sender, EventArgs e)
        {
            tBox_commanddelivered.Text = "{\"r.sysconfig.save\":1}";
        }

        private void btn_UndoSettings_Click(object sender, EventArgs e)
        {
            tBox_commanddelivered.Text = "{\"r.sysconfig.reset\":1}";
        }

        private void rB_ethernet_enable_CheckedChanged(object sender, EventArgs e)
        {
            if(rB_ethernet_enable.Checked == true)
            {
                rB_ethernet_disable.Checked = false;
            }
        }

        private void rB_ethernet_disable_CheckedChanged(object sender, EventArgs e)
        {
            if (rB_ethernet_disable.Checked == true)
            {
                rB_ethernet_enable.Checked = false;
            }
        }

        private void cBox_wttls_EAPIDType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cBox_wttls_EAPIDType.Text == "2 = Custom")
            {
                tBox_wttls_eapid.Enabled = true;
            }
            else
            {
                tBox_wttls_eapid.Enabled = false;
            }
        }

        private void cBox_wtls_EAPIDType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cBox_wtls_EAPIDType.Text == "2 = Custom")
            {
                tBox_wtls_eapid.Enabled = true;
            }
            else
            {
                tBox_wtls_eapid.Enabled = false;
            }
        }

        private void btn_Submit_Click(object sender, EventArgs e)
        {
            IndicatorForm.instance.RPCTxt.Text = tBox_commanddelivered.Text.ToString();
            IndicatorForm.instance.PubAttb.Enabled = false;
            IndicatorForm.instance.PubRPCReq.Enabled = true;
            SetLogText("Successfully Submit RPC Reply!");
        }

        private void cBox_mqtt_topictype_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cBox_mqtt_topictype.Text == "2 = Custom")
            {
                tBox_mqtt_pubtele.Enabled = true;
                tBox_mqtt_pubatt.Enabled = true;
                tBox_mqtt_subatt.Enabled = true;
                tBox_mqtt_subrpcreq.Enabled = true;
                tBox_mqtt_subrpcreply.Enabled = true;
            }
            else
            {
                tBox_mqtt_pubtele.Enabled = false;
                tBox_mqtt_pubatt.Enabled = false;
                tBox_mqtt_subatt.Enabled = false;
                tBox_mqtt_subrpcreq.Enabled = false;
                tBox_mqtt_subrpcreply.Enabled = false;
            }
        }

        public void SetLogText(string GetText)
        {
            tBox_log.Text += DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + " : " + Environment.NewLine + GetText + Environment.NewLine + Environment.NewLine;
        }
    }
}
