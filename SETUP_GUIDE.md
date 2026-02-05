# POS Application - Local Setup Guide

## 📋 Prerequisites

### Required
- **Node.js 18+** (includes npm)
- **dotnet 10** (for .NET backend)
- **Your system**: macOS, Linux, or Windows

### Check Installation
```bash
node --version      # Should be v18.0.0+
npm --version       # Should be 8.0.0+
dotnet --version    # Should be 10.0+
```

## 🚀 Quick Start

### Option 1: Automated Script (Recommended)

1. **Make the script executable** (first time only):
```bash
chmod +x /Users/navyamanda/Desktop/POS/run-app.sh
```

2. **Run the application**:
```bash
/Users/navyamanda/Desktop/POS/run-app.sh
```

This will:
- ✅ Check for Node.js/npm
- ✅ Install frontend dependencies
- ✅ Start the Angular dev server on port 4200
- ✅ Confirm backend is running on port 5001

### Option 2: Manual Setup

#### Step 1: Start Backend (if not already running)
```bash
cd /Users/navyamanda/Desktop/POS/AuthService/src/AuthService.API
dotnet run
# Backend will start on http://localhost:5001
```

#### Step 2: Start Frontend in a new terminal
```bash
cd /Users/navyamanda/Desktop/POS/views
npm install    # First time only
npm start      # Start dev server on http://localhost:4200
```

## 🌐 Access the Application

Once both services are running:

- **Frontend**: http://localhost:4200
- **Backend API**: http://localhost:5001
- **Health Check**: http://localhost:5001/api/v1/auth/health

## 🔐 Demo Credentials

Use these to test the login:
- **Email**: `testuser@example.com`
- **Password**: `TestPassword123!`

## 🛠️ Troubleshooting

### Issue: `npm: command not found`
**Solution**: Install Node.js from https://nodejs.org/
- Download LTS version (18+)
- Run the installer
- Restart your terminal
- Run `node --version` to verify

### Issue: Port 4200 already in use
**Solution**: Stop the process using port 4200
```bash
lsof -i :4200        # List process using port 4200
kill -9 <PID>        # Kill the process
```

### Issue: Port 5001 already in use
**Solution**: Stop the backend service
```bash
lsof -i :5001
kill -9 <PID>
```

### Issue: `npm install` fails
**Solution**: Clear npm cache and retry
```bash
npm cache clean --force
cd /Users/navyamanda/Desktop/POS/views
npm install
```

## 📁 Project Structure

```
POS/
├── AuthService/           ← .NET Backend (port 5001)
│   └── src/AuthService.API/
│       └── Program.cs
├── views/                 ← Angular Frontend (port 4200)
│   ├── src/
│   ├── package.json
│   └── angular.json
├── infrastructure/        ← Docker & DB setup
├── run-app.sh            ← Quick start script
└── docker-compose-app.yml ← Docker Compose config
```

## 🔍 Verify Everything is Working

### Check Backend Health
```bash
curl http://localhost:5001/api/v1/auth/health
# Should return: {"status":"healthy","timestamp":"...","service":"AuthService"}
```

### Check Frontend
Open http://localhost:4200 in your browser
- You should see the login page
- Demo credentials should work

### Check API Communication
The frontend should successfully connect to the backend
- Login should work
- JWT tokens should be generated
- Dashboard should display user info

## 📚 Available Commands

### Frontend (Views)
```bash
cd /Users/navyamanda/Desktop/POS/views

npm start              # Start dev server (port 4200)
npm run build          # Build for production
npm test               # Run tests
npm run lint           # Check code quality
```

### Backend (AuthService)
```bash
cd /Users/navyamanda/Desktop/POS/AuthService/src/AuthService.API

dotnet run             # Start dev server (port 5001)
dotnet build           # Build the project
dotnet test            # Run tests
```

## 🐳 Docker Setup (Alternative)

If you have Docker installed:
```bash
cd /Users/navyamanda/Desktop/POS
docker-compose -f docker-compose-app.yml up
```

This will:
- Pull Node.js image
- Pull .NET image
- Build and start both services
- Available on the same ports

## 🆘 Need Help?

1. Check the logs in your terminal
2. Verify both services are running on correct ports
3. Clear browser cache (Ctrl+Shift+Del)
4. Restart both services
5. Check the README files in each service folder

## ✅ Success Checklist

- [ ] Node.js 18+ installed
- [ ] npm working (`npm --version`)
- [ ] Backend running on http://localhost:5001
- [ ] Frontend running on http://localhost:4200
- [ ] Can login with demo credentials
- [ ] Dashboard loads after login
- [ ] No errors in browser console (F12)

---

**Enjoy developing the POS System! 🎉**
