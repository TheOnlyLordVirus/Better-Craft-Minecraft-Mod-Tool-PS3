/// <summary>
/// The Better_Craft Namespace.
/// </summary>
namespace Better_Craft
{
    using System;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using System.Media;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Reflection;
    using System.Linq.Expressions;
    using System.Globalization;
    using System.Collections.Generic;
    using Expression = System.Linq.Expressions.Expression;

    using PS3Lib;
    using Minecraft_Cheats;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region variables

        /// <summary>
        /// Our current API.
        /// </summary>
        private SelectAPI currentAPI = SelectAPI.ControlConsole;

        /// <summary>
        /// The sound that plays when you click a button.
        /// </summary>
        private SoundPlayer clickSound = new SoundPlayer(Properties.Resources.minecraftClick);

        /// <summary>
        /// Should we show the user a warning about not using CCAPI?
        /// </summary>
        private bool apiMessage = true;

        /// <summary>
        /// Stores the buttons in for the cheat panel.
        /// </summary>
        private Dictionary<string, Button> modbuttons = new Dictionary<string, Button>();

        /// <summary>
        /// Store all of the grids in this array.
        /// </summary>
        private Grid[] grids;

        #endregion

        /// <summary>
        /// The "MainWindow" class constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            
            // This is just here so the XAML view isn't so annoying to look at while coding...
            scrollImageOne.Visibility = Visibility.Visible;
            scrollImageTwo.Visibility = Visibility.Visible;

            grids = new Grid[] { connectAndAttachGrid, modsGrid, optionsGrid };
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
        /// Close the tool
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region Connect and Attach Grid

