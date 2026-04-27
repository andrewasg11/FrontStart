## Project Structure

~~~

├── src/
│   ├── Library.Api/                 # Presentation Layer (API Endpoints & Config)
│   │   ├── Controllers/             # API Route Handlers (Books, Borrows, Members)
│   │   ├── Middleware/              # Global Error Handling
│   │   ├── Properties/              # Debug/Launch Profiles
│   │   ├── appsettings.json         # Configuration & Connection Strings
│   │   ├── Library.Api.csproj       # Project Metadata
│   │   ├── Library.http             # API Testing File
│   │   └── Program.cs               # App Startup & Dependency Injection
│   │
│   ├── Library.Application/         # Application Layer (Business Logic)
│   │   ├── DTOs/                    # Data Transfer Objects for API Requests/Responses
│   │   ├── Exceptions/              # Custom Domain-Specific Exceptions
│   │   ├── Interfaces/              # Service Contracts
│   │   ├── Services/                # Logic Implementations (e.g., Borrowing rules)
│   │   └── Library.Application.csproj
│   │
│   ├── Library.Domain/              # Domain Layer (Core Entities & Contracts)
│   │   ├── Entities/                # Database Models (Book, Member, etc.)
│   │   ├── Interfaces/              # Repository Contracts (Data access definitions)
│   │   └── Library.Domain.csproj
│   │
│   └── Library.Infrastructure/      # Infrastructure Layer (Data Access)
│       ├── Data/                    # Database Context (EF Core)
│       ├── Repositories/            # Concrete Data Access Implementations
│       └── Library.Infrastructure.csproj
│
├── .gitignore                       # Ignored Git Files (bin/obj/etc.)
├── Library.sln                      # Visual Studio Solution File
└── README.md                        # Project Documentation

~~~