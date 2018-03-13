using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThoughtWorks.QRCode.Codec;

namespace XJTU.Common
{
   public class QRCodeLogic
    {
        public byte[] GetRCode(string content)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder
            {
                QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE,
                QRCodeScale = 4,
                QRCodeVersion = 8,
                QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M
            };

            System.Drawing.Image image = qrCodeEncoder.Encode(content);
            string filename = DateTime.Now.ToString("yyyymmddhhmmssfff").ToString() + ".jpg";
            string filepath = filename;

            //System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
            MemoryStream fs = new MemoryStream();
            image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);

            return fs.ToArray();
        }
    }
}
