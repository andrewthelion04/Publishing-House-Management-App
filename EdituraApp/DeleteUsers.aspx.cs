using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace EdituraWeb
{
    public partial class DeleteUsers : System.Web.UI.Page
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
                lblWelcome.Text = "Salut, admin!";
                LoadRoleFilter();
                LoadUsers(); // foloseste filtrul curent
                ResetSelectionUI();
            }
        }

        // filtru pe baza de rol
        private void LoadRoleFilter()
        {
            ddlRoleFilter.Items.Clear();
            ddlRoleFilter.Items.Add(new ListItem("Toti utilizatorii", "all"));
            ddlRoleFilter.Items.Add(new ListItem("Clienti", "client"));
            ddlRoleFilter.Items.Add(new ListItem("Editori", "editor"));
            ddlRoleFilter.Items.Add(new ListItem("Admini", "admin"));

            ddlRoleFilter.SelectedValue = "all";
        }

        // aplicare filtru
        protected void btnApplyFilter_Click(object sender, EventArgs e)
        {
            LoadUsers();
            ResetSelectionUI();

            lblMsg.CssClass = "text-success mt-3 d-block";
            lblMsg.Text = "Filtru aplicat.";
        }

        // resetare filtru
        protected void btnResetFilter_Click(object sender, EventArgs e)
        {
            ddlRoleFilter.SelectedValue = "all";
            LoadUsers();
            ResetSelectionUI();

            lblMsg.CssClass = "text-success mt-3 d-block";
            lblMsg.Text = "Filtru resetat.";
        }

        // logout

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }

        // refresh

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadUsers();
            ResetSelectionUI();
            lblMsg.CssClass = "text-success mt-3 d-block";
            lblMsg.Text = "Lista a fost reîncărcată.";
        }

        // atentionare lipsa selectie utilizator
        private void ResetSelectionUI()
        {
            btnDelete.Enabled = false;
            lblSelected.Text = "Niciun utilizator selectat.";
            lblMsg.CssClass = "mt-3 d-block";
        }

        // incarcare useri
        private void LoadUsers()
        {
            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;
            string filter = ddlRoleFilter.SelectedValue;

            string sql = "SELECT id_utilizator, username, rol FROM Utilizatori";
            if (filter != "all")
                sql += " WHERE rol = @rol";
            sql += " ORDER BY rol, username";

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                if (filter != "all")
                    cmd.Parameters.AddWithValue("@rol", filter);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvUsers.DataSource = dt;
                    gvUsers.DataBind();

                    lblCount.Text = "Randuri: " + dt.Rows.Count;
                }
            }
        }

        // handler pentru evenimentul SelectedIndexChanged
        protected void gvUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(gvUsers.SelectedDataKey.Value);

            btnDelete.Enabled = true;

            // username si rol din randul selectat
            string username = gvUsers.SelectedRow.Cells[1].Text;
            string rol = gvUsers.SelectedRow.Cells[2].Text;

            lblSelected.Text = $"Selectat: <b>{username}</b> (rol: <b>{rol}</b>, id: <b>{userId}</b>)";

            lblMsg.CssClass = "text-primary mt-3 d-block";
            lblMsg.Text = "Apasă Delete pentru a șterge utilizatorul selectat.";
        }

        // delete utilizator
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (gvUsers.SelectedDataKey == null)
            {
                lblMsg.CssClass = "text-danger mt-3 d-block";
                lblMsg.Text = "Selectează mai întâi un utilizator.";
                return;
            }

            int userId = Convert.ToInt32(gvUsers.SelectedDataKey.Value);
            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    using (SqlTransaction tr = con.BeginTransaction())
                    {
                        // 1) rol
                        SqlCommand getRole = new SqlCommand(
                            "SELECT rol FROM Utilizatori WHERE id_utilizator=@id", con, tr);
                        getRole.Parameters.AddWithValue("@id", userId);

                        object roleObj = getRole.ExecuteScalar();
                        if (roleObj == null)
                        {
                            tr.Rollback();
                            lblMsg.CssClass = "text-danger mt-3 d-block";
                            lblMsg.Text = "Utilizatorul nu mai există.";
                            LoadUsers();
                            ResetSelectionUI();
                            return;
                        }

                        string role = roleObj.ToString();

                        // 2) nu stergem admin
                        if (role == "admin")
                        {
                            tr.Rollback();
                            lblMsg.CssClass = "text-danger mt-3 d-block";
                            lblMsg.Text = "Nu poți șterge un utilizator cu rol ADMIN!";
                            btnDelete.Enabled = false;
                            return;
                        }

                        // 3) daca e client, stergem: DetaliiComanda -> Comenzi -> Clienti
                        SqlCommand getClientId = new SqlCommand(
                            "SELECT id_client FROM Clienti WHERE id_utilizator=@id", con, tr);
                        getClientId.Parameters.AddWithValue("@id", userId);

                        object clientIdObj = getClientId.ExecuteScalar();
                        bool isClient = (clientIdObj != null && clientIdObj != DBNull.Value);

                        if (isClient)
                        {
                            int idClient = Convert.ToInt32(clientIdObj);

                            SqlCommand delDetails = new SqlCommand(@"
DELETE dc
FROM DetaliiComanda dc
JOIN Comenzi co ON dc.id_comanda = co.id_comanda
WHERE co.id_client = @cid", con, tr);
                            delDetails.Parameters.AddWithValue("@cid", idClient);
                            delDetails.ExecuteNonQuery();

                            SqlCommand delOrders = new SqlCommand(
                                "DELETE FROM Comenzi WHERE id_client=@cid", con, tr);
                            delOrders.Parameters.AddWithValue("@cid", idClient);
                            delOrders.ExecuteNonQuery();

                            SqlCommand delClient = new SqlCommand(
                                "DELETE FROM Clienti WHERE id_client=@cid", con, tr);
                            delClient.Parameters.AddWithValue("@cid", idClient);
                            delClient.ExecuteNonQuery();
                        }

                        // 4) sterge utilizatorul (client/editor)
                        SqlCommand delUser = new SqlCommand(
                            "DELETE FROM Utilizatori WHERE id_utilizator=@id", con, tr);
                        delUser.Parameters.AddWithValue("@id", userId);
                        delUser.ExecuteNonQuery();

                        tr.Commit();
                    }
                }

                LoadUsers();
                btnDelete.Enabled = false;
                lblSelected.Text = "Niciun utilizator selectat.";

                lblMsg.CssClass = "text-success mt-3 d-block";
                lblMsg.Text = "Utilizator șters cu succes (și datele asociate, dacă era client).";
            }
            catch (Exception ex)
            {
                lblMsg.CssClass = "text-danger mt-3 d-block";
                lblMsg.Text = "Eroare: " + ex.Message;
            }
        }
    }
}
