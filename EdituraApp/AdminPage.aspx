<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminPage.aspx.cs" Inherits="EdituraWeb.AdminPage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Admin Dashboard • EdituraApp</title>

    <!-- Bootstrap 5 -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>

    <!-- Bootstrap Icons -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css" rel="stylesheet" />

    <style>
        body {
            font-family: 'Segoe UI', sans-serif;
            background: radial-gradient(1200px 800px at 10% 10%, rgba(13,110,253,.18), transparent 60%),
                        radial-gradient(900px 700px at 90% 20%, rgba(25,135,84,.16), transparent 55%),
                        linear-gradient(135deg, #f7f9fc 0%, #eef2f7 100%);
            min-height: 100vh;
        }

        .hero {
            border-radius: 18px;
            overflow: hidden;
            border: 0;
        }

        .hero-left {
            background: linear-gradient(135deg, rgba(13,110,253,.95) 0%, rgba(111,66,193,.92) 55%, rgba(25,135,84,.86) 100%);
            color: #fff;
            position: relative;
            min-height: 180px;
        }

        .hero-left::after {
            content: "";
            position: absolute;
            inset: -120px -120px auto auto;
            width: 320px;
            height: 320px;
            background: rgba(255,255,255,.14);
            border-radius: 50%;
        }

        .hero-left::before {
            content: "";
            position: absolute;
            inset: auto auto -140px -140px;
            width: 380px;
            height: 380px;
            background: rgba(255,255,255,.10);
            border-radius: 50%;
        }

        .hero-content {
            position: relative;
            z-index: 1;
            padding: 26px 26px;
        }

        .brand-badge {
            display: inline-flex;
            align-items: center;
            gap: 10px;
            padding: 8px 12px;
            border-radius: 999px;
            background: rgba(255,255,255,.16);
            border: 1px solid rgba(255,255,255,.20);
            font-weight: 700;
        }

        .muted { color:#6c757d; }
        .stat-card { border-radius: 14px; }
        .stat-value { font-size: 1.35rem; font-weight: 800; }
        .action-card { border-radius: 14px; }
        .action-card .btn { border-radius: 12px; font-weight: 700; }
        .btn-pill { border-radius: 12px; font-weight: 700; padding: 10px 14px; }
        .mini { font-size: .92rem; }
    </style>
</head>

<body>
<form id="form1" runat="server">
    <div class="container mt-4 mb-5">

        <!-- HERO / HEADER  -->
        <div class="card shadow-lg hero mb-4">
            <div class="row g-0">
                <div class="col-lg-5 hero-left">
                    <div class="hero-content">
                        <div class="brand-badge">
                            <i class="bi bi-shield-lock"></i>
                            EdituraApp • Admin
                        </div>

                        <h2 class="mt-3 fw-bold mb-1">Admin Dashboard</h2>
                        <div class="opacity-90">
                            Utilizatori, comenzi, rapoarte — într-un singur loc.
                        </div>

                        <div class="mt-3 mini opacity-75">
                            Tip: anularea unei comenzi restochează automat cărțile.
                        </div>
                    </div>
                </div>

                <div class="col-lg-7 bg-white">
                    <div class="p-4 p-lg-5">
                        <asp:Label ID="lblWelcome" runat="server" CssClass="h4 fw-bold d-block mb-1"></asp:Label>
                        <div class="muted mb-3">Alege o opțiune și verifică rapid statusul aplicației.</div>

                        <asp:Label ID="lblMsg" runat="server" CssClass="d-block"></asp:Label>

                        <div class="mt-3 d-flex flex-wrap gap-2">
                            <a href="DeleteUsers.aspx" class="btn btn-outline-danger btn-pill">
                                <i class="bi bi-people"></i> Gestionare utilizatori
                            </a>
                            <a href="DeleteOrders.aspx" class="btn btn-outline-danger btn-pill">
                                <i class="bi bi-receipt-cutoff"></i> Anulare comenzi
                            </a>
                            <a href="Reports.aspx" class="btn btn-outline-primary btn-pill">
                                <i class="bi bi-graph-up"></i> Reports
                            </a>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- STATS -->
        <div class="row g-3 mb-3">

            <div class="col-md-3">
                <div class="card shadow-sm p-4 stat-card bg-white">
                    <div class="d-flex align-items-center gap-2 mb-1">
                        <i class="bi bi-person-badge"></i>
                        <span class="muted">Total utilizatori</span>
                    </div>
                    <asp:Label ID="lblTotalUsers" runat="server" CssClass="stat-value"></asp:Label>
                </div>
            </div>

            <div class="col-md-3">
                <div class="card shadow-sm p-4 stat-card bg-white">
                    <div class="d-flex align-items-center gap-2 mb-1">
                        <i class="bi bi-people"></i>
                        <span class="muted">Clienți</span>
                    </div>
                    <asp:Label ID="lblTotalClients" runat="server" CssClass="stat-value"></asp:Label>
                </div>
            </div>

            <div class="col-md-3">
                <div class="card shadow-sm p-4 stat-card bg-white">
                    <div class="d-flex align-items-center gap-2 mb-1">
                        <i class="bi bi-book"></i>
                        <span class="muted">Cărți</span>
                    </div>
                    <asp:Label ID="lblTotalBooks" runat="server" CssClass="stat-value"></asp:Label>
                </div>
            </div>

            <div class="col-md-3">
                <div class="card shadow-sm p-4 stat-card bg-white">
                    <div class="d-flex align-items-center gap-2 mb-1">
                        <i class="bi bi-cart-check"></i>
                        <span class="muted">Comenzi</span>
                    </div>
                    <asp:Label ID="lblTotalOrders" runat="server" CssClass="stat-value"></asp:Label>
                </div>
            </div>

        </div>

        <div class="row g-3 mb-4">
            <div class="col-md-4">
                <div class="card shadow-sm p-4 stat-card bg-white">
                    <div class="d-flex align-items-center gap-2 mb-1">
                        <i class="bi bi-currency-exchange"></i>
                        <span class="muted">Venit total</span>
                    </div>
                    <asp:Label ID="lblRevenue" runat="server" CssClass="stat-value"></asp:Label>
                    <div class="muted mini mt-1">Sumă calculată din totalul comenzilor</div>
                </div>
            </div>

            <div class="col-md-8">
                <div class="card shadow-sm p-4 action-card bg-white">
                    <h5 class="fw-bold mb-2"><i class="bi bi-lightning-charge"></i> Acțiuni rapide</h5>
                    <div class="muted mb-3">Acces rapid către funcțiile de evaluare: delete / rapoarte.</div>

                    <div class="d-flex flex-wrap gap-2">
                        <a href="DeleteUsers.aspx" class="btn btn-danger btn-pill">
                            <i class="bi bi-trash3"></i> Delete Users
                        </a>
                        <a href="DeleteOrders.aspx" class="btn btn-danger btn-pill">
                            <i class="bi bi-x-circle"></i> Cancel Orders (Restock)
                        </a>
                        <a href="Reports.aspx" class="btn btn-primary btn-pill">
                            <i class="bi bi-graph-up-arrow"></i> Open Reports
                        </a>
                    </div>

                    <div class="muted mini mt-3">
                        Următorul pas: adăugare editori / reset parole / administrare stoc (dacă va fi cerut).
                    </div>
                </div>
            </div>
        </div>

        <!-- footer buttons  -->
        <div class="d-flex justify-content-center gap-2">
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
