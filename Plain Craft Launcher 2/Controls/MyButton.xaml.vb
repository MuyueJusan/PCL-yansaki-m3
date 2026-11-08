Imports System.Windows.Markup

<ContentProperty("Inlines")>
Public Class MyButton

    '声明
    Public Event Click(sender As Object, e As MouseButtonEventArgs) '自定义事件

    '自定义属性
    Public Uuid As Integer = GetUuid()
    Public ReadOnly Property Inlines As InlineCollection
        Get
            Return LabText.Inlines
        End Get
    End Property
    Public Property Text As String
        Get
            Return GetValue(TextProperty)
        End Get
        Set(value As String)
            SetValue(TextProperty, value)
        End Set
    End Property '显示文本
    Public Shared ReadOnly TextProperty As DependencyProperty = DependencyProperty.Register("Text", GetType(String), GetType(MyButton), New PropertyMetadata(New PropertyChangedCallback(
    Sub(sender As DependencyObject, e As DependencyPropertyChangedEventArgs)
        If sender IsNot Nothing Then CType(sender, MyButton).LabText.Text = e.NewValue
    End Sub)))
    Public Property TextPadding As Thickness
        Get
            Return LabText.Padding
        End Get
        Set(value As Thickness)
            LabText.Padding = value
        End Set
    End Property
    Private _ColorType As ColorState = ColorState.Normal  '配色方案
    Public Property ColorType As ColorState
        Get
            Return _ColorType
        End Get
        Set(value As ColorState)
            _ColorType = value
            RefreshColor()
        End Set
    End Property
    Public Enum ColorState
        Normal = 0
        Highlight = 1
        Red = 2
    End Enum
    '属性穿透
    Public Shared Shadows ReadOnly PaddingProperty As DependencyProperty = DependencyProperty.Register("Padding", GetType(Thickness), GetType(MyButton), New PropertyMetadata(New PropertyChangedCallback(
        Sub(sender As MyButton, e As DependencyPropertyChangedEventArgs) If sender IsNot Nothing Then sender.PanFore.Padding = e.NewValue)))
    Public Overloads Property Padding As Thickness
        Get
            Return PanFore.Padding
        End Get
        Set(value As Thickness)
            PanFore.Padding = value
        End Set
    End Property
    Public Property RealRenderTransform As Transform
        Get
            Return PanFore.RenderTransform
        End Get
        Set(value As Transform)
            PanFore.RenderTransform = value
        End Set
    End Property

    '自定义事件
    Private Const AnimationColorIn As Integer = 100
    Private Const AnimationColorOut As Integer = 200
    ''' <summary>
    ''' 按 Material 3 语义角色刷新按钮配色。
    ''' Normal = Outlined（浅底 + outline 描边 + onSurfaceVariant 文字）
    ''' Highlight = Filled（实心 primary 底 + onPrimary 文字，描边与背景同色故视觉隐形）
    ''' Red = Error container（浅红底 + onErrorContainer 文字）
    ''' 文字色与描边色已解耦：LabText.Foreground 由本方法单独驱动，不再绑定 BorderBrush。
    ''' 描边厚度恒为 1px，状态切换不会引起尺寸跳动。
    ''' </summary>
    Private Sub RefreshColor(Optional obj = Nothing, Optional e = Nothing) Handles Me.MouseEnter, Me.MouseLeave, Me.Loaded, Me.IsEnabledChanged
        Try
            Dim BgKey As String
            Dim BorderKey As String
            Dim ForeKey As String

            If Not IsEnabled Then
                'M3 禁用态：onSurface 低透明度容器 + 38% 文字
                BgKey = "ColorBrushSurfaceContainerLow"
                BorderKey = "ColorBrushOutlineVariant"
                ForeKey = "ColorBrushGray5"
            Else
                Select Case ColorType
                    Case ColorState.Normal 'M3 Outlined
                        BgKey = If(IsMouseOver, "ColorBrushSurfaceContainerHigh", "ColorBrushHalfWhite")
                        'M3 规范：描边用 outline，文字用 onSurfaceVariant，二者本来就不同色
                        BorderKey = If(IsMouseOver, "ColorBrushPrimary", "ColorBrushOutline")
                        ForeKey = If(IsMouseOver, "ColorBrushPrimary", "ColorBrushOnSurfaceVariant")
                    Case ColorState.Highlight 'M3 Filled
                        BgKey = If(IsMouseOver, "ColorBrush4", "ColorBrush3")
                        BorderKey = BgKey '描边与背景同色，视觉上等同无描边
                        ForeKey = "ColorBrushOnPrimary"
                    Case ColorState.Red 'M3 Error container
                        BgKey = If(IsMouseOver, "ColorBrushErrorContainer", "ColorBrushRedBack")
                        BorderKey = BgKey
                        ForeKey = "ColorBrushOnErrorContainer"
                    Case Else
                        BgKey = "ColorBrushHalfWhite"
                        BorderKey = "ColorBrushOnSurfaceVariant"
                        ForeKey = "ColorBrushOnSurfaceVariant"
                End Select
            End If

            If IsLoaded AndAlso AniControlEnabled = 0 Then '防止默认属性变更触发动画
                AniStart({
                    AaColor(PanFore, BackgroundProperty, BgKey, AnimationColorIn),
                    AaColor(PanFore, Border.BorderBrushProperty, BorderKey, AnimationColorIn),
                    AaColor(LabText, TextBlock.ForegroundProperty, ForeKey, AnimationColorIn)
                }, "MyButton Color " & Uuid)
            Else
                AniStop("MyButton Color " & Uuid)
                PanFore.SetResourceReference(BackgroundProperty, BgKey)
                PanFore.SetResourceReference(Border.BorderBrushProperty, BorderKey)
                LabText.SetResourceReference(TextBlock.ForegroundProperty, ForeKey)
            End If
        Catch ex As Exception
            Logger.Warn(ex, "刷新按钮颜色出错")
        End Try
    End Sub

    '鼠标点击判定（务必放在点击事件之后，以使得 Button_MouseUp 先于 Button_MouseLeave 执行）
    Private IsMouseDown As Boolean = False
    Private Sub Button_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles Me.MouseLeftButtonUp
        If Not IsMouseDown Then Return
        Logger.Info($"按下按钮：{Text}")
        RaiseEvent Click(sender, e)
        RaiseCustomEvent() '自定义事件
    End Sub
    Private Sub Button_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Me.MouseLeftButtonDown
        IsMouseDown = True
        Focus()
        AniStart({
                 AaScaleTransform(PanFore, 0.955 - CType(PanFore.RenderTransform, ScaleTransform).ScaleX, 80,, New AniEaseOutFluent(AniEasePower.ExtraStrong)),
                 AaScaleTransform(PanFore, -0.01, 700,, New AniEaseOutFluent(AniEasePower.Middle))
                 }, "MyButton Scale " & Uuid)
    End Sub
    '鼠标进入时的配色变化已统一由 RefreshColor 处理（其 Handles 已包含 Me.MouseEnter），
    '此处不再单独驱动 Background，避免两个动画组争抢同一属性。
    Private Sub Button_MouseUp() Handles Me.MouseLeftButtonUp
        If Not IsMouseDown Then Return
        IsMouseDown = False
        AniStart({
               AaScaleTransform(PanFore, 1 - CType(PanFore.RenderTransform, ScaleTransform).ScaleX, 300, 10, New AniEaseOutFluent(AniEasePower.Middle))
           }, "MyButton Scale " & Uuid)
    End Sub
    Private Sub Button_MouseLeave() Handles Me.MouseLeave
        If Not IsMouseDown Then Return
        IsMouseDown = False
        AniStart(AaScaleTransform(PanFore, 1 - CType(PanFore.RenderTransform, ScaleTransform).ScaleX, 800,, New AniEaseOutFluent(AniEasePower.Strong)), "MyButton Scale " & Uuid)
    End Sub

End Class
