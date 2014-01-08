# .NET Client for Netflix's Suro Service

Netflix's Suro repository can be found here: https://github.com/netflix/suro

## ASP.NET Integration

The library includes an HTTP Module and Request Context utility class for hooking your ASP.NET web application up to the Suro service.

Step 1: Add a reference to the Suro.Net library to your web application

Step 2: Add a configuration section to Web.config:

    <appSettings>
        ...
    </appSettings>
    ...
    <section name="suro" type="Suro.Net.Configuration.SuroConfigurationSection, Suro.Net"/>
    ...
    <system.web>
        ...

Step 3: Add the server configuration and configure accordingly:

    <suro host="localhost" port="7101" poolSize="5" compressionEnabled="false"/>

Step 4: Add the HTTP Module configuration under system.web/httpModules:

    <httpModules>
        ...
        <!-- Add the Suro module -->
        <add name="SuroModule" type="Suro.Net.Web.SuroModule, Suro.Net" />
    </httpModules>

Step 5: Add the HTTP Module configuration under system.webServer/modules:

    <modules>
        <!-- Add the Suro module -->
        <add name="SuroModule" type="Suro.Net.Web.SuroModule, Suro.Net" />
    </modules>

Step 6: Add calls to the `SuroContext` class to add messages to the Suro message set which will be sent at the end of the request.

    SuroContext.Current.Add("todo_login_success", model);