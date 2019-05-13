# Codehaks.Pagination

![alt text](https://github.com/codehaks/Codehaks.Pagination/blob/master/pagination.JPG "codehaks.pagination")

## Setup
Use nuget to download and install package :

**PM> Install-Package Codehaks.Pagination -Version 1.0.0**

### Intoduction
This is a very simple easy to use pagination Tag Helper for ASP.NET Core.

### Instalation
install using nuget :
~~~code
PM > Install-Package Codehaks.Pagination -Version 1.0.0
~~~

also you need to reference Codehaks.TagHelpers in you view or _viewImports.cshtml 
~~~csharp
@addTagHelper *, Codehaks.Pagination
~~~

you can download sample project to see it in action:
[Demo project](https://github.com/codehaks/Codehaks.Pagination/tree/master/PeopleApp)

### Usage
on your view (razor page or MVC) add <Pagination> tag:
  ~~~html
  <Pagination page-count="@Model.TotalPages" page-target="/index" page-number="@Model.PageNumber" page-range="10"></Pagination>
  ~~~
  
 *page-count* is the number of pages in your list which should be provided from your server-side code.
 
 *page-target* is the path to send/get next page.
 
 *page-range* is the range of pagination buttons to be generated. 
 *page-first* to rename the title of "First" button to other languages like "اول"
 *page-last* to rename the title of "Last" button to other languages like "آخر"
 
 ### Bugs & Issues
 please report any bugs or issues you encounter. 
 
 
