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
using System.Windows.Shapes;

namespace CalScec.DataManagment.CrystalData
{
    /// <summary>
    /// Interaktionslogik für ShowCrystalDataWindow.xaml
    /// </summary>
    public partial class ShowCrystalDataWindow : Window
    {
        public List<CODData> CrystalDataList = new List<CODData>();

        public bool ForSave = false;

        public ShowCrystalDataWindow(List<CODData> crystalDataList)
        {
            InitializeComponent();

            this.CrystalDataList = crystalDataList;
            AddDataWindow();
        }

        private void AddDataWindow()
        {
            this.CrystalDataListView.ItemsSource = CrystalDataList;

            ComboBoxItem SpaceGroupItemE = new ComboBoxItem();
            SpaceGroupItemE.Content = "";

            SymetryBox.Items.Add(SpaceGroupItemE);

            string RessourcePath = System.IO.Path.Combine(Environment.CurrentDirectory, @"Res\ReflectionConditions\SpaceGroupsHKL.xml");
            using (System.IO.StreamReader XMLResStream = new System.IO.StreamReader(RessourcePath))
            {
                using (System.Xml.XmlTextReader ReflexReader = new System.Xml.XmlTextReader(XMLResStream))
                {
                    while (ReflexReader.Read())
                    {
                        switch (ReflexReader.NodeType)
                        {
                            case System.Xml.XmlNodeType.Element:
                                if (ReflexReader.Name == "SpaceGroup")
                                {
                                    ReflexReader.Read();
                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                    {
                                        ComboBoxItem SpaceGroupItem = new ComboBoxItem();
                                        SpaceGroupItem.Content = ReflexReader.Value;

                                        SymetryBox.Items.Add(SpaceGroupItem);
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private void CrystalDataListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.CrystalDataListView.SelectedIndex != -1)
            {
                CODData SelectedCrystalData = (CODData)this.CrystalDataListView.SelectedItem;

                this.HKLListView.ItemsSource = SelectedCrystalData.HKLList;
                this.CellAText.Text = SelectedCrystalData.A.ToString("F3");
                this.CellBText.Text = SelectedCrystalData.B.ToString("F3");
                this.CellCText.Text = SelectedCrystalData.C.ToString("F3");
                this.CellAlphaText.Text = SelectedCrystalData.Alpha.ToString("F3");
                this.CellBetaText.Text = SelectedCrystalData.Beta.ToString("F3");
                this.CellGammaText.Text = SelectedCrystalData.Gamma.ToString("F3");
                this.NameText.Text = SelectedCrystalData.Name;
                this.ElementalCompositionText.Text = SelectedCrystalData.ChemicalFormula;
                this.SymetryBox.Text = SelectedCrystalData.SymmetryGroup;
            }
            else
            {
                this.HKLListView.ItemsSource = null;
                this.CellAText.Text = "";
                this.CellBText.Text = "";
                this.CellCText.Text = "";
                this.CellAlphaText.Text = "";
                this.CellBetaText.Text = "";
                this.CellGammaText.Text = "";
                this.NameText.Text = "";
                this.ElementalCompositionText.Text = "";
                this.SymetryBox.Text = "";
            }
        }

        private void HKLListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.HKLListView.SelectedIndex != -1)
            {
                HKLReflex SelectedReflex = (HKLReflex)this.HKLListView.SelectedItem;

                this.HKLHText.Text = SelectedReflex.H.ToString();
                this.HKLKText.Text = SelectedReflex.K.ToString();
                this.HKLLText.Text = SelectedReflex.L.ToString();
            }
            else
            {
                this.HKLHText.Text = "";
                this.HKLKText.Text = "";
                this.HKLLText.Text = "";
            }
        }

        private void SaveCrystalDataChanges_Click(object sender, RoutedEventArgs e)
        {
            if (this.CrystalDataListView.SelectedIndex != -1)
            {
                CODData SelectedCrystalData = (CODData)this.CrystalDataListView.SelectedItem;
                this.CrystalDataList.Remove(SelectedCrystalData);

                if (SelectedCrystalData.A.ToString("F3") != this.CellAText.Text)
                {
                    try
                    {
                        double NewCellA = Convert.ToDouble(this.CellAText.Text);
                        SelectedCrystalData.A = NewCellA;
                    }
                    catch
                    {
                        MessageBox.Show("MMMh you tried to change cell parameter A but something went terribly wrong! You should try that again and don't forget sepperator is \".\" not \",\"", "Some mistake in cell parameter A", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                if (SelectedCrystalData.B.ToString("F3") != this.CellBText.Text)
                {
                    try
                    {
                        double NewCellB = Convert.ToDouble(this.CellBText.Text);
                        SelectedCrystalData.B = NewCellB;
                    }
                    catch
                    {
                        MessageBox.Show("MMMh you tried to change cell parameter B but something went terribly wrong! You should try that again and don't forget sepperator is \".\" not \",\"", "Some mistake in cell parameter B", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                if (SelectedCrystalData.C.ToString("F3") != this.CellCText.Text)
                {
                    try
                    {
                        double NewCellC = Convert.ToDouble(this.CellCText.Text);
                        SelectedCrystalData.C = NewCellC;
                    }
                    catch
                    {
                        MessageBox.Show("MMMh you tried to change cell parameter C but something went terribly wrong! You should try that again and don't forget sepperator is \".\" not \",\"", "Some mistake in cell parameter C", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                if (SelectedCrystalData.Alpha.ToString("F3") != this.CellAlphaText.Text)
                {
                    try
                    {
                        double NewCellAlpha = Convert.ToDouble(this.CellAlphaText.Text);
                        SelectedCrystalData.Alpha = NewCellAlpha;
                    }
                    catch
                    {
                        MessageBox.Show("MMMh you tried to change cell parameter Alpha but something went terribly wrong! You should try that again and don't forget sepperator is \".\" not \",\"", "Some mistake in cell parameter Alpha", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                if (SelectedCrystalData.Beta.ToString("F3") != this.CellBetaText.Text)
                {
                    try
                    {
                        double NewCellBeta = Convert.ToDouble(this.CellBetaText.Text);
                        SelectedCrystalData.Beta = NewCellBeta;
                    }
                    catch
                    {
                        MessageBox.Show("MMMh you tried to change cell parameter Beta but something went terribly wrong! You should try that again and don't forget sepperator is \".\" not \",\"", "Some mistake in cell parameter Beta", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                if (SelectedCrystalData.Gamma.ToString("F3") != this.CellGammaText.Text)
                {
                    try
                    {
                        double NewCellGamma = Convert.ToDouble(this.CellGammaText.Text);
                        SelectedCrystalData.Gamma = NewCellGamma;
                    }
                    catch
                    {
                        MessageBox.Show("MMMh you tried to change cell parameter Gamma but something went terribly wrong! You should try that again and don't forget sepperator is \".\" not \",\"", "Some mistake in cell parameter Gamma", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                if (SelectedCrystalData.Name != this.NameText.Text)
                {
                    SelectedCrystalData.Name = this.NameText.Text;
                }
                if (SelectedCrystalData.SymmetryGroup != this.SymetryBox.Text)
                {
                    SelectedCrystalData.SymmetryGroup = this.SymetryBox.Text;
                }
                if (SelectedCrystalData.ChemicalFormula != this.ElementalCompositionText.Text)
                {
                    bool Searching = true;
                    string ElementalCompositionPrepare = this.ElementalCompositionText.Text;

                    while(Searching)
                    {
                        if(ElementalCompositionPrepare[0] == '-')
                        {
                            ElementalCompositionPrepare = ElementalCompositionPrepare.Remove(0, 1);
                        }
                        else if(ElementalCompositionPrepare[0] == ' ')
                        {
                            ElementalCompositionPrepare = ElementalCompositionPrepare.Remove(0, 1);
                        }
                        else
                        {
                            Searching = false;
                        }
                    }

                    Searching = true;

                    while (Searching)
                    {
                        if (ElementalCompositionPrepare[ElementalCompositionPrepare.Length - 1] == '-')
                        {
                            ElementalCompositionPrepare = ElementalCompositionPrepare.Remove(ElementalCompositionPrepare.Length - 1, 1);
                        }
                        else if (ElementalCompositionPrepare[ElementalCompositionPrepare.Length - 1] == ' ')
                        {
                            ElementalCompositionPrepare = ElementalCompositionPrepare.Remove(ElementalCompositionPrepare.Length - 1, 1);
                        }
                        else
                        {
                            Searching = false;
                        }
                    }

                    SelectedCrystalData.ChemicalFormula = ElementalCompositionPrepare;
                }

                SelectedCrystalData.HKLList = new List<DataManagment.CrystalData.HKLReflex>();
                Tools.Calculation.AddHKLList(SelectedCrystalData);

                this.CrystalDataList.Add(SelectedCrystalData);

                this.CrystalDataListView.Items.Refresh();
                this.HKLListView.Items.Refresh();
            }
            else
            {
                MessageBox.Show("If you want to make changes to the crystal data, you have to select one at least! I cannot read your mind!", "I am not a mind reader!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HKLListView_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Delete)
            {
                if (this.CrystalDataListView.SelectedIndex != -1)
                {
                    CODData SelectedCrystalData = (CODData)this.CrystalDataListView.SelectedItem;

                    if(this.HKLListView.SelectedIndex != -1)
                    {
                        HKLReflex SelectedReflex = (HKLReflex)this.HKLListView.SelectedItem;

                        SelectedCrystalData.HKLList.Remove(SelectedReflex);
                        this.HKLListView.Items.Refresh();
                    }
                }
            }
        }

        private void CrystalDataListView_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Delete)
            {
                if (this.CrystalDataListView.SelectedIndex != -1)
                {
                    MessageBoxResult MR = MessageBox.Show("Do you really want to delete this crystal data????", "Are you really sure????", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
                    if (MR == MessageBoxResult.Yes)
                    {
                        CODData SelectedCrystalData = (CODData)this.CrystalDataListView.SelectedItem;

                        this.CrystalDataList.Remove(SelectedCrystalData);
                        this.CrystalDataListView.Items.Refresh();
                    }
                }
            }
        }

        private void ModifyHKLButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.CrystalDataListView.SelectedIndex != -1)
            {
                if (HKLListView.SelectedIndex != -1)
                {
                    CODData SelectedCrystalData = (CODData)this.CrystalDataListView.SelectedItem;
                    HKLReflex SelectedReflex = (HKLReflex)this.HKLListView.SelectedItem;

                    bool Changed = false;
                    int NewH = SelectedReflex.H;
                    int NewK = SelectedReflex.K;
                    int NewL = SelectedReflex.L;

                    if(SelectedReflex.H.ToString() != this.HKLHText.Text)
                    {
                        try
                        {
                            NewH = Convert.ToInt32(this.HKLHText.Text);
                            Changed = true;
                        }
                        catch
                        {
                            MessageBox.Show("You need to type an integer as H!!! The old value will be used.", "Very bad format!", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    if (SelectedReflex.K.ToString() != this.HKLKText.Text)
                    {
                        try
                        {
                            NewK = Convert.ToInt32(this.HKLKText.Text);
                            Changed = true;
                        }
                        catch
                        {
                            MessageBox.Show("You need to type an integer as K!!! The old value will be used.", "Very bad format!", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    if (SelectedReflex.L.ToString() != this.HKLLText.Text)
                    {
                        try
                        {
                            NewL = Convert.ToInt32(this.HKLLText.Text);
                            Changed = true;
                        }
                        catch
                        {
                            MessageBox.Show("You need to type an integer as L!!! The old value will be used.", "Very bad format!", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }

                    foreach (HKLReflex Rx in SelectedCrystalData.HKLList)
                    {
                        if(Rx.H == NewH && Rx.K == NewK && Rx.L == NewL)
                        {
                            Changed = false;
                            break;
                        }
                    }

                    if (Changed)
                    {
                        HKLReflex ModifiedReflex = new HKLReflex(NewH, NewK, NewL, Tools.Calculation.CalculateHKLDistance(NewH, NewK, NewL, SelectedCrystalData));

                        SelectedCrystalData.HKLList.Remove(SelectedReflex);
                        SelectedCrystalData.HKLList.Add(ModifiedReflex);

                        SelectedCrystalData.HKLList.Sort((A, B) => (1) * (A.H + A.K + A.L).CompareTo(B.H + B.K + B.L));
                        this.HKLListView.Items.Refresh();
                    }
                    else
                    {
                        MessageBox.Show("I just found a reflex which is identical to that you wanted to add! I am pretty sure you don't want to add it twice.","No chnages detected!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("You need to select a HKL reflex in order to modify it!!!", "No Reflex selected!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AddNewHKLButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.CrystalDataListView.SelectedIndex != -1)
            {
                CODData SelectedCrystalData = (CODData)this.CrystalDataListView.SelectedItem;

                bool Changed = false;
                int NewH = 0;
                int NewK = 0;
                int NewL = 0;

                if (this.HKLHText.Text != "")
                {
                    try
                    {
                        NewH = Convert.ToInt32(this.HKLHText.Text);
                        Changed = true;
                    }
                    catch
                    {
                        MessageBox.Show("You need to type an integer as H!!! 0 will be used.", "Very bad format!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                if (this.HKLKText.Text != "")
                {
                    try
                    {
                        NewK = Convert.ToInt32(this.HKLKText.Text);
                        Changed = true;
                    }
                    catch
                    {
                        MessageBox.Show("You need to type an integer as K!!! 0 will be used.", "Very bad format!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                if (this.HKLLText.Text != "")
                {
                    try
                    {
                        NewL = Convert.ToInt32(this.HKLLText.Text);
                        Changed = true;
                    }
                    catch
                    {
                        MessageBox.Show("You need to type an integer as L!!! 0 will be used.", "Very bad format!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                if (!(NewH == 0 && NewK == 0 && NewL == 0))
                {
                    foreach (HKLReflex Rx in SelectedCrystalData.HKLList)
                    {
                        if (Rx.H == NewH && Rx.K == NewK && Rx.L == NewL)
                        {
                            Changed = false;
                            break;
                        }
                    }

                    if (Changed)
                    {
                        HKLReflex ModifiedReflex = new HKLReflex(NewH, NewK, NewL, Tools.Calculation.CalculateHKLDistance(NewH, NewK, NewL, SelectedCrystalData));

                        SelectedCrystalData.HKLList.Add(ModifiedReflex);

                        SelectedCrystalData.HKLList.Sort((A, B) => (1) * (A.EstimatedAngle).CompareTo(B.EstimatedAngle));
                        this.HKLListView.Items.Refresh();
                    }
                    else
                    {
                        MessageBox.Show("I just found a reflex which is identical to that you wanted to add! I am pretty sure you don't want to add it twice.", "No chnages detected!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("I am pretty sure that it is impossible to use [000] as a reflex.", "[000] as a reflex detected!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("I have no clue to which crystal data I should add your reflex! Please be so kind and tell me.", "No crystal data selected selected!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveAndExitButton_Click(object sender, RoutedEventArgs e)
        {
            ForSave = true;
            this.Close();
        }
    }
}
