'由于包含加解密等安全信息，本文件中的部分代码已被删除

Friend Module ModSecret

    '标注 PCL 的不同分支，仅用于替换标记
    Public Const VersionBranchMain As String = "OpenSource"
    '在开源版的注册表与常规版的注册表隔离，以防数据冲突
    Public Const RegFolder As String = "PCLDebug"
    '用于微软登录的 ClientId（OAuth 2.0 设备码流，公共客户端，不需要密钥）
    '内置 ID 写死在下方常量里；外部覆盖优先级更高，读取顺序为：
    '  环境变量 PCL_MS_CLIENT_ID  →  exe 同级 PCL\MsClientId.txt  →  exe 同级 MsClientId.txt  →  内置
    Public Const OAuthClientIdBuiltin As String = "ad21b870-4e14-4433-92aa-f9bfaf027334"
    Public OAuthClientId As String = SecretLoadKey(OAuthClientIdBuiltin, "MsClientId.txt", "PCL_MS_CLIENT_ID")
    'CurseForge API Key
    '内置 Key 直接写死在下方常量里（与官方发布版的写法一致），编译进 exe，开箱即用。
    '外部导入优先级更高，读取顺序为：
    '  exe 同级 PCL\CurseForgeKey.txt  →  exe 同级 CurseForgeKey.txt  →  内置 CurseForgeAPIKeyBuiltin
    '文件里只写 Key 本身（不要带 x-api-key: 之类的前缀），改完重启 PCL 即生效，无需重新编译；
    '删掉文件即自动回落到内置 Key。
    Public Const CurseForgeAPIKeyBuiltin As String = "f673b315-0321-44d2-90ec-287969ba144b"
    Public CurseForgeAPIKey As String = SecretLoadKey(CurseForgeAPIKeyBuiltin, "CurseForgeKey.txt")
    '用于匿名数据收集的腾讯云日志服务上报 URL，形如 https://{region}.cls.tencentcs.com/track?topic_id={topic_id}
    Public Const ClsBaseUrl As String = ""

    ''' <summary>
    ''' 读取一个密钥/ID：EnvName 非空时先看环境变量，然后看外部文件
    ''' （PCL\文件名 → exe 同级文件名），都没有则回退到 Builtin。
    ''' 文件内容为空白视为缺失；任何异常都回退到 Builtin，不阻断启动。
    ''' </summary>
    Private Function SecretLoadKey(Builtin As String, FileName As String, Optional EnvName As String = "") As String
        Try
            If EnvName <> "" Then
                Dim EnvValue As String = Environment.GetEnvironmentVariable(EnvName)
                If Not String.IsNullOrWhiteSpace(EnvValue) Then Return EnvValue.Trim()
            End If
            Dim Base As String = AppDomain.CurrentDomain.BaseDirectory
            Dim Candidates As String() = {
                System.IO.Path.Combine(Base, "PCL", FileName),
                System.IO.Path.Combine(Base, FileName)
            }
            For Each Candidate As String In Candidates
                If System.IO.File.Exists(Candidate) Then
                    Dim Key As String = System.IO.File.ReadAllText(Candidate).Trim()
                    If Key <> "" Then Return Key
                End If
            Next
        Catch ex As Exception
            '读取失败不应阻断启动，回退到内置值
        End Try
        Return Builtin
    End Function

#Region "网络鉴权"

    Friend Function SecretCdnSign(UrlWithMark As String) As String
        If Not UrlWithMark.EndsWithF("{CDN}") Then Return UrlWithMark
        Return UrlWithMark.Replace("{CDN}", "").Replace(" ", "%20")
    End Function
    ''' <summary>
    ''' 设置 Headers 的 UA、Referer。
    ''' </summary>
    Friend Sub SecretHeadersSign(Url As String, ByRef Req As HttpRequestMessage, Optional SimulateBrowserHeaders As Boolean = False)
        If Not Req.Headers.UserAgent.Any Then
            If Url.Contains("baidupcs.com") OrElse Url.Contains("baidu.com") Then
                Req.Headers.Add("User-Agent", "LogStatistic")  '#4951
            ElseIf SimulateBrowserHeaders Then
                Req.Headers.Add("User-Agent", $"PCL2/{VersionBaseName}.{CInt(BuildType)} Mozilla/5.0 AppleWebKit/537.36 Chrome/63.0.3239.132 Safari/537.36")
            Else
                Req.Headers.Add("User-Agent", $"PCL2/{VersionBaseName}.{CInt(BuildType)}")
            End If
        End If
        If Not SimulateBrowserHeaders Then Req.Headers.Add("Referer", $"http://{VersionCode}.open.pcl2.server/")
        If Url.Contains("api.curseforge.com") OrElse Url.Contains("forgecdn.net") Then Req.Headers.Add("x-api-key", CurseForgeAPIKey)
    End Sub

