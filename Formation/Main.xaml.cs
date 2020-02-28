using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;

namespace Formation
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class Main : Window
    {
        private string _user;
        private SqlConnection _con;
        private SqlCommand _command;
        private SqlDataReader _reader;
        private List<Utilisateur> userList;
        private string _query;
        
        public Main(string user, SqlConnection con)
        {
            _user = user;
            _con = con;
            _query = "SELECT * FROM Utilisateurs";
            _con.Open();
            using (_command = new SqlCommand(_query, _con)) { 
                _reader = _command.ExecuteReader();
                userList = new List<Utilisateur>();
                while (_reader.Read())
                {
                    userList.Add(
                        new Utilisateur(
                            _reader[1].ToString(),
                            _reader[2].ToString(),
                            DateTime.Parse(_reader[3].ToString()),
                            (Char)_reader[4]
                            )
                        );
                }
                _reader.Close();

            }
            _con.Close();
            //list.Items.Add();
            InitializeComponent();
            currentUser.Text += " " + _user;
        }

        private void ajouterUtilisateur(Utilisateur user)
        {
            _query = "INSERT INTO Utilisateur VALUES(@nom,@prenom,@dateNaiss,@sexe)";
            _con.Open();
            using(_command = new SqlCommand(_query, _con))
            {
                _command.CommandType = CommandType.Text;
                _command.Parameters.AddWithValue("@nom",user.Nom);
                _command.Parameters.AddWithValue("@prenom", user.Prenom);
                _command.Parameters.AddWithValue("@dateNaiss", user.Date);
                _command.Parameters.AddWithValue("@sexe", user.Sexe);
                _command.ExecuteNonQuery();
            }
            _con.Close();
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            char selectedSexe;

            if ((bool)homme.IsChecked) selectedSexe = 'h';
            else selectedSexe = 'f';

            Utilisateur u = new Utilisateur(nom.Text, prenom.Text, (DateTime)date.SelectedDate, selectedSexe);
            try
            {
                ajouterUtilisateur(u);
                MessageBox.Show("Ajout de l'utilisateur effectué avec succès !");
            }
            catch(Exception exc)
            {
                MessageBox.Show("Oups ! l'erreur suivante s'est produite :\n" + exc.ToString());
            }
        }
    }
}
