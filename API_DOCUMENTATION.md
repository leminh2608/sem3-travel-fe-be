# KARNEL TRAVELS API DOCUMENTATION

**Phiên bản:** 1.0

**Ngày lập:** 13/03/2026

**Mục đích:** Tài liệu này cung cấp API specification chung cho Backend (ASP.NET Core) và Frontend (ReactJS) để phát triển đồng bộ.

---

## 1. TỔNG QUAN API

### 1.1. Base URL

| Môi trường | URL |
|------------|-----|
| Development | `http://localhost:5000/api` |
| Production | `https://api.karneltravelguide.com/api` |

### 1.2. Authentication Type

- **JWT (JSON Web Token)** - Bearer Token
- Token được gửi trong header: `Authorization: Bearer <token>`

### 1.3. Content Type

- Request: `application/json`
- Response: `application/json`

### 1.4. Pagination

Tất cả các API list đều hỗ trợ pagination:

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| pageIndex | int | 1 | Số trang hiện tại |
| pageSize | int | 10 | Số lượng item trên một trang |

### 1.5. Common Response Format

#### Success Response

```json
{
  "success": true,
  "message": "Operation successful",
  "data": { },
  "pagination": {
    "pageIndex": 1,
    "pageSize": 10,
    "totalCount": 100,
    "totalPages": 10
  }
}
```

#### Error Response

```json
{
  "success": false,
  "message": "Error message description",
  "errors": [
    {
      "field": "email",
      "message": "Email is required"
    }
  ]
}
```

---

## 2. AUTHENTICATION APIs

### 2.1. Đăng nhập User

**Endpoint:** `POST /auth/login`

**Request:**

```json
{
  "email": "user@example.com",
  "password": "Password123!",
  "rememberMe": true
}
```

**Response (200):**

```json
{
  "success": true,
  "data": {
    "userId": "uuid-string",
    "email": "user@example.com",
    "fullName": "Nguyen Van A",
    "role": "User",
    "avatar": "https://...",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "uuid-string",
    "expiresIn": 3600
  }
}
```

### 2.2. Đăng ký User

**Endpoint:** `POST /auth/register`

**Request:**

```json
{
  "fullName": "Nguyen Van A",
  "email": "user@example.com",
  "password": "Password123!",
  "confirmPassword": "Password123!",
  "phoneNumber": "0912345678"
}
```

**Response (201):**

```json
{
  "success": true,
  "message": "Registration successful. Please check your email to verify account.",
  "data": {
    "userId": "uuid-string"
  }
}
```

### 2.3. Quên mật khẩu

**Endpoint:** `POST /auth/forgot-password`

**Request:**

```json
{
  "email": "user@example.com"
}
```

**Response (200):**

```json
{
  "success": true,
  "message": "Password reset link has been sent to your email."
}
```

### 2.4. Đặt lại mật khẩu

**Endpoint:** `POST /auth/reset-password`

**Request:**

```json
{
  "token": "reset-token-from-email",
  "newPassword": "NewPassword123!",
  "confirmNewPassword": "NewPassword123!"
}
```

**Response (200):**

```json
{
  "success": true,
  "message": "Password has been reset successfully."
}
```

### 2.5. Xác thực email

**Endpoint:** `POST /auth/verify-email`

**Request:**

```json
{
  "userId": "uuid-string",
  "token": "verification-token"
}
```

**Response (200):**

```json
{
  "success": true,
  "message": "Email verified successfully."
}
```

### 2.6. Refresh Token

**Endpoint:** `POST /auth/refresh-token`

**Request:**

```json
{
  "refreshToken": "uuid-string"
}
```

**Response (200):**

```json
{
  "success": true,
  "data": {
    "token": "new-jwt-token",
    "refreshToken": "new-refresh-token",
    "expiresIn": 3600
  }
}
```

### 2.7. Đăng nhập Admin

**Endpoint:** `POST /auth/admin/login`

**Request:**

```json
{
  "email": "admin@karneltravels.com",
  "password": "AdminPassword123!"
}
```

**Response (200):**

