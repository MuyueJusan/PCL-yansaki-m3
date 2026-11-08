# PCL (M3 外观重写)

> ### 第三方二次创作声明
>
> 本仓库是基于 **[Meloong-Git/PCL](https://github.com/Meloong-Git/PCL)** 修改而来的**非官方**衍生作品，
> 与 PCL 官方（龙腾猫跃 / Meloong）**没有任何隶属、授权或合作关系**。
>
> 修改范围**仅限界面外观层**，未改动任何业务逻辑。所有功能行为与原版一致。
>
> 如果你需要原版 PCL，请前往 **https://meloong.com/pcl** 下载官方版本。

---

## 这个版本改了什么

外观层按 **Material 3 Expressive** 规范重写：

- **配色**：`Modules/ModSecret.vb` 的主题引擎改为 CIELAB 色调板生成，引入 M3 的完整色彩角色
  （`primary` / `onPrimary` / `primaryContainer` / `secondaryContainer` / `surfaceContainer{Lowest..Highest}` /
  `outline` / `outlineVariant` / `error` / `errorContainer` 等），并按色调值（tone 10–95）自动推导。
- **形状**：统一的圆角尺度（4 / 8 / 12 / 16 / 28 / full），按钮由胶囊形改为 12px 圆角矩形。
- **按钮**：实现 M3 的 outlined / filled / tonal / text / error-container 五种变体，
  含 hover 与 pressed 的 state layer。
- **控件模板**：`Application.xaml`、`MyButton`、`MyCheckBox`、`MyRadioBox`、`MySlider`、`MyTextBox`、
  `MyComboBox`、`MyHint`、`MyCard`、`MyListItem`、`MyMsg*` 系列、`FormMain` 标题栏等全部重写。
- **排版**：按 M3 type scale 调整字号 / 字重 / 行高。

**兼容性说明**：所有原有资源键名（`ColorBrush*` / `ColorObject*`）、控件 `x:Name`、
以及 `Color1`–`Color8` 的语义顺序均完整保留。因此项目中约 200 处
`AaColor(..., "ColorBrushN", ...)` 动画调用和全部 `DynamicResource` 引用照常工作，
不需要跟着改代码。

**未改动**：下载、启动、版本管理、Mod 管理、整合包安装、微软登录流程、配置文件格式等。

## 构建

### 环境要求

| 依赖 | 版本 |
|---|---|
| Visual Studio | 2022（需勾选 **.NET 桌面开发**） |
| .NET Framework | 4.8 开发工具包（Targeting Pack） |
| .NET SDK | 6.0 或以上（用于编译 `PCLCS` 等 SDK 风格项目） |

### 步骤

```bash
# 1. 克隆（MeloongCore 是 submodule，必须带 --recursive）
git clone --recursive https://github.com/MuyueJusan/PCL-yansaki-m3.git
cd PCL-yansaki-m3

# 2. 打开解决方案
#    Plain Craft Launcher 2.sln
#    配置选 Release / AnyCPU，生成即可
```

产物位于 `Plain Craft Launcher 2/bin/`，主程序是 `Plain Craft Launcher 2.exe`。
`Plain Craft Launcher 2.exe.config` 必须与 exe 放在同一目录。

### 命令行构建（不装 Visual Studio）

`PCLCS` 是 SDK 风格项目，`Plain Craft Launcher 2.vbproj` 是旧式 WPF 项目，两者需要的工具链不同：

```bash
# SDK 风格项目（连带 MeloongCore / MeloongCore.Wpf）
dotnet build PCLCS/PCLCS.csproj -c Release

# 旧式 vbproj：MSBuild 4.8 + net48 Targeting Pack
msbuild "Plain Craft Launcher 2/Plain Craft Launcher 2.vbproj" /t:Rebuild /p:Configuration=Release
```

> **注意**：`C:\Windows\Microsoft.NET\Framework64\v4.0.30319\vbc.exe` 是 VB 11 时代的旧编译器，
> 不支持字符串插值和 `?.`，直接用它会在编译阶段大量报 `BC30037`。
> 命令行构建时需要额外指定新版 Roslyn（`Microsoft.Net.Compilers.Toolset`），
> 否则请老老实实用 Visual Studio。

## 凭据说明

`Modules/ModSecret.vb` 开头有两个常量：

- `CurseForgeAPIKeyBuiltin` —— CurseForge API Key。**上游开源版本这里是空的**，本项目填了一个值。
- `OAuthClientIdBuiltin` —— 微软登录用的 OAuth Client ID（公共客户端，不是密钥）。

两者都支持外部覆盖，优先级高于内置值：

| 项目 | 覆盖方式（从高到低） |
|---|---|
| CurseForge Key | `PCL\CurseForgeKey.txt` → `CurseForgeKey.txt` → 内置 |
| 微软 Client ID | 环境变量 `PCL_MS_CLIENT_ID` → `PCL\MsClientId.txt` → `MsClientId.txt` → 内置 |

文件里只写内容本身，重启 PCL 即生效，无需重新编译。

> ⚠ 自己构建发布时，建议把 `CurseForgeAPIKeyBuiltin` 改回 `""`，
> 并到 https://console.curseforge.com/ 申请自己的 Key。
> 另外，微软自 2025 年起对第三方应用访问 Java 版游戏服务 API 实行**白名单审核**，
> 自建的 Client ID 需要先通过 https://aka.ms/mce-reviewappid 审核才能用于正版登录。

## 许可与署名

本仓库继续沿用上游的 **《PCL 分发有限许可》**，详见根目录的 [`LICENCE`](LICENCE) 文件。

- 原项目：**Plain Craft Launcher** by **龙腾猫跃（Meloong）**
- 上游仓库：https://github.com/Meloong-Git/PCL
- 官方下载：https://meloong.com/pcl
- 赞助作者：https://meloong.com/afd/a/LTCat

按上游《存储库合理使用指南》的要求：

1. 本仓库明确声明为第三方基于 PCL 独立进行的二次创作产物；
2. 名称以 PCL 开头并带第三方修改后缀；
3. 在「关于」页面首位给出龙腾猫跃署名及赞助链接，并公开源代码；
4. 保留与原版一致的 Minecraft 正版购买劝导逻辑；
5. 界面主要色相保持蓝色，不实现与赞助解锁主题类似的功能。

---

<sub>以下为上游 README 原文</sub>

## Plain Craft Launcher

[![Stars](https://img.shields.io/github/stars/Meloong-Git/PCL?style=flat&label=Stars&labelColor=444444&color=eac54f)](https://github.com/Meloong-Git/PCL/)
[![Issues](https://img.shields.io/github/issues/Meloong-Git/PCL?style=flat&label=Issues&labelColor=444444&color=1F883D)](https://github.com/Meloong-Git/PCL/issues)
[![哔哩哔哩](https://img.shields.io/badge/动态-BiliBili-00A4DB?style=flat&labelColor=444444&logoSize=auto)](https://space.bilibili.com/11343203/dynamic)
[![爱发电](https://img.shields.io/badge/赞助-爱发电-946ce6?style=flat&labelColor=444444&logoSize=auto)](https://meloong.com/afd/a/LTCat)

包含 PCL 的大部分源代码，包括 UI 库、动画模块、下载模块、Minecraft 启动模块等！<br/>
代码并不是实时更新的，它会在每次 PCL 发布更新时同步一次。<br/>
非常感谢社区的大力支持！

### 相关资源
- [PCL 下载](https://meloong.com/pcl)：下载可免费使用的正式版 PCL！
- [PCL 功能投票](https://github.com/Meloong-Git/PCL/discussions/2)：来参加投票吧，开发者会优先处理票数较高的项目！
- [帮助文档库](https://github.com/LTCatt/PCL2Help)：PCL 内置帮助文档的存储库
