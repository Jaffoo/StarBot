using System.Reactive.Linq;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Data;
using StarBot.IService;
using Microsoft.Extensions.DependencyInjection;
using TBC.CommonLib;
using StarBot.Entity;
using FluentScheduler;
using UnifyBot.Message.Chain;
using UnifyBot;
using UnifyBot.Receiver.EventReceiver.Request;
using UnifyBot.Receiver.MessageReceiver;
using UnifyBot.Receiver.EventReceiver;
using UnifyBot.Model;
using UnifyBot.Message;

namespace StarBot.Extension
{
    public class MsgModel
    {
        /// <summary>
        /// 1-群，2-好友
        /// </summary>
        public int Type { get; set; } = 1;
        public string Id { get; set; } = "";
        public List<string>? Ids { get; set; }
        public string? MsgStr { get; set; }
        public MessageChain? MsgChain { get; set; }
        public string? Url { get; set; }
    }

    public class ReciverMsg
    {
        private static ReciverMsg? instance;
        private static readonly object lockObject = new object();
        ISysConfig _sysConfig;
        ISysLog _sysLog;
        ISysCache _cache;
        IQQMessage _qqMessage;

        private ReciverMsg()
        {
            var factory = DataService.BuildServiceProvider();
            _sysConfig = factory.GetService<ISysConfig>()!;
            _sysLog = factory.GetService<ISysLog>()!;
            _cache = factory.GetService<ISysCache>()!;
            _qqMessage = factory.GetService<IQQMessage>()!;
        }

        public static ReciverMsg Instance
        {
            get
            {
                lock (lockObject)
                {
                    instance ??= new ReciverMsg();
                    return instance;
                }
            }
        }
        private Model.Config Config
        {
            get
            {
                return _sysConfig.GetConfig().Result;
            }
        }
        public Queue<MsgModel> MsgQueue = new();
        private DateTime _lastSendTime = DateTime.Now;
        private readonly double _interval = 3;//单位秒
        public Bot? Bot;
        #region 全局变量
        public string Admin => Config.QQ.Admin;
        public bool Notice => Config.QQ.Notice;
        public List<string> Check
        {
            get
            {
                return _cache.GetListAsync(t => t.Type == 1).Result.Select(t => t.Content).ToList();
            }
        }
        public List<string> Permission => Config.QQ.Permission.ToListStr();
        public List<string> FuncAdmin => Config.QQ.FuncAdmin;
        public List<string> Group => Config.QQ.Group.ToListStr();
        public List<RequestFriend> RequestFriend { get; set; } = [];
        public List<RequestGroup> RequestGroup { get; set; } = [];
        public static bool BotReady { get; set; } = false;
        #endregion

        public void BotStart(Bot bot, bool botReady)
        {
            Bot = bot;
            Bot!.StartAsync();
            GroupMessageReceiver();
            FriendMessageReceiver();
            EventMessageReceiver();
            //执行插件
            Task.Run(() =>
            {
                Bot.MessageReceived.OfType<MessageReceiver>().Subscribe(mrb =>
                {
                    PluginHelper.Excute(mrb: mrb);
                });
                Bot.EventReceived.OfType<EventReceiver>().Subscribe(eb =>
                {
                    PluginHelper.Excute(eb: eb);
                });
            });
            Task.Run(HandlMsg);
            BotReady = botReady;
        }

        public void GroupMessageReceiver()
        {
            Bot!.MessageReceived.OfType<GroupReceiver>().Subscribe(async gmr =>
            {
                try
                {
                    if (!Group.Contains(gmr.GroupQQ.ToString())) return;
                    //消息链
                    var msgChain = gmr.Message!;
                    var msgText = msgChain.GetPlainText().Trim();
                    if (Config.QQ.Save && !string.IsNullOrWhiteSpace(Config.QQ.Group))
                    {
                        var msgModel = new QQMessage()
                        {
                            SenderId = gmr.Sender!.QQ.ToString(),
                            SenderName = gmr.Sender.Nickname,
                            ReciverId = gmr.Group!.GroupQQ.ToString(),
                            ReciverName = gmr.Group.GroupName,
                            Time = DateTime.Now,
                        };
                        foreach (var item in msgChain)
                        {
                            if (item.Type == Messages.Text)
                                msgModel.Content = (item as TextMessage)?.Data.Text ?? "";
                            if (item.Type == Messages.Image)
                                msgModel.Url = (item as ImageMessage)?.Data.Url;
                            if (item.Type == Messages.Record)
                                msgModel.Url = (item as RecordMessage)?.Data.Url;
                            if (item.Type == Messages.Video)
                                msgModel.Url = (item as VideoMessage)?.Data.File;
                        }
                        await _qqMessage.AddAsync(msgModel);
                    }
                    return;
                }
                catch (Exception e)
                {
                    await _sysLog.WriteLog(e.Message);
                    return;
                }
            });
        }

