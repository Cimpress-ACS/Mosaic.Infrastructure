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
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace VP.FF.PT.Common.WpfInfrastructure.Extensions
{
    using System.IO;

    /// <summary>
    /// Helperclass to load images.
    /// </summary>
    public static class LoadImageHelper
    {
        /// <summary>
        /// Basepath.
        /// </summary>
        public static readonly string BasePath = Directory.GetCurrentDirectory() + @"\Images\";

        /// <summary>
        /// Fallback icon name.
        /// </summary>
        public const string FallbackIconName =
            "missing_image";

        /// <summary>
        /// Fallback icon path.
        /// </summary>
        public const string FallbackIconPath = "pack://application:,,,/VP.FF.PT.Common.WpfInfrastructure;Component/Styles/Resources/Images/";

        private static Dictionary<string, BitmapImage> cachedIcons = new Dictionary<string, BitmapImage>();

        /// <summary>
        /// Load new image if necessary, otherwise return old image.
        /// </summary>
        /// <param name="iconKey"></param>
        /// <returns></returns>
        public static BitmapImage LoadImage(string iconKey)
        {
            return LoadImageIfNecessary(null, iconKey);
        }

        /// <summary>
        /// Load new image if necessary, otherwise return old image.
        /// </summary>
        /// <param name="uriString"></param>
        /// <param name="iconKey"></param>
        /// <returns></returns>
        public static BitmapImage LoadImage(string uriString, string iconKey)
        {
            return LoadImageIfNecessary(null, uriString, iconKey);
        }

        /// <summary>
        /// Load new image if necessary, otherwise return old image.
        /// </summary>
        /// <param name="oldBitmapImage"></param>
        /// <param name="iconKey"></param>
        /// <returns></returns>
        public static BitmapImage LoadImageIfNecessary(BitmapImage oldBitmapImage, string iconKey)
        {
            return LoadImageIfNecessary(oldBitmapImage, BasePath, iconKey);
        }

        /// <summary>
        /// Load new image if necessary, otherwise return old image.
        /// </summary>
        /// <param name="oldBitmapImage"></param>
        /// <param name="uriString"></param>
        /// <param name="iconKey"></param>
        /// <returns></returns>
        public static BitmapImage LoadImageIfNecessary(BitmapImage oldBitmapImage, string uriString, string iconKey)
        {
            if (iconKey == null)
            {
                return oldBitmapImage;
            }

            var iconUri = GetIconPathOrFallback(uriString, iconKey);

            if (iconUri != null && (oldBitmapImage == null || iconUri != oldBitmapImage.UriSource.OriginalString))
            {
                if (cachedIcons.ContainsKey(iconUri))
                {
                    return cachedIcons[iconUri];
                }
                BitmapImage image = GetBitmapImageFromUri(iconUri, UriKind.Absolute);
                cachedIcons.Add(iconUri, image);

                return image;
            }

            return oldBitmapImage;
        }

        /// <summary>
        /// Gets the path to the icon or a fallback path.
        /// </summary>
        /// <param name="uriString">
        /// The uri string.
        /// </param>
        /// <param name="iconKey">
        /// The icon key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetIconPathOrFallback(string uriString, string iconKey)
        {
            return GetIconPathOrFallback(uriString, iconKey, FallbackIconName);
        }

        /// <summary>
        /// Gets the path to the icon or a fallback path.
        /// </summary>
        /// <param name="uriString">
        /// The uri string.
        /// </param>
        /// <param name="iconKey">
        /// The icon key.
        /// </param>
        /// <param name="fallbackName">
        /// The fallback name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetIconPathOrFallback(string uriString, string iconKey, string fallbackName)
        {
            string iconUri = GetIconFileName(uriString, iconKey);
            if (!File.Exists(new Uri(iconUri, UriKind.Relative).ToString()))
            {
                iconUri = GetIconFileName(FallbackIconPath, fallbackName);
            }

            return iconUri;
        }

        private static BitmapImage GetBitmapImageFromUri(string iconUri, UriKind uriKind)
        {
            if (iconUri == null)
            {
                return null;
            }

            BitmapImage icon;
            try
            {
                icon = new BitmapImage();
                icon.BeginInit();
                icon.UriSource = new Uri(iconUri, uriKind);
                icon.EndInit();
            }
            catch (Exception)
            {
                return null;
            }

            return icon;
        }

        private static string GetIconFileName(string uriString, string iconKey)
        {
            return Path.Combine(uriString, string.Concat(iconKey, ".png"));
        }

    }

}
