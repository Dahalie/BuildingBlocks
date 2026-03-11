# `IValidator<T>`, `IBusinessRule`, `IPolicy`, `ISpecification<T>` Karşılaştırması

Bu doküman, yazılım içinde sık karışan dört kavramı ayırmak için hazırlanmıştır:

- `IValidator<T>`
- `IBusinessRule`
- `IPolicy`
- `ISpecification<T>`

Özellikle anemic domain model, result pattern ve uygulama seviyesinde kural kontrolü yapan yapılarda bu ayrım faydalıdır.

---

## Kısa Özet

| Kavram               | Ne anlatır?                        | Tipik soru                             | Kullanım alanı                            |
|----------------------|------------------------------------|----------------------------------------|-------------------------------------------|
| `IValidator<T>`      | Gelen verinin biçimsel geçerliliği | "Bu input geçerli mi?"                 | API, DTO, request, command validation     |
| `IBusinessRule`      | Tekil bir iş kuralı               | "Şu iş kuralı ihlal edildi mi?"       | Atomik domain/business kuralı             |
| `IPolicy`            | Karar mantığı veya kural seti     | "Bu durumda nasıl davranacağız?"       | Use-case, application service, karar verme |
| `ISpecification<T>`  | Bir nesne bir koşulu sağlıyor mu  | "Bu nesne bu kritere uyuyor mu?"       | Eligibility, filtreleme, seçim, sorgu      |

---

## 1. `IValidator<T>`

### Tanım

`IValidator<T>`, dışarıdan gelen verinin **şekilsel**, **biçimsel** veya **yapısal olarak** geçerli olup olmadığını kontrol eder.

Bu genelde şu tür kontrolleri kapsar:

- Zorunlu alan boş mu
- Format doğru mu
- Sayı aralık içinde mi
- Tarih parse edilebiliyor mu
- String uzunluğu uygun mu

Bu katman çoğu zaman request/DTO/command seviyesinde çalışır.

### Tipik Soru

> "Bu input kapıdan içeri girebilir mi?"

### Örnek Interface

```csharp
public interface IValidator<in T>
{
    ValidationResult Validate(T input);
}
```

### Örnek Kullanım

- `CreateOrderRequestValidator`
- `RegisterUserCommandValidator`

### Örnek Kurallar

- Email boş olamaz
- Email formatı geçerli olmalı
- `Quantity > 0` olmalı
- `Name` en fazla 200 karakter olabilir

### Ne Değildir?

Genelde şunlar saf validator konusu **değildir**:

- DB'de duplicate var mı
- Gönderilen `CustomerId` gerçekten mevcut mu
- Müşteri aktif mi
- Sipariş bu durumda değiştirilebilir mi

Bunlar artık sistemin mevcut state'ine bakmayı gerektirir ve çoğu zaman validation sınırını aşar.

---

## 2. `IBusinessRule`

### Tanım

`IBusinessRule`, tekil ve anlamlı bir iş kuralını temsil eder.

Bu daha çok _"şu iş açısından yasaktır"_ veya _"şu koşul sağlanmak zorundadır"_ gibi kurallar için uygundur.

### Tipik Soru

> "Şu iş kuralı ihlal edildi mi?"

### Örnek Interface

```csharp
public interface IBusinessRule
{
    bool IsBroken();
    string Message { get; }
}
```

Alternatif:

```csharp
public interface IBusinessRule<in TContext>
{
    RuleCheckResult Check(TContext context);
}
```

### Örnek Kullanım

- `InactiveCustomerCannotPlaceOrderRule`
- `CancelledOrderCannotBeModifiedRule`
- `DiscountCannotExceedLimitRule`

### Örnek Kurallar

- Pasif müşteri sipariş veremez
- İptal edilmiş sipariş değiştirilemez
- İndirim oranı limiti aşamaz
- Aynı kampanya ikinci kez kullanılamaz

### Güçlü Yanı

- İş kuralları isimlendirilmiş olur
- Test etmesi kolaydır
- Hata mesajı anlamlı olur
- Rule ihlali açıkça modellenmiş olur

### Zayıf Yanı

- Fazla atomik kullanılırsa sınıf sayısı artabilir
- Küçük/orta projelerde fazla törenli hale gelebilir

