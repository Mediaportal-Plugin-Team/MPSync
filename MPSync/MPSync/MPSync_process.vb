Imports MediaPortal.Configuration
Imports MediaPortal.GUI.Library

Imports System.ComponentModel
Imports System.Threading
Imports System.Data.SQLite

Public Class MPSync_process

    Private mps As New MPSync_settings
    Private Shared _folders_client, _folders_server, _folders_pause, _folders_md5, _folders_crc32, _folders(), folders, object_list As String
    Private Shared bw_dbs As New ArrayList
    Private bw_threads As Integer
    Public Shared _db_client, _db_server, _databases(), _watched_dbs(), _db_objects(), dbname(), session, sync_type As String
    Public Shared _db_sync, _db_direction, _db_sync_method, _folders_direction, _folders_sync_method As Integer
    Public Shared _db_pause, debug, check_watched, _vacuum As Boolean
    Public Shared dbinfo() As IO.FileInfo

    Dim checked_databases, checked_folders As Boolean

    Private Structure fieldnames
        Dim names As Array
    End Structure

    Public Shared Property checkThreads(ByVal type As String) As Integer
        Set(ByVal value As Integer)
            If type = "DB" Then MPSync_settings.max_DB_threads = value Else MPSync_settings.max_folder_threads = value
        End Set
        Get
            If type = "DB" Then Return MPSync_settings.max_DB_threads Else Return MPSync_settings.max_folder_threads
        End Get
    End Property

    Public Shared ReadOnly Property p_object_list As Array
        Get
            Dim list As Array = Split(object_list, "|")
            Return list
        End Get
    End Property

    Public Shared Property p_Debug As Boolean
        Get
            Dim debug As Boolean = False
            Try
                Using XMLreader As MediaPortal.Profile.Settings = New MediaPortal.Profile.Settings(MPSync_settings.GetConfigFileName)
                    debug = XMLreader.GetValueAsBool("Plugin", "debug", False)
                End Using
            Catch ex As Exception
                MPSync_process.logStats("MPSync: Error reading 'debug' value from XML with exception " & ex.Message, "ERROR")
            End Try
            Return debug
        End Get
        Set(value As Boolean)
            debug = value
        End Set
    End Property

    Public Shared ReadOnly Property p_Database(ByVal database As String) As String
        Get
            Return (database).Replace("\", "/")
        End Get
    End Property

    Public Shared Sub wait(ByVal seconds As Integer, Optional ByVal verbose As Boolean = True, Optional ByVal thread As String = Nothing)

        If thread <> Nothing Then thread = thread & " "
        If verbose Then Log.Info("MPSync: " & thread & "process sleeping " & seconds.ToString & " seconds.....")

        System.Threading.Thread.Sleep(seconds * 1000)

    End Sub

    Public Shared Function CheckPlayerplaying(ByVal thread As String) As Boolean
        Return ((thread = "db" And _db_pause) Or thread = "folders") And MediaPortal.Player.g_Player.Playing
    End Function

    Public Shared Sub logStats(ByVal message As String, ByVal msgtype As String)

        If Not p_Debug And msgtype = "DEBUG" Then Exit Sub

        Select Case msgtype
            Case "INFO"
                Log.Info(message)
            Case "ERROR"
                Log.Error(message)
        End Select

        Dim file As String = Config.GetFile(Config.Dir.Log, "mpsync.log")
        Dim fhandle As System.IO.FileStream

        If msgtype = "LOG" Then msgtype = "INFO"

        msgtype = Left(msgtype & "  ", 5)

        Dim info() As Byte = New System.Text.UTF8Encoding(True).GetBytes(DateTime.Now & " - [" & msgtype & "] " & message & vbCrLf)

        Try
            fhandle = IO.File.Open(file, IO.FileMode.Append, IO.FileAccess.Write, IO.FileShare.ReadWrite)
            fhandle.Write(info, 0, info.Length)
            fhandle.Close()
        Catch ex As Exception
        End Try

    End Sub

    Public Shared Sub getObjectSettings(ByVal objsetting As String, Optional ByRef folders_client As String = Nothing, Optional ByRef folders_server As String = Nothing, Optional ByRef folders_direction As Integer = Nothing, Optional ByRef folders_sync_method As Integer = Nothing, Optional ByRef selectedfolders() As String = Nothing, Optional ByRef folders_pause As Boolean = False, Optional ByRef folders_md5 As Boolean = False, Optional ByRef folders_crc32 As Boolean = False)

        folders = Nothing

        Try
            Using XMLreader As MediaPortal.Profile.Settings = New MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MPSync_" & UCase(objsetting) & ".xml"))

                folders_client = XMLreader.GetValueAsString("Path", "client", Nothing)
                folders_server = XMLreader.GetValueAsString("Path", "server", Nothing)
                folders_direction = XMLreader.GetValueAsInt("Path", "direction", 0)
                folders_sync_method = XMLreader.GetValueAsInt("Path", "method", 0)

                folders = XMLreader.GetValueAsString("Settings", "folders", Nothing)
                folders_pause = CBool(XMLreader.GetValueAsString("Settings", "pause while playing", False.ToString()))
                folders_md5 = CBool(XMLreader.GetValueAsString("Settings", "use MD5", False.ToString()))
                folders_crc32 = CBool(XMLreader.GetValueAsString("Settings", "use CRC32", False.ToString()))

            End Using

            ' check that both paths end with a "\"
            If InStrRev(folders_client, "\") <> Len(folders_client) Then folders_client = Trim(folders_client) & "\"
            If InStrRev(folders_server, "\") <> Len(folders_server) Then folders_server = Trim(folders_server) & "\"

            ' get list of folders to synchronise
            If folders <> Nothing Then
                If Right(folders, 1) = "|" Then folders = Left(folders, Len(folders) - 1)
                selectedfolders = Split(folders, "|")
            Else
                ReDim selectedfolders(0)
                selectedfolders(0) = "ALL"
            End If

            _folders_client = folders_client
            _folders_server = folders_server
            _folders_direction = folders_direction
            _folders_sync_method = folders_sync_method
            _folders = selectedfolders
            _folders_pause = folders_pause.ToString
            _folders_md5 = folders_md5.ToString
            _folders_crc32 = folders_crc32.ToString
        Catch ex As Exception
            MPSync_process.logStats("MPSync: Error reading MPSync_" & UCase(objsetting) & ".xml with exception " & ex.Message, "ERROR")
        End Try

    End Sub

    Public Sub MPSync_Launch()

        ' create log file
        Dim file As String = Config.GetFile(Config.Dir.Log, "mpsync.log")

        If IO.File.Exists(Config.GetFile(Config.Dir.Log, "mpsync.bak")) Then IO.File.Delete(Config.GetFile(Config.Dir.Log, "mpsync.bak"))
        If IO.File.Exists(file) Then FileIO.FileSystem.RenameFile(file, "mpsync.bak")
        Dim fhandle As System.IO.FileStream = IO.File.Open(file, IO.FileMode.OpenOrCreate)
        fhandle.Close()

        Dim workerThread As Thread

        workerThread = New Thread(AddressOf MPSyncProcess)
        workerThread.Start()

    End Sub

    Private Sub checkPath(ByVal path As String)

        Do While Not IO.Directory.Exists(path)
            logStats("MPSync: path " & path & " not available.", "LOG")
            wait(30, False)
        Loop

    End Sub

    Private Sub MPSyncProcess()

        Dim file As String = MPSync_settings.GetConfigFileName

        If Not FileIO.FileSystem.FileExists(file) Then Return

        Dim i_method As String() = {"Propagate both additions and deletions", "Propagate additions only", "Propagate deletions only"}
        Dim db_sync_value, databases, watched_dbs, objects, version, lastsync As String

        ' get configuratin from XML file
        Using XMLreader As MediaPortal.Profile.Settings = New MediaPortal.Profile.Settings(file)

            version = XMLreader.GetValueAsString("Plugin", "version", "0")
            checked_databases = XMLreader.GetValueAsBool("Plugin", "databases", True)
            checked_folders = XMLreader.GetValueAsBool("Plugin", "thumbs", True)
            p_Debug = XMLreader.GetValueAsBool("Plugin", "debug", False)
            session = XMLreader.GetValueAsString("Plugin", "session ID", Nothing)
            sync_type = XMLreader.GetValueAsString("Plugin", "sync type", "Triggers")
            lastsync = XMLreader.GetValueAsString("Plugin", "last sync", "0001-01-01 00:00:00")
            checkThreads("DB") = XMLreader.GetValueAsInt("Plugin", "max DB threads", -1)
            checkThreads("folder") = XMLreader.GetValueAsInt("Plugin", "max folder threads", -1)
            check_watched = XMLreader.GetValueAsBool("DB Settings", "watched", False)

            _db_client = XMLreader.GetValueAsString("DB Path", "client", Nothing)
            _db_server = XMLreader.GetValueAsString("DB Path", "server", Nothing)
            _db_direction = XMLreader.GetValueAsInt("DB Path", "direction", 0)
            _db_sync_method = XMLreader.GetValueAsInt("DB Path", "method", 0)

            _db_sync = XMLreader.GetValueAsInt("DB Settings", "sync periodicity", 15)
            db_sync_value = XMLreader.GetValueAsString("DB Settings", "sync periodicity value", "minutes")
            _db_pause = XMLreader.GetValueAsBool("DB Settings", "pause while playing", False)
            databases = XMLreader.GetValueAsString("DB Settings", "databases", Nothing)
            watched_dbs = XMLreader.GetValueAsString("DB Settings", "watched databases", Nothing)
            _vacuum = XMLreader.GetValueAsBool("DB Settings", "vacuum", False)
            objects = XMLreader.GetValueAsString("DB Settings", "objects", Nothing)

            object_list = XMLreader.GetValueAsString("Objects List", "list", "folders¬True|")

        End Using

        If p_Debug Then
            logStats("MPSync: process plugin version " & version & " initialisation with DEBUG.", "INFO")
        Else
            logStats("MPSync: process plugin version " & version & " initialisation.", "INFO")
        End If

        If session = Nothing Then
            session = System.Guid.NewGuid.ToString()
            Using XMLwriter As MediaPortal.Profile.Settings = New MediaPortal.Profile.Settings(MPSync_settings.GetConfigFileName)
                XMLwriter.SetValue("Plugin", "session ID", session)
            End Using
        End If

        ' write settings to log file
        If checked_databases Then
            Select Case _db_direction
                Case 0
                    logStats("MPSync: DB - " & _db_client & " <-> " & _db_server, "INFO")
                Case 1
                    logStats("MPSync: DB - " & _db_client & " --> " & _db_server, "INFO")
                Case 2
                    logStats("MPSync: DB - " & _db_client & " <-- " & _db_server, "INFO")
            End Select
            logStats("MPSync: DB synchronization method - " & i_method(_db_sync_method), "INFO")
            If databases = Nothing Then
                logStats("MPSync: DBs selected - ALL", "INFO")
            Else
                logStats("MPSync: DBs selected - " & databases, "INFO")
            End If
            If check_watched Then
                If watched_dbs = Nothing Then
                    logStats("MPSync: watched/resume selected for ALL", "INFO")
                Else
                    logStats("MPSync: watched/resume selected for " & watched_dbs, "INFO")
                End If
            Else
                logStats("MPSync: watched/resume not selected", "INFO")
            End If
            If objects = Nothing Then
                logStats("MPSync: Objects selected - ALL", "INFO")
            Else
                logStats("MPSync: Objects selected - " & objects, "INFO")
            End If
        End If

        If checked_folders Then

            If Right(object_list, 1) = "|" Then object_list = Left(object_list, Len(object_list) - 1)

            Dim item As Array
            Dim list As Array = Split(object_list, "|")

            For Each obj As String In list

                item = Split(obj, "¬")

                If item(1) = "True" Then

                    item(0) = UCase(item(0))

                    getObjectSettings(item(0))

                    Select Case _folders_direction
                        Case 0
                            logStats("MPSync: " & item(0) & " - " & _folders_client & " <-> " & _folders_server, "INFO")
                        Case 1
                            logStats("MPSync: " & item(0) & " - " & _folders_client & " --> " & _folders_server, "INFO")
                        Case 2
                            logStats("MPSync: " & item(0) & " - " & _folders_client & " <-- " & _folders_server, "INFO")
                    End Select

                    If _folders_md5 = "True" Then
                        logStats("MPSync: " & item(0) & " synchronization method - " & i_method(_folders_sync_method) & ", pause while playing - " & _folders_pause & ", use MD5 - " & _folders_md5, "INFO")
                    ElseIf _folders_crc32 = "True" Then
                        logStats("MPSync: " & item(0) & " synchronization method - " & i_method(_folders_sync_method) & ", pause while playing - " & _folders_pause & ", use CRC32 - " & _folders_crc32, "INFO")
                    Else
                        logStats("MPSync: " & item(0) & " synchronization method - " & i_method(_folders_sync_method) & ", pause while playing - " & _folders_pause, "INFO")
                    End If

                    If folders = Nothing Then
                        logStats("MPSync: " & item(0) & " selected - ALL", "INFO")
                    Else
                        logStats("MPSync: " & item(0) & " selected - " & folders, "INFO")
                    End If

                End If

            Next

        End If

        If checkThreads("DB") = -1 Then
            logStats("MPSync: Maximum DB threads - No limit", "INFO")
        Else
            logStats("MPSync: Maximum DB threads - [" & checkThreads("DB").ToString & "]", "INFO")
        End If

        If checkThreads("folder") = -1 Then
            logStats("MPSync: Maximum Folder threads - No limit", "INFO")
        Else
            logStats("MPSync: Maximum Folder threads - [" & checkThreads("folder").ToString & "]", "INFO")
        End If

        If _db_client <> Nothing And _db_server <> Nothing And checked_databases Then

            ' get list of databases to synchronise
            If databases <> Nothing Then
                If Right(databases, 1) = "|" Then databases = Left(databases, Len(databases) - 1)
                _databases = Split(databases, "|")
            Else
                ReDim _databases(0)
                _databases(0) = "ALL"
            End If

            ' get list of watched databases to synchronise
            If watched_dbs <> Nothing Then
                If Right(watched_dbs, 1) = "|" Then watched_dbs = Left(watched_dbs, Len(watched_dbs) - 1)
                _watched_dbs = Split(watched_dbs, "|")
            Else
                _watched_dbs = mps.getDatabase
            End If

            ' check that all watch databases are actually selected for synchronization
            If _databases(0) <> "ALL" Then
                For x As Integer = 0 To UBound(_watched_dbs)
                    If Not _databases.Contains(_watched_dbs(x)) Then
                        _watched_dbs(x) = ""
                    End If
                Next
            End If

            ' get list of objects to copy
            If objects <> Nothing Then
                If Right(objects, 1) = "|" Then objects = Left(objects, Len(objects) - 1)
                _db_objects = Split(objects, "|")
            Else
                ReDim _db_objects(0)
                _db_objects(0) = "ALL"
            End If

            ' calculate actual sync periodicity in seconds
            If db_sync_value = "minutes" Then _db_sync = _db_sync * 60
            If db_sync_value = "hours" Then _db_sync = _db_sync * 3600

            ' check that both paths end with a "\"
            If InStrRev(_db_client, "\") <> Len(_db_client) Then _db_client = Trim(_db_client) & "\"
            If InStrRev(_db_server, "\") <> Len(_db_server) Then _db_server = Trim(_db_server) & "\"

            ' check that server is available
            Dim checkDBPath_Thread As Thread
            checkDBPath_Thread = New Thread(AddressOf checkPath)

            logStats("MPSync: Checking availability of " & _db_server, "LOG")

            checkDBPath_Thread.Start(_db_server)

            Do While checkDBPath_Thread.IsAlive
                wait(5, False)
            Loop

            logStats("MPSync: " & _db_server & " is available.", "LOG")

            ' execute only once in a day
            If DateDiff(DateInterval.Day, CDate(lastsync), Now) > 0 Then
                ' get db info and check integrity
                If _db_direction = 1 Then
                    getDBInfo(_db_client, _db_server)
                ElseIf _db_direction = 2 Then
                    getDBInfo(_db_server, _db_client)
                End If

                ' create the required work files and triggers to handle watched status & synchronization
                If _db_direction = 1 Then
                    checkTriggers("client>server")
                ElseIf _db_direction = 2 Then
                    checkTriggers("server>client")
                End If
            End If

        End If

        If checked_folders Then

            If Right(object_list, 1) = "|" Then object_list = Left(object_list, Len(object_list) - 1)

            Dim item As Array
            Dim list As Array = Split(object_list, "|")

            For Each obj As String In list

                item = Split(obj, "¬")

                If item(1) = "True" Then

                    getObjectSettings(item(0))

                    If _folders_client <> Nothing And _folders_server <> Nothing Then

                        ' check that server is available
                        Dim checkOBJPath_Thread As Thread
                        checkOBJPath_Thread = New Thread(AddressOf checkPath)

                        logStats("MPSync: Checking availability of " & _folders_server, "LOG")

                        checkOBJPath_Thread.Start(_folders_server)

                        Do While checkOBJPath_Thread.IsAlive
                            wait(5, False)
                        Loop

                        logStats("MPSync: " & _folders_server & " is available.", "LOG")

                    End If

                End If

            Next

        End If

        If Not MPSync_settings.syncnow Then

            logStats("MPSync: Immediate Synchronization started", "DEBUG")

            Dim autoEvent As New AutoResetEvent(False)

            ThreadPool.QueueUserWorkItem(AddressOf WorkMethod, autoEvent)

            autoEvent.WaitOne()

        Else
            WorkMethod(Nothing)
        End If

    End Sub

    Public Shared Function TableExist(ByVal path As String, ByVal database As String, ByVal table As String, Optional ByVal dropifempty As Boolean = False) As Boolean

        If debug Then MPSync_process.logStats("MPSync: [TableExist] Check if table " & table & " in database " & path & database & " exists.", "DEBUG")

        Try

            Dim exist As Boolean = False

            Using SQLconnect As New SQLiteConnection(),
            SQLcommand As SQLiteCommand = SQLconnect.CreateCommand

                SQLconnect.ConnectionString = "Data Source=" & p_Database(path & database)

                If Not dropifempty Then SQLconnect.ConnectionString += ";Read Only=True;"

                SQLconnect.Open()
                SQLcommand.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='" & table & "'"
                Using SQLreader = SQLcommand.ExecuteReader()

                    While SQLreader.Read()
                        exist = True
                    End While

                End Using

                If exist And dropifempty Then
                    SQLcommand.CommandText = "SELECT COUNT(*) FROM " & table
                    Using SQLreader = SQLcommand.ExecuteReader()
                        SQLreader.Read()

                        If Int(SQLreader(0)) = 0 Then
                            SQLreader.Close()
                            SQLcommand.CommandText = "DROP TABLE " & table
                            Try
                                SQLcommand.ExecuteNonQuery()
                                exist = False
                            Catch ex As Exception
                                logStats("MPSync: [TableExist] Table not dropped with exception - " & ex.Message, "ERROR")
                            End Try
                        End If
                    End Using
                End If

            End Using

            Return exist

        Catch ex As Exception
            logStats("MPSync: [TableExist] Unexpected error " & ex.Message, "ERROR")
        End Try

    End Function

    Private Function FieldExist(ByVal path As String, ByVal database As String, ByVal table As String, ByVal field As String) As Boolean

        If debug Then MPSync_process.logStats("MPSync: [FieldExist] Check field exists for table " & table & " in database " & path & database, "DEBUG")

        Dim columns() As String = Nothing
        Dim x As Integer = 0

        Using SQLconnect As New SQLiteConnection(),
            SQLcommand As SQLiteCommand = SQLconnect.CreateCommand

            SQLconnect.ConnectionString = "Data Source=" & p_Database(path & database) & ";Read Only=True;"
            SQLconnect.Open()
            SQLcommand.CommandText = "PRAGMA table_info (" & table & ")"
            Using SQLreader = SQLcommand.ExecuteReader()

                While SQLreader.Read()
                    ReDim Preserve columns(x)
                    columns(x) = LCase(SQLreader.GetString(1))
                    x += 1
                End While
            End Using
        End Using

        If x = 0 Then ReDim Preserve columns(0)

        Return columns.Contains(LCase(field))

    End Function

    Private Function FieldList(ByVal path As String, ByVal database As String, ByVal table As String, ByVal fields As String(), Optional ByVal prefix As String = Nothing) As String

        If debug Then MPSync_process.logStats("MPSync: [FieldList] Get fields for table " & table & " in database " & path & database, "DEBUG")

        Dim f_list As String = Nothing

        For x As Integer = 0 To UBound(fields)

            If FieldExist(path, database, table, fields(x)) Then
                f_list &= prefix & fields(x) & ","
            End If

        Next

        If f_list <> Nothing Then f_list = Left(f_list, Len(f_list) - 1)

        Return f_list

    End Function

    Private Sub getDBInfo(ByVal source As String, ByVal target As String)

        logStats("MPSync: [getDBInfo] Background threads for database integrity checking started.", "DEBUG")

        Dim x As Integer = 0
        Dim bw_checkDB() As BackgroundWorker = Nothing

        bw_threads = 0

        For Each database As String In IO.Directory.GetFiles(source, "*.db3")

            If IO.Path.GetExtension(database) <> ".db3-journal" Then

                If _databases.Contains(IO.Path.GetFileName(database)) Or _databases.Contains("ALL") Then
                    ' check if there are available threads to submit current stream, unless there is no limit.

                    If checkThreads("DB") <> -1 Then

                        Do While bw_threads >= checkThreads("DB")
                            logStats("MPSync: [getDBInfo] waiting for available threads.", "DEBUG")
                            wait(10, False)
                        Loop

                    End If

                    Try
                        ReDim Preserve dbname(x)
                        ReDim Preserve dbinfo(x)
                        dbname(x) = IO.Path.GetFileName(database)
                        dbinfo(x) = My.Computer.FileSystem.GetFileInfo(database)
                        bw_dbs.Add(dbname(x))

                        logStats("MPSync: [getDBInfo] starting background thread for database " & database, "DEBUG")

                        ReDim Preserve bw_checkDB(x)
                        bw_checkDB(x) = New BackgroundWorker
                        bw_checkDB(x).WorkerSupportsCancellation = True
                        AddHandler bw_checkDB(x).DoWork, AddressOf bw_checkDB_worker
                        AddHandler bw_checkDB(x).RunWorkerCompleted, AddressOf bw_checkDB_exception

                        bw_checkDB(x).RunWorkerAsync(target & "|" & dbname(x) & "|" & database & "|" & source)

                        x += 1
                        bw_threads += 1
                    Catch ex As Exception
                        logStats("MPSync: [getDBInfo] Failed to trigger background threads for database integrity checking.", "ERROR")
                    End Try

                End If

            End If

        Next

        Do While bw_threads > 0
            Dim jobs As String = String.Join(",", bw_dbs.ToArray())
            If jobs = String.Empty Then Exit Do
            logStats("MPSync: [getDBInfo] waiting for background threads to finish... " & bw_threads.ToString & " threads remaining processing {" & jobs & "}.", "DEBUG")
            wait(10, False)
        Loop

        logStats("MPSync: [getDBInfo] Background threads for database integrity checking complete.", "DEBUG")

    End Sub

    Private Sub bw_checkDB_exception(sender As System.Object, e As RunWorkerCompletedEventArgs)

        If e.Error IsNot Nothing Then
            logStats("MPSync: [getDBInfo] Exception " & e.Error.Message, "ERROR")
        End If

    End Sub

    Private Sub bw_checkDB_worker(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs)
        Try

            Dim check As String = "ok"
            Dim parm() As String = Split(e.Argument, "|")

            logStats("MPSync: [getDBInfo][bw_checkDB_worker] Checking integrity of database " & parm(0) & parm(1) & " in progress...", "LOG")

            If IO.File.Exists(parm(0) & parm(1)) Then
                Using SQLconnect As New SQLiteConnection(),
                SQLcommand As SQLiteCommand = SQLconnect.CreateCommand

                    Try
                        SQLconnect.ConnectionString = "Data Source=" & p_Database(parm(0) & parm(1)) & ";Read Only=True;"
                        SQLconnect.Open()
                        SQLcommand.CommandText = "PRAGMA integrity_check;"
                        Using SQLreader = SQLcommand.ExecuteReader()
                            SQLreader.Read()
                            check = SQLreader.GetString(0)
                        End Using
                    Catch ex As Exception
                        check = "error"
                    End Try
                End Using

                If check = "ok" And _vacuum Then
                    logStats("MPSync: [getDBInfo][bw_checkDB_worker] VACUUM of database " & parm(0) & parm(1) & " started.", "LOG")

                    Using SQLconnect As New SQLiteConnection(),
                SQLcommand As SQLiteCommand = SQLconnect.CreateCommand
                        Try
                            SQLconnect.ConnectionString = "Data Source=" & p_Database(parm(0) & parm(1)) & ";"
                            SQLconnect.Open()
                            SQLcommand.CommandText = "VACUUM;"
                            SQLcommand.ExecuteNonQuery()
                            logStats("MPSync: [getDBInfo][bw_checkDB_worker] VACUUM of database " & parm(0) & parm(1) & " complete.", "LOG")
                        Catch ex As Exception
                            logStats("MPSync: [getDBInfo][bw_checkDB_worker] VACUUM database " & parm(2) & " failed with exception: " & ex.Message, "ERROR")
                        End Try
                    End Using
                End If

                If check <> "ok" Then IO.File.Delete(parm(0) & parm(1))

            End If

            Try
                If Not IO.File.Exists(parm(0) & parm(1)) Then
                    logStats("MPSync: Copying database " & parm(2) & " to target.", "LOG")
                    IO.File.Copy(parm(2), parm(0) & parm(1), True)
                    Drop_Triggers(parm(0), parm(1), "mpsync_update|mpsync_watch")
                    check = "copied database from source"
                End If
            Catch ex As Exception
                logStats("MPSync: [getDBInfo][bw_checkDB_worker] Error while copying database " & parm(2) & " with exception: " & ex.Message, "ERROR")
            End Try

            logStats("MPSync: [getDBInfo][bw_checkDB_worker] Checking integrity of database " & parm(0) & parm(1) & " complete - Status: " & check, "LOG")

            bw_threads -= 1
            bw_dbs.RemoveAt(bw_dbs.IndexOf(parm(1)))
        Catch ex As Exception
            logStats("MPSync: [getDBInfo][bw_checkDB_worker] Unexpected error " & ex.Message, "ERROR")
        End Try

    End Sub

    Private Sub checkTriggers(ByVal mode As String)

        Dim x As Integer
        Dim bw_checkTriggers() As BackgroundWorker = Nothing

        logStats("MPSync: [checkTriggers] Background threads for WATCH Trigger checking started.", "DEBUG")

        bw_threads = 0
        bw_dbs.Clear()

        mps.SetWatched()

        For x = 0 To UBound(_watched_dbs)

            If _watched_dbs(x) <> "" Then
                ' check if there are available threads to submit current stream, unless there is no limit.

                If checkThreads("DB") <> -1 Then

                    Do While bw_threads >= checkThreads("DB")
                        logStats("MPSync: [checkTriggers] waiting for available threads.", "DEBUG")
                        wait(10, False)
                    Loop

                End If

                ReDim Preserve bw_checkTriggers(x)
                bw_checkTriggers(x) = New BackgroundWorker
                bw_checkTriggers(x).WorkerSupportsCancellation = True
                AddHandler bw_checkTriggers(x).DoWork, AddressOf bw_watchtriggers_worker

                If check_watched Then
                    If Not bw_checkTriggers(x).IsBusy Then bw_checkTriggers(x).RunWorkerAsync("ADD|" & _watched_dbs(x))
                Else
                    If Not bw_checkTriggers(x).IsBusy Then bw_checkTriggers(x).RunWorkerAsync("DROP|" & _watched_dbs(x))
                End If

                bw_threads += 1
                bw_dbs.Add(_watched_dbs(x))
            End If

        Next

        Do While bw_threads > 0
            Dim jobs As String = String.Join(",", bw_dbs.ToArray())
            If jobs = String.Empty Then Exit Do
            logStats("MPSync: [checkTriggers - WATCH] waiting for background threads to finish... " & bw_threads.ToString & " threads remaining processing {" & jobs & "}.", "DEBUG")
            wait(10, False)
        Loop

        logStats("MPSync: [checkTriggers] Background threads for WATCH Trigger checking complete.", "DEBUG")

        If sync_type = "Triggers" Then

            logStats("MPSync: [checkTriggers] Background threads for WORK Trigger checking started.", "DEBUG")

            bw_threads = 0
            bw_dbs.Clear()

            ' create remaining triggers to handle synchronization

            If mode = "client>server" Then
                For Each database As String In IO.Directory.GetFiles(_db_client, "*.db3")

                    If IO.Path.GetExtension(database) <> ".db3-journal" Then

                        If _databases.Contains(IO.Path.GetFileName(database)) Or _databases.Contains("ALL") Then
                            ' check if there are available threads to submit current stream, unless there is no limit.

                            If checkThreads("DB") <> -1 Then

                                Do While bw_threads >= checkThreads("DB")
                                    logStats("MPSync: [checkTriggers] waiting for available threads.", "DEBUG")
                                    wait(10, False)
                                Loop

                            End If

                            ReDim Preserve bw_checkTriggers(x)
                            bw_checkTriggers(x) = New BackgroundWorker
                            bw_checkTriggers(x).WorkerSupportsCancellation = True
                            AddHandler bw_checkTriggers(x).DoWork, AddressOf bw_worktriggers_worker

                            If Not bw_checkTriggers(x).IsBusy Then bw_checkTriggers(x).RunWorkerAsync("ADD|" & database)
                            x += 1
                            bw_threads += 1
                            bw_dbs.Add(database)

                        End If
                    End If
                Next
            End If

            For Each database As String In IO.Directory.GetFiles(_db_server, "*.db3")

                If IO.Path.GetExtension(database) <> ".db3-journal" Then

                    If _databases.Contains(IO.Path.GetFileName(database)) Or _databases.Contains("ALL") Then
                        ' check if there are available threads to submit current stream, unless there is no limit.

                        If checkThreads("DB") <> -1 Then

                            Do While bw_threads >= checkThreads("DB")
                                logStats("MPSync: [checkTriggers] waiting for available threads.", "DEBUG")
                                wait(10, False)
                            Loop

                        End If

                        ReDim Preserve bw_checkTriggers(x)
                        bw_checkTriggers(x) = New BackgroundWorker
                        bw_checkTriggers(x).WorkerSupportsCancellation = True
                        AddHandler bw_checkTriggers(x).DoWork, AddressOf bw_worktriggers_worker

                        If Not bw_checkTriggers(x).IsBusy Then bw_checkTriggers(x).RunWorkerAsync("ADD|" & database)
                        x += 1
                        bw_threads += 1
                        bw_dbs.Add(database)

                    End If
                End If

            Next

            Do While bw_threads > 0
                Dim jobs As String = String.Join(",", bw_dbs.ToArray())
                If jobs = String.Empty Then Exit Do
                logStats("MPSync: [checkTriggers - WORK] waiting for background threads to finish... " & bw_threads.ToString & " threads remaining processing {" & jobs & "}.", "DEBUG")
                wait(10, False)
            Loop

            logStats("MPSync: [checkTriggers] Background threads for WORK Trigger checking complete.", "DEBUG")

        End If

        ' remove triggers from local, if any
        If sync_type = "Timestamp" Or mode = "server>client" Then

            logStats("MPSync: [checkTriggers] Background threads for DROP Trigger started.", "DEBUG")

            bw_threads = 0
            bw_dbs.Clear()

            For Each database As String In IO.Directory.GetFiles(_db_client, "*.db3")

                If IO.Path.GetExtension(database) <> ".db3-journal" Then
                    ' check if there are available threads to submit current stream, unless there is no limit.

                    If checkThreads("DB") <> -1 Then

                        Do While bw_threads >= checkThreads("DB")
                            logStats("MPSync: [checkTriggers] waiting for available threads.", "DEBUG")
                            wait(10, False)
                        Loop

                    End If

                    ReDim Preserve bw_checkTriggers(x)
                    bw_checkTriggers(x) = New BackgroundWorker
                    bw_checkTriggers(x).WorkerSupportsCancellation = True
                    AddHandler bw_checkTriggers(x).DoWork, AddressOf bw_worktriggers_worker

                    If Not bw_checkTriggers(x).IsBusy Then bw_checkTriggers(x).RunWorkerAsync("DROP|" & database)
                    x += 1
                    bw_threads += 1
                    bw_dbs.Add(database)
                End If

            Next

            Do While bw_threads > 0
                Dim jobs As String = String.Join(",", bw_dbs.ToArray())
                If jobs = String.Empty Then Exit Do
                logStats("MPSync: [checkTriggers - DROP] waiting for background threads to finish... " & bw_threads.ToString & " threads remaining processing {" & jobs & "}.", "DEBUG")
                wait(10, False)
            Loop

            logStats("MPSync: [checkTriggers] Background threads for DROP Trigger complete.", "DEBUG")

        End If

    End Sub

    Private Sub bw_watchtriggers_worker(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs)

        Try

            Dim parm() As String = Split(e.Argument, "|")

            logStats("MPSync: [checkTriggers][bw_watchtriggers_worker] Background watch trigger thread on database " & parm(1) & " for " & parm(0) & " started.", "DEBUG")

            If parm(0) = "ADD" Then
                Create_Watch_Tables(_db_client, parm(1))
                Create_Watch_Tables(_db_server, parm(1))
                Drop_Triggers(_db_client, parm(1), "mpsync_update|mpsync_watch")
                If _db_direction <> 1 Then
                    Create_Watch_Triggers(_db_client, parm(1))
                End If
            ElseIf parm(0) = "DROP" Then
                Drop_Watch_Tables(_db_client, parm(1))
                Drop_Triggers(_db_client, parm(1), "mpsync_update|mpsync_watch")
            End If

            bw_threads -= 1
            bw_dbs.RemoveAt(bw_dbs.LastIndexOf(parm(1)))

            logStats("MPSync: [checkTriggers][bw_watchtriggers_worker] Background watch trigger thread on database " & parm(1) & " for " & parm(0) & " complete.", "DEBUG")

        Catch ex As Exception
            logStats("MPSync: [checkTriggers][bw_watchtriggers_worker] Unexpected error " & ex.Message, "ERROR")
        End Try

    End Sub

    Private Sub bw_worktriggers_worker(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs)

        Try

            Dim parm() As String = Split(e.Argument, "|")

            logStats("MPSync: [checkTriggers][bw_worktriggers_worker] Background work trigger thread on database " & parm(1) & " started.", "DEBUG")

            If parm(0) = "DROP" Then Drop_Triggers(IO.Path.GetDirectoryName(parm(1)) & "\", IO.Path.GetFileName(parm(1)), "mpsync_work")
            If parm(0) = "ADD" Then Create_Sync_Triggers(parm(1))

            bw_threads -= 1
            bw_dbs.RemoveAt(bw_dbs.LastIndexOf(parm(1)))

            logStats("MPSync: [checkTriggers][bw_worktriggers_worker] Background work trigger thread on database " & parm(1) & " complete.", "DEBUG")

        Catch ex As Exception
            logStats("MPSync: [checkTriggers][bw_worktriggers_worker] Unexpected error " & ex.Message, "ERROR")
        End Try

    End Sub

    Private Sub Create_Watch_Tables(ByVal path As String, ByVal database As String)

        If Not IO.File.Exists(path & database) Then Exit Sub

        logStats("MPSync: [Create_Watch_Tables] Creating/altering watch table mpsync in database " & path & database, "DEBUG")

        Dim SQL As String = Nothing

        If TableExist(path, database, "mpsync", True) = False Then

            Select Case database

                Case mps.i_watched(0).database
                    SQL = "CREATE TABLE IF NOT EXISTS mpsync " & _
                          "(mps_id INTEGER PRIMARY KEY AUTOINCREMENT, tablename TEXT, mps_lastupdated TEXT, mps_session TEXT, id INTEGER, user TEXT, " & _
                          "user_rating TEXT, watched INTEGER, resume_part INTEGER, resume_time INTEGER, resume_data TEXT, alternatecovers TEXT, coverfullpath TEXT, " & _
                          "coverthumbfullpath TEXT)"
                Case mps.i_watched(1).database
                    SQL = "CREATE TABLE IF NOT EXISTS mpsync (mps_id INTEGER PRIMARY KEY AUTOINCREMENT, tablename TEXT, mps_lastupdated TEXT, mps_session TEXT, " & _
                          "idTrack INTEGER, iResumeAt INTEGER, dateLastPlayed TEXT)"
                Case mps.i_watched(2).database
                    SQL = "CREATE TABLE IF NOT EXISTS mpsync " & _
                          "(mps_id INTEGER PRIMARY KEY AUTOINCREMENT, tablename TEXT, mps_lastupdated TEXT, mps_session TEXT, CompositeID TEXT, id INTEGER, " & _
                          "EpisodeFilename TEXT, watched INTEGER, myRating TEXT, StopTime TEXT, DateWatched TEXT, WatchedFileTimeStamp INTEGER, " & _
                          "UnwatchedItems INTEGER, EpisodesUnWatched INTEGER)"
                Case mps.i_watched(3).database
                    SQL = "CREATE TABLE IF NOT EXISTS mpsync " & _
                          "(mps_id INTEGER PRIMARY KEY AUTOINCREMENT, tablename TEXT, mps_lastupdated TEXT, mps_session TEXT, idMovie INTEGER, watched BOOL, " & _
                          "timeswatched INTEGER, iwatchedPercent INTEGER, idResume INTEGER, idFile INTEGER, stoptime INTEGER, resumeData BLOOB, idBookmark INTEGER, " & _
                          "fPercentage TEXT)"

            End Select

        Else
            If FieldExist(path, database, "mpsync", "mps_session") = False Then SQL = "ALTER TABLE mpsync ADD COLUMN mps_session TEXT;"

            Select Case database

                Case mps.i_watched(0).database
                    If FieldExist(path, database, "mpsync", "id") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN id INTEGER;"
                    If FieldExist(path, database, "mpsync", "user") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN user TEXT;"
                    If FieldExist(path, database, "mpsync", "user_rating") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN user_rating TEXT;"
                    If FieldExist(path, database, "mpsync", "watched") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN watched INTEGER;"
                    If FieldExist(path, database, "mpsync", "resume_part") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN resume_part INTEGER;"
                    If FieldExist(path, database, "mpsync", "resume_data") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN resume_data TEXT;"
                    If FieldExist(path, database, "mpsync", "alternatecovers") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN alternatecovers TEXT;"
                    If FieldExist(path, database, "mpsync", "coverfullpath") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN coverfullpath TEXT;"
                    If FieldExist(path, database, "mpsync", "coverthumbfullpath") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN coverthumbfullpath TEXT;"
                Case mps.i_watched(1).database
                    If FieldExist(path, database, "mpsync", "idTrack") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN idTrack INTEGER;"
                    If FieldExist(path, database, "mpsync", "iResumeAt") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN iResumeAt INTEGER;"
                    If FieldExist(path, database, "mpsync", "dateLastPlayed") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN dateLastPlayed TEXT;"
                Case mps.i_watched(2).database
                    If FieldExist(path, database, "mpsync", "CompositeID") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN CompositeID TEXT;"
                    If FieldExist(path, database, "mpsync", "id") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN id INTEGER;"
                    If FieldExist(path, database, "mpsync", "EpisodeFilename") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN EpisoideFilename TEXT;"
                    If FieldExist(path, database, "mpsync", "watched") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN watched INTEGER;"
                    If FieldExist(path, database, "mpsync", "myRating") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN myRating TEXT;"
                    If FieldExist(path, database, "mpsync", "StopTime") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN StopTime TEXT;"
                    If FieldExist(path, database, "mpsync", "DateWatched") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN DateWatched TEXT;"
                    If FieldExist(path, database, "mpsync", "WatchedFileTimeStamp") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN WatchedFileTimeStamp INTEGER;"
                    If FieldExist(path, database, "mpsync", "UnwatchedItems") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN UnwatchedItems INTEGER;"
                    If FieldExist(path, database, "mpsync", "EpisodesUnWatched") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN EpisodesUnWatched INTEGER;"
                Case mps.i_watched(3).database
                    If FieldExist(path, database, "mpsync", "idMovie") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN idMovie INTEGER;"
                    If FieldExist(path, database, "mpsync", "watched") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN watched BOOL;"
                    If FieldExist(path, database, "mpsync", "timeswatched") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN timeswatched INTEGER;"
                    If FieldExist(path, database, "mpsync", "iwatchedPercent") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN iwatchedPercent INTEGER;"
                    If FieldExist(path, database, "mpsync", "idResume") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN idResume INTEGER;"
                    If FieldExist(path, database, "mpsync", "idFile") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN idFile INTEGER;"
                    If FieldExist(path, database, "mpsync", "stoptime") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN stoptime INTEGER;"
                    If FieldExist(path, database, "mpsync", "resumeData") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN resumeData BLOOB;"
                    If FieldExist(path, database, "mpsync", "idBookmark") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN idBookmark INTEGER;"
                    If FieldExist(path, database, "mpsync", "fPercentage") = False Then SQL = SQL & "ALTER TABLE mpsync ADD COLUMN fPercentage TEXT;"

            End Select

        End If

        If SQL <> Nothing Then

            Using SQLconnect As New SQLiteConnection(),
                SQLcommand As SQLiteCommand = SQLconnect.CreateCommand

                SQLconnect.ConnectionString = "Data Source=" & p_Database(path & database)
                Try
                    SQLconnect.Open()
                    SQLcommand.CommandText = SQL
                    SQLcommand.ExecuteNonQuery()
                Catch ex As Exception
                    logStats("MPSync: [Create_Watch_Tables] Error executing '" & SQLcommand.CommandText & "' on database " & database, "ERROR")
                End Try

            End Using

        End If

        logStats("MPSync: [Create_Watch_Tables] Watch table mpsync in database " & path & database & " created/altered.", "DEBUG")

    End Sub

    Private Sub Drop_Watch_Tables(ByVal path As String, ByVal database As String)

        If Not IO.File.Exists(path & database) Then Exit Sub

        logStats("MPSync: [Drop_Watch_Tables] Drop watch table mpsync from database " & path & database, "DEBUG")

        Using SQLconnect As New SQLiteConnection(),
            SQLcommand As SQLiteCommand = SQLconnect.CreateCommand

            SQLconnect.ConnectionString = "Data Source=" & p_Database(path & database)
            Try
                SQLconnect.Open()
                SQLcommand.CommandText = "DROP TABLE IF EXISTS mpsync"
                SQLcommand.ExecuteNonQuery()
            Catch ex As Exception
                logStats("MPSync: [Drop_Watch_Tables] Error executing '" & SQLcommand.CommandText & "' from database " & path & database, "ERROR")
            End Try

        End Using

        logStats("MPSync: [Drop_Watch_Tables] Watch table mpsync from database " & path & database & " dropped.", "DEBUG")

    End Sub

    Private Sub Create_Watch_Triggers(ByVal path As String, ByVal database As String)

        If Not IO.File.Exists(path & database) Then Exit Sub

        logStats("MPSync: [Create_Watch_Triggers] Creating watch triggers in database " & path & database, "DEBUG")

        Dim SQL As String = Nothing
        Dim tables() As String = Nothing
        Dim keys() As String = Nothing
        Dim fields() As fieldnames = Nothing
        Dim tblflds As String = Nothing
        Dim newflds As String = Nothing

        Select Case database

            Case mps.i_watched(0).database
                ReDim tables(1), keys(1), fields(1)

                tables(0) = "user_movie_settings"
                keys(0) = "id"
                fields(0).names = {"id", "user", "user_rating", "watched", "resume_part", "resume_time", "resume_data"}

                tables(1) = "movie_info"
                keys(1) = "id"
                fields(1).names = {"id", "alternatecovers", "coverfullpath", "coverthumbfullpath"}

            Case mps.i_watched(1).database
                ReDim tables(0), keys(0), fields(0)

                tables(0) = "tracks"
                keys(0) = "idTrack"
                fields(0).names = {"idTrack", "iResumeAt", "dateLastPlayed"}

            Case mps.i_watched(2).database
                ReDim tables(3), keys(3), fields(3)

                tables(0) = "local_episodes"
                keys(0) = "EpisodeFilename"
                fields(0).names = {"EpisodeFilename", "CompositeID", "DateWatched", "StopTime"}

                tables(1) = "online_episodes"
                keys(1) = "CompositeID"
                fields(1).names = {"CompositeID", "watched", "myRating"}

                tables(2) = "online_series"
                keys(2) = "id"
                fields(2).names = {"ID", "WatchedFileTimeStamp", "UnwatchedItems", "EpisodesUnWatched"}

                tables(3) = "season"
                keys(3) = "id"
                fields(3).names = {"ID", "UnwatchedItems", "EpisodesUnWatched"}

            Case mps.i_watched(3).database
                ReDim tables(2), keys(2), fields(2)

                tables(0) = "movie"
                keys(0) = "idMovie"
                fields(0).names = {"idMovie", "watched", "timeswatched", "iwatchedPercent"}

                tables(1) = "resume"
                keys(1) = "idResume"
                fields(1).names = {"idResume", "idFile", "stoptime", "resumeData"}

                tables(2) = "bookmark"
                keys(2) = "idResume"
                fields(2).names = {"idBookMark", "idFile", "fPercentage"}

        End Select

        ' build the SQL statement for the creation of the respective triggers

        For x As Integer = 0 To UBound(tables)

            If tables(x) <> Nothing Then

                If TableExist(path, database, tables(x)) Then

                    tblflds = FieldList(path, database, tables(x), fields(x).names)

                    If tblflds <> Nothing Then
                        newflds = FieldList(path, database, tables(x), fields(x).names, "new.")

                        SQL = SQL & "CREATE TRIGGER IF NOT EXISTS mpsync_watch_" & tables(x) & " " & _
                              "AFTER UPDATE OF " & tblflds & " ON " & tables(x) & " " & _
                              "BEGIN " & _
                              "DELETE FROM mpsync WHERE tablename='" & tables(x) & "' AND mps_session='" & session & "' AND " & keys(x) & "=new." & keys(x) & "; " & _
                              "INSERT OR REPLACE INTO mpsync(tablename,mps_lastupdated,mps_session," & tblflds & ") " & _
                              "VALUES('" & tables(x) & "',datetime('now','localtime'),'" & session & "'," & newflds & "); " & _
                              "END; "
                    End If
                End If
            End If
        Next

        If SQL <> Nothing Then

            Using SQLconnect As New SQLiteConnection(),
                SQLcommand As SQLiteCommand = SQLconnect.CreateCommand

                SQLconnect.ConnectionString = "Data Source=" & p_Database(path & database)
                Try
                    SQLconnect.Open()
                    SQLcommand.CommandText = SQL
                    SQLcommand.ExecuteNonQuery()
                Catch ex As Exception
                    logStats("MPSync: [Create_Watch_Triggers] Error executing '" & SQLcommand.CommandText & " on database " & path & database, "ERROR")
                End Try
            End Using

        End If

        logStats("MPSync: [Create_Watch_Triggers] Watch triggers in database " & path & database & " created.", "DEBUG")

    End Sub

    Private Sub Drop_Triggers(ByVal path As String, ByVal database As String, ByVal searchpattern As String, Optional ByVal table As String = Nothing)

        If Not IO.File.Exists(path & database) Then Exit Sub

        logStats("MPSync: [Drop_Triggers] Drop triggers in database " & path & database, "DEBUG")

        Dim SQL As String = Nothing
        Dim pattern() As String = Split(searchpattern, "|")

        Using SQLconnect As New SQLiteConnection(),
            SQLcommand As SQLiteCommand = SQLconnect.CreateCommand

            Try
                SQLconnect.ConnectionString = "Data Source=" & p_Database(path & database)
                SQLconnect.Open()

                If table = Nothing Then
                    SQLcommand.CommandText = "SELECT name FROM sqlite_master WHERE type='trigger'"
                Else
                    SQLcommand.CommandText = "SELECT name FROM sqlite_master WHERE type='trigger' and tbl_name='" & table & "'"
                End If

                Using SQLreader = SQLcommand.ExecuteReader()

                    While SQLreader.Read()
                        For x As Integer = 0 To UBound(pattern)
                            If Left(SQLreader.GetString(0), Len(pattern(x))) = pattern(x) Then SQL = SQL & "DROP TRIGGER " & SQLreader.GetString(0) & "; "
                        Next
                    End While
                End Using

                If SQL <> Nothing Then
                    SQLcommand.CommandText = SQL
                    SQLcommand.ExecuteNonQuery()
                End If
            Catch ex As Exception
                logStats("MPSync: [Drop_Triggers] Error executing '" & SQLcommand.CommandText & "' on database " & path & database, "ERROR")
            End Try

        End Using

        logStats("MPSync: [Drop_Triggers] Triggers in database " & path & database & " dropped.", "DEBUG")

    End Sub

    Private Sub Create_Sync_Triggers(ByVal database As String)

        If Not IO.File.Exists(database) Then Exit Sub

        logStats("MPSync: [Create_Sync_Triggers] Check synchronization triggers in database " & database, "DEBUG")

        Dim x As Integer = -1
        Dim table() As String = Nothing
        Dim trigger() As String = Nothing
        Dim SQL() As String = Nothing
        Dim d_SQL, u_SQL, i_SQL As String
        Dim omit As Array = {"mpsync", "mpsync_trigger", "sqlite_sequence", "sqlite_stat1", "sqlite_stat2"}

        Using SQLconnect As New SQLiteConnection(),
          SQLcommand As SQLiteCommand = SQLconnect.CreateCommand

            SQLconnect.ConnectionString = "Data Source=" & p_Database(database)
            SQLconnect.Open()
            SQLcommand.CommandText = "SELECT name FROM sqlite_master WHERE type='table'"
            Using SQLreader = SQLcommand.ExecuteReader()
                While SQLreader.Read()

                    If Array.IndexOf(omit, SQLreader(0)) = -1 Then
                        x += 1
                        ReDim Preserve table(x)
                        table(x) = SQLreader.GetString(0)
                    End If

                End While
            End Using

            If x = -1 Then
                ReDim Preserve table(0)
                table(0) = Nothing
            End If

            SQLcommand.CommandText = "SELECT sql FROM sqlite_master WHERE type='trigger'"
            Using SQLreader = SQLcommand.ExecuteReader()
                x = -1

                While SQLreader.Read()

                    If Array.IndexOf(omit, SQLreader(0)) = -1 Then
                        x += 1
                        ReDim Preserve trigger(x)
                        trigger(x) = SQLreader.GetString(0)
                    End If

                End While
            End Using

            If x = -1 Then
                ReDim Preserve trigger(0)
                trigger(0) = Nothing
            End If

            If TableExist(IO.Path.GetDirectoryName(database) & "\", IO.Path.GetFileName(database), "mpsync_trigger", True) = False Then

                logStats("MPSync: [Create_Sync_Triggers] Creating work table mpsync_trigger in database " & database, "LOG")

                Try
                    SQLcommand.CommandText = "CREATE TABLE IF NOT EXISTS mpsync_trigger (id INTEGER PRIMARY KEY AUTOINCREMENT, tablename TEXT, lastupdated TEXT)"
                    SQLcommand.ExecuteNonQuery()
                Catch ex As Exception
                    logStats("MPSync: [Create_Sync_Triggers] Error executing '" & SQLcommand.CommandText & " on database " & database, "ERROR")
                End Try

        End If

        For x = 0 To UBound(table)

            u_SQL = "CREATE TRIGGER mpsync_work_u_" & table(x) & " " & _
                    "AFTER UPDATE ON " & table(x) & " " & _
                    "BEGIN " & _
                    "UPDATE mpsync_trigger SET lastupdated=datetime('now','localtime') WHERE tablename='" & table(x) & "'; " & _
                    "INSERT INTO mpsync_trigger(tablename,lastupdated) " & _
                    "SELECT '" & table(x) & "',datetime('now','localtime') " & _
                    "WHERE NOT EXISTS (SELECT 1 FROM mpsync_trigger WHERE tablename = '" & table(x) & "' LIMIT 1); " & _
                    "END"

            i_SQL = "CREATE TRIGGER mpsync_work_i_" & table(x) & " " & _
                    "AFTER INSERT ON " & table(x) & " " & _
                    "BEGIN " & _
                    "UPDATE mpsync_trigger SET lastupdated=datetime('now','localtime') WHERE tablename='" & table(x) & "'; " & _
                    "INSERT INTO mpsync_trigger(tablename,lastupdated) " & _
                    "SELECT '" & table(x) & "',datetime('now','localtime') " & _
                    "WHERE NOT EXISTS (SELECT 1 FROM mpsync_trigger WHERE tablename = '" & table(x) & "' LIMIT 1); " & _
                    "END"

            d_SQL = "CREATE TRIGGER mpsync_work_d_" & table(x) & " " & _
                    "AFTER DELETE ON " & table(x) & " " & _
                    "BEGIN " & _
                    "UPDATE mpsync_trigger SET lastupdated=datetime('now','localtime') WHERE tablename='" & table(x) & "'; " & _
                    "INSERT INTO mpsync_trigger(tablename,lastupdated) " & _
                    "SELECT '" & table(x) & "',datetime('now','localtime') " & _
                    "WHERE NOT EXISTS (SELECT 1 FROM mpsync_trigger WHERE tablename = '" & table(x) & "' LIMIT 1); " & _
                    "END"

            If trigger.Contains(u_SQL) And trigger.Contains(i_SQL) And trigger.Contains(d_SQL) Then
                    u_SQL = Nothing
                    i_SQL = Nothing
                    d_SQL = Nothing
                Else
                    u_SQL = u_SQL & "; "
                    i_SQL = i_SQL & "; "
                    d_SQL = d_SQL & "; "
                End If

                ReDim Preserve SQL(x)
                SQL(x) = u_SQL & i_SQL & d_SQL

            Next

        End Using

        For x = 0 To UBound(SQL)
            If SQL(x) <> Nothing Then Drop_Triggers(IO.Path.GetDirectoryName(database) & "\", IO.Path.GetFileName(database), "mpsync_work", table(x))
        Next

        Using SQLconnect As New SQLiteConnection(),
        SQLcommand As SQLiteCommand = SQLconnect.CreateCommand
            Try

                SQLconnect.ConnectionString = "Data Source=" & p_Database(database)
                SQLconnect.Open()

                For x = 0 To UBound(SQL)

                    If SQL(x) <> Nothing Then

                        logStats("MPSync: [Create_Sync_Triggers] Creating synchronization triggers on table " & table(x) & " in database " & database, "LOG")

                        SQLcommand.CommandText = SQL(x)
                        SQLcommand.ExecuteNonQuery()

                    End If

                Next
            Catch ex As Exception
                logStats("MPSync: [Create_Sync_Triggers] Error executing '" & SQLcommand.CommandText & " on table " & table(x) & " in database " & database & " with exception: " & ex.Message, "ERROR")
            End Try
        End Using

        logStats("MPSync: [Create_Sync_Triggers] Synchronization triggers in database " & database & " checked.", "DEBUG")

    End Sub

    Private Sub WorkMethod(stateInfo As Object)

        Dim db As Boolean = True
        Dim folders As Boolean = True

        If _db_client <> Nothing And _db_server <> Nothing And checked_databases Then
            Log.Debug("MPSync: Started Database thread")
            Dim bw_db As New BackgroundWorker
            bw_db.WorkerSupportsCancellation = True
            AddHandler bw_db.DoWork, AddressOf MPSync_process_DB.bw_db_worker
            bw_db.RunWorkerAsync()
        Else
            db = False
        End If

        If _folders_client <> Nothing And _folders_server <> Nothing And checked_folders Then
            Log.Debug("MPSync: Started folders thread")
            Dim bw_folders As New BackgroundWorker
            bw_folders.WorkerSupportsCancellation = True
            AddHandler bw_folders.DoWork, AddressOf MPSync_process_Folders.bw_folders_worker
            bw_folders.RunWorkerAsync()
        Else
            folders = False
        End If

        If Not MPSync_settings.syncnow Then
            If db = False And folders = False Then CType(stateInfo, AutoResetEvent).Set()
        End If

    End Sub

End Class