```json
{
  "success": true,
  "data": {
    "userId": "uuid-string",
    "email": "admin@karneltravels.com",
    "fullName": "Admin User",
    "role": "Admin",
    "permissions": ["ManageUsers", "ManageBookings", "ManageContent"],
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresIn": 3600
  }
}
```

---

## 3. USER APIs

### 3.1. Lấy thông tin Profile

**Endpoint:** `GET /users/profile`

**Headers:** `Authorization: Bearer <token>`

**Response (200):**

```json
{
  "success": true,
  "data": {
    "userId": "uuid-string",
    "fullName": "Nguyen Van A",
    "email": "user@example.com",
    "phoneNumber": "0912345678",
    "avatar": "https://...",
    "dateOfBirth": "1990-01-01",
    "gender": "Male",
    "isEmailVerified": true,
    "createdAt": "2026-01-01T00:00:00Z"
  }
}
```

### 3.2. Cập nhật Profile

**Endpoint:** `PUT /users/profile`

**Headers:** `Authorization: Bearer <token>`

**Request:**

```json
{
  "fullName": "Nguyen Van A",
  "phoneNumber": "0912345678",
  "dateOfBirth": "1990-01-01",
  "gender": "Male"
}
```

**Response (200):**

```json
{
  "success": true,
  "message": "Profile updated successfully.",
  "data": { ... updated profile ... }
}
```

### 3.3. Upload Avatar

**Endpoint:** `POST /users/avatar`

**Headers:** `Authorization: Bearer <token>`

**Content-Type:** `multipart/form-data`

**Request Body:** `file` (image file)

**Response (200):**

```json
{
  "success": true,
  "data": {
    "avatarUrl": "https://cdn.karneltravels.com/avatars/uuid.jpg"
  }
}
```

### 3.4. Đổi mật khẩu

**Endpoint:** `PUT /users/change-password`

**Headers:** `Authorization: Bearer <token>`

**Request:**

```json
{
  "currentPassword": "OldPassword123!",
  "newPassword": "NewPassword123!",
  "confirmNewPassword": "NewPassword123!"
}
```

**Response (200):**

```json
{
  "success": true,
  "message": "Password changed successfully."
}
```

### 3.5. Quản lý địa chỉ

#### 5.5.1. Lấy danh sách địa chỉ

**Endpoint:** `GET /users/addresses`

**Headers:** `Authorization: Bearer <token>`

**Response (200):**

```json
{
  "success": true,
  "data": [
    {
      "addressId": "uuid-string",
      "addressLine": "123 Nguyen Trai Street",
      "ward": "Ward 1",
      "district": "District 1",
      "city": "Ho Chi Minh City",
      "country": "Vietnam",
      "isDefault": true
    }
  ]
}
```

#### 5.5.2. Thêm địa chỉ

**Endpoint:** `POST /users/addresses`

**Headers:** `Authorization: Bearer <token>`

**Request:**

```json
{
  "addressLine": "123 Nguyen Trai Street",
  "ward": "Ward 1",
  "district": "District 1",
  "city": "Ho Chi Minh City",
  "country": "Vietnam",
  "isDefault": true
}
```

#### 5.5.3. Cập nhật địa chỉ

**Endpoint:** `PUT /users/addresses/{addressId}`

#### 5.5.4. Xóa địa chỉ

**Endpoint:** `DELETE /users/addresses/{addressId}`

---

## 4. TOURIST SPOT APIs

### 4.1. Lấy danh sách điểm du lịch

**Endpoint:** `GET /tourist-spots`

**Query Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| search | string | Tìm kiếm theo tên |
| region | string | Vùng miền (North, Central, South) |
| type | string | Loại (Beach, Mountain, Historical, Waterfall) |
| sortBy | string | Sắp xếp (Name, Rating, Popularity) |
| sortOrder | string | ASC hoặc DESC |
| pageIndex | int | Số trang |
| pageSize | int | Số item/trang |

**Response (200):**

