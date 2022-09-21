#region Lord Virus's PS3Lib Implementation.
//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing.Text;
//using System.Globalization;
//using System.Linq;
//using System.Media;
//using System.Reflection;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using PS3ManagerAPI;

//namespace PS3Lib
//{
//    public enum Lang
//    {
//        Null,
//        French,
//        English,
//        German
//    }

//    public enum SelectAPI
//    {
//        ControlConsole,
//        TargetManager,
//        PS3Manager
//    }

//    public class PS3API
//    {
//        private static string targetName = String.Empty;
//        private static string targetIp = String.Empty;
//        public PS3API(SelectAPI API = SelectAPI.TargetManager)
//        {
//            SetAPI.API = API;
//            MakeInstanceAPI(API);
//        }

//        public void setTargetName(string value)
//        {
//            targetName = value;
//        }

//        private void MakeInstanceAPI(SelectAPI API)
//        {
//            if (API == SelectAPI.TargetManager)
//                if (Common.TmApi == null)
//                    Common.TmApi = new TMAPI();
//            if (API == SelectAPI.ControlConsole)
//                if (Common.CcApi == null)
//                    Common.CcApi = new CCAPI();
//            if (API == SelectAPI.PS3Manager)
//                if (Common.PS3MAPI == null)
//                    Common.PS3MAPI = new PS3MAPI();
//        }

//        private class SetLang
//        {
//            public static Lang defaultLang = Lang.Null;
//        }

//        private class SetAPI
//        {
//            public static SelectAPI API;
//        }

//        private class Common
//        {
//            public static CCAPI CcApi;
//            public static TMAPI TmApi;
//            public static PS3MAPI PS3MAPI;
//        }

//        /// <summary>Force a language for the console list popup.</summary>
//        public void SetFormLang(Lang Language)
//        {
//            SetLang.defaultLang = Language;
//        }

//       /// <summary>init again the connection if you use a Thread or a Timer.</summary>
//        public void InitTarget()
//        {
//            if (SetAPI.API == SelectAPI.TargetManager)
//                Common.TmApi.InitComms();
//        }

//        /// <summary>Connect your console with selected API.</summary>
//        public bool ConnectTarget(int target = 0)
//        {
//            // We'll check again if the instance has been done, else you'll got an exception error.
//            MakeInstanceAPI(GetCurrentAPI());

//            bool result = false;
//            if (SetAPI.API == SelectAPI.TargetManager)
//                result = Common.TmApi.ConnectTarget(target);
//            else if (SetAPI.API == SelectAPI.ControlConsole)
//                result = new ConsoleList(this).Show();
//            else
//                result = Common.PS3MAPI.ConnectTarget();
//            return result;
//        }

//        /// <summary>Connect your console with CCAPI.</summary>
//        public bool ConnectTarget(string ip)
//        {
//            // We'll check again if the instance has been done.
//            MakeInstanceAPI(GetCurrentAPI());
//            if (Common.CcApi.SUCCESS(Common.CcApi.ConnectTarget(ip)))
//            {
//                targetIp = ip;
//                return true;
//            }
//            else return false;
//        }

//        /// <summary>Disconnect Target with selected API.</summary>
//        public void DisconnectTarget()
//        {
//            if (SetAPI.API == SelectAPI.TargetManager)
//                Common.TmApi.DisconnectTarget();
//            else Common.CcApi.DisconnectTarget();
//        }

//        /// <summary>Attach the current process (current Game) with selected API.</summary>
//        public bool AttachProcess()
//        {
//            // We'll check again if the instance has been done.
//            MakeInstanceAPI(GetCurrentAPI());

//            bool AttachResult = false;
//            if (SetAPI.API == SelectAPI.TargetManager)
//                AttachResult = Common.TmApi.AttachProcess();
//            else if (SetAPI.API == SelectAPI.ControlConsole)
//                AttachResult = Common.CcApi.SUCCESS(Common.CcApi.AttachProcess());
//            else if (SetAPI.API == SelectAPI.PS3Manager)
//                AttachResult = Common.PS3MAPI.AttachProcess();
//            return AttachResult;
//        }

//        public string GetConsoleName()
//        {
//            if (SetAPI.API == SelectAPI.TargetManager)
//                return Common.TmApi.SCE.GetTargetName();
//            else if(SetAPI.API == SelectAPI.ControlConsole)
//            {
//                if (targetName != String.Empty)
//                    return targetName;

//                if (targetIp != String.Empty)
//                {
//                    List<CCAPI.ConsoleInfo> Data = new List<CCAPI.ConsoleInfo>();
//                    Data = Common.CcApi.GetConsoleList();
//                    if (Data.Count > 0)
//                    {
//                        for (int i = 0; i < Data.Count; i++)
//                            if (Data[i].Ip == targetIp)
//                                return Data[i].Name;
//                    }
//                }
//                return targetIp;
//            }
//            else /*if (SetAPI.API == SelectAPI.PS3ManagerAPI)*/
//            {
//                return Common.PS3MAPI.PS3.GetFirmwareVersion_Str() + Common.PS3MAPI.PS3.GetFirmwareType();
//            }
//        }

//        /// <summary>Set memory to offset with selected API.</summary>
//        public void SetMemory(uint offset, byte[] buffer)
//        {
//            if (SetAPI.API == SelectAPI.TargetManager)
//                Common.TmApi.SetMemory(offset, buffer);
//            else if (SetAPI.API == SelectAPI.ControlConsole)
//                Common.CcApi.SetMemory(offset, buffer);
//            else if (SetAPI.API == SelectAPI.PS3Manager)
//                Common.PS3MAPI.Process.Memory.Set(Common.PS3MAPI.Process.Process_Pid, offset, buffer);
//        }

//        /// <summary>Get memory from offset using the Selected API.</summary>
//        public void GetMemory(uint offset, byte[] buffer)
//        {
//            if (SetAPI.API == SelectAPI.TargetManager)
//                Common.TmApi.GetMemory(offset, buffer);
//            else if (SetAPI.API == SelectAPI.ControlConsole)
//                Common.CcApi.GetMemory(offset, buffer);
//            else if (SetAPI.API == SelectAPI.PS3Manager)
//                Common.PS3MAPI.Process.Memory.Get(Common.PS3MAPI.Process.Process_Pid, offset, buffer);
//        }

//        /// <summary>Get memory from offset with a length using the Selected API.</summary>
//        public byte[] GetBytes(uint offset, int length)
//        {
//            byte[] buffer = new byte[length];
//            if (SetAPI.API == SelectAPI.TargetManager)
//                Common.TmApi.GetMemory(offset, buffer);
//            else if (SetAPI.API == SelectAPI.ControlConsole)
//                Common.CcApi.GetMemory(offset, buffer);
//            else if (SetAPI.API == SelectAPI.PS3Manager)
//                Common.PS3MAPI.Process.Memory.Get(Common.PS3MAPI.Process.Process_Pid, offset, buffer);
//            return buffer;
//        }

//        /// <summary>Change current API.</summary>
//        public void ChangeAPI(SelectAPI API)
//        {
//            SetAPI.API = API;
//            MakeInstanceAPI(GetCurrentAPI());
//        }

//        /// <summary>Return selected API.</summary>
//        public SelectAPI GetCurrentAPI()
//        {
//            return SetAPI.API;
//        }

//        /// <summary>Return selected API into string format.</summary>
//        public string GetCurrentAPIName()
//        {
//            string output = String.Empty;
//            if (SetAPI.API == SelectAPI.TargetManager)
//                output = Enum.GetName(typeof(SelectAPI), SelectAPI.TargetManager).Replace("Manager"," Manager");
//            else if(SetAPI.API == SelectAPI.ControlConsole)
//                output = Enum.GetName(typeof(SelectAPI), SelectAPI.ControlConsole).Replace("Console", " Console");
//            else if(SetAPI.API == SelectAPI.PS3Manager)
//                output = Enum.GetName(typeof(SelectAPI), SelectAPI.PS3Manager).Replace("Manager", " Manager");
//            return output;
//        }

//        /// <summary>This will find the dll ps3tmapi_net.dll for TMAPI.</summary>
//        public Assembly PS3TMAPI_NET()
//        {
//            return Common.TmApi.PS3TMAPI_NET();
//        }

//        /// <summary>Use the extension class with your selected API.</summary>
//        public Extension Extension
//        {
//            get { return new Extension(SetAPI.API); }
//        }

//        /// <summary>Access to all TMAPI functions.</summary>
//        public TMAPI TMAPI
//        {
//            get { return new TMAPI(); }
//        }

//        /// <summary>Access to all CCAPI functions.</summary>
//        public CCAPI CCAPI
//        {
//            get { return new CCAPI(); }
//        }

//        /// <summary>Access to all PS3MAPI functions.</summary>
//        public PS3MAPI PS3MAPI
//        {
//            get { return new PS3MAPI(); }
//        }

//        public class ConsoleList
//        {
//            private PS3API Api;
//            private List<CCAPI.ConsoleInfo> data;

//            public static int y;
//            public static int x;
//            public static Point newpoint = default(Point);
//            private Panel panel1 = new Panel();
//            private Panel panel2 = new Panel();
//            private Panel panel3 = new Panel();
//            private Panel panel4 = new Panel();
//            private Panel panel5 = new Panel();
//            private Button button1 = new Button();
//            private Label label1 = new Label();
//            private Button btnConnect = new Button();
//            private Button btnRefresh = new Button();
//            private ListViewGroup listViewGroup = new ListViewGroup("Consoles", HorizontalAlignment.Center);
//            private ListView listView = new ListView();
//            private Form formList = new Form();

//            public ConsoleList(PS3API f)
//            {
//                Api = f;
//                data = Api.CCAPI.GetConsoleList();
//            }

