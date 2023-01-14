using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace Bachelorarbeit_Berechnungstool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Load();
        }

        private void Load()
        {
            foreach (UIElement ele in Grid.Children)
            {
                if (ele.GetType() == typeof(TextBox))
                {
                    TextBox element = ele as TextBox;

                    element.Text = "";
                }
            }

            foreach (UIElement ele in PushrodGrid.Children)
            {
                if (ele.GetType() == typeof(TextBox))
                {
                    TextBox element = ele as TextBox;

                    element.Text = "";
                }
            }

            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\Setups\\"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Setups\\");
            }
            string[] Setups = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Setups\\");

            SetupComboBox.Items.Add("");
            for (int i = 0; i < Setups.Length; i++)
            {
                SetupComboBox.Items.Add(System.IO.Path.GetFileName(Setups[i]).Replace(".setup", ""));
            }
        }

        private void CalcBtn_Click(object sender, RoutedEventArgs e)
        {
            Calc();
        }

        private void Calc()
        {
            // Berechnung Nickung
            float Mmot = float.Parse(MmotTxtBox.Text);
            float Drad = float.Parse(DradTxtBox.Text) / 1000;


            float lWB = float.Parse(lWBTxtBox.Text);
            float lTW = float.Parse(lTWTxtBox.Text);
            float lCOG = float.Parse(lCOGTxtBox.Text);
            float hCOG = float.Parse(hCOGTxtBox.Text);

            float m = float.Parse(MassTxtBox.Text);


            float lQL = float.Parse(lQLTxtBox.Text);
            float lQLPR = float.Parse(lQLPRTxtBox.Text);
            float ldQL = float.Parse(ldQLTxtBox.Text);




            // Berechnung Wankung

            float lAPPR = float.Parse(lAPPRTxtBox.Text);

            float Rmin = float.Parse(RminTxtBox.Text);
            float v = float.Parse(vTxtBox.Text);




            float hUL = float.Parse(hULTxtBox.Text);
            float lUL = float.Parse(lULTxtBox.Text);


            float lRTunten = float.Parse(lRTuntenTxtBox.Text);
            float lRToben = float.Parse(lRTobenTxtBox.Text);


            float bQL1 = float.Parse(bQL1TxtBox.Text);
            float bQL2 = float.Parse(bQL2TxtBox.Text);
            float E = float.Parse(ETxtBox.Text);
            float Rp02 = float.Parse(Rp02TxtBox.Text);

            // Ausgabe

            double Fg = m * 9.81;

            double Fbrems = Mmot * 2 * Drad;

            double Fh = (Fg * lCOG - Fbrems * hCOG) / lWB;

            double Fv = Fg - Fh;

            double Fz = m * (v * v / Rmin);

            double Fvinnen = (Fg * lTW - 2 * Fz * hCOG) / (2 * lTW);

            double Fvaussen = Fg - Fvinnen;

            double Fzmax = Fv + Fvaussen;

            double Fymax = Fz * (Fvinnen / Fvaussen) * 0.5;

            double Fx = Fbrems / 4;



            double FzRTunten = Fzmax * (lQL + ldQL) / lQL;

            double FyRTunten = Fymax * (lRTunten + lRToben) / lRToben;

            double FyRToben = Fymax * (lRTunten / lRToben);

            double FxRTunten = Fx * (lRTunten + lRToben) / lRToben;

            double FxRToben = Fx * (lRTunten / lRToben);

            double Fzqlapunten = FzRTunten * (lQL - lQLPR) / lQLPR;

            double Fzqlapunten1 = Fzqlapunten * (bQL1 + bQL2) / bQL2;

            double Fzqlapunten2 = Fzqlapunten * (bQL1 + bQL2) / bQL1;



            double Fqlap = (Fzmax * lQLPR) / lQL;

            double alpha = Math.Atan((lQLPR + lAPPR - lUL) / hUL);

            double Fpr = Fqlap * Math.Cos(alpha);

            double Ffed = Fqlap * (lUL / hUL);

            double LambdaG = Math.PI * Math.Sqrt(E / (0.8 * Rp02));

            try
            {
                StreamReader SR = new StreamReader(Directory.GetCurrentDirectory() + "\\Protokoll\\Berechnungsprotokoll Fahrwerksauslegung.htm");

                string Content = SR.ReadToEnd();

                SR.Close();
                SR.Dispose();

                Content = Content.Replace("Fradox", FxRToben.ToString());
                Content = Content.Replace("Fradoy", FyRToben.ToString());
                Content = Content.Replace("Fradoz", "-----");
                Content = Content.Replace("Fradores", (Math.Sqrt(FyRToben * FyRToben + FxRToben * FxRToben)).ToString());

                Content = Content.Replace("Fradux", FxRTunten.ToString());
                Content = Content.Replace("Fraduy", FyRTunten.ToString());
                Content = Content.Replace("Fraduz", FzRTunten.ToString());
                Content = Content.Replace("Fradures", (Math.Sqrt(FxRTunten * FxRTunten + FyRTunten * FyRTunten + FzRTunten * FzRTunten)).ToString());

                Content = Content.Replace("Fqlo1x", "-----");
                Content = Content.Replace("Fqlo1y", "-----");
                Content = Content.Replace("Fqlo1z", "-----");
                Content = Content.Replace("Fqlo1res", "-----");

                Content = Content.Replace("Fqlo2x", "-----");
                Content = Content.Replace("Fqlo2y", "-----");
                Content = Content.Replace("Fqlo2z", "-----");
                Content = Content.Replace("Fqlo2res", "-----");

                Content = Content.Replace("Fqlu1x", "-----");
                Content = Content.Replace("Fqlu1y", "-----");
                Content = Content.Replace("Fqlu1z", Fzqlapunten1.ToString());
                Content = Content.Replace("Fqlu1res", Fzqlapunten1.ToString());

                Content = Content.Replace("Fqlu2x", "-----");
                Content = Content.Replace("Fqlu2y", "-----");
                Content = Content.Replace("Fqlu2z", Fzqlapunten2.ToString());
                Content = Content.Replace("Fqlu2res", Fzqlapunten2.ToString());

                if(!Directory.Exists(Directory.GetCurrentDirectory() + "\\Berechnungen\\"))
                {
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Berechnungen\\");
                }
                if(SetupComboBox.SelectedIndex != 0)
                {
                    using (StreamWriter SW = new StreamWriter(Directory.GetCurrentDirectory() + "\\Berechnungen\\" + SetupComboBox.Text + ".html", false, System.Text.Encoding.UTF8))
                    {
                        SW.Write(Content.Replace("#NAME#", SetupComboBox.Text));
                        SW.Flush();

                        SW.Close();
                        SW.Dispose();
                    }


                    var p = new Process();
                    p.StartInfo = new ProcessStartInfo(Directory.GetCurrentDirectory() + "\\Berechnungen\\" + SetupComboBox.Text + ".html")
                    {
                        UseShellExecute = true
                    };
                    p.Start();
                }
                else
                {
                    if(SetupNameTxtBox.Text.Trim() != "")
                    {
                        using (StreamWriter SW = new StreamWriter(Directory.GetCurrentDirectory() + "\\Berechnungen\\" + SetupNameTxtBox.Text + ".html", false, System.Text.Encoding.UTF8))
                        {
                            SW.Write(Content.Replace("#NAME#", SetupNameTxtBox.Text));
                            SW.Flush();

                            SW.Close();
                            SW.Dispose();
                        }

                        var p = new Process();
                        p.StartInfo = new ProcessStartInfo(Directory.GetCurrentDirectory() + "\\Berechnungen\\" + SetupComboBox.Text + ".html")
                        {
                            UseShellExecute = true
                        };
                        p.Start();
                    }
                    else
                    {
                        MessageBox.Show("Musst noch nen Namen eintragen du Keck", "FEHLER");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "FEHLER");
            }
        }


        private void SaveSetupBtn_Click(object sender, RoutedEventArgs e)
        {
            Calc();

            Save();

            MessageBox.Show("Gespeichert!", "INFO");
        }

        private void Save()
        {
            string Path = Directory.GetCurrentDirectory() + "\\Setups\\" + SetupNameTxtBox.Text + ".setup";

            try
            {
                if(SetupNameTxtBox.Text.Trim() != "")
                {
                    string Content = "";

                    foreach (UIElement ele in Grid.Children)
                    {
                        if (ele.GetType() == typeof(TextBox))
                        {
                            TextBox element = ele as TextBox;

                            if (!element.Name.Contains("Out"))
                            {
                                Content += element.Text + "###";
                            }
                        }
                    }

                    foreach (UIElement ele in PushrodGrid.Children)
                    {
                        if (ele.GetType() == typeof(TextBox))
                        {
                            TextBox element = ele as TextBox;

                            if (!element.Name.Contains("Out"))
                            {
                                Content += element.Text + "###";
                            }
                        }
                    }

                    StreamWriter SW = new StreamWriter(Path);

                    SW.Write(Content);

                    SW.Close();
                    SW.Dispose();
                }
                else
                {
                    MessageBox.Show("Gib doch nen Dateinamen an du Lackaffe", "FEHLER");
                }
                
            }
            catch
            {
                MessageBox.Show("Kanns sein dass du beim Dateinamen reingeschissen hast?", "FEHLER");
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void SetupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadSetup();
        }

        private void LoadSetup()
        {
            string Path = Directory.GetCurrentDirectory() + "\\Setups\\" + SetupComboBox.SelectedItem + ".setup";

            string Content = "";

            if(SetupComboBox.SelectedItem.ToString().Trim() != "")
            {
                try
                {
                    StreamReader SR = new StreamReader(Path);

                    Content = SR.ReadLine();

                    SR.Close();
                    SR.Dispose();
                }
                catch
                {
                    MessageBox.Show("Jo do han isch misch fareschnet, sorry. Schlussfürheude", "FEHLER");
                    this.Close();
                }

                string[] Contents = Content.Split("###");
                int Index = 0;

                foreach (UIElement ele in Grid.Children)
                {
                    if (ele.GetType() == typeof(TextBox))
                    {
                        TextBox element = ele as TextBox;

                        if (!element.Name.Contains("Out"))
                        {
                            element.Text = Contents[Index];
                            Index++;
                        }
                    }
                }

                foreach (UIElement ele in PushrodGrid.Children)
                {
                    if (ele.GetType() == typeof(TextBox))
                    {
                        TextBox element = ele as TextBox;

                        if (!element.Name.Contains("Out"))
                        {
                            element.Text = Contents[Index];
                            Index++;
                        }
                    }
                }
            }
            else
            {
                foreach (UIElement ele in Grid.Children)
                {
                    if (ele.GetType() == typeof(TextBox))
                    {
                        TextBox element = ele as TextBox;

                        if (!element.Name.Contains("Out"))
                        {
                            element.Text = "";
                        }
                    }
                }

                foreach (UIElement ele in PushrodGrid.Children)
                {
                    if (ele.GetType() == typeof(TextBox))
                    {
                        TextBox element = ele as TextBox;

                        if (!element.Name.Contains("Out"))
                        {
                            element.Text = "";
                        }
                    }
                }
            }
        }
    }
}
