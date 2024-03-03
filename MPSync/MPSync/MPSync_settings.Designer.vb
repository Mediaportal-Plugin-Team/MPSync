<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MPSync_settings
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified imports the Windows Form Designer.  
    'Do not modify it imports the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MPSync_settings))
        Me.b_save = New System.Windows.Forms.Button()
        Me.tb_master_path = New System.Windows.Forms.TextBox()
        Me.tb_slave_path = New System.Windows.Forms.TextBox()
        Me.l_copyright = New System.Windows.Forms.Label()
        Me.b_sync_now = New System.Windows.Forms.Button()
        Me.tc_main = New System.Windows.Forms.TabControl()
        Me.tp_settings = New System.Windows.Forms.TabPage()
        Me.tc_settings = New System.Windows.Forms.TabControl()
        Me.tp_selection = New System.Windows.Forms.TabPage()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.PictureBox2 = New System.Windows.Forms.PictureBox()
        Me.cb_folders = New System.Windows.Forms.CheckBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.cb_databases = New System.Windows.Forms.CheckBox()
        Me.rb_timestamp = New System.Windows.Forms.RadioButton()
        Me.rb_triggers = New System.Windows.Forms.RadioButton()
        Me.tp_configuration = New System.Windows.Forms.TabPage()
        Me.Panel7 = New System.Windows.Forms.Panel()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.nud_max_folder_streams = New System.Windows.Forms.NumericUpDown()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.nud_max_DB_streams = New System.Windows.Forms.NumericUpDown()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.cb_debug = New System.Windows.Forms.CheckBox()
        Me.tp_database = New System.Windows.Forms.TabPage()
        Me.tc_database = New System.Windows.Forms.TabControl()
        Me.tp_db_paths = New System.Windows.Forms.TabPage()
        Me.cb_db_sync_method = New System.Windows.Forms.ComboBox()
        Me.b_db_direction = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.b_db_server = New System.Windows.Forms.Button()
        Me.tb_db_server_path = New System.Windows.Forms.TextBox()
        Me.tb_db_client_path = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.b_db_client = New System.Windows.Forms.Button()
        Me.tp_db_advancedsettings = New System.Windows.Forms.TabPage()
        Me.cb_vacuum = New System.Windows.Forms.CheckBox()
        Me.cb_watched = New System.Windows.Forms.CheckBox()
        Me.cb_db_pause = New System.Windows.Forms.CheckBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.rb_specific_db = New System.Windows.Forms.RadioButton()
        Me.rb_all_db = New System.Windows.Forms.RadioButton()
        Me.cb_db_sync = New System.Windows.Forms.ComboBox()
        Me.nud_db_sync = New System.Windows.Forms.NumericUpDown()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.clb_databases = New System.Windows.Forms.CheckedListBox()
        Me.tp_other = New System.Windows.Forms.TabPage()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.rb_o_nothing = New System.Windows.Forms.RadioButton()
        Me.rb_o_specific = New System.Windows.Forms.RadioButton()
        Me.rb_o_all = New System.Windows.Forms.RadioButton()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.clb_db_objects = New System.Windows.Forms.CheckedListBox()
        Me.tp_watched = New System.Windows.Forms.TabPage()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.rb_w_specific = New System.Windows.Forms.RadioButton()
        Me.rb_w_all = New System.Windows.Forms.RadioButton()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.clb_watched = New System.Windows.Forms.CheckedListBox()
        Me.tp_folders = New System.Windows.Forms.TabPage()
        Me.tc_objects = New System.Windows.Forms.TabControl()
        Me.tp_list = New System.Windows.Forms.TabPage()
        Me.b_delete = New System.Windows.Forms.Button()
        Me.b_edit = New System.Windows.Forms.Button()
        Me.b_add = New System.Windows.Forms.Button()
        Me.clb_object_list = New System.Windows.Forms.CheckedListBox()
        Me.tp_paths = New System.Windows.Forms.TabPage()
        Me.cb_folders_sync_method = New System.Windows.Forms.ComboBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.b_folders_server = New System.Windows.Forms.Button()
        Me.tb_folders_server_path = New System.Windows.Forms.TextBox()
        Me.tb_folders_client_path = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.b_folders_client = New System.Windows.Forms.Button()
        Me.b_folders_direction = New System.Windows.Forms.Button()
        Me.tp_advancedsettings = New System.Windows.Forms.TabPage()
        Me.cb_folders_crc32 = New System.Windows.Forms.CheckBox()
        Me.cb_folders_md5 = New System.Windows.Forms.CheckBox()
        Me.b_apply = New System.Windows.Forms.Button()
        Me.cb_folders_pause = New System.Windows.Forms.CheckBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.rb_specific_folders = New System.Windows.Forms.RadioButton()
        Me.rb_all_folders = New System.Windows.Forms.RadioButton()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.clb_objects = New System.Windows.Forms.CheckedListBox()
        Me.tp_syncnow = New System.Windows.Forms.TabPage()
        Me.lb_status = New System.Windows.Forms.ListBox()
        Me.tt_folders_md5 = New System.Windows.Forms.ToolTip(Me.components)
        Me.tc_main.SuspendLayout()
        Me.tp_settings.SuspendLayout()
        Me.tc_settings.SuspendLayout()
        Me.tp_selection.SuspendLayout()
        Me.Panel5.SuspendLayout()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tp_configuration.SuspendLayout()
        Me.Panel7.SuspendLayout()
        CType(Me.nud_max_folder_streams, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nud_max_DB_streams, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel2.SuspendLayout()
        Me.tp_database.SuspendLayout()
        Me.tc_database.SuspendLayout()
        Me.tp_db_paths.SuspendLayout()
        Me.tp_db_advancedsettings.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        CType(Me.nud_db_sync, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tp_other.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.tp_watched.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.tp_folders.SuspendLayout()
        Me.tc_objects.SuspendLayout()
        Me.tp_list.SuspendLayout()
        Me.tp_paths.SuspendLayout()
        Me.tp_advancedsettings.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.tp_syncnow.SuspendLayout()
        Me.SuspendLayout()
        '
        'b_save
        '
        Me.b_save.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.b_save.Location = New System.Drawing.Point(294, 192)
        Me.b_save.Name = "b_save"
        Me.b_save.Size = New System.Drawing.Size(128, 23)
        Me.b_save.TabIndex = 58
        Me.b_save.Text = "Save Configuration"
        Me.b_save.UseVisualStyleBackColor = True
        '
        'tb_master_path
        '
        Me.tb_master_path.Location = New System.Drawing.Point(12, 45)
        Me.tb_master_path.Name = "tb_master_path"
        Me.tb_master_path.Size = New System.Drawing.Size(492, 20)
        Me.tb_master_path.TabIndex = 51
        '
        'tb_slave_path
        '
        Me.tb_slave_path.Location = New System.Drawing.Point(15, 143)
        Me.tb_slave_path.Name = "tb_slave_path"
        Me.tb_slave_path.Size = New System.Drawing.Size(492, 20)
        Me.tb_slave_path.TabIndex = 54
        '
        'l_copyright
        '
        Me.l_copyright.AutoSize = True
        Me.l_copyright.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.l_copyright.ForeColor = System.Drawing.SystemColors.ActiveCaption
        Me.l_copyright.Location = New System.Drawing.Point(2, 256)
        Me.l_copyright.Name = "l_copyright"
        Me.l_copyright.Size = New System.Drawing.Size(160, 13)
        Me.l_copyright.TabIndex = 61
        Me.l_copyright.Text = "Copyright © 2013-2018, m3rcury"
        '
        'b_sync_now
        '
        Me.b_sync_now.Enabled = False
        Me.b_sync_now.Location = New System.Drawing.Point(160, 192)
        Me.b_sync_now.Name = "b_sync_now"
        Me.b_sync_now.Size = New System.Drawing.Size(128, 23)
        Me.b_sync_now.TabIndex = 62
        Me.b_sync_now.Text = "Synchronise Now"
        Me.b_sync_now.UseVisualStyleBackColor = True
        '
        'tc_main
        '
        Me.tc_main.Controls.Add(Me.tp_settings)
        Me.tc_main.Controls.Add(Me.tp_database)
        Me.tc_main.Controls.Add(Me.tp_folders)
        Me.tc_main.Controls.Add(Me.tp_syncnow)
        Me.tc_main.Location = New System.Drawing.Point(5, 3)
        Me.tc_main.Name = "tc_main"
        Me.tc_main.SelectedIndex = 0
        Me.tc_main.Size = New System.Drawing.Size(590, 250)
        Me.tc_main.TabIndex = 64
        '
        'tp_settings
        '
        Me.tp_settings.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.tp_settings.Controls.Add(Me.tc_settings)
        Me.tp_settings.Controls.Add(Me.b_save)
        Me.tp_settings.Controls.Add(Me.b_sync_now)
        Me.tp_settings.Location = New System.Drawing.Point(4, 22)
        Me.tp_settings.Name = "tp_settings"
        Me.tp_settings.Padding = New System.Windows.Forms.Padding(3)
        Me.tp_settings.Size = New System.Drawing.Size(582, 224)
        Me.tp_settings.TabIndex = 3
        Me.tp_settings.Text = "Settings"
        '
        'tc_settings
        '
        Me.tc_settings.Controls.Add(Me.tp_selection)
        Me.tc_settings.Controls.Add(Me.tp_configuration)
        Me.tc_settings.Location = New System.Drawing.Point(3, 7)
        Me.tc_settings.Name = "tc_settings"
        Me.tc_settings.SelectedIndex = 0
        Me.tc_settings.Size = New System.Drawing.Size(576, 178)
        Me.tc_settings.TabIndex = 68
        '
        'tp_selection
        '
        Me.tp_selection.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.tp_selection.Controls.Add(Me.Panel5)
        Me.tp_selection.Controls.Add(Me.Panel1)
        Me.tp_selection.Location = New System.Drawing.Point(4, 22)
        Me.tp_selection.Name = "tp_selection"
        Me.tp_selection.Padding = New System.Windows.Forms.Padding(3)
        Me.tp_selection.Size = New System.Drawing.Size(568, 152)
        Me.tp_selection.TabIndex = 0
        Me.tp_selection.Text = "Sync Selection"
        '
        'Panel5
        '
        Me.Panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel5.Controls.Add(Me.PictureBox2)
        Me.Panel5.Controls.Add(Me.cb_folders)
        Me.Panel5.Location = New System.Drawing.Point(12, 80)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(545, 62)
        Me.Panel5.TabIndex = 69
        '
        'PictureBox2
        '
        Me.PictureBox2.Image = Global.MPSync.My.Resources.Resources.thumbs
        Me.PictureBox2.Location = New System.Drawing.Point(3, 5)
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.Size = New System.Drawing.Size(60, 50)
        Me.PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox2.TabIndex = 69
        Me.PictureBox2.TabStop = False
        '
        'cb_folders
        '
        Me.cb_folders.AutoSize = True
        Me.cb_folders.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cb_folders.Location = New System.Drawing.Point(176, 20)
        Me.cb_folders.Name = "cb_folders"
        Me.cb_folders.Size = New System.Drawing.Size(194, 20)
        Me.cb_folders.TabIndex = 66
        Me.cb_folders.Text = "Synchronise specific folders"
        Me.cb_folders.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel1.Controls.Add(Me.PictureBox1)
        Me.Panel1.Controls.Add(Me.cb_databases)
        Me.Panel1.Controls.Add(Me.rb_timestamp)
        Me.Panel1.Controls.Add(Me.rb_triggers)
        Me.Panel1.Location = New System.Drawing.Point(12, 10)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(545, 62)
        Me.Panel1.TabIndex = 68
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = Global.MPSync.My.Resources.Resources.database
        Me.PictureBox1.Location = New System.Drawing.Point(4, 5)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(50, 50)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 68
        Me.PictureBox1.TabStop = False
        '
        'cb_databases
        '
        Me.cb_databases.AutoSize = True
        Me.cb_databases.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cb_databases.Location = New System.Drawing.Point(171, 7)
        Me.cb_databases.Name = "cb_databases"
        Me.cb_databases.Size = New System.Drawing.Size(201, 20)
        Me.cb_databases.TabIndex = 65
        Me.cb_databases.Text = "Synchronise Database folder"
        Me.cb_databases.UseVisualStyleBackColor = True
        '
        'rb_timestamp
        '
        Me.rb_timestamp.AutoSize = True
        Me.rb_timestamp.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rb_timestamp.Location = New System.Drawing.Point(256, 33)
        Me.rb_timestamp.Name = "rb_timestamp"
        Me.rb_timestamp.Size = New System.Drawing.Size(225, 20)
        Me.rb_timestamp.TabIndex = 67
        Me.rb_timestamp.TabStop = True
        Me.rb_timestamp.Text = "Synchronize using file Timestamp"
        Me.rb_timestamp.UseVisualStyleBackColor = True
        '
        'rb_triggers
        '
        Me.rb_triggers.AutoSize = True
        Me.rb_triggers.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rb_triggers.Location = New System.Drawing.Point(62, 33)
        Me.rb_triggers.Name = "rb_triggers"
        Me.rb_triggers.Size = New System.Drawing.Size(188, 20)
        Me.rb_triggers.TabIndex = 66
        Me.rb_triggers.TabStop = True
        Me.rb_triggers.Text = "Synchronize using Triggers"
        Me.rb_triggers.UseVisualStyleBackColor = True
        '
        'tp_configuration
        '
        Me.tp_configuration.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.tp_configuration.Controls.Add(Me.Panel7)
        Me.tp_configuration.Controls.Add(Me.Panel2)
        Me.tp_configuration.Location = New System.Drawing.Point(4, 22)
        Me.tp_configuration.Name = "tp_configuration"
        Me.tp_configuration.Padding = New System.Windows.Forms.Padding(3)
        Me.tp_configuration.Size = New System.Drawing.Size(568, 152)
        Me.tp_configuration.TabIndex = 1
        Me.tp_configuration.Text = "Configuration"
        '
        'Panel7
        '
        Me.Panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel7.Controls.Add(Me.Label15)
        Me.Panel7.Controls.Add(Me.nud_max_folder_streams)
        Me.Panel7.Controls.Add(Me.Label14)
        Me.Panel7.Controls.Add(Me.Label13)
        Me.Panel7.Controls.Add(Me.nud_max_DB_streams)
        Me.Panel7.Location = New System.Drawing.Point(12, 33)
        Me.Panel7.Name = "Panel7"
        Me.Panel7.Size = New System.Drawing.Size(545, 40)
        Me.Panel7.TabIndex = 73
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label15.Location = New System.Drawing.Point(310, 11)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(60, 16)
        Me.Label15.TabIndex = 7
        Me.Label15.Text = "Folders :"
        '
        'nud_max_folder_streams
        '
        Me.nud_max_folder_streams.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.nud_max_folder_streams.Location = New System.Drawing.Point(373, 8)
        Me.nud_max_folder_streams.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.nud_max_folder_streams.Name = "nud_max_folder_streams"
        Me.nud_max_folder_streams.Size = New System.Drawing.Size(43, 22)
        Me.nud_max_folder_streams.TabIndex = 6
        Me.nud_max_folder_streams.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label14.Location = New System.Drawing.Point(419, 11)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(77, 16)
        Me.Label14.TabIndex = 5
        Me.Label14.Text = "[0 - No limit]"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label13.Location = New System.Drawing.Point(47, 11)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(195, 16)
        Me.Label13.TabIndex = 4
        Me.Label13.Text = "Maximum Threads - Database :"
        '
        'nud_max_DB_streams
        '
        Me.nud_max_DB_streams.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.nud_max_DB_streams.Location = New System.Drawing.Point(242, 8)
        Me.nud_max_DB_streams.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.nud_max_DB_streams.Name = "nud_max_DB_streams"
        Me.nud_max_DB_streams.Size = New System.Drawing.Size(43, 22)
        Me.nud_max_DB_streams.TabIndex = 0
        Me.nud_max_DB_streams.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Panel2
        '
        Me.Panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel2.Controls.Add(Me.cb_debug)
        Me.Panel2.Location = New System.Drawing.Point(12, 79)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(545, 40)
        Me.Panel2.TabIndex = 68
        '
        'cb_debug
        '
        Me.cb_debug.AutoSize = True
        Me.cb_debug.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cb_debug.Location = New System.Drawing.Point(196, 9)
        Me.cb_debug.Name = "cb_debug"
        Me.cb_debug.Size = New System.Drawing.Size(150, 20)
        Me.cb_debug.TabIndex = 65
        Me.cb_debug.Text = "Enable DEBUG logs"
        Me.cb_debug.UseVisualStyleBackColor = True
        '
        'tp_database
        '
        Me.tp_database.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.tp_database.Controls.Add(Me.tc_database)
        Me.tp_database.Location = New System.Drawing.Point(4, 22)
        Me.tp_database.Name = "tp_database"
        Me.tp_database.Padding = New System.Windows.Forms.Padding(3)
        Me.tp_database.Size = New System.Drawing.Size(582, 224)
        Me.tp_database.TabIndex = 1
        Me.tp_database.Text = "Database"
        '
        'tc_database
        '
        Me.tc_database.Controls.Add(Me.tp_db_paths)
        Me.tc_database.Controls.Add(Me.tp_db_advancedsettings)
        Me.tc_database.Controls.Add(Me.tp_other)
        Me.tc_database.Controls.Add(Me.tp_watched)
        Me.tc_database.Location = New System.Drawing.Point(5, 8)
        Me.tc_database.Name = "tc_database"
        Me.tc_database.SelectedIndex = 0
        Me.tc_database.Size = New System.Drawing.Size(573, 210)
        Me.tc_database.TabIndex = 64
        '
        'tp_db_paths
        '
        Me.tp_db_paths.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.tp_db_paths.Controls.Add(Me.cb_db_sync_method)
        Me.tp_db_paths.Controls.Add(Me.b_db_direction)
        Me.tp_db_paths.Controls.Add(Me.Label2)
        Me.tp_db_paths.Controls.Add(Me.b_db_server)
        Me.tp_db_paths.Controls.Add(Me.tb_db_server_path)
        Me.tp_db_paths.Controls.Add(Me.tb_db_client_path)
        Me.tp_db_paths.Controls.Add(Me.Label1)
        Me.tp_db_paths.Controls.Add(Me.b_db_client)
        Me.tp_db_paths.Location = New System.Drawing.Point(4, 22)
        Me.tp_db_paths.Name = "tp_db_paths"
        Me.tp_db_paths.Padding = New System.Windows.Forms.Padding(3)
        Me.tp_db_paths.Size = New System.Drawing.Size(565, 184)
        Me.tp_db_paths.TabIndex = 0
        Me.tp_db_paths.Text = "Paths"
        '
        'cb_db_sync_method
        '
        Me.cb_db_sync_method.FormattingEnabled = True
        Me.cb_db_sync_method.Location = New System.Drawing.Point(327, 91)
        Me.cb_db_sync_method.Name = "cb_db_sync_method"
        Me.cb_db_sync_method.Size = New System.Drawing.Size(215, 21)
        Me.cb_db_sync_method.TabIndex = 68
        '
        'b_db_direction
        '
        Me.b_db_direction.Image = Global.MPSync.My.Resources.Resources.sync_both
        Me.b_db_direction.Location = New System.Drawing.Point(252, 68)
        Me.b_db_direction.Name = "b_db_direction"
        Me.b_db_direction.Size = New System.Drawing.Size(64, 64)
        Me.b_db_direction.TabIndex = 67
        Me.b_db_direction.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(6, 128)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(94, 13)
        Me.Label2.TabIndex = 66
        Me.Label2.Text = "Server DB path"
        '
        'b_db_server
        '
        Me.b_db_server.Location = New System.Drawing.Point(504, 141)
        Me.b_db_server.Name = "b_db_server"
        Me.b_db_server.Size = New System.Drawing.Size(57, 23)
        Me.b_db_server.TabIndex = 65
        Me.b_db_server.Text = "Browse"
        Me.b_db_server.UseVisualStyleBackColor = True
        '
        'tb_db_server_path
        '
        Me.tb_db_server_path.Location = New System.Drawing.Point(6, 144)
        Me.tb_db_server_path.Name = "tb_db_server_path"
        Me.tb_db_server_path.Size = New System.Drawing.Size(492, 20)
        Me.tb_db_server_path.TabIndex = 64
        '
        'tb_db_client_path
        '
        Me.tb_db_client_path.Location = New System.Drawing.Point(3, 36)
        Me.tb_db_client_path.Name = "tb_db_client_path"
        Me.tb_db_client_path.Size = New System.Drawing.Size(492, 20)
        Me.tb_db_client_path.TabIndex = 61
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(3, 20)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(89, 13)
        Me.Label1.TabIndex = 63
        Me.Label1.Text = "Client DB path"
        '
        'b_db_client
        '
        Me.b_db_client.Location = New System.Drawing.Point(501, 33)
        Me.b_db_client.Name = "b_db_client"
        Me.b_db_client.Size = New System.Drawing.Size(57, 23)
        Me.b_db_client.TabIndex = 62
        Me.b_db_client.Text = "Browse"
        Me.b_db_client.UseVisualStyleBackColor = True
        '
        'tp_db_advancedsettings
        '
        Me.tp_db_advancedsettings.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.tp_db_advancedsettings.Controls.Add(Me.cb_vacuum)
        Me.tp_db_advancedsettings.Controls.Add(Me.cb_watched)
        Me.tp_db_advancedsettings.Controls.Add(Me.cb_db_pause)
        Me.tp_db_advancedsettings.Controls.Add(Me.GroupBox1)
        Me.tp_db_advancedsettings.Controls.Add(Me.cb_db_sync)
        Me.tp_db_advancedsettings.Controls.Add(Me.nud_db_sync)
        Me.tp_db_advancedsettings.Controls.Add(Me.Label4)
        Me.tp_db_advancedsettings.Controls.Add(Me.Label3)
        Me.tp_db_advancedsettings.Controls.Add(Me.clb_databases)
        Me.tp_db_advancedsettings.Location = New System.Drawing.Point(4, 22)
        Me.tp_db_advancedsettings.Name = "tp_db_advancedsettings"
        Me.tp_db_advancedsettings.Padding = New System.Windows.Forms.Padding(3)
        Me.tp_db_advancedsettings.Size = New System.Drawing.Size(565, 184)
        Me.tp_db_advancedsettings.TabIndex = 1
        Me.tp_db_advancedsettings.Text = "Advanced Settings"
        '
        'cb_vacuum
        '
        Me.cb_vacuum.AutoSize = True
        Me.cb_vacuum.CheckAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cb_vacuum.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cb_vacuum.Location = New System.Drawing.Point(6, 83)
        Me.cb_vacuum.Name = "cb_vacuum"
        Me.cb_vacuum.Size = New System.Drawing.Size(135, 17)
        Me.cb_vacuum.TabIndex = 17
        Me.cb_vacuum.Text = "Vacuum Databases"
        Me.cb_vacuum.UseVisualStyleBackColor = True
        '
        'cb_watched
        '
        Me.cb_watched.AutoSize = True
        Me.cb_watched.CheckAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cb_watched.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cb_watched.Location = New System.Drawing.Point(6, 61)
        Me.cb_watched.Name = "cb_watched"
        Me.cb_watched.Size = New System.Drawing.Size(278, 17)
        Me.cb_watched.TabIndex = 16
        Me.cb_watched.Text = "Synchronize watched status between clients"
        Me.cb_watched.UseVisualStyleBackColor = True
        '
        'cb_db_pause
        '
        Me.cb_db_pause.AutoSize = True
        Me.cb_db_pause.CheckAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cb_db_pause.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cb_db_pause.Location = New System.Drawing.Point(6, 39)
        Me.cb_db_pause.Name = "cb_db_pause"
        Me.cb_db_pause.Size = New System.Drawing.Size(172, 17)
        Me.cb_db_pause.TabIndex = 15
        Me.cb_db_pause.Text = "Pause when player active"
        Me.cb_db_pause.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.rb_specific_db)
        Me.GroupBox1.Controls.Add(Me.rb_all_db)
        Me.GroupBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.Location = New System.Drawing.Point(10, 110)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(273, 70)
        Me.GroupBox1.TabIndex = 14
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = " Synchronize "
        '
        'rb_specific_db
        '
        Me.rb_specific_db.AutoSize = True
        Me.rb_specific_db.Location = New System.Drawing.Point(11, 45)
        Me.rb_specific_db.Name = "rb_specific_db"
        Me.rb_specific_db.Size = New System.Drawing.Size(117, 17)
        Me.rb_specific_db.TabIndex = 1
        Me.rb_specific_db.TabStop = True
        Me.rb_specific_db.Text = "Specific Databases"
        Me.rb_specific_db.UseVisualStyleBackColor = True
        '
        'rb_all_db
        '
        Me.rb_all_db.AutoSize = True
        Me.rb_all_db.Location = New System.Drawing.Point(11, 21)
        Me.rb_all_db.Name = "rb_all_db"
        Me.rb_all_db.Size = New System.Drawing.Size(90, 17)
        Me.rb_all_db.TabIndex = 0
        Me.rb_all_db.TabStop = True
        Me.rb_all_db.Text = "All Databases"
        Me.rb_all_db.UseVisualStyleBackColor = True
        '
        'cb_db_sync
        '
        Me.cb_db_sync.FormattingEnabled = True
        Me.cb_db_sync.Items.AddRange(New Object() {"seconds", "minutes", "hours"})
        Me.cb_db_sync.Location = New System.Drawing.Point(160, 10)
        Me.cb_db_sync.Name = "cb_db_sync"
        Me.cb_db_sync.Size = New System.Drawing.Size(80, 21)
        Me.cb_db_sync.TabIndex = 13
        Me.cb_db_sync.Text = "seconds"
        '
        'nud_db_sync
        '
        Me.nud_db_sync.Location = New System.Drawing.Point(120, 11)
        Me.nud_db_sync.Maximum = New Decimal(New Integer() {99, 0, 0, 0})
        Me.nud_db_sync.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nud_db_sync.Name = "nud_db_sync"
        Me.nud_db_sync.Size = New System.Drawing.Size(36, 20)
        Me.nud_db_sync.TabIndex = 12
        Me.nud_db_sync.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.nud_db_sync.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(7, 13)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(115, 13)
        Me.Label4.TabIndex = 11
        Me.Label4.Text = "Synchronize every "
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(286, 9)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(195, 13)
        Me.Label3.TabIndex = 1
        Me.Label3.Text = "Select Databases to Synchronize"
        '
        'clb_databases
        '
        Me.clb_databases.Enabled = False
        Me.clb_databases.FormattingEnabled = True
        Me.clb_databases.Location = New System.Drawing.Point(289, 26)
        Me.clb_databases.Name = "clb_databases"
        Me.clb_databases.Size = New System.Drawing.Size(265, 154)
        Me.clb_databases.Sorted = True
        Me.clb_databases.TabIndex = 0
        '
        'tp_other
        '
        Me.tp_other.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.tp_other.Controls.Add(Me.Panel4)
        Me.tp_other.Controls.Add(Me.GroupBox4)
        Me.tp_other.Controls.Add(Me.Label12)
        Me.tp_other.Controls.Add(Me.clb_db_objects)
        Me.tp_other.Location = New System.Drawing.Point(4, 22)
        Me.tp_other.Name = "tp_other"
        Me.tp_other.Padding = New System.Windows.Forms.Padding(3)
        Me.tp_other.Size = New System.Drawing.Size(565, 184)
        Me.tp_other.TabIndex = 3
        Me.tp_other.Text = "Other"
        '
        'Panel4
        '
        Me.Panel4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Panel4.Controls.Add(Me.Label10)
        Me.Panel4.Location = New System.Drawing.Point(10, 139)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(273, 41)
        Me.Panel4.TabIndex = 22
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.Location = New System.Drawing.Point(16, 6)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(234, 26)
        Me.Label10.TabIndex = 24
        Me.Label10.Text = "Selected objects will just be copied over if target" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "is older."
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.rb_o_nothing)
        Me.GroupBox4.Controls.Add(Me.rb_o_specific)
        Me.GroupBox4.Controls.Add(Me.rb_o_all)
        Me.GroupBox4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox4.Location = New System.Drawing.Point(10, 10)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(273, 123)
        Me.GroupBox4.TabIndex = 21
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = " Copy "
        '
        'rb_o_nothing
        '
        Me.rb_o_nothing.AutoSize = True
        Me.rb_o_nothing.Location = New System.Drawing.Point(11, 79)
        Me.rb_o_nothing.Name = "rb_o_nothing"
        Me.rb_o_nothing.Size = New System.Drawing.Size(62, 17)
        Me.rb_o_nothing.TabIndex = 2
        Me.rb_o_nothing.TabStop = True
        Me.rb_o_nothing.Text = "Nothing"
        Me.rb_o_nothing.UseVisualStyleBackColor = True
        '
        'rb_o_specific
        '
        Me.rb_o_specific.AutoSize = True
        Me.rb_o_specific.Location = New System.Drawing.Point(11, 53)
        Me.rb_o_specific.Name = "rb_o_specific"
        Me.rb_o_specific.Size = New System.Drawing.Size(102, 17)
        Me.rb_o_specific.TabIndex = 1
        Me.rb_o_specific.TabStop = True
        Me.rb_o_specific.Text = "Specific Objects"
        Me.rb_o_specific.UseVisualStyleBackColor = True
        '
        'rb_o_all
        '
        Me.rb_o_all.AutoSize = True
        Me.rb_o_all.Location = New System.Drawing.Point(11, 27)
        Me.rb_o_all.Name = "rb_o_all"
        Me.rb_o_all.Size = New System.Drawing.Size(75, 17)
        Me.rb_o_all.TabIndex = 0
        Me.rb_o_all.TabStop = True
        Me.rb_o_all.Text = "All Objects"
        Me.rb_o_all.UseVisualStyleBackColor = True
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label12.Location = New System.Drawing.Point(286, 9)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(137, 13)
        Me.Label12.TabIndex = 20
        Me.Label12.Text = "Select Objects to Copy"
        '
        'clb_db_objects
        '
        Me.clb_db_objects.Enabled = False
        Me.clb_db_objects.FormattingEnabled = True
        Me.clb_db_objects.Location = New System.Drawing.Point(289, 26)
        Me.clb_db_objects.Name = "clb_db_objects"
        Me.clb_db_objects.Size = New System.Drawing.Size(265, 154)
        Me.clb_db_objects.Sorted = True
        Me.clb_db_objects.TabIndex = 19
        '
        'tp_watched
        '
        Me.tp_watched.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.tp_watched.Controls.Add(Me.Panel3)
        Me.tp_watched.Controls.Add(Me.GroupBox3)
        Me.tp_watched.Controls.Add(Me.Label9)
        Me.tp_watched.Controls.Add(Me.clb_watched)
        Me.tp_watched.Location = New System.Drawing.Point(4, 22)
        Me.tp_watched.Name = "tp_watched"
        Me.tp_watched.Padding = New System.Windows.Forms.Padding(3)
        Me.tp_watched.Size = New System.Drawing.Size(565, 184)
        Me.tp_watched.TabIndex = 2
        Me.tp_watched.Text = "Watched status"
        '
        'Panel3
        '
        Me.Panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Panel3.Controls.Add(Me.Label11)
        Me.Panel3.Location = New System.Drawing.Point(10, 111)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(273, 69)
        Me.Panel3.TabIndex = 18
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label11.Location = New System.Drawing.Point(16, 6)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(237, 52)
        Me.Label11.TabIndex = 24
        Me.Label11.Text = "This feature will synchronize among clients the" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "status of any music that was pla" &
    "yed, movies and " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "series that were watched together with their " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "respective resu" &
    "me position."
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.rb_w_specific)
        Me.GroupBox3.Controls.Add(Me.rb_w_all)
        Me.GroupBox3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox3.Location = New System.Drawing.Point(10, 10)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(273, 95)
        Me.GroupBox3.TabIndex = 17
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = " Synchronize status for "
        '
        'rb_w_specific
        '
        Me.rb_w_specific.AutoSize = True
        Me.rb_w_specific.Location = New System.Drawing.Point(11, 52)
        Me.rb_w_specific.Name = "rb_w_specific"
        Me.rb_w_specific.Size = New System.Drawing.Size(117, 17)
        Me.rb_w_specific.TabIndex = 1
        Me.rb_w_specific.TabStop = True
        Me.rb_w_specific.Text = "Specific Databases"
        Me.rb_w_specific.UseVisualStyleBackColor = True
        '
        'rb_w_all
        '
        Me.rb_w_all.AutoSize = True
        Me.rb_w_all.Location = New System.Drawing.Point(11, 25)
        Me.rb_w_all.Name = "rb_w_all"
        Me.rb_w_all.Size = New System.Drawing.Size(90, 17)
        Me.rb_w_all.TabIndex = 0
        Me.rb_w_all.TabStop = True
        Me.rb_w_all.Text = "All Databases"
        Me.rb_w_all.UseVisualStyleBackColor = True
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.Location = New System.Drawing.Point(286, 9)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(195, 13)
        Me.Label9.TabIndex = 16
        Me.Label9.Text = "Select Databases to Synchronize"
        '
        'clb_watched
        '
        Me.clb_watched.Enabled = False
        Me.clb_watched.FormattingEnabled = True
        Me.clb_watched.Location = New System.Drawing.Point(289, 26)
        Me.clb_watched.Name = "clb_watched"
        Me.clb_watched.Size = New System.Drawing.Size(265, 154)
        Me.clb_watched.Sorted = True
        Me.clb_watched.TabIndex = 15
        '
        'tp_folders
        '
        Me.tp_folders.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.tp_folders.Controls.Add(Me.tc_objects)
        Me.tp_folders.Location = New System.Drawing.Point(4, 22)
        Me.tp_folders.Name = "tp_folders"
        Me.tp_folders.Size = New System.Drawing.Size(582, 224)
        Me.tp_folders.TabIndex = 2
        Me.tp_folders.Text = "Folders"
        '
        'tc_objects
        '
        Me.tc_objects.Controls.Add(Me.tp_list)
        Me.tc_objects.Controls.Add(Me.tp_paths)
        Me.tc_objects.Controls.Add(Me.tp_advancedsettings)
        Me.tc_objects.Location = New System.Drawing.Point(5, 8)
        Me.tc_objects.Name = "tc_objects"
        Me.tc_objects.SelectedIndex = 0
        Me.tc_objects.Size = New System.Drawing.Size(573, 210)
        Me.tc_objects.TabIndex = 65
        '
        'tp_list
        '
        Me.tp_list.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.tp_list.Controls.Add(Me.b_delete)
        Me.tp_list.Controls.Add(Me.b_edit)
        Me.tp_list.Controls.Add(Me.b_add)
        Me.tp_list.Controls.Add(Me.clb_object_list)
        Me.tp_list.Location = New System.Drawing.Point(4, 22)
        Me.tp_list.Name = "tp_list"
        Me.tp_list.Padding = New System.Windows.Forms.Padding(3)
        Me.tp_list.Size = New System.Drawing.Size(565, 184)
        Me.tp_list.TabIndex = 2
        Me.tp_list.Text = "Folders List"
        '
        'b_delete
        '
        Me.b_delete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.b_delete.Enabled = False
        Me.b_delete.Image = Global.MPSync.My.Resources.Resources.delete
        Me.b_delete.Location = New System.Drawing.Point(506, 72)
        Me.b_delete.Name = "b_delete"
        Me.b_delete.Size = New System.Drawing.Size(40, 40)
        Me.b_delete.TabIndex = 3
        Me.b_delete.UseVisualStyleBackColor = True
        '
        'b_edit
        '
        Me.b_edit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.b_edit.Enabled = False
        Me.b_edit.Image = Global.MPSync.My.Resources.Resources.edit
        Me.b_edit.Location = New System.Drawing.Point(506, 118)
        Me.b_edit.Name = "b_edit"
        Me.b_edit.Size = New System.Drawing.Size(40, 40)
        Me.b_edit.TabIndex = 2
        Me.b_edit.UseVisualStyleBackColor = True
        '
        'b_add
        '
        Me.b_add.BackgroundImage = Global.MPSync.My.Resources.Resources.add
        Me.b_add.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.b_add.Location = New System.Drawing.Point(506, 26)
        Me.b_add.Name = "b_add"
        Me.b_add.Size = New System.Drawing.Size(40, 40)
        Me.b_add.TabIndex = 1
        Me.b_add.UseVisualStyleBackColor = True
        '
        'clb_object_list
        '
        Me.clb_object_list.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.clb_object_list.FormattingEnabled = True
        Me.clb_object_list.Location = New System.Drawing.Point(7, 8)
        Me.clb_object_list.Name = "clb_object_list"
        Me.clb_object_list.Size = New System.Drawing.Size(485, 164)
        Me.clb_object_list.Sorted = True
        Me.clb_object_list.TabIndex = 0
        '
        'tp_paths
        '
        Me.tp_paths.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.tp_paths.Controls.Add(Me.cb_folders_sync_method)
        Me.tp_paths.Controls.Add(Me.Label5)
        Me.tp_paths.Controls.Add(Me.b_folders_server)
        Me.tp_paths.Controls.Add(Me.tb_folders_server_path)
        Me.tp_paths.Controls.Add(Me.tb_folders_client_path)
        Me.tp_paths.Controls.Add(Me.Label6)
        Me.tp_paths.Controls.Add(Me.b_folders_client)
        Me.tp_paths.Controls.Add(Me.b_folders_direction)
        Me.tp_paths.Location = New System.Drawing.Point(4, 22)
        Me.tp_paths.Name = "tp_paths"
        Me.tp_paths.Padding = New System.Windows.Forms.Padding(3)
        Me.tp_paths.Size = New System.Drawing.Size(565, 184)
        Me.tp_paths.TabIndex = 0
        Me.tp_paths.Text = "Paths"
        '
        'cb_folders_sync_method
        '
        Me.cb_folders_sync_method.FormattingEnabled = True
        Me.cb_folders_sync_method.Location = New System.Drawing.Point(327, 91)
        Me.cb_folders_sync_method.Name = "cb_folders_sync_method"
        Me.cb_folders_sync_method.Size = New System.Drawing.Size(215, 21)
        Me.cb_folders_sync_method.TabIndex = 68
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(6, 128)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(73, 13)
        Me.Label5.TabIndex = 66
        Me.Label5.Text = "Server path"
        '
        'b_folders_server
        '
        Me.b_folders_server.Location = New System.Drawing.Point(504, 141)
        Me.b_folders_server.Name = "b_folders_server"
        Me.b_folders_server.Size = New System.Drawing.Size(57, 23)
        Me.b_folders_server.TabIndex = 64
        Me.b_folders_server.Text = "Browse"
        Me.b_folders_server.UseVisualStyleBackColor = True
        '
        'tb_folders_server_path
        '
        Me.tb_folders_server_path.Location = New System.Drawing.Point(6, 144)
        Me.tb_folders_server_path.Name = "tb_folders_server_path"
        Me.tb_folders_server_path.Size = New System.Drawing.Size(492, 20)
        Me.tb_folders_server_path.TabIndex = 65
        '
        'tb_folders_client_path
        '
        Me.tb_folders_client_path.Location = New System.Drawing.Point(3, 36)
        Me.tb_folders_client_path.Name = "tb_folders_client_path"
        Me.tb_folders_client_path.Size = New System.Drawing.Size(492, 20)
        Me.tb_folders_client_path.TabIndex = 62
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(3, 20)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(68, 13)
        Me.Label6.TabIndex = 63
        Me.Label6.Text = "Client path"
        '
        'b_folders_client
        '
        Me.b_folders_client.Location = New System.Drawing.Point(501, 33)
        Me.b_folders_client.Name = "b_folders_client"
        Me.b_folders_client.Size = New System.Drawing.Size(57, 23)
        Me.b_folders_client.TabIndex = 61
        Me.b_folders_client.Text = "Browse"
        Me.b_folders_client.UseVisualStyleBackColor = True
        '
        'b_folders_direction
        '
        Me.b_folders_direction.Image = Global.MPSync.My.Resources.Resources.sync_both
        Me.b_folders_direction.Location = New System.Drawing.Point(252, 68)
        Me.b_folders_direction.Name = "b_folders_direction"
        Me.b_folders_direction.Size = New System.Drawing.Size(64, 64)
        Me.b_folders_direction.TabIndex = 67
        Me.b_folders_direction.UseVisualStyleBackColor = True
        '
        'tp_advancedsettings
        '
        Me.tp_advancedsettings.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.tp_advancedsettings.Controls.Add(Me.cb_folders_crc32)
        Me.tp_advancedsettings.Controls.Add(Me.cb_folders_md5)
        Me.tp_advancedsettings.Controls.Add(Me.b_apply)
        Me.tp_advancedsettings.Controls.Add(Me.cb_folders_pause)
        Me.tp_advancedsettings.Controls.Add(Me.GroupBox2)
        Me.tp_advancedsettings.Controls.Add(Me.Label8)
        Me.tp_advancedsettings.Controls.Add(Me.clb_objects)
        Me.tp_advancedsettings.Location = New System.Drawing.Point(4, 22)
        Me.tp_advancedsettings.Name = "tp_advancedsettings"
        Me.tp_advancedsettings.Padding = New System.Windows.Forms.Padding(3)
        Me.tp_advancedsettings.Size = New System.Drawing.Size(565, 184)
        Me.tp_advancedsettings.TabIndex = 1
        Me.tp_advancedsettings.Text = "Advanced Settings"
        '
        'cb_folders_crc32
        '
        Me.cb_folders_crc32.AutoSize = True
        Me.cb_folders_crc32.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cb_folders_crc32.Location = New System.Drawing.Point(116, 38)
        Me.cb_folders_crc32.Name = "cb_folders_crc32"
        Me.cb_folders_crc32.Size = New System.Drawing.Size(168, 17)
        Me.cb_folders_crc32.TabIndex = 71
        Me.cb_folders_crc32.Text = "Use CRC32 to verify files"
        Me.tt_folders_md5.SetToolTip(Me.cb_folders_crc32, "Choose this for a more accurate synchronisation of the objects." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Be aware that th" &
        "is will increase the synchronization time.")
        Me.cb_folders_crc32.UseVisualStyleBackColor = True
        '
        'cb_folders_md5
        '
        Me.cb_folders_md5.AutoSize = True
        Me.cb_folders_md5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cb_folders_md5.Location = New System.Drawing.Point(21, 38)
        Me.cb_folders_md5.Name = "cb_folders_md5"
        Me.cb_folders_md5.Size = New System.Drawing.Size(78, 17)
        Me.cb_folders_md5.TabIndex = 70
        Me.cb_folders_md5.Text = "Use MD5"
        Me.tt_folders_md5.SetToolTip(Me.cb_folders_md5, "Choose this for a more accurate synchronisation of the objects." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Be aware that th" &
        "is will increase the synchronization time.")
        Me.cb_folders_md5.UseVisualStyleBackColor = True
        '
        'b_apply
        '
        Me.b_apply.Location = New System.Drawing.Point(10, 155)
        Me.b_apply.Name = "b_apply"
        Me.b_apply.Size = New System.Drawing.Size(100, 23)
        Me.b_apply.TabIndex = 66
        Me.b_apply.Text = "Apply Changes"
        Me.b_apply.UseVisualStyleBackColor = True
        '
        'cb_folders_pause
        '
        Me.cb_folders_pause.AutoSize = True
        Me.cb_folders_pause.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cb_folders_pause.Location = New System.Drawing.Point(21, 17)
        Me.cb_folders_pause.Name = "cb_folders_pause"
        Me.cb_folders_pause.Size = New System.Drawing.Size(172, 17)
        Me.cb_folders_pause.TabIndex = 18
        Me.cb_folders_pause.Text = "Pause when player active"
        Me.cb_folders_pause.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.rb_specific_folders)
        Me.GroupBox2.Controls.Add(Me.rb_all_folders)
        Me.GroupBox2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox2.Location = New System.Drawing.Point(10, 61)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(273, 89)
        Me.GroupBox2.TabIndex = 17
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = " Synchronize "
        '
        'rb_specific_folders
        '
        Me.rb_specific_folders.AutoSize = True
        Me.rb_specific_folders.Location = New System.Drawing.Point(11, 56)
        Me.rb_specific_folders.Name = "rb_specific_folders"
        Me.rb_specific_folders.Size = New System.Drawing.Size(97, 17)
        Me.rb_specific_folders.TabIndex = 1
        Me.rb_specific_folders.TabStop = True
        Me.rb_specific_folders.Text = "Specific folders"
        Me.rb_specific_folders.UseVisualStyleBackColor = True
        '
        'rb_all_folders
        '
        Me.rb_all_folders.AutoSize = True
        Me.rb_all_folders.Location = New System.Drawing.Point(11, 24)
        Me.rb_all_folders.Name = "rb_all_folders"
        Me.rb_all_folders.Size = New System.Drawing.Size(70, 17)
        Me.rb_all_folders.TabIndex = 0
        Me.rb_all_folders.TabStop = True
        Me.rb_all_folders.Text = "All folders"
        Me.rb_all_folders.UseVisualStyleBackColor = True
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(286, 9)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(173, 13)
        Me.Label8.TabIndex = 16
        Me.Label8.Text = "Select folders to Synchronize"
        '
        'clb_objects
        '
        Me.clb_objects.Enabled = False
        Me.clb_objects.FormattingEnabled = True
        Me.clb_objects.Location = New System.Drawing.Point(289, 26)
        Me.clb_objects.Name = "clb_objects"
        Me.clb_objects.Size = New System.Drawing.Size(265, 124)
        Me.clb_objects.Sorted = True
        Me.clb_objects.TabIndex = 15
        '
        'tp_syncnow
        '
        Me.tp_syncnow.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.tp_syncnow.Controls.Add(Me.lb_status)
        Me.tp_syncnow.Location = New System.Drawing.Point(4, 22)
        Me.tp_syncnow.Name = "tp_syncnow"
        Me.tp_syncnow.Padding = New System.Windows.Forms.Padding(3)
        Me.tp_syncnow.Size = New System.Drawing.Size(582, 224)
        Me.tp_syncnow.TabIndex = 4
        Me.tp_syncnow.Text = "Status"
        '
        'lb_status
        '
        Me.lb_status.CausesValidation = False
        Me.lb_status.Font = New System.Drawing.Font("Consolas", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lb_status.ForeColor = System.Drawing.SystemColors.InfoText
        Me.lb_status.FormattingEnabled = True
        Me.lb_status.HorizontalScrollbar = True
        Me.lb_status.ItemHeight = 10
        Me.lb_status.Location = New System.Drawing.Point(7, 7)
        Me.lb_status.Name = "lb_status"
        Me.lb_status.SelectionMode = System.Windows.Forms.SelectionMode.None
        Me.lb_status.Size = New System.Drawing.Size(569, 204)
        Me.lb_status.TabIndex = 0
        '
        'MPSync_settings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(599, 271)
        Me.Controls.Add(Me.tc_main)
        Me.Controls.Add(Me.l_copyright)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "MPSync_settings"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "MediaPortal Sync settings v"
        Me.tc_main.ResumeLayout(False)
        Me.tp_settings.ResumeLayout(False)
        Me.tc_settings.ResumeLayout(False)
        Me.tp_selection.ResumeLayout(False)
        Me.Panel5.ResumeLayout(False)
        Me.Panel5.PerformLayout()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tp_configuration.ResumeLayout(False)
        Me.Panel7.ResumeLayout(False)
        Me.Panel7.PerformLayout()
        CType(Me.nud_max_folder_streams, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nud_max_DB_streams, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.tp_database.ResumeLayout(False)
        Me.tc_database.ResumeLayout(False)
        Me.tp_db_paths.ResumeLayout(False)
        Me.tp_db_paths.PerformLayout()
        Me.tp_db_advancedsettings.ResumeLayout(False)
        Me.tp_db_advancedsettings.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.nud_db_sync, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tp_other.ResumeLayout(False)
        Me.tp_other.PerformLayout()
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.tp_watched.ResumeLayout(False)
        Me.tp_watched.PerformLayout()
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.tp_folders.ResumeLayout(False)
        Me.tc_objects.ResumeLayout(False)
        Me.tp_list.ResumeLayout(False)
        Me.tp_paths.ResumeLayout(False)
        Me.tp_paths.PerformLayout()
        Me.tp_advancedsettings.ResumeLayout(False)
        Me.tp_advancedsettings.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.tp_syncnow.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents b_save As System.Windows.Forms.Button
    Friend WithEvents tb_master_path As System.Windows.Forms.TextBox
    Friend WithEvents tb_slave_path As System.Windows.Forms.TextBox
    Friend WithEvents l_copyright As System.Windows.Forms.Label
    Friend WithEvents b_sync_now As System.Windows.Forms.Button
    Friend WithEvents tc_main As System.Windows.Forms.TabControl
    Friend WithEvents tp_database As System.Windows.Forms.TabPage
    Friend WithEvents tc_database As System.Windows.Forms.TabControl
    Friend WithEvents tp_db_paths As System.Windows.Forms.TabPage
    Friend WithEvents cb_db_sync_method As System.Windows.Forms.ComboBox
    Friend WithEvents b_db_direction As System.Windows.Forms.Button
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents b_db_server As System.Windows.Forms.Button
    Friend WithEvents tb_db_server_path As System.Windows.Forms.TextBox
    Friend WithEvents tb_db_client_path As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents b_db_client As System.Windows.Forms.Button
    Friend WithEvents tp_db_advancedsettings As System.Windows.Forms.TabPage
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents rb_specific_db As System.Windows.Forms.RadioButton
    Friend WithEvents rb_all_db As System.Windows.Forms.RadioButton
    Friend WithEvents cb_db_sync As System.Windows.Forms.ComboBox
    Friend WithEvents nud_db_sync As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents clb_databases As System.Windows.Forms.CheckedListBox
    Friend WithEvents tp_folders As System.Windows.Forms.TabPage
    Friend WithEvents tc_objects As System.Windows.Forms.TabControl
    Friend WithEvents tp_paths As System.Windows.Forms.TabPage
    Friend WithEvents cb_folders_sync_method As System.Windows.Forms.ComboBox
    Friend WithEvents b_folders_direction As System.Windows.Forms.Button
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents b_folders_server As System.Windows.Forms.Button
    Friend WithEvents tb_folders_server_path As System.Windows.Forms.TextBox
    Friend WithEvents tb_folders_client_path As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents b_folders_client As System.Windows.Forms.Button
    Friend WithEvents tp_advancedsettings As System.Windows.Forms.TabPage
    Friend WithEvents tp_settings As System.Windows.Forms.TabPage
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents rb_specific_folders As System.Windows.Forms.RadioButton
    Friend WithEvents rb_all_folders As System.Windows.Forms.RadioButton
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents clb_objects As System.Windows.Forms.CheckedListBox
    Friend WithEvents cb_db_pause As System.Windows.Forms.CheckBox
    Friend WithEvents cb_folders_pause As System.Windows.Forms.CheckBox
    Friend WithEvents cb_watched As System.Windows.Forms.CheckBox
    Friend WithEvents tp_watched As System.Windows.Forms.TabPage
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents rb_w_specific As System.Windows.Forms.RadioButton
    Friend WithEvents rb_w_all As System.Windows.Forms.RadioButton
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents clb_watched As System.Windows.Forms.CheckedListBox
    Friend WithEvents tp_syncnow As System.Windows.Forms.TabPage
    Friend WithEvents lb_status As System.Windows.Forms.ListBox
    Friend WithEvents tp_other As System.Windows.Forms.TabPage
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
    Friend WithEvents rb_o_nothing As System.Windows.Forms.RadioButton
    Friend WithEvents rb_o_specific As System.Windows.Forms.RadioButton
    Friend WithEvents rb_o_all As System.Windows.Forms.RadioButton
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents clb_db_objects As System.Windows.Forms.CheckedListBox
    Friend WithEvents tp_list As System.Windows.Forms.TabPage
    Friend WithEvents clb_object_list As System.Windows.Forms.CheckedListBox
    Friend WithEvents b_delete As System.Windows.Forms.Button
    Friend WithEvents b_edit As System.Windows.Forms.Button
    Friend WithEvents b_add As System.Windows.Forms.Button
    Friend WithEvents b_apply As System.Windows.Forms.Button
    Friend WithEvents tc_settings As System.Windows.Forms.TabControl
    Friend WithEvents tp_selection As System.Windows.Forms.TabPage
    Friend WithEvents Panel5 As System.Windows.Forms.Panel
    Friend WithEvents PictureBox2 As System.Windows.Forms.PictureBox
    Friend WithEvents cb_folders As System.Windows.Forms.CheckBox
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents cb_databases As System.Windows.Forms.CheckBox
    Friend WithEvents rb_timestamp As System.Windows.Forms.RadioButton
    Friend WithEvents rb_triggers As System.Windows.Forms.RadioButton
    Friend WithEvents tp_configuration As System.Windows.Forms.TabPage
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents cb_debug As System.Windows.Forms.CheckBox
    Friend WithEvents cb_vacuum As System.Windows.Forms.CheckBox
    Friend WithEvents tt_folders_md5 As System.Windows.Forms.ToolTip
    Friend WithEvents cb_folders_crc32 As System.Windows.Forms.CheckBox
    Friend WithEvents cb_folders_md5 As System.Windows.Forms.CheckBox
    Friend WithEvents Panel7 As Panel
    Friend WithEvents Label14 As Label
    Friend WithEvents Label13 As Label
    Friend WithEvents nud_max_DB_streams As NumericUpDown
    Friend WithEvents Label15 As Label
    Friend WithEvents nud_max_folder_streams As NumericUpDown
End Class
