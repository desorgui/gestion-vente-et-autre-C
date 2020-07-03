using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using ProjetInterface;

namespace UI
{
    public partial class FenEffectuerPret : Form
    {
        public void fincanal()
        {
            try
            {
                ChannelServices.UnregisterChannel(CADECH);
            }
            catch (Exception)
            { }
        }
        public TcpChannel CADECH = null;
        public ProjetInterface.InterfaceUtilisateur ut;
        public ProjetInterface.InterfaceCompte cl;
        public ProjetInterface.InterfaceCompte c1;
        public ProjetInterface.InterfacePret p0;
        bool erreurs = false;
        public FenEffectuerPret()
        {
            p0 = (ProjetInterface.InterfacePret)Activator.GetObject(typeof(ProjetInterface.InterfacePret), ServiceTechnique.Services.getConfiguration() + "ObjetPret");
            ut = (ProjetInterface.InterfaceUtilisateur)Activator.GetObject(typeof(ProjetInterface.InterfaceUtilisateur), ServiceTechnique.Services.getConfiguration() + "objetUtilisateur");
            c1 = (ProjetInterface.InterfaceCompte)Activator.GetObject(typeof(ProjetInterface.InterfaceCompte), ServiceTechnique.Services.getConfiguration() + "objCompte");
            InitializeComponent();
        }

        private void btnannuler_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnenregistrer_Click(object sender, EventArgs e)
        {
            Random nombre_aleatoire = new Random();
            string numpret = "pr-" + nombre_aleatoire.Next(10000, 90000);
            //string etat = "En cours";
            //decimal penalites = 0;
            DateTime datepret = DateTime.Now;
            string datepreter = datepret.ToString("MM/dd/yyyy");

            if (!msknumCompte.MaskFull)
            {
                MessageBox.Show("Remplissez le champ");
                btnenregistrer.Enabled = false;
            }
            else
                if (txtmontant.Text == "0")
                {
                    MessageBox.Show("entrez un Montant");
                }
                else if (txtmois.Text.Equals("0"))
                {
                    // Erreur.SetError(txtmontant, "");
                    MessageBox.Show("entrez le nombre de mois");
                }
                else
                {
                    //Erreur.SetError(txtmois, "");
                    if (ServiceTechnique.Services.IsServeurOn())
                    {
                        if (p0.recherchePret(msknumCompte.Text.Trim()))
                        {
                            lblmessage.Text = "ce Client a deja un pret sur";
                        }
                        else
                        {
                            decimal balance=0;
                            decimal totalsolde = 0;
                            foreach (decimal mont in c1.listesolde(msknumCompte.Text.Trim()))
                            {
                                totalsolde += mont;
                            }
                            DateTime datedelai = datepret.AddMonths(int.Parse(txtmois.Text));
                            string delai = datedelai.ToString("MM/dd/yyyy");
                            decimal interet = (decimal.Parse(txtmontant.Text) * 20) / 100;
                            balance = decimal.Parse(txtmontant.Text) + interet;
                            decimal taux = (decimal.Parse(txtmontant.Text) * 25) / 100;
                            if (totalsolde < taux)
                            {
                                lblmessage.Text = "Vous ne pouvez pas effectuer ce pret \n " +
                            " vous devez avoir 25% du montant demande apres le cumul de vos compte"; 

                            }
                            else
                            {
                                if (p0.EnregistrerPret(numpret, msknumCompte.Text.Trim(), Decimal.Parse(txtmontant.Text), balance, datepret, delai) == false)
                                {
                                    lblmessage.Text = "echec";
                                }
                                else
                                {
                                    lblmessage.Text = "Pret effectuer avec succes";
                                    msknumCompte.Clear();
                                    txtmois.Value = 0;
                                    txtmontant.Value = 0;
                                    string operation = "effectuer pret :" + numpret;
                                    ut.Trace(FenPrincipale.user_online, operation, FenPrincipale.date_online, FenPrincipale.heure_online, FenPrincipale.pc_online);


                                }
                            }

                        }
                    }
                    else
                    {

                        System.Media.SystemSounds.Beep.Play();
                        MessageBox.Show("        Serveur Indisponible    !!!");
                        //this.BeginInvoke(new MethodInvoker(this.Close));
                    }



                }
        }

        private void txtmontant_Click(object sender, EventArgs e)
        {

        }

        private void txtmontant_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