        public void FriendMessageReceiver()
        {
            Bot!.MessageReceived.OfType<PrivateReceiver>().Subscribe(async fmr =>
            {
                //消息链
                var msgChain = fmr.Message;
                var msgText = msgChain?.GetPlainText().Replace(" #", "") ?? "";
                try
                {
                    if (Admin == fmr.Sender!.QQ.ToString())
                    {
                        if (msgText == "#菜单")
                        {
                            string menu = "1、审核功能：\n#查看全部\n#查看#{第几张}\n#保存#{第几张}\n#保存全部\n#删除#{第几张}\n#删除全部\n2、好友申请：\n#同意/拒绝#{事件标识}" +
                            "\n3、微博：\n#立即同步微博\n#同步微博#{链接地址}" +
                            "\n4、发送消息：\n#发送#{群/好友}#文字/图片/语音/视频/图文/{qq号/群号}/{文字/图片链接/{文字}-{图片链接}}" +
                            "\n#微博用户搜索#{关键词}\n#关注微博用户#{用户id}\n#添加/删除微博关键词#{词}\n#重置微博关键词" +
                            "\n5、功能开关：\n#开启/关闭模块{模块名称}\n#开启/关闭转发#{模块}#qq/群\n#修改转发#{模块}#qq/群#{值}\n6、管理员：\n#添加/删除管理员#{qq}\n#删除全部管理员" +
                            "\n6、系统功能：\n#查询#{sql}\n#SQL#{sql}\n#清空聊天记录\n#修改#key/id#{key/id}#{value}";
                            await fmr.SendMessage(menu);
                            return;
                        }
                        if (msgText == "#查看全部")
                        {
                            if (Check.Count == 0)
                            {
                                await fmr.SendMessage("无待审核图片");
                                return;
                            }
                            foreach (var item in Check)
                            {
                                var newMsgChain = new MessageChainBuild().ImageByUrl(item).Build();
                                await fmr.SendMessage(newMsgChain);
                                Thread.Sleep(1000);
                            }
                            return;
                        }
                        if (msgText.Contains("#查看#"))
                        {
                            if (Check.Count == 0)
                            {
                                await fmr.SendMessage("无待审核图片");
                                return;
                            }
                            var index = msgText.Replace("#查看#", "").ToInt() - 1;
                            if (index >= Check.Count || index < 0)
                            {
                                await fmr.SendMessage($"未找到图片");
                                return;
                            }
                            var newMsgChain = new MessageChainBuild().ImageByUrl(Check[index]).Build();
                            await fmr.SendMessage(newMsgChain);
                            return;
                        }
                        if (msgText.Contains("#保存#"))
                        {
                            if (Check.Count == 0)
                            {
                                await fmr.SendMessage("无待审核图片");
                                return;
                            }
                            var index = msgText.Replace("#保存#", "").ToInt() - 1;
                            if (index > Check.Count || index < 0)
                            {
                                await fmr.SendMessage($"未找到张图片");
                                return;
                            }
                            if (await new FileHelper().Save(Check[index]))
                            {
                                var model = await _cache.GetModelAsync(t => t.Content == Check[index] && t.Type == 1);
                                if (model != null)
                                    await _cache.DeleteAsync(model);
                            }
                            await fmr.SendMessage("本地保存成功！");
                            return;
                        }
                        if (msgText == "#保存全部")
                        {
                            foreach (var item in Check)
                            {
                                if (await new FileHelper().Save(item))
                                {
                                    var list = await _cache.GetListAsync(t => t.Type == 1);
                                    if (list != null)
                                        await _cache.DeleteAsync(list);
                                }
                            }
                            await fmr.SendMessage("全部本地保存成功！");
                            return;
                        }
                        if (msgText.Contains("#删除#"))
                        {
                            if (Check.Count == 0)
                            {
                                await fmr.SendMessage("无待审核图片");
                                return;
                            }
                            var index = msgText.Replace("#删除#", "").ToInt() - 1;
                            if (index > Check.Count || index < 0)
                            {
                                await fmr.SendMessage($"未找到张图片");
                                return;
                            }
                            var model = await _cache.GetModelAsync(t => t.Content == Check[index] && t.Type == 1)!;
                            if (model != null) await _cache.DeleteAsync(model);
                            await fmr.SendMessage("删除成功！");
                            return;
                        }
                        if (msgText == "#删除全部")
                        {
                            var list = await _cache.GetListAsync(t => t.Type == 1);
                            await _cache.DeleteAsync(list);
                            await fmr.SendMessage("全部删除成功！");
                            return;
                        }

                        if (msgText.Contains("#同意#"))
                        {
                            var flagId = msgText.Replace("#同意#", "");
                            var friendEvent = RequestFriend.FirstOrDefault(t => t.Flag == flagId);
                            if (friendEvent == null)
                            {
                                await fmr.SendMessage("未找到该好友申请！");
                                return;
                            }
                            await friendEvent.Agree();
                            await fmr.SendMessage("已同意好友申请！");
                            return;
                        }
                        if (msgText.Contains("#拒绝#"))
                        {
                            var flagId = msgText.Replace("#拒绝#", "");
                            var friendEvent = RequestFriend.FirstOrDefault(t => t.Flag == flagId);
                            if (friendEvent == null)
                            {
                                await fmr.SendMessage("未找到该好友申请！");
                                return;
                            }
                            await friendEvent.Reject();
                            await fmr.SendMessage("已拒绝好友申请！");
                            return;
                        }

                        if (msgText.Contains("#同意入群#"))
                        {
                            var flagId = msgText.Replace("#同意入群#", "");
                            var friendEvent = RequestFriend.FirstOrDefault(t => t.Flag == flagId);
                            if (friendEvent == null)
                            {
                                await fmr.SendMessage("未找到入群申请！");
                                return;
                            }
                            await friendEvent.Agree();
                            await fmr.SendMessage("已同意入群申请！");
                            return;
                        }
                        if (msgText.Contains("#拒绝入群#"))
                        {
                            var flagId = msgText.Replace("#拒绝入群#", "");
                            var groupEvent = RequestGroup.FirstOrDefault(t => t.Flag == flagId);
                            if (groupEvent == null)
                            {
                                await fmr.SendMessage("未找到入群申请！");
                                return;
                            }
                            await groupEvent.Reject();
                            await fmr.SendMessage("已拒绝入群申请！");
                            return;
                        }

                        if (msgText == "#立即同步微博")
                        {
                            await new Weibo().Save();
                            return;
                        }
                        if (msgText.Contains("#同步微博#"))
                        {
                            var url = msgText.Replace("#同步微博#", "");
                            await new Weibo().SaveByUrl(url);
                            return;
                        }
                        if (msgText == "#最新日志")
                        {
                            var log = await _sysLog.GetModelAsync(t => t.Time);
                            if (log == null) return;
                            if (log.Content.Length > 500) log.Content = log.Content.Substring(0, 100);
                            MessageChain mc = new()
                            {
                                new TextMessage("时间：" + log?.Time.ToString("yyyy-MM-dd HH:mm:ss") ?? ""),
                                new TextMessage("\n详细：" + log?.Content ?? "未查询到日志！")
                            };
                            await fmr.SendMessage(mc);
                            return;
                        }
                        if (msgText.Contains("#添加管理员#"))
                        {
                            var qqNum = msgText.Replace("#添加管理员#", "");
                            if (string.IsNullOrWhiteSpace(qqNum.Trim()))
                            {
                                await fmr.SendMessage("格式错误!");
                                return;
                            }
                            if (Permission.Contains(qqNum))
                                await fmr.SendMessage("此账号已是管理员！");
                            var model = await _sysConfig.GetModelAsync(t => t.Key == "Permission") ?? throw new Exception("key为【Permission】的值不存在！");
                            model.Value += "," + qqNum;
                            await _sysConfig.UpdateAsync(model);
                            _sysConfig.ClearConfig("QQ");
                            await fmr.SendMessage("添加成功！");
                            return;
                        }
                        if (msgText == "#删除全部管理员")
                        {
                            var model = await _sysConfig.GetModelAsync(t => t.Key == "Permission") ?? throw new Exception("key为【Permission】的值不存在！");
                            model.Value = "";
                            await _sysConfig.UpdateAsync(model);
                            _sysConfig.ClearConfig("QQ");
                            await fmr.SendMessage("删除成功！");
                            return;
                        }
                        if (msgText.Contains("#删除管理员#"))
                        {
                            var qqNum = msgText.Replace("#删除管理员#", "");
                            if (string.IsNullOrWhiteSpace(qqNum.Trim()))
                            {
                                await fmr.SendMessage("格式错误!");
                                return;
                            }
                            if (!Permission.Contains(qqNum))
                            {
                                await fmr.SendMessage("此账号不是管理员！");
                                return;
                            }
                            Permission.Remove(qqNum);
                            var model = await _sysConfig.GetModelAsync(t => t.Key == "Permission") ?? throw new Exception("key为【Permission】的值不存在！");
                            model.Value = string.Join(",", Permission);
                            await _sysConfig.UpdateAsync(model);
                            _sysConfig.ClearConfig("QQ");
                            await fmr.SendMessage("删除成功！");
                            return;
                        }

                        if (msgText.Contains("#清空聊天记录"))
                        {
                            var all = await _qqMessage.GetListAsync();
                            var res = await _qqMessage.DeleteAsync(all);
                            await fmr.SendMessage("清空成功");
                            return;
                        }
                        if (msgText.Contains("#查询#key#"))
                        {
                            var words = msgText.Replace("#查询#key#", "");
                            if (string.IsNullOrWhiteSpace(words))
                            {
                                await fmr.SendMessage("格式错误！");
                                return;
                            }
                            var list = words.Split('#');
                            if (list.Length != 2)
                            {
                                await fmr.SendMessage("格式错误！");
                                return;
                            }
                            var key = list[0];
                            var value = list[1];
                            var models = await _sysConfig.GetListAsync(t => t.Key == key);
                            if (models == null)
                            {
                                await fmr.SendMessage("key不存在！");
                                return;
                            }
                            foreach (var item in models)
                            {
                                await fmr.SendMessage($"查询到关键词:{item.Key}的值为:{item.Value},id为:{item.Id}");
                            }
                            return;
                        }
                        if (msgText.Contains("#修改#key#"))
                        {
                            var words = msgText.Replace("#修改#key#", "");
                            if (string.IsNullOrWhiteSpace(words))
                            {
                                await fmr.SendMessage("格式错误！");
                                return;
                            }
                            var list = words.Split('#');
                            if (list.Length != 2)
                            {
                                await fmr.SendMessage("格式错误！");
                                return;
                            }
                            var key = list[0];
                            var value = list[1];
                            var model = await _sysConfig.GetModelAsync(t => t.Key == key);
                            if (model == null)
                            {
                                await fmr.SendMessage("key不存在！");
                                return;
                            }
                            var old = model.Value;
                            model.Value = value;
                            await _sysConfig.UpdateAsync(model);
                            _sysConfig.ClearConfig();
                            await fmr.SendMessage($"【{key}】值已修改。{old} -> {value}");
                            return;
                        }
                        if (msgText.Contains("#修改#id#"))
                        {
                            var words = msgText.Replace("#修改#id#", "");
                            if (string.IsNullOrWhiteSpace(words))
                            {
                                await fmr.SendMessage("格式错误！");
                                return;
                            }
                            var list = words.Split('#');
                            if (list.Length != 2)
                            {
                                await fmr.SendMessage("格式错误！");
                                return;
                            }
                            var id = list[0];
                            var value = list[1];
                            var model = await _sysConfig.GetModelAsync(t => t.Id == id.ToInt());
                            if (model == null)
                            {
                                await fmr.SendMessage("数据不存在！");
                                return;
                            }
                            var old = model.Value;
                            model.Value = value;
                            await _sysConfig.UpdateAsync(model);
                            _sysConfig.ClearConfig();
                            await fmr.SendMessage($"id:{id}【{model.Key}】值已修改。{old} -> {value}");
                            return;
                        }
                    }
                    if (Permission.Contains(fmr.Sender.QQ.ToString()) || fmr.Sender.QQ.ToString() == Admin)
                    {
                        if (msgText == "#菜单" && Permission.Contains(fmr.Sender.QQ.ToString()))
                        {
                            string menu = "1、功能开关：\n#开启/关闭模块{模块名称}\n#开启/关闭转发#{模块}#qq/群\n#修改转发#{模块}#qq/群#{值}\n2、发送消息：#发送#{群/好友}#文字/图片/语音/视频/图文/{qq号/群号}/{文字/图片链接/{文字}-{图片链接}}" +
                            "\n3、微博：\n#微博用户搜索#{关键词}\n#关注微博用户#{用户id}\n#添加/删除微博关键词#{词}\n#重置微博关键词";
                            await fmr.SendMessage(menu);
                            return;
                        }
                        if (msgText.Contains("#发送#"))
                        {
                            var msg = msgText.Replace("#发送#", "");
                            var list = msg.Split('#').ToList();
                            if (list.Count != 4)
                            {
                                await fmr.SendMessage("格式错误！");
                                return;
                            }
                            var msgBuild = new MessageChainBuild();
                            if (list[1] == "文字")
                                msgBuild.Text(list[3]);
                            if (list[1] == "图片")
                                msgBuild.ImageByUrl(list[3]);
                            if (list[1] == "语音")
                                msgBuild.RecordByUrl(list[3]);
                            if (list[1] == "视频")
                                msgBuild.Text(list[3]);
                            if (list[1] == "图文")
                                msgBuild.Text(list[3].Split('-')[0]).ImageByUrl(list[3].Split('-')[1]);
                            if (list[0] == "好友")
                                await SendFriendMsg(list[2], msgBuild.Build());
                            if (list[0] == "群")
                                await SendGroupMsg(list[2], msgBuild.Build());
                            return;
                        }
                        if (msgText.Contains("#开启模块#"))
                        {
                            var moudel = msgText.Replace("#开启模块#", "");
                            var old = moudel;
                            moudel = GetMoudel(moudel);
                            Config.EnableModule[moudel] = true;
                            _sysConfig.ClearConfig(moudel);
                            await fmr.SendMessage($"模块【{old}】已开启！");
                            var job = JobManager.AllSchedules.FirstOrDefault(t => t.Name.ToLower() == moudel.ToLower());
                            if (job != null) job.Enable();
                            var model = await _sysConfig.GetModelAsync(t => t.Pid == 6 && t.Key == moudel);
                            if (model != null)
                            {
                                model.Value = "true";
                                await _sysConfig.UpdateAsync(model);
                            }
                            return;
                        }
                        if (msgText.Contains("#关闭模块#"))
                        {
                            var moudel = msgText.Replace("#关闭模块#", "");
                            var old = moudel;
                            moudel = GetMoudel(moudel);
                            Config.EnableModule[moudel] = false;
                            _sysConfig.ClearConfig(moudel);
                            await fmr.SendMessage($"模块【{old}】已关闭！");
                            var job = JobManager.AllSchedules.FirstOrDefault(t => t.Name.ToLower() == moudel.ToLower());
                            if (job != null) job.Disable();
                            var model = await _sysConfig.GetModelAsync(t => t.Pid == 6 && t.Key == moudel);
                            if (model != null)
                            {
                                model.Value = "false";
                                await _sysConfig.UpdateAsync(model);
                            }
                            return;
                        }
                        if (msgText.Contains("#开启转发#"))
                        {
                            var list = msgText.Replace("#开启转发#", "").Split('#').ToList();
                            if (list.Count != 2)
                            {
                                await fmr.SendMessage("格式错误！");
                                return;
                            }
                            var moudel = list[0];
                            var type = list[1];
                            moudel = GetMoudel(moudel);
                            string forward = type == "qq" ? "ForwardQQ" : "ForwardGroup";
                            if (string.IsNullOrWhiteSpace(type))
                                type = "群";
                            if (moudel == "WBChiGua")
                            {
                                moudel = "WB";
                                forward = type == "qq" ? "WBChiGuaForwardQQ" : "WBChiGuaForwardGroup";
                            }
                            Config[moudel, forward] = true;
                            await fmr.SendMessage($"模块【{list[0]}】转发至{type}功能已开启！");
                            var pmodel = await _sysConfig.GetModelAsync(t => t.Key == moudel && t.Pid == 6);
                            if (pmodel == null) return;
                            var model = await _sysConfig.GetModelAsync(t => t.Key == forward && t.Pid == pmodel.Id);
                            if (model == null) return;
                            model.Value = "true";
                            await _sysConfig.UpdateAsync(model);
                            _sysConfig.ClearConfig(moudel);
                            return;
                        }
                        if (msgText.Contains("#关闭转发#"))
                        {
                            var list = msgText.Replace("#关闭转发#", "").Split('#').ToList();
                            if (list.Count != 2)
                            {
                                await fmr.SendMessage("格式错误！");
                                return;
                            }
                            var moudel = list[0];
                            var type = list[1];
                            moudel = GetMoudel(moudel);
                            string forward = type == "qq" ? "ForwardQQ" : "ForwardGroup";
                            if (string.IsNullOrWhiteSpace(type))
                                type = "群";
                            if (moudel == "WBChiGua")
                            {
                                moudel = "WB";
                                forward = type == "qq" ? "WBChiGuaForwardQQ" : "WBChiGuaForwardGroup";
                            }
                            Config[moudel, forward] = false;
                            await fmr.SendMessage($"模块【{list[0]}】转发至{type}功能已关闭！");
                            var pmodel = await _sysConfig.GetModelAsync(t => t.Key == moudel && t.Pid == 6);
                            if (pmodel == null) return;
                            var model = await _sysConfig.GetModelAsync(t => t.Key == forward && t.Pid == pmodel.Id);
                            if (model == null) return;
                            model.Value = "false";
                            await _sysConfig.UpdateAsync(model);
                            _sysConfig.ClearConfig(moudel);
                            return;
                        }
                        if (msgText.Contains("#修改转发#"))
                        {
                            var list = msgText.Replace("#修改转发#", "").Split('#').ToList();
                            if (list.Count != 3)
                            {
                                await fmr.SendMessage("格式错误！");
                                return;
                            }
                            var moudel = list[0];
                            var type = list[1];
                            var value = list[2];
                            moudel = GetMoudel(moudel);
                            string forward = type == "qq" ? "QQ" : "Group";
                            if (string.IsNullOrWhiteSpace(type))
                                type = "群";
                            if (moudel == "WBChiGua")
                            {
                                moudel = "WB";
                                forward = type == "qq" ? "WBChiGuaQQ" : "WBChiGuaGroup";
                            }
                            Config[moudel, forward] = false;
                            await fmr.SendMessage($"模块【{list[0]}】转发至{type}功能的值已修改为：{value}");
                            var pmodel = await _sysConfig.GetModelAsync(t => t.Key == moudel && t.Pid == 6);
                            if (pmodel == null) return;
                            var model = await _sysConfig.GetModelAsync(t => t.Key == forward && t.Pid == pmodel.Id);
                            if (model == null) return;
                            model.Value = value;
                            await _sysConfig.UpdateAsync(model);
                            _sysConfig.ClearConfig(moudel);
                            return;
                        }
                        if (msgText.Contains("#微博用户搜索#"))
                        {
                            var keyword = msgText.Replace("#微博用户搜索#", "");
                            var url = $"https://m.weibo.cn/api/container/getIndex?containerid=100103type%3D3%26q%3D{keyword}%26t%3D%26page_type%3Dsearchall";
                            var handler = new HttpClientHandler() { UseCookies = true };
                            HttpClient httpClient = new(handler);
                            httpClient.DefaultRequestHeaders.Add("user-agent", @"Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Mobile Safari/537.36 Edg/114.0.1823");
                            httpClient.DefaultRequestHeaders.Add("sec-ch-ua", "\"Not.A/Brand\"; v = \"8\", \"Chromium\"; v = \"114\", \"Microsoft Edge\"; v = \"114\"");
                            httpClient.DefaultRequestHeaders.Add("sec-ch-ua-platform", "Windows");
                            httpClient.DefaultRequestHeaders.Add("cookie", "_T_WM=98249609066; WEIBOCN_FROM=1110005030; MLOGIN=0; XSRF-TOKEN=4c5095; mweibo_short_token=1937e050d5; M_WEIBOCN_PARAMS=luicode%3D10000011%26lfid%3D100103type%253D3%2526q%253D%25E9%25BB%2584%25E6%2580%25A1%25E6%2585%2588%2526t%253D%26fid%3D100103type%253D3%2526q%253D%25E9%25BB%2584%25E6%2580%25A1%25E6%2585%2588%2526t%253D%26uicode%3D10000011");
                            var res = await httpClient.GetAsync(url);
                            var content = await res.Content.ReadAsStringAsync();
                            var result = JObject.Parse(content);
                            var resultList = JArray.FromObject(result["data"]!["cards"]!);
                            var model = resultList.FirstOrDefault(t => t["card_type"]?.ToString() == "11");
                            if (model == null) return;
                            var list = JArray.FromObject(model["card_group"]!);
                            StringBuilder msg = new("以为你搜索到以下结果：\n");
                            for (int i = 0; i < list.Count; i++)
                            {
                                var user = list[i];
                                msg.Append($"{i + 1}：{user!["user"]!["screen_name"]}({user!["user"]!["id"]}),{user!["desc1"]},{user!["desc2"]}\n");
                                if (i == 2) break;
                            }
                            msg.Append("注：结果有多个时，仅展示前三个！");
                            await fmr.SendMessage(msg.ToString());
                            return;
                        }
                        if (msgText.Contains("#删除微博关键词#"))
                        {
                            var keywords = msgText.Replace("#删除微博关键词#", "");
                            if (string.IsNullOrWhiteSpace(keywords))
                                await fmr.SendMessage("输入内容为空！");
                            if (!Config.WB.Keyword.ToListStr().Contains(keywords))
                                await fmr.SendMessage("不存在该关键词！");
                            var temp = Config.WB.Keyword.ToListStr();
                            temp.Remove(keywords);
                            Config.WB.Keyword = string.Join(",", temp);
                            var model = await _sysConfig.GetModelAsync(t => t.Key == "Keyword" && t.DataType == "list");
                            if (model == null) return;
                            model.Value = string.Join(",", Config.WB.Keyword);
                            await _sysConfig.UpdateAsync(model);
                            _sysConfig.ClearConfig("WB");
                            await fmr.SendMessage($"已删除关键词【{keywords}】");
                            return;
                        }
                        if (msgText.Contains("#添加微博关键词#"))
                        {
                            var keywords = msgText.Replace("#添加微博关键词#", "");
                            if (string.IsNullOrWhiteSpace(keywords))
                                await fmr.SendMessage("输入内容为空！");
                            if (Config.WB.Keyword.ToListStr().Contains(keywords))
                                await fmr.SendMessage("已存在该关键词！");
                            var temp = Config.WB.Keyword.ToListStr();
                            temp.Add(keywords);
                            Config.WB.Keyword = string.Join(",", temp);
                            var model = await _sysConfig.GetModelAsync(t => t.Key == "Keyword" && t.DataType == "list");
                            model!.Value = string.Join(",", Config.WB.Keyword);
                            await _sysConfig.UpdateAsync(model);
                            _sysConfig.ClearConfig("WB");
                            await fmr.SendMessage($"已添加关键词【{keywords}】");
                            return;
                        }
                        if (msgText.Contains("#重置微博关键词"))
                        {
                            var model = await _sysConfig.GetModelAsync(t => t.Key == "Keyword" && t.DataType == "list");
                            model!.Value = "";
                            await _sysConfig.UpdateAsync(model);
                            _sysConfig.ClearConfig("WB");
                            await fmr.SendMessage($"已重置关键词");
                            return;
                        }
                    }
                }
                catch (Exception e)
                {
                    await _sysLog.WriteLog(e.Message);
                    return;
                }
            });
        }