//            /// <summary>Return the systeme language, if it's others all text will be in english.</summary>
//            private Lang getSysLanguage()
//            {
//                if (SetLang.defaultLang == Lang.Null)
//                {
//                    if (CultureInfo.CurrentCulture.ThreeLetterWindowsLanguageName.StartsWith("FRA"))
//                        return Lang.French;
//                    else if (CultureInfo.CurrentCulture.ThreeLetterWindowsLanguageName.StartsWith("GER"))
//                        return Lang.German;
//                    return Lang.English;
//                }
//                else return SetLang.defaultLang;
//            }

//            private string strTraduction(string keyword)
//            {
//                Lang lang = getSysLanguage();
//                if (lang == Lang.French)
//                {
//                    switch (keyword)
//                    {
//                        case "btnConnect": return "Connexion";
//                        case "btnRefresh": return "Rafraîchir";
//                        case "errorSelect": return "Vous devez d'abord sélectionner une console.";
//                        case "errorSelectTitle": return "Sélectionnez une console.";
//                        case "selectGrid": return "Sélectionnez une console dans la grille.";
//                        case "selectedLbl": return "Sélection :";
//                        case "formTitle": return "Choisissez une console...";
//                        case "noConsole": return "Aucune console disponible, démarrez CCAPI Manager (v2.60+) et ajoutez une nouvelle console.";
//                        case "noConsoleTitle": return "Aucune console disponible.";
//                    }
//                }
//                else if(lang == Lang.German)
//                {
//                    switch (keyword)
//                    {
//                        case "btnConnect": return "Verbinde";
//                        case "btnRefresh": return "Wiederholen";
//                        case "errorSelect": return "Du musst zuerst eine Konsole auswählen.";
//                        case "errorSelectTitle": return "Wähle eine Konsole.";
//                        case "selectGrid": return "Wähle eine Konsole innerhalb dieses Gitters.";
//                        case "selectedLbl": return "Ausgewählt :";
//                        case "formTitle": return "Wähle eine Konsole...";
//                        case "noConsole": return "Keine Konsolen verfügbar - starte CCAPI Manager (v2.60+) und füge eine neue Konsole hinzu.";
//                        case "noConsoleTitle": return "Keine Konsolen verfügbar.";
//                    }
//                }
//                else
//                {
//                    switch (keyword)
//                    {
//                        case "btnConnect": return "Connection";
//                        case "btnRefresh": return "Refresh";
//                        case "errorSelect": return "You need to select a console first.";
//                        case "errorSelectTitle": return "Select a console.";
//                        case "selectGrid": return "Select a console within this grid.";
//                        case "selectedLbl": return "Selected :";
//                        case "formTitle": return "Select a console...";
//                        case "noConsole": return "None consoles available, run CCAPI Manager (v2.60+) and add a new console.";
//                        case "noConsoleTitle": return "None console available.";
//                    }
//                }
//                return "?";
//            }

//            private void xMouseDown(object sender, MouseEventArgs e)
//            {
//                PS3API.ConsoleList.x = Control.MousePosition.X - this.formList.Location.X;
//                PS3API.ConsoleList.y = Control.MousePosition.Y - this.formList.Location.Y;
//            }

//            private void xMouseMove(object sender, MouseEventArgs e)
//            {
//                if (e.Button == MouseButtons.Left)
//                {
//                    PS3API.ConsoleList.newpoint = Control.MousePosition;
//                    PS3API.ConsoleList.newpoint.X = PS3API.ConsoleList.newpoint.X - PS3API.ConsoleList.x;
//                    PS3API.ConsoleList.newpoint.Y = PS3API.ConsoleList.newpoint.Y - PS3API.ConsoleList.y;
//                    this.formList.Location = PS3API.ConsoleList.newpoint;
//                }
//            }

//            private void button1_Click(object sender, EventArgs e)
//            {
//                this.formList.Close();
//            }

//            public bool Show()
//            {
//                /* Original style
//                bool Result = false;
//                int tNum = -1;

//                // Instance of widgets
//                Label lblInfo = new Label();
//                Button btnConnect = new Button();
//                Button btnRefresh = new Button();
//                ListViewGroup listViewGroup = new ListViewGroup("Consoles", HorizontalAlignment.Left);
//                ListView listView = new ListView();
//                Form formList = new Form();

//                // Create our button connect
//                btnConnect.Location = new Point(12, 254);
//                btnConnect.Name = "btnConnect";
//                btnConnect.Size = new Size(198, 23);
//                btnConnect.TabIndex = 1;
//                btnConnect.Text = strTraduction("btnConnect");
//                btnConnect.UseVisualStyleBackColor = true;
//                btnConnect.Enabled = false;
//                btnConnect.Click += (sender, e) =>
//                {
//                    if(tNum > -1)
//                    {
//                        if (Api.ConnectTarget(data[tNum].Ip))
//                        {
//                            Api.setTargetName(data[tNum].Name);
//                            Result = true;
//                        }
//                        else Result = false;
//                        formList.Close();
//                    }
//                    else
//                        MessageBox.Show(strTraduction("errorSelect"), strTraduction("errorSelectTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
//                };

//                // Create our button refresh
//                btnRefresh.Location = new Point(216, 254);
//                btnRefresh.Name = "btnRefresh";
//                btnRefresh.Size = new Size(86, 23);
//                btnRefresh.TabIndex = 1;
//                btnRefresh.Text = strTraduction("btnRefresh");
//                btnRefresh.UseVisualStyleBackColor = true;
//                btnRefresh.Click += (sender, e) =>
//                {
//                    tNum = -1;
//                    listView.Clear();
//                    lblInfo.Text = strTraduction("selectGrid");
//                    btnConnect.Enabled = false;
//                    data = Api.CCAPI.GetConsoleList();
//                    int sizeD = data.Count();
//                    for (int i = 0; i < sizeD; i++)
//                    {
//                        ListViewItem item = new ListViewItem(" " + data[i].Name + " - " + data[i].Ip);
//                        item.ImageIndex = 0;
//                        listView.Items.Add(item);
//                    }
//                };

//                // Create our list view
//                listView.Font = new Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//                listViewGroup.Header = "Consoles";
//                listViewGroup.Name = "consoleGroup";
//                listView.Groups.AddRange(new ListViewGroup[] {listViewGroup});
//                listView.HideSelection = false;
//                listView.Location = new Point(12, 12);
//                listView.MultiSelect = false;
//                listView.Name = "ConsoleList";
//                listView.ShowGroups = false;
//                listView.Size = new Size(290, 215);
//                listView.TabIndex = 0;
//                listView.UseCompatibleStateImageBehavior = false;
//                listView.View = View.List;
//                listView.ItemSelectionChanged += (sender, e) =>
//                {
//                    tNum = e.ItemIndex;
//                    btnConnect.Enabled = true;
//                    string Name, Ip = "?";
//                    if (data[tNum].Name.Length > 18)
//                        Name = data[tNum].Name.Substring(0, 17) + "...";
//                    else Name = data[tNum].Name;
//                    if (data[tNum].Ip.Length > 16)
//                        Ip = data[tNum].Name.Substring(0, 16) + "...";
//                    else Ip = data[tNum].Ip;
//                    lblInfo.Text = strTraduction("selectedLbl") + " " + Name + " / " + Ip;
//                };

//                // Create our label
//                lblInfo.AutoSize = true;
//                lblInfo.Location = new Point(12, 234);
//                lblInfo.Name = "lblInfo";
//                lblInfo.Size = new Size(158, 13);
//                lblInfo.TabIndex = 3;
//                lblInfo.Text = strTraduction("selectGrid");

//                // Create our form
//                formList.MinimizeBox = false;
//                formList.MaximizeBox = false;
//                formList.ClientSize = new Size(314, 285);
//                formList.AutoScaleDimensions = new SizeF(6F, 13F);
//                formList.AutoScaleMode = AutoScaleMode.Font;
//                formList.FormBorderStyle = FormBorderStyle.FixedSingle;
//                formList.StartPosition = FormStartPosition.CenterScreen;
//                formList.Text = strTraduction("formTitle");
//                formList.Controls.Add(listView);
//                formList.Controls.Add(lblInfo);
//                formList.Controls.Add(btnConnect);
//                formList.Controls.Add(btnRefresh);

//                // Start to update our list
//                ImageList imgL = new ImageList();
//                //imgL.Images.Add(Resources.ps3);
//                listView.SmallImageList = imgL;
//                int sizeData = data.Count();

//                for (int i = 0; i < sizeData; i++)
//                {
//                    ListViewItem item = new ListViewItem(" " + data[i].Name + " - " + data[i].Ip);
//                    item.ImageIndex = 0;
//                    listView.Items.Add(item);
//                }

//                // If there are more than 0 targets we show the form
//                // Else we inform the user to create a console.
//                if (sizeData > 0)
//                    formList.ShowDialog();
//                else
//                {
//                    Result = false;
//                    formList.Close();
//                    MessageBox.Show(strTraduction("noConsole"), strTraduction("noConsoleTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
//                }

//                return Result;
//                */

//                // Font
//                //Create your private font collection object.
//                PrivateFontCollection pfc = new PrivateFontCollection();

//                //Select your font from the resources.
//                //My font here is "Digireu.ttf"
//                int fontLength = Better_Craft.Properties.Resources.minecraftFont.Length;

//                // create a buffer to read in to
//                byte[] fontdata = Better_Craft.Properties.Resources.minecraftFont;

//                // create an unsafe memory block for the font data
//                System.IntPtr data = Marshal.AllocCoTaskMem(fontLength);

//                // copy the bytes to the unsafe memory block
//                Marshal.Copy(fontdata, 0, data, fontLength);

//                // pass the font to the font collection
//                pfc.AddMemoryFont(data, fontLength);

