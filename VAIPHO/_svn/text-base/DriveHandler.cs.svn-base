using System;
using System.Collections.Generic;
using System.Text;
using ApplicationAPI;
using System.IO;

namespace VAIPHO
{
    public class DriveHandler
    {
        #region Delegates

        /// <summary>
        /// This delegate is used to handle notification fired by CApplicationAPI.ApplicationHandler threadsafe. It has the same input parameters as NavigationHandler.
        /// </summary>
        /// <param name="nEventID"></param>
        /// <param name="strData"></param>
        public delegate void NavigationNotificationDelegate(int nEventID, IntPtr strData);

        /// <summary>
        /// This delegate is used for simple notification in the main thread. 
        /// </summary>
        /// <param name="errorString_in"></param>
        public delegate void NotificationDelegate(string errorString_in);

        #endregion Delegates

        #region Events

        /// <summary>
        /// Static event, to which will subscribe our NotificationHandlerLocal
        /// </summary>
        public static event NavigationNotificationDelegate NavigationNotificationEvent;

        /// <summary>
        /// This event is used for main thread notification, where it passes simple string which can be written out at several places
        /// </summary>
        public static event NotificationDelegate NotificationEvent;

        #endregion Events

        #region Members

        /// <summary>
        /// Unlimited timeout
        /// </summary>
        private static int myTimeout = 0;

        /// <summary>
        /// privat instance of SError class, this is used for errorchecking
        /// </summary>
        private static SError mySerror;

        /// <summary>
        /// saving the path of drive.exe file
        /// </summary>
        private static string myDrivePath;

        /// <summary>
        /// containing the Drive's window layout parameters
        /// </summary>
        private static int[] myDriveWindowParameters;

        #endregion Members

        #region Public methods

        /// <summary>
        /// Handles the events raised by CApplicationAPI.ApplicationHandler
        /// </summary>
        /// <param name="nEventID"></param>
        /// <param name="strData"></param>
        public static void NavHandler(int nEventID, IntPtr strData)
        {
            //if NotificationEvent is not null (at the begining attached) will call our event handler
            if (NavigationNotificationEvent != null)
            {
                NavigationNotificationEvent(nEventID, strData);
            }
        }

        #region Start and stop Drive

        /// <summary>
        /// Starting Drive
        /// </summary>
        public static int StartDrive(string path_in, params int[] driveWindowParameters_in)
        {
            //checking for walid file
            if (!File.Exists(path_in))
            {
                NotifyMainWindow("The given path \"{0}\" does not exist", path_in);
                return 0;
            }
            if (CheckWindowPropertyParameters(driveWindowParameters_in) == 0)
            {
                return 0;
            }
            myDrivePath = path_in;
            myDriveWindowParameters = driveWindowParameters_in;
            int aDriveStartedValue = StartDrivePrivate();
            if (aDriveStartedValue == 1)
            {
                NotifyMainWindow("Drive started successfully");
            }
            else
            {
                NotifyMainWindow("Drive does not start, try another path!");
            }
            return aDriveStartedValue;
        }

