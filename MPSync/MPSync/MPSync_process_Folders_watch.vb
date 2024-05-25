Imports System.IO

Class MPSync_process_Folders_watch

    Dim watchfolder As FileSystemWatcher
    Dim m_path As String = Nothing
    Dim s_path As String = Nothing
    Dim t_path As String = Nothing
    Dim folder_type As String = Nothing
    Dim selected_folder() As String = Nothing
    Dim lastprcessed As String = Nothing

    Public Sub watch_folder(ByVal path As String, ByVal foldertype As String, ByVal spath As String, ByVal tpath As String, ByVal selectedfolder() As String)

        If Not Directory.Exists(path) Then
            MPSync_process.logStats("MPSync: [watch_folder] folder " & path & " does not exist", MessageType.ERR)
            Exit Sub
        End If

        m_path = path
        s_path = spath
        t_path = tpath
        folder_type = foldertype
        selected_folder = selectedfolder

        watchfolder = New System.IO.FileSystemWatcher()

        MPSync_process.logStats("MPSync: " & foldertype & " watch starting for " & path, MessageType.LOG)

        'this is the path we want to monitor
        watchfolder.Path = path
        watchfolder.IncludeSubdirectories = True

        'Add a list of Filter we want to specify
        watchfolder.NotifyFilter = NotifyFilters.DirectoryName Or NotifyFilters.FileName Or NotifyFilters.LastWrite

        ' add the handler to each event
        AddHandler watchfolder.Changed, AddressOf fileChange
        AddHandler watchfolder.Created, AddressOf fileChange
        AddHandler watchfolder.Deleted, AddressOf fileChange
        AddHandler watchfolder.Renamed, AddressOf fileRename

        'Set this property to true to start watching
        watchfolder.EnableRaisingEvents = True

    End Sub

    Private Sub fileChange(ByVal source As Object, ByVal e As System.IO.FileSystemEventArgs)

        Try
            If (IO.File.GetAttributes(e.FullPath) And FileAttributes.Directory) <> FileAttributes.Directory And lastprcessed <> e.FullPath Then

                Dim file As String = Right(e.FullPath, Len(e.FullPath) - Len(s_path)) & "|WATCH"
                Dim folder As String
                Dim l As Integer = Len(m_path) + 1

                lastprcessed = e.FullPath

                If Right(m_path, 1) <> "\" Then l += 1

                folder = Mid(e.FullPath, l, InStr(l, e.FullPath, "\") - l)

                If selected_folder.Contains(folder) Or selected_folder.Contains("ALL") Then

                    Dim parm(0) As String
                    parm(0) = file

                    If e.ChangeType = WatcherChangeTypes.Changed Or e.ChangeType = WatcherChangeTypes.Created Then
                        MPSync_process_Folders.copy_Objects(s_path, t_path, parm)
                    ElseIf e.ChangeType = WatcherChangeTypes.Deleted Then
                        MPSync_process_Folders.delete_Objects(t_path, parm)
                    End If

                End If
            End If
        Catch ex As Exception
            MPSync_process.logStats("MPSync: fileChange failed on " & e.FullPath & " with exception: " & ex.Message, MessageType.ERR)
        End Try

    End Sub

    Private Sub fileRename(ByVal source As Object, ByVal e As System.IO.RenamedEventArgs)

        If lastprcessed = e.FullPath Then Exit Sub

        lastprcessed = e.FullPath

        Try
            Dim folder As String
            Dim l As Integer = Len(m_path) + 1

            If Right(m_path, 1) <> "\" Then l += 1

            folder = Mid(e.FullPath, l, InStr(l, e.FullPath, "\") - l)

            If selected_folder.Contains(folder) Or selected_folder.Contains("ALL") Then
                If MPSync_process_Folders.fileLocked(e.OldFullPath, "filerename") Then
                    MPSync_process.logStats("MPSync: " & folder_type & " - " & e.OldFullPath & " renamed to " & e.FullPath, MessageType.LOG)
                    FileSystem.Rename(e.OldFullPath, e.FullPath)
                End If
            End If
        Catch ex As Exception
            MPSync_process.logStats("MPSync: rename failed with exception: " & ex.Message, MessageType.ERR)
        End Try

    End Sub

End Class