//                Font minecraftFont = new Font(pfc.Families[0], 9);

//                SoundPlayer clickSound = new SoundPlayer(Better_Craft.Properties.Resources.minecraftClick);

//                bool Result = false;
//                int tNum = -1;
//                this.panel1.BackColor = Color.FromArgb(55, 55, 55);
//                this.panel1.Dock = DockStyle.Left;
//                this.panel1.ForeColor = Color.White;
//                this.panel1.Location = new Point(0, 0);
//                this.panel1.Name = "panel1";
//                this.panel1.Size = new Size(3, 195);
//                this.panel1.TabIndex = 0;

//                this.panel2.BackColor = Color.FromArgb(55, 55, 55);
//                this.panel2.Dock = DockStyle.Right;
//                this.panel2.ForeColor = Color.White;
//                this.panel2.Location = new Point(294, 0);
//                this.panel2.Name = "panel2";
//                this.panel2.Size = new Size(3, 195);
//                this.panel2.TabIndex = 1;

//                this.panel3.BackColor = Color.FromArgb(55, 55, 55);
//                this.panel3.Controls.Add(this.label1);
//                this.panel3.Controls.Add(this.button1);
//                this.panel3.Controls.Add(this.panel5);
//                this.panel3.Dock = DockStyle.Top;
//                this.panel3.ForeColor = Color.White;
//                this.panel3.Location = new Point(3, 0);
//                this.panel3.Name = "panel3";
//                this.panel3.Size = new Size(291, 25);
//                this.panel3.TabIndex = 2;
//                this.panel3.MouseDown += new MouseEventHandler(this.xMouseDown);
//                this.panel3.MouseMove += new MouseEventHandler(this.xMouseMove);

//                this.panel4.BackColor = Color.FromArgb(55, 55, 55);
//                this.panel4.Dock = DockStyle.Bottom;
//                this.panel4.ForeColor = Color.White;
//                this.panel4.Location = new Point(3, 170);
//                this.panel4.Name = "panel4";
//                this.panel4.Size = new Size(291, 3);
//                this.panel4.TabIndex = 3;

//                this.button1.BackColor = Color.FromArgb(55, 55, 55);
//                this.button1.FlatAppearance.BorderColor = Color.FromArgb(50, 50, 50);
//                this.button1.FlatAppearance.BorderSize = 0;
//                this.button1.FlatStyle = FlatStyle.Flat;
//                this.button1.Font = new Font("Segoe UI", 9f, FontStyle.Bold, GraphicsUnit.Point, 0);
//                this.button1.ForeColor = Color.White;
//                this.button1.Location = new Point(262, 0);
//                this.button1.Name = "button1";
//                this.button1.Size = new Size(30, 25);
//                this.button1.TabIndex = 4;
//                this.button1.Text = "X";
//                this.button1.UseVisualStyleBackColor = false;
//                this.button1.Click += new EventHandler(this.button1_Click);

//                this.label1.AutoSize = true;
//                this.label1.Font = new Font("Segoe UI", 9f, FontStyle.Bold, GraphicsUnit.Point, 0);
//                this.label1.ForeColor = Color.White;
//                this.label1.Location = new Point(32, 6);
//                this.label1.Name = "label1";
//                this.label1.Size = new Size(130, 15);
//                this.label1.TabIndex = 0;
//                this.label1.Text = "Connect To Ps3";
//                this.label1.MouseDown += new MouseEventHandler(this.xMouseDown);
//                this.label1.MouseMove += new MouseEventHandler(this.xMouseMove);

//                this.panel5.BackColor = Color.FromArgb(55, 55, 55);
//                this.panel5.BackgroundImage = Better_Craft.Properties.Resources.TNT.ToBitmap();
//                this.panel5.BackgroundImageLayout = ImageLayout.Stretch;
//                this.panel5.ForeColor = Color.FromArgb(110, 110, 110);
//                this.panel5.Location = new Point(3, 1);
//                this.panel5.Name = "panel5";
//                this.panel5.Size = new Size(22, 22);
//                this.panel5.TabIndex = 4;

//                this.btnConnect.Location = new Point(23, 260);
//                this.btnConnect.Name = "btnConnect";
//                this.btnConnect.Size = new Size(250, 33);
//                this.btnConnect.BackColor = Color.FromArgb(110, 110, 110);
//                this.btnConnect.BackgroundImage = new Bitmap(Better_Craft.Properties.Resources.defaultButton);
//                this.btnConnect.BackgroundImageLayout = ImageLayout.Stretch;
//                this.btnConnect.Font = minecraftFont;
//                this.btnConnect.ForeColor = Color.White;
//                this.btnConnect.FlatStyle = FlatStyle.Flat;
//                this.btnConnect.FlatAppearance.BorderSize = 0;
//                this.btnConnect.TabIndex = 1;
//                this.btnConnect.Text = this.strTraduction("btnConnect");
//                this.btnConnect.UseVisualStyleBackColor = true;
//                this.btnConnect.Enabled = true;
//                this.btnConnect.Click += delegate (object sender, EventArgs e)
//                {
//                    clickSound.Play();
//                    if (tNum > -1)
//                    {
//                        if (this.Api.ConnectTarget(this.data[tNum].Ip))
//                        {
//                            this.Api.setTargetName(this.data[tNum].Name);
//                            Result = true;
//                        }
//                        else
//                        {
//                            Result = false;
//                        }
//                        this.formList.Close();
//                    }
//                    else
//                    {
//                        MessageBox.Show(this.strTraduction("errorSelect"), "Connect to Ps3", MessageBoxButtons.OK, MessageBoxIcon.Hand);
//                    }
//                };
//                //this.btnConnect.MouseUp += delegate (object sender, MouseEventArgs e)
//                //{
//                //    this.btnConnect.BackgroundImage = new Bitmap(Better_Craft.Properties.Resources.defaultButton);
//                //};
//                this.btnConnect.MouseHover += delegate (object sender, EventArgs e)
//                {
//                    btnConnect.BackgroundImage = new Bitmap(Better_Craft.Properties.Resources.highlightButton);
//                };
//                this.btnConnect.MouseLeave += delegate (object sender, EventArgs e)
//                {
//                    btnConnect.BackgroundImage = new Bitmap(Better_Craft.Properties.Resources.defaultButton);
//                };

//                this.btnRefresh.Location = new Point(23, 297);
//                this.btnRefresh.Name = "btnRefresh";
//                this.btnRefresh.BackColor = Color.FromArgb(110, 110, 110);
//                this.btnRefresh.BackgroundImage = new Bitmap(Better_Craft.Properties.Resources.defaultButton);
//                this.btnRefresh.BackgroundImageLayout = ImageLayout.Stretch;
//                this.btnRefresh.Font = minecraftFont;
//                this.btnRefresh.ForeColor = Color.White;
//                this.btnRefresh.FlatStyle = FlatStyle.Flat;
//                this.btnRefresh.FlatAppearance.BorderSize = 0;
//                this.btnRefresh.Size = new Size(250, 33);
//                this.btnRefresh.TabIndex = 1;
//                this.btnRefresh.Text = this.strTraduction("btnRefresh");
//                this.btnRefresh.UseVisualStyleBackColor = true;
//                this.btnRefresh.Click += delegate (object sender, EventArgs e)
//                {
//                    clickSound.Play();
//                    tNum = -1;
//                    this.listView.Clear();
//                    this.btnConnect.Enabled = false;
//                    this.data = this.Api.CCAPI.GetConsoleList();
//                    int num2 = this.data.Count<CCAPI.ConsoleInfo>();
//                    for (int j = 0; j < num2; j++)
//                    {
//                        ListViewItem listViewItem2 = new ListViewItem(this.data[j].Name + " - " + this.data[j].Ip);
//                        listViewItem2.ImageIndex = 0;
//                        this.listView.Items.Add(listViewItem2);
//                    }
//                };
//                //this.btnConnect.MouseUp += delegate (object sender, MouseEventArgs e)
//                //{
//                //    this.btnConnect.BackgroundImage = new Bitmap(Better_Craft.Properties.Resources.defaultButton);
//                //};
//                this.btnRefresh.MouseHover += delegate (object sender, EventArgs e)
//                {
//                    btnRefresh.BackgroundImage = new Bitmap(Better_Craft.Properties.Resources.highlightButton);
//                };
//                this.btnRefresh.MouseLeave += delegate (object sender, EventArgs e)
//                {
//                    btnRefresh.BackgroundImage = new Bitmap(Better_Craft.Properties.Resources.defaultButton);
//                };

//                this.listView.BackColor = Color.FromArgb(110, 110, 110);
//                this.listView.BackgroundImage = new Bitmap(Better_Craft.Properties.Resources.betterCraftStoneBackground);
//                this.listView.ForeColor = Color.White;
//                this.listView.Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
//                this.listViewGroup.Header = "Consoles";
//                this.listViewGroup.Name = "consoleGroup";
//                this.listView.Groups.AddRange(new ListViewGroup[]
//                {
//                    this.listViewGroup
//                });
//                this.listView.HideSelection = false;
//                this.listView.Location = new Point(23, 41);
//                this.listView.MultiSelect = false;
//                this.listView.Name = "ConsoleList";
//                this.listView.ShowGroups = false;
//                this.listView.Size = new Size(250, 215);
//                this.listView.TabIndex = 0;
//                this.listView.UseCompatibleStateImageBehavior = false;
//                this.listView.View = View.List;
//                this.listView.ItemSelectionChanged += delegate (object sender, ListViewItemSelectionChangedEventArgs e)
//                {
//                    clickSound.Play();
//                    tNum = e.ItemIndex;
//                    this.btnConnect.Enabled = true;
//                    if (this.data[tNum].Name.Length > 18)
//                    {
//                        string text = this.data[tNum].Name.Substring(0, 17) + "...";
//                    }
//                    else
//                    {
//                        string text = this.data[tNum].Name;
//                    }
//                    if (this.data[tNum].Ip.Length > 16)
//                    {
//                        string text2 = this.data[tNum].Name.Substring(0, 16) + "...";
//                    }
//                    else
//                    {
//                        string text2 = this.data[tNum].Ip;
//                    }
//                };

