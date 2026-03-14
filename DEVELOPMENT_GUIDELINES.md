# KarnelTravels - Quy Tắc Phát Triển FE & BE

## Mục lục
1. [Quy tắc chung](#1-quy-tắc-chung)
2. [Frontend (FE) Guidelines](#2-frontend-fe-guidelines)
3. [Backend (BE) Guidelines](#3-backend-be-guidelines)
4. [Trao đổi dữ liệu FE-BE](#4-trao-đổi-dữ-liệu-fe-be)
5. [API Design Rules](#5-api-design-rules)

---

## 1. Quy tắc chung

### 1.1 Git Workflow
- **Branch naming**: `feature/TEN- chức-năng`, `fix/TEN- lỗi`, `hotfix/TEN- sửa-gấp`
- **Commit message**: Tiếng Việt không dấu, rõ ràng, ví dụ: `them chuc nang login`, `sua loi validate`
- **Code review**: Bắt buộc review trước khi merge vào main/develop

### 1.2 Cấu trúc Project
```
KarnelTravels/
├── KarnelTravels.API/          # Backend (.NET)
│   ├── Controllers/            # API Endpoints
│   ├── Services/               # Business Logic
│   ├── Models/                 # Entities & DTOs
│   ├── Data/                   # Database Context
│   └── ...
│
└── KarnelTravels.FE/           # Frontend (React + Vite)
    ├── src/
    │   ├── components/         # Reusable components
    │   │   ├── ui/            # Base UI (Button, Input...)
    │   │   └── common/        # Shared components (Navbar, Footer)
    │   ├── layouts/           # Page layouts
    │   ├── features/          # Feature-based components
    │   │   ├── admin/        # Admin pages
    │   │   ├── user/         # User pages
    │   │   └── auth/         # Auth pages
    │   ├── services/          # API calls
    │   ├── hooks/             # Custom hooks
    │   ├── utils/             # Helper functions
    │   ├── routes/            # Router config
    │   └── context/           # React Context
    └── ...
```

---

## 2. Frontend (FE) Guidelines

### 2.1 Công nghệ sử dụng
- **Framework**: React 19+ với Vite
- **Styling**: Tailwind CSS
- **Icons**: Lucide React
- **Routing**: React Router DOM v7
- **HTTP Client**: Axios hoặc Fetch API
- **State Management**: React Context + useReducer

### 2.2 Quy tắc đặt tên

| Loại | Quy tắc | Ví dụ |
|------|---------|-------|
| Component | PascalCase | `TourCard.jsx`, `Navbar.jsx` |
| File thường | camelCase | `apiService.js`, `useAuth.js` |
| CSS Classes | kebab-case | `tour-card`, `btn-primary` |
| Biến/Hàm | camelCase | `getTours()`, `isLoading` |
| Hằng số | UPPER_SNAKE | `API_BASE_URL`, `MAX_PAGE_SIZE` |
| Component Folder | PascalCase | `TourCard/`, `Navbar/` |

### 2.3 Cấu trúc Component

```jsx
// src/features/user/tours/TourCard.jsx

// 1. Import dependencies
import { Link } from 'react-router-dom';
import { MapPin, Calendar } from 'lucide-react';

// 2. Define component với props destructuring
const TourCard = ({ tour, onBook }) => {
  // 3. Local state (nếu cần)
  const [isHovered, setIsHovered] = useState(false);

  // 4. Effects (nếu cần)
  useEffect(() => {
    // side effects
  }, [dependencies]);

  // 5. Helper functions (nếu cần)
  const formatPrice = (price) => {
    return new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND'
    }).format(price);
  };

  // 6. Render
  return (
    <div className="tour-card">
      {/* content */}
    </div>
  );
};

// 7. PropTypes (nếu dùng) hoặc TypeScript interfaces
TourCard.propTypes = {
  tour: PropTypes.shape({
    id: PropTypes.number.isRequired,
    name: PropTypes.string.isRequired,
    price: PropTypes.number
  }),
  onBook: PropTypes.func
};

export default TourCard;
```

### 2.4 Quy tắc import

```jsx
// Thứ tự import:
// 1. External libraries
import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import axios from 'axios';

// 2. Internal components
import Button from '@/components/ui/Button';
import TourCard from '@/features/user/tours/TourCard';

// 3. Hooks & Services
import { useAuth } from '@/hooks/useAuth';
import { tourService } from '@/services/tourService';

// 4. Utils
import { formatDate, formatCurrency } from '@/utils/formatters';

// 5. Styles
import './TourList.css';
```

### 2.5 Sử dụng Alias Path

Cấu hình trong `vite.config.js`:
```js
export default defineConfig({
  resolve: {
    alias: {
      '@': '/src',
    },
  },
});
```

Sử dụng:
```jsx
import Button from '@/components/ui/Button';
import TourList from '@/features/user/tours/TourList';
```

### 2.6 API Service Pattern

```javascript
// src/services/api.js
import axios from 'axios';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5000/api',
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor - thêm token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor - xử lý lỗi chung
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default api;
```

```javascript
// src/services/tourService.js
import api from './api';

export const tourService = {
  getAll: (params) => api.get('/tours', { params }),
  getById: (id) => api.get(`/tours/${id}`),
  create: (data) => api.post('/tours', data),
  update: (id, data) => api.put(`/tours/${id}`, data),
  delete: (id) => api.delete(`/tours/${id}`),
};
```

### 2.7 Custom Hook Pattern

```javascript
// src/hooks/useTours.js
import { useState, useEffect } from 'react';
import { tourService } from '@/services/tourService';

export const useTours = (params) => {
  const [tours, setTours] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchTours = async () => {
    try {
      setLoading(true);
      const response = await tourService.getAll(params);
      setTours(response.data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchTours();
  }, [params]);

  return { tours, loading, error, refetch: fetchTours };
};
```

---

## 3. Backend (BE) Guidelines

### 3.1 Công nghệ sử dụng
- **Framework**: ASP.NET Core 8+
- **ORM**: Entity Framework Core
- **Database**: SQL Server
- **Authentication**: JWT (JSON Web Tokens)
- **API Documentation**: Swagger/OpenAPI

### 3.2 Cấu trúc Controller

```csharp
// Controllers/ToursController.cs
[ApiController]
[Route("api/[controller]")]
[Authorize] // Áp dụng auth cho tất cả endpoints
public class ToursController : ControllerBase
{
    private readonly ITourService _tourService;
    
    public ToursController(ITourService tourService)
    {
        _tourService = tourService;
    }

    // GET: api/tours
    [HttpGet]
    [AllowAnonymous] // Override - cho phép truy cập công khai
    public async Task<ActionResult<IEnumerable<TourDto>>> GetAll(
        [FromQuery] TourQueryParams queryParams)
    {
        var tours = await _tourService.GetAllAsync(queryParams);
        return Ok(tours);
    }

    // GET: api/tours/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TourDto>> GetById(int id)
    {
        var tour = await _tourService.GetByIdAsync(id);
        if (tour == null) return NotFound();
        return Ok(tour);
    }

    // POST: api/tours
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TourDto>> Create([FromBody] CreateTourDto dto)
    {
        var tour = await _tourService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = tour.Id }, tour);
    }

    // PUT: api/tours/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTourDto dto)
    {
        if (id != dto.Id) return BadRequest();
        await _tourService.UpdateAsync(id, dto);
        return NoContent();
    }

    // DELETE: api/tours/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _tourService.DeleteAsync(id);
        return NoContent();
    }
}
```

### 3.3 Response Format Chuẩn

```csharp
// Models/ApiResponse.cs
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public List<string> Errors { get; set; }
    public PaginationInfo Pagination { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, string message = "Thành công")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> ErrorResponse(string message, List<string> errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}

// Models/PaginationInfo.cs
public class PaginationInfo
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}
```

### 3.4 Service Layer Pattern

```csharp
// Services/ITourService.cs
public interface ITourService
{
    Task<IEnumerable<TourDto>> GetAllAsync(TourQueryParams queryParams);
    Task<TourDto> GetByIdAsync(int id);
    Task<TourDto> CreateAsync(CreateTourDto dto);
    Task UpdateAsync(int id, UpdateTourDto dto);
    Task DeleteAsync(int id);
}

// Services/TourService.cs
public class TourService : ITourService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public TourService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TourDto>> GetAllAsync(TourQueryParams queryParams)
    {
        var query = _context.Tours.AsQueryable();

        // Filtering
        if (!string.IsNullOrEmpty(queryParams.Search))
        {
            query = query.Where(t => t.Name.Contains(queryParams.Search));
        }

        if (queryParams.MinPrice.HasValue)
        {
            query = query.Where(t => t.Price >= queryParams.MinPrice);
        }

        // Pagination
        var totalItems = query.Count();
        var tours = await query
            .Skip((queryParams.Page - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync();

        return _mapper.Map<IEnumerable<TourDto>>(tours);
    }
}
```

---

## 4. Trao đổi dữ liệu FE-BE

### 4.1 HTTP Methods & CRUD Mapping

| Method | Endpoint | Mô tả | FE Usage |
|--------|----------|-------|----------|
| GET | `/api/tours` | Lấy danh sách | `tourService.getAll(params)` |
| GET | `/api/tours/5` | Lấy chi tiết | `tourService.getById(5)` |
| POST | `/api/tours` | Tạo mới | `tourService.create(data)` |
| PUT | `/api/tours/5` | Cập nhật | `tourService.update(5, data)` |
| DELETE | `/api/tours/5` | Xóa | `tourService.delete(5)` |

### 4.2 Query Parameters Convention

```
GET /api/tours?page=1&pageSize=10&search=viet&sortBy=price&sortOrder=asc&categoryId=2
```

```csharp
// Models/TourQueryParams.cs
public class TourQueryParams
{
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 50)]
    public int PageSize { get; set; } = 10;

    public string Search { get; set; }
    public string SortBy { get; set; } = "Id";
    public string SortOrder { get; set; } = "desc";
    public int? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}
```

### 4.3 Request/Response Examples

#### GET - Lấy danh sách Tours

**Request:**
```http
GET /api/tours?page=1&pageSize=5 HTTP/1.1
Host: localhost:5000
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

**Response:**
```json
{
  "success": true,
  "message": "Thành công",
  "data": [
    {
      "id": 1,
      "name": "Tour Hà Nội - Hạ Long",
      "description": "Khám phá vịnh Hạ Long tuyệt đẹp",
      "price": 2500000,
      "duration": 3,
      "imageUrl": "/uploads/tour-1.jpg",
      "categoryName": "Tour biển"
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 5,
    "totalItems": 25,
    "totalPages": 5,
    "hasNextPage": true,
    "hasPreviousPage": false
  }
}
```

#### POST - Đặt Tour

**Request:**
```http
POST /api/bookings HTTP/1.1
Host: localhost:5000
Content-Type: application/json
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...

{
  "tourId": 1,
  "customerName": "Nguyễn Văn A",
  "customerEmail": "nguyenvana@email.com",
  "customerPhone": "0912345678",
  "adults": 2,
  "children": 1,
  "specialRequests": "Cần xe đưa đón"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Đặt tour thành công",
  "data": {
    "bookingId": 15,
    "tourId": 1,
    "customerName": "Nguyễn Văn A",
    "totalPrice": 6250000,
    "status": "Pending",
    "createdAt": "2026-03-14T10:30:00Z"
  }
}
```

#### Error Response

```json
{
  "success": false,
  "message": "Đặt tour thất bại",
  "errors": [
    "Tour không tồn tại",
    "Số lượng khách phải lớn hơn 0"
  ]
}
```

### 4.4 Authentication Flow

```
1. Đăng nhập:
   FE: POST /api/auth/login {email, password}
   BE: Validate → Tạo JWT Token → Return {token, user}

2. Lưu token:
   FE: localStorage.setItem('token', response.token)

3. Gọi API có auth:
   FE: Thêm Header "Authorization: Bearer {token}"
   BE: Validate Token → Lấy UserId → Kiểm tra quyền

4. Token hết hạn (401):
   BE: Return 401
   FE Interceptor: localStorage.removeItem('token') → redirect /login
```

### 4.5 File Upload

```javascript
// FE - Upload ảnh
const handleUpload = async (file) => {
  const formData = new FormData();
  formData.append('file', file);
  
  const response = await api.post('/upload', formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  });
  
  return response.data.url;
};
```

```csharp
// BE - Upload controller
[HttpPost("upload")]
[Authorize]
public async Task<ActionResult<UploadResult>> Upload(IFormFile file)
{
    if (file.Length > 5 * 1024 * 1024) // 5MB
        return BadRequest("File quá lớn");
    
    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
    var filePath = Path.Combine(uploadsFolder, fileName);
    
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }
    
    return Ok(new { url = $"/uploads/{fileName}" });
}
```

---

## 5. API Design Rules

### 5.1 URL Convention
- **Resource**: Danh từ số nhiều, lowercase, hyphenated
- **Nested Resources**: `/api/tours/5/bookings`
- **Actions**: Dùng HTTP verbs, không dùng `/api/tours/getAll`
- **Versioning**: `/api/v1/tours`

### 5.2 HTTP Status Codes

| Code | Mô tả | Usage |
|------|-------|-------|
| 200 | OK | Thành công, trả về data |
| 201 | Created | Tạo mới thành công |
| 204 | No Content | Xóa thành công |
| 400 | Bad Request | Validate lỗi |
| 401 | Unauthorized | Chưa đăng nhập |
| 403 | Forbidden | Không có quyền |
| 404 | Not Found | Resource không tồn tại |
| 500 | Internal Server Error | Lỗi server |

### 5.3 Naming Conventions

| Loại | Quy tắc | Ví dụ |
|------|---------|-------|
| Endpoint | lowercase, hyphenated | `/api/tour-packages` |
| Property | camelCase | `tourName`, `createdAt` |
| Enum | PascalCase | `BookingStatus.Pending` |
| Query Param | camelCase | `pageSize`, `sortOrder` |

---

## 6. Validation Rules

### 6.1 FE Validation
- Validate trước khi gọi API
- Hiển thị error message rõ ràng
- Disable button khi đang loading

```jsx
const handleSubmit = async () => {
  // 1. Validate
  if (!formData.email.includes('@')) {
    setErrors({ email: 'Email không hợp lệ' });
    return;
  }

  // 2. Set loading
  setIsSubmitting(true);

  try {
    // 3. Call API
    await authService.login(formData);
    // 4. Handle success
    navigate('/dashboard');
  } catch (error) {
    // 5. Handle error
    setErrors({ form: error.message });
  } finally {
    setIsSubmitting(false);
  }
};
```

### 6.2 BE Validation
- Validate tất cả input với Data Annotations
- Return chi tiết lỗi validate

```csharp
public class CreateTourDto
{
    [Required(ErrorMessage = "Tên tour là bắt buộc")]
    [StringLength(200, MinimumLength = 5)]
    public string Name { get; set; }

    [Required]
    [Range(100000, 100000000)]
    public decimal Price { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Url]
    public string ImageUrl { get; set; }
}
```

---

## 7. Security Checklist

- [ ] HTTPS only
- [ ] JWT với expiration ngắn (15-30 phút)
- [ ] Refresh token mechanism
- [ ] Input sanitization
- [ ] SQL Injection prevention (EF Core parameterized queries)
- [ ] CORS configuration
- [ ] Rate limiting
- [ ] File upload validation (type, size)
- [ ] XSS prevention (React auto-escapes)
- [ ] CSRF protection

---

## 8. Development Workflow

```
1. BE: Tạo Database Models & Migrations
2. BE: Tạo DTOs & API Endpoints
3. BE: Test với Swagger
4. FE: Tạo API Service
5. FE: Tạo Components & Pages
6. FE: Tích hợp API
7. Test tích hợp (FE + BE)
8. Code Review
9. Deploy
```

---

**Document Version:** 1.0  
**Last Updated:** 2026-03-14  
**Maintainers:** KarnelTravels Dev Team