---

## 3. `IPolicy`

### Tanım

`IPolicy`, bir bağlam içinde **nasıl karar verileceğini** veya **hangi kural setinin uygulanacağını** temsil eder.

Tek bir kural olmak zorunda değildir. Çoğu zaman birden fazla koşulu birlikte değerlendirir.

### Tipik Soru

> "Bu durumda nasıl davranacağız?"
>
> "Bu işlem yapılabilir mi?"
>
> "Bu karar hangi kurallara göre verilecek?"

### Örnek Interface

```csharp
public interface IPlaceOrderPolicy
{
    Result Ensure(Order order, Customer customer);
}
```

Alternatif:

```csharp
public interface IDiscountPolicy
{
    Money CalculateDiscount(Customer customer, Order order);
}
```

### Örnek Kullanım

- `IPlaceOrderPolicy`
- `ICancelOrderPolicy`
- `IAddOrderItemPolicy`
- `IDiscountPolicy`
- `IRefundPolicy`

### Örnek Senaryolar

- Sipariş oluşturulabilir mi
- İade yapılabilir mi
- Bu müşteriye hangi indirim uygulanır
- Bu kullanıcı bu role atanabilir mi

### Güçlü Yanı

- Use-case seviyesinde çok doğaldır
- Birden fazla kontrolü tek yerde toplayabilir
- Result pattern ile uyumludur
- Anemic domain model'de pratik çalışır

### Zayıf Yanı

- Sınırlar iyi çizilmezse "god policy" oluşabilir
- Çok fazla sorumluluk bir policy'ye birikebilir

---

## 4. `ISpecification<T>`

### Tanım

`ISpecification<T>`, bir nesnenin belirli bir koşulu sağlayıp sağlamadığını ifade eder.

Bu kavram daha çok bir **predicate** veya **uygunluk ölçütü** gibi düşünülebilir.

### Tipik Soru

> "Bu nesne şu kritere uyuyor mu?"

### Örnek Interface

```csharp
public interface ISpecification<in T>
{
    bool IsSatisfiedBy(T candidate);
}
```

### Örnek Kullanım

- `ActiveCustomerSpecification`
- `RefundableOrderSpecification`
- `OrderEligibleForDiscountSpecification`

### Örnek Senaryolar

- Bu müşteri aktif mi
- Bu sipariş iade edilebilir mi
- Bu ürün kampanyaya uygun mu
- Bu aday seçilebilir mi

### Güçlü Yanı

- Filtreleme ve uygunluk mantığında temizdir
- `And`, `Or`, `Not` ile compose edilebilir
- Domain diline yakın okunur

### Zayıf Yanı

- Sadece `true`/`false` çoğu zaman yetmez
- Neden başarısız olduğunu açıklamak gerekirse yetersiz kalabilir
- Karar verme veya sonuç üretme gereken yerlerde policy daha uygun olabilir

---

## Kavramsal Farklar

### `IValidator<T>` vs `IBusinessRule`

| `IValidator<T>`                               | `IBusinessRule`                                 |
|------------------------------------------------|-------------------------------------------------|
| Input doğruluğuna bakar                        | İş anlamı taşıyan kuralı temsil eder            |
| Şekilsel/yapısal kontroldür                    | Mevcut state ve domain bilgisi gerekebilir      |
| Çoğu zaman request seviyesinde çalışır         | Çoğu zaman "yasak/izin" mantığı vardır          |

**Örnek:**

- `CreateUserRequestValidator` → email boş mu, email formatı doğru mu
- `EmailMustBeUniqueRule` → bu email sistemde zaten var mı

---

### `IBusinessRule` vs `IPolicy`

| `IBusinessRule`                                | `IPolicy`                                       |
|------------------------------------------------|-------------------------------------------------|
| Daha atomik                                    | Daha kapsayıcı                                  |
| Tek bir kurala odaklanır                       | Birden fazla kuralı birlikte ele alabilir        |
| "Şu ihlal edildi mi?" mantığı taşır           | Karar verme mantığı içerir                       |

**Örnek:**

Business rules:
- Müşteri aktif olmalı
- Sipariş iptal edilmemiş olmalı
- Limit aşılmamalı

