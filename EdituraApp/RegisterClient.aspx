<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RegisterClient.aspx.cs" Inherits="EdituraWeb.RegisterClient" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Create Client Account</title>

    <!-- Bootstrap 5 -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>

    <style>
        body {
            background-color: #f0f2f5;
            font-family: 'Segoe UI', sans-serif;
        }
        .register-card {
            width: 420px;
            border-radius: 12px;
        }
    </style>
</head>

<body class="bg-light">

<form id="form1" runat="server">
    <div class="container d-flex flex-column justify-content-center align-items-center vh-100">

        <h1 class="mb-4 text-center">Creează cont de client</h1>

        <div class="card shadow p-4 register-card">

            <h3 class="text-center mb-4">Register</h3>

            <!-- Username -->
            <div class="mb-3">
                <asp:Label Text="Username:" runat="server" CssClass="form-label" />
                <asp:TextBox ID="txtUser" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="reqUser" runat="server"
                    ControlToValidate="txtUser"
                    ErrorMessage="Username obligatoriu!"
                    CssClass="text-danger" Display="Dynamic" />
            </div>

            <!-- Password -->
            <div class="mb-3">
                <asp:Label Text="Password:" runat="server" CssClass="form-label" />
                <asp:TextBox ID="txtPass" TextMode="Password" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="reqPass" runat="server"
                    ControlToValidate="txtPass"
                    ErrorMessage="Parola obligatorie!"
                    CssClass="text-danger" Display="Dynamic" />
            </div>

            <!-- Nume -->
            <div class="mb-3">
                <asp:Label Text="Nume:" runat="server" CssClass="form-label" />
                <asp:TextBox ID="txtNume" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="reqNume" runat="server"
                    ControlToValidate="txtNume"
                    ErrorMessage="Numele este obligatoriu!"
                    CssClass="text-danger" Display="Dynamic" />
            </div>

            <!-- Prenume -->
            <div class="mb-3">
                <asp:Label Text="Prenume:" runat="server" CssClass="form-label" />
                <asp:TextBox ID="txtPrenume" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="reqPrenume" runat="server"
                    ControlToValidate="txtPrenume"
                    ErrorMessage="Prenumele este obligatoriu!"
                    CssClass="text-danger" Display="Dynamic" />
            </div>

            <!-- Email -->
            <div class="mb-3">
                <asp:Label Text="Email:" runat="server" CssClass="form-label" />
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="reqEmail" runat="server"
                    ControlToValidate="txtEmail"
                    ErrorMessage="Email obligatoriu!"
                    CssClass="text-danger" Display="Dynamic" />
            </div>

            <!-- Telefon -->
            <div class="mb-3">
                <asp:Label Text="Telefon:" runat="server" CssClass="form-label" />
                <asp:TextBox ID="txtTelefon" runat="server" CssClass="form-control" />
            </div>

            <!-- Adresa -->
            <div class="mb-3">
                <asp:Label Text="Adresa:" runat="server" CssClass="form-label" />
                <asp:TextBox ID="txtAdresa" runat="server" CssClass="form-control" />
            </div>

            <!-- Create account -->
            <asp:Button ID="btnCreate" runat="server" Text="Create Account"
                CssClass="btn btn-primary w-100" OnClick="btnCreate_Click" />

            <!-- Feedback message -->
            <asp:Label ID="lblMsg" runat="server" CssClass="mt-3 text-center d-block" />

            <!-- Back to login -->
            <div class="text-center mt-3">
                <a href="Login.aspx" class="btn btn-outline-secondary w-100">Back to Login</a>
            </div>

        </div>

    </div>
<footer class="text-center text-muted mt-5 mb-3">
    © 2026 • EdituraApp • Proiect universitar – Baze de Date
</footer>
</form>

</body>
</html>
