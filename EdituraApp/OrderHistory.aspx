<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderHistory.aspx.cs" Inherits="EdituraWeb.OrderHistory" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Istoric comenzi • EdituraApp</title>

    <!-- Bootstrap 5 -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>

    <!-- Bootstrap Icons -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css" rel="stylesheet" />

    <style>
        body {
            font-family: 'Segoe UI', sans-serif;
            background: radial-gradient(1100px 700px at 15% 15%, rgba(13,110,253,.18), transparent 60%),
                        radial-gradient(900px 600px at 85% 20%, rgba(25,135,84,.18), transparent 55%),
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
                rgba(13,110,253,.95) 0%,
                rgba(25,135,84,.92) 55%,
                rgba(111,66,193,.88) 100%);
            color: #fff;
            position: relative;
            min-height: 160px;
        }

        .hero-left::after {
            content: "";
            position: absolute;
            inset: -120px -120px auto auto;
            width: 300px;
            height: 300px;
            background: rgba(255,255,255,.15);
            border-radius: 50%;
        }

        .hero-left::before {
            content: "";
            position: absolute;
            inset: auto auto -140px -140px;
            width: 360px;
            height: 360px;
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
                            <i class="bi bi-clock-history"></i>
                            EdituraApp • Client
                        </div>

                        <h2 class="mt-3 fw-bold mb-1">Istoric comenzi</h2>
                        <div class="opacity-90">
                            Vezi comenzile tale și detaliile fiecăreia.
                        </div>

                        <div class="mt-3 opacity-75">
                            Selectează o comandă pentru detalii.
                        </div>
                    </div>
                </div>

                <div class="col-lg-7 bg-white">
                    <div class="p-4 p-lg-5">
                        <h4 class="fw-bold mb-1">Gestionare comenzi</h4>
                        <div class="muted mb-3">
                            Accesează comenzile tale anterioare.
                        </div>

                        <div class="d-flex flex-wrap gap-2">
                            <a href="PlaceOrder.aspx" class="btn btn-outline-primary btn-pill">
                                <i class="bi bi-cart-plus"></i> Plasează comandă
                            </a>

                            <a href="ClientPage.aspx" class="btn btn-outline-secondary btn-pill">
                                <i class="bi bi-arrow-left"></i> Înapoi
                            </a>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- CONTINUT -->
        <div class="row g-4">

            <!-- Lista comenzi -->
            <div class="col-lg-6">
                <div class="card shadow-sm p-4 cardx bg-white">
                    <h5 class="fw-bold mb-3">
                        <i class="bi bi-receipt"></i> Comenzile mele
                    </h5>

                    <asp:GridView ID="gvOrders" runat="server"
                        CssClass="table table-striped table-bordered table-hover"
                        AutoGenerateColumns="False"
                        DataKeyNames="id_comanda"
                        OnSelectedIndexChanged="gvOrders_SelectedIndexChanged"
                        EmptyDataText="Nu ai comenzi încă.">

                        <Columns>
                            <asp:BoundField DataField="id_comanda" HeaderText="ID" />
                            <asp:BoundField DataField="data_comanda" HeaderText="Data"
                                DataFormatString="{0:yyyy-MM-dd}" />
                            <asp:BoundField DataField="total" HeaderText="Total (lei)"
                                DataFormatString="{0:N2}" />
                            <asp:CommandField ShowSelectButton="True" SelectText="Detalii" />
                        </Columns>
                    </asp:GridView>

                    <asp:Label ID="lblOrdersInfo" runat="server" CssClass="muted"></asp:Label>
                </div>
            </div>

            <!-- Detalii comanda -->
            <div class="col-lg-6">
                <div class="card shadow-sm p-4 cardx bg-white">
                    <h5 class="fw-bold mb-3">
                        <i class="bi bi-list-ul"></i> Detalii comandă
                    </h5>

                    <asp:Label ID="lblSelectedOrder" runat="server"
                        CssClass="text-primary fw-semibold d-block mb-2"></asp:Label>

                    <asp:GridView ID="gvOrderDetails" runat="server"
                        CssClass="table table-striped table-bordered table-hover"
                        AutoGenerateColumns="False"
                        EmptyDataText="Selectează o comandă din stânga pentru a vedea detaliile.">

                        <Columns>
                            <asp:BoundField DataField="titlu" HeaderText="Carte" />
                            <asp:BoundField DataField="cantitate" HeaderText="Cantitate" />
                            <asp:BoundField DataField="pret_unitar" HeaderText="Preț unitar"
                                DataFormatString="{0:N2}" />
                            <asp:BoundField DataField="subtotal" HeaderText="Subtotal"
                                DataFormatString="{0:N2}" />
                        </Columns>
                    </asp:GridView>

                    <asp:Label ID="lblDetailsTotal" runat="server"
                        CssClass="mt-2 d-block fw-semibold"></asp:Label>

                    <asp:Label ID="lblMsg" runat="server"
                        CssClass="mt-2 d-block"></asp:Label>
                </div>
            </div>

        </div>

    </div>
<footer class="text-center text-muted mt-5 mb-3">
    © 2026 • EdituraApp • Proiect universitar – Baze de Date
</footer>
</form>
</body>
</html>
