'百宝箱功能已从本改版中移除（原开源版也不含其实现）。
'这里仅保留若干被其他模块调用的公共方法，避免影响启动流程与自定义文本占位符。

Public Class PageOtherTest

    ''' <summary>
    ''' 自定义下载入口。功能已移除，静默忽略。
    ''' </summary>
    Public Shared Sub StartCustomDownload(Url As String, FileName As String, Optional Folder As String = Nothing)
    End Sub

    ''' <summary>
    ''' 今日人品。功能已移除，静默忽略。
    ''' </summary>
    Public Shared Sub Jrrp()
    End Sub

    ''' <summary>
    ''' 垃圾清理。功能已移除，静默忽略。
    ''' </summary>
    Public Shared Sub RubbishClear()
    End Sub

    ''' <summary>
    ''' 内存优化。功能已移除，静默忽略。
    ''' </summary>
    Public Shared Sub MemoryOptimize(ShowHint As Boolean)
    End Sub

    ''' <summary>
    ''' 内存优化（内部调用）。功能已移除，静默忽略。
    ''' </summary>
    Public Shared Sub MemoryOptimizeInternal(ShowHint As Boolean)
    End Sub

    ''' <summary>
    ''' 自定义文本中的 {cave} 占位符。功能已移除，返回空串。
    ''' </summary>
    Public Shared Function GetRandomCave() As String
        Return ""
    End Function

    ''' <summary>
    ''' 启动页「你知道吗」的小提示。
    ''' </summary>
    Public Shared Function GetRandomHint() As String
        Return "启动中……如果长时间没有反应，可以去「设置 → 启动」检查 Java 与内存设置。"
    End Function

    ''' <summary>
    ''' 预设提示。功能已移除，返回空串。
    ''' </summary>
    Public Shared Function GetRandomPresetHint() As String
        Return ""
    End Function

End Class