```json
{
  "success": true,
  "data": [
    {
      "spotId": "uuid-string",
      "name": "Da Nang Beach",
      "description": "Beautiful beach in Da Nang",
      "region": "Central",
      "type": "Beach",
      "images": ["https://..."],
      "rating": 4.5,
      "reviewCount": 120,
      "location": {
        "city": "Da Nang",
        "country": "Vietnam"
      }
    }
  ],
  "pagination": { ... }
}
```

### 4.2. Lấy chi tiết điểm du lịch

**Endpoint:** `GET /tourist-spots/{id}`

**Response (200):**

```json
{
  "success": true,
  "data": {
    "spotId": "uuid-string",
    "name": "Da Nang Beach",
    "description": "Detailed description...",
    "region": "Central",
    "type": "Beach",
    "images": ["https://..."],
    "rating": 4.5,
    "reviewCount": 120,
    "location": {
      "city": "Da Nang",
      "country": "Vietnam",
      "address": "Full address",
      "latitude": 16.0544,
      "longitude": 108.2022
    },
    "activities": ["Swimming", "Surfing", "Walking"],
    "bestTime": "April - October",
    "ticketPrice": 0,
    "nearbyHotels": [...],
    "nearbyRestaurants": [...]
  }
}
```

### 4.3. Tạo mới điểm du lịch (Admin)

**Endpoint:** `POST /tourist-spots`

**Headers:** `Authorization: Bearer <token>` (Admin only)

**Request:**

```json
{
  "name": "Da Nang Beach",
  "description": "Description",
  "region": "Central",
  "type": "Beach",
  "images": ["https://..."],
  "location": {
    "city": "Da Nang",
    "country": "Vietnam",
    "address": "Full address",
    "latitude": 16.0544,
    "longitude": 108.2022
  },
  "activities": ["Swimming", "Surfing"],
  "bestTime": "April - October",
  "ticketPrice": 0,
  "isActive": true
}
```

### 4.4. Cập nhật điểm du lịch (Admin)

**Endpoint:** `PUT /tourist-spots/{id}`

### 4.5. Xóa điểm du lịch (Admin)

**Endpoint:** `DELETE /tourist-spots/{id}`

---

## 5. HOTEL APIs

### 5.1. Lấy danh sách khách sạn

**Endpoint:** `GET /hotels`

**Query Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| search | string | Tìm kiếm theo tên |
| city | string | Lọc theo thành phố |
| starRating | int | Lọc theo số sao (1-5) |
| minPrice | decimal | Giá tối thiểu |
| maxPrice | decimal | Giá tối đại |
| amenities | string[] | Tiện nghi (Wifi, Pool, Restaurant) |
| sortBy | string | Sắp xếp (Price, Rating, Name) |
| pageIndex | int | Số trang |
| pageSize | int | Số item/trang |

**Response (200):**

```json
{
  "success": true,
  "data": [
    {
      "hotelId": "uuid-string",
      "name": "Grand Hotel",
      "description": "Luxury hotel in city center",
      "starRating": 5,
      "city": "Ho Chi Minh City",
      "address": "123 Le Loi Street",
      "images": ["https://..."],
      "priceRange": {
        "minPrice": 500000,
        "maxPrice": 5000000
      },
      "amenities": ["Wifi", "Pool", "Spa", "Restaurant"],
      "rating": 4.7,
      "reviewCount": 250,
      "availableRooms": 15
    }
  ],
  "pagination": { ... }
}
```

### 5.2. Lấy chi tiết khách sạn

**Endpoint:** `GET /hotels/{id}`

### 5.3. Lấy danh sách phòng của khách sạn

**Endpoint:** `GET /hotels/{id}/rooms`

**Query Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| checkInDate | date | Ngày nhận phòng |
| checkOutDate | date | Ngày trả phòng |
| guests | int | Số khách |

### 5.4. Tạo khách sạn (Admin)

**Endpoint:** `POST /hotels`

### 5.5. Cập nhật khách sạn (Admin)

**Endpoint:** `PUT /hotels/{id}`

### 5.6. Quản lý phòng khách sạn (Admin)

**Endpoint:** `GET/POST/PUT/DELETE /hotels/{hotelId}/rooms`

---

## 6. RESTAURANT APIs

