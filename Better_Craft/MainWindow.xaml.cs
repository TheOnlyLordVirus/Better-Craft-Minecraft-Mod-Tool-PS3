/// <summary>
/// The Better_Craft Namespace.
/// </summary>
namespace Better_Craft
{
    using System;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using System.Media;
    using System.IO;
    using PS3Lib;
    using System.Linq;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region variables

        /// <summary>
        /// The main PS3 API Object.
        /// </summary>
        private PS3API mainPS3 = new PS3API();

        /// <summary>
        /// Our current API.
        /// </summary>
        private SelectAPI currentAPI = SelectAPI.ControlConsole;

        /// <summary>
        /// Are we connected?
        /// </summary>
        private bool isConnected = false;

        /// <summary>
        /// The sound that plays when you click a button.
        /// </summary>
        private SoundPlayer clickSound;

        #region Offsets

        /// <summary>
        /// Creative mode invetory and instant mine.
        /// </summary>
        private static uint creativeModeGUI = 0x002F0350;

        /// <summary>
        /// Ininite blocks.
        /// </summary>
        private static uint infCraft = 0x0010673C;

        /// <summary>
        /// Health Regen Speed.
        /// </summary>
        private static uint godMode1 = 0x003A3F48;

        /// <summary>
        /// Extended Health.
        /// </summary>
        private static uint godMode2 = 0x004B2028;

        /// <summary>
        /// Fast mine.
        /// </summary>
        private static uint fastBreak = 0x00BBBD54;

        /// <summary>
        /// Instant mine.
        /// </summary>
        private static uint instantBreak = 0x00AED8B0;

        /// <summary>
        /// Movement speed.
        /// </summary>
        private static uint moveSpeed = 0x003AA9A0;

        /// <summary>
        /// Multi-Jump.
        /// </summary>
        private static uint multiJumpMod = 0x00227910;

        /// <summary>
        /// No fall damage.
        /// </summary>
        private static uint noFallDmg = 0x0039DC98;

        /// <summary>
        /// Infinte Breath underwater.
        /// </summary>
        private static uint infBreath = 0x0039DCF0;

        /// <summary>
        /// The current in game time.
        /// </summary>
        private static uint timeOfDay = 0x001DA1D8;

        /// <summary>
        /// FOV.
        /// </summary>
        private static uint FOV = 0x00C6FF88;

        /// <summary>
        /// Knock Back.
        /// </summary>
        //private static uint knockBack = 0x003A4020;

        /// <summary>
        /// Cave rendering.
        /// </summary>
        //private static uint groundHack = 0x00A9B768;

        #endregion

        #endregion

        /// <summary>
        /// The "MainWindow" class constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Stream clickSoundStream = Properties.Resources.minecraftClick;
            clickSound = new SoundPlayer(clickSoundStream);
            mainPS3.ChangeAPI(currentAPI);
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

