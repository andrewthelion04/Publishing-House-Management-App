<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageAuthors.aspx.cs" Inherits="EdituraApp.ManageAuthors" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Manage Authors • EdituraApp</title>

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
                        <i class="bi bi-person-lines-fill"></i>
                        EdituraApp • Editor
                    </div>

                    <h2 class="mt-3 fw-bold mb-1">Gestionare autori</h2>
                    <div class="opacity-90">
                        Autori, gen literar, date de contact.
                    </div>

                    <div class="mt-3 opacity-75">
                        Autorii sunt baza contractelor și a catalogului.
                    </div>
                </div>
            </div>

            <div class="col-lg-7 bg-white">
                <div class="p-4 p-lg-5">
                    <h4 class="fw-bold mb-1">Panou editor</h4>
                    <div class="muted mb-3">
                        Adaugă, caută și editează autori.
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

    <div class="row g-4">

        <!-- LISTA AUTORI -->
        <div class="col-lg-8">
            <div class="card shadow-sm p-4 cardx bg-white">
                <div class="d-flex justify-content-between align-items-center mb-2">
                    <h5 class="fw-bold m-0">
                        <i class="bi bi-people"></i> Autori
                    </h5>
                    <div style="width: 320px;">
                        <asp:TextBox ID="txtSearch" runat="server"
                            CssClass="form-control"
                            placeholder="Caută după nume / prenume..."></asp:TextBox>
                    </div>
                </div>

                <div class="d-flex gap-2 mb-3">
                    <asp:Button ID="btnSearch" runat="server" Text="Caută"
                        CssClass="btn btn-primary btn-pill"
                        OnClick="btnSearch_Click" />

                    <asp:Button ID="btnReset" runat="server" Text="Reset"
                        CssClass="btn btn-outline-secondary btn-pill"
                        OnClick="btnReset_Click" />

                    <asp:Label ID="lblCount" runat="server"
                        CssClass="ms-auto muted align-self-center"></asp:Label>
                </div>

                <asp:GridView ID="gvAuthors" runat="server"
                    CssClass="table table-striped table-bordered table-hover"
                    AutoGenerateColumns="False"
                    DataKeyNames="id_autor"
                    OnRowEditing="gvAuthors_RowEditing"
                    OnRowCancelingEdit="gvAuthors_RowCancelingEdit"
                    OnRowUpdating="gvAuthors_RowUpdating"
                    EmptyDataText="Nu există autori.">

                    <Columns>
                        <asp:BoundField DataField="id_autor" HeaderText="ID" ReadOnly="true" />

                        <asp:TemplateField HeaderText="Nume">
                            <ItemTemplate><%# Eval("nume") %></ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEditNume" runat="server"
                                    CssClass="form-control"
                                    Text='<%# Bind("nume") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Prenume">
                            <ItemTemplate><%# Eval("prenume") %></ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEditPrenume" runat="server"
                                    CssClass="form-control"
                                    Text='<%# Bind("prenume") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Naționalitate">
                            <ItemTemplate><%# Eval("nationalitate") %></ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEditNat" runat="server"
                                    CssClass="form-control"
                                    Text='<%# Bind("nationalitate") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Gen literar">
                            <ItemTemplate><%# Eval("gen_literar") %></ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEditGen" runat="server"
                                    CssClass="form-control"
                                    Text='<%# Bind("gen_literar") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Email">
                            <ItemTemplate><%# Eval("email") %></ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEditEmail" runat="server"
                                    CssClass="form-control"
                                    Text='<%# Bind("email") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:CommandField ShowEditButton="True"
                            EditText="Edit" UpdateText="Save" CancelText="Cancel" />
                    </Columns>
                </asp:GridView>

                <asp:Label ID="lblMsg" runat="server" CssClass="mt-3 d-block"></asp:Label>
            </div>
        </div>

        <!-- ADAUGARE AUTOR -->
        <div class="col-lg-4">
            <div class="card shadow-sm p-4 cardx bg-white">
                <h5 class="fw-bold mb-3">
                    <i class="bi bi-plus-circle"></i> Adaugă autor
                </h5>

                <div class="mb-3">
                    <label class="form-label">Nume *</label>
                    <asp:TextBox ID="txtNewNume" runat="server" CssClass="form-control"></asp:TextBox>
                </div>

                <div class="mb-3">
                    <label class="form-label">Prenume *</label>
                    <asp:TextBox ID="txtNewPrenume" runat="server" CssClass="form-control"></asp:TextBox>
                </div>

                <div class="mb-3">
                    <label class="form-label">Naționalitate</label>
                    <asp:TextBox ID="txtNewNat" runat="server" CssClass="form-control"></asp:TextBox>
                </div>

                <div class="mb-3">
                    <label class="form-label">Gen literar</label>
                    <asp:TextBox ID="txtNewGen" runat="server" CssClass="form-control"></asp:TextBox>
                </div>

                <div class="mb-3">
                    <label class="form-label">Email (unic)</label>
                    <asp:TextBox ID="txtNewEmail" runat="server"
                        CssClass="form-control"
                        placeholder="ex: autor@mail.com"></asp:TextBox>
                </div>

                <asp:Button ID="btnAddAuthor" runat="server"
                    Text="Adaugă autor"
                    CssClass="btn btn-success btn-pill w-100"
                    OnClick="btnAddAuthor_Click" />

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