### 6.1. Lấy danh sách nhà hàng

**Endpoint:** `GET /restaurants`

**Query Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| search | string | Tìm kiếm theo tên |
| city | string | Lọc theo thành phố |
| cuisineType | string | Loại ẩm thực (Vietnamese, Chinese, Japanese, Italian) |
| priceLevel | string | Mức giá (Budget, MidRange, HighEnd) |
| style | string | Phong cách (Restaurant, Cafe, Bar) |
| sortBy | string | Sắp xếp |
| pageIndex | int | Số trang |
| pageSize | int | Số item/trang |

### 6.2. Lấy chi tiết nhà hàng

**Endpoint:** `GET /restaurants/{id}`

### 6.3. Lấy menu của nhà hàng

**Endpoint:** `GET /restaurants/{id}/menu`

### 6.4. Đặt bàn nhà hàng

**Endpoint:** `POST /restaurants/{id}/reserve`

**Request:**

```json
{
  "guestName": "Nguyen Van A",
  "phoneNumber": "0912345678",
  "email": "user@example.com",
  "reservationDate": "2026-03-20",
  "reservationTime": "19:00",
  "numberOfGuests": 4,
  "specialRequests": "Window seat please"
}
```

---

## 7. RESORT APIs

### 7.1. Lấy danh sách resort

**Endpoint:** `GET /resorts`

**Query Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| search | string | Tìm kiếm theo tên |
| location | string | Vị trí (Beach, Mountain, Lake, Island) |
| starRating | int | Số sao |
| priceRange | string | Khoảng giá |
| amenities | string[] | Tiện nghi |
| sortBy | string | Sắp xếp |
| pageIndex | int | Số trang |
| pageSize | int | Số item/trang |

### 7.2. Lấy chi tiết resort

**Endpoint:** `GET /resorts/{id}`

### 7.3. Lấy danh sách phòng bungalow

**Endpoint:** `GET /resorts/{id}/rooms`

### 7.4. Lấy danh sách combo

**Endpoint:** `GET /resorts/{id}/packages`

---

## 8. TOUR PACKAGE APIs

### 8.1. Lấy danh sách tour

**Endpoint:** `GET /tours`

**Query Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| search | string | Tìm kiếm theo tên/điểm đến |
| destination | string | Điểm đến |
| minPrice | decimal | Giá tối thiểu |
| maxPrice | decimal | Giá tối đại |
| duration | int | Số ngày |
| departureDate | date | Ngày khởi hành |
| sortBy | string | Sắp xếp |
| pageIndex | int | Số trang |
| pageSize | int | Số item/trang |

**Response (200):**

```json
{
  "success": true,
  "data": [
    {
      "tourId": "uuid-string",
      "name": "Da Nang - Hoi An 3 Days 2 Nights",
      "description": "Explore Da Nang and Hoi An",
      "destination": "Da Nang",
      "duration": 3,
      "price": 2500000,
      "discountPrice": 2299000,
      "images": ["https://..."],
      "departureDates": ["2026-03-15", "2026-03-20", "2026-03-25"],
      "availableSlots": 20,
      "rating": 4.8,
      "reviewCount": 150,
      "includes": ["Hotel", "Meals", "Transport", "Guide"],
      "excludes": ["Personal expenses", "Travel insurance"]
    }
  ],
  "pagination": { ... }
}
```

### 8.2. Lấy chi tiết tour

**Endpoint:** `GET /tours/{id}`

**Response (200):**

```json
{
  "success": true,
  "data": {
    "tourId": "uuid-string",
    "name": "Da Nang - Hoi An 3 Days 2 Nights",
    "description": "Full description...",
    "destination": "Da Nang",
    "duration": 3,
    "price": 2500000,
    "discountPrice": 2299000,
    "images": ["https://..."],
    "gallery": ["https://..."],
    "departureDates": ["2026-03-15", "2026-03-20"],
    "availableSlots": 20,
    "rating": 4.8,
    "reviewCount": 150,
    "includes": ["Hotel 4*", "Meals", "Transport", "Guide"],
    "excludes": ["Personal expenses", "Travel insurance"],
    "itinerary": [
      {
        "day": 1,
        "title": "Arrival in Da Nang",
        "activities": ["Arrive at airport", "Check-in hotel", "Free time"]
      },
      {
        "day": 2,
        "title": "Explore Da Nang & Hoi An",
        "activities": ["Visit Marble Mountains", "Lunch", "Visit Hoi An Ancient Town"]
      }
    ],
    "reviews": [...]
  }
}
```

