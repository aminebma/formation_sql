using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formation
{
    class Utilisateur
    {
        private string _nom;
        public string Nom { get => _nom; set => _nom = value; }

        private string _prenom;
        public string Prenom { get => _prenom; set => _prenom = value; }

        private DateTime _date;
        public DateTime Date { get => _date; set => _date = value; }

        private Char _sexe;
        public Char Sexe { get => _sexe; set => _sexe = value; }

        public Utilisateur(string nom, string prenom, DateTime date, char sexe)
        {
            this._nom = nom ?? throw new ArgumentNullException(nameof(nom));
            this._prenom = prenom ?? throw new ArgumentNullException(nameof(prenom));
            this._date = date;
            this._sexe = sexe;
        }

    }
}
