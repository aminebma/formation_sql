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
        private Utilisateur _selectedUser;
        private string _query;
        
        public Main(string user, SqlConnection con)
        {
            InitializeComponent();
            _user = user;
            _con = con;
            _query = "SELECT * FROM Utilisateur";
            if(_con.State!= ConnectionState.Open) _con.Open();
            using (_command = new SqlCommand(_query, _con)) { 
                _reader = _command.ExecuteReader();
                while (_reader.Read())
                {
                    list.Items.Add(
                        new Utilisateur(
                            _reader[1].ToString(),
                            _reader[2].ToString(),
                            DateTime.Parse(_reader[3].ToString()),
                            _reader[4].ToString()
                            )
                        );
                }
                _reader.Close();

            }
            _con.Close();
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
            list.Items.Add(user);
        }

        private void modifierUtilisateur(Utilisateur user)
        {
            _query = "UPDATE Utilisateur SET nom=@nomN, prenom=@prenomN, dateNaiss=@dateN, sexe=@sexeN" +
                " WHERE nom=@nomA and prenom=@prenomA and dateNaiss=@dateA and sexe=@sexeA";
            _con.Open();
            using (_command = new SqlCommand(_query, _con))
            {
                _command.CommandType = CommandType.Text;
                _command.Parameters.AddWithValue("@nomN", user.Nom);
                _command.Parameters.AddWithValue("@prenomN", user.Prenom);
                _command.Parameters.AddWithValue("@dateN", user.Date);
                _command.Parameters.AddWithValue("@sexeN", user.Sexe);
                _command.Parameters.AddWithValue("@nomA", _selectedUser.Nom);
                _command.Parameters.AddWithValue("@prenomA", _selectedUser.Prenom);
                _command.Parameters.AddWithValue("@dateA", _selectedUser.Date);
                _command.Parameters.AddWithValue("@sexeA", _selectedUser.Sexe);
                _command.ExecuteNonQuery();
            }
            _con.Close();
            list.Items.RemoveAt(list.SelectedIndex);
            list.Items.Add(user);
        }

        private void supprimerUtilisateur(Utilisateur user)
        {
            _query = "DELETE FROM Utilisateur WHERE nom=@nomU and prenom=@prenomU and dateNaiss=@dateN and sexe=@sexeU";
            _con.Open();
            using (_command = new SqlCommand(_query, _con))
            {
                _command.CommandType = CommandType.Text;
                _command.Parameters.AddWithValue("@nomU", user.Nom);
                _command.Parameters.AddWithValue("@prenomU", user.Prenom);
                _command.Parameters.AddWithValue("@dateN", user.Date);
                _command.Parameters.AddWithValue("@sexeU", user.Sexe);
                _command.ExecuteNonQuery();
            }
            _con.Close();
            list.Items.RemoveAt(list.SelectedIndex);
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            string selectedSexe;

            if ((bool)homme.IsChecked) selectedSexe = "h";
            else selectedSexe = "f";

            Utilisateur u = new Utilisateur(nom.Text, prenom.Text, (DateTime)date.SelectedDate, selectedSexe);
            try
            {
                ajouterUtilisateur(u);
                MessageBox.Show("Ajout de l'utilisateur effectué avec succès !");
            }
            catch(Exception exc)
            {
                if (_con.State == ConnectionState.Open) _con.Close();
                MessageBox.Show("Oups ! l'erreur suivante s'est produite :\n" + exc.ToString());
            }
        }

        private void deco_Click(object sender, RoutedEventArgs e)
        {
            Connexion connexion = new Connexion();
            connexion.Show();
            this.Close();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Bye Fellas ! ;D");
            Application.Current.Shutdown();
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try 
            {
                _selectedUser = (Utilisateur)list.SelectedItem;
                nom.Text = _selectedUser.Nom;
                prenom.Text = _selectedUser.Prenom;
                date.SelectedDate = _selectedUser.Date;
                if (_selectedUser.Sexe == "h") homme.IsChecked = true;
                else femme.IsChecked = true;
            }
            catch
            {
                e.Handled = true;
            }
        }

        private void modify_Click(object sender, RoutedEventArgs e)
        {
            string selectedSexe;
            if ((bool)homme.IsChecked) selectedSexe = "h";
            else selectedSexe = "f";
            if ((_selectedUser!=null) && (nom.Text != _selectedUser.Nom || prenom.Text != _selectedUser.Prenom
                    || date.SelectedDate != _selectedUser.Date || selectedSexe != _selectedUser.Sexe))
            {
                _selectedUser.Nom = nom.Text;
                _selectedUser.Prenom = prenom.Text;
                _selectedUser.Date = (DateTime)date.SelectedDate;
                _selectedUser.Sexe = selectedSexe;
                try
                {
                    modifierUtilisateur(_selectedUser);
                    MessageBox.Show("Modification de l'utilisateur effectuée avec succès !");
                }
                catch (Exception exc)
                {
                    if (_con.State == ConnectionState.Open) _con.Close();
                    MessageBox.Show("Oups ! l'erreur suivante s'est produite :\n" + exc.ToString());
                }
            }
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {         
            try
            {
                supprimerUtilisateur(_selectedUser);
                MessageBox.Show("Suppression de l'utilisateur effectuée avec succès !");
            }
            catch (Exception exc)
            {
                if (_con.State == ConnectionState.Open) _con.Close();
                MessageBox.Show("Oups ! l'erreur suivante s'est produite :\n" + exc.ToString());
            }
        }
    }
}
