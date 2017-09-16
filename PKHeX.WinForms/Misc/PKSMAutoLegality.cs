﻿using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;

using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public partial class Blah : UserControl
    {
        public event EventHandler LegalityChanged;
        public PKM LoadShowdownSetModded_PKSM(PKM Set, bool resetForm = false , int TID = -1, int SID = -1, string OT = "")
        {
            bool trainerinfo = TID > 0;
            List<List<string>> evoChart = generateEvoLists2();
            int abilitynum = Set.AbilityNumber < 6 ? Set.AbilityNumber >> 1 : 0;
            if (resetForm)
            {
                Set.AltForm = 0;
                Set.RefreshAbility(Set.AbilityNumber < 6 ? Set.AbilityNumber >> 1 : 0);
            }
            bool shiny = Set.IsShiny;
            bool legendary = false;
            bool eventMon = false;
            int[] legendaryList = new int[] { 144, 145, 146, 150, 151, 243, 244, 245, 249, 250, 251, 377, 378, 379, 380, 381, 382, 383, 384, 385,
                                              386, 480, 481, 482, 483, 484, 485, 486, 487, 488, 489, 490, 491, 492, 493, 494, 638, 639, 640, 642,
                                              641, 645, 643, 644, 646, 647, 648, 649, 716, 717, 718, 719, 720, 721, 785, 786, 787, 788, 789, 790,
                                              791, 792, 793, 794, 795, 796, 797, 798, 799, 800, 801 };

            int[] eventList = new int[] { 251, 719, 649, 720, 385, 647, 490, 648, 721, 801, 802 };

            int[] GameVersionList = new int[] { (int)GameVersion.MN, (int)GameVersion.SN, (int)GameVersion.AS, (int)GameVersion.OR, (int)GameVersion.X,
                                                (int)GameVersion.Y, (int)GameVersion.B, (int)GameVersion.B2, (int)GameVersion.W, (int)GameVersion.W2,
                                                (int)GameVersion.D, (int)GameVersion.P, (int)GameVersion.Pt, (int)GameVersion.HG, (int)GameVersion.SS,
                                                (int)GameVersion.R, (int)GameVersion.S, (int)GameVersion.E, (int)GameVersion.FR, (int)GameVersion.LG,
                                                (int)GameVersion.CXD, (int)GameVersion.RD, (int)GameVersion.GN, (int)GameVersion.BU, (int)GameVersion.YW };
            
            foreach (int mon in legendaryList)
            {
                if (Set.Species == mon)
                {
                    legendary = true;
                }
            }

            foreach (int mon in eventList)
            {
                if (Set.Species == mon)
                {
                    eventMon = true;
                }
            }

            // Egg based pokemon
            if (!legendary && !eventMon)
            {
                for (int i = 0; i < GameVersionList.Length; i++)
                {
                    Set.Version = GameVersionList[i];
                    Set.Language = 2;
                    if (trainerinfo)
                    {
                        Set.OT_Name = OT;
                        Set.TID = TID;
                        Set.SID = SID;
                    }
                    else
                    {
                        Set.OT_Name = "Archit (TCD)";
                        Set.TID = 24521;
                        Set.SID = 42312;
                    }
                    if (Set.Version == (int)GameVersion.RD || Set.Version == (int)GameVersion.BU || Set.Version == (int)GameVersion.YW || Set.Version == (int)GameVersion.GN) Set.SID = 0;
                    Set.EggMetDate = new DateTime(2000, 1, 1);
                    Set.Egg_Location = 60002;
                    if (Set.Version == (int)GameVersion.D || Set.Version == (int)GameVersion.P || Set.Version == (int)GameVersion.Pt) Set.Egg_Location = 2002;
                    Set.Met_Level = 1;
                    Set.ConsoleRegion = 2;
                    if (Set.Version == (int)GameVersion.RD || Set.Version == (int)GameVersion.BU || Set.Version == (int)GameVersion.YW || Set.Version == (int)GameVersion.GN)
                    {
                        Set.Met_Location = 30013;
                        Set.Met_Level = 100;
                    }
                    if (Set.Version == (int)GameVersion.CXD)
                    {
                        Set.Met_Location = 30001;
                        Set.Met_Level = 100;
                    }
                    else { Set = clickMetLocationModPKSM(Set); }
                    if (Set.GenNumber > 4) Set.Met_Level = 1;
                    try
                    {
                        Set.CurrentHandler = 1;
                        Set.HT_Name = "Archit";
                        Set = SetSuggestedRelearnMoves_PKSM(Set);
                        Set.PID = PKX.GetRandomPID(Set.Species, Set.Gender, Set.Version, Set.Nature, Set.Format, (uint)(Set.AbilityNumber * 0x10001));
                        if (shiny) Set.SetShinyPID();
                        if (Set.PID == 0)
                        {
                            Set.PID = PKX.GetRandomPID(Set.Species, Set.Gender, Set.Version, Set.Nature, Set.Format, (uint)(Set.AbilityNumber * 0x10001));
                            if (shiny) Set.SetShinyPID();
                        }
                        if (Set.GenNumber < 6) Set.EncryptionConstant = Set.PID;
                        if (CommonErrorHandling2(Set))
                        {
                            if (shiny) Set.SetShinyPID();
                            return Set;
                        }
                        if (Set.GenNumber < 6) Set.EncryptionConstant = Set.PID;
                        if (new LegalityAnalysis(Set).Valid)
                        {
                            return Set;
                        }
                        else
                        {
                            LegalityAnalysis la = new LegalityAnalysis(Set);
                            Console.WriteLine(la.Report(false));
                        }
                    }
                    catch { continue; }
                }
            }

            if (!new LegalityAnalysis(Set).Valid && !eventMon)
            {
                for (int i = 0; i < GameVersionList.Length; i++)
                {
                    if (Set.Met_Level == 100) Set.Met_Level = 0;
                    Set.WasEgg = false;
                    Set.EggMetDate = null;
                    Set.Egg_Location = 0;
                    Set.Version = GameVersionList[i];
                    Set.Language = 2;
                    Set.ConsoleRegion = 2;
                    if (trainerinfo)
                    {
                        Set.OT_Name = OT;
                        Set.TID = TID;
                        Set.SID = SID;
                    }
                    else
                    {
                        Set.OT_Name = "Archit (TCD)";
                        Set.TID = 24521;
                        Set.SID = 42312;
                    }
                    if (Set.Species == 793 || Set.Species == 794 || Set.Species == 795 || Set.Species == 796 || Set.Species == 797 || Set.Species == 798 || Set.Species == 799) Set.Ball = 26;
                    if (Set.Version == (int)GameVersion.RD || Set.Version == (int)GameVersion.BU || Set.Version == (int)GameVersion.YW || Set.Version == (int)GameVersion.GN) Set.SID = 0;
                    try
                    {
                        Set.RelearnMove1 = 0;
                        Set.RelearnMove2 = 0;
                        Set.RelearnMove3 = 0;
                        Set.RelearnMove4 = 0;
                        if (Set.Version == (int)GameVersion.RD || Set.Version == (int)GameVersion.BU || Set.Version == (int)GameVersion.YW || Set.Version == (int)GameVersion.GN)
                        {
                            Set.Met_Location = 30013;
                            Set.Met_Level = 100;
                        }
                        if (Set.Version == (int)GameVersion.CXD)
                        {
                            Set.Met_Location = 30001;
                            Set.Met_Level = 100;
                        }
                        else { clickMetLocationModPKSM(Set); }
                        Set.CurrentHandler = 1;
                        Set.HT_Name = "Archit";
                        Set.PID = PKX.GetRandomPID(Set.Species, Set.Gender, Set.Version, Set.Nature, Set.Format, (uint)(Set.AbilityNumber * 0x10001));
                        if (shiny) Set.SetShinyPID();
                        if (Set.PID == 0)
                        {
                            Set.PID = PKX.GetRandomPID(Set.Species, Set.Gender, Set.Version, Set.Nature, Set.Format, (uint)(Set.AbilityNumber * 0x10001));
                            if (shiny) Set.SetShinyPID();
                        }
                        Set.RefreshAbility(abilitynum);
                        if (Set.GenNumber < 6) Set.EncryptionConstant = Set.PID;
                        if (CommonErrorHandling2(Set))
                        {
                            if (shiny) Set.SetShinyPID();
                            return Set;
                        }
                        Set.RefreshAbility(abilitynum);
                        if (Set.GenNumber < 6) Set.EncryptionConstant = Set.PID;

                        if (new LegalityAnalysis(Set).Valid)
                        {
                            return Set;
                        }
                        else
                        {
                            LegalityAnalysis la = new LegalityAnalysis(Set);
                            Console.WriteLine(la.Report(false));
                        }
                    }
                    catch { continue; }
                }
            }

            /*
            if (!new LegalityAnalysis(Set).Valid)
            {
                SAVEditor C_SAV = new SAVEditor();
                C_SAV.PKME_Tabs = new Controls.PKMEditor();
                C_SAV.PKME_Tabs.LoadShowdownSetModded(new ShowdownSet(Clipboard.GetText()), true);
            }
            */

            return Set;
        }

        private PKM SetSuggestedRelearnMoves_PKSM(PKM Set)
        {
            Set.RelearnMove1 = 0;
            Set.RelearnMove2 = 0;
            Set.RelearnMove3 = 0;
            Set.RelearnMove4 = 0;
            LegalityAnalysis Legality = new LegalityAnalysis(Set);
            if (Set.Format < 6)
                return Set;

            int[] m = Legality.GetSuggestedRelearn();
            if (m.All(z => z == 0))
                if (!Set.WasEgg && !Set.WasEvent && !Set.WasEventEgg && !Set.WasLink)
                {
                    var encounter = Legality.GetSuggestedMetInfo();
                    if (encounter != null)
                        m = encounter.Relearn;
                }

            if (Set.RelearnMoves.SequenceEqual(m))
                return Set;

            Set.RelearnMove1 = m[0];
            Set.RelearnMove2 = m[1];
            Set.RelearnMove3 = m[2];
            Set.RelearnMove4 = m[3];
            return Set;
        }

        private bool CommonErrorHandling2(PKM pk)
        {
            string hp = pk.IV_HP.ToString();
            string atk = pk.IV_ATK.ToString();
            string def = pk.IV_DEF.ToString();
            string spa = pk.IV_SPA.ToString();
            string spd = pk.IV_SPD.ToString();
            string spe = pk.IV_SPE.ToString();
            bool HTworkaround = false;
            LegalityAnalysis la = new LegalityAnalysis(pk);
            var report = la.Report(false);
            var updatedReport = report;

            if (report.Contains("Ability mismatch for encounter"))
            {
                pk.RefreshAbility(pk.AbilityNumber < 6 ? pk.AbilityNumber >> 1 : 0);
            }
            if (report.Contains("Invalid Met Location, expected Transporter."))
            {
                pk.Met_Location = 30001;
                LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
            }
            if (report.Contains("Can't have ball for encounter type."))
            {
                if (pk.B2W2)
                {
                    pk.Ball = 25; //Dream Ball
                    LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
                    updatedReport = recheckLA.Report(false);
                    report = updatedReport;
                }
                else
                {
                    pk.Ball = 0;
                    LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
                    updatedReport = recheckLA.Report(false);
                    report = updatedReport;
                }
            }
            if (report.Contains("Non japanese Mew from Faraway Island. Unreleased event."))
            {
                bool shiny = pk.IsShiny;
                pk.Language = 1;
                pk.FatefulEncounter = true;
                pk.Nickname = PKX.GetSpeciesNameGeneration(pk.Species, pk.Language, 3);
                pk.PID = PKX.GetRandomPID(pk.Species, pk.Gender, pk.Version, pk.Nature, pk.Format, (uint)(pk.AbilityNumber * 0x10001));
                if (shiny) pk.SetShinySID();
                LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
            }
            if (report.Contains("PID should be equal to EC!"))
            {
                pk.EncryptionConstant = pk.PID;
                LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
            }
            if (report.Contains("PID should be equal to EC [with top bit flipped]!"))
            {
                pk.PID = PKX.GetRandomPID(pk.Species, pk.Gender, pk.Version, pk.Nature, pk.Format, (uint)(pk.AbilityNumber * 0x10001));
                if (pk.IsShiny) pk.SetShinyPID();
                LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
            }
            if (report.Contains("PID-Gender mismatch."))
            {
                if(pk.Gender == 0)
                {
                    pk.Gender = 1;
                }
                else
                {
                    pk.Gender = 0;
                }
                LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
            }
            if (report.Contains("Current level is below met level."))
            {
                pk.CurrentLevel = 100;
                LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
            }
            if (report.Contains("Missing Ribbons: National"))
            {
                ReflectUtil.SetValue(pk, "RibbonNational", -1);
                LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
            }
            if (report.Contains("Invalid Ribbons: National"))
            {
                ReflectUtil.SetValue(pk, "RibbonNational", 0);
                LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
            }
            if (report.Contains("OT Name too long."))
            {
                pk.OT_Name = "ARCH";
                LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
            }
            if (report.Contains("OT from Generation 1/2 uses unavailable characters."))
            {
                pk.OT_Name = "ARCH";
                LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
            }
            if (report.Contains("GeoLocation Memory: Memories should be present."))
            {
                pk.Geo1_Country = 1;
                LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
            }
            if (report.Contains("Can't have ball for encounter type."))
            {
                pk.Ball = 4;
                LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
            }
            if (report.Contains("Form cannot exist outside of a battle."))
            {
                pk.AltForm = 0;
                LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
                
            }
            if (report.Contains("Special ingame Fateful Encounter flag missing"))
            {
                pk.FatefulEncounter = true;
                LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
            }
            if (report.Contains("Fateful Encounter should not be checked."))
            {
                pk.FatefulEncounter = false;
                LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
            }
            if (report.Contains("Invalid: Encounter Type PID mismatch."))
            {
                //return true;
                if (pk.Version == (int)GameVersion.CXD)
                { pk = setPIDSID(pk, pk.IsShiny, true); }
                else pk = setPIDSID(pk, pk.IsShiny);
                if (new LegalityAnalysis(pk).Valid)
                {
                    return false;
                }
                if (pk.HT_HP) { HTworkaround = true; }
                LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
                if (report.Equals("Invalid: Encounter Type PID mismatch."))
                {
                    return true;
                }
                else if (report.Contains("PID-Gender mismatch."))
                {
                    if (pk.Gender == 0)
                    {
                        pk.Gender = 1;
                    }
                    else
                    {
                        pk.Gender = 0;
                    }
                    LegalityAnalysis recheckLA2 = new LegalityAnalysis(pk);
                    updatedReport = recheckLA2.Report(false);
                    report = updatedReport;
                    if (new LegalityAnalysis(pk).Valid)
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        private PKM setPIDSID(PKM pk, bool shiny, bool XD = false)
        {
            uint hp = (uint)pk.IV_HP;
            uint atk = (uint)pk.IV_ATK;
            uint def = (uint)pk.IV_DEF;
            uint spa = (uint)pk.IV_SPA;
            uint spd = (uint)pk.IV_SPD;
            uint spe = (uint)pk.IV_SPE;
            uint nature = (uint)pk.Nature;
            bool pidsidmethod = true;
            string[] pidsid = { "", "" };
            if (XD)
            {
                pidsid = Misc.IVtoPIDGenerator.XDPID(hp, atk, def, spa, spd, spe, nature, 0);
            }
            else { pidsid = Misc.IVtoPIDGenerator.M1PID(hp, atk, def, spa, spd, spe, nature, 0); }
            pk.PID = Util.GetHexValue(pidsid[0]);
            if (pk.GenNumber < 5) pk.EncryptionConstant = pk.PID;
            pk.SID = Convert.ToInt32(pidsid[1]);
            if (shiny) pk.SetShinySID();
            LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
            string updatedReport = recheckLA.Report(false);
            Console.WriteLine(updatedReport);
            if (updatedReport.Contains("Invalid: Encounter Type PID mismatch."))
            {
                string[] hpower = { "fighting", "flying", "poison", "ground", "rock", "bug", "ghost", "steel", "fire", "water", "grass", "electric", "psychic", "ice", "dragon", "dark" };
                string hiddenpower = hpower[pk.HPType];
                string[] NatureHPIVs = Misc.IVtoPIDGenerator.getIVPID(nature, hiddenpower, XD);
                Console.WriteLine(XD);
                pk.PID = Util.GetHexValue(NatureHPIVs[0]);
                if (pk.GenNumber < 5) pk.EncryptionConstant = pk.PID;
                Console.WriteLine(NatureHPIVs[0]);
                pk.IV_HP = Convert.ToInt32(NatureHPIVs[1]);
                pk.IV_ATK = Convert.ToInt32(NatureHPIVs[2]);
                pk.IV_DEF = Convert.ToInt32(NatureHPIVs[3]);
                pk.IV_SPA = Convert.ToInt32(NatureHPIVs[4]);
                pk.IV_SPD = Convert.ToInt32(NatureHPIVs[5]);
                pk.IV_SPE = Convert.ToInt32(NatureHPIVs[6]);
                if (shiny) pk.SetShinySID();
                recheckLA = new LegalityAnalysis(pk);
                updatedReport = recheckLA.Report(false);
                if (!updatedReport.Contains("Invalid: Encounter Type PID mismatch.")) pidsidmethod = false;
                if (pidsid[0] == "0" && pidsid[1] == "0" && pidsidmethod)
                {
                    pk.PID = PKX.GetRandomPID(pk.Species, pk.Gender, pk.Version, pk.Nature, pk.Format, (uint)(pk.AbilityNumber * 0x10001));
                    pk.IV_HP = (int)hp;
                    pk.IV_ATK = (int)atk;
                    pk.IV_DEF = (int)def;
                    pk.IV_SPA = (int)spa;
                    pk.IV_SPD = (int)spd;
                    pk.IV_SPE = (int)spe;
                }
                if (hp >= 30 && pk.IV_HP !=31) pk.HT_HP = true;
                if (atk >= 30 && pk.IV_ATK != 31) pk.HT_ATK = true;
                if (def >= 30 && pk.IV_DEF != 31) pk.HT_DEF = true;
                if (spa >= 30 && pk.IV_SPA != 31) pk.HT_SPA = true;
                if (spd >= 30 && pk.IV_SPD != 31) pk.HT_SPD = true;
                if (spe >= 30 && pk.IV_SPE != 31) pk.HT_SPE = true;
                if (shiny) pk.SetShinySID();
                recheckLA = new LegalityAnalysis(pk);
                updatedReport = recheckLA.Report(false);
                if (updatedReport.Contains("PID-Gender mismatch."))
                {
                    if (pk.Gender == 0)
                    {
                        pk.Gender = 1;
                    }
                    else
                    {
                        pk.Gender = 0;
                    }
                    LegalityAnalysis recheckLA2 = new LegalityAnalysis(pk);
                    updatedReport = recheckLA2.Report(false);
                }
                if (updatedReport.Contains("Can't Hyper Train a Pokémon that isn't level 100."))
                {
                    pk.CurrentLevel = 100;
                    LegalityAnalysis recheckLA2 = new LegalityAnalysis(pk);
                    updatedReport = recheckLA2.Report(false);
                }
                LegalityAnalysis Legality = new LegalityAnalysis(pk);
                if (Legality.Valid) return pk;
                // Fix Moves if a slot is empty 
                pk.FixMoves();

                // PKX is now filled
                pk.RefreshChecksum();
                pk.RefreshAbility(pk.AbilityNumber < 6 ? pk.AbilityNumber >> 1 : 0);
                if (updatedReport.Contains("Invalid: Encounter Type PID mismatch."))
                {
                    pk.HT_HP = false;
                    pk.HT_ATK = false;
                    pk.HT_DEF = false;
                    pk.HT_SPA = false;
                    pk.HT_SPD = false;
                    pk.HT_SPE = false;
                    pk.IV_HP = (int)hp;
                    pk.IV_ATK = (int)atk;
                    pk.IV_DEF = (int)def;
                    pk.IV_SPA = (int)spa;
                    pk.IV_SPD = (int)spd;
                    pk.IV_SPE = (int)spe;
                }
            }
            return pk;
        }

        private PKM clickMetLocationModPKSM(PKM p)
        {
            LegalityAnalysis Legality = new LegalityAnalysis(p);

            var encounter = Legality.GetSuggestedMetInfo();
            if (encounter == null || (p.Format >= 3 && encounter.Location < 0))
            {
                return p;
            }

            int level = encounter.Level;
            int location = encounter.Location;
            int minlvl = Legal.GetLowestLevel(p, encounter.Species);
            if (minlvl == 0)
                minlvl = level;

            if (p.CurrentLevel >= minlvl && p.Met_Level == level && p.Met_Location == location)
                return p;
            if (minlvl < level)
                minlvl = level;
            p.Met_Location = location;
            p.Met_Level = level;
            return p;
        }

        private List<List<string>> generateEvoLists2()
        {
            int counter = 0;
            string line;
            List<List<string>> evoList = new List<List<string>>();
            List<string> blankList = new List<string>();
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "PKHeX.WinForms.Resources.text.evolutions.txt";
            System.IO.Stream stream = assembly.GetManifestResourceStream(resourceName);
            System.IO.StreamReader file = new System.IO.StreamReader(stream);
            while ((line = file.ReadLine()) != null)
            {
                if (line.Trim() == "")
                {
                    evoList.Add(blankList);
                    blankList = new List<string>();
                }
                else
                {
                    blankList.Add(line.Trim());
                }
                counter++;
            }
            file.Close();
            return evoList;
        }

        private void UpdateLegality(PKM pkm, bool skipMoveRepop = false)
        {
            LegalityAnalysis Legality = new LegalityAnalysis(pkm);
            Console.WriteLine(Legality.Report(true));
            // Refresh Move Legality
            bool[]validmoves = new bool[] { false, false, false, false };
            for (int i = 0; i < 4; i++)
                validmoves[i] = !Legality.Info?.Moves[i].Valid ?? false;

            bool[] validrelearn = new bool[] { false, false, false, false };
            if (pkm.Format >= 6)
                for (int i = 0; i < 4; i++)
                    validrelearn[i] = !Legality.Info?.Relearn[i].Valid ?? false;

            if (skipMoveRepop)
                return;
            // Resort moves
            bool fieldsLoaded = true;
            bool tmp = fieldsLoaded;
            fieldsLoaded = false;
            var cb = new[] { pkm.Move1, pkm.Move2, pkm.Move3, pkm.Move4 };
            var moves = Legality.AllSuggestedMovesAndRelearn;
            var moveList = GameInfo.MoveDataSource.OrderByDescending(m => moves.Contains(m.Value)).ToArray();
            
            fieldsLoaded |= tmp;
            LegalityChanged?.Invoke(Legality.Valid, null);
        }

    }
}