Policy:
- `PlaceOrderPolicy` bu kuralları **birlikte** değerlendirir

---

### `IBusinessRule` vs `ISpecification<T>`

| `IBusinessRule`                                | `ISpecification<T>`                             |
|------------------------------------------------|-------------------------------------------------|
| İhlal/yasak dili taşır                         | Uygunluk/eleme dili taşır                       |
| "Bunu yapamazsın" gibi okunur                  | "Şu kritere uyuyor mu" gibi okunur              |

**Örnek:**

- `InactiveCustomerCannotPlaceOrderRule`
- `ActiveCustomerSpecification`

İkisinin mantığı benzer olabilir, ama **ifade biçimi** farklıdır.

---

### `IPolicy` vs `ISpecification<T>`

| `IPolicy`                                      | `ISpecification<T>`                             |
|------------------------------------------------|-------------------------------------------------|
| Karar verir                                    | Genelde sadece uygunluk kontrol eder            |
| Bazen hesaplama yapar                          | Çoğu zaman `bool` döner                         |
| Birden fazla veriyle çalışabilir               |                                                  |
| `Result`, `Decision`, `Money` gibi sonuçlar    |                                                  |

**Örnek:**

- `DiscountEligibilitySpecification` → müşteri indirime uygun mu
- `DiscountPolicy` → uygunsa ne kadar indirim uygulanır

---

## Hangi Durumda Hangisi Kullanılmalı?

### `IValidator<T>` kullan

Şu soruları soruyorsan:
- Request düzgün mü
- Zorunlu alanlar dolu mu
- Format doğru mu
- Değer aralık içinde mi

### `IBusinessRule` kullan

Şu durumda uygundur:
- Tekil bir iş kuralını modellemek istiyorsan
- Rule ihlalini açık adlandırmak istiyorsan
- İş kuralını ayrı test etmek istiyorsan

### `IPolicy` kullan

Şu durumda uygundur:
- Use-case seviyesinde karar veriyorsan
- Birden fazla kural birlikte çalışıyorsa
- Result pattern ile akmak istiyorsan
- Anemic domain model kullanıyorsan

### `ISpecification<T>` kullan

Şu durumda uygundur:
- Eligibility/uygunluk kontrolü yapıyorsan
- Filtreleme yapıyorsan
- Kuralları compose etmek istiyorsan

---

## Anemic Domain Model İçin Öneri

Anemic domain model'de entity içine davranış ve invariant koymadığın için kurallar daha çok **dışarıda** yaşar.

Bu durumda tipik yapı şöyle olabilir:

1. `IValidator<TRequest>` → request/input validation
2. `IPolicy` → use-case seviyesinde karar/kontrol
3. Gerekirse policy içinde veya yanında `IBusinessRule`
4. Uygunluk/filtreleme gereken yerlerde `ISpecification<T>`

### Önerilen Omurga

```
CreateOrderRequestValidator
IPlaceOrderPolicy
ICancelOrderPolicy
IAddOrderItemPolicy
```

İhtiyaç artarsa:

```
CustomerMustBeActiveRule
CancelledOrderCannotBeModifiedRule
ActiveCustomerSpecification
```

---

## DB Constraint, Existence Check ve Duplicate Check Nereye Oturur?

Bazı kontroller şekilsel validation değildir, ama uygulama seviyesinde DB'ye gitmeden önce kontrol edilmek istenir.

### FK var mı?

Bu genelde:
- Existence check
- Reference existence check
- Referential integrity check

olarak düşünülür.

**DB tarafındaki karşılığı:** foreign key constraint

### Duplicate var mı?

Bu genelde:
- Duplicate check
- Uniqueness check

olarak düşünülür.

**DB tarafındaki karşılığı:** unique constraint / unique index

### Bunlar `IValidator<T>` mı?

Saf anlamda genelde **hayır**. Çünkü:

- DB state'ine bakarlar
- Repository çağırırlar
- Mevcut veriyi sorgularlar

Bu yüzden çoğu zaman **policy** içinde ele alınmaları daha doğaldır.

### Result Pattern Kullanan Yapı İçin Pratik Öneri

DB constraint'ler yine yerinde kalmalı. Çünkü onlar **nihai garantidir**.

