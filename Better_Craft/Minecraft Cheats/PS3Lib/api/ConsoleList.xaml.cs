using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;

namespace PS3Lib
{
    /// <summary>
    /// Interaction logic for ConsoleList.xaml
    /// </summary>
    public partial class ConsoleList : Window
    {
        private PS3API Api;
        private List<CCAPI.ConsoleInfo> data;
        private bool? result = null;
        private int tNum = -1;
        private SoundPlayer clickSound = new SoundPlayer(Better_Craft.Properties.Resources.minecraftClick);

        /// <summary>
        /// We wait for the result to return true or false, while its null we wait in PS3API / CCAPI.
        /// </summary>
        public bool? Result
        {
            get { return result; }
        }

        /// <summary>
        /// Create a new Console list window using the PS3API.
        /// </summary>
        public ConsoleList(PS3API api)
        {
            InitializeComponent();

            refreshButton.Content = strTraduction("btnRefresh");
            connectButton.Content = strTraduction("btnConnect");
            windowTitle.Content = strTraduction("formTitle");

            Api = api;
            data = Api.CCAPI.GetConsoleList();

            int sizeData = data.Count();

            for (int i = 0; i < sizeData; i++)
            {
                listView.Items.Add(data[i].Name + " - " + data[i].Ip);
            }

            // If there are more than 0 targets we show the form
            // Else we inform the user to create a console.
            if (sizeData > 0)
                this.ShowDialog();
            else
            {
                result = false;
                this.Close();
                MessageBox.Show(strTraduction("noConsole"), strTraduction("noConsoleTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Move window.
        /// </summary>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        /// <summary>
        /// Close window.
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            result = false;
            this.Close();
        }

        /// <summary>
        /// Change selected console.
        /// </summary>
        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tNum = listView.SelectedIndex;
            if(!tNum.Equals(-1))
            {
                connectButton.IsEnabled = true;
                string Name, Ip = "?";

                if (data[tNum].Name.Length > 18)
                    Name = data[tNum].Name.Substring(0, 17) + "...";
                else Name = data[tNum].Name;

                if (data[tNum].Ip.Length > 16)
                    Ip = data[tNum].Name.Substring(0, 16) + "...";
                else Ip = data[tNum].Ip;
            }
        }

        /// <summary>
        /// Attempt to connect
        /// </summary>
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            if (tNum > -1)
            {
                if (Api.ConnectTarget(data[tNum].Ip))
                {
                    Api.setTargetName(data[tNum].Name);
                    result = true;
                }
                else result = false;


                this.Close();
            }
            else
                MessageBox.Show(strTraduction("errorSelect"), strTraduction("errorSelectTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Refresh console list.
        /// </summary>
        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            tNum = -1;
            listView.Items.Clear();
            connectButton.IsEnabled = false;
            data = Api.CCAPI.GetConsoleList();
            int sizeD = data.Count();
            for (int i = 0; i < sizeD; i++)
            {
                listView.Items.Add(data[i].Name + " - " + data[i].Ip);
            }

            connectButton.IsEnabled = true;
        }

        #region CCAPI Language

        /// <summary>
        /// This stuff was made by FMT / imscx for multi-language support. Feel free to use or remove this as you wish.
        /// </summary>
        public enum Lang
        {
            Null,
            French,
            English,
            German
        }

        private class SetLang
        {
            public static Lang defaultLang = Lang.Null;
        }

        public void SetFormLang(Lang Language)
        {
            SetLang.defaultLang = Language;
        }

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

        public string strTraduction(string keyword)
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

        #endregion
    }
}