### 8.3. Tạo tour (Admin)

**Endpoint:** `POST /tours`

### 8.4. Cập nhật tour (Admin)

**Endpoint:** `PUT /tours/{id}`

### 8.5. Quản lý lịch trình tour (Admin)

**Endpoint:** `GET/POST/PUT/DELETE /tours/{tourId}/itinerary`

---

## 9. TRANSPORT APIs

### 9.1. Lấy danh sách phương tiện

**Endpoint:** `GET /transports`

**Query Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| type | string | Loại (Flight, Bus, Train, Car, Limousine) |
| fromCity | string | Điểm đi |
| toCity | string | Điểm đến |
| departureDate | date | Ngày khởi hành |
| minPrice | decimal | Giá tối thiểu |
| maxPrice | decimal | Giá tối đại |
| sortBy | string | Sắp xếp |
| pageIndex | int | Số trang |
| pageSize | int | Số item/trang |

**Response (200):**

```json
{
  "success": true,
  "data": [
    {
      "transportId": "uuid-string",
      "type": "Flight",
      "provider": "Vietnam Airlines",
      "fromCity": "Ho Chi Minh City",
      "toCity": "Da Nang",
      "departureTime": "06:00",
      "arrivalTime": "07:30",
      "duration": 90,
      "price": 1500000,
      "availableSeats": 50,
      "amenities": ["WiFi", "Meal", "Entertainment"]
    }
  ],
  "pagination": { ... }
}
```

### 9.2. Tìm kiếm phương tiện theo tuyến

**Endpoint:** `GET /transports/search`

### 9.3. Lấy chi tiết phương tiện

**Endpoint:** `GET /transports/{id}`

### 9.4. Quản lý phương tiện (Admin)

**Endpoint:** `GET/POST/PUT/DELETE /transports`

---

## 10. BOOKING APIs

### 10.1. Đặt tour

**Endpoint:** `POST /bookings/tour`

**Headers:** `Authorization: Bearer <token>`

**Request:**

```json
{
  "tourId": "uuid-string",
  "departureDate": "2026-03-20",
  "adults": 2,
  "children": 1,
  "contactInfo": {
    "fullName": "Nguyen Van A",
    "email": "user@example.com",
    "phoneNumber": "0912345678"
  },
  "notes": "Need vegetarian meals",
  "promoCode": "SUMMER2026"
}
```

**Response (201):**

```json
{
  "success": true,
  "data": {
    "bookingId": "uuid-string",
    "bookingCode": "KT-20260313-001",
    "status": "Pending",
    "totalAmount": 5000000,
    "discountAmount": 500000,
    "finalAmount": 4500000,
    "paymentStatus": "Pending",
    "paymentMethods": ["VNPay", "MoMo", "BankTransfer"],
    "expiredAt": "2026-03-14T00:00:00Z"
  }
}
```

### 10.2. Đặt phòng khách sạn

**Endpoint:** `POST /bookings/hotel`

**Request:**

```json
{
  "hotelId": "uuid-string",
  "roomId": "uuid-string",
  "checkInDate": "2026-03-20",
  "checkOutDate": "2023-03-23",
  "guests": 2,
  "rooms": 1,
  "contactInfo": {
    "fullName": "Nguyen Van A",
    "email": "user@example.com",
    "phoneNumber": "0912345678"
  },
  "specialRequests": "Late check-in",
  "promoCode": "SUMMER2026"
}
```

### 10.3. Đặt phương tiện

**Endpoint:** `POST /bookings/transport`

### 10.4. Lấy danh sách đơn đặt của user

**Endpoint:** `GET /bookings/my-orders`

