<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageBooks.aspx.cs" Inherits="EdituraApp.ManageBooks" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Manage Books • EdituraApp</title>

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
        .muted { color:#6c757d; }
        .btn-pill { border-radius:12px; font-weight:700; padding:10px 14px; }
        .form-control, .form-select { border-radius:12px; }
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
                        EdituraApp • Editor
                    </div>

                    <h2 class="mt-3 fw-bold mb-1">Gestionare cărți</h2>
                    <div class="opacity-90">
                        Filtrare, editare preț/stoc și adăugare cărți.
                    </div>

                    <div class="mt-3 opacity-75">
                        Legătura autor–editură se face prin contracte.
                    </div>
                </div>
            </div>

            <div class="col-lg-7 bg-white">
                <div class="p-4 p-lg-5">
                    <h4 class="fw-bold mb-1">Panou editor</h4>
                    <div class="muted mb-3">
                        Administrare catalog editorial.
                    </div>

                    <div class="d-flex flex-wrap gap-2">
                        <a href="EditorPage.aspx" class="btn btn-outline-secondary btn-pill">
                            <i class="bi bi-arrow-left"></i> Înapoi
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- FILTRE -->
    <div class="card shadow-sm p-4 cardx bg-white mb-4">
        <h5 class="fw-bold mb-3">
            <i class="bi bi-funnel"></i> Filtre
        </h5>

        <div class="row g-3">
            <div class="col-md-4">
                <label class="form-label">Titlu conține</label>
                <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" placeholder="ex: Harry"></asp:TextBox>
            </div>

            <div class="col-md-3">
                <label class="form-label">Gen</label>
                <asp:TextBox ID="txtGen" runat="server" CssClass="form-control" placeholder="ex: Fantasy"></asp:TextBox>
            </div>

            <div class="col-md-3">
                <label class="form-label">Autor</label>
                <asp:DropDownList ID="ddlAuthorFilter" runat="server" CssClass="form-select"></asp:DropDownList>
            </div>

            <div class="col-md-2">
                <label class="form-label">Editură</label>
                <asp:DropDownList ID="ddlPublisherFilter" runat="server" CssClass="form-select"></asp:DropDownList>
            </div>

            <div class="col-12 d-flex gap-2">
                <asp:Button ID="btnApplyFilters" runat="server" Text="Aplică"
                    CssClass="btn btn-primary btn-pill" OnClick="btnApplyFilters_Click" />

                <asp:Button ID="btnResetFilters" runat="server" Text="Reset"
                    CssClass="btn btn-outline-secondary btn-pill" OnClick="btnResetFilters_Click" />

                <asp:Label ID="lblCount" runat="server" CssClass="ms-auto muted align-self-center"></asp:Label>
            </div>
        </div>
    </div>

    <div class="row g-4">

        <!-- LISTA CARTI -->
        <div class="col-lg-8">
            <div class="card shadow-sm p-4 cardx bg-white">
                <h5 class="fw-bold mb-3">
                    <i class="bi bi-table"></i> Cărți (editare preț & stoc)
                </h5>

                <asp:GridView ID="gvBooks" runat="server"
                    CssClass="table table-striped table-bordered table-hover"
                    AutoGenerateColumns="False"
                    DataKeyNames="id_carte"
                    OnRowEditing="gvBooks_RowEditing"
                    OnRowCancelingEdit="gvBooks_RowCancelingEdit"
                    OnRowUpdating="gvBooks_RowUpdating"
                    EmptyDataText="Nu există cărți pentru filtrele selectate.">

                    <Columns>
                        <asp:BoundField DataField="id_carte" HeaderText="ID" ReadOnly="true" />
                        <asp:BoundField DataField="titlu" HeaderText="Titlu" ReadOnly="true" />
                        <asp:BoundField DataField="gen" HeaderText="Gen" ReadOnly="true" />
                        <asp:BoundField DataField="autor" HeaderText="Autor" ReadOnly="true" />
                        <asp:BoundField DataField="editura" HeaderText="Editură" ReadOnly="true" />

                        <asp:TemplateField HeaderText="Preț (lei)">
                            <ItemTemplate>
                                <%# string.Format("{0:0.00}", Eval("pret")) %>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEditPret" runat="server" CssClass="form-control"
                                    Text='<%# Bind("pret") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Stoc">
                            <ItemTemplate><%# Eval("stoc") %></ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEditStoc" runat="server" CssClass="form-control"
                                    Text='<%# Bind("stoc") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:CommandField ShowEditButton="True" EditText="Edit"
                            UpdateText="Save" CancelText="Cancel" />
                    </Columns>
                </asp:GridView>

                <asp:Label ID="lblMsg" runat="server" CssClass="mt-3 d-block"></asp:Label>
            </div>
        </div>

        <!-- ADAUGARE CARTE -->
        <div class="col-lg-4">
            <div class="card shadow-sm p-4 cardx bg-white">
                <h5 class="fw-bold mb-3">
                    <i class="bi bi-plus-circle"></i> Adaugă carte
                </h5>

                <div class="mb-3">
                    <label class="form-label">Titlu *</label>
                    <asp:TextBox ID="txtNewTitle" runat="server" CssClass="form-control"></asp:TextBox>
                </div>

                <div class="mb-3">
                    <label class="form-label">Gen</label>
                    <asp:TextBox ID="txtNewGen" runat="server" CssClass="form-control"></asp:TextBox>
                </div>

                <div class="mb-3">
                    <label class="form-label">Preț *</label>
                    <asp:TextBox ID="txtNewPrice" runat="server" CssClass="form-control" placeholder="ex: 49.99"></asp:TextBox>
                </div>

                <div class="mb-3">
                    <label class="form-label">Stoc *</label>
                    <asp:TextBox ID="txtNewStock" runat="server" CssClass="form-control" placeholder="ex: 10"></asp:TextBox>
                </div>

                <div class="mb-3">
                    <label class="form-label">Autor *</label>
                    <asp:DropDownList ID="ddlNewAuthor" runat="server"
                        CssClass="form-select"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlNewAuthor_SelectedIndexChanged"></asp:DropDownList>
                </div>

                <div class="mb-3">
                    <label class="form-label">Editură *</label>
                    <asp:DropDownList ID="ddlNewPublisher" runat="server" CssClass="form-select"></asp:DropDownList>
                </div>

                <asp:Button ID="btnAddBook" runat="server" Text="Adaugă"
                    CssClass="btn btn-success btn-pill w-100" OnClick="btnAddBook_Click" />

                <asp:Label ID="lblAddMsg" runat="server" CssClass="mt-3 d-block"></asp:Label>

                <div class="muted mt-3">
                    * câmpuri obligatorii
                </div>
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