        /// <summary>
        /// This method stops the communication with Drive and closes Drive's API
        /// </summary>
        /// <param name="timeout_in"></param>
        public static void StopDrive(int timeout_in)
        {
            EndApplication(timeout_in);
            CloseApi();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool CloseApi()
        {
            bool aREsult;
            if (IsApplicationRunning)
            {
                NotifyMainWindow("CloseApi() was called, there is no connection with Drive.");
                aREsult = true;
            }
            else
            {
                NotifyMainWindow("CloseApi() was called, but Drive is not running");
                aREsult = false;
            }
            CApplicationAPI.CloseApi();
            return aREsult;
        }

        public static bool EndApplication(int timeout_in)
        {
            bool aREsult;
            if (IsApplicationRunning)
            {

                NotifyMainWindow("EndApplication() was called. Communication is still alive");
                aREsult = true;
            }
            else
            {
                NotifyMainWindow("EndApplication() was called, but Drive is not running");
                aREsult = false;
            }

            CApplicationAPI.EndApplication(out mySerror, timeout_in);

            return aREsult;
        }

        public static bool IsApplicationRunning
        {
            get { return CApplicationAPI.IsApplicationRunning(myTimeout) == 1; }
        }

        public static void CkeckIfApplicationIsRunning()
        {
            if (IsApplicationRunning)
            {
                NotifyMainWindow("Drive is running");
            }
            else
            {
                NotifyMainWindow("Drive is not running");
            }
        }

        #endregion Start and stop Drive

        /// <summary>
        /// Sends the Drive to foreground and checks for error
        /// </summary>
        /// <param name="timeout_in"></param>
        public static int SendDriveToForeground(int timeout_in)
        {
            int aResult = 0;
            try
            {
                CApplicationAPI.BringApplicationToForeground(out mySerror, timeout_in);
                if (!CheckSerror())
                {
                    NotifyMainWindow("Drive sent to foreground successfully");
                }
            }
            catch
            { }
            return aResult;
        }

        /// <summary>
        /// Sends the Drive to background and checks for error
        /// </summary>
        /// <param name="timeout_in"></param>
        public static int SendDriveToBackground(int timeout_in)
        {
            int aResult = CApplicationAPI.BringApplicationToBackground(out mySerror, timeout_in);
            if (!CheckSerror())
            {
                NotifyMainWindow("Drive sent to background successfully");
            }
            return aResult;
        }

        /// <summary>
        /// Checking SError content. If there was a global error, the main window will be notificated
        /// </summary>
        /// <param name="sError_in"></param>
        /// <returns>true if there was an error</returns>
        public static bool CheckSerror(SError sError_in)
        {
            if (sError_in.nCode != 1)
            {
                string anErrorString;
                switch (sError_in.nCode)
                {
                    case 0: anErrorString = "Function not succeeded."; break;
                    case 2: anErrorString = "Drive not succeeded."; break;
                    case 3: anErrorString = "Function reached timeout."; break;
                    default: anErrorString = sError_in.GetDescription(); break;
                }
                if (NotificationEvent != null)
                {
                    NotificationEvent(anErrorString);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checking the neccessary *.dlls for running Drive in Windows folder
        /// </summary>
        public static void CheckDlls()
        {
            string[] aDllPaths = new string[] { @"c:\WINDOWS\ApplicationAPI.dll", @"c:\WINDOWS\sdkdriver.dll" };

            foreach (string s in aDllPaths)
            {
                if (!File.Exists(s))
                {
                    NotifyMainWindow("File \"{0}\" does NOT exist!", s);
                }
                else
                {
                    NotifyMainWindow("File \"{0}\" exists!", s);
                }
            }
        }

        /// <summary>
        /// Gets the device unique ID and sends notification to the main window
        /// </summary>
        public static void GetDeviceID()
        {
            string aUniqueDeviceID_out;
            if (CApplicationAPI.GetUniqueDeviceId(out mySerror, out aUniqueDeviceID_out, myTimeout) == 1)
            {
                NotifyMainWindow("Your unique device ID is: {0}", aUniqueDeviceID_out);
            }
            else
            {
                CheckSerror();
            }
        }

        /// <summary>
        /// Changing window size (left top right bottom)
        /// </summary>
        /// <param name="p"></param>
        public static void ChangeWindowSize(int[] p)
        {
            if (CheckWindowPropertyParameters(p) == 1)
            {
                if (CApplicationAPI.ChangeAppRectangle(out mySerror,
                    p[0], p[1], p[2], p[3], myTimeout) == 1)
                {
                    NotifyMainWindow("Drive window's rectangle has been changed");
                }
                else
                {
                    CheckSerror();
                }
            }
        }

        /// <summary>
        /// Shows message in Drive, user reply is set to OK, it can be Yes/No, Ok/Cancel and only OK
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p_2"></param>
        public static void ShowMessage(string messageToShow_in, bool bringApplicationToForeground_in)
        {
            int aUserReply = 1;
            if (CApplicationAPI.ShowMessage(out mySerror, messageToShow_in, 1, true, bringApplicationToForeground_in, ref aUserReply, myTimeout) == 1)
            {
                NotifyMainWindow("Message shown via ShowMessage() function!");
            }
            else
            {
                CheckSerror();
            }
        }

        /// <summary>
        /// Message is shown in the upper right corner, there is no user interaction needed
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p_2"></param>
        public static void FlashMessage(string p, bool p_2)
        {
            if (CApplicationAPI.FlashMessage(out mySerror, p, p_2, myTimeout) == 1)
            {
                NotifyMainWindow("Message shown via FlasMessage() function!");
            }
            else
            {
                CheckSerror();
            }
        }

        /// <summary>
        /// Notifies about the selected ISO's version
        /// </summary>
        /// <param name="s_in">Country's ISO</param>
        public static void GetMapVersion(string s_in)
        {
            string aMapVersion;
            CApplicationAPI.GetMapVersion(out mySerror, s_in, out aMapVersion, myTimeout);
            if (!CheckSerror())
            {
                NotifyMainWindow("Map version of {1} is {0}", aMapVersion, s_in);
            }
        }

        /// <summary>
        /// In the case, that there is a valid *.mlm file Drive switches to that map
        /// </summary>
        /// <param name="path_in">path to *.mlm file</param>
        public static void SwitchMap(string path_in)
        {
            if (!File.Exists(path_in))
            {
                NotifyMainWindow("The given path \"{0}\" does not exist", path_in);
                return;
            }
            if (CApplicationAPI.SwitchMap(out mySerror, path_in, myTimeout) != 1)
            {
                CheckSerror();
            }
            else
            {
                NotifyMainWindow("Map switched correctly");
            }
        }

        /// <summary>
        /// Changes application options by giving a reference to instance of SChangeOption, it is an inout parameter  
        /// </summary>
        /// <param name="SChangeOption_inout"></param>
        public static void ChangeApplicationOptions(ref SChangeOption SChangeOption_inout)
        {
            if (CApplicationAPI.ChangeApplicationOptions(out mySerror, ref SChangeOption_inout, myTimeout) == 1)
            {
                NotifyMainWindow("Application options changed");
            }
            else
            {
                CheckSerror();
            }
        }

        /// <summary>
        /// When there is a GPS device, or a *.nmea file is opened, Drive can give us real GPS position
        /// </summary>
        /// <param name="SGpsPosition_out"></param>
        public static void GetActualGPSPosition(out SGpsPosition SGpsPosition_out)
        {
            if (CApplicationAPI.GetActualGpsPosition(out mySerror, out SGpsPosition_out, true, myTimeout) == 1)
            {
                NotifyMainWindow("Actual GPS information got");
            }
            else
            {
                CheckSerror();
            }
        }

        /// <summary>
        /// The function enables/disables to send GPS data by SendGpsData SDK function.
        /// </summary>
        /// <param name="b_in"></param>
        public static void EnableOrDisableExternalInput(bool b_in)
        {
            if (b_in)
            {
                if (CApplicationAPI.EnableExternalGpsInput(out mySerror, myTimeout) == 1)
                {
                    NotifyMainWindow("External GPS input enabled.");
                }
                else
                {
                    CheckSerror();
                }
            }
            else
            {
                if (CApplicationAPI.DisableExternalGpsInput(out mySerror, myTimeout) == 1)
                {
                    NotifyMainWindow("External GPS input disabled.");
                }
                else
                {
                    CheckSerror();
                }
            }
        }

        /// <summary>
        /// Converts an adress to a nearby GPS location represented in LONGPOSITION
        /// </summary>
        /// <param name="longPosition_out">the result of conversion</param>
        /// <param name="strAddress_in">the address that user wants to convert</param>
        /// <param name="isPostal_in">if true, city's name is required in postal code (e.g. not it's name. This works only in few countries)
        /// <param name="valueMatch_in">if true, fuzzy search is on</param>
        /// <returns>1 if successful, other value otherwise</returns>
        public static int LocationFromAddress(out LONGPOSITION longPosition_out, string strAddress_in, bool isPostal_in, bool valueMatch_in)
        {
            int aResult;
            //creating location from address represented in string
            //mySerror - error
            //position 
            //location in string
            //postal == false, we got city's name in string not it's postal code
            //valueMatch == true, fuzzy searching is on
            //timeout set to infinity - search can take long time
            aResult = CApplicationAPI.LocationFromAddress(out mySerror, out longPosition_out, strAddress_in, isPostal_in, valueMatch_in, myTimeout);
            CheckSerror();
            return aResult;
        }

        /// <summary>
        /// Adds new itinerary
        /// </summary>
        /// <param name="Stoppoints_in">all stop points from start to destination via waypoints</param>
        /// <param name="itineraryName_in">the itinerary's name</param>
        public static void AddItinerary(SStopOffPoint[] Stoppoints_in, string itineraryName_in)
        {
            if (CApplicationAPI.AddItinerary(out mySerror, Stoppoints_in, itineraryName_in, myTimeout) == 1)
            {
                NotifyMainWindow("Itinerary \"{0}\" was created successfully", itineraryName_in);
            }
            CheckSerror();
        }

        /// <summary>
        /// If itinerary is set, route can be computed
        /// </summary>
        /// <param name="itineraryName_in">itinerary's name</param>
        /// <param name="flags_in">some restrictions</param>
        /// <param name="showApplication_in">true if user wants to bring Drive to foreground</param>
        public static void SetRoute(string itineraryName_in, int flags_in, bool showApplication_in)
        {
            if (CApplicationAPI.SetRoute(out mySerror, itineraryName_in, flags_in, showApplication_in, myTimeout) == 1)
            {
                NotifyMainWindow("Route for \"{0}\" was set successfully", itineraryName_in);
            }
            CheckSerror();
        }

        /// <summary>
        /// Gets route info, if there is a set route
        /// </summary>
        /// <param name="SRouteInfo_out"></param>
        /// <param name="maxTime_in"></param>
        public static void GetRouteInfo(out SRouteInfo SRouteInfo_out, int maxTime_in)
        {
            CApplicationAPI.GetRouteInfo(out mySerror, out SRouteInfo_out, maxTime_in);
            CheckSerror();
        }

        /// <summary>
        /// Stops navigation
        /// </summary>
        public static void StopNavigation()
        {
            if (CApplicationAPI.StopNavigation(out mySerror, myTimeout) == 1)
            {
                NotifyMainWindow("Navigation was stopped!");
            }
            else
            {
                CheckSerror();
            }
        }

        /// <summary>
        /// Navigates the GPS to a real address. If there is no valid GPS signal, a start point is set from the map
        /// </summary>
        /// <param name="address_in">address where user wants to go</param>
        /// <param name="postal_in">if true, city's postal code has to be given not its name</param>
        /// <param name="flags_in">some restrictions</param>
        /// <param name="showApplication_in">true if user wants to bring Drive to foreground</param>
        public static void NavigateToAddress(string address_in, bool postal_in, int flags_in, bool showApplication_in)
        {
            if (CApplicationAPI.NavigateToAddress(out mySerror, address_in, postal_in, flags_in, showApplication_in, myTimeout) == 1)
            {
                NotifyMainWindow("Navigate to address");
            }
            else
            {
                CheckSerror();
            }
        }

        /// <summary>
        /// Adds one item/stop to itinerary
        /// </summary>
        /// <param name="itineraryName_in">selects the choosen itinerary</param>
        /// <param name="SStopOffPoint_inout">one stop point</param>
        /// <param name="index_in">index where the new item should be put. if the index value is out of range, item is automatically added as the last one</param>
        public static void AddEntryToItinerary(string itineraryName_in, ref SStopOffPoint SStopOffPoint_inout, int index_in)
        {
            if (CApplicationAPI.AddEntryToItinerary(out mySerror, itineraryName_in, ref SStopOffPoint_inout, index_in, myTimeout) == 1)
            {
                NotifyMainWindow("New entries added to itinerary \"{0}\" successfully", itineraryName_in);
            }
            else
            {
                CheckSerror();
            }
        }

        /// <summary>
        /// Calls the GetItineraryList method which gives us an SStioOffPoint array
        /// </summary>
        /// <param name="itineraryName_in">the itinerary name user is interested in</param>
        /// <param name="SStoppofpoints_out">the waypoint array</param>
        public static void GetItineraryList(string itineraryName_in, out SStopOffPoint[] SStoppofpoints_out)
        {
            if (CApplicationAPI.GetItineraryList(out mySerror, itineraryName_in, out SStoppofpoints_out, myTimeout) == 1)
            {
                NotifyMainWindow("Got list of stops for itinerary \"{0}\"", itineraryName_in);
            }
            CheckSerror();
        }

        /// <summary>
        /// Deletes te given itinerary
        /// </summary>
        /// <param name="itineraryName_in"></param>
        public static void DeleteItinerary(string itineraryName_in)
        {
            if (CApplicationAPI.DeleteItinerary(out mySerror, itineraryName_in, myTimeout) == 1)
            {
                NotifyMainWindow("Itinerary \"{0}\" was deleted successfully", itineraryName_in);
            }
            else
            {
                CheckSerror();
            }
        }

        /// <summary>
        /// Deletes only one entry, given by index in a concrete itinerary
        /// </summary>
        /// <param name="itineraryName_in"></param>
        /// <param name="index_in"></param>
        public static void DeleteItineraryEntry(string itineraryName_in, int index_in)
        {
            if (CApplicationAPI.DeleteEntryInItinerary(out mySerror, itineraryName_in, index_in, myTimeout) == 1)
            {
                NotifyMainWindow("An entry from itinerary \"{0}\" was deleted", itineraryName_in);
            }
            CheckSerror();
        }

        #endregion Public methods

        #region Private methods

        /// <summary>
        /// Starting Drive with the proper parameters
        /// </summary>
        private static int StartDrivePrivate()
        {
            CApplicationAPI.ApplicationHandler AppHnd = new CApplicationAPI.ApplicationHandler(NavHandler);
            return CApplicationAPI.InitApi(myDrivePath, AppHnd, myDriveWindowParameters[0], myDriveWindowParameters[1], myDriveWindowParameters[2], myDriveWindowParameters[3]);

        }

        /// <summary>
        /// Checking error in this class
        /// </summary>
        private static bool CheckSerror()
        {
            return CheckSerror(mySerror);
        }

        /// <summary>
        /// Sends a NotificationEvent, which can be handled to write out its content
        /// </summary>
        /// <param name="s"></param>
        private static void NotifyMainWindow(string s)
        {
            if (NotificationEvent != null)
            {
                NotificationEvent(s);
            }
        }

        private static void NotifyMainWindow(string s_in, params object[] someStaff_in)
        {
            NotifyMainWindow(string.Format(s_in, someStaff_in));
        }

        private static int CheckWindowPropertyParameters(params int[] windowProperties_in)
        {
            //checking for walid number of parameters
            if (windowProperties_in.Length < 4)
            {
                NotifyMainWindow("Not enugh parameters for window position");
                return 0;
            }
            if (windowProperties_in.Length > 4)
            {
                NotifyMainWindow("Too many parameters for window position");
                return 0;
            }
            return 1;
        }

        #endregion Private methods


        internal static void OnMenuCommand(int p, int p_2)
        {
            CApplicationAPI.OnMenuCommand(out mySerror, p, p_2, true, myTimeout);
            CheckSerror();
        }
    }
}
