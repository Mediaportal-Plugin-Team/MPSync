Imports System.IO
Imports System.Threading

Imports DirectoryEnumerator

Public Class MPSync_process_Folders

    Private Shared checkplayer As Integer = 30

    Dim s_paths() As String = Nothing
    Dim t_paths() As String = Nothing
    Dim foldertypes() As String = Nothing

    Private Shared _bw_active_fl_jobs As Integer

    Public Shared Sub bw_folders_worker(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs)

        Try
            Dim mps_folders As New MPSync_process_Folders

            MPSync_process.logStats("MPSync: [MPSync_process.WorkMethod][bw_folders_worker] Folders synchronization started.", "INFO")

            Dim x As Integer = -1
            Dim process_thread() As Thread = Nothing
            Dim item As Array
            Dim list As Array = MPSync_process.p_object_list

            _bw_active_fl_jobs = 0

            ' populate the CRC32 table
            createCRC32table()

            For Each obj As String In list

                item = Split(obj, "¬")

                If item(1) = "True" Then
                    ' check if there are available threads to submit current stream, unless there is no limit.

                    If MPSync_process.checkThreads("folders") <> -1 Then

                        Do While _bw_active_fl_jobs >= MPSync_process.checkThreads("folders")
                            MPSync_process.logStats("MPSync: [MPSync_process.WorkMethod][bw_folders_worker] waiting for available threads.", "DEBUG")
                            MPSync_process.wait(30, False)
                        Loop

                    End If

                    _bw_active_fl_jobs += 1

                    x += 1

                    ReDim Preserve mps_folders.foldertypes(x), mps_folders.s_paths(x), mps_folders.t_paths(x)
                    mps_folders.foldertypes(x) = UCase(item(0))

                    ReDim Preserve process_thread(x)
                    process_thread(x) = New Thread(AddressOf mps_folders.folders_processing)
                    process_thread(x).Start(item(0))
                End If

            Next

            If x <> -1 Then

                Dim active As Boolean = True

                Do Until active = False

                    active = False

                    For y As Integer = 0 To x

                        If process_thread(y).IsAlive Then
                            active = True
                            Exit For
                        End If

                    Next

                    If _bw_active_fl_jobs > 0 Then
                        If MPSync_process.p_Debug Then MPSync_process.logStats("MPSync: [MPSync_process.WorkMethod][bw_folders_worker] waiting for background threads to finish... " & _bw_active_fl_jobs.ToString & " threads remaining processing.", "DEBUG")
                        MPSync_process.wait(10, False)
                    End If

                Loop

            End If

            If MPSync_settings.syncnow Then MPSync_settings.folders_complete = True

            MPSync_process.logStats("MPSync: [MPSync_process.WorkMethod][bw_folders_worker] Folders synchronization complete.", "INFO")

        Catch ex As Exception
            MPSync_process.logStats("MPSync: [MPSync_process.WorkMethod][bw_folders_worker] Unexpected error " & ex.Message, "ERROR")
        End Try

    End Sub

    Private Sub folders_processing(ByVal foldertype As String)

        Try
            Dim folders_client As String = Nothing
            Dim folders_server As String = Nothing
            Dim folders() As String = Nothing
            Dim folders_direction As Integer = Nothing
            Dim folders_sync_method As Integer = Nothing
            Dim folders_pause As Boolean = False
            Dim folders_md5 As Boolean = False
            Dim folders_crc32 As Boolean = False

            MPSync_process.getObjectSettings(foldertype, folders_client, folders_server, folders_direction, folders_sync_method, folders, folders_pause, folders_md5, folders_crc32)

            Process(UCase(foldertype), folders_client, folders_server, folders_direction, folders_sync_method, folders, folders_pause, folders_md5, folders_crc32)

        Catch ex As Exception
            MPSync_process.logStats("MPSync: [MPSync_process.WorkMethod][folders_processing] Unexpected error " & ex.Message, "ERROR")
        End Try

    End Sub

    Private Sub Process(ByVal foldertype As String, ByVal clientpath As String, ByVal serverpath As String, ByVal direction As Integer, ByVal folders_sync_method As Integer, ByVal selectedfolder() As String, ByVal folders_pause As Boolean, ByVal folders_md5 As Boolean, ByVal folders_crc32 As Boolean)

        MPSync_process.logStats("MPSync: [Process] " & foldertype & " synchronization cycle starting.", "LOG")

        ' direction is client to server
        If direction <> 2 Then
            process_Folder(foldertype, clientpath, serverpath, folders_sync_method, selectedfolder, folders_pause, folders_md5, folders_crc32)
        End If

        ' direction is server to client
        If direction <> 1 Then
            process_Folder(foldertype, serverpath, clientpath, folders_sync_method, selectedfolder, folders_pause, folders_md5, folders_crc32)
        End If

        MPSync_process.logStats("MPSync: [Process] " & foldertype & " synchronization cycle complete.", "LOG")

        _bw_active_fl_jobs -= 1

        If Not MPSync_settings.syncnow Then

            Dim x As Integer = Array.IndexOf(foldertypes, foldertype)
            Dim mps_w As New MPSync_process_Folders_watch

            ' direction is client to server
            If direction <> 2 Then
                mps_w.watch_folder(clientpath, foldertype, s_paths(x), t_paths(x), selectedfolder)
            End If

            ' direction is server to client
            If direction <> 1 Then
                mps_w.watch_folder(serverpath, foldertype, s_paths(x), t_paths(x), selectedfolder)
            End If

        End If

    End Sub

    Private Function getobjectsDetails(ByVal path As String, ByVal c_path As String, ByVal selectedfolders() As String, ByVal md5 As Boolean, ByVal crc32 As Boolean) As Array

        Dim time1, time2 As Date
        Dim objects() As String = Nothing
        Dim folder As String = Nothing
        Dim l1 As Integer = Len(path) + 1
        Dim l2 As Integer = Len(c_path)
        Dim x As Integer = -1

        MPSync_process.logStats("MPSync: [getobjectsDetails] Scanning folder " & path & " for objects", "LOG")

        Try

            time1 = Now

            For Each file As FileData In FastDirectoryEnumerator.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)

                checkPlayerActive()

                Try
                    folder = Mid(file.Path, l1, InStr(l1, file.Path, "\") - l1)

                    If selectedfolders.Contains(folder) Or selectedfolders.Contains("ALL") Then
                        x += 1
                        ReDim Preserve objects(x)
                        If md5 Then
                            objects(x) = Right(file.Path, Len(file.Path) - l2) & "|" & file.LastWriteTimeUtc & "|" & file.Size & "|" & fileMD5(file.Path)
                        ElseIf crc32 Then
                            objects(x) = Right(file.Path, Len(file.Path) - l2) & "|" & file.LastWriteTimeUtc & "|" & file.Size & "|" & fileCRC32(file.Path)
                        Else
                            objects(x) = Right(file.Path, Len(file.Path) - l2) & "|" & file.LastWriteTimeUtc & "|" & file.Size
                        End If
                    End If
                Catch ex As Exception
                End Try

            Next

            If x = -1 Then
                x = 0
                ReDim objects(x)
                objects(x) = ""
            Else
                Array.Sort(objects)
            End If

            Dim dirs As List(Of String) = New List(Of String)(Directory.EnumerateDirectories(path, "*.*", SearchOption.AllDirectories))

            dirs.Reverse()

            For Each dir As String In dirs
                dir += "\"
                folder = Mid(dir, l1, InStr(l1, dir, "\") - l1)
                If selectedfolders.Contains(folder) Or selectedfolders.Contains("ALL") Then
                    x += 1
                    ReDim Preserve objects(x)
                    objects(x) = Right(dir, Len(dir) - l2) & "|FOLDER"
                End If
            Next

            time2 = Now

        Catch ex As Exception
            MPSync_process.logStats("MPSync: [getobjectsDetails] failed to read objects from folder " & path & " with exception: " & ex.Message, "ERROR")
        End Try

        MPSync_process.logStats("MPSync: [getobjectsDetails] " & x.ToString & " objects found in folder " & path & " in " & DateDiff(DateInterval.Second, time1, time2).ToString & " seconds", "LOG")

        Return objects

    End Function

    Private Sub process_Folder(ByVal foldertype As String, ByVal source As String, ByVal target As String, ByVal folders_sync_method As Integer, ByVal selectedfolder() As String, ByVal folders_pause As Boolean, ByVal folders_md5 As Boolean, ByVal folders_crc32 As Boolean)

        If Not Directory.Exists(source) Then
            MPSync_process.logStats("MPSync: [process_Folder] folder " & source & " does not exist", "ERROR")
            Exit Sub
        End If

        Dim diff As IEnumerable(Of String)
        Dim s_folders() As String = Nothing
        Dim t_folders() As String = Nothing
        Dim s_path, t_path As String
        Dim x As Integer = 0

        Try

            If Not Directory.Exists(target) Then Directory.CreateDirectory(target)

            Do While UCase(Right(source, x)) = UCase(Right(target, x))
                x += 1
            Loop

            x -= 1
            s_path = Left(source, Len(source) - x)
            t_path = Left(target, Len(target) - x)

            x = Array.IndexOf(foldertypes, foldertype)

            s_paths(x) = s_path
            t_paths(x) = t_path

            s_folders = CType(getobjectsDetails(source, s_path, selectedfolder, folders_md5, folders_crc32), String())
            t_folders = CType(getobjectsDetails(target, t_path, selectedfolder, folders_md5, folders_crc32), String())

            ' propagate deletions or both
            If folders_sync_method <> 1 And t_folders(0) <> "" Then
                diff = t_folders.Except(s_folders, StringComparer.InvariantCultureIgnoreCase)

                MPSync_process.logStats("MPSync: [process_Folder] found " & (UBound(diff.ToArray) + 1).ToString & " differences for deletion between " & source & " and " & target, "DEBUG")

                If UBound(diff.ToArray) >= 0 Then
                    If (diff.Count / t_folders.Count) <= 0.25 Then
                        delete_Objects(t_path, diff.ToArray)
                    Else
                        MPSync_process.logStats("MPSync: [process_Folder] differences for deletion exceed 25% treshold.  Deletion not allowed for " & source & " and " & target, "INFO")
                    End If
                End If

            End If

            ' propagate additions or both
            If folders_sync_method <> 2 And s_folders(0) <> "" Then
                diff = s_folders.Except(t_folders, StringComparer.InvariantCultureIgnoreCase)

                MPSync_process.logStats("MPSync: [process_Folder] found " & (UBound(diff.ToArray) + 1).ToString & " differences for addition/replacement between " & source & " and " & target, "DEBUG")

                If UBound(diff.ToArray) >= 0 Then copy_Objects(s_path, t_path, diff.ToArray, folders_pause)

            End If

        Catch ex As Exception
            MPSync_process.logStats("MPSync: [process_Folder] process failed with exception: " & ex.Message, "ERROR")
        End Try

        If s_folders IsNot Nothing Then Array.Clear(s_folders, 0, UBound(s_folders))
        If t_folders IsNot Nothing Then Array.Clear(t_folders, 0, UBound(t_folders))

    End Sub

    Public Shared Sub copy_Objects(ByVal s_path As String, ByVal t_path As String, ByVal parm As Array, Optional ByVal folders_pause As Boolean = False)

        Dim file As Array = Nothing
        Dim x As Integer
        Dim lock As New ReaderWriterLockSlim

        Dim directory As String

        Array.Sort(parm)

        For x = 0 To UBound(parm)

            If folders_pause Then CheckPlayerActive()

            file = Split(parm(x), "|")

            If file(1) = "FOLDER" Then

                directory = t_path & file(0)

                If Not IO.Directory.Exists(directory) Then
                    IO.Directory.CreateDirectory(directory)
                    MPSync_process.logStats("MPSync: [copy_Objects] directory missing, creating " & directory, "LOG")
                End If

            Else

                directory = IO.Path.GetDirectoryName(t_path & file(0))

                If Not IO.Directory.Exists(directory) Then
                    IO.Directory.CreateDirectory(directory)
                    MPSync_process.logStats("MPSync: [copy_Objects] directory missing, creating " & directory, "LOG")
                End If

                Try
                    If Not fileLocked(s_path & file(0), "copy_Objects") Then
                        IO.File.Copy(s_path & file(0), t_path & file(0), True)
                        MPSync_process.logStats("MPSync: [copy_Objects] " & t_path & file(0) & " copied.", "DEBUG")
                    End If
                Catch ex As Exception
                    MPSync_process.logStats("MPSync: [copy_Objects] copy failed with exception: " & ex.Message, "ERROR")
                End Try

            End If

        Next

        If file(1) <> "WATCH" Then MPSync_process.logStats("MPSync: [copy_Objects] " & x.ToString & " objects added/replaced.", "LOG")

    End Sub

    Public Shared Sub delete_Objects(ByVal t_path As String, ByVal parm As Array)

        Dim file As Array = Nothing
        Dim x As Integer
        Dim lock As New ReaderWriterLockSlim

        For x = 0 To UBound(parm)

            CheckPlayerActive()

            file = Split(parm(x), "|")

            Try
                If file(1) = "FOLDER" Then
                    IO.Directory.Delete(t_path & file(0))
                    MPSync_process.logStats("MPSync: [delete_Objects] " & t_path & file(0) & " deleted.", "DEBUG")
                Else
                    If Not fileLocked(t_path & file(0), "[delete_Objects]") Then
                        IO.File.Delete(t_path & file(0))
                        MPSync_process.logStats("MPSync: [delete_Objects] " & t_path & file(0) & " deleted.", "DEBUG")
                    End If
                End If
            Catch ex As Exception
                MPSync_process.logStats("MPSync: [delete_Objects] delete failed with exception: " & ex.Message, "ERROR")
            End Try

        Next

        If file(1) <> "WATCH" Then MPSync_process.logStats("MPSync: [delete_Objects] " & x.ToString & " objects removed.", "LOG")

    End Sub

    Private Shared Sub checkPlayerActive()
        Do While MediaPortal.Player.g_Player.Playing
            MPSync_process.wait(checkplayer, False)
        Loop
    End Sub

    Public Shared Function fileLocked(filename As String, proc As String) As Boolean

        Dim lock_count As Integer = 0
        Dim wait As Integer = 2000          '2 seconds

        Do While isFileLocked(filename)

            MPSync_process.logStats("MPSync: [" & proc & "] obtaining lock on file " & filename, "DEBUG")

            lock_count += 1

            If lock_count = 10 Then
                MPSync_process.logStats("MPSync: [" & proc & "] lock on file " & filename & "not obtained.  File in use, skipping file.", "DEBUG")
                Return True
            End If

            System.Threading.Thread.Sleep(wait)

        Loop

        Return False

    End Function

    Private Shared Function isFileLocked(filename As String) As Boolean

        Dim f As FileStream = Nothing

        Try
            f = New IO.FileStream(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.None)
        Catch
            Return True
        Finally
            If f IsNot Nothing Then
                f.Close()
            End If
        End Try

        Return False

    End Function

End Class
