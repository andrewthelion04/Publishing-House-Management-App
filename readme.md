# 📚 Publishing House Management System

## 📌 Overview
This project is a database-driven application designed to manage the activity of a publishing house.  
It centralizes information about **authors, books, publishing houses, contracts, clients, and orders**, modeling real-world editorial and commercial processes.

The application was developed for educational purposes, focusing on **database design**, **SQL queries**, and **role-based application logic**.

---

## ⚙️ How the Application Works
The system uses a **role-based access model**. Each user logs in and is redirected to a specific dashboard depending on their role:

- **Administrator**
- **Editor**
- **Client**

Authentication is handled through a login page that validates user credentials against the database and stores the user role in session.

---

## 🧩 Main Functionalities

### 👤 Administrator
- View global system statistics (users, books, orders, total revenue)
- Manage users (delete editors or clients, with protections for admins)
- Delete orders with automatic stock restoration
- Generate complex analytical reports (sales, revenue, top clients, top books)

### ✍️ Editor
- Manage books (insert, edit price and stock)
- Manage authors
- Manage publishing contracts (royalty, validity period)
- Insert books only if a valid author–publisher contract exists
- View editorial statistics

### 🛒 Client
- Browse the book catalog with advanced filters
- Place orders with real-time stock validation
- View order history and detailed order breakdowns
- Track personal purchasing statistics

---

## 🗄️ Database Design
The database models the core entities of a publishing house:

- Users
- Authors
- Publishing Houses
- Books
- Publishing Contracts
- Clients
- Orders
- Order Details

Relationships include:
- One-to-many (authors → books, clients → orders)
- Many-to-many (authors ↔ publishers via contracts, books ↔ orders)
- Referential integrity enforced using primary keys, foreign keys, and constraints

---

## 🧪 Data Integrity & Consistency
- UNIQUE constraints for usernames, emails, and publisher names
- CHECK constraints for prices, stock values, and percentages
- NOT NULL constraints for mandatory fields
- Controlled deletions with manual cascade logic
- SQL transactions used for critical operations (orders, deletions)

---

## 🛠️ Technologies & Tools Used
- **C#**
- **ASP.NET WebForms**
- **SQL Server**
- **ADO.NET**
- **Bootstrap** (UI styling)
- **Session-based authentication**
- **SQL (JOINs, subqueries, aggregations)**

---

## 📊 Reporting & Queries
The application includes:
- Simple queries with JOINs for listings and filters
- Complex queries using `GROUP BY`, `SUM`, `COUNT`, subqueries, and parameters
- Dynamic reports such as:
  - Top-selling books
  - Top clients by spending
  - Revenue per publishing house
  - Clients without orders

---

## 🎯 Purpose
This project demonstrates:
- Practical database modeling
- Integration between C# and SQL
- Role-based application logic
- Real-world business rules implemented at database and application level

It provides a solid foundation for future extensions, such as a modern web API or desktop application.
