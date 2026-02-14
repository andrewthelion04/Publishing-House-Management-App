<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditorPage.aspx.cs" Inherits="EdituraApp.EditorPage" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Editor Dashboard • EdituraApp</title>

    <!-- Bootstrap 5 -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>

    <!-- Bootstrap Icons -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css" rel="stylesheet" />

    <style>
        body {
            font-family: 'Segoe UI', sans-serif;
            background: radial-gradient(1100px 700px at 15% 15%, rgba(25,135,84,.18), transparent 60%),
                        radial-gradient(900px 600px at 85% 20%, rgba(13,110,253,.18), transparent 55%),
                        linear-gradient(135deg, #f7f9fc 0%, #eef2f7 100%);
            min-height: 100vh;
        }

        .hero {
            border-radius: 18px;
            overflow: hidden;
            border: 0;
        }

        .hero-left {
            background: linear-gradient(135deg,
                rgba(25,135,84,.95) 0%,
                rgba(13,110,253,.92) 55%,
                rgba(111,66,193,.88) 100%);
            color: #fff;
            position: relative;
            min-height: 160px;
        }

        .hero-left::after {
            content: "";
            position: absolute;
            inset: -120px -120px auto auto;
            width: 320px;
            height: 320px;
            background: rgba(255,255,255,.15);
            border-radius: 50%;
        }

        .hero-left::before {
            content: "";
            position: absolute;
            inset: auto auto -140px -140px;
            width: 380px;
            height: 380px;
            background: rgba(255,255,255,.12);
            border-radius: 50%;
        }

        .hero-content {
            position: relative;
            z-index: 1;
            padding: 24px;
        }

        .brand-badge {
            display: inline-flex;
            align-items: center;
            gap: 10px;
            padding: 8px 12px;
            border-radius: 999px;
            background: rgba(255,255,255,.16);
            border: 1px solid rgba(255,255,255,.22);
            font-weight: 700;
        }

        .cardx { border-radius:14px; }
        .stat-number { font-size:1.6rem; font-weight:700; }
        .muted { color:#6c757d; }
        .btn-pill { border-radius:12px; font-weight:700; padding:10px 14px; }
    </style>
</head>

<body>
<form id="form1" runat="server">
    <div class="container mt-4 mb-5">

        <!-- HERO HEADER -->
        <div class="card shadow-lg hero mb-4">
            <div class="row g-0">
                <div class="col-lg-5 hero-left">
                    <div class="hero-content">
                        <div class="brand-badge">
                            <i class="bi bi-pencil-square"></i>
                            EdituraApp • Editor
                        </div>

                        <h2 class="mt-3 fw-bold mb-1">Editor Dashboard</h2>
                        <div class="opacity-90">
                            Gestionare catalog, autori și contracte.
                        </div>

                        <div class="mt-3 opacity-75">
                            Rol: editor
                        </div>
                    </div>
                </div>

                <div class="col-lg-7 bg-white">
                    <div class="p-4 p-lg-5">
                        <h4 class="fw-bold mb-1">Panou editor</h4>
                        <div class="muted mb-3">
                            Administrare conținut editorial.
                        </div>

                        <div class="d-flex flex-wrap gap-2 align-items-center">
                            <a href="ManageBooks.aspx" class="btn btn-primary btn-pill">
                                <i class="bi bi-book"></i> Cărți
                            </a>

                            <a href="ManageContracts.aspx" class="btn btn-outline-primary btn-pill">
                                <i class="bi bi-file-earmark-text"></i> Contracte
                            </a>

                            <a href="ManageAuthors.aspx" class="btn btn-outline-primary btn-pill">
                                <i class="bi bi-people"></i> Autori
                            </a>

                          
                            <div class="ms-auto d-flex align-items-center gap-2">
                                <i class="bi bi-person-circle text-secondary"></i>
                                <asp:Label ID="lblWelcome" runat="server" CssClass="muted"></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- STATS -->
        <div class="row g-3 mb-4">
            <div class="col-md-3">
                <div class="card shadow-sm p-3 cardx bg-white">
                    <div class="muted">Cărți</div>
                    <asp:Label ID="lblBooks" runat="server" CssClass="stat-number"></asp:Label>
                    <div class="muted">în catalog</div>
                </div>
            </div>

            <div class="col-md-3">
                <div class="card shadow-sm p-3 cardx bg-white">
                    <div class="muted">Autori</div>
                    <asp:Label ID="lblAuthors" runat="server" CssClass="stat-number"></asp:Label>
                    <div class="muted">înregistrați</div>
                </div>
            </div>

            <div class="col-md-3">
                <div class="card shadow-sm p-3 cardx bg-white">
                    <div class="muted">Edituri</div>
                    <asp:Label ID="lblPublishers" runat="server" CssClass="stat-number"></asp:Label>
                    <div class="muted">active</div>
                </div>
            </div>

            <div class="col-md-3">
                <div class="card shadow-sm p-3 cardx bg-white">
                    <div class="muted">Contracte</div>
                    <asp:Label ID="lblContracts" runat="server" CssClass="stat-number"></asp:Label>
                    <div class="muted">publicare</div>
                </div>
            </div>
        </div>

        <!-- CONTINUT -->
        <div class="row g-3">
            <div class="col-lg-6">
                <div class="card shadow-sm p-4 cardx bg-white">
                    <h5 class="fw-bold mb-3">
                        <i class="bi bi-lightning-charge"></i> Quick actions
                    </h5>

                    <div class="d-grid gap-2">
                        <a href="ManageBooks.aspx" class="btn btn-outline-primary btn-pill">
                            Adaugă / modifică cărți
                        </a>

                        <a href="ManageAuthors.aspx" class="btn btn-outline-primary btn-pill">
                            Adaugă / modifică autori
                        </a>

                        <a href="ManageContracts.aspx" class="btn btn-outline-primary btn-pill">
                            Gestionează contracte
                        </a>
                    </div>

                    <div class="muted mt-3">
                        Contractele definesc ce edituri pot publica cărțile unui autor.
                    </div>
                </div>
            </div>

            <div class="col-lg-6">
                <div class="card shadow-sm p-4 cardx bg-white">
                    <h5 class="fw-bold mb-3">
                        <i class="bi bi-info-circle"></i> Status
                    </h5>

                    <div class="muted">
                        Editorul poate gestiona:
                        <ul class="mt-2">
                            <li>catalogul de cărți </li>
                            <li>autori</li>
                            <li>contracte de publicare</li>
                        </ul>
                        Rapoartele avansate sunt disponibile doar pentru admin.
                    </div>

                    <asp:Label ID="lblMsg" runat="server" CssClass="mt-3 d-block"></asp:Label>
                </div>
            </div>
        </div>

        <!-- LOGOUT JOS  -->
        <div class="d-flex justify-content-center mt-4">
            <asp:Button ID="btnLogout" runat="server" Text="Logout"
                CssClass="btn btn-outline-secondary btn-pill"
                OnClick="btnLogout_Click" />
        </div>

    </div>
<footer class="text-center text-muted mt-5 mb-3">
    © 2026 • EdituraApp • Proiect universitar – Baze de Date
</footer>
</form>
</body>
</html>
