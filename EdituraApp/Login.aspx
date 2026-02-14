<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="EdituraWeb.Login" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Login • Editura</title>

    <!-- Bootstrap 5 -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>

    <!-- Bootstrap Icons -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css" rel="stylesheet" />

    <style>
        body {
            font-family: 'Segoe UI', sans-serif;
            min-height: 100vh;
            background: radial-gradient(1200px 800px at 10% 10%, rgba(13,110,253,.18), transparent 60%),
                        radial-gradient(900px 700px at 90% 20%, rgba(25,135,84,.16), transparent 55%),
                        linear-gradient(135deg, #f7f9fc 0%, #eef2f7 100%);
        }

        .auth-shell {
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 28px 12px;
        }

        .auth-card {
            border-radius: 18px;
            overflow: hidden;
            max-width: 980px;
            width: 100%;
            border: 0;
        }

        .brand-side {
            background: linear-gradient(135deg, rgba(13,110,253,.95) 0%, rgba(111,66,193,.92) 60%, rgba(25,135,84,.86) 100%);
            color: #fff;
            position: relative;
        }

        .brand-side::after {
            content: "";
            position: absolute;
            inset: -120px -120px auto auto;
            width: 320px;
            height: 320px;
            background: rgba(255,255,255,.14);
            border-radius: 50%;
            filter: blur(0px);
        }

        .brand-side::before {
            content: "";
            position: absolute;
            inset: auto auto -120px -120px;
            width: 360px;
            height: 360px;
            background: rgba(255,255,255,.10);
            border-radius: 50%;
        }

        .brand-content {
            position: relative;
            z-index: 1;
            padding: 34px;
        }

        .brand-badge {
            display: inline-flex;
            align-items: center;
            gap: 10px;
            padding: 8px 12px;
            border-radius: 999px;
            background: rgba(255,255,255,.16);
            border: 1px solid rgba(255,255,255,.20);
            font-weight: 600;
        }

        .brand-title {
            margin-top: 18px;
            font-weight: 800;
            letter-spacing: .2px;
        }

        .brand-subtitle {
            opacity: .92;
            margin-top: 10px;
            line-height: 1.4;
        }

        .form-side {
            background: #fff;
        }

        .form-wrap {
            padding: 34px;
        }

        .form-title {
            font-weight: 800;
            margin-bottom: 6px;
        }

        .form-hint {
            color: #6c757d;
            margin-bottom: 18px;
        }

        .input-group-text {
            background: #f7f9fc;
            border-color: #e6ebf2;
        }

        .form-control {
            border-color: #e6ebf2;
        }

        .form-control:focus {
            border-color: rgba(13,110,253,.55);
            box-shadow: 0 0 0 .2rem rgba(13,110,253,.12);
        }

        .btn-primary {
            border-radius: 12px;
            padding: 10px 14px;
            font-weight: 700;
        }

        .btn-outline-secondary {
            border-radius: 12px;
            padding: 10px 14px;
            font-weight: 700;
        }

        .tiny-footer {
            color: #6c757d;
            font-size: .9rem;
            margin-top: 18px;
        }
    </style>
</head>

<body>
<form id="form1" runat="server">
    <div class="auth-shell">
        <div class="card shadow-lg auth-card">
            <div class="row g-0">

                <!-- stanga: branding -->
                <div class="col-lg-5 brand-side d-none d-lg-block">
                    <div class="brand-content">
                        <div class="brand-badge">
                            <i class="bi bi-book"></i>
                            EdituraApp
                        </div>

                        <h2 class="brand-title">Gestionare edituri & comenzi</h2>
                        <div class="brand-subtitle">
                            Autori, contracte, catalog, comenzi — totul într-un singur loc.
                        </div>

                        <div class="mt-4 small opacity-75">
                            Tip: conturile de admin/editor sunt gestionate de backend.
                        </div>
                    </div>
                </div>

                <!-- dreapta: formular -->
                <div class="col-lg-7 form-side">
                    <div class="form-wrap">
                        <h3 class="form-title">Bun venit!</h3>
                        <div class="form-hint">Autentifică-te pentru a continua.</div>

                        <div class="mb-3">
                            <asp:Label ID="lblUser" runat="server" Text="Username" CssClass="form-label"></asp:Label>
                            <div class="input-group">
                                <span class="input-group-text"><i class="bi bi-person"></i></span>
                                <asp:TextBox ID="txtUser" runat="server" CssClass="form-control" placeholder="ex: client_ion"></asp:TextBox>
                            </div>
                        </div>

                        <div class="mb-2">
                            <asp:Label ID="lblPass" runat="server" Text="Password" CssClass="form-label"></asp:Label>
                            <div class="input-group">
                                <span class="input-group-text"><i class="bi bi-lock"></i></span>
                                <asp:TextBox ID="txtPass" runat="server" CssClass="form-control" TextMode="Password" placeholder="••••••••"></asp:TextBox>
                            </div>
                        </div>

                        <div class="d-grid gap-2 mt-3">
                            <asp:Button ID="btnLogin" runat="server" Text="Login"
                                CssClass="btn btn-primary" OnClick="btnLogin_Click" />
                            <a href="RegisterClient.aspx" class="btn btn-outline-secondary">
                                Create Account
                            </a>
                        </div>

                        <asp:Label ID="lblError" runat="server"
                            CssClass="text-danger mt-3 d-block text-center"></asp:Label>

                        <div class="tiny-footer text-center">
                            © 2026 • EdituraApp • Proiect universitar – Baze de Date
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>
</form>
</body>
</html>
