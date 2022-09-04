/*
 * Minecraft Cheats for BetterCraft by: LordVirus 8/26/2022
 * 
 * Thank you all for the extensive Minecraft ps3 reverse engineering! - LordVirus
 * 
 */

namespace Minecraft_Cheats
{
    using PS3Lib;
    using PS3ManagerAPI;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows;
    using MessageBox = System.Windows.MessageBox;

    public static class Minecraft_Cheats
    {
        #region Connect and Attatch

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
        /// Read the current cheat toggle states
        /// (This only will track and store the states of the cheats if you use the toggle helper functions)
        /// </summary>
        public static Dictionary<string, dynamic> ToggleStates
        {
            get { return CheatKeyValuePairs; }
        }

        /// <summary>
        /// Current API Instance.
        /// </summary>
        private static PS3API PS3 = new PS3API();

        /// <summary>
        /// Are we connected?
        /// </summary>
        private static bool Connected = false;

        /// <summary>
        /// Stores the current cheats loaded and their toggle states.
        /// </summary>
        private static Dictionary<string, dynamic> CheatKeyValuePairs = new Dictionary<string, dynamic>();

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
            /// Connect to the damn ps3.
            /// </summary>
            public static void Connect(SelectAPI Api)
            {
                try
                {
                    Minecraft_Cheats.PS3 = new PS3API();
                    Minecraft_Cheats.PS3.ChangeAPI(Api);
                    if (PS3.ConnectTarget())
                    {
                        // Connected and attached.
                        if (PS3.AttachProcess())
                        {
                            if (Api.Equals(SelectAPI.ControlConsole))
                            {
                                Minecraft_Cheats.CurrentPS3Api.CCAPI.Notify(CCAPI.NotifyIcon.TROPHY2, "Successfully connected and attached cheat tool to Minecraft!");
                                Minecraft_Cheats.CurrentPS3Api.CCAPI.RingBuzzer(CCAPI.BuzzerMode.Double);
                            }

                            else if(Api.Equals(SelectAPI.PS3Manager))
                            {
                                Minecraft_Cheats.CurrentPS3Api.PS3MAPI.Notify("Successfully connected and attached cheat tool to Minecraft!");
                                Minecraft_Cheats.CurrentPS3Api.PS3MAPI.RingBuzzer(PS3MAPI.PS3_CMD.BuzzerMode.Double);
                                WaitTime = 1500;
                            }

                            // PS3MAPI just writes to memory way too slowly to reset the memory durring connect.
                            if(!Api.Equals(SelectAPI.PS3Manager))
                            {
                                Task.Run(() => { Minecraft_Cheats.HelperFunctions.Reset_All_Mods(); });
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
                                Minecraft_Cheats.CurrentPS3Api.CCAPI.Notify(CCAPI.NotifyIcon.WRONGWAY, "Failed to attach to Minecraft...");
                                Minecraft_Cheats.CurrentPS3Api.CCAPI.RingBuzzer(CCAPI.BuzzerMode.Single);
                                MessageBox.Show($"Connected, but failed to attach with: \"{Api} API\"", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                            }

                            else if (Api.Equals(SelectAPI.PS3Manager))
                            {
                                Minecraft_Cheats.CurrentPS3Api.PS3MAPI.Notify("Failed to attach to Minecraft...");
                                Minecraft_Cheats.CurrentPS3Api.PS3MAPI.RingBuzzer(PS3MAPI.PS3_CMD.BuzzerMode.Single);
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
                if (Minecraft_Cheats.isConnected)
                {
                    if (CurrentPS3Api.GetCurrentAPI().Equals(SelectAPI.ControlConsole))
                    {
                        Minecraft_Cheats.CurrentPS3Api.CCAPI.RingBuzzer(CCAPI.BuzzerMode.Single);
                        Minecraft_Cheats.CurrentPS3Api.CCAPI.Notify(CCAPI.NotifyIcon.WRONGWAY, "Disconnected cheat tool from Minecraft!");
                    }

                    MessageBox.Show("Disconnected from your Playstation 3", "Status", MessageBoxButton.OK, MessageBoxImage.Information);

                    Minecraft_Cheats.CurrentPS3Api.DisconnectTarget();

                    Connected = false;
                }

                else
                {
                    MessageBox.Show("You must be connected to your PlayStation 3 to disconnect from it.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            /// <summary>
            /// Toggles an option with a boolean state.
            /// </summary>
            /// <param name="ModOption">The static function for a Minecraft_Cheats mod.</param>
            public static bool ToggleOption(Action<bool> ModOption)
            {
                string ModFunctionName = ModOption.GetMethodInfo().Name;
                if (!CheatKeyValuePairs.ContainsKey(ModFunctionName))
                {
                    CheatKeyValuePairs.Add(ModFunctionName, true);
                    ModOption(true);
                    return true;
                }

                else
                {
                    if (CheatKeyValuePairs.TryGetValue(ModFunctionName, out dynamic toggleState))
                    {
                        if (toggleState is bool)
                        {
                            CheatKeyValuePairs[ModFunctionName] = !toggleState;
                            ModOption(!toggleState);
                            return !toggleState;
                        }
                    }

                    MessageBox.Show("Toggle Failed!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }


            /// <summary>
            /// Toggles an option with multible toggle states.
            /// </summary>
            /// <param name="ModOption">The static function for a Minecraft_Cheats mod.</param>
            public static int ToggleOption(Action<int> ModOption)
            {
                int toggleSize = (int)ModOption.GetMethodInfo().GetParameters()[0].DefaultValue;

                string ModFunctionName = ModOption.GetMethodInfo().Name;
                if (!CheatKeyValuePairs.ContainsKey(ModFunctionName))
                {
                    CheatKeyValuePairs.Add(ModFunctionName, 1);
                    ModOption(1);
                    return 1;
                }

                else
                {
                    if (CheatKeyValuePairs.TryGetValue(ModFunctionName, out dynamic toggleState))
                    {
                        if (toggleState is int)
                        {
                            if (!toggleState.Equals(toggleSize))
                            {
                                ++toggleState;
                                CheatKeyValuePairs[ModFunctionName] = toggleState;
                                ModOption(toggleState);
                                return toggleState;
                            }

                            CheatKeyValuePairs[ModFunctionName] = 0;
                            ModOption(0);
                            return 0;
                        }
                    }

                    MessageBox.Show("Toggle Failed!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return 0;
                }
            }


            /// <summary>
            /// Resets all of the mods.
            /// </summary>
            public static void Reset_All_Mods()
            {
                CheatKeyValuePairs.Clear();

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

                    if (flag && cheat.GetParameters().Length.Equals(1))
                    {
                        Type ParameterType = cheat.GetParameters()[0].ParameterType;

                        if (ParameterType.ToString().Equals("System.Boolean"))
                        {
                            cheat.Invoke(null/*static call*/, new object[] { false });
                        }

                        else if (ParameterType.ToString().Equals("System.Int32"))
                        {
                            cheat.Invoke(null/*static call*/, new object[] { 0 });
                        }
                    }
                }
            }
        }
        #endregion

        #region "Int Toggles"
        public static void SUPER_JUMP(int toggle = 2) // 3E D7 0A 3D 3C 60 00
        {
            uint offset = 0x003AA77C;

            if (toggle.Equals(0))  //////Super Jump
            {
                Minecraft_Cheats.REMOVE_FALL_DAMAGE(false);
                PS3.SetMemory(offset, new byte[] { 0x3E, 0xD7, 0x0A, 0x3D }); ////SET to default
            }

            else if (toggle.Equals(1))
            {
                Minecraft_Cheats.REMOVE_FALL_DAMAGE(true);
                PS3.SetMemory(offset, new byte[] { 0x3F, 0x47, 0x7F, 0x42 });
            }

            else if (toggle.Equals(2))
            {
                Minecraft_Cheats.REMOVE_FALL_DAMAGE(true);
                PS3.SetMemory(offset, new byte[] { 0x3F, 0xD7, 0x0A, 0x3D }); 
            }
        }

        public static void TNT_EXPLOSION_SIZE(int toggle = 5)
        {
            uint offset = 0x0051E0D0;

            if (toggle.Equals(0))
            {
                PS3.SetMemory(offset, new byte[] { 0x40, 0x80 }); ////SET to default
            }

            else if(toggle.Equals(1))
            {
                PS3.SetMemory(offset, new byte[] { 0x00, 0x00 }); //No explosion
            }

            else if (toggle.Equals(2))
            {
                PS3.SetMemory(offset, new byte[] { 0x41, 0x30 }); //Small increase
            }

            else if (toggle.Equals(3))
            {
                PS3.SetMemory(offset, new byte[] { 0x41, 0x99 }); //Medium increase
            }

            else if (toggle.Equals(4))
            {
                PS3.SetMemory(offset, new byte[] { 0x42, 0x00 }); //Extrem increase
            }

            else if (toggle.Equals(5))
            {
                MessageBox.Show("This selection might seriously crash your ps3.", "Notice!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                PS3.SetMemory(offset, new byte[] { 0x43, 0x00 }); //Nuclear Explosion
            }
        }

        public static void CREEPER_EXPLOSION_SIZE(int toggle = 4)
        {
            uint offset = 0x001CC7E0;

            if (toggle.Equals(0))
            {
                PS3.SetMemory(offset, new byte[] { 0x3F, 0x80 }); ////SET to default
            }

            else if(toggle.Equals(1))
            {
                PS3.SetMemory(offset, new byte[] { 0x00, 0x00 }); //No explosion
            }

            else if (toggle.Equals(2))
            {
                PS3.SetMemory(offset, new byte[] { 0x41, 0x30 }); //Small Explosion
            }

            else if (toggle.Equals(3))
            {
                PS3.SetMemory(offset, new byte[] { 0x41, 0x99 }); //Medium Explosion
            }

            else if (toggle.Equals(4))
            {
                MessageBox.Show("This selection will seriously lag your ps3.", "Notice!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                PS3.SetMemory(offset, new byte[] { 0x42, 0x80 }); //Nuclear Explosion
            }
        }

        public static void ITEMS_SIZE(int toggle = 7)
        {
            uint offset = 0x00AF6B9C;
            uint offset1 = 0x00AF6B98;

            if (toggle.Equals(0))
            {
                byte[] buffer = new byte[] { 0x41, 0x80 }; //RESET
                byte[] buffer1 = new byte[] { 0xBF, 0x80 }; //RESET
                PS3.SetMemory(offset, buffer);
                PS3.SetMemory(offset1, buffer1);
            }
            else if (toggle.Equals(1))
            {
                byte[] buffer = new byte[] { 0xAF }; //Hide Items
                PS3.SetMemory(offset1, buffer);
            }
            else if (toggle.Equals(2))
            {
                byte[] buffer = new byte[] { 0xBF, 0xFF }; //Big Items
                PS3.SetMemory(offset1, buffer);
            }
            else if (toggle.Equals(3))
            {
                byte[] buffer = new byte[] { 0xEF }; //WTF Items
                PS3.SetMemory(offset1, buffer);
            }
            else if (toggle.Equals(4))
            {
                byte[] buffer = new byte[] { 0x40, 0xFF }; //Little Items
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(5))
            {
                byte[] buffer = new byte[] { 0x41, 0xF0 }; //items X1
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(6))
            {
                byte[] buffer = new byte[] { 0x42, 0x80 }; //Items V2
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(7))
            {
                byte[] buffer = new byte[] { 0x43, 0x80 }; //Extrem Items
                PS3.SetMemory(offset, buffer);
            }
        }

        public static void FOV_VALUE(int toggle = 11) //FOV
        {
            uint offset = 0x014C670C;

            if (toggle.Equals(0))
            {
                byte[] buffer = new byte[] { 0x3F, 0x80 }; //RESET
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(1))
            {
                byte[] buffer = new byte[] { 0x3F, 0x70 }; //X1
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(2))
            {
                byte[] buffer = new byte[] { 0x3F, 0x60 }; //X2
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(3))
            {
                byte[] buffer = new byte[] { 0x3F, 0x50 }; //X3
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(4))
            {
                byte[] buffer = new byte[] { 0x3F, 0x40 }; //X4
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(5))
            {
                byte[] buffer = new byte[] { 0x3F, 0x30 }; //X5
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(6))
            {
                byte[] buffer = new byte[] { 0x3F, 0x25 }; //X6
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(7))
            {
                byte[] buffer = new byte[] { 0x3F, 0x20 }; //X7
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(8))
            {
                byte[] buffer = new byte[] { 0x3F, 0x15 }; //X8 
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(9))
            {
                byte[] buffer = new byte[] { 0x3F, 0x10 }; //X9
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(10))
            {
                byte[] buffer = new byte[] { 0x1F, 0x80 }; //Updside Down
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(11))
            {
                byte[] buffer = new byte[] { 0x3F, 0xFF, 0xFF }; //ZOOM
                PS3.SetMemory(offset, buffer);
            }
        }

        public static void SKY_COLORS(int toggle = 7)
        {
            uint offset1 = 0x00410734;
            uint offset2 = 0x00410738;

            if (toggle.Equals(0))
            {
                // Reset
                PS3.SetMemory(offset1, new byte[] { 0x40, 0xC0 });
                PS3.SetMemory(offset2, new byte[] { 0x3F, 0x80 });
            }
            else if (toggle.Equals(1))
            {
                //GREEN SKY COLORS
                PS3.SetMemory(offset1, new byte[] { 0x40, 0x50 });
            }
            else if (toggle.Equals(2))
            {
                //BLUE SKY COLORS
                PS3.SetMemory(offset2, new byte[] { 0xBF, 0x80 });
            }
            else if (toggle.Equals(3))
            {
                 //Purple Sky Colors
                PS3.SetMemory(offset1, new byte[] { 0x49, 0xC0 });
            }
            else if (toggle.Equals(4))
            {
                //Pink Sky Colors
                PS3.SetMemory(offset2, new byte[] { 0x42, 0xC0 });
            }
            else if (toggle.Equals(5))
            {
                //Orange Sky Colors
                PS3.SetMemory(offset1, new byte[] { 0x43, 0xC0 });
            }
            else if (toggle.Equals(6))
            {
                //Black Sky Colors
                PS3.SetMemory(offset2, new byte[] { 0xF0, 0xC0 });
            }
            else if (toggle.Equals(7))
            {
                //White Sky Colors
                PS3.SetMemory(offset1, new byte[] { 0x40, 0xC0 });
                PS3.SetMemory(offset2, new byte[] { 0x3F, 0xF0 });
            }
        }

        public static void HUD_COLORS(int toggle = 6)
        {
            uint offset = 0x30DBAD64;

            if (toggle.Equals(0))
            {
                byte[] buffer = new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }; //RESET
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(1))
            {
                byte[] buffer = new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }; //GREEN
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(2))
            {
                byte[] buffer = new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x1F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }; //PURPLE
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(3))
            {
                byte[] buffer = new byte[] { 0x3F, 0xFF, 0x00, 0x00, 0x1F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }; //RED
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(4))
            {
                byte[] buffer = new byte[] { 0X5F, 0x80, 0x00, 0x00, 0x5F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }; //YELLOW
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(5))
            {
                byte[] buffer = new byte[] { 0X8F, 0x80, 0x00, 0x00, 0x8F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }; //BLUE
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(6))
            {
                byte[] buffer = new byte[] { 0X00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }; //INVISIBLE
                PS3.SetMemory(offset, buffer);
            }
        }

        public static void TIME_CYCLE(int toggle = 2)
        {
            uint offset = 0x001DA1D4;
            if (toggle.Equals(0))
            {
                //SET TO DEFAULT
                PS3.SetMemory(offset, new byte[] { 0x40 });
            }
            else if (toggle.Equals(1))
            {
                PS3.SetMemory(offset, new byte[] { 0x43 });
            }
            else if (toggle.Equals(2))
            {
                PS3.SetMemory(offset, new byte[] { 0x44 });
            }
        }

        public static void SLOW_TIME_SCALE(int toggle = 5)
        {
            uint offset = 0x00C202C9;
            if (toggle.Equals(0))
            {
                //SET TO DEFAULT
                PS3.SetMemory(offset, new byte[] { 0x50 });
            }

            else if (toggle.Equals(1))
            {
                //Speed Time -1
                PS3.SetMemory(offset, new byte[] { 0x40 });
            }
            else if (toggle.Equals(2))
            {
                //Speed Time -2
                PS3.SetMemory(offset, new byte[] { 0x30 });
            }
            else if (toggle.Equals(3))
            {
                //Speed Time -3
                PS3.SetMemory(offset, new byte[] { 0x20 });
            }
            else if (toggle.Equals(4))
            {
                //Speed Time -4
                PS3.SetMemory(offset, new byte[] { 0x10 });
            }
            else if (toggle.Equals(5))
            {
                //Speed Time -5
                PS3.SetMemory(offset, new byte[] { 0x00 });
            }
        }

        public static void FAST_TIME_SCALE(int toggle = 5)
        {
            uint offset = 0x00C202C8;
            if (toggle.Equals(0))
            {
                byte[] buffer = new byte[] { 0x3F, 0x50 }; //SET TO DEFAULT
                PS3.SetMemory(offset, buffer);
            }

            else if (toggle.Equals(1))
            {
                byte[] buffer = new byte[] { 0x3F, 0x60 }; //Speed Time X1
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(2))
            {
                byte[] buffer = new byte[] { 0x3F, 0x70 }; //Speed Time X2
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(3))
            {
                byte[] buffer = new byte[] { 0x3F, 0x80 }; //Speed Time X3
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(4))
            {
                byte[] buffer = new byte[] { 0x3F, 0x90 }; //Speed Time X4
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(5))
            {
                byte[] buffer = new byte[] { 0x3F, 0xF0 }; //Speed Time X5
                PS3.SetMemory(offset, buffer);
            }
        }

        public static void ENTITY_RENDER_HEIGHT(int toggle = 7)
        {
            uint offset = 0x00AD5EC8;
            if (toggle.Equals(0))
            {
                byte[] buffer = new byte[] { 0xBF, 0x80 }; //DEFAULT
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(1))
            {
                byte[] buffer = new byte[] { 0xBF, 0x00 };
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(2))
            {
                byte[] buffer = new byte[] { 0xBF, 0xAA };
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(3))
            {
                byte[] buffer = new byte[] { 0xBF, 0xFF };
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(4))
            {
                byte[] buffer = new byte[] { 0XC0, 0x50 };
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(5))
            {
                byte[] buffer = new byte[] { 0XC0, 0x99 };
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(6))
            {
                byte[] buffer = new byte[] { 0XC0, 0xFF };
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(7))
            {
                byte[] buffer = new byte[] { 0XC1, 0x80 };
                PS3.SetMemory(offset, buffer);
            }
        }

        public static void ENTITY_RENDER_WIDTH(int toggle = 6)
        {
            uint offset = 0x00AD5ECC;
            if (toggle.Equals(0))
            {
                byte[] buffer = new byte[] { 0x3F, 0x80 }; //DEFAULT
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(1))
            {
                byte[] buffer = new byte[] { 0x2F, 0x80 };
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(2))
            {
                byte[] buffer = new byte[] { 0x3F, 0xFF };
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(3))
            {
                byte[] buffer = new byte[] { 0x40, 0x80 };
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(4))
            {
                byte[] buffer = new byte[] { 0X40, 0xFF };
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(5))
            {
                byte[] buffer = new byte[] { 0X41, 0xFF };
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(6))
            {
                byte[] buffer = new byte[] { 0x43, 0x80 };
                PS3.SetMemory(offset, buffer);
            }
        }

        public static void FPS_VALUES(int toggle = 15)
        {
            uint offset = 0x00AF0443;
            if (toggle.Equals(0))
            {
                byte[] buffer = new byte[] { 0x00 }; //DEFAULT
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(1))
            {
                byte[] buffer = new byte[] { 0x10 }; //X1
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(2))
            {
                byte[] buffer = new byte[] { 0x20 }; //x2
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(3))
            {
                byte[] buffer = new byte[] { 0x30 }; //X3
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(4))
            {
                byte[] buffer = new byte[] { 0x40 }; //X4
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(5))
            {
                byte[] buffer = new byte[] { 0x50 }; //X5
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(6))
            {
                byte[] buffer = new byte[] { 0x60 }; //X6
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(7))
            {
                byte[] buffer = new byte[] { 0x70 }; //X7
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(8))
            {
                byte[] buffer = new byte[] { 0x80 }; //X8
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(9))
            {
                byte[] buffer = new byte[] { 0x90 }; //X9
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(10))
            {
                byte[] buffer = new byte[] { 0xA0 }; //X10
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(11))
            {
                byte[] buffer = new byte[] { 0xB0 }; //X11
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(12))
            {
                byte[] buffer = new byte[] { 0xC0 }; //X12
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(13))
            {
                byte[] buffer = new byte[] { 0xD0 }; //X13
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(14))
            {
                byte[] buffer = new byte[] { 0xE0 }; //X14
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(15))
            {
                byte[] buffer = new byte[] { 0xF0 }; //X15
                PS3.SetMemory(offset, buffer);
            }
        }

        public static void GAMEPLAY_COLORS(int toggle = 12)
        {
            uint offset = 0x3000AAF8;
            if (toggle.Equals(0))
            {
                byte[] buffer = new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80 }; //DEFAULT
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(1))
            {
                byte[] buffer = new byte[] { 0x3F, 0xFF, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80 }; //RED
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(2))
            {
                byte[] buffer = new byte[] { 0x3F, 0x00, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80 }; //CYAN
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(3))
            {
                byte[] buffer = new byte[] { 0x4F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80 }; //RED BLUE
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(4))
            {
                byte[] buffer = new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x00, 0x00, 0x00, 0x3F, 0x80 }; //Purple
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(5))
            {
                byte[] buffer = new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0xFF, 0x00, 0x00, 0x3F, 0x80 }; //Green
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(6))
            {
                byte[] buffer = new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80, 0x00, 0x00, 0x3F, 0x80 }; //Purple Green
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(7))
            {
                byte[] buffer = new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x00 }; //Yellow
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(8))
            {
                byte[] buffer = new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0xFF }; //Blue
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(9))
            {
                byte[] buffer = new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80 }; //Yellow Blue
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(10))
            {
                byte[] buffer = new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80, 0x00, 0x00, 0x4F, 0x80 }; //Cyan Red
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(11))
            {
                byte[] buffer = new byte[] { 0x4F, 0x80, 0x00, 0x00, 0x4F, 0x80, 0x00, 0x00, 0x4F, 0x80 }; //Black and white
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(12))
            {
                byte[] buffer = new byte[] { 0x4F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80 }; //Pink Red
                PS3.SetMemory(offset, buffer);
            }
        }

        public static void SELECTED_BLOCK_LINE_COLOR(int toggle = 5)
        {
            uint offset1 = 0x00B25990;
            uint offset2 = 0x00B25A59;
            uint offset3 = 0x00B25A5E;

            if (toggle.Equals(0))
            {
                PS3.SetMemory(offset1, new byte[] { 0x00, 0x00 });
                PS3.SetMemory(offset2, new byte[] { 0x40 });
                PS3.SetMemory(offset3, new byte[] { 0x08 });
            }

            // White
            else if (toggle.Equals(1))
            {
                PS3.SetMemory(offset1, new byte[] { 0x3F, 0xFF });
                PS3.SetMemory(offset2, new byte[] { 0x40 });
                PS3.SetMemory(offset3, new byte[] { 0x08 });
            }

            // Green
            else if (toggle.Equals(2))
            {
                PS3.SetMemory(offset1, new byte[] { 0x00, 0x00 });
                PS3.SetMemory(offset2, new byte[] { 0x7E });
                PS3.SetMemory(offset3, new byte[] { 0x08 });
            }

            // Cyan
            else if (toggle.Equals(3))
            {
                PS3.SetMemory(offset1, new byte[] { 0x00, 0x00 });
                PS3.SetMemory(offset2, new byte[] { 0x7E });
                PS3.SetMemory(offset3, new byte[] { 0x48 });
            }

            // Blue
            else if(toggle.Equals(4))
            {
                PS3.SetMemory(offset1, new byte[] { 0x00, 0x00 });
                PS3.SetMemory(offset2, new byte[] { 0x7E });
                PS3.SetMemory(offset2, new byte[] { 0x40 });
            }

            // Yellow
            else if(toggle.Equals(5))
            {
                PS3.SetMemory(offset1, new byte[] { 0x3F, 0xFF });
                PS3.SetMemory(offset2, new byte[] { 0x7E });
                PS3.SetMemory(offset3, new byte[] { 0x40 });
            }
            
        }

        public static void SELECTED_BLOCK_LINE_SIZE(int toggle = 3)
        {
            uint offset = 0x00B25998;

            if (toggle.Equals(0))
            {
                PS3.SetMemory(offset, new byte[] { 0x40 });
            }
            else if (toggle.Equals(1))
            {
                byte[] buffer = new byte[] { 0x41 }; //Big X1
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(2))
            {
                byte[] buffer = new byte[] { 0x42 }; //Big X2
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(3))
            {
                byte[] buffer = new byte[] { 0x60 }; //Big x3
                PS3.SetMemory(offset, buffer);
            }
        }

        public static void ITEMS_IN_HAND_POSITIONS(int toggle = 6)
        {
            uint offset = 0x00ACA524;
            if (toggle.Equals(0))
            {
                byte[] buffer = new byte[] { 0xBF, 0x00 }; //DEFAULT
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(1))
            {
                byte[] buffer = new byte[] { 0xAF, 0x80 }; //X1 RIGHT
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(2))
            {
                byte[] buffer = new byte[] { 0xBF, 0x80 }; //X1 LEFT
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(3))
            {
                byte[] buffer = new byte[] { 0x3F, 0x00 }; //X2 RIGHT
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(4))
            {
                byte[] buffer = new byte[] { 0xBF, 0xFF }; //X2 LEFT
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(5))
            {
                byte[] buffer = new byte[] { 0x3F, 0xFF }; //X3 RIGHT
                PS3.SetMemory(offset, buffer);
            }
            else if (toggle.Equals(6))
            {
                byte[] buffer = new byte[] { 0x4F, 0xFF }; //REMOVE
                PS3.SetMemory(offset, buffer);
            }
        }

        public static void WEIRD_SUN_MOON_STATES(int toggle = 4)
        {
            uint remove = 0x00B21F1C;
            uint size = 0x00B21F28;
            uint change = 0x00B21F5C;

            if (toggle.Equals(0))
            {
                byte[] buffer = new byte[] { 0x3F, 0x80 }; //DEFAULT REMOVE SUN
                byte[] buffer1 = new byte[] { 0x42, 0xC8 }; //DEFAULT SIZE SUN
                byte[] buffer2 = new byte[] { 0x43, 0xB4 }; //DEFAULT CHANGE SUN / MOON
                PS3.SetMemory(remove, buffer);
                PS3.SetMemory(size, buffer1);
                PS3.SetMemory(change, buffer2);
            }
            else if (toggle.Equals(1))
            {
                byte[] buffer = new byte[] { 0x2F, 0x80 }; //REMOVE SUN / MOON
                PS3.SetMemory(remove, buffer);
            }
            else if (toggle.Equals(2))
            {
                byte[] buffer = new byte[] { 0x3F, 0xFF }; //4 SUN + Light Moon Better
                PS3.SetMemory(remove, buffer);
            }
            else if (toggle.Equals(3))
            {
                byte[] buffer = new byte[] { 0x4F, 0xFF }; //Light Moon Max
                PS3.SetMemory(remove, buffer);
            }
            else if (toggle.Equals(4))
            {
                byte[] buffer = new byte[] { 0x43, 0x84 }; //Moon To Sun and Sun To Moon
                PS3.SetMemory(change, buffer);
            }
        }

        public static void HAND_POSITION(int toggle = 9)
        {
            uint normal = 0x00AD14EC;
            uint normal1 = 0x00AD14F0;
            uint normal2 = 0x00AD14F4;
            uint normal3 = 0x00AD14F8;
            uint normal4 = 0x00AD0274;

            if (toggle.Equals(0))
            {
                byte[] buffer = new byte[] { 0x3F, 0x0F }; //DEFAULT 
                byte[] buffer1 = new byte[] { 0xBF, 0x19 }; //DEFAULT 
                byte[] buffer2 = new byte[] { 0xBF, 0x05 }; //DEFAULT 
                byte[] buffer3 = new byte[] { 0xBF, 0x38 }; //DEFAULT 
                byte[] buffer4 = new byte[] { 0x3F, 0x23 }; //DEFAULT 
                PS3.SetMemory(normal, buffer);
                PS3.SetMemory(normal1, buffer1);
                PS3.SetMemory(normal2, buffer2);
                PS3.SetMemory(normal3, buffer3);
                PS3.SetMemory(normal4, buffer4);
            }
            if (toggle.Equals(1))
            {
                byte[] buffer = new byte[] { 0x3F, 0x0F }; //Hand Right
                PS3.SetMemory(normal, buffer);
            }
            if (toggle.Equals(2))
            {
                byte[] buffer = new byte[] { 0xBF, 0x0F }; //Hand Left
                byte[] buffer1 = new byte[] { 0xBF, 0x23 }; //Hand Left
                PS3.SetMemory(normal, buffer);
                PS3.SetMemory(normal4, buffer1);
            }
            if (toggle.Equals(3))
            {
                byte[] buffer = new byte[] { 0x8F, 0x0F }; //Middle hand
                byte[] buffer1 = new byte[] { 0x8F, 0x23 }; //Middle Hand
                PS3.SetMemory(normal, buffer);
                PS3.SetMemory(normal4, buffer1);
            }
            if (toggle.Equals(4))
            {
                byte[] buffer = new byte[] { 0x8F, 0x05 }; //Hand Up
                PS3.SetMemory(normal2, buffer);
            }
            if (toggle.Equals(5))
            {
                byte[] buffer = new byte[] { 0xBF, 0x25 }; //Hand Down
                PS3.SetMemory(normal2, buffer);
            }
            if (toggle.Equals(6))
            {
                byte[] buffer = new byte[] { 0xBF, 0x68 }; //Hand Long X1
                PS3.SetMemory(normal3, buffer);
            }
            if (toggle.Equals(7))
            {
                byte[] buffer = new byte[] { 0xBF, 0x98 }; //Hand Long X2
                PS3.SetMemory(normal3, buffer);
            }
            if (toggle.Equals(8))
            {
                byte[] buffer = new byte[] { 0xBF, 0xB8 }; //Hand Long X3
                PS3.SetMemory(normal3, buffer);
            }
            if (toggle.Equals(9))
            {
                byte[] buffer = new byte[] { 0xBF, 0xFF }; //Hand Long MAX
                PS3.SetMemory(normal3, buffer);
            }
        }

        #endregion

        #region "Bool Toggles"

        public static void GOD_MODE(bool toggle)
        {
            if (toggle)
            {
                PS3.SetMemory(0x004B2021, new byte[] { 0x80 });
            }
            else
            {
                PS3.TMAPI.SetMemory(0x004B2021, new byte[] { 0x20 });
            }
        }

        public static void SUPER_SPEED(bool toggle) // 26 AD 89 40
        {
            uint offset = 0x003ABD49;

            if (toggle)  //////Super Speed
            {
                PS3.SetMemory(offset, new byte[] { 0xFF, 0xFF, 0xFF }); ////MODIFED VALUE
            }

            else
            {
                PS3.SetMemory(offset, new byte[] { 0x26, 0xAD, 0x89 }); ////SET to default
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

        public static void FAR_KNOCKBACK(bool toggle) //<== Options Knockback
        {
            if (toggle)
            {
                PS3.SetMemory(0x003A4018, new byte[] { 0x40, 0x80 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x003A4018, new byte[] { 0x3E, 0xCC }); ////SET to default
            }
        }

        public static void ANTI_KNOCKBACK(bool toggle)
        {
            if (toggle)
            {
                PS3.SetMemory(0x003A4018, new byte[] { 0x00, 0x00 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x003A4018, new byte[] { 0x3E, 0xCC }); ////SET to default
            }
        }

        public static void INSTANT_HIT(bool toggle)
        {
            if (toggle)
            {
                PS3.SetMemory(0x003A3FF0, new byte[] { 0x40, 0x80 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x003A3FF0, new byte[] { 0x3F, 0x00 }); ////SET to default
            }
        }

        public static void INSTANT_KILL(bool toggle)
        {
            if (toggle)  //////Instant Kill
            {
                PS3.SetMemory(0x001AC412, new byte[] { 0x28, 0x90 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x001AC412, new byte[] { 0x08, 0x90 }); ////SET to default
            }
        }

        public static void FAST_BOW(bool toggle)
        {
            if (toggle)  //////Bow Fast
            {
                PS3.SetMemory(0x000FB4C6, new byte[] { 0x18, 0x18 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x000FB4C6, new byte[] { 0x08, 0x18 }); ////SET to default
            }
        }

        public static void MULTI_JUMP(bool toggle)
        {
            if (toggle)  //////Multi Jump
            {
                PS3.SetMemory(0x0022790B, new byte[] { 0x14 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x0022790B, new byte[] { 0x18 }); ////SET to default
            }
        }

        public static void INSTANT_MINE(bool toggle)
        {
            if (toggle)  //////Instant mine
            {
                PS3.SetMemory(0x00AEB090, new byte[] { 0xBF }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x00AEB090, new byte[] { 0x3F }); ////SET to default
            }
        }

        public static void INFINITE_CRAFT(bool toggle)
        {
            if (toggle)  //////Craft
            {
                PS3.SetMemory(0x0098871F, new byte[] { 0x01 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x0098871F, new byte[] { 0x00 }); ////SET to default
            }
        }

        public static void CAVE_XRAY(bool toggle)
        {
            if (toggle)  //////XRay
            {
                PS3.SetMemory(0x00A99155, new byte[] { 0x80 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x00A99155, new byte[] { 0x60 }); ////SET to default
            }
        }

        public static void REMOVE_JUMP(bool toggle)
        {
            if (toggle)  ////Remove Jump
            {
                PS3.SetMemory(0x003ABDC9, new byte[] { 0xF4 });
            }
            else
            {
                PS3.SetMemory(0x003ABDC9, new byte[] { 0xB4 });
            }
        }

        public static void DISABLE_SWIM(bool toggle)
        {
            if (toggle)  ////Disable Swim
            {
                PS3.SetMemory(0x0034B8F4, new byte[] { 0x41 });
            }
            else
            {
                PS3.SetMemory(0x0034B8F4, new byte[] { 0x40 });
            }
        }

        public static void AUTO_MINE(bool toggle)
        {
            if (toggle)  ////Auto Mine
            {
                PS3.SetMemory(0x00AEC42C, new byte[] { 0x40 });
            }
            else
            {
                PS3.SetMemory(0x00AEC42C, new byte[] { 0x41 });
            }
        }

        public static void AUTO_HIT(bool toggle)
        {
            if (toggle)  ////Auto Hit
            {
                PS3.SetMemory(0x00AEC34C, new byte[] { 0x40 });
            }
            else
            {
                PS3.SetMemory(0x00AEC34C, new byte[] { 0x41 });
            }
        }

        public static void CHANGE_MOVEMENT_SWIM(bool toggle)
        {
            if (toggle)  ////Change Movement Swim
            {
                PS3.SetMemory(0x003ABD44, new byte[] { 0xBC });
            }
            else
            {
                PS3.SetMemory(0x003ABD44, new byte[] { 0x3C });
            }
        }

        private static bool bRAINBOW_SKY = false;
        public static async void RAINBOW_SKY(bool toggle)
        {
            uint offset = 0x00410734;

            if (toggle)
            {
                if (!bLSD_TRIP)
                {
                    MessageBoxResult YesNo = MessageBox.Show("This option may trigger epileptic people to have seisures!\nDo you wish to continue?", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (YesNo.Equals(MessageBoxResult.No))
                    {
                        return;
                    }
                }

                bRAINBOW_SKY = true;
                while (bRAINBOW_SKY)
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
                }
            }
            else
            {
                bRAINBOW_SKY = false;
                await Task.Delay(1700);
                PS3.SetMemory(offset, new byte[] { 0x40, 0xC0, 0x00, 0x00, 0x3F, 0x80 }); ////Normal
            }
        }

        private static bool bRAINBOW_VISION = false;
        public static async void RAINBOW_VISION(bool toggle)
        {
            uint offset = 0x3000AAF8;

            if (toggle)
            {
                if(!bLSD_TRIP)
                {
                    MessageBoxResult YesNo = MessageBox.Show("This option may trigger epileptic people to have seisures!\nDo you wish to continue?", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if(YesNo.Equals(MessageBoxResult.No))
                    {
                        return;
                    }
                }

                bRAINBOW_VISION = true;
                while (bRAINBOW_VISION)
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
                }
            }

            else
            {
                bRAINBOW_VISION = false;
                await Task.Delay(2300);
                PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80 });
            }
        }

        private static bool bRAINBOW_HUD = false;
        public static async void RAINBOW_HUD(bool toggle)
        {
            uint offset = 0x30DBAD64;

            if (toggle)
            {
                if(!bLSD_TRIP)
                {
                    MessageBoxResult YesNo = MessageBox.Show("This option may trigger epileptic people to have seisures!\nDo you wish to continue?", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (YesNo.Equals(MessageBoxResult.No))
                    {
                        return;
                    }
                }

                bRAINBOW_HUD = true;
                while (bRAINBOW_HUD)
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
                }
            }

            else
            {
                bRAINBOW_HUD = false;
                await Task.Delay(1600);
                PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80 });
            }
        }

        /// <summary>
        /// Lysergic Acid Diethylamide Simulation.
        /// </summary>
        private static bool bLSD_TRIP = false;
        public static void LSD_TRIP(bool toggle)
        {
            if (toggle)
            {
                MessageBoxResult YesNo = MessageBox.Show("This option may trigger epileptic people to have seisures!\nDo you wish to continue?", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                
                if(YesNo.Equals(MessageBoxResult.Yes))
                {
                    bLSD_TRIP = true;
                    Minecraft_Cheats.RAINBOW_HUD(true);
                    Minecraft_Cheats.RAINBOW_SKY(true);
                    Minecraft_Cheats.RAINBOW_VISION(true);
                    Minecraft_Cheats.RED_ESP_ENTITYS(true);
                    Minecraft_Cheats.FAR_REACH_ATTACK(true);
                    Minecraft_Cheats.FROST_WALKER_WITH_DIAMOND_ORE(true);
                    Minecraft_Cheats.FAST_BOW(true);
                    Minecraft_Cheats.SPEED_CLOUDS(true);
                    Minecraft_Cheats.BLUE_CLOUDS(true);
                    Minecraft_Cheats.WEIRD_SUN_MOON_STATES(2);
                    Minecraft_Cheats.TIME_CYCLE(2);
                    Minecraft_Cheats.SELECTED_BLOCK_LINE_COLOR(3);
                    Minecraft_Cheats.FOV_VALUE(5);
                    Minecraft_Cheats.ENTITY_RENDER_HEIGHT(3);
                    Minecraft_Cheats.ENTITY_RENDER_WIDTH(2);
                }
            }

            else
            {
                bLSD_TRIP = false;
                Minecraft_Cheats.RAINBOW_HUD(false);
                Minecraft_Cheats.RAINBOW_SKY(false);
                Minecraft_Cheats.RAINBOW_VISION(false);
                Minecraft_Cheats.FAST_BOW(false);
                Minecraft_Cheats.RED_ESP_ENTITYS(false);
                Minecraft_Cheats.FAR_REACH_ATTACK(false);
                Minecraft_Cheats.FROST_WALKER_WITH_DIAMOND_ORE(false);
                Minecraft_Cheats.SPEED_CLOUDS(false);
                Minecraft_Cheats.BLUE_CLOUDS(false);
                Minecraft_Cheats.WEIRD_SUN_MOON_STATES(0);
                Minecraft_Cheats.TIME_CYCLE(0);
                Minecraft_Cheats.SELECTED_BLOCK_LINE_COLOR(0);
                Minecraft_Cheats.FOV_VALUE(0);
                Minecraft_Cheats.ENTITY_RENDER_HEIGHT(0);
                Minecraft_Cheats.ENTITY_RENDER_WIDTH(0);
            }
        }

        public static void BLUE_CLOUDS(bool toggle)
        {
            uint offset = 0x0038B964;

            if (toggle) 
            {
                PS3.SetMemory(offset, new byte[] { 0xFF, 0xCC });
            }

            else
            {
                PS3.SetMemory(offset, new byte[] { 0x3D, 0xCC });
            }
        }

        public static void SPEED_CLOUDS(bool toggle)
        {
            uint offset = 0x00B230AD;

            if (toggle)
            {
                PS3.SetMemory(offset, new byte[] { 0x70 });
            }

            else
            {
                PS3.SetMemory(offset, new byte[] { 0x80 });
            }
        }

        public static void BURN_IN_WATER(bool toggle)
        {
            if (toggle)  //////Burn
            {
                PS3.SetMemory(0x00225EA8, new byte[] { 0x41 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x00225EA8, new byte[] { 0x40 }); ////SET to default
            }
        }

        public static void MAX_PICKUP_ITEMS(bool toggle)
        {
            if (toggle)  //////Max Pick Up Items
            {
                PS3.SetMemory(0x00310AD4, new byte[] { 0x41 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x00310AD4, new byte[] { 0x40 }); ////SET to default
            }
        }

        public static void FULL_BRIGHT(bool toggle)
        {
            if (toggle)  //////Night Vision
            {
                PS3.SetMemory(0x00A9A6C8, new byte[] { 0x7F }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x00A9A6C8, new byte[] { 0x3F }); ////SET to default
            }
        }


        public static void KILL_AURA(bool toggle)
        {
            if (toggle)  //////Kill aura
            {
                PS3.SetMemory(0x00233290, new byte[] { 0xFF }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x00233290, new byte[] { 0x00 }); ////SET to default
            }
        }

        public static void REMOVE_PARTICLES(bool toggle)
        {
            if (toggle)  //////Remove Particles
            {
                PS3.SetMemory(0x014C6881, new byte[] { 0x00 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x014C6881, new byte[] { 0x80 }); ////SET to default
            }
        }


        public static void BIG_PARTICLES(bool toggle)
        {
            if (toggle)  //////Big Particles
            {
                PS3.SetMemory(0x014C6881, new byte[] { 0xFF }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x014C6881, new byte[] { 0x80 }); ////SET to default
            }
        }


        public static void FAST_BUILD(bool toggle)
        {
            if (toggle)  //////Fast Build
            {
                PS3.SetMemory(0x00AECE70, new byte[] { 0x40 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x00AECE70, new byte[] { 0x41 }); ////SET to default
            }
        }


        public static void CAN_FLY(bool toggle)
        {
            if (toggle)  //////Can Fly
            {
                PS3.SetMemory(0x00B02378, new byte[] { 0x40 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x00B02378, new byte[] { 0x41 }); ////SET to default
            }
        }

        public static void REMOVE_STARS_IN_SKY(bool toggle)
        {
            if (toggle)  //////Remove Star in Sky
            {
                PS3.SetMemory(0x0038C658, new byte[] { 0xFF }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x0038C658, new byte[] { 0x3F }); ////SET to default
            }
        }

        public static void BYPASS_MAX_ITEMS(bool toggle)
        {
            if (toggle)  //////Bypass Max Items
            {
                PS3.SetMemory(0x00310AFC, new byte[] { 0x41 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x00310AFC, new byte[] { 0x40 }); ////SET to default
            }
        }

        public static void NO_DAMAGE_HIT(bool toggle)
        {
            if (toggle)  //////No Damage Hit
            {
                PS3.SetMemory(0x003A3FF0, new byte[] { 0xFF, 0xFF }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x003A3FF0, new byte[] { 0x3F, 0x00 }); ////SET to default
            }
        }

        public static void INFINITE_BLOCK(bool toggle)
        {
            if (toggle)  //////Infinity place block
            {
                PS3.SetMemory(0x0010673F, new byte[] { 0x00 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x0010673F, new byte[] { 0x01 }); ////SET to default
            }
        }

        public static void CRITICAL_HIT(bool toggle)
        {
            if (toggle)
            {
                PS3.SetMemory(0x003ABDD1, new byte[] { 0xAF }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x003ABDD1, new byte[] { 0xEF }); ////SET to default
            }
        }

        public static void CANT_GRAB_ITEMS(bool toggle)
        {
            if (toggle)  //////Cant Grab Items
            {
                PS3.SetMemory(0x00310B0C, new byte[] { 0x41 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x00310B0C, new byte[] { 0x40 }); ////SET to default
            }
        }

        public static void DISABLE_CHANGING_WEATHER(bool toggle)
        {
            if (toggle)  //////Block Changing Weather
            {
                PS3.SetMemory(0x00393E84, new byte[] { 0x41 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x00393E84, new byte[] { 0x40 }); ////SET to default
            }
        }

        public static void ROBLOX_WALK(bool toggle)
        {
            if (toggle)  //////Fun Arms/legs
            {
                PS3.SetMemory(0x00A857D1, new byte[] { 0x00 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x00A857D1, new byte[] { 0x80 }); ////SET to default
            }
        }

        public static void CREEPER_INSTANT_EXPLODE(bool toggle)
        {
            if (toggle)  //////Creeper instant explode
            {
                PS3.SetMemory(0x001CCC2C, new byte[] { 0x40 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x001CCC2C, new byte[] { 0x41 }); ////SET to default
            }
        }

        public static void TNT_INSTANT_EXPLODE(bool toggle)
        {
            if (toggle)  //////TNT INSTANT EXPLODE
            {
                PS3.SetMemory(0x0051E6A0, new byte[] { 0x40 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x0051E6A0, new byte[] { 0x41 }); ////SET to default
            }
        }

        public static void REMOVE_FALL_DAMAGE(bool toggle)
        {
            if (toggle)  //////No FALL Damage
            {
                PS3.SetMemory(0x003A409C, new byte[] { 0x40 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x003A409C, new byte[] { 0x41 }); ////SET to default
            }
        }

        public static void ALL_PLAYERS_FAST_MINE(bool toggle)
        {
            if (toggle)  //////All Players Fast Mine V2
            {
                PS3.SetMemory(0x0010E0C6, new byte[] { 0x18 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x0010E0C6, new byte[] { 0x08 }); ////SET to default
            }
        }

        public static void WALL_HACK(bool toggle)
        {
            if (toggle)  //////Wall Hack
            {
                PS3.SetMemory(0x00A98F50, new byte[] { 0x3F }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x00A98F50, new byte[] { 0x3D }); ////SET to default
            }
        }

        public static void PLAYERS_SLIDE(bool toggle)
        {
            if (toggle)  //////Player Slide
            {
                PS3.SetMemory(0x003AAA98, new byte[] { 0x40 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x003AAA98, new byte[] { 0x41 }); ////SET to default
            }
        }

        private static bool bMOVE_WITH_INV = false;
        public static async void MOVE_WITH_INVENTORY_OPENED(bool toggle)
        {
            if (toggle)  //////Walk with inventory open
            {
                bMOVE_WITH_INV = true;
                while (bMOVE_WITH_INV)
                {
                    PS3.SetMemory(0x3000CF68, new byte[] { 0x00 }); ////For inventory open anytime
                    await Task.Delay(1000);
                }
            }
            else
            {
                bMOVE_WITH_INV = false;
            }
        }

        public static void ENTITY_INVISIBLE(bool toggle)
        {
            if (toggle)  //////Invisible players
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

        public static void REVERSE_KNOCKBACK(bool toggle)
        {
            if (toggle)  //////Reverse Knockback
            {
                PS3.SetMemory(0x003A4018, new byte[] { 0xBF, 0x80 }); ////Knocback enabled
            }
            else
            {
                PS3.SetMemory(0x003A4018, new byte[] { 0x3E, 0xCC }); ////Knockback disabled
            }
        }

        public static void ESP_CHESTS(bool toggle)
        {
            if (toggle)  //////ESP Chests V2
            {
                Minecraft_Cheats.FULL_BRIGHT(true);
                PS3.SetMemory(0x00A9C2B4, new byte[] { 0x3E, 0xFF }); ////MODIFED VALUE
            }
            else
            {
                Minecraft_Cheats.FULL_BRIGHT(false);
                PS3.SetMemory(0x00A9C2B4, new byte[] { 0x3F, 0x80 }); ////SET to default
            }
        }

        public static void TNT_CANT_EXPLODE_BLOCKS(bool toggle)
        {
            if (toggle)  //////TNT Can't explode blocks
            {
                PS3.SetMemory(0x00245DF0, new byte[] { 0x40 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x00245DF0, new byte[] { 0x41 }); ////SET to default
            }
        }

        public static void DEMI_GOD(bool toggle)
        {
            if (toggle)  /////Demi God
            {
                PS3.SetMemory(0x003A4066, new byte[] { 0x88 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x003A4066, new byte[] { 0x08 }); ////SET to default
            }
        }


        public static void GRAVITY_MOON(bool toggle)
        {
            if (toggle)  //////Gravity Moon
            {
                PS3.SetMemory(0x003ABF88, new byte[] { 0x40 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x003ABF88, new byte[] { 0x41 }); ////SET to default
            }
        }

        public static void FLAT_BLOCKS(bool toggle)
        {
            if (toggle)  //////Flat Block
            {
                PS3.SetMemory(0x000924FF, new byte[] { 0x01 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x000924FF, new byte[] { 0x00 }); ////SET to default
            }
        }

        public static void TOGGLE_SPRINT(bool toggle)
        {
            if (toggle)  //////Toogle Sprint V2
            {
                PS3.SetMemory(0x00B01EEF, new byte[] { 0x00 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x00B01EEF, new byte[] { 0x01 }); ////SET to default
            }
        }

        public static void FORCE_SNOW(bool toggle)
        {
            if (toggle)  ////Make Snow !
            {
                PS3.SetMemory(0x00A9B23E, new byte[] { 0x48 }); ////MODIFED VALUE
                PS3.SetMemory(0x00A9B986, new byte[] { 0x58 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x00A9B23E, new byte[] { 0x08 }); ////SET to default
                PS3.SetMemory(0x00A9B986, new byte[] { 0x08 }); ////SET to default
            }
        }

        public static void FORCE_RAIN(bool toggle)
        {
            if (toggle)  ////Make Rain !
            {
                PS3.SetMemory(0x00A9B23E, new byte[] { 0x48 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x00A9B23E, new byte[] { 0x08 }); ////SET to default
            }
        }

        public static void SKY_TO_NETHER(bool toggle)
        {
            if (toggle)  //////Sky Nether
            {
                PS3.SetMemory(0x00B22050, new byte[] { 0x41 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x00B22050, new byte[] { 0x40 }); ////SET to default
            }
        }


        /// <summary>
        /// Snoop Dogg was here...
        /// </summary>
        /// <param name="toggle">If you can't figure out what a toggle does you're in the wrong place.</param>
        public static void SMOKE_LOBBY(bool toggle)
        {
            if (toggle)  //////Smoke Lobby
            {
                PS3.SetMemory(0x00B24177, new byte[] { 0x01 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x00B24177, new byte[] { 0x00 }); ////SET to default
            }
        }

        public static void REMOVE_HANDS(bool toggle)
        {
            if (toggle)  //////Remove Hand
            {
                PS3.SetMemory(0x00AF10AB, new byte[] { 0x01 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x00AF10AB, new byte[] { 0x00 }); ////SET to default
            }
        }

        public static void BATTLE_MOD(bool toggle)
        {
            if (toggle)
            {
                Minecraft_Cheats.KILL_AURA(true);
                Minecraft_Cheats.INSTANT_HIT(true);
                Minecraft_Cheats.RED_ESP_ENTITYS(true);
                Minecraft_Cheats.FAR_REACH_ATTACK(true);
                PS3.SetMemory(0x00AD8158, new byte[] { 0x4C }); ////Name Over Head
                PS3.SetMemory(0x00B01DEC, new byte[] { 0x40 }); ////AutoSprint
                PS3.SetMemory(0x003097C8, new byte[] { 0x40 }); ////ID Items
                PS3.SetMemory(0x003097B8, new byte[] { 0x40 }); ////ID Items
                PS3.SetMemory(0x0090B5F3, new byte[] { 0x01 }); ////Show Armor
                PS3.SetMemory(0x00AD5A5D, new byte[] { 0xFF }); ////Damage Indicator
                PS3.SetMemory(0x00227BDC, new byte[] { 0x40 }); ////Remove Run Anim
            }
            else
            {
                Minecraft_Cheats.KILL_AURA(false);
                Minecraft_Cheats.INSTANT_HIT(false);
                Minecraft_Cheats.RED_ESP_ENTITYS(false);
                Minecraft_Cheats.FAR_REACH_ATTACK(false);
                PS3.SetMemory(0x00AD8158, new byte[] { 0x2C }); ////Name Over Head
                PS3.SetMemory(0x00B01DEC, new byte[] { 0x41 }); ////AutoSprint
                PS3.SetMemory(0x003097C8, new byte[] { 0x41 }); ////ID Items
                PS3.SetMemory(0x003097B8, new byte[] { 0x41 }); ////ID Items
                PS3.SetMemory(0x0090B5F3, new byte[] { 0x00 }); ////Show Armor
                PS3.SetMemory(0x00AD5A5D, new byte[] { 0x80 }); ////Damage Indicator
                PS3.SetMemory(0x00227BDC, new byte[] { 0x41 }); ////Remove Run Anim
            }
        }

        public static void DISABLE_FOG(bool toggle)
        {
            if (toggle)  //////Disable Fog
            {
                PS3.SetMemory(0x003A3FF0, new byte[] { 0xFF, 0x00 }); //MODIFIED VALUES
            }
            else
            {
                PS3.SetMemory(0x003A3FF0, new byte[] { 0x3F, 0x80 }); //DEFAULT VALUES
            }
        }

        public static void ALL_PLAYERS_TAKE_DAMAGE(bool toggle)
        {
            if (toggle)  //////All Players Take Damage
            {
                PS3.SetMemory(0x0039E2D4, new byte[] { 0x40 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x0039E2D4, new byte[] { 0x41 }); ////SET to default
            }
        }

        public static void FAR_REACH_ATTACK(bool toggle)
        {
            if (toggle)  //////Reach/Attack
            {
                PS3.SetMemory(0x00A95FB9, new byte[] { 0x80 }); ////reach creative ENTITY MOB
                PS3.SetMemory(0x00A95FC1, new byte[] { 0x80 }); ////reach survival ENTITY MOB
                PS3.SetMemory(0x00B351D8, new byte[] { 0x43, 0xA0 });////creative
                PS3.SetMemory(0x00B351DC, new byte[] { 0x43, 0xA0 });/////survival
            }
            else
            {
                PS3.SetMemory(0x00A95FB9, new byte[] { 0x18 }); ////reach creative ENTITY MOB
                PS3.SetMemory(0x00A95FC1, new byte[] { 0x08 }); ////reach survival ENTITY MOB
                PS3.SetMemory(0x00B351D8, new byte[] { 0x40, 0xA0 });/////creative
                PS3.SetMemory(0x00B351DC, new byte[] { 0x40, 0x90 });/////survival
            }
        }

        public static void RED_ESP_ENTITYS(bool toggle)
        {
            if (toggle)  //////ESP Players
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

        public static void DISABLE_HUD_TEXT(bool toggle)
        {
            if (toggle)  //////Disable Text HUD
            {
                PS3.SetMemory(0x008FC4B4, new byte[] { 0x40 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x008FC4B4, new byte[] { 0x41 }); ////MODIFED VALUE
            }
        }


        public static void MINE_IN_ADVENTURE(bool toggle)
        {
            if (toggle) 
            {
                PS3.SetMemory(0x002F0273, new byte[] { 0x00 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x002F0273, new byte[] { 0x01 }); ////SET to default
            }
        }

        public static void BOAT_STOP_WORKING(bool toggle)
        {
            if (toggle)  /////Stop Boat
            {
                PS3.SetMemory(0x000E0F90, new byte[] { 0x41 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x000E0F90, new byte[] { 0x40 }); ////SET to default
            }
        }

        public static void GAMMA_TO_MAX(bool toggle)
        {
            if (toggle)  /////Max Gamma
            {
                PS3.SetMemory(0x00A9C2B5, new byte[] { 0xFF }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x00A9C2B5, new byte[] { 0x80 }); ////SET to default
            }
        }

        public static void TRIDENT_RIPTIDE_TO_MAX(bool toggle)
        {
            if (toggle)  /////Trident Riptide Max
            {
                PS3.SetMemory(0x00217DCF, new byte[] { 0x08 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x00217DCF, new byte[] { 0x00 }); ////SET to default
            }
        }

        public static void NO_BLOCK_COLISSION(bool toggle)
        {
            if (toggle)  /////No Colission Entity
            {
                PS3.SetMemory(0x000108AC, new byte[] { 0x41 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x000108AC, new byte[] { 0x40 }); ////SET to default
            }
        }

        public static void FROST_WALKER(bool toggle)
        {
            if (toggle)  /////Frost Walk
            {
                PS3.SetMemory(0x00218A4F, new byte[] { 0x01 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x00218A4F, new byte[] { 0x00 }); ////SET to default
            }
        }

        public static void FROST_WALKER_WITH_DIAMOND_ORE(bool toggle)
        {
            // 32 19 F1 E0 change these 4 bytes for different block values.
            if (toggle)  // Pimp walker
            {
                PS3.SetMemory(0x014C8C84, new byte[] { 0x32, 0x18, 0xB4, 0x60 }); ////Diamond ore
                Minecraft_Cheats.FROST_WALKER(true);
            }
            else
            {
                PS3.SetMemory(0x014C8C84, new byte[] { 0x32, 0x19, 0xF1, 0xE0 });
                Minecraft_Cheats.FROST_WALKER(false);
            }
        }

        public static void NO_WEB_HAX(bool toggle)
        {
            if (toggle)  /////No Web
            {
                PS3.SetMemory(0x00234F9F, new byte[] { 0x00 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x00234F9F, new byte[] { 0x01 }); ////SET to default
            }
        }

        public static void ENTITY_GOD_MODE(bool toggle)
        {
            if (toggle)  ////Entity God Mode
            {
                PS3.SetMemory(0x003A3F6C, new byte[] { 0x40 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x003A3F6C, new byte[] { 0x41 }); ////SET to default
            }
        }

        public static void DISABLE_RESPAWN(bool toggle)
        {
            if (toggle)  ///////Lock Respawn
            {
                PS3.SetMemory(0x00AF1EE0, new byte[] { 0x4E, 0x80, 0x00, 0x20 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x00AF1EE0, new byte[] { 0xF8, 0x21, 0xFD, 0x21 }); ////MODIFED VALUE
            }
        }

        public static void WITHER_MONSTER_SPAWNER(bool toogle)
        {
            if (toogle)
            {
                MessageBox.Show("Eggs of Shulker has been changed to the Wither eggs, you can spawn it by using the egg on a empty Monster Spawner", "Notice!", MessageBoxButton.OK, MessageBoxImage.Information);
                PS3.SetMemory(0x32418A79, new byte[] { 0x77, 0x00, 0x69, 0x00, 0x74, 0x00, 0x68, 0x00, 0x65, 0x00, 0x72, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x32418A79, new byte[] { 0x73, 0x00, 0x68, 0x00, 0x75, 0x00, 0x6C, 0x00, 0x6B, 0x00, 0x65, 0x00, 0x72, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07 }); ////DEFAULT VALUE
            }
        }

        public static void SPAWN_IRON_GOLEM_EGGS(bool toogle)
        {
            if (toogle)
            {
                MessageBox.Show("Elder Guardian has been changed to the Iron Golem eggs, you can spawn it by using the egg on a empty Monster Spawner", "Notice!", MessageBoxButton.OK, MessageBoxImage.Information);
                PS3.SetMemory(0x32418D18, new byte[] { 0x30, 0x99, 0xF6, 0xA0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x32418D18, new byte[] { 0x30, 0x99, 0xD3, 0xE0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E }); ////DEFAULT VALUE
            }
        }

        public static void SPECTRAL_ARROWS_WITH_BOW(bool toggle)
        {
            if (toggle)  //////Arrows To Spectral Arrows
            {
                PS3.SetMemory(0x014C90D5, new byte[] { 0x20, 0x8D }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x014C90D5, new byte[] { 0x1E, 0xAD }); ////SET to default
            }
        }

        public static void CHANGE_AIR_TO_WATER(bool toggle)
        {
            if (toggle)  //////Air To Water
            {
                PS3.SetMemory(0x001D7FCC, new byte[] { 0x40 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x001D7FCC, new byte[] { 0x41 }); ////SET to default
            }
        }

        public static void ALL_PLAYERS_LEFT_HAND(bool toggle)
        {
            if (toggle)  /////All Players Hand To Left
            {
                PS3.SetMemory(0x0151F2F3, new byte[] { 0xF0 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x0151F2F3, new byte[] { 0xF8 }); ////SET to default
            }
        }

        public static void ROBLOX_ANIMATION(bool toggle)
        {
            if (toggle)  /////ROBLOX ANIM
            {
                PS3.SetMemory(0x002341D0, new byte[] { 0xC3 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x002341D0, new byte[] { 0xC0 }); ////SET to default
            }
        }

        public static void INFINITE_OXYGEN_IN_WATER(bool toggle)
        {
            if (toggle)  ////Infinite Oxygen In Water
            {
                PS3.SetMemory(0x0039DE28, new byte[] { 0x41 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x0039DE28, new byte[] { 0x40 }); ////SET to default
            }
        }

        public static void PLAYERS_TO_BABY(bool toggle)
        {
            if (toggle)  ////Players To Baby
            {
                PS3.SetMemory(0x0039F52F, new byte[] { 0x01 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x0039F52F, new byte[] { 0x00 }); ////SET to default
            }
        }

        public static void DISABLE_KILLED_OUT_OF_WORLD(bool toggle)
        {
            if (toggle)  /////Disable Die Out Of The World
            {
                PS3.SetMemory(0x003A9350, new byte[] { 0x4E, 0x80, 0x00, 0x20 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x003A9350, new byte[] { 0xF8, 0x21, 0xFF, 0x91 }); ////SET to default
            }
        }

        public static void PLAYERS_ON_ELYTRA(bool toggle)
        {
            if (toggle)  /////Player on Elytra
            {
                PS3.SetMemory(0x003B3008, new byte[] { 0x4E, 0x80, 0x00, 0x20 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x003B3008, new byte[] { 0xF8, 0x21, 0xFF, 0x91 }); ////SET to default
            }
        }

        public static void FREEZE_ALL_ENTITY(bool toggle)
        {
            if (toggle)  /////Freeze All Entity
            {
                PS3.SetMemory(0x003A9FE8, new byte[] { 0x4E, 0x80, 0x00, 0x20 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x003A9FE8, new byte[] { 0xF8, 0x21, 0xFF, 0x81 }); ////SET to default
            }
        }

        public static void DISABLE_PORTALS(bool toggle)
        {
            if (toggle)  ////Disable Portal
            {
                PS3.SetMemory(0x002379E7, new byte[] { 0x00 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x002379E7, new byte[] { 0x01 }); ////SET to default
            }
        }

        public static void ALL_PLAYERS_SUFFOCATE(bool toggle)
        {
            if (toggle)  ////All Players In Wall
            {
                PS3.SetMemory(0x0022FDC8, new byte[] { 0x4E, 0x80, 0x00, 0x20 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x0022FDC8, new byte[] { 0xF8, 0x21, 0xFF, 0x11 }); ////SET to default
            }
        }

        public static void ELYTRA_ON_ALL_ENTITY(bool toggle)
        {
            if (toggle)  ///////Elytra on all players
            {
                PS3.SetMemory(0x014C93D9, new byte[] { 0x1C, 0x0A, 0x60 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x014C93D9, new byte[] { 0x20, 0x94, 0x50 }); ////MODIFED VALUE
            }
        }

        public static void CREATIVE_INVENTORY(bool toggle)
        {
            if (toggle)  ///////Creative Slot V2
            {
                PS3.SetMemory(0x00AACEDC, new byte[] { 0x4E, 0x80, 0x00, 0x20 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x00AACEDC, new byte[] { 0xF8, 0x21, 0xFF, 0x71 }); ////MODIFED VALUE
            }
        }

        public static void STOP_CHUNK_LOADING(bool toggle)
        {
            if (toggle)  ///////Stop Chunks Load
            {
                PS3.SetMemory(0x00B2437C, new byte[] { 0x4E, 0x80, 0x00, 0x20 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x00B2437C, new byte[] { 0xF8, 0x21, 0xFF, 0x71 }); ////MODIFED VALUE
            }
        }

        public static void NETHER_PORTAL_WITH_DIRT(bool toggle)
        {
            if (toggle)  ////Nether Portal With Dirt
            {
                PS3.SetMemory(0x014C89FE, new byte[] { 0x14, 0x70 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x014C89FE, new byte[] { 0x5E, 0x70 }); ////SET to default
            }
        }

        public static void NETHER_PORTAL_WITH_STONE(bool toggle)
        {
            if (toggle)  ////Nether Portal With Stone
            {
                PS3.SetMemory(0x014C89FE, new byte[] { 0x11, 0xC0 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x014C89FE, new byte[] { 0x5E, 0x70 }); ////SET to default
            }
        }

        public static void DEATH_SCREEN_VISION(bool toggle)
        {
            if (toggle)
            {
                PS3.SetMemory(0x003A7654, new byte[] { 0x41 }); ////MODIFED VALUE
            }
            else
            {
                PS3.SetMemory(0x003A7654, new byte[] { 0x40 }); ////SET to default
            }
        }

        #endregion

    }
}
