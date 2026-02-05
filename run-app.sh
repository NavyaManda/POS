#!/bin/bash

# POS Application Local Setup and Run Script
# This script will help you run the POS application locally

echo "╔════════════════════════════════════════════════════════════════╗"
echo "║         POS System - Local Application Runner                 ║"
echo "╚════════════════════════════════════════════════════════════════╝"
echo ""

# Check Node.js
echo "📦 Checking Node.js..."
if ! command -v node &> /dev/null; then
    echo "❌ Node.js is not installed!"
    echo ""
    echo "📥 Please install Node.js from: https://nodejs.org/"
    echo "   - Download Node.js 18+ (LTS recommended)"
    echo "   - Run the installer"
    echo "   - Restart your terminal"
    echo "   - Run this script again"
    exit 1
else
    echo "✅ Node.js found: $(node --version)"
fi

echo ""
echo "📦 Checking npm..."
if ! command -v npm &> /dev/null; then
    echo "❌ npm is not installed!"
    exit 1
else
    echo "✅ npm found: $(npm --version)"
fi

# Install dependencies
echo ""
echo "📥 Installing frontend dependencies..."
cd /Users/navyamanda/Desktop/POS/views
npm install

if [ $? -ne 0 ]; then
    echo "❌ Failed to install dependencies"
    exit 1
fi

echo "✅ Dependencies installed successfully"

# Start the application
echo ""
echo "🚀 Starting POS Application..."
echo ""
echo "📍 Frontend will be available at: http://localhost:4200"
echo "📍 Backend API available at: http://localhost:5001"
echo ""
echo "Demo Credentials:"
echo "  Email:    testuser@example.com"
echo "  Password: TestPassword123!"
echo ""
echo "Press Ctrl+C to stop the application"
echo ""

npm start
