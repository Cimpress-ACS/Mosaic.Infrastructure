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


using System.Windows;
using System.Windows.Controls;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;

namespace VP.FF.PT.Common.WpfInfrastructure.Extensions
{
    public class PlatformItemTemplateSelector : DataTemplateSelector
    {
        private DataTemplate clampTemplate;

        public DataTemplate ClampTemplate
        {
            get { return clampTemplate; }
            set { clampTemplate = value; }
        }

        private DataTemplate shirtTemplate;

        public DataTemplate ShirtTemplate
        {
            get { return shirtTemplate; }
            set { shirtTemplate = value; }
        }

        private DataTemplate paperTemplate;

        public DataTemplate PaperTemplate
        {
            get { return paperTemplate; }
            set { paperTemplate = value; }
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Clamp)
            {
                Clamp clamp = (Clamp)item;

                if (clamp.IsEmpty)
                {
                    return clampTemplate;
                }

                return shirtTemplate;
            }

            if (item is Foil)
            {
                return paperTemplate;
            }

            return null;
        }
    }
}
