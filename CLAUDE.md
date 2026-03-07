# BuildingBlocks - Proje Talimatları

## Mimari Genel Bakış

Katmanlı, modüler bir .NET 10 altyapı kütüphanesi. Her katman bağımsız NuGet paketi olarak kullanılabilir.

### Katman Hiyerarşisi (bağımlılık yönü: yukarıdan aşağıya)

```
BuildingBlocks.Api
BuildingBlocks.Infrastructure
BuildingBlocks.Persistence
    └── BuildingBlocks.Application
            └── BuildingBlocks.Domain
                    └── BuildingBlocks.Primitives
```

### Proje Yapısı

```
src/
├── BuildingBlocks.Primitives      → Result pattern, Error, Pagination, Exceptions, Extensions
├── BuildingBlocks.Domain          → IEntity, IAuditable, IDateTrackable
├── BuildingBlocks.Application     → CQRS (MediatR), Behaviors, Repository soyutlamaları, Messaging, ConstraintCheck, Csv, Excel
├── BuildingBlocks.Infrastructure  → DateTimeProvider, LocalFileStorage, MessageBus (MassTransit), Serilog, HealthChecks, Csv, Excel
├── BuildingBlocks.Persistence     → EF Core repo implementasyonları, Outbox/Inbox, Interceptors
└── BuildingBlocks.Api             → Minimal API Endpoints, CurrentUserProvider, ExceptionMiddleware, ResultExtensions, HealthChecks
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

## Konvansiyonlar

- Her modül kendi Autofac extension metodu ile register edilir (örn: `MediatorAutofacExtensions`, `ClockAutofacExtensions`)
- Microsoft DI extension'ları da mümkünse sağlanır (`*MicrosoftExtensions`)
- Interface-first tasarım: önce `Application` katmanında soyutlama, sonra `Infrastructure`/`Persistence`'da implementasyon
- Hata yönetimi exception yerine `Result<T>` pattern ile yapılır
- Her yeni modül için ilgili `tests/` projesinde unit test yazılır

## Yeni Modül Ekleme Kuralları

1. Soyutlama (interface) → `BuildingBlocks.Application` veya uygun katmana
2. Implementasyon → `BuildingBlocks.Infrastructure` veya uygun katmana
3. DI Extension → `*AutofacExtensions` ve/veya `*MicrosoftExtensions`
4. Unit test → ilgili test projesine
5. Mevcut katman hiyerarşisine uygun bağımlılık yönü korunmalı

---

## Eklenecekler Listesi

Aşağıdaki modüller mevcut mimariye uygun şekilde, adım adım eklenecektir. Her modül tamamlandığında bu listeden silinir.

### 1. PDF İşlemleri
- **Katman:** Infrastructure
- **Amaç:** PDF oluşturma (fatura, rapor, döküman)
- **Yaklaşım:** Interface `Application`'da, implementasyon `Infrastructure`'da

### 2. HTML İşlemleri
- **Katman:** Infrastructure
- **Amaç:** HTML parse/render (email template, rapor üretimi)
- **Yaklaşım:** Interface `Application`'da, implementasyon `Infrastructure`'da

### 3. gRPC Desteği
- **Katman:** Infrastructure
- **Amaç:** Servisler arası yüksek performanslı iletişim
- **Yaklaşım:** gRPC interceptor'lar, servis bazlı altyapı

### 4. Monitoring
- **Katman:** Infrastructure + Api
- **Amaç:** Performans izleme, metrik toplama (OpenTelemetry)
- **Yaklaşım:** Traces, metrics, ve instrumentation

### 5. Notification - Email
- **Katman:** Application (interface) + Infrastructure (implementasyonlar)
- **Amaç:** Email gönderimi
- **Yaklaşım:** `IEmailSender` soyutlaması, SMTP/SendGrid/SES implementasyonları

### 6. Notification - SMS
- **Katman:** Application (interface) + Infrastructure (implementasyonlar)
- **Amaç:** SMS bildirimleri
- **Yaklaşım:** `ISmsSender` soyutlaması, Twilio/Azure Communication implementasyonları

### 7. Security
- **Katman:** Infrastructure
- **Amaç:** Encryption, hashing, data protection
- **Yaklaşım:** `IEncryptionService`, `IHashingService` soyutlamaları

### 8. Localization
- **Katman:** Infrastructure + Api
- **Amaç:** Çoklu dil desteği (i18n/l10n)
- **Yaklaşım:** Resource-based localization altyapısı

### 9. Configuration & Secrets
- **Katman:** Infrastructure
- **Amaç:** Harici secret yönetimi (Azure Key Vault, AWS Secrets Manager, HashiCorp Vault)
- **Yaklaşım:** `ISecretProvider` soyutlaması, provider implementasyonları

### 10. Multi-Provider Storage
- **Katman:** Infrastructure
- **Amaç:** Cloud storage desteği (S3, Azure Blob, GCS)
- **Yaklaşım:** Mevcut `IFileStorage` interface'ini kullanarak yeni provider'lar

### 11. Distributed Cache
- **Katman:** Infrastructure
- **Amaç:** Redis, SQL Server distributed cache
- **Yaklaşım:** `IDistributedCache` üzerine wrapper ve DI extension'ları

### 12. HTTP Handlers
- **Katman:** Infrastructure
- **Amaç:** HttpClient middleware (retry, circuit breaker, logging)
- **Yaklaşım:** `DelegatingHandler` implementasyonları, Polly entegrasyonu

### 13. Hosted Services / Background Jobs
- **Katman:** Infrastructure
- **Amaç:** Arka plan iş yönetimi
- **Yaklaşım:** `BackgroundService` base sınıfları, recurring job altyapısı

### 14. IO Utilities
- **Katman:** Infrastructure veya Primitives
- **Amaç:** Stream, dosya, path yardımcı işlemleri
- **Yaklaşım:** Extension method'lar ve utility sınıfları
