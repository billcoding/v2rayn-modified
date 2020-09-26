using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using RestSharp;
using v2rayN.Handler;
using v2rayN.Mode;

namespace v2rayN.Forms
{
    public partial class AddCustomServerUrlForm : BaseForm
    {
        public int EditIndex { get; set; }
        VmessItem vmessItem;

        public AddCustomServerUrlForm()
        {
            InitializeComponent();
        }

        private void AddCustomServerUrlForm_Load(object sender, EventArgs e)
        {
            if (EditIndex >= 0)
            {
                vmessItem = config.vmess[EditIndex];
                BindingServer();
            }
            else
            {
                vmessItem = new VmessItem();
                ClearServer();
            }
        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        private void BindingServer()
        {
            txtUrl.Text = vmessItem.url;
            txtConfigFileName.Text = vmessItem.configFileName;
            txtRemarks.Text = vmessItem.remarks;
            txtRemarks.ReadOnly = true;
        }

        /// <summary>
        /// 清除设置
        /// </summary>
        private void ClearServer()
        {
            txtUrl.Text = "";
            txtRemarks.Text = "";
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            var url = txtUrl.Text.Trim();
            if (Utils.IsNullOrEmpty(url))
            {
                UI.ShowWarning(UIRes.I18N("PleaseFillUrl"));
                return;
            }

            if (!Regex.IsMatch(url, "^https?://.+$"))
            {
                UI.ShowWarning(UIRes.I18N("PleaseFillUrl"));
                return;
            }

            btnTest.Enabled = false;
            if (Utils.UrlIsExists(url))
            {
                UI.Show("有效的地址！！！");
            }
            else
            {
                UI.ShowWarning("无效的地址！！！");
            }

            btnTest.Enabled = true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string url = txtUrl.Text;
            if (Utils.IsNullOrEmpty(url))
            {
                UI.Show(UIRes.I18N("PleaseFillUrl"));
                return;
            }

            string configFileName = txtConfigFileName.Text;
            if (Utils.IsNullOrEmpty(configFileName))
            {
                UI.Show(UIRes.I18N("PleaseFillConfigFileName"));
                return;
            }

            string remark = txtRemarks.Text;
            if (Utils.IsNullOrEmpty(remark))
            {
                UI.Show(UIRes.I18N("PleaseFillRemarks"));
                return;
            }

            if (!Utils.UrlIsExists(url))
            {
                UI.ShowWarning("无效的地址！！！");
                return;
            }

            var configContent = Utils.GetRemoteCustomConfig(url);
            if (string.IsNullOrEmpty(configContent))
            {
                UI.ShowWarning("远程配置文件为空！！！");
                return;
            }

            vmessItem.url = url;
            vmessItem.configFileName = configFileName + ".json";
            vmessItem.address = "customConf/" + vmessItem.configFileName;
            vmessItem.remarks = remark;
            vmessItem.configType = (int) EConfigType.Custom;

            //创建配置文件
            if (!Directory.Exists("customConf"))
            {
                Directory.CreateDirectory("customConf");
            }

            File.WriteAllText(vmessItem.address, configContent);

            if (ConfigHandler.AddCustomServerUrl(ref config, vmessItem) == 0)
            {
                //刷新
                UI.Show(UIRes.I18N("SuccessfullyImportedCustomServer"));
                Close();
            }
            else
            {
                UI.ShowWarning(UIRes.I18N("FailedImportedCustomServer"));
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}