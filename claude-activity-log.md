## [2026-03-07 08:30] - MODÜL EKLENDİ: gRPC Desteği
- **Durum:** Tamamlandı
- **Bağlam:** gRPC modülü eklenmesi + CustomException iyileştirmesi + ExceptionHandlingMiddleware güncellenmesi
- **Detay:**
  - `CustomException`'a `ErrorType` property eklendi (default: `ErrorType.Exception`)
  - `ResultException`'a `ErrorType` destekli constructor'lar eklendi
  - Infrastructure: `ServerExceptionInterceptor` (ErrorType + standart exception mapping), `ServerLoggingInterceptor`, `ClientLoggingInterceptor`
  - Infrastructure: Autofac + Microsoft DI extension'ları
  - Api: `GrpcServerMicrosoftExtensions.AddGrpcWithInterceptors()` - server-side gRPC registration
  - Api: `ExceptionHandlingMiddleware` ErrorType bazlı HTTP status code mapping ile güncellendi
  - Api → Infrastructure proje referansı eklendi, CLAUDE.md hiyerarşi diyagramı güncellendi
  - Api ve Infrastructure projelerine `InternalsVisibleTo` eklendi
  - NuGet: `Grpc.Core.Api` 2.76.0 (Infrastructure), `Grpc.AspNetCore.Server` 2.76.0 (Api)
  - 195 test passed (67 Primitives + 3 Domain + 6 Application + 28 Api + 6 Persistence + 85 Infrastructure)
- **Çözüm:** Tamamlandı, sorunsuz.
---

## [2026-03-07 09:00] - MODÜL EKLENDİ: Monitoring (OpenTelemetry)
- **Durum:** Tamamlandı
- **Bağlam:** Monitoring modülü eklenmesi - Traces, Metrics, Logging
- **Detay:**
  - `ObservabilityOptions` - ServiceName, ServiceVersion, OtlpEndpoint, EnableTracing/Metrics/Logging
  - `AddObservability()` - OpenTelemetry tracing + metrics konfigürasyonu (ASP.NET Core, HttpClient, EF Core, MassTransit, Runtime instrumentation)
  - `UseSerilogWithObservability()` - Mevcut Serilog yapısını bozmadan OpenTelemetry sink ekleme
  - OTLP endpoint: options'dan veya `OTEL_EXPORTER_OTLP_ENDPOINT` env var'dan okunur
  - NuGet: OpenTelemetry.* 1.15.0 (6 paket), Serilog.Sinks.OpenTelemetry 4.2.0
  - 203 test passed (67 Primitives + 3 Domain + 6 Application + 28 Api + 6 Persistence + 93 Infrastructure)
- **Çözüm:** Tamamlandı, sorunsuz. Autofac extension yok (OTel tamamen Microsoft DI tabanlı).
---

## [2026-03-07 10:00] - MODÜL EKLENDİ: Notification - Email
- **Durum:** Tamamlandı
- **Bağlam:** Email modülü eklenmesi - IEmailSender soyutlaması + SMTP implementasyonu
- **Detay:**
  - Application: `EmailAddress` record, `EmailAttachment`, `EmailMessage` modeli, `IEmailSender` interface
  - Infrastructure: `SmtpEmailSender` (System.Net.Mail tabanlı, multipart HTML+plain text, attachment desteği)
  - Infrastructure: `SmtpOptions` (Host, Port, Username, Password, UseSsl, SenderAddress, SenderName)
  - Infrastructure: `SmtpEmailAutofacExtensions` + `SmtpEmailMicrosoftExtensions` (IConfigurationSection + callback overload)
  - CLAUDE.md: Geliştirme süreci kuralları eklendi (Tartışma → Tasarım → Implementasyon)
  - 224 test passed (67 Primitives + 3 Domain + 13 Application + 28 Api + 6 Persistence + 107 Infrastructure)
- **Çözüm:** Tamamlandı, sorunsuz. Ek NuGet bağımlılığı yok (System.Net.Mail yerleşik).
---

## [2026-03-07 10:30] - MODÜL EKLENDİ: Security (Encryption + Hashing)
- **Durum:** Tamamlandı
- **Bağlam:** Security modülü eklenmesi - IHasher + IEncryptor soyutlamaları
- **Detay:**
  - Application: `IHasher` (SHA256/384/512 + HMAC), `IEncryptor`, `HashAlgorithmType` enum
  - Infrastructure: `Hasher` (System.Security.Cryptography tabanlı, hex output, FixedTimeEquals HMAC verify)
  - Infrastructure: `AesEncryptor` (AES-256-GCM, random nonce per encrypt, base64 output)
  - Infrastructure: `AesEncryptionOptions` (Key - base64 encoded 256-bit)
  - Infrastructure: `SecurityAutofacExtensions` + `SecurityMicrosoftExtensions`
  - SMS modülü listeden çıkarıldı (evrensel protokol yok, consumer kendi tanımlar)
  - 253 test passed (67 Primitives + 3 Domain + 13 Application + 28 Api + 6 Persistence + 136 Infrastructure)
- **Çözüm:** Tamamlandı, sorunsuz. Ek NuGet bağımlılığı yok (System.Security.Cryptography yerleşik).
---

## [2026-03-07 11:00] - MODÜL EKLENDİ: Localization
- **Durum:** Tamamlandı
- **Bağlam:** Localization modülü - ASP.NET Core localization konfigürasyon sarmalama
- **Detay:**
  - Api: `LocalizationOptions` (DefaultCulture, SupportedCultures, ResourcesPath, UseHeader/QueryString/Cookie)
  - Api: `LocalizationMicrosoftExtensions` - `AddLocalization(IConfigurationSection)` + `UseLocalization()` middleware
  - SupportedCultures default boş array (Bind() append sorunu nedeniyle), boşsa DefaultCulture otomatik eklenir
  - Yeni soyutlama yok, .NET'in `IStringLocalizer` olduğu gibi kullanılır
  - 262 test passed (67 Primitives + 3 Domain + 13 Application + 37 Api + 6 Persistence + 136 Infrastructure)
- **Çözüm:** Tamamlandı. Bind() array append davranışı testte yakalandı, default `[]` yapılarak düzeltildi.
---

## [2026-03-07 11:30] - MODÜL EKLENDİ: Distributed Cache
- **Durum:** Tamamlandı
- **Bağlam:** Distributed Cache modülü - IDistributedCache generic serialization extension'ları
- **Detay:**
  - Infrastructure: `DistributedCacheExtensions` - `GetAsync<T>()`, `SetAsync<T>()`, `GetOrSetAsync<T>()` (JSON serialization)
  - Yeni soyutlama veya NuGet bağımlılığı yok, `IDistributedCache` üzerine extension method'lar
  - Configuration & Secrets ve Multi-Provider Storage modülleri listeden çıkarıldı (vendor SDK bağımlılığı, .NET yerleşik mekanizmalar yeterli)
  - 269 test passed (67 Primitives + 3 Domain + 13 Application + 37 Api + 6 Persistence + 143 Infrastructure)
- **Çözüm:** Tamamlandı, sorunsuz.
---
