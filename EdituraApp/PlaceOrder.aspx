<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PlaceOrder.aspx.cs" Inherits="EdituraWeb.PlaceOrder" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Plasează comandă • EdituraApp</title>

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
                rgba(25,135,84,.92) 0%,
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
        .btn-pill { border-radius:12px; font-weight:700; padding:10px 14px; }
        .form-control, .form-select { border-radius:12px; }
        .muted { color:#6c757d; }
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
                            <i class="bi bi-cart-check"></i>
                            EdituraApp • Comandă
                        </div>

                        <h2 class="mt-3 fw-bold mb-1">Plasează o comandă</h2>
                        <div class="opacity-90">
                            Selectează cartea dorită și finalizează comanda.
                        </div>

                        <div class="mt-3 opacity-75">
                            Stocul se actualizează automat.
                        </div>
                    </div>
                </div>

                <div class="col-lg-7 bg-white">
                    <div class="p-4 p-lg-5">
                        <h4 class="fw-bold mb-1">Formular comandă</h4>
                        <div class="muted mb-3">
                            Completează datele de mai jos.
                        </div>

                        <div class="d-flex flex-wrap gap-2">
                            <a href="Catalog.aspx" class="btn btn-outline-secondary btn-pill">
                                <i class="bi bi-book"></i> Catalog
                            </a>

                            <a href="ClientPage.aspx" class="btn btn-outline-secondary btn-pill">
                                <i class="bi bi-arrow-left"></i> Înapoi
                            </a>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- FORMULAR COMANDA -->
        <div class="card shadow-sm p-4 cardx bg-white mx-auto" style="max-width: 520px;">
            <h5 class="fw-bold mb-3">
                <i class="bi bi-box-seam"></i> Alege o carte
            </h5>

            <div class="mb-3">
                <label class="form-label">Carte</label>
                <asp:DropDownList ID="ddlCarti" runat="server"
                    CssClass="form-select"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="ddlCarti_SelectedIndexChanged">
                </asp:DropDownList>
            </div>

            <div class="row g-3 mb-3">
                <div class="col-md-6">
                    <label class="form-label">Preț (lei)</label>
                    <asp:TextBox ID="txtPret" runat="server"
                        CssClass="form-control" ReadOnly="true"></asp:TextBox>
                </div>

                <div class="col-md-6">
                    <label class="form-label">Stoc disponibil</label>
                    <asp:TextBox ID="txtStoc" runat="server"
                        CssClass="form-control" ReadOnly="true"></asp:TextBox>
                </div>
            </div>

            <div class="mb-3">
                <label class="form-label">Cantitate</label>
                <asp:TextBox ID="txtCantitate" runat="server"
                    CssClass="form-control" Text="1"></asp:TextBox>
            </div>

            <asp:Button ID="btnPlaceOrder" runat="server"
                Text="Plasează comanda"
                CssClass="btn btn-primary btn-pill w-100"
                OnClick="btnPlaceOrder_Click" />

            <asp:Label ID="lblMsg" runat="server"
                CssClass="mt-3 d-block text-center"></asp:Label>
        </div>

    </div>
<footer class="text-center text-muted mt-5 mb-3">
    © 2026 • EdituraApp • Proiect universitar – Baze de Date
</footer>
</form>
</body>
</html>