        public void EventMessageReceiver()
        {
            Bot!.EventReceived.OfType<EventReceiver>().Subscribe(async e =>
            {
                try
                {
                    if (e.PostType == PostType.Notice)
                    {

                    }
                    if (e.PostType == PostType.Request)
                    {
                        if (e.RequestEventType == RequestType.Friend)
                        {
                            if (e is not RequestFriend ev) throw new Exception("好友申请事件报错");
                            RequestFriend.Add(ev);
                            await SendAdminMsg($"机器人收到添加好友申请，Flag:{ev.Flag}");
                        }
                        if (e.RequestEventType == RequestType.Group)
                        {
                            if (e is not RequestGroup ev) throw new Exception("入群申请事件报错");
                            RequestGroup.Add(ev);
                            await SendAdminMsg($"群{ev.Group.GroupName}【{ev.GroupQQ}】收到新的入群申请，Flag:{ev.Flag}");
                        }
                    }
                    if (e.PostType == PostType.MetaEvent)
                    {

                    }
                }
                catch (Exception ex)
                {
                    await _sysLog.WriteLog(ex.Message);
                    return;
                }
            });
        }
        private bool IsAuth(string keyword, string qq)
        {
            if (FuncAdmin.Contains(keyword))
                if (Admin == qq || Permission.Contains(qq))
                    return true;
                else return false;
            else return true;
        }

