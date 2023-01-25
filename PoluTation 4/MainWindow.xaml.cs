using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using DiscordRPC;
using Microsoft.Win32;
using Newtonsoft.Json;
using WpfAnimatedGif;
using MySql.Data.MySqlClient;
using System.Data;

namespace PoluTation_4
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        static Cursor CN = new Cursor(Application.GetResourceStream(new Uri("CN.cur", UriKind.Relative)).Stream);
        public MainWindow()
        {
            InitializeComponent();
            client = new DiscordRpcClient("892158864827514921");
            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                this.Min.Content = DateTime.Now.ToString("HH:mm");
            }, this.Dispatcher);
            AddGame1.Opacity = 0;
            SettingsGrid.Opacity = 0;
            SettingsGrid.IsHitTestVisible = false;
            AddGame1.IsHitTestVisible = false;
            ReferenceGif.Opacity = 0;
            Reference.Opacity = 0;
            PoluStoreG.Opacity = 0;
            PoluStoreG.IsHitTestVisible = false;
            actuaLInterface.Opacity = 0;
            Contents.Margin = new Thickness(0, 40, 0, -40);
            Contents.Opacity = 0;



            try
            {
                if (!string.IsNullOrEmpty(File.ReadAllText("bin\\apps.json")))
                {
                    //CreateGames
                    string tson = File.ReadAllText("bin\\apps.json");
                    if (!string.IsNullOrEmpty(tson))
                    {
                        dynamic json = JsonConvert.DeserializeObject(tson);
                        foreach (var app in json)
                        {
                            CreateGame("" + app.appname, "" + app.imagelink, "" + app.path);
                        }
                    }
                    //CreateUser
                    string yson = File.ReadAllText("bin\\user.json");
                    if (!string.IsNullOrEmpty(yson))
                    {
                        dynamic kson = JsonConvert.DeserializeObject(yson);
                        foreach (var app in kson)
                        {
                            ChangeUser("" + app.username, "" + app.profilepic, "" + app.theme, double.Parse("" + app.volume));
                        }
                    }
                }

            }
            catch (Exception er)
            {
                MessageBox.Show("0x906895b");
            }
        }
        Storyboard storyboard = new Storyboard();
        TimeSpan halfsecond = TimeSpan.FromMilliseconds(500);
        TimeSpan second = TimeSpan.FromSeconds(1);
        //HttpDownloader httpdownloader;
        public DiscordRpcClient client;
        Notification not = new Notification();

        string UserName;

        public void ChangeUser(string User,string Pfp,string Theme, double Volume)
        {
            if (!string.IsNullOrEmpty(User)&&User.Contains("Guest") == false)
            {
                UserName = User;
                TopUser.Content = User;
                PUsername = User;
                UsernameSetterTextBox.Text = User;
            }

            if (!string.IsNullOrEmpty(Pfp))
            {
                IconString = Pfp;
                PProfilePic = Pfp;
                PFPImagePreview.Source = new BitmapImage(new Uri(Pfp));
                PFPRec.Fill = new ImageBrush(new BitmapImage(new Uri(Pfp)));
            }

            if (!string.IsNullOrEmpty(Theme))
            {
                if (Theme.Contains(".gif"))
                {
                    try
                    {
                        ReferenceGif.Opacity = 0.5;
                        Reference.Opacity = 0;
                        ReferenceGif.Source = new Uri(Theme);
                        PTheme = Theme;
                        NotificationMenu.PlayNotification("Tema alterado", Contents);
                        VolumeG.Opacity = 0;
                        ReferenceGif.Volume = 0;
                    }
                    catch (Exception er)
                    {
                        ReferenceGif.Opacity = 0;
                        Reference.Opacity = 0;
                    }
                }
                else if (Theme.Contains(".mp4")) 
                {
                    try
                    {
                        ReferenceGif.Opacity = 0.5;
                        Reference.Opacity = 0;
                        ReferenceGif.Source = new Uri(Theme);
                        PTheme = Theme;
                        NotificationMenu.PlayNotification("Tema alterado", Contents);
                        ThemeVolume = Volume;
                        VolumeSlider.Value = ThemeVolume;
                        ReferenceGif.Volume = ThemeVolume;
                        VolumeG.Opacity = 1;
                    }
                    catch (Exception er)
                    {
                        ReferenceGif.Opacity = 0;
                        Reference.Opacity = 0;
                    }
                }
                else
                {
                    Reference.Opacity = 0.5;
                    ReferenceGif.Opacity = 0;
                    Reference.Fill = new ImageBrush(new BitmapImage(new Uri(Theme)));
                    PTheme = Theme;
                    NotificationMenu.PlayNotification("Tema alterado", Contents);
                    VolumeG.Opacity = 0;
                    ReferenceGif.Volume = 0;
                }
            }
        }

        public string RemoveEncoding(string encodedJson)
        {
            string text = File.ReadAllText("bin\\apps.json");
            var sb = new StringBuilder(encodedJson);
            if (text.Contains("]"))
            {
                sb.Replace("[", ",");
                text = text.Replace("]", "");
                File.WriteAllText("bin\\apps.json", text);
            }
            return sb.ToString();
        }

        private IEasingFunction Smooth
        {
            get;
            set;
        }
       = new QuarticEase
       {
           EasingMode = EasingMode.EaseInOut
       };

        private void BackGround_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        /*
          ((Storyboard)FindResource("fadeout")).Begin(Readtermsbutton);
        ObjectShiftPos(AfterButton, AfterButton.Margin, new Thickness(0, 269, 0, 0));
         */
        public void ObjectShiftPos(DependencyObject Object, Thickness Get, Thickness Set)
        {
            ThicknessAnimation ShiftAnimation = new ThicknessAnimation()
            {
                From = Get,
                To = Set,
                Duration = second,
                EasingFunction = Smooth,
            };
            Storyboard.SetTarget(ShiftAnimation, Object);
            Storyboard.SetTargetProperty(ShiftAnimation, new PropertyPath(MarginProperty));
            storyboard.Children.Add(ShiftAnimation);
            storyboard.Begin();
        }

        private async void LoadingScreen_Loaded(object sender, RoutedEventArgs e)
        {
            client.Initialize();
            UpdateDiscordActualGame("Carregando menu...");
            ObjectShiftPos(Ps4Logo, Ps4Logo.Margin, new Thickness(0, 234, 0, 0));
            await Task.Delay(50);
            ((Storyboard)FindResource("fadein")).Begin(Ps4Logo);
            await Task.Delay(840);
            ((Storyboard)FindResource("fadein")).Begin(loadingStat);

            //LoadingRegion
            actuaLInterface.Cursor = CN;
            TurnOffMenu.Cursor = CN;
            await Task.Delay(3050);
            AudioPlay(@"bin\Resources\SEf\PSload.wav");
            if (string.IsNullOrEmpty(UserName)) { loadingStat.Content = "Carregado! bem vindo "; }
            else
            {
                loadingStat.Content = "Carregado! bem vindo " + UserName;
            }
            await Task.Delay(500);
            ((Storyboard)FindResource("fadeout")).Begin(LoadingScreen);
            ((Storyboard)FindResource("fadein")).Begin(actuaLInterface);
            await Task.Delay(250);
            ObjectShiftPos(Contents, Contents.Margin, new Thickness(0, 0, 0, 0));
            await Task.Delay(200);
            ((Storyboard)FindResource("fadein")).Begin(Contents);
            UpdateDiscordActualGame("On library");
            await Task.Delay(11000);
            AudioStop();
        }

        private void CancelButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("kindafadeout")).Begin(CancelButton);
        }

        private void CancelButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("kindafadein")).Begin(CancelButton);
        }

        private void ConfirmButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("kindafadein")).Begin(ConfirmButton);
        }

        private void ConfirmButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("kindafadeout")).Begin(ConfirmButton);
        }

        private void TurnOff_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((Storyboard)FindResource("fadein")).Begin(TurnOffMenu);
            TurnOffMenu.IsHitTestVisible = true;
        }

        private void CancelButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            ((Storyboard)FindResource("fadeout")).Begin(TurnOffMenu);
            TurnOffMenu.IsHitTestVisible = false;
        }

        private async void ConfirmButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UpdateDiscordActualGame("Saindo...");
            TurnOffMenu.IsHitTestVisible = false;
            ((Storyboard)FindResource("fadein")).Begin(LoadingScreen);
            ((Storyboard)FindResource("fadeout")).Begin(actuaLInterface);
            loadingStat.Content = "Descarregando...";
            await Task.Delay(2500);
            loadingStat.Content = "Fechando! tchau";
            await Task.Delay(1500);
            this.Close();
        }

        private void Background_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void AddGame_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fadein")).Begin(BackgroudGradient);
        }

        private void AddGame_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fadeout")).Begin(BackgroudGradient);
        }

        private void AddGame_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsEditEnabled == false)
            {
                F1.Opacity = 0;
                F12.Opacity = 0;
                GamePanel.IsHitTestVisible = false;
                SettingsGrid.IsHitTestVisible = false;
                Topbar.IsHitTestVisible = false;
                AddGame1.IsHitTestVisible = true;
                ((Storyboard)FindResource("fadeout")).Begin(GamePanel);
                if (SettingsGrid.Opacity == 1)
                {
                    ((Storyboard)FindResource("fadeout")).Begin(SettingsGrid);
                }
            ((Storyboard)FindResource("fadeout")).Begin(Topbar);
                ((Storyboard)FindResource("fadein")).Begin(AddGame1);
                //CreateGame("Teste", "https://cdn.discordapp.com/attachments/852625903048982569/892068434827968593/unknown.png", @"C:\Test");
            }
            else
            {
                NotificationMenu.PlayNotification("Modo de editar ligado", Contents);
            }
        }

        private async void GamingController_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UpdateDiscordActualGame("On library");
            if (GamePanel.Opacity == 1 == false)
            {
                ((Storyboard)FindResource("fadein")).Begin(GamePanel);
                ((Storyboard)FindResource("fadeout")).Begin(SettingsGrid);
                if (Topbar.Opacity == 0)
                {
                    ((Storyboard)FindResource("fadeout")).Begin(Topbar);
                }
            }
            GamingController.IsHitTestVisible = false;
            await Task.Delay(500);
            GamingController.IsHitTestVisible = true;
        }

        private async void Settings_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UpdateDiscordActualGame("On settings");
            if (SettingsGrid.Opacity == 1 == false)
            {
                ((Storyboard)FindResource("fadeout")).Begin(Topbar);
                SettingsGrid.IsHitTestVisible = true;
                ((Storyboard)FindResource("fadein")).Begin(SettingsGrid);
                if (GamePanel.Opacity == 1)
                {
                    ((Storyboard)FindResource("fadeout")).Begin(GamePanel);
                }
            }
        }

        public class data
        {
            public string appname { get; set; }
            public bool disabled { get; set; }
            public string imagelink { get; set; }
            public string path { get; set; }
        }

        public class data2
        {
            public string profilepic { get; set; }
            public string theme { get; set; }
            public string username { get; set; }
            public double volume { get; set; }
        }

        public string jseon;
        public bool IsEditEnabled = false;

        public async void CreateGame(string GameNameS, string ImageUrl, string GamePath)
        {
            if (string.IsNullOrEmpty(GameNameS))
            {
                GameNameS = "Example";
            }

            List<data> _data = new List<data>();
            _data.Add(new data()
            {
                appname = GameNameS,
                imagelink = ImageUrl,
                path = GamePath,
                disabled = false
            }); ;

            Grid GameGrid = new Grid
                {
                    IsHitTestVisible = true,
                    Height = 208,
                    Width = 151
                };

                GameGrid.MouseLeftButtonDown += (s, e) =>
                {
                    if (IsEditEnabled == false)
                    {
                        try
                        {
                            Process.Start($@"{GamePath}");
                            NotificationMenu.PlayNotification($"{GameNameS} Iniciado", Contents);
                        }
                        catch (Exception ex)
                        {
                            NotificationMenu.PlayNotification($"A operação foi cancelada", Contents);
                        }
                    }
                    else
                    {
                        string text = File.ReadAllText(@"bin\apps.json");
                        if (text.Contains($"\"appname\":\"{GameNameS}\",\"disabled\":false"))
                        {
                            F1.Opacity = 1;
                            F12.Opacity = 1;
                            IconString = ImageUrl;
                            FilePathString = GamePath;
                            AdicionarJogo.Content = "Editar Jogo";
                            GameNameString = GameNameS;
                            OldGameName = GameNameS;
                            OldImageUrl = ImageUrl;
                            OldGamePath = GamePath;

                            PathSelected.Content = GamePath;
                            PathSelected.Opacity = 1;
                            GameName.Text = GameNameS;
                            IconImage.Source = new BitmapImage(new Uri(ImageUrl));

                            GamePanel.IsHitTestVisible = false;
                            SettingsGrid.IsHitTestVisible = false;
                            Topbar.IsHitTestVisible = false;
                            AddGame1.IsHitTestVisible = true;
                            ((Storyboard)FindResource("fadeout")).Begin(GamePanel);
                            if (SettingsGrid.Opacity == 1)
                            {
                                ((Storyboard)FindResource("fadeout")).Begin(SettingsGrid);
                            }
                        ((Storyboard)FindResource("fadeout")).Begin(Topbar);
                            ((Storyboard)FindResource("fadein")).Begin(AddGame1);
                        }
                        else
                        {
                            NotificationMenu.PlayNotification($"Este jogo esta desativado", Contents);
                        }
    }
                };

                LinearGradientBrush GradientBackgroundWhite = new LinearGradientBrush();
                GradientBackgroundWhite.StartPoint = new Point(0.5, 0);
                GradientBackgroundWhite.EndPoint = new Point(0.5, 1);
                GradientBackgroundWhite.GradientStops.Add(new GradientStop(Color.FromArgb(0, 120, 120, 120), 1));
                GradientBackgroundWhite.GradientStops.Add(new GradientStop(Color.FromRgb(255, 255, 255), 0));

                Rectangle GradientBK = new Rectangle
                {
                    Opacity = 0,
                    IsHitTestVisible = false,
                    Margin = new Thickness(0, 0, 0, 61),
                    Fill = GradientBackgroundWhite
                };

                LinearGradientBrush GradientBlue = new LinearGradientBrush();
                GradientBlue.StartPoint = new Point(0.5, 0);
                GradientBlue.EndPoint = new Point(0.5, 1);
                GradientBlue.GradientStops.Add(new GradientStop(Color.FromArgb(0, 0, 88, 160), 1));
                GradientBlue.GradientStops.Add(new GradientStop(Color.FromRgb(0, 46, 126), 0));

                Rectangle GradientBlueBK = new Rectangle
                {
                    IsHitTestVisible = true,
                    Width = 152,
                    Margin = new Thickness(6, 6, 6, 66),
                    Fill = GradientBlue
                };

                GameGrid.MouseEnter += (s, e) =>
                {
                    ((Storyboard)FindResource("fadein")).Begin(GradientBK);
                };

                GameGrid.MouseLeave += (s, e) =>
                {
                    ((Storyboard)FindResource("fadeout")).Begin(GradientBK);
                };

                ImageBrush IconID = new ImageBrush();
                IconID.ImageSource = new BitmapImage(new Uri(ImageUrl));

                Rectangle GameImage = new Rectangle
                {
                    IsHitTestVisible = true,
                    Fill = IconID,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Height = 124,
                    VerticalAlignment = VerticalAlignment.Top,
                    Width = 135,
                    Margin = new Thickness(9, 12, 0, 0)
                };

                Label GameLabel = new Label
                {
                    IsHitTestVisible = true,
                    Content = GameNameS,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Height = 38,
                    Margin = new Thickness(0, 147, 0, -43),
                    Width = 154,
                    Foreground = Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                    FontSize = 18,
                    FontFamily = new FontFamily("Segoe UI Light")
                };
            string lines = File.ReadAllText(@"bin\\apps.json");
            if (lines.Contains(GameNameS))
            {
                if (lines.Contains($"\"appname\":\"{GameNameS}\",\"disabled\":false"))
                {
                    GameGrid.Children.Add(GradientBK);
                    GameGrid.Children.Add(GradientBlueBK);
                    GameGrid.Children.Add(GameImage);
                    GameGrid.Children.Add(GameLabel);
                    Scroll.Children.Add(GameGrid);
                    Scroll.Width += 151;
                    NotificationMenu.PlayNotification($"{GameNameS} Adicionado", Contents);
                    DockPanel.SetDock(GameGrid, Dock.Left);
                }
            }
            else
            {
                    GameGrid.Children.Add(GradientBK);
                    GameGrid.Children.Add(GradientBlueBK);
                    GameGrid.Children.Add(GameImage);
                    GameGrid.Children.Add(GameLabel);
                    Scroll.Children.Add(GameGrid);
                    Scroll.Width += 151;
                    NotificationMenu.PlayNotification($"{GameNameS} Adicionado", Contents);
                    DockPanel.SetDock(GameGrid, Dock.Left);

                    if (string.IsNullOrEmpty(lines))
                    {
                        string json = JsonConvert.SerializeObject(_data.ToArray());
                        File.AppendAllText(@"bin\\apps.json", json);
                    }
                    else if (lines.Contains($"\"appname\":\"{GameNameS}\"") == false)
                    {
                        string json = JsonConvert.SerializeObject(_data.ToArray());

                        string res = RemoveEncoding(json);
                        File.AppendAllText(@"bin\\apps.json", res);
                    }
                }
        }

        public void UpdateDiscordActualGame(string State)
        {
            client.SetPresence(new RichPresence()
            {
                Details = State,
                State = "User : " + PUsername,
                Assets = new Assets()
                {
                    LargeImageKey = "polylogo",
                    LargeImageText = "On PoluStation",
                    SmallImageKey = "psbuttons",
                    SmallImageText = "Online"
                }
            });
        }

        public string PUsername;
        public string PProfilePic;
        public string PTheme;

        private void WindowGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape & AddGame1.Opacity == 1)
            {
                IconString = "";
                FilePathString = "";
                GameNameString = "";

                PathSelected.Content = "";
                GameName.Text = "Example";
                IconImage.Source = new BitmapImage(new Uri("pack://application:,,,/icons8-image-384.png"));

                GamePanel.IsHitTestVisible = true;
                SettingsGrid.IsHitTestVisible = false;

                Topbar.IsHitTestVisible = true;
                AddGame1.IsHitTestVisible = false;
                ((Storyboard)FindResource("fadein")).Begin(GamePanel);
                ((Storyboard)FindResource("fadein")).Begin(Topbar);

                ((Storyboard)FindResource("fadeout")).Begin(AddGame1);
            }
            else if (e.Key == Key.Escape & SettingsGrid.Opacity == 1)
            {
                List<data2> _data = new List<data2>();
                _data.Add(new data2()
                {
                    username = PUsername,
                    profilepic = PProfilePic,
                    theme = PTheme,
                    volume = ThemeVolume
                }); ;

                GamePanel.IsHitTestVisible = true;
                SettingsGrid.IsHitTestVisible = false;
                Topbar.IsHitTestVisible = true;
                AddGame1.IsHitTestVisible = false;
                ((Storyboard)FindResource("fadein")).Begin(GamePanel);
                ((Storyboard)FindResource("fadein")).Begin(Topbar);
                ((Storyboard)FindResource("fadeout")).Begin(SettingsGrid);
                string lines = File.ReadAllText(@"bin\\user.json");

                if (PUsername.Contains("Guest") == false) {
                    if (string.IsNullOrEmpty(lines))
                    {
                        string json = JsonConvert.SerializeObject(_data.ToArray());
                        File.AppendAllText(@"bin\\user.json", json);
                    }
                    else
                    {

                        File.WriteAllText(@"bin\\user.json", String.Empty);
                        string json = JsonConvert.SerializeObject(_data.ToArray());
                        File.AppendAllText(@"bin\\user.json", json);
                    }
                }
            }
            else if (e.Key == Key.Escape & AddGame1.Opacity == 1 & IsEditEnabled == true)
            {
                IconString = "";
                FilePathString = "";
                GameNameString = "";

                AdicionarJogo.Content = "Adicionar Jogo";

                PathSelected.Content = "";
                GameName.Text = "Example";
                IconImage.Source = new BitmapImage(new Uri("pack://application:,,,/icons8-image-384.png"));

                GamePanel.IsHitTestVisible = true;
                SettingsGrid.IsHitTestVisible = false;

                Topbar.IsHitTestVisible = true;
                AddGame1.IsHitTestVisible = false;

                ((Storyboard)FindResource("fadein")).Begin(GamePanel);
                ((Storyboard)FindResource("fadein")).Begin(Topbar);

                ((Storyboard)FindResource("fadeout")).Begin(AddGame1);
            }
            else if (e.Key == Key.Escape & PoluStoreG.Opacity == 1)
            {
                GamePanel.IsHitTestVisible = true;
                SettingsGrid.IsHitTestVisible = false;

                Topbar.IsHitTestVisible = true;
                PoluStoreG.IsHitTestVisible = false;

                ((Storyboard)FindResource("fadein")).Begin(GamePanel);
                ((Storyboard)FindResource("fadein")).Begin(Topbar);

                ((Storyboard)FindResource("fadeout")).Begin(PoluStoreG);
            }
            else if (e.Key.ToString() == "F1" & AddGame1.Opacity == 1 & IsEditEnabled == true)
            {
                string text = File.ReadAllText("bin\\apps.json");
                text = text.Replace($"\"appname\":\"{GameName.Text}\",\"disabled\":false", $"\"appname\":\"{GameName.Text}\",\"disabled\":true");
                File.WriteAllText("bin\\apps.json", text);

                IconString = "";
                FilePathString = "";
                GameNameString = "";

                AdicionarJogo.Content = "Adicionar Jogo";

                PathSelected.Content = "";
                GameName.Text = "Example";
                IconImage.Source = new BitmapImage(new Uri("pack://application:,,,/icons8-image-384.png"));

                GamePanel.IsHitTestVisible = true;
                SettingsGrid.IsHitTestVisible = false;

                Topbar.IsHitTestVisible = true;
                AddGame1.IsHitTestVisible = false;
               


                ((Storyboard)FindResource("fadein")).Begin(GamePanel);
                ((Storyboard)FindResource("fadein")).Begin(Topbar);

                ((Storyboard)FindResource("fadeout")).Begin(AddGame1);
                NotificationMenu.PlayNotification("Reinicie para alterações fazerem efeito!", Contents);
            }
        }

        SoundPlayer player;
        private void AudioPlay(string path)
        {
            player = new SoundPlayer(path);
            player.Load();
            player.Play();
        }

        private void AudioStop()
        {
            player.Stop();
        }

        public string IconString;
        public string GameNameString;
        public string FilePathString;

        private void IconRec_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "Selecione seu icone";
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg;)|*.png;*.jpg;*.jpeg;|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    IconString = openFileDialog.FileName;
                    IconImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                }
                catch(Exception er)
                {
                    NotificationMenu.PlayNotification("Tem que ser uma imagem!", Contents);
                }
            }
        }

        private void PathSelect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "Selecione seu executavel";
            openFileDialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                if (PathSelected.Opacity == 0)
                {
                    ((Storyboard)FindResource("fadein")).Begin(PathSelected);
                }
                FilePathString = openFileDialog.FileName;
                PathSelected.Content = openFileDialog.FileName;
            }
        }

        bool IsStartupping = true;
        private void GameName_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            if (IsStartupping == false)
            {
                GameNameString = GameName.Text;
            }
            else
            {
                IsStartupping = false;
            }
        }

        private void Path_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fadein")).Begin(SelectedRound2);
        }

        private void Path_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fadeout")).Begin(SelectedRound2);
        }

        private void Icon_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fadein")).Begin(SelectedRound1);
        }

        private void Icon_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fadeout")).Begin(SelectedRound1);
        }

        private void Name_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fadein")).Begin(SelectedRound);
        }

        private void Name_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fadeout")).Begin(SelectedRound);
        }

        private void Add_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fadein")).Begin(SelectedRound3);
        }

        private void Add_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fadeout")).Begin(SelectedRound3);
        }

        public string removeth(string text)
        {
            text = text.Replace("\\\\", "\\");
            return text.ToString();
        }

        public string addth(string text)
        {
            text = text.Replace("\\", "\\\\");
            return text.ToString();
        }

        public string OldGameName;
        public string OldImageUrl;
        public string OldGamePath;
        private void Add_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(GameNameString) & string.IsNullOrEmpty(IconString) & string.IsNullOrEmpty(FilePathString))
            {
                NotificationMenu.PlayNotification($"Executavel incorreto", Contents);
                IconString = "";
                FilePathString = "";
                GameNameString = "Example";

                PathSelected.Content = "";
                GameName.Text = "Example";
                IconImage.Source = new BitmapImage(new Uri("pack://application:,,,/icons8-image-384.png"));
            }
            else if (IsEditEnabled == true)
            {
                string text = File.ReadAllText(@"bin\apps.json");
                    try
                    {
                        text = text.Replace(OldGameName, GameNameString);
                        text = text.Replace("\\\\", "\\");
                        text = text.Replace("\"imagelink\":\"" + @OldImageUrl + "\"", "\"imagelink\":\"" + IconString + "\"");
                        text = text.Replace("\"path\":\"" + OldGamePath + "\"", "\"path\":\"" + FilePathString + "\"");
                        text = text.Replace("\\", "\\\\");
                        File.WriteAllText(@"bin\apps.json", text);
                        NotificationMenu.PlayNotification($"Necessario reiniciar o Polu", Contents);
                    }
                    catch (Exception er)
                    {

                    }
            }
            else
            {
                if (!string.IsNullOrEmpty(GameNameString) & !string.IsNullOrEmpty(IconString) & !string.IsNullOrEmpty(FilePathString))
                {
                    try
                    {
                        CreateGame(GameNameString, IconString, FilePathString);
                        IconString = "";
                        FilePathString = "";
                        GameNameString = "";

                        PathSelected.Content = "";
                        GameName.Text = "Example";
                        IconImage.Source = new BitmapImage(new Uri("pack://application:,,,/icons8-image-384.png"));

                        GamePanel.IsHitTestVisible = true;
                        SettingsGrid.IsHitTestVisible = false;
                        Topbar.IsHitTestVisible = true;
                        AddGame1.IsHitTestVisible = false;
                        ((Storyboard)FindResource("fadein")).Begin(GamePanel);
                        ((Storyboard)FindResource("fadein")).Begin(Topbar);
                        ((Storyboard)FindResource("fadeout")).Begin(AddGame1);
                    }
                    catch (Exception er)
                    {
                        NotificationMenu.PlayNotification($"Executavel incorreto", Contents);
                        IconString = "";
                        FilePathString = "";
                        GameNameString = "";

                        PathSelected.Content = "";
                        GameName.Text = "Example";
                        IconImage.Source = new BitmapImage(new Uri("pack://application:,,,/icons8-image-384.png"));
                    }
                }
                else
                {
                    NotificationMenu.PlayNotification($"Há algo de errado!", Contents);
                }
            }
        }

        private void FullScreen_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fadein")).Begin(SelectedRound4);
        }

        private void FullScreen_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fadeout")).Begin(SelectedRound4);
        }

        private void FullScreen_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NotificationMenu.PlayNotification($"Fullscreen não esta funcionando", Contents);
            /*if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
                IsFull.Content = "Fullscreen - On";
            }
            else if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                IsFull.Content = "Fullscreen - Off";
            }*/
        }

        private void ProfilePicture_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fadein")).Begin(SelectedRound6);
        }

        private void ProfilePicture_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fadeout")).Begin(SelectedRound6);
        }

        private void ProfilePicture_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "Selecione seu icone";
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg;)|*.png;*.jpg;*.jpeg;|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                IconString = openFileDialog.FileName;
                PProfilePic = openFileDialog.FileName;
                PFPImagePreview.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                PFPRec.Fill = new ImageBrush(new BitmapImage(new Uri(openFileDialog.FileName)));
            }
        }

        private void UsernameSetterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TopUser.Content = UsernameSetterTextBox.Text;
            PUsername = UsernameSetterTextBox.Text;
            Properties.Settings.Default.Username = UsernameSetterTextBox.Text;
        }

        private void Username_Category_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fadein")).Begin(SelectedRound5);
        }

        private void Username_Category_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fadeout")).Begin(SelectedRound5);
        }

        private void EditMode_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(IsEditEnabled == false)
            {
                IsEditEnabled = true;
                AdicionarJogo.Content = "Editar Jogo";
                NotificationMenu.PlayNotification("Modo edição habilitado", Contents);
            }
            else
            {
                IsEditEnabled = false;
                AdicionarJogo.Content = "Adicionar Jogo";
                NotificationMenu.PlayNotification("Modo edição desabilitado", Contents);
            }
        }

        private void Add_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void Theme_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "Selecione seu tema";
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.gif;*.jpeg;)|*.png;*.jpg;*.gif;*.mp4;*.jpeg;|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                if (openFileDialog.FileName.Contains(".gif"))
                {
                    ReferenceGif.Opacity = 0.5;
                    Reference.Opacity = 0;
                    ReferenceGif.Source = new Uri(openFileDialog.FileName);
                    PTheme = openFileDialog.FileName;
                    NotificationMenu.PlayNotification("Tema alterado", Contents);
                    VolumeG.Opacity = 0;
                    ReferenceGif.Volume = 0;
                }
                else if(openFileDialog.FileName.Contains(".mp4"))
                {
                    ReferenceGif.Opacity = 0.5;
                    Reference.Opacity = 0;
                    ReferenceGif.Source = new Uri(openFileDialog.FileName);
                    PTheme = openFileDialog.FileName;
                    NotificationMenu.PlayNotification("Tema alterado", Contents);
                    VolumeSlider.Value = ThemeVolume;
                    ReferenceGif.Volume = ThemeVolume;
                    VolumeG.Opacity = 1;
                }
                else
                {
                    try
                    {
                        Reference.Opacity = 0.5;
                        ReferenceGif.Opacity = 0;
                        Reference.Fill = new ImageBrush(new BitmapImage(new Uri(openFileDialog.FileName)));
                        PTheme = openFileDialog.FileName;
                        NotificationMenu.PlayNotification("Tema alterado", Contents);
                        VolumeG.Opacity = 0;
                        ReferenceGif.Volume = 0;
                    }
                    catch(Exception er)
                    {
                        NotificationMenu.PlayNotification("Imagem invalida", Contents);
                    }
                }
            }
        }

        private void Theme_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fadein")).Begin(SelectedRound7);
        }

        private void Theme_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fadeout")).Begin(SelectedRound7);
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ReferenceGif.Opacity = 0;
            Reference.Opacity = 0;
            PTheme = null;
            NotificationMenu.PlayNotification("Tema removido", Contents);
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            RemoveTheme.Opacity = 0.5;
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            RemoveTheme.Opacity = 1;
        }

        private void ReferenceGif_MediaEnded(object sender, RoutedEventArgs e)
        {
            try
            {
                ReferenceGif.Position = new TimeSpan(0, 0, 1);
                ReferenceGif.Play();
            }
            catch(Exception er)
            {

            }
        }

        private void TopHover_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public double ThemeVolume;
        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ThemeVolume = VolumeSlider.Value;
            ReferenceGif.Volume = VolumeSlider.Value;
        }

        private void Jogos_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fadeout")).Begin(NotSelected);
            ((Storyboard)FindResource("fadein")).Begin(Selected);
        }

        private void Jogos_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fadeout")).Begin(Selected);
            ((Storyboard)FindResource("fadein")).Begin(NotSelected);
        }

        private void Apps_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fadeout")).Begin(NotSelected1);
            ((Storyboard)FindResource("fadein")).Begin(Selected1);
        }

        private void Apps_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fadeout")).Begin(Selected1);
            ((Storyboard)FindResource("fadein")).Begin(NotSelected1);
        }

        private void Tema_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fadeout")).Begin(Selected2);
            ((Storyboard)FindResource("fadein")).Begin(NotSelected2);
        }

        private void Tema_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fadeout")).Begin(NotSelected2);
            ((Storyboard)FindResource("fadein")).Begin(Selected2);
        }

        private void Biblioteca_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fadeout")).Begin(NotSelected3);
            ((Storyboard)FindResource("fadein")).Begin(Selected3);
        }

        private void Biblioteca_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fadeout")).Begin(Selected3);
            ((Storyboard)FindResource("fadein")).Begin(NotSelected3);
        }

        private void PoluStore_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsEditEnabled == false)
            {
                F1.Opacity = 0;
                F12.Opacity = 0;
                GamePanel.IsHitTestVisible = false;
                SettingsGrid.IsHitTestVisible = false;
                Topbar.IsHitTestVisible = false;
                PoluStoreG.IsHitTestVisible = true;
                ((Storyboard)FindResource("fadeout")).Begin(GamePanel);
                if (SettingsGrid.Opacity == 1)
                {
                    ((Storyboard)FindResource("fadeout")).Begin(SettingsGrid);
                }
                ((Storyboard)FindResource("fadeout")).Begin(Topbar);
                ((Storyboard)FindResource("fadein")).Begin(PoluStoreG);
                //CreateGame("Teste", "https://cdn.discordapp.com/attachments/852625903048982569/892068434827968593/unknown.png", @"C:\Test");
            }
            else
            {
                NotificationMenu.PlayNotification("Modo de editar ligado", Contents);
            }
        }
    }
}