//                this.formList.MinimizeBox = false;
//                this.formList.MaximizeBox = false;
//                this.formList.ClientSize = new Size(297, 350);
//                this.formList.AutoScaleDimensions = new SizeF(6f, 13f);
//                this.formList.AutoScaleMode = AutoScaleMode.Font;
//                this.formList.BackColor = Color.FromArgb(25, 25, 25);
//                this.formList.ForeColor = Color.FromArgb(55, 55, 55);
//                this.formList.FormBorderStyle = FormBorderStyle.None;
//                this.formList.StartPosition = FormStartPosition.CenterScreen;
//                this.formList.Text = "Connect To Ps3";
//                this.formList.Controls.Add(this.listView);
//                this.formList.Controls.Add(this.btnConnect);
//                this.formList.Controls.Add(this.btnRefresh);
//                this.formList.Controls.Add(this.panel4);
//                this.formList.Controls.Add(this.panel3);
//                this.formList.Controls.Add(this.panel2);
//                this.formList.Controls.Add(this.panel1);
//                this.panel1.ResumeLayout(false);
//                this.panel1.PerformLayout();
//                this.panel2.ResumeLayout(false);
//                this.panel2.PerformLayout();
//                this.panel3.ResumeLayout(false);
//                this.panel3.PerformLayout();
//                this.panel4.ResumeLayout(false);
//                this.panel4.PerformLayout();
//                this.formList.ResumeLayout(false);
//                this.formList.PerformLayout();
//                int num = this.data.Count<CCAPI.ConsoleInfo>();
//                for (int i = 0; i < num; i++)
//                {
//                    ListViewItem listViewItem = new ListViewItem(" " + this.data[i].Name + " - " + this.data[i].Ip);
//                    listViewItem.ImageIndex = 0;
//                    this.listView.Items.Add(listViewItem);
//                }
//                if (num > 0)
//                {
//                    this.formList.ShowDialog();
//                }
//                else
//                {
//                    Result = false;
//                    this.formList.Close();
//                    MessageBox.Show(this.strTraduction("noConsole"), this.strTraduction("noConsoleTitle"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
//                }
//                return Result;
//            }
//        }
//    }
//}
#endregion

//// ************************************************* //
////    --- Copyright (c) 2015 iMCS Productions ---    //
//// ************************************************* //
////              PS3Lib v4 By FM|T iMCSx              //
////          PS3MAPI support added by: _NzV_          //
////    PS3MAPI Decompiled and edited for BetterCraft  //
////                                                   //
//// Features v4.5 :                                   //
//// - Support CCAPI v2.60+ C# by iMCSx.               //
//// - Read/Write memory as 'double'.                  //
//// - Read/Write memory as 'float' array.             //
//// - Constructor overload for ArrayBuilder.          //
//// - Some functions fixes.                           //
////                                                   //
//// Credits : Enstone, Buc-ShoTz, _NzV_               //
////                                                   //
//// Follow me :                                       //
////                                                   //
//// FrenchModdingTeam.com                             //
//// Twitter.com/iMCSx                                 //
//// Facebook.com/iMCSx                                //
////                                                   //
//// ************************************************* //

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PS3ManagerAPI;

[assembly: System.Windows.Media.DisableDpiAwareness]
namespace PS3Lib
{
    public enum SelectAPI
    {
        ControlConsole,
        TargetManager,
        PS3Manager
    }

    public enum Lang
    {
        Null,
        French,
        English,
        German
    }

    public class PS3API
    {
        private class SetLang
        {
            public static Lang defaultLang = Lang.Null;
        }

        /// <summary>Force a language for the console list popup.</summary>
        public void SetFormLang(Lang Language)
        {
            SetLang.defaultLang = Language;
        }

        public PS3API(SelectAPI API = SelectAPI.TargetManager)
        {
            PS3API.SetAPI.API = API;
            this.MakeInstanceAPI(API);
        }

        public void setTargetName(string value)
        {
            PS3API.targetName = value;
        }

        private void MakeInstanceAPI(SelectAPI API)
        {
            if (API == SelectAPI.TargetManager)
            {
                if (PS3API.Common.TmApi == null)
                {
                    PS3API.Common.TmApi = new TMAPI();
                    return;
                }
            }
            else if (API == SelectAPI.ControlConsole)
            {
                if (PS3API.Common.CcApi == null)
                {
                    PS3API.Common.CcApi = new CCAPI();
                    return;
                }
            }
            else if (API == SelectAPI.PS3Manager && PS3API.Common.Ps3mApi == null)
            {
                PS3API.Common.Ps3mApi = new PS3MAPI();
            }
        }

        /// <summary>init again the connection if you use a Thread or a Timer.</summary>
        public void InitTarget()
        {
            if (PS3API.SetAPI.API == SelectAPI.TargetManager)
            {
                PS3API.Common.TmApi.InitComms();
            }
        }

        /// <summary>Connect your console with selected API.</summary>
        public bool ConnectTarget(int target = 0)
        {
            this.MakeInstanceAPI(this.GetCurrentAPI());
            bool result = false;
            if (PS3API.SetAPI.API == SelectAPI.TargetManager)
            {
                result = PS3API.Common.TmApi.ConnectTarget(target);
            }
            else if (PS3API.SetAPI.API == SelectAPI.ControlConsole)
            {
                result = new ConsoleList(this).Show();
            }
            else if (PS3API.SetAPI.API == SelectAPI.PS3Manager)
            {
                result = PS3API.Common.Ps3mApi.ConnectTarget();
            }
            return result;
        }

        /// <summary>Connect your console with selected API.</summary>
        public bool ConnectTarget(string ip)
        {
            this.MakeInstanceAPI(this.GetCurrentAPI());
            bool flag = false;
            if (PS3API.SetAPI.API == SelectAPI.ControlConsole)
            {
                if (PS3API.Common.CcApi.SUCCESS(PS3API.Common.CcApi.ConnectTarget(ip)))
                {
                    PS3API.targetIp = ip;
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            else if (PS3API.SetAPI.API == SelectAPI.PS3Manager)
            {
                flag = PS3API.Common.Ps3mApi.ConnectTarget(ip);
                if (flag)
                {
                    PS3API.targetIp = ip;
                }
            }
            return flag;
        }

        /// <summary>Connect your console with PS3MAPI using ip and port.</summary>
        public bool ConnectTarget(string ip, int port)
        {
            this.MakeInstanceAPI(this.GetCurrentAPI());
            bool flag = false;
            if (PS3API.SetAPI.API == SelectAPI.PS3Manager)
            {
                flag = PS3API.Common.Ps3mApi.ConnectTarget(ip, port);
                if (flag)
                {
                    PS3API.targetIp = ip;
                }
            }
            return flag;
        }

        /// <summary>Disconnect Target with selected API.</summary>
        public void DisconnectTarget()
        {
            if (PS3API.SetAPI.API == SelectAPI.TargetManager)
            {
                PS3API.Common.TmApi.DisconnectTarget();
                return;
            }
            if (PS3API.SetAPI.API == SelectAPI.ControlConsole)
            {
                PS3API.Common.CcApi.DisconnectTarget();
                return;
            }
            if (PS3API.SetAPI.API == SelectAPI.PS3Manager)
            {
                PS3API.Common.Ps3mApi.DisconnectTarget();
            }
        }

        /// <summary>Attach the current process (current Game) with selected API.</summary>
        public bool AttachProcess()
        {
            this.MakeInstanceAPI(this.GetCurrentAPI());
            bool result = false;
            if (PS3API.SetAPI.API == SelectAPI.TargetManager)
            {
                result = PS3API.Common.TmApi.AttachProcess();
            }
            else if (PS3API.SetAPI.API == SelectAPI.ControlConsole)
            {
                result = PS3API.Common.CcApi.SUCCESS(PS3API.Common.CcApi.AttachProcess());
            }
            else if (PS3API.SetAPI.API == SelectAPI.PS3Manager)
            {
                result = PS3API.Common.Ps3mApi.AttachProcess();
            }
            return result;
        }

        public string GetConsoleName()
        {
            if (PS3API.SetAPI.API == SelectAPI.TargetManager)
            {
                return PS3API.Common.TmApi.SCE.GetTargetName();
            }
            if (PS3API.SetAPI.API == SelectAPI.ControlConsole)
            {
                if (PS3API.targetName != string.Empty)
                {
                    return PS3API.targetName;
                }
                if (PS3API.targetIp != string.Empty)
                {
                    List<CCAPI.ConsoleInfo> list = new List<CCAPI.ConsoleInfo>();
                    list = PS3API.Common.CcApi.GetConsoleList();
                    if (list.Count > 0)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (list[i].Ip == PS3API.targetIp)
                            {
                                return list[i].Name;
                            }
                        }
                    }
                }
                return PS3API.targetIp;
            }
            else
            {
                if (PS3API.SetAPI.API == SelectAPI.PS3Manager)
                {
                    return "PS3 Manager API";
                }
                return "none";
            }
        }

        /// <summary>Set memory to offset with selected API.</summary>
        public void SetMemory(uint offset, byte[] buffer)
        {
            if (PS3API.SetAPI.API == SelectAPI.TargetManager)
            {
                PS3API.Common.TmApi.SetMemory(offset, buffer);
                return;
            }
            if (PS3API.SetAPI.API == SelectAPI.ControlConsole)
            {
                PS3API.Common.CcApi.SetMemory(offset, buffer);
                return;
            }
            if (PS3API.SetAPI.API == SelectAPI.PS3Manager)
            {
                PS3API.Common.Ps3mApi.SetMemory(offset, buffer);
            }
        }

