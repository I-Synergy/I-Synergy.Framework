using ISynergy.Framework.UI.Abstractions.Controls.ToastMessages;
using ISynergy.Framework.UI.Controls.ToastNotification;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ISynergy.Framework.UI.Controls.ToastNotification.Display
{
    public class NotificationAnimator : INotificationAnimator
    {
        private readonly NotificationDisplayPart _displayPart;
        private readonly TimeSpan _showAnimationTime;
        private readonly TimeSpan _hideAnimationTime;

        public NotificationAnimator(NotificationDisplayPart displayPart, TimeSpan showAnimationTime, TimeSpan hideAnimationTime)
        {
            _displayPart = displayPart;
            _showAnimationTime = showAnimationTime;
            _hideAnimationTime = hideAnimationTime;
        }

        public void Setup()
        {
            ScaleTransform scale = new ScaleTransform(1, 0);
            _displayPart.RenderTransform = scale;
        }

        public void PlayShowAnimation()
        {
            var scale = (ScaleTransform)_displayPart.RenderTransform;
            scale.CenterY = _displayPart.ActualHeight / 2;
            scale.CenterX = _displayPart.ActualWidth / 2;

            Storyboard storyboard = new Storyboard();

            SetGrowYAnimation(storyboard);
            SetGrowXAnimation(storyboard);
            SetFadeInAnimation(storyboard);

            storyboard.Begin();
        }

        private void SetFadeInAnimation(Storyboard storyboard)
        {
            DoubleAnimation fadeInAnimation = new DoubleAnimation
            {
                Duration = _showAnimationTime,
                From = 0,
                To = 1
            };
            storyboard.Children.Add(fadeInAnimation);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath("Opacity"));
            Storyboard.SetTarget(fadeInAnimation, _displayPart);
        }

        private void SetGrowXAnimation(Storyboard storyboard)
        {
            DoubleAnimation growXAnimation = new DoubleAnimation
            {
                Duration = _showAnimationTime,
                From = 0,
                To = 1
            };
            storyboard.Children.Add(growXAnimation);
            Storyboard.SetTargetProperty(growXAnimation, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTarget(growXAnimation, _displayPart);
        }

        private void SetGrowYAnimation(Storyboard storyboard)
        {
            DoubleAnimation growYAnimation = new DoubleAnimation
            {
                Duration = _showAnimationTime,
                From = 0,
                To = 1
            };
            storyboard.Children.Add(growYAnimation);
            Storyboard.SetTargetProperty(growYAnimation, new PropertyPath("RenderTransform.ScaleY"));
            Storyboard.SetTarget(growYAnimation, _displayPart);
        }

        public void PlayHideAnimation()
        {
            _displayPart.MinHeight = 0;
            var scale = (ScaleTransform)_displayPart.RenderTransform;
            scale.CenterY = _displayPart.ActualHeight / 2;
            scale.CenterX = _displayPart.ActualWidth / 2;

            Storyboard storyboard = new Storyboard();

            SetShrinkYAnimation(storyboard);
            SetShrinkXAnimation(storyboard);
            SetFadeoutAnimation(storyboard);

            storyboard.Begin();
        }

        private void SetFadeoutAnimation(Storyboard storyboard)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation
            {
                Duration = _hideAnimationTime,
                From = 1,
                To = 0
            };

            storyboard.Children.Add(fadeOutAnimation);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath("Opacity"));
            Storyboard.SetTarget(fadeOutAnimation, _displayPart);
        }

        private void SetShrinkXAnimation(Storyboard storyboard)
        {
            DoubleAnimation shrinkXAnimation = new DoubleAnimation
            {
                Duration = _hideAnimationTime,
                From = 1,
                To = 0
            };

            storyboard.Children.Add(shrinkXAnimation);

            Storyboard.SetTargetProperty(shrinkXAnimation, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTarget(shrinkXAnimation, _displayPart);
        }

        private void SetShrinkYAnimation(Storyboard storyboard)
        {
            DoubleAnimation shrinkYAnimation = new DoubleAnimation
            {
                Duration = _hideAnimationTime,
                From = _displayPart.ActualHeight,
                To = 0
            };

            storyboard.Children.Add(shrinkYAnimation);

            Storyboard.SetTargetProperty(shrinkYAnimation, new PropertyPath("Height"));
            Storyboard.SetTarget(shrinkYAnimation, _displayPart);
        }
    }
}
