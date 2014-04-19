using System;

namespace BF2Statistics.ASP.Requests
{
    class GetBackendInfo
    {
        /// <summary>
        /// This request provides details of the backend version, and lists the unlocks
        /// </summary>
        /// <param name="Client">The HttpClient who made the request</param>
        /// <param name="Driver">The Stats Database Driver. Connection errors are handled in the calling object</param>
        public GetBackendInfo(HttpClient Client)
        {
            // Add timestamp and version info
            Client.Response.WriteResponseStart();
            Client.Response.WriteHeaderLine("ver", "now");
            Client.Response.WriteDataLine("0.1", DateTime.UtcNow.ToUnixTimestamp());

            // Next list each Unlock
            Client.Response.WriteHeaderLine("id", "kit", "name", "descr");
            Client.Response.WriteDataLine(11, 0, "Chsht_protecta", "Protecta shotgun with slugs");
            Client.Response.WriteDataLine(22, 1, "Usrif_g3a3", "H&K G3");
            Client.Response.WriteDataLine(33, 2, "USSHT_Jackhammer", "Jackhammer shotgun");
            Client.Response.WriteDataLine(44, 3, "Usrif_sa80", "SA-80");
            Client.Response.WriteDataLine(55, 4, "Usrif_g36c", "G36C");
            Client.Response.WriteDataLine(66, 5, "RULMG_PKM", "PKM");
            Client.Response.WriteDataLine(77, 6, "USSNI_M95_Barret", "Barret M82A2 (.50 cal rifle)");
            Client.Response.WriteDataLine(88, 1, "sasrif_fn2000", "FN2000");
            Client.Response.WriteDataLine(99, 2, "sasrif_mp7", "MP-7");
            Client.Response.WriteDataLine(111, 3, "sasrif_g36e", "G36E");
            Client.Response.WriteDataLine(222, 4, "usrif_fnscarl", "FN SCAR - L");
            Client.Response.WriteDataLine(333, 5, "sasrif_mg36", "MG36");
            Client.Response.WriteDataLine(444, 0, "eurif_fnp90", "P90");
            Client.Response.WriteDataLine(555, 6, "gbrif_l96a1", "L96A1");

            // Send Response to browser
            Client.Response.Send();
        }
    }
}