**Headers:** `Authorization: Bearer <token>`

**Query Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| type | string | Loại (Tour, Hotel, Resort, Transport) |
| status | string | Trạng thái |
| pageIndex | int | Số trang |
| pageSize | int | Số item/trang |

**Response (200):**

```json
{
  "success": true,
  "data": [
    {
      "bookingId": "uuid-string",
      "bookingCode": "KT-20260313-001",
      "type": "Tour",
      "itemName": "Da Nang - Hoi An 3 Days",
      "status": "Confirmed",
      "totalAmount": 4500000,
      "bookingDate": "2026-03-13T10:00:00Z",
      "serviceDate": "2026-03-20"
    }
  ],
  "pagination": { ... }
}
```

### 10.5. Lấy chi tiết đơn đặt

**Endpoint:** `GET /bookings/{id}`

**Headers:** `Authorization: Bearer <token>`

### 10.6. Hủy đơn đặt

**Endpoint:** `POST /bookings/{id}/cancel`

**Headers:** `Authorization: Bearer <token>`

**Request:**

```json
{
  "reason": "Change of plans"
}
```

### 10.7. Yêu cầu đổi ngày

**Endpoint:** `POST /bookings/{id}/reschedule`

**Request:**

```json
{
  "newDate": "2026-03-25",
  "reason": "Work conflict"
}
```

### 10.8. Thanh toán đơn đặt

**Endpoint:** `POST /bookings/{id}/payment`

**Request:**

```json
{
  "paymentMethod": "VNPay",
  "paymentInfo": {
    "bankCode": "NCB",
    "language": "vn"
  }
}
```

### 10.9. Xác nhận thanh toán (Webhook từ payment gateway)

**Endpoint:** `POST /bookings/payment/callback`

### 10.10. Lấy danh sách đơn đặt (Admin)

**Endpoint:** `GET /admin/bookings`

**Headers:** `Authorization: Bearer <token>` (Admin only)

### 10.11. Cập nhật trạng thái đơn (Admin)

**Endpoint:** `PUT /admin/bookings/{id}/status`

**Request:**

```json
{
  "status": "Confirmed",
  "note": "Booking confirmed"
}
```

---

## 11. PROMOTION APIs

### 11.1. Lấy danh sách khuyến mãi

**Endpoint:** `GET /promotions`

**Response (200):**

```json
{
  "success": true,
  "data": [
    {
      "promoId": "uuid-string",
      "code": "SUMMER2026",
      "title": "Summer Sale 2026",
      "description": "Get 20% off on all tours",
      "discountType": "Percentage",
      "discountValue": 20,
      "minOrderAmount": 1000000,
      "maxDiscountAmount": 500000,
      "startDate": "2026-03-01",
      "endDate": "2026-05-31",
      "applicableTo": ["Tour", "Hotel"],
      "isActive": true,
      "showOnHome": true
    }
  ]
}
```

### 11.2. Kiểm tra mã khuyến mãi

**Endpoint:** `GET /promotions/validate`

**Query Parameters:** `?code=SUMMER2026&orderType=Tour&orderAmount=2000000`

### 11.3. Tạo khuyến mãi (Admin)

**Endpoint:** `POST /promotions`

### 11.4. Cập nhật khuyến mãi (Admin)

**Endpoint:** `PUT /promotions/{id}`

### 11.5. Xóa khuyến mãi (Admin)

**Endpoint:** `DELETE /promotions/{id}`

---

## 12. CONTACT APIs

### 12.1. Gửi liên hệ

**Endpoint:** `POST /contacts`

**Request:**

```json
{
  "fullName": "Nguyen Van A",
  "email": "user@example.com",
  "phoneNumber": "0912345678",
  "address": "123 Main Street",
  "serviceType": "TourBooking",
  "preferredDate": "2026-03-20",
  "numberOfPeople": 4,
  "message": "I want to book a tour to Da Nang"
}
```

### 12.2. Lấy danh sách liên hệ (Admin)

**Endpoint:** `GET /admin/contacts`

### 12.3. Xem chi tiết liên hệ (Admin)

