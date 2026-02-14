using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace EdituraWeb
{
    public partial class Reports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // protectie: doar admin
            if (Session["rol"] == null || Session["rol"].ToString() != "admin")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadEdituri();
                LoadAutori();
                LoadReportList();
                SetHint();
            }
        }

        private void LoadReportList()
        {
            ddlReport.Items.Clear();

            // 6 rapoarte JOIN simple
            ddlReport.Items.Add(new ListItem("JOIN #1 - Catalog carti (Carti+Autori+Edituri)", "J1"));
            ddlReport.Items.Add(new ListItem("JOIN #2 - Contracte publicare (Contracte+Autori+Edituri)", "J2"));
            ddlReport.Items.Add(new ListItem("JOIN #3 - Comenzi cu clienti (Comenzi+Clienti)", "J3"));
            ddlReport.Items.Add(new ListItem("JOIN #4 - Detalii comanda (Detalii+Carti)", "J4"));
            ddlReport.Items.Add(new ListItem("JOIN #5 - Clienti cu username (Clienti+Utilizatori)", "J5"));
            ddlReport.Items.Add(new ListItem("JOIN #6 - Carti sub prag de stoc (Carti+Edituri)", "J6"));

            // 4 rapoarte complexe
            ddlReport.Items.Add(new ListItem("COMPLEX #1 - Top carti vandute (SUM cantitate)", "C1"));
            ddlReport.Items.Add(new ListItem("COMPLEX #2 - Top clienti dupa cheltuieli (SUM total)", "C2"));
            ddlReport.Items.Add(new ListItem("COMPLEX #3 - Venit pe edituri (SUM subtotal)", "C3"));
            ddlReport.Items.Add(new ListItem("COMPLEX #4 - Clienti fara comenzi (NOT EXISTS)", "C4"));

            ddlReport.SelectedIndex = 0;
        }

        private void LoadEdituri()
        {
            ddlEditura.Items.Clear();
            ddlEditura.Items.Add(new ListItem("Toate editurile", "0"));

            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT id_editura, denumire FROM Edituri ORDER BY denumire", con))
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        ddlEditura.Items.Add(new ListItem(rdr["denumire"].ToString(), rdr["id_editura"].ToString()));
                    }
                }
            }
        }

        private void LoadAutori()
        {
            ddlAutor.Items.Clear();
            ddlAutor.Items.Add(new ListItem("Toti autorii", "0"));

            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT id_autor, nume, prenume FROM Autori ORDER BY nume, prenume", con))
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        string text = rdr["nume"].ToString() + " " + rdr["prenume"].ToString();
                        ddlAutor.Items.Add(new ListItem(text, rdr["id_autor"].ToString()));
                    }
                }
            }
        }

        protected void ddlReport_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetHint();
        }

        private void SetHint()
        {
            string code = ddlReport.SelectedValue;
            lblMsg.Text = "";
            lblMsg.CssClass = "mt-3 d-block";
            lblInfo.Text = "";
            lblHint.Text = "";

            switch (code)
            {
                case "J1":
                    lblHint.Text = "Filtre utile: Editura, Autor, Gen, Interval pret (nu avem pret aici ca parametru, dar poti folosi Gen/Editura/Autor).";
                    break;
                case "J2":
                    lblHint.Text = "Filtre utile: Autor, Editura. (ContractePublicare + Autori + Edituri).";
                    break;
                case "J3":
                    lblHint.Text = "Filtre utile: Data start/stop (optional).";
                    break;
                case "J4":
                    lblHint.Text = "Parametru util: ID Comanda (txtOrderId).";
                    break;
                case "J5":
                    lblHint.Text = "Afiseaza clientii impreuna cu username-ul lor (JOIN Clienti + Utilizatori).";
                    break;
                case "J6":
                    lblHint.Text = "Parametru util: Prag stoc (txtStockThreshold).";
                    break;
                case "C1":
                    lblHint.Text = "Parametri utili: Top N + Data start/stop (optional). (Agregare SUM cantitate).";
                    break;
                case "C2":
                    lblHint.Text = "Parametri utili: Top N + Data start/stop (optional). (Agregare SUM total).";
                    break;
                case "C3":
                    lblHint.Text = "Parametri utili: Data start/stop (optional). (Venit pe edituri).";
                    break;
                case "C4":
                    lblHint.Text = "Clientii care NU au comenzi (NOT EXISTS). Parametrii de data sunt ignorati aici.";
                    break;
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtDateStart.Text = "";
            txtDateEnd.Text = "";
            txtTopN.Text = "";
            txtStockThreshold.Text = "";
            txtOrderId.Text = "";
            txtGen.Text = "";

            ddlEditura.SelectedIndex = 0;
            ddlAutor.SelectedIndex = 0;

            gvReport.DataSource = null;
            gvReport.DataBind();

            lblInfo.Text = "";
            lblMsg.Text = "";
            lblHint.Text = "";

            SetHint();
        }

        protected void btnRun_Click(object sender, EventArgs e)
        {
            try
            {
                RunSelectedReport();
            }
            catch (Exception ex)
            {
                lblMsg.CssClass = "text-danger mt-3 d-block";
                lblMsg.Text = "Eroare: " + ex.Message;
            }
        }

        private void RunSelectedReport()
        {
            string code = ddlReport.SelectedValue;

            // parametri comuni
            DateTime d1, d2;
            bool hasD1 = DateTime.TryParse(txtDateStart.Text.Trim(), out d1);
            bool hasD2 = DateTime.TryParse(txtDateEnd.Text.Trim(), out d2);

            int topN;
            bool hasTopN = int.TryParse(txtTopN.Text.Trim(), out topN) && topN > 0;

            int stockTh;
            bool hasStockTh = int.TryParse(txtStockThreshold.Text.Trim(), out stockTh) && stockTh >= 0;

            int orderId;
            bool hasOrderId = int.TryParse(txtOrderId.Text.Trim(), out orderId) && orderId > 0;

            int idEditura;
            int.TryParse(ddlEditura.SelectedValue, out idEditura);

            int idAutor;
            int.TryParse(ddlAutor.SelectedValue, out idAutor);

            string gen = txtGen.Text.Trim();

            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;

            string sql = "";
            SqlCommand cmd;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                switch (code)
                {
                    // -------------------
                    // 6 JOIN SIMPLE
                    // -------------------

                    // JOIN #1: Carti + Autori + Edituri (filtre autor/editura/gen)
                    case "J1":
                        sql = @"
SELECT
    c.id_carte,
    c.titlu,
    c.gen,
    c.pret,
    c.stoc,
    (a.nume + ' ' + a.prenume) AS autor,
    e.denumire AS editura
FROM Carti c
JOIN Autori a ON c.id_autor = a.id_autor
JOIN Edituri e ON c.id_editura = e.id_editura
WHERE 1=1
";
                        if (idAutor != 0) sql += " AND c.id_autor = @idAutor";
                        if (idEditura != 0) sql += " AND c.id_editura = @idEditura";
                        if (!string.IsNullOrWhiteSpace(gen)) sql += " AND c.gen LIKE @gen";
                        sql += " ORDER BY c.titlu";

                        cmd = new SqlCommand(sql, con);
                        if (idAutor != 0) cmd.Parameters.AddWithValue("@idAutor", idAutor);
                        if (idEditura != 0) cmd.Parameters.AddWithValue("@idEditura", idEditura);
                        if (!string.IsNullOrWhiteSpace(gen)) cmd.Parameters.AddWithValue("@gen", "%" + gen + "%");
                        Bind(cmd);
                        return;

                    // JOIN #2: ContractePublicare + Autori + Edituri (filtre autor/editura)
                    case "J2":
                        sql = @"
SELECT
    cp.id_contract,
    (a.nume + ' ' + a.prenume) AS autor,
    e.denumire AS editura,
    cp.data_semnare,
    cp.data_expirare,
    cp.procent_royalty
FROM ContractePublicare cp
JOIN Autori a ON cp.id_autor = a.id_autor
JOIN Edituri e ON cp.id_editura = e.id_editura
WHERE 1=1
";
                        if (idAutor != 0) sql += " AND cp.id_autor = @idAutor";
                        if (idEditura != 0) sql += " AND cp.id_editura = @idEditura";
                        sql += " ORDER BY cp.data_semnare DESC";

                        cmd = new SqlCommand(sql, con);
                        if (idAutor != 0) cmd.Parameters.AddWithValue("@idAutor", idAutor);
                        if (idEditura != 0) cmd.Parameters.AddWithValue("@idEditura", idEditura);
                        Bind(cmd);
                        return;

                    // JOIN #3: Comenzi + Clienti (filtru optional pe date)
                    case "J3":
                        sql = @"
SELECT
    co.id_comanda,
    co.data_comanda,
    co.total,
    (cl.nume + ' ' + cl.prenume) AS client,
    cl.email
FROM Comenzi co
JOIN Clienti cl ON co.id_client = cl.id_client
WHERE 1=1
";
                        if (hasD1) sql += " AND co.data_comanda >= @d1";
                        if (hasD2) sql += " AND co.data_comanda <= @d2";
                        sql += " ORDER BY co.data_comanda DESC, co.id_comanda DESC";

                        cmd = new SqlCommand(sql, con);
                        if (hasD1) cmd.Parameters.AddWithValue("@d1", d1.Date);
                        if (hasD2) cmd.Parameters.AddWithValue("@d2", d2.Date);
                        Bind(cmd);
                        return;

                    // JOIN #4: DetaliiComanda + Carti (parametru id_comanda)
                    case "J4":
                        if (!hasOrderId)
                            throw new Exception("Pentru acest raport trebuie completat ID Comanda (txtOrderId).");

                        sql = @"
SELECT
    dc.id_comanda,
    c.titlu,
    dc.cantitate,
    dc.pret_unitar,
    (dc.cantitate * dc.pret_unitar) AS subtotal
FROM DetaliiComanda dc
JOIN Carti c ON dc.id_carte = c.id_carte
WHERE dc.id_comanda = @idComanda
ORDER BY c.titlu";

                        cmd = new SqlCommand(sql, con);
                        cmd.Parameters.AddWithValue("@idComanda", orderId);
                        Bind(cmd);
                        return;

                    // JOIN #5: Clienti + Utilizatori (username)
                    case "J5":
                        sql = @"
SELECT
    u.id_utilizator,
    u.username,
    u.rol,
    cl.id_client,
    cl.nume,
    cl.prenume,
    cl.email,
    cl.telefon,
    cl.adresa
FROM Clienti cl
JOIN Utilizatori u ON cl.id_utilizator = u.id_utilizator
ORDER BY cl.nume, cl.prenume";

                        cmd = new SqlCommand(sql, con);
                        Bind(cmd);
                        return;

                    // JOIN #6: Carti + Edituri (sub prag stoc)
                    case "J6":
                        if (!hasStockTh)
                            throw new Exception("Pentru acest raport trebuie completat Prag stoc (txtStockThreshold).");

                        sql = @"
SELECT
    c.id_carte,
    c.titlu,
    c.stoc,
    e.denumire AS editura
FROM Carti c
JOIN Edituri e ON c.id_editura = e.id_editura
WHERE c.stoc < @th
ORDER BY c.stoc ASC, c.titlu";

                        cmd = new SqlCommand(sql, con);
                        cmd.Parameters.AddWithValue("@th", stockTh);
                        Bind(cmd);
                        return;

                    // -------------------
                    // 4 COMPLEXE (rescrise cu subcereri)
                    // -------------------

                    // COMPLEX #1: Top carti vandute (SUM cantitate) + interval date (optional) + TOP N
                    case "C1":
                        if (!hasTopN) topN = 5; // default

                        sql = @"
SELECT TOP (@topN)
    t.id_carte,
    t.titlu,
    t.total_bucati,
    t.incasari
FROM (
    SELECT
        c.id_carte,
        c.titlu,
        SUM(dc.cantitate) AS total_bucati,
        SUM(dc.cantitate * dc.pret_unitar) AS incasari
    FROM DetaliiComanda dc
    JOIN Carti c ON dc.id_carte = c.id_carte
    JOIN Comenzi co ON dc.id_comanda = co.id_comanda
    WHERE 1=1
";
                        if (hasD1) sql += " AND co.data_comanda >= @d1";
                        if (hasD2) sql += " AND co.data_comanda <= @d2";

                        sql += @"
    GROUP BY c.id_carte, c.titlu
) AS t
ORDER BY t.total_bucati DESC, t.incasari DESC";

                        cmd = new SqlCommand(sql, con);
                        cmd.Parameters.AddWithValue("@topN", topN);
                        if (hasD1) cmd.Parameters.AddWithValue("@d1", d1.Date);
                        if (hasD2) cmd.Parameters.AddWithValue("@d2", d2.Date);
                        Bind(cmd);
                        return;

                    // COMPLEX #2: Top clienti dupa cheltuieli (SUM total) + interval date + TOP N
                    case "C2":
                        if (!hasTopN) topN = 5; // default

                        sql = @"
SELECT TOP (@topN)
    t.id_client,
    t.client,
    t.email,
    t.total_cheltuit,
    t.nr_comenzi
FROM (
    SELECT
        cl.id_client,
        (cl.nume + ' ' + cl.prenume) AS client,
        cl.email,
        SUM(co.total) AS total_cheltuit,
        COUNT(*) AS nr_comenzi
    FROM Clienti cl
    JOIN Comenzi co ON cl.id_client = co.id_client
    WHERE 1=1
";
                        if (hasD1) sql += " AND co.data_comanda >= @d1";
                        if (hasD2) sql += " AND co.data_comanda <= @d2";

                        sql += @"
    GROUP BY cl.id_client, cl.nume, cl.prenume, cl.email
) AS t
ORDER BY t.total_cheltuit DESC, t.nr_comenzi DESC";

                        cmd = new SqlCommand(sql, con);
                        cmd.Parameters.AddWithValue("@topN", topN);
                        if (hasD1) cmd.Parameters.AddWithValue("@d1", d1.Date);
                        if (hasD2) cmd.Parameters.AddWithValue("@d2", d2.Date);
                        Bind(cmd);
                        return;

                    // COMPLEX #3: Venit pe edituri (SUM subtotal) + interval date
                    case "C3":
                        sql = @"
SELECT
    t.id_editura,
    t.editura,
    t.incasari_total,
    t.bucati_total
FROM (
    SELECT
        e.id_editura,
        e.denumire AS editura,
        SUM(dc.cantitate * dc.pret_unitar) AS incasari_total,
        SUM(dc.cantitate) AS bucati_total
    FROM Edituri e
    JOIN Carti c ON e.id_editura = c.id_editura
    JOIN DetaliiComanda dc ON c.id_carte = dc.id_carte
    JOIN Comenzi co ON dc.id_comanda = co.id_comanda
    WHERE 1=1
";
                        if (hasD1) sql += " AND co.data_comanda >= @d1";
                        if (hasD2) sql += " AND co.data_comanda <= @d2";

                        sql += @"
    GROUP BY e.id_editura, e.denumire
) AS t
ORDER BY t.incasari_total DESC";

                        cmd = new SqlCommand(sql, con);
                        if (hasD1) cmd.Parameters.AddWithValue("@d1", d1.Date);
                        if (hasD2) cmd.Parameters.AddWithValue("@d2", d2.Date);
                        Bind(cmd);
                        return;

                    // COMPLEX #4: Clienti fara comenzi (folosind subcerere cu COUNT)
                    case "C4":
                        sql = @"
SELECT
    cl.id_client,
    cl.nume,
    cl.prenume,
    cl.email
FROM Clienti cl
LEFT JOIN (
    SELECT
        co.id_client,
        COUNT(*) AS nr_comenzi
    FROM Comenzi co
    GROUP BY co.id_client
) AS x ON cl.id_client = x.id_client
WHERE ISNULL(x.nr_comenzi, 0) = 0
ORDER BY cl.nume, cl.prenume";

                        cmd = new SqlCommand(sql, con);
                        Bind(cmd);
                        return;

                    default:
                        throw new Exception("Raport necunoscut.");
                }
            }
        }

        private void Bind(SqlCommand cmd)
        {
            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                cmd.Connection = con;
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvReport.DataSource = dt;
                    gvReport.DataBind();

                    lblInfo.Text = "Randuri: " + dt.Rows.Count;
                    lblMsg.CssClass = "text-success mt-3 d-block";
                    lblMsg.Text = "Raport rulat cu succes.";
                }
            }
        }
    }
}
