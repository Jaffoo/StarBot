# StarBot 说明文档

## 因软件需求者不再使用，所以不在继续开发，现有功能不影响使用。

## 一、食用教程


**注：**

- 从2024年6月20日，已经更换基础框架，只为缩小软件体积，加快运行速度，由原来的600M缩减至100多M。

- 老版本想要更新的新版本，建议删除源文件中除了wwwroot和plugins（如果有）以外的所有文件，然后下载新压缩包解压覆盖上去。

- 新版本启动不了，请查看[常见问题](#四常见问题)


1. 下载

   方法1：前往夸克网盘下载，[下载地址](https://pan.quark.cn/s/dd90d150e6d7)。
   
   方法2：自行编译，主项目StarBot.Photino(窗体)、StarBot(窗体)和StarBot.API(api服务)，编译运行StarBot和StarBot.Photino可以得到桌面应用，而编译运行StarBot.API则是通过浏览器使用此项目，三选一，推荐StarBot.Photino，体积小，运行快。
1. 安装和运行

   目前没有安装包均为绿色版，下载的解压后找到StarBot.exe可执行文件运行即可。

2. 更新

   目前没有自动更新功能（后续考虑），如果你需要更新，下载最新的压缩包，解压后直接覆盖到原来的路径即可。
## 二、配置教程
1. 机器人配置

   方式1：[LLOneBot](https://llonebot.github.io/zh-CN)基于QQNT架构的QQ**客户端**。
   
   方式2（推荐）：[NapCat](https://napneko.github.io)基于QQNT架构的QQ**运行环境**，**可以无需客户端**，如果使用docker部署，这个比较适合。
   
2. 其他配置项

   **参考[ParkerBot配置](https://gitee.com/jaffoo/ParkerBot#%E9%85%8D%E7%BD%AE%E6%95%99%E7%A8%8B)，大同小异，不在赘述**

   其中口袋配置项略有不同，从老版本仅仅支持配置一位，现在支持配置多位，最多10位。

## 三、插件使用和开发

   1. 插件使用

      找到基于本软件开发的插件，当配置菜单中起用了机器人项时会出现插件菜单，导入使用即可。

      
   3. 插件开发

      找到本项目的PluginServer，新建插件类库项目，或者打包编译后引入编译好的dll文件或者导入nuget包文件，引用并且继承BasePlugin。

      BasePlugin基类是抽象类，所以必须实现的属性有
       - Name：插件名称(请使用英文)
       - Desc：插件描述
       - Version：版本。

      其他属性说明
       - Admin：机器人的最高超管员（不是qq群的管理员），作用例如可以限制插件功能只能超管使用。
       - Permission：机器人普通管理员（不是qq群的管理员），作用例如可以限制插件功能只能管理员使用。
       - ConfPath：配置文件路径。注：如果插件需要配置文件，那么此属性必须重写，因为此属性只有文件夹路径，具体配置文件名称和格式自行定义，重写的get方法返回时必须为base.ConfPath+"config.json"，例如：public override string ConfPath {get{return base.ConfPath+"config.json"}}。
       - LogPath：日志文件路径。和ConfPath一样的使用方式。

      插件方法说明
       - SetTimer：定时任务，传入一个方法，和Schedule方法，使用示例：base.SetTimer(() => Console.Write(1), x => x.ToRunNow().AndEvery(1).Minutes())//立即执行，并且后面每分钟执行一次;
       - Excute：执行插件中的方法（发送QQ消息的方法），可以不重写。
       - GroupMessage：发送群消息，如需发送群消息，请重写。
       - FriendMessage：好友消息，如需发送好友消息，请重写。
       - EventMessage：事件消息，如需监听收到的QQ消息事件，请重写。

      **不持支前端界面插件，仅支持后端功能插件**。

## 四、常见问题

- Q:启用了阿里云盘功能，但是无法上传？
      
  A:请检查功能是否启用，然后检查打开图片的方式时候有配置默认程序（如何配置自行百度），否则打不开二维码，登录不了阿里云盘。
  
- Q:口袋监听过一段时间就失效？
      
  A:尝试首页重新启动，如果一直失效，请联系作者。
  
- Q:抖音功能无效？

  A:是的，抖音暂时无法使用

- Q:新版本无法启动，点击无反应？
  
  A:前往微软下载[WebView2运行时](https://developer.microsoft.com/zh-cn/microsoft-edge/webview2/?form=MA13LH#download)，通常情况较新的电脑选择中间x64，如果是比较老的电脑，自行百度方法查询自己电脑是多少位的。

## 五、反馈

   如果软件有问题或者有需要的功能可以通过此项目issue进行提问，或者联系作者QQ:1615842006。
## 六、作者声明

   此项目是开源的，个人可以任意更改代码使用，此软件及代码免费使用，但禁止倒卖此软件和此项目代码以及改版。
