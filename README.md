# TaskManagerProject

Sistema de gestión de tareas con autenticación, roles y notificaciones en tiempo real.

## Descripción

TaskManagerProject es una API RESTful desarrollada en ASP.NET Core (.NET 8) que permite la gestión de tareas, usuarios y roles (profesor y estudiante). Incluye autenticación JWT, recuperación de contraseñas, confirmación de cuentas por correo electrónico y notificaciones en tiempo real mediante SignalR.

## Características principales

- Registro y autenticación de usuarios (profesor y estudiante)
- Confirmación de cuenta por correo electrónico
- Recuperación y reseteo de contraseña
- Gestión de tareas (CRUD)
- Notificaciones en tiempo real con SignalR
- Roles y autorización basada en JWT
- Documentación automática con Swagger

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (o Azure SQL)
- Visual Studio 2022 o superior

## Instalación

1. **Clona el repositorio:**
2. **Configura las cadenas de conexión en `appsettings.json`:**
3. **Configura los parámetros JWT y correo en `appsettings.json`:**
4. **Restaura los paquetes y ejecuta las migraciones:**
5. **Ejecuta la aplicación:**
   
## Uso

- Accede a la documentación Swagger en:  
  `https://localhost:5001/swagger`  
  Aquí puedes probar todos los endpoints de la API.

### Endpoints principales

#### Autenticación y cuentas

- `POST /api/Account/login`  
  Iniciar sesión y obtener JWT.

- `POST /api/Account/Register Professor`  
  Registrar un profesor.

- `POST /api/Account/Register Students`  
  Registrar un estudiante.

- `POST /api/Account/confirm-account?userId={id}&token={token}`  
  Confirmar cuenta con código enviado por correo.

- `POST /api/Account/forgot-password`  
  Solicitar recuperación de contraseña.

- `POST /api/Account/reset-password`  
  Restablecer contraseña con token.

- `POST /api/Account/logout`  
  Cerrar sesión.

#### Tareas

- `GET /api/Task`  
  Obtener todas las tareas.

- `GET /api/Task/{id}`  
  Obtener tarea por ID.

- `POST /api/Task`  
  Crear nueva tarea.

- `PUT /api/Task/{id}`  
  Actualizar tarea existente.

- `DELETE /api/Task/{id}`  
  Eliminar tarea.

#### Notificaciones

- Las notificaciones en tiempo real se envían mediante SignalR al crear o actualizar tareas.

## Seguridad y autenticación

- Todos los endpoints protegidos requieren un JWT válido en el header `Authorization: Bearer {token}`.
- Los roles (`Professor`, `Student`) determinan el acceso a ciertos endpoints (por ejemplo, solo profesores pueden eliminar usuarios).

## Pruebas

- Las pruebas unitarias están en el proyecto `TasksManagerITLAProject.Tests`.
- Se utilizan Moq y xUnit para simular servicios y validar la lógica de negocio.

## Notas adicionales

- El sistema envía correos de confirmación y recuperación de contraseña usando la configuración SMTP definida en `appsettings.json`.
- El proyecto incluye migraciones automáticas y seeders para roles y usuarios por defecto.

## Licencia

MIT

---

**¿Dudas o sugerencias?**  
Abre un issue o contacta al equipo de desarrollo.

   
   

   
