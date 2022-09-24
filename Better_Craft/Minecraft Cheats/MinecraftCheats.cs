/*
 * Minecraft Cheats for BetterCraft by: LordVirus 8/26/2022
 * 
 * Thanks to all of the scene members that did the extensive Minecraft ps3 reverse engineering that has made this possible! - LordVirus
 * 
 */

namespace Minecraft_Cheats
{
    using PS3Lib;
    using PS3ManagerAPI;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows;
    using MessageBox = System.Windows.MessageBox;

    public static class Minecraft_Cheats
    {
        #region Helpers

        /// <summary>
        /// Current API Instance.
        /// </summary>
        private static PS3API PS3 = new PS3API();

        /// <summary>
        /// Are we connected?
        /// </summary>
        private static bool Connected = false;

        /// <summary>
        /// The time needed to wait between async looping opperations.
        /// This really is used to lighten the load on PS3MAPI becuase it is painfully slow.
        /// </summary>
        private static int WaitTime = 300;

        /// <summary>
        /// Helper class file.
        /// </summary>
        public static class HelperFunctions
        {
            /// <summary>
            /// Set the API.
            /// </summary>
            public static PS3API CurrentPS3Api
            {
                get { return PS3; }
            }

            /// <summary>
            /// Are we connected?
            /// </summary>
            public static bool isConnected
            {
                get { return Connected; }
            }

            /// <summary>
            /// Connect to the damn ps3.
            /// </summary>
            public static void Connect(SelectAPI Api)
            {
                try
                {
                    Connected = false;
                    Minecraft_Cheats.PS3 = new PS3API();
                    Minecraft_Cheats.PS3.ChangeAPI(Api);
                    if (PS3.ConnectTarget())
                    {
                        // Connected and attached.
                        if (PS3.AttachProcess())
                        {
                            if (Api.Equals(SelectAPI.ControlConsole))
                            {
                                PS3.CCAPI.Notify(CCAPI.NotifyIcon.TROPHY2, "Successfully connected and attached cheat tool to Minecraft!");
                                PS3.CCAPI.RingBuzzer(CCAPI.BuzzerMode.Double);
                            }

                            else if(Api.Equals(SelectAPI.PS3Manager))
                            {
                                PS3.PS3MAPI.Notify("Successfully connected and attached cheat tool to Minecraft!");
                                PS3.PS3MAPI.RingBuzzer(PS3MAPI.PS3_CMD.BuzzerMode.Double);
                                WaitTime = 1500;
                            }

                            if(!Api.Equals(SelectAPI.PS3Manager))
                            {
                                WaitTime = 300;
                            }

                            MessageBox.Show($"Connected and Attached with \"{Api} API\"", "Status", MessageBoxButton.OK, MessageBoxImage.Information);
                            Connected = true;
                        }

                        // Failed to attach.
                        else
                        {
                            if (Api.Equals(SelectAPI.ControlConsole))
                            {
                                PS3.CCAPI.Notify(CCAPI.NotifyIcon.WRONGWAY, "Failed to attach to Minecraft...");
                                PS3.CCAPI.RingBuzzer(CCAPI.BuzzerMode.Single);
                                MessageBox.Show($"Connected, but failed to attach with: \"{Api} API\"", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                            }

                            else if (Api.Equals(SelectAPI.PS3Manager))
                            {
                                PS3.PS3MAPI.Notify("Failed to attach to Minecraft...");
                                PS3.PS3MAPI.RingBuzzer(PS3MAPI.PS3_CMD.BuzzerMode.Single);
                                MessageBox.Show($"Connected, but failed to attach with: \"{Api} API\"", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                            }

                            else if (Api.Equals(SelectAPI.TargetManager))
                                MessageBox.Show($"Connected, but failed to attach with: \"{Api} API\" Are you using a debug eboot.bin?", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);

                            PS3 = null;
                        }
                    }

                    // Failed to connect.
                    else
                    {
                        MessageBox.Show($"Failed to connect & attach with: \"{Api}\" API", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                        PS3 = null;
                    }
                }

                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);

                    //string tmapiError = "Unable to load DLL 'PS3TMAPI.dll': The specified module could not be found. (Exception from HRESULT: 0x8007007E)";
                    if (e.Message.Contains("PS3TMAPI.dll"))
                    {
                        MessageBox.Show("This probably means you need to install the ProDG tmapi software.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    PS3 = null;
                }
            }

            /// <summary>
            /// Disconnect.
            /// </summary>
            public static void Disconnect()
            {
                if (Minecraft_Cheats.HelperFunctions.isConnected)
                {
                    if (CurrentPS3Api.GetCurrentAPI().Equals(SelectAPI.ControlConsole))
                    {
                        Minecraft_Cheats.HelperFunctions.CurrentPS3Api.CCAPI.RingBuzzer(CCAPI.BuzzerMode.Single);
                        Minecraft_Cheats.HelperFunctions.CurrentPS3Api.CCAPI.Notify(CCAPI.NotifyIcon.WRONGWAY, "Disconnected cheat tool from Minecraft!");
                    }

                    MessageBox.Show("Disconnected from your Playstation 3", "Status", MessageBoxButton.OK, MessageBoxImage.Information);

                    PS3.DisconnectTarget();

                    Connected = false;
                }

                else
                {
                    MessageBox.Show("You must be connected to your PlayStation 3 to disconnect from it.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            /// <summary>
            /// Toggles a mod.
            /// </summary>
            /// <param name="ModOption">The static function for a Minecraft_Cheats mod.</param>
            public static dynamic ToggleOption<T>(Expression<Func<T>> ModOption)
            {
                try
                {
                    // Get propertyInfo and make sure it exists.
                    PropertyInfo propertyInfo = ((MemberExpression)ModOption.Body).Member as PropertyInfo;

                    if (propertyInfo is null)
                    {
                        throw new ArgumentException("The lambda expression 'ModOption' should point to a valid mod Property.");
                    }

                    // Get the name of the property.
                    string propertyName = propertyInfo.Name;

                    if (typeof(Minecraft_Cheats).GetProperty(propertyName) is null)
                    {
                        throw new ArgumentException("The lambda expression 'ModOption' should be a property of 'Minecraft_Cheats'");
                    }

                    // Get the value of the property.
                    dynamic value = propertyInfo.GetValue(null/*Static class*/);

                    // If this toggle has multible toggle states
                    if (value is int)
                    {
                        if (Attribute.IsDefined(propertyInfo, typeof(ToggleState)).Equals(false))
                        {
                            throw new ArgumentException("Your int based toggle must contain the 'ToggleState' attribute!");
                        }

                        ToggleState toggleStateAttribute = (ToggleState)propertyInfo.GetCustomAttribute(typeof(ToggleState));
                        int minSize = toggleStateAttribute.MinValue;
                        int maxSize = toggleStateAttribute.MaxValue;

                        // Reset to 0 from max size
                        if (value.Equals(maxSize) && minSize.Equals(0))
                        {
                            propertyInfo.SetValue(null/*Static class*/, 0);
                        }

                        // Reset to 0 from min size.
                        else if (value.Equals(minSize) && minSize < 0)
                        {
                            propertyInfo.SetValue(null/*Static class*/, 0);
                        }

                        // We are at max size, and min value is less than 0, we then start at -1.
                        else if(value.Equals(maxSize) && minSize < 0)
                        {
                            propertyInfo.SetValue(null/*Static class*/, -1);
                        }

                        // We are at less than 0, deincrement until min size
                        else if (value < 0)
                        {
                            propertyInfo.SetValue(null/*Static class*/, --value);
                        }

                        // Increase toggle by 1.
                        else
                        {
                            propertyInfo.SetValue(null/*Static class*/, ++value);
                        }

                        return propertyInfo.GetValue(null/*Static class*/);
                    }

                    // Toggle current state.
                    else if (value is bool)
                    {
                        propertyInfo.SetValue(null/*Static class*/, !value);
                        return propertyInfo.GetValue(null/*Static class*/);
                    }

                    else
                    {
                        throw new ArgumentException("ModOption should be a property of the 'Minecraft_Cheats' class that is either an int or bool!");
                    }
                }

                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }

            /// <summary>
            /// Resets all of the mods.
            /// </summary>
            public static void Reset_All_Mods()
            {
                PropertyInfo[] cheats = typeof(Minecraft_Cheats).GetProperties();

                foreach(PropertyInfo cheat in cheats)
                {
                    dynamic cheatValue = cheat.GetValue(null);

                    if(cheatValue is int)
                    {
                        cheat.SetValue(null, 0);
                    }

                    else if (cheatValue is bool)
                    {
                        cheat.SetValue(null, false);
                    }
                }
            }
        }

        /// <summary>
        /// Custom toggle states being tracked as attributes.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
        private class ToggleState : Attribute
        {
            private int maxValue;
            private int minValue;

            public ToggleState(int maxValue, int minValue = 0)
            {
                try
                {
                    if (minValue > 0)
                        throw new ArgumentException("MinValue for the 'ToggleState' attribute must be less than 1!");

                    this.maxValue = maxValue;
                    this.minValue = minValue;
                }

                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    Process.GetCurrentProcess().Kill();
                }
            }

            /// <summary>
            /// The max value of this toggle state.
            /// </summary>
            public int MaxValue
            {
                get { return maxValue; }
            }

            /// <summary>
            /// The min value for this toggle state
            /// (Must be less than 1!)
            /// </summary>
            public int MinValue
            {
                get { return minValue; }
            }


            /// <summary>
            /// Throw a error for out of bounds value passed to a mod property of the type 'int'.
            /// </summary>
            public static void ErrorToggleState(ToggleState toggleState)
            {
                MessageBox.Show($"ToggleState out of bounds!\nThe max value for this toggle is: {toggleState.MaxValue}\nThe min value for this toggle state is: {toggleState.MinValue}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region "Int Toggles"
        [ToggleState(2)]
        public static int SUPER_JUMP
        {
            // Get current state from reading memory
            get
            {
                byte[] buffer = new byte[4];
                PS3.GetMemory(0x003AA77C, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3E, 0xD7, 0x0A, 0x3D }))
                    return 0;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x47, 0x7F, 0x42 }))
                    return 1;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0xD7, 0x0A, 0x3D }))
                    return 2;
                else
                {
                    PS3.SetMemory(0x004B2021, new byte[] { 0x3E, 0xD7, 0x0A, 0x3D });
                    return 0;
                }

            }

            // Set memory and use a number to represent current toggle state.
            set
            {
                uint offset = 0x003AA77C;

                if (value.Equals(0))
                {
                    //Minecraft_Cheats.REMOVE_FALL_DAMAGE = false;
                    PS3.SetMemory(offset, new byte[] { 0x3E, 0xD7, 0x0A, 0x3D }); ////SET to default
                }

                else if(value.Equals(1))
                {
                    //Minecraft_Cheats.REMOVE_FALL_DAMAGE = true;
                    PS3.SetMemory(offset, new byte[] { 0x3F, 0x47, 0x7F, 0x42 });
                }

                else if(value.Equals(2))
                {
                    //Minecraft_Cheats.REMOVE_FALL_DAMAGE = true;
                    PS3.SetMemory(offset, new byte[] { 0x3F, 0xD7, 0x0A, 0x3D });
                }

                else
                    ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
            }
        }

