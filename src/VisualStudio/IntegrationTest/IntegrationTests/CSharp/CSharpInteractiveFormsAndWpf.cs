﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Windows.Automation;
using Microsoft.VisualStudio.IntegrationTest.Utilities;
using Roslyn.VisualStudio.IntegrationTests.Extensions.Interactive;
using Xunit;

namespace Roslyn.VisualStudio.IntegrationTests.CSharp
{
    [Collection(nameof(SharedIntegrationHostFixture))]
    public class CSharpInteractiveFormsAndWpf : AbstractInteractiveWindowTest
    {
        public CSharpInteractiveFormsAndWpf(VisualStudioInstanceFactory instanceFactory)
            : base(instanceFactory)
        {
            this.SubmitText(@"#r ""System.Windows.Forms""
#r ""WindowsBase""
#r ""PresentationCore""
#r ""PresentationFramework""
#r ""System.Xaml""");

            this.SubmitText(@"using System.Windows;
using System.Windows.Forms;
using Wpf = System.Windows.Controls;");
        }

        [Fact]
        public void InteractiveWithDisplayFormAndWpfWindow()
        {
            // 1) Create and display form and WPF window
            this.SubmitText(@"Form form = new Form();
form.Text = ""win form text"";
form.Show();
Window wind = new Window();
wind.Title = ""wpf window text"";
wind.Show();");

            AutomationElement form =  AutomationElementHelper.FindAutomationElementAsync("win form text").Result;
            AutomationElement  wpf = AutomationElementHelper.FindAutomationElementAsync("wpf window text").Result;

            // 3) Add UI elements to windows and verify
            this.SubmitText(@"// add a label to the form
Label l = new Label();
l.Text = ""forms label text"";
form.Controls.Add(l);
// set simple text as the body of the wpf window
Wpf.TextBlock t = new Wpf.TextBlock();
t.Text = ""wpf body text"";
wind.Content = t;");

            AutomationElement formLabel = form.FindDescendantByPath("text");
            Assert.Equal("forms label text", formLabel.Current.Name);

            AutomationElement wpfContent = wpf.FindDescendantByPath("text");
            Assert.Equal("wpf body text", wpfContent.Current.Name);

            // 4) Close windows
            this.SubmitText(@"form.Close();
wind.Close();");
        }
    }
}