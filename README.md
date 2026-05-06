# 🏋️‍♂️ Gymphony Web App (MVC)

Frontend web de la plataforma **Gymphony**. Este proyecto actúa como el cliente visual de la plataforma, consumiendo los servicios expuestos por la [API de Gymphony] https://github.com/alooonsogm/ApiGymphony para ofrecer una experiencia fluida a socios, entrenadores y administradores.

🌍 **[Visitar Gymphony en Vivo] https://gymphony.azurewebsites.net/**

## 🚀 Arquitectura y Consumo de API
Durante la migración del monolito original, el objetivo para este Frontend fue dejarlo lo más ligero posible, delegando todo el peso de los datos al Backend.

* **Servicios API:** Toda la lógica de Entity Framework fue reemplazada por clases `Services` que consumen la API desplegada en Azure mediante `HttpClient`.
* **Modelos Compartidos:** Utiliza **un paquete NuGet personalizado** creado por mí para sincronizar exactamente los mismos modelos de datos (Entidades y DTOs) que utiliza la API.
* **Manejo de Sesiones (JWT):** El login autentica contra la API y recibe un JWT. Este token se almacena de forma segura en las *Cookies de Autenticación* mediante Claims, y se inyecta dinámicamente como cabecera `Bearer` en cada petición HTTP hacia el backend.
* **Imágenes Seguras:** Las fotos de perfil consumen URLs dinámicas generadas con **Tokens SAS** por la API desde un Blob Storage privado de Azure. El logo y recursos estructurales cargan desde un Blob público.
* **Despliegue:** La aplicación está alojada y escalada utilizando **Azure App Service**.

## 🎨 UI / UX
* Diseño moderno utilizando el estilo **Glassmorphism** (efectos de cristal desenfocado) sobre fondos corporativos.
* Totalmente **Responsive**: Interfaces adaptadas meticulosamente a dispositivos móviles mediante CSS puro, reestructurando modales y tablas complejas sin depender de JavaScript adicional.
* Notificaciones asíncronas fluidas integrando **SweetAlert2** y gráficos de rendimiento generados con **Chart.js**.

## 🔑 Usuarios de Prueba
¡Entra al entorno de producción y pruébalo tú mismo! Dependiendo del rol con el que entres, el menú y los permisos cambiarán:

| Rol | Email | Contraseña |
| :--- | :--- | :--- |
| 👑 **Administrador** | `admin@gmail.com` | `12345` |
| 🏋️‍♂️ **Entrenador** | `marcosTrainer@gmail.com` | `12345` |
| 🏃 **Socio** | `alonso@gmail.com` | `12345` |

## ⚙️ Instalación Local
1. Clona el repositorio.
2. En el archivo `appsettings.json`, asegúrate de configurar la URL base de la API. (Por defecto apunta a la API desplegada en Azure, por lo que no necesitas levantar el backend localmente para probar el frontend).
```json
"ApiUrls": {
  "ApiGymphony": "AQUÍ_TU_URL_DE_LA_API_EN_AZURE" 
}
