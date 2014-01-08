# .NET Client for Netflix's Suro Service

Netflix's Suro repository can be found here: https://github.com/netflix/suro

## Package Installation

This library has been published as a Nuget package

    Install-Package Suro.Net

## Client Usage

Integration with Suro is through TCP Keel-Alive connections, meaning that the client library must make use of pooled connections.

The simplest use case is as follows:

    using (var connectionPool = new SuroConnectionPool(hostname, port))
    using (var conn = connectionPool.Acquire())
    {
        try
        {
            conn.Connect();
            ...
            connection.Send(messageSet);
        }
        catch (SuroException)
        {
            ...
        }
    }

## Building a MessageSet

The simplest way to construct an instance of `TMessageSet` (as expected by the Suro service interface) without coding directly to the API is to use the `MessageSetBuilder` helper class.

    var bob = new MessageSetBuilder()
        .AddMessage(item.RoutingKey, item.Data);
    ...
    connection.Send(bob.Build());

Messages can be compressed prior to sending with a call to `WithCompression(...)` during message set construction, as follows:

    var bob = new MessageSetBuilder()
        .WithCompression(CompressionType.Lzo)
        .AddMessage(item.RoutingKey, item.Data);
    ...
    connection.Send(bob.Build());

## ASP.NET Integration

The library includes an HTTP Module and Request Context utility class for hooking your ASP.NET web application up to the Suro service.

Step 1: Add a reference to the Suro.Net library to your web application

Step 2: Add a configuration section to Web.config:

    <configSections>
        ...
        <section name="suro" type="Suro.Net.Configuration.SuroConfigurationSection, Suro.Net"/>
    </configSections>

Step 3: Add the server configuration and configure accordingly:

    <appSettings>
        ...
    </appSettings>
    ...
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