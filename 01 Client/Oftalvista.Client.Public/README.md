# Oftalvista Frontend

<<<<<<< HEAD
Aplicación web para la gestión de una clínica oftalmológica. Permite administrar usuarios, médicos, pacientes, agendas, citas y más, desde una interfaz moderna y organizada.

## Descripción

Este proyecto es un frontend desarrollado en Angular que consume una API REST. Está diseñado como una SPA (Single Page Application), con navegación fluida y módulos cargados de forma eficiente.

La aplicación ofrece vistas y funcionalidades diferenciadas según el tipo de usuario, facilitando tanto la administración interna como el acceso de los pacientes a su información.

## Funcionalidades

- Autenticación con control de acceso por roles
- Dashboard para administrador
- Dashboard para paciente
- Gestión de usuarios
- Gestión de médicos y especialidades
- Registro y mantenimiento de pacientes
- Administración de agenda médica
- Gestión de citas
- Registro de pagos
- Consulta de historial clínico
- Recordatorios de citas

## Arquitectura

El proyecto sigue buenas prácticas de Angular moderno:

- Componentes standalone
- Lazy loading por módulos funcionales
- Guards para protección de rutas
- Interceptores HTTP para:
  - Manejo de tokens (JWT)
  - Control global de errores
  - Simulación de backend (modo demo)
- Servicios desacoplados por entidad

## Estructura general

```text
ClientApp/
├── core/              # Servicios globales, interceptores, guards
├── features/          # Módulos funcionales (usuarios, citas, etc.)
├── shared/            # Componentes reutilizables
├── layouts/           # Estructura visual (dashboards, navegación)
├── environments/      # Configuración por entorno
=======
Aplicacion web frontend del sistema **Oftalvista**, desarrollada con Angular y orientada a la gestion de autenticacion, dashboards y modulos operativos para usuarios administradores y pacientes.

## Objetivo

Este proyecto corresponde a la capa cliente de la aplicacion web. Su funcion es proporcionar la interfaz de usuario, la navegacion entre pantallas, la validacion de formularios, el consumo de servicios HTTP y la integracion con los modulos funcionales del sistema.

## Tecnologias utilizadas

Las principales tecnologias identificadas en la construccion del frontend son las siguientes:

| Tecnologia | Uso dentro del proyecto |
| --- | --- |
| **Angular 17** | Framework principal para construir la SPA (Single Page Application). |
| **TypeScript** | Lenguaje base para el desarrollo de componentes, servicios, guards e interceptors. |
| **Angular Standalone Components** | Arquitectura moderna de Angular sin `AppModule`, usando `bootstrapApplication` y componentes standalone. |
| **Angular Router** | Gestion de rutas, redireccionamientos, carga diferida mediante `loadComponent` y proteccion de vistas. |
| **Angular Forms / Reactive Forms** | Construccion y validacion reactiva de formularios, por ejemplo en el modulo de autenticacion. |
| **Angular HttpClient** | Consumo de APIs REST para la comunicacion con el backend. |
| **HTTP Interceptors** | Manejo centralizado de JWT, errores y modo demo para las peticiones HTTP. |
| **Route Guards** | Control de acceso a rutas autenticadas y rutas restringidas para administradores. |
| **Angular Material** | Biblioteca de componentes UI para tablas, dialogs, formularios, snackbars, tooltips, datepickers y otros elementos visuales. |
| **Angular CDK** | Utilidades base complementarias para componentes y comportamiento de interfaz. |
| **SCSS** | Estilos globales y por componente con variables CSS y layout responsivo. |
| **RxJS** | Programacion reactiva para observables y flujos asincronos en servicios y autenticacion. |
| **Zone.js** | Soporte de deteccion de cambios requerido por Angular. |
| **Node.js + npm** | Entorno de ejecucion y gestor de paquetes para desarrollo, instalacion y build. |
| **Angular CLI** | Herramienta de compilacion, servidor de desarrollo y empaquetado del proyecto. |

## Arquitectura del frontend

El frontend sigue una arquitectura basada en Angular moderno:

- **Bootstrap standalone** mediante `bootstrapApplication` en `src/main.ts`.
- **Configuracion global por providers** en `src/app/app.config.ts`.
- **Rutas lazy-loaded** con `loadComponent` en `src/app/app.routes.ts`.
- **Separacion por capas** en `core`, `shared` y `modules`.
- **Servicios de dominio** para entidades como usuarios, pacientes, medicos, citas, pagos, historial y recordatorios.
- **Ambientes configurables** mediante `src/environments/environment.ts` y `src/environments/environment.prod.ts`.

## Estructura funcional

El proyecto organiza su funcionalidad en modulos de negocio del dominio Oftalvista, entre ellos:

- Autenticacion
- Dashboard de administrador
- Dashboard de paciente
- Gestion de usuarios
- Gestion de especialidades medicas
- Gestion de medicos
- Gestion de pacientes
- Agenda medica
- Citas
- Pagos de citas
- Historial de citas
- Recordatorios de citas

## UI y experiencia de usuario

En el frontend se emplean componentes de Angular Material y estilos personalizados para construir una interfaz moderna y responsiva. Entre los elementos visuales implementados se encuentran:

- Barras de navegacion
- Sidebar
- Tarjetas
- Tablas con paginacion
- Dialogos modales
- Formularios validados
- Indicadores visuales tipo chip
- Layout responsivo para escritorio y pantallas pequenas

## Integracion con backend

La aplicacion consume servicios HTTP contra una API configurada por ambiente:

- Desarrollo: `http://localhost:5000/api/v1`
- Produccion: `https://api.oftalvista.pe/api/v1`

Adicionalmente, el proyecto contempla un **modo demo** controlado desde configuracion y mediante un interceptor dedicado.

## Scripts disponibles

Los scripts definidos actualmente en `package.json` son:

```bash
npm start
npm run build
```

Su funcion es:

- `npm start`: levanta el servidor de desarrollo con Angular CLI.
- `npm run build`: genera la compilacion del proyecto para despliegue.

## Dependencias principales detectadas

Dependencias de aplicacion:

- `@angular/animations`
- `@angular/cdk`
- `@angular/common`
- `@angular/compiler`
- `@angular/core`
- `@angular/forms`
- `@angular/material`
- `@angular/platform-browser`
- `@angular/platform-browser-dynamic`
- `@angular/router`
- `rxjs`
- `tslib`
- `zone.js`

Dependencias de desarrollo:

- `@angular-devkit/build-angular`
- `@angular/cli`
- `@angular/compiler-cli`
- `typescript`

## Resumen breve para entrega

Si necesitas una version corta para un informe o formato institucional, puedes usar este texto:

> El frontend de la aplicacion web Oftalvista fue construido con **Angular 17** y **TypeScript**, utilizando una arquitectura de **componentes standalone**, **Angular Router** para la navegacion, **Reactive Forms** para formularios, **HttpClient** para el consumo de APIs REST, **Angular Material y CDK** para la interfaz de usuario, **SCSS** para estilos y **RxJS** para el manejo reactivo de datos y procesos asincronos.
>>>>>>> develop
