# 📚 Publishing House Management System

## 📌 Overview
This project is a database-driven application designed to manage the activity of a publishing house.  
It models real-world editorial and commercial workflows, covering **authors, books, publishing houses, contracts, clients, and orders**.


---

## ⚙️ Application Architecture
The system is built around a **role-based access model**.  
After authentication, users are redirected to a dedicated dashboard depending on their role:

- **Administrator** – system management and reporting
- **Editor** – editorial data management
- **Client** – catalog browsing and order placement

User authentication and authorization are handled through the database, with roles stored and managed centrally.

---

## 🧩 Core Features

### 👤 Administrator
- View global system statistics (users, books, orders, total revenue)
- Manage users (delete editors or clients, administrators protected)
- Delete orders with automatic stock restoration
- Generate advanced analytical reports using complex SQL queries

### ✍️ Editor
- Manage books (insert, update price and stock)
- Manage authors
- Manage publishing contracts (royalty percentage, validity period)
- Enforce real-world constraints (books can only be published if a valid author–publisher contract exists)

### 🛒 Client
- Browse the book catalog using multiple filters (author, genre, price range)
- Place orders with real-time stock validation
- View order history and detailed order contents
- Track personal purchasing statistics

---

## 🗄️ Database Design
The database schema models the core entities involved in a publishing house:

- Users
- Authors
- Publishing Houses
- Books
- Publishing Contracts
- Clients
- Orders
- Order Details

The design includes:
- One-to-many relationships (authors → books, clients → orders)
- Many-to-many relationships (authors ↔ publishers, books ↔ orders)
- Strong referential integrity enforced via primary keys, foreign keys, and constraints

---

## 🧪 Data Integrity & Consistency
- `PRIMARY KEY` and `FOREIGN KEY` constraints on all relations
- `UNIQUE` constraints for usernames, emails, and publisher names
- `CHECK` constraints for numeric values (price, stock, royalty)
- `NOT NULL` constraints for mandatory fields
- Transaction-safe operations for order placement and deletion
- Manual cascade logic to ensure consistent stock updates

---

## 🛠️ Technologies Used
- **C#**
- **ASP.NET WebForms**
- **SQL Server**
- **ADO.NET**
- **Bootstrap** (UI styling)
- **Session-based authentication**
- **SQL (JOINs, subqueries, aggregations)**

---

## 📊 Reporting & Queries
The application includes both simple and advanced SQL queries:
- Catalog and list views using JOINs
- Parameterized filtering
- Aggregations using `SUM`, `COUNT`, and `GROUP BY`
- Business reports such as:
  - Top-selling books
  - Top clients by total spending
  - Revenue per publishing house
  - Clients without orders

---

## 💾 Database Setup

A full SQL Server database backup is provided to ensure the application runs with full functionality.

### Requirements
- SQL Server (tested with SQL Server 2022)
- SQL Server Management Studio (SSMS)

### Restore Instructions
1. Open **SQL Server Management Studio**.
2. Connect to your local SQL Server instance.
3. Right-click **Databases** → **Restore Database...**
4. Select **Device** → **Browse** and choose `PublishingHouseDB.bak`.
5. Set the destination database name to `PublishingHouseDB`.
6. Confirm and wait for the restore process to complete.

### Notes
- If file path conflicts occur, open the **Files** tab during restore and update the `.mdf` / `.ldf` locations.
- The application expects the database name to be `PublishingHouseDB`.
- All credentials included are for demonstration purposes only.

---

## ▶️ Running the Application
1. Restore the database as described above.
2. Open the solution in Visual Studio.
3. Verify the connection string (Integrated Security recommended).
4. Build and run the application.

---

## 🎯 Project Purpose
This project demonstrates:
- Practical relational database modeling
- Integration between C# and SQL Server
- Role-based access control
- Enforcement of real-world business rules at database and application level

It provides a solid foundation for future extensions, such as a modern web API or desktop application.
