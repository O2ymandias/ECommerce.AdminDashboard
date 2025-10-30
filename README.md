# FreshCart Admin Dashboard

A comprehensive admin dashboard for the FreshCart e-commerce platform, providing powerful management tools for products, orders, users, and sales analytics.

## 🎯 Overview

The FreshCart Admin Dashboard is the central hub for managing your e-commerce operations. Built to work seamlessly with the FreshCart Backend API, it provides an intuitive interface for administrators to oversee all aspects of the online store.

## 🏗️ Dashboard Features

### Navigation Modules

The dashboard provides access to the following management modules:

- **📁 Catalog** - Manage your store's product catalog

  - Products - Add, edit, and organize product listings
  - Categories - Create and manage product categories
  - Brands - Manage product.

- **👥 Users** - User management and administration

  - View and manage customer accounts
  - Handle user roles and permissions

- **📦 Orders** - Order processing and fulfillment

  - View and process customer orders
  - Update order status and payment status.

- **💰 Sales** - Sales analytics
  - Track revenue and sales metrics

## 🔗 Backend Integration

This dashboard connects to the FreshCart Backend API built with ASP.NET Core using Clean Architecture principles.

### Architecture Layers

- **API Layer** (`ECommerce.APIs`) - RESTful endpoints, authentication, and response handling
- **Application Layer** (`ECommerce.Application`) - Business logic services (Auth, Cart, Checkout, Product, etc.)
- **Domain Layer** (`ECommerce.Core`) - Core entities, DTOs, interfaces, and business rules
- **Infrastructure Layer** (`ECommerce.Infrastructure`) - Database operations, repositories, and external services

### Design Patterns

The backend implements industry-standard patterns:

- **Specification Pattern** - Complex query logic and filtering
- **Repository Pattern** - Data access abstraction
- **Unit of Work Pattern** - Transaction management
- **Factory Pattern** - Object creation and instantiation

## ⚙️ Setup & Configuration

### 1. Clone the Repository

```bash
git clone https://github.com/O2ymandias/FreshCart.AdminDashboard.git
cd FreshCart.Backend
```

### 2. Configure Application Settings

Create or update `ECommerce.APIs/appsettings.Development.json`:

```json
{
	"ConnectionStrings": {
		"Default": "",
		"Redis": ""
	},
	"AdminOptions": {
		"Email": "",
		"Password": ""
	},
	"JwtOptions": {
		"SecurityKey": ""
	},
	"EmailOptions": {
		"SenderEmail": "",
		"Password": ""
	},
	"StripeOptions": {
		"SecretKey": "",
		"PublishableKey": "",
		"WebhookSecret": ""
	}
}
```

## 📚 API Documentation

Access the interactive API documentation:

- **Swagger UI**: BaseUrl/swagger
- **API Base URL**: BaseUrl/api/
- **Postman Collection**: Import the:
  - FreshCart.postman-apis-collection.json
  - FreshCart.Dashbord.postman_collection.json

---

**FreshCart Admin Dashboard** - Empowering your e-commerce operations
