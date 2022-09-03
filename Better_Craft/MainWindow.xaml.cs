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
    using PS3Lib;
    using Minecraft_Cheats;
    using System.Windows.Input;
    using System.Reflection;
    using System.Globalization;
    using System.Threading;
    using System.Windows.Threading;
    using System.Windows.Media.Animation;

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
        /// Is the connect and attach grid open?
        /// </summary>
        private bool connectAndAttachOpen = true;

        #endregion

        /// <summary>
        /// The "MainWindow" class constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Move window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        /// <summary>
        /// Close the tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region Connect and Attach Grid

        /// <summary>
        /// Connect and attach to your PlayStation 3.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectAndAttachButton_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            Minecraft_Cheats.HelperFunctions.Connect(currentAPI);
        }

        /// <summary>
        /// Disconnect from your PlayStation 3.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            Minecraft_Cheats.HelperFunctions.Disconnect();
        }


        /// <summary>
        /// Open the main mods grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModsButton_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            if (Minecraft_Cheats.isConnected)
            {
                LoadCheats();
                ToggleModsScreen(true);
            }

            else
            {
                MessageBox.Show("You must be connected to your PlayStation 3 before you can modify its memory.", "Notice", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Open the select API grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            ToggleOptionScreen(true);
        }

        #endregion

        #region Main Mods Grid

        /// <summary>
        /// Filter the mods in the wrap pannel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void filterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadCheats(filterTextBox.Text.Trim());
        }

        /// <summary>
        /// Return to connect and attach.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModsReturnToConnectButton_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            ToggleModsScreen(false);
        }

        #endregion

        #region Options Grid

        /// <summary>
        /// Return to connect and attach.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OptionsReturnToConnectButton_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            ToggleOptionScreen(false);
        }

        /// <summary>
        /// Toggles CCAPI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectCCAPIButton_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            ToggleAPI(SelectAPI.ControlConsole);
        }

        /// <summary>
        /// Toggles TMAPI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectTMAPIButton_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            ToggleAPI(SelectAPI.TargetManager);
        }

        #endregion

        #region Core Methods

        /// <summary>
        /// Toggles the Mods screen.
        /// </summary>
        /// <param name="visible"></param>
        private void ToggleModsScreen(bool visible)
        {
            if (visible)
            {
                connectAndAttachOpen = false;
                connectAndAttachGrid.Visibility = Visibility.Hidden;
                modsGrid.Visibility = Visibility.Visible;
            }

            else
            {
                connectAndAttachOpen = true;
                modsGrid.Visibility = Visibility.Hidden;
                connectAndAttachGrid.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Toggles the Options screen.
        /// </summary>
        /// <param name="visible"></param>
        private void ToggleOptionScreen(bool visible)
        {
            if(visible)
            {
                connectAndAttachOpen = false;
                connectAndAttachGrid.Visibility = Visibility.Hidden;
                optionsGrid.Visibility = Visibility.Visible;
            }

            else
            {
                connectAndAttachOpen = true;
                optionsGrid.Visibility = Visibility.Hidden;
                connectAndAttachGrid.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Toggles our currently selected API.
        /// </summary>
        /// <param name="myAPI"></param>
        private void ToggleAPI (SelectAPI myAPI)
        {
            currentAPI = myAPI;
            
            if (myAPI.Equals(SelectAPI.ControlConsole))
            {
                WpfAnimatedGif.ImageBehavior.SetAnimatedSource(ccapiTorch, new BitmapImage(new Uri(@"pack://application:,,,/Better_Craft;component/Images/redstoneTorchOn.gif")));
                WpfAnimatedGif.ImageBehavior.SetAnimatedSource(tmapiTorch, new BitmapImage(new Uri(@"pack://application:,,,/Better_Craft;component/Images/redStoneTorchOff.png")));
            }

            else
            {
                WpfAnimatedGif.ImageBehavior.SetAnimatedSource(ccapiTorch, new BitmapImage(new Uri(@"pack://application:,,,/Better_Craft;component/Images/redStoneTorchOff.png")));
                WpfAnimatedGif.ImageBehavior.SetAnimatedSource(tmapiTorch, new BitmapImage(new Uri(@"pack://application:,,,/Better_Craft;component/Images/redstoneTorchOn.gif")));
            }
        }

        /// <summary>
        /// Loads the cheats to the tile controll
        /// </summary>
        private void LoadCheats(string filter = "")
        {
            cheatPanel.Children.Clear();
            string[] badFuncNames = { "tostring", "gettype", "gethashcode", "equals" };
            MethodInfo[] cheats = (typeof(Minecraft_Cheats)).GetMethods();

            foreach (MethodInfo cheat in cheats)
            {
                bool flag = true;
                foreach (string badFuncName in badFuncNames)
                {
                    if (cheat.Name.ToLower().Equals(badFuncName))
                        flag = false;
                }

                string cheatName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(CultureInfo.CurrentCulture.TextInfo.ToLower(cheat.Name.Replace("_", " ")));

                if (flag)
                {
                    if (filter.Equals(string.Empty) || cheatName.ToLower().Contains(filter.ToLower()))
                    {
                        SolidColorBrush toggleStateColor = new SolidColorBrush(Colors.LightCoral);

                        if (Minecraft_Cheats.ToggleStates.ContainsKey(cheat.Name))
                        {
                            if(Minecraft_Cheats.ToggleStates[cheat.Name] is bool && Minecraft_Cheats.ToggleStates[cheat.Name])
                            {
                                toggleStateColor = new SolidColorBrush(Colors.LightGreen);
                            }

                            else if (Minecraft_Cheats.ToggleStates[cheat.Name] is int && !Minecraft_Cheats.ToggleStates[cheat.Name].Equals(0))
                            {
                                toggleStateColor = new SolidColorBrush(Colors.Cyan);
                            }
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

                        // Check toggle type for delgate cast.
                        if (cheat.GetParameters().Length.Equals(1))
                        {
                            Type ParameterType = cheat.GetParameters()[0].ParameterType;

                            if (ParameterType.ToString().Equals("System.Boolean"))
                            {
                                button.Click += (sender, e) =>
                                {
                                    DoMod(button, (Action<bool>)Delegate.CreateDelegate(typeof(Action<bool>), cheat));
                                };

                                cheatPanel.Children.Add(button);
                            }

                            else if (ParameterType.ToString().Equals("System.Int32"))
                            {
                                button.Click += (sender, e) =>
                                {
                                    DoMod(button, (Action<int>)Delegate.CreateDelegate(typeof(Action<int>), cheat));
                                };

                                cheatPanel.Children.Add(button);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Toggle Logic for mods with boolean values.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="FunctionToggle"></param>
        private void DoMod(object button, Action<bool> FunctionToggle)
        {
            clickSound.Play();

            bool flag = Minecraft_Cheats.HelperFunctions.ToggleOption(FunctionToggle);
            SolidColorBrush brush = flag ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.LightCoral);
            toggleState.Content = flag ? "On" : "Off";
            toggleState.Foreground = brush;
            ((Button)button).Foreground = brush;

            Task.Delay(1300).GetAwaiter().OnCompleted(() => { toggleState.Content = ""; });
        }

        /// <summary>
        /// Toggle Logic for mods with multible toggle states.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="FunctionToggle"></param>
        private void DoMod(object button, Action<int> FunctionToggle)
        {
            clickSound.Play();

            int currentToggleState = Minecraft_Cheats.HelperFunctions.ToggleOption(FunctionToggle);
            SolidColorBrush brush = !currentToggleState.Equals(0) ? new SolidColorBrush(Colors.Cyan) : new SolidColorBrush(Colors.LightCoral);
            toggleState.Content = currentToggleState;
            toggleState.Foreground = brush;
            ((Button)button).Foreground = brush;

            Task.Delay(1300).GetAwaiter().OnCompleted(() => { toggleState.Content = ""; });
        }

        #endregion
    }
}
