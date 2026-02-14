using System;
using System.Data.SqlClient;
using System.Web.UI;

namespace EdituraWeb
{
    public partial class Login : System.Web.UI.Page
    {
        // login
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string user = txtUser.Text.Trim();
            string pass = txtPass.Text.Trim();

            string connStr = System.Configuration.ConfigurationManager
                .ConnectionStrings["EdituraDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
    "SELECT id_utilizator, rol FROM Utilizatori WHERE username=@u AND parola=@p", con);
                cmd.Parameters.AddWithValue("@u", user);
                cmd.Parameters.AddWithValue("@p", pass);

                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        int idUtilizator = Convert.ToInt32(rdr["id_utilizator"]);
                        string rol = rdr["rol"].ToString();

                        Session["rol"] = rol;
                        Session["id_utilizator"] = idUtilizator;

                        rdr.Close();

                        if (rol == "client")
                        {
                            // luam id_client
                            SqlCommand cmdClient = new SqlCommand(
                                "SELECT id_client FROM Clienti WHERE id_utilizator=@id", con);
                            cmdClient.Parameters.AddWithValue("@id", idUtilizator);

                            object clientIdObj = cmdClient.ExecuteScalar();
                            if (clientIdObj != null)
                                Session["id_client"] = Convert.ToInt32(clientIdObj);
                        }

                        if (rol == "admin") Response.Redirect("AdminPage.aspx");
                        else if (rol == "editor") Response.Redirect("EditorPage.aspx");
                        else Response.Redirect("ClientPage.aspx");
                    }
                    else
                    {
                        lblError.Text = "Username sau parola incorecte!";
                    }
                }

            }
        }
        // redirectionare pagina creare cont
        protected void btnCreateAccount_Click(object sender, EventArgs e)
        {
            Response.Redirect("RegisterClient.aspx");
        }
    }
}