**Endpoint:** `GET /admin/contacts/{id}`

### 12.4. Cập nhật trạng thái liên hệ (Admin)

**Endpoint:** `PUT /admin/contacts/{id}/status`

---

## 13. REVIEW APIs

### 13.1. Đánh giá tour

**Endpoint:** `POST /reviews/tour`

**Headers:** `Authorization: Bearer <token>`

**Request:**

```json
{
  "tourId": "uuid-string",
  "bookingId": "uuid-string",
  "rating": 5,
  "title": "Amazing experience!",
  "content": "The tour was well organized...",
  "images": ["https://..."]
}
```

### 13.2. Đánh giá khách sạn

**Endpoint:** `POST /reviews/hotel`

### 13.3. Lấy đánh giá theo item

**Endpoint:** `GET /reviews/tour/{tourId}`

### 13.4. Lấy đánh giá của user

**Endpoint:** `GET /reviews/my-reviews`

**Headers:** `Authorization: Bearer <token>`

---

## 14. FAVORITE APIs

### 14.1. Thêm vào yêu thích

**Endpoint:** `POST /favorites`

**Headers:** `Authorization: Bearer <token>`

**Request:**

```json
{
  "itemType": "Tour",
  "itemId": "uuid-string"
}
```

### 14.2. Lấy danh sách yêu thích

**Endpoint:** `GET /favorites`

**Headers:** `Authorization: Bearer <token>`

### 14.3. Xóa khỏi yêu thích

**Endpoint:** `DELETE /favorites/{id}`

---

## 15. ADMIN DASHBOARD APIs

### 15.1. Thống kê tổng quan

**Endpoint:** `GET /admin/dashboard/stats`

**Headers:** `Authorization: Bearer <token>` (Admin only)

**Response (200):**

```json
{
  "success": true,
  "data": {
    "totalTouristSpots": 150,
    "totalHotels": 80,
    "totalRestaurants": 60,
    "totalResorts": 25,
    "totalTours": 100,
    "totalTransports": 200,
    "todayBookings": 25,
    "todayRevenue": 50000000,
    "monthlyRevenue": 1500000000,
    "totalUsers": 5000,
    "newUsersThisMonth": 150,
    "pendingContacts": 10
  }
}
```

### 15.2. Doanh thu theo tháng

**Endpoint:** `GET /admin/dashboard/revenue`

**Query Parameters:** `?year=2026&month=3`

### 15.3. Đơn đặt gần đây

**Endpoint:** `GET /admin/dashboard/recent-bookings`

---

## 16. FILE UPLOAD APIs

### 16.1. Upload file đơn

**Endpoint:** `POST /upload`

**Content-Type:** `multipart/form-data`

**Request Body:** `file` (image file)

**Response (200):**

```json
{
  "success": true,
  "data": {
    "fileName": "uuid.jpg",
    "fileUrl": "https://cdn.karneltravels.com/images/uuid.jpg",
    "fileSize": 1024000,
    "contentType": "image/jpeg"
  }
}
```

### 16.2. Upload nhiều file

**Endpoint:** `POST /upload/multiple`

---

## 17. ERROR CODES

| Code | Description |
|------|-------------|
| 200 | Success |
| 201 | Created |
| 204 | No Content |
| 400 | Bad Request |
| 401 | Unauthorized |
| 403 | Forbidden |
| 404 | Not Found |
| 409 | Conflict |
| 422 | Validation Error |
| 429 | Too Many Requests |
| 500 | Internal Server Error |
| 503 | Service Unavailable |

---

## 18. HTTP STATUS CODES FOR AUTH

| Status | Description |
|--------|-------------|
| AUTH001 | Invalid email or password |
| AUTH002 | Account not verified |
| AUTH003 | Account locked |
| AUTH004 | Token expired |
| AUTH005 | Token invalid |
| AUTH006 | Refresh token expired |
| AUTH007 | Email already exists |
| AUTH008 | Password reset token invalid or expired |

---

**Người lập:** [Tên người lập]

**Ngày phê duyệt:** [Ngày phê duyệt]

**Chữ ký:** ____________________
