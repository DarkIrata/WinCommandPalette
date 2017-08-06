using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WinCommandPalette.Libs.Controls
{
    [ContentProperty("Text")]
    public partial class InAppNotification : UserControl
    {
        public event RoutedEventHandler CloseClick;

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(InAppNotification), new PropertyMetadata("Missing Infotext"));

        public string Text
        {
            get => (string)this.GetValue(TextProperty);
            set => this.SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty SlideDurationProperty =
            DependencyProperty.Register("SlideDuration", typeof(Duration), typeof(InAppNotification), new PropertyMetadata(new Duration(new TimeSpan(0,0,10)), OnSlideDurationChanged));

        public Duration SlideDuration
        {
            get => (Duration)this.GetValue(SlideDurationProperty);
            set => this.SetValue(SlideDurationProperty, value);
        }

        public static readonly DependencyProperty SlideSpeedProperty =
            DependencyProperty.Register("SlideSpeed", typeof(double), typeof(InAppNotification), new PropertyMetadata(3.0, OnSlideSpeedChanged));

        public double SlideSpeed
        {
            get => (double)this.GetValue(SlideSpeedProperty);
            set => this.SetValue(SlideSpeedProperty, value);
        }

        public static readonly DependencyProperty NoticeTypeProperty =
            DependencyProperty.Register("NoticeType", typeof(NoticeType), typeof(InAppNotification), new PropertyMetadata(NoticeType.None, new PropertyChangedCallback(OnNoticeTypeChanged)));


        public NoticeType NoticeType
        {
            get => (NoticeType)this.GetValue(NoticeTypeProperty);
            set
            {
                this.SetValue(NoticeTypeProperty, value);
            }
        }

        private Storyboard slideStoryboard = null;

        public InAppNotification()
        {
            this.InitializeComponent();
            this.MouseEnter += this.InAppNotification_MouseEnter;
            this.MouseLeave += this.InAppNotification_MouseLeave;
            this.btnClose.Click += this.BtnClose_Click;
        }

        private void InAppNotification_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.slideStoryboard.Pause();
        }

        private void InAppNotification_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.slideStoryboard.Resume();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.slideStoryboard.Stop();
            this.UpdateSlideStory();
        }

        public void ShowNotification(NoticeType noticeType, string text)
        {
            this.ShowNotification(noticeType, text, this.SlideSpeed);
        }

        public void ShowNotification(NoticeType noticeType, string text, double slideSpeed)
        {
            this.ShowNotification(noticeType, text, slideSpeed, this.SlideDuration);
        }

        public void ShowNotification(NoticeType noticeType, string text, double slideSpeed, Duration slideDuration)
        {
            this.NoticeType = noticeType;
            this.Text = text;
            this.SlideSpeed = slideSpeed;
            this.SlideDuration = slideDuration;

            this.slideStoryboard.Begin();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            this.RenderTransform = new TranslateTransform()
            {
                X = 0,
                Y = this.ActualHeight
            };

            this.UpdateSlideStory();
        }

        private DoubleAnimation GetDoubleAnimation(double toValue, bool setBeginTime = false)
        {
            var doubleAnimation = new DoubleAnimation(toValue, this.SlideDuration)
            {
                SpeedRatio = this.SlideSpeed,
            };

            if (setBeginTime)
            {
                doubleAnimation.BeginTime = this.SlideDuration.TimeSpan;
            }

            Storyboard.SetTarget(doubleAnimation, this);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("RenderTransform.(TranslateTransform.Y)"));

            return doubleAnimation;
        }

        private static void OnNoticeTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is InAppNotification notification)
            {
                notification.BaseControl.SetColorByNoticeType(notification.NoticeType);
            }
        }

        private static void OnSlideDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is InAppNotification inAppNotification)
            {
                inAppNotification.UpdateSlideStory();
            }
        }

        private static void OnSlideSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is InAppNotification inAppNotification)
            {
                inAppNotification.UpdateSlideStory();
            }
        }

        private void UpdateSlideStory()
        {
            this.slideStoryboard = new Storyboard();
            this.slideStoryboard.Children.Add(this.GetDoubleAnimation(0));
            this.slideStoryboard.Children.Add(this.GetDoubleAnimation(this.ActualHeight, true));
        }
    }
}