Uygulama tarafında yapılan kontrollerin amacı:

- Exception atmadan önce erken dönmek
- Kullanıcıya daha anlamlı hata vermek
- `Result.Failure(...)` dönebilmek

### Sağlıklı Akış

1. `IValidator<TRequest>` çalışır
2. `IPolicy` çalışır
3. Save yapılır
4. Yine de DB constraint exception gelirse, bu da `Result.Failure(...)`'a çevrilir

Çünkü **yarış durumu her zaman mümkündür**. Uygulama tarafındaki pre-check, DB garantisinin yerine geçmez.

---

## Örnek Kullanım Stili

### Request Validator

```csharp
public class CreateUserRequestValidator : IValidator<CreateUserRequest>
{
    public ValidationResult Validate(CreateUserRequest input)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(input.Email))
            errors.Add("Email boş olamaz.");

        if (input.Name?.Length > 200)
            errors.Add("Name en fazla 200 karakter olabilir.");

        return errors.Count == 0
            ? ValidationResult.Success()
            : ValidationResult.Failure(errors);
    }
}
```

### Policy

```csharp
public interface ICreateUserPolicy
{
    Task<Result> EnsureAsync(CreateUserRequest request, CancellationToken cancellationToken);
}

public class CreateUserPolicy : ICreateUserPolicy
{
    private readonly IUserRepository _users;
    private readonly IRoleRepository _roles;

    public async Task<Result> EnsureAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        if (await _users.ExistsByEmailAsync(request.Email, cancellationToken))
            return Result.Failure("Bu email zaten kullanımda.");

        if (!await _roles.ExistsAsync(request.RoleId, cancellationToken))
            return Result.Failure("Gönderilen rol bulunamadı.");

        return Result.Success();
    }
}
```

### Specification

```csharp
public class ActiveCustomerSpecification : ISpecification<Customer>
{
    public bool IsSatisfiedBy(Customer candidate)
        => candidate.IsActive;
}
```

### Business Rule

```csharp
public class InactiveCustomerCannotPlaceOrderRule : IBusinessRule
{
    private readonly Customer _customer;

    public InactiveCustomerCannotPlaceOrderRule(Customer customer)
    {
        _customer = customer;
    }

    public bool IsBroken() => !_customer.IsActive;

    public string Message => "Pasif müşteri sipariş veremez.";
}
```

---

## Önerilen İsimlendirme Yaklaşımı

### İyi Örnekler

**Validation:**
- `CreateOrderRequestValidator`
- `RegisterUserCommandValidator`

**Business Rule:**
- `CustomerMustBeActiveRule`
- `CancelledOrderCannotBeModifiedRule`

**Policy:**
- `IPlaceOrderPolicy`
- `ICreateUserPolicy`
- `IRefundPolicy`

**Specification:**
- `ActiveCustomerSpecification`
- `RefundableOrderSpecification`

### Kaçınılabilecek Şeyler

- Her şeye `Validator` demek
- Her küçük kontrol için ayrı soyutlama üretmek
- Policy ve Rule isimlerini tamamen rastgele kullanmak

---

## Sonuç

Bu dört kavram aynı şey değildir, ama birbirine yakın yerlerde kullanılır.

### En Pratik Ayrım

| Kavram              | Soru                                               |
|---------------------|-----------------------------------------------------|
| `IValidator<T>`     | Input geçerli mi?                                   |
| `IBusinessRule`     | Tekil iş kuralı ihlal edildi mi?                    |
| `IPolicy`           | Bu durumda hangi karar/kural seti uygulanacak?      |
| `ISpecification<T>` | Bu nesne şu kritere uyuyor mu?                      |

### Anemic Domain Model İçin Önerilen Yaklaşım

1. `IValidator<T>` ile request validation yap
2. Use-case seviyesinde `IPolicy` kullan
3. Kural sayısı arttıkça atomik `IBusinessRule` çıkar
4. Uygunluk/filtreleme gereken yerlerde `ISpecification<T>` kullan
5. DB constraint'leri her zaman son garanti olarak bırak

Bu ayrım, hem isimlendirmeyi temiz tutar hem de validation, business logic ve persistence concern'lerini birbirine karıştırmayı azaltır.
