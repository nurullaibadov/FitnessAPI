# 🏋️ Fitness Workout API

**Onion Architecture | .NET 8 | SQL Server | JWT Auth**

---

## 🚀 Quick Start

### 1. appsettings.json Ayarla
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=FitnessDB;Trusted_Connection=true;TrustServerCertificate=true;"
},
"JwtSettings": {
  "SecretKey": "YourSuperSecretKeyThatIsAtLeast32Characters!!2024"
}
```

### 2. Migration Oluştur ve Uygula
```bash
# API projesinden çalıştır
cd src/Presentation/FitnessAPI.API

dotnet ef migrations add InitialCreate \
  --project ../../Infrastructure/FitnessAPI.Persistence \
  --startup-project . \
  --output-dir Migrations

dotnet ef database update \
  --project ../../Infrastructure/FitnessAPI.Persistence \
  --startup-project .
```

### 3. Uygulamayı Başlat
```bash
dotnet run
# Swagger: https://localhost:7001
```

---

## 🔑 Default Admin Hesabı
- **Email:** admin@fitnessapp.com
- **Password:** Admin@123456

---

## 📡 API Endpoints

### Auth
| Method | Endpoint | Açıklama |
|--------|----------|----------|
| POST | /api/v1/auth/register | Kayıt ol |
| POST | /api/v1/auth/login | Giriş yap |
| POST | /api/v1/auth/refresh-token | Token yenile |
| POST | /api/v1/auth/forgot-password | Şifre sıfırlama maili |
| POST | /api/v1/auth/reset-password | Şifre sıfırla |
| POST | /api/v1/auth/change-password | Şifre değiştir |
| GET  | /api/v1/auth/verify-email | Email doğrula |
| POST | /api/v1/auth/logout | Çıkış yap |

### Users
| Method | Endpoint | Açıklama |
|--------|----------|----------|
| GET | /api/v1/users/me | Kendi profili getir |
| PUT | /api/v1/users/me | Profil güncelle |
| POST | /api/v1/users/me/profile-image | Profil fotoğrafı yükle |

### Workout Plans
| Method | Endpoint | Açıklama |
|--------|----------|----------|
| GET | /api/v1/workoutplans | Tüm planlar (sayfalı) |
| GET | /api/v1/workoutplans/public | Public planlar |
| GET | /api/v1/workoutplans/{id} | Plan detayı |
| POST | /api/v1/workoutplans | Yeni plan oluştur |
| PUT | /api/v1/workoutplans/{id} | Plan güncelle |
| DELETE | /api/v1/workoutplans/{id} | Plan sil |

### Exercises
| Method | Endpoint | Açıklama |
|--------|----------|----------|
| GET | /api/v1/exercises | Egzersiz listesi |
| GET | /api/v1/exercises/{id} | Egzersiz detayı |
| POST | /api/v1/exercises | Egzersiz ekle |
| PUT | /api/v1/exercises/{id} | Güncelle (Admin) |
| POST | /api/v1/exercises/{id}/approve | Onayla (Admin) |
| POST | /api/v1/exercises/{id}/image | Resim yükle (Admin) |
| DELETE | /api/v1/exercises/{id} | Sil (Admin) |

### Workout Logs
| Method | Endpoint | Açıklama |
|--------|----------|----------|
| GET | /api/v1/workoutlogs | Antrenman kayıtları |
| GET | /api/v1/workoutlogs/{id} | Kayıt detayı |
| POST | /api/v1/workoutlogs | Antrenman kaydet |
| DELETE | /api/v1/workoutlogs/{id} | Kayıt sil |

### Admin
| Method | Endpoint | Açıklama |
|--------|----------|----------|
| GET | /api/v1/admin/dashboard/stats | İstatistikler |
| GET | /api/v1/admin/dashboard/recent-users | Son kullanıcılar |
| GET | /api/v1/admin/users | Kullanıcı listesi |
| POST | /api/v1/admin/users | Kullanıcı oluştur |
| PUT | /api/v1/admin/users/{id} | Kullanıcı güncelle |
| DELETE | /api/v1/admin/users/{id} | Kullanıcı sil |
| POST | /api/v1/admin/users/{id}/toggle-status | Aktif/Pasif yap |

---

## 🏗️ Mimari

```
Domain → Application → Infrastructure/Persistence → API
```

- **Domain:** Entity'ler, Enum'lar, Exception'lar
- **Application:** Interface'ler, DTO'lar, Validator'lar, Mapper'lar
- **Persistence:** EF Core, Repository'ler, UnitOfWork, Seed
- **Infrastructure:** JWT, Email, File servisleri
- **API:** Controller'lar, Middleware, Program.cs

---

## 🔒 JWT Kullanımı

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

---

## ⚠️ Email Ayarı (Gmail)
1. Gmail → Settings → Security → 2-Step Verification → App Passwords
2. "Mail" seç → şifreyi kopyala → appsettings'e yaz
