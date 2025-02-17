using Fluxzy.Core;
using Fluxzy.Rules.Filters.RequestFilters;
using Fluxzy.Rules.Filters;
using Fluxzy;
using System.Net;
using Fluxzy.Rules.Actions;
using Microsoft.VisualBasic.Logging;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using log4net;
using System.Reflection;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        bool m_isClose = false;
        string remark;
        static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        List<MessageObject> MessageObjects = new List<MessageObject>();
        Dictionary<string, JObject> envs = new Dictionary<string, JObject>();

        bool bStart = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!bStart)
            {
                Thread newThread = new Thread(new ThreadStart(DoWork));
                newThread.Start();
                addLog("�����ɹ�");
                button1.Text = "ֹͣ";
            }
            else
            {
                bStart = false;
                button1.Text = "����";
                addLog("ֹͣ�ɹ�");
            }
        }

        public void ReloadFiddler()
        {
            remark = comboBox1.Text;
            MessageObjects.Clear();
            envs.Clear();

            string fiddlerPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fiddler");
            Directory.CreateDirectory(fiddlerPath);
            // ѭ������ini�ļ�
            string[] iniFiles = Directory.GetFiles(fiddlerPath, "*.ini");

            // ����INI�ļ�·������ӡ
            foreach (string filePath in iniFiles)
            {
                MessageObject data = new MessageObject(this, remark);
                if (data.init(Path.GetFileNameWithoutExtension(filePath), filePath))
                {
                    MessageObjects.Add(data);
                }
            }
            // ѭ������json�ļ�
            string[] jsonFiles1 = Directory.GetFiles(fiddlerPath, "*.json");

            // ����INI�ļ�·������ӡ
            foreach (string filePath in jsonFiles1)
            {
                MessageObject data = new MessageObject(this, remark);
                if (data.initJson(Path.GetFileNameWithoutExtension(filePath), filePath))
                {
                    MessageObjects.Add(data);
                }
            }

            string dirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"envs_{remark}");
            Directory.CreateDirectory(dirPath);
            string[] jsonFiles = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"envs_{remark}"), "*.json");
            foreach (string filePath in jsonFiles)
            {
                string json = File.ReadAllText(filePath);
                JObject obj = JObject.Parse(json);
                envs.Add(Path.GetFileNameWithoutExtension(filePath), obj);
            }


            log.Info("������" + MessageObjects.Count + "������");
            addLog("������" + MessageObjects.Count + "������");

            qinglong.GetInstance().startListenThread();
            button1_Click(null, null);
        }

        private async void DoWork()
        {
            // ���ô��������
            var fluxzySetting = FluxzySetting.CreateDefault(IPAddress.Loopback, 18000);
            // ���ù��򣬲��񲢼�¼�����������Ӧ
            fluxzySetting.ConfigureRule().WhenAny().Do(new RequestAction(MessageObjects, remark, envs, this)); // ��¼�����������Ӧ
            // fluxzySetting.SetArchivingPolicy(ArchivingPolicy.CreateFromDirectory(@"D:\\fluxzy\\"));
            fluxzySetting.UseBouncyCastleSslEngine();
            fluxzySetting.SetAutoInstallCertificate(true);

            await using (var proxy = new Proxy(fluxzySetting))
            {
                var endPoints = proxy.Run();
                // ��ѡ��ע��Ϊϵͳ����
                await using var _ = await SystemProxyRegistrationHelper.Create(endPoints.First());
                bStart = true;
                while (bStart)
                {
                    Thread.Sleep(1000);
                }
            }
            Program.DisableSystemProxy();
            addLog("��������ѹر�");
        }
        private void button2_Click(object sender, EventArgs e)
        {
            bStart = false;
            Thread.Sleep(1000);
            init();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }

        public void addLog(string log)
        {
            if (this.richTextBox1.InvokeRequired)
            {
                this.richTextBox1.BeginInvoke(new Action<string>(addLog), new object[] { log });
            }
            else
            {
                richTextBox1.AppendText("\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss: ") + log);
                richTextBox1.ScrollToCaret();
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            init();
        }

        private async void init()
        {
            if (!await qinglong.GetInstance().Init(this))
            {
                addLog("��ʼ����������ʧ��, ���������ļ�");
                button1.Enabled = false;
                comboBox1.Enabled = false;
                return;
            }
            else
            {
                comboBox1.Items.AddRange(qinglong.GetInstance().remarkList.ToArray());
                comboBox1.SelectedIndex = 0;
                ReloadFiddler();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!m_isClose) {
                new Thread(() =>
                {
                    bStart = false;
                    Thread.Sleep(2 * 1000);
                    m_isClose = true;
                    this.Close();
                }).Start();
                e.Cancel = true;
            }
        }
    }
}
