using ISynergy.Framework.UI.Extensions;
using Microsoft.Maui.Controls;

namespace ISynergy.Framework.UI.Maui.Extensions;

/// <summary>
/// Behavioural coverage for <see cref="PageExtensions.ApplyWindowFlowDirection{T}(T, string)"/>.
///
/// Real <see cref="Page"/> instances can only be created once the WinUI runtime (and a message
/// loop) exists, which is why the test body runs on the <see cref="WinUiTestHost"/> UI thread.
/// WinUI allows exactly one application per process, so all assertions share a single test method.
/// </summary>
[TestClass]
[OSCondition(OperatingSystems.Windows)]
public class PageExtensionsFlowDirectionTests
{
    [TestMethod]
    public void ApplyWindowFlowDirection_AppliesDirectionToPageAndChildPages()
    {
        WinUiTestHost.Run(() =>
        {
            // ── plain page ───────────────────────────────────────────────────
            var page = new ContentPage();

            var returned = page.ApplyWindowFlowDirection("ar");

            Assert.AreSame(page, returned, "The helper returns the page for chaining");
            Assert.AreEqual(FlowDirection.RightToLeft, page.FlowDirection);

            // A left-to-right culture must switch it back.
            page.ApplyWindowFlowDirection("en");
            Assert.AreEqual(FlowDirection.LeftToRight, page.FlowDirection);

            // ── FlyoutPage: Flyout and Detail get the direction explicitly ───
            var flyout = new ContentPage { Title = "Flyout" };
            var detail = new NavigationPage(new ContentPage());
            var flyoutPage = new FlyoutPage { Flyout = flyout, Detail = detail };

            flyoutPage.ApplyWindowFlowDirection("ar");

            Assert.AreEqual(FlowDirection.RightToLeft, flyoutPage.FlowDirection);
            Assert.AreEqual(FlowDirection.RightToLeft, flyout.FlowDirection, "Flyout child page");
            Assert.AreEqual(FlowDirection.RightToLeft, detail.FlowDirection, "Detail child page");

            // ── Shell: flyout content and current page ──────────────────────
            var shellFlyoutContent = new ContentPage();
            var shellPage = new ContentPage();
            var shellContent = new ShellContent { Content = shellPage };
            var shell = new Shell { FlyoutContent = shellFlyoutContent };
            shell.Items.Add(shellContent);
            shell.CurrentItem = shellContent;

            shell.ApplyWindowFlowDirection("ar");

            Assert.AreEqual(FlowDirection.RightToLeft, shell.FlowDirection);
            Assert.AreEqual(FlowDirection.RightToLeft, shellFlyoutContent.FlowDirection, "Shell flyout content");
            if (shell.CurrentPage is Page currentShellPage)
                Assert.AreEqual(FlowDirection.RightToLeft, currentShellPage.FlowDirection, "Shell current page");

            // ── NavigationPage: the current page is updated too ─────────────
            var navRoot = new ContentPage();
            var navigationPage = new NavigationPage(navRoot);

            navigationPage.ApplyWindowFlowDirection("ar");

            Assert.AreEqual(FlowDirection.RightToLeft, navigationPage.FlowDirection);
            Assert.AreEqual(FlowDirection.RightToLeft, navRoot.FlowDirection, "NavigationPage current page");
        });
    }
}
