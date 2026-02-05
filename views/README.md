# POS Frontend - Angular Authentication UI

Modern, responsive Angular application for the POS System authentication service.

## Features

- ✅ User Login with email/password
- ✅ User Registration with validation
- ✅ Password visibility toggle
- ✅ Real-time form validation
- ✅ JWT token management
- ✅ Dashboard with user profile
- ✅ Responsive design (mobile-friendly)
- ✅ Beautiful UI with gradient background
- ✅ Error handling and notifications
- ✅ Demo credentials for testing

## Technology Stack

- **Framework**: Angular 17
- **Language**: TypeScript
- **Styling**: SCSS
- **UI Framework**: Bootstrap 5
- **Icons**: Font Awesome 6
- **HTTP Client**: Angular HttpClient
- **Forms**: Reactive Forms

## Getting Started

### Prerequisites

- Node.js 18+ 
- npm or yarn

### Installation

```bash
cd frontend
npm install
```

### Running the Application

```bash
npm start
```

Application will be available at `http://localhost:4200`

### Building for Production

```bash
npm run build
```

## Project Structure

```
src/
├── app/
│   ├── components/
│   │   ├── login/           # Login component
│   │   ├── register/        # Registration component
│   │   └── dashboard/       # Dashboard component
│   ├── services/
│   │   └── auth.service.ts  # Authentication service
│   ├── guards/
│   │   └── auth.guard.ts    # Route protection
│   ├── app.component.ts     # Root component
│   └── app.routes.ts        # Route configuration
├── index.html               # Entry HTML
├── main.ts                  # Bootstrap file
└── styles.scss              # Global styles
```

## Usage

### Login

1. Navigate to `http://localhost:4200`
2. Enter credentials:
   - Email: `testuser@example.com`
   - Password: `TestPassword123!`
3. Click "Sign In"

### Register

1. Click "Sign up here" link
2. Fill in the registration form:
   - First Name
   - Last Name
   - Email
   - Password (must have: uppercase, lowercase, number, special char)
   - Confirm Password
3. Click "Create Account"

## API Integration

The frontend connects to the AuthService running on `http://localhost:5001`

### Endpoints Used

- `POST /api/v1/auth/login` - User login
- `POST /api/v1/auth/register` - User registration
- `GET /api/v1/auth/health` - Service health check

## Authentication Flow

1. User submits credentials
2. Frontend sends to AuthService
3. Backend validates and returns JWT tokens
4. Tokens stored in localStorage
5. User redirected to dashboard
6. Subsequent requests include Authorization header

## Form Validation

### Login Form
- Email: Required, valid email format
- Password: Required, minimum 6 characters

### Registration Form
- First Name: Required, 2-100 characters
- Last Name: Required, 2-100 characters
- Email: Required, valid email format
- Password: 8+ chars, uppercase, lowercase, number, special character
- Confirm Password: Must match password field

## Security Features

- ✅ JWT token storage in localStorage
- ✅ Password field masking
- ✅ HTTPS ready (when deployed)
- ✅ Form validation before submission
- ✅ Route guards for protected pages
- ✅ Secure credential handling

## Styling

- Modern gradient background (purple theme)
- Responsive grid layout
- Bootstrap 5 components
- Custom SCSS variables
- Smooth transitions and animations
- Mobile-optimized

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## Future Enhancements

- [ ] Password reset functionality
- [ ] Two-factor authentication
- [ ] Social login (OAuth)
- [ ] Remember me option
- [ ] Email verification
- [ ] User profile settings
- [ ] Dark mode theme
- [ ] Multi-language support

## License

MIT