        private string GetMoudel(string name)
        {
            var moudel = "";
            switch (name)
            {
                case "微博": moudel = "WB"; break;
                case "微博吃瓜": moudel = "WBChiGua"; break;
                case "口袋": moudel = "KD"; break;
                case "口袋48": moudel = "KD"; break;
                case "B站": moudel = "BZ"; break;
                case "抖音": moudel = "DY"; break;
                case "小红书": moudel = "XHS"; break;
                default:
                    break;
            }
            return moudel;
        }

        public async Task SendGroupMsg(string groupId, string msg)
        {
            try
            {
                if (!BotReady) return;
                if (!Config.EnableModule.Bot) return;
                if (Bot == null) return;
                var group = Bot.Groups?.FirstOrDefault(t => t.GroupQQ == groupId.ToLong());
                if (group == null) return;
                await group.SendMessage(msg);
                return;
            }
            catch (Exception ex)
            {
                await _sysLog.WriteLog(ex.Message);
            }
        }

        public async Task SendGroupMsg(List<string> groupIds, string msg)
        {
            try
            {
                if (!BotReady) return;
                if (!Config.EnableModule.Bot) return;
                if (Bot == null) return;
                foreach (var groupId in groupIds)
                {
                    var group = Bot.Groups?.FirstOrDefault(t => t.GroupQQ == groupId.ToLong());
                    if (group == null) continue;
                    await group.SendMessage(msg);
                }
                return;
            }
            catch (Exception ex)
            {
                await _sysLog.WriteLog(ex.Message);
            }
        }

