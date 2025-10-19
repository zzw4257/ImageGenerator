#!/bin/bash

# Aetherflow 启动脚本
# 用途：一键启动后端和前端服务

set -e

# 颜色定义
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# 打印带颜色的消息
print_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# 检查依赖
check_dependencies() {
    print_info "检查系统依赖..."

    if ! command -v dotnet &> /dev/null; then
        print_error ".NET SDK 未安装，请先安装 .NET 8 SDK"
        exit 1
    fi

    if ! command -v pnpm &> /dev/null; then
        print_error "pnpm 未安装，请运行: npm install -g pnpm"
        exit 1
    fi

    print_success "依赖检查通过"
}

# 初始化后端
init_backend() {
    print_info "初始化后端..."

    cd ImageGenerator

    if [ ! -f "ImageGenerator.csproj" ]; then
        print_error "未找到 ImageGenerator.csproj，请确认当前目录正确"
        cd ..
        exit 1
    fi

    # 恢复依赖
    print_info "恢复 .NET 依赖..."
    dotnet restore > /dev/null 2>&1

    # 检查数据库
    if [ ! -f "User.db" ]; then
        print_warning "数据库不存在，正在创建..."
        dotnet ef database update
    fi

    cd ..
    print_success "后端初始化完成"
}

# 初始化前端
init_frontend() {
    print_info "初始化前端..."

    cd WebUI

    if [ ! -f "package.json" ]; then
        print_error "未找到 package.json，请确认当前目录正确"
        cd ..
        exit 1
    fi

    # 检查依赖
    if [ ! -d "node_modules" ]; then
        print_warning "node_modules 不存在，正在安装依赖..."
        pnpm install
    fi

    cd ..
    print_success "前端初始化完成"
}

# 清理进程
cleanup() {
    print_warning "正在停止服务..."

    if [ ! -z "$BACKEND_PID" ]; then
        kill $BACKEND_PID 2>/dev/null || true
    fi

    if [ ! -z "$FRONTEND_PID" ]; then
        kill $FRONTEND_PID 2>/dev/null || true
    fi

    # 等待进程结束
    sleep 1

    print_success "所有服务已停止"
    exit 0
}

# 注册信号处理
trap cleanup INT TERM

# 主函数
main() {
    echo ""
    echo "╔═══════════════════════════════════════════╗"
    echo "║                                           ║"
    echo "║        🌊 Aetherflow 启动脚本 🌊         ║"
    echo "║                                           ║"
    echo "║   AIGC 资源协作与交易平台 - MVP版本      ║"
    echo "║                                           ║"
    echo "╚═══════════════════════════════════════════╝"
    echo ""

    # 检查依赖
    check_dependencies

    # 初始化
    init_backend
    init_frontend

    echo ""
    print_info "启动服务..."
    echo ""

    # 启动后端
    cd ImageGenerator
    print_info "启动后端服务 (端口: 5000)..."
    dotnet run > ../logs/backend.log 2>&1 &
    BACKEND_PID=$!
    cd ..

    # 等待后端启动
    print_info "等待后端启动..."
    sleep 8

    # 检查后端是否成功启动
    if ! ps -p $BACKEND_PID > /dev/null; then
        print_error "后端启动失败，请查看 logs/backend.log"
        exit 1
    fi

    # 启动前端
    cd WebUI
    print_info "启动前端服务 (端口: 5173)..."
    pnpm dev > ../logs/frontend.log 2>&1 &
    FRONTEND_PID=$!
    cd ..

    # 等待前端启动
    sleep 3

    # 检查前端是否成功启动
    if ! ps -p $FRONTEND_PID > /dev/null; then
        print_error "前端启动失败，请查看 logs/frontend.log"
        kill $BACKEND_PID 2>/dev/null || true
        exit 1
    fi

    echo ""
    echo "╔═══════════════════════════════════════════╗"
    echo "║                                           ║"
    echo "║        ✅ Aetherflow 启动成功！          ║"
    echo "║                                           ║"
    echo "╚═══════════════════════════════════════════╝"
    echo ""
    print_success "后端服务: http://localhost:5000"
    print_success "前端服务: http://localhost:5173"
    print_success "API文档: http://localhost:5000/scalar/v1"
    echo ""
    print_info "日志位置:"
    print_info "  - 后端: logs/backend.log"
    print_info "  - 前端: logs/frontend.log"
    echo ""
    print_warning "按 Ctrl+C 停止所有服务"
    echo ""

    # 创建日志目录
    mkdir -p logs

    # 持续运行，等待用户中断
    while true; do
        if ! ps -p $BACKEND_PID > /dev/null; then
            print_error "后端进程意外终止"
            cleanup
        fi

        if ! ps -p $FRONTEND_PID > /dev/null; then
            print_error "前端进程意外终止"
            cleanup
        fi

        sleep 5
    done
}

# 运行主函数
main