            if(mainPS3.ConnectTarget())
            {
                if(mainPS3.AttachProcess())
                {
                    isConnected = true;
                }

                // Failed to attach.
                else
                {
                    if (currentAPI == SelectAPI.ControlConsole)
                    {
                        MessageBox.Show("Connected, but failed to attach with: \"Control Console API\"", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    else if (currentAPI == SelectAPI.TargetManager)
                    {
                        MessageBox.Show("Connected, but failed to attach with: \"Target Manager API\"", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            // Failed to connect.
            else
            {
                if(currentAPI == SelectAPI.ControlConsole)
                {
                    MessageBox.Show("Failed to connect & attach with: \"Control Console API\"", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                else if(currentAPI == SelectAPI.TargetManager)
                {
                    MessageBox.Show("Failed to connect & attach with: \"Target Manager API\"", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Disconnect from your PlayStation 3.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            if (isConnected)
            {
                mainPS3.DisconnectTarget();
                isConnected = false;
            }

            else
            {
                MessageBox.Show("You must first be connected to your PlayStation 3 to disconnect from it.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// Open the main mods grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModsButton_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            if (isConnected)
            {
                ToggleModsScreen(true);
            }

            else
            {
                MessageBox.Show("You must be connected to your PlayStation 3 before you can modify its memory.", "Notice", MessageBoxButton.OK, MessageBoxImage.Error);
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
        /// Creative mode GUI toggle.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreativeMode_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            if (mainPS3.GetBytes(creativeModeGUI, 4).SequenceEqual(new byte[] { 0x38, 0x80, 0x00, 0x00 }))
            {
                mainPS3.SetMemory(creativeModeGUI, new byte[] { 0x38, 0x80, 0x00, 0x01 });
            }

            else
            {
                mainPS3.SetMemory(creativeModeGUI, new byte[] { 0x38, 0x80, 0x00, 0x00 });
            }
        }

        /// <summary>
        /// Infinte Blocks.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InfBlock_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            if(mainPS3.GetBytes(infCraft, 4).SequenceEqual(new byte[] { 0x38, 0x80, 0x00, 0x00 }))
            {
                mainPS3.SetMemory(infCraft, new byte[] { 0x38, 0x80, 0x00, 0x01 });
            }

            else
            {
                mainPS3.SetMemory(infCraft, new byte[] { 0x38, 0x80, 0x00, 0x00 });
            }
        }

        /// <summary>
        /// God mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GodMode_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            if(mainPS3.GetBytes(godMode1, 2).SequenceEqual(new byte[] { 0xFC, 0x20 }) && mainPS3.GetBytes(godMode2, 2).SequenceEqual(new byte[] { 0xFC, 0x20 }))
            {
                mainPS3.SetMemory(godMode1, new byte[] { 0xFF, 0x20 });
                mainPS3.SetMemory(godMode2, new byte[] { 0xFF, 0x20 });
            }

            else
            {
                mainPS3.SetMemory(godMode1, new byte[] { 0xFC, 0x20 });
                mainPS3.SetMemory(godMode2, new byte[] { 0xFC, 0x20 });
            }
        }

        /// <summary>
        /// Instant mine.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InstantMine_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            if (mainPS3.GetBytes(fastBreak, 2).SequenceEqual(new byte[] { 0x3F, 0x80 }) && mainPS3.GetBytes(instantBreak, 2).SequenceEqual(new byte[] { 0x3F, 0x80 }))
            {
                mainPS3.SetMemory(fastBreak, new byte[] { 0x00, 0x00 });
                mainPS3.SetMemory(instantBreak, new byte[] { 0x00, 0x00 });
            }

            else
            {
                mainPS3.SetMemory(fastBreak, new byte[] { 0x3F, 0x80 });
                mainPS3.SetMemory(instantBreak, new byte[] { 0x3F, 0x80 });
            }
        }

        /// <summary>
        /// Movement Speed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpeedMovement_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            if(mainPS3.GetBytes(moveSpeed, 2).SequenceEqual(new byte[] { 0x3F, 0x68 }))
            {
                mainPS3.SetMemory(moveSpeed, new byte[] { 0x3F, 0x30 });
            }

            else if (mainPS3.GetBytes(moveSpeed, 2).SequenceEqual(new byte[] { 0x3F, 0x30 }))
            {
                mainPS3.SetMemory(moveSpeed, new byte[] { 0x3F, 0x00 });
            }

            else
            {
                mainPS3.SetMemory(moveSpeed, new byte[] { 0x3F, 0x68 });
            }
        }

        /// <summary>
        /// Multi-Jump.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MultiJump_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            if (mainPS3.GetBytes(multiJumpMod, 4).SequenceEqual(new byte[] { 0x41, 0x82, 0x00, 0x18 }))
            {
                mainPS3.SetMemory(multiJumpMod, new byte[] { 0x41, 0x82, 0x00, 0x28 });
            }

            else
            {
                mainPS3.SetMemory(multiJumpMod, new byte[] { 0x41, 0x82, 0x00, 0x18 });
            }
        }

        /// <summary>
        /// No Fall Damage.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NoFallDamage_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            if (mainPS3.GetBytes(noFallDmg, 1).SequenceEqual(new byte[] { 0xFC }))
            {
                mainPS3.SetMemory(noFallDmg, new byte[] { 0xFF });
            }

            else
            {
                mainPS3.SetMemory(noFallDmg, new byte[] { 0xFC });
            }
        }

        /// <summary>
        /// Infinte breath under water.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InfinteBreath_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            if (mainPS3.GetBytes(infBreath, 4).SequenceEqual(new byte[] { 0x38, 0x60, 0x00, 0x00 }))
            {
                mainPS3.SetMemory(infBreath, new byte[] { 0x38, 0x60, 0x00, 0x01 });
            }

            else
            {
                mainPS3.SetMemory(infBreath, new byte[] { 0x38, 0x60, 0x00, 0x00 });
            }
        }

        /// <summary>
        /// Progress time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveTime_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            if (mainPS3.GetBytes(timeOfDay, 5).SequenceEqual(new byte[] { 0x3F, 0x00, 0x00, 0x00, 0x40 }))
            {
                mainPS3.SetMemory(timeOfDay, new byte[] { 0x3F, 0x00, 0x00, 0x00, 0xC4 });
            }

            else
            {
                mainPS3.SetMemory(timeOfDay, new byte[] { 0x3F, 0x00, 0x00, 0x00, 0x40,  });
            }
        }

        /// <summary>
        /// Toggle cave rendering on chunk update.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CaveXRay_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
        }

        /// <summary>
        /// Edit FOV.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FieldOfView_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            if (mainPS3.GetBytes(FOV, 2).SequenceEqual(new byte[] { 0x3F, 0xC9 }))
            {
                mainPS3.SetMemory(FOV, new byte[] { 0x3F, 0xB4 });
            }

            else
            {
                mainPS3.SetMemory(FOV, new byte[] { 0x3F, 0xC9 });
            }
        }

        /// <summary>
        /// Debug...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DebugMod_Click(object sender, RoutedEventArgs e)
        {
            clickSound.Play();
            MessageBox.Show("No HaX Here...", "Debug", MessageBoxButton.OK, MessageBoxImage.Error);
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
        /// <param name="visible"></param>
        private void ToggleOptionScreen(bool visible)
        {
            if(visible)
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
        /// <param name="myAPI"></param>
        private void ToggleAPI (SelectAPI myAPI)
        {
            if(myAPI == SelectAPI.ControlConsole)
            {
                currentAPI = SelectAPI.ControlConsole;
                mainPS3.ChangeAPI(currentAPI);
                WpfAnimatedGif.ImageBehavior.SetAnimatedSource(ccapiTorch, new BitmapImage(new Uri(@"pack://application:,,,/Better_Craft;component/Images/redstoneTorchOn.gif")));
                WpfAnimatedGif.ImageBehavior.SetAnimatedSource(tmapiTorch, new BitmapImage(new Uri(@"pack://application:,,,/Better_Craft;component/Images/redStoneTorchOff.png")));
            }

            else
            {
                currentAPI = SelectAPI.TargetManager;
                mainPS3.ChangeAPI(currentAPI);
                WpfAnimatedGif.ImageBehavior.SetAnimatedSource(ccapiTorch, new BitmapImage(new Uri(@"pack://application:,,,/Better_Craft;component/Images/redStoneTorchOff.png")));
                WpfAnimatedGif.ImageBehavior.SetAnimatedSource(tmapiTorch, new BitmapImage(new Uri(@"pack://application:,,,/Better_Craft;component/Images/redstoneTorchOn.gif")));
            }
        }

        #endregion
    }
}