        /// <summary>Get memory from offset using the Selected API.</summary>
        public void GetMemory(uint offset, byte[] buffer)
        {
            if (PS3API.SetAPI.API == SelectAPI.TargetManager)
            {
                PS3API.Common.TmApi.GetMemory(offset, buffer);
                return;
            }
            if (PS3API.SetAPI.API == SelectAPI.ControlConsole)
            {
                PS3API.Common.CcApi.GetMemory(offset, buffer);
                return;
            }
            if (PS3API.SetAPI.API == SelectAPI.PS3Manager)
            {
                PS3API.Common.Ps3mApi.GetMemory(offset, buffer);
            }
        }

        /// <summary>Get memory from offset with a length using the Selected API.</summary>
        public byte[] GetBytes(uint offset, int length)
        {
            byte[] array = new byte[length];
            if (PS3API.SetAPI.API == SelectAPI.TargetManager)
            {
                PS3API.Common.TmApi.GetMemory(offset, array);
            }
            else if (PS3API.SetAPI.API == SelectAPI.ControlConsole)
            {
                PS3API.Common.CcApi.GetMemory(offset, array);
            }
            else if (PS3API.SetAPI.API == SelectAPI.PS3Manager)
            {
                PS3API.Common.Ps3mApi.GetMemory(offset, array);
            }
            return array;
        }

        public void Notify(string msg, CCAPI.NotifyIcon icon = CCAPI.NotifyIcon.INFO)
        {
            if (PS3API.SetAPI.API == SelectAPI.ControlConsole)
            {
                PS3API.Common.CcApi.Notify(icon, msg);
                return;
            }
            if (PS3API.SetAPI.API == SelectAPI.PS3Manager)
            {
                PS3API.Common.Ps3mApi.Notify(msg);
            }
        }

        public void Power(PS3API.PowerFlags flag)
        {
            if (PS3API.SetAPI.API == SelectAPI.ControlConsole)
            {
                if (flag == PS3API.PowerFlags.ShutDown)
                {
                    PS3API.Common.CcApi.ShutDown(CCAPI.RebootFlags.ShutDown);
                    return;
                }
                if (flag == PS3API.PowerFlags.QuickReboot)
                {
                    PS3API.Common.CcApi.ShutDown(CCAPI.RebootFlags.SoftReboot);
                    return;
                }
                if (flag == PS3API.PowerFlags.SoftReboot)
                {
                    PS3API.Common.CcApi.ShutDown(CCAPI.RebootFlags.SoftReboot);
                    return;
                }
                if (flag == PS3API.PowerFlags.HardReboot)
                {
                    PS3API.Common.CcApi.ShutDown(CCAPI.RebootFlags.HardReboot);
                    return;
                }
            }
            else if (PS3API.SetAPI.API == SelectAPI.PS3Manager)
            {
                if (flag == PS3API.PowerFlags.ShutDown)
                {
                    PS3API.Common.Ps3mApi.Power(PS3ManagerAPI.PS3MAPI.PS3_CMD.PowerFlags.ShutDown);
                    return;
                }
                if (flag == PS3API.PowerFlags.QuickReboot)
                {
                    PS3API.Common.Ps3mApi.Power(PS3ManagerAPI.PS3MAPI.PS3_CMD.PowerFlags.QuickReboot);
                    return;
                }
                if (flag == PS3API.PowerFlags.SoftReboot)
                {
                    PS3API.Common.Ps3mApi.Power(PS3ManagerAPI.PS3MAPI.PS3_CMD.PowerFlags.SoftReboot);
                    return;
                }
                if (flag == PS3API.PowerFlags.HardReboot)
                {
                    PS3API.Common.Ps3mApi.Power(PS3ManagerAPI.PS3MAPI.PS3_CMD.PowerFlags.ShutDown);
                }
            }
        }

        public void SetConsoleID(string consoleID)
        {
            if (PS3API.SetAPI.API == SelectAPI.ControlConsole)
            {
                PS3API.Common.CcApi.SetConsoleID(consoleID);
                return;
            }
            if (PS3API.SetAPI.API == SelectAPI.PS3Manager)
            {
                PS3API.Common.Ps3mApi.SetConsoleID(consoleID);
            }
        }

        public void SetConsoleID(byte[] consoleID)
        {
            if (PS3API.SetAPI.API == SelectAPI.ControlConsole)
            {
                PS3API.Common.CcApi.SetConsoleID(consoleID);
                return;
            }
            if (PS3API.SetAPI.API == SelectAPI.PS3Manager)
            {
                PS3API.Common.Ps3mApi.SetConsoleID(consoleID);
            }
        }

        public void SetPSID(string PSID)
        {
            if (PS3API.SetAPI.API == SelectAPI.ControlConsole)
            {
                PS3API.Common.CcApi.SetPSID(PSID);
                return;
            }
            if (PS3API.SetAPI.API == SelectAPI.PS3Manager)
            {
                PS3API.Common.Ps3mApi.SetPSID(PSID);
            }
        }

        public void SetPSID(byte[] PSID)
        {
            if (PS3API.SetAPI.API == SelectAPI.ControlConsole)
            {
                PS3API.Common.CcApi.SetPSID(PSID);
                return;
            }
            if (PS3API.SetAPI.API == SelectAPI.PS3Manager)
            {
                PS3API.Common.Ps3mApi.SetPSID(PSID);
            }
        }

        public void Buzzer(PS3API.BuzzerMode flag)
        {
            if (PS3API.SetAPI.API == SelectAPI.ControlConsole)
            {
                if (flag == PS3API.BuzzerMode.Single)
                {
                    PS3API.Common.CcApi.RingBuzzer(CCAPI.BuzzerMode.Single);
                    return;
                }
                if (flag == PS3API.BuzzerMode.Double)
                {
                    PS3API.Common.CcApi.RingBuzzer(CCAPI.BuzzerMode.Double);
                    return;
                }
                if (flag == PS3API.BuzzerMode.Triple)
                {
                    PS3API.Common.CcApi.RingBuzzer(CCAPI.BuzzerMode.Continuous);
                    return;
                }
            }
            else if (PS3API.SetAPI.API == SelectAPI.PS3Manager)
            {
                if (flag == PS3API.BuzzerMode.Single)
                {
                    PS3API.Common.Ps3mApi.RingBuzzer(PS3ManagerAPI.PS3MAPI.PS3_CMD.BuzzerMode.Single);
                    return;
                }
                if (flag == PS3API.BuzzerMode.Double)
                {
                    PS3API.Common.Ps3mApi.RingBuzzer(PS3ManagerAPI.PS3MAPI.PS3_CMD.BuzzerMode.Double);
                    return;
                }
                if (flag == PS3API.BuzzerMode.Triple)
                {
                    PS3API.Common.Ps3mApi.RingBuzzer(PS3ManagerAPI.PS3MAPI.PS3_CMD.BuzzerMode.Triple);
                }
            }
        }