        /// <summary>
        /// Connect and attach to your PlayStation 3.
        /// </summary>
        private void ConnectAndAttachButton_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            Minecraft_Cheats.HelperFunctions.Connect(currentAPI);
        }

        /// <summary>
        /// Disconnect from your PlayStation 3.
        /// </summary>
        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            Minecraft_Cheats.HelperFunctions.Disconnect();
        }


        /// <summary>
        /// Open the main mods grid.
        /// </summary>
        private void ModsButton_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            if (Minecraft_Cheats.HelperFunctions.isConnected)
            {
                LoadCheats();
                ToggleGrids(modsGrid);
            }

            else
            {
                MessageBox.Show("You must be connected to your PlayStation 3 before you can modify its memory.", "Notice", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Open the select API grid.
        /// </summary>
        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            ToggleGrids(optionsGrid);
        }

        #endregion

        #region Main Mods Grid

        /// <summary>
        /// Filter the mods in the wrap pannel.
        /// </summary>
        private void filterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterMods(filterTextBox.Text.Trim());
        }

        /// <summary>
        /// Return to connect and attach.
        /// </summary>
        private void ModsReturnToConnectButton_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            ToggleGrids(connectAndAttachGrid);
        }

        #endregion

        #region Options Grid

        /// <summary>
        /// Return to connect and attach.
        /// </summary>
        private void OptionsReturnToConnectButton_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            ToggleGrids(connectAndAttachGrid);
        }

        /// <summary>
        /// Toggles CCAPI.
        /// </summary>
        private void SelectCCAPIButton_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            ToggleAPI(SelectAPI.ControlConsole);
        }

        /// <summary>
        /// Toggles TMAPI.
        /// </summary>
        private void SelectTMAPIButton_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            ToggleAPI(SelectAPI.TargetManager);
        }

        /// <summary>
        /// Toggles PS3MAPI
        /// </summary>
        private void selectPS3MAPIButton_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            ToggleAPI(SelectAPI.PS3Manager);
        }

        #endregion

        #region Core Methods

        /// <summary>
        /// Opens a grid.
        /// </summary>
        private void ToggleGrids(Grid openThisGrid)
        {
            openThisGrid.Visibility = Visibility.Visible;

            foreach(Grid grid in grids)
            {
                if(!grid.Equals(openThisGrid))
                    grid.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Toggles our currently selected API.
        /// </summary>
        private void ToggleAPI(SelectAPI myAPI)
        {
            if(apiMessage)
            {
                MessageBoxResult YesNo = MessageBox.Show("This tool is best experienced using CCAPI with a CEX eboot.bin!\n\nUsing a debug eboot.bin will change the location of some memory addresses, therefore some mods may not work while using TMAPI.\n(Most work properly though)\n\nWhile using PS3MAPI adds HEN support, it is painfully slow when writing to memory.\n\nDo you wish to continue?", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (YesNo.Equals(MessageBoxResult.Yes))
                {
                    apiMessage = false;
                }

                else
                    return;
            }

            currentAPI = myAPI;

            if (myAPI.Equals(SelectAPI.ControlConsole))
            {
                WpfAnimatedGif.ImageBehavior.SetAnimatedSource(ccapiTorch, new BitmapImage(new Uri(@"pack://application:,,,/Better_Craft;component/Images/redstoneTorchOn.gif")));
                WpfAnimatedGif.ImageBehavior.SetAnimatedSource(tmapiTorch, new BitmapImage(new Uri(@"pack://application:,,,/Better_Craft;component/Images/redStoneTorchOff.png")));
                WpfAnimatedGif.ImageBehavior.SetAnimatedSource(ps3mapiTorch, new BitmapImage(new Uri(@"pack://application:,,,/Better_Craft;component/Images/redStoneTorchOff.png")));
            }

            else if (myAPI.Equals(SelectAPI.PS3Manager))
            {
                WpfAnimatedGif.ImageBehavior.SetAnimatedSource(ccapiTorch, new BitmapImage(new Uri(@"pack://application:,,,/Better_Craft;component/Images/redStoneTorchOff.png")));
                WpfAnimatedGif.ImageBehavior.SetAnimatedSource(tmapiTorch, new BitmapImage(new Uri(@"pack://application:,,,/Better_Craft;component/Images/redStoneTorchOff.png")));
                WpfAnimatedGif.ImageBehavior.SetAnimatedSource(ps3mapiTorch, new BitmapImage(new Uri(@"pack://application:,,,/Better_Craft;component/Images/redstoneTorchOn.gif")));
            }

            else if(myAPI.Equals(SelectAPI.TargetManager))
            {
                WpfAnimatedGif.ImageBehavior.SetAnimatedSource(ccapiTorch, new BitmapImage(new Uri(@"pack://application:,,,/Better_Craft;component/Images/redStoneTorchOff.png")));
                WpfAnimatedGif.ImageBehavior.SetAnimatedSource(tmapiTorch, new BitmapImage(new Uri(@"pack://application:,,,/Better_Craft;component/Images/redstoneTorchOn.gif")));
                WpfAnimatedGif.ImageBehavior.SetAnimatedSource(ps3mapiTorch, new BitmapImage(new Uri(@"pack://application:,,,/Better_Craft;component/Images/redStoneTorchOff.png")));
            }
        }

        /// <summary>
        /// Loads the cheats to the tile controll
        /// </summary>
        private bool stopLoading;
        private async void LoadCheats()
        {
            // Clear old stuff.
            filterTextBox.Text = string.Empty;
            filterTextBox.IsEnabled = false;
            stopLoading = false;

            modbuttons.Clear();
            cheatPanel.Children.Clear();

            // Read toggle states from memory?
            MessageBoxResult YesNo = MessageBox.Show("Would you like to read cheats toggle states from memory?\n\n(This may take a while on PS3MAPI)", "Noice", MessageBoxButton.YesNo, MessageBoxImage.Question);
            bool readFromMem = true;
            if (YesNo.Equals(MessageBoxResult.No))
                readFromMem = false;

            // Add all of the cheats.
            PropertyInfo[] cheats = typeof(Minecraft_Cheats).GetProperties();
            foreach(PropertyInfo cheat in cheats)
            {
                if (stopLoading)
                    return;

                string cheatName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cheat.Name.Replace("_", " ").ToLower());
                SolidColorBrush toggleStateColor = new SolidColorBrush(Colors.LightCoral);

                if(readFromMem)
                {
                    dynamic cheatValue = cheat.GetValue(null);
                    if (cheatValue is bool && cheatValue)
                        toggleStateColor = new SolidColorBrush(Colors.LightGreen);

                    else if (cheatValue is int && !cheatValue.Equals(0))
                        toggleStateColor = new SolidColorBrush(Colors.Cyan);
                }

                Button button = new Button()
                {
                    Content = cheatName,
                    Width = 180,
                    Height = 40,
                    FontSize = 9,
                    Margin = new Thickness(10),
                    Padding = new Thickness(300),
                    Foreground = toggleStateColor
                };


                if(cheat.PropertyType.Equals(typeof(int)))
                {
                    button.Click += (sender, e) =>
                    {
                        clickSound.Play();
                        Expression<Func<int>> cheatAsALambdaProperty = Expression.Lambda<Func<int>>(Expression.MakeMemberAccess(null, cheat));
                        DoMod(button, cheatAsALambdaProperty);
                    };
                }

                else if(cheat.PropertyType.Equals(typeof(bool)))
                {
                    button.Click += (sender, e) =>
                    {
                        clickSound.Play();
                        Expression<Func<bool>> cheatAsALambdaProperty = Expression.Lambda<Func<bool>>(Expression.MakeMemberAccess(null, cheat));
                        DoMod(button, cheatAsALambdaProperty);
                    };
                }

                if (modbuttons.ContainsKey(cheatName))
                    modbuttons.Remove(cheatName);

                modbuttons.Add(cheatName, button);
                cheatPanel.Children.Add(button);

                if(readFromMem)
                    await Task.Delay(currentAPI.Equals(SelectAPI.PS3Manager) ? 500 : 1);
            }

            AddResetButton();

            filterTextBox.IsEnabled = true;
        }

        /// <summary>
        /// Filters the mods in the cheat panel without needing to reading 100's of bytes.
        /// </summary>
        private bool updatingToggles = false;
        private async void UpdateToggleStates()
        {
            if(!updatingToggles)
            {
                updatingToggles = true;
                PropertyInfo[] cheats = typeof(Minecraft_Cheats).GetProperties();
                foreach (PropertyInfo cheat in cheats)
                {
                    string cheatName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cheat.Name.Replace("_", " ").ToLower());

                    // This part takes a bit for the api to respond bytes from the ps3 (Depending on the api being used.)
                    dynamic cheatValue = null;
                    await Task.Run(() =>
                    {
                        cheatValue = cheat.GetValue(null);
                    });

                    SolidColorBrush toggleStateColor = new SolidColorBrush(Colors.LightCoral);

                    if (cheatValue is bool && cheatValue)
                        toggleStateColor = new SolidColorBrush(Colors.LightGreen);

                    else if (cheatValue is int && !cheatValue.Equals(0))
                        toggleStateColor = new SolidColorBrush(Colors.Cyan);

                    Button mod = modbuttons[cheatName];
                    mod.Foreground = toggleStateColor;
                }

                cheatPanel.Children.Clear();
                string filter = filterTextBox.Text;
                foreach (KeyValuePair<string, Button> modOption in modbuttons)
                {
                    if (filter.Equals(string.Empty) || modOption.Key.ToLower().Contains(filter.ToLower()))
                        cheatPanel.Children.Add(modOption.Value);
                }
                updatingToggles = false;
            }
        }

        /// <summary>
        /// Filters the mods in the cheatGrid
        /// </summary>
        private void FilterMods(string filter)
        {
            cheatPanel.Children.Clear();
            foreach (KeyValuePair<string, Button> modOption in modbuttons)
            {
                if (modOption.Key.ToLower().Contains(filter.ToLower()))
                {
                    cheatPanel.Children.Add(modOption.Value);
                }
            }
        }

        /// <summary>
        /// Adds the reset button to the cheat panel.
        /// </summary>
        private void AddResetButton()
        {
            string resetButtonName = "Reset All Mods";

            // Add the reset button.
            Button resetButton = new Button()
            {
                Content = resetButtonName,
                Width = 180,
                Height = 40,
                FontSize = 9,
                Margin = new Thickness(10),
                Padding = new Thickness(300),
                Foreground = new SolidColorBrush(Colors.LightCoral)
            };

            resetButton.Click += (sender, e) =>
            {
                clickSound.Play();

                // Update toggle states in UI.
                if(currentAPI.Equals(SelectAPI.PS3Manager))
                {
                    MessageBoxResult YesNo = MessageBox.Show("This will take quite a while!\nDo you wish to continue?", "PS3MAPI is slow...", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if(YesNo.Equals(MessageBoxResult.Yes))
                    {
                        Minecraft_Cheats.HelperFunctions.Reset_All_Mods();
                        UpdateToggleStates();
                    }
                }

                else
                {
                    Minecraft_Cheats.HelperFunctions.Reset_All_Mods();
                    UpdateToggleStates();
                }
            };

            if (modbuttons.ContainsKey(resetButtonName))
                modbuttons.Remove(resetButtonName);

            modbuttons.Add(resetButtonName, resetButton);
            cheatPanel.Children.Add(resetButton);
        }

        /// <summary>
        /// Toggle Logic for mods.
        /// </summary>
        private void DoMod<T>(object button, Expression<Func<T>> ModOption)
        {
            clickSound.Play();
            dynamic toggleStateValue = Minecraft_Cheats.HelperFunctions.ToggleOption(ModOption);

            SolidColorBrush toggleStateColor = new SolidColorBrush(Colors.LightCoral);

            if (toggleStateValue is bool)
            {
                if(toggleStateValue)
                {
                    toggleState.Content = "On";
                    toggleStateColor = new SolidColorBrush(Colors.LightGreen);
                }

                else
                    toggleState.Content = "Off";
            }

            else if (toggleStateValue is int)
            {
                if(!toggleStateValue.Equals(0))
                {
                    toggleStateColor = new SolidColorBrush(Colors.Cyan);
                }

                toggleState.Content = toggleStateValue;
            }
            
            toggleState.Foreground = toggleStateColor;
            ((Button)button).Foreground = toggleStateColor;
            ((Button)button).IsEnabled = false;

            // Clear toggle state notice.
            Task.Delay(500).GetAwaiter().OnCompleted(() => 
            { 
                ((Button)button).IsEnabled = true; 
                toggleState.Content = ""; 
            });

            UpdateToggleStates();
        }
        #endregion
    }
}