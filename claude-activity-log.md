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
