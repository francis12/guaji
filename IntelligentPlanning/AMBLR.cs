﻿namespace IntelligentPlanning
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Web;

    public class AMBLR : PTBase
    {
        public string KJTime = "";

        public override bool BetsMain(ConfigurationStatus.SCPlan plan, ref string pHint)
        {
            bool flag = false;
            try
            {
                string betsLine = this.GetBetsLine(plan.Type);
                string pReferer = $"{this.GetLine()}/game/index?id={this.GetBetsLotteryID(plan.Type)}";
                Dictionary<string, Dictionary<string, List<string>>> fNNumberDic = plan.FNNumberDic;
                foreach (string str3 in fNNumberDic.Keys)
                {
                    Dictionary<string, List<string>> dictionary2 = fNNumberDic[str3];
                    foreach (string str4 in dictionary2.Keys)
                    {
                        if (plan.IsMNState(str4, true))
                        {
                            flag = true;
                            pHint = "投注成功";
                        }
                        else
                        {
                            List<string> pTNumberList = plan.GetPTNumberList(dictionary2[str4]);
                            string pResponsetext = "";
                            int num = plan.FNNumber(str4);
                            string prize = this.GetPrize(plan.Type, plan.Play);
                            string str7 = CommFunc.Random(0x3b9aca00, 0x77359400).ToString() + CommFunc.Random(100, 0x3e7);
                            string[] strArray = this.GetPlayMethodID(plan.Type, plan.Play).Split(new char[] { '-' });
                            string betsLotteryID = this.GetBetsLotteryID(plan.Type);
                            string format = "code%5B0%5D%5BfanDian%5D={0}&code%5B0%5D%5BbonusProp%5D={1}&code%5B0%5D%5Bmode%5D={2}&code%5B0%5D%5BbeiShu%5D={3}&code%5B0%5D%5BorderId%5D={4}&code%5B0%5D%5BactionData%5D={5}&code%5B0%5D%5BactionNum%5D={6}&code%5B0%5D%5BplayedGroup%5D={7}&code%5B0%5D%5BplayedId%5D={8}&code%5B0%5D%5Btype%5D={9}&code%5B0%5D%5BweiShu%5D={10}&para%5Btype%5D={11}&para%5BactionNo%5D={12}&para%5BkjTime%5D={13}&is_combine=0";
                            format = string.Format(format, new object[] { "0", prize, plan.Money, Convert.ToInt32(plan.AutoTimes(str4, true)), str7, this.GetNumberList1(pTNumberList, plan.Play, plan.RXWZ), num, strArray[0], strArray[1], betsLotteryID, "0", betsLotteryID, this.GetBetsExpect(plan.CurrentExpect, ""), this.KJTime });
                            HttpHelper.GetResponse(ref pResponsetext, betsLine, "POST", format, pReferer, 0x2710, "UTF-8", true);
                            flag = this.CheckReturn(pResponsetext, true);
                            pHint = this.GetReturn(pResponsetext);
                        }
                    }
                }
            }
            catch
            {
            }
            return flag;
        }

        public override bool CheckBreakConnect(string pHint) => 
            (pHint.Contains("重新登录") || pHint.Contains(@"\u8bf7\u5148\u767b\u9646"));

        public override bool CheckReturn(string pResponseText, bool pIsChange) => 
            (pResponseText.Contains(@"\u6295\u6ce8\u6210\u529f") || (pResponseText == "投注成功"));

        private bool CheckVerifyCode(string pVerifyCode)
        {
            if (pVerifyCode.Length != 4)
            {
                return false;
            }
            return true;
        }

        public override void GetAccountsMem(ConfigurationStatus.LotteryType pType, ConfigurationStatus.SCAccountData pInfo)
        {
            try
            {
                string accountsMemLine = this.GetAccountsMemLine(pType);
                string indexLine = this.GetIndexLine();
                string pResponsetext = "";
                HttpHelper.GetResponse(ref pResponsetext, accountsMemLine, "GET", string.Empty, indexLine, 0x2710, "UTF-8", true);
                string str4 = pResponsetext.Replace("null", "");
                AppInfo.Account.BankBalance = Convert.ToDouble(str4);
            }
            catch
            {
            }
        }

        public override string GetAccountsMemLine(ConfigurationStatus.LotteryType pType) => 
            $"{this.GetLine()}/index.php/safe/userInfo";

        public override string GetAppExpect(ConfigurationStatus.LotteryType pType, string pExpect, bool pIsBets = false)
        {
            string str = pExpect;
            if (((pType == ConfigurationStatus.LotteryType.GD11X5) || (pType == ConfigurationStatus.LotteryType.SD11X5)) || (pType == ConfigurationStatus.LotteryType.SH11X5))
            {
                if (pIsBets)
                {
                    return str.Substring(2).Replace("-0", "");
                }
                return ("20" + str);
            }
            if (pType == ConfigurationStatus.LotteryType.TJSSC)
            {
                if (pIsBets)
                {
                    return str.Substring(2).Replace("-", "");
                }
                return ("20" + str);
            }
            if (((pType == ConfigurationStatus.LotteryType.XJSSC) || (pType == ConfigurationStatus.LotteryType.JX11X5)) && pIsBets)
            {
                str = str.Replace("-0", "-");
            }
            return str;
        }

        public override string GetBetsExpect(string pExpect, string pLotteryID = "")
        {
            string iD = AppInfo.Current.Lottery.ID;
            string str2 = CommFunc.ConvertBetsExpect(pExpect, iD, false, false, false);
            return this.GetAppExpect(AppInfo.Current.Lottery.Type, str2, true);
        }

        public override string GetBetsLine(ConfigurationStatus.LotteryType pType) => 
            (this.GetLine() + "/index.php/game/postCode");

        public override string GetBetsLotteryID(ConfigurationStatus.LotteryType pType)
        {
            string str = "";
            if (pType == ConfigurationStatus.LotteryType.CQSSC)
            {
                return "1";
            }
            if (pType == ConfigurationStatus.LotteryType.XJSSC)
            {
                return "12";
            }
            if (pType == ConfigurationStatus.LotteryType.TJSSC)
            {
                return "60";
            }
            if (pType == ConfigurationStatus.LotteryType.AMBLRTXFFC)
            {
                return "80";
            }
            if (pType == ConfigurationStatus.LotteryType.AMBLRAMSSC)
            {
                return "61";
            }
            if (pType == ConfigurationStatus.LotteryType.AMBLRTWSSC)
            {
                return "62";
            }
            if (pType == ConfigurationStatus.LotteryType.AMBLRQQFFC)
            {
                return "5";
            }
            if (pType == ConfigurationStatus.LotteryType.AMBLRHN2FC)
            {
                return "26";
            }
            if (pType == ConfigurationStatus.LotteryType.AMBLRHN5FC)
            {
                return "14";
            }
            if (pType == ConfigurationStatus.LotteryType.AMBLRBX15F)
            {
                return "76";
            }
            if (pType == ConfigurationStatus.LotteryType.AMBLRBXKLC)
            {
                return "75";
            }
            if (pType == ConfigurationStatus.LotteryType.SD11X5)
            {
                return "7";
            }
            if (pType == ConfigurationStatus.LotteryType.GD11X5)
            {
                return "6";
            }
            if (pType == ConfigurationStatus.LotteryType.JX11X5)
            {
                return "16";
            }
            if (pType == ConfigurationStatus.LotteryType.SH11X5)
            {
                return "15";
            }
            if (pType == ConfigurationStatus.LotteryType.AMBLRAM11X5)
            {
                return "67";
            }
            if (pType == ConfigurationStatus.LotteryType.AMBLRTW11X5)
            {
                return "68";
            }
            if (pType == ConfigurationStatus.LotteryType.PK10)
            {
                return "20";
            }
            if (pType == ConfigurationStatus.LotteryType.AMBLRAMPK10)
            {
                return "65";
            }
            if (pType == ConfigurationStatus.LotteryType.AMBLRTWPK10)
            {
                str = "66";
            }
            return str;
        }

        public override string GetIndexCode(ConfigurationStatus.LotteryType pType) => 
            $"{this.GetLine()}/index.php/index/historyList/{this.GetBetsLotteryID(pType)}";

        public override string GetIndexLine() => 
            (this.GetLine() + "/index.php/index/game");

        public override string GetLoginLine() => 
            (this.GetLine() + "/index.php/user/logined2");

        public override string GetLotteryLine(ConfigurationStatus.LotteryType pType, bool pAll = false) => 
            (this.GetLine() + "/");

        public override string GetNumberList1(List<string> pNumberList, string playName, List<int> pRXWZ = null)
        {
            int num;
            List<string> list;
            int num2;
            string str2;
            string str = "";
            if (AppInfo.Current.Lottery.Group == ConfigurationStatus.LotteryGroup.GPSSC)
            {
                if (CommFunc.CheckPlayIsFS(playName))
                {
                    str = CommFunc.Join(pNumberList, ",").Replace("*", "-");
                }
                else if (playName.Contains("定位胆"))
                {
                    char ch = playName[3];
                    num = AppInfo.FiveDic[ch.ToString()];
                    list = new List<string>();
                    for (num2 = 0; num2 < 5; num2++)
                    {
                        str2 = "*";
                        if (num2 == num)
                        {
                            str2 = CommFunc.Join(pNumberList);
                        }
                        list.Add(str2);
                    }
                    str = CommFunc.Join(list, ",").Replace("*", "-");
                }
                else if (CommFunc.CheckPlayIsZuX(playName))
                {
                    if (playName.Contains("复式"))
                    {
                        str = CommFunc.Join(pNumberList);
                    }
                }
                else
                {
                    List<string> pList = new List<string>();
                    for (num2 = 0; num2 < pNumberList.Count; num2++)
                    {
                        string item = CommFunc.Join(pNumberList[num2], ",", -1);
                        pList.Add(item);
                    }
                    str = CommFunc.Join(pList, "|");
                }
                return HttpUtility.UrlEncode(str);
            }
            if (AppInfo.Current.Lottery.Group == ConfigurationStatus.LotteryGroup.GP11X5)
            {
                if (CommFunc.CheckPlayIsDS(playName))
                {
                    str = CommFunc.Join(pNumberList, "|").Replace(" ", ",");
                }
                else if (CommFunc.CheckPlayIsFS(playName))
                {
                    str = CommFunc.Join(pNumberList, " ");
                }
                return HttpUtility.UrlEncode(str);
            }
            if (AppInfo.Current.Lottery.Group != ConfigurationStatus.LotteryGroup.GPPK10)
            {
                return str;
            }
            if (!CommFunc.CheckPlayIsDS(playName))
            {
                if (CommFunc.CheckPlayIsFS(playName))
                {
                    str = CommFunc.Join(pNumberList, ",").Replace("*", "-");
                }
                else
                {
                    num = -1;
                    if (playName.Contains("冠军"))
                    {
                        num = 0;
                    }
                    else if (playName.Contains("亚军"))
                    {
                        num = 1;
                    }
                    else
                    {
                        num = CommFunc.GetPlayNum(playName) - 1;
                    }
                    list = new List<string>();
                    int num3 = (playName == "猜冠军猜冠军") ? 1 : 10;
                    for (num2 = 0; num2 < num3; num2++)
                    {
                        str2 = "*";
                        if (num2 == num)
                        {
                            str2 = CommFunc.Join(pNumberList, " ");
                        }
                        list.Add(str2);
                    }
                    str = CommFunc.Join(list, ",").Replace("*", "-");
                }
            }
            return HttpUtility.UrlEncode(str);
        }

        public override string GetPlayMethodID(ConfigurationStatus.LotteryType pType, string playName)
        {
            string str = "";
            if (AppInfo.Current.Lottery.Group == ConfigurationStatus.LotteryGroup.GPSSC)
            {
                if (playName == "前三直选单式")
                {
                    return "2-11";
                }
                if (playName == "前三直选复式")
                {
                    return "2-10";
                }
                if (playName == "前三组三复式")
                {
                    return "3-16";
                }
                if (playName == "前三组六复式")
                {
                    return "3-17";
                }
                if (playName == "后三直选单式")
                {
                    return "2-13";
                }
                if (playName == "后三直选复式")
                {
                    return "2-12";
                }
                if (playName == "后三组三复式")
                {
                    return "3-19";
                }
                if (playName == "后三组六复式")
                {
                    return "3-20";
                }
                if (playName == "中三直选单式")
                {
                    return "2-288";
                }
                if (playName == "中三直选复式")
                {
                    return "2-287";
                }
                if (playName == "中三组三复式")
                {
                    return "3-289";
                }
                if (playName == "中三组六复式")
                {
                    return "3-290";
                }
                if (playName == "前二直选单式")
                {
                    return "4-26";
                }
                if (playName == "前二直选复式")
                {
                    return "4-25";
                }
                if (playName == "后二直选单式")
                {
                    return "4-28";
                }
                if (playName == "后二直选复式")
                {
                    return "4-27";
                }
                if (playName == "前四直选单式")
                {
                    return "66-5";
                }
                if (playName == "前四直选复式")
                {
                    return "66-4";
                }
                if (playName == "后四直选单式")
                {
                    return "66-7";
                }
                if (playName == "后四直选复式")
                {
                    return "66-6";
                }
                if (playName == "五星直选单式")
                {
                    return "1-3";
                }
                if (playName == "五星直选复式")
                {
                    return "1-2";
                }
                if (playName == "任三直选单式")
                {
                    return "";
                }
                if (playName == "任三直选复式")
                {
                    return "";
                }
                if (playName == "任二直选单式")
                {
                    return "";
                }
                if (playName == "任二直选复式")
                {
                    return "";
                }
                if (playName == "任四直选单式")
                {
                    return "";
                }
                if (playName == "任四直选复式")
                {
                    return "";
                }
                if (playName.Contains("定位胆"))
                {
                    str = "6-37";
                }
                return str;
            }
            if (AppInfo.Current.Lottery.Group == ConfigurationStatus.LotteryGroup.GP11X5)
            {
                if (playName == "前三直选单式")
                {
                    return "50-152";
                }
                if (playName == "前二直选单式")
                {
                    return "50-155";
                }
                if (playName == "任选复式一中一")
                {
                    return "9-45";
                }
                if (playName == "任选复式二中二")
                {
                    return "9-46";
                }
                if (playName == "任选复式三中三")
                {
                    return "9-47";
                }
                if (playName == "任选复式四中四")
                {
                    return "9-48";
                }
                if (playName == "任选复式五中五")
                {
                    return "9-49";
                }
                if (playName == "任选单式一中一")
                {
                    return "79-292";
                }
                if (playName == "任选单式二中二")
                {
                    return "79-293";
                }
                if (playName == "任选单式三中三")
                {
                    return "79-294";
                }
                if (playName == "任选单式四中四")
                {
                    return "79-295";
                }
                if (playName == "任选单式五中五")
                {
                    str = "79-296";
                }
                return str;
            }
            if (AppInfo.Current.Lottery.Group == ConfigurationStatus.LotteryGroup.GPPK10)
            {
                if (playName == "猜前三单式")
                {
                    return "";
                }
                if (playName == "猜前三复式")
                {
                    return "28-95";
                }
                if (playName == "猜前二单式")
                {
                    return "";
                }
                if (playName == "猜前二复式")
                {
                    return "";
                }
                if (playName == "猜前四单式")
                {
                    return "";
                }
                if (playName == "猜前四复式")
                {
                    return "";
                }
                if (playName == "猜前五单式")
                {
                    return "";
                }
                if (playName == "猜前五复式")
                {
                    return "";
                }
                if (playName == "猜冠军猜冠军")
                {
                    return "26-93";
                }
                if (playName.Contains("定位胆"))
                {
                    str = "29-96";
                }
            }
            return str;
        }

        public override string GetPrize(ConfigurationStatus.LotteryType pType, string playName)
        {
            int num = 0;
            while (true)
            {
                if ((num >= 3) || (base.Prize != ""))
                {
                    if (base.Prize == "")
                    {
                        return "";
                    }
                    double pNum = Convert.ToDouble(base.Prize);
                    if (AppInfo.Current.Lottery.Group == ConfigurationStatus.LotteryGroup.GPSSC)
                    {
                        if (playName.Contains("组三"))
                        {
                            pNum = 0.33362105263157893 * Convert.ToInt32(base.Prize);
                        }
                        else if (playName.Contains("组六"))
                        {
                            pNum = 0.16657368421052632 * Convert.ToInt32(base.Prize);
                        }
                        else
                        {
                            pNum /= 1000.0;
                            if (playName.Contains("定位胆"))
                            {
                                pNum *= 10.0;
                            }
                            else if (playName.Contains("二"))
                            {
                                pNum *= 100.0;
                            }
                            else if (playName.Contains("三"))
                            {
                                pNum *= 1000.0;
                            }
                            else if (playName.Contains("四"))
                            {
                                pNum *= 10000.0;
                            }
                            else if (playName.Contains("五"))
                            {
                                pNum *= 100000.0;
                            }
                        }
                    }
                    else if (AppInfo.Current.Lottery.Group == ConfigurationStatus.LotteryGroup.GP11X5)
                    {
                        if (playName.Contains("一"))
                        {
                            pNum /= 468.599;
                        }
                        else if (playName.Contains("二"))
                        {
                            if (playName.Contains("任"))
                            {
                                pNum /= 184.7619;
                            }
                            else
                            {
                                pNum /= 9.2328;
                            }
                        }
                        else if (playName.Contains("三"))
                        {
                            if (playName.Contains("任"))
                            {
                                pNum /= 61.54822;
                            }
                            else
                            {
                                pNum /= 1.025869;
                            }
                        }
                        else if (playName.Contains("四"))
                        {
                            pNum /= 15.48161;
                        }
                        else if (playName.Contains("五"))
                        {
                            pNum /= 2.197752;
                        }
                    }
                    else if (AppInfo.Current.Lottery.Group == ConfigurationStatus.LotteryGroup.GPPK10)
                    {
                        if (playName.Contains("定位胆") || playName.Contains("猜冠军猜冠军"))
                        {
                            pNum /= 100.0;
                        }
                        else if (!playName.Contains("和值") && playName.Contains("三"))
                        {
                            pNum /= 1.398158;
                        }
                    }
                    return CommFunc.TwoDouble(pNum, true);
                }
                this.GetSite(pType, "");
                num++;
            }
        }

        public override string GetPTHint(string pResponseText)
        {
            string str = "";
            if (this.CheckReturn(pResponseText, false))
            {
                return "投注成功";
            }
            str = CommFunc.UniconToString(CommFunc.GetIndexString(pResponseText, "\"remark\": \"", "\"", 0));
            if (str == "")
            {
                str = CommFunc.UniconToString(CommFunc.GetIndexString(pResponseText, "\"message\": \"", "\"", 0).Replace("!", ""));
            }
            return str;
        }

        public override string GetPTLotteryName(ConfigurationStatus.LotteryType pType) => 
            "";

        public override string GetQuitPTLine() => 
            (this.GetLine() + "/index.php/user/logout");

        public override string GetReturn(string pResponseText) => 
            pResponseText;

        public override string GetRXWZString(List<int> pRXWZ)
        {
            List<string> pList = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                string item = pRXWZ.Contains(i) ? "1" : "0";
                pList.Add(item);
            }
            return $"{CommFunc.Join(pList)}:";
        }

        public override void GetSite(ConfigurationStatus.LotteryType pType, string playName = "")
        {
            try
            {
                string pUrl = $"{this.GetLine()}/index.php/game/getNo/{this.GetBetsLotteryID(pType)}";
                string indexLine = this.GetIndexLine();
                string pResponsetext = "";
                HttpHelper.GetResponse(ref pResponsetext, pUrl, "GET", string.Empty, indexLine, 0x2710, "UTF-8", true);
                if (pResponsetext != "")
                {
                    base.Expect = CommFunc.GetIndexString(pResponsetext, "\"actionNo\":\"", "\"", 0);
                    base.Expect = this.GetAppExpect(pType, base.Expect, false);
                    base.Expect = CommFunc.ConvertExpect(base.Expect, pType);
                    this.KJTime = CommFunc.GetIndexString(pResponsetext, "\"actionTime\":", "}", 0);
                }
            }
            catch
            {
            }
        }

        public override string GetWebVerifyCode(string pVerifyCodeFile)
        {
            string pVerifyCode = "";
            try
            {
                string str2 = $"/index.php/user/vcode_login/{DateTime.Now.ToOADate()}";
                string pUrl = this.GetLine() + str2;
                File.Delete(pVerifyCodeFile);
                Bitmap bitmap = new Bitmap(HttpHelper.GetResponseImage(pUrl, "", "GET", "", 0x1770, "UTF-8", true));
                bitmap.Save(pVerifyCodeFile);
                bitmap.Dispose();
                while (!File.Exists(pVerifyCodeFile))
                {
                    Thread.Sleep(500);
                }
                pVerifyCode = VerifyCodeAPI.VerifyCodeMain(base.PTID, pVerifyCodeFile);
                if (!this.CheckVerifyCode(pVerifyCode))
                {
                    return this.GetWebVerifyCode(pVerifyCodeFile);
                }
            }
            catch
            {
            }
            return pVerifyCode;
        }

        public bool InputWeb(string pID, string pW, ref string pHint)
        {
            bool flag = false;
            string webVerifyCode = this.GetWebVerifyCode(AutoBetsWindow.VerifyCodeFile);
            if (webVerifyCode != "")
            {
                string loginLine = this.GetLoginLine();
                string pUrl = this.GetLoginLine();
                string pResponsetext = "";
                string str5 = HttpUtility.UrlEncode(pW);
                string pData = $"type=top&username={pID}&password={str5}&vcode={webVerifyCode}";
                HttpHelper.GetResponse(ref pResponsetext, pUrl, "POST", pData, loginLine, base.BetsTime2, "UTF-8", true);
                flag = pResponsetext.Contains("username\":\"");
                if (!flag)
                {
                    pHint = CommFunc.UniconToString(CommFunc.GetIndexString(pResponsetext, "\"msg\":\"", "\"", 0));
                    if (pHint.Contains("验证码"))
                    {
                        pHint = "";
                        return this.InputWeb(pID, pW, ref pHint);
                    }
                    return flag;
                }
                base.Rebate = CommFunc.GetIndexString(pResponsetext, "\"fanDian\":\"", "\"", 0);
                base.Prize = (1700.0 + ((Convert.ToDouble(base.Rebate) * 2.0) * 10.0)).ToString();
            }
            return flag;
        }

        public bool LoginWeb()
        {
            string pReferer = "";
            string pUrl = this.GetLine() + "/";
            string pResponsetext = "";
            HttpHelper.GetResponse(ref pResponsetext, pUrl, "GET", string.Empty, pReferer, base.BetsTime2, "UTF-8", true);
            return pResponsetext.Contains("澳门巴黎人");
        }

        public override void QuitPT()
        {
            string quitPTLine = this.GetQuitPTLine();
            string indexLine = this.GetIndexLine();
            string pResponsetext = "";
            HttpHelper.GetResponse(ref pResponsetext, quitPTLine, "GET", string.Empty, indexLine, 0x2710, "UTF-8", true);
        }

        public override bool WebLoginMain(string pID, string pW, ref string pHint)
        {
            this.KJTime = "";
            if (!this.LoginWeb())
            {
                return false;
            }
            if (!this.InputWeb(pID, pW, ref pHint))
            {
                return false;
            }
            return true;
        }
    }
}

