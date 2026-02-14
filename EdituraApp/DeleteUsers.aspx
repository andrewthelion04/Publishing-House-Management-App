<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeleteUsers.aspx.cs" Inherits="EdituraWeb.DeleteUsers" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Gestionare utilizatori • Admin</title>

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
            min-height: 160px;
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
        .mini { font-size: .92rem; }
        .form-select, .btn { border-radius: 12px; }
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
                            <i class="bi bi-people"></i>
                            EdituraApp • Admin
                        </div>

                        <h2 class="mt-3 fw-bold mb-1">Gestionare utilizatori</h2>
                        <div class="opacity-90">
                            Șterge clienți / editori. Adminii sunt protejați.
                        </div>

                        <div class="mt-3 mini opacity-75">
                            <asp:Label ID="lblWelcome" runat="server"></asp:Label>
                        </div>
                    </div>
                </div>

                <div class="col-lg-7 bg-white">
                    <div class="p-4 p-lg-5">
                        <h4 class="fw-bold mb-1">Ștergere utilizatori</h4>
                        <div class="muted mb-3">
                            Selectează un utilizator și apasă Delete. Pentru clienți, se șterg și datele asociate (comenzi/detalii).
                        </div>

                        <!-- Selected -->
                        <asp:Label ID="lblSelected" runat="server" CssClass="muted d-block"></asp:Label>

                        <!-- Filter row -->
                        <div class="d-flex flex-wrap align-items-end gap-2 mt-3">
                            <div style="min-width: 240px;">
                                <label class="form-label m-0 muted">Filtru rol</label>
                                <asp:DropDownList ID="ddlRoleFilter" runat="server" CssClass="form-select"></asp:DropDownList>
                            </div>

                            <asp:Button ID="btnApplyFilter" runat="server" Text="Aplică"
                                CssClass="btn btn-primary btn-pill" OnClick="btnApplyFilter_Click" />

                            <asp:Button ID="btnResetFilter" runat="server" Text="Reset"
                                CssClass="btn btn-outline-secondary btn-pill" OnClick="btnResetFilter_Click" />

                            <asp:Button ID="btnRefresh" runat="server" Text="Refresh"
                                CssClass="btn btn-outline-secondary btn-pill" OnClick="btnRefresh_Click" />

                            <asp:Label ID="lblCount" runat="server" CssClass="ms-lg-auto muted align-self-center"></asp:Label>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- CONTENT CARD -->
        <div class="card shadow-sm p-4 cardx bg-white">
            <div class="d-flex justify-content-between align-items-center mb-2">
                <h5 class="m-0 fw-bold"><i class="bi bi-table"></i> Lista utilizatori</h5>
                <span class="muted mini">Selectează o linie pentru a activa Delete</span>
            </div>

            <asp:GridView ID="gvUsers" runat="server"
                CssClass="table table-striped table-bordered table-hover"
                AutoGenerateColumns="False"
                DataKeyNames="id_utilizator"
                OnSelectedIndexChanged="gvUsers_SelectedIndexChanged"
                EmptyDataText="Nu exista utilizatori pentru filtrul selectat.">
                <Columns>
                    <asp:BoundField DataField="id_utilizator" HeaderText="ID" />
                    <asp:BoundField DataField="username" HeaderText="Username" />
                    <asp:BoundField DataField="rol" HeaderText="Rol" />
                    <asp:CommandField ShowSelectButton="True" SelectText="Selectează" />
                </Columns>
            </asp:GridView>

            <div class="d-flex flex-wrap gap-2 mt-3">
                <asp:Button ID="btnDelete" runat="server" Text="Șterge utilizator"
                    CssClass="btn btn-danger btn-pill"
                    Enabled="false"
                    OnClientClick="return confirm('Ești sigur că vrei să ștergi acest utilizator?');"
                    OnClick="btnDelete_Click" />

                <a href="AdminPage.aspx" class="btn btn-outline-secondary btn-pill">
                    <i class="bi bi-arrow-left"></i> Înapoi la Admin
                </a>

                <asp:Button ID="btnLogout" runat="server" Text="Logout"
                    CssClass="btn btn-outline-secondary btn-pill ms-auto"
                    OnClick="btnLogout_Click" />
            </div>

            <asp:Label ID="lblMsg" runat="server" CssClass="mt-3 d-block"></asp:Label>
        </div>

    </div>
<footer class="text-center text-muted mt-5 mb-3">
© 2026 • EdituraApp • Proiect universitar – Baze de Date
</footer>
</form>
</body>
</html>
