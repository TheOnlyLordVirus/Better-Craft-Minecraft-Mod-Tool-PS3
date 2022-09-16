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
    using System.Linq.Expressions;
    using Expression = System.Linq.Expressions.Expression;
    using System.Globalization;

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
        private void filterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadCheats(filterTextBox.Text.Trim());
        }

        /// <summary>
        /// Return to connect and attach.
        /// </summary>
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
        private void OptionsReturnToConnectButton_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            ToggleOptionScreen(false);
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
        /// Toggles the Mods screen.
        /// </summary>
        private void ToggleModsScreen(bool visible)
        {
            if (visible)
            {
                connectAndAttachGrid.Visibility = Visibility.Hidden;
                modsGrid.Visibility = Visibility.Visible;
            }

            else
            {
                modsGrid.Visibility = Visibility.Hidden;
                connectAndAttachGrid.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Toggles the Options screen.
        /// </summary>
        private void ToggleOptionScreen(bool visible)
        {
            if (visible)
            {
                connectAndAttachGrid.Visibility = Visibility.Hidden;
                optionsGrid.Visibility = Visibility.Visible;
            }

            else
            {
                optionsGrid.Visibility = Visibility.Hidden;
                connectAndAttachGrid.Visibility = Visibility.Visible;
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
        private void LoadCheats(string filter = "")
        {
            cheatPanel.Children.Clear();

            // Add all of the cheats.
            PropertyInfo[] cheats = typeof(Minecraft_Cheats).GetProperties();
            foreach(PropertyInfo cheat in cheats)
            {
                string cheatName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(CultureInfo.CurrentCulture.TextInfo.ToLower(cheat.Name.Replace("_", " ")));
                dynamic cheatValue = cheat.GetValue(null);

                if (filter.Equals(string.Empty) || cheatName.ToLower().Contains(filter.ToLower()))
                {
                    SolidColorBrush toggleStateColor = new SolidColorBrush(Colors.LightCoral);

                    if (cheatValue is bool && cheatValue)
                        toggleStateColor = new SolidColorBrush(Colors.LightGreen);

                    else if (cheatValue is int && !cheatValue.Equals(0))
                        toggleStateColor = new SolidColorBrush(Colors.Cyan);

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

                    if(cheatValue is int)
                    {
                        button.Click += (sender, e) =>
                        {
                            clickSound.Play();
                            Expression<Func<int>> cheatAsALambdaProperty = Expression.Lambda<Func<int>>(Expression.MakeMemberAccess(null, cheat));
                            DoMod(button, cheatAsALambdaProperty);
                        };
                    }

                    else if(cheatValue is bool)
                    {
                        button.Click += (sender, e) =>
                        {
                            clickSound.Play();
                            Expression<Func<bool>> cheatAsALambdaProperty = Expression.Lambda<Func<bool>>(Expression.MakeMemberAccess(null, cheat));
                            DoMod(button, cheatAsALambdaProperty);
                        };
                    }

                    cheatPanel.Children.Add(button);
                }
            }

            // Add the reset button.
            Button resetButton = new Button()
            {
                Content = "Reset All Mods",
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

                Minecraft_Cheats.HelperFunctions.Reset_All_Mods();

                // Update toggle states in UI.
                UpdateToggleStates();
            };

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

            // Update toggle states in UI.
            UpdateToggleStates();
        }

        /// <summary>
        /// Updates all of the buttons toggle state colors.
        /// </summary>
        private async void UpdateToggleStates()
        {
            await Task.Delay(2500);

            LoadCheats();
        }
        #endregion
    }
}