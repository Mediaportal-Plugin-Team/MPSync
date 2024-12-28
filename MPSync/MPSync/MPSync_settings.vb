Imports MediaPortal.Configuration
Imports System.Reflection
Imports MediaPortal.GUI.Library

Imports System.Xml

Public Class MPSync_settings

    Dim _curversion As String = Assembly.GetExecutingAssembly().GetName().Version.ToString()
    Dim i_direction(2) As Image
    Dim i_method(2), _databases, _folders, _watched_dbs, _object_list, _db_objects, _version, _session, _sync_type As String
    Dim _db_sync_method, _folders_sync_method As Integer
    Dim _clicks_db As Integer = 1
    Dim _clicks_folders As Integer = 1
    Dim lb_status_timer As New System.Timers.Timer()

    Public Shared max_DB_threads As Integer = -1
    Public Shared max_folder_threads As Integer = -1
    Public Shared syncnow As Boolean = False
    Public Shared db_complete, folders_complete As Boolean
    Public Delegate Sub lb_statusItemsAddInvoker(ByVal text As String)

    Public Structure Watched
        Dim database As String
        Dim tables As Array
    End Structure

    Public i_watched(3) As Watched

    Public Shared Function GetConfigFileName() As String
        Return Config.GetFile(Config.Dir.Config, "MPSync.xml")
    End Function

    Public Sub SetWatched()
        i_watched(0).database = "movingpictures.db3"
        i_watched(0).tables = {"user_movie_settings", "movie_info"}
        i_watched(1).database = "MusicDatabaseV13.db3"
        i_watched(1).tables = {"tracks"}
        i_watched(2).database = "TVSeriesDatabase4.db3"
        i_watched(2).tables = {"local_episodes", "online_episodes", "online_series", "season"}
        i_watched(3).database = "VideoDatabaseV5.db3"
        i_watched(3).tables = {"bookmark", "movie", "resume"}
    End Sub

    Public ReadOnly Property getDatabase As String()
        Get
            If i_watched(0).database = Nothing Then SetWatched()
            Dim database(UBound(i_watched)) As String
            For x As Integer = 0 To UBound(i_watched)
                database(x) = i_watched(x).database
            Next
            Return database
        End Get
    End Property

    Public ReadOnly Property getTables(ByVal database As String) As Array
        Get
            If i_watched(0).database = Nothing Then SetWatched()
            Dim y As Integer = Array.IndexOf(getDatabase, database)
            If y <> -1 Then
                Dim tables(UBound(i_watched(y).tables)) As String
                For x As Integer = 0 To UBound(i_watched(y).tables)
                    tables(x) = i_watched(y).tables(x)
                Next
                Return tables
            Else
                Return Nothing
            End If
        End Get
    End Property

    Private Sub populate_objectcheckedlistbox(ByRef clb As CheckedListBox, ByVal object_list As String)

        clb.Items.Clear()

        If Microsoft.VisualBasic.Right(object_list, 1) = "|" Then object_list = Microsoft.VisualBasic.Left(object_list, Len(object_list) - 1)

        Dim item As String()
        Dim list As String() = Split(object_list, "|")

        For Each obj As String In list

            item = Split(obj, "¬")

            clb.Items.Add(item(0), item(1) = "True")

        Next

    End Sub

    Private Sub populate_checkedlistbox(ByRef clb As CheckedListBox, ByVal path As String, ByVal listtype As String, Optional ByVal searchpattern As String = Nothing, Optional ByVal ommit As String = Nothing)

        clb.Items.Clear()

        Try
            If listtype = "db_db_objects" Then

                Dim all_checked As Boolean = (_databases = Nothing)

                For Each objects As String In IO.Directory.GetFiles(path, searchpattern)
                    If IO.Path.GetExtension(objects) <> ommit Or ommit = Nothing Then
                        If all_checked Then
                            clb.Items.Add(IO.Path.GetFileName(objects), True)
                        Else
                            clb.Items.Add(IO.Path.GetFileName(objects), _db_objects.Contains(IO.Path.GetFileName(objects)))
                        End If
                    End If
                Next

            ElseIf listtype = "databases" Then

                Dim all_checked As Boolean = (_databases = Nothing)

                For Each database As String In IO.Directory.GetFiles(path, searchpattern)
                    If IO.Path.GetExtension(database) <> ommit Or ommit = Nothing Then
                        If all_checked Then
                            clb.Items.Add(IO.Path.GetFileName(database), True)
                        Else
                            clb.Items.Add(IO.Path.GetFileName(database), _databases.Contains(IO.Path.GetFileName(database)))
                        End If
                    End If
                Next

            ElseIf listtype = "thumbs" Then

                Dim all_checked As Boolean = (_folders = Nothing)

                For Each folder As String In IO.Directory.GetDirectories(path)
                    If all_checked Then
                        clb.Items.Add(IO.Path.GetFileName(folder), True)
                    Else
                        clb.Items.Add(IO.Path.GetFileName(folder), _folders.Contains(IO.Path.GetFileName(folder)))
                    End If
                Next

            End If
            b_sync_now.Enabled = True
        Catch ex As Exception
            MPSync_process.logStats("MPSync: " & ex.Message, MessageType.ERR)
            b_sync_now.Enabled = False
        End Try

    End Sub

    Private Sub populate_watchedchecklistbox(ByRef clb As CheckedListBox)

        clb.Items.Clear()

        Dim all_checked As Boolean = (_watched_dbs = Nothing)

        For x As Integer = 0 To (clb_databases.CheckedItems.Count - 1)
            Dim y As Integer = Array.IndexOf(getDatabase, clb_databases.GetItemText(clb_databases.CheckedItems(x)))
            If y >= 0 Then
                If all_checked Then
                    clb.Items.Add(i_watched(y).database, True)
                Else
                    clb.Items.Add(i_watched(y).database, _watched_dbs.Contains(i_watched(y).database))
                End If
            End If
        Next

    End Sub

    Public Shared Sub MergeSettings()
        Dim fileName As String = Config.GetFile(Config.Dir.Config, "MPSync_THUMBS.xml")
        If IO.File.Exists(fileName) Then
            Using XMLreader As MediaPortal.Profile.Settings = New MediaPortal.Profile.Settings(fileName),
                  XmlWriter As MediaPortal.Profile.Settings = New MediaPortal.Profile.Settings(GetConfigFileName)


                XmlWriter.SetValue("Thumbs Path", "direction", XMLreader.GetValueAsInt("Path", "direction", 1))
                XmlWriter.SetValue("Thumbs Path", "client", XMLreader.GetValueAsString("Path", "client", Nothing))
                XmlWriter.SetValue("Thumbs Path", "server", XMLreader.GetValueAsString("Path", "server", Nothing))
                XmlWriter.SetValue("Thumbs Path", "method", XMLreader.GetValueAsInt("Path", "method", 0))
                XmlWriter.SetValue("Thumbs Settings", "folders", XMLreader.GetValueAsString("Settings", "folders", Nothing))
                XmlWriter.SetValue("Thumbs Settings", "pause while playing", XMLreader.GetValueAsBool("Settings", "pause while playing", False))
                XmlWriter.SetValue("Thumbs Settings", "use MD5", XMLreader.GetValueAsBool("Settings", "use MD5", False))
                XmlWriter.SetValue("Thumbs Settings", "use CRC32", XMLreader.GetValueAsBool("Settings", "use CRC32", False))
            End Using
            MediaPortal.Profile.Settings.SaveCache()
            IO.File.Delete(fileName)
        End If
    End Sub
    Private Sub getObjectSettings(ByVal objsetting As String)

        Using XMLreader As MediaPortal.Profile.Settings = New MediaPortal.Profile.Settings(MPSync_settings.GetConfigFileName)

            _clicks_folders = XMLreader.GetValueAsInt(objsetting & " Path", "direction", 1)
            tb_folders_client_path.Text = XMLreader.GetValueAsString(objsetting & " Path", "client", Nothing)
            tb_folders_server_path.Text = XMLreader.GetValueAsString(objsetting & " Path", "server", Nothing)
            _folders_sync_method = XMLreader.GetValueAsInt(objsetting & " Path", "method", 0)
            _folders = XMLreader.GetValueAsString(objsetting & " Settings", "folders", Nothing)
            cb_folders_pause.Checked = CBool(XMLreader.GetValueAsString(objsetting & " Settings", "pause while playing", False.ToString()))
            cb_folders_md5.Checked = CBool(XMLreader.GetValueAsString(objsetting & " Settings", "use MD5", False.ToString()))
            cb_folders_crc32.Checked = CBool(XMLreader.GetValueAsString(objsetting & " Settings", "use CRC32", False.ToString()))

        End Using

        b_folders_direction.Image = i_direction(_clicks_folders)
        cb_folders_sync_method.Text = i_method(_folders_sync_method)

        rb_all_folders.Checked = (_folders = Nothing)
        rb_specific_folders.Checked = Not rb_all_folders.Checked

        'populate the respective listboxes

        If _clicks_db <> 2 Then
            If tb_folders_client_path.Text <> Nothing Then populate_checkedlistbox(clb_objects, tb_folders_client_path.Text, "thumbs")
        ElseIf _clicks_db = 2 Then
            If tb_folders_server_path.Text <> Nothing Then populate_checkedlistbox(clb_objects, tb_folders_server_path.Text, "thumbs")
        End If

    End Sub

    Private Sub setObjectSettings(ByVal objsetting As String)

        _folders = Nothing

        If rb_specific_folders.Checked Then
            For x As Integer = 0 To (clb_objects.CheckedItems.Count - 1)
                _folders += clb_objects.GetItemText(clb_objects.CheckedItems(x)) & "|"
            Next
        End If

        Using XMLwriter As MediaPortal.Profile.Settings = New MediaPortal.Profile.Settings(MPSync_settings.GetConfigFileName)

            XMLwriter.SetValue(objsetting & " Path", "client", tb_folders_client_path.Text)
            XMLwriter.SetValue(objsetting & " Path", "server", tb_folders_server_path.Text)
            XMLwriter.SetValue(objsetting & " Path", "direction", _clicks_folders.ToString)
            XMLwriter.SetValue(objsetting & " Path", "method", Array.IndexOf(i_method, cb_folders_sync_method.Text))

            XMLwriter.SetValue(objsetting & " Settings", "pause while playing", cb_folders_pause.Checked)
            XMLwriter.SetValue(objsetting & " Settings", "use MD5", cb_folders_md5.Checked)
            XMLwriter.SetValue(objsetting & " Settings", "use CRC32", cb_folders_crc32.Checked)
            XMLwriter.SetValue(objsetting & " Settings", "folders", _folders)

        End Using

        MediaPortal.Profile.Settings.SaveCache()

    End Sub

    Private Sub getSettings()

        Dim object_list As String = Nothing

        If IO.File.Exists(MPSync_settings.GetConfigFileName) Then

            '  get settings from XML configuration file

            Using XMLreader As MediaPortal.Profile.Settings = New MediaPortal.Profile.Settings(MPSync_settings.GetConfigFileName)

                _version = XMLreader.GetValueAsString("Plugin", "version", "0")
                cb_databases.Checked = XMLreader.GetValueAsBool("Plugin", "databases", True)
                cb_folders.Checked = XMLreader.GetValueAsBool("Plugin", "thumbs", True)
                cb_debug.Checked = XMLreader.GetValueAsBool("Plugin", "debug", False)
                _session = XMLreader.GetValueAsString("Plugin", "session ID", System.Guid.NewGuid.ToString())
                _sync_type = XMLreader.GetValueAsString("Plugin", "sync type", "Triggers")
                max_DB_threads = XMLreader.GetValueAsInt("Plugin", "max DB threads", -1)
                max_folder_threads = XMLreader.GetValueAsInt("Plugin", "max folder threads", -1)

                _databases = XMLreader.GetValueAsString("DB Settings", "databases", Nothing)
                _watched_dbs = XMLreader.GetValueAsString("DB Settings", "watched databases", Nothing)
                _db_objects = XMLreader.GetValueAsString("DB Settings", "objects", Nothing)

                _clicks_db = XMLreader.GetValueAsInt("DB Path", "direction", 1)
                tb_db_client_path.Text = XMLreader.GetValueAsString("DB Path", "client", Nothing)
                tb_db_server_path.Text = XMLreader.GetValueAsString("DB Path", "server", Nothing)
                _db_sync_method = XMLreader.GetValueAsInt("DB Path", "method", 0)
                nud_db_sync.Value = XMLreader.GetValueAsInt("DB Settings", "sync periodicity", 15)
                cb_db_sync.Text = XMLreader.GetValueAsString("DB Settings", "sync periodicity value", "minutes")
                cb_db_pause.Checked = XMLreader.GetValueAsBool("DB Settings", "pause while playing", False)
                cb_watched.Checked = XMLreader.GetValueAsBool("DB Settings", "watched", False)
                cb_vacuum.Checked = XMLreader.GetValueAsBool("DB Settings", "vacuum", False)

                object_list = XMLreader.GetValueAsString("Objects List", "list", "Thumbs¬True|")

            End Using

        End If

        b_db_direction.Image = i_direction(_clicks_db)
        cb_db_sync_method.Text = i_method(_db_sync_method)
        b_folders_direction.Image = i_direction(_clicks_folders)
        cb_folders_sync_method.Text = i_method(_folders_sync_method)

        rb_all_db.Checked = (_databases = Nothing)
        rb_specific_db.Checked = Not rb_all_db.Checked
        rb_w_all.Checked = (_watched_dbs = Nothing)
        rb_w_specific.Checked = Not rb_w_all.Checked
        rb_o_all.Checked = (_db_objects = Nothing)
        rb_o_nothing.Checked = (_db_objects = "NOTHING|")
        rb_o_specific.Checked = Not (rb_o_all.Checked Or rb_o_nothing.Checked)

        rb_triggers.Checked = (_sync_type = "Triggers")
        rb_timestamp.Checked = Not rb_triggers.Checked

        rb_triggers.Enabled = cb_databases.Checked
        rb_timestamp.Enabled = cb_databases.Checked

        'populate the respective listboxes

        If _clicks_db <> 2 Then
            populate_checkedlistbox(clb_databases, tb_db_client_path.Text, "databases", "*.db3", ".db3-journal")
            populate_checkedlistbox(clb_db_objects, tb_db_client_path.Text, "db_db_objects", "*.*", ".db3")
        ElseIf _clicks_db = 2 Then
            populate_checkedlistbox(clb_databases, tb_db_server_path.Text, "databases", "*.db3", ".db3-journal")
            populate_checkedlistbox(clb_db_objects, tb_db_server_path.Text, "db_db_objects", "*.*", ".db3")
        End If

        If max_DB_threads = -1 Then nud_max_DB_streams.Value = 0 Else nud_max_DB_streams.Value = max_DB_threads
        If max_folder_threads = -1 Then nud_max_folder_streams.Value = 0 Else nud_max_folder_streams.Value = max_folder_threads

        populate_watchedchecklistbox(clb_watched)
        populate_objectcheckedlistbox(clb_object_list, object_list)

    End Sub

    Private Sub setSettings()

        _databases = Nothing

        If rb_specific_db.Checked Then
            For x As Integer = 0 To (clb_databases.CheckedItems.Count - 1)
                _databases += clb_databases.GetItemText(clb_databases.CheckedItems(x)) & "|"
            Next
        End If

        _folders = Nothing

        If rb_specific_folders.Checked Then
            For x As Integer = 0 To (clb_objects.CheckedItems.Count - 1)
                _folders += clb_objects.GetItemText(clb_objects.CheckedItems(x)) & "|"
            Next
        End If

        _watched_dbs = Nothing

        If rb_w_specific.Checked Then
            For x As Integer = 0 To (clb_watched.CheckedItems.Count - 1)
                _watched_dbs += clb_watched.GetItemText(clb_watched.CheckedItems(x)) & "|"
            Next
        End If

        _db_objects = Nothing

        If rb_o_all.Checked Then
            For x As Integer = 0 To (clb_db_objects.Items.Count - 1)
                _db_objects += clb_db_objects.GetItemText(clb_db_objects.Items(x)) & "|"
            Next
        ElseIf rb_o_specific.Checked Then
            For x As Integer = 0 To (clb_db_objects.CheckedItems.Count - 1)
                _db_objects += clb_db_objects.GetItemText(clb_db_objects.CheckedItems(x)) & "|"
            Next
        ElseIf rb_o_nothing.Checked Then
            _db_objects = "NOTHING|"
        End If

        If rb_triggers.Checked Then
            _sync_type = "Triggers"
        Else
            _sync_type = "Timestamp"
        End If

        _object_list = Nothing

        For x As Integer = 0 To (clb_object_list.Items.Count - 1)
            _object_list += clb_object_list.GetItemText(clb_object_list.Items(x)) & "¬" & clb_object_list.GetItemChecked(x) & "|"
        Next

        Using XMLwriter As MediaPortal.Profile.Settings = New MediaPortal.Profile.Settings(MPSync_settings.GetConfigFileName)

            XMLwriter.SetValue("Plugin", "version", _curversion)
            XMLwriter.SetValueAsBool("Plugin", "databases", cb_databases.Checked)
            XMLwriter.SetValueAsBool("Plugin", "thumbs", cb_folders.Checked)
            XMLwriter.SetValueAsBool("Plugin", "debug", cb_debug.Checked)
            XMLwriter.SetValue("Plugin", "session ID", _session)
            XMLwriter.SetValue("Plugin", "sync type", _sync_type)
            XMLwriter.SetValue("Plugin", "max DB threads", max_DB_threads)
            XMLwriter.SetValue("Plugin", "max folder threads", max_folder_threads)

            XMLwriter.SetValue("DB Path", "client", tb_db_client_path.Text)
            XMLwriter.SetValue("DB Path", "server", tb_db_server_path.Text)
            XMLwriter.SetValue("DB Path", "direction", _clicks_db.ToString)
            XMLwriter.SetValue("DB Path", "method", Array.IndexOf(i_method, cb_db_sync_method.Text))

            XMLwriter.SetValue("DB Settings", "sync periodicity", nud_db_sync.Value)
            XMLwriter.SetValue("DB Settings", "sync periodicity value", cb_db_sync.Text)
            XMLwriter.SetValueAsBool("DB Settings", "pause while playing", cb_db_pause.Checked)
            XMLwriter.SetValue("DB Settings", "databases", _databases)
            XMLwriter.SetValueAsBool("DB Settings", "watched", cb_watched.Checked)
            XMLwriter.SetValueAsBool("DB Settings", "vacuum", cb_vacuum.Checked)
            XMLwriter.SetValue("DB Settings", "watched databases", _watched_dbs)
            XMLwriter.SetValue("DB Settings", "objects", _db_objects)

            XMLwriter.SetValue("Objects List", "list", _object_list)

        End Using

        MediaPortal.Profile.Settings.SaveCache()

    End Sub

    Private Sub cb_databases_CheckedChanged(sender As Object, e As EventArgs) Handles cb_databases.CheckedChanged
        If cb_databases.Checked Then tc_main.TabPages.Insert(1, tp_database) Else tc_main.TabPages.Remove(tp_database)
        rb_triggers.Enabled = cb_databases.Checked
        rb_timestamp.Enabled = cb_databases.Checked
    End Sub

    Private Sub cb_folders_CheckedChanged(sender As Object, e As EventArgs) Handles cb_folders.CheckedChanged
        If cb_folders.Checked Then tc_main.TabPages.Add(tp_folders) Else tc_main.TabPages.Remove(tp_folders)
    End Sub

    Private Sub cb_watched_CheckedChanged(sender As Object, e As EventArgs) Handles cb_watched.CheckedChanged

        If cb_watched.Checked Then
            tc_database.TabPages.Add(tp_watched)
            populate_watchedchecklistbox(clb_watched)
        Else
            tc_database.TabPages.Remove(tp_watched)
        End If

    End Sub

    Private Sub b_browse_Click(sender As Object, e As EventArgs) Handles b_db_client.Click, b_folders_client.Click, b_db_server.Click, b_folders_server.Click

        Dim objShell As Object
        Dim objFolder As Object
        Dim msg As String = Nothing

        Try

            If DirectCast(sender, System.Windows.Forms.Button).Name = "b_db_client" Then
                msg = "Please select folder containing source databases"
            ElseIf DirectCast(sender, System.Windows.Forms.Button).Name = "b_folders_client" Then
                msg = "Please select folder containing source objects"
            ElseIf DirectCast(sender, System.Windows.Forms.Button).Name = "b_db_server" Then
                msg = "Please select folder containing target databases"
            ElseIf DirectCast(sender, System.Windows.Forms.Button).Name = "b_folders_server" Then
                msg = "Please select folder containing target objects"
            End If

            objShell = CreateObject("Shell.Application")
            objFolder = objShell.BrowseForFolder(0, msg, 0)

            If DirectCast(sender, System.Windows.Forms.Button).Name = "b_db_client" Then
                If IsError(objFolder.Items.Item.Path) Then
                    tb_db_client_path.Text = CStr(objFolder)
                Else
                    tb_db_client_path.Text = objFolder.Items.Item.Path
                End If
                tb_db_client_path.Focus()
            ElseIf DirectCast(sender, System.Windows.Forms.Button).Name = "b_folders_client" Then
                If IsError(objFolder.Items.Item.Path) Then
                    tb_folders_client_path.Text = CStr(objFolder)
                Else
                    tb_folders_client_path.Text = objFolder.Items.Item.Path
                End If
                tb_folders_client_path.Focus()
            ElseIf DirectCast(sender, System.Windows.Forms.Button).Name = "b_db_server" Then
                If IsError(objFolder.Items.Item.Path) Then
                    tb_db_server_path.Text = CStr(objFolder)
                Else
                    tb_db_server_path.Text = objFolder.Items.Item.Path
                End If
                tb_db_server_path.Focus()
            ElseIf DirectCast(sender, System.Windows.Forms.Button).Name = "b_folders_server" Then
                If IsError(objFolder.Items.Item.Path) Then
                    tb_folders_server_path.Text = CStr(objFolder)
                Else
                    tb_folders_server_path.Text = objFolder.Items.Item.Path
                End If
                tb_folders_server_path.Focus()
            End If

        Catch ex As Exception
        End Try

    End Sub

    Private Sub b_direction_Click(sender As Object, e As EventArgs) Handles b_db_direction.Click, b_folders_direction.Click

        If DirectCast(sender, System.Windows.Forms.Button).Name = "b_db_direction" Then

            _clicks_db += 1

            If _clicks_db > 2 Then _clicks_db = 1

            If _clicks_db = 0 Then
                cb_db_sync_method.Items.RemoveAt(0)
            Else
                cb_db_sync_method.Items.Insert(0, i_method(0))
            End If

            cb_db_sync_method.Refresh()

            b_db_direction.Image = i_direction(_clicks_db)
            b_db_direction.Refresh()

            If _clicks_db <> 2 Then
                populate_checkedlistbox(clb_databases, tb_db_client_path.Text, "*.db3")
            ElseIf _clicks_db = 2 Then
                populate_checkedlistbox(clb_databases, tb_db_server_path.Text, "*.db3")
            End If

        ElseIf DirectCast(sender, System.Windows.Forms.Button).Name = "b_folders_direction" Then

            _clicks_folders += 1

            If _clicks_folders > 2 Then _clicks_folders = 1

            If _clicks_folders = 0 Then
                cb_folders_sync_method.Items.RemoveAt(0)
            Else
                cb_folders_sync_method.Items.Insert(0, i_method(0))
            End If

            cb_folders_sync_method.Refresh()

            b_folders_direction.Image = i_direction(_clicks_folders)
            b_folders_direction.Refresh()

            If _clicks_folders <> 2 Then
                populate_checkedlistbox(clb_objects, tb_folders_client_path.Text, "thumbs")
            ElseIf _clicks_folders = 2 Then
                populate_checkedlistbox(clb_objects, tb_folders_server_path.Text, "thumbs")
            End If

        End If

    End Sub

    Private Sub b_save_Click(sender As Object, e As EventArgs) Handles b_save.Click

        setSettings()

        MsgBox("Configuration Saved", MsgBoxStyle.Information, "Central DB Synchronise")

    End Sub

    Private Sub tb_path_Leave(sender As Object, e As EventArgs) Handles tb_db_client_path.Leave, tb_db_server_path.Leave, tb_folders_client_path.Leave, tb_folders_server_path.Leave

        If Not IO.Directory.Exists(DirectCast(sender, System.Windows.Forms.TextBox).Text) Then
            MsgBox("Path not found!", MsgBoxStyle.Exclamation)
            DirectCast(sender, System.Windows.Forms.TextBox).Undo()
        Else
            If DirectCast(sender, System.Windows.Forms.TextBox).Name = "tb_db_client_path" And _clicks_db <> 2 Then
                populate_checkedlistbox(clb_databases, tb_db_client_path.Text, "databases", "*.db3")
                populate_checkedlistbox(clb_db_objects, tb_db_client_path.Text, "db_db_objects", "*.*", ".db3")
                populate_watchedchecklistbox(clb_watched)
            ElseIf DirectCast(sender, System.Windows.Forms.TextBox).Name = "tb_db_server_path" And _clicks_db = 2 Then
                populate_checkedlistbox(clb_databases, tb_db_server_path.Text, "databases", "*.db3")
                populate_checkedlistbox(clb_db_objects, tb_db_server_path.Text, "db_db_objects", "*.*", ".db3")
                populate_watchedchecklistbox(clb_watched)
            ElseIf DirectCast(sender, System.Windows.Forms.TextBox).Name = "tb_folders_client_path" And _clicks_folders <> 2 Then
                populate_checkedlistbox(clb_objects, tb_folders_client_path.Text, "thumbs")
            ElseIf DirectCast(sender, System.Windows.Forms.TextBox).Name = "tb_folders_server_path" And _clicks_folders = 2 Then
                populate_checkedlistbox(clb_objects, tb_folders_server_path.Text, "thumbs")
            End If
        End If

    End Sub

    Private Sub rb_all_db_CheckedChanged(sender As Object, e As EventArgs) Handles rb_all_db.CheckedChanged
        clb_databases.Enabled = Not rb_all_db.Checked
        populate_watchedchecklistbox(clb_watched)
    End Sub

    Private Sub rb_specific_db_CheckedChanged(sender As Object, e As EventArgs) Handles rb_specific_db.CheckedChanged
        clb_databases.Enabled = rb_specific_db.Checked
        populate_watchedchecklistbox(clb_watched)
    End Sub

    Private Sub rb_all_folders_CheckedChanged(sender As Object, e As EventArgs) Handles rb_all_folders.CheckedChanged
        clb_objects.Enabled = Not rb_all_folders.Checked
    End Sub

    Private Sub rb_specific_folders_CheckedChanged(sender As Object, e As EventArgs) Handles rb_specific_folders.CheckedChanged
        clb_objects.Enabled = rb_specific_folders.Checked
    End Sub

    Private Sub tb_object_list_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs)

        If e.KeyChar = Chr(13) Then
            SendKeys.Send(vbTab)
        End If

    End Sub

    Private Sub b_add_Click(sender As Object, e As EventArgs) Handles b_add.Click

        Dim s As String = InputBox("Name:", "Add folder item")
        If s <> "" Then
            clb_object_list.Items.Add(s, False)
            clb_object_list.TopIndex = clb_object_list.Items.Count - 1
        End If

    End Sub

    Private Sub b_delete_Click(sender As Object, e As EventArgs) Handles b_delete.Click

        If MsgBox("Are you sure?", MsgBoxStyle.YesNo, "Delete") = MsgBoxResult.Yes Then

            clb_object_list.Items.Remove(clb_object_list.SelectedItem)

            clb_object_list.SelectedItem = Nothing
            b_delete.Enabled = False
            b_edit.Enabled = False

            tc_objects.TabPages.Remove(tp_paths)
            tc_objects.TabPages.Remove(tp_advancedsettings)

        End If

    End Sub

    Private Sub b_edit_Click(sender As Object, e As EventArgs) Handles b_edit.Click

        getObjectSettings(clb_object_list.SelectedItem.ToString)

        If Not tc_objects.TabPages.Contains(tp_paths) Then tc_objects.TabPages.Add(tp_paths)
        If Not tc_objects.TabPages.Contains(tp_advancedsettings) Then tc_objects.TabPages.Add(tp_advancedsettings)

        tc_objects.SelectedTab = tp_paths

    End Sub

    Private Sub b_apply_Click(sender As Object, e As EventArgs) Handles b_apply.Click

        If tb_folders_client_path.Text <> Nothing And tb_folders_server_path.Text <> Nothing Then
            setObjectSettings(clb_object_list.SelectedItem.ToString)

            tc_objects.TabPages.Remove(tp_paths)
            tc_objects.TabPages.Remove(tp_advancedsettings)

            tb_folders_client_path.Text = Nothing
            tb_folders_server_path.Text = Nothing

            MsgBox("Configuration of " & clb_object_list.SelectedItem.ToString & " stored.", MsgBoxStyle.Information)
        End If

    End Sub

    Private Sub clb_object_list_SelectedIndexChanged(sender As Object, e As EventArgs) Handles clb_object_list.SelectedIndexChanged

        If clb_object_list.SelectedItem IsNot Nothing Then
            b_delete.Enabled = True
            b_edit.Enabled = True
        End If

    End Sub

    Private Sub clb_databases_SelectedValueChanged(sender As Object, e As EventArgs) Handles clb_databases.SelectedValueChanged
        populate_watchedchecklistbox(clb_watched)
    End Sub

    Private Sub rb_w_all_CheckedChanged(sender As Object, e As EventArgs) Handles rb_w_all.CheckedChanged
        clb_watched.Enabled = Not rb_w_all.Checked
    End Sub

    Private Sub rb_o_all_CheckedChanged(sender As Object, e As EventArgs) Handles rb_o_all.CheckedChanged
        clb_db_objects.Enabled = Not rb_o_all.Checked
    End Sub

    Private Sub rb_o_specific_CheckedChanged(sender As Object, e As EventArgs) Handles rb_o_specific.CheckedChanged
        clb_db_objects.Enabled = rb_o_specific.Checked
    End Sub

    Private Sub rb_o_nothing_CheckedChanged(sender As Object, e As EventArgs) Handles rb_o_nothing.CheckedChanged
        clb_db_objects.Enabled = Not rb_o_nothing.Checked
    End Sub

    Private Sub rb_w_specific_CheckedChanged(sender As Object, e As EventArgs) Handles rb_w_specific.CheckedChanged
        clb_watched.Enabled = rb_w_specific.Checked
    End Sub

    Private Sub cb_folders_md5_CheckedChanged(sender As Object, e As EventArgs) Handles cb_folders_md5.CheckedChanged
        If cb_folders_md5.Checked Then cb_folders_crc32.Checked = False
    End Sub

    Private Sub cb_folders_crc32_CheckedChanged(sender As Object, e As EventArgs) Handles cb_folders_crc32.CheckedChanged
        If cb_folders_crc32.Checked Then cb_folders_md5.Checked = False
    End Sub

    Private Sub nud_max_streams_ValueChanged(sender As Object, e As EventArgs) Handles nud_max_DB_streams.ValueChanged
        If nud_max_DB_streams.Value = 0 Then max_DB_threads = -1 Else max_DB_threads = nud_max_DB_streams.Value
    End Sub

    Private Sub nud_max_folder_streams_ValueChanged(sender As Object, e As EventArgs) Handles nud_max_folder_streams.ValueChanged
        If nud_max_folder_streams.Value = 0 Then max_folder_threads = -1 Else max_folder_threads = nud_max_folder_streams.Value
    End Sub

    Private Sub b_sync_now_Click(sender As Object, e As EventArgs) Handles b_sync_now.Click

        setSettings()

        syncnow = True

        SyncNowStatus()

        Dim mps As New MPSync_process
        mps.MPSync_Launch()

    End Sub

    Private Sub SyncNowStatus()

        If tc_main.TabPages.Contains(tp_syncnow) Then tc_main.TabPages.Remove(tp_syncnow)
        tc_main.TabPages.Add(tp_syncnow)
        tc_main.SelectedTab = tp_syncnow
        lb_status.Items.Clear()
        lb_status.Items.Add("Synchronization started.")

        db_complete = Not cb_databases.Checked
        folders_complete = Not cb_folders.Checked

        b_save.Enabled = False
        b_sync_now.Enabled = False

        'rename old log file
        If IO.File.Exists(Config.GetFile(Config.Dir.Log, "mpsync.bak")) Then IO.File.Delete(Config.GetFile(Config.Dir.Log, "mpsync.bak"))
        If IO.File.Exists(Config.GetFile(Config.Dir.Log, "mpsync.log")) Then FileIO.FileSystem.RenameFile(Config.GetFile(Config.Dir.Log, "mpsync.log"), "mpsync.bak")

        'initialize timer
        AddHandler lb_status_timer.Elapsed, AddressOf lb_status_timer_update
        lb_status_timer.Interval = 500
        lb_status_timer.Enabled = True
        lb_status_timer.Start()

    End Sub

    Private Sub lb_status_timer_update()

        CheckForIllegalCrossThreadCalls = False

        If Not IO.File.Exists(Config.GetFile(Config.Dir.Log, "mpsync.log")) Then Exit Sub

        Try
            ' read log file
            Dim file As String = Config.GetFile(Config.Dir.Log, "mpsync.log")
            Dim lines() As String = IO.File.ReadAllLines(file)
            Dim status As String

            For Each status In lines
                If lb_status.Items.Contains(status) = False Then
                    lb_status.Items.Add(status)
                    lb_status.TopIndex = lb_status.Items.Count - 1
                    lb_status.Refresh()
                End If
            Next
        Catch ex As Exception
        End Try

        If db_complete And folders_complete Then
            lb_status_timer.Stop()
            If lb_status.Items.Contains("Synchronization complete.") = False Then
                lb_status.Items.Add("Synchronization complete.")
                MsgBox("Synchronization complete", MsgBoxStyle.Information)
            End If
            b_save.Enabled = True
            b_sync_now.Enabled = True
        End If

    End Sub

    Private Sub MPSync_settings_FormClosed(sender As Object, e As EventArgs) Handles MyBase.FormClosed

        Using XMLwriter As MediaPortal.Profile.Settings = New MediaPortal.Profile.Settings(MPSync_settings.GetConfigFileName)
            XMLwriter.SetValue("Plugin", "version", _curversion)
        End Using

        MediaPortal.Profile.Settings.SaveCache()

        Return

    End Sub

    Private Sub MPSync_settings_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' initialize version
        Me.Text = Me.Text & _curversion

        ' initialize direction images
        i_direction(0) = My.Resources.sync_both
        i_direction(1) = My.Resources.sync_master2slave
        i_direction(2) = My.Resources.sync_slave2master

        ' temporarily remove tabs to add according to settings/activity
        tc_main.TabPages.Remove(tp_database)
        tc_main.TabPages.Remove(tp_folders)
        tc_main.TabPages.Remove(tp_syncnow)
        tc_database.TabPages.Remove(tp_watched)

        tc_objects.TabPages.Remove(tp_paths)
        tc_objects.TabPages.Remove(tp_advancedsettings)

        ' initialize methods
        i_method = {"Propagate both additions and deletions", "Propagate additions only", "Propagate deletions only"}

        For x = 0 To 2
            cb_db_sync_method.Items.Add(i_method(x))
            cb_folders_sync_method.Items.Add(i_method(x))
        Next

        getSettings()

    End Sub

End Class
