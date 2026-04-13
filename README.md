# Clean Architecture con .NET

Proyecto que implementa **Clean Architecture** usando **.NET 10** y **C#**, con un enfoque práctico en buenas prácticas
de diseño de software, separación de responsabilidades y mantenibilidad.

---

## Arquitectura y Patrones

### Clean Architecture

La solución está organizada en cuatro capas con una dependencia unidireccional hacia adentro:

```
┌──────────────────────────────────┐
│           Web.Api                │  ← Capa de presentación (endpoints REST)
├──────────────────────────────────┤
│         Infrastructure           │  ← Acceso a datos, servicios externos
├──────────────────────────────────┤
│          Application             │  ← Casos de uso, lógica de aplicación
├──────────────────────────────────┤
│            Domain                │  ← Entidades, reglas de negocio
└──────────────────────────────────┘
```

**Regla fundamental:** las capas internas no dependen de las externas. El `Domain` no conoce ni a `Application`, ni a
`Infrastructure`, ni a `Web.Api`.

### CQRS (Command Query Responsibility Segregation)

Separación explícita entre operaciones de **escritura** (Commands) y de **lectura** (Queries) usando **MediatR**:

- `ICommand` / `ICommandHandler<TCommand>` — para operaciones que modifican estado.
- `IQuery<TResponse>` / `IQueryHandler<TQuery, TResponse>` — para consultas de solo lectura.

### Repository Pattern + Unit of Work

El acceso a la base de datos está abstraído mediante interfaces (`IProductRepository`, `IUnitOfWork`) definidas en el
`Domain` e implementadas en `Infrastructure` con **Entity Framework Core**.

### Result Pattern

En lugar de usar excepciones para errores de negocio, se retorna un tipo `Result<T>` que encapsula el valor o el error.
Esto hace el flujo de control explícito y predecible.

### Pipeline Behaviors (MediatR)

Los comportamientos transversales se inyectan como middlewares del pipeline de MediatR:

- **Logging Behavior** — registra cada comando/query con su duración.
- **Validation Behavior** — ejecuta las validaciones de FluentValidation antes del handler.

---

## Stack Tecnológico

| Componente        | Tecnología                              |
|-------------------|-----------------------------------------|
| Lenguaje          | C# (.NET 10)                            |
| Framework web     | ASP.NET Core                            |
| Base de datos     | PostgreSQL 17                           |
| ORM               | Entity Framework Core 10                |
| Mediator          | MediatR 14                              |
| Validaciones      | FluentValidation 12                     |
| Logging           | Serilog + Seq                           |
| HTTP Client       | Refit                                   |
| Caché             | LazyCache                               |
| Contenedores      | Docker + Docker Compose                 |
| Tests             | xUnit, FluentAssertions, Testcontainers |
| Análisis estático | SonarAnalyzer                           |

---

## Estructura del Proyecto

```
clean-architecture/
├── src/
│   ├── Domain/             # Entidades, value objects, interfaces de repositorios
│   ├── Application/        # Casos de uso (Commands y Queries), validaciones
│   ├── Infrastructure/     # EF Core, repositorios, clientes HTTP, caché, migraciones
│   └── Web.Api/            # Endpoints REST, middlewares, configuración DI
├── tests/
│   └── ArchitectureTests/  # Tests de arquitectura e integración
├── docker-compose.yml
└── CleanArchitecture.slnx
```

---

## Requisitos Previos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Aspire](https://aspire.dev/es/get-started/install-cli/)
---

## Levantar el Proyecto Localmente

### Opción 1: Con Docker Compose (recomendada)

Levanta la API y PostgreSQL en un solo comando:

```bash
aspire run
```

Los servicios estarán disponibles en el enlace indicado en consola ya que aspire lo genera y usualmente pone los puertos y swagger disponibles alli mismo.


## Tests

El proyecto incluye dos tipos de tests:

- **Tests de arquitectura** (`Layers/`) — validan que las dependencias entre capas se respeten usando *
  *NetArchTest.Rules**. Si una capa interna referencia a una externa, el test falla.
- **Tests de integración** (`IntegrationTests/`) — levantan una instancia real de PostgreSQL con **Testcontainers** y
  ejecutan los casos de uso completos.

```bash
dotnet test tests/ArchitectureTests/ArchitectureTests.csproj
```

---

## Variables de Configuración

El archivo `appsettings.Development.json` contiene la configuración para desarrollo local. Los valores más relevantes
son:

```json
{
  "ConnectionStrings": {
    "Database": "Host=postgres;Port=5432;Database=clean-architecture;Username=postgres;Password=postgres"
  },
  "Serilog": {
    "Using": [
      "Serilog.Sinks.Console",
      "Serilog.Sinks.Seq"
    ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Information"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "Seq",
        "Args": { "ServerUrl": "http://seq:5341" }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/log-.txt",
          "rollingInterval": "Day",
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ]
  }
}
```