        public void Led(PS3API.LedColor color, PS3API.LedMode mode)
        {
            if (PS3API.SetAPI.API == SelectAPI.ControlConsole)
            {
                if (color == PS3API.LedColor.Red && mode == PS3API.LedMode.Off)
                {
                    PS3API.Common.CcApi.SetConsoleLed(CCAPI.LedColor.Red, CCAPI.LedMode.Off);
                    return;
                }
                if (color == PS3API.LedColor.Red && mode == PS3API.LedMode.On)
                {
                    PS3API.Common.CcApi.SetConsoleLed(CCAPI.LedColor.Red, CCAPI.LedMode.On);
                    return;
                }
                if (color == PS3API.LedColor.Red && mode == PS3API.LedMode.BlinkFast)
                {
                    PS3API.Common.CcApi.SetConsoleLed(CCAPI.LedColor.Red, CCAPI.LedMode.Blink);
                    return;
                }
                if (color == PS3API.LedColor.Red && mode == PS3API.LedMode.BlinkSlow)
                {
                    PS3API.Common.CcApi.SetConsoleLed(CCAPI.LedColor.Red, CCAPI.LedMode.Blink);
                    return;
                }
                if (color == PS3API.LedColor.Green && mode == PS3API.LedMode.Off)
                {
                    PS3API.Common.CcApi.SetConsoleLed(CCAPI.LedColor.Green, CCAPI.LedMode.Off);
                    return;
                }
                if (color == PS3API.LedColor.Green && mode == PS3API.LedMode.On)
                {
                    PS3API.Common.CcApi.SetConsoleLed(CCAPI.LedColor.Green, CCAPI.LedMode.On);
                    return;
                }
                if (color == PS3API.LedColor.Green && mode == PS3API.LedMode.BlinkFast)
                {
                    PS3API.Common.CcApi.SetConsoleLed(CCAPI.LedColor.Green, CCAPI.LedMode.Blink);
                    return;
                }
                if (color == PS3API.LedColor.Green && mode == PS3API.LedMode.BlinkSlow)
                {
                    PS3API.Common.CcApi.SetConsoleLed(CCAPI.LedColor.Green, CCAPI.LedMode.Blink);
                    return;
                }
                if (color == PS3API.LedColor.Yellow && mode == PS3API.LedMode.Off)
                {
                    PS3API.Common.CcApi.SetConsoleLed(CCAPI.LedColor.Red, CCAPI.LedMode.Off);
                    return;
                }
                if (color == PS3API.LedColor.Yellow && mode == PS3API.LedMode.On)
                {
                    PS3API.Common.CcApi.SetConsoleLed(CCAPI.LedColor.Red, CCAPI.LedMode.On);
                    return;
                }
                if (color == PS3API.LedColor.Yellow && mode == PS3API.LedMode.BlinkFast)
                {
                    PS3API.Common.CcApi.SetConsoleLed(CCAPI.LedColor.Red, CCAPI.LedMode.Blink);
                    return;
                }
                if (color == PS3API.LedColor.Yellow && mode == PS3API.LedMode.BlinkSlow)
                {
                    PS3API.Common.CcApi.SetConsoleLed(CCAPI.LedColor.Red, CCAPI.LedMode.Blink);
                    return;
                }
            }
            else if (PS3API.SetAPI.API == SelectAPI.PS3Manager)
            {
                if (color == PS3API.LedColor.Red && mode == PS3API.LedMode.Off)
                {
                    PS3API.Common.Ps3mApi.SetConsoleLed(PS3ManagerAPI.PS3MAPI.PS3_CMD.LedColor.Red, PS3ManagerAPI.PS3MAPI.PS3_CMD.LedMode.Off);
                    return;
                }
                if (color == PS3API.LedColor.Red && mode == PS3API.LedMode.On)
                {
                    PS3API.Common.Ps3mApi.SetConsoleLed(PS3ManagerAPI.PS3MAPI.PS3_CMD.LedColor.Red, PS3ManagerAPI.PS3MAPI.PS3_CMD.LedMode.On);
                    return;
                }
                if (color == PS3API.LedColor.Red && mode == PS3API.LedMode.BlinkFast)
                {
                    PS3API.Common.Ps3mApi.SetConsoleLed(PS3ManagerAPI.PS3MAPI.PS3_CMD.LedColor.Red, PS3ManagerAPI.PS3MAPI.PS3_CMD.LedMode.BlinkFast);
                    return;
                }
                if (color == PS3API.LedColor.Red && mode == PS3API.LedMode.BlinkSlow)
                {
                    PS3API.Common.Ps3mApi.SetConsoleLed(PS3ManagerAPI.PS3MAPI.PS3_CMD.LedColor.Red, PS3ManagerAPI.PS3MAPI.PS3_CMD.LedMode.BlinkSlow);
                    return;
                }
                if (color == PS3API.LedColor.Green && mode == PS3API.LedMode.Off)
                {
                    PS3API.Common.Ps3mApi.SetConsoleLed(PS3ManagerAPI.PS3MAPI.PS3_CMD.LedColor.Green, PS3ManagerAPI.PS3MAPI.PS3_CMD.LedMode.Off);
                    return;
                }
                if (color == PS3API.LedColor.Green && mode == PS3API.LedMode.On)
                {
                    PS3API.Common.Ps3mApi.SetConsoleLed(PS3ManagerAPI.PS3MAPI.PS3_CMD.LedColor.Green, PS3ManagerAPI.PS3MAPI.PS3_CMD.LedMode.On);
                    return;
                }
                if (color == PS3API.LedColor.Green && mode == PS3API.LedMode.BlinkFast)
                {
                    PS3API.Common.Ps3mApi.SetConsoleLed(PS3ManagerAPI.PS3MAPI.PS3_CMD.LedColor.Green, PS3ManagerAPI.PS3MAPI.PS3_CMD.LedMode.BlinkFast);
                    return;
                }
                if (color == PS3API.LedColor.Green && mode == PS3API.LedMode.BlinkSlow)
                {
                    PS3API.Common.Ps3mApi.SetConsoleLed(PS3ManagerAPI.PS3MAPI.PS3_CMD.LedColor.Green, PS3ManagerAPI.PS3MAPI.PS3_CMD.LedMode.BlinkSlow);
                    return;
                }
                if (color == PS3API.LedColor.Yellow && mode == PS3API.LedMode.Off)
                {
                    PS3API.Common.Ps3mApi.SetConsoleLed(PS3ManagerAPI.PS3MAPI.PS3_CMD.LedColor.Yellow, PS3ManagerAPI.PS3MAPI.PS3_CMD.LedMode.Off);
                    return;
                }
                if (color == PS3API.LedColor.Yellow && mode == PS3API.LedMode.On)
                {
                    PS3API.Common.Ps3mApi.SetConsoleLed(PS3ManagerAPI.PS3MAPI.PS3_CMD.LedColor.Yellow, PS3ManagerAPI.PS3MAPI.PS3_CMD.LedMode.On);
                    return;
                }
                if (color == PS3API.LedColor.Yellow && mode == PS3API.LedMode.BlinkFast)
                {
                    PS3API.Common.Ps3mApi.SetConsoleLed(PS3ManagerAPI.PS3MAPI.PS3_CMD.LedColor.Yellow, PS3ManagerAPI.PS3MAPI.PS3_CMD.LedMode.BlinkFast);
                    return;
                }
                if (color == PS3API.LedColor.Yellow && mode == PS3API.LedMode.BlinkSlow)
                {
                    PS3API.Common.Ps3mApi.SetConsoleLed(PS3ManagerAPI.PS3MAPI.PS3_CMD.LedColor.Yellow, PS3ManagerAPI.PS3MAPI.PS3_CMD.LedMode.BlinkSlow);
                }
            }
        }

        /// <summary>Change current API.</summary>
        public void ChangeAPI(SelectAPI API)
        {
            PS3API.SetAPI.API = API;
            this.MakeInstanceAPI(this.GetCurrentAPI());
        }

        /// <summary>Return selected API.</summary>
        public SelectAPI GetCurrentAPI()
        {
            return PS3API.SetAPI.API;
        }

        /// <summary>Return selected API into string format.</summary>
        public string GetCurrentAPIName()
        {
            string result = string.Empty;
            if (PS3API.SetAPI.API == SelectAPI.TargetManager)
            {
                result = Enum.GetName(typeof(SelectAPI), SelectAPI.TargetManager).Replace("Manager", " Manager");
            }
            else if (PS3API.SetAPI.API == SelectAPI.ControlConsole)
            {
                result = Enum.GetName(typeof(SelectAPI), SelectAPI.ControlConsole).Replace("Console", " Console");
            }
            else if (PS3API.SetAPI.API == SelectAPI.PS3Manager)
            {
                result = Enum.GetName(typeof(SelectAPI), SelectAPI.PS3Manager).Replace("Manager", " Manager");
            }
            return result;
        }

        /// <summary>This will find the dll ps3tmapi_net.dll for TMAPI.</summary>
        public Assembly PS3TMAPI_NET()
        {
            return PS3API.Common.TmApi.PS3TMAPI_NET();
        }

        /// <summary>Use the extension class with your selected API.</summary>
        public Extension Extension
        {
            get
            {
                return new Extension(PS3API.SetAPI.API);
            }
        }

        /// <summary>Access to all TMAPI functions.</summary>
        public TMAPI TMAPI
        {
            get
            {
                return new TMAPI();
            }
        }

        /// <summary>Access to all CCAPI functions.</summary>
        public CCAPI CCAPI
        {
            get
            {
                return new CCAPI();
            }
        }

        /// <summary>Access to all PS3MAPI functions.</summary>
        public PS3MAPI PS3MAPI
        {
            get
            {
                return new PS3MAPI();
            }
        }

        private static string targetName = string.Empty;

        private static string targetIp = string.Empty;

        private class SetAPI
        {
            public static SelectAPI API;
        }

        private class Common
        {
            public static CCAPI CcApi;

            public static TMAPI TmApi;

            public static PS3MAPI Ps3mApi;
        }

        public enum PowerFlags
        {
            ShutDown,
            QuickReboot,
            SoftReboot,
            HardReboot
        }

        public enum BuzzerMode
        {
            Single,
            Double,
            Triple
        }

        public enum LedColor
        {
            Red,
            Green,
            Yellow
        }

        public enum LedMode
        {
            Off,
            On,
            BlinkSlow,
            BlinkFast
        }

        public class ConsoleList
        {
            private PS3API Api;
            private List<CCAPI.ConsoleInfo> data;

            public static int y;
            public static int x;
            public static Point newpoint = default(Point);
            private Panel panel1 = new Panel();
            private Panel panel2 = new Panel();
            private Panel panel3 = new Panel();
            private Panel panel4 = new Panel();
            private Panel panel5 = new Panel();
            private Button button1 = new Button();
            private Label label1 = new Label();
            private Button btnConnect = new Button();
            private Button btnRefresh = new Button();
            private ListViewGroup listViewGroup = new ListViewGroup("Consoles", HorizontalAlignment.Center);
            private ListView listView = new ListView();
            private Form formList = new Form();

            public ConsoleList(PS3API f)
            {
                Api = f;
                data = Api.CCAPI.GetConsoleList();
            }

            /// <summary>Return the systeme language, if it's others all text will be in english.</summary>
            private Lang getSysLanguage()
            {
                if (SetLang.defaultLang == Lang.Null)
                {
                    if (CultureInfo.CurrentCulture.ThreeLetterWindowsLanguageName.StartsWith("FRA"))
                        return Lang.French;
                    else if (CultureInfo.CurrentCulture.ThreeLetterWindowsLanguageName.StartsWith("GER"))
                        return Lang.German;
                    return Lang.English;
                }
                else return SetLang.defaultLang;
            }

