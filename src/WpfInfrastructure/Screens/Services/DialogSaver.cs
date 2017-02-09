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


using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.Services
{
    [Export(typeof(ISaveToFileSystem))]
    public class DialogSaver : ISaveToFileSystem
    {
        public async Task<bool> SaveStringToFile(string stringToSave, string proposedFileName, string proposedExtension)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog { FileName = proposedFileName,
                                                                      DefaultExt = proposedExtension };
            bool? result = saveFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                using (Stream stream = saveFileDialog.OpenFile())
                {
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(stringToSave);
                    await stream.WriteAsync(data, 0, data.Length);
                    return true;
                }
            }
            return false;
        }
    }
}