        [ToggleState(5)]
        public static int TNT_EXPLOSION_SIZE
        {
            // Get current state from reading memory
            get
            {
                byte[] buffer = new byte[2];
                PS3.GetMemory(0x0051E0D0, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40, 0x80 }))
                    return 0;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x00, 0x00 }))
                    return 1;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41, 0x30 }))
                    return 2;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41, 0x99 }))
                    return 3;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x42, 0x00 }))
                    return 4;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x43, 0x00 }))
                    return 5;
                else
                {
                    PS3.SetMemory(0x0051E0D0, new byte[] { 0x40, 0x80 });
                    return 0;
                }
            }

            // Set memory and use a number to represent current toggle state.
            set
            {
                uint offset = 0x0051E0D0;

                if (value.Equals(0))
                    PS3.SetMemory(offset, new byte[] { 0x40, 0x80 }); ////SET to default

                else if (value.Equals(1))
                    PS3.SetMemory(offset, new byte[] { 0x00, 0x00 }); //No explosion

                else if (value.Equals(2))
                    PS3.SetMemory(offset, new byte[] { 0x41, 0x30 }); //Small increase

                else if (value.Equals(3))
                    PS3.SetMemory(offset, new byte[] { 0x41, 0x99 }); //Medium increase

                else if (value.Equals(4))
                    PS3.SetMemory(offset, new byte[] { 0x42, 0x00 }); //Extreme increase

                else if (value.Equals(5))
                {
                    MessageBox.Show("This selection will seriously lag your ps3.", "Notice!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    PS3.SetMemory(offset, new byte[] { 0x43, 0x00 }); //Nuclear Explosion
                }

                else
                    ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
            }
        }

        [ToggleState(4)]
        public static int CREEPER_EXPLOSION_SIZE
        {
            // Get current state from reading memory
            get
            {
                byte[] buffer = new byte[2];
                PS3.GetMemory(0x001CC7E0, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80 }))
                    return 0;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x00, 0x00 }))
                    return 1;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41, 0x30 }))
                    return 2;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41, 0x99 }))
                    return 3;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x42, 0x80 }))
                    return 4;
                else
                {
                    PS3.SetMemory(0x001CC7E0, new byte[] { 0x3F, 0x80 });
                    return 0;
                }
            }

            // Set memory and use a number to represent current toggle state.
            set
            {
                uint offset = 0x001CC7E0;

                if (value.Equals(0))
                    PS3.SetMemory(offset, new byte[] { 0x3F, 0x80 }); ////SET to default

                else if (value.Equals(1))
                    PS3.SetMemory(offset, new byte[] { 0x00, 0x00 }); //No explosion

                else if (value.Equals(2))
                    PS3.SetMemory(offset, new byte[] { 0x41, 0x30 }); //Small Explosion

                else if (value.Equals(3))
                    PS3.SetMemory(offset, new byte[] { 0x41, 0x99 }); //Medium Explosion

                else if (value.Equals(4))
                {
                    MessageBox.Show("This selection will seriously lag your ps3.", "Notice!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    PS3.SetMemory(offset, new byte[] { 0x42, 0x80 }); //Large Explosion
                }

                else
                    ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
            }
        }

        [ToggleState(11)]
        public static int FOV_VALUE
        {
            // Get current state from reading memory
            get
            {
                byte[] buffer = new byte[3];
                PS3.GetMemory(0x014C670C, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80, 0x00 }))
                    return 0;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x70, 0x00 }))
                    return 1;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x60, 0x00 }))
                    return 2;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x50, 0x00 }))
                    return 3;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x40, 0x00 }))
                    return 4;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x30, 0x00 }))
                    return 5;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x25, 0x00 }))
                    return 6;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x20, 0x00 }))
                    return 7;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x15, 0x00 }))
                    return 8;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x10, 0x00 }))
                    return 9;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x1F, 0x80, 0x00 }))
                    return 10;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0xFF, 0xFF }))
                    return 11;
                else
                {
                    PS3.SetMemory(0x014C670C, new byte[] { 0x3F, 0x80, 0x00 });
                    return 0;
                }
            }

            // Set memory and use a number to represent current toggle state.
            set
            {
                uint offset = 0x014C670C;

                if (value.Equals(0))
                    PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00 }); //RESET

                else if (value.Equals(1))
                    PS3.SetMemory(offset, new byte[] { 0x3F, 0x70 }); //X1

                else if (value.Equals(2))
                    PS3.SetMemory(offset, new byte[] { 0x3F, 0x60 });//X2

                else if (value.Equals(3))
                    PS3.SetMemory(offset, new byte[] { 0x3F, 0x50 }); //X3

                else if (value.Equals(4))
                    PS3.SetMemory(offset, new byte[] { 0x3F, 0x40 });//X4

                else if (value.Equals(5))
                    PS3.SetMemory(offset, new byte[] { 0x3F, 0x30 }); //X5

                else if (value.Equals(6))
                    PS3.SetMemory(offset, new byte[] { 0x3F, 0x25 }); //X6

                else if (value.Equals(7))
                    PS3.SetMemory(offset, new byte[] { 0x3F, 0x20 }); //X7

                else if (value.Equals(8))
                    PS3.SetMemory(offset, new byte[] { 0x3F, 0x15 }); //X8 

                else if (value.Equals(9))
                    PS3.SetMemory(offset, new byte[] { 0x3F, 0x10 }); //X9

                else if (value.Equals(10))
                    PS3.SetMemory(offset, new byte[] { 0x1F, 0x80 }); //Updside Down

                else if (value.Equals(11))
                    PS3.SetMemory(offset, new byte[] { 0x3F, 0xFF, 0xFF }); //ZOOM

                else
                    ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
            }
        }

        [ToggleState(7)]
        public static int SKY_COLORS
        {
            // Get current state from reading memory
            get
            {
                if (!bRAINBOW_SKY)
                {
                    byte[] buffer1 = new byte[2];
                    byte[] buffer2 = new byte[2];
                    PS3.GetMemory(0x00410734, buffer1);
                    PS3.GetMemory(0x00410738, buffer2);

                    if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x40, 0xC0 }) && Enumerable.SequenceEqual(buffer2, new byte[] { 0x3F, 0x80 }))
                        return 0;
                    else if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x40, 0x50 }) && Enumerable.SequenceEqual(buffer2, new byte[] { 0x3F, 0x80 }))
                        return 1;
                    else if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x40, 0x50 }) && Enumerable.SequenceEqual(buffer2, new byte[] { 0xBF, 0x80 }))
                        return 2;
                    else if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x49, 0xC0 }) && Enumerable.SequenceEqual(buffer2, new byte[] { 0xBF, 0x80 }))
                        return 3;
                    else if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x49, 0xC0 }) && Enumerable.SequenceEqual(buffer2, new byte[] { 0x42, 0xC0 }))
                        return 4;
                    else if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x43, 0xC0 }) && Enumerable.SequenceEqual(buffer2, new byte[] { 0x42, 0xC0 }))
                        return 5;
                    else if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x43, 0xC0 }) && Enumerable.SequenceEqual(buffer2, new byte[] { 0xF0, 0xC0 }))
                        return 6;
                    else if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x40, 0xC0 }) && Enumerable.SequenceEqual(buffer2, new byte[] { 0x3F, 0xF0 }))
                        return 7;
                    else
                    {
                        PS3.SetMemory(0x00410734, new byte[] { 0x40, 0xC0 });
                        PS3.SetMemory(0x00410738, new byte[] { 0x3F, 0x80 });
                        return 0;
                    }
                }

                else
                    return 0;
            }

            // Set memory and use a number to represent current toggle state.
            set
            {
                if(!bRAINBOW_SKY)
                {
                    uint offset1 = 0x00410734;
                    uint offset2 = 0x00410738;

                    if (value.Equals(0))
                    {
                        // Reset
                        PS3.SetMemory(offset1, new byte[] { 0x40, 0xC0 });
                        PS3.SetMemory(offset2, new byte[] { 0x3F, 0x80 });
                    }

                    else if (value.Equals(1))
                    {
                        //GREEN SKY COLORS
                        PS3.SetMemory(offset1, new byte[] { 0x40, 0x50 });
                    }

                    else if (value.Equals(2))
                    {
                        //BLUE SKY COLORS
                        PS3.SetMemory(offset2, new byte[] { 0xBF, 0x80 });
                    }

                    else if (value.Equals(3))
                    {
                        //Purple Sky Colors
                        PS3.SetMemory(offset1, new byte[] { 0x49, 0xC0 });
                    }

                    else if (value.Equals(4))
                    {
                        //Pink Sky Colors
                        PS3.SetMemory(offset2, new byte[] { 0x42, 0xC0 });
                    }

                    else if (value.Equals(5))
                    {
                        //Orange Sky Colors
                        PS3.SetMemory(offset1, new byte[] { 0x43, 0xC0 });
                    }

                    else if (value.Equals(6))
                    {
                        //Black Sky Colors
                        PS3.SetMemory(offset2, new byte[] { 0xF0, 0xC0 });
                    }

                    else if (value.Equals(7))
                    {
                        //White Sky Colors
                        PS3.SetMemory(offset1, new byte[] { 0x40, 0xC0 });
                        PS3.SetMemory(offset2, new byte[] { 0x3F, 0xF0 });
                    }

                    else
                        ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
                }
            }
        }

        [ToggleState(6)]
        public static int HUD_COLORS
        {
            // Get current state from reading memory
            get
            {
                if (!bRAINBOW_HUD)
                {
                    byte[] buffer = new byte[16];
                    PS3.GetMemory(0x30DBAD64, buffer);

                    if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }))
                        return 0;
                    else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }))
                        return 1;
                    else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x1F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }))
                        return 2;
                    else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0xFF, 0x00, 0x00, 0x1F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }))
                        return 3;
                    else if (Enumerable.SequenceEqual(buffer, new byte[] { 0X5F, 0x80, 0x00, 0x00, 0x5F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }))
                        return 4;
                    else if (Enumerable.SequenceEqual(buffer, new byte[] { 0X8F, 0x80, 0x00, 0x00, 0x8F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }))
                        return 5;
                    else if (Enumerable.SequenceEqual(buffer, new byte[] { 0X00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }))
                        return 6;
                    else
                    {
                        PS3.SetMemory(0x30DBAD64, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 });
                        return 0;
                    }
                }

                else
                    return 0;
            }

            // Set memory and use a number to represent current toggle state.
            set
            {
                if(!bRAINBOW_HUD)
                {
                    uint offset = 0x30DBAD64;
                    if (value.Equals(0))
                        PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }); //RESET

                    else if (value.Equals(1))
                        PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }); //GREEN

                    else if (value.Equals(2))
                        PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x1F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }); //PURPLE

                    else if (value.Equals(3))
                        PS3.SetMemory(offset, new byte[] { 0x3F, 0xFF, 0x00, 0x00, 0x1F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }); //RED

                    else if (value.Equals(4))
                        PS3.SetMemory(offset, new byte[] { 0X5F, 0x80, 0x00, 0x00, 0x5F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }); //YELLOW

                    else if (value.Equals(5))
                        PS3.SetMemory(offset, new byte[] { 0X8F, 0x80, 0x00, 0x00, 0x8F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }); //BLUE

                    else if (value.Equals(6))
                        PS3.SetMemory(offset, new byte[] { 0X00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }); //INVISIBLE

                    else
                        ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
                }
            }
        }

        [ToggleState(2)]
        public static int TIME_CYCLE
        {
            // Get current state from reading memory
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x001DA1D4, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
                    return 0;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x43 }))
                    return 1;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x44 }))
                    return 2;
                else
                {
                    PS3.SetMemory(0x001DA1D4, new byte[] { 0x40 });
                    return 0;
                }

            }

            // Set memory and use a number to represent current toggle state.
            set
            {
                uint offset = 0x001DA1D4;

                if (value.Equals(0))
                    PS3.SetMemory(offset, new byte[] { 0x40 }); ////SET to default

                else if (value.Equals(1))
                    PS3.SetMemory(offset, new byte[] { 0x43 });

                else if (value.Equals(2))
                    PS3.SetMemory(offset, new byte[] { 0x44 });

                else
                    ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
            }
        }

        [ToggleState(5, -5)]
        public static int TIME_SCALE
        {
            // Get current state from reading memory
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00C202C9, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x50 }))
                    return 0;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
                    return -1;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x30 }))
                    return -2;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x20 }))
                    return -3;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x10 }))
                    return -4;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x00 }))
                    return -5;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x60 }))
                    return 1;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x70 }))
                    return 2;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x80 }))
                    return 3;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x90 }))
                    return 4;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xF0 }))
                    return 5;
                else
                {
                    PS3.SetMemory(0x00C202C9, new byte[] { 0x50 });
                    return 0;
                }
            }

            // Set memory and use a number to represent current toggle state.
            set
            {
                uint offset = 0x00C202C9;

                if (value.Equals(0))
                    PS3.SetMemory(offset, new byte[] { 0x50 }); //SET TO DEFAULT

                else if (value.Equals(-1))
                    PS3.SetMemory(offset, new byte[] { 0x40 }); //Speed Time -1

                else if (value.Equals(-2))
                    PS3.SetMemory(offset, new byte[] { 0x30 }); //Speed Time -2

                else if (value.Equals(-3))
                    PS3.SetMemory(offset, new byte[] { 0x20 }); //Speed Time -3

                else if (value.Equals(-4))
                    PS3.SetMemory(offset, new byte[] { 0x10 }); //Speed Time -4

                else if (value.Equals(-5))
                    PS3.SetMemory(offset, new byte[] { 0x00 }); //Speed Time -5

                else if (value.Equals(1))
                    PS3.SetMemory(offset, new byte[] { 0x60 }); //Speed Time X1

                else if (value.Equals(2))
                    PS3.SetMemory(offset, new byte[] { 0x70 }); //Speed Time X2

                else if (value.Equals(3))
                    PS3.SetMemory(offset, new byte[] { 0x80 }); //Speed Time X3

                else if (value.Equals(4))
                    PS3.SetMemory(offset, new byte[] { 0x90 }); //Speed Time X4

                else if (value.Equals(5))
                    PS3.SetMemory(offset, new byte[] { 0xF0 }); //Speed Time X5

                else
                    ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
            }
        }

        [ToggleState(7)]
        public static int ENTITY_RENDER_HEIGHT
        {
            // Get current state from reading memory
            get
            {
                byte[] buffer = new byte[2];
                PS3.GetMemory(0x00AD5EC8, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0xBF, 0x80 }))
                    return 0;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xBF, 0x00 }))
                    return 1;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xBF, 0xAA }))
                    return 2;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xBF, 0xFF }))
                    return 3;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xC0, 0x50 }))
                    return 4;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xC0, 0x99 }))
                    return 5;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xC0, 0xFF }))
                    return 6;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0XC1, 0x80 }))
                    return 7;
                else
                {
                    PS3.SetMemory(0x00AD5EC8, new byte[] { 0xBF, 0x80 });
                    return 0;
                }
            }

            // Set memory and use a number to represent current toggle state.
            set
            {
                uint offset = 0x00AD5EC8;

                if (value.Equals(0))
                    PS3.SetMemory(offset, new byte[] { 0xBF, 0x80 });

                else if (value.Equals(1))
                    PS3.SetMemory(offset, new byte[] { 0xBF, 0x00 });

                else if (value.Equals(2))
                    PS3.SetMemory(offset, new byte[] { 0xBF, 0xAA });

                else if (value.Equals(3))
                    PS3.SetMemory(offset, new byte[] { 0xBF, 0xFF });

                else if (value.Equals(4))
                    PS3.SetMemory(offset, new byte[] { 0xC0, 0x50 });

                else if (value.Equals(5))
                    PS3.SetMemory(offset, new byte[] { 0xC0, 0x99 });

                else if (value.Equals(6))
                    PS3.SetMemory(offset, new byte[] { 0xC0, 0xFF });

                else if (value.Equals(7))
                    PS3.SetMemory(offset, new byte[] { 0XC1, 0x80 });

                else
                    ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
            }
        }

        [ToggleState(6)]
        public static int ENTITY_RENDER_WIDTH
        {
            // Get current state from reading memory
            get
            {
                byte[] buffer = new byte[2];
                PS3.GetMemory(0x00AD5ECC, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80 }))
                    return 0;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x2F, 0x80 }))
                    return 1;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0xFF }))
                    return 2;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40, 0x80 }))
                    return 3;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40, 0xFF }))
                    return 4;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41, 0xFF }))
                    return 5;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x43, 0x80 }))
                    return 6;
                else
                {
                    PS3.SetMemory(0x00AD5ECC, new byte[] { 0x3F, 0x80 });
                    return 0;
                }
            }

            // Set memory and use a number to represent current toggle state.
            set
            {
                uint offset = 0x00AD5ECC;

                if (value.Equals(0))
                    PS3.SetMemory(offset, new byte[] { 0x3F, 0x80 });

                else if (value.Equals(1))
                    PS3.SetMemory(offset, new byte[] { 0x2F, 0x80 });

                else if (value.Equals(2))
                    PS3.SetMemory(offset, new byte[] { 0x3F, 0xFF });

                else if (value.Equals(3))
                    PS3.SetMemory(offset, new byte[] { 0x40, 0x80 });

                else if (value.Equals(4))
                    PS3.SetMemory(offset, new byte[] { 0x40, 0xFF });

                else if (value.Equals(5))
                    PS3.SetMemory(offset, new byte[] { 0x41, 0xFF });

                else if (value.Equals(6))
                    PS3.SetMemory(offset, new byte[] { 0x43, 0x80 });

                else
                    ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
            }
        }

        [ToggleState(15)]
        public static int FPS_VALUES
        {
            // Get current state from reading memory
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00AF0443, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x00 }))
                    return 0;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x10 }))
                    return 1;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x20 }))
                    return 2;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x30 }))
                    return 3;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
                    return 4;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x50 }))
                    return 5;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x60 }))
                    return 6;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x70 }))
                    return 7;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x80 }))
                    return 8;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x90 }))
                    return 9;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xA0 }))
                    return 10;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xB0 }))
                    return 11;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xC0 }))
                    return 12;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xD0 }))
                    return 13;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xE0 }))
                    return 14;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xF0 }))
                    return 15;
                else
                {
                    PS3.SetMemory(0x00AF0443, new byte[] { 0x00 });
                    return 0;
                }
            }

            // Set memory and use a number to represent current toggle state.
            set
            {
                uint offset = 0x00AF0443;

                if (value.Equals(0))
                    PS3.SetMemory(offset, new byte[] { 0x00 });

                else if (value.Equals(1))
                    PS3.SetMemory(offset, new byte[] { 0x10 });

                else if (value.Equals(2))
                    PS3.SetMemory(offset, new byte[] { 0x20 });

                else if (value.Equals(3))
                    PS3.SetMemory(offset, new byte[] { 0x30 });

                else if (value.Equals(4))
                    PS3.SetMemory(offset, new byte[] { 0x40 });

                else if (value.Equals(5))
                    PS3.SetMemory(offset, new byte[] { 0x50 });

                else if (value.Equals(6))
                    PS3.SetMemory(offset, new byte[] { 0x60 });

                else if (value.Equals(7))
                    PS3.SetMemory(offset, new byte[] { 0x70 });

                else if (value.Equals(8))
                    PS3.SetMemory(offset, new byte[] { 0x80 });

                else if (value.Equals(9))
                    PS3.SetMemory(offset, new byte[] { 0x90 });

                else if (value.Equals(10))
                    PS3.SetMemory(offset, new byte[] { 0xA0 });

                else if (value.Equals(11))
                    PS3.SetMemory(offset, new byte[] { 0xB0 });

                else if (value.Equals(12))
                    PS3.SetMemory(offset, new byte[] { 0xC0 });

                else if (value.Equals(13))
                    PS3.SetMemory(offset, new byte[] { 0xD0 });

                else if (value.Equals(14))
                    PS3.SetMemory(offset, new byte[] { 0xE0 });

                else if (value.Equals(15))
                    PS3.SetMemory(offset, new byte[] { 0xF0 });

                else
                    ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
            }
        }

        [ToggleState(12)]
        public static int GAMEPLAY_COLORS
        {
            // Get current state from reading memory
            get
            {
                if (!bRAINBOW_VISION)
                {
                    byte[] buffer = new byte[10];
                    PS3.GetMemory(0x3000AAF8, buffer);

                    if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80 }))
                        return 0;
                    else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0xFF, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80 }))
                        return 1;
                    else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x00, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80 }))
                        return 2;
                    else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x4F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80 }))
                        return 3;
                    else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x00, 0x00, 0x00, 0x3F, 0x80 }))
                        return 4;
                    else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0xFF, 0x00, 0x00, 0x3F, 0x80 }))
                        return 5;
                    else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80, 0x00, 0x00, 0x3F, 0x80 }))
                        return 6;
                    else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x00 }))
                        return 7;
                    else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0xFF }))
                        return 8;
                    else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80 }))
                        return 9;
                    else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80, 0x00, 0x00, 0x4F, 0x80 }))
                        return 10;
                    else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x4F, 0x80, 0x00, 0x00, 0x4F, 0x80, 0x00, 0x00, 0x4F, 0x80 }))
                        return 11;
                    else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x4F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80 }))
                        return 12;
                    else
                    {
                        PS3.SetMemory(0x3000AAF8, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80 });
                        return 0;
                    }
                }

                else
                    return 0;
            }

            // Set memory and use a number to represent current toggle state.
            set
            {
                if(!bRAINBOW_VISION)
                {
                    uint offset = 0x3000AAF8;

                    if (value.Equals(0))
                        PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80 });//DEFAULT

                    else if (value.Equals(1))
                        PS3.SetMemory(offset, new byte[] { 0x3F, 0xFF, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80 });//RED

                    else if (value.Equals(2))
                        PS3.SetMemory(offset, new byte[] { 0x3F, 0x00, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80 });//CYAN

                    else if (value.Equals(3))
                        PS3.SetMemory(offset, new byte[] { 0x4F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80 });//RED BLUE

                    else if (value.Equals(4))
                        PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x00, 0x00, 0x00, 0x3F, 0x80 });//Purple

                    else if (value.Equals(5))
                        PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0xFF, 0x00, 0x00, 0x3F, 0x80 });//Green

                    else if (value.Equals(6))
                        PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80, 0x00, 0x00, 0x3F, 0x80 });//Purple Green

                    else if (value.Equals(7))
                        PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x00 });//Yellow

                    else if (value.Equals(8))
                        PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0xFF });//Blue

                    else if (value.Equals(9))
                        PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80 });//Yellow Blue

                    else if (value.Equals(10))
                        PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80, 0x00, 0x00, 0x4F, 0x80 });//Cyan Red

                    else if (value.Equals(11))
                        PS3.SetMemory(offset, new byte[] { 0x4F, 0x80, 0x00, 0x00, 0x4F, 0x80, 0x00, 0x00, 0x4F, 0x80 });//Black and white

                    else if (value.Equals(12))
                        PS3.SetMemory(offset, new byte[] { 0x4F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80 });//Pink Red

                    else
                        ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
                }
            }
        }

        [ToggleState(5)]
        public static int SELECTED_BLOCK_LINE_COLOR
        {
            // Get current state from reading memory
            get
            {

                byte[] buffer1 = new byte[2];
                byte[] buffer2 = new byte[1];
                byte[] buffer3 = new byte[1];
                PS3.GetMemory(0x00B25990, buffer1);
                PS3.GetMemory(0x00B25A59, buffer2);
                PS3.GetMemory(0x00B25A5E, buffer3);

                if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x00, 0x00 }) &&
                    Enumerable.SequenceEqual(buffer2, new byte[] { 0x40 }) &&
                    Enumerable.SequenceEqual(buffer3, new byte[] { 0x08 }))
                    return 0;
                else if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x3F, 0xFF }) &&
                         Enumerable.SequenceEqual(buffer2, new byte[] { 0x40 }) &&
                         Enumerable.SequenceEqual(buffer3, new byte[] { 0x08 }))
                    return 1;
                else if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x00, 0x00 }) &&
                         Enumerable.SequenceEqual(buffer2, new byte[] { 0x7E }) &&
                         Enumerable.SequenceEqual(buffer3, new byte[] { 0x08 }))
                    return 2;
                else if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x00, 0x00 }) &&
                         Enumerable.SequenceEqual(buffer2, new byte[] { 0x7E }) &&
                         Enumerable.SequenceEqual(buffer3, new byte[] { 0x48 }))
                    return 3;
                else if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x00, 0x00 }) &&
                         Enumerable.SequenceEqual(buffer2, new byte[] { 0x7E }) &&
                         Enumerable.SequenceEqual(buffer3, new byte[] { 0x40 }))
                    return 4;
                else if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x3F, 0xFF }) &&
                         Enumerable.SequenceEqual(buffer2, new byte[] { 0x7E }) &&
                         Enumerable.SequenceEqual(buffer3, new byte[] { 0x40 }))
                    return 5;
                else
                {
                    PS3.SetMemory(0x00B25990, new byte[] { 0x00, 0x00 });
                    PS3.SetMemory(0x00B25A59, new byte[] { 0x40 });
                    PS3.SetMemory(0x00B25A5E, new byte[] { 0x08 });
                    return 0;
                }
            }

            // Set memory and use a number to represent current toggle state.
            set
            {
                uint offset1 = 0x00B25990;
                uint offset2 = 0x00B25A59;
                uint offset3 = 0x00B25A5E;

                if (value.Equals(0))
                {
                    PS3.SetMemory(offset1, new byte[] { 0x00, 0x00 });
                    PS3.SetMemory(offset2, new byte[] { 0x40 });
                    PS3.SetMemory(offset3, new byte[] { 0x08 });
                }

                // White
                else if (value.Equals(1))
                {
                    PS3.SetMemory(offset1, new byte[] { 0x3F, 0xFF });
                    PS3.SetMemory(offset2, new byte[] { 0x40 });
                    PS3.SetMemory(offset3, new byte[] { 0x08 });
                }

                // Green
                else if (value.Equals(2))
                {
                    PS3.SetMemory(offset1, new byte[] { 0x00, 0x00 });
                    PS3.SetMemory(offset2, new byte[] { 0x7E });
                    PS3.SetMemory(offset3, new byte[] { 0x08 });
                }

                // Cyan
                else if (value.Equals(3))
                {
                    PS3.SetMemory(offset1, new byte[] { 0x00, 0x00 });
                    PS3.SetMemory(offset2, new byte[] { 0x7E });
                    PS3.SetMemory(offset3, new byte[] { 0x48 });
                }

                // Blue
                else if (value.Equals(4))
                {
                    PS3.SetMemory(offset1, new byte[] { 0x00, 0x00 });
                    PS3.SetMemory(offset2, new byte[] { 0x7E });
                    PS3.SetMemory(offset2, new byte[] { 0x40 });
                }

                // Yellow
                else if (value.Equals(5))
                {
                    PS3.SetMemory(offset1, new byte[] { 0x3F, 0xFF });
                    PS3.SetMemory(offset2, new byte[] { 0x7E });
                    PS3.SetMemory(offset3, new byte[] { 0x40 });
                }

                else
                    ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
            }
        }

        [ToggleState(3)]
        public static int SELECTED_BLOCK_LINE_SIZE
        {
            // Get current state from reading memory
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00B25998, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
                    return 0;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41 }))
                    return 1;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x42 }))
                    return 2;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x60 }))
                    return 3;
                else
                {
                    PS3.SetMemory(0x00B25998, new byte[] { 0x40 });
                    return 0;
                }
            }

            // Set memory and use a number to represent current toggle state.
            set
            {
                uint offset = 0x00B25998;

                if (value.Equals(0))
                    PS3.SetMemory(offset, new byte[] { 0x40 });

                else if (value.Equals(1))
                    PS3.SetMemory(offset, new byte[] { 0x41 });

                else if (value.Equals(2))
                    PS3.SetMemory(offset, new byte[] { 0x42 });

                else if (value.Equals(3))
                    PS3.SetMemory(offset, new byte[] { 0x60 });

                else
                    ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
            }
        }

        [ToggleState(4)]
        public static int WEIRD_SUN_MOON_STATES
        {
            // Get current state from reading memory
            get
            {
                byte[] buffer = new byte[2];
                PS3.GetMemory(0x00B21F1C, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80 })) //DEFAULT
                    return 0;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x2F, 0x80 })) //REMOVE SUN / MOON
                    return 1;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0xFF })) //4 SUN + Light Moon Better
                    return 2;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x4F, 0xFF })) //Light Moon Max
                    return 3;
                else
                {
                    PS3.SetMemory(0x00B21F1C, new byte[] { 0x3F, 0x80 });
                    return 0;
                }
            }

            // Set memory and use a number to represent current toggle state.
            set
            {
                uint offset = 0x00B21F1C;

                if (value.Equals(0))
                    PS3.SetMemory(offset, new byte[] { 0x3F, 0x80 });

                else if (value.Equals(1))
                    PS3.SetMemory(offset, new byte[] { 0x2F, 0x80 });

                else if (value.Equals(2))
                    PS3.SetMemory(offset, new byte[] { 0x3F, 0xFF });

                else if (value.Equals(3))
                    PS3.SetMemory(offset, new byte[] { 0x4F, 0xFF });

                else
                    ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
            }

        }

        #endregion

        #region "Bool Toggles"
        /// <summary>
        /// Sets god mode to be enabled or disabled,
        /// Returns the current toggle state based off of value set in memory.
        /// </summary>
        public static bool GOD_MODE
        {
            get 
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x004B2021, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x80 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x20 }))
                    return false;
                else
                {
                    PS3.SetMemory(0x004B2021, new byte[] { 0x20 });
                    return false;
                }
            }

            set 
            {
                if(value)
                    PS3.SetMemory(0x004B2021, new byte[] { 0x80 });
                else
                    PS3.SetMemory(0x004B2021, new byte[] { 0x20 });
            }
        }

        public static bool SUPER_SPEED // 26 AD 89 40
        {
            get
            {
                byte[] buffer = new byte[3];
                PS3.GetMemory(0x003ABD49, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0xFF, 0xFF, 0xFF }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x26, 0xAD, 0x89 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x003ABD49, new byte[] { 0x26, 0xAD, 0x89 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x003ABD49, new byte[] { 0xFF, 0xFF, 0xFF });
                else
                    PS3.SetMemory(0x003ABD49, new byte[] { 0x26, 0xAD, 0x89 });
            }
        }

        // Can't get this to work for some fucking reason.
        //public static void INFINITE_FOOD(bool toggle) // 41 82 00 10 
        //{
        //    if (toggle)  //////Infinite Food
        //    {
        //        PS3.SetMemory(0x002BA538, new byte[] { 0x40 }); ////MODIFED VALUE
        //    }
        //    else
        //    {
        //        PS3.SetMemory(0x002BA538, new byte[] { 0x41 }); ////SET to default
        //    }
        //}

        public static bool FAR_KNOCKBACK
        {
            get
            {
                byte[] buffer = new byte[2];
                PS3.GetMemory(0x003A4018, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40, 0x80 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3E, 0xCC }))
                    return false;
                else
                {
                    // Don't reset this one because there is other mods that share this offset / address.
                    // PS3.SetMemory(0x003A4018, new byte[] { 0x3E, 0xCC });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x003A4018, new byte[] { 0x40, 0x80 });
                else
                    PS3.SetMemory(0x003A4018, new byte[] { 0x3E, 0xCC });
            }
        }

        public static bool ANTI_KNOCKBACK
        {
            get
            {
                byte[] buffer = new byte[2];
                PS3.GetMemory(0x003A4018, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x00, 0x00 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3E, 0xCC }))
                    return false;
                else
                {
                    // Don't reset this one because there is other mods that share this offset / address.
                    // PS3.SetMemory(0x003A4018, new byte[] { 0x3E, 0xCC });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x003A4018, new byte[] { 0x00, 0x00 });
                else
                    PS3.SetMemory(0x003A4018, new byte[] { 0x3E, 0xCC });
            }
        }

        public static bool INSTANT_HIT
        {
            get
            {
                byte[] buffer = new byte[2];
                PS3.GetMemory(0x003A3FF0, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40, 0x80 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x00 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x003A3FF0, new byte[] { 0x3F, 0x00 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x003A3FF0, new byte[] { 0x40, 0x80 });
                else
                    PS3.SetMemory(0x003A3FF0, new byte[] { 0x3F, 0x00 });
            }
        }

        public static bool INSTANT_KILL
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x001AC412, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x28 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x08 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x001AC412, new byte[] { 0x08 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x001AC412, new byte[] { 0x28 });
                else
                    PS3.SetMemory(0x001AC412, new byte[] { 0x08 });
            }
        }

        public static bool FAST_BOW
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x000FB4C6, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x18 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x08 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x000FB4C6, new byte[] { 0x08 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x000FB4C6, new byte[] { 0x18 });
                else
                    PS3.SetMemory(0x000FB4C6, new byte[] { 0x08 });
            }
        }

        public static bool MULTI_JUMP
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x0022790B, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x14 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x18 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x0022790B, new byte[] { 0x18 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x0022790B, new byte[] { 0x14 });
                else
                    PS3.SetMemory(0x0022790B, new byte[] { 0x18 });
            }
        }

        public static bool INSTANT_MINE
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00AEB090, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0xBF }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00AEB090, new byte[] { 0x3F });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00AEB090, new byte[] { 0xBF });
                else
                    PS3.SetMemory(0x00AEB090, new byte[] { 0x3F });
            }
        }

        public static bool INFINITE_CRAFT
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x0098871F, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x01 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x00 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x0098871F, new byte[] { 0x00 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x0098871F, new byte[] { 0x01 });
                else
                    PS3.SetMemory(0x0098871F, new byte[] { 0x00 });
            }
        }

        public static bool CAVE_XRAY
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00A99155, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x80 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x60 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00A99155, new byte[] { 0x60 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00A99155, new byte[] { 0x80 });
                else
                    PS3.SetMemory(0x00A99155, new byte[] { 0x60 });
            }
        }

        public static bool REMOVE_JUMP
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x003ABDC9, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0xF4 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xB4 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x003ABDC9, new byte[] { 0xB4 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x003ABDC9, new byte[] { 0xF4 });
                else
                    PS3.SetMemory(0x003ABDC9, new byte[] { 0xB4 });
            }
        }

        public static bool DISABLE_SWIM
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x003ABD40, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0xBF }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x0034B8F4, new byte[] { 0x3F });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x003ABD40, new byte[] { 0xBF });
                else
                    PS3.SetMemory(0x003ABD40, new byte[] { 0x3F });
            }
        }

        public static bool AUTO_MINE
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00AEC42C, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00AEC42C, new byte[] { 0x41 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00AEC42C, new byte[] { 0x40 });
                else
                    PS3.SetMemory(0x00AEC42C, new byte[] { 0x41 });
            }
        }

        public static bool AUTO_HIT
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00AEC34C, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00AEC34C, new byte[] { 0x41 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00AEC34C, new byte[] { 0x40 });
                else
                    PS3.SetMemory(0x00AEC34C, new byte[] { 0x41 });
            }
        }

        public static bool CHANGE_MOVEMENT_SWIM
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x003ABD44, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0xBC }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3C }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x003ABD44, new byte[] { 0x3C });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x003ABD44, new byte[] { 0xBC });
                else
                    PS3.SetMemory(0x003ABD44, new byte[] { 0x3C });
            }
        }

        private static bool bRAINBOW_SKY = false;
        private static bool bRAINBOW_SKY_LOOP = false;
        public static bool RAINBOW_SKY
        {
            get
            {
                return bRAINBOW_SKY_LOOP;
            }

            set
            {
                uint offset = 0x00410734;

                if (value)
                {
                    async void LoopVision()
                    {
                        while (bRAINBOW_SKY_LOOP)
                        {
                            PS3.SetMemory(offset, new byte[] { 0x40, 0x50, 0x00, 0x00, 0x3F, 0x80 }); // Green
                            await Task.Delay(WaitTime);

                            PS3.SetMemory(offset, new byte[] { 0x40, 0x50, 0x00, 0x00, 0xBF, 0x80 }); // Blue
                            await Task.Delay(WaitTime);

                            PS3.SetMemory(offset, new byte[] { 0x49, 0xC0, 0x00, 0x00, 0xBF, 0x80 }); // Purple
                            await Task.Delay(WaitTime);

                            PS3.SetMemory(offset, new byte[] { 0x42, 0xC0, 0x00, 0x00, 0xBF, 0x80 }); // Pink
                            await Task.Delay(WaitTime);

                            PS3.SetMemory(offset, new byte[] { 0x43, 0xC0, 0x00, 0x00, 0xBF, 0x80 }); // Orange
                            await Task.Delay(WaitTime);

                            //PS3.SetMemory(offset, new byte[] { 0xF0, 0xC0, 0x00, 0x00, 0xBF, 0x80 }); // Black
                            //await Task.Delay(WaitTime);

                            //PS3.SetMemory(offset, new byte[] { 0x40, 0xC0, 0x00, 0x00, 0x3F, 0xF0 }); // White
                            //await Task.Delay(WaitTime);

                            if(!bRAINBOW_SKY_LOOP)
                            {
                                PS3.SetMemory(offset, new byte[] { 0x40, 0xC0, 0x00, 0x00, 0x3F, 0x80 }); ////Normal
                                bRAINBOW_SKY = false;
                            }
                        }
                    }

                    if (!bLSD_TRIP)
                    {
                        MessageBoxResult YesNo = MessageBox.Show("This option may trigger epileptic people to have seisures!\nDo you wish to continue?", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (YesNo.Equals(MessageBoxResult.No))
                        {
                            return;
                        }
                    }

                    bRAINBOW_SKY = true;
                    bRAINBOW_SKY_LOOP = true;
                    Task.Run(() => LoopVision());
                }

                else
                    bRAINBOW_SKY_LOOP = false;
            }
        }

        private static bool bRAINBOW_VISION = false;
        private static bool bRAINBOW_VISION_LOOP = false;
        public static bool RAINBOW_VISION
        {
            get 
            {
                return bRAINBOW_VISION_LOOP;
            }

            set
            {
                uint offset = 0x3000AAF8;

                if (value)
                {
                    async void LoopVision()
                    {
                        while (bRAINBOW_VISION_LOOP)
                        {
                            PS3.SetMemory(offset, new byte[] { 0x3F, 0xFF, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80 });
                            await Task.Delay(WaitTime);

                            PS3.SetMemory(offset, new byte[] { 0x3F, 0x00, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80 });
                            await Task.Delay(WaitTime);

                            //PS3.SetMemory(offset, new byte[] { 0x4F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80 });
                            //await Task.Delay(WaitTime);

                            PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x00, 0x00, 0x00, 0x3F, 0x80 });
                            await Task.Delay(WaitTime);

                            PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0xFF, 0x00, 0x00, 0x3F, 0x80 });
                            await Task.Delay(WaitTime);

                            //PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80, 0x00, 0x00, 0x3F, 0x80 });
                            //await Task.Delay(WaitTime);

                            PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x00 });
                            await Task.Delay(WaitTime);

                            PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0xFF });
                            await Task.Delay(WaitTime);

                            //PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80 });
                            //await Task.Delay(WaitTime);

                            //PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80, 0x00, 0x00, 0x4F, 0x80 });
                            //await Task.Delay(WaitTime);

                            //PS3.SetMemory(offset, new byte[] { 0x4F, 0x80, 0x00, 0x00, 0x4F, 0x80, 0x00, 0x00, 0x4F, 0x80 });
                            //await Task.Delay(WaitTime);

                            //PS3.SetMemory(offset, new byte[] { 0x4F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80 });
                            //await Task.Delay(WaitTime);

                            if(!bRAINBOW_VISION_LOOP)
                            {
                                PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80 });
                                bRAINBOW_VISION = false;
                            }
                        }
                    }

                    if (!bLSD_TRIP)
                    {
                        MessageBoxResult YesNo = MessageBox.Show("This option may trigger epileptic people to have seisures!\nDo you wish to continue?", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (YesNo.Equals(MessageBoxResult.No))
                        {
                            return;
                        }
                    }

                    bRAINBOW_VISION = true;
                    bRAINBOW_VISION_LOOP = true;
                    Task.Run(() => LoopVision());
                }

                else
                    bRAINBOW_VISION_LOOP = false;
            }
        }

        private static bool bRAINBOW_HUD = false;
        private static bool bRAINBOW_HUD_LOOP = false;
        public static bool RAINBOW_HUD
        {
            get
            {
                return bRAINBOW_HUD_LOOP;
            }

            set
            {
                uint offset = 0x30DBAD64;

                if (value)
                {
                    async void LoopVision()
                    {
                        while (bRAINBOW_HUD_LOOP)
                        {
                            PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 });
                            await Task.Delay(WaitTime);

                            PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x1F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 });
                            await Task.Delay(WaitTime);

                            PS3.SetMemory(offset, new byte[] { 0x3F, 0xFF, 0x00, 0x00, 0x1F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 });
                            await Task.Delay(WaitTime);

                            PS3.SetMemory(offset, new byte[] { 0X5F, 0x80, 0x00, 0x00, 0x5F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 });
                            await Task.Delay(WaitTime);

                            PS3.SetMemory(offset, new byte[] { 0X8F, 0x80, 0x00, 0x00, 0x8F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 });
                            await Task.Delay(WaitTime);

                            if(!bRAINBOW_HUD_LOOP)
                            {
                                PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 });
                                bRAINBOW_HUD = false;
                            }
                        }
                    }

                    if (!bLSD_TRIP)
                    {
                        MessageBoxResult YesNo = MessageBox.Show("This option may trigger epileptic people to have seisures!\nDo you wish to continue?", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (YesNo.Equals(MessageBoxResult.No))
                        {
                            return;
                        }
                    }

                    bRAINBOW_HUD = true;
                    bRAINBOW_HUD_LOOP = true;
                    Task.Run(() => LoopVision());
                }

                else
                    bRAINBOW_HUD_LOOP = false;
            }
        }

        /// <summary>
        /// Lysergic Acid Diethylamide Simulation.
        /// </summary>
        private static bool bLSD_TRIP = false;
        public static bool LSD_TRIP
        {
            get
            {
                return bLSD_TRIP;
            }

            set
            {
                if (value)
                {
                    MessageBoxResult YesNo = MessageBox.Show("This option may trigger epileptic people to have seisures!\nDo you wish to continue?", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (YesNo.Equals(MessageBoxResult.Yes))
                    {
                        bLSD_TRIP = true;
                        Minecraft_Cheats.RAINBOW_HUD = true;
                        Minecraft_Cheats.RAINBOW_SKY = true;
                        Minecraft_Cheats.RAINBOW_VISION = true;
                        Minecraft_Cheats.RED_ESP_ENTITYS = true;
                        Minecraft_Cheats.FAR_REACH_ATTACK = true;
                        Minecraft_Cheats.FROST_WALKER_WITH_DIAMOND_ORE = true;
                        Minecraft_Cheats.FAST_BOW = true;
                        Minecraft_Cheats.SPEED_CLOUDS = true;
                        Minecraft_Cheats.BLUE_CLOUDS = true;
                        Minecraft_Cheats.WEIRD_SUN_MOON_STATES = 2;
                        Minecraft_Cheats.TIME_CYCLE = 2;
                        Minecraft_Cheats.SELECTED_BLOCK_LINE_COLOR = 3;
                        Minecraft_Cheats.FOV_VALUE = 5;
                        Minecraft_Cheats.ENTITY_RENDER_HEIGHT = 3;
                        Minecraft_Cheats.ENTITY_RENDER_WIDTH = 2;
                    }
                }

                else
                {
                    bLSD_TRIP = false;
                    Minecraft_Cheats.RAINBOW_HUD = false;
                    Minecraft_Cheats.RAINBOW_SKY = false;
                    Minecraft_Cheats.RAINBOW_VISION = false;
                    Minecraft_Cheats.FAST_BOW = false;
                    Minecraft_Cheats.RED_ESP_ENTITYS = false;
                    Minecraft_Cheats.FAR_REACH_ATTACK = false;
                    Minecraft_Cheats.FROST_WALKER_WITH_DIAMOND_ORE = false;
                    Minecraft_Cheats.SPEED_CLOUDS = false;
                    Minecraft_Cheats.BLUE_CLOUDS = false;
                    Minecraft_Cheats.WEIRD_SUN_MOON_STATES = 0;
                    Minecraft_Cheats.TIME_CYCLE = 0;
                    Minecraft_Cheats.SELECTED_BLOCK_LINE_COLOR = 0;
                    Minecraft_Cheats.FOV_VALUE = 0;
                    Minecraft_Cheats.ENTITY_RENDER_HEIGHT = 0;
                    Minecraft_Cheats.ENTITY_RENDER_WIDTH = 0;
                }
            }
        }

        public static bool BLUE_CLOUDS
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x0038B964, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0xFF }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3D }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x0038B964, new byte[] { 0x3D });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x0038B964, new byte[] { 0xFF });
                else
                    PS3.SetMemory(0x0038B964, new byte[] { 0x3D });
            }
        }

        public static bool SPEED_CLOUDS
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00B230AD, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x70 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x80 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00B230AD, new byte[] { 0x80 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00B230AD, new byte[] { 0x70 });
                else
                    PS3.SetMemory(0x00B230AD, new byte[] { 0x80 });
            }
        }

        public static bool BURN_IN_WATER
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00225EA8, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00225EA8, new byte[] { 0x40 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00225EA8, new byte[] { 0x41 });
                else
                    PS3.SetMemory(0x00225EA8, new byte[] { 0x40 });
            }
        }

        public static bool MAX_PICKUP_ITEMS
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00310AD4, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00310AD4, new byte[] { 0x40 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00310AD4, new byte[] { 0x41 });
                else
                    PS3.SetMemory(0x00310AD4, new byte[] { 0x40 });
            }
        }

        public static bool FULL_BRIGHT
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00A9A6C8, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x7F }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00A9A6C8, new byte[] { 0x3F });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00A9A6C8, new byte[] { 0x7F });
                else
                    PS3.SetMemory(0x00A9A6C8, new byte[] { 0x3F });
            }
        }


        public static bool KILL_AURA
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00233290, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0xFF }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x00 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00233290, new byte[] { 0x00 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00233290, new byte[] { 0xFF });
                else
                    PS3.SetMemory(0x00233290, new byte[] { 0x00 });
            }
        }

        public static bool FAST_BUILD
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00AECE70, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00AECE70, new byte[] { 0x41 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00AECE70, new byte[] { 0x40 });
                else
                    PS3.SetMemory(0x00AECE70, new byte[] { 0x41 });
            }
        }


        public static bool CAN_FLY
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00B02378, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00B02378, new byte[] { 0x41 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00B02378, new byte[] { 0x40 });
                else
                    PS3.SetMemory(0x00B02378, new byte[] { 0x41 });
            }
        }

        public static bool REMOVE_STARS_IN_SKY
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x0038C658, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0xFF }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x0038C658, new byte[] { 0x3F });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x0038C658, new byte[] { 0xFF });
                else
                    PS3.SetMemory(0x0038C658, new byte[] { 0x3F });
            }
        }

        public static bool NO_DAMAGE_HIT
        {
            get
            {
                byte[] buffer = new byte[2];
                PS3.GetMemory(0x003A3FF0, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0xFF, 0xFF }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x00 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x003A3FF0, new byte[] { 0x3F, 0x00 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x003A3FF0, new byte[] { 0xFF, 0xFF });
                else
                    PS3.SetMemory(0x003A3FF0, new byte[] { 0x3F, 0x00 });
            }
        }

        public static bool INFINITE_BLOCK
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x0010673F, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x00 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x01 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x0010673F, new byte[] { 0x01 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x0010673F, new byte[] { 0x00 });
                else
                    PS3.SetMemory(0x0010673F, new byte[] { 0x01 });
            }
        }

        public static bool CRITICAL_HIT
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x003ABDD1, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0xAF }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xEF }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x003ABDD1, new byte[] { 0xEF });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x003ABDD1, new byte[] { 0xAF });
                else
                    PS3.SetMemory(0x003ABDD1, new byte[] { 0xEF });
            }
        }

        public static bool CANT_GRAB_ITEMS
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00310B0C, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00310B0C, new byte[] { 0x40 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00310B0C, new byte[] { 0x41 });
                else
                    PS3.SetMemory(0x00310B0C, new byte[] { 0x40 });
            }
        }

        public static bool DISABLE_CHANGING_WEATHER
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00393E84, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00393E84, new byte[] { 0x40 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00393E84, new byte[] { 0x41 });
                else
                    PS3.SetMemory(0x00393E84, new byte[] { 0x40 });
            }
        }

        public static bool ROBLOX_WALK
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00A857D1, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x00 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x80 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00A857D1, new byte[] { 0x80 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00A857D1, new byte[] { 0x00 });
                else
                    PS3.SetMemory(0x00A857D1, new byte[] { 0x80 });
            }
        }

        public static bool CREEPER_INSTANT_EXPLODE
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x001CCC2C, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x001CCC2C, new byte[] { 0x41 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x001CCC2C, new byte[] { 0x40 });
                else
                    PS3.SetMemory(0x001CCC2C, new byte[] { 0x41 });
            }
        }

        public static bool TNT_INSTANT_EXPLODE
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x0051E6A0, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x0051E6A0, new byte[] { 0x41 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x0051E6A0, new byte[] { 0x40 });
                else
                    PS3.SetMemory(0x0051E6A0, new byte[] { 0x41 });
            }
        }

        public static bool REMOVE_FALL_DAMAGE
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x003A409C, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x003A409C, new byte[] { 0x41 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x003A409C, new byte[] { 0x40 });
                else
                    PS3.SetMemory(0x003A409C, new byte[] { 0x41 });
            }
        }

        public static bool ALL_PLAYERS_FAST_MINE
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x0010E0C6, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x18 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x08 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x0010E0C6, new byte[] { 0x08 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x0010E0C6, new byte[] { 0x18 });
                else
                    PS3.SetMemory(0x0010E0C6, new byte[] { 0x08 });
            }
        }

        public static bool WALL_HACK
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00A98F50, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3D }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00A98F50, new byte[] { 0x3D });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00A98F50, new byte[] { 0x3F });
                else
                    PS3.SetMemory(0x00A98F50, new byte[] { 0x3D });
            }
        }

        public static bool PLAYERS_SLIDE
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x003AAA98, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x003AAA98, new byte[] { 0x41 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x003AAA98, new byte[] { 0x40 });
                else
                    PS3.SetMemory(0x003AAA98, new byte[] { 0x41 });
            }
        }

        private static bool bMOVE_WITH_INV = false;
        public static bool MOVE_WITH_INVENTORY_OPENED
        {
            get
            {
                return bMOVE_WITH_INV;
            }

            set
            {
                if (value)
                {
                    bMOVE_WITH_INV = true;
                    async void Loop()
                    {
                        while (bMOVE_WITH_INV)
                        {
                            PS3.SetMemory(0x3000CF68, new byte[] { 0x00 }); ////For inventory open anytime
                            await Task.Delay(2000);
                        }
                    }
                    Loop();
                }

                else
                {
                    bMOVE_WITH_INV = false;
                }
            }
        }

        public static bool ENTITY_INVISIBLE
        {
            get
            {
                byte[] buffer1 = new byte[1];
                byte[] buffer2 = new byte[1];
                PS3.GetMemory(0x00011ADC, buffer1);
                PS3.GetMemory(0x003ABDD1, buffer1);

                if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x40 }) && Enumerable.SequenceEqual(buffer2, new byte[] { 0xFF }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x41 }) && Enumerable.SequenceEqual(buffer2, new byte[] { 0xEF }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00011ADC, new byte[] { 0x41 });
                    PS3.SetMemory(0x003ABDD1, new byte[] { 0xEF });
                    return false;
                }
            }

            set
            {
                if (value)
                {
                    PS3.SetMemory(0x00011ADC, new byte[] { 0x40 });
                    PS3.SetMemory(0x003ABDD1, new byte[] { 0xFF });
                }

                else
                {
                    PS3.SetMemory(0x00011ADC, new byte[] { 0x41 });
                    PS3.SetMemory(0x003ABDD1, new byte[] { 0xEF });
                }
            }
        }

        public static bool REVERSE_KNOCKBACK
        {
            get
            {
                byte[] buffer = new byte[2];
                PS3.GetMemory(0x003A4018, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0xBF, 0x80 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3E, 0xCC }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x003A4018, new byte[] { 0x3E, 0xCC });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x003A4018, new byte[] { 0xBF, 0x80 });
                else
                    PS3.SetMemory(0x003A4018, new byte[] { 0x3E, 0xCC });
            }
        }

        public static bool ESP_CHESTS
        {
            get
            {
                byte[] buffer = new byte[2];
                PS3.GetMemory(0x00A9C2B4, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3E, 0xFF }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00A9C2B4, new byte[] { 0x3F, 0x80 });
                    return false;
                }
            }

            set
            {
                if (value)
                {
                    Minecraft_Cheats.FULL_BRIGHT = true;
                    PS3.SetMemory(0x00A9C2B4, new byte[] { 0x3E, 0xFF });
                }
                else
                {
                    Minecraft_Cheats.FULL_BRIGHT = false;
                    PS3.SetMemory(0x00A9C2B4, new byte[] { 0x3F, 0x80 });
                }
            }
        }

        public static bool TNT_CANT_EXPLODE_BLOCKS
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00245DF0, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00245DF0, new byte[] { 0x41 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00245DF0, new byte[] { 0x40 });
                else
                    PS3.SetMemory(0x00245DF0, new byte[] { 0x41 });
            }
        }

        public static bool DEMI_GOD
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x003A4066, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x88 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x08 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x003A4066, new byte[] { 0x08 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x003A4066, new byte[] { 0x88 });
                else
                    PS3.SetMemory(0x003A4066, new byte[] { 0x08 });
            }
        }


        public static bool GRAVITY_MOON
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x003ABF88, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x003ABF88, new byte[] { 0x41 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x003ABF88, new byte[] { 0x40 });
                else
                    PS3.SetMemory(0x003ABF88, new byte[] { 0x41 });
            }
        }

        public static bool FLAT_BLOCKS
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x000924FF, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x01 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x00 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x000924FF, new byte[] { 0x00 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x000924FF, new byte[] { 0x01 });
                else
                    PS3.SetMemory(0x000924FF, new byte[] { 0x00 });
            }
        }

        public static bool AUTO_SPRINT
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00B01EEF, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x00 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x01 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00B01EEF, new byte[] { 0x01 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00B01EEF, new byte[] { 0x00 });
                else
                    PS3.SetMemory(0x00B01EEF, new byte[] { 0x01 });
            }
        }

        public static bool FORCE_SNOW
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00A9B986, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x58 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x08 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00A9B986, new byte[] { 0x08 });
                    return false;
                }
            }

            set
            {
                if (value)
                {
                    Minecraft_Cheats.FORCE_RAIN = true;
                    PS3.SetMemory(0x00A9B986, new byte[] { 0x58 });
                }
                else
                {
                    Minecraft_Cheats.FORCE_RAIN = false;
                    PS3.SetMemory(0x00A9B986, new byte[] { 0x08 });
                }
            }
        }

        public static bool FORCE_RAIN
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00A9B23E, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x48 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x08 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00A9B23E, new byte[] { 0x08 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00A9B23E, new byte[] { 0x48 });
                else
                    PS3.SetMemory(0x00A9B23E, new byte[] { 0x08 });
            }
        }

        public static bool SKY_TO_NETHER
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00B22050, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00B22050, new byte[] { 0x40 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00B22050, new byte[] { 0x41 });
                else
                    PS3.SetMemory(0x00B22050, new byte[] { 0x40 });
            }
        }


        /// <summary>
        /// Snoop Dogg was here...
        /// </summary>
        public static bool SMOKE_LOBBY
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00B24177, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x01 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x00 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00B24177, new byte[] { 0x00 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00B24177, new byte[] { 0x01 });
                else
                    PS3.SetMemory(0x00B24177, new byte[] { 0x00 });
            }
        }

        public static bool REMOVE_HANDS
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00AF10AB, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x01 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x00 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00AF10AB, new byte[] { 0x00 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00AF10AB, new byte[] { 0x01 });
                else
                    PS3.SetMemory(0x00AF10AB, new byte[] { 0x00 });
            }
        }

        public static bool SHOW_ARMOR
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x0090B5F3, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x01 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x00 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x0090B5F3, new byte[] { 0x00 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x0090B5F3, new byte[] { 0x01 });
                else
                    PS3.SetMemory(0x0090B5F3, new byte[] { 0x00 });
            }
        }

        public static bool REMOVE_RUN_ANIMATION
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00227BDC, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00227BDC, new byte[] { 0x41 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00227BDC, new byte[] { 0x40 });
                else
                    PS3.SetMemory(0x00227BDC, new byte[] { 0x41 });
            }
        }

        private static bool bBattle_Mod = false;
        public static bool BATTLE_MOD
        {
            get
            {
                return bBattle_Mod;
            }

            set
            {
                if (value)
                {
                    bBattle_Mod = true;
                    Minecraft_Cheats.KILL_AURA = true;
                    Minecraft_Cheats.INSTANT_HIT = true;
                    Minecraft_Cheats.SHOW_ARMOR = true;
                    Minecraft_Cheats.AUTO_SPRINT = true;
                    Minecraft_Cheats.FAST_BOW = true;
                    Minecraft_Cheats.ANTI_KNOCKBACK = true;
                    Minecraft_Cheats.RED_ESP_ENTITYS = true;
                    Minecraft_Cheats.FAR_REACH_ATTACK = true;
                }
                else
                {
                    bBattle_Mod = false;
                    Minecraft_Cheats.KILL_AURA = false;
                    Minecraft_Cheats.INSTANT_HIT = false;
                    Minecraft_Cheats.SHOW_ARMOR = false;
                    Minecraft_Cheats.AUTO_SPRINT = false;
                    Minecraft_Cheats.FAST_BOW = false;
                    Minecraft_Cheats.ANTI_KNOCKBACK = false;
                    Minecraft_Cheats.RED_ESP_ENTITYS = false;
                    Minecraft_Cheats.FAR_REACH_ATTACK = false;
                }
            }
        }

        public static bool ALL_PLAYERS_TAKE_DAMAGE
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x0039E2D4, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x0039E2D4, new byte[] { 0x41 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x0039E2D4, new byte[] { 0x40 });
                else
                    PS3.SetMemory(0x0039E2D4, new byte[] { 0x41 });
            }
        }

        public static bool FAR_REACH_ATTACK
        {
            get
            {
                byte[] buffer1 = new byte[1];
                byte[] buffer2 = new byte[1];
                byte[] buffer3 = new byte[1];
                byte[] buffer4 = new byte[2];
                PS3.GetMemory(0x00A95FB9, buffer1);
                PS3.GetMemory(0x00A95FC1, buffer2);
                PS3.GetMemory(0x00B351D8, buffer3);
                PS3.GetMemory(0x00B351DC, buffer4);

                if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x80 }) &&
                    Enumerable.SequenceEqual(buffer2, new byte[] { 0x80 }) &&
                    Enumerable.SequenceEqual(buffer3, new byte[] { 0x43 }) &&
                    Enumerable.SequenceEqual(buffer4, new byte[] { 0x43, 0xA0 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x18 }) &&
                         Enumerable.SequenceEqual(buffer2, new byte[] { 0x08 }) &&
                         Enumerable.SequenceEqual(buffer3, new byte[] { 0x40 }) &&
                         Enumerable.SequenceEqual(buffer4, new byte[] { 0x40, 0x90 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00A95FB9, new byte[] { 0x18 }); ////reach creative ENTITY MOB
                    PS3.SetMemory(0x00A95FC1, new byte[] { 0x08 }); ////reach survival ENTITY MOB
                    PS3.SetMemory(0x00B351D8, new byte[] { 0x40 });/////creative
                    PS3.SetMemory(0x00B351DC, new byte[] { 0x40, 0x90 });/////survival
                    return false;
                }
            }

            set
            {
                if (value)  //////Reach/Attack
                {
                    PS3.SetMemory(0x00A95FB9, new byte[] { 0x80 }); ////reach creative ENTITY MOB
                    PS3.SetMemory(0x00A95FC1, new byte[] { 0x80 }); ////reach survival ENTITY MOB
                    PS3.SetMemory(0x00B351D8, new byte[] { 0x43 });////creative
                    PS3.SetMemory(0x00B351DC, new byte[] { 0x43, 0xA0 });/////survival
                }
                else
                {
                    PS3.SetMemory(0x00A95FB9, new byte[] { 0x18 }); ////reach creative ENTITY MOB
                    PS3.SetMemory(0x00A95FC1, new byte[] { 0x08 }); ////reach survival ENTITY MOB
                    PS3.SetMemory(0x00B351D8, new byte[] { 0x40 });/////creative
                    PS3.SetMemory(0x00B351DC, new byte[] { 0x40, 0x90 });/////survival
                }
            }
        }

        public static bool RED_ESP_ENTITYS
        {
            get
            {
                byte[] buffer1 = new byte[1];
                byte[] buffer2 = new byte[2];
                PS3.GetMemory(0x00AD5B60, buffer1);
                PS3.GetMemory(0x00AD5A5C, buffer2);


                if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x41 }) &&
                    Enumerable.SequenceEqual(buffer2, new byte[] { 0x6F, 0xFF }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x40 }) &&
                         Enumerable.SequenceEqual(buffer2, new byte[] { 0x3F, 0x80 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00AD5B60, new byte[] { 0x40 }); ////SET to default ESP RED
                    PS3.SetMemory(0x00AD5A5C, new byte[] { 0x3F, 0x80 }); ////SET to default
                    return false;
                }
            }

            set
            {
                if (value)  //////ESP Players
                {
                    PS3.SetMemory(0x00AD5B60, new byte[] { 0x41 }); ////MODIFED VALUE ESP RED
                    PS3.SetMemory(0x00AD5A5C, new byte[] { 0x6F, 0xFF }); ////MODIFED VALUE
                }
                else
                {
                    PS3.SetMemory(0x00AD5B60, new byte[] { 0x40 }); ////SET to default ESP RED
                    PS3.SetMemory(0x00AD5A5C, new byte[] { 0x3F, 0x80 }); ////SET to default
                }
            }
        }

        public static bool DISABLE_HUD_TEXT
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x008FC4B4, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x008FC4B4, new byte[] { 0x41 });
                    return false;
                }
            }

            set
            {
                if (value)  //////Disable Text HUD
                    PS3.SetMemory(0x008FC4B4, new byte[] { 0x40 }); ////MODIFED VALUE
                else
                    PS3.SetMemory(0x008FC4B4, new byte[] { 0x41 }); ////MODIFED VALUE
            }
        }


        public static bool MINE_IN_ADVENTURE
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x002F0273, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x00 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x01 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x002F0273, new byte[] { 0x01 });
                    return false;
                }
            }

            set
            {
                if (value)  //////Disable Text HUD
                    PS3.SetMemory(0x002F0273, new byte[] { 0x00 });
                else
                    PS3.SetMemory(0x002F0273, new byte[] { 0x01 });
            }
        }

        public static bool BOAT_STOP_WORKING
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x000E0F90, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x000E0F90, new byte[] { 0x40 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x000E0F90, new byte[] { 0x41 }); ////MODIFED VALUE
                else
                    PS3.SetMemory(0x000E0F90, new byte[] { 0x40 }); ////SET to default
            }
        }

        public static bool GAMMA_TO_MAX
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00A9C2B5, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0xFF }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x80 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00A9C2B5, new byte[] { 0x80 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00A9C2B5, new byte[] { 0xFF }); ////MODIFED VALUE
                else
                    PS3.SetMemory(0x00A9C2B5, new byte[] { 0x80 }); ////SET to default
            }
        }

        public static bool TRIDENT_RIPTIDE_TO_MAX
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00217DCF, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x08 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x00 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00217DCF, new byte[] { 0x00 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00217DCF, new byte[] { 0x08 }); ////MODIFED VALUE
                else
                    PS3.SetMemory(0x00217DCF, new byte[] { 0x00 }); ////SET to default
            }
        }

        public static bool NO_BLOCK_COLISSION
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x000108AC, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x000108AC, new byte[] { 0x40 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x000108AC, new byte[] { 0x41 }); ////MODIFED VALUE
                else
                    PS3.SetMemory(0x000108AC, new byte[] { 0x40 }); ////SET to default
            }
        }

        public static bool FROST_WALKER
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00218A4F, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x01 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x00 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00218A4F, new byte[] { 0x00 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00218A4F, new byte[] { 0x01 }); ////MODIFED VALUE
                else
                    PS3.SetMemory(0x00218A4F, new byte[] { 0x00 }); ////SET to default
            }
        }

        public static bool FROST_WALKER_WITH_DIAMOND_ORE
        {
            get
            {
                byte[] buffer = new byte[4];
                PS3.GetMemory(0x014C8C84, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x32, 0x18, 0xB4, 0x60 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x32, 0x19, 0xF1, 0xE0 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x014C8C84, new byte[] { 0x32, 0x19, 0xF1, 0xE0 });
                    return false;
                }
            }

            set
            {
                if (value)// 32 19 F1 E0 change these 4 bytes for different block values.
                {
                    PS3.SetMemory(0x014C8C84, new byte[] { 0x32, 0x18, 0xB4, 0x60 }); ////Diamond ore
                    Minecraft_Cheats.FROST_WALKER = true;
                }
                else
                {
                    PS3.SetMemory(0x014C8C84, new byte[] { 0x32, 0x19, 0xF1, 0xE0 });
                    Minecraft_Cheats.FROST_WALKER = false;
                }
            }
        }

        public static bool NO_WEB_HAX
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00234F9F, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x00 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x01 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00234F9F, new byte[] { 0x01 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00234F9F, new byte[] { 0x00 }); ////MODIFED VALUE
                else
                    PS3.SetMemory(0x00234F9F, new byte[] { 0x01 }); ////SET to default
            }
        }

        public static bool ENTITY_GOD_MODE
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x003A3F6C, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x003A3F6C, new byte[] { 0x41 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x003A3F6C, new byte[] { 0x40 }); ////MODIFED VALUE
                else
                    PS3.SetMemory(0x003A3F6C, new byte[] { 0x41 }); ////SET to default
            }
        }

        public static bool DISABLE_RESPAWN
        {
            get
            {
                byte[] buffer = new byte[4];
                PS3.GetMemory(0x00AF1EE0, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x4E, 0x80, 0x00, 0x20 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xF8, 0x21, 0xFD, 0x21 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00AF1EE0, new byte[] { 0xF8, 0x21, 0xFD, 0x21 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00AF1EE0, new byte[] { 0x4E, 0x80, 0x00, 0x20 }); ////MODIFED VALUE
                else
                    PS3.SetMemory(0x00AF1EE0, new byte[] { 0xF8, 0x21, 0xFD, 0x21 }); ////SET to default
            }
        }

        public static bool WITHER_MONSTER_SPAWNER
        {
            get
            {
                byte[] buffer = new byte[19];
                PS3.GetMemory(0x32418A79, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x77, 0x00, 0x69, 0x00, 0x74, 0x00, 0x68, 0x00, 0x65, 0x00, 0x72, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x73, 0x00, 0x68, 0x00, 0x75, 0x00, 0x6C, 0x00, 0x6B, 0x00, 0x65, 0x00, 0x72, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x32418A79, new byte[] { 0x73, 0x00, 0x68, 0x00, 0x75, 0x00, 0x6C, 0x00, 0x6B, 0x00, 0x65, 0x00, 0x72, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07 });
                    return false;
                }
            }

            set
            {
                if (value)
                {
                    MessageBox.Show("Eggs of Shulker has been changed to the Wither eggs, you can spawn it by using the egg on a empty Monster Spawner", "Notice!", MessageBoxButton.OK, MessageBoxImage.Information);
                    PS3.SetMemory(0x32418A79, new byte[] { 0x77, 0x00, 0x69, 0x00, 0x74, 0x00, 0x68, 0x00, 0x65, 0x00, 0x72, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06 }); ////MODIFED VALUE
                }

                else
                {
                    PS3.SetMemory(0x32418A79, new byte[] { 0x73, 0x00, 0x68, 0x00, 0x75, 0x00, 0x6C, 0x00, 0x6B, 0x00, 0x65, 0x00, 0x72, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07 }); ////DEFAULT VALUE
                }
            }
        }

        public static bool SPAWN_IRON_GOLEM_EGGS
        {
            get
            {
                byte[] buffer = new byte[20];
                PS3.GetMemory(0x32418D18, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x30, 0x99, 0xF6, 0xA0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x30, 0x99, 0xD3, 0xE0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x32418D18, new byte[] { 0x30, 0x99, 0xD3, 0xE0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E });
                    return false;
                }
            }

            set
            {
                if (value)
                {
                    MessageBox.Show("Elder Guardian has been changed to the Iron Golem eggs, you can spawn it by using the egg on a empty Monster Spawner", "Notice!", MessageBoxButton.OK, MessageBoxImage.Information);
                    PS3.SetMemory(0x32418D18, new byte[] { 0x30, 0x99, 0xF6, 0xA0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E }); ////MODIFED VALUE
                }
                else
                {
                    PS3.SetMemory(0x32418D18, new byte[] { 0x30, 0x99, 0xD3, 0xE0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E }); ////DEFAULT VALUE
                }
            }
        }

        public static bool SPECTRAL_ARROWS_WITH_BOW
        {
            get
            {
                byte[] buffer = new byte[2];
                PS3.GetMemory(0x014C90D5, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x20, 0x8D }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x1E, 0xAD }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x014C90D5, new byte[] { 0x1E, 0xAD });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x014C90D5, new byte[] { 0x20, 0x8D }); ////MODIFED VALUE
                else
                    PS3.SetMemory(0x014C90D5, new byte[] { 0x1E, 0xAD }); ////SET to default
            }
        }

        public static bool CHANGE_AIR_TO_WATER
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x001D7FCC, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x001D7FCC, new byte[] { 0x41 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x001D7FCC, new byte[] { 0x40 }); ////MODIFED VALUE
                else
                    PS3.SetMemory(0x001D7FCC, new byte[] { 0x41 }); ////SET to default
            }
        }

        public static bool ALL_PLAYERS_LEFT_HAND
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x0151F2F3, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0xF0 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xF8 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x0151F2F3, new byte[] { 0xF8 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x0151F2F3, new byte[] { 0xF0 }); ////MODIFED VALUE
                else
                    PS3.SetMemory(0x0151F2F3, new byte[] { 0xF8 }); ////SET to default
            }
        }

        public static bool DERP_WALK
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x002341D0, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0xC3 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xC0 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x002341D0, new byte[] { 0xC0 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x002341D0, new byte[] { 0xC3 }); ////MODIFED VALUE
                else
                    PS3.SetMemory(0x002341D0, new byte[] { 0xC0 }); ////SET to default
            }
        }

        public static bool INFINITE_OXYGEN_IN_WATER
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x0039DE28, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x0039DE28, new byte[] { 0x40 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x0039DE28, new byte[] { 0x41 }); ////MODIFED VALUE
                else
                    PS3.SetMemory(0x0039DE28, new byte[] { 0x40 }); ////SET to default
            }
        }

        public static bool PLAYERS_TO_BABY
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x0039F52F, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x01 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x00 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x0039F52F, new byte[] { 0x00 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x0039F52F, new byte[] { 0x01 }); ////MODIFED VALUE
                else
                    PS3.SetMemory(0x0039F52F, new byte[] { 0x00 }); ////SET to default
            }
        }

        public static bool DISABLE_KILLED_OUT_OF_WORLD
        {
            get
            {
                byte[] buffer = new byte[4];
                PS3.GetMemory(0x003A9350, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x4E, 0x80, 0x00, 0x20 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xF8, 0x21, 0xFF, 0x91 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x003A9350, new byte[] { 0xF8, 0x21, 0xFF, 0x91 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x003A9350, new byte[] { 0x4E, 0x80, 0x00, 0x20 }); ////MODIFED VALUE
                else
                    PS3.SetMemory(0x003A9350, new byte[] { 0xF8, 0x21, 0xFF, 0x91 }); ////SET to default
            }
        }

        public static bool ELYTRA_MODE
        {
            get
            {
                byte[] buffer = new byte[4];
                PS3.GetMemory(0x003B3008, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x4E, 0x80, 0x00, 0x20 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xF8, 0x21, 0xFF, 0x91 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x003B3008, new byte[] { 0xF8, 0x21, 0xFF, 0x91 });
                    return false;
                }
            }

            set
            {
                if (value)
                {
                    Minecraft_Cheats.ELYTRA_CAPES = true;
                    PS3.SetMemory(0x003B3008, new byte[] { 0x4E, 0x80, 0x00, 0x20 }); ////MODIFED VALUE
                }
                else
                {
                    Minecraft_Cheats.ELYTRA_CAPES = false;
                    PS3.SetMemory(0x003B3008, new byte[] { 0xF8, 0x21, 0xFF, 0x91 }); ////SET to default
                }
            }
        }

        public static bool FREEZE_ALL_ENTITY
        {
            get
            {
                byte[] buffer = new byte[4];
                PS3.GetMemory(0x003A9FE8, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x4E, 0x80, 0x00, 0x20 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xF8, 0x21, 0xFF, 0x81 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x003A9350, new byte[] { 0xF8, 0x21, 0xFF, 0x81 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x003A9FE8, new byte[] { 0x4E, 0x80, 0x00, 0x20 }); ////MODIFED VALUE
                else
                    PS3.SetMemory(0x003A9FE8, new byte[] { 0xF8, 0x21, 0xFF, 0x81 }); ////SET to default
            }
        }

        public static bool DISABLE_PORTALS
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x002379E7, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x00 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x01 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x002379E7, new byte[] { 0x01 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x002379E7, new byte[] { 0x00 }); ////MODIFED VALUE
                else
                    PS3.SetMemory(0x002379E7, new byte[] { 0x01 }); ////SET to default
            }
        }

        public static bool ALL_PLAYERS_SUFFOCATE
        {
            get
            {
                byte[] buffer = new byte[4];
                PS3.GetMemory(0x0022FDC8, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x4E, 0x80, 0x00, 0x20 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xF8, 0x21, 0xFF, 0x11 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x0022FDC8, new byte[] { 0xF8, 0x21, 0xFF, 0x11 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x0022FDC8, new byte[] { 0x4E, 0x80, 0x00, 0x20 }); ////MODIFED VALUE
                else
                    PS3.SetMemory(0x0022FDC8, new byte[] { 0xF8, 0x21, 0xFF, 0x11 }); ////SET to default
            }
        }

        public static bool ELYTRA_CAPES
        {
            get
            {
                byte[] buffer = new byte[3];
                PS3.GetMemory(0x014C93D9, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x1C, 0x0A, 0x60 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x20, 0x94, 0x50 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x014C93D9, new byte[] { 0x20, 0x94, 0x50 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x014C93D9, new byte[] { 0x1C, 0x0A, 0x60 }); ////MODIFED VALUE
                else
                    PS3.SetMemory(0x014C93D9, new byte[] { 0x20, 0x94, 0x50 }); ////SET to default
            }
        }

        public static bool CREATIVE_INVENTORY
        {
            get
            {
                byte[] buffer = new byte[4];
                PS3.GetMemory(0x00AACEDC, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x4E, 0x80, 0x00, 0x20 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xF8, 0x21, 0xFF, 0x71 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00AACEDC, new byte[] { 0xF8, 0x21, 0xFF, 0x71 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00AACEDC, new byte[] { 0x4E, 0x80, 0x00, 0x20 }); ////MODIFED VALUE
                else
                    PS3.SetMemory(0x00AACEDC, new byte[] { 0xF8, 0x21, 0xFF, 0x71 }); ////SET to default
            }
        }

        public static bool STOP_CHUNK_LOADING
        {
            get
            {
                byte[] buffer = new byte[4];
                PS3.GetMemory(0x00B2437C, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x4E, 0x80, 0x00, 0x20 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xF8, 0x21, 0xFF, 0x71 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00B2437C, new byte[] { 0xF8, 0x21, 0xFF, 0x71 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00B2437C, new byte[] { 0x4E, 0x80, 0x00, 0x20 }); ////MODIFED VALUE
                else
                    PS3.SetMemory(0x00B2437C, new byte[] { 0xF8, 0x21, 0xFF, 0x71 }); ////SET to default
            }
        }

        public static bool OPTIMIZE_CHUNKS_LOAD
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x00B21C61, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0xD7 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x30 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x00B21C61, new byte[] { 0x30 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x00B21C61, new byte[] { 0xD7 }); ////MODIFED VALUE
                else
                    PS3.SetMemory(0x00B21C61, new byte[] { 0x30 }); ////SET to default
            }
        }

        public static bool NETHER_PORTAL_WITH_DIRT
        {
            get
            {
                byte[] buffer = new byte[2];
                PS3.GetMemory(0x014C89FE, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x14, 0x70 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x5E, 0x70 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    //PS3.SetMemory(0x014C89FE, new byte[] { 0x5E, 0x70 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x014C89FE, new byte[] { 0x14, 0x70 }); ////MODIFED VALUE
                else
                    PS3.SetMemory(0x014C89FE, new byte[] { 0x5E, 0x70 }); ////SET to default
            }
        }

        public static bool NETHER_PORTAL_WITH_STONE
        {
            get
            {
                byte[] buffer = new byte[2];
                PS3.GetMemory(0x014C89FE, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x11, 0xC0 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x5E, 0x70 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    //PS3.SetMemory(0x014C89FE, new byte[] { 0x5E, 0x70 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x014C89FE, new byte[] { 0x11, 0xC0 }); ////MODIFED VALUE
                else
                    PS3.SetMemory(0x014C89FE, new byte[] { 0x5E, 0x70 }); ////SET to default
            }
        }

        public static bool DEATH_SCREEN_VISION
        {
            get
            {
                byte[] buffer = new byte[1];
                PS3.GetMemory(0x003A7654, buffer);

                if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41 }))
                    return true;
                else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
                    return false;
                else
                {
                    // If it isn't either one of these states reset it to normal and return false.
                    PS3.SetMemory(0x003A7654, new byte[] { 0x40 });
                    return false;
                }
            }

            set
            {
                if (value)
                    PS3.SetMemory(0x003A7654, new byte[] { 0x41 }); ////MODIFED VALUE
                else
                    PS3.SetMemory(0x003A7654, new byte[] { 0x40 }); ////SET to default
            }
        }

        #endregion

    }
}