            private string strTraduction(string keyword)
            {
                Lang lang = getSysLanguage();
                if (lang == Lang.French)
                {
                    switch (keyword)
                    {
                        case "btnConnect": return "Connexion";
                        case "btnRefresh": return "Rafraîchir";
                        case "errorSelect": return "Vous devez d'abord sélectionner une console.";
                        case "errorSelectTitle": return "Sélectionnez une console.";
                        case "selectGrid": return "Sélectionnez une console dans la grille.";
                        case "selectedLbl": return "Sélection :";
                        case "formTitle": return "Choisissez une console...";
                        case "noConsole": return "Aucune console disponible, démarrez CCAPI Manager (v2.60+) et ajoutez une nouvelle console.";
                        case "noConsoleTitle": return "Aucune console disponible.";
                    }
                }
                else if (lang == Lang.German)
                {
                    switch (keyword)
                    {
                        case "btnConnect": return "Verbinde";
                        case "btnRefresh": return "Wiederholen";
                        case "errorSelect": return "Du musst zuerst eine Konsole auswählen.";
                        case "errorSelectTitle": return "Wähle eine Konsole.";
                        case "selectGrid": return "Wähle eine Konsole innerhalb dieses Gitters.";
                        case "selectedLbl": return "Ausgewählt :";
                        case "formTitle": return "Wähle eine Konsole...";
                        case "noConsole": return "Keine Konsolen verfügbar - starte CCAPI Manager (v2.60+) und füge eine neue Konsole hinzu.";
                        case "noConsoleTitle": return "Keine Konsolen verfügbar.";
                    }
                }
                else
                {
                    switch (keyword)
                    {
                        case "btnConnect": return "Connection";
                        case "btnRefresh": return "Refresh";
                        case "errorSelect": return "You need to select a console first.";
                        case "errorSelectTitle": return "Select a console.";
                        case "selectGrid": return "Select a console within this grid.";
                        case "selectedLbl": return "Selected :";
                        case "formTitle": return "Select a console...";
                        case "noConsole": return "None consoles available, run CCAPI Manager (v2.60+) and add a new console.";
                        case "noConsoleTitle": return "None console available.";
                    }
                }
                return "?";
            }

            private void xMouseDown(object sender, MouseEventArgs e)
            {
                PS3API.ConsoleList.x = Control.MousePosition.X - this.formList.Location.X;
                PS3API.ConsoleList.y = Control.MousePosition.Y - this.formList.Location.Y;
            }

            private void xMouseMove(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                {
                    PS3API.ConsoleList.newpoint = Control.MousePosition;
                    PS3API.ConsoleList.newpoint.X = PS3API.ConsoleList.newpoint.X - PS3API.ConsoleList.x;
                    PS3API.ConsoleList.newpoint.Y = PS3API.ConsoleList.newpoint.Y - PS3API.ConsoleList.y;
                    this.formList.Location = PS3API.ConsoleList.newpoint;
                }
            }

            private void button1_Click(object sender, EventArgs e)
            {
                this.formList.Close();
            }