#End Region

#Region "主题"

    '==== Material 3 Expressive 设计令牌 ====
    'Color1-8 / ColorBg0-1 / ColorGray1-8 保持原有键名与语义顺序，以保证全部既有的
    'DynamicResource 引用与代码动画（AaColor）继续生效。语义映射如下：
    '  1 = onSurface（正文）        2 = primary 深（强调文字、选中描边）
    '  3 = primary（主色）          4 = primary 亮（悬停）
    '  5 = primaryContainer         6 = secondaryContainer
    '  7 = surfaceContainerLow      8 = surfaceContainerLowest
    Public Color1 As New MyColor(27, 27, 31)
    Public Color2 As New MyColor(0, 74, 148)
    Public Color3 As New MyColor(0, 99, 192)
    Public Color4 As New MyColor(0, 122, 230)
    Public Color5 As New MyColor(211, 228, 255)
    Public Color6 As New MyColor(226, 232, 240)
    Public Color7 As New MyColor(246, 247, 250)
    Public Color8 As New MyColor(253, 253, 255)
    Public ColorBg0 As New MyColor(196, 199, 208)
    Public ColorBg1 As New MyColor(190, Color7)
    Public ColorGray1 As New MyColor(66, 71, 78)
    Public ColorGray2 As New MyColor(90, 95, 102)
    Public ColorGray3 As New MyColor(115, 119, 127)
    Public ColorGray4 As New MyColor(140, 144, 152)
    Public ColorGray5 As New MyColor(196, 199, 208)
    Public ColorGray6 As New MyColor(234, 235, 240)
    Public ColorGray7 As New MyColor(240, 241, 246)
    Public ColorGray8 As New MyColor(246, 247, 250)
    Public ColorSemiTransparent As New MyColor(1, Color8)

    'M3 色彩角色（与上面 1-8 的语义并存，供新模板直接引用）
    Public ColorSurface As New MyColor(253, 253, 255)
    Public ColorSurfaceDim As New MyColor(217, 217, 224)
    Public ColorSurfaceContainerLowest As New MyColor(255, 255, 255)
    Public ColorSurfaceContainerLow As New MyColor(246, 247, 250)
    Public ColorSurfaceContainer As New MyColor(240, 241, 246)
    Public ColorSurfaceContainerHigh As New MyColor(234, 235, 240)
    Public ColorSurfaceContainerHighest As New MyColor(226, 228, 235)
    Public ColorOnSurface As New MyColor(27, 27, 31)
    Public ColorOnSurfaceVariant As New MyColor(66, 71, 78)
    Public ColorPrimary As New MyColor(0, 99, 192)
    Public ColorOnPrimary As New MyColor(255, 255, 255)
    Public ColorPrimaryContainer As New MyColor(211, 228, 255)
    Public ColorOnPrimaryContainer As New MyColor(0, 29, 62)
    Public ColorSecondaryContainer As New MyColor(226, 232, 240)
    Public ColorOnSecondaryContainer As New MyColor(24, 30, 38)
    Public ColorTertiary As New MyColor(0, 96, 120)
    Public ColorTertiaryContainer As New MyColor(190, 235, 250)
    Public ColorOutline As New MyColor(115, 119, 127)
    Public ColorOutlineVariant As New MyColor(196, 199, 208)
    Public ColorError As New MyColor(186, 26, 26)
    Public ColorOnError As New MyColor(255, 255, 255)
    Public ColorErrorContainer As New MyColor(255, 218, 214)
    Public ColorOnErrorContainer As New MyColor(65, 0, 2)
    Public ColorInverseSurface As New MyColor(48, 48, 53)
    Public ColorInverseOnSurface As New MyColor(242, 240, 246)

    Public ThemeNow As Integer = -1
    Public ColorHue As Integer = 210, ColorSat As Integer = 85, ColorLightAdjust As Integer = 0, ColorHueTopbarDelta As OneOf(Of Integer, Integer()) = 0
    Public ThemeDontClick As Integer = 0
    ''' <summary>
    ''' 当前主题对应的 CIELAB 色相角（由 ColorHue 换算而来，供调色板与背景使用）。
    ''' </summary>
    Public ThemeLabHue As Double = 281.1

    Public Sub ThemeRefresh(Optional NewTheme As Integer = -1)
        Try
            If ThemeNow = NewTheme AndAlso NewTheme >= 0 Then Return
            If NewTheme >= 0 Then ThemeNow = NewTheme

            '把用户设定的 HSL 色相换算成 CIELAB 色相角（两者刻度完全不同，必须换算）
            ThemeLabHue = SeedLabHue(ColorHue, ColorSat)
            Dim Hue As Double = ThemeLabHue
            Dim SatF As Double = Math.Max(0.0, Math.Min(100.0, ColorSat)) / 85.0
            Dim Adj As Double = ColorLightAdjust
            Dim CPrimary As Double = 46 * SatF
            Dim CSecondary As Double = 14 * SatF
            Dim CTertiary As Double = 26 * SatF
            Dim CNeutral As Double = 3.2 * SatF
            Dim CNeutralVar As Double = 7 * SatF

            '语义 1-8
            Color1 = ToneC(Hue, CNeutral, 10 + Adj)
            Color2 = ToneC(Hue, CPrimary, 30 + Adj)
            Color3 = ToneC(Hue, CPrimary, 40 + Adj)
            Color4 = ToneC(Hue, CPrimary, 52 + Adj)
            Color5 = ToneC(Hue, CPrimary, 90)
            Color6 = ToneC(Hue, CSecondary, 92)
            Color7 = ToneC(Hue, CNeutral, 96)
            Color8 = ToneC(Hue, CNeutral, 99)
            ColorBg0 = ToneC(Hue, CNeutralVar, 80)
            ColorBg1 = New MyColor(190, Color7)
            ColorSemiTransparent = New MyColor(1, Color8)
            '中性阶
            ColorGray1 = ToneC(Hue, CNeutralVar, 30)
            ColorGray2 = ToneC(Hue, CNeutralVar, 40)
            ColorGray3 = ToneC(Hue, CNeutralVar, 50)
            ColorGray4 = ToneC(Hue, CNeutralVar, 60)
            ColorGray5 = ToneC(Hue, CNeutralVar, 80)
            ColorGray6 = ToneC(Hue, CNeutral, 92)
            ColorGray7 = ToneC(Hue, CNeutral, 94)
            ColorGray8 = ToneC(Hue, CNeutral, 96)
            'M3 色彩角色
            ColorSurface = ToneC(Hue, CNeutral, 98)
            ColorSurfaceDim = ToneC(Hue, CNeutral, 87)
            ColorSurfaceContainerLowest = ToneC(Hue, CNeutral, 100)
            ColorSurfaceContainerLow = ToneC(Hue, CNeutral, 96)
            ColorSurfaceContainer = ToneC(Hue, CNeutral, 94)
            ColorSurfaceContainerHigh = ToneC(Hue, CNeutral, 92)
            ColorSurfaceContainerHighest = ToneC(Hue, CNeutral, 90)
            ColorOnSurface = Color1
            ColorOnSurfaceVariant = ColorGray1
            ColorPrimary = Color3
            ColorOnPrimary = ToneC(Hue, CPrimary, 100)
            ColorPrimaryContainer = Color5
            ColorOnPrimaryContainer = ToneC(Hue, CPrimary, 10)
            ColorSecondaryContainer = Color6
            ColorOnSecondaryContainer = ToneC(Hue, CSecondary, 10)
            ColorTertiary = ToneC(Hue + 60, CTertiary, 40 + Adj)
            ColorTertiaryContainer = ToneC(Hue + 60, CTertiary, 90)
            ColorOutline = ColorGray3
            ColorOutlineVariant = ColorBg0
            ColorError = New MyColor(186, 26, 26)
            ColorOnError = New MyColor(255, 255, 255)
            ColorErrorContainer = New MyColor(255, 218, 214)
            ColorOnErrorContainer = New MyColor(65, 0, 2)
            ColorInverseSurface = ToneC(Hue, CNeutral, 20)
            ColorInverseOnSurface = ToneC(Hue, CNeutral, 95)

            '写入资源字典：原有键名全部保留，另补 M3 角色键
            SetColorResource("ColorBrush1", Color1)
            SetColorResource("ColorBrush2", Color2)
            SetColorResource("ColorBrush3", Color3)
            SetColorResource("ColorBrush4", Color4)
            SetColorResource("ColorBrush5", Color5)
            SetColorResource("ColorBrush6", Color6)
            SetColorResource("ColorBrush7", Color7)
            SetColorResource("ColorBrush8", Color8)
            SetColorResource("ColorBrushBg0", ColorBg0)
            SetColorResource("ColorBrushBg1", ColorBg1)
            SetColorResource("ColorBrushGray1", ColorGray1)
            SetColorResource("ColorBrushGray2", ColorGray2)
            SetColorResource("ColorBrushGray3", ColorGray3)
            SetColorResource("ColorBrushGray4", ColorGray4)
            SetColorResource("ColorBrushGray5", ColorGray5)
            SetColorResource("ColorBrushGray6", ColorGray6)
            SetColorResource("ColorBrushGray7", ColorGray7)
            SetColorResource("ColorBrushGray8", ColorGray8)
            SetColorResource("ColorBrushSurface", ColorSurface)
            SetColorResource("ColorBrushSurfaceDim", ColorSurfaceDim)
            SetColorResource("ColorBrushSurfaceContainerLowest", ColorSurfaceContainerLowest)
            SetColorResource("ColorBrushSurfaceContainerLow", ColorSurfaceContainerLow)
            SetColorResource("ColorBrushSurfaceContainer", ColorSurfaceContainer)
            SetColorResource("ColorBrushSurfaceContainerHigh", ColorSurfaceContainerHigh)
            SetColorResource("ColorBrushSurfaceContainerHighest", ColorSurfaceContainerHighest)
            SetColorResource("ColorBrushSurfaceVariant", ColorSurfaceContainerHighest)
            SetColorResource("ColorBrushOnSurface", ColorOnSurface)
            SetColorResource("ColorBrushOnSurfaceVariant", ColorOnSurfaceVariant)
            SetColorResource("ColorBrushPrimary", ColorPrimary)
            SetColorResource("ColorBrushOnPrimary", ColorOnPrimary)
            SetColorResource("ColorBrushPrimaryContainer", ColorPrimaryContainer)
            SetColorResource("ColorBrushOnPrimaryContainer", ColorOnPrimaryContainer)
            SetColorResource("ColorBrushSecondaryContainer", ColorSecondaryContainer)
            SetColorResource("ColorBrushOnSecondaryContainer", ColorOnSecondaryContainer)
            SetColorResource("ColorBrushTertiary", ColorTertiary)
            SetColorResource("ColorBrushTertiaryContainer", ColorTertiaryContainer)
            SetColorResource("ColorBrushOutline", ColorOutline)
            SetColorResource("ColorBrushOutlineVariant", ColorOutlineVariant)
            SetColorResource("ColorBrushError", ColorError)
            SetColorResource("ColorBrushOnError", ColorOnError)
            SetColorResource("ColorBrushErrorContainer", ColorErrorContainer)
            SetColorResource("ColorBrushOnErrorContainer", ColorOnErrorContainer)
            SetColorResource("ColorBrushInverseSurface", ColorInverseSurface)
            SetColorResource("ColorBrushInverseOnSurface", ColorInverseOnSurface)

            SetColorObject("ColorObject1", Color1)
            SetColorObject("ColorObject2", Color2)
            SetColorObject("ColorObject3", Color3)
            SetColorObject("ColorObject4", Color4)
            SetColorObject("ColorObject5", Color5)
            SetColorObject("ColorObject6", Color6)
            SetColorObject("ColorObject7", Color7)
            SetColorObject("ColorObject8", Color8)
            SetColorObject("ColorObjectBg0", ColorBg0)
            SetColorObject("ColorObjectBg1", ColorBg1)
            SetColorObject("ColorObjectGray1", ColorGray1)
            SetColorObject("ColorObjectGray2", ColorGray2)
            SetColorObject("ColorObjectGray3", ColorGray3)
            SetColorObject("ColorObjectGray4", ColorGray4)
            SetColorObject("ColorObjectGray5", ColorGray5)
            SetColorObject("ColorObjectGray6", ColorGray6)
            SetColorObject("ColorObjectGray7", ColorGray7)
            SetColorObject("ColorObjectGray8", ColorGray8)
            SetColorObject("ColorObjectSurface", ColorSurface)
            SetColorObject("ColorObjectSurfaceContainer", ColorSurfaceContainer)
            SetColorObject("ColorObjectSurfaceContainerHigh", ColorSurfaceContainerHigh)
            SetColorObject("ColorObjectOnSurface", ColorOnSurface)
            SetColorObject("ColorObjectPrimary", ColorPrimary)
            SetColorObject("ColorObjectPrimaryContainer", ColorPrimaryContainer)
            SetColorObject("ColorObjectOutline", ColorOutline)
            SetColorObject("ColorObjectOutlineVariant", ColorOutlineVariant)
            SetColorObject("ColorObjectError", ColorError)
            ThemeRefreshMain()
        Catch ex As Exception
            Logger.Error(ex, "刷新主题颜色失败", LogBehavior.Toast)
        End Try
    End Sub
    Public Sub ThemeRefreshMain()
        RunInUi(
        Sub()
            If Not FrmMain.IsLoaded Then Return
            '顶部应用栏：M3 主色平面（替代原有的三段渐变）
            FrmMain.PanTitle.Background = New SolidColorBrush(ColorPrimary)
            FrmMain.PanTitle.Background.Freeze()
            '主页面背景：M3 表面层级
            If Settings.Get(Of Boolean)("UiBackgroundColorful") Then
                '保留"多彩背景"设置，改为同色系相邻色调的极轻渐变，符合 M3 的柔和表面
                Dim Brush = New LinearGradientBrush With {.EndPoint = New Point(0.15, 1), .StartPoint = New Point(0.85, 0)}
                Brush.GradientStops.Add(New GradientStop With {.Offset = -0.1, .Color = Tone(ThemeLabHue - 10, 8, 96)})
                Brush.GradientStops.Add(New GradientStop With {.Offset = 0.45, .Color = Tone(ThemeLabHue, 5, 98)})
                Brush.GradientStops.Add(New GradientStop With {.Offset = 1.1, .Color = Tone(ThemeLabHue + 10, 8, 96)})
                FrmMain.PanForm.Background = Brush
            Else
                FrmMain.PanForm.Background = New SolidColorBrush(ColorSurface)
            End If
            FrmMain.PanForm.Background.Freeze()
        End Sub)
    End Sub

    ''' <summary>
    ''' 把颜色写入资源字典的 ColorBrush 键。
    ''' </summary>
    Private Sub SetColorResource(Key As String, Value As MyColor)
        Application.Current.Resources(Key) = New SolidColorBrush(Value)
    End Sub

    ''' <summary>
    ''' 把颜色写入资源字典的 ColorObject 键（供 MyDropShadow 等需要 Color 的属性使用）。
    ''' </summary>
    Private Sub SetColorObject(Key As String, Value As MyColor)
        Application.Current.Resources(Key) = CType(Value, Color)
    End Sub

    ''' <summary>
    ''' 取 M3 色调：CIELAB 明度 = Tone（0-100），色相与色度固定；超出 sRGB 色域时自动降低色度。
    ''' </summary>
    Private Function Tone(Hue As Double, Chroma As Double, ToneValue As Double) As Color
        Dim T = Math.Max(0.0, Math.Min(100.0, ToneValue))
        Dim C = Math.Max(0.0, Chroma)
        For i = 0 To 40
            Dim R As Integer = 0, G As Integer = 0, B As Integer = 0
            If TryLabToRgb(T, C, Hue, R, G, B) Then Return Color.FromRgb(CByte(R), CByte(G), CByte(B))
            C *= 0.88
        Next
        Dim R2 As Integer = 0, G2 As Integer = 0, B2 As Integer = 0
        TryLabToRgb(T, 0, Hue, R2, G2, B2)
        Return Color.FromRgb(CByte(R2), CByte(G2), CByte(B2))
    End Function

    Private Function ToneC(Hue As Double, Chroma As Double, ToneValue As Double) As MyColor
        Dim C = Tone(Hue, Chroma, ToneValue)
        Return New MyColor(C.A, C.R, C.G, C.B)
    End Function

    ''' <summary>
    ''' CIELAB(D65) → sRGB。返回是否落在色域内。
    ''' </summary>
    Private Function TryLabToRgb(L As Double, C As Double, H As Double, ByRef R As Integer, ByRef G As Integer, ByRef B As Integer) As Boolean
        Dim Rad = H * Math.PI / 180
        Dim A = C * Math.Cos(Rad)
        Dim Bb = C * Math.Sin(Rad)
        Dim Fy = (L + 16) / 116
        Dim Xr = LabF((L + 16) / 116 + A / 500)
        Dim Yr = If(L > 8, Fy ^ 3, L / 903.2962962)
        Dim Zr = LabF((L + 16) / 116 - Bb / 200)
        Dim X = Xr * 0.95047
        Dim Y = Yr * 1.0
        Dim Z = Zr * 1.08883
        Dim Rl = 3.2404542 * X - 1.5371385 * Y - 0.4985314 * Z
        Dim Gl = -0.969266 * X + 1.8760108 * Y + 0.041556 * Z
        Dim Bl = 0.0556434 * X - 0.2040259 * Y + 1.0572252 * Z
        If Rl < -0.0005 OrElse Rl > 1.0005 OrElse Gl < -0.0005 OrElse Gl > 1.0005 OrElse Bl < -0.0005 OrElse Bl > 1.0005 Then Return False
        R = GammaByte(Rl)
        G = GammaByte(Gl)
        B = GammaByte(Bl)
        Return True
    End Function

    Private Function LabF(V As Double) As Double
        Return If(V ^ 3 > 0.008856, V ^ 3, (116 * V - 16) / 903.2962962)
    End Function

    ''' <summary>
    ''' 把 HSL 色相（0-360，UI 中的色轮刻度）换算为 CIELAB 色相角（0-360）。
    ''' 两者零点与刻度均不同，例如 HSL 210（蓝）在 Lab 中约为 281。
    ''' </summary>
    Private Function SeedLabHue(HslHue As Double, Sat As Double) As Double
        Dim H = ((HslHue Mod 360) + 360) Mod 360
        Dim S = Math.Max(0.0, Math.Min(100.0, Sat)) / 100.0
        Dim Cx = S
        Dim Xx = Cx * (1 - Math.Abs((H / 60) Mod 2 - 1))
        Dim R = 0.0, G = 0.0, B = 0.0
        Select Case CInt(Math.Floor(H / 60)) Mod 6
            Case 0
                R = Cx : G = Xx
            Case 1
                R = Xx : G = Cx
            Case 2
                G = Cx : B = Xx
            Case 3
                G = Xx : B = Cx
            Case 4
                R = Xx : B = Cx
            Case 5
                R = Cx : B = Xx
        End Select
        Dim M = 0.5 - Cx / 2
        R += M : G += M : B += M
        R = SrgbToLinear(R) : G = SrgbToLinear(G) : B = SrgbToLinear(B)
        Dim Xr = 0.4124564 * R + 0.3575761 * G + 0.1804375 * B
        Dim Yr = 0.2126729 * R + 0.7151522 * G + 0.072175 * B
        Dim Zr = 0.0193339 * R + 0.119192 * G + 0.9503041 * B
        Dim A = 500 * (LabF2(Xr / 0.95047) - LabF2(Yr))
        Dim Bb = 200 * (LabF2(Yr) - LabF2(Zr / 1.08883))
        Dim Deg = Math.Atan2(Bb, A) * 180 / Math.PI
        Return ((Deg Mod 360) + 360) Mod 360
    End Function

    Private Function SrgbToLinear(V As Double) As Double
        Return If(V <= 0.04045, V / 12.92, ((V + 0.055) / 1.055) ^ 2.4)
    End Function

    Private Function LabF2(T As Double) As Double
        Return If(T > 0.008856, T ^ (1 / 3), (903.2962962 * T + 16) / 116)
    End Function

    Private Function GammaByte(V As Double) As Integer
        Dim X = Math.Max(0.0, Math.Min(1.0, V))
        Dim S = If(X <= 0.0031308, 12.92 * X, 1.055 * X ^ (1 / 2.4) - 0.055)
        Return CInt(Math.Round(Math.Max(0.0, Math.Min(1.0, S)) * 255))
    End Function
    Friend Sub ThemeCheckAll(EffectSetup As Boolean)
    End Sub
    Friend Function ThemeCheckOne(Id As Integer) As Boolean
        Return True
    End Function
    Friend Function ThemeUnlock(Id As Integer, Optional ShowDoubleHint As Boolean = True, Optional UnlockHint As String = Nothing) As Boolean
        Return False
    End Function

#End Region

#Region "更新"

    Friend Sub UpdateCheckByButton()
        Hint("该版本中不包含更新功能……")
    End Sub

    Friend IsUpdateWaitingRestart As Boolean = False
    Public Sub UpdateRestart(TriggerRestartAndByEnd As Boolean)
    End Sub
    Public Sub UpdateReplace(ProcessId As Integer, OldFileName As String, NewFileName As String, TriggerRestart As Boolean)
    End Sub

    ''' <summary>
    ''' 确保 PathTemp/Latest.exe 是最新正式版的 PCL，它会被用于整合包打包。
    ''' 如果不是，则下载一个。
    ''' </summary>
    Friend Sub DownloadLatestPCL(Optional LoaderToSyncProgress As LoaderBase = Nothing)
        '注意：如果要自行实现这个功能，请换用另一个文件路径，以免与官方版本冲突
    End Sub

#End Region

#Region "联网配置"

    ''' <summary>
    ''' 联网获取的配置信息。
    ''' 若获取失败或仍在获取中，可能为 Nothing。
    ''' </summary>
    Public ServerConfig As JObject

    Public ServerLoader As New LoaderTask(Of Integer, Integer)("PCL 配置更新", Sub() Logger.Info("该版本中不包含更新通知功能……"), Priority:=ThreadPriority.BelowNormal) With
        {.ReloadTimeout = 1000 * 60 * 60} '超时 1 小时

#End Region

#Region "赞助等级"

    Public ReadOnly Property CurrentRank As DonationRank
        Get
            Return DonationRank.None
        End Get
    End Property

    Public Sub InputPotatoCode(IsUpdating As Boolean)
    End Sub
    Friend Sub GeneratePotatoCode()
    End Sub

    ''' <summary>
    ''' 获取设备识别码。
    ''' </summary>
    Friend Function GetIdentify() As String
        Return "0000-0000-0000-0000"
    End Function

#End Region

End Module
