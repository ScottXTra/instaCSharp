using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace instaCSharp
{
    public class instaCSharpClass
    {
        public static String myAccount;
        public static int loginWait = 3000;
        public static Random rnd = new Random();
        public static bool followCheckRunning = false;
        public static bool likeUsersLastPostRunning = false;
        public static bool followRunning = false;
        public static bool unFollowRunning = false;
        public static bool scrapeUsersRunning = false;
        public static bool scrapeUserDataRunning = false;
        public static bool editProfileRunning = false;
        public static List<String> logData = new List<String>();
        public static instaCSharpClass mem = new instaCSharpClass();
        public IWebDriver startDriver()
        {
            //Cleans up old sessions
            try
            {
                foreach (Process proc in Process.GetProcessesByName("chromedriver"))
                {
                    proc.Kill();
                }
            }
            catch
            {

            }
            //OPTIONS
            #region
            var service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            ChromeOptions options = new ChromeOptions();
            //options.AddArgument("--headless");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--window-position=0,300");
            options.EnableMobileEmulation("iPhone 7");
            #endregion
            IWebDriver dvr = new ChromeDriver(service, options);
            return dvr;
        }
        public void printf(String text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void login(IWebDriver dvr, String username, String passWord)
        {
            
            dvr.Navigate().GoToUrl("https://www.instagram.com/accounts/login/");
            IWebElement userNameBox = dvr.FindElement(By.Name("username"));
            userNameBox.SendKeys(username);
            IWebElement passwordBox = dvr.FindElement(By.Name("password"));
            passwordBox.SendKeys(passWord);
            passwordBox.Submit();
            Thread.Sleep(loginWait);
            IWebElement notNowButton = dvr.FindElement(By.XPath("//section/main/div/button"));
            notNowButton.Click();
            myAccount = username;
            logData.Add("Logging into instagram " + DateTime.Now.ToString("h:mm:ss tt"));
            //Console.Clear();
        }
        public void gotoIMP(IWebDriver dvr)
        {
            dvr.Navigate().GoToUrl("https://www.instagram.com/");
        }
        public void gotoMyPage(IWebDriver dvr)
        {
            dvr.Navigate().GoToUrl("https://www.instagram.com/" + myAccount + "/");
        }
        public void gotoMyFollowersPage(IWebDriver dvr)
        {
            gotoMyPage(dvr);
            IWebElement test = dvr.FindElement(By.CssSelector("[href=\"/" + myAccount + "/followers/\"]"));
            test.SendKeys(Keys.Enter);
        }
        public void gotoMyFollowingPage(IWebDriver dvr)
        {
            gotoMyPage(dvr);
            IWebElement test = dvr.FindElement(By.CssSelector("[href=\"/" + myAccount + "/following/\"]"));
            Thread.Sleep(200);
            test.SendKeys(Keys.Enter);

        }
        public void editMyProfileInfo(IWebDriver dvr, String name, String userName, String website, String bio)
        {
            editProfileRunning = false;
            dvr.Navigate().GoToUrl("https://www.instagram.com/accounts/edit/");
            IWebElement nameBox; IWebElement userNameBox; IWebElement websiteBox; IWebElement bioBox;
            if (!name.Equals(""))
            {
                nameBox = dvr.FindElement(By.Id("pepName"));
                nameBox.Clear();
                nameBox.SendKeys(name);
            }
            if (!userName.Equals(""))
            {
                userNameBox = dvr.FindElement(By.Id("pepUsername"));
                userNameBox.Clear();
                userNameBox.SendKeys(userName);
            }
            if (!website.Equals(""))
            {
                websiteBox = dvr.FindElement(By.Id("pepWebsite"));
                websiteBox.Clear();
                websiteBox.SendKeys(website);
            }
            if (!bio.Equals(""))
            {
                bioBox = dvr.FindElement(By.Id("pepBio"));
                bioBox.Clear();
                bioBox.SendKeys(bio);
            }
            IWebElement subBtn = dvr.FindElement(By.Id("pepBio"));
            subBtn.Submit();
            gotoMyPage(dvr);
            logData.Add("Edited my profile " + DateTime.Now.ToString("h:mm:ss tt"));
            editProfileRunning = false;
        }
        public List<string> getMyFollowers(IWebDriver dvr)
        {
            gotoMyFollowersPage(dvr);
            List<string> followers = new List<string>();
            Thread.Sleep(2000);
            Actions actions = new Actions(dvr);

            IWebElement userName;
            for (int i = 1; i < 2000; i++)
            {
                try
                {
                    userName = dvr.FindElement(By.XPath("//div[2]/ul/div/li[" + i + "]/div/div[1]/div[2]/div[1]/a"));
                    if (i % 100 == 0)
                    {
                        actions.MoveToElement(userName);
                        actions.Perform();
                    }
                    followers.Add(userName.Text);
                    //Console.WriteLine(userName.Text);
                }
                catch
                {
                    break;
                }
            }
            logData.Add("Downloaded my followers " + DateTime.Now.ToString("h:mm:ss tt"));
            return followers;
        }
        public List<string> getMyFollowing(IWebDriver dvr)
        {
            gotoMyFollowingPage(dvr);
            List<string> following = new List<string>();
            Thread.Sleep(2000);
            //Actions actions = new Actions(dvr);
            //actions.sc(userName);
            //actions.Perform();
            Actions actions = new Actions(dvr);

            IWebElement userName;
            for (int i = 1; i < 2000; i++)
            {
                try
                {
                    userName = dvr.FindElement(By.XPath("//div[2]/ul/div/li[" + i + "]/div/div[1]/div[2]/div[1]/a"));
                    if (i % 80 == 0)
                    {
                        actions.MoveToElement(userName);
                        actions.Perform();
                    }
                    following.Add(userName.Text);
                    //Console.WriteLine(userName.Text);
                }
                catch
                {
                    break;
                }
            }
            logData.Add("Downloaded my following " + DateTime.Now.ToString("h:mm:ss tt"));
            return following;
        }
        //DEBUG FUNCTION
        public void followCheck(IWebDriver dvr)
        {

            followCheckRunning = true;
            Console.WriteLine("Stage 1: Checking who follows you");
            List<string> followers = getMyFollowers(dvr);
            Console.WriteLine("Stage 2: Checking who you are following");
            List<string> following = getMyFollowing(dvr);
            Console.WriteLine("Stage 3: Comparing");

            printf("Your ratio is: " + following.Count + "/" + followers.Count, ConsoleColor.DarkBlue);
            printf("------------------------------------------", ConsoleColor.DarkBlue);
            printf("You follow them |-------------------------", ConsoleColor.DarkCyan);
            foreach (String person in following)
            {
                if (!followers.Contains(person))
                {
                    printf("You--->Them: " + person, ConsoleColor.DarkRed);

                }
            }
            printf("Mutual follows |--------------------------", ConsoleColor.DarkCyan);
            if (followers.Count >= following.Count)
            {
                foreach (String person in following)
                {
                    if (followers.Contains(person))
                    {
                        printf("You<-->Them: " + person, ConsoleColor.DarkYellow);

                    }
                }
            }
            else
            {
                foreach (String person in followers)
                {
                    if (following.Contains(person))
                    {
                        printf("You<-->Them: " + person, ConsoleColor.DarkYellow);

                    }
                }
            }
            printf("They follow you |-------------------------", ConsoleColor.DarkCyan);
            foreach (String person in followers)
            {
                if (!following.Contains(person))
                {
                    printf("You<---Them: " + person, ConsoleColor.DarkGreen);

                }
            }
            logData.Add("Preformed follower check " + DateTime.Now.ToString("h:mm:ss tt"));
            followCheckRunning = false;

        }

        public bool follow(IWebDriver dvr, String userName)
        {
            followRunning = true;
            dvr.Navigate().GoToUrl("https://www.instagram.com/" + userName + "/");
            IList<IWebElement> buttons = dvr.FindElements(By.CssSelector("button"));
            foreach (IWebElement button in buttons)
            {
                if (button.Text.Equals("Follow"))
                {
                    button.SendKeys(Keys.Enter);
                    logData.Add("Followed " + userName + " " + DateTime.Now.ToString("h:mm:ss tt"));
                    followRunning = false;
                    return true;
                }
            }

            logData.Add("Couldn't follow " + userName + " " + DateTime.Now.ToString("h:mm:ss tt"));
            followRunning = false;
            return false;
        }
        public bool unFollow(IWebDriver dvr, String userName)
        {
            unFollowRunning = true;
            dvr.Navigate().GoToUrl("https://www.instagram.com/" + userName + "/");
            IList<IWebElement> buttons = dvr.FindElements(By.CssSelector("button"));
            foreach (IWebElement button in buttons)
            {
                try
                {
                    if (button.Text.Equals("Following") || button.Text.Equals("Requested"))
                    {
                        button.SendKeys(Keys.Enter);
                    }
                }
                catch
                {

                }

            }
            Thread.Sleep(200);
            buttons = dvr.FindElements(By.CssSelector("button"));
            foreach (IWebElement button in buttons)
            {

                if (button.Text.Equals("Unfollow"))
                {
                    button.SendKeys(Keys.Enter);
                    logData.Add("Unfollowed " + userName + " " + DateTime.Now.ToString("h:mm:ss tt"));
                    unFollowRunning = false;
                    return true;
                }
            }
            logData.Add("Couldn't unfollow " + userName + " " + DateTime.Now.ToString("h:mm:ss tt"));
            unFollowRunning = false;
            return false;


        }
        public List<string> scrapeUsers(IWebDriver dvr, String userName, int max)
        {
            scrapeUsersRunning = true;
            dvr.Navigate().GoToUrl("https://www.instagram.com/" + userName + "/");
            IWebElement test = dvr.FindElement(By.CssSelector("[href=\"/" + userName + "/followers/\"]"));
            test.SendKeys(Keys.Enter);
            List<string> followers = new List<string>();
            Actions actions = new Actions(dvr);
            Thread.Sleep(2000);
            IWebElement user;
            for (int i = 1; i < max; i++)
            {
                try
                {
                    user = dvr.FindElement(By.XPath("//div[2]/ul/div/li[" + i + "]/div/div[1]/div[2]/div[1]/a"));
                    followers.Add(user.Text);
                    if (i % 80 == 0)
                    {
                        actions.MoveToElement(user);
                        actions.Perform();
                    }
                }
                catch
                {
                    break;
                }
            }
            TextWriter tw = new StreamWriter("scraped_" + userName + "_followers.txt");
            foreach (String s in followers)
            {
                tw.WriteLine(s);
            }
            tw.Close();
            logData.Add("Scraped followers of " + userName + " " + DateTime.Now.ToString("h:mm:ss tt"));
            scrapeUsersRunning = false;
            return followers;
        }
        public void likeUsersLastPost(IWebDriver dvr, String userName)
        {
            likeUsersLastPostRunning = true;
            dvr.Navigate().GoToUrl("https://www.instagram.com/" + userName + "/feed/");
            Thread.Sleep(2000);
            Actions actions = new Actions(dvr);
            for (int i = 0; i <= 3; i++)
            {
                try
                {
                    IWebElement likeButton = dvr.FindElement(By.XPath("//section/main/div/div[4]/div[1]/div/article[1]/div[2]/section[1]/span[1]/button"));
                    actions.MoveToElement(likeButton);
                    actions.Perform();
                    likeButton.SendKeys(Keys.Enter);
                    return;
                }
                catch
                {
                    Thread.Sleep(2000);
                }
            }
            logData.Add("Liked the most recent post of " + userName + " " + DateTime.Now.ToString("h:mm:ss tt"));
            likeUsersLastPostRunning = false;
        }
        public void commentOnUsersLastPost(IWebDriver dvr, String userName, String comment)
        {
            likeUsersLastPostRunning = true;
            dvr.Navigate().GoToUrl("https://www.instagram.com/" + userName + "/feed/");
            Thread.Sleep(2000);
            Actions actions = new Actions(dvr);
            for (int i = 0; i <= 3; i++)
            {
                try
                {
                    IWebElement commentButton = dvr.FindElement(By.XPath("//section/main/div/div[3]/div[1]/div/article[1]/div[2]/section[1]/span[2]/button"));
                    actions.MoveToElement(commentButton);
                    actions.Perform();
                    commentButton.SendKeys(Keys.Enter);
                }
                catch
                {
                    Thread.Sleep(2000);
                }
            }
            IWebElement commentBox = dvr.FindElement(By.XPath("//section/main/section/div/form/textarea"));
            commentBox.SendKeys(comment);
            commentBox.Submit();
            logData.Add("Liked the most recent post of " + userName + " " + DateTime.Now.ToString("h:mm:ss tt"));
            likeUsersLastPostRunning = false;
        }
        public void randomThreadSleep(int minSec, int maxSec)
        {
            Thread.Sleep(rnd.Next(minSec, maxSec) * 1000);
            return;
        }
        public void displayLogo()
        {
            printf("------------------------------------------------------------------------------------------------------", ConsoleColor.DarkCyan);
            printf(@" ___  ________   ________  _________  ________                 _____ ______   ________  ________      
|\  \|\   ___  \|\   ____\|\___   ___\\   __  \               |\   _ \  _   \|\   __  \|\   ___  \    
\ \  \ \  \\ \  \ \  \___|\|___ \  \_\ \  \|\  \  ____________\ \  \\\__\ \  \ \  \|\  \ \  \\ \  \   
 \ \  \ \  \\ \  \ \_____  \   \ \  \ \ \   __  \|\____________\ \  \\|__| \  \ \   __  \ \  \\ \  \  
  \ \  \ \  \\ \  \|____|\  \   \ \  \ \ \  \ \  \|____________|\ \  \    \ \  \ \  \ \  \ \  \\ \  \ 
   \ \__\ \__\\ \__\____\_\  \   \ \__\ \ \__\ \__\              \ \__\    \ \__\ \__\ \__\ \__\\ \__\
    \|__|\|__| \|__|\_________\   \|__|  \|__|\|__|               \|__|     \|__|\|__|\|__|\|__| \|__|
                   \|_________|                                                                       ", ConsoleColor.DarkCyan);
            printf("------------------------------------------------------------------------------------------------------", ConsoleColor.DarkCyan);
            printf("                                               v0.1 By Scott", ConsoleColor.DarkYellow);

        }
        public void loggerStarter()
        {
            Thread t1 = new Thread(delegate ()
            {
                logger();
            });
            t1.Start();
        }
        public void logger()
        {
            while (true)
            {
                foreach (String s in logData)
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"log.txt", true))
                    {
                        file.WriteLine(s);
                    }
                }
                logData.Clear();
                Thread.Sleep(5000);
            }
        }

    }
}
