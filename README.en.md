# Image Generator

This project is a full-stack application for generating images using various AI models. It consists of a C# backend and a Vue.js frontend.

## Project Overview

The **Image Generator** application allows users to create, view, and manage AI-generated images. Users can register with an invitation code, log in, and then generate images within conversations. The application supports different AI models for image generation and allows users to favorite and manage their generated images.

### Backend (`ImageGenerator`)

The backend is built with C# and ASP.NET Core. It provides a RESTful API for user authentication, conversation management, image generation, and more. It's designed to be extensible, allowing for the addition of new image generation clients.

### Frontend (`WebUI`)

The frontend is a single-page application built with Vue.js and TypeScript. It provides a user-friendly interface for interacting with the backend API, allowing users to generate images, view their conversation history, and manage their favorite images.

## Getting Started

### Prerequisites

- .NET 8 SDK
- Node.js (v18 or later)
- pnpm

### Backend Setup

1. **Navigate to the `ImageGenerator` directory:**
   ```bash
   cd ImageGenerator
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Update the database:**
   ```bash
   dotnet ef database update
   ```

4. **Run the backend server:**
   ```bash
   dotnet run
   ```

The backend API will be available at `http://localhost:5000`.

### Frontend Setup

1. **Navigate to the `WebUI` directory:**
   ```bash
   cd WebUI
   ```

2. **Install dependencies:**
   ```bash
   pnpm install
   ```

3. **Run the frontend development server:**
   ```bash
   pnpm dev
   ```

The frontend will be available at `http://localhost:5173`.

## Project Structure

### Backend

- `Controllers/`: Contains the API controllers that handle incoming HTTP requests.
- `Services/`: Contains the business logic for the application.
- `Database/`: Contains the Entity Framework Core DbContext and migrations.
- `Models/`: Contains the data models for the application.
- `Dtos/`: Contains the data transfer objects used for API communication.
- `Interfaces/`: Contains the interfaces for the services.
- `Helpers/`: Contains helper classes and utilities.

### Frontend

- `src/`: The main source code directory.
  - `components/`: Reusable Vue components.
  - `pages/`: The main pages of the application.
  - `services/`: Modules for making API requests to the backend.
  - `stores/`: Pinia stores for state management.
  - `composables/`: Reusable Vue composables.
  - `router/`: Vue Router configuration.
  - `layouts/`: Layout components for the application.
  - `assets/`: Static assets like images and styles.

## Usage

1. **Register:**
   - You will need an invitation code to register. The application seeds one initial invitation code upon database creation. You can find it in the `Invitations` table.
2. **Login:**
   - Log in with your newly created credentials.
3. **Generate Images:**
   - Create a new conversation and start generating images by providing a prompt.
4. **View and Manage Images:**
   - View your generated images in the conversation history.
   - Add your favorite images to the favorites list for easy access.

## Build and Deployment

### Backend

To build the backend for production, run the following command in the `ImageGenerator` directory:

```bash
dotnet publish -c Release -o ./publish
```

This will create a `publish` directory with the compiled application, which can then be deployed to a server.

### Frontend

To build the frontend for production, run the following command in the `WebUI` directory:

```bash
pnpm build
```

This will create a `dist` directory with the compiled and minified assets, which can then be served by a web server.
