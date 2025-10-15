# Image Generator AI

[![Star History Chart](https://api.star-history.com/svg?repos=zzw4257/zmage&type=Date)](https://star-history.com/#zzw4257/zmage&Date)

这是一个使用 .NET 和 Vue.js 构建的全栈AI图像生成应用程序。它允许用户通过AI模型创建、查看和管理图像。

## 项目概述

**Image Generator** 应用程序允许用户通过邀请码注册、登录，然后在会话中生成图像。该应用程序支持不同的AI模型进行图像生成，并允许用户收藏和管理他们生成的图像。

### 后端 (`ImageGenerator`)

后端使用 C# 和 ASP.NET Core 构建。它为用户认证、会话管理、图像生成等提供RESTful API。它的设计是可扩展的，允许添加新的图像生成客户端。

### 前端 (`WebUI`)

前端是使用 Vue.js 和 TypeScript 构建的单页应用程序。它提供了一个用户友好的界面，用于与后端API交互，允许用户生成图像、查看他们的会话历史记录和管理他们喜欢的图像。

## 技术栈

[![bcryptjs](https://img.shields.io/badge/bcryptjs-3.0.2-blue?style=flat-square)](https://github.com/dcodeIO/bcrypt.js)

## 项目结构

### 后端

- `Controllers/`: 包含处理传入HTTP请求的API控制器。
- `Services/`: 包含应用程序的业务逻辑。
- `Database/`: 包含EntityFramework Core DbContext和迁移。
- `Models/`: 包含应用程序的数据模型。
- `Dtos/`: 包含用于API通信的数据传输对象。
- `Interfaces/`: 包含服务的接口。
- `Helpers/`: 包含帮助类和实用程序。

### 前端

- `src/`: 主要源代码目录。
  - `components/`: 可重用的Vue组件。
  - `pages/`: 应用程序的主要页面。
  - `services/`: 用于向后端发出API请求的模块。
  - `stores/`: 用于状态管理的Pinia存储。
  - `composables/`: 可重用的Vue组合式函数。
  - `router/`: Vue路由器配置。
  - `layouts/`: 应用程序的布局组件。
  - `assets/`: 静态资产，如图像和样式。

## 快速上手 (macOS)

本指南将引导您在macOS上使用Visual Studio Code配置和运行.NET项目。

### 前提条件

- [Visual Studio Code](https://code.visualstudio.com/) 已安装。
- 项目代码已下载到您的本地计算机。

### 1. 安装VS Code插件

首先，在VS Code中安装以下插件，它们将极大地改善您的.NET开发体验：

- **C# Dev Kit**: 官方C#扩展，提供了丰富的C#语言支持。
- **.NET Core Extension Pack**: 包含一组有用的.NET相关扩展。
- **vscode-solution-explorer**: 在VS Code中添加一个解决方案资源管理器，方便管理.NET解决方案。

您可以在VS Code的扩展市场中搜索并安装它们：

![VS Code Extensions](https://i.imgur.com/8V5mZ4u.png)

### 2. 下载并安装.NET SDK

根据您的Mac处理器类型（Intel或Apple Silicon），下载并安装合适的.NET SDK。

- **查看处理器类型**:
  1. 点击屏幕左上角的苹果图标。
  2. 选择“关于本机”。
  3. 查看“处理器”信息。如果是Intel，选择x64版本；如果是Apple M系列芯片，选择Arm64版本。

- **下载.NET 6.0 SDK**:
  [点击此处进入下载页面](https://dotnet.microsoft.com/download/dotnet/6.0)

  ![Download .NET SDK](https://i.imgur.com/Trf8v2L.png)

### 3. 在VS Code中打开并运行项目

1. **重启VS Code** 并使用VS Code打开项目根目录。

2. **自动安装依赖**:
   由于您已经安装了相关插件，项目依赖项将会自动安装。

3. **使用Solution Explorer**:
   - 将`vscode-solution-explorer`插件拖到VS Code侧边栏的第一个选项卡下方的空白区域。
   - 打开`Solution Explorer`，您将看到项目结构。

4. **构建项目**:
   - 在`Solution Explorer`中，右键点击解决方案文件 (`.sln`)，然后选择“Build”。

   ![Build Solution](https://i.imgur.com/4z3Y4Y1.png)

5. **运行项目**:
   - 在`Solution Explorer`中，找到可以运行的服务（通常是`ImageGenerator`项目），右键点击它，然后选择“Run”。

   ![Run Project](https://i.imgur.com/7nJ2c8Y.png)

   如果运行失败，请尝试关闭VS Code，然后重新启动并再次执行“Run”操作。

### 运行成功

项目成功运行后，后端API将在`http://localhost:5000`可用，前端将在`http://localhost:5173`可用。
