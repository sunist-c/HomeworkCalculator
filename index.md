# Homework Calculator

## 什么是 `Homework Calculator`

`Homework Calculator`是 `SunistC`在当学委时受不了无休止的检查同学们作业命名规则以及格式的情况下，使用 `.Net Framework 4.7.2`，基于 `Windows Presentation Foundation`框架开发的工具软件，因学委换届的原因将本工具开源，这也是 `SunistC`的一个练手小项目，没有什么技术含量。

本项目的特色：

+ 没有进行多线程配置，对文件的操作又大多是耗时任务，所以假死是正常现象
+ 没有进行渲染优化，首页的表格没有进行分页，会不会卡完全看电脑性能
+ 解压有坑，zip/rar/7z/tar.gz等各种常规格式在不同设备间传输可能会导致文件损坏，进而导致文件损坏，所以暂时关闭了此功能
+ 使用了[HandyControl](https://github.com/HandyOrg/HandyControl)控件库，有好看的外观，但是用的是基础样式，有时间再考虑自己写样式
+ 使用了依赖注入(假的)思想，使用`config.json`配置文件以及`student.csv`名单文件进行配置
+ 没有为`Linux`和`macOS`提供支持，但按道理来说`.Net Core`是跨平台的，可以修改少量代码移植到其他平台上

## 怎么使用

1. 下载`Release`包，解压以后会发现以下三个文件：`config.json`，`student.csv`，`HomeworkCalculator.exe`
2. 将要检测的文件全部放在一个文件夹内
3. 将`config.json`，`student.csv`复制到上一步的文件夹内
4. 修改`config.json`和`student.csv`，内容为要检测的实际情况，参数说明见下节
5. 执行`HomeworkCalculator.exe`，选择上一步修改好的配置文件`config.json`
6. 实际情况进行操作

## 高级用法

### `config.json`的修改

`config.json`有四个属性，`AllFileType`，`MatchingRules`，`Brush`和 `DefaultFileName`。

+ `AllFileType` 是`Homework Calculator`检测的文件类型列表，其中的文件类型会被加载进运行时。文件类型拓展名需要大写，`.`标识符是不可缺少的
+ `MatchingRules`是一个枚举，有三种模式：`Name`，`StudentNumber`和`Strict`，区分大小写
  + `Name`为按照姓名进行文件名匹配，含有该同学的姓名即视为该同学提交了作业
  + `StudentNumber`为按照学号进行文件名匹配，含有该同学的学号即视为该同学提交了作业
  + `Strict`为严格按照命名规则进行匹配，命名规则将稍后解释
+ `Brush`是一个bool，为是否需要高亮显示(经检测在排序后高亮可能位置错误)
+ `DefaultFileName`是默认的文件命名规则，会在加载配置时在运行时设置

### `student.csv`的修改

你可以从 `Excel`，`Numbers`或 `WPS`等软件导出 `.csv`格式的 `student.csv`，使用名单导出就可以了，需要注意的是第一列是学号，第二列是姓名，其余信息皆为多于信息，不会进行读取

> 需要注意的是，`student.csv`文件的编码必须为UTF-8，如果打开后乱码，可以使用自带的记事本，另存为然后选择编码方式即可

### 命名规则

`HomeworkCalculator.exe`在运行时会有一个输入框提供命名规则的输入，该规则用于 `Strict`模式检测是否提交了作业，也用于文件名称的更改，命名规范由三部分(不是全部都需要)组成：

+ 变量： 使用`$`符号标识，如`$StudentNumber`，目前只有两个变量：`$StudentNumber`和`$Name`
  + `$StudentNumber`： 该变量表示学号，用来替换文件名称中学号的部分
  + `$Name`： 该变量表示姓名，用来替换文件名称中姓名的部分
+ 常量： 常量没有特殊规定，直接书写即可，但是不能包含系统在命名文件时的禁用符号和拓展名标识`.`
+ 分隔符： 使用`|`，在不同变量之间、变量与常量之间进行分隔

## FAQ

**我的电脑为什么运行不了**

> WPF应用可能需要Windows 7或更高版本的操作系统才能正常使用，可以尝试安装.Net Runtime看能否解决问题

**未响应、闪退怎么办**

> 查看主窗口右下角的状态栏是否表明了这是一个耗时操作，如果是的话，程序可能在假死，等待一会即可；如果不是的话，可能是有未经处理的异常，可能需要提交issue反馈这个BUG

**改变窗口大的小的时候反应特别慢**

> 这是因为改变窗口大小会重新渲染界面，而本程序没有进行优化，所以会导致卡顿，建议在最大化模式下使用

有其他问题请提交issue或邮件[sunit@mail.swu-acm.cn]()进行联系

## 二次开发

其实读完代码只需要不到半小时......

要在 `config.json`中加入更多字段的话，需要在 `configType`和 `Config`两个类中都增加相应字段，一定要设置 `get`方法。

## 许可

本软件使用GPL v2协议进行许可，可以在许可范围内任意使用，不过确实没什么价值(bushi
