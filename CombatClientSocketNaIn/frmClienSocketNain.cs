using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CombatClientSocketNaIn.Classes;


namespace CombatClientSocketNaIn
{
    public partial class frmClienSocketNain : Form
    {
        Random m_r;
        Elfe m_elfe;
        Nain m_nain;
        public frmClienSocketNain()
        {
            InitializeComponent();
            m_r = new Random();
            Reset();
            btnReset.Enabled = false;
            Control.CheckForIllegalCrossThreadCalls = false;
        }
        void Reset()
        {
            m_nain = new Nain(m_r.Next(10, 20), m_r.Next(2, 6), m_r.Next(0, 3));
            picNain.Image = m_nain.Avatar;
            lblVieNain.Text = "Vie: " + m_nain.Vie.ToString(); ;
            lblForceNain.Text = "Force: " + m_nain.Force.ToString();
            lblArmeNain.Text = "Arme: " + m_nain.Arme;

            m_elfe = new Elfe(1, 0, 0);
            picElfe.Image = m_elfe.Avatar;
            lblVieElfe.Text = "Vie: " + m_elfe.Vie.ToString();
            lblForceElfe.Text = "Force: " + m_elfe.Force.ToString();
            lblSortElfe.Text = "Sort: " + m_elfe.Sort.ToString();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            btnFrappe.Enabled = true;
            Reset();
        }

        private void btnFrappe_Click(object sender, EventArgs e)
        {
            Socket client;
            btnFrappe.Enabled = false;
            try
            {
                string send;
                byte[] tByteEnvoie;
                UTF8Encoding textByte = new UTF8Encoding();
                int nbOctetReception;
                byte[] tByteReceptionClient = new byte[100];
                string reponse;
                string[] reception;
                client = new Socket(SocketType.Stream, ProtocolType.Tcp);
                client.Connect(IPAddress.Parse("127.0.0.1"), 8888);
                MessageBox.Show("Assurez vous que le serveur est demarer");
                if (client.Connected)
                {
                    // envoie les données du nain sous cette forme: 
                    // vieNain;forceNain;armeNain;

                    send = m_nain.Vie.ToString()+";"+ m_nain.Force.ToString() + ";" + m_nain.Arme.ToString()+";";


                    MessageBox.Show("Client: \r\nTransmet..." + send);

                    tByteEnvoie = textByte.GetBytes(send);

                    // transmission
                    client.Send(tByteEnvoie);

                    Thread.Sleep(500);

                    // réception 
                    // reçoit les données sous cette forme: 
                    // vieNain;forceNain;armeNain;vieElfe;forceElfe;sortElfe;

                    MessageBox.Show("Client: réception de données serveur");

                    nbOctetReception = client.Receive(tByteReceptionClient);

                    reponse = Encoding.ASCII.GetString(tByteReceptionClient);
                    MessageBox.Show("\r\nReception..." + reponse);
                    reception = reponse.Split(';', '|');
                    m_nain.Vie = Int32.Parse(reception[0]);
                    m_nain.Force = Int32.Parse(reception[1]);
                    m_nain.Arme = reception[2];
                    m_elfe.Vie = Int32.Parse(reception[3]);
                    m_elfe.Force = Int32.Parse(reception[4]);
                    m_elfe.Sort = Int32.Parse(reception[5]);
                    //mettre a jour les labels
                    lblVieElfe.Text = "Vie: " + m_elfe.Vie.ToString();
                    lblForceElfe.Text = "Force: " + m_elfe.Force.ToString();
                    lblSortElfe.Text = "Sort: " + m_elfe.Sort.ToString();
                    lblVieNain.Text = "Vie: " + m_nain.Vie.ToString(); ;
                    lblForceNain.Text = "Force: " + m_nain.Force.ToString();
                    lblArmeNain.Text = "Arme: " + m_nain.Arme;
                }
                client.Close();
                if (m_elfe.Vie <= 0)
                {
                    MessageBox.Show("Nain gagnant");
                }
                else
                {
                    MessageBox.Show("Elfe gagnant");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            btnFrappe.Enabled = true;

        }
    }
}
