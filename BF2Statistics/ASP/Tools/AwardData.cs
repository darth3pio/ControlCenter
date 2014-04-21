using System;
using System.Collections.Generic;

namespace BF2Statistics.ASP
{
    class AwardData
    {
        public static Dictionary<string, int> Awards = new Dictionary<string, int>()
        {
            // Badges
            {"kcb", 1031406},   // Knife Combat Badge
            {"pcb", 1031619},   // Pistol Combat Badge
            {"Acb", 1031119},   // Assault Combat Badge
            {"Atcb", 1031120},  // Assault Combat Badge
            {"Sncb", 1031109},  // Sniper Combat Badge
            {"Socb", 1031115},  // Special Ops Combat Badge
            {"Sucb", 1031121},  // Support Combat Badge
            {"Ecb", 1031105},   // Engineer Combat Badge
            {"Mcb", 1031113},   // Medic Combat Badge
            {"Eob", 1032415},   // Explosive Ordinance Badge
            {"Fab", 1190601},   // First Aid Badge
            {"Eb", 1190507},    // Engineer Repair Badge
            {"Rb", 1191819},    // Resupply Badge
            {"Cb", 1190304},    // Command Badge
            {"Ab", 1220118},    // Armor Badge
            {"Tb", 1222016},    // Transport Badge
            {"Hb", 1220803},    // Helicopter Badge
            {"Avb", 1220122},   // Aviators Badge
            {"adb", 1220104},   // Air Defense Badge
            {"Swb", 1031923},   // Ground Defense Badge

            // Xpack Badges
            {"X1Acb", 1261119},     // Assault Specialist
            {"X1Atcb", 1261120},    // AntiTank Specialist
            {"X1Sncb", 1261109},    // Sniper Specialist
            {"X1Socb", 1261115},    // Special Ops Specialist
            {"X1Sucb", 1261121},    // Support Specialist
            {"X1Ecb", 1261105},     // Engineer Specialist
            {"X1Mcb", 1261113},     // Medical Specialist
            {"X1fbb", 1260602},     // Tactical Support Specialist
            {"X1ghb", 1260708},     // Grappling Hook Specialist
            {"X1zlb", 1262612},     // Zipline Specialist

            // Medals
            {"ph", 2191608},    // Purple Heart
            {"Msm", 2191319},   // Meritorious Service Medal
            {"Cam", 2190303},   // Combat Action Medal
            {"Acm", 2190309},   // Aviator Combat Medal
            {"Arm", 2190318},   // Armored Combat Medal
            {"Hcm", 2190308},   // Helicopter Combat Medal
            {"gcm", 2190703},   // Good Conduct Medal
            {"Cim", 2020903},   // Combat Infantry Medal
            {"Mim", 2020913},   // Marksman Infantry Medal
            {"Sim", 2020919},   // Sharpshooter Infantry Medal
            {"Mvn", 2021322},   // Medal of Valor
            {"Dsm", 2020419},   // Distinguished Service Medal
            {"pmm", 2021613},   // Peoples Medallion

            // Round Medals
            {"erg", 2051907},    // End of Round Gold
            {"ers", 2051919},    // End of Round Silver
            {"erb", 2051902},    // End of Round Bronze

            // Ribbons
            {"Car", 3240301},   // Combat Action Ribbon
            {"Mur", 3211305},   // Meritorious Unit Ribbon
            {"Ior", 3150914},   // Infantry Officer Ribbon
            {"Sor", 3151920},   // Staff Officer Ribbon
            {"Dsr", 3190409},   // Distingusihed Service Ribbon
            {"Wcr", 3242303},   // War College Ribbon
            {"Vur", 3212201},   // Valorous Unit Ribbon
            {"Lmr", 3241213},   // Legion Of Merrit
            {"Csr", 3190318},   // Crew Service Ribbon
            {"Arr", 3190118},   // Armored Ribbon
            {"Aer", 3190105},   // Aviator Ribbon
            {"Hsr", 3190803},   // Helicopter Service Ribbon
            {"Adr", 3040109},   // Airdefense Service Ribbon
            {"Gdr", 3040718},   // Ground Defense Service Ribbon
            {"Ar", 3240102},    // Airborne Ribbon
            {"gcr", 3240703},   // Good Conduct Ribbon

            // Xpack Ribbons
            {"X1Csr", 3260318},     // Crew Service Ribbon
            {"X1Arr", 3260118},     // Armored Service
            {"X1Aer", 3260105},     // Ariel Service
            {"X1Hsr", 3260803},     // Helo Specialist
        };