            public bool Show()
            {
                #region Original CCAPI Connect Form
                /*
                bool Result = false;
                int tNum = -1;

                // Instance of widgets
                Label lblInfo = new Label();
                Button btnConnect = new Button();
                Button btnRefresh = new Button();
                ListViewGroup listViewGroup = new ListViewGroup("Consoles", HorizontalAlignment.Left);
                ListView listView = new ListView();
                Form formList = new Form();

                // Create our button connect
                btnConnect.Location = new Point(12, 254);
                btnConnect.Name = "btnConnect";
                btnConnect.Size = new Size(198, 23);
                btnConnect.TabIndex = 1;
                btnConnect.Text = strTraduction("btnConnect");
                btnConnect.UseVisualStyleBackColor = true;
                btnConnect.Enabled = false;
                btnConnect.Click += (sender, e) =>
                {
                    if(tNum > -1)
                    {
                        if (Api.ConnectTarget(data[tNum].Ip))
                        {
                            Api.setTargetName(data[tNum].Name);
                            Result = true;
                        }
                        else Result = false;
                        formList.Close();
                    }
                    else
                        MessageBox.Show(strTraduction("errorSelect"), strTraduction("errorSelectTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                };

                // Create our button refresh
                btnRefresh.Location = new Point(216, 254);
                btnRefresh.Name = "btnRefresh";
                btnRefresh.Size = new Size(86, 23);
                btnRefresh.TabIndex = 1;
                btnRefresh.Text = strTraduction("btnRefresh");
                btnRefresh.UseVisualStyleBackColor = true;
                btnRefresh.Click += (sender, e) =>
                {
                    tNum = -1;
                    listView.Clear();
                    lblInfo.Text = strTraduction("selectGrid");
                    btnConnect.Enabled = false;
                    data = Api.CCAPI.GetConsoleList();
                    int sizeD = data.Count();
                    for (int i = 0; i < sizeD; i++)
                    {
                        ListViewItem item = new ListViewItem(" " + data[i].Name + " - " + data[i].Ip);
                        item.ImageIndex = 0;
                        listView.Items.Add(item);
                    }
                };

                // Create our list view
                listView.Font = new Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                listViewGroup.Header = "Consoles";
                listViewGroup.Name = "consoleGroup";
                listView.Groups.AddRange(new ListViewGroup[] {listViewGroup});
                listView.HideSelection = false;
                listView.Location = new Point(12, 12);
                listView.MultiSelect = false;
                listView.Name = "ConsoleList";
                listView.ShowGroups = false;
                listView.Size = new Size(290, 215);
                listView.TabIndex = 0;
                listView.UseCompatibleStateImageBehavior = false;
                listView.View = View.List;
                listView.ItemSelectionChanged += (sender, e) =>
                {
                    tNum = e.ItemIndex;
                    btnConnect.Enabled = true;
                    string Name, Ip = "?";
                    if (data[tNum].Name.Length > 18)
                        Name = data[tNum].Name.Substring(0, 17) + "...";
                    else Name = data[tNum].Name;
                    if (data[tNum].Ip.Length > 16)
                        Ip = data[tNum].Name.Substring(0, 16) + "...";
                    else Ip = data[tNum].Ip;
                    lblInfo.Text = strTraduction("selectedLbl") + " " + Name + " / " + Ip;
                };

                // Create our label
                lblInfo.AutoSize = true;
                lblInfo.Location = new Point(12, 234);
                lblInfo.Name = "lblInfo";
                lblInfo.Size = new Size(158, 13);
                lblInfo.TabIndex = 3;
                lblInfo.Text = strTraduction("selectGrid");

                // Create our form
                formList.MinimizeBox = false;
                formList.MaximizeBox = false;
                formList.ClientSize = new Size(314, 285);
                formList.AutoScaleDimensions = new SizeF(6F, 13F);
                formList.AutoScaleMode = AutoScaleMode.Font;
                formList.FormBorderStyle = FormBorderStyle.FixedSingle;
                formList.StartPosition = FormStartPosition.CenterScreen;
                formList.Text = strTraduction("formTitle");
                formList.Controls.Add(listView);
                formList.Controls.Add(lblInfo);
                formList.Controls.Add(btnConnect);
                formList.Controls.Add(btnRefresh);

                // Start to update our list
                ImageList imgL = new ImageList();
                //imgL.Images.Add(Resources.ps3);
                listView.SmallImageList = imgL;
                int sizeData = data.Count();

                for (int i = 0; i < sizeData; i++)
                {
                    ListViewItem item = new ListViewItem(" " + data[i].Name + " - " + data[i].Ip);
                    item.ImageIndex = 0;
                    listView.Items.Add(item);
                }

                // If there are more than 0 targets we show the form
                // Else we inform the user to create a console.
                if (sizeData > 0)
                    formList.ShowDialog();
                else
                {
                    Result = false;
                    formList.Close();
                    MessageBox.Show(strTraduction("noConsole"), strTraduction("noConsoleTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return Result;
                */
                #endregion

                // Font
                //Create your private font collection object.
                PrivateFontCollection pfc = new PrivateFontCollection();

                //Select your font from the resources.
                //My font here is "Digireu.ttf"
                int fontLength = Better_Craft.Properties.Resources.minecraftFont.Length;

                // create a buffer to read in to
                byte[] fontdata = Better_Craft.Properties.Resources.minecraftFont;

                // create an unsafe memory block for the font data
                System.IntPtr data = Marshal.AllocCoTaskMem(fontLength);

                // copy the bytes to the unsafe memory block
                Marshal.Copy(fontdata, 0, data, fontLength);

                // pass the font to the font collection
                pfc.AddMemoryFont(data, fontLength);

                Font minecraftFont = new Font(pfc.Families[0], 9);

                SoundPlayer clickSound = new SoundPlayer(Better_Craft.Properties.Resources.minecraftClick);

                bool Result = false;
                int tNum = -1;
                this.panel1.BackColor = Color.FromArgb(55, 55, 55);
                this.panel1.Dock = DockStyle.Left;
                this.panel1.ForeColor = Color.White;
                this.panel1.Location = new Point(0, 0);
                this.panel1.Name = "panel1";
                this.panel1.Size = new Size(3, 195);
                this.panel1.TabIndex = 0;

                this.panel2.BackColor = Color.FromArgb(55, 55, 55);
                this.panel2.Dock = DockStyle.Right;
                this.panel2.ForeColor = Color.White;
                this.panel2.Location = new Point(294, 0);
                this.panel2.Name = "panel2";
                this.panel2.Size = new Size(3, 195);
                this.panel2.TabIndex = 1;

                this.panel3.BackColor = Color.FromArgb(55, 55, 55);
                this.panel3.Controls.Add(this.label1);
                this.panel3.Controls.Add(this.button1);
                this.panel3.Controls.Add(this.panel5);
                this.panel3.Dock = DockStyle.Top;
                this.panel3.ForeColor = Color.White;
                this.panel3.Location = new Point(3, 0);
                this.panel3.Name = "panel3";
                this.panel3.Size = new Size(291, 25);
                this.panel3.TabIndex = 2;
                this.panel3.MouseDown += new MouseEventHandler(this.xMouseDown);
                this.panel3.MouseMove += new MouseEventHandler(this.xMouseMove);

                this.panel4.BackColor = Color.FromArgb(55, 55, 55);
                this.panel4.Dock = DockStyle.Bottom;
                this.panel4.ForeColor = Color.White;
                this.panel4.Location = new Point(3, 170);
                this.panel4.Name = "panel4";
                this.panel4.Size = new Size(291, 3);
                this.panel4.TabIndex = 3;

                this.button1.BackColor = Color.FromArgb(55, 55, 55);
                this.button1.FlatAppearance.BorderColor = Color.FromArgb(50, 50, 50);
                this.button1.FlatAppearance.BorderSize = 0;
                this.button1.FlatStyle = FlatStyle.Flat;
                this.button1.Font = new Font("Segoe UI", 9f, FontStyle.Bold, GraphicsUnit.Point, 0);
                this.button1.ForeColor = Color.White;
                this.button1.Location = new Point(262, 0);
                this.button1.Name = "button1";
                this.button1.Size = new Size(30, 25);
                this.button1.TabIndex = 4;
                this.button1.Text = "X";
                this.button1.UseVisualStyleBackColor = false;
                this.button1.Click += new EventHandler(this.button1_Click);

                this.label1.AutoSize = true;
                this.label1.Font = new Font("Segoe UI", 9f, FontStyle.Bold, GraphicsUnit.Point, 0);
                this.label1.ForeColor = Color.White;
                this.label1.Location = new Point(32, 6);
                this.label1.Name = "label1";
                this.label1.Size = new Size(130, 15);
                this.label1.TabIndex = 0;
                this.label1.Text = "Connect To Ps3";
                this.label1.MouseDown += new MouseEventHandler(this.xMouseDown);
                this.label1.MouseMove += new MouseEventHandler(this.xMouseMove);

                this.panel5.BackColor = Color.FromArgb(55, 55, 55);
                this.panel5.BackgroundImage = Better_Craft.Properties.Resources.TNT.ToBitmap();
                this.panel5.BackgroundImageLayout = ImageLayout.Stretch;
                this.panel5.ForeColor = Color.FromArgb(110, 110, 110);
                this.panel5.Location = new Point(3, 1);
                this.panel5.Name = "panel5";
                this.panel5.Size = new Size(22, 22);
                this.panel5.TabIndex = 4;

                this.btnConnect.Location = new Point(23, 260);
                this.btnConnect.Name = "btnConnect";
                this.btnConnect.Size = new Size(250, 33);
                this.btnConnect.BackColor = Color.FromArgb(110, 110, 110);
                this.btnConnect.BackgroundImage = new Bitmap(Better_Craft.Properties.Resources.defaultButton);
                this.btnConnect.BackgroundImageLayout = ImageLayout.Stretch;
                this.btnConnect.Font = minecraftFont;
                this.btnConnect.ForeColor = Color.White;
                this.btnConnect.FlatStyle = FlatStyle.Flat;
                this.btnConnect.FlatAppearance.BorderSize = 0;
                this.btnConnect.TabIndex = 1;
                this.btnConnect.Text = this.strTraduction("btnConnect");
                this.btnConnect.UseVisualStyleBackColor = true;
                this.btnConnect.Enabled = true;
                this.btnConnect.Click += delegate (object sender, EventArgs e)
                {
                    clickSound.Play();
                    if (tNum > -1)
                    {
                        if (this.Api.ConnectTarget(this.data[tNum].Ip))
                        {
                            this.Api.setTargetName(this.data[tNum].Name);
                            Result = true;
                        }
                        else
                        {
                            Result = false;
                        }
                        this.formList.Close();
                    }
                    else
                    {
                        MessageBox.Show(this.strTraduction("errorSelect"), "Connect to Ps3", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                };
                //this.btnConnect.MouseUp += delegate (object sender, MouseEventArgs e)
                //{
                //    this.btnConnect.BackgroundImage = new Bitmap(Better_Craft.Properties.Resources.defaultButton);
                //};
                this.btnConnect.MouseHover += delegate (object sender, EventArgs e)
                {
                    btnConnect.BackgroundImage = new Bitmap(Better_Craft.Properties.Resources.highlightButton);
                };
                this.btnConnect.MouseLeave += delegate (object sender, EventArgs e)
                {
                    btnConnect.BackgroundImage = new Bitmap(Better_Craft.Properties.Resources.defaultButton);
                };

                this.btnRefresh.Location = new Point(23, 297);
                this.btnRefresh.Name = "btnRefresh";
                this.btnRefresh.BackColor = Color.FromArgb(110, 110, 110);
                this.btnRefresh.BackgroundImage = new Bitmap(Better_Craft.Properties.Resources.defaultButton);
                this.btnRefresh.BackgroundImageLayout = ImageLayout.Stretch;
                this.btnRefresh.Font = minecraftFont;
                this.btnRefresh.ForeColor = Color.White;
                this.btnRefresh.FlatStyle = FlatStyle.Flat;
                this.btnRefresh.FlatAppearance.BorderSize = 0;
                this.btnRefresh.Size = new Size(250, 33);
                this.btnRefresh.TabIndex = 1;
                this.btnRefresh.Text = this.strTraduction("btnRefresh");
                this.btnRefresh.UseVisualStyleBackColor = true;
                this.btnRefresh.Click += delegate (object sender, EventArgs e)
                {
                    clickSound.Play();
                    tNum = -1;
                    this.listView.Clear();
                    this.btnConnect.Enabled = false;
                    this.data = this.Api.CCAPI.GetConsoleList();
                    int num2 = this.data.Count<CCAPI.ConsoleInfo>();
                    for (int j = 0; j < num2; j++)
                    {
                        ListViewItem listViewItem2 = new ListViewItem(this.data[j].Name + " - " + this.data[j].Ip);
                        listViewItem2.ImageIndex = 0;
                        this.listView.Items.Add(listViewItem2);
                    }
                };
                //this.btnConnect.MouseUp += delegate (object sender, MouseEventArgs e)
                //{
                //    this.btnConnect.BackgroundImage = new Bitmap(Better_Craft.Properties.Resources.defaultButton);
                //};
                this.btnRefresh.MouseHover += delegate (object sender, EventArgs e)
                {
                    btnRefresh.BackgroundImage = new Bitmap(Better_Craft.Properties.Resources.highlightButton);
                };
                this.btnRefresh.MouseLeave += delegate (object sender, EventArgs e)
                {
                    btnRefresh.BackgroundImage = new Bitmap(Better_Craft.Properties.Resources.defaultButton);
                };

                this.listView.BackColor = Color.FromArgb(110, 110, 110);
                this.listView.BackgroundImage = new Bitmap(Better_Craft.Properties.Resources.betterCraftStoneBackground);
                this.listView.ForeColor = Color.White;
                this.listView.Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
                this.listViewGroup.Header = "Consoles";
                this.listViewGroup.Name = "consoleGroup";
                this.listView.Groups.AddRange(new ListViewGroup[]
                {
                            this.listViewGroup
                });
                this.listView.HideSelection = false;
                this.listView.Location = new Point(23, 41);
                this.listView.MultiSelect = false;
                this.listView.Name = "ConsoleList";
                this.listView.ShowGroups = false;
                this.listView.Size = new Size(250, 215);
                this.listView.TabIndex = 0;
                this.listView.UseCompatibleStateImageBehavior = false;
                this.listView.View = View.List;
                this.listView.ItemSelectionChanged += delegate (object sender, ListViewItemSelectionChangedEventArgs e)
                {
                    clickSound.Play();
                    tNum = e.ItemIndex;
                    this.btnConnect.Enabled = true;
                    if (this.data[tNum].Name.Length > 18)
                    {
                        string text = this.data[tNum].Name.Substring(0, 17) + "...";
                    }
                    else
                    {
                        string text = this.data[tNum].Name;
                    }
                    if (this.data[tNum].Ip.Length > 16)
                    {
                        string text2 = this.data[tNum].Name.Substring(0, 16) + "...";
                    }
                    else
                    {
                        string text2 = this.data[tNum].Ip;
                    }
                };

                this.formList.MinimizeBox = false;
                this.formList.MaximizeBox = false;
                this.formList.ClientSize = new Size(297, 350);
                this.formList.AutoScaleDimensions = new SizeF(6f, 13f);
                this.formList.AutoScaleMode = AutoScaleMode.Font;
                this.formList.BackColor = Color.FromArgb(25, 25, 25);
                this.formList.ForeColor = Color.FromArgb(55, 55, 55);
                this.formList.FormBorderStyle = FormBorderStyle.None;
                this.formList.StartPosition = FormStartPosition.CenterScreen;
                this.formList.Text = "Connect To Ps3";
                this.formList.Controls.Add(this.listView);
                this.formList.Controls.Add(this.btnConnect);
                this.formList.Controls.Add(this.btnRefresh);
                this.formList.Controls.Add(this.panel4);
                this.formList.Controls.Add(this.panel3);
                this.formList.Controls.Add(this.panel2);
                this.formList.Controls.Add(this.panel1);
                this.panel1.ResumeLayout(false);
                this.panel1.PerformLayout();
                this.panel2.ResumeLayout(false);
                this.panel2.PerformLayout();
                this.panel3.ResumeLayout(false);
                this.panel3.PerformLayout();
                this.panel4.ResumeLayout(false);
                this.panel4.PerformLayout();
                this.formList.ResumeLayout(false);
                this.formList.PerformLayout();
                int num = this.data.Count<CCAPI.ConsoleInfo>();
                for (int i = 0; i < num; i++)
                {
                    ListViewItem listViewItem = new ListViewItem(" " + this.data[i].Name + " - " + this.data[i].Ip);
                    listViewItem.ImageIndex = 0;
                    this.listView.Items.Add(listViewItem);
                }
                if (num > 0)
                {
                    this.formList.ShowDialog();
                }
                else
                {
                    Result = false;
                    this.formList.Close();
                    MessageBox.Show(this.strTraduction("noConsole"), this.strTraduction("noConsoleTitle"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                return Result;
            }
        }
    }
}
