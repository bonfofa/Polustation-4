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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PoluTation_4
{
    /// <summary>
    /// Interação lógica para Notification.xam
    /// </summary>
    public partial class Notification : UserControl
    {
        Dictionary<string, bool> isAdded = new Dictionary<string ,bool>();
        public Notification()
        {
            InitializeComponent();
            Notification1.Margin = new Thickness(0, 84, 0, -71);
        }
        Storyboard storyboard = new Storyboard();
        TimeSpan halfsecond = TimeSpan.FromMilliseconds(500);
        TimeSpan second = TimeSpan.FromSeconds(1);

        private IEasingFunction Smooth
        {
            get;
            set;
        }
       = new QuarticEase
       {
           EasingMode = EasingMode.EaseInOut
       };

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

        private void Notification1_Loaded(object sender, RoutedEventArgs e)
        {

        }

        SoundPlayer player;
        private async void AudioPlay(string path)
        {
            player = new SoundPlayer(path);
            player.Load();
            player.Play();
        }

        private void AudioStop()
        {
            player.Stop();
        }

        public async void PlayNotification(string Message,Grid g)
        {
            if (g.Opacity == 1)
            {
                if (Notification1.Margin == new Thickness(0, 84, 0, -71))
                {
                    MessageLabel.Content = Message;
                    ObjectShiftPos(Notification1, Notification1.Margin, new Thickness(0, 0, 0, 13));
                    await Task.Delay(150);
                    AudioPlay(@"bin\Resources\SEf\NotSF.wav");
                    await Task.Delay(2500);
                    ObjectShiftPos(Notification1, Notification1.Margin, new Thickness(0, 84, 0, -71));
                }
            }
        }
    }
}
