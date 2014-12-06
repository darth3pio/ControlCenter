using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;
using BF2Statistics.Utilities;
using BF2Statistics.Database;

namespace BF2Statistics.Gamespy
{
    /// <summary>
    /// Gamespy Client Manager
    /// This class is used to proccess the client login process,
    /// create new user accounts, and fetch profile information
    /// <remarks>gpcm.gamespy.com</remarks>
    /// </summary>
    public class GpcmClient : IDisposable
    {
        #region Variables

        /// <summary>
        /// The current step of the login proccess
        /// </summary>
        private int Step = 0;

        /// <summary>
        /// The connected clients Nick
        /// </summary>
        public string ClientNick { get; protected set; }

        /// <summary>
        /// The connected clients Player Id
        /// </summary>
        public int ClientPID { get; protected set; }

        /// <summary>
        /// Defines whether we need to log a disconnect
        /// </summary>
        private bool ConnectionLogged = false;

        /// <summary>
        /// Clients table data from the gamespy database
        /// </summary>
        private Dictionary<string, object> User;

        /// <summary>
        /// The users session key
        /// </summary>
        private ushort SessionKey;

        /// <summary>
        /// The Clients generated proof string, used for checking password validity.
        /// This is used as part of the hash used to "prove" to the client
        /// that the password in our database matches what the user enters
        /// </summary>
        private string ClientChallengeKey;

        /// <summary>
        /// The Servers challange key, sent when the client first connects.
        /// This is used as part of the hash used to "proove" to the client
        /// that the password in our database matches what the user enters
        /// </summary>
        private string ServerChallengeKey;

        /// <summary>
        /// The Clients response key. The is the expected Hash value for the server
        /// when generating the client proof string
        /// </summary>
        private string ClientResponseKey;

        /// <summary>
        /// Variable that determines if the client is disconnected,
        /// and this object can be cleared from memory
        /// </summary>
        public bool Disposed { get; protected set; }

        /// <summary>
        /// The clients socket network stream
        /// </summary>
        private TcpClientStream Stream;

        /// <summary>
        /// The TcpClient for our connection
        /// </summary>
        private TcpClient Connection;

        /// <summary>
        /// The TcpClient's Endpoint
        /// </summary>
        private EndPoint ClientEP;

        /// <summary>
        /// The Connected Clients IpAddress
        /// </summary>
        public IPAddress IpAddress = null;

        /// <summary>
        /// A random... random
        /// </summary>
        private Random RandInstance = new Random((int)DateTime.Now.Ticks);

        /// <summary>
        /// Our CRC16 object for generating Checksums
        /// </summary>
        protected static Crc16 Crc;

        /// <summary>
        /// Our MD5 Object
        /// </summary>
        protected static MD5 CreateMD5;

        /// <summary>
        /// A literal backslash
        /// </summary>
        private const char Backslash = '\\';

        /// <summary>
        /// Array of characters used in generating a signiture
        /// </summary>
        private static char[] AlphaChars = {
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
                'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
            };

        /// <summary>
        /// An array of Alpha Numeric characters used in generating a random string
        /// </summary>
        private static char[] AlphaNumChars = { 
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
                'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
                'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
            };

        /// <summary>
        /// Array of Hex cahracters, with no leading 0
        /// </summary>
        private static char[] HexChars = {
                '1', '2', '3', '4', '5', '6', '7', '8', '9',
                'a', 'b', 'c', 'd', 'e', 'f'
            };

        /// <summary>
        /// Gamespy password hash table (Byte To Hex)
        /// </summary>
        private static string[] BtoH = { 
                "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0a", "0b", "0c", "0d", "0e", "0f",
                "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1a", "1b", "1c", "1d", "1e", "1f",
                "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "2a", "2b", "2c", "2d", "2e", "2f",
                "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "3a", "3b", "3c", "3d", "3e", "3f",
                "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "4a", "4b", "4c", "4d", "4e", "4f",
                "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "5a", "5b", "5c", "5d", "5e", "5f",
                "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "6a", "6b", "6c", "6d", "6e", "6f",
                "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "7a", "7b", "7c", "7d", "7e", "7f",
                "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "8a", "8b", "8c", "8d", "8e", "8f",
                "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "9a", "9b", "9c", "9d", "9e", "9f",
                "a0", "a1", "a2", "a3", "a4", "a5", "a6", "a7", "a8", "a9", "aa", "ab", "ac", "ad", "ae", "af",
                "b0", "b1", "b2", "b3", "b4", "b5", "b6", "b7", "b8", "b9", "ba", "bb", "bc", "bd", "be", "bf",
                "c0", "c1", "c2", "c3", "c4", "c5", "c6", "c7", "c8", "c9", "ca", "cb", "cc", "cd", "ce", "cf",
                "d0", "d1", "d2", "d3", "d4", "d5", "d6", "d7", "d8", "d9", "da", "db", "dc", "dd", "de", "df",
                "e0", "e1", "e2", "e3", "e4", "e5", "e6", "e7", "e8", "e9", "ea", "eb", "ec", "ed", "ee", "ef",
                "f0", "f1", "f2", "f3", "f4", "f5", "f6", "f7", "f8", "f9", "fa", "fb", "fc", "fd", "fe", "ff" 
            };

