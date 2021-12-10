# Null.TextSpeech

借助微软 Azure 服务实现的文本转语音小工具, 支持简单指令, 切换语言以及语音

### 使用

编译程序, 并编辑 AppConfig.json, 将 Subcription.ApiKey 以及 Subscription.Region 改为自己在 Azure 申请的 Speech 服务 API 密钥以及区域即可.

> TextSpeech.CurLang 和 TextSpeech.CurVoice 是当前使用的语言以及语音, 默认值为 null, 当使用 null 时, 程序会自动识别系统当前语言, 并选择该语言的默认语音

程序支持简单指令, 关于指令支持, 可以查看项目下的 AppCommands 类, 其中每一个标记了 Command 属性的方法都可以作为指令进行执行.

```bash
使用 /ShowLang 来查看当前语言
/ShowLang
使用 /ShowVoice 来查看当前使用的语音
/ShowVoice
显示所有可用语言 (在 AppConfig 中声明的)
/ShowAllLangs
设置语言为中文
/SetLang zh-CN
设置语音为中文晓晓
/SetVoice zh-CN-XiaoxiaoNeural
显示当前语言的所有可用语音
/ShowAllVoices
重载配置以及文本转语音
/Reload
重载配置
/ReloadConfig
重载文本转语音
/ReloadSpeech
```

### 引用的库

- Microsoft.CognitiveServices.Speech (微软文本转语音)

- NullLib.CommandLine (实现命令行指令)

### 编译

直接使用 Visual Studio 生成即可, 编译出的结果会包含一个拷贝的文件 AppConfig.json, 这是程序的默认配置文件, 用户需要在其中更改 API Key 以及 Region 等.
