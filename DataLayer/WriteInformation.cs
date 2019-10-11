 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace DataLayer
{
    public class WriteInformation
    {
        GeneralInfo tmpGeneral;
        TrainingProgramInfo[] tmpTrainingPr;
        HospitalInfo[] tmphospitalInfos;
        DisciplineInfo[] tmpdisciplineInfos;
        InternInfo[] tmpinternInfos;
        RegionInfo[] tmpRegionInfos;
        AlgorithmSettings tmpalgorithmSettings;
        InstanceSetting instancesetting;
        public OptimalSolution FeasibleSolution;
        bool[][][][] X_itdh;
        int[][][] timelineHospMax_twh;
        int[][] YearlyHospMax_wh;
        bool[][] timeline_it;
        bool[][] discipline_id;
        int[][] hospital_ih;
        public WriteInformation(InstanceSetting InsSetting, string location, string name, bool ifReal)
        {
            instancesetting = InsSetting;
            if (ifReal)
            {
                CreateInstancesRealLife();
            }
            else
            {
                CreateInstances();
            }

            //WriteInstance(location, name);
            WriteFeasibleSol(location, "Feasible" + name);
            ChangeObjCoeff(location, name);
        }


        /// <summary>
        /// this function creates the instances 
        /// </summary>
        public void CreateInstances()
        {
            Random random_ = new Random();

            // general Info
            tmpGeneral = new GeneralInfo();

            tmpGeneral.Disciplines = instancesetting.TotalDiscipline;
            tmpGeneral.Interns = instancesetting.TotalIntern;
            tmpGeneral.HospitalWard = (int)Math.Round(instancesetting.R_wd * tmpGeneral.Disciplines);
            double R_Cap = instancesetting.R_wh * instancesetting.R_wd * tmpGeneral.Disciplines / tmpGeneral.Interns;
            double totalInternship = 0;
            tmpGeneral.Hospitals = 0;
            for (int p = 0; p < instancesetting.TTrainingP; p++)
            {
                double Hosp = 0;
                double internship = 0;
                for (int g = 0; g < instancesetting.TDGroup; g++)
                {
                    internship += instancesetting.TotalDiscipline * (instancesetting.DisciplineDistribution_p[p] + instancesetting.R_muDp)
                               * instancesetting.DisciplineDistribution_g[g] * instancesetting.R_gk_g[g];
                }
                if (internship > totalInternship)
                {
                    totalInternship = internship;
                }
                Hosp = Math.Ceiling(totalInternship / instancesetting.AllowedDiscInHospital_p[p]);
                if (Hosp > tmpGeneral.Hospitals)
                {
                    tmpGeneral.Hospitals = (int)Hosp;
                }
            }

            tmpGeneral.Region = instancesetting.TRegion;
            tmpGeneral.TrainingPr = instancesetting.TTrainingP;
            tmpGeneral.DisciplineGr = instancesetting.TDGroup;

            //[groupName pr1 pr2 0 0]
            double[] prValueG = new double[tmpGeneral.DisciplineGr];
            prValueG = instancesetting.DisciplineDistribution_g;
            //time period 
            // 12 => 4week
            // 24 => 2week
            int[] tp_ = { 12, 24 };
            tmpGeneral.TimePriods = tp_[(int)random_.Next(0, tp_.Length)];
            tmpGeneral.TimePriods = instancesetting.TTime;

            int rnd, temp;

            //Create training program info
            tmpTrainingPr = new TrainingProgramInfo[tmpGeneral.TrainingPr];
            for (int p = 0; p < tmpGeneral.TrainingPr; p++)
            {
                tmpTrainingPr[p] = new TrainingProgramInfo(tmpGeneral.Disciplines, tmpGeneral.DisciplineGr);
                tmpTrainingPr[p].Name = (p + 1).ToString() + "_Year";
                tmpTrainingPr[p].AcademicYear = p + 1;

                // assign the preference for disciplines
                int maxdisp = tmpGeneral.Disciplines;
                int[] xx = new int[maxdisp];
                for (int ii = 0; ii < maxdisp; ii++)
                {
                    xx[ii] = ii;
                }
                int[] yy = new int[maxdisp];
                yy = xx.OrderBy(x => random_.Next()).ToArray();

                double FirstDiscP = 0;
                double FirstDiscG = 0;
                double cumolativeP = 0;
                double cumolativeG = 0;
                double lastDiscP = 0;
                double lastDiscG = 0;
                for (int pp = 0; pp < p; pp++)
                {
                    cumolativeP += instancesetting.DisciplineDistribution_p[pp];
                }
                FirstDiscP = (int)Math.Ceiling(cumolativeP * tmpGeneral.Disciplines);
                cumolativeP += instancesetting.DisciplineDistribution_p[p];
                lastDiscP = (int)Math.Ceiling(cumolativeP * tmpGeneral.Disciplines);
                lastDiscG = FirstDiscP;
                for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
                {
                    FirstDiscG = Math.Ceiling(lastDiscG);
                    cumolativeG += prValueG[g];
                    lastDiscG = FirstDiscP + (lastDiscP - FirstDiscP) * cumolativeG;
                    for (int d = (int)FirstDiscG; d < Math.Ceiling(lastDiscG) && d < tmpGeneral.Disciplines; d++)
                    {
                        tmpTrainingPr[p].InvolvedDiscipline_gd[g][d] = true;
                        tmpTrainingPr[p].Prf_d[d] = random_.Next(1, instancesetting.PrfMaxValue);
                    }
                }

                // assign the weight for preference
                tmpTrainingPr[p].weight_p = random_.Next(1, instancesetting.PrfMaxValue);
                // assign the coefficient 
                tmpTrainingPr[p].CoeffObj_SumDesi = random_.Next(1, instancesetting.CoefficientMaxValue);
                tmpTrainingPr[p].CoeffObj_MinDesi = random_.Next(1, instancesetting.CoefficientMaxValue);
                tmpTrainingPr[p].CoeffObj_ResCap = random_.Next(1, instancesetting.CoefficientMaxValue);
                tmpTrainingPr[p].CoeffObj_EmrCap = random_.Next(1, instancesetting.CoefficientMaxValue);
                tmpTrainingPr[p].CoeffObj_NotUsedAcc = random_.Next(1, instancesetting.CoefficientMaxValue);
                tmpTrainingPr[p].CoeffObj_MINDem = random_.Next(1, instancesetting.CoefficientMaxValue);

                tmpTrainingPr[p].DiscChangeInOneHosp = random_.Next(1, tmpGeneral.Disciplines);
                tmpTrainingPr[p].DiscChangeInOneHosp = instancesetting.AllowedDiscInHospital_p[p];

            }

            // mutual discipline in the training program
            for (int p = 0; p < tmpGeneral.TrainingPr; p++)
            {
                for (int pp = 0; pp < tmpGeneral.TrainingPr; pp++)
                {
                    for (int d = 0; d < tmpGeneral.Disciplines; d++)
                    {
                        bool notExist = true;
                        for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
                        {
                            if (tmpTrainingPr[p].InvolvedDiscipline_gd[g][d])
                            {
                                notExist = false;
                            }
                        }
                        for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
                        {
                            if (notExist && tmpTrainingPr[pp].InvolvedDiscipline_gd[g][d] && random_.NextDouble() < instancesetting.R_muDp)
                            {
                                tmpTrainingPr[p].InvolvedDiscipline_gd[g][d] = true;
                                break;
                            }
                        }
                    }
                }
            }

            // mutual discipline in the training program
            for (int p = 0; p < tmpGeneral.TrainingPr; p++)
            {
                for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
                {
                    for (int gg = 0; gg < tmpGeneral.DisciplineGr; gg++)
                    {
                        for (int d = 0; d < tmpGeneral.Disciplines; d++)
                        {
                            if (!tmpTrainingPr[p].InvolvedDiscipline_gd[g][d] && tmpTrainingPr[p].InvolvedDiscipline_gd[gg][d] && random_.NextDouble() < instancesetting.R_muDg)
                            {
                                tmpTrainingPr[p].InvolvedDiscipline_gd[g][d] = true;
                            }
                        }
                    }
                }
            }

            // AKA and consecutive
            for (int p = 0; p < tmpGeneral.TrainingPr; p++)
            {
                for (int d = 0; d < tmpGeneral.Disciplines; d++)
                {
                    for (int dd = 0; dd < tmpGeneral.Disciplines; dd++)
                    {
                        if (d == dd)
                        {
                            continue;
                        }
                        if (random_.NextDouble() > 0.9)
                        {
                            tmpTrainingPr[p].AKA_dD[d][dd] = true;
                            tmpTrainingPr[p].AKA_dD[dd][d] = true;
                        }
                        if (random_.NextDouble() > 0.9 && !tmpTrainingPr[p].AKA_dD[d][dd])
                        {
                            tmpTrainingPr[p].cns_dD[d][dd] = 1;
                            tmpTrainingPr[p].cns_dD[dd][d] = 1;
                        }
                    }
                }
            }

            // Create Hospital Info 
            tmphospitalInfos = new HospitalInfo[tmpGeneral.Hospitals];
            for (int h = 0; h < tmpGeneral.Hospitals; h++)
            {
                tmphospitalInfos[h] = new HospitalInfo(tmpGeneral.Disciplines, tmpGeneral.TimePriods, tmpGeneral.HospitalWard, tmpGeneral.Region);
                tmphospitalInfos[h].Name = h.ToString();

                for (int d = 0; d < tmpGeneral.Disciplines; d++)
                {
                    int w = d;
                    if (w > tmpGeneral.HospitalWard - 1)
                    {
                        w -= tmpGeneral.HospitalWard;
                    }
                    tmphospitalInfos[h].Hospital_dw[d][w] = random_.NextDouble() < instancesetting.R_wh;
                }

                for (int w = 0; w < tmpGeneral.HospitalWard; w++)
                {
                    bool haveW = false;
                    for (int d = 0; d < tmpGeneral.Disciplines; d++)
                    {
                        if (tmphospitalInfos[h].Hospital_dw[d][w])
                        {
                            haveW = true;
                            break;
                        }
                    }
                    // if the hospital has the ward has demand + reserved and emergency, otherwise 0
                    if (haveW)
                    {
                        int rate = instancesetting.MaxDem;

                        int min = random_.NextDouble() < instancesetting.R_dMin ? 1 : 0;
                        int max = random_.Next(1, rate + 1);
                        int yearlyMax = random_.Next((int)(instancesetting.MaxYearlyDemandPrcnt * tmpGeneral.TimePriods * max), tmpGeneral.TimePriods * max + 1);
                        int yearlyMin = 0;
                        for (int t = 0; t < tmpGeneral.TimePriods; t++)
                        {
                            tmphospitalInfos[h].HospitalMinDem_tw[t][w] = min;
                            tmphospitalInfos[h].HospitalMaxDem_tw[t][w] = max;
                            tmphospitalInfos[h].HospitalMaxYearly_w[w] = yearlyMax;
                            tmphospitalInfos[h].HospitalMinYearly_w[w] = yearlyMin;
                            tmphospitalInfos[h].EmergencyCap_tw[t][w] = instancesetting.EmrDem;
                            tmphospitalInfos[h].ReservedCap_tw[t][w] = instancesetting.ResDem;
                        }

                    }
                    else
                    {
                        int min = 0;
                        int max = 0;
                        for (int t = 0; t < tmpGeneral.TimePriods; t++)
                        {
                            tmphospitalInfos[h].HospitalMinDem_tw[t][w] = min;
                            tmphospitalInfos[h].HospitalMaxDem_tw[t][w] = max;
                        }

                    }
                }
                // if the hospital is in the region
                for (int r = 0; r < tmpGeneral.Region; r++)
                {
                    tmphospitalInfos[h].InToRegion_r[r] = random_.Next(0, 4) > 2;
                }
            }

            // Create Discipline Info
            tmpdisciplineInfos = new DisciplineInfo[tmpGeneral.Disciplines];
            for (int d = 0; d < tmpGeneral.Disciplines; d++)
            {
                tmpdisciplineInfos[d] = new DisciplineInfo(tmpGeneral.Disciplines, tmpGeneral.TrainingPr);
                tmpdisciplineInfos[d].Name = d.ToString();
                for (int p = 0; p < tmpGeneral.TrainingPr; p++)
                {
                    tmpdisciplineInfos[d].Duration_p[p] = random_.Next(1, (int)((double)tmpGeneral.TimePriods / totalInternship));
                    tmpdisciplineInfos[d].CourseCredit_p[p] = random_.Next(1, 10) >= 7 ? 2 : 1;
                }
            }

            for (int pp = 0; pp < tmpGeneral.TrainingPr; pp++)
            {
                for (int d = 0; d < tmpGeneral.Disciplines; d++)
                {
                    for (int dd = d + 1; dd < tmpGeneral.Disciplines; dd++)
                    {
                        bool dInv = false;
                        bool ddInv = false;
                        for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
                        {
                            if (tmpTrainingPr[pp].InvolvedDiscipline_gd[g][d])
                            {
                                dInv = true;
                            }
                            if (tmpTrainingPr[pp].InvolvedDiscipline_gd[g][dd])
                            {
                                ddInv = true;
                            }
                        }
                        if (random_.NextDouble() < instancesetting.R_Trel && !tmpdisciplineInfos[dd].Skill4D_dp[d][pp] && dd != d && dInv && ddInv)
                        {
                            tmpdisciplineInfos[d].Skill4D_dp[dd][pp] = true;
                            tmpdisciplineInfos[d].requiresSkill_p[pp] = true;
                            tmpdisciplineInfos[dd].requiredLater_p[pp] = true;
                            //minimum demand will be zero to not face conflict at time zero
                            for (int h = 0; h < tmpGeneral.Hospitals; h++)
                            {
                                for (int w = 0; w < tmpGeneral.HospitalWard; w++)
                                {
                                    for (int t = 0; t < tmpGeneral.TimePriods; t++)
                                    {
                                        if (tmphospitalInfos[h].Hospital_dw[d][w])
                                        {
                                            tmphospitalInfos[h].HospitalMinDem_tw[t][w] = 0;
                                        }
                                    }
                                }
                            }
                            // if you only want the discipline d connects to only one other discipline 
                            // otherwise clean the break
                            break;
                        }

                    }
                }
            }

            // create Intern info
            tmpinternInfos = new InternInfo[tmpGeneral.Interns];
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                tmpinternInfos[i] = new InternInfo(tmpGeneral.Hospitals, tmpGeneral.Disciplines, tmpGeneral.TimePriods, tmpGeneral.DisciplineGr, tmpGeneral.TrainingPr, tmpGeneral.Region);
                tmpinternInfos[i].ProgramID = random_.Next(0, tmpGeneral.TrainingPr);
            }
            feasibleSolution();
            setInternsList();
            setInternsOversea();
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                int totalTime = 0;
                int totalDisc = 0;
                tmpinternInfos[i].isProspective = random_.NextDouble() < instancesetting.Prespective;
                // hand out group and discipline


                for (int d = 0; d < tmpGeneral.Disciplines; d++)
                {
                    for (int h = 0; h < tmpGeneral.Hospitals; h++)
                    {
                        tmpinternInfos[i].Abi_dh[d][h] = true;
                    }
                    tmpinternInfos[i].Prf_d[d] = random_.Next(1, instancesetting.PrfMaxValue);
                    totalTime += tmpdisciplineInfos[d].Duration_p[tmpinternInfos[i].ProgramID];
                }
                // fulfilled disciplines happens for 10-20 percent of students
                double ffPrcntStudent = instancesetting.fulfilled;
                if (random_.NextDouble() < ffPrcntStudent)
                {
                    // 10 percent of discipline may fulfilled before
                    double ffPrcntDiscipline = 0.1;
                    int totalFulfilled = random_.Next(0, (int)Math.Round(ffPrcntDiscipline * totalDisc + 1, 0));

                    for (int ff = 0; ff < totalFulfilled; ff++)
                    {
                        int dIndex = random_.Next(0, tmpGeneral.Disciplines);
                        int Gr = -1;
                        int Pr = random_.Next(0, 5) < 4 ? tmpinternInfos[i].ProgramID : random_.Next(0, tmpGeneral.TrainingPr);
                        int Hosp = -1;
                        while (true)
                        {
                            for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
                            {
                                if (tmpinternInfos[i].DisciplineList_dg[dIndex][g])
                                {
                                    Gr = g;
                                    for (int h = 0; h < tmpGeneral.Hospitals; h++)
                                    {
                                        for (int d = 0; d < tmpGeneral.HospitalWard; d++)
                                        {
                                            if (tmphospitalInfos[h].Hospital_dw[dIndex][d])
                                            {
                                                Hosp = h;
                                            }
                                        }

                                    }
                                }
                            }
                            if (Gr >= 0 && Hosp >= 0)
                            {
                                break;
                            }
                            else
                            {
                                dIndex = random_.Next(0, tmpGeneral.Disciplines);
                            }

                        }

                        tmpinternInfos[i].Fulfilled_dhp[dIndex][Hosp][Pr] = true;

                    }
                }

                // oversea disciplines happens for 10-20 percent of students
                double fhPrcntStudent = instancesetting.overseaHosp;

                // region 
                for (int r = 0; r < tmpGeneral.Region; r++)
                {
                    tmpinternInfos[i].TransferredTo_r[r] = random_.NextDouble() < 0.4;
                }
                totalTime = tmpGeneral.TimePriods - totalTime;
                if (totalTime < 0)
                {
                    totalTime = 0;
                }
                totalTime = random_.Next(0, totalTime);
                int startInd = random_.Next(0, tmpGeneral.TimePriods);
                for (int t = startInd; t < totalTime && t < tmpGeneral.TimePriods; t++)
                {
                    tmpinternInfos[i].Ave_t[t] = false;
                }
                for (int h = 0; h < tmpGeneral.Hospitals; h++)
                {
                    tmpinternInfos[i].Prf_h[h] = random_.Next(1, instancesetting.PrfMaxValue);
                }
                tmpinternInfos[i].wieght_ch = -random_.Next(1, instancesetting.PrfMaxValue);
                if (tmpTrainingPr[tmpinternInfos[i].ProgramID].DiscChangeInOneHosp == 1)
                {
                    tmpinternInfos[i].wieght_ch = 0;
                }
                tmpinternInfos[i].wieght_d = random_.Next(1, instancesetting.PrfMaxValue);
                tmpinternInfos[i].wieght_h = random_.Next(1, instancesetting.PrfMaxValue);
                tmpinternInfos[i].wieght_w = -random_.Next(1, instancesetting.PrfMaxValue);
                tmpinternInfos[i].wieght_cns = -random_.Next(1, instancesetting.PrfMaxValue);
            }

            // create Region info
            tmpRegionInfos = new RegionInfo[tmpGeneral.Region];
            for (int r = 0; r < tmpGeneral.Region; r++)
            {
                tmpRegionInfos[r] = new RegionInfo(tmpGeneral.TimePriods);
                tmpRegionInfos[r].Name = "Reg_" + r;
                tmpRegionInfos[r].SQLID = r;
                int MIN = random_.Next(0, 2);
                for (int t = 0; t < tmpGeneral.TimePriods; t++)
                {
                    tmpRegionInfos[r].AvaAcc_t[t] = MIN;
                }
            }

            //Algorithm Settings
            tmpalgorithmSettings = new AlgorithmSettings();
            tmpalgorithmSettings.BPTime = 7200;
            tmpalgorithmSettings.MasterTime = 100;
            tmpalgorithmSettings.MIPTime = 7200;
            tmpalgorithmSettings.RCepsi = 0.000001;
            tmpalgorithmSettings.RHSepsi = 0.000001;
            tmpalgorithmSettings.SubTime = 600;
            tmpalgorithmSettings.NodeTime = 3600;
            tmpalgorithmSettings.BigM = 40000;
            tmpalgorithmSettings.MotivationBM = tmpalgorithmSettings.BigM / 100;
            tmpalgorithmSettings.bucketBasedImpPercentage = 0.5;
            tmpalgorithmSettings.internBasedImpPercentage = 0.1;



        }

        public void CreateInstancesRealLife()
        {
            Random random_ = new Random();

            // general Info
            tmpGeneral = new GeneralInfo();

            tmpGeneral.Disciplines = instancesetting.TotalDiscipline;
            tmpGeneral.Interns = instancesetting.TotalIntern;
            tmpGeneral.HospitalWard = (int)Math.Round(instancesetting.R_wd * tmpGeneral.Disciplines);
            double R_Cap = instancesetting.R_wh * instancesetting.R_wd * tmpGeneral.Disciplines / tmpGeneral.Interns;
            double totalInternship = 0;
            tmpGeneral.Hospitals = 30;
            for (int p = 0; p < instancesetting.TTrainingP; p++)
            {
                double Hosp = 0;
                double internship = 0;
                for (int g = 0; g < instancesetting.TDGroup; g++)
                {
                    internship += instancesetting.TotalDiscipline * (instancesetting.DisciplineDistribution_p[p] + instancesetting.R_muDp)
                               * instancesetting.DisciplineDistribution_g[g] * instancesetting.R_gk_g[g];
                }
                if (internship > totalInternship)
                {
                    totalInternship = internship;
                }
                Hosp = Math.Ceiling(totalInternship / instancesetting.AllowedDiscInHospital_p[p]);
                if (Hosp > tmpGeneral.Hospitals)
                {
                    tmpGeneral.Hospitals = (int)Hosp;
                }
            }
            tmpGeneral.Hospitals = 30;
            tmpGeneral.Region = instancesetting.TRegion;
            tmpGeneral.TrainingPr = instancesetting.TTrainingP;
            tmpGeneral.DisciplineGr = instancesetting.TDGroup;

            //[groupName pr1 pr2 0 0]
            double[] prValueG = new double[tmpGeneral.DisciplineGr];
            prValueG = instancesetting.DisciplineDistribution_g;
            //time period 
            // 12 => 4week
            // 24 => 2week
            int[] tp_ = { 12, 24 };
            tmpGeneral.TimePriods = tp_[(int)random_.Next(0, tp_.Length)];
            tmpGeneral.TimePriods = instancesetting.TTime;

            int rnd, temp;

            //Create training program info
            tmpTrainingPr = new TrainingProgramInfo[tmpGeneral.TrainingPr];
            for (int p = 0; p < tmpGeneral.TrainingPr; p++)
            {
                tmpTrainingPr[p] = new TrainingProgramInfo(tmpGeneral.Disciplines, tmpGeneral.DisciplineGr);
                tmpTrainingPr[p].Name = (p + 1).ToString() + "_Year";
                tmpTrainingPr[p].AcademicYear = p + 1;

                // assign the preference for disciplines
                int maxdisp = tmpGeneral.Disciplines;
                int[] xx = new int[maxdisp];
                for (int ii = 0; ii < maxdisp; ii++)
                {
                    xx[ii] = ii;
                }
                int[] yy = new int[maxdisp];
                yy = xx.OrderBy(x => random_.Next()).ToArray();

                double FirstDiscP = 0;
                double FirstDiscG = 0;
                double cumolativeP = 0;
                double cumolativeG = 0;
                double lastDiscP = 0;
                double lastDiscG = 0;
                for (int pp = 0; pp < p; pp++)
                {
                    cumolativeP += instancesetting.DisciplineDistribution_p[pp];
                }
                FirstDiscP = (int)Math.Ceiling(cumolativeP * tmpGeneral.Disciplines);
                cumolativeP += instancesetting.DisciplineDistribution_p[p];
                lastDiscP = (int)Math.Ceiling(cumolativeP * tmpGeneral.Disciplines);
                lastDiscG = FirstDiscP;
                for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
                {
                    FirstDiscG = Math.Ceiling(lastDiscG);
                    cumolativeG += prValueG[g];
                    lastDiscG = FirstDiscP + (lastDiscP - FirstDiscP) * cumolativeG;
                    for (int d = (int)FirstDiscG; d < Math.Ceiling(lastDiscG) && d < tmpGeneral.Disciplines; d++)
                    {
                        tmpTrainingPr[p].InvolvedDiscipline_gd[g][d] = true;
                        tmpTrainingPr[p].Prf_d[d] = random_.Next(1, instancesetting.PrfMaxValue);
                    }
                }

                // assign the weight for preference
                tmpTrainingPr[p].weight_p = random_.Next(1, instancesetting.PrfMaxValue);
                tmpTrainingPr[p].weight_p = 0;
                // assign the coefficient 
                tmpTrainingPr[p].CoeffObj_SumDesi = random_.Next(1, instancesetting.CoefficientMaxValue);
                tmpTrainingPr[p].CoeffObj_MinDesi = random_.Next(1, instancesetting.CoefficientMaxValue);
                tmpTrainingPr[p].CoeffObj_ResCap = random_.Next(1, instancesetting.CoefficientMaxValue);
                tmpTrainingPr[p].CoeffObj_EmrCap = random_.Next(1, instancesetting.CoefficientMaxValue);
                tmpTrainingPr[p].CoeffObj_NotUsedAcc = random_.Next(1, instancesetting.CoefficientMaxValue);
                tmpTrainingPr[p].CoeffObj_MINDem = random_.Next(1, instancesetting.CoefficientMaxValue);

                tmpTrainingPr[p].DiscChangeInOneHosp = random_.Next(1, tmpGeneral.Disciplines);
                tmpTrainingPr[p].DiscChangeInOneHosp = instancesetting.AllowedDiscInHospital_p[p];

            }

            // mutual discipline in the training program
            for (int p = 0; p < tmpGeneral.TrainingPr; p++)
            {
                for (int pp = 0; pp < tmpGeneral.TrainingPr; pp++)
                {
                    for (int d = 0; d < tmpGeneral.Disciplines; d++)
                    {
                        bool notExist = true;
                        for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
                        {
                            if (tmpTrainingPr[p].InvolvedDiscipline_gd[g][d])
                            {
                                notExist = false;
                            }
                        }
                        for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
                        {
                            if (notExist && tmpTrainingPr[pp].InvolvedDiscipline_gd[g][d] && random_.NextDouble() < instancesetting.R_muDp)
                            {
                                tmpTrainingPr[p].InvolvedDiscipline_gd[g][d] = true;
                                break;
                            }
                        }
                    }
                }
            }

            // mutual discipline in the training program
            for (int p = 0; p < tmpGeneral.TrainingPr; p++)
            {
                for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
                {
                    for (int gg = 0; gg < tmpGeneral.DisciplineGr; gg++)
                    {
                        for (int d = 0; d < tmpGeneral.Disciplines; d++)
                        {
                            if (!tmpTrainingPr[p].InvolvedDiscipline_gd[g][d] && tmpTrainingPr[p].InvolvedDiscipline_gd[gg][d] && random_.NextDouble() < instancesetting.R_muDg)
                            {
                                tmpTrainingPr[p].InvolvedDiscipline_gd[g][d] = true;
                            }
                        }
                    }
                }
            }

            // Create Hospital Info 
            tmphospitalInfos = new HospitalInfo[tmpGeneral.Hospitals];
            for (int h = 0; h < tmpGeneral.Hospitals; h++)
            {
                tmphospitalInfos[h] = new HospitalInfo(tmpGeneral.Disciplines, tmpGeneral.TimePriods, tmpGeneral.HospitalWard, tmpGeneral.Region);
                tmphospitalInfos[h].Name = h.ToString();

                for (int d = 0; d < tmpGeneral.Disciplines; d++)
                {
                    int w = d;
                    if (w > tmpGeneral.HospitalWard - 1)
                    {
                        w -= tmpGeneral.HospitalWard;
                    }
                    tmphospitalInfos[h].Hospital_dw[d][w] = random_.NextDouble() <= instancesetting.R_wh;
                }

                for (int w = 0; w < tmpGeneral.HospitalWard; w++)
                {
                    bool haveW = false;
                    for (int d = 0; d < tmpGeneral.Disciplines; d++)
                    {
                        if (tmphospitalInfos[h].Hospital_dw[d][w])
                        {
                            haveW = true;
                            break;
                        }
                    }
                    // if the hospital has the ward has demand + reserved and emergency, otherwise 0
                    if (haveW)
                    {
                        int rate = instancesetting.MaxDem;

                        int min = random_.NextDouble() < instancesetting.R_dMin ? 1 : 0;
                        int max = random_.Next(1, rate + 1);
                        int yearlyMax = random_.Next((int)(instancesetting.MaxYearlyDemandPrcnt * tmpGeneral.TimePriods * max), tmpGeneral.TimePriods * max + 1);
                        int yearlyMin = 0;
                        for (int t = 0; t < tmpGeneral.TimePriods; t++)
                        {
                            tmphospitalInfos[h].HospitalMinDem_tw[t][w] = min;
                            tmphospitalInfos[h].HospitalMaxDem_tw[t][w] = max;
                            tmphospitalInfos[h].HospitalMaxYearly_w[w] = yearlyMax;
                            tmphospitalInfos[h].HospitalMinYearly_w[w] = yearlyMin;
                            tmphospitalInfos[h].EmergencyCap_tw[t][w] = instancesetting.EmrDem;
                            tmphospitalInfos[h].ReservedCap_tw[t][w] = instancesetting.ResDem;
                        }

                    }
                    else
                    {
                        int min = 0;
                        int max = 0;
                        for (int t = 0; t < tmpGeneral.TimePriods; t++)
                        {
                            tmphospitalInfos[h].HospitalMinDem_tw[t][w] = min;
                            tmphospitalInfos[h].HospitalMaxDem_tw[t][w] = max;
                        }

                    }
                }
                // if the hospital is in the region
                for (int r = 0; r < tmpGeneral.Region; r++)
                {
                    tmphospitalInfos[h].InToRegion_r[r] = h < 3;
                }
            }

            // Create Discipline Info
            tmpdisciplineInfos = new DisciplineInfo[tmpGeneral.Disciplines];
            for (int d = 0; d < tmpGeneral.Disciplines; d++)
            {
                tmpdisciplineInfos[d] = new DisciplineInfo(tmpGeneral.Disciplines, tmpGeneral.TrainingPr);
                tmpdisciplineInfos[d].Name = d.ToString();
                for (int p = 0; p < tmpGeneral.TrainingPr; p++)
                {
                    tmpdisciplineInfos[d].Duration_p[p] = 1;
                    tmpdisciplineInfos[d].CourseCredit_p[p] = 1;
                }
            }

            for (int pp = 0; pp < tmpGeneral.TrainingPr; pp++)
            {
                for (int d = 0; d < tmpGeneral.Disciplines; d++)
                {
                    for (int dd = d + 1; dd < tmpGeneral.Disciplines; dd++)
                    {
                        bool dInv = false;
                        bool ddInv = false;
                        for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
                        {
                            if (tmpTrainingPr[pp].InvolvedDiscipline_gd[g][d])
                            {
                                dInv = true;
                            }
                            if (tmpTrainingPr[pp].InvolvedDiscipline_gd[g][dd])
                            {
                                ddInv = true;
                            }
                        }
                        if (random_.NextDouble() < instancesetting.R_Trel && !tmpdisciplineInfos[dd].Skill4D_dp[d][pp] && dd != d && dInv && ddInv)
                        {
                            tmpdisciplineInfos[d].Skill4D_dp[dd][pp] = true;
                            tmpdisciplineInfos[d].requiresSkill_p[pp] = true;
                            tmpdisciplineInfos[dd].requiredLater_p[pp] = true;
                            //minimum demand will be zero to not face conflict at time zero
                            for (int h = 0; h < tmpGeneral.Hospitals; h++)
                            {
                                for (int w = 0; w < tmpGeneral.HospitalWard; w++)
                                {
                                    for (int t = 0; t < tmpGeneral.TimePriods; t++)
                                    {
                                        if (tmphospitalInfos[h].Hospital_dw[d][w])
                                        {
                                            tmphospitalInfos[h].HospitalMinDem_tw[t][w] = 0;
                                        }
                                    }
                                }
                            }
                            // if you only want the discipline d connects to only one other discipline 
                            // otherwise clean the break
                            break;
                        }

                    }
                }
            }

            // create Intern info
            tmpinternInfos = new InternInfo[tmpGeneral.Interns];
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                tmpinternInfos[i] = new InternInfo(tmpGeneral.Hospitals, tmpGeneral.Disciplines, tmpGeneral.TimePriods, tmpGeneral.DisciplineGr, tmpGeneral.TrainingPr, tmpGeneral.Region);
                tmpinternInfos[i].ProgramID = random_.Next(0, tmpGeneral.TrainingPr);
            }
            feasibleSolution();
            setInternsListRealLife();
            setInternsOverseaRealLife();
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                int totalTime = 0;
                int totalDisc = 0;
                tmpinternInfos[i].isProspective = random_.NextDouble() < instancesetting.Prespective;
                // hand out group and discipline


                for (int d = 0; d < tmpGeneral.Disciplines; d++)
                {
                    for (int h = 0; h < tmpGeneral.Hospitals; h++)
                    {
                        tmpinternInfos[i].Abi_dh[d][h] = true;
                    }
                    tmpinternInfos[i].Prf_d[d] = random_.Next(1, instancesetting.PrfMaxValue);
                    totalTime += tmpdisciplineInfos[d].Duration_p[tmpinternInfos[i].ProgramID];
                }
                // fulfilled disciplines happens for 10-20 percent of students
                double ffPrcntStudent = instancesetting.fulfilled;
                if (random_.NextDouble() < ffPrcntStudent)
                {
                    // 10 percent of discipline may fulfilled before
                    double ffPrcntDiscipline = 0.1;
                    int totalFulfilled = random_.Next(0, (int)Math.Round(ffPrcntDiscipline * totalDisc + 1, 0));

                    for (int ff = 0; ff < totalFulfilled; ff++)
                    {
                        int dIndex = random_.Next(0, tmpGeneral.Disciplines);
                        int Gr = -1;
                        int Pr = random_.Next(0, 5) < 4 ? tmpinternInfos[i].ProgramID : random_.Next(0, tmpGeneral.TrainingPr);
                        int Hosp = -1;
                        while (true)
                        {
                            for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
                            {
                                if (tmpinternInfos[i].DisciplineList_dg[dIndex][g])
                                {
                                    Gr = g;
                                    for (int h = 0; h < tmpGeneral.Hospitals; h++)
                                    {
                                        for (int d = 0; d < tmpGeneral.HospitalWard; d++)
                                        {
                                            if (tmphospitalInfos[h].Hospital_dw[dIndex][d])
                                            {
                                                Hosp = h;
                                            }
                                        }

                                    }
                                }
                            }
                            if (Gr >= 0 && Hosp >= 0)
                            {
                                break;
                            }
                            else
                            {
                                dIndex = random_.Next(0, tmpGeneral.Disciplines);
                            }

                        }

                        tmpinternInfos[i].Fulfilled_dhp[dIndex][Hosp][Pr] = true;

                    }
                }

                // oversea disciplines happens for 10-20 percent of students
                double fhPrcntStudent = instancesetting.overseaHosp;

                // region 
                for (int r = 0; r < tmpGeneral.Region; r++)
                {
                    tmpinternInfos[i].TransferredTo_r[r] = random_.NextDouble() < 0.5;
                }
                totalTime = tmpGeneral.TimePriods - totalTime;
                if (totalTime < 0)
                {
                    totalTime = 0;
                }
                totalTime = random_.Next(0, totalTime);
                int startInd = random_.Next(0, tmpGeneral.TimePriods);
                for (int t = startInd; t < totalTime && t < tmpGeneral.TimePriods; t++)
                {
                    tmpinternInfos[i].Ave_t[t] = false;
                }
                for (int h = 0; h < tmpGeneral.Hospitals; h++)
                {
                    tmpinternInfos[i].Prf_h[h] = random_.Next(1, instancesetting.PrfMaxValue);
                }
                tmpinternInfos[i].wieght_ch = -random_.Next(1, instancesetting.PrfMaxValue);
                if (tmpTrainingPr[tmpinternInfos[i].ProgramID].DiscChangeInOneHosp == 1)
                {
                    tmpinternInfos[i].wieght_ch = 0;
                }
                tmpinternInfos[i].wieght_d = random_.Next(1, instancesetting.PrfMaxValue);
                tmpinternInfos[i].wieght_cns = random_.Next(1, instancesetting.PrfMaxValue);
                tmpinternInfos[i].wieght_h = random_.Next(1, instancesetting.PrfMaxValue);
                tmpinternInfos[i].wieght_w = -random_.Next(1, instancesetting.PrfMaxValue);
                // real case
                tmpinternInfos[i].ShouldattendInGr_g[0] = 7;
                tmpinternInfos[i].ShouldattendInGr_g[1] = 1;
                tmpinternInfos[i].ShouldattendInGr_g[2] = 1;
                tmpinternInfos[i].ShouldattendInGr_g[3] = 1;
            }

            // create Region info
            tmpRegionInfos = new RegionInfo[tmpGeneral.Region];
            for (int r = 0; r < tmpGeneral.Region; r++)
            {
                tmpRegionInfos[r] = new RegionInfo(tmpGeneral.TimePriods);
                tmpRegionInfos[r].Name = "Reg_" + r;
                tmpRegionInfos[r].SQLID = r;
                int MIN = random_.Next(0, 2);
                MIN = 5;
                for (int t = 0; t < tmpGeneral.TimePriods; t++)
                {
                    tmpRegionInfos[r].AvaAcc_t[t] = MIN;
                }
            }

            //Algorithm Settings
            tmpalgorithmSettings = new AlgorithmSettings();
            tmpalgorithmSettings.BPTime = 7200;
            tmpalgorithmSettings.MasterTime = 100;
            tmpalgorithmSettings.MIPTime = 7200;
            tmpalgorithmSettings.RCepsi = 0.000001;
            tmpalgorithmSettings.RHSepsi = 0.000001;
            tmpalgorithmSettings.SubTime = 600;
            tmpalgorithmSettings.NodeTime = 3600;
            tmpalgorithmSettings.BigM = 40000;
            tmpalgorithmSettings.MotivationBM = tmpalgorithmSettings.BigM / 100;
            tmpalgorithmSettings.bucketBasedImpPercentage = 0.25;
            tmpalgorithmSettings.internBasedImpPercentage = 0.25;



        }

        public void WriteInstance(string location, string name)
        {
            TextWriter tw = new StreamWriter(location + name + ".txt");
            string strline;
            strline = "// Interns Disciplines Hospitals TimePriods TrPrograms HospitalWards DisciplineGr Region";

            //write the general Data
            tw.WriteLine(strline);
            tw.WriteLine(tmpGeneral.Interns + " " + tmpGeneral.Disciplines
                        + " " + tmpGeneral.Hospitals + " " + tmpGeneral.TimePriods
                        + " " + tmpGeneral.TrainingPr + " " + tmpGeneral.HospitalWard
                        + " " + tmpGeneral.DisciplineGr + " " + tmpGeneral.Region);
            tw.WriteLine();
            //write training program info
            strline = "// PrName Year WeightPrf Obj_SumDesi Obj_MinDesi Obj_ResCap Obj_EmrCap Obj_NUsedAcc AllowedChange [p]";
            tw.WriteLine(strline);
            for (int p = 0; p < tmpGeneral.TrainingPr; p++)
            {
                tw.WriteLine(tmpTrainingPr[p].Name + " " + tmpTrainingPr[p].AcademicYear
                             + " " + tmpTrainingPr[p].weight_p + " " + tmpTrainingPr[p].CoeffObj_SumDesi
                             + " " + tmpTrainingPr[p].CoeffObj_MinDesi + " " + tmpTrainingPr[p].CoeffObj_ResCap
                             + " " + tmpTrainingPr[p].CoeffObj_EmrCap + " " + tmpTrainingPr[p].CoeffObj_NotUsedAcc + " " + tmpTrainingPr[p].DiscChangeInOneHosp + " " + tmpTrainingPr[p].CoeffObj_MINDem);
            }

            tw.WriteLine();

            strline = "// Preference for discipline [p][d]";
            tw.WriteLine(strline);
            for (int p = 0; p < tmpGeneral.TrainingPr; p++)
            {
                for (int d = 0; d < tmpGeneral.Disciplines; d++)
                {
                    tw.Write(tmpTrainingPr[p].Prf_d[d] + " ");
                }
                tw.WriteLine();
            }
            tw.WriteLine();

            strline = "// Involved discipline  [p][g][d]";
            tw.WriteLine(strline);
            for (int p = 0; p < tmpGeneral.TrainingPr; p++)
            {
                strline = "// Program  " + p;
                tw.WriteLine(strline);
                for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
                {
                    for (int d = 0; d < tmpGeneral.Disciplines; d++)
                    {
                        tw.Write((tmpTrainingPr[p].InvolvedDiscipline_gd[g][d] == true ? 1 : 0).ToString() + " ");
                    }
                    tw.WriteLine();
                }

            }
            tw.WriteLine();

            //strline = "// Also Known As  [p]: [d][D]";
            //tw.WriteLine(strline);
            //for (int p = 0; p < tmpGeneral.TrainingPr; p++)
            //{
            //    strline = "// Program  " + p;
            //    tw.WriteLine(strline);
            //    for (int dd = 0; dd < tmpGeneral.Disciplines; dd++)
            //    {
            //        for (int d = 0; d < tmpGeneral.Disciplines; d++)
            //        {
            //            tw.Write((tmpTrainingPr[p].AKA_dD[d][dd] == true ? 1 : 0).ToString() + " ");
            //        }
            //        tw.WriteLine();
            //    }

            //}
            //tw.WriteLine();

            //strline = "// Consecutive  [p]: [d][D]";
            //tw.WriteLine(strline);
            //for (int p = 0; p < tmpGeneral.TrainingPr; p++)
            //{
            //    strline = "// Program  " + p;
            //    tw.WriteLine(strline);
            //    for (int dd = 0; dd < tmpGeneral.Disciplines; dd++)
            //    {
            //        for (int d = 0; d < tmpGeneral.Disciplines; d++)
            //        {
            //            tw.Write((tmpTrainingPr[p].cns_dD[d][dd]).ToString() + " ");
            //        }
            //        tw.WriteLine();
            //    }

            //}
            //tw.WriteLine();

            // write Hospital Info 
            strline = "// Involved discipline in ward in hospital [h]  =>[w][d]";
            tw.WriteLine(strline);
            for (int h = 0; h < tmpGeneral.Hospitals; h++)
            {
                tw.WriteLine("Hospital" + h);
                for (int w = 0; w < tmpGeneral.HospitalWard; w++)
                {
                    for (int d = 0; d < tmpGeneral.Disciplines; d++)
                    {
                        if (tmphospitalInfos[h].Hospital_dw[d][w])
                        {
                            tw.Write(1 + " ");
                        }
                        else
                        {
                            tw.Write(0 + " ");
                        }
                    }
                    tw.WriteLine();
                }


            }

            tw.WriteLine();
            for (int h = 0; h < tmpGeneral.Hospitals; h++)
            {
                strline = "// Max Demand for ward in each time period [t][w] in Hospital" + h;
                tw.WriteLine(strline);
                for (int t = 0; t < tmpGeneral.TimePriods; t++)
                {
                    for (int d = 0; d < tmpGeneral.HospitalWard; d++)
                    {
                        tw.Write(tmphospitalInfos[h].HospitalMaxDem_tw[t][d] + " ");
                    }
                    tw.WriteLine();
                }
            }

            tw.WriteLine();
            for (int h = 0; h < tmpGeneral.Hospitals; h++)
            {
                strline = "// Min Demand for ward in each time period [t][w] in Hospital" + h;
                tw.WriteLine(strline);
                for (int t = 0; t < tmpGeneral.TimePriods; t++)
                {
                    for (int d = 0; d < tmpGeneral.HospitalWard; d++)
                    {
                        if (tmphospitalInfos[h].HospitalMinDem_tw[t][d] > 0)
                        {
                            tw.Write(tmphospitalInfos[h].HospitalMinDem_tw[t][d] + " ");
                        }
                        else
                        {
                            tw.Write(0 + " ");
                        }
                    }
                    tw.WriteLine();
                }
            }

            tw.WriteLine();
            for (int h = 0; h < tmpGeneral.Hospitals; h++)
            {
                strline = "// Reserved Demand for ward in each time period [t][w] in Hospital" + h;
                tw.WriteLine(strline);
                for (int t = 0; t < tmpGeneral.TimePriods; t++)
                {
                    for (int d = 0; d < tmpGeneral.HospitalWard; d++)
                    {
                        if (tmphospitalInfos[h].ReservedCap_tw[t][d] > 0)
                        {
                            tw.Write(tmphospitalInfos[h].ReservedCap_tw[t][d] + " ");
                        }
                        else
                        {
                            tw.Write(0 + " ");
                        }
                    }
                    tw.WriteLine();
                }
            }

            tw.WriteLine();
            for (int h = 0; h < tmpGeneral.Hospitals; h++)
            {
                strline = "// Emergency Demand for ward in each time period [t][w] in Hospital" + h;
                tw.WriteLine(strline);
                for (int t = 0; t < tmpGeneral.TimePriods; t++)
                {
                    for (int d = 0; d < tmpGeneral.HospitalWard; d++)
                    {
                        if (tmphospitalInfos[h].EmergencyCap_tw[t][d] > 0)
                        {
                            tw.Write(tmphospitalInfos[h].EmergencyCap_tw[t][d] + " ");
                        }
                        else
                        {
                            tw.Write(0 + " ");
                        }
                    }
                    tw.WriteLine();
                }
            }

            tw.WriteLine();
            strline = "// Yearly Max demand [h][w]";
            tw.WriteLine(strline);
            for (int h = 0; h < tmpGeneral.Hospitals; h++)
            {
                for (int d = 0; d < tmpGeneral.HospitalWard; d++)
                {
                    if (tmphospitalInfos[h].HospitalMaxYearly_w[d] > 0)
                    {
                        tw.Write(tmphospitalInfos[h].HospitalMaxYearly_w[d] + " ");
                    }
                    else
                    {
                        tw.Write(0 + " ");
                    }
                }
                tw.WriteLine();

            }

            tw.WriteLine();
            strline = "// Yearly Min demand [h][w]";
            tw.WriteLine(strline);
            for (int h = 0; h < tmpGeneral.Hospitals; h++)
            {
                for (int d = 0; d < tmpGeneral.HospitalWard; d++)
                {
                    if (tmphospitalInfos[h].HospitalMinYearly_w[d] > 0)
                    {
                        tw.Write(tmphospitalInfos[h].HospitalMinYearly_w[d] + " ");
                    }
                    else
                    {
                        tw.Write(0 + " ");
                    }
                }
                tw.WriteLine();

            }

            tw.WriteLine();
            strline = "// located in regions [h][r]";
            tw.WriteLine(strline);
            for (int h = 0; h < tmpGeneral.Hospitals; h++)
            {
                for (int t = 0; t < tmpGeneral.Region; t++)
                {
                    if (tmphospitalInfos[h].InToRegion_r[t])
                    {
                        tw.Write(1 + " ");
                    }
                    else
                    {
                        tw.Write(0 + " ");
                    }


                } tw.WriteLine();
            }

            tw.WriteLine();



            // write Discipline Info
            strline = "// Discipline duration different in Program [d][p]";
            tw.WriteLine(strline);
            for (int d = 0; d < tmpGeneral.Disciplines; d++)
            {
                for (int p = 0; p < tmpGeneral.TrainingPr; p++)
                {
                    tw.Write(tmpdisciplineInfos[d].Duration_p[p] + " ");
                }
                tw.WriteLine();
            }
            tw.WriteLine();
            strline = "// Course credit different in Program [d][p]";
            tw.WriteLine(strline);
            for (int d = 0; d < tmpGeneral.Disciplines; d++)
            {
                for (int p = 0; p < tmpGeneral.TrainingPr; p++)
                {
                    tw.Write(tmpdisciplineInfos[d].CourseCredit_p[p] + " ");
                }
                tw.WriteLine();
            }
            tw.WriteLine();


            for (int d = 0; d < tmpGeneral.Disciplines; d++)
            {
                strline = "// required skill[d'][p] for discipline " + d;
                tw.WriteLine(strline);
                for (int dd = 0; dd < tmpGeneral.Disciplines; dd++)
                {
                    for (int p = 0; p < tmpGeneral.TrainingPr; p++)
                    {
                        if (tmpdisciplineInfos[d].Skill4D_dp[dd][p])
                        {
                            tw.Write(1 + " ");
                        }
                        else
                        {
                            tw.Write(0 + " ");
                        }
                    }
                    tw.WriteLine();
                }

                tw.WriteLine();
            }
            tw.WriteLine();


            //write Intern info		
            strline = "// InternName Program isProspective W_disc W_hosp W_change W_wait  [i][w]";
            tw.WriteLine(strline);
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                tw.WriteLine(i + " " + tmpinternInfos[i].ProgramID + " " + tmpinternInfos[i].isProspective + " " + tmpinternInfos[i].wieght_d + " " + tmpinternInfos[i].wieght_h + " " + tmpinternInfos[i].wieght_ch + " " + tmpinternInfos[i].wieght_w + " " + tmpinternInfos[i].wieght_cns);

            }

            tw.WriteLine();

            strline = "// Discipline list";
            tw.WriteLine(strline);
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                strline = "// Intern " + i + ": [g][d]";
                tw.WriteLine(strline);
                for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
                {
                    for (int d = 0; d < tmpGeneral.Disciplines; d++)
                    {
                        if (tmpinternInfos[i].DisciplineList_dg[d][g])
                        {
                            tw.Write(1 + " ");
                        }
                        else
                        {
                            tw.Write(0 + " ");
                        }
                    }
                    tw.WriteLine();
                }

            }
            tw.WriteLine();

            strline = "// total discipline should be fulfilled from each group [i][g]";
            tw.WriteLine(strline);
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                for (int d = 0; d < tmpGeneral.DisciplineGr; d++)
                {
                    tw.Write(tmpinternInfos[i].ShouldattendInGr_g[d] + " ");
                }
                tw.WriteLine();
            }
            tw.WriteLine();

            strline = "// Oversea request i - d - t - RequiredDiscipline";
            tw.WriteLine(strline);
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                for (int d = 0; d < tmpGeneral.Disciplines; d++)
                {
                    for (int t = 0; t < tmpGeneral.TimePriods; t++)
                    {
                        if (tmpinternInfos[i].OverSea_dt[d][t])
                        {
                            tw.WriteLine(i + " " + d + " " + t);

                        }

                    }
                }

            }
            tw.WriteLine("---");

            strline = "// Oversea Discipline requirement  [i oversea] [d1 d2 d3]";
            tw.WriteLine(strline);
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                bool OVERSEA = false;
                for (int d = 0; d < tmpGeneral.Disciplines; d++)
                {
                    for (int t = 0; t < tmpGeneral.TimePriods; t++)
                    {
                        if (tmpinternInfos[i].OverSea_dt[d][t])
                        {
                            OVERSEA = true;
                        }

                    }
                }
                for (int d = 0; d < tmpGeneral.Disciplines && OVERSEA; d++)
                {
                    if (tmpinternInfos[i].FHRequirment_d[d])
                    {
                        tw.Write(1 + " ");
                    }
                    else
                    {
                        tw.Write(0 + " ");
                    }
                }
                if (OVERSEA)
                {
                    tw.WriteLine();
                }
            }
            tw.WriteLine();

            strline = "// Transfered to region [i][r]";
            tw.WriteLine(strline);
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                for (int d = 0; d < tmpGeneral.Region; d++)
                {
                    if (tmpinternInfos[i].TransferredTo_r[d])
                    {
                        tw.Write(1 + " ");
                    }
                    else
                    {
                        tw.Write(0 + " ");
                    }
                }
                tw.WriteLine();
            }
            tw.WriteLine();


            strline = "// Ability";
            tw.WriteLine(strline);
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                strline = "// Ability " + i + " [d][h]";
                tw.WriteLine(strline);
                for (int h = 0; h < tmpGeneral.Hospitals; h++)
                {
                    for (int d = 0; d < tmpGeneral.Disciplines; d++)
                    {
                        if (tmpinternInfos[i].Abi_dh[d][h])
                        {
                            tw.Write(1 + " ");
                        }
                        else
                        {
                            tw.Write(0 + " ");
                        }
                    }
                    tw.WriteLine();
                }

            }
            tw.WriteLine();

            strline = "// Fulfilled disciplines i-d-h-p";
            tw.WriteLine(strline);
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                for (int d = 0; d < tmpGeneral.Disciplines; d++)
                {
                    for (int h = 0; h < tmpGeneral.Hospitals; h++)
                    {
                        for (int p = 0; p < tmpGeneral.TrainingPr; p++)
                        {
                            if (tmpinternInfos[i].Fulfilled_dhp[d][h][p])
                            {
                                tw.WriteLine(i + " " + d + " " + h + " " + p);
                            }
                        }

                    }
                }
            }
            tw.WriteLine("---");

            strline = "// Preference for discipline [i][d]";
            tw.WriteLine(strline);
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                for (int d = 0; d < tmpGeneral.Disciplines; d++)
                {
                    tw.Write(tmpinternInfos[i].Prf_d[d] + " ");


                }
                tw.WriteLine();
            }
            tw.WriteLine();

            strline = "// Preference for hospital [i][h]";
            tw.WriteLine(strline);
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                for (int d = 0; d < tmpGeneral.Hospitals; d++)
                {
                    tw.Write(tmpinternInfos[i].Prf_h[d] + " ");

                }
                tw.WriteLine();
            }
            tw.WriteLine();

            strline = "// Availability [i][t]";
            tw.WriteLine(strline);
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                for (int d = 0; d < tmpGeneral.TimePriods; d++)
                {
                    if (tmpinternInfos[i].Ave_t[d])
                    {
                        tw.Write(1 + " ");
                    }
                    else
                    {
                        tw.Write(0 + " ");
                    }

                }
                tw.WriteLine();
            }
            tw.WriteLine();

            // write Region info
            strline = "// RegionNme SqlID";
            tw.WriteLine(strline);
            for (int i = 0; i < tmpGeneral.Region; i++)
            {
                tw.WriteLine(tmpRegionInfos[i].Name + " " + tmpRegionInfos[i].SQLID);
            }
            tw.WriteLine();

            strline = "// Region Accommodation [r][t]";
            tw.WriteLine(strline);
            for (int i = 0; i < tmpGeneral.Region; i++)
            {
                for (int d = 0; d < tmpGeneral.TimePriods; d++)
                {
                    tw.Write(tmpRegionInfos[i].AvaAcc_t[d] + " ");


                }
                tw.WriteLine();
            }
            tw.WriteLine();


            // write algoritm setting
            strline = "// Algorithm Settings MIPtime BPtime Nodetime Mastertime Subtime RCepsi RCepsi BigM impBucket% impIntern%";
            tw.WriteLine(strline);
            tw.WriteLine(tmpalgorithmSettings.MIPTime + " " + tmpalgorithmSettings.BPTime + " " + tmpalgorithmSettings.NodeTime + " " + tmpalgorithmSettings.MasterTime + " " + tmpalgorithmSettings.SubTime + " " + tmpalgorithmSettings.RHSepsi + " " + tmpalgorithmSettings.RCepsi + " " + tmpalgorithmSettings.BigM + " " + tmpalgorithmSettings.bucketBasedImpPercentage + " " + tmpalgorithmSettings.internBasedImpPercentage);
            tw.WriteLine();


            // write Instance setting 
            tw.WriteLine("Ratio_wd: " + instancesetting.R_wd);
            tw.WriteLine("Ratio_gk: " + String.Join(", ", instancesetting.R_gk_g.Select(p => p.ToString()).ToArray()));
            tw.WriteLine("Ratio_wh: " + instancesetting.R_wh);
            tw.WriteLine("R_sk: " + instancesetting.R_Trel);
            tw.WriteLine("R_Ov: " + instancesetting.overseaHosp);
            tw.WriteLine("Dist_p: " + String.Join(", ", instancesetting.DisciplineDistribution_p.Select(p => p.ToString()).ToArray()));
            tw.WriteLine("Dist_g: " + String.Join(", ", instancesetting.DisciplineDistribution_g.Select(p => p.ToString()).ToArray()));
            tw.WriteLine("Ratio_muG: " + instancesetting.R_muDg);
            tw.WriteLine("Ratio_muP: " + instancesetting.R_muDp);
            tw.WriteLine("I: " + tmpGeneral.Interns);
            tw.WriteLine("H: " + tmpGeneral.Hospitals);
            tw.WriteLine("D: " + tmpGeneral.Disciplines);


            tw.Close();
        }

        public void feasibleSolution()
        {
            Random random = new Random();
            timelineHospMax_twh = new int[tmpGeneral.TimePriods][][];
            YearlyHospMax_wh = new int[tmpGeneral.HospitalWard][];
            for (int t = 0; t < tmpGeneral.TimePriods; t++)
            {
                timelineHospMax_twh[t] = new int[tmpGeneral.HospitalWard][];
                for (int w = 0; w < tmpGeneral.HospitalWard; w++)
                {
                    timelineHospMax_twh[t][w] = new int[tmpGeneral.Hospitals];
                    YearlyHospMax_wh[w] = new int[tmpGeneral.Hospitals];
                    for (int h = 0; h < tmpGeneral.Hospitals; h++)
                    {
                        timelineHospMax_twh[t][w][h] = tmphospitalInfos[h].HospitalMaxDem_tw[t][w] + tmphospitalInfos[h].ReservedCap_tw[t][w];
                        YearlyHospMax_wh[w][h] = tmphospitalInfos[h].HospitalMaxYearly_w[w];

                    }
                }
            }

            new ArrayInitializer().CreateArray(ref timeline_it, tmpGeneral.Interns, tmpGeneral.TimePriods, false);
            new ArrayInitializer().CreateArray(ref discipline_id, tmpGeneral.Interns, tmpGeneral.Disciplines, false);
            new ArrayInitializer().CreateArray(ref hospital_ih, tmpGeneral.Interns, tmpGeneral.Hospitals, 0);
            new ArrayInitializer().CreateArray(ref X_itdh, tmpGeneral.Interns, tmpGeneral.TimePriods, tmpGeneral.Disciplines, tmpGeneral.Hospitals + 1, false);

            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                int t = 0;
                for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
                {
                    for (int d = 0; d < tmpGeneral.Disciplines; d++)
                    {
                        int infinityLoop = 0;
                        int disIndex = random.Next(0, tmpGeneral.Disciplines);

                        infinityLoop = 0;
                        while (!tmpTrainingPr[tmpinternInfos[i].ProgramID].InvolvedDiscipline_gd[g][disIndex] || discipline_id[i][disIndex])
                        {
                            disIndex = random.Next(0, tmpGeneral.Disciplines);
                            infinityLoop++;
                            if (infinityLoop == tmpGeneral.Disciplines)
                            {
                                break;
                            }
                        }
                        if (infinityLoop == tmpGeneral.Disciplines)
                        {
                            continue;
                        }
                        bool cando = true;
                        if (tmpdisciplineInfos[disIndex].requiresSkill_p[tmpinternInfos[i].ProgramID])
                        {
                            cando = true;
                            for (int dd = 0; dd < tmpGeneral.Disciplines && cando; dd++)
                            {
                                if (tmpdisciplineInfos[disIndex].Skill4D_dp[dd][tmpinternInfos[i].ProgramID])
                                {
                                    if (!discipline_id[i][dd])
                                    {
                                        disIndex = dd;
                                        cando = false;
                                    }
                                }
                            }
                        }
                        // AKA 
                        for (int dd = 0; dd < tmpGeneral.Disciplines && cando; dd++)
                        {
                            if (tmpTrainingPr[tmpinternInfos[i].ProgramID].AKA_dD[dd][disIndex] && discipline_id[i][dd])
                            {
                                cando = false;
                            }
                        }
                        int Hindex = random.Next(0, tmpGeneral.Hospitals);
                        // check if there is a ward for this discipline in this hospital
                        infinityLoop = 0;
                        while (true)
                        {
                            bool thereIsWard = false;
                            int theW = -1;
                            for (int w = 0; w < tmpGeneral.HospitalWard; w++)
                            {
                                if (tmphospitalInfos[Hindex].Hospital_dw[disIndex][w] && hospital_ih[i][Hindex] < tmpTrainingPr[tmpinternInfos[i].ProgramID].DiscChangeInOneHosp)
                                {
                                    thereIsWard = true;
                                    theW = w;
                                    break;
                                }
                            }
                            // if he can be in the hospital
                            if (thereIsWard && (hospital_ih[i][Hindex] >= tmpTrainingPr[tmpinternInfos[i].ProgramID].DiscChangeInOneHosp || timelineHospMax_twh[t][theW][Hindex] == 0 || YearlyHospMax_wh[theW][Hindex] <= 0))
                            {
                                thereIsWard = false;
                            }
                            if (thereIsWard)
                            {
                                break;
                            }
                            else
                            {
                                Hindex = random.Next(0, tmpGeneral.Hospitals);
                                infinityLoop++;
                                if (infinityLoop == tmpGeneral.Hospitals)
                                {
                                    break;
                                }
                            }
                        }
                        if (infinityLoop == tmpGeneral.Hospitals)
                        {
                            continue;
                        }
                        for (int w = 0; w < tmpGeneral.HospitalWard; w++)
                        {
                            if (tmphospitalInfos[Hindex].Hospital_dw[disIndex][w])
                            {

                                if (timelineHospMax_twh[t][w][Hindex] > 0 && YearlyHospMax_wh[w][Hindex] > 0)
                                {
                                    bool theInternIsFree = true;
                                    if (t + tmpdisciplineInfos[disIndex].Duration_p[tmpinternInfos[i].ProgramID] > tmpGeneral.TimePriods - 1)
                                    {
                                        theInternIsFree = false;
                                    }
                                    for (int tt = t; tt < t + tmpdisciplineInfos[disIndex].Duration_p[tmpinternInfos[i].ProgramID] && theInternIsFree; tt++)
                                    {
                                        if (timelineHospMax_twh[tt][w][Hindex] <= 0 || timeline_it[i][tt])
                                        {
                                            theInternIsFree = false;
                                        }
                                    }
                                    // check the skills
                                    if (theInternIsFree)
                                    {
                                        for (int dd = 0; dd < tmpGeneral.Disciplines; dd++)
                                        {
                                            if (tmpdisciplineInfos[disIndex].Skill4D_dp[dd][tmpinternInfos[i].ProgramID])
                                            {
                                                if (!discipline_id[i][dd])
                                                {
                                                    theInternIsFree = false;
                                                }
                                            }
                                        }

                                    }
                                    if (theInternIsFree)
                                    {
                                        YearlyHospMax_wh[w][Hindex]--;
                                        for (int tt = t; tt < t + tmpdisciplineInfos[disIndex].Duration_p[tmpinternInfos[i].ProgramID]; tt++)
                                        {
                                            timelineHospMax_twh[tt][w][Hindex]--;
                                            timeline_it[i][tt] = true;
                                        }
                                        X_itdh[i][t][disIndex][Hindex] = true;
                                        hospital_ih[i][Hindex]++;
                                        t += tmpdisciplineInfos[disIndex].Duration_p[tmpinternInfos[i].ProgramID];
                                        discipline_id[i][disIndex] = true;
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }

        public void setInternsList()
        {
            Random random = new Random();
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                for (int d = 0; d < tmpGeneral.Disciplines; d++)
                {
                    if (discipline_id[i][d])
                    {
                        for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
                        {
                            if (tmpTrainingPr[tmpinternInfos[i].ProgramID].InvolvedDiscipline_gd[g][d])
                            {
                                tmpinternInfos[i].DisciplineList_dg[d][g] = true;
                                tmpinternInfos[i].ShouldattendInGr_g[g] += tmpdisciplineInfos[d].CourseCredit_p[tmpinternInfos[i].ProgramID];
                                tmpinternInfos[i].K_AllDiscipline += tmpdisciplineInfos[d].CourseCredit_p[tmpinternInfos[i].ProgramID];

                                break;
                            }
                        }
                    }
                }
            }
            // the extra added discipline to group
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                for (int d = 0; d < tmpGeneral.Disciplines; d++)
                {
                    for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
                    {
                        if (tmpTrainingPr[tmpinternInfos[i].ProgramID].InvolvedDiscipline_gd[g][d])
                        {
                            if (random.NextDouble() < 1 - instancesetting.R_gk_g[g])
                            {
                                tmpinternInfos[i].DisciplineList_dg[d][g] = true;
                            }
                        }
                    }
                }
            }
        }
        public void setInternsListRealLife()
        {
            Random random = new Random();
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                for (int d = 0; d < tmpGeneral.Disciplines; d++)
                {
                    for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
                    {
                        if (tmpTrainingPr[tmpinternInfos[i].ProgramID].InvolvedDiscipline_gd[g][d])
                        {
                            tmpinternInfos[i].DisciplineList_dg[d][g] = true;
                            //tmpinternInfos[i].ShouldattendInGr_g[g]++;
                            break;
                        }
                    }

                }
            }
        }
        public void setInternsOversea()
        {
            Random random = new Random();
            int[] theT_R = new int[tmpGeneral.TrainingPr];
            int[] theD_R = new int[tmpGeneral.TrainingPr];
            for (int p = 0; p < tmpGeneral.TrainingPr; p++)
            {
                theT_R[p] = -1;
                int theI = random.Next(0, tmpGeneral.Interns);
                while (tmpinternInfos[theI].ProgramID != p)
                {
                    theI = random.Next(0, tmpGeneral.Interns);
                }

                int theD = random.Next(0, tmpGeneral.Disciplines);
                int counter = 0;
                while (!discipline_id[theI][theD])
                {
                    counter++;
                    theD = random.Next(0, tmpGeneral.Disciplines);
                    if (counter == tmpGeneral.Disciplines)
                    {
                        break;
                    }
                }
                if (counter == tmpGeneral.Disciplines)
                {
                    p--;
                    continue;
                }
                theD_R[p] = theD;
            }

            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                if (discipline_id[i][theD_R[tmpinternInfos[i].ProgramID]])
                {
                    int theT = -1;
                    for (int t = 0; t < tmpGeneral.TimePriods && theT < 0; t++)
                    {
                        for (int h = 0; h < tmpGeneral.Hospitals && theT < 0; h++)
                        {
                            if (X_itdh[i][t][theD_R[tmpinternInfos[i].ProgramID]][h])
                            {
                                theT = t;
                            }
                        }
                    }
                    if (theT >= 0)
                    {
                        bool getOversea = false;
                        theT = random.Next(theT, tmpGeneral.TimePriods);
                        for (int t = theT + 1; t < tmpGeneral.TimePriods && !getOversea; t++)
                        {
                            for (int d = 0; d < tmpGeneral.Disciplines && !getOversea; d++)
                            {
                                for (int h = 0; h < tmpGeneral.Hospitals && !getOversea; h++)
                                {
                                    if (X_itdh[i][t][d][h])
                                    {
                                        if (random.NextDouble() < instancesetting.overseaHosp)
                                        {
                                            tmpinternInfos[i].OverSea_dt[d][t] = true;
                                            tmpinternInfos[i].FHRequirment_d[theD_R[tmpinternInfos[i].ProgramID]] = true;
                                            X_itdh[i][t][d][h] = false;
                                            X_itdh[i][t][d][tmpGeneral.Hospitals] = true;
                                        }
                                        getOversea = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public void setInternsOverseaRealLife()
        {
            Random random = new Random();
            int[] theT_R = new int[tmpGeneral.TrainingPr];
            int[] theD_R = new int[tmpGeneral.TrainingPr];
            for (int p = 0; p < tmpGeneral.TrainingPr; p++)
            {
                theT_R[p] = -1;
                int theI = random.Next(0, tmpGeneral.Interns);
                while (tmpinternInfos[theI].ProgramID != p)
                {
                    theI = random.Next(0, tmpGeneral.Interns);
                }

                int theD = random.Next(0, tmpGeneral.Disciplines);
                int counter = 0;
                while (!discipline_id[theI][theD])
                {
                    counter++;
                    theD = random.Next(0, tmpGeneral.Disciplines);
                    if (counter == tmpGeneral.Disciplines)
                    {
                        break;
                    }
                }
                if (counter == tmpGeneral.Disciplines)
                {
                    p--;
                    continue;
                }
                theD_R[p] = theD;
            }

            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                if (discipline_id[i][theD_R[tmpinternInfos[i].ProgramID]])
                {
                    int theT = -1;
                    for (int t = 0; t < tmpGeneral.TimePriods && theT < 0; t++)
                    {
                        for (int h = 0; h < tmpGeneral.Hospitals && theT < 0; h++)
                        {
                            if (X_itdh[i][t][theD_R[tmpinternInfos[i].ProgramID]][h])
                            {
                                theT = t;
                            }
                        }
                    }
                    if (true)
                    {
                        bool getOversea = false;
                        theT = random.Next(theT, tmpGeneral.TimePriods);
                        for (int t = theT + 1; t < tmpGeneral.TimePriods && !getOversea; t++)
                        {
                            for (int d = 0; d < tmpGeneral.Disciplines && !getOversea; d++)
                            {
                                for (int h = 0; h < tmpGeneral.Hospitals && !getOversea; h++)
                                {
                                    if (X_itdh[i][t][d][h])
                                    {
                                        if (random.NextDouble() < instancesetting.overseaHosp)
                                        {
                                            tmpinternInfos[i].OverSea_dt[d][t] = true;
                                            tmpinternInfos[i].FHRequirment_d[theD_R[tmpinternInfos[i].ProgramID]] = false;
                                        }
                                        getOversea = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public void WriteFeasibleSol(string location, string name)
        {
            AllData allData = new AllData();
            allData.AlgSettings = tmpalgorithmSettings;
            allData.Discipline = tmpdisciplineInfos;
            allData.General = tmpGeneral;
            allData.Hospital = tmphospitalInfos;
            allData.Intern = tmpinternInfos;
            allData.Region = tmpRegionInfos;
            allData.TrainingPr = tmpTrainingPr;

            FeasibleSolution = new OptimalSolution(allData);
            FeasibleSolution.copyRosters(X_itdh);
            for (int i = 0; i < tmpGeneral.Hospitals; i++)
            {
                for (int d = 0; d < tmpGeneral.Disciplines; d++)
                {
                    for (int t = 0; t < tmpGeneral.TimePriods; t++)
                    {
                        if (tmpinternInfos[i].OverSea_dt[d][t])
                        {
                            for (int h = 0; h < tmpGeneral.Hospitals; h++)
                            {
                                if (X_itdh[i][t][d][h])
                                {
                                    FeasibleSolution.Intern_itdh[i][t][d][h] = false;
                                    FeasibleSolution.Intern_itdh[i][t][d][tmpGeneral.Hospitals] = true;
                                }
                            }
                        }
                    }
                }
            }

            FeasibleSolution.WriteSolution(location, name);
        }

        public void ChangeObjCoeff(string location, string name)
        {
            string[] nameCoeff = new string[6] { "Alpha", "Beta", "Gamma", "Delta", "Lambda", "Noe" };
            string[] level = new string[4] { "00", "01", "05", "10" };
            //double[][] weight = new double[24][]{new double[6] { 0.00, 0.20, 0.20, 0.20, 0.20, 0.20 },
            //									 new double[6] { 0.17, 0.17, 0.17, 0.17, 0.17, 0.17 },
            //									 new double[6] { 0.50, 0.10, 0.10, 0.10, 0.10, 0.10 },
            //									 new double[6] { 0.67, 0.07, 0.07, 0.07, 0.07, 0.07 },
            //									 new double[6] { 0.20, 0.00, 0.20, 0.20, 0.20, 0.20 },
            //									 new double[6] { 0.17, 0.17, 0.17, 0.17, 0.17, 0.17 },
            //									 new double[6] { 0.10, 0.50, 0.10, 0.10, 0.10, 0.10 },
            //									 new double[6] { 0.07, 0.67, 0.07, 0.07, 0.07, 0.07 },
            //									 new double[6] { 0.20, 0.20, 0.00, 0.20, 0.20, 0.20 },
            //									 new double[6] { 0.17, 0.17, 0.17, 0.17, 0.17, 0.17 },
            //									 new double[6] { 0.10, 0.10, 0.50, 0.10, 0.10, 0.10 },
            //									 new double[6] { 0.07, 0.07, 0.67, 0.07, 0.07, 0.07 },
            //									 new double[6] { 0.20, 0.20, 0.20, 0.00, 0.20, 0.20 },
            //									 new double[6] { 0.17, 0.17, 0.17, 0.17, 0.17, 0.17 },
            //									 new double[6] { 0.10, 0.10, 0.10, 0.50, 0.10, 0.10 },
            //									 new double[6] { 0.07, 0.07, 0.07, 0.67, 0.07, 0.07 },
            //									 new double[6] { 0.20, 0.20, 0.20, 0.20, 0.00, 0.20 },
            //									 new double[6] { 0.17, 0.17, 0.17, 0.17, 0.17, 0.17 },
            //									 new double[6] { 0.10, 0.10, 0.10, 0.10, 0.50, 0.10 },
            //									 new double[6] { 0.07, 0.07, 0.07, 0.07, 0.67, 0.07 },
            //									 new double[6] { 0.20, 0.20, 0.20, 0.20, 0.20, 0.00 },
            //									 new double[6] { 0.17, 0.17, 0.17, 0.17, 0.17, 0.17 },
            //									 new double[6] { 0.10, 0.10, 0.10, 0.10, 0.10, 0.50 },
            //									 new double[6] { 0.07, 0.07, 0.07, 0.07, 0.07, 0.67 }};

            //for (int i = 0; i < nameCoeff.Count(); i++)
            //{
            //    for (int l = 0; l < level.Count(); l++)
            //    {
            //        for (int p = 0; p < tmpGeneral.TrainingPr; p++)
            //        {
            //            tmpTrainingPr[p].CoeffObj_SumDesi = weight[i * level.Count() + l][0];
            //            tmpTrainingPr[p].CoeffObj_MinDesi = weight[i * level.Count() + l][1];
            //            tmpTrainingPr[p].CoeffObj_ResCap = weight[i * level.Count() + l][2];
            //            tmpTrainingPr[p].CoeffObj_EmrCap = weight[i * level.Count() + l][3];
            //            tmpTrainingPr[p].CoeffObj_NotUsedAcc = weight[i * level.Count() + l][4];
            //            tmpTrainingPr[p].CoeffObj_MINDem = weight[i * level.Count() + l][5];
            //        }
            //        string NewLoc = location + nameCoeff[i] + level[l] + "\\";
            //        if (!Directory.Exists(NewLoc))
            //        {
            //            Directory.CreateDirectory(NewLoc);
            //        }
            //        WriteInstance(NewLoc, name);
            //    }
            //}
            double[] Wlevels = new double[] { 0, 1, 5, 10 };
            int expr = (int)Math.Pow(Wlevels.Length, nameCoeff.Length);
            double[][] weight = new double[expr][];

            int counter = -1;
            for (int i = 0; i < Wlevels.Length; i++)
            {
                for (int j = 0; j < Wlevels.Length; j++)
                {
                    for (int k = 0; k < Wlevels.Length; k++)
                    {
                        for (int l = 0; l < Wlevels.Length; l++)
                        {
                            for (int m = 0; m < Wlevels.Length; m++)
                            {
                                for (int n = 0; n < Wlevels.Length; n++)
                                {

                                    double alpha = Wlevels[i];
                                    double Beta = Wlevels[j];
                                    double Gamma = Wlevels[k];
                                    double Delta = Wlevels[l];
                                    double Lambda = Wlevels[m];
                                    double Noe = Wlevels[n];
                                     alpha = Wlevels[i];
                                     Beta = Wlevels[j];
                                     Gamma = Wlevels[k];
                                     Delta = Wlevels[l];
                                     Lambda = Wlevels[m];
                                     Noe = Wlevels[n];
                                    double sum = alpha + Beta + Gamma + Delta + Lambda + Noe;
                                    if (sum == 0)
                                    {
                                        sum = 1;
                                    }
                                    counter++;
                                    weight[counter] = new double[] { alpha / sum, Beta / sum, Gamma / sum, Delta / sum, Lambda / sum, Noe / sum };
                                }
                            }
                        }
                    }
                }
            }
           StreamWriter deescription = new StreamWriter(location + "Description.txt");
            for (int i = 0; i < weight.Count(); i++)
            {
                for (int p = 0; p < tmpGeneral.TrainingPr; p++)
                {
                    tmpTrainingPr[p].CoeffObj_SumDesi =    Math.Round(weight[i][0],2);
                    tmpTrainingPr[p].CoeffObj_MinDesi =    Math.Round(weight[i][1],2);
                    tmpTrainingPr[p].CoeffObj_ResCap =     Math.Round(weight[i][2],2);
                    tmpTrainingPr[p].CoeffObj_EmrCap =     Math.Round(weight[i][3],2);
                    tmpTrainingPr[p].CoeffObj_NotUsedAcc = Math.Round(weight[i][4],2);
                    tmpTrainingPr[p].CoeffObj_MINDem =     Math.Round(weight[i][5],2);
                }
                string NewLoc = location + "G_" + (i + 1).ToString() + "\\";
                if (!Directory.Exists(NewLoc))
                {
                    Directory.CreateDirectory(NewLoc);
                }
                WriteInstance(NewLoc, name);               


                deescription.WriteLine(Math.Round(weight[i][0], 2) 
                    + "\t"+ Math.Round(weight[i][1], 2) + "\t" + Math.Round(weight[i][2], 2) + "\t" 
                    + Math.Round(weight[i][3], 2) + "\t" + Math.Round(weight[i][4], 2) + "\t" + Math.Round(weight[i][5], 2));
            }
            deescription.Close();
        }

        public void ChangeObjCoeffRandom(string location, string name)
        {
            string[] nameCoeff = new string[6] { "Alpha", "Beta", "Gamma", "Delta", "Lambda", "Noe" };
            string[] level = new string[4] { "00", "01", "05", "10" };
            double[] Wlevels = new double[] { 0, 1, 5, 10 };
            int expr = (int)Math.Pow(Wlevels.Length, nameCoeff.Length);
            double[][] weight = new double[expr][];

            int counter = -1;
            for (int i = 0; i < Wlevels.Length; i++)
            {
                for (int j = 0; j < Wlevels.Length; j++)
                {
                    for (int k = 0; k < Wlevels.Length; k++)
                    {
                        for (int l = 0; l < Wlevels.Length; l++)
                        {
                            for (int m = 0; m < Wlevels.Length; m++)
                            {
                                for (int n = 0; n < Wlevels.Length; n++)
                                {

                                    double alpha = Wlevels[i];
                                    double Beta = Wlevels[j];
                                    double Gamma = Wlevels[k];
                                    double Delta = Wlevels[l];
                                    double Lambda = Wlevels[m];
                                    double Noe = Wlevels[n];
                                    alpha = Wlevels[i];
                                    Beta = Wlevels[j];
                                    Gamma = Wlevels[k];
                                    Delta = Wlevels[l];
                                    Lambda = Wlevels[m];
                                    Noe = Wlevels[n];
                                    double sum = alpha + Beta + Gamma + Delta + Lambda + Noe;
                                    if (sum == 0)
                                    {
                                        sum = 1;
                                    }
                                    counter++;
                                    weight[counter] = new double[] { alpha / sum, Beta / sum, Gamma / sum, Delta / sum, Lambda / sum, Noe / sum };
                                }
                            }
                        }
                    }
                }
            }
            StreamWriter deescription = new StreamWriter(location + "Description.txt");
            for (int i = 0; i < weight.Count(); i++)
            {
                for (int p = 0; p < tmpGeneral.TrainingPr; p++)
                {
                    tmpTrainingPr[p].CoeffObj_SumDesi = Math.Round(weight[i][0], 2);
                    tmpTrainingPr[p].CoeffObj_MinDesi = Math.Round(weight[i][1], 2);
                    tmpTrainingPr[p].CoeffObj_ResCap = Math.Round(weight[i][2], 2);
                    tmpTrainingPr[p].CoeffObj_EmrCap = Math.Round(weight[i][3], 2);
                    tmpTrainingPr[p].CoeffObj_NotUsedAcc = Math.Round(weight[i][4], 2);
                    tmpTrainingPr[p].CoeffObj_MINDem = Math.Round(weight[i][5], 2);
                }
                string NewLoc = location + "G_" + (i + 1).ToString() + "\\";
                if (!Directory.Exists(NewLoc))
                {
                    Directory.CreateDirectory(NewLoc);
                }
                WriteInstance(NewLoc, name);


                deescription.WriteLine(Math.Round(weight[i][0], 2)
                    + "\t" + Math.Round(weight[i][1], 2) + "\t" + Math.Round(weight[i][2], 2) + "\t"
                    + Math.Round(weight[i][3], 2) + "\t" + Math.Round(weight[i][4], 2) + "\t" + Math.Round(weight[i][5], 2));
            }
            deescription.Close();
        }
        public void ChangeDesCoeff(string location, string name)
		{
			string[] nameCoeff = new string[] { "W_h", "W_d", "W_p", "W_c", "W_w", "W_cs" };
			string[] level = new string[4] { "00", "01", "05", "10" };
			int[][] weight = new int[][]{new int[]{0 , 1 , 1 , 1 , 1 , 1},
										   new int[]{1 , 1 , 1 , 1 , 1 , 1},
										   new int[]{5 , 1 , 1 , 1 , 1 , 1},
										   new int[]{10 , 1 , 1 , 1 , 1, 1},
										   new int[]{1 , 0 , 1 , 1 , 1 , 1},
										   new int[]{1 , 1 , 1 , 1 , 1 , 1},
										   new int[]{1 , 5 , 1 , 1 , 1 , 1},
										   new int[]{1 , 10 , 1 , 1 , 1, 1},
										   new int[]{1 , 1 , 0 , 1 , 1 , 1},
										   new int[]{1 , 1 , 1 , 1 , 1 , 1},
										   new int[]{1 , 1 , 5 , 1 , 1 , 1},
										   new int[]{1 , 1 , 10 , 1 , 1, 1},
										   new int[]{1 , 1 , 1 , 0 , 1 , 1},
										   new int[]{1 , 1 , 1 , 1 , 1 , 1},
										   new int[]{1 , 1 , 1 , 5 , 1 , 1},
										   new int[]{1 , 1 , 1 , 10 , 1, 1},
										   new int[]{1 , 1 , 1 , 1 , 0 , 1},
										   new int[]{1 , 1 , 1 , 1 , 1 , 1},
										   new int[]{1 , 1 , 1 , 1 , 5 , 1},
										   new int[]{1 , 1 , 1 , 1 , 10, 1},
                                           new int[]{1 , 1 , 1 , 1 , 1 , 0},
                                           new int[]{1 , 1 , 1 , 1 , 1 , 1},
                                           new int[]{1 , 1 , 1 , 1 , 1 , 5},
                                           new int[]{1 , 1 , 1 , 1 , 1 , 10}};
			for (int i = 0; i < nameCoeff.Count(); i++)
			{
				for (int l = 0; l < level.Count(); l++)
				{
					for (int p = 0; p < tmpGeneral.TrainingPr; p++)
					{
						tmpTrainingPr[p].weight_p = weight[i * level.Count() + l][2];
					}
					for (int ii = 0; ii < tmpGeneral.Interns; ii++)
					{
						tmpinternInfos[ii].wieght_h= weight[i * level.Count() + l][0];
						tmpinternInfos[ii].wieght_d = weight[i * level.Count() + l][1];
						tmpinternInfos[ii].wieght_ch = -weight[i * level.Count() + l][3];
						tmpinternInfos[ii].wieght_w = -weight[i * level.Count() + l][4];
                        tmpinternInfos[ii].wieght_cns = -weight[i * level.Count() + l][5];
                    }
					string NewLoc = location + nameCoeff[i] + level[l] + "\\";
					if (!Directory.Exists(NewLoc))
					{
						Directory.CreateDirectory(NewLoc);
					}
					WriteInstance(NewLoc, name);
				}
			}
		}
	}
}
