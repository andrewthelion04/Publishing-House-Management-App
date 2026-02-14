<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Catalog.aspx.cs" Inherits="EdituraWeb.Catalog" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Catalog • EdituraApp</title>

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
            padding: 24px;
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
        .cardx { border-radius:14px; }
        .btn-pill { border-radius:12px; font-weight:700; padding:10px 14px; }
        .form-control, .form-select { border-radius:12px; }
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
                            <i class="bi bi-book"></i>
                            EdituraApp • Catalog
                        </div>

                        <h2 class="mt-3 fw-bold mb-1">Catalog cărți</h2>
                        <div class="opacity-90">
                            Caută și filtrează cărțile disponibile.
                        </div>

                        <div class="mt-3 mini opacity-75">
                            Tip: poți combina filtrele pentru interogări complexe.
                        </div>
                    </div>
                </div>

                <div class="col-lg-7 bg-white">
                    <div class="p-4 p-lg-5">
                        <h4 class="fw-bold mb-1">Explorare catalog</h4>
                        <div class="muted mb-3">
                            Folosește filtrele pentru a restrânge rezultatele.
                        </div>

                        <div class="d-flex flex-wrap gap-2">
                            <a href="ClientPage.aspx" class="btn btn-outline-secondary btn-pill">
                                <i class="bi bi-arrow-left"></i> Înapoi
                            </a>

                            <!-- buton filtre -->
                            <button type="button"
                                class="btn btn-outline-primary btn-pill d-flex align-items-center gap-2"
                                data-bs-toggle="collapse"
                                data-bs-target="#filtersPanel"
                                aria-expanded="false"
                                aria-controls="filtersPanel">
                                <i class="bi bi-sliders"></i>
                                Filtre
                                <i class="bi bi-chevron-down"></i>
                            </button>

                            <asp:Label ID="lblInfo" runat="server" CssClass="muted align-self-center"></asp:Label>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- FILTRE (COLLAPSE) -->
        <div class="collapse" id="filtersPanel">
            <div class="card shadow-sm p-4 cardx bg-white mb-4">
                <div class="d-flex justify-content-between align-items-center mb-2">
                    <h5 class="m-0 fw-bold"><i class="bi bi-funnel"></i> Filtre</h5>
                    <span class="muted mini">Completează doar ce ai nevoie</span>
                </div>

                <div class="row g-3 mt-1">
                    <div class="col-md-4">
                        <label class="form-label">Autor</label>
                        <asp:DropDownList ID="ddlAutori" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Editură</label>
                        <asp:DropDownList ID="ddlEdituri" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Gen</label>
                        <asp:TextBox ID="txtGen" runat="server" CssClass="form-control" placeholder="ex: Fantasy"></asp:TextBox>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label">Titlu</label>
                        <asp:TextBox ID="txtTitlu" runat="server" CssClass="form-control" placeholder="ex: Harry"></asp:TextBox>
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Preț minim</label>
                        <asp:TextBox ID="txtPretMin" runat="server" CssClass="form-control" placeholder="ex: 10"></asp:TextBox>
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Preț maxim</label>
                        <asp:TextBox ID="txtPretMax" runat="server" CssClass="form-control" placeholder="ex: 100"></asp:TextBox>
                    </div>

                    <div class="col-12 d-flex gap-2 mt-2">
                        <asp:Button ID="btnApply" runat="server" Text="Aplică filtre"
                            CssClass="btn btn-primary btn-pill" OnClick="btnApply_Click" />

                        <asp:Button ID="btnReset" runat="server" Text="Reset"
                            CssClass="btn btn-outline-secondary btn-pill" OnClick="btnReset_Click" />
                    </div>
                </div>
            </div>
        </div>

        <!-- LISTA CARTI -->
        <div class="card shadow-sm p-4 cardx bg-white">
            <div class="d-flex justify-content-between align-items-center mb-2">
                <h5 class="m-0 fw-bold"><i class="bi bi-table"></i> Lista cărților</h5>
                <span class="muted mini">Rezultate filtrate</span>
            </div>

            <asp:GridView ID="gvCatalog" runat="server"
                CssClass="table table-striped table-bordered table-hover"
                AutoGenerateColumns="False"
                EmptyDataText="Nu există cărți pentru filtrele selectate.">
                <Columns>
                    <asp:BoundField DataField="id_carte" HeaderText="ID" />
                    <asp:BoundField DataField="titlu" HeaderText="Titlu" />
                    <asp:BoundField DataField="gen" HeaderText="Gen" />
                    <asp:BoundField DataField="pret" HeaderText="Preț" DataFormatString="{0:N2}" />
                    <asp:BoundField DataField="stoc" HeaderText="Stoc" />
                    <asp:BoundField DataField="autor" HeaderText="Autor" />
                    <asp:BoundField DataField="editura" HeaderText="Editură" />
                </Columns>
            </asp:GridView>
        </div>

    </div>
<footer class="text-center text-muted mt-5 mb-3">
    © 2026 • EdituraApp • Proiect universitar – Baze de Date
</footer>
</form>
</body>
</html>