        /// <summary>
        /// An Event that is fired when the client successfully logs in.
        /// </summary>
        public static event ConnectionUpdate OnSuccessfulLogin;

        /// <summary>
        /// Event fired when that remote connection logs out, or
        /// the socket gets disconnected. This event will not fire
        /// unless OnSuccessfulLogin event was fired first.
        /// </summary>
        public static event GpcmConnectionClosed OnDisconnect;

        #endregion Variables

        /// <summary>
        /// Static Construct. Builds the Crc table uesd in the login process
        /// </summary>
        static GpcmClient()
        {
            Crc = new Crc16(Crc16Mode.Standard);
            CreateMD5 = MD5.Create();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Client">The Tcp Client connection</param>
        public GpcmClient(TcpClient Client)
        {
            // Set default variable values
            ClientNick = "Connecting...";
            Connection = Client;
            ClientEP = Connection.Client.RemoteEndPoint;
            IpAddress = ((IPEndPoint)ClientEP).Address;
            Disposed = false;

            // Create our Client Stream
            Stream = new TcpClientStream(Client);
            Stream.OnDisconnect += new ConnectionClosed(Stream_OnDisconnect);
            Stream.DataReceived += new DataRecivedEvent(Stream_DataReceived);

            // Start by sending the server challenge
            SendServerChallenge();
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~GpcmClient()
        {
            if (!Disposed)
                this.Dispose();
        }

        /// <summary>
        /// Disposes of the client object. The connection is no longer
        /// closed here and the Disconnect even is NO LONGER fired
        /// </summary>
        public void Dispose()
        {
            // Preapare to be unloaded from memory
            this.Disposed = true;
        }

        /// <summary>
        /// Logs the client out of the game client, and closes the stream
        /// </summary>
        public void Disconnect(int where)
        {
            // Console.WriteLine("Logout Called from: " + where);
            // LoginServer.Database.Execute("UPDATE accounts SET session=0 WHERE id=" + ClientPID);
            // If connection is still alive, disconnect user
            try
            {
                if (Connection.Client.IsConnected())
                {
                    Stream.IsClosing = true;
                    Connection.Close();
                }
            }
            catch { }

            // Log
            if (ConnectionLogged)
                LoginServer.Log("[GPCM] Client Disconnected: {0} {1} {2}", ClientNick, ClientPID, ClientEP);

            // Call disconnect event
            if (OnDisconnect != null)
                OnDisconnect(this);
        }

        #region Stream Callbacks

        /// <summary>
        /// Main listner loop. Keeps an open stream between the client and server while
        /// the client is logged in / playing
        /// </summary>
        private void Stream_DataReceived(string message)
        {
            // Read client message, and parse it into key value pairs
            string[] recieved = message.TrimStart(Backslash).Split(Backslash);
            Dictionary<string, string> Recv = ConvertToKeyValue(recieved);

            // Switch by task
            switch (recieved[0])
            {
                case "newuser":
                    HandleNewUser(Recv);
                    Step++;
                    break;
                case "login":
                    ProcessLogin(Recv, message);
                    Step++;
                    break;
                case "getprofile":
                    if (Step < 2)
                    {
                        SendProfile(false);
                        Step++;
                    }
                    else
                        SendProfile(true);
                    break;
                case "updatepro":
                    UpdateUser(Recv);
                    Step++;
                    break;
                case "logout":
                    Disconnect(0);
                    break;
                default:
                    LoginServer.Log("Unkown Message Passed: {0}", message);
                    break;
            }
        }

        /// <summary>
        /// Main loop for handling the client stream
        /// </summary>
        private void Stream_OnDisconnect()
        {
            Disconnect(8);
        }

        #endregion Stream Callbacks

        #region Login Steps

        /// <summary>
        ///  This method starts off by sending a random string 10 characters
        ///  in length, known as the Server challenge key. This is used by 
        ///  the client to return a client challenge key, which is used
        ///  to validate login information later.
        ///  </summary>
        public void SendServerChallenge()
        {
            // First we need to create a random string the length of 10 characters
            StringBuilder Temp = new StringBuilder();
            for (int i = 0; i < 10; i++)
                Temp.Append(AlphaChars[RandInstance.Next(AlphaChars.Length)]);

            // Next we send the client the challenge key
            ServerChallengeKey = Temp.ToString();
            Stream.Send("\\lc\\1\\challenge\\{0}\\id\\1\\final\\", ServerChallengeKey);
        }

        /// <summary>
        /// This method verifies the login information sent by
        /// the client, and returns encrypted data for the client
        /// to verify as well
        /// </summary>
        public void ProcessLogin(Dictionary<string, string> Recv, string Message)
        {
            // Set instance variables now that we know who's connected
            try
            {
                ClientNick = Recv["uniquenick"];
                ClientChallengeKey = Recv["challenge"];
                ClientResponseKey = Recv["response"];
            }
            catch (KeyNotFoundException)
            {
                Program.ErrorLog.Write("A KeyNotFoundException occured during the login process! Query was: " + Message);
                Stream.Send("\\error\\\\err\\265\\fatal\\\\errmsg\\The uniquenick provided is incorrect!\\id\\1\\final\\");
                Disconnect(2);
                return;
            }

            // Dispose connection after use
            try
            {
                using (GamespyDatabase Conn = new GamespyDatabase())
                {
                    // Get user data from database
                    User = Conn.GetUser(ClientNick);
                    if (User == null)
                    {
                        Stream.Send("\\error\\\\err\\265\\fatal\\\\errmsg\\The uniquenick provided is incorrect!\\id\\1\\final\\");
                        Disconnect(2);
                        return;
                    }

                    // Set client PID var
                    ClientPID = Int32.Parse(User["id"].ToString());

                    // Use the GenerateProof method to compare with the "response" value. This validates the given password
                    if (ClientResponseKey == GenerateProof(ClientChallengeKey, ServerChallengeKey))
                    {
                        // Create session key
                        SessionKey = Crc.ComputeChecksum(ClientNick);

                        // Password is correct
                        Stream.Send(
                            "\\lc\\2\\sesskey\\{0}\\proof\\{1}\\userid\\{2}\\profileid\\{2}\\uniquenick\\{3}\\lt\\{4}__\\id\\1\\final\\",
                            SessionKey,
                            GenerateProof(ServerChallengeKey, ClientChallengeKey), // Do this again, Params are reversed!
                            ClientPID,
                            ClientNick,
                            GenerateRandomString(22) // Generate LT whatever that is (some sort of random string, 22 chars long)
                        );

                        // Log Incoming Connections
                        LoginServer.Log("[GPCM] Client Login: {0} {1} {2}", ClientNick, ClientPID, ClientEP);
                        ConnectionLogged = true;

                        // Call successful login event
                        OnSuccessfulLogin(this);
                        Conn.Execute("UPDATE accounts SET lastip=@P0 WHERE id=@P1", IpAddress, ClientPID);
                    }
                    else
                    {
                        // Log Incoming Connections
                        LoginServer.Log("[GPCM] Failed Login Attempt: {0} {1} {2}", ClientNick, ClientPID, ClientEP);

                        // Password is incorrect with database value
                        Stream.Send("\\error\\\\err\\260\\fatal\\\\errmsg\\The password provided is incorrect.\\id\\1\\final\\");
                        Disconnect(3);
                    }
                }
            }
            catch
            {
                Disconnect(4);
                return;
            }
        }

        /// <summary>
        /// This method is called when the client requests for the Account profile
        /// </summary>
        /// <param name="retrieve">Determines the return ID</param>
        private void SendProfile(bool retrieve)
        {
            Stream.Send(
                "\\pi\\\\profileid\\{0}\\nick\\{1}\\userid\\{0}\\email\\{2}\\sig\\{3}\\uniquenick\\{1}\\pid\\0\\firstname\\\\lastname\\" +
                "\\countrycode\\{4}\\birthday\\16844722\\lon\\0.000000\\lat\\0.000000\\loc\\\\id\\{5}\\final\\",
                User["id"], ClientNick, User["email"], GenerateSig(), User["country"], (retrieve ? "5" : "2")
            );
        }

        #endregion Steps

        #region User Methods

        /// <summary>
        /// Whenever the "newuser" command is recieved, this method is called to
        /// add the new users information into the database
        /// </summary>
        /// <param name="Recv">Array of parms sent by the server</param>
        private void HandleNewUser(Dictionary<string, string> Recv)
        {
            string Nick = Recv["nick"];
            string Email = Recv["email"];

            // Make sure the user doesnt exist already
            try
            {
                using (GamespyDatabase Conn = new GamespyDatabase())
                {
                    if (Conn.UserExists(Nick))
                    {
                        Stream.Send("\\error\\\\err\\516\\fatal\\\\errmsg\\This account name is already in use!\\id\\1\\final\\");
                        Disconnect(5);
                        return;
                    }

                    // We need to decode the Gamespy specific encoding for the password
                    string Password = GamespyUtils.DecodePassword(Recv["passwordenc"]);
                    bool result = Conn.CreateUser(Nick, Password, Email, "US");
                    User = Conn.GetUser(Nick);

                    // Fetch the user to make sure we are good
                    if (!result || User == null)
                    {
                        Stream.Send("\\error\\\\err\\516\\fatal\\\\errmsg\\Error creating account!\\id\\1\\final\\");
                        Disconnect(6);
                        return;
                    }

                    Stream.Send("\\nur\\\\userid\\{0}\\profileid\\{0}\\id\\1\\final\\", User["id"]);
                }
            }
            catch
            {
                Disconnect(7);
                return;
            }
        }


        /// <summary>
        /// Updates the Users Country code when sent by the client
        /// </summary>
        /// <param name="recv">Array of information sent by the server</param>
        private void UpdateUser(Dictionary<string, string> Recv)
        {
            // Set clients country code
            try
            {
                using (GamespyDatabase Conn = new GamespyDatabase())
                    Conn.UpdateUser(ClientNick, Recv["countrycode"]);
            }
            catch
            {
                //Dispose();
            }
        }

        #endregion

        #region Misc Methods

        /// <summary>
        /// Converts a recived parameter array from the client string to a keyValue pair dictionary
        /// </summary>
        /// <param name="parts">The array of data from the client</param>
        /// <returns></returns>
        private Dictionary<string, string> ConvertToKeyValue(string[] parts)
        {
            Dictionary<string, string> Dic = new Dictionary<string, string>();
            for (int i = 2; i < parts.Length; i += 2)
            {
                if (!Dic.ContainsKey(parts[i]))
                    Dic.Add(parts[i], parts[i + 1]);
            }

            return Dic;
        }

        /// <summary>
        /// Generates an encrypted reponse to return to the client, which verifies
        /// the clients account information, and login info
        /// </summary>
        /// <param name="challenge1">First challenge key</param>
        /// <param name="challenge2">Second challenge key</param>
        /// <returns>Encrypted account info / Login verification</returns>
        private string GenerateProof(string challenge1, string challenge2)
        {
            // Prepare variables
            StringBuilder Response = new StringBuilder();
            StringBuilder PassHash = new StringBuilder();
            byte[] Md5Hash;

            // Convert MD5 password bytes to hex characters
            Md5Hash = CreateMD5.ComputeHash(Encoding.ASCII.GetBytes(User["password"].ToString()));
            foreach (byte b in Md5Hash)
                PassHash.Append(BtoH[b]);

            // Generate our string to be hashed
            string pHash = PassHash.ToString();
            PassHash.Append(' ', 48); // 48 spaces
            PassHash.Append(String.Concat(ClientNick, challenge1, challenge2, pHash));

            // Create our final MD5 hash, and convert all the bytes to hex yet again
            Md5Hash = CreateMD5.ComputeHash(Encoding.ASCII.GetBytes(PassHash.ToString()));
            foreach (byte b in Md5Hash)
                Response.Append(BtoH[b]);

            return Response.ToString();
        }

        /// <summary>
        /// Generates a random alpha-numeric string
        /// </summary>
        /// <param name="length">The lenght of the string to be generated</param>
        /// <returns></returns>
        private string GenerateRandomString(int length)
        {
            StringBuilder Response = new StringBuilder();
            for (int i = 0; i < length; i++)
                Response.Append(AlphaNumChars[RandInstance.Next(62)]);

            return Response.ToString();
        }

        /// <summary>
        /// Generates a random signature
        /// </summary>
        /// <returns></returns>
        private string GenerateSig()
        {
            StringBuilder Response = new StringBuilder();
            for (int length = 0; length < 32; length++)
                Response.Append(HexChars[RandInstance.Next(14)]);

            return Response.ToString();
        }

        #endregion
    }
}