        public async Task SendGroupMsg(string groupId, MessageChain msg)
        {
            try
            {
                if (!BotReady) return;
                if (!Config.EnableModule.Bot) return;
                if (Bot == null) return;
                var group = Bot.Groups?.FirstOrDefault(t => t.GroupQQ == groupId.ToLong());
                if (group == null) return;
                await group.SendMessage(msg);
                return;
            }
            catch (Exception ex)
            {
                await _sysLog.WriteLog(ex.Message);
                return;
            }
        }

        public async Task SendGroupMsg(List<string> groupIds, MessageChain msg)
        {
            try
            {
                if (!BotReady) return;
                if (!Config.EnableModule.Bot) return;
                if (Bot == null) return;
                foreach (var groupId in groupIds)
                {
                    var group = Bot.Groups?.FirstOrDefault(t => t.GroupQQ == groupId.ToLong());
                    if (group == null) continue;
                    await group.SendMessage(msg);
                }
                return;
            }
            catch (Exception ex)
            {
                await _sysLog.WriteLog(ex.Message);
                return;
            }
        }

        public async Task SendFriendMsg(string friendId, string msg)
        {
            try
            {
                if (!BotReady) return;
                if (!Config.EnableModule.Bot) return;
                if (Bot == null) return;
                var friend = Bot.Friends?.FirstOrDefault(t => t.QQ == friendId.ToLong());
                if (friend == null) return;
                await friend.SendMessage(msg);
                return;
            }
            catch (Exception ex)
            {
                await _sysLog.WriteLog(ex.Message);
                return;
            }
        }

