using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Seleniumtest_raspopova;

public class SeleniumTestsforPractice
{
    public ChromeDriver driver;

    [SetUp]
    public void SetUp()
    {
        var options = new ChromeOptions();
        options.AddArguments("--no-sandbox", "start-maximized", "disable-extensions");
        
        driver = new ChromeDriver(options);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        
        Autorization();
    }
    
    [Test]
    public void Authorization()
    {
        var currentUrl = driver.Url;
        currentUrl.Should().Be("https://staff-testing.testkontur.ru/news");
    }

    [Test]
    public void NavigationTest() //проверка навигации меню переходом на вкладку "Сообщества"
    {
        var community = driver.FindElements(By.CssSelector("[data-tid='Community']")).First(element => element.Displayed);
        community.Click();
        
        var communityTitle = driver.FindElement(By.CssSelector("[data-tid='Title']"));
        Assert.That(communityTitle.Text == "Сообщества");
        Assert.That(driver.Url == "https://staff-testing.testkontur.ru/communities", "мы ожидали получить https://staff-testing.testkontur.ru/communities, а получили" + driver.Url);
    }

    [Test]
    public void ComminityEnter() //вступить в первое в списке сообщество (там нет ни новостей, ни обсуждений) и проверить отображение пометки "Я участник"
    {
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities");

        var community = driver.FindElements(By.CssSelector("[data-tid='Link']")).First();
        community.Click();
        
        var joinButton = driver.FindElements(By.CssSelector("[data-tid='Join']")).First();
        joinButton.Click();
        
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities");
        
        var participant =
            driver.FindElement(
                By.CssSelector("div > div.sc-kSCemg.OGHxE > span.sc-btlduw.kMTXgk"));

        Assert.That(participant.Displayed, "Галочка -Я участник- должна быть отображена, но не отображается");
    }
    
    [Test]
    public void NoNewsMessage() //проверить отображение сообщения, что новостей пока что нет
    {
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities/eb49f672-ffdb-4fc8-a44e-0eeab0dae15d");
        
        var message =
            driver.FindElement(
                By.TagName("h2"));
        Assert.That(message.Text == "Пока новостей нет", "должен отображаться текст о том, что новостей нет, но не отображается");
    }
    
    [Test]
    public void CreateNews() //публикация новости
    {
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities/eb49f672-ffdb-4fc8-a44e-0eeab0dae15d");

        var textArea = driver.FindElement(By.CssSelector("[data-tid='Addition']"));
        textArea.Click();
        
        var newsTitle  = driver.FindElement(By.CssSelector("[class='notranslate public-DraftEditor-content']"));
        newsTitle.SendKeys("The first news here");
        
        var createDiscussionButton = driver.FindElement(By.CssSelector("[class='react-ui-m0adju']"));
        createDiscussionButton.Click();
        
        var createdNews = driver.FindElement(By.CssSelector("[class='sc-erxZQA fbGXrZ']"));
        Assert.That(createdNews.Displayed, "созданная новость должна отображаться, но не отображается");
    }
    
    [Test]
    public void EmptyNews() //попытка публикации пустой новости
    {
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities/eb49f672-ffdb-4fc8-a44e-0eeab0dae15d");

        var textArea = driver.FindElement(By.CssSelector("[data-tid='Addition']"));
        textArea.Click();
        
        var newsTitle  = driver.FindElement(By.CssSelector("[class='notranslate public-DraftEditor-content']"));
        newsTitle.SendKeys("");
        
        var createDiscussionButton = driver.FindElement(By.CssSelector("[class='react-ui-m0adju']"));
        createDiscussionButton.Click();
        
        var errorMessage = driver.FindElement(By.CssSelector("section.sc-ckTSus.kVjkXt > div > div > div:nth-child(2) > section > span > div > div > div:nth-child(1) > div:nth-child(1)"));
        Assert.That(errorMessage.Text == "Введите сообщение либо добавьте хотя бы одно вложение",
            "должно отображаться сообщение об ошибке, что пустую новость нельзя опубликовать, но не отображается");
    }
    
    [Test]
    public void CommunityExit() //выход из сообщества
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities/eb49f672-ffdb-4fc8-a44e-0eeab0dae15d");

        var menuButton = driver.FindElement(By.CssSelector("div.sc-jJMGnK.fXZvGR > div:nth-child(2) > div > span > button"));
        menuButton.Click();
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='Quit']")));
        var exitButton  = driver.FindElements(By.CssSelector("[class='sc-eWnToP fEDxD']")).Last();
        exitButton.Click();
        
        var enterButton = driver.FindElement(By.CssSelector("[data-tid='Join']"));
        Assert.That(enterButton.Displayed, "должна быть отображена кнопка вступления в сообщество, но не отображена");
    }
    
    public void Autorization()
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru");
        
        var login = driver.FindElement(By.Id("Username"));
        login.SendKeys("raspa03@mail.ru");

        var password = driver.FindElement(By.Name("Password"));
        password.SendKeys("Rubi2010**");
        
        var enter = driver.FindElement(By.Name("button"));
        enter.Click();
        
        wait.Until(ExpectedConditions.UrlContains("https://staff-testing.testkontur.ru/news"));
    }
    
    [TearDown]
    public void TearDown()
        {
            driver.Quit(); 
        }
    }