using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fluxzy.Core.Breakpoints;
using Fluxzy.Core;
using Fluxzy.Rules;
using Fluxzy.Clients.Headers;
using Microsoft.VisualBasic.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Windows.Forms;
using log4net;
using System.Reflection;

namespace WinFormsApp1
{
    internal class RequestAction : Fluxzy.Rules.Action
    {
        Dictionary<string, JObject> m_envs;
        List<MessageObject> m_MessageObjects;
        string m_remark;
        Form1 m_form1;
        public RequestAction(List<MessageObject> messageObjects, string remark, Dictionary<string, JObject> envs, Form1 form)
        { 
            m_MessageObjects = messageObjects;
            m_envs = envs;
            m_remark = remark;
            m_form1 = form;
        }
        public override FilterScope ActionScope => FilterScope.RequestHeaderReceivedFromClient;

        public override string DefaultDescription => nameof(RequestAction);

        public override ValueTask InternalAlter(ExchangeContext context, Exchange? exchange, Connection? connection, FilterScope scope, BreakPointManager breakPointManager)
        {
            foreach (MessageObject obj in m_MessageObjects)
            {
                if (obj.host == exchange.Authority.HostName)
                {
                    context.RegisterRequestBodySubstitution(new RequestBodySubstitution(exchange!, obj, m_remark, m_envs, m_form1));
                }
            }

            return default;
        }
    }

    internal class RequestBodySubstitution : IStreamSubstitution
    {
        private readonly Exchange _exchange;
        private readonly MessageObject messageObject;
        private readonly string remark;
        private static readonly object _lock = new object();
        Dictionary<string, JObject> _envs;
        private Form1 form;
        public RequestBodySubstitution(Exchange exchange, MessageObject messageObject, string remark, Dictionary<string, JObject> envs, Form1 form)
        {
            _exchange = exchange;
            this.messageObject = messageObject;
            this.remark = remark;
            _envs = envs;
            this.form = form;
        }

        public async ValueTask<Stream> Substitute(Stream originalStream)
        {
            var memoryStream = new MemoryStream();
            await originalStream.CopyToAsync(memoryStream);
            MessageObject item = new MessageObject(form, remark);
            memoryStream.Seek(0, SeekOrigin.Begin);

            item.copy(messageObject);
            try
            {
                item.analyze(_exchange);
                Program.log.Info($"抓到请求报文 host: {messageObject.host}");
                if (!item.verify())
                {
                    Program.log.Info("报文验证不通过");
                    return memoryStream;
                }
            }
            catch (Exception e)
            {
                Program.log.Error("报文解析异常", e);
                return memoryStream;
            }
            
            // if (form != null) { form.addLog($"抓到请求报文 host: {oS.host}"); }
            // 保存文件并根据内容判断是否上传
            if (item.getEnv().Length > 0)
            {
                var jsonText = new
                {
                    name = item.envName,
                    value = item.getEnv(),
                    remark = remark,
                    run = item.iRun == 1 ? true : false,
                    taskName = item.taskName,
                };

                string json = JsonConvert.SerializeObject(jsonText);

                // 将 JSON 字符串写入文件
                bool bUpdateQL = false;
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"envs_{remark}", $"{item.taskName}.json");
                // log.Debug($"写入文件路径: {filePath}");
                lock (_lock)
                {
                    if (_envs.ContainsKey(item.taskName))
                    {
                        try
                        {
                            if ((string)_envs[item.taskName]["value"] != item.getEnv())
                            {
                                _envs[item.taskName]["value"] = item.getEnv();
                                bUpdateQL = true;
                                File.WriteAllText(filePath, json);
                            }
                        }
                        catch (Exception e)
                        {
                            bUpdateQL = false;
                            Program.log.Error("文件读取异常", e);
                        }
                    }
                    else
                    {
                        bUpdateQL = true;
                        _envs.Add(item.taskName, JObject.Parse(json));
                        File.WriteAllText(filePath, json);
                    }
                }
                if (bUpdateQL)
                {
                    qinglong.GetInstance().addInfo(item.envName, json);
                }
            }

            return memoryStream;
        }
    }
}