        public async Task SendFriendMsg(List<string> friendIds, string msg)
        {
            try
            {
                if (!BotReady) return;
                if (!Config.EnableModule.Bot) return;
                if (Bot == null) return;
                foreach (var friendId in friendIds)
                {
                    var friend = Bot.Friends?.FirstOrDefault(t => t.QQ == friendId.ToLong());
                    if (friend == null) continue;
                    await friend.SendMessage(msg);
                }
                return;
            }
            catch (Exception ex)
            {
                await _sysLog.WriteLog(ex.Message);
                return;
            }
        }

        public async Task SendFriendMsg(string friendId, MessageChain msg)
        {
            try
            {
                if (!BotReady) return;
                if (!Config.EnableModule.Bot) return;
                if (Bot == null) return;
                var friend = Bot.Friends?.FirstOrDefault(t => t.QQ == friendId.ToLong());
                if (friend == null) return;
                await friend.SendMessage(msg);
                return;
            }
            catch (Exception ex)
            {
                await _sysLog.WriteLog(ex.Message);
                return;
            }
        }

        public async Task SendFriendMsg(List<string> friendIds, MessageChain msg)
        {
            try
            {
                if (!BotReady) return;
                if (!Config.EnableModule.Bot) return;
                if (Bot == null) return;
                foreach (var friendId in friendIds)
                {
                    var friend = Bot.Friends?.FirstOrDefault(t => t.QQ == friendId.ToLong());
                    if (friend == null) continue;
                    await friend.SendMessage(msg);
                }
                return;
            }
            catch (Exception ex)
            {
                await _sysLog.WriteLog(ex.Message);
                return;
            }
        }

