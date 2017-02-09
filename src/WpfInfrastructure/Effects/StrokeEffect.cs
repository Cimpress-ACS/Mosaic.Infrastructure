/* Copyright 2017 Cimpress

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License. */


using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace VP.FF.PT.Common.WpfInfrastructure.Effects
{
    public enum StrokeMode
    {
        NONE = 0,
        OUTER = 1,
        INNER = 2,
        CENTER = 3
    }

    public class Stroke2_0Effect : ShaderEffect
    {
        private const string DIRECTORY_PATH = "pack://application:,,,/VP.FF.PT.Common.WpfInfrastructure;component/Styles/Resources/Shader/";
        private const string SHADER_NAME_INNER = "Stroke2_0Effect_Inner.ps";
        private const string SHADER_NAME_OUTER = "Stroke2_0Effect_Outer.ps";
        private const string SHADER_NAME_CENTER = "Stroke2_0Effect_Center.ps";
        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(Stroke2_0Effect), 0);
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(Stroke2_0Effect), new UIPropertyMetadata(((double)(53D)), PixelShaderConstantCallback(0)));
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(double), typeof(Stroke2_0Effect), new UIPropertyMetadata(((double)(53D)), PixelShaderConstantCallback(1)));
        public static readonly DependencyProperty StrokeModeProperty = DependencyProperty.Register("StrokeMode", typeof(StrokeMode), typeof(Stroke2_0Effect), new UIPropertyMetadata(StrokeMode.NONE, OnStrokeModeChanged));
        public static readonly DependencyProperty StrokeColorProperty = DependencyProperty.Register("StrokeColor", typeof(Color), typeof(Stroke2_0Effect), new UIPropertyMetadata(Color.FromArgb(255, 86, 44, 46), PixelShaderConstantCallback(2)));
        public static readonly DependencyProperty OriginalOpacityProperty = DependencyProperty.Register("OriginalOpacity", typeof(double), typeof(Stroke2_0Effect), new UIPropertyMetadata(((double)(1D)), PixelShaderConstantCallback(3)));

        public Stroke2_0Effect()
        {
            var pixelShader = new PixelShader();
            this.PixelShader = pixelShader;

            this.UpdateShaderValue(InputProperty);
            this.UpdateShaderValue(WidthProperty);
            this.UpdateShaderValue(HeightProperty);
            this.UpdateShaderValue(StrokeColorProperty);
            this.UpdateShaderValue(OriginalOpacityProperty);
        }

        public Brush Input
        {
            get
            {
                return (Brush)this.GetValue(InputProperty);
            }

            set
            {
                this.SetValue(InputProperty, value);
            }
        }

        public double Width
        {
            get
            {
                return (double)this.GetValue(WidthProperty);
            }

            set
            {
                this.SetValue(WidthProperty, value);
            }
        }

        public double Height
        {
            get
            {
                return (double)this.GetValue(HeightProperty);
            }

            set
            {
                this.SetValue(HeightProperty, value);
            }
        }

        public StrokeMode StrokeMode
        {
            get
            {
                return (StrokeMode)GetValue(StrokeModeProperty);
            }

            set
            {
                this.SetValue(StrokeModeProperty, value);
            }
        }

        public Color StrokeColor
        {
            get
            {
                return (Color)this.GetValue(StrokeColorProperty);
            }

            set
            {
                this.SetValue(StrokeColorProperty, value);
            }
        }

        public double OriginalOpacity
        {
            get
            {
                return (double)this.GetValue(OriginalOpacityProperty);
            }

            set
            {
                this.SetValue(OriginalOpacityProperty, value);
            }
        }

        private static void OnStrokeModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (Stroke2_0Effect)d;
            var path = DIRECTORY_PATH;

            if (control.StrokeMode == StrokeMode.CENTER)
            {
                path += SHADER_NAME_CENTER;
            }

            if (control.StrokeMode == StrokeMode.INNER)
            {
                path += SHADER_NAME_INNER;
            }

            if (control.StrokeMode == StrokeMode.OUTER)
            {
                path += SHADER_NAME_OUTER;
            }
            
            control.PixelShader.UriSource = new Uri(path, UriKind.Absolute);
        }
    }
}
