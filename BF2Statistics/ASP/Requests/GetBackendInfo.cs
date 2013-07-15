using System;

namespace BF2Statistics.ASP.Requests
{
    class GetBackendInfo
    {
        public GetBackendInfo(ASPResponse Response)
        {
            // Add timestamp and version info
            FormattedOutput Output = new FormattedOutput("ver", "now");
            Output.AddRow("0.1", DateTime.UtcNow.ToUnixTimestamp());
            Response.AddData(Output);

            // Next list each Unlock
            Output = new FormattedOutput("id", "kit", "name", "descr");
            Output.AddRow(11, 0, "Chsht_protecta", "Protecta shotgun with slugs");
            Output.AddRow(22, 1, "Usrif_g3a3", "H&K G3");
            Output.AddRow(33, 2, "USSHT_Jackhammer", "Jackhammer shotgun");
            Output.AddRow(44, 3, "Usrif_sa80", "SA-80");
            Output.AddRow(55, 4, "Usrif_g36c", "G36C");
            Output.AddRow(66, 5, "RULMG_PKM", "PKM");
            Output.AddRow(77, 6, "USSNI_M95_Barret", "Barret M82A2 (.50 cal rifle)");
            Output.AddRow(88, 1, "sasrif_fn2000", "FN2000");
            Output.AddRow(99, 2, "sasrif_mp7", "MP-7");
            Output.AddRow(111, 3, "sasrif_g36e", "G36E");
            Output.AddRow(222, 4, "usrif_fnscarl", "FN SCAR - L");
            Output.AddRow(333, 5, "sasrif_mg36", "MG36");
            Output.AddRow(444, 0, "eurif_fnp90", "P90");
            Output.AddRow(555, 6, "gbrif_l96a1", "L96A1");
            Response.AddData(Output);

            // Send Response to browser
            Response.Send();
        }
    }
}
