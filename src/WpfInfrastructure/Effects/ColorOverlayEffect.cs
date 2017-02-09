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
    public class ColorOverlayEffect : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Input", typeof(ColorOverlayEffect), 0);
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(ColorOverlayEffect), new UIPropertyMetadata(Color.FromArgb(255, 255, 0, 255), PixelShaderConstantCallback(0)));
        public static readonly DependencyProperty AlphaTresholdProperty = DependencyProperty.Register("AlphaTreshold", typeof(double), typeof(ColorOverlayEffect), new UIPropertyMetadata(0.1D, PixelShaderConstantCallback(1)));

        public ColorOverlayEffect()
        {
            PixelShader pixelShader = new PixelShader
                {
                    UriSource =
                        new Uri("pack://application:,,,/VP.FF.PT.Common.WpfInfrastructure;component/Styles/Resources/Shader/ColorOverlay.ps", UriKind.Absolute)
                    
                };

            this.PixelShader = pixelShader;

            this.UpdateShaderValue(InputProperty);
            this.UpdateShaderValue(ColorProperty);
            this.UpdateShaderValue(AlphaTresholdProperty);
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

        public Color Color
        {
            get
            {
                return (Color)this.GetValue(ColorProperty);
            }

            set
            {
                this.SetValue(ColorProperty, value);
            }
        }

        public double AlphaTreshold
        {
            get
            {
                return (double)this.GetValue(AlphaTresholdProperty);
            }

            set
            {
                this.SetValue(AlphaTresholdProperty, value);
            }
        }
    }
}
