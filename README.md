# StarBot 说明文档

## 一、食用教程
1. 下载

   方法1：前往夸克网盘下载，[下载地址](https://gitee.com/link?target=https%3A%2F%2Fpan.quark.cn%2Fs%2Fdd90d150e6d7)。
   
   方法2：自行编译，主项目StarBot(窗体)和StarBot.API(api服务)，编译运行StarBot可以得到windows桌面应用，而编译运行StarBot.API则是通过浏览器使用此项目，二选一。
2. 安装和运行

   目前没有安装包均为绿色版，下载的解压后找到StarBot.exe可执行文件运行即可。
## 二、配置教程
1. 机器人配置

   方式1：[LLOneBot](https://llonebot.github.io/zh-CN)基于QQNT架构的QQ**客户端**，可视化配置，配置简单（比mirai简单的不是亿点点），内存占用较高300MB起步。
   
   方式2：[NapCat](https://napneko.github.io/zh-CN)基于QQNT架构的QQ**运行环境**，**无需客户端**，非可视化配置，配置相比LLOneBot复杂一点，内存占用较低正常情况不高于100MB。
   
2. 其他配置项

   **参考[ParkerBot](https://gitee.com/jaffoo/ParkerBot#%E9%85%8D%E7%BD%AE%E6%95%99%E7%A8%8B)，大同小异，不在赘述**

## 三、插件使用和开发

   1. 插件使用

      找到基于本软件开发的插件，当配置菜单中起用了机器人项时会出现插件菜单，导入使用即可。
      
   2. 插件开发

      找到本项目的PluginServer，新建插件类库项目，或者打包编译后引入编译好的dll文件或者导入nuget包文件，引用并且继承BasePlugin。

      BasePlugin基类是抽象类，所以必须实现的属性有
       - Name：插件名称(请使用英文)
       - Desc：插件描述
       - Version：版本。

      其他属性说明
       - Admin：qq群超管
       - Permission：qq群普通管理员
       - ConfPath：配置文件路径。注：如果插件需要配置文件，那么此属性必须重写，以为此属性只有文件夹路径，具体配置文件名称和格式自行定义，重写的get方法返回时必须为base.ConfPath+"config.json"，例如：public override string ConfPath {get{return base.ConfPath+"config.json"}}。
       - LogPath：日志文件路径。和ConfPath一样的使用方式。

      插件方法说明
       - SetTimer：定时任务，传入一个方法，和Schedule方法，使用示例：base.SetTimer(() => Console.Write(1), x => x.ToRunNow().AndEvery(1).Minutes())//立即执行，并且后面每分钟执行一次;
       - Excute：执行插件中的方法（发送QQ消息的方法），可以不重写。
       - GroupMessage：发送群消息，如需发送群消息，请重写。
       - FriendMessage：好友消息，如需发送好友消息，请重写。
       - EventMessage：事件消息，如需监听收到的QQ消息事件，请重写。

      **不持支前端界面插件，仅支持后端功能插件**。

      
