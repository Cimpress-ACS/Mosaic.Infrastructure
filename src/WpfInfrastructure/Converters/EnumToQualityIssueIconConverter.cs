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
using System.Windows.Data;

namespace VP.FF.PT.Common.WpfInfrastructure.Converters
{
    public enum QualityIssueLoading
    {
        DamagesShirt = 1,
        EmptyHanger = 2,
        DefectHanger = 3,
        WrongShirtType = 4,
    }

    public enum QualityIssueProcess
    {
        SomethingWrongWithShirt = 1,
        SomethingWrongWithFoil = 2,
        WrongCountryOfOrigin = 3
    }

    public enum QualityIssueControl
    {
        SomethingWrongWithShirt = 1,
        WrongPrintBending = 2,
        WrongPrintImage = 3,
        WrongPrintOther = 4,
        WrongCountryOfOrigin = 5
    }

    public class EnumToQualityIssueIconConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is QualityIssueLoading)
            {
                switch ((QualityIssueLoading)value)
                {
                    case QualityIssueLoading.DamagesShirt:
                        return "o";
                    case QualityIssueLoading.DefectHanger:
                        return "n";
                    case QualityIssueLoading.EmptyHanger:
                        return "q";
                    case QualityIssueLoading.WrongShirtType:
                        return "t";
                    default:
                        return string.Empty;
                }
            }

            if (value is QualityIssueProcess)
            {
                switch ((QualityIssueProcess)value)
                {
                    case QualityIssueProcess.SomethingWrongWithFoil:
                        return "e";
                    case QualityIssueProcess.SomethingWrongWithShirt:
                        return "p";
                    case QualityIssueProcess.WrongCountryOfOrigin:
                        return "c";
                    default:
                        return string.Empty;
                }
            }

            if (value is QualityIssueControl)
            {
                switch ((QualityIssueControl)value)
                {
                    case QualityIssueControl.SomethingWrongWithShirt:
                        return "o";
                    case QualityIssueControl.WrongCountryOfOrigin:
                        return "c";
                    case QualityIssueControl.WrongPrintBending:
                        return "f";
                    case QualityIssueControl.WrongPrintImage:
                        return "k";
                    case QualityIssueControl.WrongPrintOther:
                        return "l";
                    default:
                        return string.Empty;
                }
            }

            return string.Empty;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
