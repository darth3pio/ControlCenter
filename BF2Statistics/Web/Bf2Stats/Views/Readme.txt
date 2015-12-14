Starting with Control Center v2.3.0, we have moved from T4 templates to using Razor templates (.cshtml).
The reason for this change is to allow users to change the html code behind the pages to their desire.
These template are compiled into C# code and loaded into memory when the Http ASP Server is started,
Therfor any edits that are made to the cshtml files will require a full program restart to take affect. 

Adding Custom Pages:

Each template file that is added to the Views folder should start as so:

@using RazorEngine
@using RazorEngine.Templating
@using BF2Statistics.Web.Bf2Stats
@inherits HtmlTemplateBase<BF2PageModel>
@{
    Layout = "_layout.cshtml";
}

Each page that was created and implemented into the program will use a different Model for each page. Since
there is no way to create our own model without editing the source code of this project, you will have to 
settle for using the base BF2PageModel, which has just the basic variables and methods that you will need.

Your custom views can be accessed by the following address format: [YourSiteUrl]/Bf2Stats/[ViewName]
NOTE: Do not include the .cshtml extension within the address bar or you will get a 404 Not Found.

Using the Layout:

The example about includes the following line "Layout = "_layout.cshtml";". Remove this if you wish to remove
the template surrounding the main content area of your page.

Coding Syntax:

For coding, please use this reference to get a quick overview of the Razor Syntax:
http://haacked.com/archive/2011/01/06/razor-syntax-quick-reference.aspx/

Model Properties:

Here is a list of available properties and methods in the Model:
source-code: https://github.com/BF2Statistics/ControlCenter/tree/master/BF2Statistics/Web/Bf2Stats/Models/BF2PageModel.cs