        public static List<BackendAward> BackendAwards = new List<BackendAward>()
        {
            // Middle Eastern Service Ribbon
            { new BackendAward(3191305, new AwardCriteria("maps", "count(mapid)", 7, "mapid IN (0,1,2,3,4,5,6) AND time >= 1")) },

            // Far East Service Ribbon
            { new BackendAward(3190605, new AwardCriteria("maps", "count(mapid)", 6, "mapid IN (100,101,102,103,105,601) AND time >= 1")) },

            // Navycross medal
            { new BackendAward(2021403, new AwardCriteria("army", "count(id)", 1, "time0 >= 360000*### AND best0 >= 100*### AND win0 >= 100*###")) },

            // Golden Scimitar
            { new BackendAward(2020719, new AwardCriteria("army", "count(id)", 1, "time1 >= 360000*### AND best1 >= 100*### AND win1 >= 100*###")) },

            // Peoples Madallion
            { new BackendAward(2021613, new AwardCriteria("army", "count(id)", 1, "time2 >= 360000*### AND best2 >= 100*### AND win2 >= 100*###")) },

            // European Union Service Medal
            { new BackendAward(2270521, new AwardCriteria("army", "count(id)", 1, "time9 >= 360000*### AND best9 >= 100*### AND win9 >= 100*###")) },

            // European Union Service Ribbon
            { new BackendAward(3270519, 
                // 1 second on each map
                new AwardCriteria("maps", "count(mapid)", 3, "mapid IN (10,11,110) AND time >= 1"),
                // 50 Hours played between the 3 maps
                new AwardCriteria("maps", "sum(time)", 180000, "mapid IN (10,11,110)"))
            },

            // North American Service Ribbon
            { new BackendAward(3271401,  
                // 1 second on each map
                new AwardCriteria("maps", "count(mapid)", 3, "mapid IN (200,201,202) AND time >= 1"),
                // 25 Hours played between the 3 maps
                new AwardCriteria("maps", "sum(time)", 90000, "mapid IN (200,201,202)"))
            },

            // Xpack //

            // Navy Seal Special Service Medal
            { new BackendAward(2261913, new AwardCriteria("army", "count(id)", 1, "time3 >= 180000*### AND best3 >= 100*### AND win3 >= 50*###")) },

            // SAS Special Service Medal
            { new BackendAward(2261919, new AwardCriteria("army", "count(id)", 1, "time4 >= 180000*### AND best4 >= 100*### AND win4 >= 50*###")) },

            // SPETZ Special Service Medal
            { new BackendAward(2261613, new AwardCriteria("army", "count(id)", 1, "time5 >= 180000*### AND best5 >= 100*### AND win5 >= 50*###")) },

            // MECSF Special Service Medal
            { new BackendAward(2261303, new AwardCriteria("army", "count(id)", 1, "time6 >= 180000*### AND best6 >= 100*### AND win6 >= 50*###")) },

            // Rebels Special Service Medal
            { new BackendAward(2261802, new AwardCriteria("army", "count(id)", 1, "time7 >= 180000*### AND best7 >= 100*### AND win7 >= 50*###")) },

            // Insurgent Special Service Medal
            { new BackendAward(2260914, new AwardCriteria("army", "count(id)", 1, "time8 >= 180000*### AND best8 >= 100*### AND win8 >= 50*###")) },

            // Navy Seal Special Service Ribbon
            { new BackendAward(3261919, 
                new AwardCriteria("army", "count(id)", 1, "time3 >= 180000"),
                new AwardCriteria("maps", "count(mapid)", 3, "mapid IN (300,301,304) AND time >= 1")
            )},

            // SAS Special Service Ribbon
            { new BackendAward(3261901, 
                new AwardCriteria("army", "count(id)", 1, "time4 >= 180000"),
                new AwardCriteria("maps", "count(mapid)", 3, "mapid IN (302,303,307) AND time >= 1")
            )},

            // SPETZNAS Service Ribbon
            { new BackendAward(3261819, 
                new AwardCriteria("army", "count(id)", 1, "time5 >= 180000"),
                new AwardCriteria("maps", "count(mapid)", 3, "mapid IN (305,306,307) AND time >= 1")
            )},

            // MECSF Service Ribbon
            { new BackendAward(3261319, 
                new AwardCriteria("army", "count(id)", 1, "time6 >= 180000"),
                new AwardCriteria("maps", "count(mapid)", 3, "mapid IN (300,301,304) AND time >= 1")
            )},

            // Rebel Service Ribbon
            { new BackendAward(3261805, 
                new AwardCriteria("army", "count(id)", 1, "time7 >= 180000"),
                new AwardCriteria("maps", "count(mapid)", 2, "mapid IN (305,306) AND time >= 1")
            )},

            // Insurgent Service Ribbon
            { new BackendAward(3260914, 
                new AwardCriteria("army", "count(id)", 1, "time8 >= 180000"),
                new AwardCriteria("maps", "count(mapid)", 2, "mapid IN (302,303) AND time >= 1")
            )},
        };
    }
}
