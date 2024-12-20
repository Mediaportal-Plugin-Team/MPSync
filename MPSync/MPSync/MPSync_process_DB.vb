Imports System.ComponentModel
Imports System.Data.SQLite

Public Class MPSync_process_DB

    Dim lastsync As String
    Dim dlm As String = Chr(7) & "~" & Chr(30)
    Dim _bw_active_db_jobs, bw_sync_db_jobs As Integer
    Dim bw_dbs As New ArrayList
    Dim bw_sync_db() As BackgroundWorker

    Public Structure ColumnInfo
        Public name As String
        Public type As String
        Public dflt_value As String
        Public pk As Boolean
        Public notNull As Boolean
        Public Overrides Function ToString() As String
            Return String.Format("[{0}:{1}]", name, type)
        End Function
    End Structure

    Public Class TableInfo
        Public source As String
        Public target As String
        Public database As String
        Public name As String
        Public Sub New(asource As String, atarget As String, adatabase As String, aname As String)
            source = asource
            target = atarget
            database = adatabase
            name = aname
        End Sub
    End Class
    Public Class SyncDbParams
        Public tables As List(Of TableInfo)
    End Class

    Private ReadOnly Property p_Session As String
        Get
            Dim session As String = Nothing
            Try
                Using XMLreader As MediaPortal.Profile.Settings = New MediaPortal.Profile.Settings(MPSync_settings.GetConfigFileName)
                    session = XMLreader.GetValueAsString("Plugin", "session ID", Nothing)
                End Using
            Catch ex As Exception
                MPSync_process.logStats("MPSync: Error reading 'session' value from XML with exception " & ex.Message, MessageType.ERR)
            End Try
            Return session
        End Get
    End Property

    Private Property p_lastsync As String
        Get
            Dim lastsync As String = "0001-01-01 00:00:00"
            Try
                Using XMLreader As MediaPortal.Profile.Settings = New MediaPortal.Profile.Settings(MPSync_settings.GetConfigFileName)
                    lastsync = XMLreader.GetValueAsString("Plugin", "last sync", "0001-01-01 00:00:00")
                End Using
            Catch ex As Exception
                MPSync_process.logStats("MPSync: Error reading 'last sync' value from XML with exception " & ex.Message, MessageType.ERR)
            End Try
            Return lastsync
        End Get
        Set(value As String)
            Try
                Using XMLwriter As MediaPortal.Profile.Settings = New MediaPortal.Profile.Settings(MPSync_settings.GetConfigFileName)
                    XMLwriter.SetValue("Plugin", "last sync", value)
                End Using
                MediaPortal.Profile.Settings.SaveCache()
            Catch ex As Exception
                MPSync_process.logStats("MPSync: Error writing 'last sync' value in XML with exception " & ex.Message, MessageType.ERR)
            End Try
        End Set
    End Property

    Public Function LoadTable(ByVal path As String, ByVal database As String, ByVal table As String, Optional ByRef columns As List(Of ColumnInfo) = Nothing, Optional ByVal where As String = Nothing, Optional ByVal order As String = Nothing) As Array

        MPSync_process.logStats("MPSync: [LoadTable] Load values from table " & table & " in database " & path & database, MessageType.DEBUG)

        If columns Is Nothing Then columns = New List(Of ColumnInfo)

        Dim x, y, z, records As Integer
        Dim fields As String = "*"
        Dim data(,) As String = Nothing

        Using SQLconnect As New SQLiteConnection(),
            SQLcommand As SQLiteCommand = SQLconnect.CreateCommand

            Try

                If columns.Count = 0 Then
                    columns = getFields(path, database, table)
                Else
                    fields = getSelectFields(columns)
                End If

                z = getPK(columns)
                records = RecordCount(path, database, table, where)

                SQLconnect.ConnectionString = "Data Source=" & MPSync_process.p_Database(path & database) & ";Read Only=True;"
                SQLconnect.Open()

                If records > 0 Then

                    SQLcommand.CommandText = "SELECT rowid, " & fields & " FROM " & table

                    If where <> Nothing Then
                        SQLcommand.CommandText &= " WHERE " & where
                    End If

                    If order <> Nothing Then
                        SQLcommand.CommandText &= " ORDER BY " & order
                    End If

                    Using SQLreader = SQLcommand.ExecuteReader()

                        ReDim Preserve data(2, records - 1)
                        Try

                            While SQLreader.Read()

                                data(0, x) = SQLreader.GetString(0)

                                For y = 0 To columns.Count - 1
                                    If Not IsDBNull(SQLreader(y + 1)) Then
                                        Select Case columns(y).type
                                            Case "INTEGER", "REAL", "BLOB"
                                                data(1, x) &= SQLreader(y + 1).ToString.Replace(",", ".") & dlm
                                                If y = z Then data(2, x) = SQLreader(y + 1).ToString
                                            Case "TIMESTAMP"
                                                data(1, x) &= Format(SQLreader(y + 1), "yyyy-MM-dd HH:mm:ss") & dlm
                                                If y = z Then data(2, x) = Format(SQLreader(y + 1), "yyyy-MM-dd HH:mm:ss")
                                            Case Else
                                                data(1, x) &= SQLreader.GetString(y + 1) & dlm
                                                If y = z Then data(2, x) = SQLreader.GetString(y + 1)
                                        End Select
                                    Else
                                        data(1, x) &= "NULL" & dlm
                                        If y = z Then data(2, x) = "NULL"
                                    End If
                                Next

                                x += 1

                            End While
                        Catch ex As Exception
                            MPSync_process.logStats("MPSync: Error reading table " & table & " rowid """ & SQLreader.GetString(0) & """ in " & database & " with exception: " & ex.Message, MessageType.DEBUG)
                            Throw
                        End Try

                    End Using
                Else
                    ReDim data(2, 0)
                End If

            Catch ex As Exception
                MPSync_process.logStats("MPSync: Error reading data from table " & table & " in database " & database, MessageType.ERR)
                data = Nothing
            End Try

        End Using

        Return data

    End Function

    Public Function FormatValue(ByVal value As Object, ByVal type As String) As String

        'MPSync_process.logStats("MPSync: [FormatValue]", MessageType.DEBUG)

        Dim fmtvalue As String

        If value.ToString = "NULL" Then
            fmtvalue = "NULL,"
        Else
            Select Case type

                Case "INTEGER", "REAL"
                    fmtvalue = value.ToString.Replace(",", ".") & ","
                Case "BOOL"
                    If value.ToString = "True" Then fmtvalue = "'1'," Else fmtvalue = "'0',"
                Case Else
                    fmtvalue = "'" & value.ToString.Replace("'", "''") & "',"

            End Select
        End If

        Return fmtvalue

    End Function

    Private Function BuildUpdateArray_mpsync(ByVal w_values(,) As String, ByVal s_data As Array, ByVal mps_columns As List(Of ColumnInfo), ByVal columns As List(Of ColumnInfo)) As Array

        'MPSync_process.logStats("MPSync: [BuildUpdateArray_mpsync]", MessageType.DEBUG)

        Dim x, z As Integer
        Dim w_pk(), s_pk() As String

        If w_values.OfType(Of String)().ToArray().Length = 0 Then Return s_data

        w_pk = getPkValues(w_values, mps_columns, columns)

        If s_data.OfType(Of String)().ToArray().Length > 0 Then

            s_pk = getPkValues(s_data, mps_columns, columns)

            x = Array.IndexOf(mps_columns.OfType(Of String)().ToArray(), "mps_lastupdated")

            For y As Integer = 0 To (s_data.GetLength(1) - 1)

                z = w_pk.Contains(s_pk(y))

                If z <> -1 Then

                    If getLastUpdateDate(s_data(1, y), x) > getLastUpdateDate(w_values(1, z), x) Then
                        w_values(1, z) = s_data(1, y)
                    End If

                End If

            Next

        End If

        Return w_values

    End Function

    Public Function getFields(ByVal path As String, ByVal database As String, ByVal table As String) As List(Of ColumnInfo)

        'MPSync_process.logStats("MPSync: [getFields]", MessageType.DEBUG)

        Dim columns As List(Of ColumnInfo) = New List(Of ColumnInfo)

        Using SQLconnect As New SQLiteConnection(),
            SQLcommand As SQLiteCommand = SQLconnect.CreateCommand
            Dim x As Integer = 0

            Try
                SQLconnect.ConnectionString = "Data Source=" & MPSync_process.p_Database(path & database) & ";Read Only=True;"
                SQLconnect.Open()
                SQLcommand.CommandText = "PRAGMA table_info (" & table & ")"
                Using SQLreader = SQLcommand.ExecuteReader()
                    While SQLreader.Read()
                        Dim t As ColumnInfo = New ColumnInfo
                        t.name = LCase(SQLreader.GetString(1))
                        t.type = UCase(SQLreader.GetString(2))
                        t.notNull = SQLreader.GetBoolean(3)
                        If Not IsDBNull(SQLreader(4)) Then t.dflt_value = SQLreader.GetString(4).Replace("'", "")
                        t.pk = SQLreader.GetBoolean(5)
                        columns.Add(t)
                    End While
                End Using
            Catch ex As Exception
                MPSync_process.logStats("MPSync: Error getFields " & table & " in " & database & " with exception: " & ex.Message, MessageType.DEBUG)
            End Try

        End Using

        Return columns

    End Function

    Private Function getSelectFields(ByVal columns As List(Of ColumnInfo)) As String

        'MPSync_process.logStats("MPSync: [getSelectFields]", MessageType.DEBUG)

        Dim fields As String = Nothing

        For x = 0 To columns.Count - 1
            fields &= columns(x).name & ","
        Next

        Return Left(fields, Len(fields) - 1)

    End Function

    Private Function RecordCount(ByVal path As String, ByVal database As String, ByVal table As String, Optional ByVal where As String = Nothing) As Integer

        MPSync_process.logStats("MPSync: [RecordCount] Get number of records for table " & table & " in database " & path & database, MessageType.DEBUG)

        Dim x As Integer
        Using SQLconnect As New SQLiteConnection(),
            SQLcommand As SQLiteCommand = SQLconnect.CreateCommand

            Try

                SQLconnect.ConnectionString = "Data Source=" & MPSync_process.p_Database(path & database) & ";Read Only=True;"
                SQLconnect.Open()

                If where = Nothing Then
                    SQLcommand.CommandText = "SELECT COUNT(*) FROM " & table
                Else
                    SQLcommand.CommandText = "SELECT COUNT(*) FROM " & table & " WHERE " & where
                End If

                Using SQLreader = SQLcommand.ExecuteReader()
                    SQLreader.Read()
                    x = SQLreader.GetInt32(0)
                End Using
            Catch ex As Exception
                x = 0
            End Try

        End Using

        Return x

    End Function

    Public Function getPkValues(ByVal values As Array, ByVal mps_columns As List(Of ColumnInfo), ByVal columns As List(Of ColumnInfo)) As String()

        'MPSync_process.logStats("MPSync: [getPkValues]", MessageType.DEBUG)

        Dim x, y As Integer
        Dim temp2() As String
        Dim PKs() As String = Nothing

        x = getPK(columns)

        If x = -1 Then Return Nothing

        Dim mps_cols As String() = mps_columns.Select(Function(m) m.name).ToArray

        x = Array.IndexOf(mps_cols, columns(x).name)

        Dim temp1 As Array = getArray(values, 1)

        If temp1(0) IsNot Nothing Then
            For y = 0 To UBound(temp1)
                temp2 = Split(temp1(y), dlm)
                ReDim Preserve PKs(y)
                PKs(y) = temp2(x)
            Next
        Else
            Return Nothing
        End If

        Return PKs

    End Function

    Public Function getPK(ByVal columns As List(Of ColumnInfo), Optional ByRef pkey As String = Nothing) As Integer

        'MPSync_process.logStats("MPSync: [getPK]", MessageType.DEBUG)

        Dim x As Integer = 0
        While x < columns.Count And Not columns(x).pk
            x += 1
        End While

        If x < columns.Count Then
            pkey = columns(x).name
            Return x
        Else
            pkey = Nothing
            Return -1
        End If

    End Function

    Public Function getLastUpdateDate(ByVal values As String, ByVal index As Integer) As String

        'MPSync_process.logStats("MPSync: [getLastUpdateDate]", MessageType.DEBUG)

        Dim a_values() As String = Split(values, dlm)

        Return a_values(index)

    End Function

    Public Function getArray(ByVal array As Array, ByVal dimension As Integer) As Array

        'MPSync_process.logStats("MPSync: [getArray]", MessageType.DEBUG)

        If array Is Nothing Then Return Nothing

        Dim newarray(0) As String

        Try
            For x As Integer = 0 To UBound(array, 2)
                ReDim Preserve newarray(x)
                newarray(x) = array(dimension, x)
            Next
        Catch ex As Exception
            Return Nothing
        End Try

        Return newarray

    End Function

    Public Function getCurrentTableValues(ByVal path As String, ByVal database As String, ByVal table As String, ByVal columns As List(Of ColumnInfo), ByVal mps_cols As Array, ByVal pkey As String, ByVal fields As String, ByVal where As String) As Array

        MPSync_process.logStats("MPSync: [getCurrentTableValues] Get current table values from " & table & " in database " & path & database, MessageType.DEBUG)

        Dim records As Integer = RecordCount(path, database, table, where)

        If records > 0 Then

            MPSync_process.logStats("MPSync: [getCurrentTableValues] Load current table values from " & table & " in database " & path & database, MessageType.DEBUG)

            Dim curvalues() As String

            Using SQLconnect As New SQLiteConnection(),
                SQLcommand As SQLiteCommand = SQLconnect.CreateCommand

                SQLconnect.ConnectionString = "Data Source=" & MPSync_process.p_Database(path & database) & ";Read Only=True;"
                SQLconnect.Open()
                SQLcommand.CommandText = "SELECT " & fields & " FROM " & table & " WHERE " & where
                Using SQLreader = SQLcommand.ExecuteReader()
                    SQLreader.Read()

                    Dim i, x, z As Integer

                    curvalues = Nothing

                    For x = 0 To columns.Count - 1
                        z = Array.IndexOf(mps_cols, columns(x).name)
                        If z <> -1 Then
                            If columns(x).name <> pkey Then
                                ReDim Preserve curvalues(i)
                                If Not IsDBNull(SQLreader(i)) Then
                                    curvalues(i) = columns(x).name & "=" & FormatValue(SQLreader(i), columns(x).type)
                                Else
                                    curvalues(i) = columns(x).name & "=" & FormatValue("NULL", columns(x).type)
                                End If
                                i += 1
                            End If
                        End If
                    Next
                End Using

            End Using

            Return curvalues

        Else
            Return Nothing
        End If

    End Function

    Public Function getUpdateValues(ByVal newvalues As String(), ByVal curvalues As String()) As String

        'MPSync_process.logStats("MPSync: [getUpdateValues] Getting update values by comparing existing values with new ones.", MessageType.DEBUG)

        Dim updvalues As String = Nothing

        For x As Integer = 0 To UBound(newvalues)
            If newvalues(x) <> curvalues(x) Then
                updvalues &= newvalues(x)
            End If
        Next

        If updvalues <> Nothing Then updvalues = Left(updvalues, Len(updvalues) - 1)

        Return updvalues

    End Function

    Public Shared Sub bw_db_worker(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs)
        Try
            Dim mps_db As New MPSync_process_DB

            Do

                If Not MPSync_process.CheckPlayerplaying("db") Then

                    MPSync_process.logStats("MPSync: [MPSync_process.WorkMethod][bw_db_worker] DB synchronization cycle starting.", MessageType.LOG)

                    mps_db.bw_sync_db_jobs = 0
                    Array.Resize(mps_db.bw_sync_db, 0)

                    ' direction is client to server or both
                    If MPSync_process._db_direction <> 2 Then
                        mps_db.Process_DB_folder(MPSync_process._db_client, MPSync_process._db_server)
                    End If

                    ' direction is server to client or both
                    If MPSync_process._db_direction <> 1 Then
                        mps_db.Process_DB_folder(MPSync_process._db_server, MPSync_process._db_client)
                    End If

                    If Not MPSync_settings.syncnow Then
                        MPSync_process.logStats("MPSync: [MPSync_process.WorkMethod][bw_db_worker] DB synchronization cycle complete.", MessageType.LOG)
                    Else
                        MPSync_settings.db_complete = True
                        MPSync_process.logStats("MPSync: [MPSync_process.WorkMethod][bw_db_worker] DB synchronization complete.", MessageType.INFO)
                        Exit Do
                    End If

                    MPSync_process.wait(MPSync_process._db_sync, , "DB")

                Else
                    MPSync_process.wait(5, False)
                End If

            Loop

        Catch ex As Exception
            MPSync_process.logStats("MPSync: [MPSync_process.WorkMethod][bw_db_worker] Unexpected error " & ex.Message, MessageType.ERR)
        End Try

    End Sub

    Private Sub Process_DB_folder(ByVal source As String, ByVal target As String)

        If Not IO.Directory.Exists(source) Then
            MPSync_process.logStats("MPSync: [Process_DB_folder] folder " & source & " does not exist", MessageType.ERR)
            Exit Sub
        End If

        MPSync_process.logStats("MPSync: [Process_DB_folder] synchronizing from " & source & " to " & target, MessageType.DEBUG)


        Dim x As Integer
        Dim s_lastwrite, t_lastwrite As Date

        _bw_active_db_jobs = 0
        lastsync = p_lastsync

        Try

            For Each database As String In IO.Directory.GetFiles(source, "*.db3")

                If Not MPSync_process.CheckPlayerplaying("db") Then

                    If IO.Path.GetExtension(database) <> ".db3-journal" Then

                        Dim db As String = IO.Path.GetFileName(database)

                        If MPSync_process._databases.Contains(db) Or MPSync_process._databases.Contains("ALL") Then

                            If MPSync_process.sync_type = "Triggers" Then
                                ProcessTables(source, target, db)
                            Else

                                x = Array.IndexOf(MPSync_process.dbname, db)

                                s_lastwrite = My.Computer.FileSystem.GetFileInfo(database).LastWriteTimeUtc
                                t_lastwrite = My.Computer.FileSystem.GetFileInfo(target & db).LastWriteTimeUtc

                                If MPSync_process.dbinfo(x).LastWriteTimeUtc < s_lastwrite Or MPSync_process.dbinfo(x).LastWriteTimeUtc <> t_lastwrite Then
                                    ProcessTables(source, target, db)
                                Else
                                    MPSync_process.logStats("MPSync: [Process_DB_folder] no changes detected in " & database & ". Skipping synchronization.", MessageType.DEBUG)
                                End If

                            End If

                        End If

                    End If

                Else
                    MPSync_process.logStats("MPSync: [Process_DB_folder] synchronizing from " & source & " to " & target & " aborted as player playing.", MessageType.INFO)
                    Exit For
                End If

            Next
        Catch ex As Exception
            MPSync_process.logStats("MPSync: [Process_DB_folder] Unexpected error " & ex.Message, MessageType.ERR)
        End Try

        If Not MPSync_process.CheckPlayerplaying("db") Then
            p_lastsync = Now.ToLocalTime.ToString("yyyy-MM-dd HH:mm:ss")
            If Not MPSync_process._db_objects.Contains("NOTHING") Then ProcessObject(source, target)
        Else
            _bw_active_db_jobs = 0
        End If

        Do While _bw_active_db_jobs > 0
            Dim jobs As String = String.Join(",", bw_dbs.ToArray())
            If jobs = String.Empty Then Exit Do
            If MPSync_process.CheckPlayerplaying("db") Then
                _bw_active_db_jobs = 0
                MPSync_process.logStats("MPSync: [Process_DB_folder] aborted as player playing.", MessageType.INFO)
            Else
                If MPSync_process.p_Debug Then MPSync_process.logStats("MPSync: [Process_DB_folder] waiting for background threads to finish... " & _bw_active_db_jobs.ToString & " threads remaining processing {" & jobs & "}.", MessageType.DEBUG)
                MPSync_process.wait(10, False)
            End If
        Loop

    End Sub

    Private Function checkbytrigger(ByVal source As String, ByVal target As String, ByVal database As String) As List(Of TableInfo)

        Dim parm As List(Of TableInfo) = New List(Of TableInfo)

        Using SQLconnect As New SQLiteConnection(),
            SQLcommand As SQLiteCommand = SQLconnect.CreateCommand

            MPSync_process.logStats("MPSync: [checkbytrigger] checking for tables in database " & source & database & " that need synchronization...", MessageType.DEBUG)

            SQLconnect.ConnectionString = "Data Source=" & MPSync_process.p_Database(source & database) & ";Read Only=True;"
            SQLconnect.Open()

            Try

                SQLcommand.CommandText = "SELECT tablename FROM mpsync_trigger WHERE lastupdated > '" & lastsync & "' ORDER BY lastupdated, tablename"
                Using SQLreader = SQLcommand.ExecuteReader()

                    While SQLreader.Read()
                        parm.Add(New TableInfo(source, target, database, SQLreader.GetString(0)))

                    End While
                End Using

            Catch ex As Exception
                MPSync_process.logStats("MPSync: [checkbytrigger] Error reading mpsync_trigger from " & target & database & " with exception: " & ex.Message, MessageType.ERR)
            End Try
        End Using

        If MPSync_process.TableExist(target, database, "mpsync") Then

            Using SQLconnect As New SQLiteConnection(),
                SQLcommand As SQLiteCommand = SQLconnect.CreateCommand

                MPSync_process.logStats("MPSync: [checkbytrigger] checking for records in mpsync table in database " & target & database, MessageType.DEBUG)


                SQLconnect.ConnectionString = "Data Source=" & MPSync_process.p_Database(target & database) & ";Read Only=True;"
                SQLconnect.Open()
                SQLcommand.CommandText = "SELECT tablename FROM mpsync ORDER BY mps_lastupdated, tablename"
                Using SQLreader = SQLcommand.ExecuteReader()
                    Try

                        While SQLreader.Read()

                            If Not parm.Any(Function(p) p.name = SQLreader.GetString(0)) Then
                                parm.Add(New TableInfo(source, target, database, SQLreader.GetString(0)))
                            End If

                        End While

                    Catch ex As Exception
                        MPSync_process.logStats("MPSync: [checkbytrigger] Error reading mpsync from " & target & database & " with exception: " & ex.Message, MessageType.ERR)
                    End Try
                End Using
            End Using

        End If


        MPSync_process.logStats("MPSync: [checkbytrigger] " & parm.Count.ToString & " tables in database " & source & database & " need synchronization.", MessageType.DEBUG)

        Return parm

    End Function

    Private Function getDatabaseRecords(ByVal source As String, ByVal target As String, ByVal database As String, ByVal parm As List(Of TableInfo)) As List(Of TableInfo)

        Dim finalparm As List(Of TableInfo) = parm

        If MPSync_process.p_Debug Then MPSync_process.logStats("MPSync: [getDatabaseRecords] database " & database & " tables record count started.", MessageType.DEBUG)

        Using SQLconnect As New SQLiteConnection(),
            SQLcommand1 As SQLiteCommand = SQLconnect.CreateCommand,
            SQLcommand2 As SQLiteCommand = SQLconnect.CreateCommand

            SQLconnect.ConnectionString = "Data Source=" & MPSync_process.p_Database(source & database) & ";Read Only=True;"
            SQLconnect.Open()

            SQLcommand1.CommandText = "ATTACH DATABASE '" & MPSync_process.p_Database(target & database) & "' AS target"
            SQLcommand1.ExecuteNonQuery()

            SQLcommand1.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'mpsync%' AND name NOT LIKE 'sqlite%'"
            Using SQLreader1 = SQLcommand1.ExecuteReader()

                While SQLreader1.Read()

                    SQLcommand2.CommandText = "SELECT CASE WHEN (SELECT COUNT(*) FROM " & SQLreader1.GetString(0) & " EXCEPT SELECT COUNT(*) FROM target." & SQLreader1.GetString(0) & ") IS NULL THEN 0 ELSE 1 END"
                    Using SQLreader2 = SQLcommand2.ExecuteReader()
                        SQLreader2.Read()

                        If Int(SQLreader2(0)) = 1 Then
                            If Not finalparm.Any(Function(f) f.name = SQLreader1.GetString(0)) Then
                                finalparm.Add(New TableInfo(source, target, database, SQLreader1.GetString(0)))
                            End If
                        End If

                    End Using

                End While
            End Using
        End Using

        If MPSync_process.p_Debug Then MPSync_process.logStats("MPSync: [getDatabaseRecords] database " & database & " tables record count complete.", MessageType.DEBUG)

        Return finalparm

    End Function

    Private Sub ProcessTables(ByVal source As String, ByVal target As String, ByVal database As String)

        MPSync_process.logStats("MPSync: [ProcessTables] process for database " & source & database & " started.", MessageType.DEBUG)

        ' execute only once in a day
        If DateDiff(DateInterval.Day, CDate(lastsync), Now) > 0 Then CheckTables(source, target, database)

        Dim parm As SyncDbParams = New SyncDbParams


        If MPSync_process.TableExist(source, database, "mpsync_trigger") Then
            parm.tables = getDatabaseRecords(source, target, database, checkbytrigger(source, target, database))
        Else
            Dim omit As Array = {"mpsync", "mpsync_trigger", "sqlite_sequence", "sqlite_stat1", "sqlite_stat2"}

            Using SQLconnect As New SQLiteConnection(),
                SQLcommand As SQLiteCommand = SQLconnect.CreateCommand

                SQLconnect.ConnectionString = "Data Source=" & MPSync_process.p_Database(source & database) & ";Read Only=True;"
                SQLconnect.Open()

                MPSync_process.logStats("MPSync: [ProcessTables] selecting tables from database " & source & database, MessageType.DEBUG)


                SQLcommand.CommandText = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name"
                Using SQLreader = SQLcommand.ExecuteReader()

                    While SQLreader.Read()

                        If Array.IndexOf(omit, SQLreader.GetString(0)) = -1 Then
                            parm.tables.Add(New TableInfo(source, target, database, SQLreader.GetString(0)))
                        End If

                    End While
                End Using

            End Using

            MPSync_process.logStats("MPSync: [ProcessTables] " & parm.tables.Count.ToString & " table selected from database " & source & database, MessageType.DEBUG)
        End If

        If parm.tables.Count > 0 Then
            ' check if there are available threads to submit current stream, unless there is no limit.

            If MPSync_process.checkThreads("DB") <> -1 Then

                Do While _bw_active_db_jobs >= MPSync_process.checkThreads("DB")
                    MPSync_process.logStats("MPSync: [ProcessTables] waiting for available threads.", MessageType.DEBUG)
                    MPSync_process.wait(10, False)
                Loop

            End If

            ReDim Preserve bw_sync_db(bw_sync_db_jobs)
            bw_sync_db(bw_sync_db_jobs) = New BackgroundWorker
            bw_sync_db(bw_sync_db_jobs).WorkerSupportsCancellation = True
            AddHandler bw_sync_db(bw_sync_db_jobs).DoWork, AddressOf bw_sync_db_worker

            If Not bw_sync_db(bw_sync_db_jobs).IsBusy Then bw_sync_db(bw_sync_db_jobs).RunWorkerAsync(parm)

            bw_sync_db_jobs += 1
            _bw_active_db_jobs += 1
            bw_dbs.Add(database)
        End If

        MPSync_process.logStats("MPSync: [ProcessTables] process for database " & source & database & " complete.", MessageType.DEBUG)

    End Sub

    Private Sub ProcessObject(ByVal source As String, ByVal target As String)

        Dim s_lastwrite, t_lastwrite As Date

        For Each objects As String In IO.Directory.GetFiles(source, "*.*")

            Dim obj As String = IO.Path.GetFileName(objects)

            If MPSync_process._db_objects.Contains(obj) Then

                s_lastwrite = My.Computer.FileSystem.GetFileInfo(objects).LastWriteTimeUtc
                t_lastwrite = My.Computer.FileSystem.GetFileInfo(target & obj).LastWriteTimeUtc

                If s_lastwrite > t_lastwrite Then
                    Try
                        MPSync_process.logStats("MPSync: [ProcessObject] Copying object " & objects & " to " & target & obj, MessageType.LOG)
                        IO.File.Copy(objects, target & obj, True)
                    Catch ex As Exception
                        MPSync_process.logStats("MPSync: [ProcessObject] Error copying " & objects & " with exception: " & ex.Message, MessageType.ERR)
                    End Try
                Else
                    MPSync_process.logStats("MPSync: [ProcessObject] No changes detected in " & objects & ". Skipping copy.", MessageType.DEBUG)
                End If

            End If

        Next

    End Sub

    Private Sub CheckTables(ByVal source As String, ByVal target As String, ByVal database As String)

        MPSync_process.logStats("MPSync: [CheckTables] Check tables structures in database " & database & " started.", MessageType.DEBUG)

        Using SQLconnect As New SQLiteConnection(),
            SQLcommand As SQLiteCommand = SQLconnect.CreateCommand
            Dim s_columns, t_columns As List(Of ColumnInfo)

            SQLconnect.ConnectionString = "Data Source=" & MPSync_process.p_Database(source & database) & ";Read Only=True;"
            SQLconnect.Open()
            SQLcommand.CommandText = "SELECT name, sql FROM sqlite_master WHERE type=""table"""
            Using SQLreader = SQLcommand.ExecuteReader()

                While SQLreader.Read()

                    s_columns = getFields(source, database, SQLreader.GetString(0))
                    t_columns = getFields(target, database, SQLreader.GetString(0))

                    If t_columns Is Nothing Then
                        CreateTable(target, database, SQLreader.GetString(1))
                        t_columns = s_columns
                    End If
                    Dim diff As IEnumerable(Of ColumnInfo) = s_columns.Where(Function(s) Not t_columns.Any(Function(t) t.name = s.name))

                    If diff.Count > 0 Then
                        AddTableMissingFields(target, database, SQLreader.GetString(0), diff)
                    End If

                End While
            End Using

        End Using

        MPSync_process.logStats("MPSync: [CheckTables] Check tables structures in database " & database & " complete.", MessageType.DEBUG)

    End Sub

    Private Sub CreateTable(ByVal path As String, ByVal database As String, ByVal sql As String)

        Using SQLconnect As New SQLiteConnection(),
            SQLcommand As SQLiteCommand = SQLconnect.CreateCommand

            MPSync_process.logStats("MPSync: [Createtable] " & sql & " in database " & path & database, MessageType.DEBUG)

            Try
                SQLconnect.ConnectionString = "Data Source=" & MPSync_process.p_Database(path & database)
                SQLconnect.Open()
                SQLcommand.CommandText = sql
                SQLcommand.ExecuteNonQuery()
            Catch ex As Exception
                MPSync_process.logStats("MPSync: [Createtable] " & sql & " error with exception: " & ex.Message, MessageType.ERR)
            End Try

        End Using

    End Sub

    Private Sub AddTableMissingFields(ByVal path As String, ByVal database As String, ByVal table As String, ByVal missing As IEnumerable(Of ColumnInfo))

        Dim SQL As String
        Using SQLconnect As New SQLiteConnection(),
            SQLcommand As SQLiteCommand = SQLconnect.CreateCommand

            MPSync_process.logStats("MPSync: [AddTableMissingFields] Adding missing columns on target for table " & table & " in database " & database, MessageType.LOG)

            SQLconnect.ConnectionString = "Data Source=" & MPSync_process.p_Database(path & database)
            SQLconnect.Open()

            For x As Integer = 0 To missing.Count - 1

                SQL = "ALTER TABLE " & table & " ADD COLUMN " & missing(x).name & " " & missing(x).type & " "

                If missing(x).notNull Then SQL &= "NOT NULL "

                Try
                    MPSync_process.logStats("MPSync: [AddTableMissingFields] " & SQL, MessageType.DEBUG)
                    SQLcommand.CommandText = SQL
                    SQLcommand.ExecuteNonQuery()
                Catch ex As Exception
                    MPSync_process.logStats("MPSync: [AddTableMissingFields] " & SQL & " error with exception: " & ex.Message, MessageType.DEBUG)
                    MPSync_process.logStats("MPSync: [AddTableMissingFields] Error adding field " & missing(x).name & " to table " & table & " in " & path & database, MessageType.ERR)
                End Try

            Next

        End Using

    End Sub

    Private Sub bw_sync_db_worker(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs)

        Try

            If e.Argument.GetType() Is GetType(SyncDbParams) Then
                Dim parm As SyncDbParams = CType(e.Argument, SyncDbParams)

                MPSync_process.logStats("MPSync: [ProcessTables][bw_sync_db_worker] background synchronization of " & parm.tables(0).database & " database started.", MessageType.LOG)

                Dim x As Integer

                For x = 0 To parm.tables.Count - 1
                    db_worker(parm.tables(x).source, parm.tables(x).target, parm.tables(x).database, parm.tables(x).name)
                Next

                If MPSync_process.sync_type = "Timestamp" Then

                    Dim s_lastwrite As Date = My.Computer.FileSystem.GetFileInfo(parm.tables(0).source & parm.tables(0).database).LastWriteTimeUtc
                    Dim t_lastwrite As Date = My.Computer.FileSystem.GetFileInfo(parm.tables(0).target & parm.tables(0).database).LastWriteTimeUtc

                    x = Array.IndexOf(MPSync_process.dbname, parm.tables(0).database)

                    If s_lastwrite > t_lastwrite Then
                        MPSync_process.dbinfo(x).LastWriteTimeUtc = s_lastwrite
                    ElseIf s_lastwrite < t_lastwrite Then
                        MPSync_process.dbinfo(x).LastWriteTimeUtc = t_lastwrite
                    End If

                End If

                _bw_active_db_jobs -= 1
                bw_dbs.RemoveAt(bw_dbs.IndexOf(parm.tables(0).database))

                MPSync_process.logStats("MPSync: [ProcessTables][bw_sync_db_worker] background synchronization of " & parm.tables(0).database & " database completed.", MessageType.LOG)
            End If


        Catch ex As Exception
            MPSync_process.logStats("MPSync: [ProcessTables][bw_sync_db_worker] Unexpected error " & ex.Message, MessageType.ERR)
        End Try

    End Sub

    Private Sub db_worker(ByVal s_path As String, ByVal t_path As String, ByVal database As String, ByVal table As String)

        Dim columns As List(Of ColumnInfo) = Nothing

        MPSync_process.logStats("MPSync: [db_worker] synchronization of table " & table & " in database " & t_path & database & " started.", MessageType.LOG)

        If MPSync_process.check_watched Then
            ' check if master client
            If MPSync_process._db_direction <> 2 Then
                UpdateMaster(s_path, t_path, database, table, True)
            Else
                UpdateSlave(t_path, s_path, database, table)
            End If
        End If

        columns = getFields(s_path, database, table)

        If Synchronize_DB(s_path, t_path, database, table, columns, MPSync_process._db_sync_method) Then
            MPSync_process.logStats("MPSync: [db_worker] synchronization of table " & table & " in database " & t_path & database & " complete.", MessageType.LOG)
        Else
            MPSync_process.logStats("MPSync: [db_worker] synchronization of table " & table & " in database " & t_path & database & " failed.", MessageType.LOG)
        End If

    End Sub

    Public Function Synchronize_DB(ByVal s_path As String, ByVal t_path As String, ByVal database As String, ByVal table As String, ByVal columns As List(Of ColumnInfo), ByVal method As Integer) As Boolean

        MPSync_process.logStats("MPSync: [Synchronize_DB] synchronization of table " & table.Replace("~", String.Empty) & " in database " & t_path & database & " in progress...", MessageType.LOG)

        If DeleteRecords(s_path, t_path, database, table, columns, method) Then Return InsertRecords(s_path, t_path, database, table, columns, method)

        Return False

    End Function

    Private Sub UpdateSlave(ByVal source As String, ByVal target As String, ByVal database As String, ByVal table As String)

        Dim mps As New MPSync_settings

        Dim x As Integer = Array.IndexOf(mps.getDatabase, database)

        If x <> -1 Then
            If Array.IndexOf(mps.getTables(database), table) <> -1 Then

                MPSync_process.logStats("MPSync: [UpdateSlave] synchronization of mpsync for table " & table & " in database " & source & database & " in progress...", MessageType.LOG)

                Dim columns As List(Of ColumnInfo) = getFields(source, database, "mpsync")

                If Synchronize_DB(source, target, database, "~mpsync~", columns, 1) Then UpdateMaster(source, target, database, table)

                MPSync_process.logStats("MPSync: [UpdateSlave] synchronization of mpsync for table " & table & " in database " & source & database & " complete.", MessageType.LOG)

            End If
        End If

    End Sub

    Private Sub UpdateMaster(ByVal source As String, ByVal target As String, ByVal database As String, ByVal table As String, Optional ByVal master As Boolean = False)

        Dim mps As New MPSync_settings

        Dim x As Integer = Array.IndexOf(mps.getDatabase, database)

        If x <> -1 Then
            If Array.IndexOf(mps.getTables(database), table) <> -1 Then

                MPSync_process.logStats("MPSync: [UpdateMaster] synchronization of watched for table " & table & " in database " & source & database & " in progress...", MessageType.LOG)

                Dim mps_columns As List(Of ColumnInfo) = Nothing
                Dim columns As List(Of ColumnInfo)
                Dim s_data, t_data, w_values As Array

                s_data = LoadTable(source, database, "mpsync", mps_columns, "tablename = '" & table & "'", "mps_lastupdated")
                t_data = LoadTable(target, database, "mpsync", mps_columns, "tablename = '" & table & "'", "mps_lastupdated")

                If s_data Is Nothing And t_data Is Nothing Then
                    MPSync_process.logStats("MPSync: [UpdateMaster] synchronization of watched for table " & table & " in database " & source & database & " nothing to update.", MessageType.LOG)
                    Exit Sub
                End If

                columns = getFields(source, database, table)
                w_values = BuildUpdateArray_mpsync(t_data, s_data, mps_columns, columns)

                If master Then
                    UpdateRecords_mpsync(source, database, table, w_values, mps_columns, columns)
                    Cleanup_mpsync(target, database, t_data)
                Else
                    UpdateRecords_mpsync(target, database, table, w_values, mps_columns, columns)
                    Cleanup_mpsync(source, database, s_data)
                End If

                MPSync_process.logStats("MPSync: [UpdateMaster] synchronization of watched for table " & table & " in database " & source & database & " complete.", MessageType.LOG)

            End If
        End If

    End Sub

    Private Sub UpdateRecords_mpsync(ByVal path As String, ByVal database As String, ByVal table As String, ByVal w_values As Array, ByVal table_columns As List(Of ColumnInfo), ByVal columns As List(Of ColumnInfo))

        If w_values.OfType(Of String)().ToArray().Length = 0 Then Exit Sub

        Dim i, x, z As Integer
        Dim pkey As String = Nothing
        Dim updcols As Array = table_columns.Select(Function(t) t.name).ToArray
        Dim fields, updvalues, where, update(), a_values() As String
        Using SQLconnect As New SQLiteConnection(),
            SQLcommand As SQLiteCommand = SQLconnect.CreateCommand

            x = getPK(columns, pkey)

            SQLconnect.ConnectionString = "Data Source=" & MPSync_process.p_Database(path & database)
            SQLconnect.Open()
            SQLcommand.CommandText = "PRAGMA temp_store=2;PRAGMA journal_mode=off;PRAGMA synchronous=off;"
            SQLcommand.ExecuteNonQuery()

            For y As Integer = 0 To UBound(w_values, 2)

                i = 0
                fields = Nothing
                update = Nothing
                where = Nothing
                a_values = Split(w_values(1, y), dlm)

                For x = 0 To columns.Count - 1
                    z = Array.IndexOf(updcols, columns(x).name)
                    If z <> -1 Then
                        If columns(x).name <> pkey Then
                            ReDim Preserve update(i)
                            update(i) = columns(x).name & "=" & FormatValue(a_values(z), columns(x).type)
                            fields &= columns(x).name & ","
                            i += 1
                        Else
                            where = pkey & " = " & FormatValue(a_values(z), columns(x).type)
                        End If
                    End If
                Next

                fields = Left(fields, Len(fields) - 1)
                where = Left(where, Len(where) - 1)

                ' get update values from table and compare if anything changed
                Dim curvalues() As String

                curvalues = getCurrentTableValues(path, database, table, columns, updcols, pkey, fields, where)

                If curvalues IsNot Nothing Then

                    ' construct update clause
                    updvalues = getUpdateValues(update, curvalues)

                    If updvalues <> Nothing Then

                        Try
                            MPSync_process.logStats("MPSync: [UpdateRecords_mpsync] UPDATE " & table & " SET " & updvalues & " WHERE " & where, MessageType.DEBUG)
                            SQLcommand.CommandText = "UPDATE " & table & " SET " & updvalues & " WHERE " & where
                            SQLcommand.ExecuteNonQuery()
                        Catch ex As Exception
                            MPSync_process.logStats("MPSync: [UpdateRecords_mpsync] SQL statement [" & (SQLcommand.CommandText).Replace("""", "'") & "] on " & path & database & " failed with exception: " & ex.Message, MessageType.DEBUG)
                            MPSync_process.logStats("MPSync: [UpdateRecords_mpsync] Error synchronizing table " & table & " in database " & path & database, MessageType.ERR)
                        End Try

                    End If
                End If

            Next

        End Using

    End Sub

    Private Function GetColumnsToCheck(ByVal tablename As String, ByVal columns As List(Of ColumnInfo)) As String
        If Not MPSync_process.check_watched And tablename = "user_movie_settings" Then
            Dim colArray As String() = columns.Where(Function(c) c.name <> "watched" And Not c.name Like "resume*").Select(Function(c2) c2.name).ToArray
            Return String.Join(",", colArray)
        Else
            Return "*"
        End If
    End Function

    Private Function InsertRecords(ByVal s_path As String, ByVal t_path As String, ByVal database As String, ByVal table As String, ByVal columns As List(Of ColumnInfo), ByVal method As Integer) As Boolean

        ' propagate additions
        If method = 2 Then Return True

        ' do not process these tables
        Dim omit As Array = {"mpsync", "mpsync_trigger", "sqlite_sequence", "sqlite_stat1", "sqlite_stat2"}

        If Array.IndexOf(omit, table) <> -1 Then Return True

        table = table.Replace("~", String.Empty)

        Dim rtc As Boolean = True

        If Not MPSync_process.CheckPlayerplaying("db") Then

            MPSync_process.logStats("MPSync: [InsertRecords] adding missing entries on target for table " & table & " in database " & t_path & database, MessageType.DEBUG)

            Using SQLconnect As New SQLiteConnection(),
                SQLmemory As New SQLiteConnection(),
                SQLcommand As SQLiteCommand = SQLmemory.CreateCommand

                Dim pkey As String = Nothing
                Dim y As Integer = 0

                SQLconnect.ConnectionString = "Data Source=" & MPSync_process.p_Database(s_path & database) & ";Read Only=True;"
                SQLconnect.Open()

                SQLmemory.ConnectionString = "Data Source=:memory:"
                SQLmemory.Open()

                SQLconnect.BackupDatabase(SQLmemory, "main", "main", -1, Nothing, 0)

                SQLconnect.Close()
                SQLconnect.Dispose()

                SQLcommand.CommandText = "PRAGMA temp_store=2;PRAGMA journal_mode=off;PRAGMA synchronous=off;"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.CommandText = "ATTACH DATABASE '" & MPSync_process.p_Database(t_path & database) & "' AS target"
                SQLcommand.ExecuteNonQuery()

                Try
                    If getPK(columns, pkey) = -1 Then pkey = columns(0).name

                    Dim cols As String = GetColumnsToCheck(table, columns)
                    SQLcommand.CommandText = "INSERT INTO target." & table & " SELECT * FROM " & table &
                        " WHERE " & pkey & " in ( SELECT " & pkey & " FROM ( SELECT " & cols & " FROM " & table & " EXCEPT SELECT " & cols & " FROM target." & table & ") )"

                    MPSync_process.logStats("MPSync: [InsertRecords] " & SQLcommand.CommandText, MessageType.DEBUG)

                    SQLcommand.ExecuteNonQuery()

                    SQLcommand.CommandText = "SELECT CASE WHEN MAX(CHANGES()) IS NULL THEN 0 ELSE MAX(CHANGES()) END FROM target." & table
                    Using SQLreader = SQLcommand.ExecuteReader()
                        SQLreader.Read()
                        y = SQLreader.GetInt32(0)
                    End Using

                    MPSync_process.logStats("MPSync: [InsertRecords] " & y.ToString & " records added in " & table & " in database " & t_path & database, MessageType.DEBUG)
                Catch ex As Exception
                    MPSync_process.logStats("MPSync: [InsertRecords] SQL statement [" & (SQLcommand.CommandText).Replace("""", "'") & "] on " & t_path & database & " failed with exception: " & ex.Message, MessageType.DEBUG)
                    MPSync_process.logStats("MPSync: [InsertRecords] Error adding record to table " & table & " in database " & t_path & database & " with exception: " & ex.Message, MessageType.ERR)
                    rtc = False
                End Try

            End Using
        End If

        Return rtc

    End Function

    Private Function DeleteRecords(ByVal s_path As String, ByVal t_path As String, ByVal database As String, ByVal table As String, ByVal columns As List(Of ColumnInfo), ByVal method As Integer) As Boolean

        ' propagate deletions
        If method = 1 Or table = "mpsync" Then Return True

        table = table.Replace("~", String.Empty)

        Dim rtc As Boolean = True

        If Not MPSync_process.CheckPlayerplaying("db") Then

            MPSync_process.logStats("MPSync: [DeleteRecords] deleting extra entries on target for table " & table & " in database " & t_path & database, MessageType.DEBUG)

            Using SQLconnect As New SQLiteConnection(),
                SQLmemory As New SQLiteConnection(),
                SQLcommand As SQLiteCommand = SQLmemory.CreateCommand

                Dim pkey As String = Nothing
                Dim y As Integer = 0

                SQLconnect.ConnectionString = "Data Source=" & MPSync_process.p_Database(s_path & database) & ";Read Only=True;"
                SQLconnect.Open()

                SQLmemory.ConnectionString = "Data Source=:memory:"
                SQLmemory.Open()

                SQLconnect.BackupDatabase(SQLmemory, "main", "main", -1, Nothing, 0)

                SQLconnect.Close()
                SQLconnect.Dispose()

                SQLcommand.CommandText = "PRAGMA temp_store=2;PRAGMA journal_mode=off;PRAGMA synchronous=off;"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.CommandText = "ATTACH DATABASE '" & MPSync_process.p_Database(t_path & database) & "' AS target"
                SQLcommand.ExecuteNonQuery()

                Try
                    If getPK(columns, pkey) = -1 Then pkey = columns(0).name

                    Dim cols As String = GetColumnsToCheck(table, columns)
                    SQLcommand.CommandText = "DELETE FROM target." & table & " WHERE " & pkey & " IN (SELECT " & pkey & " FROM (SELECT " & cols & " FROM target." & table & " EXCEPT SELECT " & cols & " FROM " & table & "))"

                    MPSync_process.logStats("MPSync: [DeleteRecords] " & SQLcommand.CommandText, MessageType.DEBUG)

                    SQLcommand.ExecuteNonQuery()

                    SQLcommand.CommandText = "SELECT CASE WHEN MAX(CHANGES()) IS NULL THEN 0 ELSE MAX(CHANGES()) END FROM target." & table
                    Using SQLreader = SQLcommand.ExecuteReader()
                        SQLreader.Read()
                        y = SQLreader.GetInt32(0)
                    End Using

                    MPSync_process.logStats("MPSync: [DeleteRecords] " & y.ToString & " records deleted from " & table & " in database " & t_path & database, MessageType.DEBUG)
                Catch ex As Exception
                    MPSync_process.logStats("MPSync: [DeleteRecords] SQL statement [" & (SQLcommand.CommandText).Replace("""", "'") & "] on " & t_path & database & " failed with exception: " & ex.Message, MessageType.DEBUG)
                    MPSync_process.logStats("MPSync: Error [DeleteRecords] deleting records from table " & table & " in database " & t_path & database & " with exception: " & ex.Message, MessageType.ERR)
                    rtc = False
                End Try

            End Using

        End If

        Return rtc

    End Function

    Private Sub Cleanup_mpsync(ByVal path As String, ByVal database As String, ByVal data As Array)

        If data.OfType(Of String)().ToArray().Length = 0 Or data(0, 0) = Nothing Then Exit Sub

        Using SQLconnect As New SQLiteConnection(),
            SQLcommand As SQLiteCommand = SQLconnect.CreateCommand

            SQLconnect.ConnectionString = "Data Source=" & MPSync_process.p_Database(path & database)
            SQLconnect.Open()

            Try
                For y = 0 To UBound(data, 2)
                    MPSync_process.logStats("MPSync: [Cleanup_mpsync] DELETE FROM mpsync WHERE rowid = " & data(0, y), MessageType.DEBUG)
                    SQLcommand.CommandText = "DELETE FROM mpsync WHERE rowid = " & data(0, y)
                    SQLcommand.ExecuteNonQuery()
                Next
            Catch ex As Exception
                MPSync_process.logStats("MPSync: [Cleanup_mpsync] SQL statement [" & (SQLcommand.CommandText).Replace("""", "'") & "] failed with exception: " & ex.Message, MessageType.DEBUG)
                MPSync_process.logStats("MPSync: [Cleanup_mpsync] Error deleting record from table mpsync in database " & path & database & " with exception: " & ex.Message, MessageType.ERR)
            End Try

        End Using

    End Sub

End Class
