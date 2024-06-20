
# Biblioteca

Api web basica de biblioteca hecho con aspnet core 8, mysql, jwt para autenticacion y documentación con swagger.

### Conexion Database
Cambiar la configuracion con su respectiva conexion de base de datos

```sh
"DefaultConnection": "server=localhost;port=3306;database=library-aspnet-2024;user=root;Password='';sslMode=none"
```

### Servicio de Correo
Las pruebas estan realizadas con MailTrap, cambiar la configuracion con sus respectivas credenciales

```sh
"MailSettings": {
    "Server": "sandbox.smtp.mailtrap.io",
    "Port": 2525,
    "SenderName": "Library",
    "SenderEmail": "library.town@org.com",
    "UserName": "c2339fea42601f",
    "Password": "447d3d96bd8cc3"
  }
```

### Librerias
Principales librerias usadas: 

| Plugin | Info |
| ------ | ------ |
| [PomeloMysql] | Es un proveedor de Entity Framework Core (EF Core) para MySQL, MariaDB, Amazon Aurora, Azure Database para MySQL y otras bases de datos compatibles con MySQL
| [AutoMapper] | AutoMapper es una pequeña biblioteca sencilla creada para resolver un problema engañosamente complejo: deshacerse del código que asigna un objeto a otro.
| [FluentValidation] | FluentValidation es una biblioteca de validación para .NET que utiliza una interfaz fluida y expresiones lambda para crear reglas de validación fuertemente tipadas.
| [JwtAuthentication] | Middleware ASP.NET Core que permite que una aplicación reciba un token para la autenticacion.
| [MailKit] | MailKit es una biblioteca de cliente de correo multiplataforma construida sobre MimeKit.
| [ODATA] | Mejora la manera en que se visualiza las respuestas de las tablas relacionadas

## _English_

# Library
Basic web API for a library built with ASP.NET Core 8, MySQL, JWT for authentication, and documented with Swagger.

### Database Connection
Update the configuration with its respective database connection.

```sh
"DefaultConnection": "server=localhost;port=3306;database=library-aspnet-2024;user=root;Password='';sslMode=none"
```

### Email Service
Testing has been done with MailTrap; update the configuration with its respective credentials.

```sh
"MailSettings": {
    "Server": "sandbox.smtp.mailtrap.io",
    "Port": 2525,
    "SenderName": "Library",
    "SenderEmail": "library.town@org.com",
    "UserName": "c2339fea42601f",
    "Password": "447d3d96bd8cc3"
  }
```

### Libraries
Main libraries used:

| Library |	Description |
| ------ | ------ |
| [PomeloMysql] | Provides Entity Framework Core (EF Core) provider for MySQL, MariaDB, Amazon Aurora, Azure Database for MySQL, and other MySQL-compatible databases.
| [AutoMapper] | AutoMapper is a simple library built to solve a deceptively complex problem: getting rid of code that maps one object to another.
| [FluentValidation] |	FluentValidation is a .NET validation library that uses a fluent interface and lambda expressions for building strongly-typed validation rules.
| [JwtAuthentication] |	ASP.NET Core middleware that enables an application to receive a token for authentication.
| [MailKit] |	MailKit is a cross-platform mail client library built on MimeKit.
| [ODATA] |	Enhances the way related table responses are visualized.

[PomeloMysql]: <https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/9.0.0-preview.1#readme-body-tab>
[AutoMapper]: <https://www.nuget.org/packages/AutoMapper>
[FluentValidation]: <https://www.nuget.org/packages/FluentValidation>
[JwtAuthentication]: <https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer/9.0.0-preview.4.24267.6>
[MailKit]: <https://www.nuget.org/packages/MailKit>
[ODATA]: <https://www.nuget.org/packages/Microsoft.AspNetCore.OData>