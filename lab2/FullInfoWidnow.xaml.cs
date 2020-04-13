using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace lab2
{
    /// <summary>
    /// Interaction logic for ChangesWindow.xaml
    /// </summary>
    public partial class FullInfoWindow : Window
    {
        public Threat CurThreat { get; set; }

        public FullInfoWindow()
        {
            InitializeComponent();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            ThreatIdField.Text          = String.Concat("УБИ.", CurThreat.Id);
            ThreatNameField.Text        = CurThreat.Name;
            ThreatDescriptionField.Text = CurThreat.Description;
            ThreatSourceField.Text      = CurThreat.Source;
            ObjectOfInfluenceFiled.Text = CurThreat.ImpactObject;
            ConfidentialityField.Text   = CurThreat.isBrokenConfidentiality ? "да" : "нет";
            IntegrityField.Text         = CurThreat.isBrokenIntegrity       ? "да" : "нет";
            AvailobilityField.Text      = CurThreat.isBrokenAvailobility    ? "да" : "нет";

            if (CurThreat.Id == 0) 
            {
                SoundPlayer sp = new SoundPlayer(lab2.Properties.Resources._1);
                sp.Play(); 
            }

            base.OnContentRendered(e);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); 
        }
    }
}
