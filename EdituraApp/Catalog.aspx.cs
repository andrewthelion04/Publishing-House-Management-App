using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace EdituraWeb
{
    public partial class Catalog : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // optional: protejeaza pagina pentru clienti
            if (Session["rol"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadAutori();
                LoadEdituri();
                LoadCatalog(); // fara filtre initial
            }
        }

        //incarcare autori
        private void LoadAutori()
        {
            ddlAutori.Items.Clear();
            ddlAutori.Items.Add(new System.Web.UI.WebControls.ListItem("Toti autorii", "0"));

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
                        string val = rdr["id_autor"].ToString();
                        ddlAutori.Items.Add(new System.Web.UI.WebControls.ListItem(text, val));
                    }
                }
            }
        }

        //incarcare edituri
        private void LoadEdituri()
        {
            ddlEdituri.Items.Clear();
            ddlEdituri.Items.Add(new System.Web.UI.WebControls.ListItem("Toate editurile", "0"));

            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT id_editura, denumire FROM Edituri ORDER BY denumire", con))
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        ddlEdituri.Items.Add(new System.Web.UI.WebControls.ListItem(
                            rdr["denumire"].ToString(),
                            rdr["id_editura"].ToString()
                        ));
                    }
                }
            }
        }

        protected void btnApply_Click(object sender, EventArgs e)
        {
            LoadCatalog();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            ddlAutori.SelectedIndex = 0;
            ddlEdituri.SelectedIndex = 0;
            txtGen.Text = "";
            txtTitlu.Text = "";
            txtPretMin.Text = "";
            txtPretMax.Text = "";

            LoadCatalog();
        }

        // incarcarea catalogului de carti
        private void LoadCatalog()
        {
            int idAutor = 0;
            int idEditura = 0;
            decimal pretMin, pretMax;

            int.TryParse(ddlAutori.SelectedValue, out idAutor);
            int.TryParse(ddlEdituri.SelectedValue, out idEditura);

            bool hasPretMin = decimal.TryParse(txtPretMin.Text.Trim(), out pretMin);
            bool hasPretMax = decimal.TryParse(txtPretMax.Text.Trim(), out pretMax);

            string gen = txtGen.Text.Trim();
            string titlu = txtTitlu.Text.Trim();

            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;

            string sql = @"
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

            // filtre (parametri variabili)
            if (idAutor != 0) sql += " AND c.id_autor = @idAutor";
            if (idEditura != 0) sql += " AND c.id_editura = @idEditura";
            if (!string.IsNullOrWhiteSpace(gen)) sql += " AND c.gen LIKE @gen";
            if (!string.IsNullOrWhiteSpace(titlu)) sql += " AND c.titlu LIKE @titlu";
            if (hasPretMin) sql += " AND c.pret >= @pretMin";
            if (hasPretMax) sql += " AND c.pret <= @pretMax";

            sql += " ORDER BY c.titlu";

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                if (idAutor != 0) cmd.Parameters.AddWithValue("@idAutor", idAutor);
                if (idEditura != 0) cmd.Parameters.AddWithValue("@idEditura", idEditura);
                if (!string.IsNullOrWhiteSpace(gen)) cmd.Parameters.AddWithValue("@gen", "%" + gen + "%");
                if (!string.IsNullOrWhiteSpace(titlu)) cmd.Parameters.AddWithValue("@titlu", "%" + titlu + "%");
                if (hasPretMin) cmd.Parameters.AddWithValue("@pretMin", pretMin);
                if (hasPretMax) cmd.Parameters.AddWithValue("@pretMax", pretMax);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvCatalog.DataSource = dt;
                    gvCatalog.DataBind();

                    lblInfo.Text = "Rezultate: " + dt.Rows.Count;
                }
            }
        }
    }
}
