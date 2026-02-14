<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientPage.aspx.cs" Inherits="EdituraApp.ClientPage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Client Dashboard • EdituraApp</title>

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
            min-height: 170px;
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
            padding: 24px 24px;
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
        .cardx { border-radius: 14px; }
        .btn-pill { border-radius: 12px; font-weight: 700; padding: 10px 14px; }
        .stat-number { font-size:1.6rem; font-weight:800; }
        .mini { font-size:.92rem; }
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
                            <i class="bi bi-person-circle"></i>
                            EdituraApp • Client
                        </div>

                        <h2 class="mt-3 fw-bold mb-1">Client Dashboard</h2>
                        <div class="opacity-90">
                            Catalog, comenzi și istoric — totul din contul tău.
                        </div>

                        <div class="mt-3 mini opacity-75">
                            <asp:Label ID="lblWelcome" runat="server"></asp:Label>
                        </div>
                    </div>
                </div>

                <div class="col-lg-7 bg-white">
                    <div class="p-4 p-lg-5">
                        <h4 class="fw-bold mb-1">Bun venit!</h4>
                        <div class="muted mb-3">
                            Alege o acțiune rapidă sau consultă statisticile tale.
                        </div>

                        <div class="d-flex flex-wrap gap-2">
                            <a href="Catalog.aspx" class="btn btn-primary btn-pill">
                                <i class="bi bi-book"></i> Catalog
                            </a>
                            <a href="PlaceOrder.aspx" class="btn btn-outline-primary btn-pill">
                                <i class="bi bi-cart-plus"></i> Plasează comandă
                            </a>
                            <a href="OrderHistory.aspx" class="btn btn-outline-secondary btn-pill">
                                <i class="bi bi-clock-history"></i> Istoric comenzi
                            </a>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- STATISTICI -->
        <div class="row g-3 mb-4">
            <div class="col-md-4">
                <div class="card shadow-sm p-4 cardx bg-white">
                    <div class="muted">Comenzi plasate</div>
                    <asp:Label ID="lblOrdersCount" runat="server" CssClass="stat-number"></asp:Label>
                    <div class="muted mini">număr total</div>
                </div>
            </div>

            <div class="col-md-4">
                <div class="card shadow-sm p-4 cardx bg-white">
                    <div class="muted">Total cheltuit</div>
                    <asp:Label ID="lblTotalSpent" runat="server" CssClass="stat-number"></asp:Label>
                    <div class="muted mini">lei</div>
                </div>
            </div>

            <div class="col-md-4">
                <div class="card shadow-sm p-4 cardx bg-white">
                    <div class="muted">Ultima comandă</div>
                    <asp:Label ID="lblLastOrder" runat="server" CssClass="stat-number"></asp:Label>
                    <div class="muted mini">data</div>
                </div>
            </div>
        </div>

        <!-- CONTINUT -->
        <div class="row g-3">
            <div class="col-lg-6">
                <div class="card shadow-sm p-4 cardx bg-white">
                    <h5 class="fw-bold mb-3"><i class="bi bi-lightning-charge"></i> Acțiuni rapide</h5>

                    <div class="d-grid gap-2">
                        <a href="Catalog.aspx" class="btn btn-outline-primary btn-pill">
                            Vezi catalogul de cărți
                        </a>
                        <a href="PlaceOrder.aspx" class="btn btn-outline-primary btn-pill">
                            Plasează o comandă
                        </a>
                        <a href="OrderHistory.aspx" class="btn btn-outline-primary btn-pill">
                            Istoric comenzi
                        </a>
                    </div>

                    <div class="muted mini mt-3">
                        Tip: poți filtra catalogul după autor, editură sau gen.
                    </div>
                </div>
            </div>

            <div class="col-lg-6">
                <div class="card shadow-sm p-4 cardx bg-white">
                    <h5 class="fw-bold mb-3"><i class="bi bi-info-circle"></i> Informații</h5>

                    <div class="muted">
                        Din contul tău poți:
                        <ul class="mt-2">
                            <li>să vezi cărțile disponibile</li>
                            <li>să plasezi comenzi</li>
                            <li>să consulți istoricul comenzilor</li>
                        </ul>
                    </div>

                    <asp:Label ID="lblMsg" runat="server" CssClass="mt-3 d-block"></asp:Label>
                </div>
            </div>
        </div>

        <!-- FOOTER ACTIONS -->
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
