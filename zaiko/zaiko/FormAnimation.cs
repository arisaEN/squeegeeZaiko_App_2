using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows;

namespace zaiko.anime
{
    internal class FormAnimation
    {
        // フェードインアニメーションを適用
        public void ApplyFadeInAnimation(UIElement element)
        {
            DoubleAnimation fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.5)
            };
            element.BeginAnimation(UIElement.OpacityProperty, fadeIn);
        }

        public void ApplyFadeOutAndClose(Window window)
        {
            // フェードアウトアニメーションを作成
            DoubleAnimation fadeOut = new DoubleAnimation
            {
                From = window.Opacity,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.5) // フェードアウトの所要時間
            };

            // アニメーションの完了時にウィンドウを閉じる
            fadeOut.Completed += (s, e) => window.Close();

            // フェードアウトを適用
            window.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }


        //---------------------------------------------------------------------------------------------------------------------
    }
}
