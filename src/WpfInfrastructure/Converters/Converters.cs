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

namespace VP.FF.PT.Common.WpfInfrastructure.Converters
{
    public static class Converters
    {
        public static IsRootConverter IsRootConverter = new IsRootConverter();

        public static IsLastItemInBranchConverter IsLastItemInBranchConverter = new IsLastItemInBranchConverter();

        public static ToUpperStringConverter ToUpperStringConverter = new ToUpperStringConverter();

        public static IconKeyToIconStyleMultiConverter IcoinKeyToIconStyleMultiConverter = new IconKeyToIconStyleMultiConverter();

        public static ItemsToCountConverter ItemsToCountConverter = new ItemsToCountConverter();

        public static BoolToIntConverter BoolToIntConverter = new BoolToIntConverter();

        public static AlignmentLineToPositionConverter AlignmentLineToPositionConverter = new AlignmentLineToPositionConverter();

        public static ColorToBrushConverter ColorToBrushConverter = new ColorToBrushConverter();

        public static ImageSourceToVisibilityConverter ImageSourceToVisibilityConverter = new ImageSourceToVisibilityConverter();
        
        public static EnumToQualityIssueIconConverter EnumToQualityIssueIconConverter = new EnumToQualityIssueIconConverter();

        public static BooleanToVisibilityConverter BooleanToVisibilityInvertedConverter = new BooleanToVisibilityConverter
        {
            FalseValue = Visibility.Hidden,
            TrueValue = Visibility.Hidden
        };

        public static BooleanToVisibilityConverter BooleanToVisibilityConverter = new BooleanToVisibilityConverter
        {
            FalseValue = Visibility.Collapsed,
            TrueValue = Visibility.Visible
        };

        public static BooleanNotConverter BooleanNotConverter = new BooleanNotConverter();

        public static InvertSignConverter InvertSignConverter = new InvertSignConverter();

        public static ZeroToVisibilityHiddenConverter ZeroToVisibilityHiddenConverter = new ZeroToVisibilityHiddenConverter();
    }
}