        public async Task SendAdminMsg(string msg)
        {
            try
            {
                if (!BotReady) return;
                if (!Config.EnableModule.Bot) return;
                if (Bot == null || !Notice) return;
                var friend = Bot.Friends?.FirstOrDefault(t => t.QQ == Admin.ToLong());
                if (friend == null) return;
                await friend.SendMessage(msg);
                return;
            }
            catch (Exception ex)
            {
                await _sysLog.WriteLog(ex.Message);
                return;
            }
        }
        public async Task SendAdminMsg(MessageChain msg)
        {
            try
            {
                if (!BotReady) return;
                if (!Config.EnableModule.Bot) return;
                if (Bot == null || !Notice) return;
                var friend = Bot.Friends?.FirstOrDefault(t => t.QQ == Admin.ToLong());
                if (friend == null) return;
                await friend.SendMessage(msg);
                return;
            }
            catch (Exception ex)
            {
                await _sysLog.WriteLog(ex.Message);
                return;
            }
        }

        public async void HandlMsg()
        {
            while (true)
            {
                try
                {

                    double interval = (DateTime.Now - _lastSendTime).TotalSeconds;
                    if (MsgQueue.Count > 0 && interval >= _interval)
                    {
                        var msgModel = MsgQueue.Dequeue();
                        if (msgModel.Type == 1)
                        {
                            if (msgModel.Ids != null)
                            {
                                if (msgModel.MsgChain?.Count > 0)
                                {
                                    await SendGroupMsg(msgModel.Ids, msgModel.MsgChain);
                                }
                                else if (!string.IsNullOrWhiteSpace(msgModel.MsgStr)) await SendGroupMsg(msgModel.Ids, msgModel.MsgStr);
                            }
                            else if (!string.IsNullOrWhiteSpace(msgModel.Id))
                            {
                                if (msgModel.MsgChain?.Count > 0) await SendGroupMsg(msgModel.Id, msgModel.MsgChain);
                                else if (!string.IsNullOrWhiteSpace(msgModel.MsgStr)) await SendGroupMsg(msgModel.Id, msgModel.MsgStr);
                            }
                        }
                        if (msgModel.Type == 2)
                        {
                            if (msgModel.Ids != null)
                            {
                                if (msgModel.MsgChain?.Count > 0) await SendFriendMsg(msgModel.Ids, msgModel.MsgChain);
                                else if (!string.IsNullOrWhiteSpace(msgModel.MsgStr)) await SendFriendMsg(msgModel.Ids, msgModel.MsgStr);
                            }
                            else if (!string.IsNullOrWhiteSpace(msgModel.Id))
                            {
                                if (msgModel.MsgChain?.Count > 0) await SendFriendMsg(msgModel.Id, msgModel.MsgChain);
                                else if (!string.IsNullOrWhiteSpace(msgModel.MsgStr)) await SendFriendMsg(msgModel.Id, msgModel.MsgStr);
                            }
                        }
                        _lastSendTime = DateTime.Now;
                    }
                    Thread.Sleep(100);

                }
                catch (Exception e)
                {
                    await _sysLog.WriteLog(e.Message);
                    continue;
                }
            }
        }


        public static void AddMsg(MsgModel model)
        {
            Instance.MsgQueue.Enqueue(model);
        }
    }
}
