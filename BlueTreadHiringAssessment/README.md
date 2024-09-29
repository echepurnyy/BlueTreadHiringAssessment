Overview

This project's purpose is to allow the user to search for entertainment attractions and venues, 
using the Ticketmaster API. Using the name of their favorite band, soccer team, or upcoming movie, 
they can look up the upcoming events featuring them.

Installation

This project is implemented as a webapp, and can be hosted in a few different ways. Below are the instructions to publish it to Azure and IIS using Visual Studio.
Prerequisites:
-Visual Studio 2022 with ASP.NET 8.0 framework installed
-A hosting environment running Windows Server with IIS, that has .NET Core SDK (https://learn.microsoft.com/en-us/dotnet/core/sdk) installed
OR
-A Microsoft Azure subscription 

Steps to deploy using IIS:
1.On the IIS server, create a folder to contain the app's published folders and files. In a following step, the folder's path is provided to IIS as the physical path to the app. 
2.In IIS Manager, open the server's node in the Connections panel. Right-click the Sites folder. Select Add Website from the contextual menu.
3.Provide a Site name and set the Physical path to the app's deployment folder that you created. Provide the Binding configuration and create the website by selecting OK.
4.Confirm the process model identity has the proper permissions.
5.If the default identity of the app pool (Process Model > Identity) is changed from ApplicationPoolIdentity to another identity, verify that the new identity has the required permissions to access the app's folder, database, and other required resources. For example, the app pool requires read and write access to folders where the app reads and writes files.
6. In Visual Studio, right-click on the project in Solution Explorer and select Publish.
7.In the Pick a publish target dialog, select the Folder publish option.
8.Set the Folder or File Share path.
9.If you created a folder for the IIS site that's available on the development machine as a network share, provide the path to the share. The current user must have write access to publish to the share.
10.If you're unable to deploy directly to the IIS site folder on the IIS server, publish to a folder on removable media and physically move the published app to the IIS site folder on the server, which is the site's Physical path in IIS Manager. Move the contents of the bin/Release/{TARGET FRAMEWORK}/publish folder to the IIS site folder on the server, which is the site's Physical path in IIS Manager.
11.Select the Publish button. Wait until the publish action completes, and browse the site.

Steps to deploy using Azure
1.Right-click on the project in Solution Explorer and select Publish. Contextual menu open with Publish link highlighted.
2.In the Publish dialog, select Azure, then select Next.
3.Select Azure App Service (Windows), select Next, then select Azure Service.
4.In the App Service tab, select Create new.
5. In the Create App Service dialog, the Name, Resource Group, and Hosting Plan entry fields are populated. You can keep these names or change them.
6. Select Create. After creation is completed the dialog is automatically closed and the Publish dialog gets focus again.
7.Select Finish.
8.Click Publish. Visual Studio publishes your app to Azure. When the deployment completes, the app is opened in a browser.
9.After publishing, proceed to your Azure account page. There, find the deployed app in the App Services.
10.On the app's page, go to Environment Variables, and create an entry for the Ticketmaster API key. Use TMApiKey as name, and your API key as the value. 

User Guide

-When the app is started, a loading screen with the large EncoreTix logo appears. After that, the app's landing page is displayed.
-The EncoreTix logo at the top of the page serves as a way to clear the query and the search results. Click on it to clear everything out.
-Click the textbox with the "Search attractions..." placeholder to enter your query. When clicking an empty searchbox, it defaults to the band Phish.
-After entering your search query and clicking the Submit button, you'll either see a "No results found" image, or the list of attractions retrieved.
-Each entry in the list of attractions is clickable, and will lead you to a page that attraction's upcoming events, along with the links to their X account, YouTube channel, Spotify account, and homepage, if any.
-The upcoming events on the attractions' pages display event names, locations, and dates.

Configuration

The app is configurable through the appsettings.json file. If it is deployed in Azure, the Environment Variables on the 
app's App Services page is used instead. The only setting there that needs to be configured is the Ticketmaster API key, 
with TMApiKey as the configuration variable's name, and the API key as its value.

