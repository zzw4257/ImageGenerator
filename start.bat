@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion

:: Aetherflow 启动脚本 (Windows)
:: 用途：一键启动后端和前端服务

title Aetherflow Launcher

:: 颜色代码（通过 echo 实现）
set "INFO=[INFO]"
set "SUCCESS=[SUCCESS]"
set "WARNING=[WARNING]"
set "ERROR=[ERROR]"

echo.
echo ╔═══════════════════════════════════════════╗
echo ║                                           ║
echo ║        🌊 Aetherflow 启动脚本 🌊         ║
echo ║                                           ║
echo ║   AIGC 资源协作与交易平台 - MVP版本      ║
echo ║                                           ║
echo ╚═══════════════════════════════════════════╝
echo.

:: 检查依赖
echo %INFO% 检查系统依赖...

where dotnet >nul 2>nul
if %errorlevel% neq 0 (
    echo %ERROR% .NET SDK 未安装，请先安装 .NET 8 SDK
    echo 下载地址: https://dotnet.microsoft.com/download/dotnet/8.0
    pause
    exit /b 1
)

where pnpm >nul 2>nul
if %errorlevel% neq 0 (
    echo %ERROR% pnpm 未安装，请运行: npm install -g pnpm
    pause
    exit /b 1
)

echo %SUCCESS% 依赖检查通过
echo.

:: 初始化后端
echo %INFO% 初始化后端...

if not exist "ImageGenerator\ImageGenerator.csproj" (
    echo %ERROR% 未找到 ImageGenerator.csproj，请确认当前目录正确
    pause
    exit /b 1
)

cd ImageGenerator

echo %INFO% 恢复 .NET 依赖...
dotnet restore >nul 2>nul

if not exist "User.db" (
    echo %WARNING% 数据库不存在，正在创建...
    dotnet ef database update
    if %errorlevel% neq 0 (
        echo %ERROR% 数据库创建失败
        cd ..
        pause
        exit /b 1
    )
)

cd ..
echo %SUCCESS% 后端初始化完成
echo.

:: 初始化前端
echo %INFO% 初始化前端...

if not exist "WebUI\package.json" (
    echo %ERROR% 未找到 package.json，请确认当前目录正确
    pause
    exit /b 1
)

cd WebUI

if not exist "node_modules" (
    echo %WARNING% node_modules 不存在，正在安装依赖...
    call pnpm install
    if %errorlevel% neq 0 (
        echo %ERROR% 前端依赖安装失败
        cd ..
        pause
        exit /b 1
    )
)

cd ..
echo %SUCCESS% 前端初始化完成
echo.

:: 创建日志目录
if not exist "logs" mkdir logs

:: 启动服务
echo %INFO% 启动服务...
echo.

:: 启动后端
echo %INFO% 启动后端服务 (端口: 5000)...
start "Aetherflow Backend" /min cmd /c "cd ImageGenerator && dotnet run > ..\logs\backend.log 2>&1"

:: 等待后端启动
echo %INFO% 等待后端启动...
timeout /t 8 /nobreak >nul

:: 启动前端
echo %INFO% 启动前端服务 (端口: 5173)...
start "Aetherflow Frontend" /min cmd /c "cd WebUI && pnpm dev > ..\logs\frontend.log 2>&1"

:: 等待前端启动
timeout /t 3 /nobreak >nul

:: 显示成功信息
echo.
echo ╔═══════════════════════════════════════════╗
echo ║                                           ║
echo ║        ✅ Aetherflow 启动成功！          ║
echo ║                                           ║
echo ╚═══════════════════════════════════════════╝
echo.
echo %SUCCESS% 后端服务: http://localhost:5000
echo %SUCCESS% 前端服务: http://localhost:5173
echo %SUCCESS% API文档: http://localhost:5000/scalar/v1
echo.
echo %INFO% 日志位置:
echo   - 后端: logs\backend.log
echo   - 前端: logs\frontend.log
echo.
echo %WARNING% 服务已在后台启动，关闭此窗口不会停止服务
echo %WARNING% 要停止服务，请关闭 "Aetherflow Backend" 和 "Aetherflow Frontend" 窗口
echo.
echo %INFO% 3秒后将自动打开浏览器...
timeout /t 3 /nobreak >nul

:: 打开浏览器
start http://localhost:5173

echo.
echo %SUCCESS% 浏览器已打开，按任意键退出此窗口...
pause >nul

endlocal
