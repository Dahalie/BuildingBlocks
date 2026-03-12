# BuildingBlocks - Proje Talimatları

## Mimari Genel Bakış

Katmanlı, modüler bir .NET 10 altyapı kütüphanesi. Her katman bağımsız NuGet paketi olarak kullanılabilir.

### Katman Hiyerarşisi (bağımlılık yönü: yukarıdan aşağıya)

```
BuildingBlocks.Api
    └── BuildingBlocks.Infrastructure
BuildingBlocks.Persistence
    └── BuildingBlocks.Application
            ├── BuildingBlocks.Contracts
            └── BuildingBlocks.Domain
                    └── BuildingBlocks.Primitives
```

### Proje Yapısı

```
src/
├── BuildingBlocks.Primitives      → Result pattern, Error, Pagination, Exceptions, Extensions, IO Utilities
├── BuildingBlocks.Contracts       → Messaging contract'ları (IIntegrationEvent, IntegrationEventBase) — sıfır bağımlılık
├── BuildingBlocks.Domain          → IEntity<TId>, IAuditable<TUserId>, IDateTrackable, Repository soyutlamaları, IDomainService
├── BuildingBlocks.Application     → CQRS (MediatR), Behaviors, IPolicy, PolicyBase, ICurrentUserProvider<TUserId>, Messaging (IMessageBus, IOutboxWriter), Csv, Excel, Pdf, Html, Email, Security
├── BuildingBlocks.Infrastructure  → DateTimeProvider, LocalFileStorage, MessageBus (MassTransit), Serilog, HealthChecks, Csv, Excel, Pdf, Html, Email (SMTP), Security (AES/Hashing), Monitoring (OpenTelemetry), gRPC, Jobs (Quartz.NET), Caching
├── BuildingBlocks.Persistence     → EF Core repo implementasyonları, Outbox/Inbox, Interceptors
└── BuildingBlocks.Api             → Minimal API Endpoints, ExceptionMiddleware, ResultExtensions, HealthChecks, Localization, gRPC
tests/
├── BuildingBlocks.Primitives.Tests
├── BuildingBlocks.Domain.Tests
├── BuildingBlocks.Application.Tests
├── BuildingBlocks.Infrastructure.Tests
├── BuildingBlocks.Persistence.Tests
└── BuildingBlocks.Api.Tests
```

## Teknoloji Stack

- .NET 10.0, C# (nullable enabled, implicit usings)
- DI: Autofac (birincil) + Microsoft.Extensions.DependencyInjection (ASP.NET Core)
- CQRS: MediatR 14
- Validation: FluentValidation 12
- ORM: EF Core 10 + Npgsql (PostgreSQL)
- Messaging: MassTransit 8
- Logging: Serilog
- Monitoring: OpenTelemetry 1.15
- Jobs: Quartz.NET 3.16
- gRPC: Grpc.AspNetCore.Server / Grpc.Core.Api 2.76

## Konvansiyonlar

- Her modül kendi Autofac extension metodu ile register edilir (örn: `MediatorAutofacExtensions`, `ClockAutofacExtensions`)
- Microsoft DI extension'ları da mümkünse sağlanır (`*MicrosoftExtensions`)
- Interface-first tasarım: domain soyutlamaları (repository, domain service) `Domain` katmanında, application-level soyutlamalar (policy, messaging) `Application` katmanında, implementasyonlar `Infrastructure`/`Persistence`'da
- Hata yönetimi exception yerine `Result<T>` pattern ile yapılır
- Her yeni modül için ilgili `tests/` projesinde unit test yazılır

## Yeni Modül Ekleme Kuralları

### Dosya/Katman Kuralları
1. Soyutlama (interface) → `BuildingBlocks.Application` veya uygun katmana
2. Implementasyon → `BuildingBlocks.Infrastructure` veya uygun katmana
3. DI Extension → `*AutofacExtensions` ve/veya `*MicrosoftExtensions`
4. Unit test → ilgili test projesine
5. Mevcut katman hiyerarşisine uygun bağımlılık yönü korunmalı

### Geliştirme Süreci (ZORUNLU)
Yeni modül eklerken aşağıdaki adımlar **sırasıyla** takip edilir. Direkt implementasyona geçmek YASAKTIR.

1. **Tartışma** — Modülün kapsamı, bizim sisteme nasıl oturacağı, hangi katmanlara ne düşeceği, hangi soyutlamaların gerektiği konuşulur. Kullanıcıya sorular sorulur, alternatifler sunulur, kararlar birlikte alınır.
2. **Tasarım kararı** — Tartışma sonucunda dosya yapısı, interface'ler, NuGet paketleri, konfigürasyon yaklaşımı netleşir. Kullanıcı onaylar.
3. **Implementasyon** — Onaylanan tasarıma göre kod yazılır.

### Implementasyon İlkeleri
- Başka projelerden veya kütüphanelerden kod kopyalanmaz. Kendi mimarimize, konvansiyonlarımıza uygun şekilde sıfırdan yazılır.
- Bir teknolojinin "standart kullanımı"nı biliyoruz; mesele o teknolojiyi bizim katman yapımıza, DI pattern'ımıza, hata yönetimimize (`ErrorType`, `Result<T>`) nasıl entegre edeceğimizdir.
- Mevcut modüllerle tutarlılık esastır: aynı naming, aynı DI pattern, aynı test yaklaşımı.
- Her modül mevcut sistemi iyileştirebilir (örn: gRPC modülü `CustomException`'a `ErrorType` eklenmesini tetikledi). Bu tür çapraz iyileştirmeler teşvik edilir.

